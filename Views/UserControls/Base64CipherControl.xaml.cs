using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Base64 Kodlama algoritması için WPF kullanıcı kontrol sınıfı.
/// Base64, binary verileri ASCII metin formatına dönüştüren bir kodlama sistemidir.
/// Şifreleme değildir - sadece encoding/decoding işlemidir (güvenlik sağlamaz).
/// 8-bit binary veriyi 6-bit gruplara böler ve 64 karakterlik bir alfabe kullanarak temsil eder.
/// E-posta ekleri, URL'ler ve veri aktarımında yaygın olarak kullanılır.
/// </summary>
public partial class Base64CipherControl : UserControl
{
    /// <summary>
    /// Base64 kodlama algoritmasının mantığını içeren servis nesnesi.
    /// Binary-to-text encoding işlemlerini gerçekleştirir.
    /// </summary>
    private Base64Cipher cipher = new Base64Cipher();

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım kodlama bilgilerini tutan koleksiyon.
    /// Her adım byte'ların nasıl 6-bit gruplara bölündüğünü ve Base64 karakterlerine dönüştüğünü gösterir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni kodlama adımı ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// Base64CipherControl sınıfının yapıcı metodu.
    /// Kontrolü başlatır, adım görüntüleme listesini bağlar ve
    /// ilk kodlama işlemini tetikler.
    /// </summary>
    public Base64CipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için veri bağlamayı ayarla
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // İlk kodlama işlemini başlat
        UpdateCipher();
    }

    /// <summary>
    /// Kodlama modu (Encode/Decode) değiştiğinde tetiklenen olay işleyici.
    /// Base64 şifreleme değil kodlamadır, bu nedenle "Encrypt/Decrypt" yerine "Encode/Decode" terimleri kullanılır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen ComboBox kontrolü</param>
    /// <param name="e">Seçim değişikliği olay argümanları</param>
    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;

        // Etiketleri güncelle
        // Encode: düz metin -> Base64
        // Decode: Base64 -> düz metin
        bool isEncode = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncode ? "Plain Text" : "Base64 Encoded";
        if (OutputLabel != null)
            OutputLabel.Text = isEncode ? "Base64 Encoded" : "Plain Text";

        UpdateCipher();
    }

    /// <summary>
    /// Giriş metni değiştiğinde tetiklenen olay işleyici.
    /// Her karakter girişinde kodlama işlemi güncellenir.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen TextBox kontrolü</param>
    /// <param name="e">Metin değişikliği olay argümanları</param>
    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    /// <summary>
    /// Base64 kodlama işlemini gerçekleştiren ana metod.
    /// Encode: Her 3 byte (24 bit) 4 Base64 karakterine (her biri 6 bit) dönüştürülür.
    /// Decode: Her 4 Base64 karakteri 3 byte'a geri dönüştürülür.
    /// Padding: Eğer byte sayısı 3'ün katı değilse, '=' karakteri ile tamamlanır.
    /// </summary>
    private void UpdateCipher()
    {
        // UI bileşenlerinin hazır olduğundan emin ol
        if (PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        // Giriş metni varsa kodlama işlemini gerçekleştir
        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            bool isEncode = ModeComboBox.SelectedIndex == 0;

            // Base64 kodlama/çözme işlemi
            // Encode: metin -> byte array -> 6-bit gruplar -> Base64 karakterler
            // Decode: Base64 karakterler -> 6-bit gruplar -> byte array -> metin
            string result = isEncode
                ? cipher.Encode(PlainTextInput.Text)
                : cipher.Decode(PlainTextInput.Text);

            CipherTextOutput.Text = result;

            // Her adımı göster: byte'lar, 6-bit gruplar, Base64 karakterler
            AnimateSteps(isEncode, PlainTextInput.Text);
        }
        else
        {
            CipherTextOutput.Text = "";
            StopStepAnimation();
            displayedSteps.Clear();
        }
    }

    /// <summary>
    /// Base64 kodlama adımlarını animasyonlu olarak görüntüler.
    /// Her 3 byte'lık grubun 4 Base64 karakterine dönüşümü 300ms aralıklarla gösterilir.
    /// </summary>
    /// <param name="isEncode">Encode mi decode mi yapıldığını belirtir</param>
    /// <param name="text">İşlenecek metin</param>
    private void AnimateSteps(bool isEncode, string text)
    {
        StopStepAnimation();
        displayedSteps.Clear();

        // Base64 dönüşüm adımlarını al
        var steps = isEncode
            ? cipher.GetEncodingSteps(text)
            : cipher.GetDecodingSteps(text);

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

