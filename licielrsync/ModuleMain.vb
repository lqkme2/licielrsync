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
Imports System.Text.RegularExpressions

Module ModuleMain

    ''--------------------------------------------------------------------
    ''                        G L O B A L E S
    ''--------------------------------------------------------------------

    Friend Fm As FrameMain = Nothing
    Friend Fp As FramePasswordPrompt = Nothing
    Friend CurrentCultureInfo As CultureInfo
    Friend FirstLoad As Boolean = True, Progress As Boolean = False
    Friend RsyncPaths As New Hashtable, FileSizes As New Hashtable
    Friend Processus As Process = Nothing
    Friend ProcessusSuspended As Boolean = False
    Friend GlobalSize As Long = -1, GlobalSizeSent As Long = 0, CurrentSize As Long = -1, CurrentProgress As Integer = 0
    Friend AppAssembly As String = My.Application.Info.AssemblyName
    Friend AppExe As String = String.Format("{0}.exe", AppAssembly)
    Friend AppPath As String = String.Format("{0}\", My.Application.Info.DirectoryPath)
    Friend AppPathUpdate As String = String.Format("{0}update\", AppPath)
    Friend AppIcon As Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath)
    Friend RsyncDirectory As String = String.Format("{0}rsync", AppPath), RsyncPath As String = "", LastLine As String = "", CurrentFile As String = ""
    Friend Delegate Sub CustomMethodInvoker(ByVal var() As Object)

    ''--------------------------------------------------------------------
    ''                        L O C A L E S
    ''--------------------------------------------------------------------

    Private Const AppProjectHome As String = "http://licielrsync.googlecode.com/svn/trunk/"
    Friend AppVersionCheckUrl As String = String.Format("{0}{1}/My%20Project/AssemblyInfo.vb", AppProjectHome, AppAssembly)
    Friend AppVersionCheckUrlUpdater As String = String.Format("{0}{1}-updater/My%20Project/AssemblyInfo.vb", AppProjectHome, AppAssembly)
    Private ReadOnly AppExeUpdater As String = String.Format("{0}-updater.exe", AppAssembly)
    Private Const AppExe7Z As String = "7z.exe"
    Private Const AppExe7ZDll As String = "7z.dll"
    Private Const AppExeSha17Z As String = "20FEA1314DBED552D5FEDEE096E2050369172EE1"
    Private Const AppExeSha17ZDll As String = "344FAF61C3EB76F4A2FB6452E83ED16C9CCE73E0"
    Private ReadOnly AppDownloadUrl As String = String.Format("{0}{1}/redist/{1}-{{0}}.7z", AppProjectHome, AppAssembly)
    Private ReadOnly AppDownloadUrlUpdater As String = String.Format("{0}{1}-updater/bin/Release/{2}", AppProjectHome, AppAssembly, AppExeUpdater)
    Private ReadOnly AppDownloadUrlUpdater7Z920 As String = String.Format("{0}{1}/redist/{2}", AppProjectHome, AppAssembly, AppExe7Z)
    Private ReadOnly AppDownloadUrlUpdater7Z920Dll As String = String.Format("{0}{1}/redist/{2}", AppProjectHome, AppAssembly, AppExe7ZDll)
    Private Const NotEmptyPattern As String = "\S+"
    Private Const ProgressPattern As String = "(\d+)\%.*(\d{2}|\d{1}):(\d{2}|\d{1}):(\d{2}|\d{1})\s*(\(.*\))*$"
    Private Const WinPathPattern As String = "^(([a-zA-Z]):\\(.*)|(\\\\))"

    ''--------------------------------------------------------------------
    '' Main
    ''
    '' Program entry point
    ''--------------------------------------------------------------------

    Friend Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FrameMain)
    End Sub

    Friend Sub Start(ByVal frm As FrameMain)
        Fm = frm
        Fp = New FramePasswordPrompt
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
        Dim dlgResult As DialogResult = Fm.FolderBrowserDialog.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.OK AndAlso Fm.FolderBrowserDialog.SelectedPath <> "" Then Return String.Format("{0}\", Fm.FolderBrowserDialog.SelectedPath)
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
            SendMessage(String.Format(L("msg11"), offVersionStr), L("msg10"), Fm, ToolTipIcon.Info, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If
        If ret = 1 AndAlso LicielMessage.Send(String.Format(L("msg12"), offVersionStr), L("msg10"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, frame) = DialogResult.No Then Exit Sub
        Dim updaterVersion As String = Nothing
        Try
            updaterVersion = Reflection.AssemblyName.GetAssemblyName(String.Format("{0}{1}", AppPathUpdate, AppExeUpdater)).Version.ToString
        Catch
        End Try
        Try
            If Not Directory.Exists(AppPathUpdate) Then Directory.CreateDirectory(AppPathUpdate)
        Catch ex As Exception
            HandleError("::directory", ex.ToString)
            Exit Sub
        End Try
        If Not File.Exists(String.Format("{0}{1}", AppPathUpdate, AppExeUpdater)) OrElse updaterVersion = Nothing OrElse CheckVersion(AppVersionCheckUrlUpdater, updaterVersion)(0) = 1 Then _
            Download(AppDownloadUrlUpdater, String.Format("{0}{1}", AppPathUpdate, AppExeUpdater))
        If EncryptFSha1(String.Format("{0}{1}", AppPathUpdate, AppExe7Z)) <> AppExeSha17Z Then Download(AppDownloadUrlUpdater7Z920, String.Format("{0}{1}", AppPathUpdate, AppExe7Z))
        If EncryptFSha1(String.Format("{0}{1}", AppPathUpdate, AppExe7ZDll)) <> AppExeSha17ZDll Then Download(AppDownloadUrlUpdater7Z920Dll, String.Format("{0}{1}", AppPathUpdate, AppExe7ZDll))
        Dim packageUrl As String = String.Format(AppDownloadUrl, offVersionStr)
        Dim packageName As String = Regex.Match(packageUrl, "^http:.*/([^/]+)$").Groups(1).Value
        If packageName = "" Then HandleError("::update", "packageName is empty")
        Download(packageUrl, String.Format("{0}{1}", AppPathUpdate, packageName))
        ret = SimpleProcessStart(String.Format("{0}{1}", AppPathUpdate, AppExe7Z), String.Format("x -y ""{0}{1}"" -o""{0}""", AppPathUpdate, packageName), True, True)
        If ret <> 0 Then HandleError("::7zip", String.Format("Error in command line : 7z.exe x -y ""{0}{1}"" -o""{0}""", AppPathUpdate, packageName))
        SimpleProcessStart(String.Format("{0}{1}", AppPathUpdate, AppExeUpdater), "--update", True, False)
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
        If driveLetter = "" Then Return path.Replace("\", "/")
        Dim fullPath As String = matchObj.Groups(3).Value.Replace("\", "/")
        Return String.Format("/cygdrive/{0}/{1}", driveLetter, fullPath)
    End Function

    ''--------------------------------------------------------------------
    '' BuildArgument
    ''
    '' Command-line construction
    ''--------------------------------------------------------------------

    Private Function BuildArgument(ByVal dryrun As Boolean)
        Return String.Format("{0}""{1}"" ""{2}""", BuildOptions(dryrun), FormatPath(Fm.TextBoxSrc.Text), FormatPath(Fm.TextBoxDst.Text))
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
                Case Fm.TextBoxLogs.Name
                    With control
                        .Lines = obj(1).ToArray()
                        .SelectionStart = .TextLength
                        .ScrollToCaret()
                    End With
                Case Fm.TextBoxErrors.Name
                    With control
                        .AppendText(obj(1))
                        .SelectionStart = .TextLength
                        .ScrollToCaret()
                    End With
                    If IsWin7 Then
                        If TaskBar Is Nothing Then TaskBar = CType(New CTaskbarList, ITaskbarList4)
                        TaskBar.SetProgressState(CType(Fm.Handle, Integer), Tbpflag.TbpfError)
                    End If
                Case Fm.ButtonExec.Name
                    Dim active As Boolean = obj(1)
                    control.Enabled = active
                    Fm.ButtonTest.Enabled = active
                    Fm.ButtonPause.Enabled = Not active
                    Fm.ButtonStop.Enabled = Not active
                    Fm.TextBoxLogs.ScrollBars = ScrollBars.Vertical
                    Fm.TextBoxErrors.ScrollBars = ScrollBars.Vertical
                    Fm.TextBoxLogs.ScrollToCaret()
                    Fm.TextBoxErrors.ScrollToCaret()
                    If Not active Then
                        Fm.TextBoxLogs.ScrollBars = ScrollBars.None
                        Fm.TextBoxErrors.ScrollBars = ScrollBars.None
                        Fm.ButtonPause.BackColor = SystemColors.Control
                        Fm.ButtonStop.BackColor = SystemColors.Control
                        control.BackColor = SystemColors.Control
                        Fm.ButtonTest.BackColor = SystemColors.Control
                    End If
                Case Fm.ProgressBar.Name
                    Dim percent As Long = obj(1)
                    If percent = 0 Then Exit Sub
                    Fm.ProgressBarText.Text = String.Format("{0}%", Math.Round(percent))
                    control.Value = percent
                    If IsWin7 Then
                        Dim position As Long = obj(2)
                        Dim total As Long = obj(3)
                        If TaskBar Is Nothing Then TaskBar = CType(New CTaskbarList, ITaskbarList4)
                        TaskBar.SetProgressValue(CType(Fm.Handle, Integer), position, total)
                        If position >= total Then TaskBar.SetProgressState(CType(Fm.Handle, Integer), Tbpflag.TbpfNoprogress)
                    End If
            End Select
        Catch ex As Exception
            HandleError("", ex.ToString)
        End Try
    End Sub

    ''--------------------------------------------------------------------
    '' ThreadReadStreams
    ''
    '' Stub function used to read the standard and error streams
    ''--------------------------------------------------------------------

    Private Sub ThreadReadStreams(ByVal arg As Object)
        Dim line As String = ""
        Dim stream As StreamReader = arg(0)
        Try
            Select Case arg(1)
                Case True
                    Dim textBoxLogsLines As New List(Of String), carriageReturn As Boolean = False, progressMatch As Object
                    Do While Not stream.EndOfStream
                        line = stream.ReadLine()
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
                        Fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {Fm.TextBoxLogs, textBoxLogsLines}})
                        If Progress AndAlso CurrentFile <> "" AndAlso progressMatch.Success Then
                            CurrentProgress = CInt(progressMatch.Groups(1).Value)
                            UpdateProgress(CurrentProgress >= 100)
                        End If
                    Loop
                Case False
                    Dim pwdSent As Boolean = False
                    Do While Not stream.EndOfStream
                        Do While stream.Peek >= 0
                            Dim c(0) As Char
                            stream.Read(c, 0, c.Length)
                            line = String.Concat(line, CType(c, String))
                        Loop
                        If Not pwdSent AndAlso Regex.Match(line, "^Password\: $").Success Then
                            Dim srInp As StreamWriter = arg(2)
                            If Fp Is Nothing Then Fp = New FramePasswordPrompt()
                            Fm.Invoke(New MethodInvoker(Sub() Fp.ShowDialog(Fm)))
                            srInp.WriteLine(Fp.Passwd)
                            srInp.Flush()
                            pwdSent = True
                            Fp.Passwd = Nothing
                            Continue Do
                        End If
                        If Not Regex.Match(line, NotEmptyPattern).Success Or (Regex.Match(line, "^Password\: $").Success AndAlso stream.EndOfStream) Then Continue Do
                        line = line.Replace(ControlChars.Lf, ControlChars.CrLf)
                        Fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {Fm.TextBoxErrors, String.Format("{0}", line)}})
                        line = ""
                    Loop
            End Select
        Catch ex As Exception
            HandleError("", ex.ToString)
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
        Fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {Fm.ProgressBar, percent, GlobalSizeSent + sentData, GlobalSize}})
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
        Dim srInp As StreamWriter = Nothing
        Dim thdStd As Threading.Thread = Nothing
        Dim thdErr As Threading.Thread = Nothing
        Dim settingsHideWnd As Boolean
        Dim settingsRedir As Boolean
        Try
            Dim arg As String = BuildArgument(obj(1))
            settingsHideWnd = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("hidewnd")
            settingsRedir = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("redir")
            Progress = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
            Processus = New Process()
            Processus.StartInfo.FileName = If(settingsHideWnd, obj(0), "cmd.exe")
            If Not settingsHideWnd Then arg = String.Format("/C ""{0} {1}"" & PAUSE", obj(0), arg)
            Processus.EnableRaisingEvents = False
            Processus.StartInfo.UseShellExecute = False
            Processus.StartInfo.CreateNoWindow = settingsHideWnd
            Processus.StartInfo.RedirectStandardInput = settingsRedir
            Processus.StartInfo.RedirectStandardOutput = settingsRedir
            Processus.StartInfo.RedirectStandardError = settingsRedir
            If settingsRedir Then
                Processus.StartInfo.StandardOutputEncoding = Encoding.UTF8
                Processus.StartInfo.StandardErrorEncoding = Encoding.UTF8
            End If
            Processus.StartInfo.WindowStyle = If(settingsHideWnd, ProcessWindowStyle.Hidden, ProcessWindowStyle.Normal)
            If arg <> "" Then Processus.StartInfo.Arguments = arg
            CacheSizes()
            Fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {Fm.ButtonExec, False}})
            Processus.Start()
            If settingsRedir Then
                srStd = Processus.StandardOutput
                srErr = Processus.StandardError
                srInp = Processus.StandardInput
                thdStd = New Threading.Thread(AddressOf ThreadReadStreams)
                thdStd.IsBackground = True
                thdStd.Start({srStd, True})
                thdErr = New Threading.Thread(AddressOf ThreadReadStreams)
                thdErr.IsBackground = True
                thdErr.Start({srErr, False, srInp})
            End If
            Processus.WaitForExit()
            If Not thdStd Is Nothing Then
                While thdErr.IsAlive Or thdStd.IsAlive
                    Application.DoEvents()
                End While
            End If
        Catch ex As Exception
            HandleError("", ex.ToString)
        Finally
            If Not Fp Is Nothing AndAlso Not Fp.Passwd Is Nothing Then Fp.Passwd = Nothing
            If Not thdErr Is Nothing Then thdErr.Abort()
            If Not thdStd Is Nothing Then thdStd.Abort()
            If Not srErr Is Nothing Then srErr.Close()
            If Not srStd Is Nothing Then srStd.Close()
            If Not srInp Is Nothing Then srInp.Close()
            If Not Processus Is Nothing AndAlso settingsHideWnd Then Processus.Close()
            Fm.BeginInvoke(New CustomMethodInvoker(AddressOf InvokeChangeControl), New Object() {New Object() {Fm.ButtonExec, True}})
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
        Dim str As String = ""
        Try
            If dryrun Then str = "--dry-run "
            For Each opt As Object In My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")
                Dim key = opt.Key
                If opt.Value Then
                    Select Case key
                        Case "-v"
                            key = String.Format("-{0}", New String("v"c, CInt(My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("-v"))))
                    End Select
                    str = String.Format("{0}{1} ", str, key)
                End If
            Next
            If Regex.Match(My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions"), "\S+").Success Then str = String.Concat(str, String.Format("{0} ", My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("customoptions")))
        Catch ex As Exception
            HandleError("", ex.ToString)
        End Try
        Return str
    End Function

    ''--------------------------------------------------------------------
    '' UpdateStatusBarCommand
    ''
    '' Update the command line shown on status bar
    ''--------------------------------------------------------------------

    Friend Sub UpdateStatusBarCommand(ByVal dryrun As Boolean)
        Fm.StatusBarText.Text = If(Not My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsVar")("showcmd"), "", BuildOptions(dryrun))
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
            SendMessage(L("msg7"), L("msg8"), Fm, ToolTipIcon.Error, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        For Each dir As String In Directory.GetDirectories(RsyncDirectory)
            If Not Regex.Match(Path.GetFileName(dir), "^rsync-\d").Success Or Not File.Exists(String.Format("{0}\bin\rsync.exe", dir)) Then Continue For
            Try
                If Not File.Exists(String.Format("{0}\etc\fstab", dir)) Then
                    Directory.CreateDirectory(String.Format("{0}\etc", dir))
                    Using sw As StreamWriter = File.AppendText(String.Format("{0}\etc\fstab", dir))
                        sw.Write("none /cygdrive cygdrive binary,posix=0,user,noacl 0 0")
                    End Using
                End If
                If Not Directory.Exists(String.Format("{0}\tmp", dir)) Then Directory.CreateDirectory(String.Format("{0}\tmp", dir))
            Catch
            End Try
            rsyncName = Regex.Replace(Path.GetFileName(dir), "\-(?<date>(\d{4}\-\d{2}\-\d{2}|\d{2}\-\d{2}\-\d{4}))(?<hash>\-\w+)", " (${date})")
            If rsyncName = "rsync-3.0.8-ntstreams" Then
                RsyncPaths(String.Format("{0} (unofficial)", rsyncName)) = dir
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
        Fm.ComboRsync.Items.AddRange(items)
        Fm.ComboRsync.SelectedIndex = 0
    End Sub

    ''--------------------------------------------------------------------
    '' ResetProgress
    ''
    '' Reset progress bar datas
    ''--------------------------------------------------------------------

    Friend Sub ResetProgress()
        Fm.ProgressBar.Visible = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
        Fm.ProgressBarText.Visible = My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress")
        If Not My.Settings.Profiles(My.Settings.CurrentProfile)("OptionsSwitch")("--progress") Then Exit Sub
        Fm.ProgressBar.Value = 0
        Fm.ProgressBarText.Text = ""
        GlobalSize = -1
        GlobalSizeSent = 0
        CurrentSize = -1
        CurrentProgress = 0
        CurrentFile = ""
        FileSizes = New Hashtable
    End Sub

    ''--------------------------------------------------------------------
    '' Download
    ''
    '' Used to download version numbers or the -updater.exe
    ''--------------------------------------------------------------------

    Private Function Download(ByVal url As String, Optional ByVal dest As String = "") As String
        Dim httpRequest As HttpWebRequest = Nothing
        Dim httpResponse As HttpWebResponse = Nothing
        Dim isFile As Boolean = dest <> ""
        Dim ret As String = ""
        Try
            httpRequest = WebRequest.Create(url)
            httpRequest.Method = "GET"
            httpRequest.UserAgent = String.Format("licielrsync/{0} (licielrsync.googlecode.com)", Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())
            httpRequest.Accept = "*/*"
            httpRequest.Timeout = 20000
            httpRequest.ReadWriteTimeout = 20000
            httpResponse = httpRequest.GetResponse
            If Not isFile Then
                Using httpResponseReader As New StreamReader(httpResponse.GetResponseStream())
                    ret = httpResponseReader.ReadToEnd()
                End Using
            Else
                Using fileStream As New FileStream(dest, FileMode.Create)
                    Do
                        Dim readBytes(4096) As Byte
                        Dim bytesread As Integer = httpResponse.GetResponseStream.Read(readBytes, 0, 4096)
                        fileStream.Write(readBytes, 0, bytesread)
                        If bytesread = 0 Then Exit Do
                    Loop
                End Using
                ret = dest
            End If
        Catch ex As Exception
            HandleError("::download", ex.ToString)
        Finally
            If Not httpResponse Is Nothing Then httpResponse.Close()
            If Not httpRequest Is Nothing Then httpRequest.Abort()
        End Try
        Return ret
    End Function

    ''--------------------------------------------------------------------
    '' SendMessage
    ''
    '' Choose between a messagebox or tray message
    ''--------------------------------------------------------------------

    Private Sub SendMessage(ByVal text As String, ByVal title As String, Optional frame As Form = Nothing, Optional tipIcon As ToolTipIcon = Nothing, Optional buttons As MessageBoxButtons = Nothing, Optional icon As MessageBoxIcon = Nothing)
        If My.Settings.ShowInTray AndAlso Not frame Is Nothing Then
            LicielMessage.SendTray(frame, text, title, tipIcon, 2000)
        Else
            LicielMessage.Send(text, title, buttons, icon, frame)
        End If
    End Sub

    ''--------------------------------------------------------------------
    '' WriteFile
    ''
    '' Write a text log file to disk
    ''--------------------------------------------------------------------

    Friend Sub WriteFile(ByRef t As String, ByVal fichier As String)
        Try
            Using sw As StreamWriter = File.AppendText(fichier)
                sw.Write(String.Format("{0}{1}", ControlChars.CrLf, t))
            End Using
        Catch
        End Try
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
                errorText = String.Format(L("msg15"), AppPath, AppAssembly)
            Case "::download"
                errorText = String.Format(L("msg14"), AppPath, AppAssembly)
            Case "::update"
                errorText = L("msg9")
            Case "::directory"
                errorText = String.Format(L("msg13"), AppPath, AppAssembly)
            Case "::7zip"
                errorText = L("msg16")
        End Select
        If errorText.Length > 0 Then SendMessage(errorText, L("msg8"), Fm, ToolTipIcon.Error, MessageBoxButtons.OK, MessageBoxIcon.Error)
        errorFullLog = String.Format("[{0} --- {1}] {2}{3}Details : {4}", DateTime.Now, type, errorText, ControlChars.CrLf, ex)
        WriteFile(errorFullLog, String.Format("{0}{1}_errors.log", AppPath, AppAssembly))
        If Debugger.IsAttached Then LicielMessage.Send(errorFullLog, String.Format("{0} error", AppExe), MessageBoxButtons.OK, MessageBoxIcon.Error, Fm)
    End Sub
End Module
