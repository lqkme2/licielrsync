Option Explicit On

Namespace My

    ''--------------------------------------------------------------------
    '' Main
    ''
    '' This is safer to load from here than using the main frame events
    '' 
    '' Startup: Raised when the application starts, before the startup form is created.
    '' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    '' UnhandledException: Raised if the application encounters an unhandled exception.
    '' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    '' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    ''--------------------------------------------------------------------


    Partial Friend Class MyApplication

        Private Shared Sub MyApplicationStartup(ByVal sender As Object, e As EventArgs) Handles Me.Startup
            'ModuleMain.Main()
        End Sub

        Private Shared Sub MyApplicationShutdown(ByVal sender As Object, e As EventArgs) Handles Me.Shutdown
            Try
                If Not FrameMain.NotifyIcon1 Is Nothing Then FrameMain.NotifyIcon1.Visible = False
                If Not Processus Is Nothing Then
                    Processus.Kill()
                    Processus.Close()
                End If
            Catch
            End Try
        End Sub
    End Class

End Namespace

