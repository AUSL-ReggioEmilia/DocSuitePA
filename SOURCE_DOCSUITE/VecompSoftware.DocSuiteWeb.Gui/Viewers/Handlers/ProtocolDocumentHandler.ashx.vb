Imports System.Web
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Web.SessionState
Imports System.Collections.Generic

Namespace Viewers.Handlers

    Public Class ProtocolDocumentHandler
        Inherits DocumentHandler
        Implements IRequiresSessionState
        Private _currentProtocol As Protocol

        Private ReadOnly Property CurrentProtocol As Protocol
            Get
                If _currentProtocol Is Nothing AndAlso UniqueId.HasValue Then
                    _currentProtocol = FacadeFactory.ProtocolFacade.GetById(UniqueId.Value)
                End If
                Return _currentProtocol
            End Get
        End Property

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            _currentHttpContext = context
            If DocSuiteContext.Current.PrivacyEnabled Then
                IdContainer = CurrentProtocol.Container.Id
                If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                    PrivacyRoles = CurrentProtocol.Roles.Where(Function(t) Not String.IsNullOrEmpty(t.Type) AndAlso t.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.UniqueId).ToArray()
                    PublicRoles = CurrentProtocol.Roles.Where(Function(t) String.IsNullOrEmpty(t.Type) OrElse Not t.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.UniqueId).ToArray()
                End If
            End If
            'TODO: abilitare quando il controllo dei diritti verrà fatto tramite la sql function delle document unit
            'CheckDocumentUnitExistence = True
            ElaborateDocument(context)
        End Sub

        Protected Overrides Function CheckRight() As Boolean
            Dim right As New ProtocolRights(CurrentProtocol, DocSuiteContext.Current.ProtocolEnv.ProtocolDocumentHandlerStatusCancel)
            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                right.ContainerRightDictionary = CommonShared.UserContainerRightDictionary
                right.RoleRightDictionary = CommonShared.UserRoleRightDictionary
            End If
            IsUserAuthorized = right.IsUserAuthorized
            Return right.IsReadable OrElse right.IsHighilightViewable
        End Function

        Protected Overrides Function CheckPrivacyRight() As Boolean
            Dim right As New ProtocolRights(CurrentProtocol, DocSuiteContext.Current.ProtocolEnv.ProtocolDocumentHandlerStatusCancel)
            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                right.ContainerRightDictionary = CommonShared.UserContainerRightDictionary
                right.RoleRightDictionary = CommonShared.UserRoleRightDictionary
            End If
            Return right.IsDocumentReadable
        End Function

        Protected Overrides Sub LogView(documentName As String, guid As Guid)
            Dim message As String = String.Format("Visualizzato documento ""{0}"" [{1}]", documentName, guid.ToString("N"))
            If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso CurrentProtocol.Roles.Count > 0 Then
                Dim resultMessage As Boolean = False
                If CurrentProtocol.Roles.Any(Function(t) Not String.IsNullOrEmpty(t.Type) AndAlso t.Type.Equals(ProtocolRoleTypes.Privacy)) Then
                    Dim privacyRoles As IList(Of Role) = FacadeFactory.RoleFacade.GetPrivacyUserRoles(CurrentProtocol.Roles.Where(Function(t) Not String.IsNullOrEmpty(t.Type) AndAlso t.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.UniqueId).ToArray())
                    If privacyRoles.Count > 0 Then
                        Dim roleDescription As String = String.Join(", ", privacyRoles.Select(Function(r) r.Name))
                        message = String.Concat(message, " - utente autorizzato al trattamento privacy ", If(privacyRoles.Count = 1, "nel settore [", "nei settori ["), roleDescription, "]")
                        resultMessage = True
                    End If
                End If
                If Not resultMessage AndAlso CurrentProtocol.Roles.Any(Function(t) String.IsNullOrEmpty(t.Type) OrElse Not t.Type.Equals(ProtocolRoleTypes.Privacy)) Then
                    Dim userRoles As IList(Of Role) = FacadeFactory.RoleFacade.GetUserRolesByIds(CurrentProtocol.Roles.Where(Function(t) String.IsNullOrEmpty(t.Type) OrElse Not t.Type.Equals(ProtocolRoleTypes.Privacy)).Select(Function(r) r.Role.UniqueId).ToArray(),
                                                                                                DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled)
                    If userRoles.Count > 0 Then
                        Dim roleDescription As String = String.Join(", ", userRoles.Select(Function(r) r.Name))
                        message = String.Concat(message, " - utente appartenente ", If(userRoles.Count = 1, "al settore [", "ai settori ["), roleDescription, "]")
                    End If
                End If
            End If
            FacadeFactory.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PD, message)
        End Sub

    End Class

End Namespace