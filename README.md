# 🔐 Network Security - Cryptographic Techniques

Modern, interaktif C# WPF uygulaması. Network Security dersi için 8 klasik ve modern şifreleme algoritmasının görsel gösterimi.

---

## 🚀 Hızlı Başlangıç

### Ön Gereksinimler

- Windows 10/11 (64-bit)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Adım 1: Derleme

Projeyi derlemek için:

```bash
1-DERLE.bat
```

Bu komut projeyi Debug modunda derler ve `bin\Debug\net9.0-windows\` klasörüne çıktı oluşturur.

### Adım 2: Çalıştırma

Uygulamayı başlatmak için:

```bash
2-AC.bat
```

Uygulama otomatik olarak açılacaktır.

### Alternatif: Manuel Çalıştırma

```bash
# Derleme
dotnet build --configuration Debug

# Çalıştırma
dotnet run
```

---

## ✨ Özellikler

### 🎨 Modern Arayüz

- **Dark Theme** - Profesyonel, göz yormayan tasarım
- **Smooth Animasyonlar** - Hardware accelerated, optimize edilmiş
- **Unique Colors** - Her cipher için özel renk paleti
- **Modern Scrollbar** - Minimal, zarif
- **Responsive** - Tam ekran optimizasyonu

### ⌨️ Klavye Kısayolları

- `ESC` → Ana menüye dön (cipher ekranlarında)
- `X` → Uygulamayı kapat (sağ üst)

### 🔐 8 Cipher Algoritması

#### 1. Caesar Cipher 🔒

- 0-25 arası kaydırma (shift)
- Brute force saldırı gösterimi
- Gerçek zamanlı şifreleme/çözme

#### 2. Monoalphabetic 🔑

- 26 harfli sabit anahtar
- Random key generator
- Karakter haritası

#### 3. Playfair 🧩

- 5x5 matris ile bigram şifreleme
- Görsel matris gösterimi
- I/J birleştirilmiş

#### 4. Hill Cipher 🏔️

- 2x2 matris şifreleme
- Determinant kontrolü
- Linear algebra tabanlı

#### 5. Vigenère 🌀

- Keyword tabanlı polyalphabetic
- Değişken kaydırma
- Klasik güçlü cipher

#### 6. Transposition 🔀

- Columnar transposition
- Ayarlanabilir sütun sayısı
- Matris görselleştirme

#### 7. XOR Cipher 🔐

- Bitwise XOR operasyonu
- Hex çıktı formatı
- Byte-level şifreleme
- Key cycling

#### 8. Base64 Encoding 📦

- UTF-8 text encoding
- Base64 çıktı formatı
- Çift yönlü dönüşüm
- Byte görselleştirme

---

## 📋 Sistem Gereksinimleri

### Minimum

- Windows 10 (64-bit)
- .NET 9.0 SDK
- 2 GB RAM
- 1366x768 ekran

### Önerilen

- Windows 11 (64-bit)
- .NET 9.0 SDK (güncel)
- 4 GB+ RAM
- 1920x1080+ ekran
- DirectX 11 GPU

---

## 📁 Proje Yapısı

```
network-security-cipher-app/
├── Services/                    # Cipher algoritmaları
│   ├── CaesarCipher.cs
│   ├── MonoalphabeticCipher.cs
│   ├── PlayfairCipher.cs
│   ├── HillCipher.cs
│   ├── VigenereCipher.cs
│   ├── TranspositionCipher.cs
│   ├── XORCipher.cs
│   ├── Base64Cipher.cs
│   └── StepInfo.cs              # Adım bilgisi modeli
│
├── Views/UserControls/          # Cipher UI ekranları
│   ├── CaesarCipherControl.xaml
│   ├── MonoalphabeticCipherControl.xaml
│   ├── PlayfairCipherControl.xaml
│   ├── HillCipherControl.xaml
│   ├── VigenereCipherControl.xaml
│   ├── TranspositionCipherControl.xaml
│   ├── XORCipherControl.xaml
│   └── Base64CipherControl.xaml
│   (+ .xaml.cs code-behind dosyaları)
│
├── MainWindow.xaml              # Ana pencere
├── App.xaml                     # Uygulama giriş noktası
├── SecurityProject.csproj       # Proje dosyası
│
├── 1-DERLE.bat                  # Derleme scripti
├── 2-AC.bat                     # Çalıştırma scripti
│
├── README.md                    # Bu dosya
└── KULLANIM.md                  # Detaylı kullanım kılavuzu
```

---

## 🎯 Performans Optimizasyonları

- **Hardware Acceleration** - GPU rendering
- **BitmapCache** - Animasyon optimizasyonu
- **Reduced Shadow Blur** - %70 daha az GPU yükü
- **Optimized Animations** - Hızlı, smooth geçişler

---

## ⚠️ Sorun Giderme

### "dotnet komutu bulunamadı"

**Çözüm:** .NET SDK yükleyin

1. https://dotnet.microsoft.com/download/dotnet/9.0
2. SDK'yı indirip kurun
3. Bilgisayarı yeniden başlatın
4. `1-KURULUM.bat` çalıştırın

### "Proje derlenemedi"

**Çözüm:**

```bash
dotnet clean
dotnet restore
dotnet build
```

### Uygulama yavaş

**Çözüm:**

- GPU sürücülerini güncelleyin
- Windows güncellemesi yapın
- Diğer programları kapatın

---

## 🎓 Eğitim Amaçlı Kullanım

Bu uygulama **klasik kriptografi öğretimi** içindir:

- ✅ Tarihi şifreleme teknikleri
- ✅ Algoritma mantığını anlama
- ✅ Görsel öğrenme
- ✅ Interaktif deneyimler

⚠️ **Gerçek güvenlik için KULLANMAYIN!**
Modern güvenlik: AES-256, RSA, SHA-256 gibi algoritmalar kullanın.

---

## 📚 Detaylı Kullanım

Tüm cipher'ların detaylı kullanım kılavuzu için:
→ **[KULLANIM.md](KULLANIM.md)** dosyasına bakın

---

## 🛠️ Geliştirme

### Kodu Düzenleme

1. Visual Studio 2022 veya VS Code ile açın
2. Değişikliklerinizi yapın
3. `1-DERLE.bat` ile derleyin
4. `2-AC.bat` ile test edin

### Yeni Cipher Ekleme

1. `Services/` altına yeni cipher class'ı ekleyin
2. `Views/UserControls/` altına XAML ekranı oluşturun
3. `MainWindow.xaml` içine cipher kartı ekleyin
4. `MainWindow.xaml.cs` içine navigation kodu yazın

---

## 📝 Lisans

Bu proje eğitim amaçlıdır ve açık kaynak kodludur.
