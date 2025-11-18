@echo off
echo ========================================
echo REBUILD COMPLETO DO BACKEND
echo ========================================
echo.

cd backend\src\WMS.API

echo [1/4] Limpando binarios antigos...
dotnet clean
echo.

echo [2/4] Removendo pastas bin e obj...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
echo.

echo [3/4] Restaurando pacotes...
dotnet restore
echo.

echo [4/4] Compilando projeto...
dotnet build --no-restore
echo.

echo ========================================
echo BUILD COMPLETO!
echo ========================================
echo.
echo Agora execute: dotnet run
echo.

pause
