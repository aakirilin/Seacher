using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seacher.Models
{
    public class ExecuteReaderParams
    {
        public IEnumerable<string> Properties { get; set; }
        public IEnumerable<DBField> Fields { get; set; }
        public IEnumerable<string> Tablets { get; set; }
        public string Condition { get; set; }
    }
}
