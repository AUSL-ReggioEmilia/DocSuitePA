Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.PECMails
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Limilabs.Mail.Appointments
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.PECMails
Imports VecompSoftware.DocSuiteWeb.Data.Entity.PECMails
Imports VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
Imports VecompSoftware.DocSuiteWeb.DTO.PECMails
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltCategorySchema
    Inherits CommonBasePage



#Region "Fields"
    Private _currentCategorySchema As CategorySchema
    Private _categorySchemaFinder As NHCategorySchemaFinder
#End Region

#Region "Properties"
    Private Property Version As Short
    Private Property StartDate As DateTimeOffset
    Private Property EndDate As DateTimeOffset?
    Private Property Note As String

    Private ReadOnly Property CategorySchemaFinder As NHCategorySchemaFinder
        Get
            If _categorySchemaFinder Is Nothing Then
                _categorySchemaFinder = New NHCategorySchemaFinder()
            End If
            Return _categorySchemaFinder
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub grdCategorySchema_ItemDataBound(ByVal sender As System.Object, ByVal e As GridItemEventArgs) Handles grdCategorySchema.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As CategorySchema = DirectCast(e.Item.DataItem, CategorySchema)

        If Not (item.EndDate Is Nothing OrElse item.EndDate.Value > DateTime.Today) Then
            e.Item.SelectableMode = GridItemSelectableMode.None
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblCategoryRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjax()
        If Not Page.IsPostBack Then
            InitializePage()
        End If
    End Sub
    Protected Sub TbltCategorySchema_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("ReloadSchemas") Then
            LoadCategorySchemas()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializePage()
        LoadCategorySchemas()
    End Sub

    Private Sub LoadCategorySchemas()
        grdCategorySchema.Finder = CategorySchemaFinder
        grdCategorySchema.DataBindFinder()
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltCategorySchema_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdCategorySchema, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(grdCategorySchema, grdCategorySchema, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
#End Region

End Class