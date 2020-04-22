Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports NHibernate.Transform

Public Class NHRoleUserFinder

#Region " Constructors "

    Public Sub New()
        Me.RoleIdIn = New List(Of Integer)()
        Me.TypeIn = New List(Of String)
        Me.DSWEnvironmentIn = New List(Of DSWEnvironment)
        Me.AccountIn = New List(Of String)
    End Sub

#End Region

#Region " Properties "

    Public Property RoleIdIn As List(Of Integer)

    Public Property TypeIn As List(Of String)

    Public Property DSWEnvironmentIn As List(Of DSWEnvironment)

    Public Property AccountIn As List(Of String)

    Public Property RoleName As String

    Public Property RoleEnabled As Boolean?

    Protected Property Criteria As ICriteria



#End Region

#Region " Methods "

    Protected Overridable Sub Decorate()
        Criteria.CreateAlias("Role", "R")
        If Not Me.RoleIdIn.IsNullOrEmpty() Then
            Me.Criteria.Add(Restrictions.In("R.Id", Me.RoleIdIn.ToArray()))
        End If
        If Not Me.TypeIn.IsNullOrEmpty() Then
            Me.Criteria.Add(Restrictions.In("Type", Me.TypeIn.ToArray()))
        End If
        If Not Me.DSWEnvironmentIn.IsNullOrEmpty() AndAlso Not Me.DSWEnvironmentIn.Contains(DSWEnvironment.Any) Then
            Me.Criteria.Add(Restrictions.In("DSWEnvironment", Me.DSWEnvironmentIn.ToArray()))
        End If
        If Not Me.AccountIn.IsNullOrEmpty() Then
            Me.Criteria.Add(Restrictions.In("Account", Me.AccountIn.ToArray()))
        End If
        If Not String.IsNullOrWhiteSpace(RoleName) Then
            Criteria.Add(Restrictions.Like("R.Name", RoleName, MatchMode.Anywhere))
        End If

        If RoleEnabled.HasValue Then
            Criteria.Add(Restrictions.Eq("R.IsActive", Convert.ToInt16(RoleEnabled.Value)))
        End If
        Criteria.Add(Restrictions.Eq("R.TenantId", DocSuiteContext.Current.CurrentTenant.TenantId))
    End Sub

    Public Overridable Function List() As IList(Of RoleUser)
        Dim factoryName As String = [Enum].GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(factoryName)
            Me.Criteria = session.CreateCriteria(Of RoleUser)()
            Me.Decorate()
            Me.Criteria.SetResultTransformer(Transformers.DistinctRootEntity)
            Dim Order As New Order("Description", True)
            Me.Criteria.AddOrder(Order)
            Return Me.Criteria.List(Of RoleUser)()
        End Using
    End Function

#End Region

End Class
