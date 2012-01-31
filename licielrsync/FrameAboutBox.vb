Public NotInheritable Class FrameAboutBox

    Private Sub AboutBox1Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim applicationTitle As String
        If My.Application.Info.Title <> "" Then
            applicationTitle = My.Application.Info.Title
        Else
            applicationTitle = IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Text = String.Format("About {0}", applicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        LabelProductName.Text = My.Application.Info.ProductName
        LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        LabelCopyright.Text = My.Application.Info.Copyright
        LabelCompanyName.Text = My.Application.Info.CompanyName
        TextBoxDescription.Text = My.Application.Info.Description
    End Sub

    Private Sub OkButtonClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles OKButton.Click
        Close()
    End Sub

    Private Shared Sub TableLayoutPanelPaint(sender As System.Object, e As Windows.Forms.PaintEventArgs) Handles TableLayoutPanel.Paint

    End Sub
End Class
