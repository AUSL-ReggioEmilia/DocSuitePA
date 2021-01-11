Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager

Public Class NHSecurityUserFinder
    Inherits NHibernateBaseFinder(Of SecurityUsers, SecurityUsers)

#Region " Properties "

    Public MaxResults As Integer?
    Public Property Ids As IEnumerable(Of Integer)
    Public Property Domain As String
    Public Property Account As String
    ''' <summary>
    ''' ATTENZIONE! Questa proprietà deve essere sempre usata insiema alla proprietà CurrentUser 
    ''' per ottenerne un corretto funzionamento
    ''' </summary>
    ''' <returns></returns>
    Public Property ExcludedGroupIds As IEnumerable(Of Integer)
    Public Property CurrentUser As String

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New(ByVal dbName As String)
        SessionFactoryName = dbName
    End Sub

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub

#End Region

#Region " Methods "
    Protected Overrides Function CreateCriteria() As ICriteria
        Return Decorate(NHibernateSession.CreateCriteria(persistentType, "SU"))
    End Function

    Protected Overridable Function Decorate(criteria As ICriteria) As ICriteria
        If MaxResults.HasValue Then
            criteria.SetMaxResults(MaxResults.Value)
        End If
        If Not Ids.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("SU.Id", Ids.ToArray()))
        End If
        If Not Domain.IsNullOrEmpty() Then
            criteria.Add(Restrictions.Eq("SU.UserDomain", Domain))
        End If
        If Not Account.IsNullOrEmpty() Then
            criteria.Add(Restrictions.Eq("SU.Account", Account))
        End If

        If Not ExcludedGroupIds.IsNullOrEmpty() AndAlso Not CurrentUser.IsNullOrEmpty() Then
            criteria.Add(Restrictions.Not(Restrictions.In("SU.Group.Id", ExcludedGroupIds.ToArray()) AndAlso Restrictions.Eq("SU.Account", CurrentUser)))
        End If

        Return criteria
    End Function

    Public Overridable Function ListKeys() As IList(Of Integer)
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetProjection(Projections.Property("SU.Id"))
        Return criteria.List(Of Integer)()
    End Function

    Public Overridable Function RowCount() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Overrides Function DoSearch() As IList(Of SecurityUsers)
        Dim criteria As ICriteria = CreateCriteria()
        criteria.CreateAlias("SU.Group", "SG", JoinType.InnerJoin)
        Return criteria.List(Of SecurityUsers)()
    End Function

#End Region

End Class
