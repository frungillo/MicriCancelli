# ============================================================
#  MicriCancelli - Installa o aggiorna dal PC del varco
#
#  Scarica l'ultima release da GitHub e la installa nella cartella indicata
#  (predefinita: C:\MicriCancelli). Se il programma è in esecuzione lo chiude,
#  sovrascrive i file, CONSERVA database (DB\) e log (logs\) e lo riavvia.
#
#  Prima installazione (PowerShell, anche non amministratore):
#     irm https://raw.githubusercontent.com/frungillo/MicriCancelli/main/deploy/aggiorna.ps1 | iex
#
#  Aggiornamenti successivi: doppio clic su aggiorna.cmd nella cartella del
#  programma, oppure:
#     powershell -ExecutionPolicy Bypass -File C:\MicriCancelli\aggiorna.ps1
#
#  Parametri facoltativi:  -Cartella D:\Altro   -Versione 2.0.1   -NonAvviare
# ============================================================
param(
    [string]$Cartella = "C:\MicriCancelli",
    [string]$Versione = "",          # vuoto = ultima release
    [switch]$NonAvviare
)

$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
$repo = "frungillo/MicriCancelli"

Write-Host ""
Write-Host "=== MicriCancelli - installazione/aggiornamento ===" -ForegroundColor Cyan

# ---- 1. individua la release ----
$api = if ($Versione) { "https://api.github.com/repos/$repo/releases/tags/v$Versione" } else { "https://api.github.com/repos/$repo/releases/latest" }
$release = Invoke-RestMethod -Uri $api -Headers @{ 'User-Agent' = 'MicriCancelli-aggiorna' }
$asset = $release.assets | Where-Object { $_.name -like 'MicriCancelli-v*.zip' } | Select-Object -First 1
if (-not $asset) { throw "La release $($release.tag_name) non contiene lo zip del programma." }
$nuova = $release.tag_name.TrimStart('v')

$fileVersione = Join-Path $Cartella 'VERSIONE.txt'
$attuale = if (Test-Path $fileVersione) { (Get-Content $fileVersione -Raw).Trim() } else { $null }
Write-Host ("  Installata: {0}   Disponibile: {1}" -f ($(if ($attuale) { "v$attuale" } else { "nessuna" })), "v$nuova")
if ($attuale -eq $nuova -and -not $Versione) {
    Write-Host "  Già aggiornato. Nessuna operazione." -ForegroundColor Green
    exit 0
}

# ---- 2. scarica ----
$tmp = Join-Path $env:TEMP "MicriCancelli-v$nuova"
if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
New-Item -ItemType Directory -Force $tmp | Out-Null
$zip = Join-Path $tmp $asset.name
Write-Host "  Scarico $($asset.name) ($([math]::Round($asset.size / 1MB, 1)) MB)..."
Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $zip -Headers @{ 'User-Agent' = 'MicriCancelli-aggiorna' }
Expand-Archive -Path $zip -DestinationPath (Join-Path $tmp 'estratto') -Force
$sorgente = Join-Path $tmp 'estratto'

# ---- 3. chiude il programma se è aperto ----
$processi = Get-Process -Name 'MicriCancelli' -ErrorAction SilentlyContinue
if ($processi) {
    Write-Host "  Chiudo MicriCancelli in esecuzione..."
    $processi | Stop-Process -Force
    Start-Sleep -Seconds 2
}

# ---- 4. copia conservando DB e log ----
$primaInstallazione = -not (Test-Path (Join-Path $Cartella 'MicriCancelli.exe'))
New-Item -ItemType Directory -Force $Cartella | Out-Null
Get-ChildItem $sorgente -Force | ForEach-Object {
    if ($_.Name -in @('DB', 'logs') -and -not $primaInstallazione) { return }   # dati del cliente: non si toccano
    $dest = Join-Path $Cartella $_.Name
    if ($_.PSIsContainer) {
        if (Test-Path $dest) { Remove-Item $dest -Recurse -Force }
        Copy-Item $_.FullName $dest -Recurse -Force
    }
    else { Copy-Item $_.FullName $dest -Force }
}
# se per qualche motivo manca il DB (cartella nuova), lo prende dal pacchetto
if (-not (Test-Path (Join-Path $Cartella 'DB\cancelli.db'))) {
    Copy-Item (Join-Path $sorgente 'DB') (Join-Path $Cartella 'DB') -Recurse -Force
}

# comodità: un .cmd per aggiornare con doppio clic e un collegamento sul desktop
Set-Content (Join-Path $Cartella 'aggiorna.cmd') "@echo off`r`npowershell -NoProfile -ExecutionPolicy Bypass -File `"%~dp0aggiorna.ps1`"`r`npause" -Encoding ASCII
try {
    $desktop = [Environment]::GetFolderPath('Desktop')
    $shell = New-Object -ComObject WScript.Shell
    $link = $shell.CreateShortcut((Join-Path $desktop 'MicriCancelli.lnk'))
    $link.TargetPath = Join-Path $Cartella 'MicriCancelli.exe'
    $link.WorkingDirectory = $Cartella
    $link.Save()
} catch { }

Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
Write-Host ("  Installata v{0} in {1}" -f $nuova, $Cartella) -ForegroundColor Green

# ---- 5. riavvia ----
if (-not $NonAvviare) {
    Start-Process -FilePath (Join-Path $Cartella 'MicriCancelli.exe') -WorkingDirectory $Cartella
    Write-Host "  MicriCancelli avviato."
}
