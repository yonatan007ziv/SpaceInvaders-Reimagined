using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace GameWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = RenderMode.Default;
            base.OnStartup(e);
        }
    }
}
