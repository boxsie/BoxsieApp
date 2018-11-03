using BoxsieApp.Core.Config.Contracts;
using BoxsieApp.Core.Storage;

namespace BoxsieApp.Core.Config
{
    public partial class GeneralUserConfig : IUserConfig
    {
        public string UserDataPath { get; set; }

        public void SetDefault()
        {
            UserDataPath = GetDefaultUserDataPath();
        }

        private static string GetDefaultUserDataPath()
        {
            return StorageUtils.GetDefaultUserDataPath(Cfg.GetConfig<GeneralConfig>().AppName);
        }
    }
}