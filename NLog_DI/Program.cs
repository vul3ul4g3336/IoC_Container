
using IOCServiceCollection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog_DI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NLog_DI
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var config = CreateConfig();
            var serviceProvider = CreateServiceProvider(config);

            var form = serviceProvider.GetService(typeof(Form1)) as Form;
            Application.Run(form);

        }
        private static IConfiguration CreateConfig()
        {
            var config = new ConfigurationBuilder()
                         .SetBasePath(System.IO.Directory
                                            .GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                         .AddJsonFile("appsettings.json", true, true)
                         .Build();
            return config;
        }
        private static IServiceProvider CreateServiceProvider(IConfiguration config)
        {
            var serviceCollection = new IoC_Container.ServiceCollection();
            serviceCollection.AddTransient<Form1>(); // Runner is the custom class
            serviceCollection.AddTransient<Runner>();
            serviceCollection.AddLogging(loggingBuilder =>
                                   {
                                       // configure Logging with NLog
                                       loggingBuilder.ClearProviders();
                                       loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                                       loggingBuilder.AddNLog(config);
                                   });
            
            return serviceCollection.BuildServiceProvider();
        }
    }
}
