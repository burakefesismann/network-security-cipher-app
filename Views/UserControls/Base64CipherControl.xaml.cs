using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class Base64CipherControl : UserControl
{
    private Base64Cipher cipher = new Base64Cipher();

    public Base64CipherControl()
    {
        InitializeComponent();
        UpdateCipher();
    }

    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeComboBox == null) return;
        
        bool isEncode = ModeComboBox.SelectedIndex == 0;
        if (InputLabel != null)
            InputLabel.Text = isEncode ? "Plain Text" : "Base64 Encoded";
        if (OutputLabel != null)
            OutputLabel.Text = isEncode ? "Base64 Encoded" : "Plain Text";
        
        UpdateCipher();
    }

    private void PlainTextInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCipher();
    }

    private void UpdateCipher()
    {
        if (PlainTextInput == null || CipherTextOutput == null || ModeComboBox == null) return;

        if (!string.IsNullOrWhiteSpace(PlainTextInput.Text))
        {
            bool isEncode = ModeComboBox.SelectedIndex == 0;
            string result = isEncode 
                ? cipher.Encode(PlainTextInput.Text) 
                : cipher.Decode(PlainTextInput.Text);
            CipherTextOutput.Text = result;
            
            // Update step-by-step display
            if (StepsDisplay != null)
            {
                var steps = isEncode 
                    ? cipher.GetEncodingSteps(PlainTextInput.Text)
                    : cipher.GetDecodingSteps(PlainTextInput.Text);
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

