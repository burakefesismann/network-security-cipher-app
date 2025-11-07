namespace SecurityProject.Services;

/// <summary>
/// Transpozisyon (Yer Değiştirme) Şifreleme algoritmasını uygulayan sınıf.
///
/// Transpozisyon Şifresi Nedir?
/// Karakterleri değiştirmek yerine yerlerini değiştirerek şifreleme yapan bir yöntemdir.
/// Harflerin kendisi değişmez, sadece sıralaması karıştırılır.
///
/// Çalışma Prensibi:
/// 1. Metin, belirli sütun sayısına (anahtar) sahip bir matrise yerleştirilir
/// 2. Satır satır (soldan sağa) yazılır
/// 3. Sütun sütun (yukarıdan aşağıya) okunur
/// 4. Bu, karakterlerin yerlerinin değişmesine neden olur
///
/// Örnek (Anahtar = 3):
/// Düz Metin: "HELLO"
/// Matrise Yerleştirme (3 sütun):
/// H E L
/// L O X  (padding)
///
/// Sütun sütun okuma:
/// 1. sütun: H, L
/// 2. sütun: E, O
/// 3. sütun: L, X
/// Sonuç: HLEOLX
///
/// Şifre Türleri:
/// - Yukarıdaki örnekte kullanılan yönteme "Columnar Transposition" denir
/// - Rail Fence, Route Cipher gibi çeşitleri vardır
/// - Bu implement, en basit columnar transposition'dır
///
/// Önemli Not:
/// Transpozisyon şifreleri tek başına zayıftır.
/// Genellikle değiştirme (substitution) şifreleriyle birlikte kullanılır.
/// Bu kombinasyona "Product Cipher" denir (örn: Enigma makinesi).
///
/// Güvenlik:
/// - Frekans analizine karşı dirençlidir (harfler değişmediği için)
/// - Anagram çözme teknikleri ile kırılabilir
/// - Kısa anahtarlar kolayca deneme-yanılma ile kırılır
/// </summary>
public class TranspositionCipher
{
    /// <summary>
    /// Şifreleme anahtarı - matrisin sütun sayısını belirler.
    /// Daha yüksek değerler daha güvenlidir ancak daha fazla padding gerektirir.
    /// </summary>
    private int key;

    /// <summary>
    /// TranspositionCipher sınıfının yeni bir örneğini oluşturur.
    ///
    /// Anahtar Özellikleri:
    /// - Pozitif bir tam sayı olmalıdır
    /// - Küçük anahtarlar (2-3) çok zayıftır
    /// - Büyük anahtarlar daha güvenlidir ancak padding artırır
    /// - İdeal: sqrt(metin_uzunluğu) civarında
    /// </summary>
    /// <param name="key">Matris sütun sayısı (pozitif tam sayı).</param>
    /// <exception cref="ArgumentException">Anahtar 0 veya negatifse fırlatılır.</exception>
    public TranspositionCipher(int key)
    {
        if (key <= 0)
            throw new ArgumentException("Key must be positive");

        this.key = key;
    }

    /// <summary>
    /// Düz metni transpozisyon şifresi ile şifreler.
    ///
    /// Şifreleme Algoritması:
    /// 1. Metni büyük harfe çevir ve boşlukları temizle
    /// 2. Sütun sayısına göre satır sayısını hesapla
    /// 3. Metni matrise satır satır yerleştir
    /// 4. Eksik hücreler için 'X' padding ekle
    /// 5. Matrisi sütun sütun oku ve şifreli metni oluştur
    ///
    /// Matris Boyutları:
    /// Sütun sayısı = anahtar
    /// Satır sayısı = ceil(metin_uzunluğu / anahtar)
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Şifrelenmiş metin (büyük harfler, padding ile).</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        plainText = plainText.ToUpper();

        // Basitlik için boşlukları kaldır
        // Not: Boşluklar şifrelemeye dahil edilebilir ancak bu örnekte kaldırıldı
        string cleanedText = plainText.Replace(" ", "");

        int cols = key;
        // Satır sayısını hesapla (yukarı yuvarla)
        int rows = (int)Math.Ceiling((double)cleanedText.Length / cols);

        // Matris oluştur
        char[,] matrix = new char[rows, cols];

