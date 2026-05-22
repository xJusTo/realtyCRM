using System.Collections.Generic;
using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для заявок на бронирование просмотра.
    /// </summary>
    public interface IBookingRepository : IRepository<Booking>
    {
        /// <summary>
        /// Получает список заявок на объекты недвижимости конкретного риэлтора.
        /// </summary>
        Task<IEnumerable<Booking>> GetByRealtorIdAsync(int realtorId);

        /// <summary>
        /// Получает список заявок конкретного клиента по его электронной почте.
        /// </summary>
        Task<IEnumerable<Booking>> GetByClientEmailAsync(string clientEmail);
    }
}
