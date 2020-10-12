Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel


<Serializable(), DataObject()> _
Public Class NHibernateProtocolContactManualFinder
    Inherits NHibernateBaseFinder(Of ProtocolContactManual, ProtocolContactManual)

#Region " Constructor "

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

#End Region

#Region " Fields "
    Private _description As String = String.Empty
#End Region

#Region "Finder Properties"

    Public Property Description() As String
        Get
            Return _description.Replace("_", " ")
        End Get
        Set(ByVal value As String)
            _description = value.Replace(" ", "_")
        End Set
    End Property

    Public Property DescriptionContain As Boolean

    Public Property Year As Short?
    Public Property Number As Integer?

#End Region

#Region " NHibernate Properties "

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

#Region " Methods "
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "PCM")
        Dim recipientMatchMode As MatchMode = If(DescriptionContain, MatchMode.Anywhere, MatchMode.Start)

        If Not String.IsNullOrEmpty(Description) Then
            criteria.Add(Expression.Like("Contact.Description", Description, recipientMatchMode))
        End If

        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("Protocol.Year", Year.Value))
        End If

        If Number.HasValue Then
            criteria.Add(Restrictions.Eq("Protocol.Number", Number.Value))
        End If

        Return criteria
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of ProtocolContactManual)
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of ProtocolContactManual)()
    End Function
#End Region
End Class