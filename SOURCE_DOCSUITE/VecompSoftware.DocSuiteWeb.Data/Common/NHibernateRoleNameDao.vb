Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform

Public Class NHibernateRoleNameDao
    Inherits BaseNHibernateDao(Of RoleName)



#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Overrides Sub Save(ByRef entity As RoleName)
        MyBase.Save(entity)
        NHibernateSession.SessionFactory.Evict(GetType(RoleName))
    End Sub

    Public Overrides Sub Update(ByRef entity As RoleName)
        MyBase.Update(entity)
        NHibernateSession.SessionFactory.Evict(GetType(RoleName))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As RoleName)
        MyBase.UpdateNoLastChange(entity)
        NHibernateSession.SessionFactory.Evict(GetType(RoleName))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As RoleName)
        MyBase.UpdateOnly(entity)
        NHibernateSession.SessionFactory.Evict(GetType(RoleName))
    End Sub

    Public Overrides Sub Delete(ByRef entity As RoleName)
        MyBase.Delete(entity)
        NHibernateSession.SessionFactory.Evict(GetType(RoleName))
    End Sub

    Public Function GetRoleNameByIdRole(ByVal IdRole As Integer) As RoleName

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")
        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.Add(Restrictions.IsNull("ToDate"))
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        Return criteria.UniqueResult(Of RoleName)

    End Function

    Public Function GetRoleNameByIdRoleActive(ByVal IdRole As Integer) As RoleName

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")
        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.IsNull("ToDate"))
        Return criteria.UniqueResult(Of RoleName)

    End Function

    Public Function GetRoleNameByIdRoleDate(ByVal IdRole As Integer) As RoleName

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")
        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.LeProperty("FromDate", DateTime.Now.ToString))
        criteria.Add(Restrictions.GeProperty("ToDate", DateTime.Now.ToString))
        Dim disju As New Disjunction
        disju.Add(Restrictions.IsNotNull("ToDate"))
        criteria.Add(disju)
        Return criteria.UniqueResult(Of RoleName)

    End Function

    Public Function GetRoleNamesByIdRole(ByVal IdRole As Integer) As IList(Of RoleName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        Return criteria.List(Of RoleName)()
    End Function


    Public Function GetRoleNamesByIds(ByVal idRoleList As Integer()) As IList(Of RoleName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("Id", idRoleList))
        Return criteria.List(Of RoleName)()
    End Function

    Public Function GetRoleNamesByIncrementalForDate(ByVal IdRole As Integer) As IList(Of RoleName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")
        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.AddOrder(New [Order]("FromDate", False))
        Return criteria.List(Of RoleName)()
    End Function

    ''' <summary>
    ''' Metodo per la ricerca di un oggetto RoleName per la data selezionata. 
    ''' </summary>
    ''' <param name="IdRole"></param>
    ''' <param name="SearchDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRoleNamesByValidDate(ByVal IdRole As Integer, ByVal SelectedDate As DateTime, tenantId As String) As RoleName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")

        Dim cj_l As Conjunction = New Conjunction()
        Dim cj_r As Conjunction = New Conjunction()

        cj_l.Add(Restrictions.IsNotNull("ToDate"))
        cj_l.Add(Restrictions.Le("FromDate", SelectedDate))
        cj_l.Add(Restrictions.Gt("ToDate", SelectedDate))

        cj_r.Add(Restrictions.IsNull("ToDate"))
        cj_r.Add(Restrictions.Le("FromDate", SelectedDate))
        criteria.Add(Restrictions.Or(cj_l, cj_r))

        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.Add(Restrictions.Eq("r.TenantId", Guid.Parse(tenantId)))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("FromDate"), "FromDate")
        proj.Add(Projections.Property("ToDate"), "ToDate")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of RoleName))
        Return criteria.UniqueResult(Of RoleName)()
    End Function

    ''' <summary>
    ''' Metodo per la ricerca di un oggetto RoleName per la data selezionata. Dove il ToDate è null
    ''' </summary>
    ''' <param name="IdRole"></param>
    ''' <param name="SelectedDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRoleNamesHistoryByValidDate(ByVal IdRole As Integer, ByVal SelectedDate As DateTime) As RoleName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")

        Dim disju As New Disjunction
        disju.Add(Restrictions.Ge("ToDate", SelectedDate))
        disju.Add(Restrictions.IsNull("ToDate"))
        criteria.Add(Restrictions.Le("FromDate", SelectedDate))
        criteria.Add(disju)

        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("FromDate"), "FromDate")
        proj.Add(Projections.Property("ToDate"), "ToDate")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of RoleName))
        Return criteria.UniqueResult(Of RoleName)()
    End Function


    ''' <summary>
    ''' Per un determinato contatto di rubrica, seleziono la storicizzazione più vecchia.
    ''' </summary>
    ''' <param name="IdRole"></param>
    ''' <returns></returns>
    Public Function GetRoleNamesOlder(ByVal IdRole As Integer) As RoleName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "r")

        criteria.Add(Restrictions.Eq("r.Id", IdRole))
        criteria.AddOrder(New [Order]("FromDate", False))
        criteria.Add(Restrictions.Eq("r.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.SetFirstResult(1)

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("FromDate"), "FromDate")
        proj.Add(Projections.Property("ToDate"), "ToDate")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of RoleName))
        Return criteria.UniqueResult(Of RoleName)()
    End Function

#End Region
End Class
