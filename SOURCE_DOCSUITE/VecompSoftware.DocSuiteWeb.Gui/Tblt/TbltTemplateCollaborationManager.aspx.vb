Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltTemplateCollaborationManager
    Inherits UserBasePage

#Region "Fields"
    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder
    Private Const TEMPLATE_ACTIVE_IMAGE_PATH As String = "~/App_Themes/DocSuite2008/imgset16/detail_page_item_template_active.png"
    Private Const TEMPLATE_NOT_ACTIVE_IMAGE_PATH As String = "~/App_Themes/DocSuite2008/imgset16/detail_page_item_template_notactive.png"
    Private Const TEMPLATE_DRAFT_IMAGE_PATH As String = "~/App_Themes/DocSuite2008/imgset16/detail_page_item_template_draft.png"
    Private Const TEMPLATE_LOCKED_IMAGE_PATH As String = "~/Comm/Images/CheckOut.png"
    Private Const TEMPLATE_NOT_LOCKED_IMAGE_PATH As String = "~/Comm/Images/CheckIn.png"
    Private Const TEMPLATE_GESTIONE_URL As String = "~/User/TemplateUserCollGestione.aspx?Action=Edit&Type={0}&TemplateId={1}"
    Private Const RESL_DOCUMENT_TYPE As String = "resl"
    Private Const PROT_DOCUMENT_TYPE As String = "prot"
    Private Const SERIES_DOCUMENT_TYPE As String = "series"
#End Region

#Region "Properties"
    Private ReadOnly Property CurrentTemplateCollaborationFinder As TemplateCollaborationFinder
        Get
            If _currentTemplateCollaborationFinder Is Nothing Then
                Dim finder As TemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                finder.ResetDecoration()
                finder.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
                finder.SortExpressions.Add(New KeyValuePair(Of String, String)("Entity.Name", "ASC"))
                _currentTemplateCollaborationFinder = finder
            End If
            Return _currentTemplateCollaborationFinder
        End Get
    End Property

    Private ReadOnly Property ViewNotActive As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("ViewNotActive", False)
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub grdTemplateCollaboration_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdTemplateCollaboration.ItemDataBound
        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
            Dim combo As RadComboBox = DirectCast(filterItem.FindControl("cmbStatus"), RadComboBox)

            If grdTemplateCollaboration.Finder.FilterExpressions.Any(Function(x) x.Key.Eq("Entity.Status")) Then
                Dim control As Control = filterItem.FindControl("cmbStatus")
                Dim value As String = grdTemplateCollaboration.Finder.FilterExpressions("Entity.Status").FilterValue.ToString()
                FilterHelper.SetFilterValue(control, value)
            End If
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As WebAPIDto(Of TemplateCollaboration) = DirectCast(e.Item.DataItem, WebAPIDto(Of TemplateCollaboration))
        With DirectCast(e.Item.FindControl("lblNameLink"), HyperLink)
            .Text = boundHeader.Entity.Name
            .NavigateUrl = String.Format(TEMPLATE_GESTIONE_URL, CollaborationFacade.GetPageTypeFromDocumentType(boundHeader.Entity.DocumentType), boundHeader.Entity.UniqueId)
        End With

        With DirectCast(e.Item.FindControl("imgTemplateStatus"), Image)
            Select Case boundHeader.Entity.Status
                Case TemplateCollaborationStatus.Active
                    .ImageUrl = TEMPLATE_ACTIVE_IMAGE_PATH
                    .ToolTip = "Template pubblicato"
                Case TemplateCollaborationStatus.Draft
                    .ImageUrl = TEMPLATE_DRAFT_IMAGE_PATH
                    .ToolTip = "Template in bozza"
                Case TemplateCollaborationStatus.NotActive
                    .ImageUrl = TEMPLATE_NOT_ACTIVE_IMAGE_PATH
                    .ToolTip = "Template in errore"
            End Select
        End With
    End Sub

    Protected Sub cmbStatus_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim comboBox As RadComboBox = DirectCast(sender, RadComboBox)

        Dim filterItem As GridFilteringItem = DirectCast(comboBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        Dim filterType As Data.FilterExpression.FilterType = Data.FilterExpression.FilterType.NoFilter
        If (Not String.IsNullOrEmpty(comboBox.SelectedValue)) Then
            filterType = Data.FilterExpression.FilterType.IsEnum
        End If
        filters.Add(New Data.FilterExpression("Entity.Status", GetType(TemplateCollaborationStatus), comboBox.SelectedValue, filterType))
        filterItem.FireCommandEvent("CustomFilter", filters)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(grdTemplateCollaboration, grdTemplateCollaboration, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups) AndAlso Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Utente non abilitato alla gestione del template di Collaborazione")
        End If

        If ViewNotActive Then
            CurrentTemplateCollaborationFinder.FilterExpressions.Add("Entity.Status", New Data.FilterExpression("Entity.Status", GetType(TemplateCollaborationStatus), Convert.ToInt32(TemplateCollaborationStatus.NotActive), Data.FilterExpression.FilterType.IsEnum))
        End If
        grdTemplateCollaboration.PageSize = CurrentTemplateCollaborationFinder.PageSize
        grdTemplateCollaboration.Finder = CurrentTemplateCollaborationFinder
        grdTemplateCollaboration.DataBindFinder()
    End Sub
#End Region

End Class