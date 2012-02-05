''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' moduleprocess
''
'' process manipulations
''----------------------------------------------------------------------------------------------



Imports System.ComponentModel
Imports System.Runtime.InteropServices

Module ModuleProcess

    Private Enum ThreadAccess As Integer
        'Terminate = &H1
        SuspendResume = &H2
        'GetContext = &H8
        'SetContext = &H10
        'SetInformation = &H20
        'QueryInformation = &H40
        'SetThreadToken = &H80
        'Impersonate = &H100
        'DirectImpersonation = &H200
    End Enum

    Friend Sub SuspendProcess(ByVal process As Process)
        Dim th As IntPtr, ret As Integer, errcode As Integer
        For Each t As ProcessThread In process.Threads
            SafeNativeMethods.SetLastError(0)
            th = SafeNativeMethods.OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            ret = Marshal.GetLastWin32Error()
            If th = IntPtr.Zero AndAlso ret <> 0 Then
                HandleError("::process", String.Format("Error with Kernel32!OpenThread() : {0}", New Win32Exception(errcode).ToString()))
                Exit Sub
            End If
            SafeNativeMethods.SetLastError(0)
            ret = SafeNativeMethods.SuspendThread(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", String.Format("Error with Kernel32!SuspendThread() : {0}", New Win32Exception(errcode).ToString()))
                Exit Sub
            End If
            SafeNativeMethods.SetLastError(0)
            ret = SafeNativeMethods.CloseHandle(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", String.Format("Error with Kernel32!CloseHandle() : {0}", New Win32Exception(errcode).ToString()))
                Exit Sub
            End If
        Next
    End Sub

    Friend Sub ResumeProcess(ByVal process As Process)
        Dim th As IntPtr, ret As Integer, errcode As Integer
        For Each t As ProcessThread In process.Threads
            SafeNativeMethods.SetLastError(0)
            th = SafeNativeMethods.OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            errcode = Marshal.GetLastWin32Error()
            If th = IntPtr.Zero AndAlso errcode <> 0 Then
                HandleError("::process", String.Format("Error with Kernel32!OpenThread() : {0}", New Win32Exception(errcode).ToString()))
                Exit Sub
            End If
            SafeNativeMethods.SetLastError(0)
            ret = SafeNativeMethods.ResumeThread(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", String.Format("Error with Kernel32!ResumeThread() : {0}", New Win32Exception(errcode).ToString()))
                Exit Sub
            End If
            SafeNativeMethods.SetLastError(0)
            ret = SafeNativeMethods.CloseHandle(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", String.Format("Error with Kernel32!CloseHandle() : {0}", New Win32Exception(errcode).ToString()))
                Exit Sub
            End If
        Next
    End Sub

End Module
