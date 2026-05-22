namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Пользователь системы (Клиент или Риэлтор).
    /// </summary>
    public class User
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
        /// Адрес электронной почты (используется как логин).
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Хеш пароля.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя (Client или Realtor).
        /// </summary>
        public string Role { get; set; } = "Client";

        /// <summary>
        /// Предпочтения по недвижимости (только для клиентов).
        /// </summary>
        public string Preferences { get; set; } = string.Empty;

        /// <summary>
        /// Комиссионная ставка (только для риэлторов).
        /// </summary>
        public decimal CommissionRate { get; set; }
    }
}
