using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Caesar Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// Bu kontrol, kullanıcıların metin şifreleme ve şifre çözme işlemlerini gerçekleştirmesini,
/// adım adım şifreleme sürecini görselleştirmesini ve brute force saldırılarını simüle etmesini sağlar.
/// Caesar şifresi, alfabedeki her harfi sabit bir sayı kadar kaydırarak çalışan klasik bir şifreleme tekniğidir.
/// </summary>
public partial class CaesarCipherControl : UserControl
{
    /// <summary>
    /// Caesar şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// Şifreleme, şifre çözme ve brute force işlemlerini gerçekleştirir.
    /// </summary>
    private CaesarCipher cipher = new CaesarCipher();

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım şifreleme bilgilerini tutan koleksiyon.
    /// ObservableCollection kullanılarak UI otomatik olarak güncellenir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni adım ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// CaesarCipherControl sınıfının yapıcı metodu.
    /// Kontrolün başlatılması sırasında XAML bileşenlerini yükler,
    /// adım görüntüleme listesini bağlar ve ilk şifreleme işlemini tetikler.
    /// </summary>
    public CaesarCipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için ItemsControl'ün veri kaynağını ayarla
        // Bu, WPF'nin data binding mekanizmasını kullanarak ObservableCollection değişikliklerini otomatik olarak UI'a yansıtır
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // Kontrolün ilk yüklendiğinde varsayılan değerlerle şifreleme işlemini başlat
        UpdateCipher();
    }

    /// <summary>
    /// Şifreleme modu (Encrypt/Decrypt) değiştiğinde tetiklenen olay işleyici.
    /// Kullanıcı ComboBox'tan mod seçtiğinde, arayüz etiketleri güncellenir ve şifreleme yeniden yapılır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları, eski ve yeni seçilen öğeleri içerir</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Null kontrolü - kontrol henüz başlatılmamışsa işlem yapma
        if (ModeComboBox == null) return;

        // Seçilen index 0 ise şifreleme (Encrypt), 1 ise şifre çözme (Decrypt)
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;

        // Giriş alanı etiketini moda göre güncelle
        // Encrypt modunda "Plain Text" (düz metin), Decrypt modunda "Cipher Text" (şifreli metin)
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text";

        // Çıkış alanı etiketini moda göre güncelle
        // Encrypt modunda "Cipher Text" (şifreli metin), Decrypt modunda "Plain Text" (düz metin)
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text" : "Plain Text";

        // Yeni modla şifreleme işlemini yeniden gerçekleştir
        UpdateCipher();
    }

    /// <summary>
    /// Kaydırma değeri (shift key) değiştiğinde tetiklenen olay işleyici.
    /// Slider kontrolü ile kullanıcı 1-25 arası bir değer seçer ve bu değer şifreleme anahtarı olarak kullanılır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Slider kontrolü</param>
    /// <param name="e">Değer değişikliği olay argümanları, eski ve yeni değeri içerir</param>
    private void KeySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        // Anahtar değerini gösteren etiketi güncelle
        // Slider double değer döndürür, int'e çevirerek göster
        if (KeyValueText != null)
            KeyValueText.Text = $"Key: {(int)KeySlider.Value}";

        // Yeni anahtar değeriyle şifreleme işlemini yeniden gerçekleştir
        UpdateCipher();
    }

    /// <summary>
    /// Giriş metin alanı değiştiğinde tetiklenen olay işleyici.
    /// Kullanıcı her karakter yazdığında veya sildiğinde bu olay tetiklenir ve şifreleme güncellenir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Metin her değiştiğinde şifreleme işlemini yeniden gerçekleştir
        // Bu, gerçek zamanlı şifreleme deneyimi sağlar
        UpdateCipher();
    }

    /// <summary>
    /// Şifreleme işlemini gerçekleştiren ana metod.
    /// Kullanıcı girişini alır, seçilen moda göre şifreler veya şifre çözer,
    /// sonucu arayüze yazar ve adım adım animasyonu başlatır.
    /// Bu metod, metin, mod veya anahtar her değiştiğinde otomatik olarak çağrılır.
    /// </summary>
    private void UpdateCipher()
    {
        // Tüm gerekli UI bileşenlerinin yüklendiğinden emin ol
        // Eğer herhangi biri null ise, kontrol henüz tam başlatılmamış demektir
        if (PlainTextInput == null || KeySlider == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Giriş metninin boş olup olmadığını kontrol et
        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            // Slider'dan anahtar değerini al (1-25 arası)
            int key = (int)KeySlider.Value;

            // Şifreleme mi şifre çözme mi yapılacağını belirle
            bool isEncrypt = ModeComboBox.SelectedIndex == 0;

            // Seçilen moda göre uygun işlemi gerçekleştir
            // Ternary operator kullanarak kod tekrarını azalt
            string result = isEncrypt
                ? cipher.Encrypt(PlainTextInput.Text, key)
                : cipher.Decrypt(PlainTextInput.Text, key);

            // Sonucu çıkış metin kutusuna yaz
            CipherTextOutput.Text = result;

            // Adım adım şifreleme sürecini animasyonlu olarak göster
            // Bu, kullanıcının algoritmanın nasıl çalıştığını anlamasına yardımcı olur
            AnimateSteps(isEncrypt, PlainTextInput.Text, key);
        }
        else
        {
            // Giriş boşsa çıkışı temizle
            CipherTextOutput.Text = "";

            // Çalışan animasyonu durdur
            StopStepAnimation();

            // Gösterilen adımları temizle
            displayedSteps.Clear();
        }
    }

    /// <summary>
    /// Şifreleme adımlarını animasyonlu olarak görüntüleyen metod.
    /// Her adım 300ms aralıklarla ekrana eklenir, böylece kullanıcı
    /// şifreleme sürecini adım adım takip edebilir.
    /// </summary>
    /// <param name="isEncrypt">True ise şifreleme adımları, false ise şifre çözme adımları gösterilir</param>
    /// <param name="text">İşlenecek metin</param>
    /// <param name="key">Kaydırma değeri (shift key)</param>
    private void AnimateSteps(bool isEncrypt, string text, int key)
    {
        // Önceki animasyon varsa durdur
        StopStepAnimation();

        // Önceki adımları temizle
        displayedSteps.Clear();

        // Seçilen işlem türüne göre adımları al
        var steps = isEncrypt
            ? cipher.GetEncryptionSteps(text, key)
            : cipher.GetDecryptionSteps(text, key);

        // Eğer adım yoksa (örneğin boş metin), işlemi sonlandır
        if (steps.Count == 0) return;

        // Adımları geçici listeye kaydet
        pendingSteps = steps;
        int currentIndex = 0;

        // DispatcherTimer oluştur - UI thread üzerinde periyodik olarak çalışır
        stepTimer = new System.Windows.Threading.DispatcherTimer();

        // Her 300 milisaniyede bir yeni adım ekle
        stepTimer.Interval = TimeSpan.FromMilliseconds(300);

        // Timer her tetiklendiğinde çalışacak lambda fonksiyonu
        stepTimer.Tick += (s, e) =>
        {
            // Hala gösterilecek adım varsa
            if (currentIndex < pendingSteps.Count)
            {
                // Bir sonraki adımı görünür koleksiyona ekle
                // ObservableCollection olduğu için UI otomatik güncellenir
                displayedSteps.Add(pendingSteps[currentIndex]);
                currentIndex++;
            }
            else
            {
                // Tüm adımlar gösterildi, animasyonu durdur
                StopStepAnimation();
            }
        };

        // Timer'ı başlat
        stepTimer.Start();
    }

    /// <summary>
    /// Çalışan adım animasyonunu durduran yardımcı metod.
    /// Timer'ı durdurur ve ilgili kaynakları temizler.
    /// Bellek sızıntısını önlemek için timer referansını null'a çeker.
    /// </summary>
    private void StopStepAnimation()
    {
        // Timer aktif mi kontrol et
        if (stepTimer != null)
        {
            // Timer'ı durdur
            stepTimer.Stop();

            // Referansı temizle (garbage collection için)
            stepTimer = null;
        }

        // Bekleyen adımlar listesini temizle
        pendingSteps = null;
    }

    /// <summary>
    /// Brute Force saldırısını simüle eden buton tıklama olay işleyici.
    /// Tüm olası anahtarları (1-25) deneyerek şifreli metni çözmeye çalışır.
    /// Bu, Caesar şifresinin güvenlik zayıflığını gösterir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Button kontrolü</param>
    /// <param name="e">Tıklama olay argümanları</param>
    private void BruteForceButton_Click(object sender, RoutedEventArgs e)
    {
        // Gerekli UI bileşenlerinin var olduğundan emin ol
        if (PlainTextInput == null || BruteForceResults == null || ModeComboBox == null) return;

        // Şifreleme moduna göre hangi metni brute force yapacağımızı belirle
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;

        // Encrypt modundaysa çıktıyı, decrypt modundaysa girdiyi kullan
        string cipherText = isEncrypt
            ? (CipherTextOutput?.Text ?? "")
            : PlainTextInput.Text;

        // Şifreli metin boşsa kullanıcıyı uyar
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            // Moda göre uygun hata mesajını göster
            MessageBox.Show(isEncrypt
                ? "Please encrypt some text first!"
                : "Please enter cipher text to brute force!",
                "Brute Force",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // Tüm olası anahtarlarla (1-25) şifre çözme denemesi yap
        var results = cipher.BruteForce(cipherText);

        // Sonuçları formatla ve ListBox'a bağla
        // Her satırda anahtar numarası ve sonuç metni gösterilir
        // LINQ Select kullanarak index bilgisini de ekleriz
        BruteForceResults.ItemsSource = results.Select((text, index) => $"Key {index + 1,2}: {text}");
    }

    /// <summary>
    /// Ana menüye geri dönüş butonu tıklama olay işleyici.
    /// WPF görsel ağacında yukarı doğru gezinerek MainWindow'u bulur
    /// ve NavigateToMenu metodunu çağırarak ana menü ekranına geçiş yapar.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Button kontrolü</param>
    /// <param name="e">Tıklama olay argümanları</param>
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Görsel ağaçta (visual tree) yukarı doğru gezinme başlat
        var parent = Parent;

        // MainWindow'a ulaşana kadar parent'ları kontrol et
        while (parent != null && !(parent is MainWindow))
        {
            // FrameworkElement ise Parent özelliğini kullan
            parent = parent is FrameworkElement fe ? fe.Parent : null;
        }

        // MainWindow bulunduysa menüye dön
        if (parent is MainWindow mainWindow)
        {
            // MainWindow'daki NavigateToMenu metodunu çağır
            // Bu metod ContentControl'ü MenuControl ile değiştirir
            mainWindow.NavigateToMenu();
        }
    }
}
