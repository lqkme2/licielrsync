Imports System.Threading

Friend NotInheritable Class LicielMessage

    Private Shared Sub Actualize(ByVal sender As System.Object, ByVal e As EventArgs)
        sender.Activate()
    End Sub

    Public Shared Function Send(ByVal message As String, ByVal title As String, ByVal buttons As MessageBoxButtons, ByVal icons As MessageBoxIcon, Optional parent As Form = Nothing) As DialogResult
        Dim topmostForm As Form = Nothing
        Dim result As DialogResult = DialogResult.None
        Dim newThread As Thread = Nothing
        Dim messageBoxRect As SafeNativeMethods.Rect
        Dim messageBoxHwnd As IntPtr
        Dim messageBoxWidth As Integer
        Dim messageBoxHeight As Integer
        Try
            topmostForm = New Form()
            topmostForm.Name = "topmostForm"
            topmostForm.Icon = AppIcon
            topmostForm.Size = New Size(100, 100)
            topmostForm.StartPosition = FormStartPosition.CenterParent
            topmostForm.Location = New Point(0, 0)
            topmostForm.Opacity = 0
            topmostForm.TopMost = True
            topmostForm.ShowInTaskbar = False
            AddHandler topmostForm.Deactivate, AddressOf Actualize
            ''
            '' The trick here is to use display a ShowDialog asynchronously so you can
            '' reposition the messagebox relatively to it, we know .CenterParent
            '' is working with ShowDialog() and buggy with Show() and .CenterParent gives
            '' the real center unlike the common way loc.X / 2 most devs do, this last does
            '' not work well because the Location property does not consider borders
            '' when .CenterParent does it
            ''
            '' FindWindow is used to detect when the MessageBox has spawned and it returns its window handle
            '' Once the handle is found, a call to SetWindowPos reposition it, relative to the hidden ShowDialog
            '' Invokes are used to bypass cross thread operations.
            ''
            parent.BeginInvoke(New MethodInvoker(Sub() topmostForm.ShowDialog(parent)))
            newThread = New Thread(Sub(o As Object) parent.Invoke(New MethodInvoker(Sub() result = MessageBox.Show(topmostForm, message, title, buttons, icons))))
            newThread.IsBackground = True
            newThread.Start()
            Do
                Thread.Sleep(1)
                messageBoxHwnd = SafeNativeMethods.FindWindow(IntPtr.Zero, title)
            Loop While messageBoxHwnd = IntPtr.Zero
            SafeNativeMethods.GetWindowRect(messageBoxHwnd, messageBoxRect)
            messageBoxWidth = CType(messageBoxRect.Right, Integer) - CType(messageBoxRect.Left, Integer)
            messageBoxHeight = CType(messageBoxRect.Top, Integer) - CType(messageBoxRect.Bottom, Integer)
            SafeNativeMethods.SetWindowPos(messageBoxHwnd, 0, (topmostForm.Location.X - (messageBoxWidth / 2)) + (topmostForm.Width / 2), (topmostForm.Location.Y + (messageBoxHeight / 2)) + (topmostForm.Height / 2), 0, 0, &H20 Or &H4 Or &H1)
            While result = DialogResult.None
                Thread.Sleep(1)
            End While
        Catch ex As Exception
            MsgBox(ex.ToString)
            End
        Finally
            parent.Invoke(New MethodInvoker(Sub() If Not topmostForm Is Nothing Then topmostForm.Close()))
            If Not newThread Is Nothing AndAlso newThread.IsAlive Then newThread.Abort()
        End Try
        Return result
    End Function
    Public Shared Sub SendTray(ByVal fm As FrameMain, ByVal message As String, ByVal title As String, ByVal t As ToolTipIcon, ByVal time As Long)
        fm.NotifyIcon1.Icon = AppIcon
        fm.NotifyIcon1.ShowBalloonTip(time, title, message, t)
    End Sub
End Class