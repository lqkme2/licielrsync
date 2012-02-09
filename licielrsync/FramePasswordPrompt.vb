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
    End Sub
End Class