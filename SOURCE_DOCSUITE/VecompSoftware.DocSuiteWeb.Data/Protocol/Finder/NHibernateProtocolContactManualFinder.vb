Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel


<Serializable(), DataObject()> _
Public Class NHibernateProtocolContactManualFinder
    Inherits NHibernateBaseFinder(Of ProtocolContactManual, ProtocolContactManual)

#Region "New"

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

#End Region

#Region "Private Fields"
    Private _year As Int16?
    Private _number As Integer?

    Private _description As String = String.Empty
    Private _descriptionContain As Boolean = False

#End Region

#Region "Finder Properties"

    Property Description() As String
        Get
            Return _description.Replace("_", " ")
        End Get
        Set(ByVal value As String)
            _description = value.Replace(" ", "_")
        End Set
    End Property

    Property DescriptionContain() As Boolean
        Get
            Return _descriptionContain
        End Get
        Set(value As Boolean)
            _descriptionContain = value
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

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "PCM")
        Dim recipientMatchMode As MatchMode = If(DescriptionContain, MatchMode.Anywhere, MatchMode.Start)

        If Not String.IsNullOrEmpty(Description) Then
            criteria.Add(Expression.Like("Contact.Description", Description, recipientMatchMode))
        End If

        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("Id.Year", Year.Value))
        End If

        If Number.HasValue Then
            criteria.Add(Restrictions.Eq("Id.Number", Number.Value))
        End If

        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of ProtocolContactManual)
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of ProtocolContactManual)()
    End Function
End Class