Imports NHibernate
Imports NHibernate.Criterion
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionJournalTemplateDao
    Inherits BaseNHibernateDao(Of ResolutionJournalTemplate)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetTemplatesByGroup(templateGroup As String, enabledOnly As Boolean?) As IList(Of ResolutionJournalTemplate)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionJournalTemplate)("RJT")
        If Not String.IsNullOrEmpty(templateGroup) Then
            criteria.Add(Restrictions.Eq("TemplateGroup", templateGroup))
        End If
        If enabledOnly.GetValueOrDefault(False) Then
            criteria.Add(Restrictions.Eq("IsEnabled", True))
        End If
        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of ResolutionJournalTemplate)()
    End Function

End Class
