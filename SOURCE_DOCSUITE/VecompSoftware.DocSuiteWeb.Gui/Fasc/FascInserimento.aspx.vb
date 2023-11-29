Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.MassimariScarto
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.MassimariScarto
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata

Partial Public Class FascInserimento
    Inherits FascBasePage

#Region " Fields "

    Dim _idCategory As Integer?
    Dim _protocolYear As Short?
    Dim _protocolNumber As Integer?
    Dim _currentCategory As Category
    Private Const FASCICLE_INSERT_CALLBACK As String = "fascInserimento.insertCallback('{0}');"
    Private Const INITIALIZE_CALLBACK As String = "fascInserimento.initializeCallback({0});"
    Private _currentMassimarioScartoFacade As MassimarioScartoFacade
    Private _idDocumentUnit As Guid?
    Private _environment As Integer?
    Private _currentIdUDSRepository As Guid?
    Private _contactFacade As ContactFacade

#End Region

#Region " Properties "
    Private ReadOnly Property IdCategory As Integer?
        Get
            If Not _idCategory.HasValue Then
                _idCategory = Request.QueryString.GetValueOrDefault(Of Integer?)("IdCategory", Nothing)
            End If
            Return _idCategory
        End Get
    End Property

    Public ReadOnly Property IdDocumentUnit As Guid?
        Get
            If Not _idDocumentUnit.HasValue Then
                _idDocumentUnit = Request.QueryString.GetValueOrDefault(Of Guid?)("IdDocumentUnit", Nothing)
            End If
            Return _idDocumentUnit
        End Get
    End Property

    Public ReadOnly Property Environment As Integer?
        Get
            If Not _environment.HasValue Then
                _environment = Request.QueryString.GetValueOrDefault(Of Integer?)("Environment", Nothing)
            End If
            Return _environment
        End Get
    End Property

    Public ReadOnly Property CurrentIdUDSRepository As Guid?
        Get
            If Not _currentIdUDSRepository.HasValue Then
                _currentIdUDSRepository = Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSRepository", Nothing)
            End If
            Return _currentIdUDSRepository
        End Get
    End Property

    Public ReadOnly Property FasciclesPanelVisibilities As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.FasciclesPanelVisibilities)
        End Get
    End Property

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return fasciclePageContent
        End Get
    End Property


#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack() Then
            uscFascicleInsert.PageContentDiv = PageContentDiv
        End If
        If IdCategory.HasValue Then
            Dim idsCategoryWithCategoryFascicle As IList(Of String) = CurrentCategoryFascicleFacade.GetFirstIdCategoryWithProcedureCategoryFascicle(IdCategory.Value)
            uscFascicleInsert.IdCategory = idsCategoryWithCategoryFascicle.FirstOrDefault()
        End If
    End Sub

    Protected Sub FascInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If
            Select Case ajaxModel.ActionName
                Case "Insert"
                    Dim contactId As Integer
                    If uscFascicleInsert.RespContactDTO IsNot Nothing AndAlso uscFascicleInsert.RespContactDTO.Contact IsNot Nothing Then
                        contactId = uscFascicleInsert.RespContactDTO.Contact.Id
                    End If

                    AjaxManager.ResponseScripts.Add(String.Format(FASCICLE_INSERT_CALLBACK, JsonConvert.SerializeObject(contactId)))
                    Exit Select
            End Select
        Catch
            Exit Sub
        End Try
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnInserimento)
    End Sub

#End Region

End Class

