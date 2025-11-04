namespace SecurityProject.Services;

public class CaesarCipher
{
    public string Encrypt(string plainText, int key)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        char[] result = new char[plainText.Length];
        
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];
            
            if (char.IsLetter(c))
            {
                char baseChar = char.IsUpper(c) ? 'A' : 'a';
                result[i] = (char)(((c - baseChar + key) % 26) + baseChar);
            }
            else
            {
                result[i] = c;
            }
        }
        
        return new string(result);
    }

    public string Decrypt(string cipherText, int key)
    {
        return Encrypt(cipherText, 26 - (key % 26));
    }

    public List<string> BruteForce(string cipherText)
    {
        List<string> results = new List<string>();
        
        for (int i = 1; i <= 25; i++)
        {
            results.Add(Decrypt(cipherText, i));
        }
        
        return results;
    }
}
