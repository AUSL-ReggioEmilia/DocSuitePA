Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UDSSearch
    Inherits UDSBasePage

#Region "Fields"
    Private Const PAGE_SEARCH_TITLE As String = "Ricerca Archivi"
    Private Const UDS_RESULTS_PAGE_URL As String = "~/UDS/UDSResults.aspx?Type=UDS&CopyToPEC={0}"

#Region "Odata query"
    Private Const ODATA_DOCUMENTUNIT_FILTER As String = "DocumentUnit/DocumentUnitRoles/any(r:r/{0} in ({1}))"
    Private Const ODATA_CONTACT_FILTER As String = "Contacts/any(contacts:contacts/{0} eq {1} and contacts/ContactLabel eq '{2}')"
    Private Const ODATA_CONTACT_MANUAL_FILTER As String = "(Contacts/any(c:contains(c/{0},'{1}') and c/ContactLabel eq '{2}') or Contacts/any(c1:contains(c1/Contact/Description,'{1}') and c1/ContactLabel eq '{2}'))"
#End Region
#End Region

#Region "Properties"
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

    Private ReadOnly Property CopyToPEC As Boolean?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Boolean?)("CopyToPEC", Nothing)
        End Get
    End Property

    Private ReadOnly Property CurrentUDSRepositoryId As Guid?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSRepository", Nothing)
        End Get
    End Property

    Private ReadOnly Property CurrentUDSTypologyId As Guid?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSTypology", Nothing)
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub UDSIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles uscUDS.UDSIndexChanged
        If String.IsNullOrEmpty(e.Value) Then
            btnSearch.Enabled = False
            Return
        End If

        btnSearch.Enabled = True
    End Sub

    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        CurrentFinder = uscUDS.GetFinderModel()

        Dim SearchModel As UDSRepositorySearchModel = PopulateSearchModel()
        Dim DetailsSearchModel As UDSRepositoryDetailsSearchModel = PopulateDetailsSearchModel()
        Session("TempUDSRepositorySearchFilters") = SearchModel
        Session("TempUDSRepositoryDetailsSearchFilters") = DetailsSearchModel
        uscUDS.SaveDynamicFiltersToSession()
        Response.Redirect(String.Format(UDS_RESULTS_PAGE_URL, CopyToPEC))
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If

        If Session("TempUDSRepositoryDetailsSearchFilters") IsNot Nothing Then
            Dim DetailsSearchModel As UDSRepositoryDetailsSearchModel = CType(Session("TempUDSRepositoryDetailsSearchFilters"), UDSRepositoryDetailsSearchModel)
            Session("TempUDSRepositoryDetailsSearchFilters") = Nothing
            If DetailsSearchModel IsNot Nothing Then
                uscUDS.InitializeFilters(DetailsSearchModel)
            End If
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscUDS, btnSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, btnSearch)
    End Sub

    Private Sub Initialize()
        Title = PAGE_SEARCH_TITLE
        CurrentFinder = Nothing
        uscUDS.ActionType = uscUDS.ACTION_TYPE_SEARCH
        uscUDS.CurrentUDSRepositoryId = If(CurrentUDSRepositoryId.HasValue, CurrentUDSRepositoryId.Value, Nothing)
        uscUDS.CurrentUDSTypologyId = If(CurrentUDSTypologyId.HasValue, CurrentUDSTypologyId.Value, Nothing)
        uscUDS.CopyToPEC = CopyToPEC
        btnSearch.Enabled = False
    End Sub

    Public Function PopulateSearchModel() As UDSRepositorySearchModel
        Dim searchModel As UDSRepositorySearchModel = New UDSRepositorySearchModel()
        searchModel.TipologyId = uscUDS.DropDownListTypology.SelectedValue
        searchModel.UDSRepositoryId = uscUDS.DropDownListUDS.SelectedValue
        Return searchModel
    End Function

    Public Function PopulateDetailsSearchModel() As UDSRepositoryDetailsSearchModel
        Dim detailsSearchModel As UDSRepositoryDetailsSearchModel = New UDSRepositoryDetailsSearchModel()
        detailsSearchModel.Year = uscUDS.uscDataFinder.Year
        detailsSearchModel.Number = uscUDS.uscDataFinder.Number
        detailsSearchModel.DateFrom = uscUDS.uscDataFinder.RegistrationDateFrom
        detailsSearchModel.DateTo = uscUDS.uscDataFinder.RegistrationDateTo
        detailsSearchModel.Subject = uscUDS.uscDataFinder.Subject
        detailsSearchModel.CategoryId = If(uscUDS.uscDataFinder.IdCategory, 0)
        detailsSearchModel.IsCancelledArchive = uscUDS.uscDataFinder.ViewDeletedUDS
        Return detailsSearchModel
    End Function
#End Region

End Class