using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SecurityProject.Views;
using SecurityProject.Views.UserControls;
using SecurityProject.Services;

namespace SecurityProject;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// MainWindow sınıfının yapıcı metodu.
    /// Uygulama açıldığında ana pencereyi başlatır ve gerekli olay işleyicilerini bağlar.
    /// </summary>
    public MainWindow()
    {
        // XAML'de tanımlanan UI bileşenlerini yükle
        InitializeComponent();
        
        // Pencere yüklendiğinde çalışacak olay işleyicisini bağla
        // Bu, kartların animasyonlu yüklenmesini sağlar
        Loaded += MainWindow_Loaded;
        
        // Klavye tuşlarına basıldığında çalışacak olay işleyicisini bağla
        // ESC tuşu ile menüye dönüş özelliği için
        KeyDown += MainWindow_KeyDown;

        // Uygulamayı tam ekran modunda başlat
        // Kullanıcı deneyimini iyileştirmek için
        WindowState = WindowState.Maximized;
    }

    /// <summary>
    /// Klavye tuşlarına basıldığında tetiklenen olay işleyici.
    /// ESC tuşuna basıldığında ve içerik alanı görünürse ana menüye döner.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen kontrol (MainWindow)</param>
    /// <param name="e">Basılan tuş bilgilerini içeren KeyEventArgs</param>
    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        // ESC tuşuna basıldıysa ve şu anda bir şifreleme sayfası görüntüleniyorsa
        // Ana menüye geri dön
        if (e.Key == Key.Escape && ContentArea.Visibility == Visibility.Visible)
        {
            NavigateToMenu();
        }
    }

    /// <summary>
    /// Pencere yüklendiğinde tetiklenen olay işleyici.
    /// Ana menü kartlarının animasyonlu görünmesini sağlar.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen kontrol (MainWindow)</param>
    /// <param name="e">Yükleme olay argümanları</param>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Pencere yüklendiğinde kartları animasyonlu olarak göster
        // Bu, kullanıcı deneyimini iyileştirir ve görsel olarak çekici bir başlangıç sağlar
        AnimateCards();
    }

    /// <summary>
    /// Ana menüdeki şifreleme kartlarını animasyonlu olarak gösterir.
    /// Her kart sırayla (100ms gecikmeyle) fade-in ve slide-up animasyonu ile görünür hale gelir.
    /// Bu, kullanıcıya görsel olarak çekici bir başlangıç deneyimi sunar.
    /// </summary>
    private void AnimateCards()
    {
        // Tüm şifreleme kartlarını bir diziye topla
        // Bu kartlar ana menüde gösterilen şifreleme yöntemlerini temsil eder
        var cards = new Border[] { CaesarCard, MonoalphabeticCard, PlayfairCard, HillCard, VigenereCard, TranspositionCard, XORCard, Base64Card };
        
        // Her kart için başlangıç durumunu ayarla
        foreach (var card in cards)
        {
            // Kartı başlangıçta görünmez yap (opacity = 0)
            card.Opacity = 0;
            
            // Animasyon için gerekli transform'ları hazırla
            // TranslateTransform: Y ekseninde 30 piksel aşağıdan başlat (slide-up efekti için)
            var translateTransform = new TranslateTransform { Y = 30 };
            // ScaleTransform: Normal boyutta tut (scale animasyonu için hazır)
            var scaleTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            
            // Her iki transform'u da bir TransformGroup içinde birleştir
            // Bu, kart üzerinde hem kaydırma hem de ölçeklendirme animasyonları yapılmasına izin verir
            card.RenderTransform = new TransformGroup
            {
                Children = new System.Windows.Media.TransformCollection { translateTransform, scaleTransform }
            };
        }

        // Her kart için sıralı animasyon başlat
        // Her kart bir öncekinden 100ms sonra başlar (kademeli görünme efekti)
        for (int i = 0; i < cards.Length; i++)
        {
            // Lambda ifadesinde kullanmak için indeksi yerel değişkene kopyala
            // Bu, closure probleminden kaçınmak için gereklidir
            int index = i;
            
            // Her kart için ayrı bir timer oluştur
            var timer = new System.Windows.Threading.DispatcherTimer();
            
            // Timer'ın ne zaman tetikleneceğini belirle
            // Her kart bir öncekinden 100ms sonra başlar (i * 100ms gecikme)
            timer.Interval = TimeSpan.FromMilliseconds(100 * i);
            
            // Timer tetiklendiğinde çalışacak olay işleyici
            timer.Tick += (s, e) =>
            {
                // Timer'ı durdur (sadece bir kez çalışsın)
                timer.Stop();
                
                // İlgili kartı animasyonlu olarak göster
                AnimateCardIn(cards[index]);
            };
            
            // Timer'ı başlat
            timer.Start();
        }
    }

    /// <summary>
    /// Belirli bir kartı animasyonlu olarak gösterir.
    /// Kart fade-in (opacity 0'dan 1'e) ve slide-up (Y: 30'dan 0'a) animasyonları ile görünür hale gelir.
    /// </summary>
    /// <param name="card">Animasyonlanacak Border kontrolü (şifreleme kartı)</param>
    private void AnimateCardIn(Border card)
    {
        // Fade-in animasyonu: Opacity 0'dan 1'e 500ms içinde geçiş
        // Kart yavaşça görünür hale gelir
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
        
        // Slide-up animasyonu: Y pozisyonu 30 pikselden 0'a 500ms içinde geçiş
        // Kart aşağıdan yukarı doğru kayarak gelir
        var slideIn = new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(500));
        
        // Opacity özelliğine fade-in animasyonunu uygula
        card.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        
        // Kartın RenderTransform'unu TransformGroup olarak al
        // Daha önce AnimateCards metodunda bu transform'lar ayarlanmıştı
        var transformGroup = card.RenderTransform as TransformGroup;
        
        // TransformGroup geçerliyse ve içinde TranslateTransform varsa
        if (transformGroup?.Children.Count > 0 && transformGroup.Children[0] is TranslateTransform translateTransform)
        {
            // Y pozisyonuna slide-up animasyonunu uygula
            translateTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);
        }
    }

    /// <summary>
    /// Kapatma butonu tıklama olay işleyici.
    /// Uygulamayı kapatır.
    /// </summary>
    /// <param name="sender">Olayı tetikleyen Button kontrolü</param>
    /// <param name="e">Tıklama olay argümanları</param>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // Pencereyi kapat ve uygulamayı sonlandır
        this.Close();
    }

    /// <summary>
    /// Belirli bir kullanıcı kontrolünü (şifreleme sayfası) içerik alanına yükler.
    /// Ana menüyü gizler ve yeni sayfayı fade-in animasyonu ile gösterir.
    /// </summary>
    /// <param name="page">Gösterilecek UserControl (şifreleme kontrolü)</param>
    private void NavigateToPage(UserControl page)
    {
        // İçerik alanı kontrolü yoksa işlem yapma
        if (ContentArea == null) return;
        
        // Ana menüyü gizle (görünmez yap)
        MainMenuGrid.Visibility = Visibility.Collapsed;
        
        // İçerik alanına yeni sayfayı yükle
        ContentArea.Content = page;
        
        // İçerik alanını görünür yap
        ContentArea.Visibility = Visibility.Visible;
        
        // Yumuşak bir geçiş için fade-in animasyonu uygula
        // Opacity 0'dan 1'e 300ms içinde geçiş yapar
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        ContentArea.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }
    
    /// <summary>
    /// Ana menüye geri döner.
    /// İçerik alanını gizler ve ana menüyü fade-in animasyonu ile gösterir.
    /// ESC tuşu veya geri butonu ile çağrılabilir.
    /// </summary>
    public void NavigateToMenu()
    {
        // Gerekli kontroller yoksa işlem yapma
        if (ContentArea == null || MainMenuGrid == null) return;
        
        // İçerik alanını gizle (şifreleme sayfası kapanır)
        ContentArea.Visibility = Visibility.Collapsed;
        
        // Ana menüyü görünür yap
        MainMenuGrid.Visibility = Visibility.Visible;
        
        // Yumuşak bir geçiş için fade-in animasyonu uygula
        // Ana menü 0'dan 1'e opacity ile 300ms içinde görünür hale gelir
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        MainMenuGrid.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }

    /// <summary>
    /// Caesar şifreleme butonu tıklama olay işleyici.
    /// Caesar şifreleme sayfasına yönlendirir.
    /// </summary>
    private void CaesarButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new CaesarCipherControl());
    }

    /// <summary>
    /// Monoalfabetik şifreleme butonu tıklama olay işleyici.
    /// Monoalfabetik şifreleme sayfasına yönlendirir.
    /// </summary>
    private void MonoalphabeticButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new MonoalphabeticCipherControl());
    }

    /// <summary>
    /// Playfair şifreleme butonu tıklama olay işleyici.
    /// Playfair şifreleme sayfasına yönlendirir.
    /// </summary>
    private void PlayfairButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new PlayfairCipherControl());
    }

    /// <summary>
    /// Hill şifreleme butonu tıklama olay işleyici.
    /// Hill şifreleme sayfasına yönlendirir.
    /// </summary>
    private void HillButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new HillCipherControl());
    }

    /// <summary>
    /// Vigenere şifreleme butonu tıklama olay işleyici.
    /// Vigenere şifreleme sayfasına yönlendirir.
    /// </summary>
    private void VigenereButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new VigenereCipherControl());
    }

    /// <summary>
    /// Transpozisyon şifreleme butonu tıklama olay işleyici.
    /// Transpozisyon şifreleme sayfasına yönlendirir.
    /// </summary>
    private void TranspositionButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new TranspositionCipherControl());
    }

    /// <summary>
    /// XOR şifreleme butonu tıklama olay işleyici.
    /// XOR şifreleme sayfasına yönlendirir.
    /// </summary>
    private void XORButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new XORCipherControl());
    }

    /// <summary>
    /// Base64 kodlama butonu tıklama olay işleyici.
    /// Base64 kodlama sayfasına yönlendirir.
    /// </summary>
    private void Base64Button_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new Base64CipherControl());
    }

    /// <summary>
    /// Caesar şifreleme kartına tıklama olay işleyici.
    /// Kartın herhangi bir yerine tıklandığında (buton hariç) ilgili sayfaya yönlendirir.
    /// Kart tıklama animasyonu da uygulanır.
    /// </summary>
    private void CaesarCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Eğer tıklanan öğe bir buton ise bu olayı görmezden gel
        // Butonların kendi tıklama olayları var ve çift tetiklenmeyi önlemek için
        if (e.Source is Button) return;
        
        // Kart tıklama animasyonunu başlat (geri çekme efekti)
        AnimateCardClick(sender);
        
        // İlgili şifreleme sayfasına yönlendir
        NavigateToPage(new CaesarCipherControl());
    }
    
    /// <summary>
    /// Monoalfabetik şifreleme kartına tıklama olay işleyici.
    /// </summary>
    private void MonoalphabeticCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new MonoalphabeticCipherControl());
    }
    
    /// <summary>
    /// Playfair şifreleme kartına tıklama olay işleyici.
    /// </summary>
    private void PlayfairCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new PlayfairCipherControl());
    }
    
    /// <summary>
    /// Hill şifreleme kartına tıklama olay işleyici.
    /// </summary>
    private void HillCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new HillCipherControl());
    }
    
    /// <summary>
    /// Vigenere şifreleme kartına tıklama olay işleyici.
    /// </summary>
    private void VigenereCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new VigenereCipherControl());
    }
    
    /// <summary>
    /// Transpozisyon şifreleme kartına tıklama olay işleyici.
    /// </summary>
    private void TranspositionCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new TranspositionCipherControl());
    }
    
    /// <summary>
    /// XOR şifreleme kartına tıklama olay işleyici.
    /// </summary>
    private void XORCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new XORCipherControl());
    }
    
    /// <summary>
    /// Base64 kodlama kartına tıklama olay işleyici.
    /// </summary>
    private void Base64Card_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new Base64CipherControl());
    }
    
    /// <summary>
    /// Kart tıklama animasyonunu uygular.
    /// Kart tıklandığında hafifçe küçülüp (0.95) tekrar normal boyuta (1.0) döner.
    /// Bu, kullanıcıya görsel geri bildirim sağlar.
    /// </summary>
    /// <param name="sender">Tıklanan Border kontrolü (şifreleme kartı)</param>
    private void AnimateCardClick(object sender)
    {
        // Gönderilen nesnenin Border olduğunu kontrol et
        if (sender is Border card)
        {
            // Scale animasyonu: 0.95'ten 1.0'a 100ms içinde geçiş
            // Kart hafifçe küçülüp tekrar normal boyuta döner (geri çekme efekti)
            var scaleAnimation = new DoubleAnimation(0.95, 1, TimeSpan.FromMilliseconds(100));
            
            // Kartın RenderTransform'unu TransformGroup olarak al
            // Daha önce AnimateCards metodunda bu transform'lar ayarlanmıştı
            var transformGroup = card.RenderTransform as TransformGroup;
            
            // TransformGroup geçerliyse ve içinde ScaleTransform varsa (2. eleman)
            if (transformGroup?.Children.Count > 1 && transformGroup.Children[1] is ScaleTransform scaleTransform)
            {
                // X ve Y eksenlerinde scale animasyonunu uygula
                // Bu, kartın hem genişliğini hem yüksekliğini eşit oranda değiştirir
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
            }
        }
    }
}