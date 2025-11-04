using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class VigenereCipherControl : UserControl
{
    private VigenereCipher? cipher;
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();
    private System.Windows.Threading.DispatcherTimer? stepTimer;
    private List<StepInfo>? pendingSteps;

    public VigenereCipherControl()
    {
        InitializeComponent();
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;
        if (KeyInput != null)
            KeyInput.Text = "KEY";
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

    private void KeyInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void UpdateCipher()
    {
        if (KeyInput == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;
        if (string.IsNullOrWhiteSpace(KeyInput.Text))
            return;

        cipher = new VigenereCipher(KeyInput.Text);
        
        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            bool isEncrypt = ModeComboBox.SelectedIndex == 0;
            string result = isEncrypt 
                ? cipher.Encrypt(PlainTextInput.Text) 
                : cipher.Decrypt(PlainTextInput.Text);
            CipherTextOutput.Text = result;
            
            // Update step-by-step display with animation
            AnimateSteps(isEncrypt, PlainTextInput.Text);
        }
        else
        {
            CipherTextOutput.Text = "";
            StopStepAnimation();
            displayedSteps.Clear();
        }
    }

    private void AnimateSteps(bool isEncrypt, string text)
    {
        if (cipher == null) return;
        StopStepAnimation();
        displayedSteps.Clear();
        
        var steps = isEncrypt 
            ? cipher.GetEncryptionSteps(text)
            : cipher.GetDecryptionSteps(text);
        
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
