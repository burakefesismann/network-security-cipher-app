namespace SecurityProject.Services;

/// <summary>
/// Vigenere Şifreleme algoritmasını uygulayan sınıf.
///
/// Vigenere Şifresi Nedir?
/// 16. yüzyılda Blaise de Vigenere tarafından geliştirilen polialfabetik bir şifreleme yöntemidir.
/// "Kırılmaz şifre" (Le Chiffre Indéchiffrable) olarak bilinir ancak bu tam doğru değildir.
///
/// Temel Prensipler:
/// 1. Bir anahtar kelime kullanılır (örn: "KEY")
/// 2. Anahtar, metin uzunluğu boyunca tekrar edilir
/// 3. Her düz metin harfi, karşılık gelen anahtar harfi kadar kaydırılır
/// 4. Her pozisyonda farklı kaydırma miktarı kullanılır
///
/// Caesar'dan Farkı:
/// - Caesar: Tüm harfler aynı miktarda kaydırılır (monoalfabetik)
/// - Vigenere: Her harf farklı miktarda kaydırılabilir (polialfabetik)
///
/// Örnek:
/// Düz metin: HELLO
/// Anahtar  : KEYKE (KEY kelimesi tekrarlanarak)
/// K=10, E=4, Y=24 kaydırma demektir
/// H+K: H(7)+K(10)=R(17)
/// E+E: E(4)+E(4)=I(8)
/// L+Y: L(11)+Y(24)=J(9)
/// L+K: L(11)+K(10)=V(21)
/// O+E: O(14)+E(4)=S(18)
/// Sonuç: RIJVS
///
/// Matematiksel Formül:
/// Şifreleme: C[i] = (P[i] + K[i mod len(K)]) mod 26
/// Şifre Çözme: P[i] = (C[i] - K[i mod len(K)] + 26) mod 26
///
/// Güvenlik:
/// - Frekans analizine monoalfabetik şifrelerden daha dirençlidir
/// - Kasiski testi ile anahtar uzunluğu bulunabilir
/// - Anahtar uzunluğu bilinirse frekans analizi ile kırılabilir
/// - Modern standartlara göre güvenli değildir
/// </summary>
public class VigenereCipher
{
    /// <summary>
    /// Şifreleme ve şifre çözme için kullanılan anahtar kelime.
    /// Büyük harflerle saklanır.
    /// </summary>
    private string key;

    /// <summary>
    /// VigenereCipher sınıfının yeni bir örneğini oluşturur.
    ///
    /// Anahtar Özellikleri:
    /// - Her uzunlukta olabilir
    /// - Uzun anahtarlar daha güvenlidir
    /// - İçinde tekrar eden karakterler güvenliği azaltır
    /// - En güvenli anahtar: rastgele ve metinle aynı uzunlukta (One-Time Pad)
    /// </summary>
    /// <param name="key">Şifreleme anahtarı kelimesi.</param>
    public VigenereCipher(string key)
    {
        // Anahtarı büyük harfe çevir ve sakla
        this.key = key.ToUpper();
    }

    /// <summary>
    /// Düz metni Vigenere şifresi ile şifreler.
    ///
    /// Şifreleme Algoritması:
    /// 1. Metni büyük harfe çevir
    /// 2. Her harf için:
    ///    a. Karşılık gelen anahtar harfini bul (döngüsel olarak)
    ///    b. Anahtar harfinin değeri kadar kaydir
    ///    c. Mod 26 ile alfabede kal
    /// 3. Harf olmayan karakterler değişmeden kalır
    ///
    /// Formül: C = (P + K) mod 26
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Şifrelenmiş metin (büyük harfler).</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        plainText = plainText.ToUpper();
        char[] result = new char[plainText.Length];

