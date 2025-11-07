namespace SecurityProject.Services;

/// <summary>
/// Playfair Şifreleme algoritmasını uygulayan sınıf.
///
/// Playfair Şifresi Nedir?
/// 1854'te Charles Wheatstone tarafından icat edilen,
/// ancak Lord Playfair tarafından popülerleştirilen bir şifreleme yöntemidir.
/// Tarihte yaygın kullanılan ilk digraf (iki harfli) şifreleme tekniğidir.
///
/// Temel Prensipler:
/// 1. 5x5'lik bir anahtar matrisi kullanılır
/// 2. Metin çiftler (digraph) halinde işlenir
/// 3. I ve J harfleri aynı kabul edilir (26 harf → 25 hücre)
/// 4. Aynı harfin yan yana gelmesi durumunda araya 'X' eklenir
///
/// Şifreleme Kuralları:
/// - Aynı satırdaki harfler: Sağa doğru kaydırılır (döngüsel)
/// - Aynı sütundaki harfler: Aşağı doğru kaydırılır (döngüsel)
/// - Dikdörtgen oluşturan harfler: Sütunlar yer değiştirilir
///
/// Örnek Anahtar Matrisi ("PLAYFAIR" anahtarı ile):
/// P L A Y F
/// I R B C D
/// E G H K M
/// N O Q S T
/// U V W X Z
///
/// Güvenlik:
/// Monoalfabetik şifrelerden daha güvenlidir çünkü frekans analizini zorlaştırır.
/// Ancak modern standartlara göre zayıftır ve kriptanaliz ile kırılabilir.
/// </summary>
public class PlayfairCipher
{
    /// <summary>
    /// 5x5'lik şifreleme matrisi.
    /// Anahtar ve alfabeden oluşturulur.
    /// </summary>
    private char[,] keyMatrix;

    /// <summary>
    /// Her karakterin matristeki pozisyonunu (satır, sütun) saklayan dictionary.
    /// Hızlı erişim için kullanılır - O(1) karmaşıklık.
    /// </summary>
    private Dictionary<char, (int row, int col)> positionMap;

    /// <summary>
    /// PlayfairCipher sınıfının yeni bir örneğini oluşturur.
    ///
    /// Matris Oluşturma Süreci:
    /// 1. Anahtardaki harfler (tekrarsız) önce yerleştirilir
    /// 2. Kalan alfabe harfleri sırayla eklenir
    /// 3. J harfi atlayarak I kullanılır
    /// </summary>
    /// <param name="key">Matris oluşturmak için kullanılacak anahtar kelime.</param>
    public PlayfairCipher(string key)
    {
        keyMatrix = new char[5, 5];
        positionMap = new Dictionary<char, (int, int)>();

        BuildKeyMatrix(key);
    }

