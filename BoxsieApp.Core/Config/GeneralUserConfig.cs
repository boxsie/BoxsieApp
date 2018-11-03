using System.IO;
using BoxsieApp.Core.Config.Contracts;
using BoxsieApp.Core.Storage;

namespace BoxsieApp.Core.Config
{
    public partial class GeneralUserConfig : IUserConfig
    {
        public string UserDataPath { get; set; }
        public string LogOutputPath { get; set; }

        public void SetDefault()
        {
            UserDataPath = GetDefaultUserDataPath();
            LogOutputPath = Path.Combine(UserDataPath, "Log");
        }

        private static string GetDefaultUserDataPath()
        {
            return StorageUtils.GetDefaultUserDataPath(Cfg.GetConfig<GeneralConfig>().AppName);
        }
    }
}