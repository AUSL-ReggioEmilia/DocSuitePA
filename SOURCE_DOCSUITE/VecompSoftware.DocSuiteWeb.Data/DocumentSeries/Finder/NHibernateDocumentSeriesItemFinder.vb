Imports NHibernate.SqlCommand
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports System.ComponentModel
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Criterion
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.Helpers

<Serializable(), DataObject()>
Public Class NHibernateDocumentSeriesItemFinder(Of THeader)
    Inherits NHibernateBaseFinder(Of DocumentSeriesItem, THeader)

#Region " Constructors "

    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        EnablePaging = True
        IdOwnerRoleIn = New List(Of Integer)()
        LinkTypes = New List(Of DocumentSeriesItemLinkType)()
    End Sub

#End Region

#Region " Properties "

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Property LinkTypes As List(Of DocumentSeriesItemLinkType)

    Public Property LinkContains As String

    Public Property IdMainIn As List(Of Guid)
    Public Property IdAnnexedIn As List(Of Guid)

    Public Property Year As Integer?
    Public Property NumberFrom As Integer?
    Public Property NumberTo As Integer?

    Public Property IdArchive() As Integer?
    Public Property IdDocumentSeriesIn As List(Of Integer)

    Public Property RegistrationDateFrom As DateTimeOffset?
    Public Property RegistrationDateTo As DateTimeOffset?

    Public Property PublishingDateFrom As DateTime?
    Public Property PublishingDateTo As DateTime?

    Public Property RetireDateFrom As DateTime?
    Public Property RetireDateTo As DateTime?

    Public Property IsPublished As Boolean?
    Public Property IsRetired As Boolean?
    Public Property IsPriority As Boolean?

    Public Property SubjectContains As String
    Public Property SubjectStartsWith As String

    Public Property CategoryPath As String

    Public Property ItemStatusIn As List(Of DocumentSeriesItemStatus)

    Public Property IdSubsectionIn As List(Of Integer)
    Public Property IdOwnerRoleIn As List(Of Integer)

    Public Property LogDate As Range(Of DateTime)
    Public Property LastModifiedSortingView As Boolean?

#End Region

