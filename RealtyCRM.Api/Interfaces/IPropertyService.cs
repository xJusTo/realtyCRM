using System.Collections.Generic;
using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с объектами недвижимости.
    /// </summary>
    public interface IPropertyService
    {
        /// <summary>
        /// Получает список всех доступных объектов недвижимости.
        /// </summary>
        Task<IEnumerable<Property>> GetAvailablePropertiesAsync();

        /// <summary>
        /// Получает информацию об объекте недвижимости по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор объекта.</param>
        Task<Property?> GetPropertyByIdAsync(int id);

        /// <summary>
        /// Добавляет новый объект недвижимости в систему.
        /// </summary>
        /// <param name="property">Объект недвижимости.</param>
        Task AddPropertyAsync(Property property);

        /// <summary>
        /// Изменяет статус объекта недвижимости.
        /// </summary>
        /// <param name="id">Идентификатор объекта.</param>
        /// <param name="status">Новый статус.</param>
        Task ChangeStatusAsync(int id, PropertyStatus status);

        /// <summary>
        /// Получает отфильтрованный список объектов недвижимости.
        /// </summary>
        Task<IEnumerable<Property>> GetFilteredPropertiesAsync(
            PropertyType? type = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            double? minArea = null,
            double? maxArea = null,
            PropertyStatus? status = null,
            int? realtorId = null);
    }
}
