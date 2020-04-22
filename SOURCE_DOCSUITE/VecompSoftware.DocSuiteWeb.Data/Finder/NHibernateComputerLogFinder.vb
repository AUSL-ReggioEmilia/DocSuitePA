Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports VecompSoftware.Helpers.NHibernate

<Serializable(), DataObject()> _
Public Class NHibernateComputerLogFinder
    Inherits NHibernateBaseFinder(Of ComputerLog, ComputerLog)

#Region "Fields"

    Private _dateFrom As Date?
    Private _dateTo As Date?
    Private _systemServer As String
    Private _systemComputer As String
    Private _systemUser As String

    Private _enablePaging As Boolean

#End Region

#Region "Properties"
    Public Property LastOperationDateFrom() As Date?
        Get
            Return _dateFrom
        End Get
        Set(ByVal value As Date?)
            _dateFrom = value
        End Set
    End Property

    Public Property LastOperationDateTo() As Date?
        Get
            Return _dateTo
        End Get
        Set(ByVal value As Date?)
            _dateTo = value
        End Set
    End Property

    Public Property SystemServer() As String
        Get
            Return _systemServer
        End Get
        Set(ByVal value As String)
            _systemServer = value
        End Set
    End Property

    Public Property SystemComputer() As String
        Get
            Return _systemComputer
        End Get
        Set(ByVal value As String)
            _systemComputer = value
        End Set
    End Property

    Public Property SystemUser() As String
        Get
            Return _systemUser
        End Get
        Set(ByVal value As String)
            _systemUser = value
        End Set
    End Property

    Public Property EnablePaging() As Boolean
        Get
            Return _enablePaging
        End Get
        Set(ByVal value As Boolean)
            _enablePaging = value
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

        'Filtro Data di Registrazione a partire da
        If LastOperationDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("LastOperationDate", _dateFrom)))
        End If

        'Filtro Data di Registrazione fino a
        If LastOperationDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("LastOperationDate", _dateTo)))
        End If

        'Server
        If Not String.IsNullOrEmpty(SystemServer) Then
            criteria.Add(Expression.Like("SystemServer", _systemServer, MatchMode.Anywhere))
        End If

        'Computer
        If Not String.IsNullOrEmpty(SystemComputer) Then
            criteria.Add(Expression.Like("Id", SystemComputer, MatchMode.Anywhere))
        End If


        If Not String.IsNullOrEmpty(SystemUser) Then
            criteria.Add(Expression.Like("SystemUser", _systemUser, MatchMode.Anywhere))
        End If

        'Aggancia filtri
        AttachFilterExpressions(criteria)

        Return criteria
    End Function
#End Region

#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As IList(Of ComputerLog)
        Dim criteria As ICriteria = Me.CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Ascending)
            'criteria.AddOrder(Order.Asc("Id"))
        End If

        If EnablePaging Then
            criteria.SetFirstResult(_startIndex)
            criteria.SetMaxResults(_pageSize)
        End If

        ' 05/04/2012 Rocca, forzatura viewer
        Dim resList As IList(Of ComputerLog) = criteria.List(Of ComputerLog)()
        'If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.ForceViewer) Then
        '    For Each item As ComputerLog In resList
        '        item.AdvancedViewer = DocSuiteContext.Current.ProtocolEnv.ForceViewer
        '    Next
        'End If

        Return resList

    End Function
#End Region

End Class
