using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class CaesarCipherControl : UserControl
{
    private CaesarCipher cipher = new CaesarCipher();
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();
    private System.Windows.Threading.DispatcherTimer? stepTimer;
    private List<StepInfo>? pendingSteps;

    public CaesarCipherControl()
    {
        InitializeComponent();
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;
        UpdateCipher();
    }

    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;
        
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text" : "Plain Text";
        
        UpdateCipher();
    }

    private void KeySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (KeyValueText != null)
            KeyValueText.Text = $"Key: {(int)KeySlider.Value}";
        UpdateCipher();
    }

    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void UpdateCipher()
    {
        if (PlainTextInput == null || KeySlider == null || CipherTextOutput == null || ModeComboBox == null) return;
        
        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            int key = (int)KeySlider.Value;
            bool isEncrypt = ModeComboBox.SelectedIndex == 0;
            string result = isEncrypt 
                ? cipher.Encrypt(PlainTextInput.Text, key) 
                : cipher.Decrypt(PlainTextInput.Text, key);
            CipherTextOutput.Text = result;
            
            // Update step-by-step display with animation
            AnimateSteps(isEncrypt, PlainTextInput.Text, key);
        }
        else
        {
            CipherTextOutput.Text = "";
            StopStepAnimation();
            displayedSteps.Clear();
        }
    }

    private void AnimateSteps(bool isEncrypt, string text, int key)
    {
        StopStepAnimation();
        displayedSteps.Clear();
        
        var steps = isEncrypt 
            ? cipher.GetEncryptionSteps(text, key)
            : cipher.GetDecryptionSteps(text, key);
        
        if (steps.Count == 0) return;
        
        pendingSteps = steps;
        int currentIndex = 0;
        
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

    private void StopStepAnimation()
    {
        if (stepTimer != null)
        {
            stepTimer.Stop();
            stepTimer = null;
        }
        pendingSteps = null;
    }

    private void BruteForceButton_Click(object sender, RoutedEventArgs e)
    {
        if (PlainTextInput == null || BruteForceResults == null || ModeComboBox == null) return;
        
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        string cipherText = isEncrypt 
            ? (CipherTextOutput?.Text ?? "") 
            : PlainTextInput.Text;
        
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            MessageBox.Show(isEncrypt 
                ? "Please encrypt some text first!" 
                : "Please enter cipher text to brute force!", 
                "Brute Force", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var results = cipher.BruteForce(cipherText);
        BruteForceResults.ItemsSource = results.Select((text, index) => $"Key {index + 1,2}: {text}");
    }
    
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Parent MainWindow'u bul ve menüye geri dön
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
