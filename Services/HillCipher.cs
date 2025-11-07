namespace SecurityProject.Services;

/// <summary>
/// Hill Şifreleme algoritmasını uygulayan sınıf.
///
/// Hill Şifresi Nedir?
/// 1929'da Lester S. Hill tarafından icat edilen, doğrusal cebir temelli bir şifreleme yöntemidir.
/// İlk kez matematiği pratik olarak kullanan poligrafik (çok harfli) bir şifreleme tekniğidir.
///
/// Temel Prensipler:
/// 1. Şifreleme anahtarı bir matristir (genellikle 2x2 veya 3x3)
/// 2. Düz metin vektörlere bölünür ve matris çarpımı ile şifrelenir
/// 3. Tüm işlemler modulo 26 (alfabe uzunluğu) aritmetiği ile yapılır
///
/// Matematiksel Formül:
/// Şifreleme: C = (K × P) mod 26
/// Şifre Çözme: P = (K^-1 × C) mod 26
/// Burada K=Anahtar matrisi, P=Düz metin vektörü, C=Şifreli metin vektörü
///
/// Anahtar Gereksinimleri:
/// - Matris kare olmalıdır (nxn)
/// - Determinantı 26 ile aralarında asal olmalıdır (gcd(det(K), 26) = 1)
/// - Bu, matrisin mod 26'da tersinin var olmasını garantiler
///
/// Örnek (2x2 matris):
/// Anahtar Matrisi K = [6  24]
///                     [1  13]
/// det(K) = 6*13 - 24*1 = 78 - 24 = 54 mod 26 = 2
/// gcd(2, 26) = 2 ≠ 1, bu matris kullanılamaz!
///
/// Güvenlik:
/// Frekans analizine karşı dirençlidir çünkü aynı harf her seferinde farklı şifrelenebilir.
/// Ancak known-plaintext saldırılarına karşı zayıftır.
/// </summary>
public class HillCipher
{
    /// <summary>
    /// Şifreleme için kullanılan anahtar matrisi.
    /// 2x2 veya 3x3 boyutlarında olabilir.
    /// </summary>
    private int[,] keyMatrix;

    /// <summary>
    /// HillCipher sınıfının yeni bir örneğini oluşturur.
    ///
    /// Matris Doğrulama:
    /// 1. Matrisin kare olması kontrol edilir
    /// 2. Boyutunun 2x2 veya 3x3 olması zorunludur
    /// 3. Determinantın 26 ile aralarında asal olması gerekir
    ///
    /// Determinant Koşulu:
    /// Determinant 26 ile aralarında asal değilse, matrisin mod 26'da tersi yoktur.
    /// Bu durumda şifre çözme imkansız olur, bu yüzden exception fırlatılır.
    /// </summary>
    /// <param name="matrix">Şifreleme için kullanılacak kare matris.</param>
    /// <exception cref="ArgumentException">
    /// Matris kare değilse, 2x2 veya 3x3 değilse, ya da determinantı 26 ile aralarında asal değilse.
    /// </exception>
    public HillCipher(int[,] matrix)
    {
        // Kare matris kontrolü
        if (matrix.GetLength(0) != matrix.GetLength(1))
            throw new ArgumentException("Key matrix must be square");

        // Boyut kontrolü - sadece 2x2 ve 3x3 desteklenir
        if (matrix.GetLength(0) != 2 && matrix.GetLength(0) != 3)
            throw new ArgumentException("Key matrix must be 2x2 or 3x3");

        // Determinant kontrolü - 26 ile aralarında asal olmalı
        int det = Determinant(matrix);
        if (GCD(det, 26) != 1)
            throw new ArgumentException("Key matrix determinant must be coprime with 26");

        keyMatrix = matrix;
    }

    /// <summary>
    /// Düz metni Hill şifresi ile şifreler.
    ///
    /// Şifreleme Algoritması:
    /// 1. Metni temizle (sadece harfleri al, büyük harfe çevir)
    /// 2. Metni matris boyutuna göre bloklara böl
    /// 3. Gerekirse 'X' ile padding ekle
    /// 4. Her bloku vektör olarak al ve matris çarpımı yap
    /// 5. Sonucu mod 26 ile harfe çevir
    ///
    /// Matematiksel İşlem:
    /// Her blok için: C = (K × P) mod 26
    /// C = Şifreli vektör, K = Anahtar matrisi, P = Düz metin vektörü
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Şifrelenmiş metin (büyük harfler).</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        plainText = plainText.ToUpper();
        List<char> textChars = new List<char>();

