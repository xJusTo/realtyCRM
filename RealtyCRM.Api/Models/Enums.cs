namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Тип недвижимости.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// Квартира.
        /// </summary>
        Apartment,
        /// <summary>
        /// Дом.
        /// </summary>
        House,
        /// <summary>
        /// Коммерческая недвижимость.
        /// </summary>
        Commercial
    }

    /// <summary>
    /// Статус объекта недвижимости.
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>
        /// Доступно для продажи/аренды.
        /// </summary>
        Available,
        /// <summary>
        /// Забронировано.
        /// </summary>
        Booked,
        /// <summary>
        /// Продано.
        /// </summary>
        Sold
    }
}
