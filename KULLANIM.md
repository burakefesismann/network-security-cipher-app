# 📖 Kullanım Kılavuzu

Bu kılavuz, uygulamadaki tüm cipher algoritmalarının nasıl kullanılacağını detaylı olarak açıklar.

---

## 🎯 Genel Kullanım

### Uygulamayı Başlatma

1. **İlk Kullanım:** Önce `1-DERLE.bat` dosyasını çalıştırın (projeyi derler)
2. **Çalıştırma:** `2-AC.bat` dosyasını çalıştırın (uygulamayı açar)
3. Ana menüde 8 cipher kartı görürsünüz
4. Herhangi bir karta tıklayın
5. **ESC** tuşu ile ana menüye dönün

### Ortak Özellikler

- **Encrypt/Decrypt** modu değiştirme
- **Gerçek zamanlı** şifreleme/çözme
- **Key/Keyword** girişi
- **Plain Text** ve **Cipher Text** alanları

---

## 🔒 1. Caesar Cipher

### Ne İşe Yarar?

Her harfi alfabede belirli sayıda kaydırır.

- A → D (shift=3)
- HELLO → KHOOR (shift=3)

### Nasıl Kullanılır?

1. **Shift Value** (0-25)
   
   - Slider ile kaydırma miktarını ayarlayın
   - 0 = değişiklik yok
   - 13 = ROT13 cipher
   - 25 = 1 harf geri

2. **Encrypt Mode**
   
   - Plain Text'e metin yazın
   - Otomatik şifrelenir

3. **Decrypt Mode**
   
   - Cipher Text'e şifreli metin yazın
   - Otomatik çözülür

4. **Brute Force**
   
   - "Try All Keys" butonuna basın
   - 26 farklı key ile denemeleri görün
   - Doğru metni bulun

### Örnek

```
Plain Text:  ATTACK AT DAWN
Shift:       3
Cipher Text: DWWDFN DW GDZQ
```

---

## 🔑 2. Monoalphabetic Cipher

### Ne İşe Yarar?

Her harf için sabit bir eşleştirme tablosu kullanır.

- A → Z
- B → Y
- C → X
- vb.

### Nasıl Kullanılır?

1. **Substitution Key**
   
   - 26 harfli key girin
   - Veya "Generate Random Key" butonuna basın
   - Örnek: `ZYXWVUTSRQPONMLKJIHGFEDCBA`

2. **Encrypt**
   
   - Plain Text yazın
   - Key'e göre şifrelenmiş metni görün

3. **Decrypt**
   
   - Cipher Text yazın
   - Aynı key ile çözün

### Önemli

- Key'de her harf **bir kez** olmalı
- 26 karakter uzunluğunda olmalı
- Büyük/küçük harf duyarsız

### Örnek

```
Key:         QWERTYUIOPASDFGHJKLZXCVBNM
Plain Text:  HELLO
Cipher Text: ITSSG
```

---

## 🧩 3. Playfair Cipher

### Ne İşe Yarar?

