namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Объект недвижимости.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Уникальный идентификатор объекта.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Адрес объекта.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Описание объекта.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Цена объекта.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Площадь объекта.
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Тип недвижимости.
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        /// Текущий статус объекта.
        /// </summary>
        public PropertyStatus Status { get; set; }

        /// <summary>
        /// Идентификатор ответственного риэлтора.
        /// </summary>
        public int RealtorId { get; set; }

        /// <summary>
        /// Ссылка на фотографию объекта.
        /// </summary>
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
