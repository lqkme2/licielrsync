''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' classmessages
''
'' LicielMessage.Send() : makes a topmost messagebox centered to its parent window
''
'' the trick to get the true center is to display a showdialog asynchronously so you can
'' reposition the messagebox relatively to it, we know .centerparent
'' is working with showdialog() and buggy with show() and .centerparent gives
'' the real center unlike the common calculated method most developers choose today, this last does
'' not center perfectly because the location property does not consider borders which are of
'' various sizes from a windows to another one. centerparent circumvent this limitation
''
'' so findwindow is used to detect when the messagebox has spawned and it returns its window handle.
'' once the handle is found, a call to setwindowpos reposition it, relative to the hidden showdialog.
''----------------------------------------------------------------------------------------------



Friend NotInheritable Class LicielMessage

    Public Shared Function Send(ByVal message As String, ByVal title As String, ByVal buttons As MessageBoxButtons, ByVal icons As MessageBoxIcon, Optional parent As FrameMain = Nothing) As DialogResult
        Dim topmostForm As Form = Nothing
        Dim result As DialogResult = DialogResult.None
        Dim newThread As Threading.Thread = Nothing
        Dim isParentValid = Not parent Is Nothing AndAlso parent.Visible AndAlso parent.Location.X >= -100 AndAlso parent.Location.Y >= -100
        Try
            topmostForm = New Form()
            topmostForm.Icon = AppIcon
            topmostForm.Size = New Size(40, 40)
            topmostForm.StartPosition = FormStartPosition.CenterScreen
            topmostForm.Location = New Point(0, 0)
            topmostForm.Opacity = 0
            topmostForm.TopMost = True
            topmostForm.ShowInTaskbar = False
            If isParentValid Then
                topmostForm.StartPosition = FormStartPosition.CenterParent
                newThread = New Threading.Thread(Sub(i As Object)
                                                     Dim timeout As New Stopwatch
                                                     Dim messageBoxWidth As Integer
                                                     Dim messageBoxHeight As Integer
                                                     Dim messageBoxRect As SafeNativeMethods.Rect
                                                     Dim messageBoxHwnd As IntPtr
                                                     Try
                                                         parent.BeginInvoke(New MethodInvoker(Sub() topmostForm.ShowDialog(parent)))
                                                         newThread = New Threading.Thread(Sub(j As Object) parent.Invoke(New MethodInvoker(Sub() result = MessageBox.Show(topmostForm, message, title, buttons, icons))))
                                                         newThread.IsBackground = True
                                                         newThread.Start()
                                                         Do
                                                             Threading.Thread.Sleep(1)
                                                             messageBoxHwnd = SafeNativeMethods.FindWindow(IntPtr.Zero, title)
                                                             If Not Sleep(timeout) Then Throw New ApplicationException()
                                                         Loop While messageBoxHwnd = IntPtr.Zero
                                                         SafeNativeMethods.GetWindowRect(messageBoxHwnd, messageBoxRect)
                                                         messageBoxWidth = CType(messageBoxRect.Right, Integer) - CType(messageBoxRect.Left, Integer)
                                                         messageBoxHeight = CType(messageBoxRect.Bottom, Integer) - CType(messageBoxRect.Top, Integer)
                                                         SafeNativeMethods.SetWindowPos(messageBoxHwnd, 0, (topmostForm.Location.X - (messageBoxWidth / 2)) + (topmostForm.Width / 2), (topmostForm.Location.Y - (messageBoxHeight / 2)) + (topmostForm.Height / 2), 0, 0, &H20 Or &H4 Or &H1)
                                                         While result = DialogResult.None
                                                             Threading.Thread.Sleep(1)
                                                         End While
                                                     Catch
                                                     Finally
                                                         If Not topmostForm Is Nothing AndAlso Not topmostForm.IsDisposed Then parent.Invoke(New MethodInvoker(Sub() topmostForm.Close()))
                                                         If Not newThread Is Nothing AndAlso newThread.IsAlive Then newThread.Abort()
                                                     End Try
                                                 End Sub)
                newThread.IsBackground = False
                newThread.Start()
                While result = DialogResult.None
                    Application.DoEvents()
                End While
            Else
                result = MessageBox.Show(topmostForm, message, title, buttons, icons)
            End If
        Catch ex As ApplicationException
            result = MessageBox.Show(topmostForm, message, title, buttons, icons)
        Catch ex As Exception
            HandleError("", ex.ToString)
        Finally
            If isParentValid AndAlso Not topmostForm Is Nothing AndAlso Not topmostForm.IsDisposed Then
                parent.Invoke(New MethodInvoker(Sub() topmostForm.Close()))
            ElseIf Not topmostForm Is Nothing AndAlso Not topmostForm.IsDisposed Then
                topmostForm.Close()
            End If
            If Not newThread Is Nothing AndAlso newThread.IsAlive Then newThread.Abort()
        End Try
        Return result
    End Function

    Public Shared Sub SendTray(ByVal fm As FrameMain, ByVal message As String, ByVal title As String, ByVal t As ToolTipIcon, ByVal time As Long)
        'fm.NotifyIcon1.Icon = AppIcon
        fm.NotifyIcon1.ShowBalloonTip(time, title, message, t)
    End Sub

    Private Shared Function Sleep(ByVal timeout As Stopwatch) As Boolean
        ''
        '' Timeout 3 seconds
        ''
        If Not timeout.IsRunning Then timeout.Start()
        If timeout.ElapsedMilliseconds > 3000 Then Return False
        Return True
    End Function
End Class
