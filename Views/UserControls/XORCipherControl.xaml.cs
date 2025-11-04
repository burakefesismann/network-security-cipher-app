using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class XORCipherControl : UserControl
{
    private XORCipher cipher = new XORCipher();

    public XORCipherControl()
    {
        InitializeComponent();
        if (KeyInput != null)
            KeyInput.Text = "KEY";
        UpdateCipher();
    }

    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;
        
        bool isEncrypt = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncrypt ? "Plain Text" : "Cipher Text (Hex)";
        if (OutputLabel != null)
            OutputLabel.Text = isEncrypt ? "Cipher Text (Hex)" : "Plain Text";
        
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

        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            bool isEncrypt = ModeComboBox.SelectedIndex == 0;
            string result = isEncrypt 
                ? cipher.Encrypt(PlainTextInput.Text, KeyInput.Text) 
                : cipher.Decrypt(PlainTextInput.Text, KeyInput.Text);
            CipherTextOutput.Text = result;
            
            // Update step-by-step display
            if (StepsDisplay != null && !string.IsNullOrWhiteSpace(KeyInput.Text))
            {
                var steps = isEncrypt 
                    ? cipher.GetEncryptionSteps(PlainTextInput.Text, KeyInput.Text)
                    : cipher.GetDecryptionSteps(PlainTextInput.Text, KeyInput.Text);
                StepsDisplay.ItemsSource = steps;
            }
        }
        else
        {
            CipherTextOutput.Text = "";
            if (StepsDisplay != null)
                StepsDisplay.ItemsSource = null;
        }
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

