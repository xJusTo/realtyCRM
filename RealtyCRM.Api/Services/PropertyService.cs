using System.Collections.Generic;
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
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync()
        {
            return await _propertyRepository.GetFilteredAsync(status: PropertyStatus.Available);
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

        public Task<IEnumerable<Property>> GetFilteredPropertiesAsync(
            PropertyType? type = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            double? minArea = null,
            double? maxArea = null,
            PropertyStatus? status = null,
            int? realtorId = null)
        {
            return _propertyRepository.GetFilteredAsync(type, minPrice, maxPrice, minArea, maxArea, status, realtorId);
        }
    }
}
