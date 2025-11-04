using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class VigenereCipherControl : UserControl
{
    private VigenereCipher? cipher;

    public VigenereCipherControl()
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
        }
        else
        {
            CipherTextOutput.Text = "";
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
