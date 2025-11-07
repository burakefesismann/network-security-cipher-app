namespace SecurityProject.Services;

/// <summary>
/// Caesar Şifreleme algoritmasını uygulayan sınıf.
/// Caesar şifresi, tarihteki en eski ve en basit şifreleme yöntemlerinden biridir.
/// Julius Caesar tarafından kullanıldığı için bu ismi almıştır.
///
/// Çalışma Prensibi:
/// - Alfabedeki her harf, belirli sayıda (anahtar) kaydırılarak şifrelenir
/// - Örneğin, anahtar=3 ise: A→D, B→E, C→F şeklinde dönüşüm yapılır
/// - Z harfine ulaşıldığında tekrar A'dan başlanır (döngüsel yapı)
///
/// Güvenlik Notu:
/// Caesar şifresi çok zayıf bir şifreleme yöntemidir çünkü sadece 25 olası anahtar vardır.
/// Brute force (kaba kuvvet) saldırısıyla kolayca kırılabilir.
/// </summary>
public class CaesarCipher
{
    /// <summary>
    /// Düz metni Caesar algoritması kullanarak şifreler.
    /// Formül: C = (P + K) mod 26
    /// Burada C=Şifreli karakter, P=Düz karakter pozisyonu, K=Anahtar
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin. Boş veya null olabilir.</param>
    /// <param name="key">Kaydırma miktarı (anahtar). Pozitif veya negatif olabilir.</param>
    /// <returns>Şifrelenmiş metin. Harf olmayan karakterler değişmeden kalır.</returns>
    public string Encrypt(string plainText, int key)
    {
        // Boş veya null metin kontrolü
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        // Sonuç için aynı uzunlukta karakter dizisi oluştur
        char[] result = new char[plainText.Length];

        // Her karakteri tek tek işle
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];

            // Sadece harfleri şifrele
            if (char.IsLetter(c))
            {
                // Büyük harf için 'A', küçük harf için 'a' baz karakteri olarak kullan
                char baseChar = char.IsUpper(c) ? 'A' : 'a';

                // Kaydırma formülü: (karakter_pozisyonu + anahtar) mod 26
                // Mod 26 kullanımı alfabenin döngüsel yapısını sağlar (Z'den sonra A'ya döner)
                result[i] = (char)(((c - baseChar + key) % 26) + baseChar);
            }
            else
            {
                // Harf olmayan karakterler (boşluk, noktalama vb.) değişmeden kalır
                result[i] = c;
            }
        }

        return new string(result);
    }

    /// <summary>
    /// Şifreli metni Caesar algoritması kullanarak çözer.
    ///
    /// Çözme İşlemi:
    /// Şifre çözmek için ters kaydırma yapılır.
    /// Formül: P = C - K = C + (26 - K) mod 26
    /// Bu, şifrelemenin tersini uygular ve orijinal metni geri verir.
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <param name="key">Şifreleme sırasında kullanılan anahtar.</param>
    /// <returns>Çözülmüş düz metin.</returns>
    public string Decrypt(string cipherText, int key)
    {
        // Şifre çözme = Ters yönde şifreleme
        // 26 - key ile ters kaydırma yapılır
        // Mod 26 ile negatif değerler düzeltilir
        return Encrypt(cipherText, 26 - (key % 26));
    }

    /// <summary>
    /// Caesar şifresine brute force (kaba kuvvet) saldırısı uygular.
    ///
    /// Brute Force Nedir?
    /// Tüm olası anahtarları (1-25) deneyerek şifreli metni çözme yöntemidir.
    /// Caesar şifresi sadece 25 farklı anahtar olduğu için bu yöntemle kolayca kırılabilir.
    ///
    /// Kullanım Senaryosu:
    /// Şifreleme anahtarını bilmiyorsanız, bu metod tüm olası sonuçları verir.
    /// Sonuçlar arasından anlamlı olan metin orijinal metindir.
    /// </summary>
    /// <param name="cipherText">Kırılacak şifreli metin.</param>
    /// <returns>1'den 25'e kadar her anahtar için çözülmüş metin listesi.</returns>
    public List<string> BruteForce(string cipherText)
    {
        List<string> results = new List<string>();

        // 1'den 25'e kadar tüm olası anahtarları dene
        // 0 ve 26 dahil edilmez çünkü bunlar şifreleme yapmaz
        for (int i = 1; i <= 25; i++)
        {
            // Her anahtar için şifre çözme yap ve sonucu listeye ekle
            results.Add(Decrypt(cipherText, i));
        }

        return results;
    }

    /// <summary>
    /// Şifreleme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// Bu metod, şifreleme sürecinin her adımını detaylı olarak kaydeder.
    /// Öğrencilerin algoritmanın nasıl çalıştığını anlamasına yardımcı olur.
    ///
    /// Her Adım İçerir:
    /// - Hangi karakter işleniyor
    /// - Karakterin pozisyonu (A=0, B=1, ..., Z=25)
    /// - Kaydırma işlemi sonucu
    /// - Sonuç karakteri
    /// </summary>
    /// <param name="plainText">Şifrelenecek düz metin.</param>
    /// <param name="key">Kullanılacak anahtar (kaydırma miktarı).</param>
    /// <returns>Her şifreleme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncryptionSteps(string plainText, int key)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        // İlk adım: Başlangıç bilgileri
        steps.Add(new StepInfo(1, "Initialization", "Plain Text", plainText, $"Starting encryption with key = {key}"));

        var result = new System.Text.StringBuilder();
        int stepNum = 2;

        // Her karakteri işle ve adımları kaydet
        for (int i = 0; i < plainText.Length; i++)
        {
            char c = plainText[i];

            if (char.IsLetter(c))
            {
                // Büyük/küçük harf için baz karakteri belirle
                char baseChar = char.IsUpper(c) ? 'A' : 'a';

                // Orijinal pozisyonu hesapla (A=0, B=1, ...)
                int originalPos = c - baseChar;

                // Yeni pozisyonu hesapla: (pozisyon + anahtar) mod 26
                int newPos = (originalPos + key) % 26;

                // Yeni karakteri oluştur
                char encrypted = (char)(newPos + baseChar);
                result.Append(encrypted);

                // Bu adımı kaydet
                steps.Add(new StepInfo(stepNum++, $"Process '{c}'",
                    $"Position: {originalPos} ({c})",
                    $"New Position: {newPos} ({encrypted})",
                    $"{c} → {encrypted} (shift by {key})"));
            }
            else
            {
                // Harf olmayan karakterler aynen kalır
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
    /// Not:
    /// Şifre çözme, ters yönde şifreleme olduğu için
    /// GetEncryptionSteps metodunu ters anahtar (26-key) ile çağırır.
    /// </summary>
    /// <param name="cipherText">Çözülecek şifreli metin.</param>
    /// <param name="key">Şifreleme sırasında kullanılan anahtar.</param>
    /// <returns>Her şifre çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecryptionSteps(string cipherText, int key)
    {
        return GetEncryptionSteps(cipherText, 26 - (key % 26));
    }
}
