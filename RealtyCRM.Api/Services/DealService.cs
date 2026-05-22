using System;
using System.Collections.Generic;
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
        private readonly IDealRepository _dealRepository;
        private readonly IPropertyRepository _propertyRepository;

        public DealService(IDealRepository dealRepository, IPropertyRepository propertyRepository)
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
            deal.DealDate = DateTime.UtcNow;
            await _dealRepository.AddAsync(deal);

            // Обновляем статус недвижимости
            property.Status = PropertyStatus.Sold;
            await _propertyRepository.UpdateAsync(property);

            return deal;
        }

        public Task<IEnumerable<Deal>> GetDealsByRealtorAsync(int realtorId)
        {
            return _dealRepository.GetByRealtorIdAsync(realtorId);
        }
    }
}
