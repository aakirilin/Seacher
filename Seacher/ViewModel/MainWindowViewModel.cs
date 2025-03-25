using Seacher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Seacher.Windows;
using Seacher.Common;
using System.Runtime;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.VisualBasic;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Seacher.ViewModel
{
    /*
        если загружается настроки то нужно обновить список имен таблиц и выбранную таблицу
        при этом нужно выбирать только главные таблицы 
    
     */
    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        private SQLAdapter sQLiteAdapter;
        private SQLAdapter settingsSQLAdapter;
        private SettingsSerializerSQlite serializerSQlite;
        private AppSettings appSettings;
        private readonly IServiceProvider serviceProvider;
        private IEnumerable<object> data;
        public IEnumerable<object> Data
        {
            get => data;
            set => SetProperty(ref data, value);
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                SetProperty(ref selectedIndex, value);
                if (value >= 0 && value < DBTablesNames.Count)
                {
                    SelectedTableName = DBTablesNames[selectedIndex];
                    appSettings.SelectTableIndex = value;
                    var serializer = new SettingsSerializerSQlite();
                    serializer.Serialize(appSettings);
                    OnPropertyChanged(nameof(SelectedTableName));
                }
            }
        }

        private string selectedTableName;
        public string SelectedTableName
        {
            get => selectedTableName;
            set
            {
                SetProperty(ref selectedTableName, value);
            }
        }

        private string selectedTableTitle;
        public string SelectedTableTitle
        {
            get => selectedTableTitle;
            set
            {
                SetProperty(ref selectedTableTitle, value);

            }
        }

        private List<string> dbTablesTitles;
        public List<string> DBTablesTitles
        {
            get => dbTablesTitles;
            set => SetProperty(ref dbTablesTitles, value);
        }

        private List<string> dBTablesNames;
        public List<string> DBTablesNames
        {
            get => dBTablesNames;
            set
            {
                SetProperty(ref dBTablesNames, value);
                OnPropertyChanged(nameof(SelectedTableName));
            }
        }

        private Type CreateType(IEnumerable<DBField> fields)
        {
            var aName = new AssemblyName("DynamicAssembly");

            var ab = AssemblyBuilder.DefineDynamicAssembly(aName ,AssemblyBuilderAccess.RunAndCollect);

            var mb = ab.DefineDynamicModule(ab.GetName().FullName);

            var tb = mb.DefineType("DynamicType", TypeAttributes.Public);

            foreach (var field in fields)
            {
                PropertyBuilder property = tb.DefineProperty(
                    field.Name,
                    PropertyAttributes.HasDefault,
                    typeof(string),
                    null);
                
                MethodAttributes getSetAttr = 
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName | 
                    MethodAttributes.HideBySig;

                MethodBuilder get = tb.DefineMethod(
                    $"get_{field.Name}",
                    getSetAttr,
                    typeof(string),
                    Type.EmptyTypes);

                ILGenerator pGetIL = get.GetILGenerator();
                pGetIL.Emit(OpCodes.Ldarg_0);
                pGetIL.Emit(OpCodes.Ldfld);
                pGetIL.Emit(OpCodes.Ret);

                MethodBuilder set = tb.DefineMethod(
                    $"set_{field.Name}",
                    getSetAttr,
                    null,
                    new Type[] { typeof(string) });

                ILGenerator numberSetIL = set.GetILGenerator();
                numberSetIL.Emit(OpCodes.Ldarg_0);
                numberSetIL.Emit(OpCodes.Ldarg_1);
                numberSetIL.Emit(OpCodes.Stfld);
                numberSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(get);
                property.SetSetMethod(set);

                var attributeCtor = tb.GetConstructor([typeof(DisplayNameAttribute)]);
                CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(attributeCtor, [field.Title]);
                property.SetCustomAttribute(caBuilder);
            }

            Type? t = tb.CreateType();
            return t;
        }

        public MainWindowViewModel(AppSettings appSettings, IServiceProvider serviceProvider)
        {
            this.appSettings = appSettings;        
            this.serviceProvider = serviceProvider;
            sQLiteAdapter = SQLAdapter.Create(appSettings.DBMSType, appSettings.ConnectionString);

            settingsSQLAdapter = SQLAdapter.Create(DBMSTypes.SQLight, SQLAdapter.SettingsCS);
            serializerSQlite = new SettingsSerializerSQlite();
            serializerSQlite.CreateDB();

            DBTablesNames = new List<string>();
            LoadSettings();

            OpenSettingsCommand = new Command((p) =>
            {
                var window = serviceProvider.GetRequiredService<SetingsWindow>();
                window.Closed += (object? sender, EventArgs e) =>
                {
                    LoadSettings();
                };

                window.ShowDialog();
                Data = null;
            });

            SearchCommand = new Command((p) =>
            {
                var mainTableName = SelectedTableName; // settings.DBTables.First().Name; // исправить на выбранную таблицу
                var fields = appSettings.DBTables
                    .FirstOrDefault(t => t.Name.Equals(mainTableName))
                    .Fields;
                
                var linkedTablets = appSettings.DBTables.Where(
                   t => !String.IsNullOrWhiteSpace(t.MainTable) && t.MainTable.Equals(mainTableName));

                var linkedTabletsFields = linkedTablets.SelectMany(t => t.Fields);

                var @params = new ExecuteReaderParams();

                fields.AddRange(linkedTabletsFields);
                @params.Fields = fields;
                @params.Tablets = linkedTablets.Select((t, n) => t.GetTableRelation(n)).ToList();
                @params.Condition = "";

                var type = CreateType(fields);

                Data = sQLiteAdapter.ExecuteReader(type, @params);
            });
        }

        public ICommand OpenSettingsCommand { get; }
        public ICommand SearchCommand { get; }

        public void LoadSettings()
        {
            DBTablesNames = appSettings?
                .DBTables?
                .Where(t => String.IsNullOrWhiteSpace(t.MainTable))?
                .Select(t => t.Name)?
                .ToList();

            DBTablesTitles = appSettings?
                .DBTables?
                .Where(t => String.IsNullOrWhiteSpace(t.MainTable))?
                .Select(t => t.Title)?
                .ToList();

            SelectedIndex = appSettings.SelectTableIndex;
            CreateInputFields(appSettings?.SelectTable?.Fields ?? new List<DBField>());
        }

        public void CreateInputFields(IEnumerable<DBField> fields)
        {
            foreach (var field in fields)
            {
                var control = new Control();
                switch (field.ConditionType)
                {
                    case ConditionTypes.String: control = new TextBox(); break;
                    case ConditionTypes.Bool: control = new CheckBox(); break;
                    case ConditionTypes.ComboBox: control = new ComboBox(); break;
                }
            }
        }

        public void Dispose()
        {
            sQLiteAdapter.Dispose();
            settingsSQLAdapter.Dispose();
        }
    }
}
