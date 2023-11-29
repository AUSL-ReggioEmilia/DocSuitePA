Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports NHibernate.Transform
Imports VecompSoftware.DocSuiteWeb.Data.PEC.Finder
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Criterion

<Serializable()>
Public Class NHibernatePECMailFinder
    Inherits NHibernateBaseFinder(Of PECMail, PecMailHeader)

#Region " Fields "

#End Region

#Region " Properties "

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        End Get
    End Property

    Public Property TopMaxRecords As Integer

    Public Property EnablePaging As Boolean

    Public Property Year As Short?

    Public Property Number As Integer?

    Public Property RecordedInDocSuite As Short?

    Public Property OnlyNotRecorded As Boolean? = Nothing

    Public Property NotRead As Boolean? = Nothing

    Public Property Direction As Short?

    Public Property XTrasporto As String

    Public Property XRiferimentoMessageId As String

    Public Property MailSubject As String

    Public Property MailboxIds As Short()

    Public Property WithSegnatura As Boolean

    Public Property ReceiptCriteria As PECReceiptCriteria?

    Public Property ReceiptNotLinked As Boolean

    Public Property RegistrationUserCriteria As PECRegistrationUserCriteria?

    Public Property TaskHeaderCriteria As PECTaskHeaderCriteria?

    Public Property Roles As String

    Public Property Destinated As Boolean? = Nothing

    Public Property RecordedDateFrom As Date? = Nothing


    Public Property RecordedDateTo As Date? = Nothing

    ''' <summary> Pec attive </summary>
    Public Property Actives As Boolean?

    ''' <summary> Pec con anomalie JeepService </summary>
    Public Property Anomalies As Boolean?

    Public Property Handler As String

    ''' <summary> Definisce il mittente da cercare </summary>
    Public Property Sender As String

    Public Property RegistrationUser As String

    Public Property RegistrationUsers As String()

    Public Property Recipient As String

    Public Property InvertedSort As Boolean

    Public Property MailDateFrom As Date? = Nothing


    Public Property MailDateTo As Date? = Nothing

    Public Property UnsentMails As Boolean

    ''' <summary> Definisce se devono essere incluse anche le PEC da splittare </summary>
    ''' <remarks> Nella grafica dovrebbe essere false (come di default) e nel JeepService invece true </remarks>
    Public Property IncludeMultiples As Boolean

    Public Property IncludeNormalAndMultiples As Boolean = False

    Public Property PecMailTypeIncluded As List(Of PECMailType)

    ''' <summary> Definisce se cercare le PEC con checksum </summary>
    ''' <value>
    ''' --> nothing: non fa niente
    ''' --> true: SOLO pec con checksum
    ''' --> false: SOLO pec SENZA checksum
    ''' </value>
    Public Property WithChecksum As Boolean?

    ''' <summary> Definisce se cercare le PEC con originalMailBox </summary>
    ''' <value>
    ''' --> nothing: non fa niente
    ''' --> true: SOLO pec con originalMailBox
    ''' --> false: SOLO pec SENZA originalMailBox
    ''' </value>
    Public Property WithOriginalRecipient As Boolean?

    ''' <summary> Definisce una restrizione di MailBoxId per un particolare Sender </summary>
    ''' <remarks> funzionalità pensata per la visualizzazione "All" delle caselle di protocollazione coerentemente con l'impostazione "IsProtocolBoxExplicit" </remarks>
    Public Property MailBoxIdsBySender As KeyValuePair(Of String, Short())?

    Public Property TaskHeaderIdIn As IEnumerable(Of Integer)

    ''' <summary> Definisce se cercare le PEC con controllo sui settori </summary>
    Public Property CheckSecurityRight As Boolean? = Nothing
    Public Property OnlyProtocolBox As Boolean? = Nothing

#End Region

#Region " Constructors "

    Public Sub New()
        RecordedDateFrom = Nothing
        RecordedDateTo = Nothing
        MailDateFrom = Nothing
        MailDateTo = Nothing
        UnsentMails = False
        IncludeMultiples = False
        IncludeNormalAndMultiples = False
        EnablePaging = True
        OnlyProtocolBox = Nothing
    End Sub

#End Region

