@echo off
chcp 65001 >nul
title Network Security - Derle

cd /d "%~dp0"
echo.
echo ================================
echo  Network Security - Derleme
echo ================================
echo.
echo Proje derleniyor...
echo.
dotnet build network-security-cipher-app.sln --configuration Debug
echo.
if errorlevel 1 (
    echo [HATA] Derleme BASARISIZ!
    echo.
    pause
    exit /b 1
) else (
    echo [OK] Derleme BASARILI!
    echo.
    echo Cikti: bin\Debug\net9.0-windows\SecurityProject.exe
    echo.
)
pause

