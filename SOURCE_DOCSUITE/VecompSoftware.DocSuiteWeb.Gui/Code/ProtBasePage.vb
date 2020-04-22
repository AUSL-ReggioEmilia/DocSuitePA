Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits

Public Class ProtBasePage
    Inherits CommonBasePage

#Region " Constants "

    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Prot"

#End Region

#Region " Fields "

    Private _currentProtocolYear As Short?

    Private _currentProtocolNumber As Integer?

    Private _currentProtocol As Protocol

    Private _currentProtocolRights As ProtocolRights

    Private _currentProtocolRightsStatusCancel As ProtocolRights

    Private _facade As FacadeFactory

    Private _workflowOperation As Boolean?

    Private _idWorkflowActivity As Guid?

    Private _currentDocumentUnitChain As IList(Of DocumentUnitChain) = Nothing

    Private _currentFascicleDocumentUnits As IList(Of FascicleDocumentUnit) = Nothing

    Private _currentWorkflowActivityFacade As WorkflowActivityFacade = Nothing

    Private _currentWorkflowInstanceFacade As WorkflowInstanceFacade = Nothing

    Private _currentHasContainerRight As Boolean?

#End Region

#Region " Properties "

    Public Overrides ReadOnly Property Facade() As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory("ProtDB")
            End If
            Return _facade
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowInstanceFacade As WorkflowInstanceFacade
        Get
            If _currentWorkflowInstanceFacade Is Nothing Then
                _currentWorkflowInstanceFacade = New WorkflowInstanceFacade(DocSuiteContext.Current.User.FullUserName)
            End If

            Return _currentWorkflowInstanceFacade
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowActivityFacade As WorkflowActivityFacade
        Get
            If _currentWorkflowActivityFacade Is Nothing Then
                _currentWorkflowActivityFacade = New WorkflowActivityFacade(DocSuiteContext.Current.User.FullUserName)
            End If

            Return _currentWorkflowActivityFacade
        End Get
    End Property

    Public ReadOnly Property CurrentProtocolYear As Short
        Get
            If Not _currentProtocolYear.HasValue Then
                _currentProtocolYear = GetKeyValue(Of Short)("Year")
            End If
            Return _currentProtocolYear.Value
        End Get
    End Property

    Public ReadOnly Property CurrentProtocolNumber As Integer
        Get
            If Not _currentProtocolNumber.HasValue Then
                _currentProtocolNumber = GetKeyValue(Of Integer)("Number")
            End If
            Return _currentProtocolNumber.Value
        End Get
    End Property

    Public ReadOnly Property CurrentProtocol() As Protocol
        Get
            If _currentProtocol Is Nothing Then
                _currentProtocol = Facade.ProtocolFacade.GetById(CurrentProtocolYear, CurrentProtocolNumber, False)
            End If
            Return _currentProtocol
        End Get
    End Property
    Public ReadOnly Property CurrentDocumentUnitChains As IList(Of DocumentUnitChain)
        Get
            If _currentDocumentUnitChain Is Nothing AndAlso CurrentProtocol IsNot Nothing Then
                Dim documentUnitChainFinder As DocumentUnitChainFinder = New DocumentUnitChainFinder(DocSuiteContext.Current.CurrentTenant)
                documentUnitChainFinder.IdDocumentUnit = CurrentProtocol.UniqueId
                documentUnitChainFinder.EnablePaging = False
                documentUnitChainFinder.ExpandProperties = False
                _currentDocumentUnitChain = documentUnitChainFinder.DoSearch().Select(Function(f) f.Entity).ToList()
            End If

            Return _currentDocumentUnitChain
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleDocumentUnits As IList(Of FascicleDocumentUnit)
        Get
            If _currentFascicleDocumentUnits Is Nothing Then
                _currentFascicleDocumentUnits = Facade.FascicleDocumentUnitFacade.GetByProtocol(CurrentProtocol)
            End If
            Return _currentFascicleDocumentUnits
        End Get
    End Property

    Public Overloads ReadOnly Property CurrentProtocolRights As ProtocolRights
        Get
            If _currentProtocolRights Is Nothing Then
                _currentProtocolRights = New ProtocolRights(CurrentProtocol)
                If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                    _currentProtocolRights.ContainerRightDictionary = CommonShared.UserContainerRightDictionary
                    _currentProtocolRights.RoleRightDictionary = CommonShared.UserRoleRightDictionary
                End If
            End If
            Return _currentProtocolRights
        End Get
    End Property

    Public Overloads ReadOnly Property CurrentProtocolRightsStatusCancel As ProtocolRights
        Get
            If _currentProtocolRightsStatusCancel Is Nothing Then
                _currentProtocolRightsStatusCancel = New ProtocolRights(CurrentProtocol, True)
            End If
            Return _currentProtocolRightsStatusCancel
        End Get
    End Property

    Protected ReadOnly Property IsWorkflowOperation() As Boolean
        Get
            If Not _workflowOperation.HasValue Then
                _workflowOperation = Request.QueryString.GetValueOrDefault("IsWorkflowOperation", False)
            End If
            Return _workflowOperation.Value
        End Get
    End Property

    Protected ReadOnly Property CurrentIdWorkflowActivity As Guid?
        Get
            If _idWorkflowActivity Is Nothing Then
                _idWorkflowActivity = GetKeyValue(Of Guid?)("IdWorkflowActivity")
            End If
            Return _idWorkflowActivity
        End Get
    End Property


    Protected ReadOnly Property CurrentHasContainerRight As Boolean
        Get
            If Not _currentHasContainerRight.HasValue Then
                _currentHasContainerRight = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)
            End If
            Return _currentHasContainerRight.Value
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Type Is Nothing AndAlso Not Type.Eq("Prot") Then
            FileLogger.Error(LoggerName, String.Concat("ProtBasePage without Prot Type : ", Request.Url))

            If Not GetValueOrDefault(Of Integer?)("Number", Nothing).HasValue OrElse Not GetValueOrDefault(Of Integer?)("Year", Nothing).HasValue Then
                Throw New DocSuiteException("Protocollo", String.Concat("Impossibile accedere alla funzionalità specifica del protocollo. ", ProtocolEnv.DefaultErrorMessage))
            End If

            Dim s As String = String.Format("Year={0}&Number={1}&Type=Prot", CurrentProtocolYear, CurrentProtocolNumber)
            Response.Redirect("~/Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(s))
        End If
    End Sub

#End Region

#Region "Methods"

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, ProtBasePage)(key)
    End Function

    Public Overloads Function GetValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function


    Public Sub ReloadCurrentProtocolState()
        Me._currentProtocol = Nothing
        Me._currentProtocolNumber = Nothing
        Me._currentProtocolYear = Nothing
        Me._currentProtocolRights = Nothing
        Me._currentProtocolRightsStatusCancel = Nothing
    End Sub
    Public Sub InitializeDocumentLabels(uploaddocumentsLabels As IList(Of Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)))
        For Each uploaddocumentLabels As Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType) In uploaddocumentsLabels
            uploaddocumentLabels.Item1.Caption = ProtocolEnv.DocumentUnitLabels(uploaddocumentLabels.Item2)
            uploaddocumentLabels.Item1.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(uploaddocumentLabels.Item2)
        Next
    End Sub
#End Region

End Class
