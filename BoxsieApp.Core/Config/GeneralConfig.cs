using BoxsieApp.Core.Config.Contracts;

namespace BoxsieApp.Core.Config
{
    public partial class GeneralConfig : BaseConfig<GeneralUserConfig>
    {
        public string AppName { get; set; }
        public string DbFilename { get; set; }
        public string UserDataDirName { get; set; }
        public string EncryptKeyBase { get; set; }
    }
}