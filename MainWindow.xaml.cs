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
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        KeyDown += MainWindow_KeyDown;

        // Tam ekran yap
        WindowState = WindowState.Maximized;
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && ContentArea.Visibility == Visibility.Visible)
        {
            NavigateToMenu();
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Animate cards on load
        AnimateCards();
    }

    private void AnimateCards()
    {
        var cards = new Border[] { CaesarCard, MonoalphabeticCard, PlayfairCard, HillCard, VigenereCard, TranspositionCard, XORCard, Base64Card };
        
        foreach (var card in cards)
        {
            card.Opacity = 0;
            // Setup both transforms
            var translateTransform = new TranslateTransform { Y = 30 };
            var scaleTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            card.RenderTransform = new TransformGroup
            {
                Children = new System.Windows.Media.TransformCollection { translateTransform, scaleTransform }
            };
        }

        for (int i = 0; i < cards.Length; i++)
        {
            int index = i;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100 * i);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                AnimateCardIn(cards[index]);
            };
            timer.Start();
        }
    }

    private void AnimateCardIn(Border card)
    {
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
        var slideIn = new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(500));
        
        card.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        var transformGroup = card.RenderTransform as TransformGroup;
        if (transformGroup?.Children.Count > 0 && transformGroup.Children[0] is TranslateTransform translateTransform)
        {
            translateTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void NavigateToPage(UserControl page)
    {
        if (ContentArea == null) return;
        
        // Menüyü gizle, ContentArea'yi göster
        MainMenuGrid.Visibility = Visibility.Collapsed;
        ContentArea.Content = page;
        ContentArea.Visibility = Visibility.Visible;
        
        // Fade animasyonu
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        ContentArea.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }
    
    public void NavigateToMenu()
    {
        if (ContentArea == null || MainMenuGrid == null) return;
        
        // ContentArea'yi gizle, menüyü göster
        ContentArea.Visibility = Visibility.Collapsed;
        MainMenuGrid.Visibility = Visibility.Visible;
        
        // Fade animasyonu
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        MainMenuGrid.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }

    private void CaesarButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new CaesarCipherControl());
    }

    private void MonoalphabeticButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new MonoalphabeticCipherControl());
    }

    private void PlayfairButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new PlayfairCipherControl());
    }

    private void HillButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new HillCipherControl());
    }

    private void VigenereButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new VigenereCipherControl());
    }

    private void TranspositionButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new TranspositionCipherControl());
    }

    private void XORButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new XORCipherControl());
    }

    private void Base64Button_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage(new Base64CipherControl());
    }

    // Card MouseDown handlers - tüm karta tıklama için
    private void CaesarCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return; // Button tıklamasını ignore et
        AnimateCardClick(sender);
        NavigateToPage(new CaesarCipherControl());
    }
    
    private void MonoalphabeticCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new MonoalphabeticCipherControl());
    }
    
    private void PlayfairCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new PlayfairCipherControl());
    }
    
    private void HillCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new HillCipherControl());
    }
    
    private void VigenereCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new VigenereCipherControl());
    }
    
    private void TranspositionCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new TranspositionCipherControl());
    }
    
    private void XORCard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new XORCipherControl());
    }
    
    private void Base64Card_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button) return;
        AnimateCardClick(sender);
        NavigateToPage(new Base64CipherControl());
    }
    
    private void AnimateCardClick(object sender)
    {
        if (sender is Border card)
        {
            var scaleAnimation = new DoubleAnimation(0.95, 1, TimeSpan.FromMilliseconds(100));
            var transformGroup = card.RenderTransform as TransformGroup;
            if (transformGroup?.Children.Count > 1 && transformGroup.Children[1] is ScaleTransform scaleTransform)
            {
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
            }
        }
    }
}