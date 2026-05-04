using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtyCRM.Api.Interfaces
{
    /// <summary>
    /// Обобщенный интерфейс репозитория для работы с данными.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получает все сущности.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получает сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Добавляет новую сущность.
        /// </summary>
        /// <param name="entity">Сущность для добавления.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Обновляет существующую сущность.
        /// </summary>
        /// <param name="entity">Сущность с обновленными данными.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Удаляет сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор для удаления.</param>
        Task DeleteAsync(int id);
    }
}
