Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class FascRicerca
    Inherits FascBasePage

#Region "Properties"

    Public ReadOnly Property ChoiseFolderEnabled As String
        Get
            Return GetKeyValueOrDefault("ChoiseFolderEnabled", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SelectedFascicleFolderId As String
        Get
            Return GetKeyValueOrDefault("SelectedFascicleFolderId", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleId As String
        Get
            Return GetKeyValueOrDefault("CurrentFascicleId", String.Empty)
        End Get
    End Property
    Public ReadOnly Property DefaultCategoryId As Integer?
        Get
            Return GetKeyValueOrDefault(Of Integer?)("DefaultCategoryId", Nothing)
        End Get
    End Property
    Public ReadOnly Property BackButtonEnabled() As Boolean
        Get
            Return GetKeyValueOrDefault("BackButtonEnabled", False)
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        uscFascicleFinder.DefaultCategoryId = DefaultCategoryId
        If Action.Eq("SearchFascicles") Then
            MasterDocSuite.TitleVisible = False
        End If
        If Not IsPostBack Then
            btnSearch.Focus()
        End If
    End Sub

#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, pageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf FascRicerca_AjaxRequest
    End Sub

    Protected Sub FascRicerca_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Try
            Dim ajaxModel As AjaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If
            Select Case ajaxModel.ActionName
                Case "Search"
                    If ajaxModel.Value.Count = 2 Then
                        uscFascicleFinder.CurrentMetadataValue = ajaxModel.Value(0)
                        uscFascicleFinder.CurrentMetadataValues = JsonConvert.DeserializeObject(Of ICollection(Of MetadataFinderModel))(ajaxModel.Value(1))
                    End If
                    Search()
                    Exit Select
            End Select
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub Search()
        If ProtocolEnv.SearchMaxRecords <> 0 Then
            uscFascicleFinder.PageSize = ProtocolEnv.SearchMaxRecords
        End If
        SessionSearchController.SaveSessionFinder(uscFascicleFinder.Finder, SessionSearchController.SessionFinderType.FascFinderType)
        ClearSessions(Of FascRisultati)()
        Response.Redirect($"~/Fasc/FascRisultati.aspx?Type=Fasc&Action={Action}&ChoiseFolderEnabled={ChoiseFolderEnabled}&SelectedFascicleFolderId={SelectedFascicleFolderId}&CurrentFascicleId={CurrentFascicleId}&BackButtonEnabled={BackButtonEnabled}")
    End Sub
#End Region

End Class
