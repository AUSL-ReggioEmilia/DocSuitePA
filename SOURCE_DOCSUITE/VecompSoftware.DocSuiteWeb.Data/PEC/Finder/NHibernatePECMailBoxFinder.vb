Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Transform
Imports NHibernate.SqlCommand

<Serializable(), DataObject()>
Public Class NHibernatePECMailBoxFinder
    Inherits NHibernateBaseFinder(Of PECMailBox, PECMailBox)


#Region "Properties"
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Property HasIncomingServer As Boolean?

    Public Property HasOutgoingServer As Boolean?

    Public Property IntegratedEnabled As Boolean?

    Public Property CheckUserRights As Boolean?

    Public Property RoleGroupPECRight As Boolean?

    Public Property OnlyProtocolBox As Boolean?

    Public Property RoleGroupProtocolMailRight As Boolean?

    Public Property GroupIds As IList(Of Integer)

    Public Property MailboxIds As Short()

    Public Property RoleIds As Integer()

    Public Property UDSEnabled As Boolean?

#End Region

#Region "Constuctor"

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub

#End Region

#Region " Methods "

    Public Overloads Overrides Function DoSearch() As IList(Of PECMailBox)
        Dim criteria As ICriteria = CreateCriteria()

        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of PECMailBox)()
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of PECMailBox)("PMB")

        If (HasIncomingServer.HasValue AndAlso HasIncomingServer.Value) Then
            criteria.Add(Restrictions.IsNotNull("PMB.IncomingServerName"))
        End If

        If (HasOutgoingServer.HasValue AndAlso HasOutgoingServer.Value) Then
            criteria.Add(Restrictions.IsNotNull("PMB.OutgoingServerName"))
        End If

        ' Mail box
        If MailboxIds IsNot Nothing AndAlso MailboxIds.Count() > 0 Then
            criteria.Add(Restrictions.In("PMB.Id", MailboxIds))
        End If

        ' Filter by Roles 
        If RoleIds IsNot Nothing AndAlso RoleIds.Count() > 0 Then
            criteria.CreateAlias("PMB.Roles", "R", JoinType.InnerJoin)
            criteria.Add(Restrictions.In("R.Id", RoleIds))
        End If

        If OnlyProtocolBox.HasValue Then
            criteria.Add(Restrictions.Eq("IsProtocolBox", OnlyProtocolBox.Value))
        End If

        If IntegratedEnabled.HasValue Then
            If IntegratedEnabled.Value Then
                criteria.Add(Restrictions.Eq("Managed", True))
            Else
                criteria.Add(Restrictions.Eq("Unmanaged", True))
            End If
        End If

        If CheckUserRights.HasValue AndAlso CheckUserRights.Value Then
            criteria.CreateAliasIfNotExists("PMB.Roles", "R", JoinType.InnerJoin)
            criteria.CreateAlias("R.RoleGroups", "RG", JoinType.InnerJoin)

            Dim pattern As String = "1".PadLeft(1, "_"c)

            If (RoleGroupPECRight.HasValue AndAlso RoleGroupPECRight.Value AndAlso OnlyProtocolBox = False) OrElse
                                  (RoleGroupPECRight.HasValue AndAlso RoleGroupPECRight.Value AndAlso RoleGroupProtocolMailRight.HasValue AndAlso RoleGroupProtocolMailRight.Value AndAlso OnlyProtocolBox = False) Then
                pattern = "1".PadLeft(3, "_"c)
            End If

            If (RoleGroupProtocolMailRight.HasValue AndAlso RoleGroupProtocolMailRight.Value AndAlso OnlyProtocolBox = True) OrElse
                                (RoleGroupPECRight.HasValue AndAlso RoleGroupPECRight.Value AndAlso RoleGroupProtocolMailRight.HasValue AndAlso RoleGroupProtocolMailRight.Value AndAlso OnlyProtocolBox = True) Then
                pattern = "1".PadLeft(4, "_"c)
            End If

            If UDSEnabled.HasValue AndAlso UDSEnabled.Value Then
                criteria.Add(Restrictions.Or(Restrictions.Like("RG.DocumentSeriesRights", "1".PadLeft(1, "_"c), MatchMode.Start),
                                         Restrictions.Like("RG._protocolRights", pattern, MatchMode.Start)))
            Else
                criteria.Add(Restrictions.Like("RG._protocolRights", pattern, MatchMode.Start))
            End If

            criteria.Add(Restrictions.Eq("R.IsActive", True))
            criteria.Add(Restrictions.InG("RG.SecurityGroup.Id", GroupIds))
            criteria.AddOrder(Order.Asc("R.Name"))
        End If

        Return criteria
    End Function

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)


    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachFilterExpressions(criteria)

        criteria.SetProjection(Projections.CountDistinct("PMB.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

#End Region


End Class