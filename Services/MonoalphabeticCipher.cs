namespace SecurityProject.Services;

public class MonoalphabeticCipher
{
    private Dictionary<char, char> encryptionMap;
    private Dictionary<char, char> decryptionMap;

    public MonoalphabeticCipher(string key)
    {
        if (string.IsNullOrEmpty(key) || key.Length != 26)
            throw new ArgumentException("Key must contain exactly 26 unique letters");

        var uniqueChars = key.ToUpper().Distinct().ToList();
        if (uniqueChars.Count != 26)
            throw new ArgumentException("Key must contain 26 unique letters");

        encryptionMap = new Dictionary<char, char>();
        decryptionMap = new Dictionary<char, char>();

        for (int i = 0; i < 26; i++)
        {
            encryptionMap[(char)('A' + i)] = key.ToUpper()[i];
            decryptionMap[key.ToUpper()[i]] = (char)('A' + i);
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        char[] result = new char[plainText.Length];
        
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];
            
            if (char.IsLetter(c))
            {
                result[i] = char.IsUpper(c) 
                    ? encryptionMap[c] 
                    : char.ToLower(encryptionMap[char.ToUpper(c)]);
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

        char[] result = new char[cipherText.Length];
        
        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];
            
            if (char.IsLetter(c))
            {
                result[i] = char.IsUpper(c) 
                    ? decryptionMap[c] 
                    : char.ToLower(decryptionMap[char.ToUpper(c)]);
            }
            else
            {
                result[i] = c;
            }
        }
        
        return new string(result);
    }

    public static string GenerateRandomKey()
    {
        Random random = new Random();
        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();
        
        // Fisher-Yates shuffle
        for (int i = alphabet.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (alphabet[i], alphabet[j]) = (alphabet[j], alphabet[i]);
        }
        
        return new string(alphabet.ToArray());
    }

    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, "Starting encryption with substitution key"));
        
        var result = new System.Text.StringBuilder();
        int stepNum = 2;
        
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];
            
            if (char.IsLetter(c))
            {
                char upperC = char.ToUpper(c);
                char encrypted = encryptionMap[upperC];
                if (char.IsLower(c))
                    encrypted = char.ToLower(encrypted);
                result.Append(encrypted);
                
                steps.Add(new StepInfo(stepNum++, $"Map '{c}'", 
                    $"Input: {c}", 
                    $"Output: {encrypted}", 
                    $"{c} → {encrypted} (using substitution table)"));
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

        steps.Add(new StepInfo(1, "Initialization", "Cipher Text", cipherText, "Starting decryption with substitution key"));
        
        var result = new System.Text.StringBuilder();
        int stepNum = 2;
        
        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];
            
            if (char.IsLetter(c))
            {
                char upperC = char.ToUpper(c);
                char decrypted = decryptionMap[upperC];
                if (char.IsLower(c))
                    decrypted = char.ToLower(decrypted);
                result.Append(decrypted);
                
                steps.Add(new StepInfo(stepNum++, $"Map '{c}'", 
                    $"Input: {c}", 
                    $"Output: {decrypted}", 
                    $"{c} → {decrypted} (using reverse substitution table)"));
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
