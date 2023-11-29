Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()>
Public Class PECMailBoxRoleFacade
    Inherits BaseProtocolFacade(Of PECMailBoxRole, PecMailBoxRoleCompositeKey, NHibernatePECMailBoxRoleDao)

    ''' <summary>
    ''' Verifica che l'operatore passato appartenga al settore passato per parametro
    ''' </summary>
    ''' <param name="role">Settore da verificare</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CurrentUserBelongsPecRole(role As Role) As Boolean
        Dim authorize As Boolean = CurrentUserBelongsPecRole(New List(Of Role) From {role})
        Return authorize
    End Function

    Public Function CurrentUserBelongsPecRole(roles As IList(Of Role)) As Boolean
        If roles.IsNullOrEmpty() Then
            Return False
        End If

        Dim targetRoles As IList(Of Role) = FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, 3, True, roles.First().IdTenantAOO)

        Dim belongs As Boolean = False
        If Not targetRoles.IsNullOrEmpty() Then
            belongs = roles.Any(Function(role) targetRoles.Contains(role))
        End If
        Return belongs
    End Function
End Class
