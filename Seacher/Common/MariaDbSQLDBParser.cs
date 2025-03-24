using Seacher.Enttiy;
using Seacher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Common
{
    public class MariaDbSQLDBParser : ISQLDBParser
    {
        public IEnumerable<FieldsEnttiy> GetFields(ISQLAdapter adapter, string tabletName)
        {
            var @params = new ExecuteReaderParams();

            @params.Tablets = ["information_schema.columns"];
            @params.Fields = [
                new DBField(){ Name = "column_name" },
                new DBField(){ Name = "data_type" },
                new DBField(){ Name = "max_length" },
            ];
            @params.Condition = $"table_name = '{tabletName}'";
            @params.Properties = ["ColumnName", "DataType", "MaxLength"];

            var fields = adapter.ExecuteReader(typeof(DBTabletFieldEnttiy), @params)
                .OfType<DBTabletFieldEnttiy>()
                .Select(t => new FieldsEnttiy { Name = t.ColumnName, TabletsName = tabletName})
                .ToArray();

            return fields;
        }

        public IEnumerable<string> GetTablesNames(ISQLAdapter adapter)
        {
            var @params = new ExecuteReaderParams();

            @params.Tablets = ["information_schema.TABLES"];
            @params.Fields = [
                new DBField(){ Name = "TABLE_NAME" }
            ];
            @params.Condition = "";
            @params.Properties = ["TabletName"];

            var tablesNames = adapter.ExecuteReader(typeof(DBTabletNameEnttiy), @params)
                .OfType<DBTabletNameEnttiy>()
                .Select(t => t.TabletName)
                .ToArray();

            return tablesNames;
        }
    }
}
