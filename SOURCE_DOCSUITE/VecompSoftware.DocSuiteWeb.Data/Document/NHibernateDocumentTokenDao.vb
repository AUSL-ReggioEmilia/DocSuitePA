Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentTokenDao
    Inherits BaseNHibernateDao(Of DocumentToken)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetMaxIncremental(ByVal year As Short, ByVal number As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.SetProjection(Projections.Max("Id.Incremental"))

        Dim result As Object = criteria.UniqueResult()
        If result Is Nothing Then
            Return 0S
        Else
            Return CType(result, Short)
        End If
    End Function

    Public Function GetCountByRoleSource(ByVal Role As Role) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("RoleSource", Role))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetCountByRoleDestination(ByVal Role As Role) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("RoleDestination", Role))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetTokenList(ByVal year As Short, ByVal number As Integer, ByVal idTokenTypes As String(), _
                                Optional ByVal bAddResponseCriteria As Boolean = False, Optional ByVal iStep As Integer = 0, _
                                Optional ByVal bIncludeIsActive As Boolean = True) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.Add(Expression.In("DocumentTabToken.Id", idTokenTypes))
        If bIncludeIsActive Then
            criteria.Add(Expression.Gt("IsActive", False))
        End If
        If bAddResponseCriteria Then
            criteria.Add(Expression.Not(Restrictions.Eq("Response", "A")))
        End If

        If iStep > 0 Then
            criteria.Add(Restrictions.Eq("DocStep", iStep))
        End If

        criteria.AddOrder(Order.Asc("DocStep"))
        criteria.AddOrder(Order.Asc("SubStep"))
        criteria.AddOrder(Order.Asc("IncrementalOrigin"))

        Return criteria.List(Of DocumentToken)()

    End Function

    Function GetTokenListForAllTokenTypes(ByVal year As Short, ByVal number As Integer, Optional ByVal bAddResponseCriteria As Boolean = False, Optional ByVal Incremental As Int16 = 0, Optional ByVal Role As Role = Nothing) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("DocumentTabToken", "DocumentTabToken", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))

        If bAddResponseCriteria Then
            criteria.Add(Expression.Not(Restrictions.Eq("Response", "A")))
        End If

        If Incremental <> 0 Then
            criteria.Add(Restrictions.Eq("Incremental", Incremental))
        End If

        If Role IsNot Nothing Then
            criteria.Add(Expression.Or(Restrictions.Eq("RoleDestination.Id", Role.Id), Restrictions.Eq("RoleSource.Id", Role.Id)))
        End If

        criteria.AddOrder(Order.Asc("DocStep"))
        criteria.AddOrder(Order.Asc("SubStep"))
        criteria.AddOrder(Order.Asc("Incremental"))

        Return criteria.List(Of DocumentToken)()

    End Function


    Function GetTokenListCC(ByVal year As Short, ByVal number As Integer, Optional ByVal bAddResponseCriteria As Boolean = False, Optional ByVal Incremental As Int16 = 0, Optional ByVal Role As Integer = 0) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))

        If bAddResponseCriteria Then
            criteria.Add(Expression.Not(Restrictions.Eq("Response", "A")))
        End If

        If Incremental <> 0 Then
            criteria.Add(Restrictions.Eq("DocStep", Incremental))
        End If

        criteria.Add(Restrictions.Eq("DocumentTabToken.Id", "CC"))

        criteria.AddOrder(Order.Asc("DocStep"))
        criteria.AddOrder(Order.Asc("SubStep"))
        criteria.AddOrder(Order.Asc("IncrementalOrigin"))

        Return criteria.List(Of DocumentToken)()

    End Function


    Function GetTokenOperationDateList(ByVal year As Short, ByVal number As Integer, ByVal idTokenTypes As String(), ByVal roles As String()) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.Add(Expression.Gt("IsActive", False))
        criteria.Add(Expression.In("DocumentTabToken.Id", idTokenTypes))

        criteria.Add(Expression.In("RoleDestination.Id", roles))

        criteria.AddOrder(Order.Asc("DocStep"))
        criteria.AddOrder(Order.Asc("SubStep"))
        criteria.AddOrder(Order.Asc("IncrementalOrigin"))

        Return criteria.List(Of DocumentToken)()

    End Function

    Function GetDocumentTokenByTokenType(ByVal year As Short, ByVal number As Integer, ByVal idTokenTypes As String(), ByVal roles As String(), Optional ByVal bAddResponseCriteria As Boolean = False, Optional ByVal isActive As Boolean = False) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.Add(Expression.In("DocumentTabToken.Id", idTokenTypes))

        If roles.Length > 0 Then
            criteria.Add(Expression.In("RoleDestination.Id", roles))
        End If

        If bAddResponseCriteria Then
            criteria.Add(Expression.Not(Restrictions.Eq("Response", "A")))
        End If
        If isActive Then
            criteria.Add(Restrictions.Ge("IsActive", False))
        End If

        criteria.AddOrder(Order.Asc("DocStep"))
        criteria.AddOrder(Order.Asc("SubStep"))
        criteria.AddOrder(Order.Asc("IncrementalOrigin"))

        Return criteria.List(Of DocumentToken)()

    End Function

    Public Function GetByYearNumber(ByVal year As Short, ByVal number As Integer, ByVal isActive As Boolean) As IList(Of DocumentToken)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.Add(Restrictions.Eq("IsActive", isActive))

        Return criteria.List(Of DocumentToken)()
    End Function


    Function GetTokenList(ByVal year As Short, ByVal number As Integer) As List(Of Object)

        criteria = NHibernateSession.CreateCriteria(persitentType, "T")

        criteria.CreateAlias("T.RoleDestination", "RoleDestination", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateAlias("T.DocumentTabToken", "DocumentTabToken", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("T.Year", year))
        criteria.Add(Restrictions.Eq("T.Number", number))
        criteria.Add(Expression.Not(Restrictions.Eq("DocumentTabToken.Id", "CC")))

        criteria.SetProjection(Projections.ProjectionList.Add(Projections.GroupProperty("T.DocStep")).Add(Projections.GroupProperty("T.SubStep")).Add(Projections.GroupProperty("RoleDestination.Id")).Add(Projections.GroupProperty("RoleDestination.Name")).Add(Projections.GroupProperty("RoleDestination.FullIncrementalPath")))

        criteria.AddOrder(Order.Asc("T.DocStep"))
        criteria.AddOrder(Order.Asc("T.SubStep"))

        Return criteria.List()
    End Function

    Public Function DocumentTokenRoleCC(ByVal Year As Short, ByVal Number As Integer, Optional ByVal iStep As Integer = 0) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType, "T")

        criteria.CreateAlias("T.DocumentTabToken", "DocumentTabToken")

        criteria.Add(Restrictions.Eq("T.Year", Year))
        criteria.Add(Restrictions.Eq("T.Number", Number))
        criteria.Add(Restrictions.Eq("DocumentTabToken.Id", "CC"))
        criteria.Add(Expression.Not(Restrictions.Eq("T.Response", "A")))

        If iStep <> 0 Then
            criteria.Add(Restrictions.Eq("T.DocStep", CType(iStep, Short)))
        End If

        criteria.AddOrder(Order.Asc("T.DocStep"))
        criteria.AddOrder(Order.Asc("T.SubStep"))
        criteria.AddOrder(Order.Asc("T.Id.Incremental"))

        Return criteria.List(Of DocumentToken)()

    End Function

    Public Function DocumentTokenStep(ByVal year As Short, ByVal number As Integer, ByVal idRole As Integer) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType, "T")

        criteria.CreateAlias("T.RoleDestination", "RoleDestination", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("T.Year", year))
        criteria.Add(Restrictions.Eq("T.Number", number))
        criteria.Add(Restrictions.Eq("RoleDestination.Id", IdRole))
        criteria.Add(Expression.Gt("T.IsActive", False))

        criteria.AddOrder(Order.Asc("T.OperationDate"))

        Return criteria.List(Of DocumentToken)()

    End Function

    Public Function GetTokenSuspend(ByVal Year As Short, ByVal Number As Integer, ByVal iStep As Integer, ByVal idTokenType As String()) As IList(Of DocumentToken)

        criteria = NHibernateSession.CreateCriteria(persitentType, "T")

        criteria.Add(Restrictions.Eq("T.Year", Year))
        criteria.Add(Restrictions.Eq("T.Number", Number))
        criteria.Add(Expression.In("T.DocumentTabToken.Id", idTokenType))
        criteria.Add(Expression.Not(Restrictions.Eq("T.IsActive", False)))

        If Array.Exists(idTokenType, AddressOf TokenTypeRRCondition) And Array.Exists(idTokenType, AddressOf TokenTypePTCondition) Then
            criteria.Add(Restrictions.Eq("T.DocStep", Convert.ToInt16(iStep) - Convert.ToInt16(1)))
        ElseIf Array.Exists(idTokenType, AddressOf TokenTypeRPCondition) Then
            criteria.Add(Restrictions.Eq("T.DocStep", Convert.ToInt16(iStep)))
        End If

        criteria.AddOrder(Order.Asc("T.DocStep"))
        criteria.AddOrder(Order.Asc("T.Incremental"))

        Return criteria.List(Of DocumentToken)()
    End Function

