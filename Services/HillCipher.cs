namespace SecurityProject.Services;

public class HillCipher
{
    private int[,] keyMatrix;

    public HillCipher(int[,] matrix)
    {
        if (matrix.GetLength(0) != matrix.GetLength(1))
            throw new ArgumentException("Key matrix must be square");
        
        if (matrix.GetLength(0) != 2 && matrix.GetLength(0) != 3)
            throw new ArgumentException("Key matrix must be 2x2 or 3x3");

        // Check if determinant is coprime with 26
        int det = Determinant(matrix);
        if (GCD(det, 26) != 1)
            throw new ArgumentException("Key matrix determinant must be coprime with 26");

        keyMatrix = matrix;
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        plainText = plainText.ToUpper();
        List<char> textChars = new List<char>();
        
        foreach (char c in plainText)
        {
            if (char.IsLetter(c))
                textChars.Add(c);
        }
        
        int matrixSize = keyMatrix.GetLength(0);
        int paddedLength = (textChars.Count + matrixSize - 1) / matrixSize * matrixSize;
        
        while (textChars.Count < paddedLength)
            textChars.Add('X');

        List<char> result = new List<char>();
        
        for (int i = 0; i < textChars.Count; i += matrixSize)
        {
            int[] vector = new int[matrixSize];
            
            for (int j = 0; j < matrixSize; j++)
            {
                vector[j] = textChars[i + j] - 'A';
            }
            
            int[] encryptedVector = MatrixMultiply(vector);
            
            for (int j = 0; j < matrixSize; j++)
            {
                result.Add((char)(encryptedVector[j] % 26 + 'A'));
            }
        }
        
        return new string(result.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        cipherText = cipherText.ToUpper();
        List<char> textChars = new List<char>();
        
        foreach (char c in cipherText)
        {
            if (char.IsLetter(c))
                textChars.Add(c);
        }
        
        int matrixSize = keyMatrix.GetLength(0);
        int[,] inverseMatrix = InverseKeyMatrix();
        List<char> result = new List<char>();
        
        for (int i = 0; i < textChars.Count; i += matrixSize)
        {
            int[] vector = new int[matrixSize];
            
            for (int j = 0; j < matrixSize; j++)
            {
                vector[j] = textChars[i + j] - 'A';
            }
            
            int[] decryptedVector = MatrixMultiplyInverse(vector, inverseMatrix);
            
            for (int j = 0; j < matrixSize; j++)
            {
                result.Add((char)(decryptedVector[j] % 26 + 'A'));
            }
        }
        
        return new string(result.ToArray());
    }

    private int[] MatrixMultiply(int[] vector)
    {
        int size = vector.Length;
        int[] result = new int[size];
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                result[i] += keyMatrix[i, j] * vector[j];
            }
        }
        
        return result;
    }

    private int[] MatrixMultiplyInverse(int[] vector, int[,] inverse)
    {
        int size = vector.Length;
        int[] result = new int[size];
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                result[i] += inverse[i, j] * vector[j];
            }
        }
        
        return result;
    }

    private int[,] InverseKeyMatrix()
    {
        int size = keyMatrix.GetLength(0);
        int[,] inverse = new int[size, size];
        
        if (size == 2)
        {
            int det = Determinant(keyMatrix);
            int detInverse = ModularInverse(det, 26);
            
            inverse[0, 0] = keyMatrix[1, 1];
            inverse[0, 1] = -keyMatrix[0, 1];
            inverse[1, 0] = -keyMatrix[1, 0];
            inverse[1, 1] = keyMatrix[0, 0];
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    inverse[i, j] = (inverse[i, j] * detInverse) % 26;
                    if (inverse[i, j] < 0)
                        inverse[i, j] += 26;
                }
            }
        }
        
        return inverse;
    }

    private int Determinant(int[,] matrix)
    {
        int size = matrix.GetLength(0);
        
        if (size == 2)
        {
            return (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) % 26;
        }
        else if (size == 3)
        {
            return (matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) -
                    matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]) +
                    matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0])) % 26;
        }
        
        return 0;
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return Math.Abs(a);
    }

    private int ModularInverse(int a, int m)
    {
        a = a % m;
        for (int x = 1; x < m; x++)
        {
            if ((a * x) % m == 1)
                return x;
        }
        return 1;
    }

    public int[,] GetKeyMatrix() => keyMatrix;
}
