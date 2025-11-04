using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class HillCipherControl : UserControl
{
    private HillCipher? cipher;

    public HillCipherControl()
    {
        InitializeComponent();
        MatrixChanged(null!, null!);
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

    private void MatrixChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void UpdateCipher()
    {
        if (M00 == null || M01 == null || M10 == null || M11 == null || 
            PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;
        
        if (!int.TryParse(M00.Text, out int m00) || 
            !int.TryParse(M01.Text, out int m01) ||
            !int.TryParse(M10.Text, out int m10) || 
            !int.TryParse(M11.Text, out int m11))
            return;

        try
        {
            int[,] matrix = { { m00, m01 }, { m10, m11 } };
            cipher = new HillCipher(matrix);
            
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
        catch (ArgumentException)
        {
            CipherTextOutput.Text = "Invalid key matrix!";
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
