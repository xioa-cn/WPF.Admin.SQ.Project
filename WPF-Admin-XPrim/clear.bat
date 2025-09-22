@echo off
title Cleanup Script

echo Warning: This will delete all bin and obj folders!
set /p input=Continue? (Y/N): 
if /i not "%input%"=="Y" exit /b

echo Cleaning folders...

for /d /r . %%d in (bin) do (
    if exist "%%d" (
        echo Deleting: %%d
        rd /s /q "%%d"
    )
)

for /d /r . %%d in (obj) do (
    if exist "%%d" (
        echo Deleting: %%d
        rd /s /q "%%d"
    )
)

echo Cleanup complete!
pause