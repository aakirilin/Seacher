using Seacher.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Seacher.Models.DBTable;

namespace Seacher.Models
{
    public class DBSettings : IDBQerry
    {
        public const string AppSettingsFileName = "AppSettings.json";

        public string Title;
        public string Description;
        public string ConnectionString;
        public List<DBTable> DBTables;

        public string GetAlias(int tablePosition)
        {
            throw new NotImplementedException();
        }

        public string GetFields(int tablePosition) =>
            string.Join(", ", DBTables.Select((t, n) => $"{t.GetFields(n)}"));

        public string GetTableName(int tablePosition) =>
            string.Join(", ", DBTables.Select((t, n) => $"{t.GetTableName(n)}"));

        public string GetTableRelation(int tablePosition) =>
            string.Join(", ", DBTables.Select((t, n) => $"{t.GetTableRelation(n)}"));

    }
}


