''----------------------------------------------------------------------------------------------
''
'' LicielRsync -  A multi-threaded interface for Rsync on Windows
'' By Arnaud Dovi - ad@heapoverflow.com
'' Rsync - http://rsync.samba.org
''
'' ModuleSettings
''
'' Work around a Visual Studio 2010 limitation with the use of
'' Hashtables in My Project > Settings
''
'' Configuration.SettingsSerializeAs.Binary translates the hashtable element to a base64 string
''----------------------------------------------------------------------------------------------
Option Explicit On


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
