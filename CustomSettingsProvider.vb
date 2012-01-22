Imports System
Imports System.Xml
Imports System.Collections
Imports System.Configuration
Imports System.Windows.Forms
Imports System.Collections.Specialized

Public Class CustomSettingsProvider
    Inherits SettingsProvider

    Const Settingsroot As String = "Settings" 'XML Root Node

    Public Overrides Sub Initialize(ByVal title As String, ByVal col As NameValueCollection)
        MyBase.Initialize(ApplicationName, col)
    End Sub

    Public Overrides Property ApplicationName() As String
        Get
            If Application.ProductName.Trim.Length > 0 Then
                Return Application.ProductName
            Else
                Dim fi As New IO.FileInfo(Application.ExecutablePath)
                Return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)
            End If
        End Get
        Set(ByVal value As String)
            'Do nothing
        End Set
    End Property

    Overridable Function GetAppSettingsPath() As String
        'Used to determine where to store the settings
        Dim fi As New IO.FileInfo(Application.ExecutablePath)
        Return fi.DirectoryName
    End Function

    Overridable Function GetAppSettingsFilename() As String
        'Used to determine the filename to store the settings
        Return ApplicationName & ".settings"
    End Function

    Public Overrides Sub SetPropertyValues(ByVal context As SettingsContext, ByVal propvals As SettingsPropertyValueCollection)
        'Iterate through the settings to be stored
        'Only dirty settings are included in propvals, and only ones relevant to this provider
        For Each propval As SettingsPropertyValue In propvals
            SetValue(propval)
        Next
        Try
            SettingsXml.Save(IO.Path.Combine(GetAppSettingsPath, GetAppSettingsFilename))
        Catch ex As Exception
            'Ignore if cant save, device been ejected
        End Try
    End Sub

    Public Overrides Function GetPropertyValues(ByVal context As SettingsContext, ByVal props As SettingsPropertyCollection) As SettingsPropertyValueCollection
        'Create new collection of values
        Dim values As SettingsPropertyValueCollection = New SettingsPropertyValueCollection()
        'Iterate through the settings to be retrieved
        For Each setting As SettingsProperty In props
            Dim value As SettingsPropertyValue = New SettingsPropertyValue(setting)
            value.IsDirty = False
            value.SerializedValue = GetValue(setting)
            values.Add(value)
        Next
        Return values
    End Function

    Private _mSettingsXml As XmlDocument = Nothing

    Private ReadOnly Property SettingsXml() As XmlDocument
        Get
            'If we dont hold an xml document, try opening one.  
            'If it doesnt exist then create a new one ready.
            If _mSettingsXml Is Nothing Then
                _mSettingsXml = New XmlDocument
                Try
                    _mSettingsXml.Load(IO.Path.Combine(GetAppSettingsPath, GetAppSettingsFilename))
                Catch ex As Exception
                    'Create new document
                    Dim dec As XmlDeclaration = _mSettingsXml.CreateXmlDeclaration("1.0", "utf-8", String.Empty)
                    _mSettingsXml.AppendChild(dec)
                    Dim nodeRoot As XmlNode
                    nodeRoot = _mSettingsXml.CreateNode(XmlNodeType.Element, Settingsroot, "")
                    _mSettingsXml.AppendChild(nodeRoot)
                End Try
            End If
            Return _mSettingsXml
        End Get
    End Property

    Private Function GetValue(ByVal setting As SettingsProperty) As String
        Dim ret As String
        Try
            If IsRoaming(setting) Then
                ret = SettingsXml.SelectSingleNode(Settingsroot & "/" & setting.Name).InnerText
            Else
                ret = SettingsXml.SelectSingleNode(Settingsroot & "/" & My.Computer.Name & "/" & setting.Name).InnerText
            End If
        Catch ex As Exception
            If Not setting.DefaultValue Is Nothing Then
                ret = setting.DefaultValue.ToString
            Else
                ret = ""
            End If
        End Try
        Return ret
    End Function

    Private Sub SetValue(ByVal propVal As SettingsPropertyValue)
        Dim machineNode As XmlElement
        Dim settingNode As XmlElement
        'Determine if the setting is roaming.
        'If roaming then the value is stored as an element under the root
        'Otherwise it is stored under a machine name node 
        Try
            If IsRoaming(propVal.Property) Then
                settingNode = DirectCast(SettingsXml.SelectSingleNode(Settingsroot & "/" & propVal.Name), XmlElement)
            Else
                settingNode = DirectCast(SettingsXml.SelectSingleNode(Settingsroot & "/" & My.Computer.Name & "/" & propVal.Name), XmlElement)
            End If
        Catch ex As Exception
            settingNode = Nothing
        End Try
        'Check to see if the node exists, if so then set its new value
        If Not settingNode Is Nothing Then
            settingNode.InnerText = propVal.SerializedValue.ToString
        Else
            If IsRoaming(propVal.Property) Then
                'Store the value as an element of the Settings Root Node
                settingNode = SettingsXml.CreateElement(propVal.Name)
                settingNode.InnerText = propVal.SerializedValue.ToString
                SettingsXml.SelectSingleNode(Settingsroot).AppendChild(settingNode)
            Else
                'Its machine specific, store as an element of the machine name node,
                'creating a new machine name node if one doesnt exist.
                Try
                    machineNode = DirectCast(SettingsXml.SelectSingleNode(Settingsroot & "/" & My.Computer.Name), XmlElement)
                Catch ex As Exception
                    machineNode = SettingsXml.CreateElement(My.Computer.Name)
                    SettingsXml.SelectSingleNode(Settingsroot).AppendChild(machineNode)
                End Try
                If machineNode Is Nothing Then
                    machineNode = SettingsXml.CreateElement(My.Computer.Name)
                    SettingsXml.SelectSingleNode(Settingsroot).AppendChild(machineNode)
                End If
                settingNode = SettingsXml.CreateElement(propVal.Name)
                settingNode.InnerText = propVal.SerializedValue.ToString
                machineNode.AppendChild(settingNode)
            End If
        End If
    End Sub

    Private Shared Function IsRoaming(ByVal prop As SettingsProperty) As Boolean
        'Determine if the setting is marked as Roaming
        For Each d As DictionaryEntry In prop.Attributes
            Dim a As Attribute = DirectCast(d.Value, Attribute)
            If TypeOf a Is SettingsManageabilityAttribute Then
                Return True
            End If
        Next
        Return False
    End Function

End Class
