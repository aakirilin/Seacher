using Seacher.Common;
using Seacher.Models;
using Seacher.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Seacher.ViewModel
{
    public class DBConectionWindowViewModel : BaseViewModel
    {
        public ICommand TestCommand { get; }

        public string[] DBMSNames => Enum.GetNames<DBMSTypes>();

        private string selectedDBMSName;
        public string SelectedDBMSName
        {
            get => selectedDBMSName;
            set => SetProperty(ref selectedDBMSName, value);
        }

        private string connectionString;
        public string ConnectionString
        {
            get => connectionString;
            set => SetProperty(ref connectionString, value);
        }


        public DBConectionWindowViewModel()
        {
            TestCommand = new Command((p) =>
            {

            });
        }
    }
}
