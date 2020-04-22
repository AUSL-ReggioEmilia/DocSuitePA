Public Class uscContactSearchRest
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public Property ApplyAuthorizations As Boolean?
    Protected ReadOnly Property ApplyAuthorizationsJString As String
        Get
            If Not ApplyAuthorizations.HasValue Then
                Return "null"
            End If
            Return ApplyAuthorizations.Value.ToString().ToLower()
        End Get
    End Property
    Public Property ExcludeRoleContacts As Boolean?
    Protected ReadOnly Property ExcludeRoleContactsJString As String
        Get
            If Not ExcludeRoleContacts.HasValue Then
                Return "null"
            End If
            Return ExcludeRoleContacts.Value.ToString().ToLower()
        End Get
    End Property
    Public Property ParentId As Integer?
    Protected ReadOnly Property ParentIdJString As String
        Get
            If Not ParentId.HasValue Then
                Return "null"
            End If
            Return ParentId.Value.ToString()
        End Get
    End Property
    Public Property ParentToExclude As Integer?
    Protected ReadOnly Property ParentToExcludeJString As String
        Get
            If Not ParentToExclude.HasValue Then
                Return "null"
            End If
            Return ParentToExclude.Value.ToString()
        End Get
    End Property

    Public ReadOnly Property MainPanel As Control
        Get
            Return pnlMainContent
        End Get
    End Property

    Public Property FilterByParentId As Integer?
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "

#End Region

End Class