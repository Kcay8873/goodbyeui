<div align="center">

# GoodbyeUI

**GoodbyeDPI için modern, Windows 11 tasarımlı masaüstü istemcisi**

Tek tıkla bağlan · CMD yok · Sistem tepsisi · Çoklu dil · Açık/Koyu tema

![version](https://img.shields.io/badge/version-1.0.0-0078D4)
![platform](https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D4)
![.NET](https://img.shields.io/badge/.NET-9-512BD4)
![license](https://img.shields.io/badge/license-MIT-3DA639)

</div>

---

## Nedir?

[GoodbyeDPI](https://github.com/ValdikSS/GoodbyeDPI), DPI tabanlı engellemeleri aşan açık
kaynaklı bir araçtır ama komut satırından parametrelerle çalıştırılır. **GoodbyeUI**, bu aracı
arka planda yöneten; teknik bilgi gerektirmeyen, VPN istemcisi hissi veren bir arayüz sunar.

- 🟢 Ortada büyük **Bağlan** butonu, canlı durum ve animasyon
- ⚙️ Preset sistemi (Hızlı / Dengeli / Maksimum uyumluluk / Özel) — parametre ezberlemek yok
- 🌍 Türkçe + İngilizce (kolayca yeni dil eklenebilir), anlık dil değişimi
- 🎨 Açık / Koyu / Sistem teması (yeniden başlatma yok)
- 🔔 Windows bildirimleri, sistem tepsisi menüsü (durum + Bağlan/Kes)
- 🛡️ Otomatik Administrator yetkisi · self-healing (beklenmedik çökmede yeniden bağlanır)
- 📦 GoodbyeDPI ikilileri **gömülü** — kullanıcı ayrıca indirme yapmaz

## Kurulum

1. [Releases](https://github.com/Kcay8873/GoodbyeUI/releases) sayfasından
   **`GoodbyeUI-Setup-1.0.0.exe`** dosyasını indir.
2. Çalıştır → kurulum sihirbazını takip et → aç.
3. Uygulama açılınca **Bağlan**'a bas. Hepsi bu.

> **Gereksinim:** Windows 10/11 **64-bit**. Başka hiçbir şey kurmana gerek yok
> (.NET runtime ve GoodbyeDPI zaten içinde gömülü).

> **"Bilinmeyen yayıncı" uyarısı:** Uygulama henüz dijital imzalı olmadığından Windows
> SmartScreen bir uyarı gösterebilir. **Ek bilgi → Yine de çalıştır** ile devam edebilirsin.

## Kaynaktan Derleme

Gereksinim: **.NET 9 SDK** (`winget install Microsoft.DotNet.SDK.9`).

```powershell
dotnet build GoodbyeUI.sln -c Debug            # derle
dotnet test tests/GoodbyeUI.Tests              # testler
dotnet run --project src/GoodbyeUI.App         # çalıştır (UAC ister)

# Yayın (self-contained tek .exe)
dotnet publish src/GoodbyeUI.App/GoodbyeUI.App.csproj -c Release -r win-x64 -o artifacts/app

# Kurulum paketi (Inno Setup 6 gerekir: winget install JRSoftware.InnoSetup)
& "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe" installer\GoodbyeUI.iss
```

## Teknoloji & Mimari

.NET 9 · WPF + WPF-UI (Fluent/Mica) · MVVM (CommunityToolkit) · Dependency Injection · Serilog

```
GoodbyeUI.App  ──►  GoodbyeUI.Core  ◄──  GoodbyeUI.Infrastructure
   (WPF/MVVM)        (arayüz+model)        (somut servisler)
```

## Lisans & Atıf

GoodbyeUI uygulama kodu **MIT** lisanslıdır (bkz. [LICENSE](LICENSE)). Uygulama, açık kaynak
**GoodbyeDPI** (ValdikSS, Apache-2.0) ve **WinDivert** projelerini gömülü olarak kullanır;
lisans metinleri `src/GoodbyeUI.Infrastructure/Assets/goodbyedpi/` altında korunur.

Geliştiren: **[Kcay8873](https://github.com/Kcay8873)** · Destek: [Patreon](https://www.patreon.com/c/kcay8873)
