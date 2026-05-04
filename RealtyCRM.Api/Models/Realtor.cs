namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Риэлтор, работающий в агентстве.
    /// </summary>
    public class Realtor : User
    {
        /// <summary>
        /// Комиссионная ставка риэлтора.
        /// </summary>
        public decimal CommissionRate { get; set; }
    }
}
