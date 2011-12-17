''----------------------------------------------------------------------------------------------
''
'' LicielRsync -  A multi-threaded interface for Rsync on Windows
'' By Arnaud Dovi - ad@heapoverflow.com
'' Rsync - http://rsync.samba.org
''
'' FrameMain
''
'' Primary interface
''----------------------------------------------------------------------------------------------
Option Explicit On


Imports System
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading


Public Class FrameMain

    ''--------------------------------------------------------------------
    '' ButtonClick
    ''
    '' Stub function used to handle all button clicks
    ''--------------------------------------------------------------------

    Private Sub ButtonClick(sender As System.Object, e As EventArgs) Handles ButtonExec.Click, ButtonTest.Click, ButtonAdd.Click, ButtonDel.Click, ButtonSrcOpen.Click, ButtonDstOpen.Click, ButtonPause.Click, ButtonStop.Click
        Try
            Select Case sender.name
                Case ButtonExec.Name, ButtonTest.Name
                    If File.Exists(RsyncPath) And TextBoxSrc.Text <> "" And TextBoxDst.Text <> "" Then
                        TextBoxLogs.Text = ""
                        TextBoxErrors.Text = ""
                        ResetProgress()
                        Dim thd As Thread = New Thread(AddressOf ThreadProcessStart)
                        thd.IsBackground = True
                        thd.Start({RsyncPath, sender.name = ButtonTest.Name})
                    End If
                Case ButtonSrcOpen.Name
                    TextBoxSrc.Text = GetDirectory(TextBoxSrc.Text)
                    'TextBoxSrc.Text = "C:\TEST1\"
                Case ButtonDstOpen.Name
                    TextBoxDst.Text = GetDirectory(TextBoxDst.Text)
                    'TextBoxDst.Text = "E:\TEST2\"
                Case ButtonAdd.Name
                    Dim newProfile As String = ComboProfiles.Text
                    newProfile = Regex.Replace(newProfile, "^\s+", "")
                    newProfile = Regex.Replace(newProfile, "\s+$", "")
                    If newProfile = "" Or My.Settings.ProfilesList.Contains(newProfile) Or Regex.Match(newProfile, "^\s+$").Success Then Exit Sub
                    My.Settings.ProfilesList.Add(newProfile)
                    My.Settings.Save()
                    ComboProfiles.Items.AddRange(New Object() {newProfile})
                    LoadProfile(newProfile)
                Case ButtonDel.Name
                    Dim todeleteProfile As String = ComboProfiles.Text
                    If todeleteProfile = "" Or todeleteProfile = My.Settings.Properties.Item("CurrentProfile").DefaultValue Or Not My.Settings.ProfilesList.Contains(todeleteProfile) Or Regex.Match(todeleteProfile, "^\s+$").Success Then Exit Sub
                    My.Settings.ProfilesList.Remove(todeleteProfile)
                    My.Settings.P(todeleteProfile) = Nothing
                    My.Settings.Save()
                    ComboProfiles.Items.Remove(todeleteProfile)
                    ComboProfiles.SelectedIndex = 0
                Case ButtonPause.Name
                    Select Case ProcessusSuspended
                        Case True
                            ResumeProcess(Processus)
                            ProcessusSuspended = False
                            ButtonPause.Text = "Pause"
                        Case False
                            SuspendProcess(Processus)
                            ProcessusSuspended = True
                            ButtonPause.Text = "Resume"
                    End Select
                Case ButtonStop.Name
                    If ProcessusSuspended Then
                        ButtonPause.Text = "Pause"
                        ProcessusSuspended = False
                    End If
                    Processus.Kill()
                    Processus.Close()
            End Select
            UpdateStatusBarCommand(sender.name = ButtonTest.Name)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub


    ''--------------------------------------------------------------------
    '' CheckBoxChanged
    ''
    '' Stub function used to handle all checkbox checks
    ''--------------------------------------------------------------------

    Private Sub CheckBoxChanged(sender As Object, e As EventArgs) _
     Handles CbProgress.CheckedChanged, CbRecurse.CheckedChanged, CbDate.CheckedChanged, _
             CbPerm.CheckedChanged, CbVerbose.CheckedChanged, CbGroup.CheckedChanged, CbOwner.CheckedChanged, CbDelta.CheckedChanged, CbFrench.CheckedChanged, CbEnglish.CheckedChanged, CbDelete.CheckedChanged, CbExisting.CheckedChanged, CbNewer.CheckedChanged, CbSizeOnly.CheckedChanged, CbReadable.CheckedChanged, CbPermWin.CheckedChanged, CbWinCompat.CheckedChanged, CbExistingOnly.CheckedChanged, CbChecksum.CheckedChanged, CbIgnoreTimes.CheckedChanged, CbShowCmd.CheckedChanged, CbHideWindows.CheckedChanged, CbRedir.CheckedChanged
        Select Case sender.Name
            Case CbEnglish.Name
                If Not sender.checked Then Exit Sub
                ChangeLanguage(sender.Tag)
                My.Settings.Locales = "English"
                My.Settings.Save()
                CbEnglish.CheckOnClick = Not CbEnglish.Checked
                CbFrench.CheckOnClick = CbEnglish.Checked
                CbFrench.Checked = Not sender.Checked
                Exit Sub
            Case CbFrench.Name
                If Not sender.Checked Then Exit Sub
                ChangeLanguage(sender.Tag)
                My.Settings.Locales = "French"
                My.Settings.Save()
                CbFrench.CheckOnClick = Not CbFrench.Checked
                CbEnglish.CheckOnClick = CbFrench.Checked
                CbEnglish.Checked = Not sender.Checked
                Exit Sub
            Case CbShowCmd.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")("showcmd") = sender.Checked
                My.Settings.Save()
                UpdateStatusBarCommand(False)
                Exit Sub
            Case CbHideWindows.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")("hidewnd") = sender.Checked
                My.Settings.Save()
                Exit Sub
            Case CbRedir.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")("redir") = sender.Checked
                My.Settings.Save()
                Exit Sub
        End Select
        My.Settings.P(My.Settings.CurrentProfile)("OptionsSwitch")(sender.Tag) = sender.Checked
        My.Settings.Save()
        If sender.Name = CbVerbose.Name Then ComboVerbose.Enabled = CbVerbose.Checked
        UpdateStatusBarCommand(False)
    End Sub

    ''--------------------------------------------------------------------
    '' ComboSelectedIndexChanged
    ''
    '' Stub function used to handle all listbox changes
    ''--------------------------------------------------------------------

    Private Sub ComboSelectedIndexChanged(sender As System.Object, e As EventArgs) Handles ComboVerbose.SelectedIndexChanged, ComboProfiles.SelectedIndexChanged, ComboRsync.SelectedIndexChanged
        Select Case sender.name
            Case ComboVerbose.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")(sender.tag) = sender.Text
                My.Settings.Save()
            Case ComboProfiles.Name
                Dim selectedProfile = ComboProfiles.Items(ComboProfiles.SelectedIndex)
                LoadProfile(selectedProfile)
            Case ComboRsync.Name
                RsyncPath = RsyncPaths(ComboRsync.Items(ComboRsync.SelectedIndex)) & "\bin\rsync.exe"
        End Select
        UpdateStatusBarCommand(False)
    End Sub

    ''--------------------------------------------------------------------
    '' TextBoxTextChanged
    ''
    '' Stub function used to handle all textbox changes
    ''--------------------------------------------------------------------

    Private Sub TextBoxTextChanged(sender As System.Object, e As EventArgs) Handles TextBoxSrc.TextChanged, TextBoxDst.TextChanged, TextBoxOptions.TextChanged
        Select Case sender.name
            Case TextBoxSrc.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")("srcpath") = TextBoxSrc.Text
                My.Settings.Save()
            Case TextBoxDst.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")("dstpath") = TextBoxDst.Text
                My.Settings.Save()
            Case TextBoxOptions.Name
                My.Settings.P(My.Settings.CurrentProfile)("OptionsVar")("customoptions") = TextBoxOptions.Text
                My.Settings.Save()
                UpdateStatusBarCommand(False)
        End Select
    End Sub

    ''--------------------------------------------------------------------
    '' Size
    ''
    '' To work around a minimize problem while saving the size with
    '' OnPropertyChanged
    ''--------------------------------------------------------------------

    Private Sub FrameMainValidateSize(sender As System.Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        DataBindings.Item(1).WriteValue()
    End Sub

    Private Sub FrameMainResizeEnd(sender As System.Object, e As EventArgs) Handles MyBase.ResizeEnd
        DataBindings.Item(1).WriteValue()
    End Sub

End Class
