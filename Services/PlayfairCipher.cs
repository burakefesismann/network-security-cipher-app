namespace SecurityProject.Services;

public class PlayfairCipher
{
    private char[,] keyMatrix;
    private Dictionary<char, (int row, int col)> positionMap;

    public PlayfairCipher(string key)
    {
        keyMatrix = new char[5, 5];
        positionMap = new Dictionary<char, (int, int)>();
        
        BuildKeyMatrix(key);
    }

    private void BuildKeyMatrix(string key)
    {
        HashSet<char> used = new HashSet<char> { 'J' }; // I and J are treated as same
        char[] keyChars = key.ToUpper().Replace('J', 'I').ToCharArray();
        
        int row = 0, col = 0;
        
        // Fill key
        foreach (char c in keyChars)
        {
            if (char.IsLetter(c) && !used.Contains(c))
            {
                keyMatrix[row, col] = c;
                positionMap[c] = (row, col);
                used.Add(c);
                col++;
                if (col == 5)
                {
                    col = 0;
                    row++;
                }
            }
        }
        
        // Fill rest of alphabet
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (c == 'J') continue;
            
            if (!used.Contains(c))
            {
                keyMatrix[row, col] = c;
                positionMap[c] = (row, col);
                used.Add(c);
                col++;
                if (col == 5)
                {
                    col = 0;
                    row++;
                }
            }
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        string preparedText = PrepareText(plainText);
        
        if (preparedText.Length % 2 != 0)
            preparedText += 'X';

        List<char> result = new List<char>();
        
        for (int i = 0; i < preparedText.Length; i += 2)
        {
            char c1 = preparedText[i];
            char c2 = preparedText[i + 1];
            
            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];
            
            if (r1 == r2)
            {
                // Same row
                result.Add(keyMatrix[r1, (c1_pos + 1) % 5]);
                result.Add(keyMatrix[r2, (c2_pos + 1) % 5]);
            }
            else if (c1_pos == c2_pos)
            {
                // Same column
                result.Add(keyMatrix[(r1 + 1) % 5, c1_pos]);
                result.Add(keyMatrix[(r2 + 1) % 5, c2_pos]);
            }
            else
            {
                // Rectangle
                result.Add(keyMatrix[r1, c2_pos]);
                result.Add(keyMatrix[r2, c1_pos]);
            }
        }
        
        return new string(result.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText) || cipherText.Length % 2 != 0)
            return string.Empty;

        List<char> result = new List<char>();
        
        for (int i = 0; i < cipherText.Length; i += 2)
        {
            char c1 = cipherText[i];
            char c2 = cipherText[i + 1];
            
            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];
            
            if (r1 == r2)
            {
                result.Add(keyMatrix[r1, (c1_pos + 4) % 5]);
                result.Add(keyMatrix[r2, (c2_pos + 4) % 5]);
            }
            else if (c1_pos == c2_pos)
            {
                result.Add(keyMatrix[(r1 + 4) % 5, c1_pos]);
                result.Add(keyMatrix[(r2 + 4) % 5, c2_pos]);
            }
            else
            {
                result.Add(keyMatrix[r1, c2_pos]);
                result.Add(keyMatrix[r2, c1_pos]);
            }
        }
        
        return new string(result.ToArray());
    }

    private string PrepareText(string text)
    {
        text = text.ToUpper().Replace('J', 'I');
        List<char> result = new List<char>();
        
        foreach (char c in text)
        {
            if (char.IsLetter(c))
                result.Add(c);
        }
        
        // Handle double letters
        for (int i = 0; i < result.Count - 1; i += 2)
        {
            if (result[i] == result[i + 1])
            {
                result.Insert(i + 1, 'X');
            }
        }
        
        return new string(result.ToArray());
    }

    public char[,] GetKeyMatrix() => keyMatrix;

    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, "Starting Playfair encryption"));

        string preparedText = PrepareText(plainText);
        steps.Add(new StepInfo(2, "Step 2: Prepare Text", plainText, preparedText, "Removed non-letters, replaced J with I"));

        if (preparedText.Length % 2 != 0)
        {
            preparedText += 'X';
            steps.Add(new StepInfo(3, "Step 3: Add Padding", preparedText.Substring(0, preparedText.Length - 1), preparedText, "Added 'X' to make even length"));
        }

        List<char> result = new List<char>();
        int stepNum = preparedText.Length % 2 != 0 ? 4 : 3;
        int pairNum = 1;

        for (int i = 0; i < preparedText.Length; i += 2)
        {
            char c1 = preparedText[i];
            char c2 = preparedText[i + 1];
            
            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];
            
            char e1, e2;
            string rule;
            
            if (r1 == r2)
            {
                e1 = keyMatrix[r1, (c1_pos + 1) % 5];
                e2 = keyMatrix[r2, (c2_pos + 1) % 5];
                rule = "Same row: shift right";
            }
            else if (c1_pos == c2_pos)
            {
                e1 = keyMatrix[(r1 + 1) % 5, c1_pos];
                e2 = keyMatrix[(r2 + 1) % 5, c2_pos];
                rule = "Same column: shift down";
            }
            else
            {
                e1 = keyMatrix[r1, c2_pos];
                e2 = keyMatrix[r2, c1_pos];
                rule = "Rectangle: swap columns";
            }
            
            result.Add(e1);
            result.Add(e2);
            
            steps.Add(new StepInfo(stepNum++, $"Pair {pairNum++}: {c1}{c2}", 
                $"{c1} at ({r1},{c1_pos}), {c2} at ({r2},{c2_pos})", 
                $"{e1}{e2}", 
                rule));
        }
        
        steps.Add(new StepInfo(stepNum, "Final Result", plainText, new string(result.ToArray()), "Encryption complete"));
        
        return steps;
    }

    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();
        
        if (string.IsNullOrEmpty(cipherText) || cipherText.Length % 2 != 0)
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Cipher Text", cipherText, "Starting Playfair decryption"));

        List<char> result = new List<char>();
        int stepNum = 2;
        int pairNum = 1;

        for (int i = 0; i < cipherText.Length; i += 2)
        {
            char c1 = cipherText[i];
            char c2 = cipherText[i + 1];
            
            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];
            
            char d1, d2;
            string rule;
            
            if (r1 == r2)
            {
                d1 = keyMatrix[r1, (c1_pos + 4) % 5];
                d2 = keyMatrix[r2, (c2_pos + 4) % 5];
                rule = "Same row: shift left";
            }
            else if (c1_pos == c2_pos)
            {
                d1 = keyMatrix[(r1 + 4) % 5, c1_pos];
                d2 = keyMatrix[(r2 + 4) % 5, c2_pos];
                rule = "Same column: shift up";
            }
            else
            {
                d1 = keyMatrix[r1, c2_pos];
                d2 = keyMatrix[r2, c1_pos];
                rule = "Rectangle: swap columns";
            }
            
            result.Add(d1);
            result.Add(d2);
            
            steps.Add(new StepInfo(stepNum++, $"Pair {pairNum++}: {c1}{c2}", 
                $"{c1} at ({r1},{c1_pos}), {c2} at ({r2},{c2_pos})", 
                $"{d1}{d2}", 
                rule));
        }
        
        steps.Add(new StepInfo(stepNum, "Final Result", cipherText, new string(result.ToArray()), "Decryption complete"));
        
        return steps;
    }
}
