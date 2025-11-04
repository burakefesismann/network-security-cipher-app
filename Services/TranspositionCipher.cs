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

    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, $"Starting transposition encryption with key = {key}"));

        plainText = plainText.ToUpper();
        string cleanedText = plainText.Replace(" ", "");
        steps.Add(new StepInfo(2, "Step 2: Clean Text", plainText, cleanedText, "Removed spaces"));

        int cols = key;
        int rows = (int)Math.Ceiling((double)cleanedText.Length / cols);
        
        steps.Add(new StepInfo(3, "Step 3: Create Matrix", cleanedText, $"Matrix: {rows}x{cols}", $"Created {rows} rows × {cols} columns matrix"));

        char[,] matrix = new char[rows, cols];
        int charIndex = 0;
        
        string matrixStr = "";
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (charIndex < cleanedText.Length)
                    matrix[row, col] = cleanedText[charIndex++];
                else
                    matrix[row, col] = 'X';
                matrixStr += matrix[row, col] + " ";
            }
            matrixStr += "\n";
        }
        
        steps.Add(new StepInfo(4, "Step 4: Fill Matrix Row-wise", cleanedText, matrixStr.Trim(), "Filled matrix row by row"));

        List<char> result = new List<char>();
        string readOrder = "";
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                result.Add(matrix[row, col]);
                readOrder += matrix[row, col];
            }
            readOrder += " | ";
        }
        
        steps.Add(new StepInfo(5, "Step 5: Read Column-wise", matrixStr.Trim(), readOrder.TrimEnd(' ', '|'), "Read matrix column by column"));
        
        steps.Add(new StepInfo(6, "Final Result", plainText, new string(result.ToArray()), "Encryption complete"));
        
        return steps;
    }

    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(cipherText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Cipher Text", cipherText, $"Starting transposition decryption with key = {key}"));

        cipherText = cipherText.ToUpper();
        int cols = key;
        int rows = (int)Math.Ceiling((double)cipherText.Length / cols);
        
        steps.Add(new StepInfo(2, "Step 2: Create Matrix", cipherText, $"Matrix: {rows}x{cols}", $"Created {rows} rows × {cols} columns matrix"));

        char[,] matrix = new char[rows, cols];
        int charIndex = 0;
        
        string matrixStr = "";
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (charIndex < cipherText.Length)
                    matrix[row, col] = cipherText[charIndex++];
                matrixStr += matrix[row, col] + " ";
            }
            matrixStr += "\n";
        }
        
        steps.Add(new StepInfo(3, "Step 3: Fill Matrix Column-wise", cipherText, matrixStr.Trim(), "Filled matrix column by column"));

        List<char> result = new List<char>();
        string readOrder = "";
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                result.Add(matrix[row, col]);
                readOrder += matrix[row, col];
            }
            readOrder += " ";
        }
        
        steps.Add(new StepInfo(4, "Step 4: Read Row-wise", matrixStr.Trim(), readOrder.Trim(), "Read matrix row by row"));
        
        string final = new string(result.ToArray()).TrimEnd('X');
        steps.Add(new StepInfo(5, "Final Result", cipherText, final, "Decryption complete"));
        
        return steps;
    }
}
