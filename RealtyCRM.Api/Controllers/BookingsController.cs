using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с заявками на просмотр недвижимости.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Создает новую заявку на бронирование просмотра.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Booking>> Create([FromBody] Booking booking)
        {
            try
            {
                var result = await _bookingService.CreateBookingAsync(booking);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла внутренняя ошибка при создании бронирования.");
            }
        }

        /// <summary>
        /// Получает список заявок с фильтрацией по риэлтору или по email клиента.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> Get(
            [FromQuery] int? realtorId = null,
            [FromQuery] string? clientEmail = null)
        {
            try
            {
                if (realtorId.HasValue)
                {
                    var bookings = await _bookingService.GetBookingsByRealtorAsync(realtorId.Value);
                    return Ok(bookings);
                }
                else if (!string.IsNullOrEmpty(clientEmail))
                {
                    var bookings = await _bookingService.GetBookingsByClientAsync(clientEmail);
                    return Ok(bookings);
                }

                return BadRequest("Необходимо указать либо realtorId, либо clientEmail.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла внутренняя ошибка при получении списка заявок.");
            }
        }

        /// <summary>
        /// Обновляет статус заявки (Approved, Rejected).
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            try
            {
                var success = await _bookingService.UpdateBookingStatusAsync(id, status);
                if (!success)
                {
                    return NotFound($"Заявка с ID {id} или связанный объект недвижимости не найдены.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла внутренняя ошибка при обновлении статуса заявки: {ex.Message}");
            }
        }
    }
}
