using System.Collections.Generic;
using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для объектов недвижимости.
    /// </summary>
    public interface IPropertyRepository : IRepository<Property>
    {
        /// <summary>
        /// Получает список недвижимости по фильтрам.
        /// </summary>
        Task<IEnumerable<Property>> GetFilteredAsync(
            PropertyType? type = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            double? minArea = null,
            double? maxArea = null,
            PropertyStatus? status = null,
            int? realtorId = null);
    }
}