    /// <summary>
    /// Verilen anahtardan 5x5'lik Playfair matrisini oluşturur.
    ///
    /// Algoritma:
    /// 1. J harfini kullanılmış olarak işaretle (I ile birleştirilecek)
    /// 2. Anahtar kelimesindeki her harfi (tekrarsız) matrise ekle
    /// 3. Kalan alfabeyi (A-Z, J hariç) sırayla ekle
    /// 4. Her harfin pozisyonunu positionMap'e kaydet
    ///
    /// Not: J harfi her zaman I ile aynı kabul edilir.
    /// </summary>
    /// <param name="key">Anahtar kelime.</param>
    private void BuildKeyMatrix(string key)
    {
        HashSet<char> used = new HashSet<char> { 'J' }; // I ve J aynı kabul edilir
        char[] keyChars = key.ToUpper().Replace('J', 'I').ToCharArray();

        int row = 0, col = 0;

        // Önce anahtar kelimesindeki harfleri yerleştir
        foreach (char c in keyChars)
        {
            // Sadece harf karakterleri ve henüz kullanılmayanları al
            if (char.IsLetter(c) && !used.Contains(c))
            {
                keyMatrix[row, col] = c;
                positionMap[c] = (row, col);
                used.Add(c);

                // Sonraki hücreye geç
                col++;
                if (col == 5)  // Satır tamamlandı
                {
                    col = 0;
                    row++;
                }
            }
        }

        // Kalan alfabeyi ekle (J hariç)
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (c == 'J') continue;  // J'yi atla

            if (!used.Contains(c))
            {
                keyMatrix[row, col] = c;
                positionMap[c] = (row, col);
                used.Add(c);

                col++;
                if (col == 5)  // Satır tamamlandı
                {
                    col = 0;
                    row++;
                }
            }
        }
    }

    /// <summary>
    /// Düz metni Playfair algoritması ile şifreler.
    ///
    /// Şifreleme Adımları:
    /// 1. Metni hazırla (harf dışı karakterleri çıkar, J→I dönüşümü)
    /// 2. Çift harf varsa araya 'X' ekle
    /// 3. Tek sayıda harf varsa sona 'X' ekle
    /// 4. Her harf çiftini Playfair kurallarına göre şifrele
    ///
    /// Playfair Kuralları:
    /// - Aynı satır: Her harf bir sağdaki harfle değiştirilir
    /// - Aynı sütun: Her harf bir alttaki harfle değiştirilir
    /// - Dikdörtgen: Harfler kendi satırlarında kalır, sütunlar değişir
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Şifrelenmiş metin (sadece büyük harfler).</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        // Metni hazırla: Temizle, J→I çevir, çift harfleri ayır
        string preparedText = PrepareText(plainText);

        // Tek sayıda karakter varsa sona 'X' ekle
        if (preparedText.Length % 2 != 0)
            preparedText += 'X';

        List<char> result = new List<char>();

        // Her çifti işle
        for (int i = 0; i < preparedText.Length; i += 2)
        {
            char c1 = preparedText[i];
            char c2 = preparedText[i + 1];

            // Her iki harfin de matristeki pozisyonunu al
            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];

            if (r1 == r2)  // Aynı satırdalar
            {
                // Her harf bir sağdaki harfle değiştirilir (mod 5 ile döngüsel)
                result.Add(keyMatrix[r1, (c1_pos + 1) % 5]);
                result.Add(keyMatrix[r2, (c2_pos + 1) % 5]);
            }
            else if (c1_pos == c2_pos)  // Aynı sütundalar
            {
                // Her harf bir alttaki harfle değiştirilir (mod 5 ile döngüsel)
                result.Add(keyMatrix[(r1 + 1) % 5, c1_pos]);
                result.Add(keyMatrix[(r2 + 1) % 5, c2_pos]);
            }
            else  // Dikdörtgen oluştururlar
            {
                // Her harf kendi satırında kalır, diğerinin sütununa geçer
                result.Add(keyMatrix[r1, c2_pos]);
                result.Add(keyMatrix[r2, c1_pos]);
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Şifreli metni Playfair algoritması ile çözer.
    ///
    /// Çözme Kuralları (Şifrelemenin tersi):
    /// - Aynı satır: Her harf bir soldaki harfle değiştirilir
    /// - Aynı sütun: Her harf bir üstteki harfle değiştirilir
    /// - Dikdörtgen: Şifreleme ile aynı işlem (sütun değiştirme simetriktir)
    ///
    /// Not: +4 kullanımı -1 ile aynıdır (mod 5): (x + 4) % 5 = (x - 1) % 5
    /// Bu, negatif sonuçları önler.
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Çözülmüş metin.</returns>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText) || cipherText.Length % 2 != 0)
            return string.Empty;

        List<char> result = new List<char>();

        // Her çifti işle
        for (int i = 0; i < cipherText.Length; i += 2)
        {
            char c1 = cipherText[i];
            char c2 = cipherText[i + 1];

            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];

            if (r1 == r2)  // Aynı satır
            {
                // Her harf bir soldaki harfle değiştirilir
                // +4 kullanımı -1 ile aynıdır (mod 5 aritmetiğinde)
                result.Add(keyMatrix[r1, (c1_pos + 4) % 5]);
                result.Add(keyMatrix[r2, (c2_pos + 4) % 5]);
            }
            else if (c1_pos == c2_pos)  // Aynı sütun
            {
                // Her harf bir üstteki harfle değiştirilir
                result.Add(keyMatrix[(r1 + 4) % 5, c1_pos]);
                result.Add(keyMatrix[(r2 + 4) % 5, c2_pos]);
            }
            else  // Dikdörtgen
            {
                // Sütun değiştirme simetriktir
                result.Add(keyMatrix[r1, c2_pos]);
                result.Add(keyMatrix[r2, c1_pos]);
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Metni Playfair şifrelemesi için hazırlar.
    ///
    /// Hazırlama Adımları:
    /// 1. Tüm harfleri büyük harfe çevir
    /// 2. J harflerini I'ya dönüştür
    /// 3. Harf olmayan karakterleri çıkar
    /// 4. Yan yana aynı harfler varsa araya 'X' ekle
    ///
    /// Örnek: "HELLO" → "HEL", "LO" çiftleri
    /// "BALLOON" → "BA", "LX", "LO", "ON" çiftleri (LL arasına X eklendi)
    /// </summary>
    /// <param name="text">Ham metin.</param>
    /// <returns>Hazırlanmış metin (sadece harfler, J→I).</returns>
    private string PrepareText(string text)
    {
        text = text.ToUpper().Replace('J', 'I');
        List<char> result = new List<char>();

        // Sadece harfleri al
        foreach (char c in text)
        {
            if (char.IsLetter(c))
                result.Add(c);
        }

        // Yan yana aynı harfler varsa araya 'X' ekle
        // Not: i+=2 yapıyoruz çünkü 'X' ekledikten sonraki karakteri zaten kontrol ettik
        for (int i = 0; i < result.Count - 1; i += 2)
        {
            if (result[i] == result[i + 1])
            {
                result.Insert(i + 1, 'X');
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Anahtar matrisini döndürür.
    /// </summary>
    /// <returns>5x5'lik anahtar matrisi.</returns>
    public char[,] GetKeyMatrix() => keyMatrix;

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Metin hazırlama sürecini gösterir
    /// - Her harf çiftinin nasıl şifrelendiğini açıklar
    /// - Hangi kuralın uygulandığını belirtir (aynı satır/sütun/dikdörtgen)
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, "Starting Playfair encryption"));

        // Adım 2: Metni hazırla
        string preparedText = PrepareText(plainText);
        steps.Add(new StepInfo(2, "Step 2: Prepare Text", plainText, preparedText, "Removed non-letters, replaced J with I"));

        // Adım 3: Gerekirse padding ekle
        if (preparedText.Length % 2 != 0)
        {
            preparedText += 'X';
            steps.Add(new StepInfo(3, "Step 3: Add Padding", preparedText.Substring(0, preparedText.Length - 1), preparedText, "Added 'X' to make even length"));
        }

        List<char> result = new List<char>();
        int stepNum = preparedText.Length % 2 != 0 ? 4 : 3;
        int pairNum = 1;

        // Her çifti şifrele ve adımları kaydet
        for (int i = 0; i < preparedText.Length; i += 2)
        {
            char c1 = preparedText[i];
            char c2 = preparedText[i + 1];

            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];

            char e1, e2;
            string rule;

            // Hangi kural uygulanacak?
            if (r1 == r2)  // Aynı satır
            {
                e1 = keyMatrix[r1, (c1_pos + 1) % 5];
                e2 = keyMatrix[r2, (c2_pos + 1) % 5];
                rule = "Same row: shift right";
            }
            else if (c1_pos == c2_pos)  // Aynı sütun
            {
                e1 = keyMatrix[(r1 + 1) % 5, c1_pos];
                e2 = keyMatrix[(r2 + 1) % 5, c2_pos];
                rule = "Same column: shift down";
            }
            else  // Dikdörtgen
            {
                e1 = keyMatrix[r1, c2_pos];
                e2 = keyMatrix[r2, c1_pos];
                rule = "Rectangle: swap columns";
            }

            result.Add(e1);
            result.Add(e2);

            // Bu çift için adım ekle
            steps.Add(new StepInfo(stepNum++, $"Pair {pairNum++}: {c1}{c2}",
                $"{c1} at ({r1},{c1_pos}), {c2} at ({r2},{c2_pos})",
                $"{e1}{e2}",
                rule));
        }

        steps.Add(new StepInfo(stepNum, "Final Result", plainText, new string(result.ToArray()), "Encryption complete"));

        return steps;
    }

    /// <summary>
    /// Şifre çözme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// Her şifreli çiftin nasıl çözüldüğünü gösterir.
    /// Ters kuralların (sola, yukarı kaydırma) nasıl uygulandığını açıklar.
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(cipherText) || cipherText.Length % 2 != 0)
            return steps;

        steps.Add(new StepInfo(1, "Step 1: Input", "Cipher Text", cipherText, "Starting Playfair decryption"));

        List<char> result = new List<char>();
        int stepNum = 2;
        int pairNum = 1;

        // Her çifti çöz
        for (int i = 0; i < cipherText.Length; i += 2)
        {
            char c1 = cipherText[i];
            char c2 = cipherText[i + 1];

            var (r1, c1_pos) = positionMap[c1];
            var (r2, c2_pos) = positionMap[c2];

            char d1, d2;
            string rule;

            // Hangi kural uygulanacak?
            if (r1 == r2)  // Aynı satır - sola kaydır
            {
                d1 = keyMatrix[r1, (c1_pos + 4) % 5];
                d2 = keyMatrix[r2, (c2_pos + 4) % 5];
                rule = "Same row: shift left";
            }
            else if (c1_pos == c2_pos)  // Aynı sütun - yukarı kaydır
            {
                d1 = keyMatrix[(r1 + 4) % 5, c1_pos];
                d2 = keyMatrix[(r2 + 4) % 5, c2_pos];
                rule = "Same column: shift up";
            }
            else  // Dikdörtgen - sütun değiştir (aynı işlem)
            {
                d1 = keyMatrix[r1, c2_pos];
                d2 = keyMatrix[r2, c1_pos];
                rule = "Rectangle: swap columns";
            }

            result.Add(d1);
            result.Add(d2);

            // Bu çift için adım ekle
            steps.Add(new StepInfo(stepNum++, $"Pair {pairNum++}: {c1}{c2}",
                $"{c1} at ({r1},{c1_pos}), {c2} at ({r2},{c2_pos})",
                $"{d1}{d2}",
                rule));
        }

        steps.Add(new StepInfo(stepNum, "Final Result", cipherText, new string(result.ToArray()), "Decryption complete"));

        return steps;
    }
}
