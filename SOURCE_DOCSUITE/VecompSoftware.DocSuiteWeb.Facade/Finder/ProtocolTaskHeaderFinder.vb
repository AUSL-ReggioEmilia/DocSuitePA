Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate.SqlCommand
Imports NHibernate.Transform
Imports VecompSoftware.Helpers.NHibernate


<Serializable()> _
Public Class ProtocolTaskHeaderFinder
    Inherits NHibernateProtocolFinder


#Region " Fields "

    Private _factory As FacadeFactory

#End Region

#Region " Properties "

    Public Property ContactDescription As String

    Public Property ContactDescriptionSearchBehaviour As TextSearchBehaviour

    Public Property TaskHeaderIdIn As IEnumerable(Of Integer)

    Private ReadOnly Property Factory As FacadeFactory
        Get
            If _factory Is Nothing Then
                _factory = New FacadeFactory("ProtDB")
            End If
            Return _factory
        End Get
    End Property

#End Region

#Region " Methods "

    Public Overrides Function DoSearchHeader() As IList(Of ProtocolHeader)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)

        Dim result As IList(Of ProtocolHeader)

        DecorateCriteria(criteria)
        SetProjectionHeaders(criteria)
        AttachSortExpressions(criteria)
        result = criteria.List(Of ProtocolHeader)()
        Dim conversion As IList(Of ProtocolHeader) = New List(Of ProtocolHeader)(result.Count)
        For Each a As ProtocolHeader In result
            a.RegistrationDate = a.RegistrationDate.ToLocalTime()
            conversion.Add(a)
        Next
        Return conversion
    End Function

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        Dim result As Integer = 0
        EnablePaging = False
        DecorateCriteria(criteria)

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.GroupProperty("P.Id"))
        proj.Add(Projections.Alias(Projections.RowCount(), "Count"))
        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ProtocolCount))
        result = criteria.List(Of ProtocolCount)().Count

        If TopMaxRecords > 0 Then
            Return Math.Min(result, TopMaxRecords)
        End If
        EnablePaging = True
        Return result
    End Function

    Protected Overrides Sub DecorateCriteria(ByRef criteria As ICriteria)
        MyBase.DecorateCriteria(criteria)

        If Not String.IsNullOrWhiteSpace(Me.ContactDescription) Then

            MyBase.AddJoinAlias(criteria, "Contacts", "PC", JoinType.LeftOuterJoin)

            MyBase.AddJoinAlias(criteria, "PC.Contact", "PCC", JoinType.LeftOuterJoin)

            MyBase.AddJoinAlias(criteria, "P.ManualContacts", "MPC", JoinType.LeftOuterJoin)

            Dim disj As New Disjunction()

            disj.Add(Restrictions.And(Restrictions.IsNotNull("PC.Id"), AbstractCriterionBuilder.TextLike("PCC.Description", ContactDescription.Trim(), ContactDescriptionSearchBehaviour)))

            disj.Add(Restrictions.And(Restrictions.IsNotNull("MPC.Id"), AbstractCriterionBuilder.TextLike("MPC.Contact.Description", ContactDescription.Trim(), ContactDescriptionSearchBehaviour)))
            criteria.Add(disj)

        End If

        If Not Me.TaskHeaderIdIn.IsNullOrEmpty() Then
            criteria.CreateAlias("TaskHeader", "THP", JoinType.InnerJoin)
            criteria.Add(Restrictions.In("THP.Id", TaskHeaderIdIn.ToArray()))
        End If

    End Sub

#End Region

End Class
