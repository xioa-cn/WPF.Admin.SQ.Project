@echo off
setlocal enabledelayedexpansion

set /p userInput=Please enter the authorization code:

echo !userInput! > authcode.txt

echo Write Application Success

pause
