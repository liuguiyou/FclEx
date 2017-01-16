REM "..\tools\gitlink\GitLink.exe" ..\ -u https://github.com/huoshan12345/FclEx -c release

@ECHO OFF
SET /P VERSION_SUFFIX=Please enter version-suffix (can be left empty): 
dotnet pack "..\src\FclEx" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet pack "..\src\FclEx.Component" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet pack "..\src\FclEx.Json" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
pause