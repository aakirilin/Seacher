using Seacher.Models;
using Seacher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Seacher.Windows
{
    /// <summary>
    /// Логика взаимодействия для SetingsWindow.xaml
    /// </summary>
    public partial class SetingsWindow : Window
    {
        public SetingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
