Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class CollaborationRights

    Public Shared Function GetCollaborationProtocolEnabled(idTenantAOO As Guid) As Boolean
        If Not DocSuiteContext.Current.IsProtocolEnabled Then
            Return False
        ElseIf Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If

        Return Not FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, True, idTenantAOO).IsNullOrEmpty()
    End Function

    Public Shared Function GetCollaborationResolutionEnabled(idTenantAOO As Guid) As Boolean
        If Not DocSuiteContext.Current.IsResolutionEnabled Then
            Return False
        ElseIf Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If

        Return Not FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Resolution, ResolutionRoleRightPositions.Enabled, True, idTenantAOO).IsNullOrEmpty()
    End Function

    Public Shared Function GetCollaborationSeriesEnabled(idTenantAOO As Guid) As Boolean
        If Not DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
            Return False
        ElseIf Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If
        Return Not FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.DocumentSeries, DocumentSeriesRoleRightPositions.Enabled, True, idTenantAOO).IsNullOrEmpty()
    End Function

    Public Shared Function HasCollaborationRights(idTenantAOO As Guid) As Boolean
        Return GetCollaborationProtocolEnabled(idTenantAOO) OrElse GetCollaborationResolutionEnabled(idTenantAOO) OrElse GetCollaborationSeriesEnabled(idTenantAOO) OrElse DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible
    End Function

    Public Shared Function IsManager(idTenantAOO As Guid) As Boolean
        Dim highest As RoleUserType = FacadeFactory.Instance.RoleUserFacade.GetHighestUserType(idTenantAOO)
        Return highest = RoleUserType.D OrElse highest = RoleUserType.V
    End Function

    Public Shared Function GetIsCollaborationEnabled(idTenantAOO As Guid) As Boolean
        If Not DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled Then
            Return False
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible Then
            Return True
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If

        Return HasCollaborationRights(idTenantAOO)
    End Function

    Public Shared Function GetInserimentoAllaVisioneFirmaEnabled(idTenantAOO As Guid) As Boolean
        Return DocSuiteContext.Current.ProtocolEnv.InserimentoAllaVisioneFirmaEnabled _
            AndAlso HasCollaborationRights(idTenantAOO)
    End Function

    Public Shared Function GetInserimentoAlProtocolloSegreteriaEnabled(idTenantAOO As Guid) As Boolean
        ' Se sono Dirigente o Vice di collaborazione di almeno un settore aggiungo il punto di menù di inserimento alla segreteria.
        Return DocSuiteContext.Current.ProtocolEnv.InserimentoAlProtocolloSegreteriaEnabled _
            AndAlso HasCollaborationRights(idTenantAOO) AndAlso (IsManager(idTenantAOO) OrElse DocSuiteContext.Current.ProtocolEnv.MenuRightEnabled)
    End Function

End Class
