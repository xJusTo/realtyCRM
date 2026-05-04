namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Клиент агентства (покупатель или арендатор).
    /// </summary>
    public class Client : User
    {
        /// <summary>
        /// Предпочтения клиента по недвижимости.
        /// </summary>
        public string Preferences { get; set; } = string.Empty;
    }
}
