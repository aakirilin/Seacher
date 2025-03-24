using Seacher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Common
{
    public class AppSettings
    {
        public List<DBTable> DBTables { get; set; }
        public string SelectTableName { get; set; }
        public string ConnectionString { get; set; }
        public DBMSTypes DBMSType { get; set; }


        public DBTable SelectTable
        {
            get => DBTables?.FirstOrDefault(t => t.Name.Equals(SelectTableName));
        }

        public AppSettings()
        {
        }

        public AppSettings(SettingsSerializerSQlite settingsSerializer)
        {
            var appSettings = new AppSettings();
            settingsSerializer.Deserialize(ref appSettings);

            DBTables = appSettings.DBTables;
            SelectTableName = appSettings.SelectTableName;
            ConnectionString = appSettings.ConnectionString;
            DBMSType = appSettings.DBMSType;
        }
    }
}
