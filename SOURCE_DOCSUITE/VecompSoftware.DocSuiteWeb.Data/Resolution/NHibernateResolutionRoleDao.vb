
Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionRoleDao
    Inherits BaseNHibernateDao(Of ResolutionRole)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Private Function RestrictedRight(expression As ICriterion, resolution As Resolution, rightPosition As Integer?, active As Boolean?) As Boolean?
        Dim criteria As ICriteria = CreateGetResolutionRoleCriteria(rightPosition, active, True, ResolutionRoleRightType.Guaranteed)
        criteria.Add(Restrictions.Eq("RESL.Id", resolution.Id))

        ' Verifico se ce ne sono per l'istanza richniesta
        Dim count As Integer = criteria.UniqueResult(Of Integer)

        If count = 0 Then
            Return Nothing ' Nessuna restriction per il diritto richiesto
        End If

        ' CI SONO restrizioni per il diritto richiesto: solo se appartengo al settore mantengo il diritto
        ' Verifico se sono in un settore con diritto garantito
        criteria.Add(expression)

        Return criteria.UniqueResult(Of Integer) > 0

    End Function

    Public Function RestrictedRightBySG(ByVal resolution As Resolution, idGroupIn As IList(Of Integer), rightPosition As Integer?, active As Boolean?) As Boolean?
        Return RestrictedRight(Restrictions.In("RG.SecurityGroup.Id", idGroupIn.ToArray()), resolution, rightPosition, active)
    End Function

    Public Function CheckRightBySG(ByVal resolution As Resolution, idGroupIn As IList(Of Integer), rightPosition As Integer?, active As Boolean?) As Boolean
        Dim criteria As ICriteria = CreateGetResolutionRoleCriteria(rightPosition, active, True, ResolutionRoleRightType.Added)

        criteria.Add(Restrictions.In("RG.SecurityGroup.Id", idGroupIn.ToArray()))
        criteria.Add(Restrictions.Eq("RESL.Id", resolution.Id))

        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Private Function CreateGetResolutionRoleCriteria(rightPosition As Integer?, active As Boolean?, getRowCount As Boolean, rightType As ResolutionRoleRightType) As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionRole)("RR")
        criteria.CreateAlias("RR.Resolution", "RESL")
        criteria.CreateAlias("RR.Role", "R")
        criteria.CreateAlias("R.RoleGroups", "RG")
        criteria.CreateAlias("RR.ResolutionRoleType", "T")

        If rightPosition.HasValue Then
            Dim pattern As String = "1".PadLeft(rightPosition.Value, "_"c)
            Select Case rightType
                Case ResolutionRoleRightType.Added
                    criteria.Add(Restrictions.Like("T.RightsAdded", pattern, MatchMode.Start))
                Case ResolutionRoleRightType.Guaranteed
                    criteria.Add(Restrictions.Like("T.RightsGuaranteed", pattern, MatchMode.Start))
            End Select
        End If

        If active.HasValue Then
            If active.Value Then
                criteria.Add(Restrictions.Eq("R.IsActive", True))
            Else
                criteria.Add(Restrictions.Eq("R.IsActive", False))
            End If
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        If getRowCount Then
            criteria.SetProjection(Projections.RowCount())
        Else
            criteria.AddOrder(Order.Asc("R.Name"))
        End If

        Return criteria
    End Function

    Public Enum ResolutionRoleRightType
        Added
        Guaranteed
    End Enum

End Class
