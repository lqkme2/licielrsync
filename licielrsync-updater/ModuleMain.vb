''--------------------------------------------------------------------
''
'' liciel_transfert_updater - Outil mise à jour de transfert des fichiers utilisateurs
'' Copyright (C) 2011 LICIEL Environnement
'' Par Arnaud DOVI et Stéphane DELOT - http://www.liciel.com
''
'' Modifications, reproductions et diffusions interdites
''
'' ModuleMain
''
'' Point d'entrée et fonctions principales
''--------------------------------------------------------------------


Module ModuleMain


    ''--------------------------------------------------------------------
    ''                        L O C A L E S
    ''--------------------------------------------------------------------

    Private ReadOnly AppPath As String = My.Application.Info.DirectoryPath & "\"
    Private ReadOnly LicielRsyncPathPacked As String = AppPath & "licielrsync"
    Private ReadOnly LicielRsyncPath As String = AppPath & "..\"

    ''--------------------------------------------------------------------
    '' Main
    ''
    '' Point d'entrée du programme
    ''--------------------------------------------------------------------

    Sub Main()
        Dim retryTimer As Diagnostics.Stopwatch = Nothing, tick As Long = 0
        Try
            If Not My.Application.CommandLineArgs.Contains("--update") Then End

            Dim p() As Diagnostics.Process = Diagnostics.Process.GetProcessesByName(Diagnostics.Process.GetCurrentProcess().ProcessName)
            If p.Length > 1 Then Environment.Exit(0)

            For Each dirPath In IO.Directory.GetDirectories(LicielRsyncPathPacked, "*", IO.SearchOption.AllDirectories)
                IO.Directory.CreateDirectory(dirPath.Replace(LicielRsyncPathPacked, LicielRsyncPath))
            Next

            For Each newPath In IO.Directory.GetFiles(LicielRsyncPathPacked, "*.*", IO.SearchOption.AllDirectories)
                IO.File.Copy(newPath, newPath.Replace(LicielRsyncPathPacked, LicielRsyncPath), True)
            Next
        Catch ex As Exception
            SafeNativeMethods.MessageBox(IntPtr.Zero, ex.ToString, "licielrsync-updater exception", &H10)
        End Try
        Try
retry:
            Diagnostics.Process.Start(LicielRsyncPath & "licielrsync.exe")
        Catch ex As Exception
            If retryTimer Is Nothing Then
                retryTimer = New Diagnostics.Stopwatch()
                retryTimer.Start()
            End If
            Threading.Thread.Sleep(500)
            If retryTimer.ElapsedMilliseconds > 20000 Then
                SafeNativeMethods.MessageBox(IntPtr.Zero, "Failed to restart licielrsync after 20sec.", "licielrsync-updater", &H10)
                End
            End If
            GoTo retry
        End Try

    End Sub

End Module

Friend NotInheritable Class SafeNativeMethods
    Public Declare Function MessageBox Lib "user32.dll" Alias "MessageBoxA" (ByVal hWnd As IntPtr, ByVal msg As String, ByVal title As String, ByVal lParam As Integer) As Integer
End Class
