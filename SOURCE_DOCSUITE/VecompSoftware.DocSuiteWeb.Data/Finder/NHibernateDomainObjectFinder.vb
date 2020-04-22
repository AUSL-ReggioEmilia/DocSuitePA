Imports System.IO
Imports System.Linq
Imports NHibernate.Impl
Imports NHibernate
Imports System.ComponentModel
Imports VecompSoftware.NHibernateManager
Imports NHibernate.Criterion
Imports NHibernate.Impl.CriteriaImpl

<Serializable(), DataObject()>
Public Class NHibernateDomainObjectFinder(Of T)
    Inherits NHibernateBaseFinder(Of T, T)

#Region "Fields"
    Private _dcSerialize As Byte()
#End Region

#Region "NHibernate Properties"

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

    Public Sub New(ByVal criteria As CriteriaImpl, ByVal dbName As String)
        SessionFactoryName = dbName

        Dim dc As DetachedCriteria = CreateDetachFromCriteria(criteria, GetType(T))

        _dcSerialize = SerializeDC(dc)
    End Sub

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim dc As DetachedCriteria = DeSerializeDC(_dcSerialize)

        Dim criteria As ICriteria = dc.GetExecutableCriteria(NHibernateSession)

        MyBase.AttachFilterExpressions(criteria)

        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of T)
        Dim criteria As ICriteria = CreateCriteria()

        If SortExpressions.Count > 0 Then
            Dim orderList As IList(Of OrderEntry) = CType(criteria, CriteriaImpl).IterateOrderings().ToList()
            orderList.Clear()
            MyBase.AttachSortExpressions(criteria)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of T)()
    End Function

    Private Function SerializeDC(ByVal dc As DetachedCriteria) As Byte()
        Dim ms As New MemoryStream
        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
        bf.Serialize(ms, dc)
        Return ms.ToArray()
    End Function

    Private Function DeSerializeDC(ByVal buffer As Byte()) As Object
        Dim ms As New MemoryStream(buffer)
        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
        Return bf.Deserialize(ms)
    End Function

End Class
