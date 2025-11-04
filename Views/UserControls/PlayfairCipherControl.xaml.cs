using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class PlayfairCipherControl : UserControl
{
    private PlayfairCipher? cipher;

    public PlayfairCipherControl()
    {
        InitializeComponent();
        if (KeyInput != null)
            KeyInput.Text = "MONARCHY";
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

        try
        {
            cipher = new PlayfairCipher(KeyInput.Text);
            DisplayKeyMatrix();
            
            if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
            {
                bool isEncrypt = ModeComboBox.SelectedIndex == 0;
                string result = isEncrypt 
                    ? cipher.Encrypt(PlainTextInput.Text) 
                    : cipher.Decrypt(PlainTextInput.Text);
                CipherTextOutput.Text = result;
                
                // Update step-by-step display
                if (StepsDisplay != null)
                {
                    var steps = isEncrypt 
                        ? cipher.GetEncryptionSteps(PlainTextInput.Text)
                        : cipher.GetDecryptionSteps(PlainTextInput.Text);
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
        catch
        {
            // Invalid key, ignore
        }
    }

    private void DisplayKeyMatrix()
    {
        if (cipher == null || KeyMatrixDisplay == null) return;

        var matrix = cipher.GetKeyMatrix();
        var chars = new List<char>();

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                chars.Add(matrix[i, j]);
            }
        }

        KeyMatrixDisplay.ItemsSource = chars;
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
