''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' modulesettings
''
'' work around a limitation in visual Studio 2010 with the use of
'' hashtables in my project > settings
''----------------------------------------------------------------------------------------------



Imports System.ComponentModel
Imports System.Globalization

Namespace My
    Partial Friend NotInheritable Class MySettings
        <Configuration.UserScopedSettingAttribute(), _
         DebuggerNonUserCodeAttribute(), _
         Configuration.SettingsSerializeAs(Configuration.SettingsSerializeAs.Binary), _
         Configuration.SettingsManageabilityAttribute(Configuration.SettingsManageability.Roaming)>
        Public Property Profiles() As Hashtable
            Get
                Return CType(Me("Profiles"), Hashtable)
            End Get
            Set(value As Hashtable)
                Me("Profiles") = value
            End Set
        End Property

        <Configuration.UserScopedSettingAttribute(), _
         DebuggerNonUserCodeAttribute(), _
         Configuration.SettingsSerializeAs(Configuration.SettingsSerializeAs.Binary), _
         Configuration.SettingsManageabilityAttribute(Configuration.SettingsManageability.Roaming)>
        Public Property ProfilesList() As List(Of String)
            Get
                Return CType(Me("ProfilesList"), List(Of String))
            End Get
            Set(value As List(Of String))
                Me("ProfilesList") = value
            End Set
        End Property
    End Class
End Namespace

