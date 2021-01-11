Public Class CommonSelContactRest
    Inherits CommBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property CallerId As String
        Get
            Return GetKeyValue(Of String, CommonSelContactRest)("ParentID")
        End Get
    End Property

    Public ReadOnly Property AVCPBusinessContactEnabled As Boolean
        Get
            Return GetKeyValue(Of Boolean, CommonSelContactRest)("AVCPBusinessContactEnabled")
        End Get
    End Property

    Public ReadOnly Property FilterByParentId As Integer?
        Get
            Return GetKeyValue(Of Integer?, CommonSelContactRest)("FilterByParentId")
        End Get
    End Property

    Private ReadOnly Property ParentToExclude As Integer?
        Get
            If ProtocolEnv.AVCPIdBusinessContact > 0 AndAlso Not AVCPBusinessContactEnabled Then
                Return ProtocolEnv.AVCPIdBusinessContact
            End If
            Return Nothing
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        uscContactRest.ApplyAuthorizations = True
        uscContactRest.ExcludeRoleContacts = Not ProtocolEnv.RoleContactEnabled
        uscContactRest.ParentToExclude = ParentToExclude
        uscContactRest.FilterByParentId = FilterByParentId
    End Sub
#End Region

#Region " Methods "

#End Region

End Class