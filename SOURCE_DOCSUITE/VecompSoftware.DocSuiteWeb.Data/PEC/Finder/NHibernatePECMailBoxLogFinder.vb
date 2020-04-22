Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports VecompSoftware.Helpers.NHibernate

<Serializable(), DataObject()> _
Public Class NHibernatePECMailBoxLogFinder
    Inherits NHibernateBaseFinder(Of PECMailBoxLog, PECMailBoxLog)

#Region "Fields"
    Private _enablePaging As Boolean
    Private _dateFrom As Date?
    Private _dateTo As Date?
    Private _mailBoxIds As Short()
    Private _type As String
    Private _description As String
#End Region

#Region "Properties"
    Public Property ExcludeJSEvalActivities As Boolean = True
    Public Property IncludeJSEvalActivities As Boolean

    Public Property ExplicitDateTime() As Date?

    Public Property EnablePaging() As Boolean
        Get
            Return _enablePaging
        End Get
        Set(ByVal value As Boolean)
            _enablePaging = value
        End Set
    End Property
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

    Public Property MailboxIds() As Short()
        Get
            Return _mailBoxIds
        End Get
        Set(ByVal value As Short())
            _mailBoxIds = value
        End Set
    End Property

    Public Property ElemType() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

#End Region

#Region "Constuctor"
    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        ExcludeJSEvalActivities = True
        IncludeJSEvalActivities = False
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
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("Date", LastOperationDateFrom.Value)))
        End If

        'Filtro Data di Registrazione fino a
        If LastOperationDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("Date", LastOperationDateTo.Value)))
        End If

        If ExplicitDateTime.HasValue Then
            criteria.Add(Restrictions.Eq("Date", ExplicitDateTime.Value))
        End If

        ' Mail box
        If _mailBoxIds IsNot Nothing AndAlso _mailBoxIds.Length > 0 Then
            criteria.Add(Expression.In("MailBox.Id", _mailBoxIds))
        End If

        ' Tipo
        If Not String.IsNullOrEmpty(ElemType) Then
            criteria.Add(Restrictions.Eq("Type", ElemType))
        End If

        If ExcludeJSEvalActivities Then
            criteria.Add(Expression.Not(Restrictions.Eq("Type", "TimeEval")))
            criteria.Add(Expression.Not(Restrictions.Eq("Type", "ErrorEval")))
            criteria.Add(Expression.Not(Restrictions.Eq("Type", "PECErrorEval")))
            criteria.Add(Expression.Not(Restrictions.Eq("Type", "PECReadedEval")))
            criteria.Add(Expression.Not(Restrictions.Eq("Type", "PECDoneEval")))
        End If

        If IncludeJSEvalActivities Then
            Dim dsj As Disjunction = New Disjunction()
            dsj.Add(Restrictions.Eq("Type", "TimeEval"))
            dsj.Add(Restrictions.Eq("Type", "ErrorEval"))
            dsj.Add(Restrictions.Eq("Type", "PECErrorEval"))
            dsj.Add(Restrictions.Eq("Type", "PECReadedEval"))
            dsj.Add(Restrictions.Eq("Type", "PECDoneEval"))
            criteria.Add(dsj)
        End If

        ' Descrizione
        If Not String.IsNullOrEmpty(Description) Then
            criteria.Add(Restrictions.Eq("Description", Description))
        End If

        'Aggancia filtri
        AttachFilterExpressions(criteria)

        Return criteria
    End Function
#End Region

#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As IList(Of PECMailBoxLog)
        Dim criteria As ICriteria = Me.CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Descending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of PECMailBoxLog)()
    End Function
#End Region

End Class