Bigram (2'li harf çiftleri) şifreler. 5x5 matris kullanır.

### Nasıl Kullanılır?

1. **Keyword**
   
   - Bir keyword girin (örn: "MONARCHY")
   - Otomatik 5x5 matris oluşturulur
   - I ve J aynı kabul edilir

2. **Matris Görselleştirme**
   
   - Keyword'den oluşan matrisi görün
   - Her harf bir kez görünür

3. **Encrypt/Decrypt**
   
   - Metin çift harflerle işlenir
   - Aynı harfler X ile ayrılır
   - Tek harf varsa son Z eklenir

### Kurallar

- Aynı satırdaki harfler → sağa kayar
- Aynı sütundaki harfler → aşağı kayar
- Farklı satır/sütun → dikdörtgen köşeleri

### Örnek

```
Keyword:     PLAYFAIR
Matris:      P L A Y F
             I/J R E X M
             B C D G H
             K N O Q S
             T U V W Z

Plain Text:  HELLO
Bigrams:     HE LX LO
Cipher Text: DMYRANM
```

---

## 🏔️ 4. Hill Cipher

### Ne İşe Yarar?

Linear algebra (2x2 matris) ile şifreler.

### Nasıl Kullanılır?

1. **2x2 Key Matrix**
   
   ```
   [a b]
   [c d]
   ```
   
   - 4 sayı girin (0-25 arası)
   - Determinant 26'ya bölünebilir olmamalı

2. **Örnek Valid Key**
   
   ```
   [3  3]
   [2  5]
   
   Determinant = (3×5) - (3×2) = 9 ✅
   ```

3. **Encrypt**
   
   - 2'li harf grupları matris ile çarpılır
   - Matematiksel işlem

4. **Decrypt**
   
   - Inverse matrix kullanılır
   - Otomatik hesaplanır

### Örnek

```
Key Matrix:  [3 3]
             [2 5]

Plain Text:  HELP
Pairs:       HE LP
Cipher Text: HYLK
```

---

## 🌀 5. Vigenère Cipher

### Ne İşe Yarar?

Keyword ile polyalphabetic (çok alfabeli) şifreleme.

### Nasıl Kullanılır?

1. **Keyword**
   
   - Bir kelime girin (örn: "KEY")
   - Her harf farklı shift değeri
   - K=10, E=4, Y=24

2. **Encrypt**
   
   - Plain text her harfi keyword harfiyle kaydırılır
   - Keyword tekrar eder
     
     ```
     Plain:    H E L L O
     Keyword:  K E Y K E
     Shifts:   10+4+24+10+4
     Cipher:   R I J V S
     ```

3. **Decrypt**
   
   - Aynı keyword ile ters işlem

### Örnek

```
Keyword:     LEMON
Plain Text:  ATTACKATDAWN
Cipher Text: LXFOPVEFRNHR
```

---

## 🔀 6. Transposition Cipher

### Ne İşe Yarar?

Harfleri yer değiştirir, değiştirmez. Sütunlu okuma.

### Nasıl Kullanılır?

1. **Column Count**
   
   - Kaç sütun kullanılacak (2-10)
   - Örnek: 4 sütun

2. **Matrix Visualization**
   
   ```
   Plain Text: HELLOWORLD (10 harf)
   Columns: 4
   
   H E L L
   O W O R
   L D
   ```

3. **Encrypt**
   
   - Sütun sütun oku
   - Cipher: HOLWEDOLOR

4. **Decrypt**
   
   - Aynı sütun sayısı ile ters işlem

### Örnek

```
Columns:     5
Plain Text:  ATTACKATDAWN
Matrix:      A T T A C
             K A T D A
             W N

Cipher Text: AKWTAANTTADC
```

---

## 🔐 7. XOR Cipher

### Ne İşe Yarar?

Her byte'ı bir key byte'ı ile XOR (exclusive OR) işlemine sokar. Bitwise operasyon.

### Nasıl Kullanılır?

1. **Key**
   
   - Bir key girin (herhangi bir metin)
   - Key uzunluğu mesajdan kısa olabilir
   - Key otomatik tekrar eder

2. **Encrypt**
   
   - Plain Text yazın
   - Hex formatında çıktı alın
   - Byte-by-byte işlem görün

3. **Decrypt**
   
   - Hex formatında Cipher Text girin
   - Aynı key ile çözün
   - XOR tersine çevrilebilir (symmetric)

### XOR İşlemi Nasıl Çalışır?

```
Plain: H (72)  = 01001000
Key:   K (75)  = 01001011
---------------------
XOR Result:     = 00000011 (3)

Decrypt:
Cipher: 3      = 00000011
Key:    K (75) = 01001011
---------------------
XOR Result:     = 01001000 (72 = 'H')
```

### Örnek

```
Key:         SECRET
Plain Text:  HELLO
Cipher (Hex): 1B 00 1F 1F 1C
Decrypt:     HELLO
```

### Önemli

- Çıktı HEX formatında olduğu için decrypt sırasında hex girişi gerekir
- Key tekrar eder (SECRETSECRETSECRET...)
- Symmetric cipher (aynı key ile şifreleme/çözme)

---

## 📦 8. Base64 Encoding

### Ne İşe Yarar?

Binary veriyi ASCII karakterlere dönüştürür. Email, URL, veri transferi için kullanılır.

### Nasıl Kullanılır?

1. **Encode Mode**
   
   - Plain Text yazın (UTF-8)
   - Otomatik Base64 formatına çevrilir
   - Karakter başına byte gösterimi

2. **Decode Mode**
   
   - Base64 formatında text girin
   - Otomatik decode edilir
   - Orijinal text'i geri alın

### Base64 Nasıl Çalışır?

- Her 3 byte → 4 karakter
- Sadece A-Z, a-z, 0-9, +, / karakterleri
- Padding için = kullanılır

### Örnek

```
Plain Text:   HELLO
Bytes:        72 69 76 76 79 (UTF-8)
Base64:       SEVMTE8=

Decode:
Base64:       SEVMTE8=
Plain Text:   HELLO
```

### Karakter Tablosu

```
A-Z = 0-25
a-z = 26-51
0-9 = 52-61
+   = 62
/   = 63
=   = padding
```

### Kullanım Alanları

- Email attachments (MIME)
- URL parametreleri
- JSON web tokens (JWT)
- Resim embedding (data URLs)

### Önemli

- Base64 **şifreleme değildir**, sadece encoding'dir
- Veriler kolayca decode edilebilir
- Veri boyutu ~33% artar (3 byte → 4 karakter)
- Güvenlik sağlamaz, sadece transfer kolaylığı

---

## 💡 İpuçları

### Genel

- Önce **Encrypt** modda test edin
- Sonra **Decrypt** modda doğrulayın
- Key'leri not edin
- Büyük/küçük harf farkı yok

### Performans

- Uzun metinler için bekleyin
- Gerçek zamanlı işlem yapılır
- Matris görselleştirmeleri otomatik

### Güvenlik

- Bu algoritmalar **eğitim amaçlıdır**
- Gerçek güvenlik için kullanmayın
- Modern şifreleme: AES-256, RSA

---

## 🆘 Sık Sorulan Sorular

### Q: Key yanlışsa ne olur?

**A:** Decrypt yanlış sonuç verir. Doğru key şart.

### Q: Boşluklar şifrelenir mi?

**A:** Hayır, sadece harfler işlenir.

### Q: Sayılar ve semboller?

**A:** İşlenmez, olduğu gibi kalır.

### Q: Türkçe karakter?

**A:** Desteklenmez. Sadece A-Z.

### Q: En güvenli hangisi?

**A:** Eğitim için hepsi eşit. Gerçekte hiçbiri güvenli değil.

---
