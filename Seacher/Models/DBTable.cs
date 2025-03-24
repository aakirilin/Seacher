using Seacher.Enttiy;
using Seacher.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Seacher.Models.DBTable;

namespace Seacher.Models
{
    public class DBTable : IDBQerry
    { 
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title;
        /// <summary>
        /// Описание
        /// </summary>
        public string Description;
        /// <summary>
        /// Название
        /// </summary>
        public string Name;
        /// <summary>
        /// Поля
        /// </summary>
        public List<DBField> Fields;
        /// <summary>
        /// Основная таблица
        /// </summary>
        public string MainTable;
        /// <summary>
        /// Связь с основной таблицей
        /// </summary>
        public string RelationToMainTable;

        public DBTable() 
        { 
            Fields = new List<DBField>();
        }


        public string GetQerry(int tablePosition) => 
            $" select {GetFields(tablePosition)} " +
            $" from {GetTableName(tablePosition)} " +
            $" {GetTableRelation(tablePosition)}";
        public string GetFields(int tablePosition) => 
            string.Join(", ", Fields.Select((f, n) => $"t{tablePosition}.{f.Name} as t{tablePosition}_f{n}"));

        public string GetTableName(int tablePosition) => $"{Name} as t{tablePosition}";

        public string GetTableRelation(int tablePosition)
        {
            if (String.IsNullOrWhiteSpace(RelationToMainTable))
            {
                return GetAlias(tablePosition);
            }
            else
            {
                return $"{RelationToMainTable} join {GetAlias(tablePosition)}";
            }
        }
        public string GetAlias(int tablePosition) => $"{Name} as t{tablePosition}";

        public static explicit operator DBTable(DBTableViewModel v)
        {         
            return new DBTable
            {
                Title = v?.Title,
                Description = v?.Description,
                Name = v?.Name,
                Fields = v?.Fields?.Select(f => (DBField)f)?.ToList(),
                MainTable = v?.MainTable,
                RelationToMainTable = v?.RelationToMainTable
            };
        }

        public static explicit operator DBTable(TabletsEnttiy t)
        {
            return new DBTable
            {
                Title = t?.Title,
                Description = t?.Description,
                Name = t?.Name,
                Fields = new List<DBField>(),
                MainTable = t?.MainTable,
                RelationToMainTable = t?.RelationToMainTable
            };
        }
    }
}