        // Matrise metni satır satır yerleştir
        int charIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (charIndex < cleanedText.Length)
                    matrix[row, col] = cleanedText[charIndex++];
                else
                    matrix[row, col] = 'X'; // Padding ekle
            }
        }

        // Sütun sütun oku
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

    /// <summary>
    /// Şifreli metni transpozisyon algoritması ile çözer.
    ///
    /// Çözme Algoritması:
    /// 1. Sütun sayısına göre satır sayısını hesapla
    /// 2. Matrise metni sütun sütun yerleştir
    /// 3. Matrisi satır satır oku ve düz metni elde et
    /// 4. Sondaki 'X' padding'i temizle
    ///
    /// Not:
    /// Şifre çözme, şifrelemenin tersidir.
    /// Yazma yönü: Sütun sütun
    /// Okuma yönü: Satır satır
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Çözülmüş düz metin (padding temizlenmiş).</returns>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        cipherText = cipherText.ToUpper();

        int cols = key;
        int rows = (int)Math.Ceiling((double)cipherText.Length / cols);

        char[,] matrix = new char[rows, cols];

        // Matrise metni sütun sütun yerleştir
        int charIndex = 0;
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (charIndex < cipherText.Length)
                    matrix[row, col] = cipherText[charIndex++];
            }
        }

        // Satır satır oku
        List<char> result = new List<char>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                result.Add(matrix[row, col]);
            }
        }

        // Sondaki 'X' padding'i temizle
        // TrimEnd kullanımı: Sadece sondaki X'leri kaldırır, ortadaki X'lere dokunmaz
        return new string(result.ToArray()).TrimEnd('X');
    }

    /// <summary>
    /// Kullanılan anahtarı (sütun sayısını) döndürür.
    /// </summary>
    /// <returns>Matris sütun sayısı.</returns>
    public int GetKey() => key;

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Metin temizleme işlemini gösterir
    /// - Matris boyutlarının nasıl hesaplandığını açıklar
    /// - Satır satır yazma işlemini detaylandırır
    /// - Sütun sütun okuma işlemini gösterir
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        // Adım 1: Başlangıç
        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, $"Starting transposition encryption with key = {key}"));

        // Adım 2: Metin temizleme
        plainText = plainText.ToUpper();
        string cleanedText = plainText.Replace(" ", "");
        steps.Add(new StepInfo(2, "Step 2: Clean Text", plainText, cleanedText, "Removed spaces"));

        int cols = key;
        int rows = (int)Math.Ceiling((double)cleanedText.Length / cols);

        // Adım 3: Matris boyutları
        steps.Add(new StepInfo(3, "Step 3: Create Matrix", cleanedText, $"Matrix: {rows}x{cols}", $"Created {rows} rows × {cols} columns matrix"));

        char[,] matrix = new char[rows, cols];
        int charIndex = 0;

        // Matrise yerleştir ve görselleştir
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

        // Adım 4: Matrise satır satır yazma
        steps.Add(new StepInfo(4, "Step 4: Fill Matrix Row-wise", cleanedText, matrixStr.Trim(), "Filled matrix row by row"));

        // Sütun sütun okuma
        List<char> result = new List<char>();
        string readOrder = "";
        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                result.Add(matrix[row, col]);
                readOrder += matrix[row, col];
            }
            readOrder += " | "; // Sütunları ayır
        }

        // Adım 5: Sütun sütun okuma
        steps.Add(new StepInfo(5, "Step 5: Read Column-wise", matrixStr.Trim(), readOrder.TrimEnd(' ', '|'), "Read matrix column by column"));

        // Son adım
        steps.Add(new StepInfo(6, "Final Result", plainText, new string(result.ToArray()), "Encryption complete"));

        return steps;
    }

    /// <summary>
    /// Şifre çözme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Matris boyutlarının nasıl hesaplandığını gösterir
    /// - Sütun sütun yazma işlemini detaylandırır
    /// - Satır satır okuma işlemini gösterir
    /// - Padding temizleme işlemini açıklar
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(cipherText))
            return steps;

        // Adım 1: Başlangıç
        steps.Add(new StepInfo(1, "Step 1: Input", "Cipher Text", cipherText, $"Starting transposition decryption with key = {key}"));

        cipherText = cipherText.ToUpper();
        int cols = key;
        int rows = (int)Math.Ceiling((double)cipherText.Length / cols);

        // Adım 2: Matris boyutları
        steps.Add(new StepInfo(2, "Step 2: Create Matrix", cipherText, $"Matrix: {rows}x{cols}", $"Created {rows} rows × {cols} columns matrix"));

        char[,] matrix = new char[rows, cols];
        int charIndex = 0;

        // Sütun sütun yerleştir ve görselleştir
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

        // Adım 3: Sütun sütun yazma
        steps.Add(new StepInfo(3, "Step 3: Fill Matrix Column-wise", cipherText, matrixStr.Trim(), "Filled matrix column by column"));

        // Satır satır okuma
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

        // Adım 4: Satır satır okuma
        steps.Add(new StepInfo(4, "Step 4: Read Row-wise", matrixStr.Trim(), readOrder.Trim(), "Read matrix row by row"));

        // Padding'i temizle
        string final = new string(result.ToArray()).TrimEnd('X');

        // Son adım
        steps.Add(new StepInfo(5, "Final Result", cipherText, final, "Decryption complete"));

        return steps;
    }
}
