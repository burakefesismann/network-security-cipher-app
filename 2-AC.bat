@echo off
chcp 65001 >nul
title Network Security - Ac

cd /d "%~dp0"
echo.
echo Uygulama aciliyor...
start "" "bin\Debug\net9.0-windows\SecurityProject.exe"
echo Tamamlandi!
timeout /t 1 >nul

