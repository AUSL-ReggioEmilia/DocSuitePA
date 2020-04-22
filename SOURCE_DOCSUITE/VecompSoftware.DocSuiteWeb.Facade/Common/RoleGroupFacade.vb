Imports System.Collections.Generic
Imports System.ComponentModel
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()>
Public Class RoleGroupFacade
    Inherits CommonFacade(Of RoleGroup, Guid, NHibernateRoleGroupDao)
    Implements IGroupFacade(Of RoleGroup)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

    Public Function DeleteGroup(ByRef obj As RoleGroup) As Boolean Implements IGroupFacade(Of RoleGroup).DeleteGroup
        Return Delete(obj)
    End Function

    ''' <summary> Torna la lista di <see>RoleGroup</see> inerente il <see>SecurityGroup</see> </summary>
    ''' <param name="securityGroup"> Gruppo da cercare </param>
    Public Function GetBySecurityGroup(ByRef securityGroup As SecurityGroups, Optional ordered As Boolean = False) As IList(Of RoleGroup)
        Return _dao.GetBySecurityGroup(securityGroup, ordered)
    End Function

    Public Function GetByRolesAndGroup(idRoles As Integer(), group As String) As IList(Of RoleGroup)
        Return _dao.GetByRolesAndGroup(idRoles, group)
    End Function

    ''' <summary> Torna la lista di <see>RoleGroup</see> di tutte le mailboxes <see>RoleGroup</see> </summary>
    Public Function GetByPecMailBoxes(ByVal pecMailBoxes As ICollection(Of PECMailBox)) As IList(Of String)
        Return _dao.GetByPecMailBoxes(pecMailBoxes)
    End Function

    ''' <summary>
    '''  Verifica se un determinato gruppo utenti associato ad un settore, è abilitato ad uno specifico dominio.
    ''' </summary>
    Public Function CheckGroupRights(group As RoleGroup, environment As DSWEnvironment) As Boolean
        Return _dao.CheckGroupRights(group, environment)
    End Function

    Public Function GetByRoleAndGroups(idRole As Integer, groups As String()) As IList(Of RoleGroup)
        Return _dao.GetByRoleAndGroups(idRole, groups)
    End Function

    Public Shared Function TraslitteraDiritti(environment As DSWEnvironment, group As RoleGroup) As String
        Dim codedRights As String = Nothing
        Dim enumType As Type = Nothing
        Select Case environment
            Case DSWEnvironment.Document
                codedRights = group.DocumentRights
                enumType = GetType(DocumentRoleRightPositions)
            Case DSWEnvironment.Protocol
                codedRights = group.ProtocolRightsString
                enumType = GetType(ProtocolRoleRightPositions)
            Case DSWEnvironment.Resolution
                codedRights = group.ResolutionRights
                enumType = GetType(ResolutionRoleRightPositions)
            Case DSWEnvironment.DocumentSeries
                codedRights = group.DocumentSeriesRights
                enumType = GetType(DocumentSeriesRoleRightPositions)
        End Select
        If String.IsNullOrEmpty(codedRights) OrElse enumType Is Nothing Then
            Return String.Empty
        End If


        Dim results As New List(Of String)
        For i As Integer = 1 To codedRights.Length
            If codedRights.Chars(i - 1) <> "1"c Then
                Continue For
            End If

            Dim enumObject As Object = [Enum].ToObject(enumType, i)
            If enumObject Is Nothing Then
                Continue For
            End If
            results.Add(EnumHelper.GetDescription(CType(enumObject, [Enum])))
        Next i
        Return String.Join(", ", results)
    End Function

    Public Overrides Sub Save(ByRef entity As RoleGroup)
        Dim gName As String = entity.Name
        Dim rId As Guid = entity.Role.UniqueId
        Dim documentR As String = entity.DocumentRights
        Dim protR As RoleProtocolRights = entity.ProtocolRights
        Dim resolutionR As String = entity.ResolutionRights
        Dim documentSerieR As String = entity.DocumentSeriesRights
        MyBase.Save(entity)
        FacadeFactory.Instance.TableLogFacade.Insert("RoleGroup", LogEvent.INS, String.Format("Inserito Gruppo {0} con DocumentRights {1}, ProtocolRights {2}, ResolutionRights {3}, DocumentSeriesRights {4}", gName, documentR, protR, resolutionR, documentSerieR), rId)
    End Sub
    Public Overrides Sub UpdateOnly(ByRef entity As RoleGroup)
        Dim gName As String = entity.Name
        Dim rId As Guid = entity.Role.UniqueId
        Dim documentR As String = entity.DocumentRights
        Dim protR As RoleProtocolRights = entity.ProtocolRights
        Dim resolutionR As String = entity.ResolutionRights
        Dim documentSerieR As String = entity.DocumentSeriesRights
        MyBase.UpdateOnly(entity)
        FacadeFactory.Instance.TableLogFacade.Insert("RoleGroup", LogEvent.UP, String.Format("Modificato Gruppo {0} con DocumentRights {1}, ProtocolRights {2}, ResolutionRights {3}, DocumentSeriesRights {4}", gName, documentR, protR, resolutionR, documentSerieR), rId)
    End Sub

    Public Function GetByRole(role As Role) As ICollection(Of RoleGroup)
        Return GetByRole(role.Id)
    End Function

    Public Function GetByRole(idRole As Integer) As ICollection(Of RoleGroup)
        Return _dao.GetByRole(idRole)
    End Function

    Public Shared Function InitializeNewInstanceFromExistingRoleGroup(roleGroup As RoleGroup, role As Role) As RoleGroup
        Dim newInstanceRoleGroup As RoleGroup = New RoleGroup With {
            .DocumentRights = roleGroup.DocumentRights,
            .DocumentSeriesRights = roleGroup.DocumentSeriesRights,
            .Name = roleGroup.Name,
            .ProtocolRights = roleGroup.ProtocolRights,
            .ResolutionRights = roleGroup.ResolutionRights,
            .SecurityGroup = roleGroup.SecurityGroup,
            .Role = role
        }
        Return newInstanceRoleGroup
    End Function
End Class
