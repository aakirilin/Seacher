using Seacher.Models;

namespace Seacher.Common
{
    public interface ISQLAdapter
    {
        void Open();
        void Close();
        void ExecuteNonQuery(string qerrys, string delimiter = ";");
        IEnumerable<object> ExecuteReader(Type type, ExecuteReaderParams @params);

    }
}
