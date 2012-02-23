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
        Try
            Dim threadId As Integer, handle As Object
            For Each t As ProcessThread In process.Threads
                threadId = t.Id
                Verify(Function() UnsafeNativeMethods.OpenThread(ThreadAccess.SuspendResume, False, threadId))
                handle = VerifyReturn
                Verify(Function() UnsafeNativeMethods.SuspendThread(handle))
                Verify(Function() UnsafeNativeMethods.CloseHandle(handle))
            Next
        Catch ex As Exception
            HandleError("", ex.ToString)
        End Try
    End Sub

    Friend Sub ResumeProcess(ByVal process As Process)
        Try
            Dim threadId As Integer, handle As Object
            For Each t As ProcessThread In process.Threads
                threadId = t.Id
                Verify(Function() UnsafeNativeMethods.OpenThread(ThreadAccess.SuspendResume, False, threadId))
                handle = VerifyReturn
                Verify(Function() UnsafeNativeMethods.ResumeThread(handle))
                Verify(Function() UnsafeNativeMethods.CloseHandle(handle))
            Next
        Catch ex As Exception
            HandleError("", ex.ToString)
        End Try
    End Sub

End Module
