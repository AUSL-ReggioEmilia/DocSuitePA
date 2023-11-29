Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentTypeDao
    Inherits BaseNHibernateDao(Of DocumentType)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function GetAll() As IList(Of DocumentType)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.AddOrder(Order.Asc("Description"))
        Return (criteria.List(Of DocumentType)())
    End Function

    Public Function GetMaxId() As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetProjection(Projections.ProjectionList.Add(Projections.Max("Id")))
        Return criteria.UniqueResult(Of Integer)()
    End Function


    Public Function DocTypeSearch(ByVal idDocType As Integer, ByVal onlyIsActive As Boolean, ByVal packageEnabled As Boolean, ByVal description As String) As IList(Of DocumentType)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If idDocType > 0 Then
            criteria.Add(Restrictions.Eq("Id", idDocType))
        End If

        If onlyIsActive Then
            criteria.Add(Restrictions.Eq("IsActive", True))
        End If

        If description.Length > 0 Then
            criteria.Add(Restrictions.Eq("Description", description))
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of DocumentType)()
    End Function

    Public Function GetByCode(code As String) As DocumentType
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of DocumentType)()
        criteria.Add(Restrictions.Eq("Code", code))
        criteria.SetMaxResults(1)
        criteria.AddOrder(Order.Desc("Id"))
        Return criteria.UniqueResult(Of DocumentType)()
    End Function

End Class
