''----------------------------------------------------------------------------------------------
'' licielrsync - a multi-threaded interface for rsync on windows
'' by Arnaud Dovi - ad@heapoverflow.com
'' homepage - licielrsync.googlecode.com
''
'' rsync is maintained by Wayne Davison
'' homepage - rsync.samba.org
''
'' modulevbmsilreader.vb
''
'' a simple function to check the return value of w32 api and throw an exception
'' the part retrieving the function name inside the delegate is inspired on 
'' Haibo Luo's MSIL Reader in C#, simplified to the bare minimum because we are
'' only looking for a call procedure which the win32 api is next to, more informations
'' at hxxp://blogs.msdn.com/b/haibo_luo/archive/2006/11/16/take-two-il-visualizer.aspx
''
'' Sample usage :
''
'' Verify(Function() NativeMethod.MessageBoxA(...))
''
'' (this code may need some tweaking if you are using more than one method inside the api call)
''----------------------------------------------------------------------------------------------



Imports System.Reflection
Imports System.ComponentModel
Imports System.Runtime.InteropServices

Friend Module ModuleVbMsilReader

    ''--------------------------------------------------------------------
    '' Verify
    ''
    '' Simplify the verification of win32 api return values
    ''--------------------------------------------------------------------

    Friend VerifyReturn As Object = Nothing

    Friend Delegate Function Win32Delegate() As Object

    Friend Sub Verify(ByVal w32Func As Win32Delegate)
        Try
            UnsafeNativeMethods.SetLastError(0)
            VerifyReturn = w32Func()
            Dim errVal As Integer = Marshal.GetLastWin32Error()
            If TypeOf VerifyReturn Is IntPtr AndAlso VerifyReturn = IntPtr.Zero AndAlso errVal <> 0 Or _
            TypeOf VerifyReturn Is Integer AndAlso VerifyReturn <= 0 AndAlso errVal <> 0 Then
                Dim methodBase As MethodInfo = w32Func.Method
                Dim methodByteArray As Byte() = methodBase.GetMethodBody().GetILAsByteArray()
                ''
                '' find the call procedure (if many calls found the last one has more chance to be our win32 api call)
                ''
                Array.Reverse(methodByteArray)
                Dim callIndex As Integer = Array.FindIndex(methodByteArray, Function(x) x = 40)
                Array.Reverse(methodByteArray)
                callIndex = (methodByteArray.Length - 1) - callIndex
                ''
                '' the win32 function is always next to the call
                ''
                Dim token As Integer = BitConverter.ToInt32(methodByteArray, callIndex + 1)
                Throw New ApplicationException(String.Format("An error is returned by a win32 api{0}{1}{0}{2}", ControlChars.CrLf, methodBase.Module.ResolveMethod(token).ToString, New Win32Exception(errVal).ToString()))
            End If
        Catch ex As ApplicationException
            Throw
        Catch ex As Exception
            Throw
        End Try
    End Sub

End Module
