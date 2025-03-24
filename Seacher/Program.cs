using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seacher.Common;
using Seacher.ViewModel;
using Seacher.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // создаем хост приложения
            var host = Host.CreateDefaultBuilder()
                // внедряем сервисы
                .ConfigureServices(services =>
                {
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<AddTabletWindow>();
                    services.AddTransient<DBConectionWindow>();
                    services.AddTransient<SetingsWindow>();

                    services.AddTransient<AddTabletWindowViewModel>();
                    services.AddTransient<DBConectionWindowViewModel>();
                    services.AddTransient<DBFieldViewModel>();
                    services.AddTransient<DBSettingsViewModel>();
                    services.AddTransient<DBTableViewModel>();
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<SettingsViewModel>();

                    services.AddSingleton<SettingsSerializerSQlite>();
                    services.AddSingleton<AppSettings>();
                })
                .Build();
            // получаем сервис - объект класса App
            var app = host.Services.GetService<App>();
            // запускаем приложения
            app?.Run();
        }
    }
}
