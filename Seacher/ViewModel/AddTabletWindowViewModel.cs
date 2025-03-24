using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Seacher.ViewModel
{
    public class AddTabletWindowViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public ICommand AddCommand { get; set; }
    }
}
