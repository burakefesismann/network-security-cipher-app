using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class TranspositionCipherControl : UserControl
{
    private TranspositionCipher? cipher;
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();
    private System.Windows.Threading.DispatcherTimer? stepTimer;
    private List<StepInfo>? pendingSteps;

    public TranspositionCipherControl()
    {
        InitializeComponent();
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;
        KeySlider_ValueChanged(null!, null!);
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
        if (KeyValueText != null && KeySlider != null)
            KeyValueText.Text = $"Columns: {(int)KeySlider.Value}";
        UpdateCipher();
    }

    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void UpdateCipher()
    {
        if (KeySlider == null || PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;
        
        try
        {
            int key = (int)KeySlider.Value;
            cipher = new TranspositionCipher(key);
            
            if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
            {
                bool isEncrypt = ModeComboBox.SelectedIndex == 0;
                string result = isEncrypt 
                    ? cipher.Encrypt(PlainTextInput.Text) 
                    : cipher.Decrypt(PlainTextInput.Text);
                CipherTextOutput.Text = result;
                
                if (isEncrypt)
                    DisplayMatrix(PlainTextInput.Text, key);
                else
                    DisplayMatrixDecrypt(PlainTextInput.Text, key);
                
                // Update step-by-step display with animation
                AnimateSteps(isEncrypt, PlainTextInput.Text);
            }
            else
            {
                CipherTextOutput.Text = "";
                if (MatrixDisplay != null)
                    MatrixDisplay.ItemsSource = null;
                StopStepAnimation();
                displayedSteps.Clear();
            }
        }
        catch
        {
            // Invalid key
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

    private void DisplayMatrix(string text, int cols)
    {
        if (MatrixDisplay == null) return;
        
        text = text.ToUpper().Replace(" ", "");
        int rows = (int)Math.Ceiling((double)text.Length / cols);
        var chars = new List<char>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                chars.Add(index < text.Length ? text[index] : ' ');
            }
        }

        MatrixDisplay.ItemsSource = chars;
    }

    private void DisplayMatrixDecrypt(string text, int cols)
    {
        if (MatrixDisplay == null) return;
        
        text = text.ToUpper().Replace(" ", "");
        int rows = (int)Math.Ceiling((double)text.Length / cols);
        var chars = new List<char>();

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                int index = col * rows + row;
                chars.Add(index < text.Length ? text[index] : ' ');
            }
        }

        MatrixDisplay.ItemsSource = chars;
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
