using System;
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
        /// <param name="deal">Объект сделки с указанием ID объекта, клиента, риэлтора и финальной цены.</param>
        /// <returns>Созданная запись о сделке.</returns>
        /// <response code="200">Сделка успешно оформлена, статус объекта изменен на Sold.</response>
        /// <response code="400">Ошибка валидации или неверное состояние объекта (например, уже продан).</response>
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
    }
}
