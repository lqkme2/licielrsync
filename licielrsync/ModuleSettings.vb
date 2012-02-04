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
