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
        private readonly IRepository<Property> _propertyRepository;

        public PropertiesController(IPropertyService propertyService, IRepository<Property> propertyRepository)
        {
            _propertyService = propertyService;
            _propertyRepository = propertyRepository;
        }

        /// <summary>
        /// Получает все объекты недвижимости.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetAll()
        {
            var properties = await _propertyRepository.GetAllAsync();
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
        /// Изменяет статус объекта недвижимости.
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> ChangeStatus(int id, [FromBody] PropertyStatus status)
        {
            await _propertyService.ChangeStatusAsync(id, status);
            return NoContent();
        }
    }
}
