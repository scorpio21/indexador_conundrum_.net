; Instalador de EncodeNet8
; Requiere Inno Setup 6.x.

#define MyAppName "EncodeNet8"
#ifndef MyAppVersion
  #define MyAppVersion "0.0.82-beta"
#endif
#define MyAppPublisher "scorpio21"
#define MyAppExeName "EncodeNet8.exe"
#define PublishDir "publish\win-x64-singlefile"

[Setup]
AppId={{B7E32C7D-6E9C-4A2D-9C20-ED0E00000001}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableDirPage=no
DisableProgramGroupPage=no
OutputBaseFilename=EncodeNet8_{#MyAppVersion}_Setup
OutputDir=.\publish
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64os
SetupIconFile=EncodeNet8\Assets\Icons\logo.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el escritorio"; GroupDescription: "Tareas adicionales:"; Flags: unchecked

[Files]
; Publicación completa: ejecutable, DLL, Datos, encode y Assets.
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "MANUAL_ENCODE_NET8.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Ejecutar {#MyAppName}"; Flags: nowait postinstall skipifsilent
