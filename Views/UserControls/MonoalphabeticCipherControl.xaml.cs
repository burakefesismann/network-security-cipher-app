using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Monoalfabetik (Tek Alfabeli) Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// Bu şifreleme yönteminde, her harf alfabede farklı bir harfle değiştirilir (substitution cipher).
/// 26 harflik alfabe için 26! (yaklaşık 4x10^26) farklı anahtar kombinasyonu mümkündür.
/// Kullanıcı rastgele anahtar üretebilir veya kendi anahtarını girebilir.
/// </summary>
public partial class MonoalphabeticCipherControl : UserControl
{
    /// <summary>
    /// Monoalfabetik şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// Null olabilir çünkü geçerli anahtar girilene kadar başlatılmaz.
    /// </summary>
    private MonoalphabeticCipher? cipher;

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
    /// MonoalphabeticCipherControl sınıfının yapıcı metodu.
    /// Kontrolün başlatılması sırasında XAML bileşenlerini yükler,
    /// adım görüntüleme listesini bağlar ve otomatik olarak rastgele bir anahtar üretir.
    /// </summary>
    public MonoalphabeticCipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için ItemsControl'ün veri kaynağını ayarla
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // Kontrolün ilk yüklendiğinde otomatik olarak rastgele anahtar üret
        // null! kullanımı: sender ve e parametreleri bu çağrıda kullanılmadığı için
        GenerateKeyButton_Click(null!, null!);
    }

    /// <summary>
    /// Şifreleme modu (Encrypt/Decrypt) değiştiğinde tetiklenen olay işleyici.
    /// Kullanıcı ComboBox'tan mod seçtiğinde, arayüz etiketleri güncellenir ve şifreleme yeniden yapılır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Null kontrolü - kontrol henüz başlatılmamışsa işlem yapma
        if (ModeComboBox == null) return;

        // Seçilen index 0 ise şifreleme (Encrypt), 1 ise şifre çözme (Decrypt)
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;

        // Giriş ve çıkış alanı etiketlerini moda göre güncelle
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text" : "Plain Text";

        // Yeni modla şifreleme işlemini yeniden gerçekleştir
        UpdateCipher();
    }

    /// <summary>
    /// Anahtar metin kutusu değiştiğinde tetiklenen olay işleyici.
    /// Kullanıcı anahtar girerken veya değiştirirken gerçek zamanlı şifreleme yapılır.
    /// Anahtar 26 benzersiz harf içermelidir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void KeyInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Anahtar her değiştiğinde şifreleme işlemini yeniden gerçekleştir
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
        UpdateCipher();
    }

    /// <summary>
    /// Rastgele anahtar üretme butonu tıklama olay işleyici.
    /// MonoalphabeticCipher sınıfının statik metodunu kullanarak
    /// alfabedeki 26 harfin rastgele bir permütasyonunu oluşturur.
    /// Bu, kriptografik olarak güvenli rastgele sayı üreteci kullanır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Button kontrolü</param>
    /// <param name="e">Tıklama olay argümanları</param>
    private void GenerateKeyButton_Click(object sender, RoutedEventArgs e)
    {
        // Anahtar metin kutusuna rastgele üretilen 26 harflik permütasyonu yaz
        if (KeyInput != null)
            KeyInput.Text = MonoalphabeticCipher.GenerateRandomKey();
    }

    /// <summary>
    /// Şifreleme işlemini gerçekleştiren ana metod.
    /// Girilen anahtarı doğrular, geçerliyse cipher nesnesini oluşturur ve
    /// kullanıcı girişini şifreler veya şifresini çözer.
    /// </summary>
    private void UpdateCipher()
    {
        // Tüm gerekli UI bileşenlerinin yüklendiğinden emin ol
        if (KeyInput == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Anahtar boşsa işlemi sonlandır
        if (string.IsNullOrWhiteSpace(KeyInput.Text))
            return;

        try
        {
            // Girilen anahtarla yeni cipher nesnesi oluştur
            // Constructor içinde anahtar validasyonu yapılır:
            // - Tam 26 karakter olmalı
            // - Sadece A-Z harfleri içermeli
            // - Her harf bir kez kullanılmalı (tekrar olmamalı)
            cipher = new MonoalphabeticCipher(KeyInput.Text);

            // Giriş metni varsa şifreleme/çözme işlemi yap
            if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
            {
                // Modu belirle
                bool isEncrypt = ModeComboBox.SelectedIndex == 0;

                // Seçilen moda göre işlemi gerçekleştir
                string result = isEncrypt
                    ? cipher.Encrypt(PlainTextInput.Text)
                    : cipher.Decrypt(PlainTextInput.Text);

                // Sonucu çıkış alanına yaz
                CipherTextOutput.Text = result;

                // Adım adım şifreleme sürecini animasyonlu olarak göster
                AnimateSteps(isEncrypt, PlainTextInput.Text);
            }
            else
            {
                // Giriş boşsa çıkışı temizle
                CipherTextOutput.Text = "";
                StopStepAnimation();
                displayedSteps.Clear();
            }
        }
        catch (ArgumentException ex)
        {
            // Anahtar validasyon hatası durumunda kullanıcıyı bilgilendir
            // Örnek hatalar: eksik karakter, tekrarlı harf, geçersiz karakter
            MessageBox.Show(ex.Message, "Invalid Key",
                           MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Şifreleme adımlarını animasyonlu olarak görüntüleyen metod.
    /// Her adım 300ms aralıklarla ekrana eklenir, böylece kullanıcı
    /// her harfin hangi harfe dönüştüğünü adım adım takip edebilir.
    /// </summary>
    /// <param name="isEncrypt">True ise şifreleme adımları, false ise şifre çözme adımları gösterilir</param>
    /// <param name="text">İşlenecek metin</param>
    private void AnimateSteps(bool isEncrypt, string text)
    {
        // Cipher nesnesi yoksa (geçersiz anahtar durumu) işlem yapma
        if (cipher == null) return;

        // Önceki animasyon ve adımları temizle
        StopStepAnimation();
        displayedSteps.Clear();

        // Seçilen işlem türüne göre adımları al
        var steps = isEncrypt
            ? cipher.GetEncryptionSteps(text)
            : cipher.GetDecryptionSteps(text);

        // Adım yoksa işlemi sonlandır
        if (steps.Count == 0) return;

        // Adımları geçici listeye kaydet
        pendingSteps = steps;
        int currentIndex = 0;

        // DispatcherTimer oluştur - UI thread üzerinde çalışır
        stepTimer = new System.Windows.Threading.DispatcherTimer();
        stepTimer.Interval = TimeSpan.FromMilliseconds(300);

        // Timer her tetiklendiğinde bir sonraki adımı ekle
        stepTimer.Tick += (s, e) =>
        {
            if (currentIndex < pendingSteps.Count)
            {
                // ObservableCollection'a ekleme UI'ı otomatik günceller
                displayedSteps.Add(pendingSteps[currentIndex]);
                currentIndex++;
            }
            else
            {
                // Tüm adımlar gösterildiyse animasyonu durdur
                StopStepAnimation();
            }
        };

        // Animasyonu başlat
        stepTimer.Start();
    }

    /// <summary>
    /// Çalışan adım animasyonunu durduran yardımcı metod.
    /// Timer'ı durdurur ve kaynakları temizler.
    /// </summary>
    private void StopStepAnimation()
    {
        // Timer aktif mi kontrol et
        if (stepTimer != null)
        {
            stepTimer.Stop();
            stepTimer = null;
        }
        pendingSteps = null;
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
        // Görsel ağaçta yukarı doğru gezinme
        var parent = Parent;
        while (parent != null && !(parent is MainWindow))
        {
            parent = parent is FrameworkElement fe ? fe.Parent : null;
        }

        // MainWindow bulunduysa menüye dön
        if (parent is MainWindow mainWindow)
        {
            mainWindow.NavigateToMenu();
        }
    }
}
