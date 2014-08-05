rmdir /S /Q deploy
mkdir deploy
set src=..\OpenNFS1\bin\WindowsGL\Debug
set dest=.\deploy
xcopy %src%\Content %dest%\Content\ /s /e
copy %src%\gameconfig.json %dest%
copy %src%\OpenNFS1.exe %dest%
copy %src%\*.dll %dest%
copy %src%\*.pdb %dest%
del %dest%\IgnoreMe.dll
pause