#Region "TokenType Array Conditions"
    Private Function TokenTypeRRCondition(ByVal item As String) As Boolean
        If item = "RR" Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function TokenTypePTCondition(ByVal item As String) As Boolean
        If item = "PT" Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function TokenTypeRPCondition(ByVal item As String) As Boolean
        If item = "RP" Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region

    Public Function GetDocumentTokenRoleP(ByVal year As Short, ByVal number As Integer, Optional ByVal idRoleDestination As Integer = 0) As IList(Of DocumentToken)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        Dim tokenTypes As New ArrayList
        tokenTypes.Add("PA")
        tokenTypes.Add("PM")
        tokenTypes.Add("PC")
        tokenTypes.Add("PT")
        tokenTypes.Add("PR")

        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.Add(Expression.In("DocumentTabToken.Id", tokenTypes.ToArray()))
        If idRoleDestination <> 0 Then
            criteria.Add(Restrictions.Eq("RoleDestination.Id", idRoleDestination))
        End If
        criteria.Add(Expression.Gt("IsActive", False))
        criteria.AddOrder(Order.Asc("DocStep"))
        criteria.AddOrder(Order.Asc("SubStep"))
        criteria.AddOrder(Order.Asc("IncrementalOrigin"))

        Return criteria.List(Of DocumentToken)()
    End Function
End Class
