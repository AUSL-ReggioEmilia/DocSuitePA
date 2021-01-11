Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class ProtBasePage
    Inherits CommonBasePage

#Region " Constants "

    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Prot"

#End Region

#Region " Fields "

    Private _currentProtocol As Protocol
    Private _currentProtocolRights As ProtocolRights
    Private _currentProtocolRightsStatusCancel As ProtocolRights
    Private _facade As FacadeFactory
    Private _currentDocumentUnitChain As IList(Of Entity.DocumentUnits.DocumentUnitChain) = Nothing
    Private _currentFascicleDocumentUnits As IList(Of FascicleDocumentUnit) = Nothing
    Private _currentHasContainerRight As Boolean?
    Private _currentProtocolId As Guid?

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

    Public ReadOnly Property CurrentProtocolId As Guid
        Get
            If Not _currentProtocolId.HasValue Then
                _currentProtocolId = GetKeyValue(Of Guid)("UniqueId")
            End If
            Return _currentProtocolId.Value
        End Get
    End Property

    Public ReadOnly Property CurrentProtocol() As Protocol
        Get
            If _currentProtocol Is Nothing Then
                _currentProtocol = Facade.ProtocolFacade.GetById(CurrentProtocolId)
            End If
            Return _currentProtocol
        End Get
    End Property
    Public ReadOnly Property CurrentDocumentUnitChains As IList(Of Entity.DocumentUnits.DocumentUnitChain)
        Get
            If _currentDocumentUnitChain Is Nothing AndAlso CurrentProtocol IsNot Nothing Then
                Dim result As ICollection(Of WebAPIDto(Of DocumentUnitChain)) = WebAPIImpersonatorFacade.ImpersonateFinder(New DocumentUnitChainFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.IdDocumentUnit = CurrentProtocol.Id
                        finder.EnablePaging = False
                        finder.ExpandProperties = False
                        Return finder.DoSearch()
                    End Function)

                _currentDocumentUnitChain = result.Select(Function(f) f.Entity).ToList()
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

            If Not GetValueOrDefault(Of Guid?)("UniqueId", Nothing).HasValue Then
                Throw New DocSuiteException("Protocollo", String.Concat("Impossibile accedere alla funzionalità specifica del protocollo. ", ProtocolEnv.DefaultErrorMessage))
            End If

            Dim qs As String = $"UniqueId={CurrentProtocolId}&Type=Prot"
            Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck(qs)}")
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
