using System.Windows;
using ADTO.DCloud.Desktop.ViewModels;

namespace ADTO.DCloud.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
