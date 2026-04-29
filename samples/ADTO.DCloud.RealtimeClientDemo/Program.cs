using ADTO.DCloud.RealtimeClientDemo.Forms;

namespace ADTO.DCloud.RealtimeClientDemo;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}
