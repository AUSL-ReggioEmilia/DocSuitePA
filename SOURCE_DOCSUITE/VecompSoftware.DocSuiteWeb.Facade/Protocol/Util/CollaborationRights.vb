Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class CollaborationRights

    Public Shared Function GetCollaborationProtocolEnabled() As Boolean
        If Not DocSuiteContext.Current.IsProtocolEnabled Then
            Return False
        ElseIf Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If

        Return Not FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, True).IsNullOrEmpty()
    End Function

    Public Shared Function GetCollaborationResolutionEnabled() As Boolean
        If Not DocSuiteContext.Current.IsResolutionEnabled Then
            Return False
        ElseIf Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If

        Return Not FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Resolution, ResolutionRoleRightPositions.Enabled, True).IsNullOrEmpty()
    End Function

    Public Shared Function GetCollaborationSeriesEnabled() As Boolean
        If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
            Return True
        End If
        Return Not FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.DocumentSeries, ResolutionRoleRightPositions.Enabled, True).IsNullOrEmpty()
    End Function

    Public Shared Function HasCollaborationRights() As Boolean
        Return GetCollaborationProtocolEnabled() OrElse GetCollaborationResolutionEnabled() OrElse DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible
    End Function

    Public Shared Function IsManager() As Boolean
        Dim highest As RoleUserType = FacadeFactory.Instance.RoleUserFacade.GetHighestUserType()
        Return highest = RoleUserType.D OrElse highest = RoleUserType.V
    End Function

    Public Shared Function GetIsCollaborationEnabled() As Boolean
        If Not DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled Then
            Return False
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible Then
            Return True
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return True
        End If

        Return HasCollaborationRights()
    End Function

    Public Shared Function GetInserimentoAllaVisioneFirmaEnabled() As Boolean
        Return DocSuiteContext.Current.ProtocolEnv.InserimentoAllaVisioneFirmaEnabled _
            AndAlso HasCollaborationRights()
    End Function

    Public Shared Function GetInserimentoAlProtocolloSegreteriaEnabled() As Boolean
        ' Se sono Dirigente o Vice di collaborazione di almeno un settore aggiungo il punto di menù di inserimento alla segreteria.
        Return DocSuiteContext.Current.ProtocolEnv.InserimentoAlProtocolloSegreteriaEnabled _
            AndAlso HasCollaborationRights() _
            AndAlso IsManager()
    End Function

End Class
