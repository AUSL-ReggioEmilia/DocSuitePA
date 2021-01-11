Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager

Public Class NHSecurityGroupFinder
    Inherits NHibernateBaseFinder(Of SecurityGroups, SecurityGroups)

#Region " Properties "
    Public Property OnContainerEnabled As Integer = Nothing
    Public Property OnRoleEnabled As Integer = Nothing

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
        Return Decorate(NHibernateSession.CreateCriteria(persistentType, "SG"))
    End Function

    Protected Overridable Function Decorate(criteria As ICriteria) As ICriteria
        criteria.Add(Restrictions.Eq("TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
        criteria.AddOrder(Order.Asc("GroupName"))

        If Not OnContainerEnabled = Nothing Then
            Dim containerExistCriteria As DetachedCriteria = DetachedCriteria.For(Of ContainerGroup)("CNG")
            containerExistCriteria.Add(Restrictions.Eq("CNG.Container.Id", OnContainerEnabled))
            containerExistCriteria.SetProjection(Projections.Property("CNG.Id"))
            containerExistCriteria.Add(Restrictions.EqProperty("CNG.SecurityGroup.Id", "SG.Id"))
            criteria.Add(Subqueries.Exists(containerExistCriteria))
        End If
        If Not OnRoleEnabled = Nothing Then
            Dim roleExistCriteria As DetachedCriteria = DetachedCriteria.For(Of RoleGroup)("RG")
            roleExistCriteria.Add(Restrictions.Eq("RG.Role.Id", OnRoleEnabled))
            roleExistCriteria.SetProjection(Projections.Property("RG.Id"))
            roleExistCriteria.Add(Restrictions.EqProperty("RG.SecurityGroup.Id", "SG.Id"))
            criteria.Add(Subqueries.Exists(roleExistCriteria))
        End If

        Return criteria
    End Function

    Public Overrides Function DoSearch() As IList(Of SecurityGroups)
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of SecurityGroups)()
    End Function

    Public Sub ClearFinder()
        Me.OnContainerEnabled = Nothing
        Me.OnRoleEnabled = Nothing
    End Sub
#End Region

End Class
