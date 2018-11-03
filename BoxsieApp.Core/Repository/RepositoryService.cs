using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Storage;
using NLog;

namespace BoxsieApp.Core.Repository
{
    public class RepositoryService
    {
        private readonly ILogger _logger;
        private readonly GeneralConfig _generalConfig;

        private List<string> _availableTables;

        public RepositoryService(ILogger logger, GeneralConfig generalConfig)
        {
            _logger = logger;
            _generalConfig = generalConfig;
            _availableTables = new List<string>();

            EnsureDbCreated();
        }

        private void EnsureDbCreated()
        {
            _logger.Log(LogLevel.Info, $"Looking for database...");

            var dbPath = StorageUtils.PathCombine(_generalConfig.UserConfig.UserDataPath, _generalConfig.DbFilename);

            _logger.Log(LogLevel.Info, $"Looking for database at {dbPath}");

            if (File.Exists(dbPath))
                return;

            _logger.Log(LogLevel.Info, $"Database not found, creating...");

            SQLiteConnection.CreateFile(dbPath);

            _logger.Log(LogLevel.Info, $"Database created");
        }

        private string GetConnectionString()
        {
            return $"Data Source={StorageUtils.PathCombine(_generalConfig.UserConfig.UserDataPath, _generalConfig.DbFilename)};";
        }
    }
}