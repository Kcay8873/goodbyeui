; GoodbyeUI kurulum betiği (Inno Setup 6)
; Derleme: ISCC.exe installer\GoodbyeUI.iss  (önce self-contained exe publish edilmeli)

#define MyAppName "GoodbyeUI"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Kcay8873"
#define MyAppURL "https://github.com/Kcay8873"
#define MyAppExeName "GoodbyeUI.exe"

[Setup]
; Sabit AppId: yükseltmeler aynı uygulamayı günceller (yeni GUID üretmeyin).
AppId={{E528182A-DA4B-4C3B-B7BD-46C7E31555A7}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
; Denetim Masası > Programlar'daki "Destek bağlantısı" ve "Yardım bağlantısı" GitHub'a gider.
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppContact={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
DisableDirPage=auto
OutputDir=..\artifacts\installer
OutputBaseFilename=GoodbyeUI-Setup-{#MyAppVersion}
SetupIconFile=..\src\GoodbyeUI.App\Assets\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
VersionInfoVersion={#MyAppVersion}
; Kurulum/kaldırma sırasında uygulama açıksa kapatılmasını iste (tek-örnek mutex'i).
AppMutex=GoodbyeUI_SingleInstance_Mutex
CloseApplications=yes

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Self-contained tek dosya (önceden publish edilmiş).
Source: "..\artifacts\app\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
; Uygulama requireAdministrator manifestine sahip; shellexec ile UAC üzerinden başlatılır (kod 740 önlenir).
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent shellexec

[UninstallRun]
; Uygulamanın oluşturmuş olabileceği otomatik başlatma görevini temizle.
Filename: "{sys}\schtasks.exe"; Parameters: "/delete /tn ""GoodbyeUI Autostart"" /f"; Flags: runhidden; RunOnceId: "DelAutostart"
