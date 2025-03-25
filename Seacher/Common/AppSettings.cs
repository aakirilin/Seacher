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
        public string ConnectionString { get; set; }
        public int SelectTableIndex { get; set; }
        public DBMSTypes DBMSType { get; set; }


        public DBTable SelectTable
        {
            get => DBTables?[SelectTableIndex];
        }

        public AppSettings()
        {
        }

        public AppSettings(SettingsSerializerSQlite settingsSerializer)
        {
            var appSettings = new AppSettings();
            settingsSerializer.Deserialize(ref appSettings);

            DBTables = appSettings.DBTables;
            SelectTableIndex = appSettings.SelectTableIndex;
            ConnectionString = appSettings.ConnectionString;
            DBMSType = appSettings.DBMSType;
        }
    }
}
