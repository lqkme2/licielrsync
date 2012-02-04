''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' frameaboutbox
''
'' about frame
''----------------------------------------------------------------------------------------------



Friend NotInheritable Class FrameAboutBox

    Private Sub OnLoadAboutBox1(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        Text = String.Format("About {0}", AppAssembly)
        LabelProductName.Text = My.Application.Info.ProductName
        LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        LabelCopyright.Text = My.Application.Info.Copyright
        Label1.Text = "licielrsync.googlecode.com"
        TextBoxDescription.Text = L("about")
    End Sub

    Private Sub OnClickOkButton(ByVal sender As System.Object, ByVal e As EventArgs) Handles OKButton.Click
        Close()
    End Sub

    Private Sub OnClickWebButton(sender As System.Object, e As EventArgs) Handles WebButton.Click
        Process.Start("explorer.exe", """http://licielrsync.googlecode.com""")
    End Sub

End Class
