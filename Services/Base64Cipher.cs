namespace SecurityProject.Services;

public class Base64Cipher
{
    public string Encode(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(textBytes);
        }
        catch
        {
            return "Encoding error";
        }
    }

    public string Decode(string base64Text)
    {
        if (string.IsNullOrEmpty(base64Text))
            return string.Empty;

        try
        {
            byte[] base64Bytes = Convert.FromBase64String(base64Text);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
        catch
        {
            return "Invalid Base64 format";
        }
    }

    public List<StepInfo> GetEncodingSteps(string plainText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, "Starting Base64 encoding"));

        try
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            steps.Add(new StepInfo(2, "Step 2: Convert to Bytes", plainText, 
                string.Join(" ", textBytes.Select(b => $"0x{b:X2}")), 
                $"Converted to {textBytes.Length} bytes (UTF-8 encoding)"));

            string base64Result = Convert.ToBase64String(textBytes);
            steps.Add(new StepInfo(3, "Step 3: Base64 Encoding", 
                string.Join(" ", textBytes.Select(b => $"0x{b:X2}")), 
                base64Result, 
                "Applied Base64 encoding algorithm"));

            steps.Add(new StepInfo(4, "Final Result", plainText, base64Result, "Base64 encoding complete"));
        }
        catch
        {
            steps.Add(new StepInfo(1, "Error", plainText, "Encoding error", "Failed to encode text"));
        }
        
        return steps;
    }

    public List<StepInfo> GetDecodingSteps(string base64Text)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(base64Text))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Base64 Text", base64Text, "Starting Base64 decoding"));

        try
        {
            byte[] base64Bytes = Convert.FromBase64String(base64Text);
            steps.Add(new StepInfo(2, "Step 2: Base64 Decoding", base64Text, 
                string.Join(" ", base64Bytes.Select(b => $"0x{b:X2}")), 
                $"Decoded to {base64Bytes.Length} bytes"));

            string decodedText = System.Text.Encoding.UTF8.GetString(base64Bytes);
            steps.Add(new StepInfo(3, "Step 3: Convert to Text", 
                string.Join(" ", base64Bytes.Select(b => $"0x{b:X2}")), 
                decodedText, 
                "Converted bytes to UTF-8 text"));

            steps.Add(new StepInfo(4, "Final Result", base64Text, decodedText, "Base64 decoding complete"));
        }
        catch
        {
            steps.Add(new StepInfo(1, "Error", base64Text, "Invalid Base64 format", "Failed to decode Base64 string"));
        }
        
        return steps;
    }
}

