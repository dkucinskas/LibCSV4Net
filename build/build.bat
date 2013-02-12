rem @echo off
setlocal

set nu_path=%~dp0%..\.nuget
set path=%nu_path%;%path%

cd ..\LibCSV
rem echo %path%
rem nuget.exe spec

nuget.exe pack LibCSV.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory  "..\out"

endlocal
