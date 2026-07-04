# GoodbyeUI

[![version](https://img.shields.io/badge/version-1.0.0-0078D4)](https://github.com/Kcay8873/GoodbyeUI/releases)
[![platform](https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D4)](https://github.com/Kcay8873/GoodbyeUI/releases)
[![.NET](https://img.shields.io/badge/.NET-9-512BD4)](https://dotnet.microsoft.com/)
[![license](https://img.shields.io/badge/license-MIT-3DA639)](LICENSE)

**İnternet kısıtlamalarını ve yavaşlatmalarını aşan, VPN'lere göre sıfır hız kaybı sunan, Windows 11 tasarımlı tek tıkla çalışan masaüstü uygulaması.**

---

## 🌍 GoodbyeUI Nedir ve Ne İşe Yarar?

Günümüzde birçok internet sağlayıcısı (İSS), belirli web sitelerine, sosyal medya platformlarına veya oyun sunucularına erişimi kısıtlamak ya da yavaşlatmak için **DPI (Derin Paket İnceleme)** adı verilen bir filtreleme yöntemi kullanır. Bu sistem, internet trafiğinizi izleyerek nereye bağlandığınızı tespit eder ve erişiminizi engeller.

**GoodbyeUI**, bilgisayarınızdan çıkan veri paketlerini akıllıca parçalayarak ve düzenleyerek bu filtreleme sistemlerinin kafasını karıştırır. Böylece engellenmiş veya hızı kısıtlanmış sitelere **sanki hiçbir engel yokmuş gibi, doğrudan ve tam hızınızla** bağlanmanızı sağlar.

### Neden Klasik VPN'ler Yerine GoodbyeUI?

Bir VPN kullandığınızda tüm internet trafiğiniz önce yurt dışındaki başka bir sunucuya gider, oradan siteye ulaşır ve aynı yoldan geri döner. Bu durum:
- İnternet hızınızı ciddi oranda düşürür,
- Oyunlarda gecikme (ping) süresini tavan yaptırır,
- Gizlilik ve veri güvenliği endişeleri yaratır.

**GoodbyeUI bir VPN değildir!** Trafiğinizi asla üçüncü parti bir sunucudan geçirmez; sadece veri paketlerinizin formatını yerel olarak bilgisayarınızda düzenler. Bu sayede **%0 hız kaybı**, **sıfır ping artışı** ve **tam gizlilik** ile sınırsız bir internet deneyimi sunar.

---

## ✨ Öne Çıkan Özellikler

- **Tek Tıkla Özgürlük:** Karmaşık ağ ayarlarıyla, DNS sunucularıyla veya teknik detaylarla boğuşmanıza gerek yok. Uygulamayı açın ve ortadaki büyük **Bağlan** butonuna basın.
- **Akıllı Hazır Ayarlar (Presetler):** Farklı internet sağlayıcılarının farklı engelleme yöntemleri olabilir. GoodbyeUI, her ağa kolayca uyum sağlamanız için *Hızlı*, *Dengeli*, *Maksimum Uyumluluk* veya kendi *Özel* ayarlarınızı tek tıkla seçebileceğiniz hazır profillerle gelir.
- **Sessizce Arka Planda Çalışır:** Bağlantıyı başlattıktan sonra pencereyi kapattığınızda uygulama sistem tepsisine (sağ alt köşeye) küçülür. Ekranınızı kaplamaz, sizi rahatsız etmez.
- **Kesintisiz Deneyim (Akıllı Toparlanma):** Arka planda ağ bağlantınız anlık olarak kopsa bile uygulama bunu algılar ve sistemi otomatik olarak onararak bağlantınızı yeniler.
- **Modern ve Şık Arayüz:** Windows 11'in estetik Fluent ve Mica tasarım diline tam uyumludur. Göz yormayan Açık/Koyu tema desteği ve anlık Türkçe/İngilizce dil seçeneği sunar.

---

## 🚀 Kurulum ve Kullanım

1. [Releases](https://github.com/Kcay8873/GoodbyeUI/releases) sayfasından en güncel **`GoodbyeUI-Setup-1.0.0.exe`** dosyasını indirin.
2. Kurulumu tamamlayın ve uygulamayı çalıştırın.
3. Ana ekrandaki **Bağlan** butonuna tıklayın ve özgür internetin tadını çıkarın!

> **🪟 SmartScreen Uyarısı Hakkında:**
> Projemiz henüz bireysel olarak geliştirildiğinden ve pahalı dijital imza sertifikaları içermediğinden, Windows SmartScreen ilk kurulumda *"Bilinmeyen yayıncı"* uyarısı gösterebilir. Kuruluma devam etmek için **Ek bilgi ➔ Yine de çalıştır** seçeneğine tıklayabilirsiniz. Uygulamanın tüm kaynak kodu şeffaf bir şekilde bu depoda açıktır.

---

## ⚙️ Arka Planda Ne Çalışıyor? (Güçlü Altyapı)

GoodbyeUI, arka planda DPI engellerini aşma konusunda rüştünü ispatlamış, dünya çapında milyonlarca kişi tarafından kullanılan açık kaynaklı **[GoodbyeDPI](https://github.com/ValdikSS/GoodbyeDPI)** motorunu kullanır.

Orijinal GoodbyeDPI sadece komut satırı (CMD) üzerinden karmaşık parametrelerle çalışırken; **GoodbyeUI** bu güçlü motoru kendi içine gömülü olarak barındırır ve ona modern, akıllı, self-healing (kendi kendini onaran) bir grafik arayüz kazandırır. Harici olarak hiçbir ek yazılım indirmenize gerek kalmaz.

---

## 🛠️ Geliştiriciler İçin (Kaynaktan Derleme)

Projeyi bilgisayarınızda derlemek veya geliştirmeye katkıda bulunmak isterseniz **.NET 9 SDK** kurulu olmalıdır (`winget install Microsoft.DotNet.SDK.9`).

```bash
# Projeyi derle
dotnet build GoodbyeUI.sln -c Debug

# Birim testlerini çalıştır
dotnet test tests/GoodbyeUI.Tests

# Uygulamayı başlat (Yönetici izni gerektirir)
dotnet run --project src/GoodbyeUI.App

# Tek dosya (.exe) olarak yayınla (Release)
dotnet publish src/GoodbyeUI.App/GoodbyeUI.App.csproj -c Release -r win-x64 -o artifacts/app

# Kurulum paketi oluştur (Inno Setup 6 gerekir: winget install JRSoftware.InnoSetup)
& "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe" installer\GoodbyeUI.iss
```

---

## 📄 Lisans & Atıf

GoodbyeUI uygulama kodu **MIT** lisanslıdır (bkz. [LICENSE](LICENSE)). Uygulama, açık kaynak **GoodbyeDPI** (ValdikSS, Apache-2.0) ve **WinDivert** projelerini gömülü olarak kullanır; lisans metinleri `src/GoodbyeUI.Infrastructure/Assets/goodbyedpi/` altında korunmaktadır.

Geliştiren: **[Kcay8873](https://github.com/Kcay8873)** · Destek: [Patreon](https://www.patreon.com/c/kcay8873)
