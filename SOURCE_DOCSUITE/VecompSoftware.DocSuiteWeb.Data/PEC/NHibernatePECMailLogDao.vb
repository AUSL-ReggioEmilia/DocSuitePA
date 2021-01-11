Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECMailLogDao
    Inherits BaseNHibernateDao(Of PECMailLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByPEC(pec As PECMail) As IList(Of PECMailLog)
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of PECMailLog)()
        criteria.SetMaxResults(1)
        criteria.Add(Restrictions.Eq("Mail.Id", pec.Id))
        criteria.AddOrder(Order.Desc("Id"))
        Return criteria.List(Of PECMailLog)()
    End Function


    Public Function GetLogTypeByPEC(pec As PECMail, logType As PECMailLogType) As PECMailLog
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of PECMailLog)()
        criteria.SetMaxResults(1)
        criteria.Add(Restrictions.Eq("Mail.Id", pec.Id))
        criteria.Add(Restrictions.Eq("Type", logType.ToString))
        criteria.AddOrder(Order.Desc("Date"))
        Return criteria.UniqueResult(Of PECMailLog)()
    End Function


End Class
