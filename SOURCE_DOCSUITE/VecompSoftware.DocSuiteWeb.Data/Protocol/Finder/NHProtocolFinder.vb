Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager

Public Class NHProtocolFinder

#Region " Properties "

    Public MaxResults As Integer?
    Public Property IdContainerIn As IEnumerable(Of Integer)

    Protected Property Criteria As ICriteria

#End Region

#Region " Methods "

    Protected Overridable Sub Decorate()
        If MaxResults.HasValue Then
            Criteria.SetMaxResults(MaxResults.Value)
        End If
        If Not IdContainerIn.IsNullOrEmpty() Then
            Criteria.Add(Restrictions.In("P.Container.Id", IdContainerIn.ToArray()))
        End If
    End Sub

    Public Overridable Function List() As IList(Of Protocol)
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession("ProtDB")
            Criteria = session.CreateCriteria(Of Protocol)("P")
            Criteria.SetFetchMode("P.PecMails", FetchMode.Lazy)
            Criteria.SetFetchMode("P.AdvancedProtocol", FetchMode.Lazy)
            Decorate()
            Return Criteria.List(Of Protocol)()
        End Using
    End Function
    Public Overridable Function ListKeys() As IList(Of YearNumberCompositeKey)
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession("ProtDB")
            Criteria = session.CreateCriteria(Of Protocol)("P")
            Decorate()
            Criteria.SetProjection(Projections.Property("P.Id"))
            Return Criteria.List(Of YearNumberCompositeKey)()
        End Using
    End Function

    Public Overridable Function RowCount() As Integer
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession("ProtDB")
            Criteria = session.CreateCriteria(Of Protocol)("P")
            Decorate()
            Criteria.SetProjection(Projections.RowCount())
            Return Criteria.UniqueResult(Of Integer)()
        End Using
    End Function

#End Region

End Class
