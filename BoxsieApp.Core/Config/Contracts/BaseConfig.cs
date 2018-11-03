using System.IO;
using System.Threading.Tasks;
using BoxsieApp.Core.Storage;
using BoxsieApp.Core.Storage.Stores;
using Newtonsoft.Json;

namespace BoxsieApp.Core.Config.Contracts
{
    public abstract class BaseConfig<T> : IConfig where T : IUserConfig, new()
    {
        public string UserConfigFilename { get; set; }

        [JsonIgnore]
        public T UserConfig { get; private set; }

        public IUserConfig GetUserConfig()
        {
            return UserConfig;
        }

        public async Task LoadUserConfigAsync(string appDataPath)
        {
            var filePath = Path.Combine(appDataPath, UserConfigFilename);

            using (var js = new JsonStore<T>())
            {
                if (File.Exists(filePath))
                    UserConfig = await js.ReadAsync(filePath);
                else
                {
                    UserConfig = new T();
                    UserConfig.SetDefault();

                    await js.WriteAsync(filePath, UserConfig);
                }
            }
        }

        public async Task LoadEncryptedUserConfigAsync(string appDataPath, string key)
        {
            var filePath = Path.Combine(appDataPath, UserConfigFilename);

            using (var js = new EncryptedJsonStore<T>(key))
            {
                if (File.Exists(filePath))
                    UserConfig = await js.ReadAsync(filePath);
                else
                {
                    UserConfig = new T();
                    UserConfig.SetDefault();

                    await js.WriteAsync(filePath, UserConfig);
                }
            }
        }
    }
}