namespace SecurityProject.Services;

public class VigenereCipher
{
    private string key;

    public VigenereCipher(string key)
    {
        this.key = key.ToUpper();
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        plainText = plainText.ToUpper();
        char[] result = new char[plainText.Length];
        int keyIndex = 0;

        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];
            
            if (char.IsLetter(c))
            {
                char keyChar = key[keyIndex % key.Length];
                result[i] = (char)(((c - 'A' + keyChar - 'A') % 26) + 'A');
                keyIndex++;
            }
            else
            {
                result[i] = c;
            }
        }

        return new string(result);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        cipherText = cipherText.ToUpper();
        char[] result = new char[cipherText.Length];
        int keyIndex = 0;

        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];
            
            if (char.IsLetter(c))
            {
                char keyChar = key[keyIndex % key.Length];
                result[i] = (char)(((c - 'A' - keyChar + 'A' + 26) % 26) + 'A');
                keyIndex++;
            }
            else
            {
                result[i] = c;
            }
        }

        return new string(result);
    }

    public string GetKey() => key;

    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, $"Starting encryption with key: {key}"));

        plainText = plainText.ToUpper();
        var result = new System.Text.StringBuilder();
        int keyIndex = 0;
        int stepNum = 2;

        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];
            
            if (char.IsLetter(c))
            {
                char keyChar = key[keyIndex % key.Length];
                int plainPos = c - 'A';
                int keyPos = keyChar - 'A';
                int newPos = (plainPos + keyPos) % 26;
                char encrypted = (char)(newPos + 'A');
                result.Append(encrypted);
                keyIndex++;
                
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'", 
                    $"Plain: {c} (pos {plainPos}), Key: {keyChar} (pos {keyPos})", 
                    $"Cipher: {encrypted} (pos {newPos})", 
                    $"{c} + {keyChar} = {encrypted} (mod 26)"));
            }
            else
            {
                result.Append(c);
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'", 
                    "Non-letter character", 
                    c.ToString(), 
                    "Non-letter characters remain unchanged"));
            }
        }
        
        steps.Add(new StepInfo(stepNum, "Final Result", plainText, result.ToString(), "Encryption complete"));
        
        return steps;
    }

    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(cipherText))
            return steps;

        steps.Add(new StepInfo(1, "Initialization", "Cipher Text", cipherText, $"Starting decryption with key: {key}"));

        cipherText = cipherText.ToUpper();
        var result = new System.Text.StringBuilder();
        int keyIndex = 0;
        int stepNum = 2;

        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];
            
            if (char.IsLetter(c))
            {
                char keyChar = key[keyIndex % key.Length];
                int cipherPos = c - 'A';
                int keyPos = keyChar - 'A';
                int newPos = (cipherPos - keyPos + 26) % 26;
                char decrypted = (char)(newPos + 'A');
                result.Append(decrypted);
                keyIndex++;
                
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'", 
                    $"Cipher: {c} (pos {cipherPos}), Key: {keyChar} (pos {keyPos})", 
                    $"Plain: {decrypted} (pos {newPos})", 
                    $"{c} - {keyChar} = {decrypted} (mod 26)"));
            }
            else
            {
                result.Append(c);
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'", 
                    "Non-letter character", 
                    c.ToString(), 
                    "Non-letter characters remain unchanged"));
            }
        }
        
        steps.Add(new StepInfo(stepNum, "Final Result", cipherText, result.ToString(), "Decryption complete"));
        
        return steps;
    }
}
