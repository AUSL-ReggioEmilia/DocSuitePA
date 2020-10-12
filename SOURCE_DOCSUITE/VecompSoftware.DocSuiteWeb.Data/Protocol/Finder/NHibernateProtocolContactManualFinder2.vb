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
Public Class NHibernateProtocolContactManualFinder2
    Inherits NHibernateBaseFinder(Of ProtocolContactManual, ProtocolContactManual)

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
    Public Property IdProtocolIn As YearNumberCompositeKey()
    Public Property Description As String
    Public Property DescriptionSearchBehaviour As TextSearchBehaviour

#End Region

#Region " Methods "

    Public Overrides Function DoSearch() As IList(Of ProtocolContactManual)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        Return criteria.List(Of ProtocolContactManual)()
    End Function
    Public Function GetProtocolKeys() As IList(Of YearNumberCompositeKey)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        criteria.SetProjection(Projections.ProjectionList() _
            .Add(Projections.Property("P.Year"), "Year") _
            .Add(Projections.Property("P.Number"), "Number"))
        Return criteria.List(Of YearNumberCompositeKey)()
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolContactManual)("PCM")
        criteria.CreateAliasIfNotExists("PCM.Protocol", "P")
        Return criteria
    End Function
    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)
        If Not IdContactIn.IsNullOrEmpty() Then
            criteria.Add(Restrictions.In("PCM.Id", IdContactIn))
        End If
        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("P.Year", Year))
        End If
        If Not IdProtocolIn.IsNullOrEmpty() Then
            Dim disj As New Disjunction()
            disj.Add(Expression.Sql("1=0"))
            Dim restrictions As IEnumerable(Of AbstractCriterion) = IdProtocolIn.GroupBy(Function(g) g.Year.Value).Select(Function(g) GetRestrictionFrom(g))
            restrictions.ToList().ForEach(Sub(r) disj.Add(r))
            criteria.Add(disj)
        End If
        If Not String.IsNullOrEmpty(Description) Then
            Dim descriptionLike As AbstractCriterion = AbstractCriterionBuilder.TextLike("PCM.Contact.Description", Description.Trim(), DescriptionSearchBehaviour)
            criteria.Add(descriptionLike)
        End If
    End Sub

    Private Function GetRestrictionFrom(grouping As IGrouping(Of Short, YearNumberCompositeKey)) As AbstractCriterion
        Return Restrictions.And(Restrictions.Eq("P.Year", grouping.Key), Restrictions.In("P.Number", grouping.Select(Function(k) k.Number).ToArray()))
    End Function

#End Region

End Class