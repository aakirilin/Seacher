using Microsoft.Extensions.DependencyInjection;
using Seacher.Common;
using Seacher.Models;
using Seacher.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Seacher.ViewModel
{
    public class SettingsViewModel : BaseViewModel, IDisposable
    {
        private AppSettings appSettings;

        private AppSettings AppSettings
        {
            get => appSettings;
            set
            {
                SetProperty(ref appSettings, value);
                OnPropertyChanged(nameof(DBTables));
                OnPropertyChanged(nameof(SelectedTablet));
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        private readonly IServiceProvider serviceProvider;
        private SQLAdapter adapter;

        private ObservableCollection<DBTableViewModel> dBTables;
        public ObservableCollection<DBTableViewModel> DBTables 
        {
            get => dBTables;
            set
            {
                SetProperty(ref dBTables, value);
                AppSettings.DBTables = value?
                    .Select(t => (DBTable)t)?
                    .ToList() ?? new List<DBTable>();
            }
        }

        private DBTableViewModel selectedTablet;
        public DBTableViewModel SelectedTablet
        { 
            get => selectedTablet;
            set
            {
                SetProperty(ref selectedTablet, value);
                AppSettings.SelectTableName = value?.Name ?? "";
            }
        }

        public string connectionString;
        public string ConnectionString 
        {
            get => connectionString;
            set
            {
                SetProperty(ref connectionString, value);
                AppSettings.ConnectionString = value;
            }
        }

        public ICommand OpenCStringWindowCommand {  get; }
        public ICommand GetShemaCommand {  get; }
        public ICommand AddTabletCommand {  get; }
        public ICommand SelectTabletCommand {  get; }
        public ICommand SaveCommand {  get; }

        public SettingsViewModel(AppSettings appSettings, IServiceProvider serviceProvider)
        {
            this.AppSettings = appSettings;
            this.serviceProvider = serviceProvider;
            adapter = SQLAdapter.Create(DBMSTypes.SQLight, SQLAdapter.SettingsCS);
            var serializer = new SettingsSerializerSQlite();
            serializer.CreateDB();

            DBTables = new ObservableCollection<DBTableViewModel>(
                appSettings.DBTables.Select(t => new DBTableViewModel(t))
            );
            SelectedTablet = new DBTableViewModel(appSettings.SelectTable);

            GetShemaCommand = new Command((p) =>
            {
                var csWindow = serviceProvider.GetRequiredService<DBConectionWindow>();
                csWindow.ShowDialog();
            });

            AddTabletCommand = new Command((p) =>
            {
                var window = serviceProvider.GetRequiredService<AddTabletWindow>();
                var viewModel = new AddTabletWindowViewModel();

                viewModel.AddCommand = new Command((p) =>
                {
                    DBTables.Add(new DBTableViewModel(null)
                    {
                        Description = viewModel.Description,
                        Name = viewModel.Name,
                        Title = viewModel.Title,
                        Fields = new ObservableCollection<DBFieldViewModel>(new List<DBFieldViewModel>())
                    }); ;
                    window.Close(); 
                });
                window.DataContext = viewModel;
                window.ShowDialog();
                
            });

            SaveCommand = new Command((p) =>
            {
                var tables = DBTables.Select(t =>(DBTable)t).ToList();
                var selectTable = (DBTable)SelectedTablet ?? tables.FirstOrDefault();

                AppSettings.DBTables = tables;
                AppSettings.SelectTableName = selectedTablet.Name;

                var serializer = new SettingsSerializerSQlite();
                serializer.Serialize(AppSettings);
            });            
            LoadSettings();
        }

        public void LoadSettings()
        {
            var serializer = new SettingsSerializerSQlite();

            var tables = appSettings.DBTables?.Select(t => new DBTableViewModel(t));
            DBTables = new ObservableCollection<DBTableViewModel>(tables ?? new List<DBTableViewModel>());
            var selectTableName = appSettings.SelectTableName;

            SelectedTablet = DBTables.FirstOrDefault(t => t.Name.Equals(selectTableName));
            ConnectionString = appSettings.ConnectionString;
        }

        public void Dispose()
        {
            adapter.Dispose();
        }
    }
}
