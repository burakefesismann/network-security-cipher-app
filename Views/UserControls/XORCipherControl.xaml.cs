using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// XOR Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// XOR (Exclusive OR) şifresi, modern kriptografide temel bir operasyondur.
/// Her karakter ile anahtar karakteri arasında bit düzeyinde XOR işlemi uygulanır.
/// XOR'un özel özelliği: (A XOR B) XOR B = A, bu nedenle aynı anahtar hem şifrelemede hem çözmede kullanılır.
/// Sonuç hexadecimal (onaltılık) formatta gösterilir.
/// </summary>
public partial class XORCipherControl : UserControl
{
    /// <summary>
    /// XOR şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// Bit düzeyinde XOR işlemlerini gerçekleştirir.
    /// </summary>
    private XORCipher cipher = new XORCipher();

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım şifreleme bilgilerini tutan koleksiyon.
    /// Her adım bir karakterin binary gösterimi ve XOR işlemini gösterir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni XOR işlemi adımı ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// XORCipherControl sınıfının yapıcı metodu.
    /// Kontrolü başlatır, adım görüntüleme listesini bağlar ve
    /// varsayılan anahtar olarak "KEY" kelimesini kullanır.
    /// </summary>
    public XORCipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için veri bağlamayı ayarla
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // Varsayılan anahtar kelimeyi ayarla
        if (KeyInput != null)
            KeyInput.Text = "KEY";

        // İlk şifreleme işlemini başlat
        UpdateCipher();
    }

    /// <summary>
    /// Şifreleme modu değiştiğinde tetiklenen olay işleyici.
    /// XOR'da encrypt ve decrypt aynı işlem olsa da, girdi formatı farklıdır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;

        // Etiketleri güncelle
        // Encrypt: düz metin gir, hex al
        // Decrypt: hex gir, düz metin al
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text (Hex)";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text (Hex)" : "Plain Text";

        UpdateCipher();
    }

    /// <summary>
    /// Anahtar kelime değiştiğinde tetiklenen olay işleyici.
    /// Anahtar tekrar edilerek her karakter için XOR işlemi yapılır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void KeyInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    /// <summary>
    /// Giriş metni değiştiğinde tetiklenen olay işleyici.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    /// <summary>
    /// XOR şifreleme işlemini gerçekleştiren ana metod.
    /// Her karakter byte'a çevrilir ve ilgili anahtar byte'ı ile XOR'lanır.
    /// Sonuç hexadecimal formatta gösterilir.
    /// XOR özelliği: aynı işlem hem şifreler hem çözer (A XOR B XOR B = A).
    /// </summary>
    private void UpdateCipher()
    {
        // UI bileşenlerinin hazır olduğundan emin ol
        if (KeyInput == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Anahtar boşsa işlem yapma
        if (string.IsNullOrWhiteSpace(KeyInput.Text))
            return;

        // Giriş metni varsa XOR işlemini gerçekleştir
        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            bool isEncrypt = ModeComboBox.SelectedIndex == 0;

            // XOR işlemi: her karakter byte'a çevrilir, anahtar byte'ı ile XOR'lanır
            // Encrypt: metin + anahtar -> hex
            // Decrypt: hex + anahtar -> metin
            string result = isEncrypt
                ? cipher.Encrypt(PlainTextInput.Text, KeyInput.Text)
                : cipher.Decrypt(PlainTextInput.Text, KeyInput.Text);

            CipherTextOutput.Text = result;

            // Her karakterin binary gösterimi ve XOR işlemi adımlarını göster
            if (!string.IsNullOrWhiteSpace(KeyInput.Text))
                AnimateSteps(isEncrypt, PlainTextInput.Text, KeyInput.Text);
        }
        else
        {
            CipherTextOutput.Text = "";
            StopStepAnimation();
            displayedSteps.Clear();
        }
    }

    /// <summary>
    /// XOR şifreleme adımlarını animasyonlu olarak görüntüler.
    /// Her karakterin binary gösterimi, anahtar karakterin binary'si ve
    /// sonuç XOR değeri 300ms aralıklarla gösterilir.
    /// </summary>
    /// <param name="isEncrypt">Şifreleme mi şifre çözme mi yapıldığını belirtir</param>
    /// <param name="text">İşlenecek metin</param>
    /// <param name="key">XOR anahtarı</param>
    private void AnimateSteps(bool isEncrypt, string text, string key)
    {
        StopStepAnimation();
        displayedSteps.Clear();

        // Her karakter için XOR işlemi adımlarını al
        var steps = isEncrypt
            ? cipher.GetEncryptionSteps(text, key)
            : cipher.GetDecryptionSteps(text, key);

        if (steps.Count == 0) return;

        pendingSteps = steps;
        int currentIndex = 0;

        // Animasyon başlat
        stepTimer = new System.Windows.Threading.DispatcherTimer();
        stepTimer.Interval = TimeSpan.FromMilliseconds(300);
        stepTimer.Tick += (s, e) =>
        {
            if (currentIndex < pendingSteps.Count)
            {
                displayedSteps.Add(pendingSteps[currentIndex]);
                currentIndex++;
            }
            else
            {
                StopStepAnimation();
            }
        };
        stepTimer.Start();
    }

    /// <summary>
    /// Adım animasyonunu durdurur ve kaynakları temizler.
    /// </summary>
    private void StopStepAnimation()
    {
        if (stepTimer != null)
        {
            stepTimer.Stop();
            stepTimer = null;
        }
        pendingSteps = null;
    }

    /// <summary>
    /// Ana menüye geri dönüş butonu tıklama olay işleyici.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Button kontrolü</param>
    /// <param name="e">Tıklama olay argümanları</param>
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var parent = Parent;
        while (parent != null && !(parent is MainWindow))
        {
            parent = parent is FrameworkElement fe ? fe.Parent : null;
        }

        if (parent is MainWindow mainWindow)
        {
            mainWindow.NavigateToMenu();
        }
    }
}

