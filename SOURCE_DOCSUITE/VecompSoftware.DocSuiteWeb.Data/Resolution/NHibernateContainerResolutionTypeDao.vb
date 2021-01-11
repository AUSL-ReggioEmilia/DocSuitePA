Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq

Public Class NHibernateContainerResolutionTypeDao
    Inherits BaseNHibernateDao(Of ContainerResolutionType)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetAllowedResolutionTypes(idContainer As Integer) As IList(Of ContainerResolutionType)
        criteria = NHibernateSession.CreateCriteria(persitentType, "CRT")
        criteria.Add(Restrictions.Eq("CRT.Id.idContainer", idContainer))

        Return criteria.List(Of ContainerResolutionType)()
    End Function

    Function GetAllowedContainers(resolutionTypeId As Short, ByVal groups As String(), ByVal isActive As Short, ByVal rights As ResolutionRightPositions?) As IList(Of ContainerResolutionType)
        criteria = NHibernateSession.CreateCriteria(persitentType, "CRT")
        criteria.CreateAlias("CRT.container", "C", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("C.ContainerGroups", "CG", SqlCommand.JoinType.LeftOuterJoin)
        ' TODO: far diventare idResolutionType uno short

        criteria.Add(Restrictions.Eq("CRT.Id.idResolutionType", CInt(resolutionTypeId)))

        Const fields As String = "_resolutionRights"

        criteria.Add(Restrictions.In("CG.SecurityGroup.Id", groups.ToArray()))

        Select Case isActive
            Case 1
                criteria.Add(Restrictions.Eq("C.IsActive", 1S))
                Dim disj As New Disjunction()
                disj.Add(Restrictions.And(Restrictions.IsNull("C.ActiveFrom"), Restrictions.IsNull("C.ActiveTo")))
                disj.Add(Restrictions.And(Restrictions.Ge("C.ActiveTo", DateTime.Now), Restrictions.Le("C.ActiveFrom", DateTime.Now)))
                criteria.Add(disj)
            Case 0
                Dim disj As New Disjunction()
                disj.Add(Restrictions.Eq("C.IsActive", 0S))
                disj.Add(Restrictions.Le("C.ActiveTo", DateTime.Now))
                disj.Add(Restrictions.Ge("C.ActiveFrom", DateTime.Now))
                criteria.Add(disj)
            Case 3
                criteria.Add(Expression.Lt("C.IsActive", isActive))
        End Select

        'Il gruppo dell'utente deve possedere diritti 
        If rights.HasValue Then
            Dim rightMask As String = RIGHTS_LENGTH
            Mid$(rightMask, CInt(rights.Value), 1) = "1"
            criteria.Add(Restrictions.Like("CG." & fields, rightMask, MatchMode.Start))
        End If

        criteria.AddOrder(Order.Asc("C.Name"))
        criteria.AddOrder(Order.Asc("CG.Name"))

        criteria.SetResultTransformer(New Transform.DistinctRootEntityResultTransformer())

        Return criteria.List(Of ContainerResolutionType)()
    End Function

#End Region

End Class
