using System.Threading.Tasks;

namespace BoxsieApp.Core.Config.Contracts
{
    public interface IConfig
    {
        IUserConfig GetUserConfig();
        Task LoadUserConfigAsync(string appDataPath);
        Task LoadEncryptedUserConfigAsync(string appDataPath, string key);
    }
}