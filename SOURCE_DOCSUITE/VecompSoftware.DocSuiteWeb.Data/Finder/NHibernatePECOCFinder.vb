Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernatePECOCFinder
    Inherits NHibernateBaseFinder(Of PECOC, PECOC)

#Region " Fields "
#End Region

#Region " Constructors "

    Public Sub New(ByVal dbName As String)
        SessionFactoryName = dbName
    End Sub

#End Region

#Region " Properties "

    Public ResolutionType As ResolutionType

    Public Property FromDate As DateTime?

    Public Property ToDate As DateTime?

    Public Status As PECOCStatus?

#End Region

#Region " NHibernate Properties "
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property
#End Region

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "P")

        If ResolutionType IsNot Nothing Then
            criteria.Add(Restrictions.Eq("P.ResolutionType", ResolutionType))
        End If

        If FromDate.HasValue Then
            criteria.Add(Restrictions.Eq("P.FromDate", FromDate.Value))
        End If

        If ToDate.HasValue Then
            criteria.Add(Restrictions.Eq("P.ToDate", ToDate.Value))
        End If

        If Status.HasValue Then
            criteria.Add(Restrictions.Eq("P.Status", Status.Value))
        End If

        criteria.Add(Restrictions.Eq("P.IsActive", Convert.ToInt16(1)))

        Return criteria
    End Function

#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As IList(Of PECOC)
        Dim criteria As ICriteria = Me.CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Descending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of PECOC)()
    End Function
#End Region
End Class
