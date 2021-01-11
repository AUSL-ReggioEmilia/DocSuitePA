Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods


Partial Public Class uscDocmToken
    Inherits DocSuite2008BaseControl

#Region " Properties "

    Public Property Year As Short
    Public Property Number As Integer
    Public Property CurrentDocument As Document

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Year = Request.QueryString.GetValue(Of Short)("Year")
        Number = Request.QueryString.GetValue(Of Integer)("Number")

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub dgTokens_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgTokens.ItemDataBound
        If Not e.Item.ItemType.Equals(GridItemType.Item) AndAlso Not e.Item.ItemType.Equals(GridItemType.AlternatingItem) Then
            Exit Sub
        End If

        Dim token As DocumentToken = DirectCast(e.Item.DataItem, DocumentToken)

        With DirectCast(e.Item.FindControl("imgInfo"), Image)
            Select Case token.Response
                Case "N"
                    .ImageUrl = "../Comm/Images/Remove16.gif"
                Case "A"
                    .ImageUrl = "../Comm/Images/Delete.gif"
                Case Else
                    .ImageUrl = ImagePath.SmallEmpty
            End Select
        End With
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgTokens, dgTokens, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Dim finder As NHibernateDocumentTokenFinder = Facade.DocumentTokenFinder()
        finder.DocumentYear = Year
        finder.DocumentNumber = Number
        dgTokens.Finder = finder
        dgTokens.MasterTableView.AllowPaging = False
        dgTokens.DataBindFinder()
    End Sub


#End Region

End Class
