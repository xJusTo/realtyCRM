using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для управления сделками.
    /// </summary>
    public interface IDealService
    {
        /// <summary>
        /// Создает новую сделку.
        /// </summary>
        /// <param name="deal">Данные сделки.</param>
        /// <returns>Созданная сделка.</returns>
        Task<Deal> CreateDealAsync(Deal deal);

        /// <summary>
        /// Получает список сделок для конкретного риэлтора.
        /// </summary>
        Task<System.Collections.Generic.IEnumerable<Deal>> GetDealsByRealtorAsync(int realtorId);
    }
}
