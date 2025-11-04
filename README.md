# 🔐 Network Security - Cryptographic Techniques

Modern, interaktif C# WPF uygulaması. Network Security dersi için 6 klasik şifreleme algoritmasının görsel gösterimi.

---

## 🚀 Hızlı Başlangıç

### 1️⃣ Kurulum
```bash
1-KURULUM.bat
```
- .NET 9.0 SDK'yı kontrol eder
- Proje bağımlılıklarını yükler
- Release modda derler

### 2️⃣ Çalıştırma
```bash
3-CALISTIR.bat
```

### 3️⃣ Yeniden Derleme (Kod değiştirdiyseniz)
```bash
2-DERLE.bat
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

### 🔐 6 Cipher Algoritması

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
security/
├── Services/                    # Cipher algoritmaları
│   ├── CaesarCipher.cs
│   ├── MonoalphabeticCipher.cs
│   ├── PlayfairCipher.cs
│   ├── HillCipher.cs
│   ├── VigenereCipher.cs
│   └── TranspositionCipher.cs
│
├── Views/UserControls/          # Cipher UI ekranları
│   ├── CaesarCipherControl.xaml
│   ├── MonoalphabeticCipherControl.xaml
│   ├── PlayfairCipherControl.xaml
│   ├── HillCipherControl.xaml
│   ├── VigenereCipherControl.xaml
│   └── TranspositionCipherControl.xaml
│
├── MainWindow.xaml              # Ana pencere
├── App.xaml                     # Stil ve temalar
│
├── 1-KURULUM.bat                # İlk kurulum
├── 2-DERLE.bat                  # Derleme
├── 3-CALISTIR.bat               # Çalıştırma
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
3. `2-DERLE.bat` ile derleyin
4. `3-CALISTIR.bat` ile test edin

### Yeni Cipher Ekleme
1. `Services/` altına yeni cipher class'ı
2. `Views/UserControls/` altına XAML ekranı
3. `MainWindow.xaml` içine kart ekle
4. `MainWindow.xaml.cs` içine navigation

---

## 📝 Lisans

Bu proje eğitim amaçlıdır ve açık kaynak kodludur.

---

## 🙏 Teşekkürler

Network Security dersi için hazırlanmıştır.

**🔐 Güvenli Şifreleme Öğrenin! 🚀**
