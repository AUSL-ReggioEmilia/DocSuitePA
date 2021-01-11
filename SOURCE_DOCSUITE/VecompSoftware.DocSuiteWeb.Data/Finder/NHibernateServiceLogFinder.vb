Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateServiceLogFinder
    Inherits NHibernateBaseFinder(Of ServiceLog, ServiceLog)

#Region "Fields"
    Private _dateFrom As Date?
    Private _dateTo As Date?
    Private _session As String
    Private _text As String
    Private _level As Nullable(Of Short)
#End Region

#Region "Properties"
    Public Property DateFrom() As Date?
        Get
            Return _dateFrom
        End Get
        Set(ByVal value As Date?)
            _dateFrom = value
        End Set
    End Property

    Public Property DateTo() As Date?
        Get
            Return _dateTo
        End Get
        Set(ByVal value As Date?)
            _dateTo = value
        End Set
    End Property

    Public Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
        End Set
    End Property

    Public Property Session() As String
        Get
            Return _session
        End Get
        Set(ByVal value As String)
            _session = value
        End Set
    End Property

    Public Property Level() As Nullable(Of Short)
        Get
            Return _level
        End Get
        Set(ByVal value As Nullable(Of Short))
            _level = value
        End Set
    End Property
#End Region

#Region "Constuctor"
    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
    End Sub
#End Region
    
#Region " NHibernate Properties "
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property
#End Region

#Region "Criteria"
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType)

        'Data da
        If DateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("DateTime", _dateFrom))
        End If

        'Data a
        If DateTo.HasValue Then
            criteria.Add(Restrictions.Le("DateTime", _dateTo))
        End If

        'Sessione
        If Not String.IsNullOrEmpty(Session) Then
            criteria.Add(Expression.Like("Session", _session, MatchMode.Anywhere))
        End If

        'Descrizione
        If Not String.IsNullOrEmpty(Text) Then
            criteria.Add(Restrictions.Ge("Text", _text))
        End If

        'Livello
        If Level.HasValue Then
            criteria.Add(Restrictions.Eq("Level", _level))
        End If

        'Aggancia filtri
        AttachFilterExpressions(criteria)

        Return criteria
    End Function
#End Region
    
#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As IList(Of ServiceLog)
        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "DateTime", SortOrder.Ascending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of ServiceLog)()
    End Function
#End Region

#Region "CountAll"
    Public Function CountAll() As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType)
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function
#End Region

End Class
