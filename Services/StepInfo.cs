namespace SecurityProject.Services;

/// <summary>
/// Şifreleme/Çözme işlemlerinin adım adım görselleştirilmesi için kullanılan veri modeli
/// Her bir şifreleme adımının detaylarını (girdi, çıktı, açıklama) tutar
/// </summary>
public class StepInfo
{
    /// <summary>
    /// Adım numarası (1, 2, 3, ...)
    /// UI'da adımların sıralı gösterilmesi için kullanılır
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Adımın kısa açıklaması
    /// Örnek: "Harfi 3 pozisyon kaydır", "Matris çarpımı yap"
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Bu adıma girdi olarak verilen değer
    /// Örnek: "A (pozisyon 0)", "HELLO"
    /// </summary>
    public string Input { get; set; } = string.Empty;

    /// <summary>
    /// Bu adımın çıktı değeri
    /// Örnek: "D (pozisyon 3)", "KHOOR"
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Adımın detaylı matematiksel veya algoritmik açıklaması (opsiyonel)
    /// Örnek: "Shift: (0 + 3) mod 26 = 3"
    /// </summary>
    public string? Explanation { get; set; }

    /// <summary>
    /// StepInfo sınıfının constructor'ı
    /// Tüm adım bilgilerini alarak yeni bir StepInfo nesnesi oluşturur
    /// </summary>
    /// <param name="stepNumber">Adım numarası</param>
    /// <param name="description">Adım açıklaması</param>
    /// <param name="input">Girdi değeri</param>
    /// <param name="output">Çıktı değeri</param>
    /// <param name="explanation">Detaylı açıklama (opsiyonel)</param>
    public StepInfo(int stepNumber, string description, string input, string output, string? explanation = null)
    {
        StepNumber = stepNumber;
        Description = description;
        Input = input;
        Output = output;
        Explanation = explanation;
    }
}

