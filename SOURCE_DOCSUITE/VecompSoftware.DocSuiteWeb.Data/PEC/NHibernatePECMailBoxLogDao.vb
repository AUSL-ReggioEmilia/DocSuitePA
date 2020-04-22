Imports NHibernate.Criterion
Imports NHibernate
Imports System.Linq
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Transform

Public Class NHibernatePECMailBoxLogDao
    Inherits BaseNHibernateDao(Of PECMailBoxLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetLogItem(ByVal description As String, ByVal type As String) As IList(Of PECMailBoxLog)
        Dim query As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        query.Add(Restrictions.Eq("Description", description))
        query.Add(Restrictions.Eq("Type", type))
        Return query.List(Of PECMailBoxLog)()
    End Function

    Public Function GetLastRecord(ByVal pecMailBoxId As Short) As Date?
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("MailBox.Id", Convert.ToInt16(pecMailBoxId)))
        crit.SetProjection(Projections.Max("Date"))
        Try
            Return New Date?(crit.List(Of DateTime).FirstOrDefault())
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function GetLastRecords() As IList(Of PECMailBoxReportDateDto)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Max("Date"), "Date")
        proj.Add(Projections.GroupProperty("MailBox.Id"), "Id")
        crit.SetProjection(proj)
        crit.SetResultTransformer(Transformers.AliasToBean(Of PECMailBoxReportDateDto))
        Return crit.List(Of PECMailBoxReportDateDto)()

    End Function

    Public Function GetLastRecordWithoutError(ByVal pecMailBoxId As Short) As Date?

        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType, "S")
        Dim detach As DetachedCriteria = DetachedCriteria.For(persitentType, "P")

        crit.Add(Restrictions.Eq("S.MailBox.Id", Convert.ToInt16(pecMailBoxId)))
        crit.Add(Restrictions.Eq("S.Type", "TimeEval"))

        detach.Add(Restrictions.Eq("P.MailBox.Id", Convert.ToInt16(pecMailBoxId)))
        detach.Add(Restrictions.Eq("P.Type", "ErrorEval"))
        detach.Add(Restrictions.EqProperty("P.Date", "S.Date"))
        detach.SetProjection(Projections.Id())

        crit.Add(Subqueries.NotExists(detach))
        crit.SetProjection(Projections.Max("Date"))
        Try
            Return New Date?(crit.List(Of DateTime).FirstOrDefault())
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function GetLastRecordsWithoutError() As IList(Of PECMailBoxReportDateDto)

        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType, "S")
        Dim detach As DetachedCriteria = DetachedCriteria.For(persitentType, "P")

        crit.Add(Restrictions.Eq("S.Type", "TimeEval"))

        detach.Add(Restrictions.EqProperty("P.MailBox.Id", "S.MailBox.Id"))
        detach.Add(Restrictions.Eq("P.Type", "ErrorEval"))
        detach.Add(Restrictions.EqProperty("P.Date", "S.Date"))
        detach.SetProjection(Projections.Id())

        crit.Add(Subqueries.NotExists(detach))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Max("S.Date"), "Date")
        proj.Add(Projections.GroupProperty("S.MailBox.Id"), "Id")
        crit.SetProjection(proj)
        crit.SetResultTransformer(Transformers.AliasToBean(Of PECMailBoxReportDateDto))
        Return crit.List(Of PECMailBoxReportDateDto)()
    End Function
End Class
