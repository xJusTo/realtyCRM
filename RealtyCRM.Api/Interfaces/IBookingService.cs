using System.Collections.Generic;
using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с заявками на просмотр недвижимости (бронированиями).
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Создает новую заявку и переводит статус недвижимости в Booked.
        /// </summary>
        Task<Booking> CreateBookingAsync(Booking booking);

        /// <summary>
        /// Возвращает список заявок для конкретного риэлтора.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByRealtorAsync(int realtorId);

        /// <summary>
        /// Возвращает список заявок конкретного клиента.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByClientAsync(string clientEmail);

        /// <summary>
        /// Обновляет статус заявки (Pending, Approved, Rejected).
        /// При отклонении (Rejected) статус недвижимости возвращается в Available.
        /// При подтверждении (Approved) статус недвижимости переходит в Sold (через создание сделки).
        /// </summary>
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
    }
}
