Imports System
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao



Public Class NHibernateDocumentDao
    Inherits BaseNHibernateDao(Of Document)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetCountByContainer(ByVal container As Container) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container", container))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)
    End Function

    Public Function GetCountByRole(ByVal role As Role) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Role", role))
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)
    End Function

    Public Function GetCountByCategory(ByVal Category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Category", Category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)
    End Function

    Public Function GetCountBySubCategory(ByVal Category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("SubCategory", Category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)
    End Function

    Public Function GetUserDocumentDiary(ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As ICollection(Of UserDiary)

        Dim qQuery As IQuery = NHibernateSession.GetNamedQuery("DocmUserDiary")

        qQuery.SetResultTransformer(Transformers.AliasToBean(New UserDiary().GetType()))
        qQuery = qQuery.SetParameter("SystemUser", DocSuiteContext.Current.User.FullUserName)
        qQuery = qQuery.SetParameter("LogDateFrom", pDateFrom.BeginOfTheDay().ToVecompSoftwareString())
        qQuery = qQuery.SetParameter("LogDateTo", pDateTo.EndOfTheDay().ToVecompSoftwareString())

        Return qQuery.List(Of UserDiary)()


    End Function

    Function GetDocumentProtocol(ByVal arrContacts As String) As IList(Of DocumentContact)
        Dim sqlQuery As String = " SELECT {dc.*} " & _
                                 " FROM DocumentContact {dc} " & _
                                 " INNER JOIN Contact as c on dc.IdContact = c.Incremental " & _
                                 " INNER JOIN Document as d on d.Year = dc.Year AND d.Number = dc.Number " & _
                                 " WHERE dc.IdContact IN (" & arrContacts & ")"
        Dim qry As ISQLQuery = NHibernateSession.CreateSQLQuery(sqlQuery)
        qry.AddEntity("dc", New DocumentContact().GetType())

        Return qry.List(Of DocumentContact)()
    End Function

    Function GetDocuments(ByVal keyList As IList(Of YearNumberCompositeKey)) As IList(Of Document)
        criteria = NHibernateSession.CreateCriteria(persitentType, "D")
        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateAlias("Category", "G", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateAlias("Container", "N", SqlCommand.JoinType.LeftOuterJoin)
        Dim disju As Disjunction = Expression.Disjunction()
        For Each key As YearNumberCompositeKey In keyList
            disju.Add(Expression.And(Restrictions.Eq("D.Year", key.Year), Restrictions.Eq("D.Number", key.Number)))
        Next
        criteria.Add(disju)

        Return criteria.List(Of Document)()
    End Function

    Public Function GetExpiryFolders(ByRef document As Document) As IList(Of DocumentFolder)
        ' TODO: spostare in NHibernateDocumuentFolderDao
        criteria = NHibernateSession.CreateCriteria(GetType(DocumentFolder), "DF")
        criteria.Add(Restrictions.Eq("Id.Year", document.Year))
        criteria.Add(Restrictions.Eq("Id.Number", document.Number))
        criteria.Add(Restrictions.IsNull("Role.Id"))
        criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("ExpiryDate", Date.Now())))

        Return criteria.List(Of DocumentFolder)()
    End Function

    Public Function GetConservationDocuments(ByVal ToDate As DateTime) As IList(Of Document)
        'criteria = NHibernateSession.CreateCriteria(persitentType, "P")
        'criteria.CreateAlias("Container", "Container", NHibernate.SqlCommand.JoinType.InnerJoin)
        'criteria.CreateAlias("Location", "Location", NHibernate.SqlCommand.JoinType.InnerJoin) ' TODO: Chiedere se usare la location del prot o del container

        'criteria.Add(Restrictions.Eq("ConservationStatus", "M"c))
        'criteria.Add(Restrictions.Eq("Container.Conservation", CType(1, Byte)))
        'criteria.Add(Expression.Not(Restrictions.Eq("P.IdDocument", 0))) ' TODO: Chiedere se va bene filtra + righe ma restano marcate come M
        'criteria.Add(Restrictions.Le("P.RegistrationDate", ToDate)) ' TODO: Chiedere date da includere
        '' TODO: Container.IsActive ?

        'Dim projList As ProjectionList = Projections.ProjectionList()
        'projList.Add(Projections.Property("P.Year"), "Year")
        'projList.Add(Projections.Property("P.Number"), "Number")
        'projList.Add(Projections.Property("P.RegistrationDate"), "RegistrationDate")
        'projList.Add(Projections.Property("P.ProtocolObject"), "ProtocolObject")
        'projList.Add(Projections.Property("P.Type"), "Type")
        'projList.Add(Projections.Property("P.Container"), "Container")
        'projList.Add(Projections.Property("P.Category"), "Category")
        'projList.Add(Projections.Property("P.IdDocument"), "IdDocument")
        'projList.Add(Projections.Property("P.IdAttachments"), "IdAttachments")
        'projList.Add(Projections.Property("P.Location"), "Location")
        'criteria.SetProjection(projList)
        ''criteria.SetResultTransformer(New NHibernate.Transform.AliasToBeanResultTransformer(GetType(ProtocolConservationHeader)))
        'criteria.SetResultTransformer(New NHibernate.Transform.AliasToBeanResultTransformer(GetType(Protocol)))

        'Return criteria.List(Of Protocol)()
        Return Nothing
    End Function

End Class
