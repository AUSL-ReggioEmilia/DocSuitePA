Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateResolutionJournalFinder
    Inherits NHibernateBaseFinder(Of ResolutionJournal, ResolutionJournal)


#Region " Constructors "

    Public Sub New(ByVal dbName As String)
        SessionFactoryName = dbName
    End Sub

#End Region

#Region " Properties "

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property Year As Nullable(Of Short)
    Public Property Template As ResolutionJournalTemplate
    Public Property IncludeDeleted As Boolean
    Public Property TemplateGroup As String

#End Region

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionJournal)("RJ")
        criteria.Add(Restrictions.Eq("RJ.IsActive", 1S))

        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("RJ.Year", Year))
        End If
        If Not Template Is Nothing Then
            criteria.Add(Restrictions.Eq("RJ.Template.Id", Template.Id))
        End If
        If Not String.IsNullOrEmpty(TemplateGroup) Then
            criteria.CreateAlias("RJ.Template", "Template")
            criteria.Add(Restrictions.Eq("Template.TemplateGroup", TemplateGroup))
        End If

        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of ResolutionJournal)
        Dim criteria As ICriteria = Me.CreateCriteria()
        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "RJ.Year", SortOrder.Descending)
            AttachSortExpressions(criteria, "RJ.Month", SortOrder.Descending)
        End If
        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
        Return criteria.List(Of ResolutionJournal)()
    End Function

End Class
