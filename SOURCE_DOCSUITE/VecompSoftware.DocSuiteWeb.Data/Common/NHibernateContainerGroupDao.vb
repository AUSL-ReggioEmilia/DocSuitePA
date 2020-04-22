Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateContainerGroupDao
    Inherits BaseNHibernateDao(Of ContainerGroup)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Torna la lista di <see>ContainerGroup</see> inerente il <see>SecurityGroup</see>
    ''' </summary>
    ''' <param name="securityGroup">Gruppo da cercare</param>
    Public Function GetBySecurityGroup(ByRef securityGroup As SecurityGroups, Optional ordered As Boolean = False) As IList(Of ContainerGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("SecurityGroup.Id", securityGroup.Id))

        If ordered Then
            criteria.CreateAlias("Container", "C", SqlCommand.JoinType.InnerJoin)
            criteria.AddOrder(Order.Asc("C.Name"))
        End If

        Return criteria.List(Of ContainerGroup)()
    End Function

    Public Function GetByIdContainer(ByVal idContainer As Integer) As IList(Of ContainerGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        Return criteria.List(Of ContainerGroup)()
    End Function

    Public Function GetByContainerAndName(idContainer As Integer, groupName As String) As ContainerGroup
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        criteria.Add(Restrictions.Eq("Name", groupName))
        Return criteria.UniqueResult(Of ContainerGroup)()
    End Function

    Public Function GetByContainerAndGroups(idContainer As Integer, groups As String()) As IList(Of ContainerGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        criteria.Add(Restrictions.In("Name", groups))
        Return criteria.List(Of ContainerGroup)()
    End Function

    Public Function GetByContainersAndGroup(idContainers As Integer(), group As String) As IList(Of ContainerGroup)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("Container.Id", idContainers))
        criteria.Add(Restrictions.Eq("Name", group))
        Return criteria.List(Of ContainerGroup)()
    End Function

    Public Function GetMaxPrivacyLevel(idContainer As Integer, domain As String, account As String) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        criteria.CreateAlias("SecurityGroup", "SG")
        criteria.CreateAlias("SG.SecurityUsers", "SU")
        criteria.Add(Restrictions.Eq("SU.UserDomain", domain))
        criteria.Add(Restrictions.Eq("SU.Account", account))

        criteria.SetProjection(Projections.ProjectionList.Add(Projections.Max("PrivacyLevel")))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function HasContainerRight(idContainer As Integer, domain As String, account As String, right As Integer, env As DSWEnvironment) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        Dim pattern As String = "1".PadLeft(right, "_"c)
        criteria.Add(Restrictions.Like("_protocolRights", pattern, MatchMode.Start))
        Select Case env
            Case DSWEnvironment.Protocol
                criteria.Add(Restrictions.Like("_protocolRights", pattern, MatchMode.Start))
            Case DSWEnvironment.Resolution
                criteria.Add(Restrictions.Like("_resolutionRights", pattern, MatchMode.Start))
            Case DSWEnvironment.Document
                criteria.Add(Restrictions.Like("_documentRights", pattern, MatchMode.Start))
            Case DSWEnvironment.DocumentSeries
                criteria.Add(Restrictions.Like("DocumentSeriesRights", pattern, MatchMode.Start))
            Case DSWEnvironment.Desk
                criteria.Add(Restrictions.Like("DeskRights", pattern, MatchMode.Start))
            Case DSWEnvironment.UDS
                criteria.Add(Restrictions.Like("UDSRights", pattern, MatchMode.Start))
            Case Else
                Throw New InvalidOperationException("È necessario specificare un DSWEnvironment valido se si valorizza rightPosition.")
        End Select

        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        criteria.CreateAlias("SecurityGroup", "SG")
        criteria.CreateAlias("SG.SecurityUsers", "SU")
        criteria.Add(Restrictions.Eq("SU.UserDomain", domain))
        criteria.Add(Restrictions.Eq("SU.Account", account))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)() > 0
    End Function

End Class
