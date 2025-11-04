@echo off
chcp 65001 >nul
title Network Security - Derle

cd /d "%~dp0"
echo.
echo Proje derleniyor...
dotnet build --configuration Debug
echo.
if errorlevel 1 (
    echo [HATA] Derleme BASARISIZ!
    pause
    exit /b 1
) else (
    echo [OK] Derleme BASARILI!
)
echo.
pause

