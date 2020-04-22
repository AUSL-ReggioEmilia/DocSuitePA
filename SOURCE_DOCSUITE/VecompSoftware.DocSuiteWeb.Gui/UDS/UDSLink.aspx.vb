Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UDSLink
    Inherits UDSBasePage

    Private Const PAGE_SEARCH_TITLE As String = "Ricerca Archivi"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Public ReadOnly Property CurrentUDSRepositoryId As Guid?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSRepository", Nothing)
        End Get
    End Property

    Private ReadOnly Property CurrentUDSTypologyId As Guid?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSTypology", Nothing)
        End Get
    End Property

    Private Property CurrentFinder As UDSFinderDto
        Get
            If Session("CurrentFinder") IsNot Nothing Then
                Return DirectCast(Session("CurrentFinder"), UDSFinderDto)
            End If
            Return Nothing
        End Get
        Set(value As UDSFinderDto)
            If value Is Nothing Then
                Session.Remove("CurrentFinder")
            Else
                Session("CurrentFinder") = value
            End If
        End Set
    End Property

    Protected Sub udsLink_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, "CurentFinderChanged") Then
            CurrentFinder = uscUDS.GetFinderModel()
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf udsLink_AjaxRequest
    End Sub

    Private selectedRepositoryValue As String
    Public ReadOnly Property SelectedUDSRepository() As Guid?
        Get
            Return uscUDS.CurrentUDSRepositoryId
        End Get
    End Property

    Private Sub Initialize()
        Title = PAGE_SEARCH_TITLE
        uscUDS.ActionType = uscUDS.ACTION_TYPE_SEARCH
        uscUDS.CurrentUDSRepositoryId = If(CurrentUDSRepositoryId, Nothing)
        uscUDS.CurrentUDSTypologyId = If(CurrentUDSTypologyId, Nothing)
    End Sub

End Class