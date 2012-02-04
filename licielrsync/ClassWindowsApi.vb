''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' classwindowsapi
''
'' functions built in windows dll libraries
''----------------------------------------------------------------------------------------------



Friend NotInheritable Class SafeNativeMethods
    Friend Declare Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    Friend Declare Function OpenThread Lib "kernel32.dll" (ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwThreadId As Integer) As IntPtr
    Friend Declare Function SuspendThread Lib "kernel32.dll" (ByVal hThread As IntPtr) As Integer
    Friend Declare Function ResumeThread Lib "kernel32.dll" (ByVal hThread As IntPtr) As Integer
    Friend Declare Function CloseHandle Lib "kernel32.dll" (ByVal hHandle As IntPtr) As Integer
    Friend Declare Function SetLastError Lib "kernel32.dll" (ByVal dwErrCode As Integer) As Integer
    Friend Declare Function ReplaceFile Lib "kernel32.dll" Alias "ReplaceFileA" (ByVal lpReplacedFileName As String, ByVal lpReplacementFileName As String, ByVal lpBackupFileName As Object, ByVal dwReplaceFlags As Integer, ByVal lpExclude As IntPtr, ByVal lpReserved As IntPtr) As Integer
    Friend Declare Function SetParent Lib "user32.dll" (ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As IntPtr
    Friend Declare Function SetWindowLong Lib "user32.dll" Alias "SetWindowLongA" (ByVal hWnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Long) As Integer
    Friend Declare Function SetWindowPos Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As Integer) As Boolean
    Friend Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (ByVal zero As IntPtr, ByVal lpWindowName As String) As IntPtr
    Friend Declare Function GetWindowRect Lib "user32.dll" (ByVal hwnd As IntPtr, ByRef lpRect As Rect) As Int32

    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)>
    Friend Structure Rect
        Friend ReadOnly Left As IntPtr
        Friend ReadOnly Top As IntPtr
        Friend ReadOnly Right As IntPtr
        Friend ReadOnly Bottom As IntPtr
    End Structure
End Class