Friend Module ModuleSettings

    ''--------------------------------------------------------------------
    '' LoadProfile
    ''
    '' Profiles load
    ''--------------------------------------------------------------------

    Friend Sub LoadProfile(ByVal profileName As String)
        If profileName = My.Settings.CurrentProfile Then Exit Sub
        My.Settings.CurrentProfile = profileName
        My.Settings.Save()
        InitializeOptions()
        LoadConfig()
    End Sub

    ''--------------------------------------------------------------------
    '' LocalizeControlsText
    ''
    '' Loop to change all controls text
    ''--------------------------------------------------------------------

    Private Sub LocalizeControlsText(ByVal ctrlCol As Control.ControlCollection, ByVal resources As ComponentResourceManager, ByVal cInfo As CultureInfo)
        For Each c As Control In ctrlCol
            resources.ApplyResources(c, c.Name, cInfo)
            If resources.GetString(String.Format("{0}.ToolTip", c.Name)) <> "" Then Fm.ToolTip1.SetToolTip(c, resources.GetString(String.Format("{0}.ToolTip", c.Name), cInfo))
            LocalizeControlsText(c.Controls, resources, cInfo)
        Next c
    End Sub

    ''--------------------------------------------------------------------
    '' LocalizeToolStripsText
    ''
    '' Loop to change all toolstrip menu items text
    ''--------------------------------------------------------------------

    Private Sub LocalizeToolStripsText(ByVal toolStrCol As ToolStripItemCollection, ByVal resources As ComponentResourceManager, ByVal cInfo As CultureInfo)
        For Each t In toolStrCol
            If TypeOf t Is ToolStripSeparator Then Continue For
            resources.ApplyResources(t, t.Name, cInfo)
            LocalizeToolStripsText(t.DropDownItems, resources, cInfo)
        Next t
    End Sub

    ''--------------------------------------------------------------------
    '' ChangeLanguage
    ''
    '' Change language dynamically
    ''--------------------------------------------------------------------

    Friend Sub ChangeLanguage(ByVal lang As String)
        Dim cultureInfo = If(lang = "", Nothing, New CultureInfo(lang, False))
        CurrentCultureInfo = cultureInfo
        Dim s() As Control = New Control() {Fm, Fp}
        For Each _c In s
            Dim resources As ComponentResourceManager = New ComponentResourceManager(_c.GetType)
            resources.ApplyResources(_c, "$this", cultureInfo)
            For Each c In _c.Controls
                resources.ApplyResources(c, c.Name, cultureInfo)
                If resources.GetString(String.Format("{0}.ToolTip", c.Name)) <> "" Then Fm.ToolTip1.SetToolTip(c, resources.GetString(String.Format("{0}.ToolTip", c.Name), cultureInfo))
                LocalizeControlsText(c.Controls, resources, cultureInfo)
                If TypeOf c Is MenuStrip Then
                    LocalizeToolStripsText(c.Items, resources, cultureInfo)
                End If
            Next c
        Next _c
    End Sub

    ''--------------------------------------------------------------------
    '' LoadConfig
    ''
    '' Default options load
    ''--------------------------------------------------------------------

    Friend Sub LoadConfig(Optional ByVal init As Boolean = False)
        Try
            If init Then
                For Each _text In My.Settings.ProfilesList
                    Fm.ComboProfiles.Items.AddRange(New Object() {_text})
                Next
                Fm.ComboProfiles.SelectedIndex = Fm.ComboProfiles.FindStringExact(My.Settings.CurrentProfile)
            End If
            Fm.CbDate.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-t")
            Fm.CbRecurse.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-r")
            Fm.CbVerbose.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-v")
            Fm.CbProgress.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
            Fm.CbPerm.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-p")
            Fm.CbOwner.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-o")
            Fm.CbGroup.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-g")
            Fm.CbDelta.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--no-whole-file")
            Fm.CbWinCompat.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--modify-window=2")
            Fm.CbDelete.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--delete")
            Fm.CbExisting.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-existing")
            Fm.CbNewer.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-u")
            Fm.CbSizeOnly.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--size-only")
            Fm.CbFS.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-x")
            Fm.CbReadable.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-h")
            Fm.CbChecksum.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-c")
            Fm.CbExistingOnly.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--existing")
            Fm.CbIgnoreTimes.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-times")
            Fm.CbPermWin.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--backup-nt-streams --restore-nt-streams")
            Fm.CbEnglish.Checked = My.Settings.Locales = "English"
            Fm.CbFrench.Checked = My.Settings.Locales = "French"
            Fm.CbEnglish.CheckOnClick = Not Fm.CbEnglish.Checked
            Fm.CbFrench.CheckOnClick = Not Fm.CbFrench.Checked
            Fm.ComboVerbose.Enabled = Fm.CbVerbose.Checked
            Fm.ComboVerbose.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("-v")
            Fm.TextBoxSrc.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("srcpath")
            Fm.TextBoxDst.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("dstpath")
            Fm.TextBoxOptions.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions")
            Fm.CbShowCmd.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("showcmd")
            Fm.CbHideWindows.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("hidewnd")
            Fm.CbRedir.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("redir")
            Fm.TrayIconNoticeToolStripMenuItem.Enabled = My.Settings.ShowInTray
            Fm.TrayIconEnabledToolStripMenuItem.Checked = My.Settings.ShowInTray
            Fm.TrayIconNoticeStartToolStripMenuItem.Checked = My.Settings.TrayNoticeStart
            If My.Settings.CloseToTray <> -1 Then Fm.TrayIconCloseToolStripMenuItem.Checked = My.Settings.CloseToTray = 1
            If My.Settings.MinimizeToTray <> -1 Then Fm.TrayIconMinimizeToolStripMenuItem.Checked = My.Settings.MinimizeToTray = 1
        Catch ex As Exception
            HandleError("", ex.ToString)
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' InitializeOptions
    ''
    '' Default options initialization
    ''--------------------------------------------------------------------

    Friend Sub InitializeOptions()
        Try
            If My.Settings.ProfilesList Is Nothing Then
                My.Settings.ProfilesList = New List(Of String)
                My.Settings.ProfilesList.Add(My.Settings.CurrentProfile)
            End If
            If My.Settings.Profiles Is Nothing Then My.Settings.Profiles = New Hashtable()
            If My.Settings.Profiles(My.Settings.CurrentProfile) Is Nothing Then My.Settings.Profiles(My.Settings.CurrentProfile) = New Hashtable()
            If My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar") Is Nothing Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar") = New Hashtable()
            If My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch") Is Nothing Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch") = New Hashtable()
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-r") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-r") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-t") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-t") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("-v") Is String Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("-v") = "1"
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-v") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-v") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--backup-nt-streams --restore-nt-streams") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--backup-nt-streams --restore-nt-streams") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("srcpath") Is String Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("srcpath") = ""
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("dstpath") Is String Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("dstpath") = ""
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("showcmd") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("showcmd") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("hidewnd") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("hidewnd") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("redir") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("redir") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions") Is String Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions") = ""
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-p") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-p") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-o") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-o") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-g") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-g") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--no-whole-file") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--no-whole-file") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--modify-window=2") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--modify-window=2") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--delete") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--delete") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-existing") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-existing") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-u") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-u") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--size-only") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--size-only") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-x") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-x") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-h") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-h") = True
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-c") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-c") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--existing") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--existing") = False
            If Not TypeOf My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-times") Is Boolean Then My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-times") = False
            My.Settings.Save()
        Catch ex As Exception
            HandleError("", ex.ToString)
        End Try
    End Sub
End Module