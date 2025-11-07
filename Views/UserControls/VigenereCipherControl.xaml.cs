using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Vigenère Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// Vigenère şifresi, 16. yüzyılda Blaise de Vigenère tarafından geliştirilen polialfabetik bir şifreleme tekniğidir.
/// Bir anahtar kelime kullanır ve bu kelime tekrar tekrar uygulanarak her harf farklı bir kaydırma değeri ile şifrelenir.
/// Caesar şifresinden daha güvenlidir çünkü frekans analizi saldırılarına karşı daha dayanıklıdır.
/// Tabula recta veya Vigenère karesi olarak bilinen bir matris kullanılabilir.
/// </summary>
public partial class VigenereCipherControl : UserControl
{
    /// <summary>
    /// Vigenère şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// Anahtar kelimeyi tekrarlayarak her karakter için farklı kaydırma uygular.
    /// </summary>
    private VigenereCipher? cipher;

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım şifreleme bilgilerini tutan koleksiyon.
    /// Her adım bir harfin hangi anahtar harfi ile nasıl kaydırıldığını gösterir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni kaydırma adımı ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// VigenereCipherControl sınıfının yapıcı metodu.
    /// Kontrolü başlatır, adım görüntüleme listesini bağlar ve
    /// varsayılan anahtar olarak "KEY" kelimesini kullanır.
    /// </summary>
    public VigenereCipherControl()
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
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;

        // Etiketleri güncelle
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text" : "Plain Text";

        UpdateCipher();
    }

    /// <summary>
    /// Anahtar kelime değiştiğinde tetiklenen olay işleyici.
    /// Anahtar kelime tekrar edilerek her karakter için kaydırma değeri belirlenir.
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
    /// Vigenère şifreleme işlemini gerçekleştiren ana metod.
    /// Anahtar kelimeyi tekrarlayarak her harf için farklı Caesar kaydırması uygular.
    /// Örnek: Anahtar "KEY" ise, 1. harf K(10) kadar, 2. harf E(4) kadar, 3. harf Y(24) kadar kaydırılır, sonra tekrar K ile başlar.
    /// </summary>
    private void UpdateCipher()
    {
        // UI bileşenlerinin hazır olduğundan emin ol
        if (KeyInput == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Anahtar boşsa işlem yapma
        if (string.IsNullOrWhiteSpace(KeyInput.Text))
            return;

        // Anahtar kelime ile Vigenère cipher nesnesi oluştur
        cipher = new VigenereCipher(KeyInput.Text);

        // Giriş metni varsa şifreleme işlemini gerçekleştir
        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            bool isEncrypt = ModeComboBox.SelectedIndex == 0;

            // Her karakter için anahtar kelimeyi tekrarla ve kaydır
            // Encrypt: Ci = (Pi + Ki) mod 26
            // Decrypt: Pi = (Ci - Ki) mod 26
            string result = isEncrypt
                ? cipher.Encrypt(PlainTextInput.Text)
                : cipher.Decrypt(PlainTextInput.Text);

            CipherTextOutput.Text = result;

            // Her karakterin hangi anahtar harfi ile kaydırıldığını göster
            AnimateSteps(isEncrypt, PlainTextInput.Text);
        }
        else
        {
            CipherTextOutput.Text = "";
            StopStepAnimation();
            displayedSteps.Clear();
        }
    }

    /// <summary>
    /// Vigenère şifreleme adımlarını animasyonlu olarak görüntüler.
    /// Her karakterin hangi anahtar harfi ile kaydırıldığını 300ms aralıklarla gösterir.
    /// </summary>
    /// <param name="isEncrypt">Şifreleme mi şifre çözme mi yapıldığını belirtir</param>
    /// <param name="text">İşlenecek metin</param>
    private void AnimateSteps(bool isEncrypt, string text)
    {
        if (cipher == null) return;

        StopStepAnimation();
        displayedSteps.Clear();

        // Her karakter için anahtar kelime eşleşmesi adımlarını al
        var steps = isEncrypt
            ? cipher.GetEncryptionSteps(text)
            : cipher.GetDecryptionSteps(text);

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
