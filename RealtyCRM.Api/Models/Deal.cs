using System;

namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Сделка с недвижимостью.
    /// </summary>
    public class Deal
    {
        /// <summary>
        /// Уникальный идентификатор сделки.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор проданного объекта.
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// Идентификатор клиента.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Идентификатор риэлтора, проводившего сделку.
        /// </summary>
        public int RealtorId { get; set; }

        /// <summary>
        /// Дата заключения сделки.
        /// </summary>
        public DateTime DealDate { get; set; }

        /// <summary>
        /// Итоговая цена сделки.
        /// </summary>
        public decimal FinalPrice { get; set; }
    }
}
