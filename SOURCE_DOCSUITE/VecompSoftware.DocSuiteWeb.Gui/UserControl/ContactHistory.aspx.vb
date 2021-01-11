Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons

Public Class ContactHistory
    Inherits CommonBasePage


#Region " Fields "
    Private _contactNameFacade As ContactNameFacade
    Private _currentId As Integer

#End Region

#Region " Properties "
    Public ReadOnly Property ContactNameFacade As ContactNameFacade
        Get
            If _contactNameFacade Is Nothing Then
                _contactNameFacade = New ContactNameFacade()
            End If
            Return _contactNameFacade
        End Get
    End Property

    Public ReadOnly Property CurrentId As Integer
        Get
            Return Convert.ToInt32(Request.QueryString("IdContact"))
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
        AjaxManager.AjaxSettings.AddAjaxSetting(grdContactHistory, grdContactHistory, MasterDocSuite.AjaxDefaultLoadingPanel)

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
        grdContactHistory.DataSource = ContactNameFacade.GetContactNamesByIncremental(CurrentId)
        grdContactHistory.DataBind()
    End Sub


    Private Sub grdRoleHistory_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdContactHistory.ItemDataBound

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim ContactName As ContactName = DirectCast(e.Item.DataItem, ContactName)


        Dim lblNomeContatto As Label = DirectCast(e.Item.FindControl("lblNomeContatto"), Label)
        lblNomeContatto.Text = ContactName.Name

        Dim lblFromDate As Label = DirectCast(e.Item.FindControl("lblFromDate"), Label)
        lblFromDate.Text = ContactName.FromDate.ToString

        Dim lblToDate As Label = DirectCast(e.Item.FindControl("lblToDate"), Label)
        lblToDate.Text = ContactName.ToDate.ToString

    End Sub


#End Region

End Class