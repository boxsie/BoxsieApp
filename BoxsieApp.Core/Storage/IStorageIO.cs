using System.Threading.Tasks;

namespace BoxsieApp.Core.Storage
{
    public interface IStorageIO<T>
    {
        Task WriteAsync(string filePath, T content);
        Task<T> ReadAsync(string filePath);
    }
}