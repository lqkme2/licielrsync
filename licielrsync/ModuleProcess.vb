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

    Public Sub SuspendProcess(ByVal process As Process)
        Dim th As IntPtr, ret As Integer, errcode As Integer
        For Each t As ProcessThread In process.Threads
            th = SafeNativeMethods.OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            ret = Marshal.GetLastWin32Error()
            If th = IntPtr.Zero AndAlso ret <> 0 Then
                HandleError("::process", "Error with Kernel32!OpenThread() : " & New Win32Exception(errcode).ToString())
                Exit Sub
            End If
            ret = SafeNativeMethods.SuspendThread(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", "Error with Kernel32!SuspendThread() : " & New Win32Exception(errcode).ToString())
                Exit Sub
            End If
            ret = SafeNativeMethods.CloseHandle(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", "Error with Kernel32!CloseHandle() : " & New Win32Exception(errcode).ToString())
                Exit Sub
            End If
        Next
    End Sub

    Public Sub ResumeProcess(ByVal process As Process)
        Dim th As IntPtr, ret As Integer, errcode As Integer
        For Each t As ProcessThread In process.Threads
            th = SafeNativeMethods.OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            errcode = Marshal.GetLastWin32Error()
            If th = IntPtr.Zero AndAlso errcode <> 0 Then
                HandleError("::process", "Error with Kernel32!OpenThread() : " & New Win32Exception(errcode).ToString())
                Exit Sub
            End If
            ret = SafeNativeMethods.ResumeThread(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", "Error with Kernel32!ResumeThread() : " & New Win32Exception(errcode).ToString())
                Exit Sub
            End If
            ret = SafeNativeMethods.CloseHandle(th)
            errcode = Marshal.GetLastWin32Error()
            If ret <= 0 AndAlso errcode <> 0 Then
                HandleError("::process", "Error with Kernel32!CloseHandle() : " & New Win32Exception(errcode).ToString())
                Exit Sub
            End If
        Next
    End Sub

End Module
