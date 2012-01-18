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

    Private Declare Function OpenThread Lib "kernel32.dll" (ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwThreadId As Integer) As Integer
    Private Declare Function SuspendThread Lib "kernel32.dll" (ByVal hThread As Integer) As Integer
    Private Declare Function ResumeThread Lib "kernel32.dll" (ByVal hThread As Integer) As Integer
    Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hHandle As Integer) As Integer
    Private Declare Function SetLastError Lib "kernel32" (ByVal dwErrCode As Integer) As Integer

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
        Dim th As Integer
        For Each t As ProcessThread In process.Threads
            SetLastError(0)
            th = OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            If th <= 0 AndAlso Marshal.GetLastWin32Error() <> 0 Then
                HandleError("::process", "Error with Kernel32!OpenThread() : " & New Win32Exception(Marshal.GetLastWin32Error()).ToString())
                Exit Sub
            End If
            SetLastError(0)
            If SuspendThread(th) <= 0 AndAlso Marshal.GetLastWin32Error() <> 0 Then
                HandleError("::process", "Error with Kernel32!SuspendThread() : " & New Win32Exception(Marshal.GetLastWin32Error()).ToString())
                Exit Sub
            End If
            SetLastError(0)
            If CloseHandle(th) <= 0 AndAlso Marshal.GetLastWin32Error() <> 0 Then
                HandleError("::process", "Error with Kernel32!CloseHandle() : " & New Win32Exception(Marshal.GetLastWin32Error()).ToString())
                Exit Sub
            End If
        Next
    End Sub

    Public Sub ResumeProcess(ByVal process As Process)
        Dim th As Integer
        For Each t As ProcessThread In process.Threads
            th = OpenThread(ThreadAccess.SuspendResume, False, t.Id)
            If th <= 0 AndAlso Marshal.GetLastWin32Error() <> 0 Then
                HandleError("::process", "Error with Kernel32!OpenThread() : " & New Win32Exception(Marshal.GetLastWin32Error()).ToString())
                Exit Sub
            End If
            If ResumeThread(th) <= 0 AndAlso Marshal.GetLastWin32Error() <> 0 Then
                HandleError("::process", "Error with Kernel32!ResumeThread() : " & New Win32Exception(Marshal.GetLastWin32Error()).ToString())
                Exit Sub
            End If
            If CloseHandle(th) <= 0 AndAlso Marshal.GetLastWin32Error() <> 0 Then
                HandleError("::process", "Error with Kernel32!CloseHandle() : " & New Win32Exception(Marshal.GetLastWin32Error()).ToString())
                Exit Sub
            End If
        Next
    End Sub

End Module
