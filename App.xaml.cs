using System.Configuration;
using System.Data;
using System.Windows;

namespace SecurityProject;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Global exception handler
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Bir hata oluştu:\n\n{e.Exception.Message}\n\nDetay: {e.Exception.StackTrace}", 
                       "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}

