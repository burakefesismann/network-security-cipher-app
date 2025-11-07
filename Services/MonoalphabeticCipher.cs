namespace SecurityProject.Services;

/// <summary>
/// Monoalfabetik (Tek Alfabeli) Şifreleme algoritmasını uygulayan sınıf.
///
/// Monoalfabetik Şifre Nedir?
/// Bu şifreleme yönteminde, alfabedeki her harf başka bir harfle değiştirilir.
/// Değiştirme kuralları bir anahtar tarafından belirlenir ve tüm metin boyunca sabittir.
///
/// Örnek:
/// Anahtar: QWERTYUIOPASDFGHJKLZXCVBNM
/// Normal : ABCDEFGHIJKLMNOPQRSTUVWXYZ
/// A harfi → Q, B harfi → W, C harfi → E olur
///
/// Caesar Şifresinden Farkı:
/// - Caesar: Tüm harfler aynı miktarda kaydırılır (26 olası anahtar)
/// - Monoalfabetik: Her harf farklı bir harfle değiştirilebilir (26! = yaklaşık 4x10^26 olası anahtar)
///
/// Güvenlik:
/// Caesar'dan çok daha güvenlidir ancak frekans analizi ile kırılabilir.
/// Dildeki harf kullanım sıklıklarına bakarak şifre çözülebilir.
/// </summary>
public class MonoalphabeticCipher
{
    /// <summary>
    /// Şifreleme için kullanılan eşleme tablosu.
    /// Anahtar: Normal alfabe harfi (A-Z)
    /// Değer: Şifreli karşılığı
    /// </summary>
    private Dictionary<char, char> encryptionMap;

    /// <summary>
    /// Şifre çözme için kullanılan ters eşleme tablosu.
    /// Anahtar: Şifreli harf
    /// Değer: Orijinal harf (A-Z)
    /// </summary>
    private Dictionary<char, char> decryptionMap;

    /// <summary>
    /// MonoalphabeticCipher sınıfının yeni bir örneğini oluşturur.
    ///
    /// Anahtar Gereksinimleri:
    /// - Tam olarak 26 karakter içermelidir
    /// - Her harf (A-Z) tam olarak bir kez bulunmalıdır
    /// - Tekrarlayan harf olmamalıdır
    ///
    /// Çalışma Mantığı:
    /// Anahtar, normal alfabenin (A-Z) şifreli karşılığını temsil eder.
    /// Örnek: Anahtar "QWERTY..." ise A→Q, B→W, C→E olur.
    /// </summary>
    /// <param name="key">26 benzersiz harften oluşan şifreleme anahtarı.</param>
    /// <exception cref="ArgumentException">
    /// Anahtar boş, null, 26 karakterden farklı uzunlukta veya tekrar eden harf içeriyorsa fırlatılır.
    /// </exception>
    public MonoalphabeticCipher(string key)
    {
        // Anahtar uzunluk kontrolü
        if (string.IsNullOrEmpty(key) || key.Length != 26)
            throw new ArgumentException("Key must contain exactly 26 unique letters");

        // Benzersiz karakter kontrolü - her harf bir kez kullanılmalı
        var uniqueChars = key.ToUpper().Distinct().ToList();
        if (uniqueChars.Count != 26)
            throw new ArgumentException("Key must contain 26 unique letters");

        // Eşleme tablolarını oluştur
        encryptionMap = new Dictionary<char, char>();
        decryptionMap = new Dictionary<char, char>();

        // Her normal harf için şifreli karşılığını belirle
        // A→key[0], B→key[1], ..., Z→key[25]
        for (int i = 0; i < 26; i++)
        {
            // Şifreleme eşlemesi: Normal harf → Şifreli harf
            encryptionMap[(char)('A' + i)] = key.ToUpper()[i];

            // Şifre çözme eşlemesi: Şifreli harf → Normal harf
            decryptionMap[key.ToUpper()[i]] = (char)('A' + i);
        }
    }

    /// <summary>
    /// Düz metni monoalfabetik şifreleme ile şifreler.
    ///
    /// İşlem Adımları:
    /// 1. Her harfi büyük harfe çevir
    /// 2. Eşleme tablosundan karşılığını bul
    /// 3. Orijinal büyük/küçük harf durumunu koru
    /// 4. Harf olmayan karakterler değişmeden kalır
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Şifrelenmiş metin.</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        char[] result = new char[plainText.Length];

