Imports NHibernate
Imports NHibernate.Criterion
Imports System.ComponentModel
Imports System.Linq
Imports System.Text.RegularExpressions
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.NHibernateManager



<Serializable(), DataObject()> _
Public Class NHibernateProtocolContactFinder2
    Inherits NHibernateBaseFinder(Of ProtocolContact, ProtocolContact)

#Region " Constructors "

    Public Sub New()
        SessionFactoryName = "ProtDB"
        DescriptionSearchBehaviour = New TextSearchBehaviour()
    End Sub

#End Region

#Region " Properties "

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property IdContactIn As Integer()
    Public Property Year As Short?
    Public Property IdProtocolIn As ICollection(Of Guid)
    Public Property Description As String
    Public Property DescriptionSearchBehaviour As TextSearchBehaviour

#End Region

#Region " Methods "

    Public Overrides Function DoSearch() As IList(Of ProtocolContact)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        Return criteria.List(Of ProtocolContact)()
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolContact)("PC")
        criteria.CreateAliasIfNotExists("PC.Contact", "C")
        criteria.CreateAliasIfNotExists("PC.Protocol", "P")
        Return criteria
    End Function
    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)
        If Not IdContactIn.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("PC.Contact.Id", IdContactIn))
        End If
        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("P.Year", Year))
        End If
        If Not IdProtocolIn.IsNullOrEmpty() Then
            criteria.Add(Restrictions.InG("P.Id", IdProtocolIn))
        End If
        If Not String.IsNullOrEmpty(Description) Then
            Dim descriptionLike As AbstractCriterion = AbstractCriterionBuilder.TextLike("C.Description", Description.Trim(), DescriptionSearchBehaviour)
            criteria.Add(descriptionLike)
        End If
    End Sub

#End Region

End Class