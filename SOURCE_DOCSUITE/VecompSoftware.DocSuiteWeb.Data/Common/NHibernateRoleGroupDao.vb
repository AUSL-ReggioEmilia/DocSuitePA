Imports VecompSoftware.NHibernateManager.Dao
Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateRoleGroupDao
    Inherits BaseNHibernateDao(Of RoleGroup)

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Overrides Sub Save(ByRef entity As RoleGroup)
        MyBase.Save(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(RoleGroup))
    End Sub

    Public Overrides Sub Update(ByRef entity As RoleGroup)
        MyBase.Update(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(RoleGroup))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As RoleGroup)
        MyBase.UpdateNoLastChange(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(RoleGroup))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As RoleGroup)
        MyBase.UpdateOnly(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(RoleGroup))
    End Sub

    Public Overrides Sub Delete(ByRef entity As RoleGroup)
        MyBase.Delete(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(RoleGroup))
    End Sub

    ''' <summary>
    ''' Torna la lista di <see>RoleGroup</see> inerente il <see>SecurityGroup</see>
    ''' </summary>
    ''' <param name="securityGroup">Gruppo da cercare</param>
    Public Function GetBySecurityGroup(ByRef securityGroup As SecurityGroups, Optional ordered As Boolean = False) As IList(Of RoleGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("SecurityGroup.Id", securityGroup.Id))
        If ordered Then
            criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
            criteria.AddOrder(Order.Asc("R.Name"))
        End If
        Return criteria.List(Of RoleGroup)()
    End Function

    Public Function GetByRolesAndGroup(idRoles As Integer(), group As String) As IList(Of RoleGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("Role.Id", idRoles))
        criteria.Add(Restrictions.Eq("Name", group))
        Return criteria.List(Of RoleGroup)()
    End Function

    Public Function GetByPecMailBoxes(ByVal pecMailBoxes As ICollection(Of PECMailBox)) As IList(Of String)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("R.Mailboxes", "PMBR", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.In("PMBR.Id", pecMailBoxes.Select(Function(m) m.Id).ToArray()))
        criteria.SetProjection(Projections.Distinct(Projections.Property("Name")))

        Return criteria.List(Of String)()
    End Function

    ''' <summary>
    ''' Verifica se un determinato gruppo utenti associato ad un settore, è abilitato ad uno specifico dominio.
    ''' </summary>
    ''' <param name="group">gruppo utenti da verificare</param>
    ''' <param name="environment">dominio</param>
    ''' <returns></returns>
    Public Function CheckGroupRights(group As RoleGroup, environment As DSWEnvironment) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("Id", group.Id))

        Select Case environment
            Case DSWEnvironment.Protocol
                criteria.Add(Restrictions.Not(Restrictions.Eq("_protocolRights", GroupRights.EmptyRights)))
                criteria.Add(Restrictions.IsNotNull("_protocolRights"))
            Case DSWEnvironment.Resolution
                criteria.Add(Restrictions.Not(Restrictions.Eq("_resolutionRights", GroupRights.EmptyRights)))
                criteria.Add(Restrictions.IsNotNull("_resolutionRights"))
            Case DSWEnvironment.Document
                criteria.Add(Restrictions.Not(Restrictions.Eq("_documentRights", GroupRights.EmptyRights)))
                criteria.Add(Restrictions.IsNotNull("_documentRights"))
            Case DSWEnvironment.DocumentSeries
                criteria.Add(Restrictions.Not(Restrictions.Eq("DocumentSeriesRights", GroupRights.EmptyRights)))
                criteria.Add(Restrictions.IsNotNull("DocumentSeriesRights"))
            Case Else
                Throw New InvalidOperationException("È necessario specificare un DSWEnvironment valido.")
        End Select

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.CountDistinct("Name"))
        criteria.SetProjection(proj)
        Dim result As Integer = criteria.UniqueResult(Of Integer)()
        Return result > 0
    End Function

    Public Function GetByRoleAndGroups(idRole As Integer, groups As String()) As IList(Of RoleGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Role.Id", idRole))
        criteria.Add(Restrictions.In("Name", groups))
        Return criteria.List(Of RoleGroup)()
    End Function

    Public Function GetByRole(idRole As Integer) As IList(Of RoleGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Role.Id", idRole))
        Return criteria.List(Of RoleGroup)()
    End Function
End Class
