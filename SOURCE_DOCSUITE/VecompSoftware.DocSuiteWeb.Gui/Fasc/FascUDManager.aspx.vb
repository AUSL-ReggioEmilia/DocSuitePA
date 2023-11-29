Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons

Public Class FascUDManager
    Inherits FascBasePage

#Region "Fields"
    Private _currentUDUniqueId As Guid?
    Private _currentCategoryId As Integer?
    Private _currentUDRepositoryName As String
    Private _currentIdUDSRepository As Guid?
    Private Const CHANGE_CATEGORY_CALLBACK As String = "fascUDManager.changeCategoryCallback('{0}','{1}');"
    Private Const ERROR_CALLBACK As String = "fascUDManager.errorCallback('{0}');"
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentUDUniqueId As Guid
        Get
            If Not _currentUDUniqueId.HasValue Then
                _currentUDUniqueId = Request.QueryString.GetValue(Of Guid?)("UniqueId")
            End If
            Return _currentUDUniqueId.Value
        End Get
    End Property

    Public ReadOnly Property CurrentCategoryId As Integer?
        Get
            If Not _currentCategoryId.HasValue Then
                _currentCategoryId = Request.QueryString.GetValue(Of Integer?)("CategoryId")
            End If
            Return _currentCategoryId
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

    Protected ReadOnly Property ValidationModel As String
        Get
            Dim category As Category = Nothing
            If ProtocolEnv.IsDistributionEnabled AndAlso UDTypeId.Equals(DSWEnvironment.Protocol) Then
                Dim protocol As Protocol = Facade.ProtocolFacade.GetById(CurrentUDUniqueId)
                category = protocol.Category
            Else
                category = Facade.CategoryFacade.GetById(CurrentCategoryId.Value)
            End If

            Return SerializeValidationModel(category)
        End Get
    End Property

    'TODO: questa property deve contenere il tipo della UD da gestire recuperato da QueryString
    Protected ReadOnly Property UDTypeId As Integer
        Get
            Return Request.QueryString.GetValue(Of Integer)("UDType")
        End Get
    End Property

    Protected ReadOnly Property SignalRAddress As String
        Get
            Return DocSuiteContext.Current.CurrentTenant.SignalRAddress
        End Get
    End Property

    Public ReadOnly Property CurrentUDRepositoryName As String
        Get
            If String.IsNullOrEmpty(_currentUDRepositoryName) Then
                _currentUDRepositoryName = Request.QueryString.GetValueOrDefault(Of String)("UDSRepositoryName", String.Empty)
            End If
            Return _currentUDRepositoryName
        End Get
    End Property

    Public ReadOnly Property FolderSelectionEnabled As Boolean
        Get
            Return GetKeyValueOrDefault("FolderSelectionEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CategoryFullIncrementalPath As String
        Get
            Return Request.QueryString.Get("CategoryFullIncrementalPath")
        End Get
    End Property

    Public ReadOnly Property FascicleObject As String
        Get
            Return Request.QueryString.Get("FascicleObject")
        End Get
    End Property

    Public ReadOnly Property AuthorizedFasciclesEnabled As Boolean
        Get
            Return GetKeyValueOrDefault("AuthorizedFasciclesEnabled", False)
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        uscFascicleSearch.DefaultCategoryId = CurrentCategoryId
        uscFascicleSearch.CategoryFullIncrementalPath = CategoryFullIncrementalPath
        uscFascicleSearch.FascicleObject = FascicleObject
        uscFascicleSearch.DSWEnvironment = UDTypeId
        If Not IsPostBack Then
            uscFascicleSearch.FolderSelectionEnabled = FolderSelectionEnabled
            rgvAssociatedFascicles.DataSource = New List(Of String)()
            availableFascicleRow.Visible = CurrentODataFacade.HasViewableRight(CurrentUDUniqueId, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
        End If
    End Sub

    Private Sub FascUDManager_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument IsNot Nothing Then
            Dim ajaxModel As AjaxModel = Nothing
            Try
                ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            Catch
            End Try

            If ajaxModel Is Nothing Then
                Return
            End If

            Dim categoryId As Integer = Nothing
            If ProtocolEnv.IsDistributionEnabled AndAlso ajaxModel.ActionName.Equals("ChangeCategory") AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Any() AndAlso ajaxModel.Value(0) IsNot Nothing AndAlso Integer.TryParse(ajaxModel.Value(0), categoryId) Then
                Try
                    Dim protocol As Protocol = Facade.ProtocolFacade.GetById(CurrentUDUniqueId)
                    Dim category As Category = Facade.CategoryFacade.GetById(categoryId, False)
                    protocol.Category = category
                    Facade.ProtocolFacade.Update(protocol)
                    Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, String.Concat("Modifica classificatore: ", category.Id, "(", category.Name, ") da Gestione fascicolo"))
                    Facade.ProtocolFacade.SendUpdateProtocolCommand(protocol)
                    AjaxManager.ResponseScripts.Add(String.Format(CHANGE_CATEGORY_CALLBACK, category.Name, SerializeValidationModel(category)))
                Catch ex As Exception
                    FileLogger.Error(LoggerName, String.Concat("Errore in fase di modifica di classificatore: ", ex.Message), ex)
                    AjaxManager.ResponseScripts.Add(String.Format(ERROR_CALLBACK, "Si è verificato un errore in fase di modifica del classificatore di protocollo."))
                End Try
            End If

        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascUDManager_AjaxRequest
    End Sub

    Private Function SerializeValidationModel(category As Category) As String
        Dim firstCategoryWithCategoryFascicle As String() = New CategoryFascicleFacade().GetFirstIdCategoryWithProcedureCategoryFascicle(category.Id).ToArray
        Dim canManageFascicle As Boolean = CurrentODataFacade.HasViewableRight(CurrentUDUniqueId, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
        Dim canInsertFascicle As Boolean = CurrentODataFacade.HasFascicleInsertRight(FascicleType.Procedure)
        Dim canChangeCategory As Boolean = False
        If ProtocolEnv.IsDistributionEnabled AndAlso UDTypeId.Equals(DSWEnvironment.Protocol) Then
            Dim protocol As Protocol = Facade.ProtocolFacade.GetById(CurrentUDUniqueId)
            If protocol IsNot Nothing Then
                Dim categoryFascicleFacade As CategoryFascicleFacade = New CategoryFascicleFacade()
                Dim protocolrights As ProtocolRights = New ProtocolRights(protocol, True)
                canChangeCategory = protocolrights.IsCurrentUserDistributionManager.HasValue AndAlso protocolrights.IsCurrentUserDistributionManager.Value AndAlso
                    CurrentODataFacade.HasProcedureDistributionInsertRight(category.Id)
            End If
        End If

        Return JsonConvert.SerializeObject(New With {.CanManageFascicle = canManageFascicle, .CanInsertFascicle = canInsertFascicle, .CanChangeCategory = canChangeCategory})
    End Function
#End Region

End Class