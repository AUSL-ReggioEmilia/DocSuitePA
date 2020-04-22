Imports NHibernate
Imports NHibernate.Criterion
Imports System.ComponentModel
Imports VecompSoftware.NHibernateManager

<Serializable(), DataObject()> _
Public Class NHibernateDocumentSeriesItemLogFinder
    Inherits NHibernateBaseFinder(Of DocumentSeriesItemLog, DocumentSeriesItemLog)

    Public Property DocumentSeriesItem() As DocumentSeriesItem

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        EnablePaging = True
    End Sub

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemLog)("DSIL")
        Return criteria
    End Function

    Public Overrides Function DoSearch() As IList(Of DocumentSeriesItemLog)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        Return criteria.List(Of DocumentSeriesItemLog)()
    End Function

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)
    End Function

    Protected Sub DecorateCriteria(ByRef criteria As ICriteria)
        If DocumentSeriesItem IsNot Nothing Then
            criteria.Add(Restrictions.Eq("DSIL.DocumentSeriesItem", DocumentSeriesItem))
            criteria.Add(Restrictions.Not(Restrictions.Like("LogType", DocumentSeriesItemLogType.SB)))
        End If
    End Sub
End Class
