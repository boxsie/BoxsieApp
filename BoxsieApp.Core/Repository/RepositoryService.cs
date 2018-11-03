﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Logging;
using BoxsieApp.Core.Storage;

namespace BoxsieApp.Core.Repository
{
    public class RepositoryService
    {
        private readonly ILog _logger;
        private readonly GeneralConfig _generalConfig;

        private List<string> _availableTables;

        public RepositoryService(ILog logger, GeneralConfig generalConfig)
        {
            _logger = logger;
            _generalConfig = generalConfig;
            _availableTables = new List<string>();

            EnsureDbCreated();
        }

        private void EnsureDbCreated()
        {
            _logger.WriteLine($"Looking for database...", LogLvl.Info);

            var dbPath = Path.Combine(_generalConfig.UserConfig.UserDataPath, _generalConfig.DbFilename);

            _logger.WriteLine($"Looking for database at {dbPath}", LogLvl.Info);

            if (File.Exists(dbPath))
                return;

            _logger.WriteLine($"Database not found, creating...", LogLvl.Info);

            SQLiteConnection.CreateFile(dbPath);

            _logger.WriteLine($"Database created", LogLvl.Info);
        }

        private string GetConnectionString()
        {
            return $"Data Source={Path.Combine(_generalConfig.UserConfig.UserDataPath, _generalConfig.DbFilename)};";
        }
    }
}