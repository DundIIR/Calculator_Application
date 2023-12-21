using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Calculator_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();

            //serviceCollection.AddSingleton<IMemory, RamMemory>();
            serviceCollection.AddSingleton<IMemory, FileMemory>();
            //serviceCollection.AddSingleton<IMemory, DBMemory>();


            serviceCollection.AddTransient<MainViewModel>();

            Provider = serviceCollection.BuildServiceProvider();
        }

        public static ServiceProvider Provider { get; private set; }
    }
}
