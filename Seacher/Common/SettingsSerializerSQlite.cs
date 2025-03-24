using Seacher.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Seacher.Enttiy;
using System.Windows.Shapes;

namespace Seacher.Common
{
    public class SettingsSerializerSQlite
    {
        private SQLAdapter adapter;
        private Dictionary<int, string> migrations = new Dictionary<int, string>()
        {
            { 1001, """
                INSERT INTO setting (name, sValue) values ('DBCSType', 'SQLight');
                """}
        };

        public SettingsSerializerSQlite() 
        {
            this.adapter = SQLAdapter.Create(DBMSTypes.SQLight, SQLAdapter.SettingsCS);
        }

        private static string AppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version
                .ToString();

            return version;
        }

        public void CreateDB()
        {
            var qerry = $"""
                DROP TABLE If EXISTS setting;
                DROP TABLE If EXISTS tablets;
                DROP TABLE If EXISTS fields;

                CREATE TABLE setting
                (
                    name VARCHAR(20) PRIMARY KEY,
                    iValue int,
                    sValue VARCHAR(100)	
                );

                CREATE TABLE tablets
                (
                	name VARCHAR(20) PRIMARY KEY,
                	title VARCHAR(20),
                	discription VARCHAR(20),
                	mainTable VARCHAR(20),
                	relationToMainTable VARCHAR(20)
                );

                CREATE TABLE fields
                (
                	tabletsName VARCHAR(20), 
                	title VARCHAR(20),
                	name VARCHAR(20),
                	discription VARCHAR(20),
                	inQerry int DEFAULT 0,
                	inCodition int DEFAULT 0,
                	conditionType VARCHAR(20),
                	PRIMARY KEY (tabletsName, name)
                );

                INSERT INTO setting (name, iValue, sValue) VALUEs ('version', NULL, '1.0.0.0');
                INSERT INTO setting (name, iValue, sValue) VALUEs ('cs', NULL, '');
                INSERT INTO setting (name, iValue, sValue) VALUEs ('selTableName', NULL, '');
                """;

            long dbSize = new FileInfo(SQLAdapter.DBFileName).Length;
            if (!File.Exists(SQLAdapter.DBFileName) || dbSize == 0)
            {
                adapter.Open();
                adapter.ExecuteNonQuery(qerry);
                adapter.Close();
            }
            if (File.Exists(SQLAdapter.DBFileName))
            {
                adapter.Open();

                var @params = new ExecuteReaderParams();

                @params.Tablets = ["setting"];
                @params.Fields = [
                    new DBField(){ Name = "name" },
                    new DBField(){ Name = "iValue" },
                    new DBField(){ Name = "sValue" },
                ];
                @params.Condition = "name = 'version'";
                @params.Properties = ["SValue"];

                var dbVersion = adapter.ExecuteReader(typeof(GlobalSettingsEnttiy), @params)
                    .OfType<GlobalSettingsEnttiy>().First().SValue;

                var dbVersionNumber = int.Parse(dbVersion.Replace(".", ""));

                var requiredMigrations = migrations
                    .Where(m => m.Key > dbVersionNumber)
                    .OrderBy(m => m.Key)
                    .Select(m => m.Value);

                var updateQerry = String.Join("", requiredMigrations) + $"""
                    UPDATE setting SET sValue = '{AppVersion()}' WHERE name = 'version';
                    """;

                adapter.ExecuteNonQuery(updateQerry);

                adapter.Close();
            }
        }