#Region " Methods "

    Protected Sub DecorateCriteria(ByRef criteria As ICriteria)
        criteria.ClearOrders()

        If Not AttachSortExpressions(criteria) Then
            If _InvertedSort Then
                criteria.AddOrder(Order.Asc("PM.MailDate"))
            Else
                criteria.AddOrder(Order.Desc("PM.MailDate"))
            End If
        End If

        If EnablePaging Then
            criteria.SetFirstResult(_startIndex)
            criteria.SetMaxResults(_pageSize)
        End If
    End Sub
    Private Sub DecorateSecurities(ByRef criteria As ICriteria)
        If CheckSecurityRight Then
            Dim roleIntIds As Integer() = New Integer() {}
            If Not String.IsNullOrEmpty(Roles) Then
                roleIntIds = Roles.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()
            End If
            Dim dcPecMailBoxId As DetachedCriteria = DetachedCriteria.For(GetType(PECMailBoxRole), "PMBR")
            dcPecMailBoxId.CreateAlias("PMBR.PECMailBox", "PMB", JoinType.InnerJoin)
            dcPecMailBoxId.Add(Restrictions.In("PMBR.Role.Id", roleIntIds))

            If OnlyProtocolBox.HasValue Then
                dcPecMailBoxId.Add(Restrictions.Eq("PMB.IsProtocolBox", OnlyProtocolBox.Value))
            End If
            dcPecMailBoxId.SetProjection(Projections.Property("PMBR.PECMailBox.Id"))

            criteria.Add(Subqueries.PropertyIn("PM.MailBox.Id", dcPecMailBoxId))
        End If
    End Sub
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "PM")
        criteria.CreateAlias("PM.DocumentUnit", "DU", JoinType.LeftOuterJoin)
        Dim dis As Disjunction = New Disjunction()
        Dim con As Conjunction = New Conjunction()

        If NotRead.HasValue AndAlso NotRead.Value Then
            Dim logCriteria As DetachedCriteria = DetachedCriteria.For(Of PECMailLog)("PML_I")
            logCriteria.SetProjection(Projections.Id)
            logCriteria.Add(Restrictions.EqProperty("PM.Id", "PML_I.Mail.Id"))
            logCriteria.Add(Restrictions.Eq("PML_I.Type", "Read"))
            criteria.Add(Subqueries.NotExists(logCriteria))
        End If

        DecorateSecurities(criteria)

        'Filtro Anno
        If Year.HasValue Then
            con.Add(Restrictions.Eq("PM.Year", Year.Value))
        End If

        'Filtro Numero
        If Number.HasValue Then
            con.Add(Restrictions.Eq("PM.Number", Number.Value))
        End If

        'Filtro RecordedInDocSuite
        If RecordedInDocSuite.HasValue Then
            con.Add(Restrictions.Eq("PM.RecordedInDocSuite", RecordedInDocSuite.Value))
        End If
        If OnlyNotRecorded.GetValueOrDefault(False) Then
            con.Add(Restrictions.IsNull("PM.RecordedInDocSuite"))
        End If

        ' Filtro pec in ingresso/pec in uscita
        If Direction.HasValue Then
            con.Add(Restrictions.Eq("PM.Direction", Direction.Value))
        End If

        If Actives.HasValue AndAlso Actives.Value Then
            con.Add(Restrictions.Eq("PM.IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        Else
            con.Add(Restrictions.Eq("PM.IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Delete)))
        End If

        'Filtro per Handler
        If Not String.IsNullOrEmpty(Handler) Then
            Dim disj As New Disjunction
            disj.Add(Restrictions.Eq("PM.Handler", Handler))
            disj.Add(Restrictions.Eq("PM.Handler", String.Empty))
            disj.Add(Restrictions.IsNull("PM.Handler"))
            con.Add(disj)
        End If

        If Direction.HasValue AndAlso Direction.Value = PECMailDirection.Ingoing Then
            con.Add(Restrictions.Not(Restrictions.Eq("PM.PECType", PECMailType.Receipt)))
        End If

        'Filtro MailType
        If Not String.IsNullOrEmpty(_XTrasporto) Then
            con.Add(Restrictions.Like("PM.XTrasporto", _XTrasporto))
        End If

        'Riferimento mailID
        If Not String.IsNullOrEmpty(_XRiferimentoMessageId) Then
            con.Add(Restrictions.Eq("PM.XRiferimentoMessageID", _XRiferimentoMessageId))
        End If

        'Filtro MailSubject
        If Not String.IsNullOrEmpty(_MailSubject) Then
            con.Add(Restrictions.Like("PM.MailSubject", _MailSubject, MatchMode.Anywhere))
        End If

        ' segnatura
        If _WithSegnatura Then
            con.Add(Restrictions.IsNotNull("PM.Segnatura"))
        End If

        'Filtro Sender
        If Not String.IsNullOrEmpty(Sender) Then
            con.Add(Restrictions.Like("PM.MailSenders", Sender, MatchMode.Anywhere))
        End If


        'Filtro per registrationuser
        If RegistrationUserCriteria.HasValue Then
            Select Case RegistrationUserCriteria.Value
                Case PECRegistrationUserCriteria.MyRegistrationUser
                    con.Add(Restrictions.Like("PM.RegistrationUser", RegistrationUser, MatchMode.Anywhere))
                Case PECRegistrationUserCriteria.MySectors
                    con.Add(Restrictions.In("PM.RegistrationUser", RegistrationUsers))
            End Select

        End If

        'Filtro per TaskHeaderCriteria
        If TaskHeaderCriteria.HasValue Then
            Select Case TaskHeaderCriteria.Value
                Case PECTaskHeaderCriteria.ExcludeTaskHeader
                    criteria.CreateAlias("PM.TaskHeader", "THP", JoinType.LeftOuterJoin)
                    con.Add(Restrictions.IsNull("THP.Id"))
                Case PECTaskHeaderCriteria.WithTaskHeader
                    criteria.CreateAlias("PM.TaskHeader", "THP", JoinType.InnerJoin)
                    con.Add(Restrictions.IsNotNull("THP.Id"))
            End Select

        End If

        ' Filtri relativi alla casella di posta
        If Not MailboxIds.IsNullOrEmpty() OrElse MailBoxIdsBySender.HasValue Then
            '' Aggancio i MailBoxIds singoli oppure la congiunzione con gli Id ristretti
            con.Add(FilterPecMailBox())
        End If


        ' Filtro PECMail destinate.
        If DocSuiteContext.Current.ProtocolEnv.IsPECDestinationEnabled AndAlso Destinated.HasValue Then
            If Destinated.Value Then
                con.Add(Restrictions.Eq("PM.IsDestinated", True))
            Else
                con.Add(Restrictions.Or(Restrictions.IsNull("PM.IsDestinated"), Restrictions.Eq("PM.IsDestinated", False)))
            End If
        End If

        ' Filtro per data registrazione protocollate.
        If RecordedDateFrom.HasValue OrElse RecordedDateTo.HasValue Then
            con.Add(Expression.IsNotNull("PM.Number"))
            Select Case True
                Case RecordedDateFrom.HasValue AndAlso RecordedDateTo.HasValue
                    ' Verifico la coerenza dell'intervallo specificato
                    If RecordedDateFrom <= RecordedDateTo.Value.AddDays(1).AddSeconds(-1) Then
                        con.Add(Restrictions.Or(Restrictions.Between("ProtocolRegistrationDate", New DateTimeOffset(RecordedDateFrom), New DateTimeOffset(RecordedDateTo.Value.AddDays(1).AddSeconds(-1))),
                                                Restrictions.Between("UDSRegistrationDate", New DateTimeOffset(RecordedDateFrom), New DateTimeOffset(RecordedDateTo.Value.AddDays(1).AddSeconds(-1)))))
                    Else
                        con.Add(Restrictions.Or(Restrictions.Between("ProtocolRegistrationDate", New DateTimeOffset(RecordedDateTo), New DateTimeOffset(RecordedDateFrom.Value.AddDays(1).AddSeconds(-1))),
                                                Restrictions.Between("UDSRegistrationDate", New DateTimeOffset(RecordedDateTo), New DateTimeOffset(RecordedDateFrom.Value.AddDays(1).AddSeconds(-1)))))
                    End If
                Case RecordedDateFrom.HasValue
                    con.Add(Restrictions.Or(Restrictions.Ge("ProtocolRegistrationDate", New DateTimeOffset(RecordedDateFrom)),
                                            Restrictions.Ge("UDSRegistrationDate", New DateTimeOffset(RecordedDateFrom))))
                Case RecordedDateTo.HasValue
                    con.Add(Restrictions.Or(Restrictions.Le("ProtocolRegistrationDate", New DateTimeOffset(RecordedDateTo.Value.AddDays(1).AddSeconds(-1))),
                                            Restrictions.Le("UDSRegistrationDate", New DateTimeOffset(RecordedDateTo.Value.AddDays(1).AddSeconds(-1)))))
            End Select
        End If

        'Filtro per l'esito dell'invio
        If ReceiptCriteria.HasValue Then
            Select Case ReceiptCriteria.Value
                Case PECReceiptCriteria.ErrorOrDelay
                    'con.CreateAlias("PM.Receipts", "PMR", JoinType.InnerJoin)
                    con.Add(Subqueries.NotExists(GetErrorOrDelayExclusionCriteria()))
                    con.Add(Subqueries.Exists(GetErrorOrDelayCriteria()))
                Case PECReceiptCriteria.ReceptionOnly
                    criteria.CreateAlias("PM.Receipts", "PMR", JoinType.InnerJoin)
                    con.Add(Subqueries.NotExists(GetReceptionOnlyCriteria()))
                Case PECReceiptCriteria.WithoutResult
                    criteria.CreateAlias("PM.Receipts", "PMR", JoinType.LeftOuterJoin)
                    con.Add(Restrictions.IsNull("PMR.Id"))
            End Select

        End If

        ' Filtro per PEC scompagnate. Sono tutte le PEC di ricevuta che sono legate a una mail inviata NON inviata da DocSuite
        If ReceiptNotLinked Then
            con.Add(Subqueries.PropertyIn("PM.Id", GetReceiptNotLinkedCriteria()))
        End If

        ' Filtro per data spedizione.
        If MailDateFrom.HasValue OrElse MailDateTo.HasValue OrElse UnsentMails Then
            Dim mailDateCriterion As ICriterion = Nothing

            If MailDateFrom.HasValue AndAlso MailDateTo.HasValue Then
                mailDateCriterion = Expression.Between("MailDate", MailDateFrom, MailDateTo.Value.AddDays(1).AddSeconds(-1))
                If MailDateTo.Value.AddDays(1).AddSeconds(-1) < MailDateFrom Then
                    mailDateCriterion = Expression.Between("MailDate", MailDateTo, MailDateFrom.Value.AddDays(1).AddSeconds(-1))
                End If
            ElseIf MailDateFrom.HasValue Then
                mailDateCriterion = Restrictions.Ge("MailDate", MailDateFrom)
            ElseIf MailDateTo.HasValue Then
                mailDateCriterion = Restrictions.Le("MailDate", MailDateTo.Value.AddDays(1).AddSeconds(-1))
            End If

            Dim unsentMailsCriterion As ICriterion = Nothing
            If UnsentMails Then
                unsentMailsCriterion = Restrictions.IsNull("MailDate")
            End If

            If mailDateCriterion IsNot Nothing AndAlso unsentMailsCriterion IsNot Nothing Then
                mailDateCriterion = Expression.Or(mailDateCriterion, unsentMailsCriterion)
                'ElseIf unsentMailsCriterion IsNot Nothing Then
                '    mailDateCriterion = unsentMailsCriterion
            End If

            If mailDateCriterion IsNot Nothing Then
                con.Add(mailDateCriterion)
            End If
        End If

        If Not String.IsNullOrEmpty(Recipient) Then
            con.Add(Restrictions.Like("PM.MailRecipients", Recipient, MatchMode.Anywhere))
        End If

        If Not PecMailTypeIncluded.IsNullOrEmpty() Then
            con.Add(Restrictions.In("PM.PECType", PecMailTypeIncluded))
        End If

        If (Not IncludeNormalAndMultiples) Then
            con.Add(Restrictions.Eq("Multiple", IncludeMultiples))
        End If

        If WithChecksum.HasValue Then
            If WithChecksum.Value Then
                con.Add(Restrictions.IsNotNull("Checksum"))
            Else
                con.Add(Restrictions.IsNull("Checksum"))
            End If
        End If

        If WithOriginalRecipient.HasValue Then
            If WithOriginalRecipient.Value Then
                con.Add(Restrictions.IsNotNull("OriginalRecipient"))
            Else
                con.Add(Restrictions.IsNull("OriginalRecipient"))
            End If
        End If

        ' TODO: Copiato da ProtocolFinder.GetFilterSession, appena possibile usare per intero quella logica o migliorare
        If DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderEnabled AndAlso Not TaskHeaderIdIn.IsNullOrEmpty() Then
            Dim dcTaskHeader As DetachedCriteria = DetachedCriteria.For(Of TaskHeaderPECMail)("THPM")
            dcTaskHeader.Add(Restrictions.EqProperty("THPM.PECMail.Id", "PM.Id"))

            dcTaskHeader.Add(Restrictions.In("THPM.Header.Id", TaskHeaderIdIn.ToArray()))
            dcTaskHeader.SetProjection(Projections.Constant(True))

            con.Add(Subqueries.Exists(dcTaskHeader))
        End If

        If Anomalies.HasValue AndAlso Anomalies.Value Then
            Dim conj As Conjunction = New Conjunction()

            ' Filtri relativi alla casella di posta
            If Not MailboxIds.IsNullOrEmpty() OrElse MailBoxIdsBySender.HasValue Then
                '' Aggancio i MailBoxIds singoli oppure la congiunzione con gli Id ristretti
                conj.Add(FilterPecMailBox())
            End If

            If Direction.HasValue Then
                conj.Add(Restrictions.Eq("PM.Direction", Direction.Value))
            End If

            conj.Add(Restrictions.In("PM.IsActive", New Short() {
                                             ActiveType.Cast(ActiveType.PECMailActiveType.Error),
                                             ActiveType.Cast(ActiveType.PECMailActiveType.Processing)}))
            dis.Add(conj)
        End If
        dis.Add(con)
        criteria.Add(dis)
        AttachFilterExpressions(criteria)

        Return criteria
    End Function

    Private Function FilterPecMailBox() As Disjunction
        Dim mailBoxIdsDisj As New Disjunction()
        'Mantengo in MailBoxIds tutti gli id di MailBox da gestire
        'ne faccio un sottoinsieme per la gestione di ProtocolBox
        'nel caso in cui non siano presenti protocolbox la gestione resta identica
        If Not MailboxIds.IsNullOrEmpty() Then
            Dim notRestrictedMailBoxIds As Short() = MailboxIds
            If MailBoxIdsBySender.HasValue Then
                '' Se ho imposto una restrizione allora riduco i MailBoxIds escludendo quelli da restringere
                notRestrictedMailBoxIds = notRestrictedMailBoxIds.Where(Function(id) Not MailBoxIdsBySender.Value.Value.Contains(id)).ToArray()
            End If
            If notRestrictedMailBoxIds.Count > 0 Then
                mailBoxIdsDisj.Add(XmlInCriterion.Create("PM.MailBox.Id", notRestrictedMailBoxIds, 1000))
            End If
        End If

        '' Combino il Sender con l'eventuale lista ristretta
        If MailBoxIdsBySender.HasValue Then
            '' AND fra Sender e lista ristretta
            '' il tutto in OR con il resto delle caselle
            Dim conj As New Conjunction()
            conj.Add(Restrictions.Like("PM.MailSenders", MailBoxIdsBySender.Value.Key, MatchMode.Anywhere))
            conj.Add(Restrictions.In("PM.MailBox.Id", MailBoxIdsBySender.Value.Value))
            mailBoxIdsDisj.Add(conj)
        End If
        Return mailBoxIdsDisj
    End Function
    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        Dim countRecords As Integer

        criteria.ClearOrders()
        criteria.SetProjection(Projections.SqlProjection("COUNT( {alias}.IDPECMail) as MAILCOUNT", New String() {"MailCount"}, New Type.Int32Type() {NHibernateUtil.Int32}))
        countRecords = criteria.UniqueResult(Of Integer)()

        If TopMaxRecords > 0 Then
            Return Math.Min(countRecords, TopMaxRecords)
        Else
            Return countRecords
        End If
    End Function

    Private Function GetErrorOrDelayCriteria() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of PECMailReceipt)("PMR_I")
        dc.SetProjection(Projections.Constant(True))
        dc.Add(Restrictions.EqProperty("PM.Id", "PMR_I.PECMail.Id"))
        dc.Add(Restrictions.In("PMR_I.ReceiptType", New String() {PECMailTypes.PreavvisoErroreConsegna.ToString(), PECMailTypes.NonAccettazione.ToString(), PECMailTypes.ErroreConsegna.ToString()}))
        Return dc
    End Function

    Private Function GetErrorOrDelayExclusionCriteria() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of PECMailReceipt)("PMR_I")
        dc.SetProjection(Projections.Constant(True))
        dc.Add(Restrictions.EqProperty("PM.Id", "PMR_I.PECMail.Id"))
        dc.Add(Restrictions.Eq("PMR_I.ReceiptType", PECMailTypes.AvvenutaConsegna.ToString()))
        Return dc
    End Function

    Private Function GetReceptionOnlyCriteria() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of PECMailReceipt)("PMR_I")
        dc.SetProjection(Projections.Constant(True))
        dc.Add(Restrictions.EqProperty("PM.Id", "PMR_I.PECMail.Id"))
        dc.Add(Restrictions.Not(Restrictions.Eq("PMR_I.ReceiptType", PECMailTypes.Accettazione)))
        Return dc
    End Function

    Private Sub SetProjectionsHeaders(ByRef criteria As ICriteria)
        Dim detachedAttachmentsCount As DetachedCriteria = DetachedCriteria.For(Of PECMailAttachment)("pma_AttachmentsCount")
        With detachedAttachmentsCount
            .Add(Restrictions.EqProperty("pma_AttachmentsCount.Mail.Id", "PM.Id"))
            .SetProjection(Projections.RowCount)
        End With

        Dim detachedReadCount As DetachedCriteria = DetachedCriteria.For(Of PECMailLog)("pml_ReadCount")
        With detachedReadCount
            .Add(Restrictions.Eq("pml_ReadCount.Type", "Read"))
            .Add(Restrictions.EqProperty("pml_ReadCount.Mail.Id", "PM.Id"))
            .SetProjection(Projections.RowCount)
        End With

        Dim detachedMoveCount As DetachedCriteria = DetachedCriteria.For(Of PECMailLog)("pml_MoveCount")
        With detachedMoveCount
            .Add(Restrictions.Eq("pml_MoveCount.Type", "Move"))
            .Add(Restrictions.EqProperty("pml_MoveCount.Mail.Id", "PM.Id"))
            .SetProjection(Projections.RowCount)
        End With

        Dim detachedLastReplyMailId As DetachedCriteria = DetachedCriteria.For(Of PECMailLog)("pml_LastReplyMailId")
        With detachedLastReplyMailId
            .Add(Restrictions.Eq("pml_LastReplyMailId.Type", "Replied"))
            .Add(Restrictions.EqProperty("pml_LastReplyMailId.Mail.Id", "PM.Id"))
            .SetProjection(Projections.Max(Projections.Property("pml_LastReplyMailId.DestinationMail.Id")))
        End With

        Dim detachedLastForwardMailId As DetachedCriteria = DetachedCriteria.For(Of PECMailLog)("pml_LastForwardMailId")
        With detachedLastForwardMailId
            .Add(Restrictions.Eq("pml_LastForwardMailId.Type", "Forwarded"))
            .Add(Restrictions.EqProperty("pml_LastForwardMailId.Mail.Id", "PM.Id"))
            .SetProjection(Projections.Max(Projections.Property("pml_LastForwardMailId.DestinationMail.Id")))
        End With

        Dim detachedSendRolesLogCount As DetachedCriteria = DetachedCriteria.For(Of ProtocolLog)("pl_Log")
        With detachedSendRolesLogCount
            .Add(Restrictions.EqProperty("pl_Log.Year", "PM.Year"))
            .Add(Restrictions.EqProperty("pl_Log.Number", "PM.Number"))
            .Add(Restrictions.Eq("DU.Environment", DirectCast(DSWEnvironment.Protocol, Integer)))
            .Add(Restrictions.GeProperty("pl_Log.LogDate", "PM.RegistrationDate"))
            .Add(Restrictions.Eq("pl_Log.LogType", "PW"))
            .SetProjection(Projections.RowCount)
        End With

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.SubQuery(detachedAttachmentsCount), "AttachmentsCount")
        proj.Add(Projections.SubQuery(detachedReadCount), "ReadCount")
        proj.Add(Projections.SubQuery(detachedMoveCount), "MoveCount")
        proj.Add(Projections.SubQuery(detachedLastReplyMailId), "LastReplyMailId")
        proj.Add(Projections.SubQuery(detachedLastForwardMailId), "LastForwardMailId")
        proj.Add(Projections.SubQuery(detachedSendRolesLogCount), "ExistSendRolesLog")

        proj.Add(Projections.Property("PM.Id"), "Id") _
            .Add(Projections.Property("PM.Direction"), "Direction") _
            .Add(Projections.Property("PM.Year"), "Year") _
            .Add(Projections.Property("PM.Number"), "Number") _
            .Add(Projections.Property("PM.MailSubject"), "MailSubject") _
            .Add(Projections.Property("PM.MailSenders"), "MailSenders") _
            .Add(Projections.Property("PM.MailRecipients"), "MailRecipients") _
            .Add(Projections.Property("PM.MailRecipientsCc"), "MailRecipientsCc") _
            .Add(Projections.Property("PM.ReceivedAsCc"), "ReceivedAsCc") _
            .Add(Projections.Property("PM.MailDate"), "MailDate") _
            .Add(Projections.Property("PM.MailPriority"), "MailPriority") _
            .Add(Projections.Property("PM.XTrasporto"), "XTrasporto") _
            .Add(Projections.Property("PM.XRiferimentoMessageID"), "XRiferimentoMessageID") _
            .Add(Projections.Property("PM.Segnatura"), "Segnatura") _
            .Add(Projections.Property("PM.RecordedInDocSuite"), "RecordedInDocSuite") _
            .Add(Projections.Property("PM.MailBox.Id"), "MailBoxId") _
            .Add(Projections.Property("PMB.MailBoxName"), "MailBoxName") _
            .Add(Projections.Property("PM.IsToForward"), "IsToForward") _
            .Add(Projections.Property("PM.IsValidForInterop"), "IsValidForInterop") _
            .Add(Projections.Property("PM.IsActive"), "IsActive") _
            .Add(Projections.Property("PM.Handler"), "Handler") _
            .Add(Projections.Property("PM.RegistrationDate"), "RegistrationDate") _
            .Add(Projections.Property("DU.IdUDSRepository"), "IdUDSRepository") _
            .Add(Projections.Property("DU.Id"), "IdDocumentUnit") _
            .Add(Projections.Property("DU.Environment"), "DocumentUnitType") _
            .Add(Projections.Conditional(Restrictions.IsNull("PM.Number"), Projections.Constant(False), Projections.Constant(True)), "HasProtocol") _
            .Add(Projections.Property("PM.Size"), "Size") _
            .Add(Projections.Property("PM.PECType"), "PECType")

        If DocSuiteContext.Current.ProtocolEnv.IsPECDestinationEnabled Then
            proj.Add(Projections.Property("PM.IsDestinated"), "IsDestinated")
        End If

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of PecMailHeader))
    End Sub

    Public Overloads Overrides Function DoSearch() As IList(Of PECMail)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)

        Return criteria.List(Of PECMail)()
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of PecMailHeader)
        Dim criteria As ICriteria = CreateCriteria()
        criteria.CreateAlias("PM.MailBox", "PMB", JoinType.InnerJoin)
        DecorateCriteria(criteria)
        SetProjectionsHeaders(criteria)

        Dim headers As IList(Of PecMailHeader)
        headers = criteria.List(Of PecMailHeader)()
        Dim conversion As IList(Of PecMailHeader) = New List(Of PecMailHeader)(headers.Count)
        For Each a As PecMailHeader In headers
            a.RegistrationDate = a.RegistrationDate.ToLocalTime()
            conversion.Add(a)
        Next
        Return conversion
    End Function

    ''' <summary>
    ''' Seleziono tutte le ricevute allegate a una mail.
    ''' Se non trovo nessuna ricevuta significa che 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetReceiptNotLinkedCriteria() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of PECMailReceipt)("PMR_I")
        dc.SetProjection(Projections.Property("PMR_I.Parent.Id"))
        dc.Add(Restrictions.In("PMR_I.ReceiptType", New String() {PECMailTypes.Accettazione.ToString(), PECMailTypes.AvvenutaConsegna.ToString()}))
        dc.Add(Restrictions.IsNull("PMR_I.PECMail"))
        Return dc
    End Function


#End Region

End Class