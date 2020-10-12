Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class uscContactRest
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public Property ApplyAuthorizations As Boolean
    Public Property ExcludeRoleContacts As Boolean?
    Public Property ParentId As Integer?
    Public Property ParentToExclude As Integer?
    Public Property FilterByParentId As Integer?
    Public Property FilterByTenantEnabled As Boolean = True
    Public ReadOnly Property PanelContent As Panel
        Get
            Return pnlContent
        End Get
    End Property
    Public Property CreateManualContactEnabled As Boolean = True
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        uscContactSearchRest.ApplyAuthorizations = ApplyAuthorizations
        uscContactSearchRest.ExcludeRoleContacts = ExcludeRoleContacts
        uscContactSearchRest.ParentId = ParentId
        uscContactSearchRest.FilterByParentId = FilterByParentId
        uscContactSearchRest.ParentToExclude = ParentToExclude
        uscContactSearchRest.FilterByTenantEnabled = FilterByTenantEnabled
    End Sub
#End Region

#Region " Methods "

#End Region

End Class