        // Her karakteri işle
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];

            if (char.IsLetter(c))
            {
                // Harfi büyük harfe çevir ve eşleme tablosunda ara
                // Sonucu orijinal büyük/küçük harf durumuna göre ayarla
                result[i] = char.IsUpper(c)
                    ? encryptionMap[c]  // Büyük harf ise doğrudan kullan
                    : char.ToLower(encryptionMap[char.ToUpper(c)]);  // Küçük harf ise sonucu küçült
            }
            else
            {
                // Harf olmayan karakterler (boşluk, noktalama vb.) aynen kalsın
                result[i] = c;
            }
        }

        return new string(result);
    }

    /// <summary>
    /// Şifreli metni çözerek orijinal düz metne geri döndürür.
    ///
    /// Çözme İşlemi:
    /// Şifreleme işleminin tersidir. Ters eşleme tablosu (decryptionMap) kullanılır.
    /// Şifreli harf → Orijinal harf dönüşümü yapılır.
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Çözülmüş düz metin.</returns>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        char[] result = new char[cipherText.Length];

        // Her karakteri işle
        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];

            if (char.IsLetter(c))
            {
                // Şifreli harfi ters eşleme tablosunda ara ve orijinal halini bul
                // Büyük/küçük harf durumunu koru
                result[i] = char.IsUpper(c)
                    ? decryptionMap[c]  // Büyük harf ise doğrudan kullan
                    : char.ToLower(decryptionMap[char.ToUpper(c)]);  // Küçük harf ise sonucu küçült
            }
            else
            {
                // Harf olmayan karakterler aynen kalsın
                result[i] = c;
            }
        }

        return new string(result);
    }

    /// <summary>
    /// Rastgele bir monoalfabetik şifreleme anahtarı üretir.
    ///
    /// Anahtar Üretimi:
    /// 1. Standart alfabeyi (A-Z) alır
    /// 2. Fisher-Yates karıştırma algoritması ile rastgele sıralar
    /// 3. Karıştırılmış alfabeyi anahtar olarak döndürür
    ///
    /// Fisher-Yates Algoritması:
    /// - En verimli rastgele karıştırma algoritmasıdır
    /// - Her permütasyon eşit olasılıkla üretilir
    /// - O(n) zaman karmaşıklığına sahiptir
    ///
    /// Kullanım:
    /// Güvenli bir anahtar oluşturmak için kullanılır.
    /// Her çalıştırmada farklı bir anahtar üretir.
    /// </summary>
    /// <returns>26 harften oluşan rastgele karıştırılmış anahtar dizesi.</returns>
    public static string GenerateRandomKey()
    {
        Random random = new Random();
        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();

        // Fisher-Yates karıştırma algoritması
        // Sondan başa doğru her eleman için:
        for (int i = alphabet.Count - 1; i > 0; i--)
        {
            // 0 ile i arasında rastgele bir indeks seç
            int j = random.Next(i + 1);

            // i ve j pozisyonlarındaki elemanları yer değiştir (swap)
            // C# 7.0+ tuple syntax kullanarak tek satırda swap
            (alphabet[i], alphabet[j]) = (alphabet[j], alphabet[i]);
        }

        // Karıştırılmış listeyi string'e çevir ve döndür
        return new string(alphabet.ToArray());
    }

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// Her karakterin hangi karaktere dönüştüğünü gösterir.
    /// Eşleme tablosunun nasıl kullanıldığını açıklar.
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        // Başlangıç adımı
        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, "Starting encryption with substitution key"));

        var result = new System.Text.StringBuilder();
        int stepNum = 2;

        // Her karakteri işle ve adımları kaydet
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];

            if (char.IsLetter(c))
            {
                // Büyük harfe çevir ve eşleme tablosundan bul
                char upperC = char.ToUpper(c);
                char encrypted = encryptionMap[upperC];

                // Orijinal küçük harf ise sonucu da küçült
                if (char.IsLower(c))
                    encrypted = char.ToLower(encrypted);

                result.Append(encrypted);

                // Bu dönüşüm adımını kaydet
                steps.Add(new StepInfo(stepNum++, $"Map '{c}'",
                    $"Input: {c}",
                    $"Output: {encrypted}",
                    $"{c} → {encrypted} (using substitution table)"));
            }
            else
            {
                // Harf olmayan karakterler değişmez
                result.Append(c);
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'",
                    "Non-letter character",
                    c.ToString(),
                    "Non-letter characters remain unchanged"));
            }
        }

        // Son adım: Tamamlanma
        steps.Add(new StepInfo(stepNum, "Final Result", plainText, result.ToString(), "Encryption complete"));

        return steps;
    }

    /// <summary>
    /// Şifre çözme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// Her şifreli karakterin hangi orijinal karaktere dönüştüğünü gösterir.
    /// Ters eşleme tablosunun nasıl kullanıldığını açıklar.
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(cipherText))
            return steps;

        // Başlangıç adımı
        steps.Add(new StepInfo(1, "Initialization", "Cipher Text", cipherText, "Starting decryption with substitution key"));

        var result = new System.Text.StringBuilder();
        int stepNum = 2;

        // Her şifreli karakteri işle
        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];

            if (char.IsLetter(c))
            {
                // Büyük harfe çevir ve ters eşleme tablosundan orijinal harfi bul
                char upperC = char.ToUpper(c);
                char decrypted = decryptionMap[upperC];

                // Orijinal küçük harf ise sonucu da küçült
                if (char.IsLower(c))
                    decrypted = char.ToLower(decrypted);

                result.Append(decrypted);

                // Bu çözme adımını kaydet
                steps.Add(new StepInfo(stepNum++, $"Map '{c}'",
                    $"Input: {c}",
                    $"Output: {decrypted}",
                    $"{c} → {decrypted} (using reverse substitution table)"));
            }
            else
            {
                // Harf olmayan karakterler değişmez
                result.Append(c);
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'",
                    "Non-letter character",
                    c.ToString(),
                    "Non-letter characters remain unchanged"));
            }
        }

        // Son adım: Tamamlanma
        steps.Add(new StepInfo(stepNum, "Final Result", cipherText, result.ToString(), "Decryption complete"));

        return steps;
    }
}
