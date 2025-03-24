using Seacher.Enttiy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Common
{
    public interface ISQLDBParser
    {
        IEnumerable<string> GetTablesNames(ISQLAdapter adapter);
        IEnumerable<FieldsEnttiy> GetFields(ISQLAdapter adapter, string tabletName);
    }
}
