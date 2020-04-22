Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class DocmTokenUser
    Inherits DocmBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub InitializeAjaxSettings()
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(RadGrid1, RadGrid1, MasterDocSuite.AjaxLoadingPanelSearch)
    End Sub

    Private Sub Initialize()
        Dim finder As NHibernateDocumentTokenUserFinder = Facade.DocumentTokenUserFinder
        finder.DocumentYear = CurrentDocumentYear
        finder.DocumentNumber = CurrentDocumentNumber
        finder.PageSize = DocumentEnv.SearchMaxRecords
        RadGrid1.Finder = finder
        RadGrid1.DataBindFinder()
    End Sub


    Public Function SetImageUrl(ByVal isActive As Integer) As String
        Dim s As String = String.Empty
        Select Case isActive
            Case 0 : s = "../Comm/Images/Remove16.gif"
            Case 1 : s = "../Comm/Images/User16.gif"
        End Select
        Return s
    End Function

    Protected Sub RadGrid1_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles RadGrid1.ItemDataBound
        Dim gridItem As GridDataItem = TryCast(e.Item, GridDataItem)
        If (gridItem IsNot Nothing) Then
            If gridItem.ItemType = Telerik.Web.UI.GridItemType.AlternatingItem Or gridItem.ItemType = Telerik.Web.UI.GridItemType.Item Then
                Dim documentTokenUser As DocumentTokenUser = CType(e.Item.DataItem, DocumentTokenUser)
                Dim image As Image = e.Item.FindControl("Image1")
                image.ImageUrl = SetImageUrl(documentTokenUser.IsActive)

            End If

        End If

    End Sub
End Class