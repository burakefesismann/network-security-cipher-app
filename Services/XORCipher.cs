namespace SecurityProject.Services;

public class XORCipher
{
    public string Encrypt(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key))
            return string.Empty;

        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        
        byte[] result = new byte[textBytes.Length];
        
        for (int i = 0; i < textBytes.Length; i++)
        {
            result[i] = (byte)(textBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }
        
        // Convert to hexadecimal string for display
        return BitConverter.ToString(result).Replace("-", " ");
    }

    public string Decrypt(string cipherText, string key)
    {
        if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key))
            return string.Empty;

        try
        {
            // Remove spaces and convert hex string back to bytes
            string hexString = cipherText.Replace(" ", "").Replace("-", "");
            
            if (string.IsNullOrEmpty(hexString) || hexString.Length % 2 != 0)
            {
                return "Invalid hex format (must be even length)";
            }
            
            byte[] cipherBytes = new byte[hexString.Length / 2];
            
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                cipherBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] result = new byte[cipherBytes.Length];
            
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                result[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }
            
            return System.Text.Encoding.UTF8.GetString(result);
        }
        catch
        {
            return "Invalid hex format";
        }
    }

    public List<StepInfo> GetEncryptionSteps(string plainText, string key)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key))
            return steps;

        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, $"Starting XOR encryption with key: {key}"));

        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] result = new byte[textBytes.Length];
        int stepNum = 2;

        for (int i = 0; i < textBytes.Length; i++)
        {
            byte textByte = textBytes[i];
            byte keyByte = keyBytes[i % keyBytes.Length];
            result[i] = (byte)(textByte ^ keyByte);
            
            steps.Add(new StepInfo(stepNum++, $"XOR byte {i + 1}", 
                $"Text: {textByte} (0x{textByte:X2}), Key: {keyByte} (0x{keyByte:X2})", 
                $"Result: {result[i]} (0x{result[i]:X2})", 
                $"{textByte} XOR {keyByte} = {result[i]}"));
        }
        
        string hexResult = BitConverter.ToString(result).Replace("-", " ");
        steps.Add(new StepInfo(stepNum, "Final Result (Hex)", plainText, hexResult, "Encryption complete - converted to hexadecimal"));
        
        return steps;
    }

    public List<StepInfo> GetDecryptionSteps(string cipherText, string key)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key))
            return steps;

        try
        {
            string hexString = cipherText.Replace(" ", "").Replace("-", "");
            
            if (string.IsNullOrEmpty(hexString) || hexString.Length % 2 != 0)
            {
                steps.Add(new StepInfo(1, "Error", "Invalid hex format", "Error", "Hex string must be even length"));
                return steps;
            }
            
            steps.Add(new StepInfo(1, "Initialization", "Cipher Text (Hex)", cipherText, $"Starting XOR decryption with key: {key}"));

            byte[] cipherBytes = new byte[hexString.Length / 2];
            
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                cipherBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] result = new byte[cipherBytes.Length];
            int stepNum = 2;

            for (int i = 0; i < cipherBytes.Length; i++)
            {
                byte cipherByte = cipherBytes[i];
                byte keyByte = keyBytes[i % keyBytes.Length];
                result[i] = (byte)(cipherByte ^ keyByte);
                
                steps.Add(new StepInfo(stepNum++, $"XOR byte {i + 1}", 
                    $"Cipher: {cipherByte} (0x{cipherByte:X2}), Key: {keyByte} (0x{keyByte:X2})", 
                    $"Result: {result[i]} (0x{result[i]:X2})", 
                    $"{cipherByte} XOR {keyByte} = {result[i]}"));
            }
            
            string decryptedText = System.Text.Encoding.UTF8.GetString(result);
            steps.Add(new StepInfo(stepNum, "Final Result", cipherText, decryptedText, "Decryption complete"));
            
            return steps;
        }
        catch
        {
            steps.Add(new StepInfo(1, "Error", "Invalid format", "Error", "Failed to decode hex string"));
            return steps;
        }
    }
}

