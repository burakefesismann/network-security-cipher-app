using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

/// <summary>
/// Transposition (Yer Değiştirme) Şifreleme algoritması için WPF kullanıcı kontrol sınıfı.
/// Transposition şifresi, harflerin yerini değiştirerek şifreleme yapar (karakterleri değiştirmez, sadece sıralarını değiştirir).
/// Bu implementasyon sütunlu transposition kullanır: metin belirlenen sayıda sütuna yazılır ve sütun sütun okunur.
/// Substitution şifrelerinden (Caesar, Vigenère) farklıdır çünkü karakterleri değiştirmez, sadece yeniden düzenler.
/// </summary>
public partial class TranspositionCipherControl : UserControl
{
    /// <summary>
    /// Transposition şifreleme algoritmasının mantığını içeren servis nesnesi.
    /// Sütun sayısını ve matris düzenlemesini yönetir.
    /// </summary>
    private TranspositionCipher? cipher;

    /// <summary>
    /// Kullanıcı arayüzünde görüntülenen adım adım şifreleme bilgilerini tutan koleksiyon.
    /// Her adım metnin matrise nasıl yerleştirildiğini ve okunduğunu gösterir.
    /// </summary>
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();

    /// <summary>
    /// Adım adım animasyonları kontrol eden zamanlayıcı.
    /// Her 300 milisaniyede bir yeni transposition adımı ekler.
    /// </summary>
    private System.Windows.Threading.DispatcherTimer? stepTimer;

    /// <summary>
    /// Gösterilmeyi bekleyen adımların listesi.
    /// Animasyon sırasında sırayla displayedSteps koleksiyonuna aktarılır.
    /// </summary>
    private List<StepInfo>? pendingSteps;

    /// <summary>
    /// TranspositionCipherControl sınıfının yapıcı metodu.
    /// Kontrolü başlatır, adım görüntüleme listesini bağlar ve
    /// varsayılan sütun sayısı ile ilk şifreleme işlemini tetikler.
    /// </summary>
    public TranspositionCipherControl()
    {
        // XAML'de tanımlanan UI bileşenlerini başlat
        InitializeComponent();

        // Adım adım gösterim için veri bağlamayı ayarla
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;

        // Varsayılan sütun sayısı ile güncellemeyi tetikle
        KeySlider_ValueChanged(null!, null!);
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
    /// Sütun sayısı slider'ı değiştiğinde tetiklenen olay işleyici.
    /// Sütun sayısı transposition şifresinin anahtarıdır (metin kaç sütuna bölünecek).
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Slider kontrolü</param>
    /// <param name="e">Değer değişikliği olay argümanları</param>
    private void KeySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        // Sütun sayısını gösteren etiketi güncelle
        if (KeyValueText != null && KeySlider != null)
            KeyValueText.Text = $"Columns: {(int)KeySlider.Value}";

        // Yeni sütun sayısı ile şifreleme işlemini güncelle
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
    /// Transposition şifreleme işlemini gerçekleştiren ana metod.
    /// Metni belirlenen sayıda sütuna yerleştirir ve sütun sütun okuyarak şifreler.
    /// Encrypt: Satır satır yaz, sütun sütun oku.
    /// Decrypt: Sütun sütun yaz, satır satır oku.
    /// </summary>
    private void UpdateCipher()
    {
        // UI bileşenlerinin hazır olduğundan emin ol
        if (KeySlider == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        try
        {
            // Sütun sayısını al (anahtar)
            int key = (int)KeySlider.Value;

            // TranspositionCipher nesnesi oluştur
            cipher = new TranspositionCipher(key);

            // Giriş metni varsa işlemi gerçekleştir
            if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
            {
                bool isEncrypt = ModeComboBox.SelectedIndex == 0;

                // Transposition işlemini uygula
                string result = isEncrypt
                    ? cipher.Encrypt(PlainTextInput.Text)
                    : cipher.Decrypt(PlainTextInput.Text);

                CipherTextOutput.Text = result;

                // Matris görselleştirmesini göster
                // Encrypt: satır satır yazılmış hali
                // Decrypt: sütun sütun yazılmış hali
                if (isEncrypt)
                    DisplayMatrix(PlainTextInput.Text, key);
                else
                    DisplayMatrixDecrypt(PlainTextInput.Text, key);

                // Adım adım süreci göster
                AnimateSteps(isEncrypt, PlainTextInput.Text);
            }
            else
            {
                // Giriş boşsa her şeyi temizle
                CipherTextOutput.Text = "";
                if (MatrixDisplay != null)
                    MatrixDisplay.ItemsSource = null;
                StopStepAnimation();
                displayedSteps.Clear();
            }
        }
        catch
        {
            // Geçersiz anahtar (sütun sayısı) durumunda sessizce yakala
        }
    }

    /// <summary>
    /// Transposition şifreleme adımlarını animasyonlu olarak görüntüler.
    /// </summary>
    /// <param name="isEncrypt">Şifreleme mi şifre çözme mi yapıldığını belirtir</param>
    /// <param name="text">İşlenecek metin</param>
    private void AnimateSteps(bool isEncrypt, string text)
    {
        if (cipher == null) return;

        StopStepAnimation();
        displayedSteps.Clear();

        // Transposition adımlarını al
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
    /// Şifreleme (Encrypt) modu için matris görselleştirmesi.
    /// Metni satır satır yerleştirir (soldan sağa, yukarıdan aşağıya).
    /// Okuma ise sütun sütun yapılır (yukarıdan aşağıya, soldan sağa).
    /// </summary>
    /// <param name="text">Görselleştirilecek metin</param>
    /// <param name="cols">Sütun sayısı</param>
    private void DisplayMatrix(string text, int cols)
    {
        if (MatrixDisplay == null) return;

        // Metni büyük harfe çevir ve boşlukları kaldır
        text = text.ToUpper().Replace(" ", "");

        // Satır sayısını hesapla (yukarı yuvarlama ile)
        int rows = (int)Math.Ceiling((double)text.Length / cols);

        var chars = new List<char>();

        // Metni satır satır matrise yerleştir
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                // Metin biterse boşluk ekle
                chars.Add(index < text.Length ? text[index] : ' ');
            }
        }

        // Listeyi ItemsControl'e bağla (XAML'de grid düzeninde gösterilir)
        MatrixDisplay.ItemsSource = chars;
    }

    /// <summary>
    /// Şifre çözme (Decrypt) modu için matris görselleştirmesi.
    /// Metni sütun sütun yerleştirir (yukarıdan aşağıya, soldan sağa).
    /// Okuma ise satır satır yapılır (soldan sağa, yukarıdan aşağıya).
    /// </summary>
    /// <param name="text">Görselleştirilecek metin</param>
    /// <param name="cols">Sütun sayısı</param>
    private void DisplayMatrixDecrypt(string text, int cols)
    {
        if (MatrixDisplay == null) return;

        // Metni büyük harfe çevir ve boşlukları kaldır
        text = text.ToUpper().Replace(" ", "");

        // Satır sayısını hesapla
        int rows = (int)Math.Ceiling((double)text.Length / cols);

        var chars = new List<char>();

        // Metni sütun sütun matrise yerleştir
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                int index = col * rows + row;
                // Metin biterse boşluk ekle
                chars.Add(index < text.Length ? text[index] : ' ');
            }
        }

        // Listeyi ItemsControl'e bağla
        MatrixDisplay.ItemsSource = chars;
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
