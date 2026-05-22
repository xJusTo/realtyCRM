using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Services
{
    /// <summary>
    /// Сервис для работы с заявками на бронирование просмотра недвижимости.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IUserRepository _userRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IPropertyRepository propertyRepository,
            IDealRepository dealRepository,
            IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _propertyRepository = propertyRepository;
            _dealRepository = dealRepository;
            _userRepository = userRepository;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            var property = await _propertyRepository.GetByIdAsync(booking.PropertyId);
            if (property == null)
            {
                throw new InvalidOperationException("Объект недвижимости не найден.");
            }

            if (property.Status != PropertyStatus.Available)
            {
                throw new InvalidOperationException("Недвижимость недоступна для бронирования.");
            }

            // Переводим статус недвижимости в ""Забронировано""
            property.Status = PropertyStatus.Booked;
            await _propertyRepository.UpdateAsync(property);

            // Сохраняем саму заявку
            booking.BookingDate = DateTime.UtcNow;
            booking.Status = "Pending";
            await _bookingRepository.AddAsync(booking);

            return booking;
        }

        public Task<IEnumerable<Booking>> GetBookingsByRealtorAsync(int realtorId)
        {
            return _bookingRepository.GetByRealtorIdAsync(realtorId);
        }

        public Task<IEnumerable<Booking>> GetBookingsByClientAsync(string clientEmail)
        {
            return _bookingRepository.GetByClientEmailAsync(clientEmail);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            var property = await _propertyRepository.GetByIdAsync(booking.PropertyId);
            if (property == null) return false;

            if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                // Если отказ, возвращаем статус недвижимости в ""Доступно""
                property.Status = PropertyStatus.Available;
                await _propertyRepository.UpdateAsync(property);

                booking.Status = "Rejected";
                await _bookingRepository.UpdateAsync(booking);
                return true;
            }
            else if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                // Если покупка подтверждена, регистрируем сделку
                booking.Status = "Approved";
                await _bookingRepository.UpdateAsync(booking);

                // Ищем клиента в системе по email, чтобы связать сделку с его профилем
                var clientUser = await _userRepository.GetByEmailAsync(booking.ClientEmail);
                int clientId = clientUser?.Id ?? 0;

                var deal = new Deal
                {
                    PropertyId = booking.PropertyId,
                    ClientId = clientId,
                    RealtorId = property.RealtorId,
                    DealDate = DateTime.UtcNow,
                    FinalPrice = property.Price
                };

                // Добавляем сделку
                await _dealRepository.AddAsync(deal);

                // Меняем статус объекта на ""Продано""
                property.Status = PropertyStatus.Sold;
                await _propertyRepository.UpdateAsync(property);
                return true;
            }

            return false;
        }
    }
}
