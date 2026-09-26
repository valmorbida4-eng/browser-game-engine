@echo off
chcp 65001 >nul
echo ========================================================
echo   Instalando Atalho do Aether Games para Todos os Usuários
echo ========================================================
echo.

set "SOURCE=%~dp0Aether Games.exe"
set "ICON=%~dp0app.ico"
set "SHORTCUT_PATH=C:\Users\Public\Desktop\Aether Games.lnk"

powershell -NoProfile -ExecutionPolicy Bypass -Command "$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('%SHORTCUT_PATH%'); $s.TargetPath = '%SOURCE%'; $s.WorkingDirectory = '%~dp0'; $s.Description = 'Aether Games - VoxelCraft & Reinos RTS'; $s.IconLocation = '%SOURCE%,0'; $s.Save()"

if exist "%SHORTCUT_PATH%" (
    echo [SUCESSO] Atalho criado em: %SHORTCUT_PATH%
) else (
    echo [AVISO] Se falhou, execute este arquivo como Administrador!
)

echo.
pause
