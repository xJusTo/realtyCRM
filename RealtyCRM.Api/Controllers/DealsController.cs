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
        /// Создает новую сделку.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Deal>> Create(Deal deal)
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
