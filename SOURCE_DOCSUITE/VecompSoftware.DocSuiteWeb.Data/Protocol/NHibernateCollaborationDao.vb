Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateCollaborationDao
    Inherits BaseNHibernateDao(Of Collaboration)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetFDQDocuments(ByVal idCollList As List(Of Integer)) As IList(Of DocumentFDQDTO)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateCriteria("CollaborationSigns", "S").Add(Restrictions.Eq("S.IsActive", Convert.ToInt16(1)))
        criteria.Add(Expression.In("Id", idCollList))

        'Proiezioni
        Dim projList As ProjectionList = Projections.ProjectionList
        projList.Add(Projections.Property("Id"), "Collaboration")
        projList.Add(Projections.Property("CollaborationObject"), "Object")
        projList.Add(Projections.Property("Location.Id"), "IdLocation")
        projList.Add(Projections.SqlProjection("'Doc. Principale' AS DocumentType", New String() {"DocumentType"}, New NHibernate.Type.StringType() {NHibernateUtil.String}), "DocumentType")
        projList.Add(Projections.SqlProjection("0 AS Incremental", New String() {"Incremental"}, New NHibernate.Type.Int16Type() {NHibernateUtil.Int16}), "Incremental")

        Dim sqlIdDocument As String = "(SELECT TOP 1 IdDocument FROM CollaborationVersioning WHERE idCollaboration = {alias}.idCollaboration AND CollaborationIncremental = 0 ORDER BY Incremental DESC) as IdDocument"
        projList.Add(Projections.SqlProjection(sqlIdDocument, New String() {"IdDocument"}, New NHibernate.Type.Int32Type() {NHibernateUtil.Int32}))

        Dim sqlDocumentName As String = "(SELECT TOP 1 DocumentName FROM CollaborationVersioning WHERE idCollaboration = {alias}.idCollaboration AND CollaborationIncremental = 0 ORDER BY Incremental DESC) as DocumentName"
        projList.Add(Projections.SqlProjection(sqlDocumentName, New String() {"DocumentName"}, New NHibernate.Type.StringType() {NHibernateUtil.String}))

        criteria.SetProjection(projList)
        criteria.SetResultTransformer(New AliasToBeanResultTransformer(GetType(DocumentFDQDTO)))

        Return criteria.List(Of DocumentFDQDTO)()
    End Function

    Public Function GetFDQAttachments(ByVal idCollList As List(Of Integer)) As IList(Of DocumentFDQDTO)

        criteria = NHibernateSession.CreateCriteria(GetType(CollaborationVersioning), "CV")
        criteria.CreateAlias("CV.Collaboration", "C", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Expression.In("C.Id", idCollList))
        criteria.Add(Expression.Gt("CV.CollaborationIncremental", Convert.ToInt16(0)))
       

        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationVersioning))
        detach.CreateAlias("Collaboration", "C_CV", SqlCommand.JoinType.InnerJoin)
        detach.Add(Restrictions.EqProperty("C_CV.Id", "C.Id"))
        detach.Add(Restrictions.EqProperty("CollaborationIncremental", "CV.CollaborationIncremental"))
        detach.Add(Restrictions.Eq("IsActive", True))

        detach.SetProjection(Projections.Max("Incremental"))
        criteria.Add(Subqueries.PropertyEq("CV.Incremental", detach))

        'Proiezioni
        Dim projList As ProjectionList = Projections.ProjectionList
        projList.Add(Projections.Property("CV.Id"), "Collaboration")
        projList.Add(Projections.Property("CV.IdDocument"), "IdDocument")
        projList.Add(Projections.Property("CV.DocumentName"), "DocumentName")
        projList.Add(Projections.Property("CV.CollaborationIncremental"), "Incremental")
        projList.Add(Projections.SqlProjection("'Allegato' AS DocumentType", New String() {"DocumentType"}, New NHibernate.Type.StringType() {NHibernateUtil.String}), "DocumentType")

        criteria.SetProjection(projList)
        criteria.SetResultTransformer(New AliasToBeanResultTransformer(GetType(DocumentFDQDTO)))

        Return criteria.List(Of DocumentFDQDTO)()
    End Function

    ''' <summary> Ritorna la collaborazione più recente legata alla resolution. </summary>
    ''' <value>Nothing se non la trova. </value>
    ''' <remarks> Non dovrebbero esserci collaborazioni legate a più resolution. </remarks>
    Public Function GetByResolution(ByVal resolutionId As Integer) As Collaboration
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("Resolution.Id", resolutionId))
        crit.AddOrder(Order.Desc("Id"))
        crit.SetMaxResults(1)
        Return crit.UniqueResult(Of Collaboration)()
    End Function
    Public Function GetByAccount(username As String) As IList(Of Collaboration)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Like("RegistrationUser", String.Format("\{0}", username), MatchMode.End))
        criteria.Add(Restrictions.Not(Restrictions.Eq("IdStatus", CollaborationStatusType.PT.ToString())))

        Return criteria.List(Of Collaboration)()
    End Function

    Public Function GetByProtocol(year As Short, number As Integer) As Collaboration
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of Collaboration)()
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of Collaboration)()
    End Function

    Public Function GetByDocumentSeriesItem(idDocumentSeriesItem As Integer) As Collaboration
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of Collaboration)()
        criteria.Add(Restrictions.Eq("DocumentSeriesItem.Id", idDocumentSeriesItem))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of Collaboration)()
    End Function
    Public Function GetByIdDocumentUnit(idDocumentUnit As Guid) As Collaboration
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdDocumentUnit", idDocumentUnit))
        Return criteria.UniqueResult(Of Collaboration)()
    End Function
#End Region

End Class
