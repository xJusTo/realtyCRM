using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Controllers
{
    /// <summary>
    /// Контроллер для управления объектами недвижимости.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// Получает все объекты недвижимости с возможностью фильтрации.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetAll(
            [FromQuery] PropertyType? type = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] double? minArea = null,
            [FromQuery] double? maxArea = null,
            [FromQuery] PropertyStatus? status = null,
            [FromQuery] int? realtorId = null)
        {
            var properties = await _propertyService.GetFilteredPropertiesAsync(
                type, minPrice, maxPrice, minArea, maxArea, status, realtorId);
            return Ok(properties);
        }

        /// <summary>
        /// Получает только доступные объекты недвижимости.
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Property>>> GetAvailable()
        {
            var properties = await _propertyService.GetAvailablePropertiesAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Добавляет новый объект недвижимости (для риэлторов).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Property>> Create([FromBody] Property property)
        {
            await _propertyService.AddPropertyAsync(property);
            return CreatedAtAction(nameof(GetAvailable), new { id = property.Id }, property);
        }

        /// <summary>
        /// Изменяет статус объекта недвижимости.
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> ChangeStatus(int id, [FromBody] PropertyStatus status)
        {
            await _propertyService.ChangeStatusAsync(id, status);
            return NoContent();
        }

        /// <summary>
        /// Обновляет параметры объекта недвижимости.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Property property)
        {
            if (id != property.Id)
            {
                return BadRequest("Идентификаторы объекта не совпадают.");
            }

            var existing = await _propertyService.GetPropertyByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Объект недвижимости не найден.");
            }

            await _propertyService.UpdatePropertyAsync(property);
            return NoContent();
        }

        /// <summary>
        /// Удаляет объект недвижимости.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _propertyService.GetPropertyByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Объект недвижимости не найден.");
            }

            await _propertyService.DeletePropertyAsync(id);
            return NoContent();
        }
    }
}
