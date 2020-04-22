Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentTokenUserDao
    Inherits BaseNHibernateDao(Of DocumentTokenUser)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
    ''' <summary> Consente di ottenere un identificativo univoco per l'elemento aggiunto. </summary>
    Public Function GetMaxIncremental(ByVal year As Short, ByVal number As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))
        criteria.SetProjection(Projections.Max("Id.Incremental"))

        Dim uniqueRes As Object = criteria.UniqueResult()
        If uniqueRes Is Nothing Then
            Return 0
        Else
            Return CType(uniqueRes, Short)
        End If
    End Function



    Public Function GetDocumentTokenUserList(ByVal year As Short, ByVal number As Integer, ByVal DocumentToken As DocumentToken, _
                                                Optional ByVal Account As String = "", Optional ByVal bAddIncremental As Boolean = False, _
                                                Optional ByVal bAddStepNumber As Boolean = False, Optional ByVal bAddSubStep As Boolean = False, _
                                                Optional ByVal bAddRoleDestination As Boolean = False, Optional ByVal bAddIsActive As Boolean = False _
                                            ) As IList(Of DocumentTokenUser)

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        If Not DocumentToken Is Nothing Then


            If (Not bAddIncremental And bAddStepNumber) Then
                criteria.Add(Restrictions.Eq("DocStep", DocumentToken.DocStep))
                criteria.Add(Restrictions.Eq("SubStep", DocumentToken.SubStep))
            End If

            If bAddIncremental Then
                criteria.Add(Restrictions.Le("DocStep", DocumentToken.DocStep))
            End If

            If bAddRoleDestination Then
                criteria.Add(Restrictions.Eq("Role.Id", DocumentToken.RoleDestination.Id))
            End If

            If bAddIsActive Then
                criteria.Add(Restrictions.Eq("IsActive", 1S))
            End If

            If Account <> String.Empty Then
                criteria.Add(Restrictions.Eq("Account", Account))
            End If

            Select Case DocumentToken.DocStep
                Case 0
                    criteria.AddOrder(Order.Asc("DocStep"))
                    criteria.AddOrder(Order.Asc("SubStep"))
                    criteria.AddOrder(Order.Asc("UserName"))
                Case Else
                    criteria.AddOrder(Order.Asc("UserName"))
            End Select

        End If

        Return criteria.List(Of DocumentTokenUser)()
    End Function

 
    Public Function GetByYearNumber(ByVal year As Short, ByVal number As Integer) As IList(Of DocumentTokenUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Role", "Role", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("id.Year", year))
        criteria.Add(Restrictions.Eq("id.Number", number))

        Return criteria.List(Of DocumentTokenUser)()
    End Function


    Public Function DocumentTokenUserSearch( _
        ByVal Year As Short, ByVal Number As Integer, _
        Optional ByVal StepNumber As Integer = 0, Optional ByVal SubStep As Integer = 0, _
        Optional ByVal IdRoleDestination As Integer = 0, Optional ByVal IsActive As Boolean = False, _
        Optional ByVal Account As String = "", Optional ByVal Incremental As Boolean = False) As IList(Of DocumentTokenUser)

        criteria = NHibernateSession.CreateCriteria(persitentType, "T")

        criteria.CreateAlias("T.Role", "Role")

        criteria.Add(Restrictions.Eq("T.Id.Year", Year))
        criteria.Add(Restrictions.Eq("T.Id.Number", Number))

        If Not Incremental And StepNumber <> 0 Then
            criteria.Add(Restrictions.Eq("T.DocStep", CType(StepNumber, Short)))
            criteria.Add(Restrictions.Eq("T.SubStep", CType(SubStep, Short)))
        End If

        If Incremental Then
            criteria.Add(Restrictions.Le("T.DocStep", CType(StepNumber, Short)))
        End If

        If IdRoleDestination <> 0 Then
            criteria.Add(Restrictions.Eq("Role.Id", IdRoleDestination))
        End If

        If IsActive Then
            criteria.Add(Restrictions.Eq("T.IsActive", Int16.Parse(1)))
        End If

        If Account <> "" Then
            criteria.Add(Restrictions.Eq("T.Account", Account))
        End If

        Select Case StepNumber
            Case 0
                criteria.AddOrder(Order.Asc("T.DocStep"))
                criteria.AddOrder(Order.Asc("T.SubStep"))
                criteria.AddOrder(Order.Asc("T.UserName"))
            Case Else
                criteria.AddOrder(Order.Asc("T.UserName"))

        End Select


        Return criteria.List(Of DocumentTokenUser)()
    End Function
End Class