        // Anahtar indeksini takip et (sadece harfler için ilerler)
        int keyIndex = 0;

        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];

            if (char.IsLetter(c))
            {
                // Döngüsel olarak anahtar karakterini al
                // keyIndex mod key.Length ile anahtarın sonuna gelince başa dön
                char keyChar = key[keyIndex % key.Length];

                // Şifreleme formülü: (düz_metin + anahtar) mod 26
                // c - 'A': Harfi sayıya çevir (A=0, B=1, ..., Z=25)
                // keyChar - 'A': Anahtar harfinin kaydırma miktarı
                // % 26: Alfabede kal
                // + 'A': Tekrar harfe çevir
                result[i] = (char)(((c - 'A' + keyChar - 'A') % 26) + 'A');

                // Bir harf işlendi, anahtar indeksini artır
                keyIndex++;
            }
            else
            {
                // Harf olmayan karakterler aynen kalsın
                // Anahtar indeksi artmaz (sadece harfler için kullanılır)
                result[i] = c;
            }
        }

        return new string(result);
    }

    /// <summary>
    /// Şifreli metni Vigenere algoritması ile çözer.
    ///
    /// Çözme Algoritması:
    /// Şifrelemenin tersidir. Her harf, anahtar harfi kadar geriye kaydırılır.
    ///
    /// Formül: P = (C - K + 26) mod 26
    /// +26 eklenmesinin nedeni: Negatif sonuçları önlemek
    /// Örnek: (5 - 10) % 26 = -5 (yanlış)
    ///        (5 - 10 + 26) % 26 = 21 (doğru)
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Çözülmüş düz metin.</returns>
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
                // Karşılık gelen anahtar karakteri
                char keyChar = key[keyIndex % key.Length];

                // Şifre çözme formülü: (şifreli_metin - anahtar + 26) mod 26
                // +26: Negatif değerleri pozitife çevirmek için
                result[i] = (char)(((c - 'A' - keyChar + 'A' + 26) % 26) + 'A');

                keyIndex++;
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
    /// Kullanılan anahtarı döndürür.
    /// </summary>
    /// <returns>Şifreleme anahtarı (büyük harfler).</returns>
    public string GetKey() => key;

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Her karakterin hangi anahtar karakteri ile şifrelendiğini gösterir
    /// - Kaydırma miktarını ve sonucu açıklar
    /// - Formülün nasıl uygulandığını detaylandırır
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        // İlk adım: Başlangıç
        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, $"Starting encryption with key: {key}"));

        plainText = plainText.ToUpper();
        var result = new System.Text.StringBuilder();
        int keyIndex = 0;
        int stepNum = 2;

        // Her karakteri işle
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];

            if (char.IsLetter(c))
            {
                // Anahtar karakterini al
                char keyChar = key[keyIndex % key.Length];

                // Pozisyonları hesapla
                int plainPos = c - 'A';       // Düz metin karakterinin pozisyonu (0-25)
                int keyPos = keyChar - 'A';   // Anahtar karakterinin kaydırma miktarı (0-25)
                int newPos = (plainPos + keyPos) % 26;  // Yeni pozisyon

                char encrypted = (char)(newPos + 'A');
                result.Append(encrypted);
                keyIndex++;

                // Bu adımı kaydet
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'",
                    $"Plain: {c} (pos {plainPos}), Key: {keyChar} (pos {keyPos})",
                    $"Cipher: {encrypted} (pos {newPos})",
                    $"{c} + {keyChar} = {encrypted} (mod 26)"));
            }
            else
            {
                // Harf olmayan karakterler
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
    /// - Her şifreli karakterin hangi anahtar ile çözüldüğünü gösterir
    /// - Geriye kaydırma işlemini açıklar
    /// - Negatif değerlerin nasıl düzeltildiğini gösterir (+26 ekleme)
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(cipherText))
            return steps;

        steps.Add(new StepInfo(1, "Initialization", "Cipher Text", cipherText, $"Starting decryption with key: {key}"));

        cipherText = cipherText.ToUpper();
        var result = new System.Text.StringBuilder();
        int keyIndex = 0;
        int stepNum = 2;

        // Her karakteri çöz
        for (int i = 0; i < cipherText.Length; i++)
        {
            char c = cipherText[i];

            if (char.IsLetter(c))
            {
                // Anahtar karakterini al
                char keyChar = key[keyIndex % key.Length];

                // Pozisyonları hesapla
                int cipherPos = c - 'A';      // Şifreli karakterin pozisyonu
                int keyPos = keyChar - 'A';   // Anahtar kaydırma miktarı
                int newPos = (cipherPos - keyPos + 26) % 26;  // Orijinal pozisyon (+26 negatif önleme)

                char decrypted = (char)(newPos + 'A');
                result.Append(decrypted);
                keyIndex++;

                // Bu adımı kaydet
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'",
                    $"Cipher: {c} (pos {cipherPos}), Key: {keyChar} (pos {keyPos})",
                    $"Plain: {decrypted} (pos {newPos})",
                    $"{c} - {keyChar} = {decrypted} (mod 26)"));
            }
            else
            {
                // Harf olmayan karakterler
                result.Append(c);
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'",
                    "Non-letter character",
                    c.ToString(),
                    "Non-letter characters remain unchanged"));
            }
        }

        steps.Add(new StepInfo(stepNum, "Final Result", cipherText, result.ToString(), "Decryption complete"));

        return steps;
    }
}
