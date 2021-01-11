Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscFascicleSearch
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const SEARCH_BY_CATEGORY_ACTION As String = "searchByCategory"
    Private Const SEARCH_BY_SUBJECT_ACTION As String = "searchBySubject"
    Private Const SEARCH_BY_MEADATA_VALUE_ACTION As String = "searchByMetadataValue"
    Private Const OPEN_WINDOW_CALLBACK As String = "uscFascicleSearch.openWindowCallback('{0}', '{1}')"
#End Region

#Region " Properties "
    Public ReadOnly Property PageControl As Control
        Get
            Return pageContent
        End Get
    End Property

    Public Property MinHeight As String

    Public Property DefaultCategoryId As Integer?
    Public Property FolderSelectionEnabled As Boolean
    Public Property FascicleObject As String
    Public Property CategoryFullIncrementalPath As String
    Public Property DSWEnvironment As Integer
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not String.IsNullOrEmpty(MinHeight) Then
            finderContent.Style.Add("min-height", MinHeight)
        End If

        uscFascicleFolders.IsVisibile = FolderSelectionEnabled
        uscFascicleFolders.Visible = FolderSelectionEnabled
        If Not IsPostBack Then
            fascSummaryColumn.Span = If(FolderSelectionEnabled, 8, 12)
            If FolderSelectionEnabled Then
                fascFoldersColumn.Span = 4
            End If
        End If
    End Sub

    Protected Sub uscFascicleSearch_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try
        If ajaxModel Is Nothing Then
            Return
        End If

        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 _
            AndAlso (ajaxModel.ActionName.Eq(SEARCH_BY_CATEGORY_ACTION) OrElse ajaxModel.ActionName.Eq(SEARCH_BY_SUBJECT_ACTION) _
            OrElse ajaxModel.ActionName.Eq(SEARCH_BY_MEADATA_VALUE_ACTION)) Then
            Dim fascicleFinderModel As FascicleFinderModel = New FascicleFinderModel()
            fascicleFinderModel.IsManager = True
            fascicleFinderModel.IsSecretary = True
            Select Case ajaxModel.ActionName
                Case SEARCH_BY_CATEGORY_ACTION
                    fascicleFinderModel.Classifications = ajaxModel.Value(0)
                Case SEARCH_BY_SUBJECT_ACTION
                    fascicleFinderModel.Subject = ajaxModel.Value(0)
                Case SEARCH_BY_MEADATA_VALUE_ACTION
                    fascicleFinderModel.IsManager = False
                    fascicleFinderModel.IsSecretary = False
                    Dim metadataFinder As MetadataFinderModel = JsonConvert.DeserializeObject(Of MetadataFinderModel)(ajaxModel.Value(0))
                    Dim metadataDate As Date = Nothing
                    Dim IsDate As Boolean = Date.TryParseExact(metadataFinder.Value, "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture, DateTimeStyles.None, metadataDate)
                    If IsDate Then
                        metadataFinder.Value = metadataDate.ToString("yyyy-MM-dd")
                    End If
                    fascicleFinderModel.MetadataValue = metadataFinder.Value
            End Select
            RegisterFascicleFinder(fascicleFinderModel, "searchFasciclesByCategory")
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscFascicleSearch_AjaxRequest
    End Sub

    Private Sub RegisterFascicleFinder(ByVal fascicleFinderModel As FascicleFinderModel, ByVal windowName As String)
        Dim fascicleFinder As FascicleFinder = New FascicleFinder(DocSuiteContext.Current.Tenants)
        fascicleFinder.EnablePaging = True
        fascicleFinder.FromPostMethod = True
        fascicleFinder.FascicleFinderModel = fascicleFinderModel
        fascicleFinder.PageSize = 10
        SessionSearchController.SaveSessionFinder(fascicleFinder, SessionSearchController.SessionFinderType.FascFinderType)
        AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_CALLBACK, "../Fasc/FascRisultati.aspx?Type=Fasc&Action=SearchFascicles&EnableSessionFilterLoading=True", windowName))
    End Sub
#End Region

End Class