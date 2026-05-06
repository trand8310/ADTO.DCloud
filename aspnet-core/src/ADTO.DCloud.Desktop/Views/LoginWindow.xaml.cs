using System.Windows;
using ADTO.DCloud.Desktop.ViewModels;

namespace ADTO.DCloud.Desktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
