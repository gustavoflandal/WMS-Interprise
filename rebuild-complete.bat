@echo off
echo ========================================
echo LIMPEZA E REBUILD TOTAL
echo ========================================
echo.

cd backend\src\WMS.API

echo [1/6] Parando qualquer processo dotnet...
taskkill /F /IM dotnet.exe 2>nul
timeout /t 2 >nul
echo.

echo [2/6] Deletando bin e obj...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
if exist ..\WMS.Application\bin rmdir /s /q ..\WMS.Application\bin
if exist ..\WMS.Application\obj rmdir /s /q ..\WMS.Application\obj
if exist ..\WMS.Domain\bin rmdir /s /q ..\WMS.Domain\bin
if exist ..\WMS.Domain\obj rmdir /s /q ..\WMS.Domain\obj
if exist ..\WMS.Infrastructure\bin rmdir /s /q ..\WMS.Infrastructure\bin
if exist ..\WMS.Infrastructure\obj rmdir /s /q ..\WMS.Infrastructure\obj
if exist ..\WMS.Shared\bin rmdir /s /q ..\WMS.Shared\bin
if exist ..\WMS.Shared\obj rmdir /s /q ..\WMS.Shared\obj
echo.

echo [3/6] Limpando solucao...
cd ..
dotnet clean WMS.sln
echo.

echo [4/6] Restaurando pacotes...
dotnet restore WMS.sln
echo.

echo [5/6] Compilando solucao...
dotnet build WMS.sln --no-restore
echo.

echo [6/6] Verificando erros de compilacao...
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ========================================
    echo ERRO NA COMPILACAO!
    echo ========================================
    pause
    exit /b 1
)

echo.
echo ========================================
echo BUILD COMPLETO COM SUCESSO!
echo ========================================
echo.
echo Agora execute:
echo cd WMS.API
echo dotnet run
echo.

pause
