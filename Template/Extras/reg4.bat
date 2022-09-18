@echo off
cd /d "%~dp0"
regjump HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\bam\State\UserSettings
exit