        public void Serialize(AppSettings settings)
        {
            adapter.Open();

            var qerry = $"""
                UPDATE setting SET sValue = '{AppVersion()}' WHERE name = 'version';
                UPDATE setting SET sValue = '{settings.ConnectionString}' WHERE name = 'cs';
                UPDATE setting SET sValue = '{settings.SelectTableName}' WHERE name = 'selTableName';
                UPDATE setting SET sValue = '{settings.DBMSType.ToString()}' WHERE name = 'DBCSType';
                """;
            adapter.ExecuteNonQuery(qerry);

            foreach (var table in settings.DBTables)
            {
                qerry = $"""
                    INSERT INTO tablets (name, title, discription, mainTable, relationToMainTable) values ('{table.Name}', '{table.Title}', '{table.Description}', '{table.MainTable}', '{table.RelationToMainTable}') 
                    ON CONFLICT(name) DO UPDATE SET title = '{table.Title}', discription = '{table.Description}', mainTable = '{table.MainTable}', relationToMainTable = '{table.RelationToMainTable}'; 
                """;
                adapter.ExecuteNonQuery(qerry);

                foreach (var field in table.Fields)
                {
                    var inQerry = field.InQerry ? 1 : 0;
                    var inCodition = field.InCodition ? 1 : 0;
                    qerry = $"""
                        INSERT INTO fields (tabletsName, name, title, discription, inQerry, inCodition, conditionType) 
                        values ('{table.Name}', '{field.Name}', '{field.Title}', '{field.Description}', {inQerry}, {inCodition}, '{Enum.GetName(field.ConditionType)}')
                        ON CONFLICT(tabletsName, name) DO UPDATE SET tabletsName = '{table.Name}', title = '{field.Title}', discription = '{field.Description}', inQerry={inQerry}, inCodition={inCodition}, conditionType='{Enum.GetName(field.ConditionType)}';
                    """;

                    adapter.ExecuteNonQuery(qerry);
                }
            }

            adapter.Close();
        }

        public AppSettings Deserialize(ref AppSettings result)
        {
            adapter.Open();

            CreateDB();

            var @params = new ExecuteReaderParams();

            @params.Tablets = ["setting"];
            @params.Fields = [
                new DBField(){ Name = "name" }, 
                new DBField(){ Name = "iValue" }, 
                new DBField(){ Name = "sValue" }, 
            ];
            @params.Condition = "";
            @params.Properties = ["SValue", "SValue", "SValue", "SValue"];

            var globalSettings = adapter.ExecuteReader(typeof(GlobalSettingsEnttiy), @params)
                .OfType<GlobalSettingsEnttiy>()
                .ToDictionary(s => s.Name, s => s);

            var version = globalSettings["version"].SValue;
            result.ConnectionString = globalSettings["cs"].SValue;
            result.SelectTableName = globalSettings["selTableName"].SValue;
            result.DBMSType = Enum.Parse<DBMSTypes>(globalSettings["DBCSType"].SValue);

            @params.Tablets = ["tablets"];
            @params.Fields = [
                new DBField(){ Name = "name" },
                new DBField(){ Name = "Title" },
                new DBField(){ Name = "Discription" },
                new DBField(){ Name = "MainTable" },
                new DBField(){ Name = "RelationToMainTable" }
            ];
            @params.Condition = "";
            @params.Properties = ["Name", "Title", "Discription", "MainTable", "RelationToMainTable"];

            var tablets = adapter.ExecuteReader(typeof(TabletsEnttiy), @params)
                .OfType<TabletsEnttiy>()
                .ToArray();

            @params.Tablets = ["fields"];
            @params.Fields = [
                new DBField(){ Name = "tabletsName" },
                new DBField(){ Name = "title" },
                new DBField(){ Name = "name" },
                new DBField(){ Name = "discription" },
                new DBField(){ Name = "inQerry" },
                new DBField(){ Name = "inCodition" },
                new DBField(){ Name = "conditionType" }
            ];
            @params.Condition = "";
            @params.Properties = ["TabletsName", "Title", "Name", "Discription", "InQerry", "InCodition", "ConditionType"];

            var fields = adapter.ExecuteReader(typeof(FieldsEnttiy), @params)
                .OfType<FieldsEnttiy>()
                .ToArray();

            result.DBTables = new List<DBTable>();

            foreach ( var tablet in tablets)
            {
                var t = (DBTable)tablet;
                t.Fields = fields
                    .Where(f => f.TabletsName.Equals(t.Name))
                    .Select(f => (DBField)f)
                    .ToList();

                result.DBTables.Add(t);
            }

            adapter.Close();
            
            return result;
        }
    }
}
