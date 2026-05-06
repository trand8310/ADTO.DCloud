using System.Windows;
using ADTO.DCloud.Desktop.Views;
using Prism.Ioc;

namespace ADTO.DCloud.Desktop.Services;

public sealed class ApplicationNavigator(IContainerProvider containerProvider) : IApplicationNavigator
{
    public void ShowMainWindow()
    {
        var main = containerProvider.Resolve<MainWindow>();
        Swap(main);
    }

    public void ShowLoginWindow()
    {
        var login = containerProvider.Resolve<LoginWindow>();
        Swap(login);
    }

    private static void Swap(Window next)
    {
        var current = Application.Current.MainWindow;
        Application.Current.MainWindow = next;
        next.Show();
        current?.Close();
    }
}
