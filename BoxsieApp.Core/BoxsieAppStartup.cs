using System;
using System.Linq;
using System.Reflection;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Config.Contracts;
using BoxsieApp.Core.Net;
using BoxsieApp.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BoxsieApp.Core
{
    public abstract class BoxsieAppStartup
    {
        protected abstract void ConfigureServices(IServiceCollection services);
        protected abstract void Configure(IServiceProvider serviceProvider);

        public IBosieApp Initialise<T>() where T : IBosieApp
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(IBosieApp), typeof(T));

            ConfigureBoxsieServices(serviceCollection);
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            ConfigureBoxsie(serviceProvider);
            Configure(serviceProvider);
            
            return serviceProvider.GetService<IBosieApp>();
        }
        
        private static void ConfigureBoxsieServices(IServiceCollection services)
        {
            services.AddLogging(builder => { builder.SetMinimumLevel(LogLevel.Trace); });

            services.AddSingleton<RepositoryService>();

            services.AddSingleton<HttpClientFactory>();

            AddConfigs(services);
        }

        private static void ConfigureBoxsie(IServiceProvider serviceProvider)
        {
            Cfg.InitialiseConfig(serviceProvider.GetServices<IConfig>());
        }

        private static void AddConfigs(IServiceCollection services)
        {
            services.AddSingleton<GeneralConfig>(x => Cfg.ConfigFactory<GeneralConfig>());
            services.AddSingleton<IConfig, GeneralConfig>(x => x.GetService<GeneralConfig>());

            var contract = typeof(IConfig);

            var configTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()
                    .Where(y => contract.IsAssignableFrom(y) && !y.IsInterface && !y.IsAbstract && y != typeof(GeneralConfig)));

            foreach (var configType in configTypes)
            {
                services.AddSingleton(configType, x => Cfg.ConfigFactory(configType));
                services.AddSingleton(contract, x => x.GetService(configType));
            }
        }
    }
}