        // Sadece harfleri al
        foreach (char c in plainText)
        {
            if (char.IsLetter(c))
                textChars.Add(c);
        }

        int matrixSize = keyMatrix.GetLength(0);

        // Padding ile uzunluğu matris boyutunun katı yap
        int paddedLength = (textChars.Count + matrixSize - 1) / matrixSize * matrixSize;

        while (textChars.Count < paddedLength)
            textChars.Add('X');

        List<char> result = new List<char>();

        // Her bloğu işle
        for (int i = 0; i < textChars.Count; i += matrixSize)
        {
            // Karakterleri sayılara çevir (A=0, B=1, ..., Z=25)
            int[] vector = new int[matrixSize];

            for (int j = 0; j < matrixSize; j++)
            {
                vector[j] = textChars[i + j] - 'A';
            }

            // Matris çarpımı yap
            int[] encryptedVector = MatrixMultiply(vector);

            // Sonucu mod 26 al ve harfe çevir
            for (int j = 0; j < matrixSize; j++)
            {
                result.Add((char)(encryptedVector[j] % 26 + 'A'));
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Şifreli metni Hill şifresi ile çözer.
    ///
    /// Çözme Algoritması:
    /// 1. Anahtar matrisinin tersini hesapla (mod 26)
    /// 2. Şifreli metni bloklara böl
    /// 3. Her bloğu ters matris ile çarp
    /// 4. Sonucu mod 26 al ve harfe çevir
    ///
    /// Matematiksel İşlem:
    /// Her blok için: P = (K^-1 × C) mod 26
    /// P = Düz metin vektörü, K^-1 = Ters matris, C = Şifreli vektör
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Çözülmüş düz metin.</returns>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        cipherText = cipherText.ToUpper();
        List<char> textChars = new List<char>();

        // Sadece harfleri al
        foreach (char c in cipherText)
        {
            if (char.IsLetter(c))
                textChars.Add(c);
        }

        int matrixSize = keyMatrix.GetLength(0);

        // Ters matrisi hesapla
        int[,] inverseMatrix = InverseKeyMatrix();
        List<char> result = new List<char>();

        // Her bloğu çöz
        for (int i = 0; i < textChars.Count; i += matrixSize)
        {
            // Karakterleri sayılara çevir
            int[] vector = new int[matrixSize];

            for (int j = 0; j < matrixSize; j++)
            {
                vector[j] = textChars[i + j] - 'A';
            }

            // Ters matris ile çarp
            int[] decryptedVector = MatrixMultiplyInverse(vector, inverseMatrix);

            // Sonucu harfe çevir
            for (int j = 0; j < matrixSize; j++)
            {
                result.Add((char)(decryptedVector[j] % 26 + 'A'));
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Anahtar matrisi ile vektörü çarpar.
    ///
    /// Matris Çarpımı:
    /// result[i] = Σ(keyMatrix[i,j] * vector[j]) for j=0 to size-1
    ///
    /// Bu işlem düz metni şifreli metne dönüştürür.
    /// </summary>
    /// <param name="vector">Çarpılacak vektör (düz metin bloğu).</param>
    /// <returns>Çarpım sonucu vektör (mod almadan önce).</returns>
    private int[] MatrixMultiply(int[] vector)
    {
        int size = vector.Length;
        int[] result = new int[size];

        // Her satır için
        for (int i = 0; i < size; i++)
        {
            // Satır ile vektörün iç çarpımını hesapla
            for (int j = 0; j < size; j++)
            {
                result[i] += keyMatrix[i, j] * vector[j];
            }
        }

        return result;
    }

    /// <summary>
    /// Ters matris ile vektörü çarpar.
    ///
    /// Şifre çözme için kullanılır.
    /// Ters matris ile çarpım orijinal metni geri verir.
    /// </summary>
    /// <param name="vector">Çarpılacak vektör (şifreli metin bloğu).</param>
    /// <param name="inverse">Ters matris.</param>
    /// <returns>Çarpım sonucu vektör.</returns>
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

    /// <summary>
    /// Anahtar matrisinin mod 26'daki tersini hesaplar.
    ///
    /// Ters Matris Formülü (2x2 için):
    /// K^-1 = det(K)^-1 * adj(K) mod 26
    /// adj(K) = [d  -b]  (K = [a  b])
    ///          [-c  a]        [c  d]
    ///
    /// Not: Şu an sadece 2x2 matrisler için implement edilmiştir.
    /// 3x3 için daha karmaşık adjoint ve cofactor hesaplamaları gerekir.
    /// </summary>
    /// <returns>Ters matris (mod 26).</returns>
    private int[,] InverseKeyMatrix()
    {
        int size = keyMatrix.GetLength(0);
        int[,] inverse = new int[size, size];

        if (size == 2)
        {
            // Determinantı hesapla
            int det = Determinant(keyMatrix);

            // Determinantın mod 26'daki tersini bul
            int detInverse = ModularInverse(det, 26);

            // Adjoint matrisi oluştur
            // [d  -b]
            // [-c  a]
            inverse[0, 0] = keyMatrix[1, 1];
            inverse[0, 1] = -keyMatrix[0, 1];
            inverse[1, 0] = -keyMatrix[1, 0];
            inverse[1, 1] = keyMatrix[0, 0];

            // Her elemanı determinantın tersi ile çarp ve mod 26 al
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    inverse[i, j] = (inverse[i, j] * detInverse) % 26;

                    // Negatif değerleri düzelt
                    if (inverse[i, j] < 0)
                        inverse[i, j] += 26;
                }
            }
        }

        return inverse;
    }

    /// <summary>
    /// Matrisin determinantını hesaplar.
    ///
    /// 2x2 Matris Determinantı:
    /// det([a b]) = ad - bc
    ///     [c d]
    ///
    /// 3x3 Matris Determinantı (Sarrus Kuralı):
    /// det = a(ei-fh) - b(di-fg) + c(dh-eg)
    ///
    /// Sonuç mod 26 alınır.
    /// </summary>
    /// <param name="matrix">Determinantı hesaplanacak matris.</param>
    /// <returns>Determinant (mod 26).</returns>
    private int Determinant(int[,] matrix)
    {
        int size = matrix.GetLength(0);

        if (size == 2)
        {
            // 2x2: ad - bc
            return (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) % 26;
        }
        else if (size == 3)
        {
            // 3x3: Sarrus kuralı ile hesapla
            return (matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) -
                    matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]) +
                    matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0])) % 26;
        }

        return 0;
    }

    /// <summary>
    /// İki sayının en büyük ortak bölenini (EBOB) hesaplar.
    ///
    /// Öklid Algoritması:
    /// gcd(a, b) = gcd(b, a mod b)
    /// gcd(a, 0) = a
    ///
    /// Bu algoritma, iki sayının aralarında asal olup olmadığını kontrol etmek için kullanılır.
    /// gcd(a, b) = 1 ise a ve b aralarında asaldır.
    /// </summary>
    /// <param name="a">İlk sayı.</param>
    /// <param name="b">İkinci sayı.</param>
    /// <returns>En büyük ortak bölen.</returns>
    private int GCD(int a, int b)
    {
        // Öklid algoritması
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return Math.Abs(a);
    }

    /// <summary>
    /// Bir sayının mod m'deki çarpımsal tersini bulur.
    ///
    /// Modüler Ters:
    /// a * x ≡ 1 (mod m) eşitliğini sağlayan x değerini bulur.
    /// Örnek: 3'ün mod 26'daki tersi 9'dur çünkü 3*9 = 27 ≡ 1 (mod 26)
    ///
    /// Not: Bu basit bir brute force implementasyonu.
    /// Daha verimli yöntem: Genişletilmiş Öklid Algoritması.
    ///
    /// Ters sadece a ve m aralarında asal ise vardır (gcd(a,m) = 1).
    /// </summary>
    /// <param name="a">Tersi bulunacak sayı.</param>
    /// <param name="m">Modulo değeri.</param>
    /// <returns>Modüler ters. Bulunamazsa 1 döner.</returns>
    private int ModularInverse(int a, int m)
    {
        a = a % m;

        // 1'den m-1'e kadar dene
        for (int x = 1; x < m; x++)
        {
            if ((a * x) % m == 1)
                return x;
        }

        return 1;
    }

    /// <summary>
    /// Anahtar matrisini döndürür.
    /// </summary>
    /// <returns>Şifreleme anahtar matrisi.</returns>
    public int[,] GetKeyMatrix() => keyMatrix;

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Metin temizleme ve padding işlemini gösterir
    /// - Her bloğun vektöre nasıl dönüştüğünü açıklar
    /// - Matris çarpımının nasıl yapıldığını detaylandırır
    /// - Mod 26 işleminin sonucunu gösterir
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, "Starting Hill cipher encryption"));

        plainText = plainText.ToUpper();
        List<char> textChars = new List<char>();

        // Harf olmayan karakterleri çıkar
        foreach (char c in plainText)
        {
            if (char.IsLetter(c))
                textChars.Add(c);
        }

        steps.Add(new StepInfo(2, "Step 2: Clean Text", plainText, new string(textChars.ToArray()), "Removed non-letters"));

        int matrixSize = keyMatrix.GetLength(0);
        int paddedLength = (textChars.Count + matrixSize - 1) / matrixSize * matrixSize;

        // Padding ekle
        while (textChars.Count < paddedLength)
            textChars.Add('X');

        if (paddedLength > textChars.Count - (textChars.Count % matrixSize))
        {
            steps.Add(new StepInfo(3, "Step 3: Padding", new string(textChars.ToArray()),
                new string(textChars.ToArray()),
                $"Added 'X' padding to make length multiple of {matrixSize}"));
        }

        List<char> result = new List<char>();
        int stepNum = 4;
        int blockNum = 1;

        // Her bloğu işle
        for (int i = 0; i < textChars.Count; i += matrixSize)
        {
            int[] vector = new int[matrixSize];
            string vectorStr = "";

            // Vektörü oluştur
            for (int j = 0; j < matrixSize; j++)
            {
                vector[j] = textChars[i + j] - 'A';
                vectorStr += $"{textChars[i + j]}({vector[j]}) ";
            }

            // Matris çarpımı yap
            int[] encryptedVector = MatrixMultiply(vector);
            string encryptedStr = "";

            for (int j = 0; j < matrixSize; j++)
            {
                char enc = (char)(encryptedVector[j] % 26 + 'A');
                result.Add(enc);
                encryptedStr += $"{enc} ";
            }

            steps.Add(new StepInfo(stepNum++, $"Block {blockNum++}: Matrix Multiply",
                vectorStr.Trim(),
                encryptedStr.Trim(),
                $"Multiplied vector by key matrix"));
        }

        steps.Add(new StepInfo(stepNum, "Final Result", plainText, new string(result.ToArray()), "Encryption complete"));

        return steps;
    }

    /// <summary>
    /// Şifre çözme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Ters matris hesaplamasını gösterir
    /// - Her bloğun nasıl çözüldüğünü açıklar
    /// - Ters matris çarpımının sonucunu detaylandırır
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(cipherText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Cipher Text", cipherText, "Starting Hill cipher decryption"));

        cipherText = cipherText.ToUpper();
        List<char> textChars = new List<char>();

        // Sadece harfleri al
        foreach (char c in cipherText)
        {
            if (char.IsLetter(c))
                textChars.Add(c);
        }

        steps.Add(new StepInfo(2, "Step 2: Clean Text", cipherText, new string(textChars.ToArray()), "Removed non-letters"));

        int matrixSize = keyMatrix.GetLength(0);

        // Ters matrisi hesapla
        int[,] inverseMatrix = InverseKeyMatrix();

        steps.Add(new StepInfo(3, "Step 3: Calculate Inverse Matrix", "Key Matrix", "Inverse Matrix", "Calculated inverse of key matrix (mod 26)"));

        List<char> result = new List<char>();
        int stepNum = 4;
        int blockNum = 1;

        // Her bloğu çöz
        for (int i = 0; i < textChars.Count; i += matrixSize)
        {
            int[] vector = new int[matrixSize];
            string vectorStr = "";

            for (int j = 0; j < matrixSize; j++)
            {
                vector[j] = textChars[i + j] - 'A';
                vectorStr += $"{textChars[i + j]}({vector[j]}) ";
            }

            // Ters matris ile çarp
            int[] decryptedVector = MatrixMultiplyInverse(vector, inverseMatrix);
            string decryptedStr = "";

            for (int j = 0; j < matrixSize; j++)
            {
                char dec = (char)(decryptedVector[j] % 26 + 'A');
                result.Add(dec);
                decryptedStr += $"{dec} ";
            }

            steps.Add(new StepInfo(stepNum++, $"Block {blockNum++}: Matrix Multiply",
                vectorStr.Trim(),
                decryptedStr.Trim(),
                $"Multiplied vector by inverse key matrix"));
        }

        // Padding'i temizle
        string final = new string(result.ToArray()).TrimEnd('X');
        steps.Add(new StepInfo(stepNum, "Final Result", cipherText, final, "Decryption complete"));

        return steps;
    }
}
