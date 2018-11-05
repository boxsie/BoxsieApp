using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoxsieApp.Core.Repository
{
    public interface IRepository<T>
    {
        Task CreateTable(string tableName);

        Task<int> CreateAsync(T entity);
        Task<IEnumerable<int>> CreateAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetWhereAsync(List<WhereClause> where);
    }
}