using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SecurityProject.Services;

namespace SecurityProject.Views.UserControls;

public partial class Base64CipherControl : UserControl
{
    private Base64Cipher cipher = new Base64Cipher();
    private ObservableCollection<StepInfo> displayedSteps = new ObservableCollection<StepInfo>();
    private System.Windows.Threading.DispatcherTimer? stepTimer;
    private List<StepInfo>? pendingSteps;

    public Base64CipherControl()
    {
        InitializeComponent();
        if (StepsDisplay != null)
            StepsDisplay.ItemsSource = displayedSteps;
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
            
            // Update step-by-step display with animation
            AnimateSteps(isEncode, PlainTextInput.Text);
        }
        else
        {
            CipherTextOutput.Text = "";
            StopStepAnimation();
            displayedSteps.Clear();
        }
    }

    private void AnimateSteps(bool isEncode, string text)
    {
        StopStepAnimation();
        displayedSteps.Clear();
        
        var steps = isEncode 
            ? cipher.GetEncodingSteps(text)
            : cipher.GetDecodingSteps(text);
        
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

