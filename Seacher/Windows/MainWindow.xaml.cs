using Seacher.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Seacher.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            viewModel.OnCreateInputFields += (controls) =>
            {
                FieldsPanel.Children.Clear();
                controls.ForEach(c => { FieldsPanel.Children.Add(c); });                
            };
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CreateInputFields();
        }
    }
}