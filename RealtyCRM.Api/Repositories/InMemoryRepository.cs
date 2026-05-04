using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealtyCRM.Api.Interfaces;

namespace RealtyCRM.Api.Repositories
{
    /// <summary>
    /// Репозиторий в памяти для хранения сущностей.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _entities = new();
        private readonly object _lock = new();

        public Task<IEnumerable<T>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_entities.AsEnumerable().ToList() as IEnumerable<T>);
            }
        }

        public Task<T?> GetByIdAsync(int id)
        {
            lock (_lock)
            {
                var entity = _entities.FirstOrDefault(e => GetId(e) == id);
                return Task.FromResult(entity);
            }
        }

        public Task AddAsync(T entity)
        {
            lock (_lock)
            {
                _entities.Add(entity);
                return Task.CompletedTask;
            }
        }

        public Task UpdateAsync(T entity)
        {
            lock (_lock)
            {
                var id = GetId(entity);
                var existing = _entities.FirstOrDefault(e => GetId(e) == id);
                if (existing != null)
                {
                    _entities.Remove(existing);
                    _entities.Add(entity);
                }
                return Task.CompletedTask;
            }
        }

        public Task DeleteAsync(int id)
        {
            lock (_lock)
            {
                var existing = _entities.FirstOrDefault(e => GetId(e) == id);
                if (existing != null)
                {
                    _entities.Remove(existing);
                }
                return Task.CompletedTask;
            }
        }

        private int GetId(T entity)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property.");
            return (int)prop.GetValue(entity)!;
        }
    }
}
