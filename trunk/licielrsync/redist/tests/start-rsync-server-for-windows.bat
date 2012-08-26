@ECHO OFF
SETLOCAL EnableDelayedExpansion
SET rsyncdir=rsync-3.0.9
IF NOT EXIST "..\rsync\%rsyncdir%\tmp" md "..\rsync\%rsyncdir%\tmp"
IF NOT EXIST "..\rsync\%rsyncdir%\etc" md "..\rsync\%rsyncdir%\etc"
copy /Y mkgroup.exe "..\rsync\%rsyncdir%\bin"
copy /Y mkpasswd.exe "..\rsync\%rsyncdir%\bin"
copy /Y rsync.secrets.txt "..\rsync\%rsyncdir%\etc"
"..\rsync\%rsyncdir%\bin\mkgroup.exe" -l > "..\rsync\%rsyncdir%\etc\group"
"..\rsync\%rsyncdir%\bin\mkpasswd.exe" -l > "..\rsync\%rsyncdir%\etc\passwd"
"..\rsync\%rsyncdir%\bin\rsync.exe" --daemon --config="../tests/win-rsyncd.conf"
IF %errorlevel% == 0 echo RSYNC SERVER STARTED (..\rsync\%rsyncdir%\bin\rsync.exe)
SETLOCAL DisableDelayedExpansion
PAUSE