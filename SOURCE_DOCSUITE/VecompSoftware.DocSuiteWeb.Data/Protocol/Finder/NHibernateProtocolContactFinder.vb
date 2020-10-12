Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel


<Serializable(), DataObject()> _
Public Class NHibernateProtocolContactFinder
    Inherits NHibernateBaseFinder(Of ProtocolContact, ProtocolContact)

#Region "New"

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

#End Region

#Region "Private Fields"

    Private _idContacts As List(Of Integer)

    Private _year As Int16?
    Private _number As Integer?

#End Region

#Region "Finder Properties"

    Public Property IdContacts() As List(Of Integer)
        Get
            Return _idContacts
        End Get
        Set(value As List(Of Integer))
            _idContacts = value
        End Set
    End Property
    Property Year As Int16?
        Get
            Return _year
        End Get
        Set(value As Int16?)
            _year = value
        End Set
    End Property
    Property Number As Integer?
        Get
            Return _number
        End Get
        Set(value As Integer?)
            _number = value
        End Set
    End Property


#End Region

#Region "NHibernate Properties"

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "PC")

        If Not IdContacts Is Nothing Then
            criteria.Add(Expression.In("Contact.Id", IdContacts))
        End If

        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("Protocol.Year", Year.Value))
        End If

        If Number.HasValue Then
            criteria.Add(Restrictions.Eq("Protocol.Number", Number.Value))
        End If

        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As System.Collections.Generic.IList(Of ProtocolContact)
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of ProtocolContact)()
    End Function
End Class
