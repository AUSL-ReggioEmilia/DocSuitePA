Imports System.Collections.Generic

Public Class RoleUserEventArgs
    Inherits EventArgs

#Region " Properties "
    Public Property RoleUser As KeyValuePair(Of String, String)
    Public Property Cancel As Boolean
    Public Property IsPersistent As Boolean
#End Region

#Region " Constructor "

    Public Sub New(ByVal roleUser As KeyValuePair(Of String, String))
        _RoleUser = roleUser
    End Sub

#End Region

End Class
