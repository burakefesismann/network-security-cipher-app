using System.Configuration;
using System.Data;
using System.Windows;

namespace SecurityProject;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Uygulama başlatıldığında çağrılan metod.
    /// Global hata yakalama mekanizmasını kurar.
    /// </summary>
    /// <param name="e">Başlatma olay argümanları</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Temel başlatma işlemlerini yap (Application sınıfının OnStartup'ı)
        base.OnStartup(e);
        
        // Uygulama genelinde yakalanmayan hataları yakalamak için olay işleyicisini bağla
        // Bu, beklenmeyen hataların kullanıcıya gösterilmesini ve uygulamanın çökmesini önler
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    /// <summary>
    /// Uygulama genelinde yakalanmayan hataları yakalayan olay işleyici.
    /// Hata mesajını kullanıcıya gösterir ve uygulamanın çökmesini önler.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen nesne (Application)</param>
    /// <param name="e">Hata bilgilerini içeren DispatcherUnhandledExceptionEventArgs</param>
    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // Kullanıcıya hata mesajını göster
        // Hata mesajı, detaylı açıklama ve stack trace içerir
        MessageBox.Show($"Bir hata oluştu:\n\n{e.Exception.Message}\n\nDetay: {e.Exception.StackTrace}", 
                       "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        
        // Hatayı işlendi olarak işaretle
        // Bu, uygulamanın çökmesini önler ve kullanıcının devam etmesine izin verir
        e.Handled = true;
    }
}

