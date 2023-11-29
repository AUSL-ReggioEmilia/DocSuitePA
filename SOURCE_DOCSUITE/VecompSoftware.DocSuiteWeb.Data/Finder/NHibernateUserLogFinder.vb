Imports NHibernate
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq

<Serializable(), DataObject()>
Public Class NHibernateUserLogFinder
    Inherits NHibernateBaseFinder(Of UserLog, UserLog)

#Region "Fields"

#End Region

#Region "Properties"
    Public Property LastOperationDateFrom As DateTimeOffset?

    Public Property LastOperationDateTo As DateTimeOffset?

    Public Property SystemServer As String

    Public Property SystemUser As String

    Public Property SystemUserContains As String

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

        ''Data accesso da
        'If LastOperationDateFrom.HasValue Then
        '    criteria.Add(Restrictions.Ge("LastOperationDate", _dateFrom))
        'End If

        ''Data accesso a
        'If LastOperationDateTo.HasValue Then
        '    criteria.Add(Restrictions.LeProperty("LastOperationDate", _dateTo))
        'End If

        'Filtro Data di Registrazione a partire da
        If LastOperationDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("LastOperationDate", New DateTimeOffset(LastOperationDateFrom.Value.Date.BeginOfTheDay())))
        End If

        'Filtro Data di Registrazione fino a
        If LastOperationDateTo.HasValue Then
            criteria.Add(Restrictions.Le("LastOperationDate", New DateTimeOffset(LastOperationDateTo.Value.Date.EndOfTheDay())))
        End If

        'Server
        If Not String.IsNullOrEmpty(SystemServer) Then
            criteria.Add(Expression.Like("SystemServer", SystemServer, MatchMode.Anywhere))
        End If

        'Utente
        If Not String.IsNullOrEmpty(SystemUser) Then
            'criteria.Add(Expression.Like("SystemUser", _systemUser, MatchMode.Anywhere))
            criteria.Add(Restrictions.Eq("Id", SystemUser))
        End If

        If Not String.IsNullOrEmpty(SystemUserContains) Then
            criteria.Add(Restrictions.Like("Id", SystemUserContains, MatchMode.Anywhere))
        End If

        'Aggancia filtri
        AttachFilterExpressions(criteria)

        Return criteria
    End Function
#End Region

#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As System.Collections.Generic.IList(Of UserLog)
        Dim criteria As ICriteria = Me.CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Ascending)
            'criteria.AddOrder(Order.Asc("Id"))
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)
        Dim encryptedUserLogs As List(Of UserLog) = criteria.List(Of UserLog)()

        Dim decryptedUserLogs As List(Of UserLog) = New List(Of UserLog)

        For Each decryptedUserLog As UserLog In encryptedUserLogs
            If Not String.IsNullOrEmpty(decryptedUserLog.UserProfile) Then
                decryptedUserLog.UserProfile = Helpers.Security.EncryptionHelper.DecryptString(decryptedUserLog.UserProfile, DocSuiteContext.PasswordEncryptionKey)
            End If
            decryptedUserLogs.Add(decryptedUserLog)
        Next

        Return decryptedUserLogs
    End Function
#End Region

End Class
