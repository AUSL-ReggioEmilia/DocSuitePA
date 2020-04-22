Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform

Public Class NHibernateContactNameDao
    Inherits BaseNHibernateDao(Of ContactName)



#Region " Constructors "
    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Overrides Sub Save(ByRef entity As ContactName)
        MyBase.Save(entity)
        NHibernateSession.SessionFactory.Evict(GetType(ContactName))
    End Sub

    Public Overrides Sub Update(ByRef entity As ContactName)
        MyBase.Update(entity)
        NHibernateSession.SessionFactory.Evict(GetType(ContactName))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As ContactName)
        MyBase.UpdateNoLastChange(entity)
        NHibernateSession.SessionFactory.Evict(GetType(ContactName))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As ContactName)
        MyBase.UpdateOnly(entity)
        NHibernateSession.SessionFactory.Evict(GetType(ContactName))
    End Sub

    Public Overrides Sub Delete(ByRef entity As ContactName)
        MyBase.Delete(entity)
        NHibernateSession.SessionFactory.Evict(GetType(ContactName))
    End Sub

    Public Function GetContactNameByIncremental(ByVal Incremental As Integer) As ContactName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Contact", "c")
        criteria.Add(Restrictions.Eq("c.Id", Incremental))
        criteria.Add(Restrictions.IsNull("ToDate"))
        Return criteria.UniqueResult(Of ContactName)()
    End Function

    Public Function GetContactNamesByIncrementalForDate(ByVal Incremental As Integer) As IList(Of ContactName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Contact", "c")
        criteria.Add(Restrictions.Eq("c.Id", Incremental))

        Return criteria.List(Of ContactName)()
    End Function


    Public Function GetContactNamesByIncremental(ByVal Incremental As Integer) As IList(Of ContactName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Contact", "c")
        criteria.Add(Restrictions.Eq("c.Id", Incremental))
        Return criteria.List(Of ContactName)()
    End Function


    Public Function GetContactNamesByValidDate(ByVal Incremental As Integer, ByVal SelectedDate As DateTime) As ContactName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Contact", "c")

        Dim cj_l As Conjunction = New Conjunction()
        Dim cj_r As Conjunction = New Conjunction()

        cj_l.Add(Restrictions.IsNotNull("ToDate"))
        cj_l.Add(Restrictions.Le("FromDate", SelectedDate))
        cj_l.Add(Restrictions.Gt("ToDate", SelectedDate))

        cj_r.Add(Restrictions.IsNull("ToDate"))
        cj_r.Add(Restrictions.Le("FromDate", SelectedDate))
        criteria.Add(Restrictions.Or(cj_l, cj_r))

        criteria.Add(Restrictions.Eq("c.Id", Incremental))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("FromDate"), "FromDate")
        proj.Add(Projections.Property("ToDate"), "ToDate")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ContactName))
        Return criteria.UniqueResult(Of ContactName)()
    End Function

    Public Function GetContactNamesHistoryByValidDate(ByVal Incremental As Integer, ByVal SelectedDate As DateTime) As ContactName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Contact", "c")

        Dim disju As New Disjunction
        disju.Add(Restrictions.Ge("ToDate", SelectedDate))
        disju.Add(Restrictions.IsNull("ToDate"))
        criteria.Add(Restrictions.Le("FromDate", SelectedDate))
        criteria.Add(disju)

        criteria.Add(Restrictions.Eq("c.Id", Incremental))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("FromDate"), "FromDate")
        proj.Add(Projections.Property("ToDate"), "ToDate")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ContactName))
        Return criteria.UniqueResult(Of ContactName)()
    End Function

    ''' <summary>
    ''' Per un determinato contatto di rubrica, seleziono la storicizzazione più vecchia.
    ''' </summary>
    ''' <param name="Incremental"></param>
    ''' <returns></returns>
    Public Function GetContactNamesOlder(ByVal Incremental As Integer) As ContactName
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Contact", "c")

        criteria.Add(Restrictions.Eq("c.Id", Incremental))
        criteria.AddOrder(New [Order]("FromDate", False))
        criteria.SetFirstResult(1)

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("FromDate"), "FromDate")
        proj.Add(Projections.Property("ToDate"), "ToDate")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ContactName))
        Return criteria.UniqueResult(Of ContactName)()
    End Function

#End Region

End Class
