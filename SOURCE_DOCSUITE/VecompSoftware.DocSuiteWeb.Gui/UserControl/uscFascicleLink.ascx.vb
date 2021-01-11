Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons

Public Class uscFascicleLink
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private Const CATEGORY_CHANGE_HANDLER As String = "uscFascicleLink.onCategoryChanged({0});"
#End Region

    Public Property ViewOnlyFascicolable As Boolean?

    Public Property Rights As String

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public Property IdCategory As Integer

#End Region


#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscCategory.ViewOnlyFascicolable = ViewOnlyFascicolable
        uscCategory.Rights = Rights
        InitializeAjax()
    End Sub

    Private Sub uscClassificatore_CategoryChange(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryAdded, uscCategory.CategoryRemoved
        If Not uscCategory.HasSelectedCategories Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, 0))
            Exit Sub
        End If

        Dim category As Category = uscCategory.SelectedCategories.First()

        If category Is Nothing Then
            AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, 0))
            Exit Sub
        End If

        AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_CHANGE_HANDLER, category.Id))
    End Sub

    Protected Sub uscDossierFascLinkAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, "FascLinkCallback") Then
            uscCategory.CategoryID = Int32.Parse(ajaxModel.Value(0))

        End If


    End Sub

#End Region


#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscDossierFascLinkAjaxRequest
    End Sub

#End Region
End Class