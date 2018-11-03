using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxsieApp.Core.Repository
{
    public interface IRepository<in T>
    {
        Task CreateTable(string connectionString, string tableName);

        Task<int> CreateAsync(T entity);
        Task<IEnumerable<int>> CreateAsync(IEnumerable<T> entities);
    }
}