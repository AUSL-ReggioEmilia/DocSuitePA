Imports NHibernate.SqlCommand
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate.Criterion
Imports NHibernate
Imports VecompSoftware.NHibernateManager

<Serializable()>
Public Class NHMessageEmailFinder
    Inherits NHibernateBaseFinder(Of MessageEmail, MessageEmail)

#Region " Properties "

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Property Unsent As Boolean
    Public Property SentFrom As DateTime? = Nothing
    Public Property SentTo As DateTime? = Nothing
    Public Property SenderEmail As String = Nothing
    Public Property RecipientEmail As String = Nothing
    Public Property Subject As String = Nothing

#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

#End Region

#Region " Methods "

    Public Overloads Overrides Function DoSearch() As IList(Of MessageEmail)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        Return criteria.List(Of MessageEmail)()
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "ME")

        ' Filtro per data spedizione.
        If SentFrom.HasValue OrElse SentTo.HasValue OrElse Unsent Then
            Dim mailDateCriterion As ICriterion = Nothing

            If SentFrom.HasValue AndAlso SentTo.HasValue Then
                mailDateCriterion = Restrictions.Between("ME.SentDate", SentFrom.Value.BeginOfTheDay(), SentTo.Value.EndOfTheDay())
                If SentTo.Value.EndOfTheDay() < SentFrom.Value Then
                    mailDateCriterion = Restrictions.Between("ME.SentDate", SentTo.Value.BeginOfTheDay(), SentFrom.Value.EndOfTheDay())
                End If

            ElseIf SentFrom.HasValue Then
                mailDateCriterion = Restrictions.Ge("ME.SentDate", SentFrom.Value.BeginOfTheDay())

            ElseIf SentTo.HasValue Then
                mailDateCriterion = Restrictions.Le("ME.SentDate", SentTo.Value.EndOfTheDay())

            End If

            Dim unsentMailsCriterion As ICriterion = Nothing
            If Unsent Then
                unsentMailsCriterion = Restrictions.IsNull("ME.SentDate")
            End If

            If mailDateCriterion IsNot Nothing AndAlso unsentMailsCriterion IsNot Nothing Then
                mailDateCriterion = Restrictions.Or(mailDateCriterion, unsentMailsCriterion)
            End If

            If mailDateCriterion IsNot Nothing Then
                criteria.Add(mailDateCriterion)
            End If
        End If

        ' Filtro senders
        If Not String.IsNullOrWhiteSpace(SenderEmail) Then
            criteria.CreateAliasIfNotExists("ME.Message", "MEM", JoinType.InnerJoin)
            criteria.SetFetchMode("Message.MessageContacts", FetchMode.Lazy)

            Dim dcSender As DetachedCriteria = DetachedCriteria.For(Of MessageContact)("MEMSender")
            dcSender.Add(Restrictions.EqProperty("MEMSender.Message.Id", "MEM.Id"))

            dcSender.CreateAlias("MEMSender.ContactEmails", "MEMSenderCE", JoinType.InnerJoin)

            dcSender.Add(Restrictions.Eq("MEMSender.ContactPosition", MessageContact.ContactPositionEnum.Sender))
            dcSender.Add(Restrictions.Like("MEMSenderCE.Email", SenderEmail, MatchMode.Anywhere))
            dcSender.SetProjection(Projections.Constant(True))

            criteria.Add(Subqueries.Exists(dcSender))
        End If

        ' Filtro recipients
        If Not String.IsNullOrWhiteSpace(RecipientEmail) Then
            criteria.CreateAliasIfNotExists("ME.Message", "MEM", JoinType.InnerJoin)
            criteria.SetFetchMode("Message.MessageContacts", FetchMode.Lazy)

            Dim dcRecipient As DetachedCriteria = DetachedCriteria.For(Of MessageContact)("MEMRecipient")
            dcRecipient.Add(Restrictions.EqProperty("MEMRecipient.Message.Id", "MEM.Id"))

            dcRecipient.CreateAlias("MEMRecipient.ContactEmails", "MEMRecipientCE", JoinType.InnerJoin)

            dcRecipient.Add(Restrictions.In("MEMRecipient.ContactPosition", {MessageContact.ContactPositionEnum.Recipient, MessageContact.ContactPositionEnum.RecipientCc, MessageContact.ContactPositionEnum.RecipientBcc}))
            dcRecipient.Add(Restrictions.Like("MEMRecipientCE.Email", RecipientEmail, MatchMode.Anywhere))
            dcRecipient.SetProjection(Projections.Constant(True))
            
            criteria.Add(Subqueries.Exists(dcRecipient))
        End If

        ' Filtro subject
        If Not String.IsNullOrWhiteSpace(Subject) Then
            criteria.Add(Restrictions.Like("ME.Subject", Subject, MatchMode.Anywhere))
        End If

        ' Filtri griglia
        AttachFilterExpressions(criteria)

        Return criteria
    End Function

    Protected Sub DecorateCriteria(ByRef criteria As ICriteria)
        criteria.ClearOrders()

        If Not AttachSortExpressions(criteria) Then
            criteria.AddOrder(Order.Desc("ME.Id"))
        End If

        If EnablePaging Then
            criteria.SetFirstResult(_startIndex)
            criteria.SetMaxResults(_pageSize)
        End If
    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetProjection(Projections.CountDistinct("ME.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

#End Region

End Class
