# ============================================================
#  MicriCancelli - Crea una release su GitHub
#
#  Uso (dalla macchina di sviluppo, con GitHub CLI autenticata):
#     .\deploy\release.ps1 -Versione 2.0.0 [-Note "testo delle note"]
#
#  Cosa fa:
#   1. compila in Release con la versione indicata (dotnet publish)
#   2. crea lo zip  pubblicazione\MicriCancelli-v<versione>.zip
#      (contiene anche DB\cancelli.db vuoto per le installazioni nuove e
#       lo script aggiorna.ps1 da usare sul PC del varco)
#   3. pubblica tag e release v<versione> su GitHub con lo zip allegato
# ============================================================
param(
    [Parameter(Mandatory = $true)][string]$Versione,
    [string]$Note = "",
    [switch]$SoloZip
)

$ErrorActionPreference = 'Stop'
if ($Versione -notmatch '^\d+\.\d+\.\d+$') { throw "Versione non valida: usare il formato X.Y.Z (es. 2.0.0)" }

$radice   = Split-Path $PSScriptRoot -Parent
$progetto = Join-Path $radice 'MicriCancelli.csproj'
$uscita   = Join-Path $radice "pubblicazione\v$Versione"
$zip      = Join-Path $radice "pubblicazione\MicriCancelli-v$Versione.zip"

Write-Host ""
Write-Host "=== MicriCancelli - Release v$Versione ===" -ForegroundColor Cyan

if (Test-Path $uscita) { Remove-Item $uscita -Recurse -Force }
if (Test-Path $zip)    { Remove-Item $zip -Force }

Write-Host "[1/3] Compilazione Release..." -ForegroundColor Yellow
dotnet publish $progetto -c Release -o $uscita "/p:Version=$Versione" --nologo -v q
if ($LASTEXITCODE -ne 0) { throw "Compilazione non riuscita" }

# File che non devono andare nel pacchetto
Remove-Item (Join-Path $uscita '*.pdb') -Force -ErrorAction SilentlyContinue
Remove-Item (Join-Path $uscita 'logs')  -Recurse -Force -ErrorAction SilentlyContinue

# Lo script di aggiornamento viaggia dentro il pacchetto: sul PC del varco resta accanto all'exe
Copy-Item (Join-Path $PSScriptRoot 'aggiorna.ps1') $uscita -Force
Set-Content (Join-Path $uscita 'VERSIONE.txt') $Versione -Encoding ASCII

# Controllo: senza le librerie native di SQLite il programma non parte sul PC del cliente
foreach ($f in @('MicriCancelli.exe', 'System.Data.SQLite.dll', 'x64\SQLite.Interop.dll', 'x86\SQLite.Interop.dll', 'QRCoder.dll', 'DB\cancelli.db')) {
    if (-not (Test-Path (Join-Path $uscita $f))) { throw "Nel pacchetto manca ${f}: release interrotta" }
}

Write-Host "[2/3] Creazione zip..." -ForegroundColor Yellow
Compress-Archive -Path (Join-Path $uscita '*') -DestinationPath $zip -CompressionLevel Optimal
$dimensione = [math]::Round((Get-Item $zip).Length / 1MB, 1)
Write-Host "      $zip ($dimensione MB)"

if ($SoloZip) {
    Write-Host "Solo zip richiesto: release GitHub non creata." -ForegroundColor Yellow
    exit 0
}

Write-Host "[3/3] Release su GitHub..." -ForegroundColor Yellow
if ([string]::IsNullOrWhiteSpace($Note)) { $Note = "Release v$Versione di MicriCancelli." }
$Note += "`n`nInstallazione/aggiornamento sul PC del varco: vedi README (sezione Distribuzione)."
Push-Location $radice
try {
    # (via cmd: in PowerShell 5.1 lo stderr di gh diventerebbe un errore bloccante)
    cmd /c "gh release view v$Versione >nul 2>&1"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "      la release v$Versione esiste: sostituisco lo zip allegato" -ForegroundColor Yellow
        gh release upload "v$Versione" $zip --clobber
        if ($LASTEXITCODE -ne 0) { throw "gh release upload non riuscito" }
    }
    else {
        gh release create "v$Versione" $zip --title "MicriCancelli v$Versione" --notes $Note
        if ($LASTEXITCODE -ne 0) { throw "gh release create non riuscito" }
    }
}
finally { Pop-Location }

Write-Host ""
Write-Host "=== Release v$Versione pubblicata ===" -ForegroundColor Green
Write-Host "  https://github.com/frungillo/MicriCancelli/releases/tag/v$Versione"
Write-Host "  Sul PC del varco: .\aggiorna.ps1  (oppure la riga unica indicata nel README)"
