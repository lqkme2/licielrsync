''----------------------------------------------------------------------------------------------
''
'' LicielRsync -  A multi-threaded interface for Rsync on Windows
'' By Arnaud Dovi - ad@heapoverflow.com
'' Rsync - http://rsync.samba.org
''
'' ModuleProcess
''
'' Process functions
''----------------------------------------------------------------------------------------------
Option Explicit On


Module ModuleProcess

    Private Declare Function OpenThread Lib "kernel32.dll" (ByVal dwDesiredAccess As ThreadAccess, ByVal bInheritHandle As Boolean, ByVal dwThreadId As UInteger) As IntPtr
    Private Declare Function SuspendThread Lib "kernel32.dll" (ByVal hThread As IntPtr) As UInteger
    Private Declare Function ResumeThread Lib "kernel32.dll" (ByVal hThread As IntPtr) As UInteger
    Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hHandle As IntPtr) As Boolean

    Public Enum ThreadAccess As Integer
        Terminate = (&H1)
        SuspendResume = (&H2)
        GetContext = (&H8)
        SetContext = (&H10)
        SetInformation = (&H20)
        QueryInformation = (&H40)
        SetThreadToken = (&H80)
        Impersonate = (&H100)
        DirectImpersonation = (&H200)
    End Enum

    Public Sub SuspendProcess(ByVal process As Process)
        For Each t As ProcessThread In process.Threads
            Dim th As IntPtr = OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            If th <> IntPtr.Zero Then
                SuspendThread(th)
                CloseHandle(th)
            End If
        Next
    End Sub

    Public Sub ResumeProcess(ByVal process As Process)
        For Each t As ProcessThread In process.Threads
            Dim th As IntPtr = OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            If th <> IntPtr.Zero Then
                ResumeThread(th)
                CloseHandle(th)
            End If
        Next
    End Sub

End Module
