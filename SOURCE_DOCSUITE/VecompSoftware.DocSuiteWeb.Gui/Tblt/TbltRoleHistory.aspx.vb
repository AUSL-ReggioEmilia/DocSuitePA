Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons


Public Class TbltHistory
    Inherits CommonBasePage


#Region " Fields "
    Private _roleNamesFacade As RoleNameFacade
    Private _currentId As String

#End Region

#Region " Properties "
    Public ReadOnly Property RoleNamesFacade As RoleNameFacade
        Get
            If _roleNamesFacade Is Nothing Then
                _roleNamesFacade = New RoleNameFacade()
            End If
            Return _roleNamesFacade
        End Get
    End Property

    Public ReadOnly Property CurrentId As Integer
        Get
            Return Convert.ToInt32(Request.QueryString("IdRole"))
        End Get
    End Property

  
#End Region

#Region " Events "



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            DataBindHistoryGrid()
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(grdRoleHistory, grdRoleHistory, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub
    Private Sub RadAjaxManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        DataBindHistoryGrid()
    End Sub

 

    Private Sub cmdClose_Click(sender As Object, e As EventArgs) Handles cmdClose.Click
        AjaxManager.ResponseScripts.Add("CloseWin();")
    End Sub

#End Region


#Region " Methods "

    Private Sub DataBindHistoryGrid()
        grdHistoryDatabind()
    End Sub

    Private Sub grdHistoryDatabind()
        grdRoleHistory.DataSource = RoleNamesFacade.GetRoleNamesByIdRole(CurrentId)
        grdRoleHistory.DataBind()
    End Sub


    Private Sub grdRoleHistory_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdRoleHistory.ItemDataBound

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim ContactName As RoleName = DirectCast(e.Item.DataItem, RoleName)


        Dim lblNomeSettore As Label = DirectCast(e.Item.FindControl("lblNomeSettore"), Label)
        lblNomeSettore.Text = ContactName.Name

        Dim lblFromDate As Label = DirectCast(e.Item.FindControl("lblFromDate"), Label)
        lblFromDate.Text = ContactName.FromDate.ToString

        Dim lblToDate As Label = DirectCast(e.Item.FindControl("lblToDate"), Label)
        lblToDate.Text = ContactName.ToDate.ToString

    End Sub


#End Region

End Class