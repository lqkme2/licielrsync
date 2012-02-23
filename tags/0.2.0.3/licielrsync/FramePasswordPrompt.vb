Friend Class FramePasswordPrompt
    Friend Passwd As String = Nothing

    Private Sub OnClickButton1(sender As System.Object, e As EventArgs) Handles Button1.Click
        Passwd = TextBox1.Text
        TextBox1.Text = ""
        Close()
    End Sub

    Private Sub OnLoadFramePasswordPrompt(sender As System.Object, e As EventArgs) Handles MyBase.Load
        Icon = AppIcon
        TextBox1.Text = ""
        TextBox1.Select()
        Dim originalWidth As Integer = Width
        Width = Label1.Width + 60
        Location = New Point(Location.X - ((Width - originalWidth) / 2), Location.Y)
    End Sub

    Private Sub OnKeyUpTextBox1(sender As System.Object, e As Windows.Forms.KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyCode = Keys.Enter Then
            Button1.PerformClick()
        End If
    End Sub
End Class