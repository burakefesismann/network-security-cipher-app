using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Hill Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// Hill şifresi, lineer cebir ve matris çarpımı kullanarak şifreleme yapan polialfabetik bir şifreleme tekniğidir.
/// Lester S. Hill tarafından 1929'da icat edilmiştir.
/// Bu implementasyon 2x2 matris kullanır ve metni 2'li bloklar halinde işler.
/// Matrisin tersi alınabilir olmalıdır (determinant mod 26'da ters çevrilebilir olmalı).
/// </summary>
public partial class HillCipherControl : UserControl
{
    /// <summary>
    /// Hill şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// 2x2 anahtar matrisi ve matris işlemlerini yönetir.
    /// </summary>
    private HillCipher? cipher;

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım şifreleme bilgilerini tutan koleksiyon.
    /// Her adım bir karakter bloğunun matris çarpımı ile nasıl şifrelendiğini gösterir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni matris işlemi adımı ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// HillCipherControl sınıfının yapıcı metodu.
    /// Kontrolü başlatır, adım görüntüleme listesini bağlar ve
    /// varsayılan matris değerleri ile ilk şifreleme işlemini tetikler.
    /// </summary>
    public HillCipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için veri bağlamayı ayarla
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // Varsayılan matris değerleri ile ilk güncellemeyi tetikle
        MatrixChanged(null!, null!);
    }

    /// <summary>
    /// Şifreleme modu değiştiğinde tetiklenen olay işleyici.
    /// Encrypt modunda anahtar matrisi kullanılır, Decrypt modunda matrisin tersi kullanılır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;

        // Etiketleri moda göre güncelle
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text" : "Plain Text";

        // Yeni modla şifreleme işlemini yeniden gerçekleştir
        UpdateCipher();
    }

    /// <summary>
    /// Matris elemanlarından herhangi biri değiştiğinde tetiklenen olay işleyici.
    /// 2x2 matrisin dört elemanından biri değiştiğinde şifreleme güncellenir.
    /// Matris geçerli olmalıdır (determinant mod 26'da tersi alınabilir olmalı).
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü (M00, M01, M10, M11)</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void MatrixChanged(object sender, TextChangedEventArgs e)
    {
        // Matris değiştiğinde şifreleme işlemini güncelle
        UpdateCipher();
    }

    /// <summary>
    /// Giriş metni değiştiğinde tetiklenen olay işleyici.
    /// Her karakter girişinde matris çarpımı ile şifreleme güncellenir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    /// <summary>
    /// Hill şifreleme işlemini gerçekleştiren ana metod.
    /// Matris elemanlarını parse eder, 2x2 anahtar matrisini oluşturur ve
    /// metni 2'li bloklar halinde matris çarpımı ile şifreler.
    /// Matris determinantı mod 26'da tersi alınabilir olmalıdır.
    /// </summary>
    private void UpdateCipher()
    {
        // Tüm UI bileşenlerinin yüklendiğinden emin ol
        if (M00 == null || M01 == null || M10 == null || M11 == null ||
            PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Matris elemanlarını integer'a parse et
        // Hepsi geçerli sayı olmalı
        if (!int.TryParse(M00.Text, out int m00) ||
            !int.TryParse(M01.Text, out int m01) ||
            !int.TryParse(M10.Text, out int m10) ||
            !int.TryParse(M11.Text, out int m11))
            return;

        try
        {
            // 2x2 anahtar matrisini oluştur
            // [ m00  m01 ]
            // [ m10  m11 ]
            int[,] matrix = { { m00, m01 }, { m10, m11 } };

            // HillCipher nesnesi oluştur
            // Constructor içinde matris validasyonu yapılır:
            // - Determinant hesaplanır
            // - Determinant mod 26'da 0 olmamalı
            // - Determinantın mod 26'da modüler tersi alınabilir olmalı
            cipher = new HillCipher(matrix);

            // Giriş metni varsa şifreleme işlemini gerçekleştir
            if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
            {
                bool isEncrypt = ModeComboBox.SelectedIndex == 0;

                // Metni 2'li bloklara böl ve her blok için matris çarpımı uygula
                // Encrypt: C = (K × P) mod 26
                // Decrypt: P = (K⁻¹ × C) mod 26
                string result = isEncrypt
                    ? cipher.Encrypt(PlainTextInput.Text)
                    : cipher.Decrypt(PlainTextInput.Text);

                CipherTextOutput.Text = result;

                // Her bloğun matris çarpımı adımlarını göster
                AnimateSteps(isEncrypt, PlainTextInput.Text);
            }
            else
            {
                CipherTextOutput.Text = "";
                StopStepAnimation();
                displayedSteps.Clear();
            }
        }
        catch (ArgumentException)
        {
            // Geçersiz matris durumunda hata mesajı göster
            // Örnek: determinant 0 veya mod 26'da tersi alınamayan matris
            CipherTextOutput.Text = "Invalid key matrix!";
        }
    }

    /// <summary>
    /// Hill şifreleme adımlarını animasyonlu olarak görüntüler.
    /// Her 2'li blok için matris çarpımı adımları 300ms aralıklarla gösterilir.
    /// </summary>
    /// <param name="isEncrypt">Şifreleme mi şifre çözme mi yapıldığını belirtir</param>
    /// <param name="text">İşlenecek metin</param>
    private void AnimateSteps(bool isEncrypt, string text)
    {
        if (cipher == null) return;

        // Önceki animasyonu temizle
        StopStepAnimation();
        displayedSteps.Clear();

        // Matris çarpımı adımlarını al
        var steps = isEncrypt
            ? cipher.GetEncryptionSteps(text)
            : cipher.GetDecryptionSteps(text);

        if (steps.Count == 0) return;

        pendingSteps = steps;
        int currentIndex = 0;

        // Animasyon zamanlayıcısını başlat
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
        // Görsel ağaçta MainWindow'u bul
        var parent = Parent;
        while (parent != null && !(parent is MainWindow))
        {
            parent = parent is FrameworkElement fe ? fe.Parent : null;
        }

        // Ana menüye dön
        if (parent is MainWindow mainWindow)
        {
            mainWindow.NavigateToMenu();
        }
    }
}
