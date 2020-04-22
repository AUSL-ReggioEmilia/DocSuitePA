Imports NHibernate
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateProtocolJournalLogFinder
    Inherits NHibernateBaseFinder(Of ProtocolJournalLog, ProtocolJournalLog)

#Region "Enum"
    Public Enum ProtocolErrorType
        [Nothing] = 0
        Errors = 1
        Corrects = 2
    End Enum
#End Region

#Region "Private Fields"
    Private _startDate As Nullable(Of Date)
    Private _endDate As Nullable(Of Date)
    Private _checkErrors As ProtocolErrorType = ProtocolErrorType.Nothing
    Private _checkEmpty As Boolean = False
#End Region

#Region "Public Properties"
    Public Property StartDate() As Nullable(Of Date)
        Get
            Return _startDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _startDate = value
        End Set
    End Property

    Public Property EndDate() As Nullable(Of Date)
        Get
            Return _endDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _endDate = value
        End Set
    End Property

    Public Property ErrorType() As ProtocolErrorType
        Get
            Return _checkErrors
        End Get
        Set(ByVal value As ProtocolErrorType)
            _checkErrors = value
        End Set
    End Property

    Public Property CheckEmpty() As Boolean
        Get
            Return _checkEmpty
        End Get
        Set(ByVal value As Boolean)
            _checkEmpty = value
        End Set
    End Property
#End Region

#Region "IFinder Implementation"
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType)

        criteria.CreateAlias("Location", "Location", SqlCommand.JoinType.LeftOuterJoin)

        If StartDate.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("ProtocolJournalDate", StartDate.Value)))
        End If

        If EndDate.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("ProtocolJournalDate", EndDate.Value)))
        End If

        If ErrorType <> ProtocolErrorType.Nothing Then
            Select Case ErrorType
                Case ProtocolErrorType.Corrects
                    criteria.Add(Restrictions.Eq("ProtocolError", 0))
                Case ProtocolErrorType.Errors
                    criteria.Add(Expression.Not(Restrictions.Eq("ProtocolError", 0)))
            End Select
        End If

        If Not CheckEmpty Then
            criteria.Add(Expression.Not(Restrictions.Eq("ProtocolTotal", 0)))
        End If

        AttachFilterExpressions(criteria)
        AttachSQLExpressions(criteria)

        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As System.Collections.Generic.IList(Of ProtocolJournalLog)
        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "LogDate", SortOrder.Ascending)
            'criteria.AddOrder(Order.Asc("LogDate"))
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)

        Return criteria.List(Of ProtocolJournalLog)()
    End Function
#End Region

#Region "NHibernate Properties"
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom("ProtDB")
        End Get
    End Property
#End Region

End Class
