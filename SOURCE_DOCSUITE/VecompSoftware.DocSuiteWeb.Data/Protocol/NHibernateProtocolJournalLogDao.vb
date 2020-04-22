Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolJournalLogDao
    Inherits BaseNHibernateDao(Of ProtocolJournalLog)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DBName As String)
        MyBase.New(DBName)
    End Sub


    Public Function GetLastLogDate() As Date?
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolJournalLog)()
        criteria.SetMaxResults(1)
        criteria.SetProjection(Projections.Max("ProtocolJournalDate"))
        Return criteria.UniqueResult(Of DateTime)()
    End Function

    ''' <summary>
    ''' Elimina l'associazione protocolli/registro giornaliero per una specifica data di registrazione.
    ''' </summary>
    ''' <param name="registrationDate"></param>
    ''' <remarks></remarks>
    Public Sub ClearProtocolJournalReferencesByDate(registrationDate As DateTimeOffset)
        Dim sql As String = "update Protocol p set p.JournalDate=null, p.JournalLog.Id=null where p.RegistrationDate between :LOWERLIMIT and :UPPERLIMIT"
        Dim query As IQuery = NHibernateSession.CreateQuery(sql)
        Dim lowerLimit As DateTime = registrationDate.Date
        query.SetDateTimeOffset("LOWERLIMIT", lowerLimit)
        Dim upperLimit As DateTime = lowerLimit.AddDays(1).AddSeconds(-1)
        query.SetDateTimeOffset("UPPERLIMIT", upperLimit)

        query.ExecuteUpdate()
    End Sub


    Public Function GetUnfinishedLogDates(lowerDateLimit As Date?, upperDateLimit As Date?) As IList(Of DateTime)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolJournalLog)()
        criteria.Add(Restrictions.IsNull("EndDate"))

        Dim lower As Date? = lowerDateLimit
        Dim upper As Date? = upperDateLimit
        ' Se gli estremi non sono coerenti eseguo l'inversione
        If lower.HasValue AndAlso upper.HasValue AndAlso upper < lower Then
            lower = upperDateLimit
            upper = lowerDateLimit
        End If
        If lower.HasValue AndAlso upper.HasValue Then
            criteria.Add(Restrictions.Between("ProtocolJournalDate", lower, upper))
        ElseIf lower.HasValue Then
            criteria.Add(Restrictions.Ge("ProtocolJournalDate", lower))
        ElseIf upper.HasValue Then
            criteria.Add(Restrictions.Le("ProtocolJournalDate", upper))
        End If

        criteria.SetProjection(Projections.Distinct(Projections.Property("ProtocolJournalDate")))
        criteria.AddOrder(Order.Asc("ProtocolJournalDate"))

        Return criteria.List(Of DateTime)()
    End Function

    ''' <summary>
    ''' Ritorna l'elenco dei registri di protocollo incompleti nel periodo specificato.
    ''' </summary>
    ''' <param name="lowerLimit">Data di limite inferiore</param>
    ''' <param name="upperLimit">Data di limite superiore</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBrokenJournals(lowerLimit As Date?, upperLimit As Date?) As IList(Of ProtocolJournalLog)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolJournalLog)()
        criteria.Add(Restrictions.IsNull("EndDate"))

        Dim lower As Date? = lowerLimit
        Dim upper As Date? = upperLimit
        ' Se gli estremi non sono coerenti eseguo l'inversione
        If lower.HasValue AndAlso upper.HasValue AndAlso upper < lower Then
            lower = upperLimit
            upper = lowerLimit
        End If
        If lower.HasValue AndAlso upper.HasValue Then
            criteria.Add(Restrictions.Between("ProtocolJournalDate", lower, upper))
        ElseIf lower.HasValue Then
            criteria.Add(Restrictions.Ge("ProtocolJournalDate", lower))
        ElseIf upper.HasValue Then
            criteria.Add(Restrictions.Le("ProtocolJournalDate", upper))
        End If
        criteria.AddOrder(Order.Asc("ProtocolJournalDate"))

        Return criteria.List(Of ProtocolJournalLog)()
    End Function

End Class
