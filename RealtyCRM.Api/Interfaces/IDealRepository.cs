using System.Collections.Generic;
using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для сделок купли-продажи.
    /// </summary>
    public interface IDealRepository : IRepository<Deal>
    {
        /// <summary>
        /// Получает список всех совершенных сделок для конкретного риэлтора.
        /// </summary>
        Task<IEnumerable<Deal>> GetByRealtorIdAsync(int realtorId);
    }
}
