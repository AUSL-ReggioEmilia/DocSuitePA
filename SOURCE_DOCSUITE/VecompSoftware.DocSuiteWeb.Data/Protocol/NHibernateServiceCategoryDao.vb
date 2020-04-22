Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateServiceCategoryDao
    Inherits BaseNHibernateDao(Of ServiceCategory)

    Public Enum DescriptionSearchType
        One = 1
        All = 2
    End Enum

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function GetAll() As IList(Of ServiceCategory)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.AddOrder(Order.Asc("Code"))
        criteria.AddOrder(Order.Asc("Description"))
        Return criteria.List(Of ServiceCategory)()
    End Function

    Public Function GetObjectByDescription(ByVal Description As String, ByVal SearchType As DescriptionSearchType, ByVal Code As String) As IList(Of ServiceCategory)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If Not (String.IsNullOrEmpty(Description)) Then
            Dim words As String() = Description.Split(" "c)
            Select Case SearchType
                Case DescriptionSearchType.All
                    Dim conju As Conjunction = Expression.Conjunction()
                    For Each word As String In words
                        conju.Add(Expression.Like("Description", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(conju)
                Case DescriptionSearchType.One
                    Dim disju As Disjunction = Expression.Disjunction()
                    For Each word As String In words
                        disju.Add(Expression.Like("Description", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(disju)
            End Select
        End If

        If Not (String.IsNullOrEmpty(Code)) Then
            Dim codici As String() = Code.Split(" "c)
            Select Case SearchType
                Case DescriptionSearchType.All
                    Dim conju As Conjunction = Expression.Conjunction()
                    For Each codice As String In codici
                        conju.Add(Expression.Like("Code", codice, MatchMode.Anywhere))
                    Next
                    criteria.Add(conju)
                Case DescriptionSearchType.One
                    Dim disju As Disjunction = Expression.Disjunction()
                    For Each codice As String In codici
                        disju.Add(Expression.Like("Code", codice, MatchMode.Anywhere))
                    Next
                    criteria.Add(disju)
            End Select
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of ServiceCategory)()
    End Function

End Class