using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Services
{
    /// <summary>
    /// Реализация сервиса для работы с недвижимостью.
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly IRepository<Property> _propertyRepository;

        public PropertyService(IRepository<Property> propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync()
        {
            var all = await _propertyRepository.GetAllAsync();
            return all.Where(p => p.Status == PropertyStatus.Available);
        }

        public Task<Property?> GetPropertyByIdAsync(int id)
        {
            return _propertyRepository.GetByIdAsync(id);
        }

        public Task AddPropertyAsync(Property property)
        {
            return _propertyRepository.AddAsync(property);
        }

        public async Task ChangeStatusAsync(int id, PropertyStatus status)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property != null)
            {
                property.Status = status;
                await _propertyRepository.UpdateAsync(property);
            }
        }
    }
}
