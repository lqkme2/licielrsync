﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrameMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrameMain))
        Me.ComboProfiles = New System.Windows.Forms.ComboBox()
        Me.ButtonAdd = New System.Windows.Forms.Button()
        Me.ButtonDel = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ButtonStop = New System.Windows.Forms.Button()
        Me.CbRedir = New System.Windows.Forms.CheckBox()
        Me.CbHideWindows = New System.Windows.Forms.CheckBox()
        Me.ButtonPause = New System.Windows.Forms.Button()
        Me.ButtonTest = New System.Windows.Forms.Button()
        Me.ButtonExec = New System.Windows.Forms.Button()
        Me.ButtonSrcOpen = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.ComboVerbose = New System.Windows.Forms.ComboBox()
        Me.CbProgress = New System.Windows.Forms.CheckBox()
        Me.CbShowCmd = New System.Windows.Forms.CheckBox()
        Me.CbReadable = New System.Windows.Forms.CheckBox()
        Me.CbVerbose = New System.Windows.Forms.CheckBox()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.CbChecksum = New System.Windows.Forms.CheckBox()
        Me.CbDelta = New System.Windows.Forms.CheckBox()
        Me.CbExistingOnly = New System.Windows.Forms.CheckBox()
        Me.CbSizeOnly = New System.Windows.Forms.CheckBox()
        Me.CbNewer = New System.Windows.Forms.CheckBox()
        Me.CbIgnoreTimes = New System.Windows.Forms.CheckBox()
        Me.CbExisting = New System.Windows.Forms.CheckBox()
        Me.CbDelete = New System.Windows.Forms.CheckBox()
        Me.CbRecurse = New System.Windows.Forms.CheckBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.CbPerm = New System.Windows.Forms.CheckBox()
        Me.CbOwner = New System.Windows.Forms.CheckBox()
        Me.CbGroup = New System.Windows.Forms.CheckBox()
        Me.CbDate = New System.Windows.Forms.CheckBox()
        Me.TextBoxOptions = New System.Windows.Forms.TextBox()
        Me.TextBoxDst = New System.Windows.Forms.TextBox()
        Me.ButtonDstOpen = New System.Windows.Forms.Button()
        Me.TextBoxSrc = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.CbFS = New System.Windows.Forms.CheckBox()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.CbWinCompat = New System.Windows.Forms.CheckBox()
        Me.CbPermWin = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.TextBoxLogs = New System.Windows.Forms.TextBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.TextBoxErrors = New System.Windows.Forms.TextBox()
        Me.StatusBar = New System.Windows.Forms.StatusStrip()
        Me.StatusBarText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ProgressBarText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ProgressBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.StatusBarText1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.ComboRsync = New System.Windows.Forms.ComboBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.Menu1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.LanguageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CbEnglish = New System.Windows.Forms.ToolStripMenuItem()
        Me.CbFrench = New System.Windows.Forms.ToolStripMenuItem()
        Me.Menu2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.StatusBar.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ComboProfiles
        '
        resources.ApplyResources(Me.ComboProfiles, "ComboProfiles")
        Me.ComboProfiles.FormattingEnabled = True
        Me.ComboProfiles.MinimumSize = New System.Drawing.Size(50, 0)
        Me.ComboProfiles.Name = "ComboProfiles"
        '
        'ButtonAdd
        '
        resources.ApplyResources(Me.ButtonAdd, "ButtonAdd")
        Me.ButtonAdd.Name = "ButtonAdd"
        Me.ToolTip1.SetToolTip(Me.ButtonAdd, resources.GetString("ButtonAdd.ToolTip"))
        Me.ButtonAdd.UseVisualStyleBackColor = True
        '
        'ButtonDel
        '
        resources.ApplyResources(Me.ButtonDel, "ButtonDel")
        Me.ButtonDel.Name = "ButtonDel"
        Me.ToolTip1.SetToolTip(Me.ButtonDel, resources.GetString("ButtonDel.ToolTip"))
        Me.ButtonDel.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.ComboProfiles)
        Me.GroupBox1.Controls.Add(Me.ButtonDel)
        Me.GroupBox1.Controls.Add(Me.ButtonAdd)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'GroupBox2
        '
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Controls.Add(Me.ButtonStop)
        Me.GroupBox2.Controls.Add(Me.CbRedir)
        Me.GroupBox2.Controls.Add(Me.CbHideWindows)
        Me.GroupBox2.Controls.Add(Me.ButtonPause)
        Me.GroupBox2.Controls.Add(Me.ButtonTest)
        Me.GroupBox2.Controls.Add(Me.ButtonExec)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'ButtonStop
        '
        resources.ApplyResources(Me.ButtonStop, "ButtonStop")
        Me.ButtonStop.Name = "ButtonStop"
        Me.ToolTip1.SetToolTip(Me.ButtonStop, resources.GetString("ButtonStop.ToolTip"))
        Me.ButtonStop.UseVisualStyleBackColor = True
        '
        'CbRedir
        '
        resources.ApplyResources(Me.CbRedir, "CbRedir")
        Me.CbRedir.Name = "CbRedir"
        Me.CbRedir.Tag = ""
        Me.ToolTip1.SetToolTip(Me.CbRedir, resources.GetString("CbRedir.ToolTip"))
        Me.CbRedir.UseVisualStyleBackColor = True
        '
        'CbHideWindows
        '
        resources.ApplyResources(Me.CbHideWindows, "CbHideWindows")
        Me.CbHideWindows.Name = "CbHideWindows"
        Me.CbHideWindows.Tag = ""
        Me.ToolTip1.SetToolTip(Me.CbHideWindows, resources.GetString("CbHideWindows.ToolTip"))
        Me.CbHideWindows.UseVisualStyleBackColor = True
        '
        'ButtonPause
        '
        resources.ApplyResources(Me.ButtonPause, "ButtonPause")
        Me.ButtonPause.Name = "ButtonPause"
        Me.ToolTip1.SetToolTip(Me.ButtonPause, resources.GetString("ButtonPause.ToolTip"))
        Me.ButtonPause.UseVisualStyleBackColor = True
        '
        'ButtonTest
        '
        resources.ApplyResources(Me.ButtonTest, "ButtonTest")
        Me.ButtonTest.Name = "ButtonTest"
        Me.ToolTip1.SetToolTip(Me.ButtonTest, resources.GetString("ButtonTest.ToolTip"))
        Me.ButtonTest.UseVisualStyleBackColor = True
        '
        'ButtonExec
        '
        resources.ApplyResources(Me.ButtonExec, "ButtonExec")
        Me.ButtonExec.Name = "ButtonExec"
        Me.ToolTip1.SetToolTip(Me.ButtonExec, resources.GetString("ButtonExec.ToolTip"))
        Me.ButtonExec.UseVisualStyleBackColor = True
        '
        'ButtonSrcOpen
        '
        resources.ApplyResources(Me.ButtonSrcOpen, "ButtonSrcOpen")
        Me.ButtonSrcOpen.Name = "ButtonSrcOpen"
        Me.ButtonSrcOpen.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PictureBox1.Image = Global.LicielRsync.My.Resources.Resources.newrsynclogo
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'TabControl1
        '
        resources.ApplyResources(Me.TabControl1, "TabControl1")
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.SystemColors.Window
        Me.TabPage1.Controls.Add(Me.GroupBox5)
        Me.TabPage1.Controls.Add(Me.GroupBox9)
        Me.TabPage1.Controls.Add(Me.GroupBox4)
        Me.TabPage1.Controls.Add(Me.TextBoxOptions)
        Me.TabPage1.Controls.Add(Me.TextBoxDst)
        Me.TabPage1.Controls.Add(Me.ButtonDstOpen)
        Me.TabPage1.Controls.Add(Me.ButtonSrcOpen)
        Me.TabPage1.Controls.Add(Me.TextBoxSrc)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.Label1)
        resources.ApplyResources(Me.TabPage1, "TabPage1")
        Me.TabPage1.Name = "TabPage1"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.ComboVerbose)
        Me.GroupBox5.Controls.Add(Me.CbProgress)
        Me.GroupBox5.Controls.Add(Me.CbShowCmd)
        Me.GroupBox5.Controls.Add(Me.CbReadable)
        Me.GroupBox5.Controls.Add(Me.CbVerbose)
        resources.ApplyResources(Me.GroupBox5, "GroupBox5")
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.TabStop = False
        '
        'ComboVerbose
        '
        resources.ApplyResources(Me.ComboVerbose, "ComboVerbose")
        Me.ComboVerbose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboVerbose.FormattingEnabled = True
        Me.ComboVerbose.Items.AddRange(New Object() {resources.GetString("ComboVerbose.Items"), resources.GetString("ComboVerbose.Items1"), resources.GetString("ComboVerbose.Items2"), resources.GetString("ComboVerbose.Items3"), resources.GetString("ComboVerbose.Items4")})
        Me.ComboVerbose.Name = "ComboVerbose"
        Me.ComboVerbose.Tag = "-v"
        '
        'CbProgress
        '
        resources.ApplyResources(Me.CbProgress, "CbProgress")
        Me.CbProgress.Name = "CbProgress"
        Me.CbProgress.Tag = "--progress"
        Me.ToolTip1.SetToolTip(Me.CbProgress, resources.GetString("CbProgress.ToolTip"))
        Me.CbProgress.UseVisualStyleBackColor = True
        '
        'CbShowCmd
        '
        resources.ApplyResources(Me.CbShowCmd, "CbShowCmd")
        Me.CbShowCmd.Name = "CbShowCmd"
        Me.CbShowCmd.Tag = ""
        Me.ToolTip1.SetToolTip(Me.CbShowCmd, resources.GetString("CbShowCmd.ToolTip"))
        Me.CbShowCmd.UseVisualStyleBackColor = True
        '
        'CbReadable
        '
        resources.ApplyResources(Me.CbReadable, "CbReadable")
        Me.CbReadable.Name = "CbReadable"
        Me.CbReadable.Tag = "-h"
        Me.ToolTip1.SetToolTip(Me.CbReadable, resources.GetString("CbReadable.ToolTip"))
        Me.CbReadable.UseVisualStyleBackColor = True
        '
        'CbVerbose
        '
        resources.ApplyResources(Me.CbVerbose, "CbVerbose")
        Me.CbVerbose.Name = "CbVerbose"
        Me.CbVerbose.Tag = "-v"
        Me.ToolTip1.SetToolTip(Me.CbVerbose, resources.GetString("CbVerbose.ToolTip"))
        Me.CbVerbose.UseVisualStyleBackColor = True
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.CbChecksum)
        Me.GroupBox9.Controls.Add(Me.CbDelta)
        Me.GroupBox9.Controls.Add(Me.CbExistingOnly)
        Me.GroupBox9.Controls.Add(Me.CbSizeOnly)
        Me.GroupBox9.Controls.Add(Me.CbNewer)
        Me.GroupBox9.Controls.Add(Me.CbIgnoreTimes)
        Me.GroupBox9.Controls.Add(Me.CbExisting)
        Me.GroupBox9.Controls.Add(Me.CbDelete)
        Me.GroupBox9.Controls.Add(Me.CbRecurse)
        resources.ApplyResources(Me.GroupBox9, "GroupBox9")
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.TabStop = False
        '
        'CbChecksum
        '
        resources.ApplyResources(Me.CbChecksum, "CbChecksum")
        Me.CbChecksum.Name = "CbChecksum"
        Me.CbChecksum.Tag = "-c"
        Me.ToolTip1.SetToolTip(Me.CbChecksum, resources.GetString("CbChecksum.ToolTip"))
        Me.CbChecksum.UseVisualStyleBackColor = True
        '
        'CbDelta
        '
        resources.ApplyResources(Me.CbDelta, "CbDelta")
        Me.CbDelta.Name = "CbDelta"
        Me.CbDelta.Tag = "--no-whole-file"
        Me.ToolTip1.SetToolTip(Me.CbDelta, resources.GetString("CbDelta.ToolTip"))
        Me.CbDelta.UseVisualStyleBackColor = True
        '
        'CbExistingOnly
        '
        resources.ApplyResources(Me.CbExistingOnly, "CbExistingOnly")
        Me.CbExistingOnly.Name = "CbExistingOnly"
        Me.CbExistingOnly.Tag = "--existing"
        Me.ToolTip1.SetToolTip(Me.CbExistingOnly, resources.GetString("CbExistingOnly.ToolTip"))
        Me.CbExistingOnly.UseVisualStyleBackColor = True
        '
        'CbSizeOnly
        '
        resources.ApplyResources(Me.CbSizeOnly, "CbSizeOnly")
        Me.CbSizeOnly.Name = "CbSizeOnly"
        Me.CbSizeOnly.Tag = "--size-only"
        Me.ToolTip1.SetToolTip(Me.CbSizeOnly, resources.GetString("CbSizeOnly.ToolTip"))
        Me.CbSizeOnly.UseVisualStyleBackColor = True
        '
        'CbNewer
        '
        resources.ApplyResources(Me.CbNewer, "CbNewer")
        Me.CbNewer.Name = "CbNewer"
        Me.CbNewer.Tag = "-u"
        Me.ToolTip1.SetToolTip(Me.CbNewer, resources.GetString("CbNewer.ToolTip"))
        Me.CbNewer.UseVisualStyleBackColor = True
        '
        'CbIgnoreTimes
        '
        resources.ApplyResources(Me.CbIgnoreTimes, "CbIgnoreTimes")
        Me.CbIgnoreTimes.Name = "CbIgnoreTimes"
        Me.CbIgnoreTimes.Tag = "--ignore-times"
        Me.ToolTip1.SetToolTip(Me.CbIgnoreTimes, resources.GetString("CbIgnoreTimes.ToolTip"))
        Me.CbIgnoreTimes.UseVisualStyleBackColor = True
        '
        'CbExisting
        '
        resources.ApplyResources(Me.CbExisting, "CbExisting")
        Me.CbExisting.Name = "CbExisting"
        Me.CbExisting.Tag = "--ignore-existing"
        Me.ToolTip1.SetToolTip(Me.CbExisting, resources.GetString("CbExisting.ToolTip"))
        Me.CbExisting.UseVisualStyleBackColor = True
        '
        'CbDelete
        '
        resources.ApplyResources(Me.CbDelete, "CbDelete")
        Me.CbDelete.Name = "CbDelete"
        Me.CbDelete.Tag = "--delete"
        Me.ToolTip1.SetToolTip(Me.CbDelete, resources.GetString("CbDelete.ToolTip"))
        Me.CbDelete.UseVisualStyleBackColor = True
        '
        'CbRecurse
        '
        resources.ApplyResources(Me.CbRecurse, "CbRecurse")
        Me.CbRecurse.Name = "CbRecurse"
        Me.CbRecurse.Tag = "-r"
        Me.ToolTip1.SetToolTip(Me.CbRecurse, resources.GetString("CbRecurse.ToolTip"))
        Me.CbRecurse.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.CbPerm)
        Me.GroupBox4.Controls.Add(Me.CbOwner)
        Me.GroupBox4.Controls.Add(Me.CbGroup)
        Me.GroupBox4.Controls.Add(Me.CbDate)
        resources.ApplyResources(Me.GroupBox4, "GroupBox4")
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.TabStop = False
        '
        'CbPerm
        '
        resources.ApplyResources(Me.CbPerm, "CbPerm")
        Me.CbPerm.Name = "CbPerm"
        Me.CbPerm.Tag = "-p"
        Me.ToolTip1.SetToolTip(Me.CbPerm, resources.GetString("CbPerm.ToolTip"))
        Me.CbPerm.UseVisualStyleBackColor = True
        '
        'CbOwner
        '
        resources.ApplyResources(Me.CbOwner, "CbOwner")
        Me.CbOwner.Name = "CbOwner"
        Me.CbOwner.Tag = "-o"
        Me.ToolTip1.SetToolTip(Me.CbOwner, resources.GetString("CbOwner.ToolTip"))
        Me.CbOwner.UseVisualStyleBackColor = True
        '
        'CbGroup
        '
        resources.ApplyResources(Me.CbGroup, "CbGroup")
        Me.CbGroup.Name = "CbGroup"
        Me.CbGroup.Tag = "-g"
        Me.ToolTip1.SetToolTip(Me.CbGroup, resources.GetString("CbGroup.ToolTip"))
        Me.CbGroup.UseVisualStyleBackColor = True
        '
        'CbDate
        '
        resources.ApplyResources(Me.CbDate, "CbDate")
        Me.CbDate.Name = "CbDate"
        Me.CbDate.Tag = "-t"
        Me.ToolTip1.SetToolTip(Me.CbDate, resources.GetString("CbDate.ToolTip"))
        Me.CbDate.UseVisualStyleBackColor = True
        '
        'TextBoxOptions
        '
        resources.ApplyResources(Me.TextBoxOptions, "TextBoxOptions")
        Me.TextBoxOptions.Name = "TextBoxOptions"
        Me.ToolTip1.SetToolTip(Me.TextBoxOptions, resources.GetString("TextBoxOptions.ToolTip"))
        '
        'TextBoxDst
        '
        resources.ApplyResources(Me.TextBoxDst, "TextBoxDst")
        Me.TextBoxDst.Name = "TextBoxDst"
        Me.ToolTip1.SetToolTip(Me.TextBoxDst, resources.GetString("TextBoxDst.ToolTip"))
        '
        'ButtonDstOpen
        '
        resources.ApplyResources(Me.ButtonDstOpen, "ButtonDstOpen")
        Me.ButtonDstOpen.Name = "ButtonDstOpen"
        Me.ButtonDstOpen.UseVisualStyleBackColor = True
        '
        'TextBoxSrc
        '
        resources.ApplyResources(Me.TextBoxSrc, "TextBoxSrc")
        Me.TextBoxSrc.Name = "TextBoxSrc"
        Me.ToolTip1.SetToolTip(Me.TextBoxSrc, resources.GetString("TextBoxSrc.ToolTip"))
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        Me.ToolTip1.SetToolTip(Me.Label3, resources.GetString("Label3.ToolTip"))
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        Me.ToolTip1.SetToolTip(Me.Label2, resources.GetString("Label2.ToolTip"))
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        Me.ToolTip1.SetToolTip(Me.Label1, resources.GetString("Label1.ToolTip"))
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.SystemColors.Window
        Me.TabPage2.Controls.Add(Me.CbFS)
        Me.TabPage2.Controls.Add(Me.GroupBox8)
        resources.ApplyResources(Me.TabPage2, "TabPage2")
        Me.TabPage2.Name = "TabPage2"
        '
        'CbFS
        '
        resources.ApplyResources(Me.CbFS, "CbFS")
        Me.CbFS.Name = "CbFS"
        Me.CbFS.Tag = "-x"
        Me.ToolTip1.SetToolTip(Me.CbFS, resources.GetString("CbFS.ToolTip"))
        Me.CbFS.UseVisualStyleBackColor = True
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.CbWinCompat)
        Me.GroupBox8.Controls.Add(Me.CbPermWin)
        resources.ApplyResources(Me.GroupBox8, "GroupBox8")
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.TabStop = False
        '
        'CbWinCompat
        '
        resources.ApplyResources(Me.CbWinCompat, "CbWinCompat")
        Me.CbWinCompat.Name = "CbWinCompat"
        Me.CbWinCompat.Tag = "--modify-window=2"
        Me.ToolTip1.SetToolTip(Me.CbWinCompat, resources.GetString("CbWinCompat.ToolTip"))
        Me.CbWinCompat.UseVisualStyleBackColor = True
        '
        'CbPermWin
        '
        resources.ApplyResources(Me.CbPermWin, "CbPermWin")
        Me.CbPermWin.Name = "CbPermWin"
        Me.CbPermWin.Tag = "--backup-nt-streams --restore-nt-streams"
        Me.ToolTip1.SetToolTip(Me.CbPermWin, resources.GetString("CbPermWin.ToolTip"))
        Me.CbPermWin.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.Controls.Add(Me.TextBoxLogs)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        '
        'TextBoxLogs
        '
        Me.TextBoxLogs.AcceptsReturn = True
        resources.ApplyResources(Me.TextBoxLogs, "TextBoxLogs")
        Me.TextBoxLogs.Name = "TextBoxLogs"
        '
        'GroupBox6
        '
        resources.ApplyResources(Me.GroupBox6, "GroupBox6")
        Me.GroupBox6.Controls.Add(Me.TextBoxErrors)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.TabStop = False
        '
        'TextBoxErrors
        '
        Me.TextBoxErrors.AcceptsReturn = True
        resources.ApplyResources(Me.TextBoxErrors, "TextBoxErrors")
        Me.TextBoxErrors.Name = "TextBoxErrors"
        '
        'StatusBar
        '
        Me.StatusBar.GripMargin = New System.Windows.Forms.Padding(0)
        Me.StatusBar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusBarText, Me.ProgressBarText, Me.ProgressBar})
        resources.ApplyResources(Me.StatusBar, "StatusBar")
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.SizingGrip = False
        '
        'StatusBarText
        '
        Me.StatusBarText.Name = "StatusBarText"
        resources.ApplyResources(Me.StatusBarText, "StatusBarText")
        Me.StatusBarText.Spring = True
        '
        'ProgressBarText
        '
        Me.ProgressBarText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ProgressBarText.Name = "ProgressBarText"
        resources.ApplyResources(Me.ProgressBarText, "ProgressBarText")
        '
        'ProgressBar
        '
        Me.ProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ProgressBar.Name = "ProgressBar"
        resources.ApplyResources(Me.ProgressBar, "ProgressBar")
        '
        'StatusBarText1
        '
        Me.StatusBarText1.Name = "StatusBarText1"
        resources.ApplyResources(Me.StatusBarText1, "StatusBarText1")
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.ComboRsync)
        resources.ApplyResources(Me.GroupBox7, "GroupBox7")
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.TabStop = False
        '
        'ComboRsync
        '
        resources.ApplyResources(Me.ComboRsync, "ComboRsync")
        Me.ComboRsync.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboRsync.FormattingEnabled = True
        Me.ComboRsync.MinimumSize = New System.Drawing.Size(70, 0)
        Me.ComboRsync.Name = "ComboRsync"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Menu1, Me.Menu2})
        Me.MenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        resources.ApplyResources(Me.MenuStrip1, "MenuStrip1")
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'Menu1
        '
        Me.Menu1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LanguageToolStripMenuItem})
        Me.Menu1.Name = "Menu1"
        resources.ApplyResources(Me.Menu1, "Menu1")
        '
        'LanguageToolStripMenuItem
        '
        Me.LanguageToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CbEnglish, Me.CbFrench})
        Me.LanguageToolStripMenuItem.Name = "LanguageToolStripMenuItem"
        resources.ApplyResources(Me.LanguageToolStripMenuItem, "LanguageToolStripMenuItem")
        '
        'CbEnglish
        '
        resources.ApplyResources(Me.CbEnglish, "CbEnglish")
        Me.CbEnglish.CheckOnClick = True
        Me.CbEnglish.Name = "CbEnglish"
        Me.CbEnglish.Tag = "en-US"
        '
        'CbFrench
        '
        resources.ApplyResources(Me.CbFrench, "CbFrench")
        Me.CbFrench.CheckOnClick = True
        Me.CbFrench.Name = "CbFrench"
        Me.CbFrench.Tag = "fr-FR"
        '
        'Menu2
        '
        Me.Menu2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.Menu2.Name = "Menu2"
        resources.ApplyResources(Me.Menu2, "Menu2")
        '
        'AboutToolStripMenuItem
        '
        resources.ApplyResources(Me.AboutToolStripMenuItem, "AboutToolStripMenuItem")
        Me.AboutToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        '
        'FolderBrowserDialog
        '
        resources.ApplyResources(Me.FolderBrowserDialog, "FolderBrowserDialog")
        '
        'FrameMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.StatusBar)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.GroupBox6)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.TabControl1)
        Me.DataBindings.Add(New System.Windows.Forms.Binding("Location", Global.LicielRsync.My.MySettings.Default, "Location_Frame", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.DataBindings.Add(New System.Windows.Forms.Binding("Size", Global.LicielRsync.My.MySettings.Default, "Size_Frame", True))
        Me.Location = Global.LicielRsync.My.MySettings.Default.Location_Frame
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FrameMain"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.StatusBar.ResumeLayout(False)
        Me.StatusBar.PerformLayout()
        Me.GroupBox7.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents ComboProfiles As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonAdd As System.Windows.Forms.Button
    Friend WithEvents ButtonDel As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents ButtonExec As System.Windows.Forms.Button
    Friend WithEvents ButtonSrcOpen As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TextBoxDst As System.Windows.Forms.TextBox
    Friend WithEvents ButtonDstOpen As System.Windows.Forms.Button
    Friend WithEvents TextBoxSrc As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ButtonTest As System.Windows.Forms.Button
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents TextBoxLogs As System.Windows.Forms.TextBox
    Friend WithEvents CbProgress As System.Windows.Forms.CheckBox
    Friend WithEvents CbRecurse As System.Windows.Forms.CheckBox
    Friend WithEvents CbDate As System.Windows.Forms.CheckBox
    Friend WithEvents CbPerm As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents CbVerbose As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents TextBoxErrors As System.Windows.Forms.TextBox
    Friend WithEvents ComboVerbose As System.Windows.Forms.ComboBox
    Friend WithEvents StatusBar As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusBarText1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents ComboRsync As System.Windows.Forms.ComboBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents Menu2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CbOwner As System.Windows.Forms.CheckBox
    Friend WithEvents CbGroup As System.Windows.Forms.CheckBox
    Friend WithEvents CbDelta As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox9 As System.Windows.Forms.GroupBox
    Friend WithEvents Menu1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LanguageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CbEnglish As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CbFrench As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CbDelete As System.Windows.Forms.CheckBox
    Friend WithEvents CbExisting As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Friend WithEvents CbWinCompat As System.Windows.Forms.CheckBox
    Friend WithEvents CbPermWin As System.Windows.Forms.CheckBox
    Friend WithEvents CbNewer As System.Windows.Forms.CheckBox
    Friend WithEvents CbSizeOnly As System.Windows.Forms.CheckBox
    Friend WithEvents CbReadable As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonPause As System.Windows.Forms.Button
    Friend WithEvents ButtonStop As System.Windows.Forms.Button
    Friend WithEvents ProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents CbChecksum As System.Windows.Forms.CheckBox
    Friend WithEvents CbFS As System.Windows.Forms.CheckBox
    Friend WithEvents CbExistingOnly As System.Windows.Forms.CheckBox
    Friend WithEvents CbIgnoreTimes As System.Windows.Forms.CheckBox
    Friend WithEvents ProgressBarText As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents CbShowCmd As System.Windows.Forms.CheckBox
    Friend WithEvents StatusBarText As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TextBoxOptions As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents CbHideWindows As System.Windows.Forms.CheckBox
    Friend WithEvents CbRedir As System.Windows.Forms.CheckBox

End Class
