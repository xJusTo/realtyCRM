using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Controllers
{
    /// <summary>
    /// Контроллер для управления сделками.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly IDealService _dealService;

        public DealsController(IDealService dealService)
        {
            _dealService = dealService;
        }

        /// <summary>
        /// Оформление новой сделки купли-продажи.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Deal), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Deal>> Create([FromBody] Deal deal)
        {
            try
            {
                var result = await _dealService.CreateDealAsync(deal);
                return Ok(result);
            }
            catch (InvalidDealStateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получает список сделок риэлтора.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Deal>>> Get([FromQuery] int? realtorId = null)
        {
            try
            {
                if (realtorId.HasValue)
                {
                    var deals = await _dealService.GetDealsByRealtorAsync(realtorId.Value);
                    return Ok(deals);
                }
                
                return BadRequest("Необходимо указать realtorId для получения списка сделок.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла внутренняя ошибка при получении списка сделок.");
            }
        }
    }
}
