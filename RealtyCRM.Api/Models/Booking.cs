using System;

namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Заявка на просмотр недвижимости (бронирование).
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Уникальный идентификатор заявки.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор объекта недвижимости.
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// Имя клиента, оставившего заявку.
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// Телефон клиента для связи.
        /// </summary>
        public string ClientPhone { get; set; } = string.Empty;

        /// <summary>
        /// Электронная почта клиента для связи.
        /// </summary>
        public string ClientEmail { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания заявки.
        /// </summary>
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Статус заявки (Pending, Approved, Rejected).
        /// </summary>
        public string Status { get; set; } = "Pending";
    }
}
