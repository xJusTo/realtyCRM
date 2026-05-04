namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Базовый класс пользователя системы.
    /// </summary>
    public abstract class User
    {
        /// <summary>
        /// Уникальный идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное имя.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Адрес электронной почты.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
