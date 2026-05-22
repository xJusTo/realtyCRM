using System.Threading.Tasks;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для пользователей, расширяющий общий репозиторий.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Получает пользователя по адресу электронной почты.
        /// </summary>
        Task<User?> GetByEmailAsync(string email);
    }
}
