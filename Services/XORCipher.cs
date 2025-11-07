namespace SecurityProject.Services;

/// <summary>
/// XOR (Exclusive OR) Şifreleme algoritmasını uygulayan sınıf.
///
/// XOR Şifresi Nedir?
/// XOR (Özel VEYA) bit düzeyinde mantıksal işlem kullanarak şifreleme yapan basit ama güçlü bir yöntemdir.
/// Modern kriptografinin temel taşlarından biridir ve birçok karmaşık algoritmada kullanılır.
///
/// XOR Mantık Kapısı:
/// 0 XOR 0 = 0
/// 0 XOR 1 = 1
/// 1 XOR 0 = 1
/// 1 XOR 1 = 0
/// Kural: Bitler farklıysa 1, aynıysa 0
///
/// Önemli Matematiksel Özellik:
/// A XOR B XOR B = A
/// Bu özellik sayesinde aynı anahtar hem şifreleme hem şifre çözme için kullanılır!
///
/// Şifreleme: Cipher = Plain XOR Key
/// Şifre Çözme: Plain = Cipher XOR Key (aynı işlem!)
///
/// Örnek:
/// Plain: 'A' = 01000001 (65)
/// Key:   'K' = 01001011 (75)
/// XOR sonucu = 00001010 (10)
/// Tekrar XOR = 01000001 (65 = 'A') ← Orijinal değer geri geldi!
///
/// One-Time Pad (Tek Kullanımlık Anahtar):
/// Eğer anahtar:
/// - Tam rastgele ise
/// - Metinle aynı uzunlukta ise
/// - Sadece bir kez kullanılırsa
/// Matematiksel olarak kırılamaz bir şifreleme sağlar!
///
/// Pratik Kullanım:
/// Gerçek uygulamalarda genellikle kısa anahtarlar tekrar edilir.
/// Bu durumda güvenlik azalır ancak hala kullanışlıdır.
///
/// Modern Kullanım Alanları:
/// - AES, DES gibi blok şifrelerinin içinde
/// - Stream cipher'lar (RC4, Salsa20)
/// - Ağ protokollerinde (WEP, WPA)
/// - Veri maskeleme ve obfuscation
///
/// Güvenlik Notu:
/// - Kısa ve tekrar eden anahtarlar frekans analizi ile kırılabilir
/// - Anahtar asla düz metin ile birlikte saklanmamalıdır
/// - Aynı anahtar ile aynı mesajı iki kez şifrelemeyin!
/// </summary>
public class XORCipher
{
    /// <summary>
    /// Düz metni XOR algoritması ile şifreler.
    ///
    /// Şifreleme Süreci:
    /// 1. Düz metni UTF-8 byte dizisine çevir
    /// 2. Anahtarı UTF-8 byte dizisine çevir
    /// 3. Her byte'ı karşılık gelen anahtar byte'ı ile XOR'la
    /// 4. Anahtar kısaysa döngüsel olarak tekrar et
    /// 5. Sonucu hexadecimal string olarak göster
    ///
    /// Hexadecimal Gösterim:
    /// Binary veriyi ekranda göstermek için hexadecimal (16'lık sayı sistemi) kullanılır.
    /// Her byte (8 bit) iki hexadecimal karakterle gösterilir.
    /// Örnek: 255 = 0xFF, 10 = 0x0A
    ///
    /// Neden Hexadecimal?
    /// - Şifreli byte'lar yazdırılamaz karakterler içerebilir
    /// - Hex gösterim tüm değerleri güvenle gösterir
    /// - Network ve dosya sistemleri için uyumludur
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <param name="key">Şifreleme anahtarı (her uzunlukta olabilir).</param>
    /// <returns>Hexadecimal string formatında şifreli metin.</returns>
    public string Encrypt(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key))
            return string.Empty;

        // Metni ve anahtarı byte dizilerine çevir
        // UTF-8 encoding: Tüm Unicode karakterleri destekler
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

        byte[] result = new byte[textBytes.Length];

        // Her byte'ı XOR'la
        for (int i = 0; i < textBytes.Length; i++)
        {
            // Anahtar döngüsel olarak kullanılır (i % keyBytes.Length)
            // Bu, kısa anahtarların tekrar edilmesini sağlar
            result[i] = (byte)(textBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        // Sonucu hexadecimal string'e çevir
        // Format: "XX XX XX" (her byte iki hex karakter + boşluk)
        // BitConverter.ToString: Byte dizisini hex string'e çevirir
        // Replace("-", " "): Ayraç olarak boşluk kullan
        return BitConverter.ToString(result).Replace("-", " ");
    }

    /// <summary>
    /// Şifreli metni XOR algoritması ile çözer.
    ///
    /// Çözme Süreci:
    /// 1. Hexadecimal string'i byte dizisine çevir
    /// 2. Anahtarı byte dizisine çevir
    /// 3. Her byte'ı karşılık gelen anahtar byte'ı ile XOR'la (aynı işlem!)
    /// 4. Sonucu UTF-8 string'e çevir
    ///
    /// XOR'un Simetrik Özelliği:
    /// Şifreleme ve şifre çözme aynı işlemdir!
    /// Cipher XOR Key = Plain
    /// Bu özellik XOR'u çok verimli yapar.
    ///
    /// Hata Durumları:
    /// - Geçersiz hex format: "Invalid hex format" döner
    /// - Tek sayıda hex karakter: Hata
    /// - Geçersiz hex karakterler: Hata
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin (hexadecimal format).</param>
    /// <param name="key">Şifreleme sırasında kullanılan anahtar.</param>
    /// <returns>Çözülmüş düz metin.</returns>
    public string Decrypt(string cipherText, string key)
    {
        if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key))
            return string.Empty;

        try
        {
            // Boşlukları ve tireleri temizle
            string hexString = cipherText.Replace(" ", "").Replace("-", "");

            // Hex string uzunluğu çift sayı olmalı (her byte = 2 hex karakter)
            if (string.IsNullOrEmpty(hexString) || hexString.Length % 2 != 0)
            {
                return "Invalid hex format (must be even length)";
            }

            // Hex string'i byte dizisine çevir
            byte[] cipherBytes = new byte[hexString.Length / 2];

            for (int i = 0; i < cipherBytes.Length; i++)
            {
                // Her 2 hex karakterden 1 byte oluştur
                // Substring(i * 2, 2): i'nci byte'ın hex temsilini al
                // Convert.ToByte(..., 16): Hex string'i byte'a çevir (16 = hexadecimal)
                cipherBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            // Anahtarı byte dizisine çevir
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] result = new byte[cipherBytes.Length];

            // XOR işlemi (şifreleme ile aynı!)
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                result[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            // Byte dizisini UTF-8 string'e çevir
            return System.Text.Encoding.UTF8.GetString(result);
        }
        catch
        {
            // Hex parsing hatası
            return "Invalid hex format";
        }
    }

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Her byte'ın nasıl XOR'landığını gösterir
    /// - Binary ve hexadecimal temsilleri açıklar
    /// - XOR işleminin matematiksel detaylarını verir
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <param name="key">Şifreleme anahtarı.</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText, string key)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key))
            return steps;

        // Başlangıç adımı
        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, $"Starting XOR encryption with key: {key}"));

        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] result = new byte[textBytes.Length];
        int stepNum = 2;

        // Her byte'ı XOR'la ve adımları kaydet
        for (int i = 0; i < textBytes.Length; i++)
        {
            byte textByte = textBytes[i];
            byte keyByte = keyBytes[i % keyBytes.Length];

            // XOR işlemi
            result[i] = (byte)(textByte ^ keyByte);

            // Bu adımı detaylı olarak kaydet
            // Format: "Text: {decimal} (0x{hex}), Key: {decimal} (0x{hex})"
            // 0x notasyonu hexadecimal sayı gösterir
            // :X2 format: 2 haneli büyük harf hex (örn: 0A, FF)
            steps.Add(new StepInfo(stepNum++, $"XOR byte {i + 1}",
                $"Text: {textByte} (0x{textByte:X2}), Key: {keyByte} (0x{keyByte:X2})",
                $"Result: {result[i]} (0x{result[i]:X2})",
                $"{textByte} XOR {keyByte} = {result[i]}"));
        }

        // Sonucu hex string'e çevir
        string hexResult = BitConverter.ToString(result).Replace("-", " ");

        // Son adım
        steps.Add(new StepInfo(stepNum, "Final Result (Hex)", plainText, hexResult, "Encryption complete - converted to hexadecimal"));

        return steps;
    }

    /// <summary>
    /// Şifre çözme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Hex string'in nasıl byte'lara çevrildiğini gösterir
    /// - Her byte'ın nasıl XOR'landığını açıklar
    /// - XOR'un simetrik özelliğini vurgular (aynı işlem!)
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin (hexadecimal).</param>
    /// <param name="key">Şifreleme anahtarı.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText, string key)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key))
            return steps;

        try
        {
            // Hex string'i temizle
            string hexString = cipherText.Replace(" ", "").Replace("-", "");

            // Uzunluk kontrolü
            if (string.IsNullOrEmpty(hexString) || hexString.Length % 2 != 0)
            {
                steps.Add(new StepInfo(1, "Error", "Invalid hex format", "Error", "Hex string must be even length"));
                return steps;
            }

            // Başlangıç adımı
            steps.Add(new StepInfo(1, "Initialization", "Cipher Text (Hex)", cipherText, $"Starting XOR decryption with key: {key}"));

            // Hex string'i byte dizisine çevir
            byte[] cipherBytes = new byte[hexString.Length / 2];

            for (int i = 0; i < cipherBytes.Length; i++)
            {
                cipherBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] result = new byte[cipherBytes.Length];
            int stepNum = 2;

            // Her byte'ı XOR'la (şifreleme ile aynı!)
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                byte cipherByte = cipherBytes[i];
                byte keyByte = keyBytes[i % keyBytes.Length];

                // XOR işlemi
                result[i] = (byte)(cipherByte ^ keyByte);

                // Bu adımı kaydet
                steps.Add(new StepInfo(stepNum++, $"XOR byte {i + 1}",
                    $"Cipher: {cipherByte} (0x{cipherByte:X2}), Key: {keyByte} (0x{keyByte:X2})",
                    $"Result: {result[i]} (0x{result[i]:X2})",
                    $"{cipherByte} XOR {keyByte} = {result[i]}"));
            }

            // Byte dizisini string'e çevir
            string decryptedText = System.Text.Encoding.UTF8.GetString(result);

            // Son adım
            steps.Add(new StepInfo(stepNum, "Final Result", cipherText, decryptedText, "Decryption complete"));

            return steps;
        }
        catch
        {
            // Hata durumu
            steps.Add(new StepInfo(1, "Error", "Invalid format", "Error", "Failed to decode hex string"));
            return steps;
        }
    }
}
