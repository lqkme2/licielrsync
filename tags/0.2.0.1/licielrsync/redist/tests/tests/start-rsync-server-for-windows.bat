@ECHO OFF
SETLOCAL EnableDelayedExpansion
SET rsyncdir=rsync-3.0.9
IF NOT EXIST "..\rsync\%rsyncdir%\tmp" md ..\rsync\%rsyncdir%\tmp
IF NOT EXIST "..\rsync\%rsyncdir%\etc" md ..\rsync\%rsyncdir%\etc
..\rsync\%rsyncdir%\bin\rsync.exe --daemon --config=../tests/win-rsyncd.conf
copy /Y group ..\rsync\%rsyncdir%\etc\
copy /Y passwd ..\rsync\%rsyncdir%\etc\
copy /Y rsync.secrets.txt ..\rsync\%rsyncdir%\etc\
SETLOCAL DisableDelayedExpansion
echo RSYNC SERVER STARTED (..\rsync\%rsyncdir%\bin\rsync.exe)
PAUSE