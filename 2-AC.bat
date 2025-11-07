@echo off
chcp 65001 >nul
title Network Security - Calistir

cd /d "%~dp0"
echo.
echo ================================
echo  Network Security - Calistir
echo ================================
echo.

if not exist "bin\Debug\net9.0-windows\SecurityProject.exe" (
    echo [HATA] EXE dosyasi bulunamadi!
    echo.
    echo Once 1-DERLE.bat dosyasini calistirin.
    echo.
    pause
    exit /b 1
)

echo Uygulama aciliyor...
echo.
start "" "bin\Debug\net9.0-windows\SecurityProject.exe"
echo [OK] Uygulama baslatildi!
echo.
timeout /t 2 >nul

