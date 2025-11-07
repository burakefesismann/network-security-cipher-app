namespace SecurityProject.Services;

/// <summary>
/// Base64 Kodlama/Çözme algoritmasını uygulayan sınıf.
///
/// Base64 Nedir?
/// Base64, binary (ikili) veriyi ASCII metin karakterlerine dönüştüren bir kodlama yöntemidir.
/// Şifreleme DEĞİLDİR! Sadece bir kodlama (encoding) yöntemidir.
///
/// ÖNEMLI UYARI:
/// Base64 GÜVENLİK SAĞLAMAZ!
/// - Veriyi gizlemez, sadece formatını değiştirir
/// - Herkes kolayca çözebilir (decode edebilir)
/// - Şifreleme algoritması değil, veri formatı dönüşümüdür
/// - Güvenlik için AES, RSA gibi gerçek şifreleme kullanın
///
/// Neden Base64 Kullanılır?
/// 1. Binary veriyi metin olarak göndermek için
/// 2. E-mail eklerinde (MIME encoding)
/// 3. URL'lerde ve JSON/XML'de binary veri taşımak
/// 4. HTML/CSS'de resim gömme (data URI)
/// 5. Veritabanında binary saklama
///
/// Nasıl Çalışır?
/// 1. Binary veri 6 bitlik gruplara bölünür
/// 2. Her 6 bit (0-63 arası) bir karaktere karşılık gelir
/// 3. 64 karakterli bir alfabe kullanılır: A-Z, a-z, 0-9, +, /
/// 4. Padding için '=' karakteri kullanılır
///
/// Matematiksel Açıklama:
/// 3 byte (24 bit) → 4 Base64 karakteri
/// 24 bit ÷ 6 bit = 4 grup
/// Her grup bir karakter (0-63 → A-Z, a-z, 0-9, +, /)
///
/// Örnek Dönüşüm:
/// Text: "ABC"
/// Binary: 01000001 01000010 01000011
/// 6-bit gruplar: 010000 010100 001001 000011
/// Decimal: 16, 20, 9, 3
/// Base64: Q U J D
///
/// Padding:
/// Eğer son grup 6 bitten az ise '=' ile doldurulur:
/// 1 byte kaldı → 2 karakter + "=="
/// 2 byte kaldı → 3 karakter + "="
///
/// Karakterler:
/// [0-25]  → A-Z
/// [26-51] → a-z
/// [52-61] → 0-9
/// [62]    → +
/// [63]    → /
/// Padding → =
///
/// Veri Büyümesi:
/// Base64 kodlama veriyi ~33% büyütür.
/// 3 byte → 4 karakter
/// n byte → ceil(n/3)*4 karakter
///
/// Kullanım Örnekleri:
/// - Email: Content-Transfer-Encoding: base64
/// - HTML: <img src="data:image/png;base64,iVBORw0K...">
/// - JWT: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
/// - Basic Auth: Authorization: Basic dXNlcjpwYXNz
///
/// Güvenlik Notu:
/// - Parolaları Base64 ile "şifrelemeyin"!
/// - Hassas veriyi Base64 ile gizleyemezsiniz
/// - Base64 sadece encoding, encryption değil
/// - Herkes online araçlarla decode edebilir
/// </summary>
public class Base64Cipher
{
    /// <summary>
    /// Düz metni Base64 formatına kodlar (encode eder).
    ///
    /// NOT: Bu işlem ŞIFRELEME değildir!
    /// Sadece binary veriyi ASCII metne dönüştürür.
    /// Herkes kolayca geri çevirebilir (decode).
    ///
    /// Kodlama Süreci:
    /// 1. Metni UTF-8 byte dizisine çevir
    /// 2. .NET'in Convert.ToBase64String metodunu kullan
    /// 3. Bu metod otomatik olarak:
    ///    - 6 bitlik gruplara böler
    ///    - Base64 karakterlerine dönüştürür
    ///    - Gerekirse padding ekler
    ///
    /// Örnek:
    /// "Hello" → "SGVsbG8="
    /// </summary>
    /// <param name="plainText">Kodlanacak düz metin.</param>
    /// <returns>Base64 formatında kodlanmış metin.</returns>
    public string Encode(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            // Metni UTF-8 byte dizisine çevir
            // UTF-8: Unicode karakterleri destekleyen standart encoding
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            // Byte dizisini Base64 string'e çevir
            // .NET'in built-in metodu RFC 4648 standardına uygun
            return Convert.ToBase64String(textBytes);
        }
        catch
        {
            return "Encoding error";
        }
    }

    /// <summary>
    /// Base64 formatındaki metni çözer (decode eder).
    ///
    /// Çözme Süreci:
    /// 1. Base64 string'i byte dizisine çevir
    /// 2. .NET'in Convert.FromBase64String metodunu kullan
    /// 3. Byte dizisini UTF-8 string'e çevir
    ///
    /// Hata Durumları:
    /// - Geçersiz Base64 karakterler
    /// - Yanlış padding
    /// - Bozuk format
    /// Bu durumlarda "Invalid Base64 format" döner.
    ///
    /// Örnek:
    /// "SGVsbG8=" → "Hello"
    /// </summary>
    /// <param name="base64Text">Çözülecek Base64 metni.</param>
    /// <returns>Çözülmüş düz metin.</returns>
    public string Decode(string base64Text)
    {
        if (string.IsNullOrEmpty(base64Text))
            return string.Empty;

        try
        {
            // Base64 string'i byte dizisine çevir
            // Geçersiz format varsa exception fırlatır
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Byte dizisini UTF-8 string'e çevir
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
        catch
        {
            // Base64 parsing hatası
            return "Invalid Base64 format";
        }
    }

    /// <summary>
    /// Base64 kodlama işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - UTF-8 byte dönüşümünü gösterir
    /// - Her byte'ı hexadecimal formatda gösterir
    /// - Base64 algoritmasının nasıl çalıştığını açıklar
    ///
    /// Not:
    /// Gerçek Base64 algoritması bit düzeyinde çalışır ancak
    /// basitlik için sadece byte → Base64 dönüşümü gösterilir.
    /// </summary>
    /// <param name="plainText">Kodlanacak düz metin.</param>
    /// <returns>Her kodlama adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetEncodingSteps(string plainText)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(plainText))
            return steps;

        // Adım 1: Başlangıç
        steps.Add(new StepInfo(1, "Step 1: Input", "Plain Text", plainText, "Starting Base64 encoding"));

        try
        {
            // Adım 2: UTF-8 byte dönüşümü
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            // Her byte'ı hexadecimal formatda göster
            // Select: Her elemanı dönüştür
            // b => $"0x{b:X2}": Byte'ı "0x" ile başlayan 2 haneli hex string'e çevir
            // string.Join(" ", ...): Boşlukla ayırarak birleştir
            steps.Add(new StepInfo(2, "Step 2: Convert to Bytes", plainText,
                string.Join(" ", textBytes.Select(b => $"0x{b:X2}")),
                $"Converted to {textBytes.Length} bytes (UTF-8 encoding)"));

            // Adım 3: Base64 kodlama
            string base64Result = Convert.ToBase64String(textBytes);
            steps.Add(new StepInfo(3, "Step 3: Base64 Encoding",
                string.Join(" ", textBytes.Select(b => $"0x{b:X2}")),
                base64Result,
                "Applied Base64 encoding algorithm"));

            // Son adım: Tamamlanma
            steps.Add(new StepInfo(4, "Final Result", plainText, base64Result, "Base64 encoding complete"));
        }
        catch
        {
            steps.Add(new StepInfo(1, "Error", plainText, "Encoding error", "Failed to encode text"));
        }

        return steps;
    }

    /// <summary>
    /// Base64 çözme işleminin adım adım açıklamasını oluşturur.
    ///
    /// Eğitim Amaçlı:
    /// - Base64 string'in byte'lara dönüşümünü gösterir
    /// - Her byte'ı hexadecimal formatda gösterir
    /// - UTF-8 string dönüşümünü açıklar
    /// </summary>
    /// <param name="base64Text">Çözülecek Base64 metni.</param>
    /// <returns>Her çözme adımını içeren StepInfo listesi.</returns>
    public List<StepInfo> GetDecodingSteps(string base64Text)
    {
        var steps = new List<StepInfo>();

        if (string.IsNullOrEmpty(base64Text))
            return steps;

        // Adım 1: Başlangıç
        steps.Add(new StepInfo(1, "Step 1: Input", "Base64 Text", base64Text, "Starting Base64 decoding"));

        try
        {
            // Adım 2: Base64 çözme (byte dizisine çevirme)
            byte[] base64Bytes = Convert.FromBase64String(base64Text);
            steps.Add(new StepInfo(2, "Step 2: Base64 Decoding", base64Text,
                string.Join(" ", base64Bytes.Select(b => $"0x{b:X2}")),
                $"Decoded to {base64Bytes.Length} bytes"));

            // Adım 3: UTF-8 string dönüşümü
            string decodedText = System.Text.Encoding.UTF8.GetString(base64Bytes);
            steps.Add(new StepInfo(3, "Step 3: Convert to Text",
                string.Join(" ", base64Bytes.Select(b => $"0x{b:X2}")),
                decodedText,
                "Converted bytes to UTF-8 text"));

            // Son adım: Tamamlanma
            steps.Add(new StepInfo(4, "Final Result", base64Text, decodedText, "Base64 decoding complete"));
        }
        catch
        {
            steps.Add(new StepInfo(1, "Error", base64Text, "Invalid Base64 format", "Failed to decode Base64 string"));
        }

        return steps;
    }
}
