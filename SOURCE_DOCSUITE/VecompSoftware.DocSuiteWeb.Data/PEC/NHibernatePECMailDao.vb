Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports NHibernate.Transform
Imports NHibernate.Util
Imports VecompSoftware.Helpers
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECMailDao
    Inherits BaseNHibernateDao(Of PECMail)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetMailStatusByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As String
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        Dim statuses As ICollection(Of String) = EnumHelper.GetDescriptions(GetType(PECMailReceiptType))
        crit.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))
        crit.Add(Restrictions.Eq("XRiferimentoMessageID", xRiferimentoMessageId))
        crit.Add(Restrictions.In("MailType", statuses.ToList()))
        crit.AddOrder(Order.Desc("MailDate"))
        crit.SetProjection(Projections.Property("MailType"))

        Dim results As IList(Of String) = crit.List(Of String)()
        If Not results.Any() Then
            Return String.Empty
        End If

        Dim enumCollection As New List(Of PECMailReceiptType)
        For Each item As String In results
            Dim value As PECMailReceiptType = EnumHelper.ParseDescriptionToEnum(Of PECMailReceiptType)(item)
            enumCollection.Add(value)
        Next

        'Casisitica di ricevute non conosciuta
        If Not enumCollection.Any() Then
            Return String.Empty
        End If
        Dim retVal As String = EnumHelper.GetDescription(enumCollection.OrderByDescending(Function(o) o).First())
        Return retVal
    End Function

    Public Function LogicDelete(ByRef pecs As ICollection(Of Integer)) As Integer
        Using statelessSession As IStatelessSession = NHibernateSessionManager.Instance.OpenStatelessSession(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
            Using transaction As ITransaction = statelessSession.BeginTransaction()
                Dim updateQueryString As String = "update PECMail p set p.IsActive = 2 WHERE p.Id in (:pecIds)"
                Dim updated As Integer = statelessSession.CreateQuery(updateQueryString) _
                                         .SetParameterList("pecIds", pecs) _
                                        .ExecuteUpdate()
                If Not updated > 0 Then
                    Return 0
                End If
                Dim insertQueryString As String = "insert into PECMailLog (Mail, Description, Type, Date, SystemComputer, SystemUser) " &
                            " select p, (:description), (:type), (:date), (:systemComputer), (:systemUser) from PECMail p WHERE p.Id in (:pecIds) "
                Dim inserted As Integer = statelessSession.CreateQuery(insertQueryString) _
                            .SetParameterList("pecIds", pecs) _
                            .SetParameter("type", PECMailLogType.Delete.ToString()) _
                            .SetParameter("description", "isActive ---> 2 per svuotamento cestino") _
                            .SetParameter("date", Date.UtcNow) _
                            .SetParameter("systemComputer", DocSuiteContext.Current.UserComputer) _
                            .SetParameter("systemUser", DocSuiteContext.Current.User.FullUserName) _
                            .ExecuteUpdate()

                transaction.Commit()
                Return inserted
            End Using
        End Using
    End Function

    Public Function GetProtocol(pec As PECMail) As Protocol
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Protocol)
        criteria.Add(Restrictions.Eq("Id", pec.DocumentUnit.Id))
        Return criteria.UniqueResult(Of Protocol)
    End Function

    Public Function MailExistsByUIDAndPecMailBox(ByVal uid As String, ByVal idPecMailBox As Integer) As Boolean
        Dim retval As Boolean = False

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MailUID", uid))
        criteria.Add(Restrictions.Eq("IDPECMailBox", idPecMailBox))
        criteria.SetMaxResults(1)
        Dim list As IList(Of PECMail) = criteria.List(Of PECMail)()
        retval = list.Count > 0

        Return retval
    End Function

    Public Function GetMailByUid(ByVal uid As String) As PECMail
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MailUID", uid))
        Dim list As IList(Of PECMail) = criteria.List(Of PECMail)()
        If list.Count > 1 Then
            Throw New Exception($"Errore in recupero Mail con UID {uid}. Chiave duplicata.")
        End If
        If list.Count = 1 Then
            Return list(0)
        End If

        Return Nothing
    End Function
    Public Function GetOriginalPECFromReferenceToSDIIdentification(ByVal referenceToPECMessageId As String) As PECMail
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of PECMailReceipt)

        criteria.Add(Restrictions.Eq("Identification", referenceToPECMessageId))
        criteria.Add(Restrictions.Eq("ReceiptType", "avvenuta-consegna"))
        criteria.Add(Restrictions.IsNotNull("PECMail"))

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Dim pecMailReceipt As PECMailReceipt = criteria.UniqueResult(Of PECMailReceipt)()
        If pecMailReceipt IsNot Nothing Then
            Return pecMailReceipt.PECMail
        End If
        Return Nothing
    End Function

    Public Function GetOriginalPECFromPAAttachmentFileName(ByVal attachmentFileName As String) As PECMail
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Attachments", "A", JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Outgoing)))
        Dim disj As Disjunction = New Disjunction()
        disj.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        disj.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Processing)))
        criteria.Add(disj)
        criteria.Add(Restrictions.Eq("A.AttachmentName", attachmentFileName))

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.UniqueResult(Of PECMail)()
    End Function

    Public Function GetOutgoingMailByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As PECMail
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("XRiferimentoMessageID", xRiferimentoMessageId))
        criteria.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Outgoing)))

        Return criteria.UniqueResult(Of PECMail)()
    End Function

    Public Function GetIncomingMailByXRiferimentoMessageID(ByVal xRiferimentoMessageID As String) As IList(Of PECMail)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("XRiferimentoMessageID", xRiferimentoMessageID))
        criteria.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))

        Return criteria.List(Of PECMail)()
    End Function

    Public Function GetMailByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As PECMail
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("MessageID", xRiferimentoMessageId))
        criteria.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Outgoing)))

        Return criteria.UniqueResult(Of PECMail)()
    End Function

    Public Function GetOutgoingMailHistory(ByVal idPecMail As Integer) As IList(Of PECMail)

        Dim mailTypes As String() = {PECMailTypes.AvvenutaConsegna, PECMailTypes.Accettazione, PECMailTypes.NonAccettazione, PECMailTypes.ErroreConsegna, PECMailTypes.PreavvisoErroreConsegna}
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        Dim subQuery As DetachedCriteria = DetachedCriteria.For(Of PECMail)("pecmail")
        subQuery.Add(Restrictions.Eq("Id", idPecMail))
        subQuery.Add(Restrictions.Eq("MailType", PECMailTypes.Invio))
        subQuery.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Outgoing)))
        subQuery.SetProjection(Projections.Property("XRiferimentoMessageID"))

        criteria.Add(Subqueries.PropertyEq("XRiferimentoMessageID", subQuery))
        criteria.Add(Restrictions.In("MailType", mailTypes))
        criteria.AddOrder(Order.Asc("MailDate"))
        Return criteria.List(Of PECMail)()

    End Function

    Public Function GetOutgoingMails(ByVal idPecMailBox As Int16, ByVal onlyMultiplePecs As Boolean, ByVal maxResults As Integer, Optional ByVal useStatusProcessing As Boolean = False) As IList(Of Integer)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        crit.Add(Restrictions.Eq("MailBox.Id", idPecMailBox))
        crit.Add(Restrictions.IsNull("MailDate"))
        crit.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Outgoing)))

        ' Carico le PEC generate ed effettivamente pronte per l'invio oppure le PEC bloccate in stato processing
        If useStatusProcessing Then
            Dim disj As Disjunction = New Disjunction()
            disj.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
            disj.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Processing)))
            crit.Add(disj)
        Else
            ' Carico le PEC generate ed effettivamente pronte per l'invio.
            crit.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        End If

        ' Definisco se caricare o meno le PEC multiple
        crit.Add(Restrictions.Eq("Multiple", onlyMultiplePecs))

        ' Definisco il numero massimo di risultati
        crit.SetMaxResults(maxResults)

        crit.AddOrder(Order.Asc("MailDate"))

        ' Definisco una proiezione sull'ID (per motivi di performance, per evitare il deadlock)
        crit.SetProjection(Projections.Property("Id"))

        Return crit.List(Of Integer)()
    End Function

    Public Function GetMailsToForward(ByVal idPecMailBox As Int16) As IList(Of PECMail)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("MailBox.Id", idPecMailBox))
        criteria.Add(Restrictions.Eq("IsToForward", True))
        criteria.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))

        Return criteria.List(Of PECMail)()
    End Function

    ''' <summary> Estrae tutte le mail che non sono ancora state convertite alla DSW8. </summary>
    Public Function GetDsw7StoredMail(ByVal idPecMailBox As Short, ByVal maxResults As Integer, ByVal startDate As Date?, ByVal endDate As Date?) As IList(Of PECMail)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(Of PECMail)()
        crit.Add(Restrictions.Eq("MailBox.Id", idPecMailBox))
        crit.Add(Restrictions.IsNull("PECType"))
        ''Solo le PEC da data a data
        If startDate.HasValue Then
            crit.Add(Restrictions.Ge("MailDate", startDate.Value))
        End If
        If endDate.HasValue Then
            crit.Add(Restrictions.Le("MailDate", endDate.Value))
        End If
        Dim disj As New Disjunction()
        disj.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Delete))).Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        crit.Add(disj)
        crit.AddOrder(Order.Desc("MailDate"))
        crit.SetMaxResults(maxResults)
        Return crit.List(Of PECMail)()
    End Function

    Public Function GetEmptyMails() As IList(Of PECMail)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MailBody", ""))
        criteria.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))
        criteria.Add(Restrictions.IsEmpty("Attachments"))
        Return criteria.List(Of PECMail)()
    End Function

    Public Function GetNotHashedMails(ByVal idPecMailBox As Short) As IList(Of PECMail)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))
        crit.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        crit.Add(Restrictions.Eq("MailBox.Id", idPecMailBox))
        crit.Add(Restrictions.IsNull("Checksum"))
        crit.SetMaxResults(200)
        Return crit.List(Of PECMail)()
    End Function

    Public Function Exists(uid As String, box As PECMailBox) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MailUID", uid))
        criteria.Add(Restrictions.Eq("MailBox", box))
        criteria.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        criteria.SetProjection(Projections.RowCount())

        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Public Function ChecksumExists(ByVal checksum As String, ByVal originalRecipient As String, includeOnError As Boolean) As Boolean
        Dim isActiveIn As List(Of Integer) = New List(Of Integer) From {0, 1, 2}
        If includeOnError Then
            isActiveIn.Add(255)
        End If
        Dim criteria As ICriteria = GetChecksumCriteria(checksum, originalRecipient, isActiveIn)
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Public Function HeaderChecksumExists(ByVal headerChecksum As String, ByVal originalRecipient As String, includeOnError As Boolean) As Boolean
        Dim isActiveIn As List(Of Integer) = New List(Of Integer) From {0, 1, 2}
        If includeOnError Then
            isActiveIn.Add(255)
        End If
        Dim criteria As ICriteria = GetHeaderChecksumCriteria(headerChecksum, originalRecipient, isActiveIn)
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Public Function GetByChecksum(ByVal checksum As String, ByVal originalRecipient As String) As IList(Of PECMail)
        Return GetByChecksum(checksum, originalRecipient, Nothing)
    End Function

    Public Function GetByChecksum(ByVal checksum As String, ByVal originalRecipient As String, ByVal isActiveIn As List(Of Integer)) As IList(Of PECMail)
        Dim criteria As ICriteria = GetChecksumCriteria(checksum, originalRecipient, isActiveIn)
        Return criteria.List(Of PECMail)()
    End Function

    Public Function GetByHeaderChecksum(ByVal headerChecksum As String, ByVal originalRecipient As String) As IList(Of PECMail)
        Return GetByHeaderChecksum(headerChecksum, originalRecipient, Nothing)
    End Function

    Public Function GetByHeaderChecksum(ByVal headerChecksum As String, ByVal originalRecipient As String, ByVal isActiveIn As List(Of Integer)) As IList(Of PECMail)
        Dim criteria As ICriteria = GetHeaderChecksumCriteria(headerChecksum, originalRecipient, isActiveIn)
        Return criteria.List(Of PECMail)()
    End Function

    Private Function GetChecksumCriteria(ByVal checksum As String, ByVal originalRecipient As String, ByVal isActiveIn As List(Of Integer)) As ICriteria
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        crit.Add(Restrictions.Eq("Checksum", checksum))

        crit.Add(Restrictions.Eq("OriginalRecipient", originalRecipient))
        crit.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))

        If isActiveIn.Count > 0 Then
            crit.Add(Restrictions.In("IsActive", isActiveIn))
        End If

        Return crit
    End Function

    Private Function GetHeaderChecksumCriteria(ByVal headerChecksum As String, ByVal originalRecipient As String, ByVal isActiveIn As List(Of Integer)) As ICriteria
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        crit.Add(Restrictions.Eq("HeaderChecksum", headerChecksum))

        crit.Add(Restrictions.Eq("OriginalRecipient", originalRecipient))
        crit.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))

        If isActiveIn.Count > 0 Then
            crit.Add(Restrictions.In("IsActive", isActiveIn))
        End If

        Return crit
    End Function



End Class
