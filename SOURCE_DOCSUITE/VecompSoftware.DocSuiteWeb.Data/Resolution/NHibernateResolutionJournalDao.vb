Imports System
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateResolutionJournalDao
    Inherits BaseNHibernateDao(Of ResolutionJournal)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByYear(year As Integer) As IList(Of ResolutionJournal)
        Dim criteria As ICriteria = ActiveCriteria()
        criteria.Add(Restrictions.Eq("IsActive", True))
        criteria.Add(Restrictions.Eq("Year", year))
        Return criteria.List(Of ResolutionJournal)()
    End Function

    ''' <summary>
    ''' Ritorna tutte le resolution journal attive
    ''' </summary>
    ''' <returns></returns>
    Public Function GetActive() As IList(Of ResolutionJournal)
        Dim criteria As ICriteria = ActiveCriteria()
        criteria.Add(Restrictions.Eq("IsActive", True))
        Return criteria.List(Of ResolutionJournal)()
    End Function


    ''' <summary> Recupero l'ultimo Registro salvato per il Template indicato. </summary>
    Public Function GetLast(template As ResolutionJournalTemplate) As ResolutionJournal

        Dim criteria As ICriteria = ActiveCriteria()
        criteria.SetMaxResults(1) ' Solo il primo
        criteria.Add(Restrictions.Eq("IsActive", True))
        criteria.Add(Restrictions.Eq("Template.Id", template.Id))
        criteria.AddOrder(Order.Desc("Year")).AddOrder(Order.Desc("Month"))

        Return criteria.UniqueResult(Of ResolutionJournal)()
    End Function

    Public Function GetLastBeforeYearAndMonth(template As ResolutionJournalTemplate, year As Short, month As Integer) As ResolutionJournal

        Dim criteria As ICriteria = ActiveCriteria()
        criteria.SetMaxResults(1) ' Solo il primo
        criteria.Add(Restrictions.Eq("IsActive", True))
        criteria.Add(Restrictions.Eq("Template.Id", template.Id))
        criteria.Add(Restrictions.Le("Year", year))
        criteria.Add(Restrictions.Le("Month", month))
        criteria.AddOrder(Order.Desc("Year")).AddOrder(Order.Desc("Month"))

        Return criteria.UniqueResult(Of ResolutionJournal)()
    End Function

    ''' <summary>
    ''' Crea il registro sequenzialmente successivo all'ultimo presente nel Template indicato
    ''' </summary>
    Public Function BuildNext(template As ResolutionJournalTemplate) As ResolutionJournal
        Dim resolutionJournal As New ResolutionJournal()
        resolutionJournal.IsActive = True
        ' prendo l'ultimo registro
        Dim lastJournal As ResolutionJournal = Nothing
        If template IsNot Nothing Then
            lastJournal = GetLast(template)
        End If

        If lastJournal IsNot Nothing Then
            ' Creo il registro per il mese successivo
            Dim [date] As New DateTime(lastJournal.Year, lastJournal.Month, 1)
            [date] = [date].AddMonths(1)
            resolutionJournal.Year = [date].Year
            resolutionJournal.Month = [date].Month

            If template.Pagination.GetValueOrDefault(False) Then
                Dim lastPage As Integer = lastJournal.LastPage
                ' Se è il nuovo anno riparto con le pagine
                If [date].Month = 1 Then
                    lastPage = 0
                End If
                resolutionJournal.FirstPage = lastPage + 1
            End If
        Else
            ' Creo il primo registro
            resolutionJournal.Year = 2011
            resolutionJournal.Month = 1
            resolutionJournal.FirstPage = 1
        End If

        Return resolutionJournal
    End Function

    Private Function ActiveCriteria() As ICriteria
        Return NHibernateSession.CreateCriteria(persitentType)
    End Function

End Class
