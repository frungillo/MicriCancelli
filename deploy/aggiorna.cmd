@echo off
rem MicriCancelli - aggiorna all'ultima release pubblicata su GitHub (conserva DB e log)
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0aggiorna.ps1"
pause
