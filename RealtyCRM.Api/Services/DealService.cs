using System;
using System.Threading.Tasks;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Services
{
    /// <summary>
    /// Реализация сервиса для управления сделками.
    /// </summary>
    public class DealService : IDealService
    {
        private readonly IRepository<Deal> _dealRepository;
        private readonly IRepository<Property> _propertyRepository;

        public DealService(IRepository<Deal> dealRepository, IRepository<Property> propertyRepository)
        {
            _dealRepository = dealRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<Deal> CreateDealAsync(Deal deal)
        {
            var property = await _propertyRepository.GetByIdAsync(deal.PropertyId);
            
            if (property == null || property.Status != PropertyStatus.Available)
            {
                throw new InvalidDealStateException("Недвижимость недоступна для сделки");
            }

            // Добавляем сделку
            await _dealRepository.AddAsync(deal);

            // Обновляем статус недвижимости
            property.Status = PropertyStatus.Sold;
            await _propertyRepository.UpdateAsync(property);

            return deal;
        }
    }
}
