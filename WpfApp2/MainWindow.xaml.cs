
using System.Windows;
using WpfApp2.ViewModels;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}