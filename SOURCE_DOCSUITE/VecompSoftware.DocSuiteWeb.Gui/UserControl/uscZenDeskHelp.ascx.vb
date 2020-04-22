Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.ZenDesk.Services

Public Class uscZenDeskHelp
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Public Property Categories() As String
    Public Property AjaxDefaultLoadingPanel() As RadAjaxLoadingPanel
    Public ReadOnly Property zdService As ZenDeskService
        Get
            Return New ZenDeskService(ProtocolEnv.ZenDeskUrl, ProtocolEnv.ZenDeskEmail, ProtocolEnv.ZenDeskToken)
        End Get
    End Property
    Public Property IsButtonPressed As String
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub UscFascicoloAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Try
                Select Case ajaxModel.ActionName
                    Case "LoadSectionsAndArticles"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim SectionsByCategory As List(Of SectionModel) = LoadSectionsByCategory(ajaxModel.Value(0))
                            Dim SectionsByCategoryJson As String = ValidateJsonResult(ConvertToJson(SectionsByCategory))
                            AjaxManager.ResponseScripts.Add($"zenDesk.loadSections(""{SectionsByCategoryJson}"");")
                            For Each section As SectionModel In SectionsByCategory
                                Dim ArticlesBySection As List(Of ArticleModel) = LoadArticlesBySection(section.id.ToString())
                                Dim ArticlesBySectionJson As String = ValidateJsonResult(ConvertToJson(ArticlesBySection))
                                AjaxManager.ResponseScripts.Add($"zenDesk.loadArticlesBySection(""{ArticlesBySectionJson}"", ""{SectionsByCategory.IndexOf(section)}"");")
                            Next
                        End If
                    Case "LoadSearchedArticles"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim SearchedArticles As ArticlesSearchModel = LoadSearchedArticles(ajaxModel.Value(0), ajaxModel.Value(1))
                            Dim SearchedArticlesJson As String = ValidateJsonResult(ConvertToJson(SearchedArticles.Results))
                            AjaxManager.ResponseScripts.Add($"zenDesk.setArticlesNextPage(""{SearchedArticles.NextPage}"");")
                            AjaxManager.ResponseScripts.Add($"zenDesk.loadSearchedArticles(""{SearchedArticlesJson}"");")
                        End If
                    Case "LoadArticleSearchNextPage"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim NextSearchedArticles As ArticlesSearchModel = LoadNextArticles(ajaxModel.Value(0))
                            Dim NextSearchedArticlesJson As String = ValidateJsonResult(ConvertToJson(NextSearchedArticles.Results))
                            AjaxManager.ResponseScripts.Add($"zenDesk.setArticlesNextPage(""{NextSearchedArticles.NextPage}"");")
                            AjaxManager.ResponseScripts.Add($"zenDesk.loadNextSearchedArticles(""{NextSearchedArticlesJson}"");")
                        End If
                End Select
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore nel caricamento dei dati del articoli.", ex)
                AjaxManager.Alert("Errore nel caricamento dei dati del articoli.")
                Return
            End Try

        End If

    End Sub
#End Region

#Region "Methods"
    Sub Initialize()
        Categories = ValidateJsonResult(ConvertToJson(LoadCategories()))
    End Sub
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscFascicoloAjaxRequest
    End Sub
    Function LoadCategories() As List(Of CategoryModel)
        Dim categories As List(Of CategoryModel) = zdService.GetCategories().Result.ToList()
        Return categories
    End Function
    Function LoadSectionsByCategory(categoryId As String) As List(Of SectionModel)
        Dim sections As List(Of SectionModel) = zdService.GetSectionsByCategoryId(categoryId).Result.ToList()
        Return sections
    End Function
    Function LoadArticlesBySection(sectionId As String) As List(Of ArticleModel)
        Dim articles As List(Of ArticleModel) = zdService.GetArticlesBySectionId(sectionId).Result.ToList()
        Return articles
    End Function
    Function LoadSearchedArticles(categoryId As String, searchFilter As String) As ArticlesSearchModel
        Dim articles As ArticlesSearchModel = zdService.GetArticles(searchFilter, categoryId).Result
        Return articles
    End Function
    Function LoadNextArticles(nextPageUrl As String) As ArticlesSearchModel
        Dim articles As ArticlesSearchModel = zdService.GetNextArticles(nextPageUrl).Result
        Return articles
    End Function
    Function ConvertToJson(Of TModel)(model As TModel) As String
        Return JsonConvert.SerializeObject(model)
    End Function
    Function ValidateJsonResult(result As String) As String
        Return result.Replace("\", "\\").Replace("""", "\""\""")
    End Function
#End Region

End Class