#Region " Methods "

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        criteria.SetProjection(Projections.CountDistinct("DSI.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function
    Public Overloads Overrides Function DoSearch() As IList(Of DocumentSeriesItem)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        Return criteria.List(Of DocumentSeriesItem)()
    End Function

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub
    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)
        If IdMainIn IsNot Nothing AndAlso IdMainIn.Count > 0 Then
            criteria.Add(XmlInCriterion.Create("DSI.IdMain", IdMainIn))
        End If

        If IdAnnexedIn IsNot Nothing AndAlso IdAnnexedIn.Count > 0 Then
            criteria.Add(Restrictions.In("DSI.IdAnnexed", IdAnnexedIn.ToArray()))
        End If

        If Year.HasValue Then

            If ItemStatusIn.Contains(DocumentSeriesItemStatus.Draft) Then
                Dim disj As New Disjunction()
                disj.Add(Restrictions.Eq("DSI.Year", Year))
                Dim disjDraft As New Conjunction()
                disjDraft.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft))
                disjDraft.Add(Restrictions.Between("DSI.RegistrationDate", New DateTimeOffset(Year.Value, 1, 1, 0, 0, 0, DateTimeOffset.UtcNow.Offset), New DateTimeOffset(Year.Value, 12, 31, 0, 0, 0, DateTimeOffset.UtcNow.Offset)))
                disj.Add(disjDraft)
                criteria.Add(disj)
            Else
                criteria.Add(Restrictions.Eq("DSI.Year", Year))
            End If



        End If

        InnerNumericRange(NumberFrom, NumberTo)
        If NumberFrom.HasValue AndAlso NumberTo.HasValue Then
            criteria.Add(Restrictions.Between("DSI.Number", NumberFrom, NumberTo))
        ElseIf NumberFrom.HasValue Then
            criteria.Add(Restrictions.Ge("DSI.Number", NumberFrom))
        ElseIf NumberTo.HasValue Then
            criteria.Add(Restrictions.Le("DSI.Number", NumberTo))
        End If

        If IdArchive.HasValue Then
            criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
            criteria.CreateAliasIfNotExists("DS.Container", "DISCO", JoinType.InnerJoin)
            criteria.CreateAliasIfNotExists("DISCO.Archive", "DISCOCA")
            criteria.Add(Restrictions.Eq("DISCOCA.Id", IdArchive.Value))
        End If

        If IdDocumentSeriesIn IsNot Nothing AndAlso IdDocumentSeriesIn.Count > 0 Then
            criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
            criteria.Add(Restrictions.In("DS.Id", IdDocumentSeriesIn.ToArray()))
        End If

        InnerDateTimeOffsetRange(RegistrationDateFrom, RegistrationDateTo)
        If RegistrationDateFrom.HasValue AndAlso RegistrationDateTo.HasValue Then
            criteria.Add(Restrictions.Between("DSI.RegistrationDate", RegistrationDateFrom.Value, RegistrationDateTo.Value.AddDays(1).AddSeconds(-1)))
        ElseIf RegistrationDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("DSI.RegistrationDate", RegistrationDateFrom.Value))
        ElseIf RegistrationDateTo.HasValue Then
            criteria.Add(Restrictions.Le("DSI.RegistrationDate", RegistrationDateTo.Value.AddDays(1).AddSeconds(-1)))
        End If

        InnerDateTimeRange(PublishingDateFrom, PublishingDateTo)
        If PublishingDateFrom.HasValue AndAlso PublishingDateTo.HasValue Then
            criteria.Add(Restrictions.Between("DSI.PublishingDate", PublishingDateFrom, PublishingDateTo.Value.AddDays(1).AddSeconds(-1)))
        ElseIf PublishingDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("DSI.PublishingDate", PublishingDateFrom))
        ElseIf PublishingDateTo.HasValue Then
            criteria.Add(Restrictions.Le("DSI.PublishingDate", PublishingDateTo.Value.AddDays(1).AddSeconds(-1)))
        End If

        InnerDateTimeRange(RetireDateFrom, RetireDateTo)
        If RetireDateFrom.HasValue AndAlso RetireDateTo.HasValue Then
            criteria.Add(Restrictions.Between("DSI.RetireDate", RetireDateFrom, RetireDateTo.Value.AddDays(1).AddSeconds(-1)))
        ElseIf RetireDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("DSI.RetireDate", RetireDateFrom))
        ElseIf RetireDateTo.HasValue Then
            criteria.Add(Restrictions.Le("DSI.RetireDate", RetireDateTo.Value.AddDays(1).AddSeconds(-1)))
        End If

        If Me.IsPublished.HasValue Then
            criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
            If Me.IsPublished Then
                criteria.Add(Restrictions.Eq("DS.PublicationEnabled", True))
                criteria.Add(Me.GetIsPublishedConjunction())
            Else
                Dim disj As New Disjunction()
                disj.Add(Restrictions.IsNull("DS.PublicationEnabled"))
                disj.Add(Restrictions.Eq("DS.PublicationEnabled", False))
                disj.Add(Restrictions.IsNull("DSI.PublishingDate"))
                disj.Add(Restrictions.Gt("DSI.PublishingDate", DateTime.Today))
                criteria.Add(disj)
            End If
        End If

        If IsRetired.HasValue Then
            If IsRetired.Value Then
                criteria.Add(Restrictions.IsNotNull("DSI.RetireDate"))
                criteria.Add(Restrictions.Le("DSI.RetireDate", DateTime.Today))
            Else
                Dim disj As New Disjunction()
                disj.Add(Restrictions.IsNull("DSI.RetireDate"))
                disj.Add(Restrictions.Gt("DSI.RetireDate", DateTime.Today))
                criteria.Add(disj)
            End If
        End If

        If IsPriority.HasValue Then
            criteria.Add(Restrictions.Eq("DSI.Priority", IsPriority.Value))
        End If

        If Not String.IsNullOrEmpty(SubjectContains) Then
            criteria.Add(Restrictions.Like(Projections.Property("DSI.Subject"), SubjectContains, MatchMode.Anywhere))
        End If
        If Not String.IsNullOrEmpty(SubjectStartsWith) Then
            criteria.Add(Restrictions.Like(Projections.Property("DSI.Subject"), SubjectStartsWith, MatchMode.Start))
        End If

        If Not String.IsNullOrEmpty(CategoryPath) Then

            If CategoryPath.Split("|").Length() = 1 Then

                criteria.CreateAlias("DSI.Category", "CA")
            Else
                criteria.CreateAlias("DSI.SubCategory", "CA")
            End If

            criteria.Add(Restrictions.Eq(Projections.Property("CA.FullIncrementalPath"), CategoryPath))
        End If

        If Not ItemStatusIn.IsNullOrEmpty() Then
            Dim disj As New Disjunction()
            For Each status As DocumentSeriesItemStatus In ItemStatusIn
                disj.Add(Restrictions.Eq("DSI.Status", status))
            Next

            If ItemStatusIn.Contains(DocumentSeriesItemStatus.Draft) AndAlso Year.HasValue Then
                Dim disjDraft As New Conjunction()
                disjDraft.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft))
                disjDraft.Add(Restrictions.Between("DSI.RegistrationDate", New DateTimeOffset(Year.Value, 1, 1, 0, 0, 0, New TimeSpan(2, 0, 0)), New DateTimeOffset(Year.Value, 12, 31, 0, 0, 0, New TimeSpan(2, 0, 0))))
                disj.Add(disjDraft)
            End If
            criteria.Add(disj)
        End If

        If Not IdSubsectionIn.IsNullOrEmpty() Then
            criteria.CreateAlias("DSI.DocumentSeriesSubsection", "DSS")
            If IdSubsectionIn.HasSingleItem Then
                criteria.Add(Restrictions.Eq("DSS.Id", IdSubsectionIn(0)))
            Else
                criteria.Add(Restrictions.In("DSS.Id", IdSubsectionIn.ToArray()))
            End If
        End If

        If Not IdOwnerRoleIn.IsNullOrEmpty() Then
            criteria.CreateAliasIfNotExists("DSI.DocumentSeriesItemRoles", "DSIR", JoinType.InnerJoin)
            criteria.Add(Restrictions.Eq("DSIR.LinkType", DocumentSeriesItemRoleLinkType.Owner))
            If IdOwnerRoleIn.HasSingleItem Then
                criteria.Add(Restrictions.Eq("DSIR.Role.Id", IdOwnerRoleIn(0)))
            Else
                criteria.Add(Restrictions.In("DSIR.Role.Id", IdOwnerRoleIn.ToArray()))
            End If
        End If

        If LogDate IsNot Nothing AndAlso LogDate.HasValue Then
            criteria.CreateAliasIfNotExists("DSI.Logs", "DSIL")
            criteria.AddRestrictionsBetween("DSIL.LogDate", LogDate)
        End If

        If Not LinkTypes.IsNullOrEmpty() Then
            criteria.CreateAliasIfNotExists("DSI.DocumentSeriesItemLinks", "DSIK", JoinType.InnerJoin)
            criteria.Add(Restrictions.IsNotNull("DSIK.LinkType"))
            Dim disj As New Disjunction()
            For Each linkType As DocumentSeriesItemLinkType In LinkTypes
                disj.Add(Restrictions.Eq("DSIK.LinkType", linkType))
            Next
            criteria.Add(disj)

        End If

        If DocSuiteContext.Current.IsResolutionEnabled AndAlso Not String.IsNullOrEmpty(LinkContains) AndAlso Not String.IsNullOrWhiteSpace(LinkContains) Then
            Dim projList As ProjectionList = Projections.ProjectionList()
            criteria.CreateAliasIfNotExists("DSI.DocumentSeriesItemLinks", "DSIK", JoinType.InnerJoin)
            criteria.CreateAliasIfNotExists("DSIK.Resolution", "R", JoinType.InnerJoin)
            Dim disj As New Disjunction()
            Dim columns As New List(Of String)() From {
                "R.InclusiveNumber", "R.AdoptionUser", "R.ResolutionObject", "R.Note", "R.ServiceNumber",
                "R.PublishingUser", "R.Leaveuser", "R.AlternativeProposer", "R.AlternativeManager",
                "R.AlternativeAssignee", "R.AlternativeRecipient", "R.EffectivenessUser"
            }
            Dim lstr As String = String.Concat("%", LinkContains, "%")
            For Each column As String In columns
                disj.Add(Restrictions.Like(column, lstr))
            Next
            criteria.Add(disj)
        End If
    End Sub

    Protected Function GetIsPublishedConjunction() As Conjunction
        Dim conj As New Conjunction()
        conj.Add(IsNotDraftDisjunction())
        conj.Add(Restrictions.IsNotNull("DSI.PublishingDate"))
        conj.Add(Restrictions.Le("DSI.PublishingDate", DateTime.Today))

        Dim disj As New Disjunction()
        disj.Add(Restrictions.IsNull("DSI.RetireDate"))
        disj.Add(Restrictions.Gt("DSI.RetireDate", DateTime.Today))
        conj.Add(disj)

        Return conj
    End Function
    Protected Function IsNotDraftDisjunction() As Disjunction
        Dim disjStatus As New Disjunction()
        disjStatus.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Active))
        disjStatus.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Canceled))
        Return disjStatus
    End Function
    Protected Overrides Function AttachSortExpressions(ByRef criteria As ICriteria) As Boolean
        If SortExpressions.Count = 0 Then
            Dim defaultSort As New Dictionary(Of String, String) From {{"DSI.Status", "DESC"}, {"DSI.Year", "DESC"}, {"DSI.Number", "DESC"}, {"DSI.Id", "DESC"}}
            For Each item As KeyValuePair(Of String, String) In defaultSort
                SortExpressions.Add(item.Key, item.Value)
            Next
        End If

        If LastModifiedSortingView.HasValue AndAlso LastModifiedSortingView.Value Then
            SortExpressions.Clear()
            criteria.AddOrder(Order.Desc(Projections.Conditional(
            Restrictions.Or(Restrictions.GtProperty("PublishingDate", "LastChangedDate"), Restrictions.IsNull("LastChangedDate")),
            Projections.Cast(NHibernateUtil.DateTimeOffset, Projections.Property("PublishingDate")), Projections.Property("LastChangedDate"))))
        End If
        Return MyBase.AttachSortExpressions(criteria)
    End Function
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItem)("DSI")
        Return criteria
    End Function

    Private Sub InnerDateTimeRange(ByRef rangeFrom As DateTime?, ByRef rangeTo As DateTime?)
        If Not (rangeFrom.HasValue AndAlso rangeTo.HasValue) Then
            Return
        End If
        If rangeTo.Value.AddDays(1).AddSeconds(-1) < rangeFrom Then
            Dim tempFrom As DateTime? = rangeFrom
            rangeFrom = rangeTo
            rangeTo = tempFrom
        End If
    End Sub

    Private Sub InnerDateTimeOffsetRange(ByRef rangeFrom As DateTimeOffset?, ByRef rangeTo As DateTimeOffset?)
        If Not (rangeFrom.HasValue AndAlso rangeTo.HasValue) Then
            Return
        End If
        If rangeTo.Value.AddDays(1).AddSeconds(-1) < rangeFrom Then
            Dim tempFrom As DateTimeOffset? = rangeFrom
            rangeFrom = rangeTo
            rangeTo = tempFrom
        End If
    End Sub

    Private Sub InnerNumericRange(ByRef rangeFrom As Integer?, ByRef rangeTo As Integer?)
        If Not (rangeFrom.HasValue AndAlso rangeTo.HasValue) Then
            Return
        End If
        If rangeTo < rangeFrom Then
            Dim tempFrom As Integer? = rangeFrom
            rangeFrom = rangeTo
            rangeTo = tempFrom
        End If
    End Sub

#End Region

End Class
