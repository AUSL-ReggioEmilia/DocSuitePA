Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform

Public Class NHibernateContainerPropertyDao
    Inherits BaseNHibernateDao(Of ContainerProperty)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Ritorna tutte le proprietà dinamiche (del contenitore) con un determinato nome </summary>

    Public Function GetPropertyByName(ByVal Name As String) As IList(Of ContainerProperty)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Name", Name))
        criteria.Add(Restrictions.IsNotNull("Id"))
        criteria.AddOrder(Order.Asc("Id"))
        Return criteria.List(Of ContainerProperty)()
    End Function

    ''' <summary> Ritorna tutte le proprietà dinamiche associate al contenitore </summary>
    Public Function GetPropertyByContainerId(ByVal idContainer As Integer) As IList(Of ContainerProperty)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        criteria.Add(Restrictions.IsNotNull("Id"))
        criteria.AddOrder(Order.Asc("Id"))
        Return criteria.List(Of ContainerProperty)()
    End Function


    ''' <summary> Ritorna tutte la proprietà dinamiche associate al contenitore </summary>
    Public Function GetPropertyByNameAndContainer(propertyName As String, idContainer As Integer) As ContainerProperty
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container.Id", idContainer))
        criteria.Add(Restrictions.Eq("Name", propertyName))
        criteria.Add(Restrictions.IsNotNull("Id"))
        criteria.AddOrder(Order.Asc("Id"))
        Return criteria.UniqueResult(Of ContainerProperty)()
    End Function
#End Region

End Class
