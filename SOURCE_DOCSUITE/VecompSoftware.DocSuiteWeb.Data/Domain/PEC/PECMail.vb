Imports System.Linq

<Serializable()>
Public Class PECMail
    Inherits DomainObject(Of Int32)
    Implements ISupportLogicDelete, IAuditable

#Region " Fields "

#End Region

#Region " Properties "

    ''' <summary>Stabilisce se la PEC è in ingresso o uscita</summary>
    ''' <value>Valori possibili su <see>PECMailDirection</see></value>
    Public Overridable Property Direction As Short

    Public Overridable Property Year As Short?

    Public Overridable Property Number As Integer?

    Public Overridable Property OriginalRecipient As String

    Public Overridable Property MailUID As String

    Public Overridable Property MailContent As String

    Public Overridable Property MailSubject As String

    Public Overridable Property MailSenders As String

    Public Overridable Property MailRecipients As String

    Public Overridable Property MailRecipientsCc As String

    Public Overridable Property ReceivedAsCc As Boolean?

    Public Overridable Property MailDate As DateTime?

    Public Overridable Property MailType As String

    Public Overridable Property MailError As String

    Public Overridable Property MailPriority As Short?

    Public Overridable Property XTrasporto As String

    Public Overridable Property MessageID As String

    Public Overridable Property XRiferimentoMessageID As String

    Public Overridable Property Segnatura As String

    Public Overridable Property MessaggioRitornoName As String

    Public Overridable Property MessaggioRitornoStream As String

    Public Overridable Property MailBody As String

    Public Overridable Property RecordedInDocSuite As Short?

    Public Overridable Property IsToForward As Boolean

    Public Overridable Property IsValidForInterop As Boolean

    ''' <summary> Allegati </summary>
    Public Overridable Property Attachments As IList(Of PECMailAttachment)

    ''' <summary> Mailbox nella quale è residente la PEC </summary>
    ''' <remarks> Solo <see cref="Location"/> determina dove questa è archiviata, non la location della <see cref="MailBox"/>. </remarks>
    Public Overridable Property MailBox As PECMailBox

    ''' <summary>Stabilisce lo stato di visibità della PEC 
    ''' Valori possibili
    ''' 0 : Cestino
    ''' 1 : Attiva
    ''' 2 : Cancellata e non visibile
    ''' </summary>
    Public Overridable Property IsActive As Short Implements ISupportLogicDelete.IsActive

    Public Overridable Property MailStatus As Byte?

    Public Overridable Property IsDestinated As Boolean?

    Public Overridable Property DestinationNote As String

    Public Overridable Property LogEntries As IList(Of PECMailLog)

    Public Overridable Property Handler As String

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    ''' <summary> Location in cui è archiviata copia della PEC. </summary>
    ''' <remarks> Può differire dalla location della <see cref="MailBox"/>. </remarks>
    Public Overridable Property Location As Location

    ''' <summary> Identificativo della catena documentale che contiene copia dell'intera PEC </summary>
    Public Overridable Property IDMailContent As Guid

    ''' <summary> Identificativo della catena documentale in cui sono archiviate gli allegati della PEC </summary>
    Public Overridable Property IDAttachments As Guid

    Public Overridable Property IDPostacert As Guid

    Public Overridable Property IDDaticert As Guid

    Public Overridable Property IDSegnatura As Guid

    Public Overridable Property IDSmime As Guid

    Public Overridable Property IDEnvelope As Guid

    Public Overridable Property Receipts As IList(Of PECMailReceipt)

    Public Overridable Property TaskHeader As IList(Of TaskHeaderPECMail)

    ''' <summary> Tipo di PEC </summary>
    ''' <value> Se Nothing non è ancora stato aggiornato </value>
    Public Overridable Property PECType As PECMailType?

    Public Overridable Property Checksum As String

    Public Overridable Property HeaderChecksum As String

    ''' <summary> Definisce se la PEC rappresenta un template master </summary>
    ''' <remarks> Se true è possibile generare più PEC in base al parametro <see cref="MultipleType" /> </remarks>
    Public Overridable Property Multiple As Boolean

    Public Overridable Property SplittedFrom As Integer

    Public Overridable Property ProcessStatus As PECMailProcessStatus

    ''' <summary> Grandezza complessiva della PEC (corrisponde al size del blobbone) </summary>
    Public Overridable Property Size As Long?

    ''' <summary> Definisce la tipologia di moltiplicazione della PEC</summary>
    ''' <value> Di default inserisce 0 che è la duplicazione retrocompatibile </value>
    ''' <remarks> <see cref="Multiple" /> </remarks>
    Public Overridable Property MultipleType As PecMailMultipleTypeEnum

    Public Overridable Property InvoiceStatus As InvoiceStatus?

    Public Overridable Property DocumentUnit As DocumentUnit

#End Region

#Region " Constructor "

    Public Sub New()
        Attachments = New List(Of PECMailAttachment)
        Multiple = False
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods "

    Public Overrides Function Clone() As Object
        Return Clone(True, False, Nothing)
    End Function

    Public Overridable Overloads Function Clone(ByVal cloneAttachments As Boolean, ByVal cloneReceipts As Boolean, ByRef sourcePecMail As PECMail) As PECMail
        Dim clonedPecMail As PECMail = CType(MyBase.Clone(), PECMail)
        '' Rimuovo l'id per evitare sovrapposizione
        clonedPecMail.Id = Nothing
        clonedPecMail.UniqueId = Guid.NewGuid()

        '' Disattivo la PEC per evitare che venga gestita prima del necessario
        clonedPecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Disabled)

        '' Resetto i collegamenti con gli allegati per evitare errori
        clonedPecMail.Attachments = New List(Of PECMailAttachment)()

        '' Effettuo la clonazione degli allegati se richiesta
        If cloneAttachments Then
            For Each clonedPecMailAttachment As PECMailAttachment In From pecMailAttachment In Attachments Select CType(pecMailAttachment.Clone(), PECMailAttachment)
                clonedPecMailAttachment.Mail = clonedPecMail
                clonedPecMail.Attachments.Add(clonedPecMailAttachment)
            Next
        End If

        clonedPecMail.Receipts = New List(Of PECMailReceipt)()
        '' Effettuo la clonazione delle ricevute
        If cloneReceipts Then

            For Each clonedPECMailReceipt As PECMailReceipt In Receipts.Select(Function(r) CType(r.Clone(), PECMailReceipt))
                clonedPECMailReceipt.PECMail = clonedPecMail
                clonedPecMail.Receipts.Add(clonedPECMailReceipt)
            Next
        End If

        '' Effettuo il reset dei Log
        clonedPecMail.LogEntries = Nothing

        If sourcePecMail IsNot Nothing Then
            clonedPecMail.Multiple = False
            clonedPecMail.SplittedFrom = sourcePecMail.Id
        End If

        Return clonedPecMail
    End Function

    Public Overridable Function HasDocumentUnit() As Boolean
        Return Year.HasValue AndAlso Number.HasValue
    End Function

#End Region

End Class