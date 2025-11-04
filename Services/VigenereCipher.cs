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
}
