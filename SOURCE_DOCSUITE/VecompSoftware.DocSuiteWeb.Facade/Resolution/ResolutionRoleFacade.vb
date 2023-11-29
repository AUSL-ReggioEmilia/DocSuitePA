Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ResolutionRoleFacade
    Inherits BaseResolutionFacade(Of ResolutionRole, ResolutionRoleCompositeKey, NHibernateResolutionRoleDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function CheckRight(resolution As Resolution, rightPosition As ResolutionRightPositions, active As Boolean?) As Boolean
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.CheckRightBySG(resolution, groups.Select(Function(g) g.Id).ToList(), rightPosition, active)
    End Function

    ' Verifica se il diritto richiesto è stato revocato
    Public Function CheckRestrictedRight(resolution As Resolution, rightPosition As ResolutionRightPositions, active As Boolean?) As Boolean?
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.RestrictedRightBySG(resolution, groups.Select(Function(g) g.Id).ToList(), rightPosition, active)
    End Function

    ''' <summary> Inserisce il settore tra quelli autorizzati nella Resolution </summary>
    ''' <returns>ResolutionRole creato o recuperato.</returns>
    ''' <remarks>Se la relazione esiste restituisce l'elemento del DB</remarks>
    Public Function AddRole(resl As Resolution, idRole As Integer, idResolutionRoleType As Integer, Optional needTransaction As Boolean = True) As ResolutionRole

        Dim id As New ResolutionRoleCompositeKey()
        id.IdResolution = resl.Id
        id.IdRole = idRole
        id.IdResolutionRoleType = idResolutionRoleType

        Dim rr As ResolutionRole = GetById(id)
        If rr Is Nothing Then
            rr = New ResolutionRole()
            rr.Id = id
            rr.UniqueIdResolution = resl.UniqueId
            Save(rr, _dbName, needTransaction)
        Else
            Update(rr, _dbName, needTransaction)
        End If

        Factory.ResolutionLogFacade.Log(resl, ResolutionLogType.RM, String.Format("Aggiunta autorizzazione [{0},{1}]", idRole, idResolutionRoleType), needTransaction)

        Return rr
    End Function

End Class