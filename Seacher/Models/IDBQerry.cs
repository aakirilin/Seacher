using Seacher.ViewModel;

namespace Seacher.Models
{
    public interface IDBQerry
    {
        public string GetFields(int tablePosition);
        public string GetTableName(int tablePosition);
        public string GetTableRelation(int tablePosition);
        public string GetAlias(int tablePosition);
    }
}
