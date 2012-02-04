''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' moduletaskbarwin7.vb
''
'' display the work progress on the taskbar icon (Windows 7 only)
''----------------------------------------------------------------------------------------------



Imports System.Runtime.InteropServices

Module ModuleTaskbarWin7
    Friend IsWin7 = Environment.OSVersion.Version.Major > 6 OrElse (Environment.OSVersion.Version.Major = 6 AndAlso Environment.OSVersion.Version.Minor >= 1)

    Friend Enum Tbpflag As Integer
        TbpfNoprogress = 0
        TbpfIndeterminate = &H1
        TbpfNormal = &H2
        TbpfError = &H4
        TbpfPaused = &H8
    End Enum

    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C43DC798-95D1-4BEA-9030-BB99E2983A1A")>
    Friend Interface ITaskbarList4
        ''
        '' ITaskbarList
        ''
        Sub HrInit()
        Sub AddTab(hwnd As Integer)
        Sub DeleteTab(hwnd As Integer)
        Sub ActivateTab(hwnd As Integer)
        Sub SetActiveAlt(hwnd As Integer)
        ''
        '' ITaskbarList2
        ''
        Sub MarkFullscreenWindow(hwnd As Integer, ByVal fFullscreen As Boolean)
        ''
        '' ITaskbarList3
        ''
        Sub SetProgressValue(ByVal hwnd As Integer, ByVal ullCompleted As Long, ByVal ullTotal As Long)
        Sub SetProgressState(ByVal hwnd As Integer, ByVal tbpFlags As Tbpflag)
    End Interface

    <ComImport(), ClassInterfaceAttribute(ClassInterfaceType.None), Guid("56FDF344-FD6D-11d0-958A-006097C9A090")>
    Friend Class CTaskbarList
    End Class

    Friend TaskBar As ITaskbarList4 = Nothing

End Module
