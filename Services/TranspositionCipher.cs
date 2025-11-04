namespace SecurityProject.Services;

public class TranspositionCipher
{
    private int key;

    public TranspositionCipher(int key)
    {
        if (key <= 0)
            throw new ArgumentException("Key must be positive");
        
        this.key = key;
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        plainText = plainText.ToUpper();
        
        // Remove spaces for simplicity
        string cleanedText = plainText.Replace(" ", "");
        
        int cols = key;
        int rows = (int)Math.Ceiling((double)cleanedText.Length / cols);
        
        char[,] matrix = new char[rows, cols];
        
        // Fill matrix with plaintext
        int charIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (charIndex < cleanedText.Length)
                    matrix[row, col] = cleanedText[charIndex++];
                else
                    matrix[row, col] = 'X'; // Padding
            }
        }
        
        // Read column-wise
        List<char> result = new List<char>();
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                result.Add(matrix[row, col]);
            }
        }
        
        return new string(result.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        cipherText = cipherText.ToUpper();
        
        int cols = key;
        int rows = (int)Math.Ceiling((double)cipherText.Length / cols);
        
        char[,] matrix = new char[rows, cols];
        
        // Fill matrix column-wise
        int charIndex = 0;
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (charIndex < cipherText.Length)
                    matrix[row, col] = cipherText[charIndex++];
            }
        }
        
        // Read row-wise
        List<char> result = new List<char>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                result.Add(matrix[row, col]);
            }
        }
        
        return new string(result.ToArray()).TrimEnd('X');
    }

    public int GetKey() => key;
}
