# ============================================================
#  MicriCancelli - Installa o aggiorna dal PC del varco
#
#  Scarica l'ultima release da GitHub e la installa nella cartella indicata
#  (predefinita: C:\MicriCancelli). Se il programma è in esecuzione lo chiude,
#  sovrascrive i file, CONSERVA database (DB\) e log (logs\) e lo riavvia.
#
#  Prima installazione: scaricare lo zip dalla pagina delle release, "Annulla blocco"
#  nelle proprietà dello zip, estrarre in C:\MicriCancelli, avviare MicriCancelli.exe.
#  (Niente righe "irm ... | iex": Microsoft Defender le blocca come tecnica ClickFix.)
#
#  Il repository è pubblico: non serve nessun token. Se dovesse tornare privato,
#  lo script accetta un token GitHub di sola lettura (-Token, variabile d'ambiente
#  MICRI_GITHUB_TOKEN o file github-token.txt nella cartella del programma).
#
#  Aggiornamenti successivi: doppio clic su aggiorna.cmd nella cartella del
#  programma, oppure:
#     powershell -ExecutionPolicy Bypass -File C:\MicriCancelli\aggiorna.ps1
#
#  Parametri facoltativi:  -Cartella D:\Altro   -Versione 2.0.1   -Token ...   -NonAvviare
# ============================================================
param(
    [string]$Cartella = "C:\MicriCancelli",
    [string]$Versione = "",          # vuoto = ultima release
    [string]$Token = "",
    [switch]$NonAvviare
)

$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
$repo = "frungillo/MicriCancelli"

# Se lo script sta nella cartella del programma (è arrivato con lo zip), aggiorna QUELLA cartella,
# ovunque sia stata scelta; C:\MicriCancelli resta il default solo quando lo script gira da solo.
if (-not $PSBoundParameters.ContainsKey('Cartella') -and $PSScriptRoot -and (Test-Path (Join-Path $PSScriptRoot 'MicriCancelli.exe'))) {
    $Cartella = $PSScriptRoot
}
$fileToken = Join-Path $Cartella 'github-token.txt'

Write-Host ""
Write-Host "=== MicriCancelli - installazione/aggiornamento ===" -ForegroundColor Cyan

# ---- 0. token (repository privato) ----
if (-not $Token) { $Token = $env:MICRI_GITHUB_TOKEN }
if (-not $Token -and (Test-Path $fileToken)) { $Token = (Get-Content $fileToken -Raw).Trim() }
$intestazioni = @{ 'User-Agent' = 'MicriCancelli-aggiorna'; 'Accept' = 'application/vnd.github+json' }
if ($Token) { $intestazioni['Authorization'] = "Bearer $Token" }

# ---- 1. individua la release ----
$api = if ($Versione) { "https://api.github.com/repos/$repo/releases/tags/v$Versione" } else { "https://api.github.com/repos/$repo/releases/latest" }
try {
    $release = Invoke-RestMethod -Uri $api -Headers $intestazioni
}
catch {
    if (-not $Token) { throw "GitHub non risponde o rifiuta la richiesta ($($_.Exception.Message)): controllare la connessione a internet; se il repository è tornato privato serve un token di sola lettura (vedi intestazione dello script)." }
    throw "GitHub ha rifiutato la richiesta ($($_.Exception.Message)): token scaduto o senza permesso 'Contents: read' su $repo ?"
}
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
# Per i repository privati l'asset va richiesto all'API con Accept: application/octet-stream:
# GitHub risponde con un redirect a un indirizzo firmato, da seguire SENZA l'intestazione Authorization.
function Scarica-Asset([string]$url, [string]$destinazione) {
    $richiesta = [System.Net.HttpWebRequest]::Create($url)
    $richiesta.UserAgent = 'MicriCancelli-aggiorna'
    $richiesta.Accept = 'application/octet-stream'
    $richiesta.AllowAutoRedirect = $false
    if ($Token) { $richiesta.Headers['Authorization'] = "Bearer $Token" }
    $risposta = $richiesta.GetResponse()
    try {
        $codice = [int]$risposta.StatusCode
        if ($codice -ge 300 -and $codice -lt 400) {
            $dove = $risposta.Headers['Location']
            $risposta.Close()
            Invoke-WebRequest -Uri $dove -OutFile $destinazione -Headers @{ 'User-Agent' = 'MicriCancelli-aggiorna' }
            return
        }
        $flusso = $risposta.GetResponseStream()
        $file = [System.IO.File]::Create($destinazione)
        try { $flusso.CopyTo($file) } finally { $file.Close(); $flusso.Close() }
    }
    finally { $risposta.Close() }
}

$tmp = Join-Path $env:TEMP "MicriCancelli-v$nuova"
if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
New-Item -ItemType Directory -Force $tmp | Out-Null
$zip = Join-Path $tmp $asset.name
Write-Host "  Scarico $($asset.name) ($([math]::Round($asset.size / 1MB, 1)) MB)..."
Scarica-Asset $asset.url $zip
Expand-Archive -Path $zip -DestinationPath (Join-Path $tmp 'estratto') -Force
$sorgente = Join-Path $tmp 'estratto'
if (-not (Test-Path (Join-Path $sorgente 'MicriCancelli.exe'))) { throw "Lo zip scaricato non contiene MicriCancelli.exe" }

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
# il token resta sul PC per gli aggiornamenti successivi (solo lettura del repository)
if ($Token) { Set-Content $fileToken $Token -Encoding ASCII -NoNewline }

# toglie il "marchio del web" ai file appena copiati: niente avvisi di Windows a ogni avvio
Get-ChildItem $Cartella -Recurse -File | Unblock-File -ErrorAction SilentlyContinue

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
