Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateUserErrorFinder
    Inherits NHibernateBaseFinder(Of UserError, UserError)

#Region "Fields"
    Private _errorDate As Date?
    Private _systemServer As String
    Private _systemUser As String
#End Region

#Region "Properties"
    Public Property ErrorDate() As Date?
        Get
            Return _errorDate
        End Get
        Set(ByVal value As Date?)
            _errorDate = value
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

    Public Property SystemUser() As String
        Get
            Return _systemUser
        End Get
        Set(ByVal value As String)
            _systemUser = value
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

        'Data Errore
        If ErrorDate.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.EqualToDateIsoFormat("ErrorDate", _errorDate.Value)))
        End If

        'Server
        If Not String.IsNullOrEmpty(SystemServer) Then
            criteria.Add(Expression.Like("SystemServer", _systemServer, MatchMode.Anywhere))
        End If

        'Utente
        If Not String.IsNullOrEmpty(SystemUser) Then
            criteria.Add(Expression.Like("SystemUser", _systemUser, MatchMode.Anywhere))
        End If

        'Aggancia filtri
        AttachFilterExpressions(criteria)

        Return criteria
    End Function
#End Region

#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As IList(Of UserError)
        Dim criteria As ICriteria = CreateCriteria()

        AttachSortExpressions(criteria)

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of UserError)()
    End Function
#End Region

End Class