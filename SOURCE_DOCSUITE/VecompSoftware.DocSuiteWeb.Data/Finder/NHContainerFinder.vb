Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports NHibernate.Transform
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data

Public Class NHContainerFinder
    Inherits NHibernateBaseFinder(Of Container, Container)

#Region " Properties "

    Public Property Name As String

    Public Property LocationTypeIn As IEnumerable(Of LocationTypeEnum)

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
        Return criteria
    End Function
#End Region

#Region " Methods "

    Protected Function Decorate(criteria As ICriteria) As ICriteria
        If Not String.IsNullOrWhiteSpace(Me.Name) Then
            criteria.Add(Restrictions.Like("Name", Me.Name, MatchMode.Anywhere))
        End If

        If Not Me.LocationTypeIn.IsNullOrEmpty() Then
            Dim disj As New Disjunction
            For Each item As LocationTypeEnum In Me.LocationTypeIn.ToList()
                disj.Add(Restrictions.IsNotNull(item.ToString()))
            Next
            criteria.Add(disj)
        End If
        Return criteria
    End Function

    Public Overridable Function List() As IList(Of Container)
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetFetchMode("ContainerGroups", FetchMode.Join)
        criteria.SetFetchMode("ContainerGroups.SecurityGroup", FetchMode.Join)
        criteria = Decorate(criteria)
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        criteria.AddOrder(Order.Asc("Name"))
        Return criteria.List(Of Container)()
    End Function

    Public Overrides Function DoSearch() As IList(Of Container)
        Return List()
    End Function

#End Region

End Class
