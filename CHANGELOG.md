# Changelog

## v1.0.0 — İlk sürüm

İlk kararlı sürüm. GoodbyeDPI'yi arka planda yöneten, Windows 11 Fluent tasarımlı,
tek tıkla bağlanan masaüstü istemcisi.

### Özellikler
- **Tek tıkla bağlan:** ortada büyük bağlan butonu, canlı durum ve animasyonlar (glow + spinner).
- **Preset sistemi:** Hızlı / Dengeli / Maksimum uyumluluk / Özel. Sahada doğrulanmış
  Türkiye/DNS-redirect komutlarına dayanır (kullanıcı ham parametre görmez).
- **Çoklu dil:** Türkçe + İngilizce, anlık geçiş (yeniden başlatma yok).
- **Tema:** Açık / Koyu / Sistem, anlık geçiş.
- **Sistem tepsisi:** kapatınca tepsiye küçülür; sağ tık menüsünde durum + Bağlan/Kes/Aç/Çıkış.
- **Bildirimler, günlük ekranı, ayarlar** (autostart, açılışta bağlan, log seviyesi vb.).
- **Gömülü GoodbyeDPI 0.2.3rc3 + WinDivert:** kullanıcı ayrıca indirme yapmaz.

### Dayanıklılık
- Beklenmedik çökmede **kendi kendini onaran otomatik yeniden bağlanma**.
- Bağlanmadan önce artık (orphan) goodbyedpi process temizliği.
- Kesme/çıkışta WinDivert sürücüsünü durdurup siler (sürücü sızıntısı / bayat kayıt yok).
- İkililer içerik-hash'li klasöre çıkarılır (yükseltmede kilitli `.sys` sorunu olmaz).

### Kurulum
`GoodbyeUI-Setup-1.0.0.exe` — Windows 10/11 64-bit. Ek bir şey kurmaya gerek yok.
