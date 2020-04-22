Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateContainerDocTypeDao
    Inherits BaseNHibernateDao(Of ContainerDocType)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Ritorna l'elenco di tipi documento abbinati al contenitore specificato </summary>
    ''' <param name="idContainer">Id del contenitore</param>
    ''' <param name="isActive">Stato del tipo documento (attivo si/no)</param>
    ''' <returns>Lista di oggetti di tipo DocumentType</returns>
    Public Function ContainerDocTypeSearch(ByVal idContainer As Integer, Optional ByVal isActive As Boolean = False) As IList(Of DocumentType)
        Dim allowed As IList(Of DocumentType)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(GetType(DocumentType))
        criteria.CreateAlias("ContainerDocTypes", "cdt", SqlCommand.JoinType.InnerJoin)
        If isActive Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        criteria.Add(Restrictions.Eq("cdt.Container.Id", idContainer))
        criteria.Add(Restrictions.Eq("cdt.IsAllowed", True))
        criteria.AddOrder(Order.Asc("Description"))
        allowed = criteria.List(Of DocumentType)()

        If allowed.Count = 0 Then
            criteria = Nothing
            criteria = NHibernateSession.CreateCriteria(GetType(DocumentType))
            criteria.CreateAlias("ContainerDocTypes", "cdt", SqlCommand.JoinType.LeftOuterJoin)
            If isActive Then
                criteria.Add(Restrictions.Eq("IsActive", 1S))
            End If
            criteria.Add(Expression.Not(Expression.And(Expression.And(Expression.Or(Restrictions.Eq("cdt.Container.Id", idContainer), Expression.IsNotNull("cdt.Container.Id")), Expression.IsNotNull("cdt.IsAllowed")), Restrictions.Eq("cdt.IsAllowed", False))))
            criteria.AddOrder(Order.Asc("Description"))
            criteria.SetResultTransformer(Transformers.DistinctRootEntity)
            allowed = criteria.List(Of DocumentType)()
        End If

        Return allowed
    End Function

End Class
