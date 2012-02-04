''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' modulemain
''
'' primary module
''----------------------------------------------------------------------------------------------



Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Globalization
Imports System.ComponentModel
Imports System.Text.RegularExpressions

Module ModuleMain

    ''--------------------------------------------------------------------
    ''                        G L O B A L E S
    ''--------------------------------------------------------------------

    Friend CurrentCultureInfo As CultureInfo
    Friend FirstLoad As Boolean = True, Progress As Boolean = False
    Friend RsyncPaths As New Hashtable, FileSizes As New Hashtable
    Friend Processus As Process = Nothing
    Friend ProcessusSuspended As Boolean = False
    Friend GlobalSize As Long = -1, GlobalSizeSent As Long = 0, CurrentSize As Long = -1, CurrentProgress As Integer = 0
    Friend AppAssembly As String = My.Application.Info.AssemblyName
    Friend AppExe As String = AppAssembly & ".exe"
    Friend AppPath As String = My.Application.Info.DirectoryPath & "\"
    Friend AppPathUpdate As String = AppPath & "update\"
    Friend AppIcon As Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath)
    Friend RsyncDirectory As String = AppPath & "rsync", RsyncPath As String = "", LastLine As String = "", CurrentFile As String = ""
    Friend Delegate Sub CustomMethodInvoker(ByVal var() As Object)

    ''--------------------------------------------------------------------
    ''                        L O C A L E S
    ''--------------------------------------------------------------------

    Private Const AppProjectHome As String = "http://ad-test-proj.googlecode.com/svn/"
    Friend AppVersionCheckUrl As String = AppProjectHome & AppAssembly & "/My%20Project/AssemblyInfo.vb"
    Friend AppVersionCheckUrlUpdater As String = AppProjectHome & AppAssembly & "-updater/My%20Project/AssemblyInfo.vb"
    Private ReadOnly AppExeUpdater As String = AppAssembly & "-updater.exe"
    Private Const AppExe7Z As String = "7z.exe"
    Private Const AppExe7ZDll As String = "7z.dll"
    Private Const AppExeSha17Z As String = "20FEA1314DBED552D5FEDEE096E2050369172EE1"
    Private Const AppExeSha17ZDll As String = "344FAF61C3EB76F4A2FB6452E83ED16C9CCE73E0"
    Private ReadOnly AppDownloadUrl As String = AppProjectHome & AppAssembly & "/redist/" & AppAssembly & "-{0}.7z"
    Private ReadOnly AppDownloadUrlUpdater As String = AppProjectHome & AppAssembly & "-updater/bin/Release/" & AppExeUpdater
    Private ReadOnly AppDownloadUrlUpdater7Z920 As String = AppProjectHome & AppAssembly & "/redist/" & AppExe7Z
    Private ReadOnly AppDownloadUrlUpdater7Z920Dll As String = AppProjectHome & AppAssembly & "/redist/" & AppExe7ZDll
    Private _fm As FrameMain = Nothing
    Private Const NotEmptyPattern As String = "\S+"
    Private Const ProgressPattern As String = "(\d+)\%.*(\d{2}|\d{1}):(\d{2}|\d{1}):(\d{2}|\d{1})\s*(\(.*\))*$"
    Private Const WinPathPattern As String = "^(([a-zA-Z]):\\(.*)|(\\\\))"

    ''--------------------------------------------------------------------
    '' Main
    ''
    '' Program entry point
    ''--------------------------------------------------------------------

    Friend Sub Main(ByVal frame As Form)
        _fm = frame
        ''
        '' Old configs import
        ''
        If My.Settings.ForceUpdate And Not My.Settings.ShouldReset Then
            My.Settings.Upgrade()
            My.Settings.ForceUpdate = False
            My.Settings.Save()
        End If
        If My.Settings.ShouldReset Then
            My.Settings.ShouldReset = False
            My.Settings.ForceUpdate = False
            My.Settings.Save()
        End If
        ''
        '' Loading config
        ''
        InitializeOptions()
        LoadConfig(True)
        ''
        '' Detect version of rsync present and ready to use
        ''
        InitializeRsyncs()
    End Sub

    ''--------------------------------------------------------------------
    '' L
    ''
    '' Localizations
    ''--------------------------------------------------------------------

    Friend Function L(ByVal resStr As String)
        Return My.Resources.ResourceManager.GetString(resStr, CurrentCultureInfo)
    End Function

    ''--------------------------------------------------------------------
    '' GetDirectory
    ''
    '' Path selection
    ''--------------------------------------------------------------------

    Friend Function GetDirectory(ByVal controlText As String)
        Dim dlgResult As DialogResult = _fm.FolderBrowserDialog.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.OK AndAlso _fm.FolderBrowserDialog.SelectedPath <> "" Then Return _fm.FolderBrowserDialog.SelectedPath & "\"
        Return controlText
    End Function

    ''--------------------------------------------------------------------
    '' LoadUpdates
    ''
    '' Launch the update procedures
    ''--------------------------------------------------------------------

    Friend Sub LoadUpdates(ByVal threadObject As Object)
        Dim frame As FrameMain = threadObject(0)
        Dim updateObject As Object = CheckVersion(AppVersionCheckUrl, Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString)
        Dim ret As Integer = updateObject(0)
        Dim offVersionStr As String = updateObject(1)
        If ret = -1 Then
            HandleError("::update", "")
            Exit Sub
        End If
        If ret = 0 Then
            SendMessage(String.Format(L("msg11"), offVersionStr), L("msg10"), _fm, ToolTipIcon.Info, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If
        If ret = 1 AndAlso LicielMessage.Send(String.Format(L("msg12"), offVersionStr), L("msg10"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, frame) = DialogResult.No Then Exit Sub
        Dim updaterVersion As String = Nothing
        Try
            updaterVersion = Reflection.AssemblyName.GetAssemblyName(AppPathUpdate & AppExeUpdater).Version.ToString
        Catch
        End Try
        Try
            If Not Directory.Exists(AppPathUpdate) Then Directory.CreateDirectory(AppPathUpdate)
        Catch ex As Exception
            HandleError("::directory", ex.ToString)
            Exit Sub
        End Try
        If Not File.Exists(AppPathUpdate & AppExeUpdater) OrElse updaterVersion = Nothing OrElse CheckVersion(AppVersionCheckUrlUpdater, updaterVersion)(0) = 1 Then _
            Download(AppDownloadUrlUpdater, AppPathUpdate & AppExeUpdater)
        If EncryptFSha1(AppPathUpdate & AppExe7Z) <> AppExeSha17Z Then Download(AppDownloadUrlUpdater7Z920, AppPathUpdate & AppExe7Z)
        If EncryptFSha1(AppPathUpdate & AppExe7ZDll) <> AppExeSha17ZDll Then Download(AppDownloadUrlUpdater7Z920Dll, AppPathUpdate & AppExe7ZDll)
        Dim packageUrl As String = String.Format(AppDownloadUrl, offVersionStr)
        Dim packageName As String = Regex.Match(packageUrl, "^http:.*/([^/]+)$").Groups(1).Value
        If packageName = "" Then HandleError("::update", "packageName is empty")
        Download(packageUrl, AppPathUpdate & packageName)
        ret = SimpleProcessStart(AppPathUpdate & AppExe7Z, "x -y """ & AppPathUpdate & packageName & """ -o""" & AppPathUpdate & """", True, True)
        If ret <> 0 Then HandleError("::7zip", "Error in command line : 7z.exe x -y """ & AppPathUpdate & packageName & """ -o""" & AppPathUpdate & """")
        SimpleProcessStart(AppPathUpdate & AppExeUpdater, "--update", True, False)
        Environment.Exit(0)
    End Sub

    ''--------------------------------------------------------------------
    '' EncryptFSha1
    ''
    '' get sha1 identity of a file
    ''--------------------------------------------------------------------

    Private Function EncryptFSha1(ByVal filepath As String) As String
        Try
            If Not File.Exists(filepath) Then Return ""
            Using reader As New FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using md5 As New System.Security.Cryptography.SHA1CryptoServiceProvider
                    Dim hash() As Byte = md5.ComputeHash(reader)
                    Return Replace(BitConverter.ToString(hash), "-", "").ToUpper
                End Using
            End Using
        Catch ex As Exception
            HandleError("::directory", ex.ToString)
        End Try
        Return ""
    End Function

    ''--------------------------------------------------------------------
    '' CheckVersion
    ''
    '' Retrieve and compare the assembly version from the googlecode svn
    ''--------------------------------------------------------------------

    Private Function CheckVersion(ByVal url As String, ByVal curVersion As String) As Object
        Dim response As String = Download(url)
        If response = "" Then Return New Object() {-1, ""}
        ''
        '' Googlecode raw  release version number = \<Assembly\:\sAssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)
        '' Googlecode html release version number = Assembly\:\sAssemblyVersion\((\&quot\;|"")(\d+\.\d+\.\d+\.\d+)(\&quot\;|"")\)
        ''
        Dim offVersionStr As String = Regex.Match(response, "\<Assembly\:\sAssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)").Groups(1).Value
        If offVersionStr = "" Then Return New Object() {-1, ""}
        Dim curVersionStr As String = curVersion
        If curVersionStr = "" Then Return New Object() {-1, ""}
        If offVersionStr <> curVersionStr Then Return New Object() {1, offVersionStr}
        Return New Object() {0, offVersionStr}
        'If response = "" Or Not Regex.Match(AppUpdateUrl, Application.ProductName).Success Then
        'Dim offVersionInt As Integer = CType(Regex.Replace(offVersionStr, "\.", ""), Integer)
        'Dim curVersionInt As Integer = CType(Regex.Replace(String.Format("{0}{1}{2}{3}", curVersion.Major, curVersion.Minor, curVersion.Build, curVersion.Revision), "\.", ""), Integer)
        'If offVersionStr = "" Or curVersionInt < 0 Then Return New Object() {-1, ""}
    End Function

    ''--------------------------------------------------------------------
    '' FormatPath
    ''
    '' Convert Windows to Cygwin paths
    ''--------------------------------------------------------------------

    Private Function FormatPath(ByVal path As String)
        Dim matchObj As Match = Regex.Match(path, WinPathPattern)
        If Not matchObj.Success Then Return path
        Dim driveLetter As String = matchObj.Groups(2).Value
        If driveLetter = "" Then Return path.Replace("\", "/") ' -- unc path \\server\share
        Dim fullPath As String = matchObj.Groups(3).Value.Replace("\", "/")
        Return "/cygdrive/" & driveLetter & "/" & fullPath ' -- windows path c:\directory\
    End Function

    ''--------------------------------------------------------------------
    '' BuildArgument
    ''
    '' Command-line construction
    ''--------------------------------------------------------------------

    Private Function BuildArgument(ByVal dryrun As Boolean)
        Return BuildOptions(dryrun) & """" & FormatPath(_fm.TextBoxSrc.Text) & """ " & """" & FormatPath(_fm.TextBoxDst.Text) & """"
    End Function

    ''--------------------------------------------------------------------
    '' InvokeChangeControl
    ''
    '' Stub function used to update frames by other threads
    ''--------------------------------------------------------------------

    Private Sub InvokeChangeControl(ByVal obj() As Object)
        Try
            Dim control As Object = obj(0)
            Select Case control.Name
                Case _fm.TextBoxLogs.Name
                    control.Lines = obj(1).ToArray()
                    control.SelectionStart = control.TextLength
                    control.ScrollToCaret()
                Case _fm.TextBoxErrors.Name
                    control.AppendText(obj(1))
                    control.SelectionStart = control.TextLength
                    control.ScrollToCaret()
                    If IsWin7 Then
                        If TaskBar Is Nothing Then TaskBar = CType(New CTaskbarList, ITaskbarList4)
                        TaskBar.SetProgressState(CType(_fm.Handle, Integer), Tbpflag.TbpfError)
                    End If
                Case _fm.ButtonExec.Name
                    Dim active As Boolean = obj(1)
                    control.Enabled = active
                    _fm.ButtonTest.Enabled = active
                    _fm.ButtonPause.Enabled = Not active
                    _fm.ButtonStop.Enabled = Not active
                    If Not active Then
                        _fm.ButtonPause.BackColor = SystemColors.Control
                        _fm.ButtonStop.BackColor = SystemColors.Control
                    End If
                Case _fm.ProgressBar.Name
                    _fm.ProgressBarText.Text = Math.Round(obj(1)) & "%"
                    control.Value = obj(1)
                    If IsWin7 Then
                        If TaskBar Is Nothing Then TaskBar = CType(New CTaskbarList, ITaskbarList4)
                        TaskBar.SetProgressValue(CType(_fm.Handle, Integer), CType(obj(2), Long), CType(obj(3), Long))
                        If obj(2) >= obj(3) Then TaskBar.SetProgressState(CType(_fm.Handle, Integer), Tbpflag.TbpfNoprogress)
                    End If
                Case "topmostForm"
                    _fm.Controls.Add(obj(0))
                    'obj(0).Parent = _fm
            End Select
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' ThreadReadStreams
    ''
    '' Stub function used to read the standard and error streams
    ''--------------------------------------------------------------------

    Private Sub ThreadReadStreams(ByVal arg As Object)
        Dim stdLog = arg(1)
        Dim line As String
        Try
            Select Case stdLog
                Case True
                    Dim textBoxLogsLines As New List(Of String), carriageReturn As Boolean = False, progressMatch As Object
                    Do While Not arg(0).EndOfStream
                        line = arg(0).ReadLine()
                        If Not Regex.Match(line, NotEmptyPattern).Success Then Continue Do
                        If Progress AndAlso CurrentFile = "" Then
                            Try
                                If FileSizes.ContainsKey(line) Then
                                    CurrentSize = FileSizes(line)
                                    CurrentFile = line
                                End If
                            Catch
                            End Try
                        End If
                        If Not carriageReturn Then
                            textBoxLogsLines.Add(line)
                        Else
                            textBoxLogsLines(textBoxLogsLines.Count - 1) = line
                        End If
                        progressMatch = Regex.Match(line, ProgressPattern)
                        carriageReturn = progressMatch.Success AndAlso progressMatch.Groups(5).Value = "" 'progressMatch.Success
                        _fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {_fm.TextBoxLogs, textBoxLogsLines}})
                        If Progress AndAlso CurrentFile <> "" AndAlso progressMatch.Success Then
                            CurrentProgress = CInt(progressMatch.Groups(1).Value)
                            UpdateProgress(CurrentProgress >= 100)
                        End If
                    Loop
                Case False
                    Do While Not arg(0).EndOfStream
                        line = arg(0).ReadLine()
                        If Not Regex.Match(line, NotEmptyPattern).Success Then Continue Do
                        _fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {_fm.TextBoxErrors, line & vbCrLf}})
                    Loop
            End Select
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' UpdateProgress
    ''
    '' Update the progress bar
    ''--------------------------------------------------------------------

    Private Sub UpdateProgress(Optional ByVal done As Boolean = False)
        Dim sentData As Long = CurrentSize * (CurrentProgress / 100)
        Dim percent As Long = 100 / (GlobalSize / (GlobalSizeSent + sentData))
        If percent > 100 Then percent = 100
        _fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {_fm.ProgressBar, percent, GlobalSizeSent + sentData, GlobalSize}})
        If done Then
            GlobalSizeSent += CurrentSize
            CurrentFile = ""
            CurrentSize = -1
        End If
    End Sub


    ''--------------------------------------------------------------------
    '' CacheSizes
    ''
    '' Global size calculation for progress bar
    ''--------------------------------------------------------------------

    Private Sub CacheSizes()
        Dim dir As String = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("srcpath")
        If Not Regex.Match(dir, WinPathPattern).Success Then Exit Sub
        Dim length As Long
        For Each file In Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
            length = New FileInfo(file).Length
            FileSizes(file.Replace(dir, "").Replace("\", "/")) = length
            GlobalSize += length
        Next
    End Sub

    ''--------------------------------------------------------------------
    '' ThreadProcessStart
    ''
    '' Start process and read output streams with new threads
    ''--------------------------------------------------------------------

    Friend Sub ThreadProcessStart(ByVal obj As Object)
        Dim srStd As StreamReader = Nothing
        Dim srErr As StreamReader = Nothing
        Dim thdStd As Threading.Thread = Nothing
        Dim thdErr As Threading.Thread = Nothing
        Try
            Dim arg As String = BuildArgument(obj(1))
            Dim settingsHideWnd As Boolean = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("hidewnd")
            Dim settingsRedir As Boolean = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("redir")
            Progress = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
            Processus = New Process()
            Processus.StartInfo.FileName = obj(0)
            Processus.EnableRaisingEvents = False
            Processus.StartInfo.UseShellExecute = False
            Processus.StartInfo.CreateNoWindow = settingsHideWnd
            Processus.StartInfo.RedirectStandardOutput = settingsRedir
            Processus.StartInfo.RedirectStandardError = settingsRedir
            If settingsRedir Then
                Processus.StartInfo.StandardOutputEncoding = Encoding.UTF8
                Processus.StartInfo.StandardErrorEncoding = Encoding.UTF8
            End If
            Processus.StartInfo.WindowStyle = If(settingsHideWnd, ProcessWindowStyle.Hidden, ProcessWindowStyle.Normal)
            If arg <> "" Then Processus.StartInfo.Arguments = arg
            CacheSizes()
            _fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {_fm.ButtonExec, False}})
            Processus.Start()
            If settingsRedir Then
                srStd = Processus.StandardOutput
                srErr = Processus.StandardError
                thdStd = New Threading.Thread(AddressOf ThreadReadStreams)
                thdStd.IsBackground = True
                thdStd.Start({srStd, True})
                thdErr = New Threading.Thread(AddressOf ThreadReadStreams)
                thdErr.IsBackground = True
                thdErr.Start({srErr, False})
            End If
            Processus.WaitForExit()
            If Not thdStd Is Nothing Then
                While thdErr.IsAlive Or thdStd.IsAlive
                    Application.DoEvents()
                End While
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            If Not thdErr Is Nothing Then thdErr.Abort()
            If Not thdStd Is Nothing Then thdStd.Abort()
            If Not srErr Is Nothing Then srErr.Close()
            If Not srStd Is Nothing Then srStd.Close()
            If Not Processus Is Nothing Then Processus.Close()
            _fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {_fm.ButtonExec, True}})
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' SimpleProcessStart
    ''
    '' Start updater process
    ''--------------------------------------------------------------------

    Private Function SimpleProcessStart(ByVal f As String, Optional ByVal arg As String = "", Optional ByVal hideWnd As Boolean = False, Optional ByVal wait As Boolean = False) As Integer
        Dim ret As Integer = 101
        Using objProcess As New Process()
            Try
                objProcess.StartInfo.FileName = f
                objProcess.StartInfo.UseShellExecute = True
                objProcess.StartInfo.CreateNoWindow = hideWnd
                objProcess.StartInfo.WindowStyle = If(Not hideWnd, ProcessWindowStyle.Normal, ProcessWindowStyle.Hidden)
                If arg <> "" Then objProcess.StartInfo.Arguments = arg
                objProcess.Start()
                If wait Then
                    objProcess.WaitForExit()
                    ret = objProcess.ExitCode()
                End If
            Catch ex As Exception
                HandleError("::process", ex.ToString)
            End Try
        End Using
        Return ret
    End Function

    ''--------------------------------------------------------------------
    '' BuildOptions
    ''
    '' Command-line options
    ''--------------------------------------------------------------------

    Private Function BuildOptions(ByVal dryrun As Boolean)
        Try
            Dim str As String = ""
            If dryrun Then str = "--dry-run "
            For Each opt As Object In My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")
                Dim key = opt.Key
                If opt.Value Then
                    Select Case key
                        Case "-v"
                            key = "-" & New String("v"c, CInt(My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("-v")))
                    End Select
                    str = str & key & " "
                End If
            Next
            If Regex.Match(My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions"), "\S+").Success Then str = String.Concat(str, My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions") & " ")
            Return str
        Catch ex As Exception
            Return MsgBox(ex.ToString)
        End Try
    End Function

    ''--------------------------------------------------------------------
    '' UpdateStatusBarCommand
    ''
    '' Update the command line shown on status bar
    ''--------------------------------------------------------------------

    Friend Sub UpdateStatusBarCommand(ByVal dryrun As Boolean)
        _fm.StatusBarText.Text = If(Not My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("showcmd"), "", BuildOptions(dryrun))
    End Sub

    ''--------------------------------------------------------------------
    '' InitializeRsyncs
    ''
    '' Detect rsync that are presents and setup cygwin files
    ''--------------------------------------------------------------------

    Private Sub InitializeRsyncs()
        Dim items(100) As String
        Dim i As Integer = 0
        Dim rsyncName As String
        If Not Directory.Exists(RsyncDirectory) Then
            SendMessage(L("msg7"), L("msg8"), _fm, ToolTipIcon.Error, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        For Each dir As String In Directory.GetDirectories(RsyncDirectory)
            If Not Regex.Match(Path.GetFileName(dir), "^rsync-\d").Success Or Not File.Exists(dir & "\bin\rsync.exe") Then Continue For
            Try
                If Not File.Exists(dir & "\etc\fstab") Then
                    Directory.CreateDirectory(dir & "\etc")
                    Using sw As StreamWriter = File.AppendText(dir & "\etc\fstab")
                        sw.Write("none /cygdrive cygdrive binary,posix=0,user,noacl 0 0")
                    End Using
                End If
                If Not Directory.Exists(dir & "\tmp") Then Directory.CreateDirectory(dir & "\tmp")
            Catch
            End Try
            rsyncName = Regex.Replace(Path.GetFileName(dir), "\-(?<date>(\d{4}\-\d{2}\-\d{2}|\d{2}\-\d{2}\-\d{4}))(?<hash>\-\w+)", " (${date})")
            If rsyncName = "rsync-3.0.8-ntstreams" Then
                RsyncPaths(rsyncName & " (unofficial)") = dir
                Continue For
            End If
            items(i) = rsyncName
            RsyncPaths(items(i)) = dir
            i += 1
        Next
        If i = 0 Then Exit Sub
        If RsyncPaths.ContainsKey("rsync-3.0.8-ntstreams (unofficial)") Then
            items(i) = "rsync-3.0.8-ntstreams (unofficial)"
            i += 1
        End If
        Array.Resize(items, i)
        _fm.ComboRsync.Items.AddRange(items)
        _fm.ComboRsync.SelectedIndex = 0
    End Sub

    ''--------------------------------------------------------------------
    '' InitializeOptions
    ''
    '' Default options initialization
    ''--------------------------------------------------------------------

    Private Sub InitializeOptions()
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
            MsgBox(ex.ToString)
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' LoadConfig
    ''
    '' Default options load
    ''--------------------------------------------------------------------

    Private Sub LoadConfig(Optional ByVal init As Boolean = False)
        Try
            If init Then
                For Each _text In My.Settings.ProfilesList
                    _fm.ComboProfiles.Items.AddRange(New Object() {_text})
                Next
                _fm.ComboProfiles.SelectedIndex = _fm.ComboProfiles.FindStringExact(My.Settings.CurrentProfile)
            End If
            _fm.CbDate.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-t")
            _fm.CbRecurse.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-r")
            _fm.CbVerbose.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-v")
            _fm.CbProgress.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
            _fm.CbPerm.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-p")
            _fm.CbOwner.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-o")
            _fm.CbGroup.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-g")
            _fm.CbDelta.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--no-whole-file")
            _fm.CbWinCompat.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--modify-window=2")
            _fm.CbDelete.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--delete")
            _fm.CbExisting.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-existing")
            _fm.CbNewer.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-u")
            _fm.CbSizeOnly.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--size-only")
            _fm.CbFS.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-x")
            _fm.CbReadable.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-h")
            _fm.CbChecksum.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("-c")
            _fm.CbExistingOnly.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--existing")
            _fm.CbIgnoreTimes.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--ignore-times")
            _fm.CbPermWin.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--backup-nt-streams --restore-nt-streams")
            _fm.CbEnglish.Checked = My.Settings.Locales = "English"
            _fm.CbFrench.Checked = My.Settings.Locales = "French"
            _fm.CbEnglish.CheckOnClick = Not _fm.CbEnglish.Checked
            _fm.CbFrench.CheckOnClick = Not _fm.CbFrench.Checked
            _fm.ComboVerbose.Enabled = _fm.CbVerbose.Checked
            _fm.ComboVerbose.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("-v")
            _fm.TextBoxSrc.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("srcpath")
            _fm.TextBoxDst.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("dstpath")
            _fm.TextBoxOptions.Text = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions")
            _fm.CbShowCmd.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("showcmd")
            _fm.CbHideWindows.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("hidewnd")
            _fm.CbRedir.Checked = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("redir")
            _fm.TrayIconNoticeToolStripMenuItem.Enabled = My.Settings.ShowInTray
            _fm.TrayIconEnabledToolStripMenuItem.Checked = My.Settings.ShowInTray
            _fm.TrayIconNoticeStartToolStripMenuItem.Checked = My.Settings.TrayNoticeStart
            If My.Settings.CloseToTray <> -1 Then _fm.TrayIconCloseToolStripMenuItem.Checked = My.Settings.CloseToTray = 1
            If My.Settings.MinimizeToTray <> -1 Then _fm.TrayIconMinimizeToolStripMenuItem.Checked = My.Settings.MinimizeToTray = 1
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' LocalizeControlsText
    ''
    '' Loop to change all controls text
    ''--------------------------------------------------------------------

    Private Sub LocalizeControlsText(ByVal ctrlCol As Control.ControlCollection, ByVal resources As ComponentResourceManager, ByVal cInfo As CultureInfo)
        For Each c As Control In ctrlCol
            resources.ApplyResources(c, c.Name, cInfo)
            If resources.GetString(c.Name & ".ToolTip") <> "" Then _fm.ToolTip1.SetToolTip(c, resources.GetString(c.Name & ".ToolTip", cInfo))
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(_fm.GetType)
        resources.ApplyResources(_fm, "$this", cultureInfo)
        For Each c In _fm.Controls
            resources.ApplyResources(c, c.Name, cultureInfo)
            If resources.GetString(c.Name & ".ToolTip") <> "" Then _fm.ToolTip1.SetToolTip(c, resources.GetString(c.Name & ".ToolTip", cultureInfo))
            LocalizeControlsText(c.Controls, resources, cultureInfo)
            If TypeOf c Is MenuStrip Then
                LocalizeToolStripsText(c.Items, resources, cultureInfo)
            End If
        Next c
    End Sub

    ''--------------------------------------------------------------------
    '' ResetProgress
    ''
    '' Reset progress bar datas
    ''--------------------------------------------------------------------

    Friend Sub ResetProgress()
        _fm.ProgressBar.Visible = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
        _fm.ProgressBarText.Visible = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
        If Not My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress") Then Exit Sub
        _fm.ProgressBar.Value = 0
        _fm.ProgressBarText.Text = "0%"
        GlobalSize = -1
        GlobalSizeSent = 0
        CurrentSize = -1
        CurrentProgress = 0
        CurrentFile = ""
        FileSizes = New Hashtable
    End Sub

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
    '' WriteFile
    ''
    '' Write a text file to disk
    ''--------------------------------------------------------------------

    Private Sub WriteFile(ByRef t As String, ByVal fichier As String)
        Dim sw As StreamWriter = Nothing
        Try
            sw = File.AppendText(fichier)
            sw.Write(ControlChars.CrLf & t)
        Catch
        Finally
            If Not sw Is Nothing Then sw.Close()
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' Download
    ''
    '' Used to download version numbers or the -updater.exe
    ''--------------------------------------------------------------------

    Private Function Download(ByVal url As String, Optional ByVal dest As String = "") As String
        Dim httpRequest As HttpWebRequest = Nothing
        Dim httpResponse As HttpWebResponse = Nothing
        Dim httpResponseReader As StreamReader = Nothing
        Dim fileStream As FileStream = Nothing
        Dim isFile As Boolean = dest <> ""
        Dim ret As String = ""
        Try
            httpRequest = WebRequest.Create(url)
            httpRequest.Method = "GET"
            httpRequest.UserAgent = "LicielRsync/" & Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() & " (licielrsync.googlecode.com)"
            httpRequest.Accept = "*/*"
            httpRequest.Timeout = 20000
            httpRequest.ReadWriteTimeout = 20000
            httpResponse = httpRequest.GetResponse
            If Not isFile Then
                httpResponseReader = New StreamReader(httpResponse.GetResponseStream())
                ret = httpResponseReader.ReadToEnd()
            Else
                fileStream = New FileStream(dest, FileMode.Create)
                Do
                    Dim readBytes(4096) As Byte
                    Dim bytesread As Integer = httpResponse.GetResponseStream.Read(readBytes, 0, 4096)
                    fileStream.Write(readBytes, 0, bytesread)
                    If bytesread = 0 Then Exit Do
                Loop
                ret = dest
            End If
        Catch ex As Exception
            HandleError("::download", ex.ToString)
        Finally
            If Not fileStream Is Nothing Then fileStream.Close()
            If Not httpResponseReader Is Nothing Then httpResponseReader.Close()
            If Not httpResponse Is Nothing Then httpResponse.Close()
            If Not httpRequest Is Nothing Then httpRequest.Abort()
        End Try
        Return ret
    End Function

    ''--------------------------------------------------------------------
    '' SendMessage
    ''
    '' Used to display message in a messagebox or tray message
    ''--------------------------------------------------------------------

    Private Sub SendMessage(ByVal text As String, ByVal title As String, Optional frame As Form = Nothing, Optional tipIcon As ToolTipIcon = Nothing, Optional buttons As MessageBoxButtons = Nothing, Optional icon As MessageBoxIcon = Nothing)
        If My.Settings.ShowInTray AndAlso Not frame Is Nothing Then
            LicielMessage.SendTray(frame, text, title, tipIcon, 2000)
        Else
            LicielMessage.Send(text, title, buttons, icon, frame)
        End If
    End Sub

    ''--------------------------------------------------------------------
    '' HandleError
    ''
    '' Manage handled and unhandled errors
    ''--------------------------------------------------------------------

    Friend Sub HandleError(ByVal type As String, ByVal ex As String)
        Dim errorText As String = "", errorFullLog As String
        Select Case type
            Case "::process"
                errorText = String.Format(L("msg15"), AppPath & AppAssembly)
            Case "::download"
                errorText = String.Format(L("msg14"), AppPath & AppAssembly)
            Case "::update"
                errorText = L("msg9")
            Case "::directory"
                errorText = String.Format(L("msg13"), AppPath & AppAssembly)
            Case "::7zip"
                errorText = L("msg16")
        End Select
        If errorText.Length > 0 Then SendMessage(errorText, L("msg8"), _fm, ToolTipIcon.Error, MessageBoxButtons.OK, MessageBoxIcon.Error)
        errorFullLog = "[" & DateTime.Now & " --- " & type & "] " & errorText & ControlChars.CrLf & "Details : " & ex
        WriteFile(errorFullLog, AppPath & AppAssembly & "_errors.log")
        If Debugger.IsAttached Then LicielMessage.Send(errorFullLog, AppExe & " error", MessageBoxButtons.OK, MessageBoxIcon.Error, _fm)
    End Sub

End Module
