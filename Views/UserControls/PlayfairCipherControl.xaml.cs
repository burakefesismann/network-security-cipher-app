using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Playfair Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// Playfair şifresi, 5x5'lik bir anahtar matrisi kullanarak harfleri ikili gruplar (digraph) halinde şifreleyen
/// klasik bir şifreleme tekniğidir. Charles Wheatstone tarafından icat edilmiş, Lord Playfair tarafından popülerleştirilmiştir.
/// Anahtar matrisinde I ve J harfleri aynı hücrede birleştirilir, böylece 25 hücreye 26 harf sığdırılır.
/// </summary>
public partial class PlayfairCipherControl : UserControl
{
    /// <summary>
    /// Playfair şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// 5x5 anahtar matrisini ve digraph tabanlı şifreleme kurallarını yönetir.
    /// </summary>
    private PlayfairCipher? cipher;

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım şifreleme bilgilerini tutan koleksiyon.
    /// Her adım bir digraph'ın nasıl şifrelendiğini gösterir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni digraph adımı ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// PlayfairCipherControl sınıfının yapıcı metodu.
    /// Kontrolü başlatır, adım görüntüleme listesini bağlar ve
    /// varsayılan anahtar olarak "MONARCHY" kelimesini kullanır (klasik Playfair örneği).
    /// </summary>
    public PlayfairCipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için veri bağlamayı ayarla
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // Varsayılan anahtar olarak "MONARCHY" kelimesini kullan
        // Bu, Playfair şifresinin klasik örneğidir
        if (KeyInput != null)
            KeyInput.Text = "MONARCHY";

        // İlk şifreleme işlemini başlat ve anahtar matrisini oluştur
        UpdateCipher();
    }

    /// <summary>
    /// Şifreleme modu değiştiğinde tetiklenen olay işleyici.
    /// Encrypt/Decrypt seçimine göre arayüz etiketlerini günceller.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;

        // Seçilen moda göre etiketleri güncelle
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text" : "Plain Text";

        // Yeni modla şifreleme işlemini yeniden gerçekleştir
        UpdateCipher();
    }

    /// <summary>
    /// Anahtar girişi değiştiğinde tetiklenen olay işleyici.
    /// Yeni anahtarla 5x5 matris yeniden oluşturulur ve şifreleme güncellenir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void KeyInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    /// <summary>
    /// Giriş metni değiştiğinde tetiklenen olay işleyici.
    /// Her karakter girişinde şifreleme işlemi güncellenir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    /// <summary>
    /// Playfair şifreleme işlemini gerçekleştiren ana metod.
    /// Anahtar matrisini oluşturur, metni digraph'lara böler ve
    /// Playfair kurallarına göre (aynı satır, sütun veya köşegen) şifreler.
    /// </summary>
    private void UpdateCipher()
    {
        // Tüm UI bileşenlerinin yüklendiğinden emin ol
        if (KeyInput == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Anahtar boşsa işlem yapma
        if (string.IsNullOrWhiteSpace(KeyInput.Text))
            return;

        try
        {
            // Girilen anahtarla 5x5 Playfair matrisi oluştur
            // Matris: önce anahtar kelime (tekrarsız), sonra kalan harfler
            cipher = new PlayfairCipher(KeyInput.Text);

            // Oluşturulan anahtar matrisini görsel olarak göster
            DisplayKeyMatrix();

            // Giriş metni varsa şifreleme/çözme işlemi yap
            if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
            {
                bool isEncrypt = ModeComboBox.SelectedIndex == 0;

                // Playfair algoritmasını uygula:
                // 1. Metni digraph'lara (ikili gruplara) böl
                // 2. Her digraph için matris kurallarını uygula
                string result = isEncrypt
                    ? cipher.Encrypt(PlainTextInput.Text)
                    : cipher.Decrypt(PlainTextInput.Text);

                CipherTextOutput.Text = result;

                // Her digraph'ın nasıl şifrelendiğini adım adım göster
                AnimateSteps(isEncrypt, PlainTextInput.Text);
            }
            else
            {
                CipherTextOutput.Text = "";
                StopStepAnimation();
                displayedSteps.Clear();
            }
        }
        catch
        {
            // Geçersiz anahtar durumunda sessizce hata yakala
            // Kullanıcı yazarken geçici hatalar oluşabilir
        }
    }

    /// <summary>
    /// Playfair şifreleme adımlarını animasyonlu olarak görüntüler.
    /// Her digraph (harf çifti) şifreleme adımı 300ms aralıklarla gösterilir.
    /// </summary>
    /// <param name="isEncrypt">Şifreleme mi şifre çözme mi yapıldığını belirtir</param>
    /// <param name="text">İşlenecek metin</param>
    private void AnimateSteps(bool isEncrypt, string text)
    {
        if (cipher == null) return;

        // Önceki animasyonu temizle
        StopStepAnimation();
        displayedSteps.Clear();

        // Digraph bazlı adımları al
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
    /// 5x5 Playfair anahtar matrisini görsel olarak görüntüler.
    /// Matris, kullanıcının anahtarı anlamasına ve şifreleme sürecini takip etmesine yardımcı olur.
    /// </summary>
    private void DisplayKeyMatrix()
    {
        if (cipher == null || KeyMatrixDisplay == null) return;

        // Cipher'dan 5x5 anahtar matrisini al
        var matrix = cipher.GetKeyMatrix();
        var chars = new List<char>();

        // 2D matrisi 1D listeye dönüştür (satır satır)
        // ItemsControl grid layout ile 5x5 şeklinde görüntüleyecek
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                chars.Add(matrix[i, j]);
            }
        }

        // Listeyi ItemsControl'e bağla
        KeyMatrixDisplay.ItemsSource = chars;
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
