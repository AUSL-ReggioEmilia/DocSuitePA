Imports Telerik.Web.UI

Public Class uscContattiSelRest
    Inherits System.Web.UI.UserControl
#Region "Properties"
    Public ReadOnly Property PanelContent As Panel
        Get
            Return pnlContent
        End Get
    End Property

    Public Property Required As Boolean
        Get
            Return AnyNodeCheck.Enabled
        End Get
        Set(ByVal value As Boolean)
            AnyNodeCheck.Enabled = value
        End Set
    End Property

    Public Property FilterByParentId As Integer?
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscContactRest.FilterByParentId = FilterByParentId
    End Sub

End Class