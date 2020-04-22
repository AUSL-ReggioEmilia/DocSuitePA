Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.NHibernateManager
Imports NHibernate.SqlCommand
Imports VecompSoftware.DocSuiteWeb.Data

Public Class NHibernateOChartFinder
    Inherits NHibernateBaseFinder(Of OChart, OChart)

#Region " Constants "

#End Region

#Region " Properties "
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        End Get
    End Property

    Public Property IdIn As IEnumerable(Of Guid)
    Public Property IsEnabled As Boolean?
    Public Property PreviousOf As DateTime?
    Public Property FollowingOf As DateTime?
    Public Property EffectivenessLowerBound As Boolean

#End Region

#Region " Methods "

    Public Overloads Overrides Function DoSearch() As IList(Of OChart)
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of OChart)()
    End Function

    Public Overridable Function ListHierarchies(Optional session As ISession = Nothing) As List(Of OChart)

        If session Is Nothing Then
            session = NHibernateSession()
        End If

        Dim hierarchies As List(Of OChart)

        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        hierarchies = criteria.List(Of OChart)().ToList()
        'non necessario in quanto con il mapping (inverse = true') il caricamento è già gerarchico
        'hierarchies.ForEach(Sub(h) h.RebuildHierarchy())

        Return hierarchies

    End Function

    Private Sub DecorateIdIn(criteria As ICriteria)
        If IdIn.IsNullOrEmpty() Then
            Return
        End If

        If IdIn.HasSingle() Then
            criteria.Add(Restrictions.Eq("Id", IdIn.First()))
            Return
        End If

        criteria.Add(Restrictions.In("Id", IdIn.ToArray()))
    End Sub

    Private Sub DecorateIsEnabled(criteria As ICriteria)
        If Not IsEnabled.HasValue Then
            Return
        End If

        If IsEnabled Then
            criteria.Add(Restrictions.Eq("Enabled", True))
            Return
        End If

        criteria.Add(Restrictions.Or(Restrictions.Eq("Enabled", False), Restrictions.IsNull("Enabled")))
    End Sub

    Private Sub DecorateEffectivenessLowerBound(criteria As ICriteria)
        If Not EffectivenessLowerBound Then
            Return
        End If

        Dim dsj_l As Conjunction = New Conjunction()
        Dim dsj_r As Conjunction = New Conjunction()

        dsj_l.Add(Restrictions.IsNotNull("EndDate"))
        dsj_l.Add(Restrictions.Le("StartDate", DateTime.Now))
        dsj_l.Add(Restrictions.Gt("EndDate", DateTime.Now))

        dsj_r.Add(Restrictions.IsNull("EndDate"))
        dsj_r.Add(Restrictions.Le("StartDate", DateTime.Now))

        criteria.Add(Restrictions.Eq("Enabled", True))
        criteria.Add(Restrictions.Or(dsj_l, dsj_r))
    End Sub

    Private Sub DecoratePreviousOf(criteria As ICriteria)
        If Not PreviousOf.HasValue OrElse EffectivenessLowerBound Then
            Return
        End If

        Dim dsj_l As Conjunction = New Conjunction()
        Dim dsj_r As Conjunction = New Conjunction()

        dsj_l.Add(Restrictions.IsNotNull("EndDate"))
        dsj_l.Add(Restrictions.Le("StartDate", PreviousOf.Value))
        dsj_l.Add(Restrictions.Gt("EndDate", PreviousOf.Value))

        dsj_r.Add(Restrictions.IsNull("EndDate"))
        dsj_r.Add(Restrictions.Le("StartDate", PreviousOf.Value))

        criteria.Add(Restrictions.Or(dsj_l, dsj_r))
    End Sub

    Private Sub DecorateFollowingOf(criteria As ICriteria)
        If Not FollowingOf.HasValue OrElse EffectivenessLowerBound Then
            Return
        End If
        criteria.Add(Restrictions.Ge("StartDate", FollowingOf.Value))
        criteria.AddOrder(Order.Asc("StartDate"))
    End Sub

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "O")
        DecorateIdIn(criteria)
        DecorateIsEnabled(criteria)
        DecorateEffectivenessLowerBound(criteria)
        DecoratePreviousOf(criteria)
        DecorateFollowingOf(criteria)
        Return criteria
    End Function

#End Region

End Class
