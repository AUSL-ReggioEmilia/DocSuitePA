Imports System.Web.Security
Imports System.Security.Principal
Imports System.Runtime.InteropServices
Imports VecompSoftware.Services.Logging

' TODO: Spostare in un helper in c#, ripulire e sopratutto IMPLEMENTARE IDISPOSE così poi abemus using
Public Class Impersonator
    Dim LOGON32_LOGON_INTERACTIVE As Integer = 2
    Dim LOGON32_PROVIDER_DEFAULT As Integer = 0
    Dim ImpersonationContext As WindowsImpersonationContext
    Dim ImpersonatorEnabled As Boolean = False
    Private Declare Function LogonUserA Lib "advapi32.dll" (ByVal lpszUsername As String, _
                            ByVal lpszDomain As String, _
                            ByVal lpszPassword As String, _
                            ByVal dwLogonType As Integer, _
                            ByVal dwLogonProvider As Integer, _
                            ByRef phToken As IntPtr) As Integer

    Private Declare Auto Function DuplicateToken Lib "advapi32.dll" ( _
                            ByVal ExistingTokenHandle As IntPtr, _
                            ByVal ImpersonationLevel As Integer, _
                            ByRef DuplicateTokenHandle As IntPtr) As Integer

    Private Declare Auto Function RevertToSelf Lib "advapi32.dll" () As Long
    Private Declare Auto Function CloseHandle Lib "kernel32.dll" (ByVal handle As IntPtr) As Long

    Public Function ImpersonateValidUser(ByVal FullName As String, ByVal Password As String) As Boolean
        Dim UserName As String = Mid(FullName, InStr(FullName, "\") + 1)
        Dim Domain As String = Left(FullName, InStr(FullName, "\") - 1)
        Dim tempWindowsIdentity As WindowsIdentity
        Dim token As IntPtr = IntPtr.Zero
        Dim tokenDuplicate As IntPtr = IntPtr.Zero
        ImpersonateValidUser = False
        If RevertToSelf() Then
            If LogonUserA(UserName, Domain, Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, token) <> 0 Then
                If DuplicateToken(token, 2, tokenDuplicate) <> 0 Then
                    tempWindowsIdentity = New WindowsIdentity(tokenDuplicate)
                    ImpersonationContext = tempWindowsIdentity.Impersonate()
                    If Not ImpersonationContext Is Nothing Then
                        ImpersonateValidUser = True
                    End If
                End If
            End If
        End If
        If Not tokenDuplicate.Equals(IntPtr.Zero) Then
            CloseHandle(tokenDuplicate)
        End If
        If Not token.Equals(IntPtr.Zero) Then
            CloseHandle(token)
        End If
        ImpersonatorEnabled = True
    End Function

    Public Sub ImpersonationUndo()
        If ImpersonatorEnabled And ImpersonationContext IsNot Nothing Then
            ImpersonationContext.Undo()
            ImpersonatorEnabled = False
        End If
    End Sub

    Protected Overrides Sub Finalize()
        ImpersonationUndo()
        MyBase.Finalize()
    End Sub
End Class
