Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text
Imports NHibernate
Imports VecompSoftware.Commons.Interfaces.CQRS.Events
Imports VecompSoftware.Core.Command
Imports VecompSoftware.Core.Command.CQRS.Commands.Entities.Protocols
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Protocols
Imports VecompSoftware.DocSuiteWeb.Data.Formatter
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
Imports VecompSoftware.DocSuiteWeb.Facade.Interfaces
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Workflow.Actions
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.Services.Command.CQRS.Commands.Entities.Protocols
Imports VecompSoftware.Services.Logging
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols

<ComponentModel.DataObject()>
Public Class ProtocolFacade
    Inherits BaseProtocolFacade(Of Protocol, YearNumberCompositeKey, NHibernateProtocolDao)

    Public Enum ProtocolLinkType
        Normale = 0
        RispondiDaPEC = 1
    End Enum

#Region " Events "

    Public Event BeforeProtocolCreate As ProtocolEventHandler
    Public Sub OnBeforeProtocolCreate(args As ProtocolEventArgs)
        RaiseEvent BeforeProtocolCreate(Me, args)
    End Sub

    Public Event AfterProtocolCreate As ProtocolEventHandler
    Public Sub OnAfterProtocolCreate(args As ProtocolEventArgs)
        RaiseEvent AfterProtocolCreate(Me, args)
    End Sub

    Public Event BeforeGenerateSignature As ProtocolEventHandler
    Public Sub OnBeforeGenerateSignature(args As ProtocolEventArgs)
        RaiseEvent BeforeGenerateSignature(Me, args)
    End Sub

    Public Event AfterGenerateSignature As ProtocolEventHandler
    Public Sub OnAfterGenerateSignature(args As ProtocolEventArgs)
        RaiseEvent AfterGenerateSignature(Me, args)
    End Sub

    Public Event BeforeUpdate As ProtocolEventHandler
    Public Sub OnBeforeUpdate(args As ProtocolEventArgs)
        RaiseEvent BeforeUpdate(Me, args)
    End Sub

    Public Event AfterUpdate As ProtocolEventHandler
    Public Sub OnAfterUpdate(args As ProtocolEventArgs)
        RaiseEvent AfterUpdate(Me, args)
    End Sub

    Public Event BeforeSave As ProtocolEventHandler
    Public Sub OnBeforeSave(args As ProtocolEventArgs)
        RaiseEvent BeforeSave(Me, args)
    End Sub

    Public Event AfterSave As ProtocolEventHandler
    Public Sub OnAfterSave(args As ProtocolEventArgs)
        RaiseEvent AfterSave(Me, args)
    End Sub

    Public Event BeforeActivation As ProtocolEventHandler
    Public Sub OnBeforeActivation(args As ProtocolEventArgs)
        RaiseEvent BeforeActivation(Me, args)
    End Sub

    Public Event AfterActivation As ProtocolEventHandler
    Public Sub OnAfterActivation(args As ProtocolEventArgs)
        RaiseEvent AfterActivation(Me, args)
    End Sub

    Public Event BeforeAddAttachments As ProtocolEventHandler
    Public Sub OnBeforeAddAttachments(args As ProtocolEventArgs)
        RaiseEvent BeforeAddAttachments(Me, args)
    End Sub

    Public Event AfterAddAttachments As ProtocolEventHandler
    Public Sub OnAfterAddAttachments(args As ProtocolEventArgs)
        RaiseEvent AfterAddAttachments(Me, args)
    End Sub

    Public Event BeforeAddAnnexes As ProtocolEventHandler
    Public Sub OnBeforeAddAnnexes(args As ProtocolEventArgs)
        RaiseEvent BeforeAddAnnexes(Me, args)
    End Sub

    Public Event AfterAddAnnexes As ProtocolEventHandler
    Public Sub OnAfterAddAnnexes(args As ProtocolEventArgs)
        RaiseEvent AfterAddAnnexes(Me, args)
    End Sub

    Public Event BeforeAddDocument As ProtocolEventHandler
    Public Sub OnBeforeAddDocument(args As ProtocolEventArgs)
        RaiseEvent BeforeAddDocument(Me, args)
    End Sub

    Public Event AfterAddDocument As ProtocolEventHandler
    Public Sub OnAfterAddDocument(args As ProtocolEventArgs)
        RaiseEvent AfterAddDocument(Me, args)
    End Sub

    Public Event AfterInsert As ProtocolEventHandler
    Public Sub OnAfterInsert(args As ProtocolEventArgs)
        RaiseEvent AfterInsert(Me, args)
    End Sub

    Public Event AfterEdit As ProtocolEventHandler
    Public Sub OnAfterEdit(args As ProtocolEventArgs)
        RaiseEvent AfterEdit(Me, args)
    End Sub

    Public Event AfterCancel As ProtocolEventHandler
    Public Sub OnAfterCancel(args As ProtocolEventArgs)
        RaiseEvent AfterCancel(Me, args)
    End Sub

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Fields "

    Private _protocolRejectionContainer As Container
    Private _observer As IFacadeObserver(Of ProtocolFacade)
    Private _contactNamesFacade As ContactNameFacade
    Private _contactFacade As ContactFacade
    Private _commandInsertFacade As CommandFacade(Of ICommandCreateProtocol)
    Private _commandUpdateFacade As CommandFacade(Of ICommandUpdateProtocol)
    Private _mapperProtocolEntity As MapperProtocolEntity
    Private _mapperCategoryFascicle As MapperCategoryFascicle
    Private _categoryFascicleDao As CategoryFascicleDao

    Private Const FileLoggerName As String = "ProtocolLog"

    Private _currentPecMailFacade As PECMailFacade

#End Region

#Region " Properties "

    Public ReadOnly Property ContactNamesFacade As ContactNameFacade
        Get
            If _contactNamesFacade Is Nothing Then
                _contactNamesFacade = New ContactNameFacade
            End If
            Return _contactNamesFacade
        End Get
    End Property

    Public ReadOnly Property MapperProtocolEntity As MapperProtocolEntity
        Get
            If _mapperProtocolEntity Is Nothing Then
                _mapperProtocolEntity = New MapperProtocolEntity
            End If
            Return _mapperProtocolEntity
        End Get
    End Property

    Public ReadOnly Property MapperCategoryFascicle As MapperCategoryFascicle
        Get
            If _mapperCategoryFascicle Is Nothing Then
                _mapperCategoryFascicle = New MapperCategoryFascicle
            End If
            Return _mapperCategoryFascicle
        End Get
    End Property

    'Si utilizza il Dao per problemi di referenze circolari
    Public ReadOnly Property CategoryFascicleDao As CategoryFascicleDao
        Get
            If _categoryFascicleDao Is Nothing Then
                _categoryFascicleDao = New CategoryFascicleDao(ProtDB)
            End If
            Return _categoryFascicleDao
        End Get
    End Property

    Private ReadOnly Property Observer As IFacadeObserver(Of ProtocolFacade)
        Get
            If _observer Is Nothing Then
                ' Verifico sia configurato l'observer.
                If String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.ObserversAssemblyName) Then
                    Return Nothing
                End If

                ' Verifico esista l'assembly.
                Dim observerPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", DocSuiteContext.Current.ProtocolEnv.ObserversAssemblyName)
                If Not File.Exists(observerPath) Then
                    FileLogger.Warn(FileLoggerName, "Assembly mancante: " & DocSuiteContext.Current.ProtocolEnv.ObserversAssemblyName)
                    Return Nothing
                End If

                ' Inizializzo l'observer.
                Try
                    _observer = DirectCast(ReflectionHelper.InstanceClass(observerPath, DocSuiteContext.Current.ProtocolEnv.ProtocolFacadeObserverName), IFacadeObserver(Of ProtocolFacade))
                Catch ex As Exception
                    FileLogger.Error(FileLoggerName, "Errore in caricamento assembly: " & DocSuiteContext.Current.ProtocolEnv.ObserversAssemblyName, ex)
                    Return Nothing
                End Try
            End If
            Return _observer
        End Get
    End Property

    ''' <summary> Contenitore per i protocolli rigettati </summary>
    ''' <remarks> Funzione nata per ENAV </remarks>
    Public ReadOnly Property RejectionContainer As Container
        Get
            If _protocolRejectionContainer Is Nothing Then
                _protocolRejectionContainer = Factory.ContainerFacade.GetById(DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionContainerId, False)
            End If

            Return _protocolRejectionContainer
        End Get
    End Property

    Private ReadOnly Property CommandInsertFacade As CommandFacade(Of ICommandCreateProtocol)
        Get
            If _commandInsertFacade Is Nothing Then
                _commandInsertFacade = New CommandFacade(Of ICommandCreateProtocol)()
            End If
            Return _commandInsertFacade
        End Get
    End Property

    Private ReadOnly Property CommandUpdateFacade As CommandFacade(Of ICommandUpdateProtocol)
        Get
            If _commandUpdateFacade Is Nothing Then
                _commandUpdateFacade = New CommandFacade(Of ICommandUpdateProtocol)()
            End If
            Return _commandUpdateFacade
        End Get
    End Property

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentPecMailFacade As PECMailFacade
        Get
            If _currentPecMailFacade Is Nothing Then
                _currentPecMailFacade = New PECMailFacade()
            End If
            Return _currentPecMailFacade
        End Get
    End Property

#End Region

#Region "[ WSProt - Per ASMN-RE/AUSL-RE ]"

    ''' <summary> Permette di inserire un Protocollo partendo dal rispettivo xml. </summary>
    ''' <returns>Identificativo del protocollo appena creato</returns>
    Public Function InsertProtocol(ByVal protocolXml As ProtocolXML, ByVal username As String) As Protocol
        If Not CheckInsertPreviousYear() Then
            Throw New DocSuiteException("Impossibile inserire nuovi protocolli.", String.Format("Non è ancora stato eseguito il cambio anno. {0}", DocSuiteContext.Current.ProtocolEnv.DefaultErrorMessage))
        End If
        ' Creo il protocollo
        Dim protocol As Protocol = CreateProtocol()

        ' Contenitore
        protocol.Container = Factory.ContainerFacade.GetById(protocolXml.Container)
        protocol.Location = protocol.Container.ProtLocation
        protocol.AttachLocation = protocol.Container.ProtAttachLocation

        ' Classificatore
        protocol.Category = Factory.CategoryFacade.GetById(protocolXml.Category)
        ' Tipo documento
        Dim tableTypeFacade As New TableDocTypeFacade()
        Dim documentType As DocumentType = tableTypeFacade.GetById(protocolXml.DocumentType)
        If DocSuiteContext.Current.ProtocolEnv.IsTableDocTypeEnabled Then
            protocol.DocumentType = documentType
        End If

        ' Tipo di protocollo
        Dim protocolType As ProtocolType = Factory.ProtocolTypeFacade.GetById(protocolXml.Type)
        protocol.Type = protocolType

        ' Assegno i settori
        For Each idRole As Integer In protocolXml.Authorizations
            Dim role As Role = Factory.RoleFacade.GetById(idRole)
            If role Is Nothing Then
                Throw New ArgumentException(String.Format("Attenzione settore non presente. RoleId: {0}", idRole))
            End If
            protocol.AddRole(role, username, DateTimeOffset.UtcNow)
        Next

        ' Oggetto 
        protocol.ProtocolObject = StringHelper.Clean(protocolXml.Object, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)

        ' Data
        Dim data As Date
        If DateTime.TryParseExact(protocolXml.Data, "d/M/yyyy", Nothing, DateTimeStyles.None, data) Then
            protocol.DocumentDate = data
        Else
            protocol.DocumentDate = Nothing
        End If

        ' Note
        protocol.Note = StringHelper.Clean(protocolXml.Notes, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        ' Assegnatari/Proponente
        protocol.Subject = If(String.IsNullOrEmpty(protocolXml.Assignee), Nothing, protocolXml.Assignee)
        ' Categoria di servizio    
        protocol.ServiceCategory = If(String.IsNullOrEmpty(protocolXml.ServiceCode), Nothing, protocolXml.ServiceCode)

        ' Inserimento dei contatti dei Mittenti
        BindContactsFromProtocolXML(protocol, protocolXml.Senders, True)

        ' Inserimento dei contatti dei Destinatari
        BindContactsFromProtocolXML(protocol, protocolXml.Recipients, False)

        ' Stato di parcheggio del protocollo
        protocol.IdStatus = ProtocolStatusId.Errato

        ' Stato di ProtocolStatus
        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
            If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.ProtocolStatusWcfDefault) Then
                protocol.AdvanceProtocol.Status = FacadeFactory.Instance.ProtocolStatusFacade.GetById(DocSuiteContext.Current.ProtocolEnv.ProtocolStatusWcfDefault)
            End If
            If Not String.IsNullOrEmpty(protocolXml.Status) Then
                Dim currentStatus As ProtocolStatus = FacadeFactory.Instance.ProtocolStatusFacade.GetById(protocolXml.Status)
                If currentStatus Is Nothing Then
                    currentStatus = FacadeFactory.Instance.ProtocolStatusFacade.GetByDescription(protocolXml.Status).FirstOrDefault()
                End If
                If currentStatus IsNot Nothing Then
                    protocol.AdvanceProtocol.Status = currentStatus
                End If
            End If
        End If

        Save(protocol)

        Return protocol
    End Function

    ''' <summary> Permette di inserire i contatti nel protocollo dal xml, con validazione dei dati. </summary>
    ''' <param name="protocol">protocollo da creare dal protocolXML</param>
    ''' <param name="listContactBag">lista dei contatti</param>
    ''' <param name="isSender">true se contatti di tipo Mittete, altrimenti false in caso di destinatari</param>
    Private Sub BindContactsFromProtocolXML(ByRef protocol As Protocol, ByVal listContactBag As List(Of ContactBag), ByVal isSender As Boolean)
        Dim contactFacade As New ContactFacade()
        Dim contactTypeFacade As New ContactTypeFacade()

        For Each contactBag As ContactBag In listContactBag
            For Each contactXml As ContactXML In contactBag.Contacts
                Select Case contactBag.SourceType
                    Case ContactTypeEnum.AddressBook, ContactTypeEnum.AD
                        'Case ContactTypeEnum.IPA
                        Dim contact As Contact = contactFacade.GetById(contactXml.Id)
                        If contact Is Nothing Then
                            Throw New ArgumentException(String.Format("Attenzione contatto non presente in rubrica id: {0}", contactXml.Id), "id")
                        End If

                        If isSender Then  ' Mittenti
                            protocol.AddSender(contact, contactXml.Cc)
                        Else              ' Destinatari
                            protocol.AddRecipient(contact, contactXml.Cc)
                        End If
                    Case Else
                        ' Tipo di Contatto
                        Dim contactType As ContactType = contactTypeFacade.GetById(contactXml.Type(0))
                        If contactType Is Nothing Then
                            Throw New DocSuiteException(String.Format("Attenzione tipo di contatto non presente type: {0}", contactXml.Type))
                        End If

                        Dim contact As New Contact()
                        If contactXml.Description IsNot Nothing Then
                            contact.Description = contactXml.Description
                        Else
                            contact.Description = String.Format("{0}|{1}", contactXml.Surname, contactXml.Name)
                        End If

                        contact.ContactType = contactType
                        contact.EmailAddress = contactXml.StandardMail
                        contact.CertifiedMail = contactXml.CertifiedMail
                        contact.FiscalCode = contactXml.FiscalCode

                        ' Indirizzo contatto
                        Dim address As New Address
                        If contactXml.Address IsNot Nothing Then
                            address.Address = contactXml.Address.Name
                            address.City = contactXml.Address.City
                            address.ZipCode = contactXml.Address.Cap
                            address.CivicNumber = contactXml.Address.Number
                            address.CityCode = contactXml.Address.Prov
                            If Not String.IsNullOrEmpty(contactXml.Address.Type) Then
                                Dim contactPlaceNameFacade As New ContactPlaceNameFacade
                                Dim lstPlaceName As IList(Of ContactPlaceName) = contactPlaceNameFacade.GetByDescription(contactXml.Address.Type)
                                If lstPlaceName.Count > 0 Then
                                    address.PlaceName = lstPlaceName(0)
                                End If
                            End If
                        End If

                        contact.Address = address

                        contact.TelephoneNumber = contactXml.Telephone
                        contact.FaxNumber = contactXml.Fax
                        contact.Note = contactXml.Notes
                        ' Data
                        Dim data As Date
                        If DateTime.TryParseExact(contactXml.BirthDate, "d/M/yyyy", Nothing, DateTimeStyles.None, data) Then
                            contact.BirthDate = data
                        Else
                            contact.BirthDate = Nothing
                        End If

                        contact.BirthPlace = contactXml.BirthPlace

                        If isSender Then ' Mittenti
                            protocol.AddSenderManual(contact, contactXml.Cc)
                        Else             ' Destinatari
                            protocol.AddRecipientManual(contact, contactXml.Cc)
                        End If
                End Select
            Next
        Next
    End Sub

    ''' <summary> Validazione xml protocollo come da GUI. </summary>
    ''' <param name="protocolXML">xml del protocollo</param>
    Public Sub CheckDataForInsert(ByVal protocolXML As ProtocolXML)

        ' Verifico il tipo di protocollo
        Dim protocolTypeFacade As New ProtocolTypeFacade
        Dim protocolType As ProtocolType = protocolTypeFacade.GetById(protocolXML.Type)
        If protocolType Is Nothing Then
            Throw New ArgumentException("Impossibile salvare. Il Campo TIPO PROTOCOLLO non è valorizzato o non è numerico. Valore Attuale: " & protocolXML.Type)
        End If

        ' Verifica data protocollo mittente
        If protocolXML.Type = -1 Then
            If protocolXML.Data Is Nothing Then
                protocolXML.Data = DateTime.Today.ToString()
            End If
            If DateTime.Parse(protocolXML.Data) > DateTime.Today Then
                Throw New ArgumentException("Impossibile salvare.La data del protocollo mittente deve essere antecedente a quella odierna. Verificare xml")
            End If
        End If

        'Verifica oggetto
        If protocolXML.Object.Length > 255 Then
            Throw New ArgumentException("Impossibile salvare. Il campo Oggetto ha superato i caratteri disponibili.\n\n" & "(Caratteri " & protocolXML.Object.Length & " Disponibili " & 255 & ")")
        End If

        If DocSuiteContext.Current.ProtocolEnv.ObjectMinLength <> 0 Then
            If protocolXML.Object.Length < DocSuiteContext.Current.ProtocolEnv.ObjectMinLength Then
                Throw New ArgumentException("Impossibile salvare. Il campo Oggetto non ha raggiunto i caratteri minimi (" & DocSuiteContext.Current.ProtocolEnv.ObjectMinLength & " caratteri).")
            End If
        End If

        'Verifico il contenitore
        Dim container As Container = Factory.ContainerFacade.GetById(protocolXML.Container)
        If container Is Nothing Then
            Throw New ArgumentException(String.Format("Attenzione contenitore con id = {0} non presente", protocolXML.Container))
        End If

        If container.ProtLocation Is Nothing Then
            Throw New ArgumentException(String.Format("Impossibile salvare. La LOCATION del Protocollo per il Contenitore {0} non è stata definita.", container.Name))
        End If

        Dim tableTypeFacade As New TableDocTypeFacade()
        Dim documentType As DocumentType = tableTypeFacade.GetById(protocolXML.DocumentType)
        If DocSuiteContext.Current.ProtocolEnv.EnvTableDocTypeRequired AndAlso (documentType Is Nothing) Then
            Throw New ArgumentException("Impossibile salvare. Il Campo TIPO DOCUMENTO non è valorizzato o non è numerico. Valore Attuale: " & protocolXML.DocumentType)
        End If

        Dim category As Category = Factory.CategoryFacade.GetById(protocolXML.Category)
        If category Is Nothing Then
            Throw New ArgumentException(String.Format("Attenzione Categoria con id = {0} non presente", protocolXML.Category))
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled AndAlso Not String.IsNullOrEmpty(protocolXML.Status) Then
            Dim results As IList(Of ProtocolStatus) = FacadeFactory.Instance.ProtocolStatusFacade.GetByDescription(protocolXML.Status)
            If (results Is Nothing OrElse Not results.Any()) AndAlso
                FacadeFactory.Instance.ProtocolStatusFacade.GetById(protocolXML.Status) Is Nothing Then
                Throw New ArgumentException(String.Format("Attenzione Stato protocollo {0} non presente (sia come ID che come descrizione)", protocolXML.Status))
            End If
        End If

    End Sub

    ''' <summary> Permette di inserire il documento principale in formato stream ad un protocollo </summary>
    ''' <param name="protocol">protocollo sul quale si vuole carticare il documento principale</param>
    ''' <param name="stream">stream del documento</param>
    ''' <param name="documentName">nome del documento da caricare</param>
    Public Sub BiblosInsert(ByVal protocol As Protocol, ByVal stream As Byte(), ByVal documentName As String, ByVal username As String)

        ' Aggiungo a biblos su una nuova catena il documento principale passato come stream
        Dim doc As New MemoryDocumentInfo(stream, documentName)
        doc.Signature = GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Main})

        protocol.IdDocument = doc.ArchiveInBiblos(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB).BiblosChainId
        protocol.DocumentCode = documentName

        ' Aggiorno il protocollo con il riferimento all'id del documento su biblos
        Update(protocol)
        ' Log dell'inserimento
        Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, String.Format("Documento (Add by {0}): {1}", username, documentName))
    End Sub

    ''' <summary> Permette di inserire un documento come allegato in formato stream ad un protocollo </summary>
    ''' <param name="protocol">protocollo sul quale si vuole caricare l'allegato</param>
    ''' <param name="stream">stream del documento</param>
    ''' <param name="documentName">nome del documento da caricare</param>
    Public Sub BiblosInsertAllegati(ByVal protocol As Protocol, ByVal stream As Byte(), ByVal documentName As String, ByVal username As String)

        ' Aggiungo a biblos l'allegato passato come stream
        Dim doc As New MemoryDocumentInfo(stream, documentName)
        doc.Signature = GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Attachment})

        Dim loc As UIDLocation = GetAttachmentLocation(protocol)
        protocol.IdAttachments = doc.ArchiveInBiblos(loc.Server, loc.Archive, protocol.IdAttachments.GetValueOrDefault(0)).BiblosChainId

        ' Aggiorno il protocollo con il riferimento all'id degli allegati su biblos
        Update(protocol)
        ' Log dell'inserimento allegato
        Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, String.Format("Allegati (Add by {0}): {1}", username, documentName))
    End Sub

    ''' <summary> Crea un xml che idica l'elenco completo di tutte le PEC (ingresso e uscita) </summary>
    ''' <param name="protocol">oggetto protocollo</param>
    ''' <returns>xml con elenco completo PEC</returns>
    Public Function GetPecs(ByVal protocol As Protocol) As String
        Dim protocolPecsXML As New ProtocolPecsXML
        protocolPecsXML.Year = protocol.Year
        protocolPecsXML.Number = protocol.Number

        If protocol.PecMails.Count > 0 Then
            protocolPecsXML.Pecs = New List(Of PECXML)
        End If
        For Each pecMail As PECMail In protocol.PecMails
            Dim pec As New PECXML
            pec.Id = pecMail.Id

            ' PecMailBox
            Dim mailBox As New Mailbox
            mailBox.Id = pecMail.MailBox.Id
            mailBox.AssociatedMail = pecMail.MailBox.MailBoxName
            pec.Mailbox = mailBox

            pec.Direction = pecMail.Direction
            pec.Subject = pecMail.MailSubject
            pec.Sender = pecMail.MailSenders

            ' Recipients
            Dim lstRecipient As List(Of String) = StringHelper.ConvertStringToList(Of String)(pecMail.MailRecipients, ";"c)
            If lstRecipient.Count > 0 Then
                pec.Recipients = New List(Of RecipientXML)
                For Each item As String In lstRecipient
                    Dim recipient As New RecipientXML
                    recipient.Name = item
                    recipient.Type = "to" 'cc  e ccn sono da implementare mancano nel sistema PEC
                    pec.Recipients.Add(recipient)
                Next
            End If

            pec.Timestamp = If(pecMail.MailDate Is Nothing, Nothing, pecMail.MailDate.ToString())

            ' CertificationData
            Dim certificationData As New CertificationData
            ' è una pecmail certificata
            certificationData.IsCertified = CurrentPecMailFacade.IsCertified(pecMail)
            ' é una pecmail interoperabile
            If pecMail.Segnatura IsNot Nothing AndAlso pecMail.IsValidForInterop Then
                certificationData.IsInterop = True
            Else
                certificationData.IsInterop = False
            End If
            pec.CertificationData = certificationData

            ' Body della mail
            pec.Body = pecMail.MailBody

            ' Receipt
            If pecMail.Direction = PECMailDirection.Outgoing Then
                ' Ottengo tutti i messaggi in uscita con il medesimo XRiferimentoMessageID
                Dim lstPecMailXRif As IList(Of PECMail) = CurrentPecMailFacade.GetIncomingMailByXRiferimentoMessageId(pecMail.XRiferimentoMessageID)

                pec.Receipts = New List(Of Receipt)
                For Each item As PECMail In lstPecMailXRif
                    Dim receipt As New Receipt
                    receipt.Timestamp = If(item.MailDate Is Nothing, Nothing, item.MailDate.Value.ToString())
                    receipt.Type = item.MailType
                    If item.MailError IsNot Nothing Then
                        receipt.Value = item.MailError
                    End If
                    pec.Receipts.Add(receipt)
                Next
            End If

            protocolPecsXML.Pecs.Add(pec)
        Next
        Return SerializationHelper.SerializeToStringWithoutNamespace(protocolPecsXML)
    End Function

    ''' <summary> Ritorna xml con info del protocollo passato come parametro. </summary>
    Public Function GetProtocolInfo(ByVal protocol As Protocol) As String
        Dim protocolXml As New ProtocolXML
        protocolXml.RegistrationDate = protocol.RegistrationDate
        protocolXml.Year = protocol.Year
        protocolXml.Number = protocol.Number
        protocolXml.Type = protocol.Type.Id
        protocolXml.Container = protocol.Container.Id
        protocolXml.DocumentType = protocolXml.DocumentType

        If protocol.DocumentDate.HasValue Then
            protocolXml.Data = If(protocol.DocumentDate Is Nothing, Nothing, protocol.DocumentDate.Value.ToString())
        End If

        protocolXml.Object = protocol.ProtocolObject
        protocolXml.Category = protocol.Category.Id
        protocolXml.Notes = protocol.Note
        protocolXml.Assignee = protocol.Subject
        protocolXml.ServiceCode = protocol.ServiceCategory
        protocolXml.IdStatus = protocol.IdStatus.Value

        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled AndAlso protocol.Status IsNot Nothing Then
            protocolXml.Status = protocol.Status.Id
        End If

        ' Settori
        protocolXml.Authorizations = New List(Of Integer)
        For Each role As ProtocolRole In protocol.Roles
            protocolXml.Authorizations.Add(role.Role.Id)
        Next

        ' Ottengo i riferimenti di biblos del documento principale e allegati
        Dim doc As BiblosDocumentInfo = GetDocument(protocol)
        Dim attachments() As BiblosDocumentInfo = GetAttachments(protocol)

        ' xmlelement Documents
        Dim documentsXml As New DocumentsXml()

        ' Creo xmlElement del documento principale 
        documentsXml.MainDocument = New List(Of BiblosDocumentInfoXml)
        If doc IsNot Nothing Then
            SetBiblosDocumentInfoXml(doc, documentsXml.MainDocument)
        End If

        ' Creo xmlelement degli allegati se presenti
        If attachments.Length > 0 Then
            documentsXml.Attachments = New List(Of BiblosDocumentInfoXml)
            SetBiblosDocumentInfoXml(attachments, documentsXml.Attachments)
        End If

        protocolXml.Document = documentsXml

        ' Mittenti
        GetProtocolXMLContact(GetSenders(protocol), protocolXml.Senders)
        ' Destinatari
        GetProtocolXMLContact(GetRecipients(protocol), protocolXml.Recipients)

        Return SerializationHelper.SerializeToStringWithoutNamespace(protocolXml)
    End Function

    Private Sub SetBiblosDocumentInfoXml(ByVal doc() As BiblosDocumentInfo, ByRef biblosDocumentInfo As List(Of BiblosDocumentInfoXml))
        For Each item As BiblosDocumentInfo In doc
            SetBiblosDocumentInfoXml(item, biblosDocumentInfo)
        Next
    End Sub

    ''' <summary>
    ''' Crea una lista di biblosDocumentInfo per identificare la location del documento principale e/o allegati
    ''' </summary>
    Private Sub SetBiblosDocumentInfoXml(ByVal doc As BiblosDocumentInfo, ByRef biblosDocumentInfo As List(Of BiblosDocumentInfoXml))
        Dim indexPosition As Integer = Services.Biblos.Models.BiblosDocumentInfo.GetInChainPosition(doc.Server, doc.DocumentParentId, doc.DocumentId)
        Dim biblosDocInfo As New BiblosDocumentInfoXml
        biblosDocInfo.Server = doc.Server
        biblosDocInfo.Archive = doc.ArchiveName
        biblosDocInfo.Id = doc.BiblosChainId
        biblosDocInfo.Enum = indexPosition
        biblosDocInfo.ChainId = doc.ChainId.ToString()
        biblosDocInfo.DocumentId = doc.DocumentId.ToString()

        biblosDocumentInfo.Add(biblosDocInfo)
    End Sub

    Public Sub GetProtocolXMLContact(ByVal lstContactDTO As List(Of Data.ContactDTO), ByRef lstContactBag As List(Of ContactBag))
        ' Contatti da rubrica
        Dim contactBagRubrica As New ContactBag
        contactBagRubrica.Contacts = New List(Of ContactXML)
        contactBagRubrica.SourceType = ContactTypeEnum.AddressBook
        ' Contatti da manuali
        Dim contactBagManuale As New ContactBag
        contactBagManuale.Contacts = New List(Of ContactXML)
        contactBagManuale.SourceType = ContactTypeEnum.Manual

        ' MITTENTI o DESTINATARI
        For Each item As Data.ContactDTO In lstContactDTO
            Dim contactXML As New ContactXML
            Select Case item.Type
                Case Data.ContactDTO.ContactType.Address
                    contactXML.Id = item.Contact.Id
                    contactXML.Type = item.Contact.ContactType.Id.ToString()
                    contactXML.Cc = item.IsCopiaConoscenza
                    contactBagRubrica.Contacts.Add(contactXML)
                Case Else
                    If item.Contact.Description.Contains("|") Then
                        Dim array As String() = item.Contact.Description.Split("|"c)
                        If array.Count > 1 Then
                            contactXML.Surname = array(0)
                            contactXML.Name = array(1)
                        Else
                            contactXML.Surname = array(0)
                        End If
                    Else
                        contactXML.Description = item.Contact.Description
                    End If
                    contactXML.Type = item.Contact.ContactType.Id.ToString()
                    contactXML.Cc = item.IsCopiaConoscenza
                    contactXML.StandardMail = item.Contact.EmailAddress
                    contactXML.CertifiedMail = item.Contact.CertifiedMail
                    contactXML.FiscalCode = item.Contact.FiscalCode

                    ' Address
                    If item.Contact.Address IsNot Nothing Then
                        contactXML.Address = New AddressXML()
                        contactXML.Address.Cap = item.Contact.Address.ZipCode
                        contactXML.Address.City = item.Contact.Address.City
                        contactXML.Address.Name = item.Contact.Address.Address
                        contactXML.Address.Prov = item.Contact.Address.CityCode
                        If item.Contact.Address.PlaceName IsNot Nothing Then
                            contactXML.Address.Type = item.Contact.Address.PlaceName.Description
                        End If
                    End If

                    contactXML.Telephone = item.Contact.TelephoneNumber
                    contactXML.Fax = item.Contact.FaxNumber
                    contactXML.Notes = item.Contact.Note

                    If item.Contact.BirthDate.HasValue Then
                        contactXML.BirthDate = item.Contact.BirthDate.Value.ToString()
                    Else
                        contactXML.BirthDate = Nothing
                    End If

                    contactBagManuale.Contacts.Add(contactXML)
            End Select
        Next

        lstContactBag = New List(Of ContactBag)
        If contactBagRubrica.Contacts.Count > 0 Then
            lstContactBag.Add(contactBagRubrica)
        End If
        If contactBagManuale.Contacts.Count > 0 Then
            lstContactBag.Add(contactBagManuale)
        End If
    End Sub

    Public Function GetSenders(protocol As Protocol) As List(Of Data.ContactDTO)
        Return GetContacts(protocol, ProtocolContactCommunicationType.Sender)
    End Function

    Public Function GetRecipients(protocol As Protocol) As List(Of Data.ContactDTO)
        Return GetContacts(protocol, ProtocolContactCommunicationType.Recipient)
    End Function

    Public Function GetContacts(protocol As Protocol, comunicationType As String) As List(Of Data.ContactDTO)
        Dim tor As New List(Of Data.ContactDTO)

        'esegue il caricamento dei contatti da rubrica
        Dim l As IList(Of ProtocolContact) = Factory.ProtocolContactFacade.GetByComunicationType(protocol.Year, protocol.Number, comunicationType)
        For Each protContact As ProtocolContact In l
            Dim vContactDto As New Data.ContactDTO()
            vContactDto.Contact = protContact.Contact

            Dim contactName As ContactName = ContactNamesFacade.GetContactNamesByValidDate(protContact.Contact.Id, protocol.RegistrationDate.ToLocalTime().DateTime)
            If contactName IsNot Nothing Then
                vContactDto.Contact.Description = contactName.Name
            End If
            vContactDto.IsCopiaConoscenza = protContact.Type = "CC"
            vContactDto.Type = Data.ContactDTO.ContactType.Address
            tor.Add(vContactDto)
        Next

        'esegue il caricamento dei contatti manuali
        Dim lm As IList(Of ProtocolContactManual) = Factory.ProtocolContactManualFacade.GetByComunicationType(protocol.Year, protocol.Number, comunicationType)
        For Each protContact As ProtocolContactManual In lm
            Dim vContactDto As New Data.ContactDTO()
            vContactDto.Contact = protContact.Contact
            vContactDto.IsCopiaConoscenza = protContact.Type = "CC"
            vContactDto.Type = Data.ContactDTO.ContactType.Manual
            vContactDto.IdManualContact = protContact.Id
            tor.Add(vContactDto)
        Next


        Return tor
    End Function

#End Region

#Region " NEW FUNCTIONS "

    ''' <summary> Ottiene la lista con tutti i documenti del protocollo. </summary>
    Public Shared Function GetAllDocuments(ByVal protocol As Protocol) As List(Of DocumentInfo)
        Dim list As New List(Of DocumentInfo)

        Dim doc As BiblosDocumentInfo = GetDocument(protocol)
        If doc IsNot Nothing Then
            list.Add(doc)
        End If

        list.AddRange(GetAttachments(protocol))

        list.AddRange(GetAnnexes(protocol))
        Return list
    End Function

    Public Shared Function GetDocument(prot As Protocol, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo
        Dim doc As BiblosDocumentInfo = Nothing
        If prot.IdDocument.GetValueOrDefault(0) > 0 Then
            doc = New BiblosDocumentInfo(prot.Location.DocumentServer, prot.Location.ProtBiblosDSDB, prot.IdDocument.GetValueOrDefault(0))
            If (doc IsNot Nothing AndAlso includeUniqueId) Then
                doc = SetProtocolUniqueIdAttribute(doc, prot.UniqueId, DSWEnvironment.Protocol)
            End If
        End If
        Return doc
    End Function

    Public Shared Function GetDocumentWithExternalKey(prot As Protocol) As BiblosDocumentInfo
        Dim tor As BiblosDocumentInfo = GetDocument(prot)
        If tor IsNot Nothing Then
            tor.ExternalKey = tor.DocumentId.ToString()
        End If
        Return tor
    End Function

    Public Shared Function GetUIDDocument(prot As Protocol) As UIDDocument
        FileLogger.Debug("WSProtLog", String.Format("ProtocolFacade.GetUIDDocument: prot.Location.DocumentServer = {0} prot.Location.ProtBiblosDSDB = {1}", prot.Location.DocumentServer, prot.Location.ProtBiblosDSDB))
        Return New UIDDocument(prot.Location.DocumentServer, prot.Location.ProtBiblosDSDB, prot.IdDocument.GetValueOrDefault(0), 0)

    End Function

    Public Shared Function GetAttachmentsUID(prot As Protocol) As UIDChain
        Dim protAttachmentLocation As UIDLocation = New ProtocolFacade().GetAttachmentLocation(prot)
        Return New UIDChain(protAttachmentLocation.Server, protAttachmentLocation.Archive, prot.IdAttachments.GetValueOrDefault(0))
    End Function

    Public Shared Function GetAttachments(prot As Protocol, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim structs As BiblosDocumentInfo() = New BiblosDocumentInfo() {}
        If prot.IdAttachments.GetValueOrDefault(0) > 0 Then
            Dim id As UIDChain = GetAttachmentsUID(prot)
            If id.Id > 0 Then
                structs = BiblosDocumentInfo.GetDocuments(id).ToArray()
                If (includeUniqueId) Then
                    For Each item As BiblosDocumentInfo In structs
                        item = SetProtocolUniqueIdAttribute(item, prot.UniqueId, DSWEnvironment.Protocol)
                    Next
                End If
            End If
        End If
        Return structs
    End Function

    Public Shared Function GetAttachmentsWithExternalKey(prot As Protocol) As BiblosDocumentInfo()
        Dim tor As BiblosDocumentInfo() = GetAttachments(prot)
        If tor IsNot Nothing Then
            For Each biblosDocumentInfo As BiblosDocumentInfo In tor
                biblosDocumentInfo.ExternalKey = biblosDocumentInfo.DocumentId.ToString()
            Next
        End If
        Return tor
    End Function

    ''' <summary> Permette di ottenere tutti gli annessi della catena contenente in protocol </summary>
    ''' <param name="prot">Protocollo dal quale si vuole ottenere gli annexes associati</param>
    ''' <returns>array di annexes incapsulati nell'oggetto BiblosDocumentInfo </returns>
    Public Shared Function GetAnnexes(prot As Protocol, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim structs As BiblosDocumentInfo() = New BiblosDocumentInfo() {}
        If prot.IdAnnexed <> Guid.Empty Then
            structs = BiblosDocumentInfo.GetDocuments(prot.Location.DocumentServer, prot.IdAnnexed).ToArray()
            If (includeUniqueId) Then
                For Each item As BiblosDocumentInfo In structs
                    item = SetProtocolUniqueIdAttribute(item, prot.UniqueId, DSWEnvironment.Protocol)
                Next
            End If
        End If
        Return structs
    End Function

    Public Shared Function GetAnnexesWithExternalKey(prot As Protocol) As BiblosDocumentInfo()
        Dim tor As BiblosDocumentInfo() = GetAnnexes(prot)
        If tor IsNot Nothing Then
            For Each biblosDocumentInfo As BiblosDocumentInfo In tor
                biblosDocumentInfo.ExternalKey = biblosDocumentInfo.DocumentId.ToString()
            Next
        End If
        Return tor
    End Function

    Public Shared Function GetDematerialised(prot As Protocol, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim structs As BiblosDocumentInfo() = New BiblosDocumentInfo() {}
        If prot.DematerialisationChainId.HasValue AndAlso prot.Location IsNot Nothing AndAlso Not String.IsNullOrEmpty(prot.Location.DocumentServer) Then
            structs = BiblosDocumentInfo.GetDocumentsLatestVersion(prot.Location.DocumentServer, prot.DematerialisationChainId.Value).ToArray()
            If (includeUniqueId) Then
                For Each item As BiblosDocumentInfo In structs
                    item = SetProtocolUniqueIdAttribute(item, prot.UniqueId, DSWEnvironment.Protocol)
                Next
            End If
        End If
        Return structs
    End Function

    Public Function GenerateSignature(ByVal protocol As Protocol, ByVal mydate As DateTime, ByVal info As ProtocolSignatureInfo) As String
        Dim args As New ProtocolEventArgs
        args.Protocol = protocol
        OnBeforeGenerateSignature(args)

        Dim signature As StringBuilder
        If Not args.Cancel Then
            signature = New StringBuilder()
            signature.AppendFormat("{0} {1} del {2:dd/MM/yyyy}", DocSuiteContext.Current.ProtocolEnv.SignatureString, protocol.Id, mydate)

            Select Case DocSuiteContext.Current.ProtocolEnv.SignatureType
                Case 0
                Case 1
                    signature.Insert(0, " ")
                    signature.Insert(0, DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
                    If info IsNot Nothing Then
                        Select Case info.DocumentType
                            Case ProtocolDocumentType.Attachment
                                signature.Append(" (Allegato)")
                            Case ProtocolDocumentType.Annexed
                                signature.Append(" (Annesso)")
                        End Select
                    End If

                Case 2
                    signature.Insert(0, " ")
                    signature.Insert(0, DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
                    signature.AppendFormat(" {0}", protocol.Container.Name)

                Case 3
                    signature.Insert(0, " ")
                    signature.Insert(0, DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
                    Select Case protocol.Type.Id
                        Case -1
                            signature.Append(" Ingresso")
                        Case 1
                            signature.Append(" Uscita")
                    End Select
                    signature.AppendFormat(" {0}", protocol.Container.Name)

                Case 4
                    signature.Insert(0, " ")
                    signature.Insert(0, DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
                    signature.AppendFormat(" {0}", protocol.Container.Name)

                    ' Recupero elenco dei settori
                    If Not protocol.Roles.IsNullOrEmpty() Then
                        Dim validRoles As IEnumerable(Of ProtocolRole) = protocol.Roles.Where(Function(protocolRole) protocolRole.Role IsNot Nothing AndAlso Not String.IsNullOrEmpty(protocolRole.Role.ServiceCode))
                        signature.AppendFormat(" [{0}]", String.Join("-", validRoles.Select(Function(protocolrole) protocolrole.Role.ServiceCode.ToUpper())))
                    End If

                Case 5
                    If info Is Nothing Then
                        info = New ProtocolSignatureInfo()
                    End If

                    Dim format As String = String.Empty

                    Select Case info.DocumentType
                        Case ProtocolDocumentType.Main
                            format = DocSuiteContext.Current.ProtocolEnv.ProtocolSignatureFormat
                        Case ProtocolDocumentType.Attachment
                            format = DocSuiteContext.Current.ProtocolEnv.AttachmentSignatureFormat
                        Case ProtocolDocumentType.Annexed
                            format = DocSuiteContext.Current.ProtocolEnv.AnnexedSignatureFormat
                    End Select

                    If Not String.IsNullOrEmpty(format) Then
                        signature = New StringBuilder()
                        signature.AppendFormat(New ProtocolSignatureFormatter(), format, DocSuiteContext.Current.ProtocolEnv, protocol, info)
                    Else
                        ' Se non ho un formato carico quello standard
                        ' noop
                    End If
                    ' TODO: vreificare se è voluto il salto della OnAfterGenerateSignature
                    Return signature.ToString()

                Case Else
                    signature = New StringBuilder()

            End Select
        Else
            signature = New StringBuilder(args.Tag.ToString())
        End If

        args = New ProtocolEventArgs()
        args.Protocol = protocol
        args.Tag = signature.ToString()
        OnAfterGenerateSignature(args)

        Return signature.ToString()
    End Function

    Public Function GetViewerLink(prot As Protocol) As String
        Dim querystring As String = String.Format("DataSourceType=prot&year={0}&number={1}", prot.Year, prot.Number)

        Return String.Concat("~/viewers/ProtocolViewer.aspx?", CommonShared.AppendSecurityCheck(querystring))
    End Function

    Public Function GetMergedDocuments(protocol As Protocol, includeAttachments As ICollection(Of BiblosDocumentInfo)) As MergeDocumentResult
        Return GetMergedDocuments(protocol, includeAttachments, GetAnnexes(protocol))
    End Function

    ''' <summary> Esegue il merge dei documenti di un protocollo in un unico PDF </summary>
    Public Function GetMergedDocuments(protocol As Protocol, includeAttachments As ICollection(Of BiblosDocumentInfo), includeAnnexes As ICollection(Of BiblosDocumentInfo)) As MergeDocumentResult
        Dim merger As PdfMerge = New PdfMerge()
        Dim errors As IList(Of String) = New List(Of String)

        Dim documentsToMerge As List(Of DocumentInfo) = New List(Of DocumentInfo)()
        documentsToMerge.Add(GetDocument(protocol))
        If includeAttachments.Count > 0 Then
            documentsToMerge.AddRange(includeAttachments)
        End If

        If includeAnnexes.Count > 0 Then
            documentsToMerge.AddRange(includeAnnexes)
        End If

        For Each document As DocumentInfo In documentsToMerge.Where(Function(x) StringHelper.ExistsIn(DocSuiteContext.Current.ProtocolEnv.StampaConformeExtensions, Path.GetExtension(x.Name), "|"c))
            Try
                merger.AddDocument(document.GetPdfStream())
            Catch ex As Exception
                errors.Add(String.Concat("Errore in conversione documento ", document.Name, " -> ", ex.Message))
            End Try
        Next

        For Each excludedDocument As DocumentInfo In documentsToMerge.Where(Function(x) Not StringHelper.ExistsIn(DocSuiteContext.Current.ProtocolEnv.StampaConformeExtensions, Path.GetExtension(x.Name), "|"c))
            errors.Add(String.Concat("Non è possibile eseguire la conversione del documento ", excludedDocument.Name))
        Next

        Using stream As MemoryStream = New MemoryStream()
            merger.Merge(stream)
            Return New MergeDocumentResult() With {.MergedDocument = New MemoryDocumentInfo(stream.ToArray(), "Merged.pdf"), .Errors = errors}
        End Using
    End Function
#End Region

    Public Sub SendMail(ByVal protocol As Protocol)
        FileLogger.Info(LoggerName, String.Format("{0}: ProtocolnFacade.SendMail - Protocollo [{1}] per {2}-{3} : Preparazione invio", DocSuiteContext.Current.User.FullUserName, protocol.Id, protocol.Number, protocol.Year))
        NHibernateSessionManager.Instance.GetSessionFrom(ProtDB).Refresh(protocol)
        Dim idMessage As Integer = Factory.MessageEmailFacade.SendEmailMessage(CreateMailMessage(protocol, False))
        FileLogger.Info(LoggerName, String.Format("{0}: ProtocolnFacade.SendMail - Protocollo [{1}] per {2}-{3} : Mail inserita in coda di invio [id {4}]", DocSuiteContext.Current.User.FullUserName, protocol.Id, protocol.Number, protocol.Year, idMessage))
    End Sub

    Private Function CreateMailMessage(ByRef CurrentProtocol As Protocol, isDispositionNotification As Boolean) As MessageEmail
        Dim contacts As New List(Of MessageContactEmail)
        Dim authorizMailFrom As String = DocSuiteContext.Current.ProtocolEnv.AuthorizMailFrom
        Dim authorizMailTo As String = RoleFacade.GetEmailAddresses(CurrentProtocol.GetRoles())
        Dim protSignature As String = GetSignature(CurrentProtocol)
        'aggiungo l'oggetto
        Dim mailSubject As String = String.Format("{0} {1}", DocSuiteContext.ProductName, protSignature)
        'aggiungo il corpo
        Dim mailBody As String = GetBody(CurrentProtocol, protSignature)
        If Not String.IsNullOrEmpty(authorizMailFrom) Then
            Dim mails As Array = authorizMailFrom.Split(";"c)
            For Each mail As String In mails
                Dim newSender As MessageContactEmail = FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact("", DocSuiteContext.Current.User.FullUserName, mail, MessageContact.ContactPositionEnum.Sender)
                contacts.Add(newSender)
            Next
        End If
        If Not String.IsNullOrEmpty(authorizMailTo) Then
            Dim mails As Array = authorizMailTo.Split(";"c)
            For Each mail As String In mails
                Dim newRecipient As MessageContactEmail = FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact("", DocSuiteContext.Current.User.FullUserName, mail, MessageContact.ContactPositionEnum.Recipient)
                contacts.Add(newRecipient)
            Next
        End If
        If contacts.Count < 2 Then
            Throw New Exception("Contatti insufficienti per spedire la notifica.")
        End If
        Dim email As MessageEmail = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(contacts, mailSubject, mailBody, isDispositionNotification)
        email.Priority = System.Net.Mail.MailPriority.Normal
        Return email
    End Function

    Private Function GetSignature(ByRef CurrentProtocol As Protocol) As String
        Dim signature As String = String.Format("Protocollo n. {0} {1:0000000} del {2:dd/MM/yyyy}", CurrentProtocol.Year, CurrentProtocol.Number, CurrentProtocol.RegistrationDate.ToLocalTime())
        Return signature
    End Function
    Private Function GetBody(ByRef CurrentProtocol As Protocol, protSignature As String) As String
        Dim authorizMailBody As String = DocSuiteContext.Current.ProtocolEnv.AuthorizMailBody
        If String.IsNullOrEmpty(authorizMailBody) Then
            authorizMailBody = "<br /><a href='{3}?Tipo=Prot&Azione=Apri&Anno={0}&Numero={1}'>{2}</a>"
        End If
        Dim mailBody As String = String.Format(authorizMailBody, CurrentProtocol.Year, CurrentProtocol.Number, protSignature, DocSuiteContext.Current.CurrentTenant.DSWUrl)
        Return mailBody
    End Function


    Public Function CheckInsertPreviousYear() As Boolean
        If DocSuiteContext.Current.ProtocolEnv.AllowPreviousYear Then
            Return True
        End If
        If FacadeFactory.Instance.ParameterFacade.GetCurrent().LastUsedYear <> DateTime.Now.Year Then
            Return False
        End If
        Return True
    End Function

    Public Function InsertInvoiceProtocol(dto As ProtocolDTO) As ProtocolDTO
        Dim useSuspendedProtocol As Boolean = dto.UseProtocolReserve

        If Not DocSuiteContext.Current.ProtocolEnv.IsInvoiceEnabled Then
            Throw New Exception("Il modulo gestione fatture di protocollo non è più abilitato. Verificare le impostazioni con l'amministratore di sistema.")
        End If

        If DocSuiteContext.Current.ProtocolEnv.InvoiceSDIEnabled Then
            Throw New ArgumentException("Il modulo gestione fatture di protocollo non è più abilitato. Verificare le impostazioni con l'amministratore di sistema.")
        End If
        If DocSuiteContext.Current.ProtocolEnv.InvoiceSDIEnabled Then
            Throw New ArgumentException("Il modulo gestione fatture di protocollo non è più abilitato. Verificare le impostazioni con l'amministratore di sistema")
        End If
        If Not dto.AccountingYear.HasValue Then
            Throw New DocSuiteException("Anno IVA non specificato.")
        End If

        If (dto.UseProtocolReserve AndAlso dto.AccountingYear.Value = Date.Now.Year) Then
            Throw New Exception("Non è possibile utilizzare la riserva di protocollo per fatture afferenti all'anno fiscale corrente.")
        End If

        If (dto.UseProtocolReserve AndAlso (Not dto.ProtocolReserveFrom.HasValue OrElse Not dto.ProtocolReserveTo.HasValue)) Then
            Throw New Exception("Non è possibile utilizzare la riserva di protocollo senza aver specificare un perido di ricerca dei protocolli sospesi")
        End If

        If (dto.UseProtocolReserve AndAlso FacadeFactory.Instance.ProtocolFacade.CountProtSuspended(dto.AccountingYear.Value, dto.ProtocolReserveFrom.Value, dto.ProtocolReserveTo.Value) <= 0) Then
            Throw New Exception("Non sono disponibili protocolli sospesi nel periodo selezionato")
        End If

        If Not useSuspendedProtocol Then
            If Not CheckInsertPreviousYear() Then
                Throw New DocSuiteException("Impossibile inserire nuovi protocolli.", String.Format("Non è ancora stato eseguito il cambio anno. {0}", DocSuiteContext.Current.ProtocolEnv.DefaultErrorMessage))
            End If
        End If

        Dim containerExt As New ContainerExtensionFacade("ProtDB")
        Dim containerAsInvoice As Boolean = containerExt.GetByContainerAndKey(dto.Container.Id.Value, ContainerExtensionType.FT).Any()
        If Not containerAsInvoice Then
            Throw New Exception("Il contenitore selezionato non è abilitato alla Fatturazione.")
        End If

        Dim accountingSectional As String = String.Empty
        If dto.AccountingSectionalNumber.HasValue Then
            Dim containersSectional As IList(Of ContainerExtension) = containerExt.GetByContainerAndKey(dto.Container.Id.Value, ContainerExtensionType.SC)
            Dim sectionalNumberExist As Boolean = containersSectional.Any(Function(w) w.AccountingSectionalNumber.Equals(dto.AccountingSectionalNumber))
            If Not sectionalNumberExist Then
                Throw New Exception("Il sezionale numerico inserito non risulta configurato in DB.")
            End If
            accountingSectional = containersSectional.Where(Function(x) x.AccountingSectionalNumber.Equals(dto.AccountingSectionalNumber)).Select(Function(s) s.KeyValue).Single()
        End If

        Dim invoiceExist As Boolean = SerachInvoiceAccountingDouble(dto.Container.Id.Value, accountingSectional, dto.AccountingYear.Value, dto.AccountingNumber.Value).Any()
        If invoiceExist Then
            Throw New Exception("Fattura già presente.")
        End If

        If DocSuiteContext.Current.ProtocolEnv.ProtocolKindEnabled Then
            If Not dto.IdProtocolKind.HasValue Then
                Throw New Exception("Nessun ProtocolKind specificato.")
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsTableDocTypeEnabled Then
            If Not dto.IdDocumentType.HasValue Then
                Throw New Exception("Proprietà IdDocumentType non valorizzata.")
            End If
        End If

        If (dto.LinkReferenceContainer IsNot Nothing AndAlso dto.LinkReferenceContainer.Id.HasValue AndAlso
                (dto.Senders Is Nothing OrElse dto.Senders.Any() OrElse dto.Senders.Any(Function(f) String.IsNullOrEmpty(f.Description)))) Then

            Dim protocolParent As Protocol.AdvancedProtocol = SerachInvoiceAccountingDouble(dto.LinkReferenceContainer.Id.Value, accountingSectional, dto.AccountingYear.Value, dto.AccountingNumber.Value).FirstOrDefault()
            If (protocolParent IsNot Nothing) Then
                dto.Senders = protocolParent.Protocol.GetSenders().Select(Function(f) New API.ContactDTO() With {.Code = f.Contact.SearchCode}).ToArray()
            End If

        End If


        If Not dto.HasSenders() AndAlso Not dto.HasSendersManual() AndAlso Not dto.HasRecipients() AndAlso Not dto.HasRecipientsManual() Then
            Throw New Exception("Il protocollo non ha contatti specificati")
        End If

        Dim senders As List(Of Contact) = Nothing
        Dim sendersManual As List(Of Contact) = Nothing
        Dim recipients As List(Of Contact) = Nothing
        Dim recipientsManual As List(Of Contact) = Nothing

        Dim container As Container = FacadeFactory.Instance.ContainerFacade.GetById(dto.Container.Id.Value)
        Dim parent As Contact = FacadeFactory.Instance.ProtocolContactFacade.GetInvoiceContactGroup(container)

        If dto.HasSenders() Then
            senders = dto.Senders.Select(Function(s) GetContact(s, parent)).ToList()
            If Not senders.Any() Then
                Throw New Exception("Errore in gestione Mittenti.")
            End If
        End If

        If dto.HasSendersManual() Then
            sendersManual = dto.SendersManual.Select(Function(s) GetContactManual(s, parent)).ToList()
            If Not sendersManual.Any() Then
                Throw New Exception("Errore in gestione Mittenti manuali.")
            End If
        End If

        If dto.HasRecipients() Then
            recipients = dto.Recipients.Select(Function(s) GetContact(s, parent)).ToList()
            If Not recipients.Any() Then
                Throw New Exception("Errore in gestione Destinatari.")
            End If
            If Not recipients.All(Function(f) Not String.IsNullOrEmpty(f.FiscalCode)) Then
                Throw New Exception(String.Format("I destinatari {0} non possiedono Codice Fiscale/Partita IVA.", String.Join(", ", recipients.Where(Function(f) String.IsNullOrEmpty(f.FiscalCode)).Select(Function(f) f.Description).ToArray())))
            End If
        End If

        If dto.HasRecipientsManual() Then
            recipientsManual = dto.RecipientsManual.Select(Function(s) GetContactManual(s, parent)).ToList()
            If Not recipientsManual.Any() Then
                Throw New Exception("Errore in gestione Destinatari manuali.")
            End If
        End If

        Dim protocol As Protocol = Nothing
        If useSuspendedProtocol AndAlso dto.AccountingYear.HasValue AndAlso dto.ProtocolReserveFrom.HasValue AndAlso dto.ProtocolReserveTo.HasValue Then
            protocol = Me.GetFirstProtocolSuspended(dto.AccountingYear.Value, dto.ProtocolReserveFrom.Value, dto.ProtocolReserveTo.Value)
        Else
            protocol = Me.CreateProtocol()
        End If
        protocol.Container = container
        protocol.Location = protocol.Container.ProtLocation
        protocol.AttachLocation = protocol.Container.ProtAttachLocation
        protocol.Type = FacadeFactory.Instance.ProtocolTypeFacade.GetById(dto.Direction.Value)

        protocol.IdStatus = ProtocolStatusId.Errato
        If DocSuiteContext.Current.ProtocolEnv.ProtocolKindEnabled Then
            protocol.IdProtocolKind = dto.IdProtocolKind.Value
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsTableDocTypeEnabled Then
            protocol.DocumentType = FacadeFactory.Instance.DocumentTypeFacade.GetById(dto.IdDocumentType.Value)
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
            protocol.Status = FacadeFactory.Instance.ProtocolStatusFacade.GetById("C")
        End If

        If dto.HasSenders() AndAlso senders IsNot Nothing Then
            For Each sender As Contact In senders
                protocol.AddSender(sender, False)
            Next
        End If

        If dto.HasSendersManual() AndAlso sendersManual IsNot Nothing Then
            For Each sender As Contact In sendersManual
                protocol.AddSenderManual(sender, False)
            Next
        End If

        If dto.HasRecipients() AndAlso recipients IsNot Nothing Then
            For Each recipient As Contact In recipients
                protocol.AddRecipient(recipient, False)
            Next
        End If

        If dto.HasRecipientsManual() AndAlso recipientsManual IsNot Nothing Then
            For Each recipientManual As Contact In recipientsManual
                protocol.AddRecipientManual(recipientManual, False)
            Next
        End If

        protocol.Category = FacadeFactory.Instance.CategoryFacade.GetById(dto.Category.Id.Value)
        protocol.ProtocolObject = Me.PurgeText(dto.Subject)
        AddProtocolInvoice(protocol, dto.InvoiceNumber, dto.InvoiceDate, accountingSectional, dto.AccountingYear, dto.AccountingDate, dto.AccountingNumber, dto.AccountingSectionalNumber)

        'Se viene utilizzato un protocollo sospeso devo eseguire un Update, altrimenti eseguo un Save semplice.
        If useSuspendedProtocol Then
            UpdateOnly(protocol)
            FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, "Modificato protocollo da API.")
        Else
            Save(protocol)
            FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PI, "Inserimento fattura da API.")
        End If

        'Inizio a gestire i documenti
        Dim attachmentsCount As Integer = 0
        If dto.HasAttachments() Then
            attachmentsCount = dto.Attachments.Count
        End If
        Dim signatureInfo As New ProtocolSignatureInfo(attachmentsCount)
        Dim invoiceAttribute As Dictionary(Of String, String) = GetDocumentAttributes(protocol, True)

        FileLogger.Debug(LoggerName, String.Concat("invoiceAttribute.Count ->", invoiceAttribute.Count))
        For Each attr As KeyValuePair(Of String, String) In invoiceAttribute
            FileLogger.Debug(LoggerName, String.Concat(attr.Key, " ->", attr.Value))
        Next

        ' Archivio il documento principale.
        Dim docInfo As DocumentInfo = dto.Document.ToDocumentInfos().First()
        AddDocument(protocol, docInfo, signatureInfo, invoiceAttribute)
        protocol.DocumentCode = dto.Document.Name

        ' Archivio gli allegati parte integrante.
        If dto.HasAttachments() Then
            Dim attachments As List(Of DocumentInfo) = dto.Attachments.SelectMany(Function(a) a.ToDocumentInfos()).ToList()
            AddAttachments(protocol, attachments, signatureInfo)
        End If

        ' Archivio gli annessi.
        If dto.HasAnnexes() Then
            Dim annexes As List(Of DocumentInfo) = dto.Annexes.SelectMany(Function(a) a.ToDocumentInfos()).ToList()
            AddAnnexes(protocol, annexes, signatureInfo)
        End If

        ' A questo punto i vari documenti dovrebbero essere stati archiviati correttamente in Biblos.
        ' Posso quindi procedere ad attivare il protocollo.
        protocol.IdStatus = ProtocolStatusId.Attivo
        UpdateNoLastChange(protocol)
        'Invio comando di inserimento alle web api
        SendInsertProtocolCommand(protocol)
        If (dto.LinkReferenceContainer IsNot Nothing AndAlso dto.LinkReferenceContainer.Id.HasValue) Then
            Dim protocolParent As Protocol.AdvancedProtocol = SerachInvoiceAccountingDouble(dto.LinkReferenceContainer.Id.Value, accountingSectional, dto.AccountingYear.Value, dto.AccountingNumber.Value).FirstOrDefault()
            If (protocolParent IsNot Nothing) Then
                AddProtocolLink(protocolParent.Protocol, protocol, ProtocolLinkType.Normale, Nothing)
            End If
        End If

        ' Costruisco il dto di risposta.
        Dim dtoResult As New ProtocolDTO(protocol.Year, protocol.Number)
        If protocol.IdDocument.HasValue Then
            dtoResult.AddBiblosDocument(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB, protocol.IdDocument.Value)
        End If
        If protocol.IdAttachments.HasValue Then
            dtoResult.AddBiblosAttachment(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB, protocol.IdAttachments.Value)
        End If
        If Not protocol.IdAnnexed.IsEmpty() Then
            dtoResult.AddBiblosAnnexed(protocol.Location.DocumentServer, protocol.IdAnnexed)
        End If
        dtoResult.Subject = dto.Subject
        dtoResult.IdProtocolKind = protocol.IdProtocolKind
        Return dtoResult
    End Function

    Public Function InsertProtocol(dto As ProtocolDTO) As ProtocolDTO
        If Not CheckInsertPreviousYear() Then
            Throw New DocSuiteException("Impossibile inserire nuovi protocolli.", String.Format("Non è ancora stato eseguito il cambio anno. {0}", DocSuiteContext.Current.ProtocolEnv.DefaultErrorMessage))
        End If
        Dim protocol As Protocol = Me.CreateProtocol()

        protocol.Container = FacadeFactory.Instance.ContainerFacade.GetById(dto.Container.Id.Value)
        protocol.Location = protocol.Container.ProtLocation
        protocol.AttachLocation = protocol.Container.ProtAttachLocation

        If DocSuiteContext.Current.ProtocolEnv.IsTableDocTypeEnabled Then
            protocol.DocumentType = FacadeFactory.Instance.DocumentTypeFacade.GetById(dto.IdDocumentType.Value)
        End If

        protocol.Type = FacadeFactory.Instance.ProtocolTypeFacade.GetById(dto.Direction.Value)

        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
            protocol.Status = FacadeFactory.Instance.ProtocolStatusFacade.GetById("C")
        End If

        If dto.HasSenders() Then
            Dim senders As List(Of Contact) = dto.Senders.Select(Function(d) Me.GetContact(d)).ToList()
            senders.ForEach(Sub(c)
                                If c.ContactType IsNot Nothing AndAlso c.ContactType.Id = ContactType.Mistery Then
                                    c.ContactType.Id = ContactType.Person
                                    protocol.AddSenderManual(c, False)
                                Else
                                    If c.ContactType IsNot Nothing AndAlso c.ContactType.Id = ContactType.Ipa Then
                                        protocol.AddSenderManual(c, False)
                                    Else
                                        protocol.AddSender(c, False)
                                    End If
                                End If
                            End Sub)
        End If

        If dto.HasRecipients() Then
            Dim recipients As List(Of Contact) = dto.Recipients.Select(Function(d) Me.GetContact(d)).ToList()
            recipients.ForEach(Sub(c)
                                   If c.ContactType IsNot Nothing AndAlso c.ContactType.Id = ContactType.Mistery Then
                                       c.ContactType.Id = ContactType.Person
                                       protocol.AddRecipientManual(c, False)
                                   Else
                                       If c.ContactType IsNot Nothing AndAlso c.ContactType.Id = ContactType.Ipa Then
                                           protocol.AddSenderManual(c, False)
                                       Else
                                           protocol.AddRecipient(c, False)
                                       End If
                                   End If
                               End Sub)
        End If

        If dto.HasRecipientsManual() Then
            Dim manualContact As Contact = Nothing
            For Each manualDto As IContactDTO In dto.RecipientsManual
                manualContact = New Contact With {
                    .Description = manualDto.Description,
                    .ContactType = New ContactType(ContactType.Person),
                    .Address = New Address() With {
                        .Address = manualDto.Address,
                        .City = manualDto.City,
                        .CityCode = manualDto.CityCode,
                        .CivicNumber = manualDto.CivicNumber,
                        .ZipCode = manualDto.ZipCode
                    },
                    .EmailAddress = manualDto.EmailAddress,
                    .Code = manualDto.Code
                }
                protocol.AddRecipientManual(manualContact, False)
            Next
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsIssueEnabled Then
            If dto.HasFascicles() Then
                Dim fascicles As List(Of Contact) = dto.Fascicles.Select(Function(f) Me.GetContact(f)).ToList()
                ' Considero i soli contatti da rubrica.
                Dim available As List(Of Contact) = fascicles.Where(Function(f) f.Id > 0).ToList()
                available.ForEach(Sub(c)
                                      c.ContactType.Id = ContactType.Person
                                      Me.AddProtocolContactIssue(protocol, c)
                                  End Sub)
            End If
        End If

        protocol.Category = FacadeFactory.Instance.CategoryFacade.GetById(dto.Category.Id.Value)

        protocol.ProtocolObject = Me.PurgeText(dto.Subject)

        protocol.IdStatus = ProtocolStatusId.Errato
        Me.Save(protocol)
        FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PI, "Inserimento da API.")

        Dim attachmentsCount As Integer = 0
        If dto.HasAttachments() Then
            attachmentsCount = dto.Attachments.Count
        End If
        Dim signatureInfo As New ProtocolSignatureInfo(attachmentsCount)

        ' Archivio il documento principale.
        Me.AddDocument(protocol, dto.Document.ToDocumentInfos().First(), signatureInfo)
        protocol.DocumentCode = dto.Document.Name

        ' Archivio gli allegati parte integrante.
        If dto.HasAttachments() Then
            Dim attachments As IList(Of DocumentInfo) = dto.Attachments.SelectMany(Function(a) a.ToDocumentInfos()).ToList()
            Me.AddAttachments(protocol, attachments, signatureInfo)
        End If

        ' Archivio gli annessi.
        If dto.HasAnnexes() Then
            Dim annexes As List(Of DocumentInfo) = dto.Annexes.SelectMany(Function(a) a.ToDocumentInfos()).ToList()
            Me.AddAnnexes(protocol, annexes, signatureInfo)
        End If

        ' A questo punto i vari documenti dovrebbero essere stati archiviati correttamente in Biblos.
        ' Posso quindi procedere ad attivare il protocollo.
        protocol.IdStatus = ProtocolStatusId.Attivo
        Me.UpdateNoLastChange(protocol)
        'Invio comando di creazione protocollo alle web api
        SendInsertProtocolCommand(protocol)

        ' Costruisco il dto di risposta.
        Dim result As New ProtocolDTO(protocol.Year, protocol.Number)
        If protocol.IdDocument.HasValue Then
            result.AddBiblosDocument(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB, protocol.IdDocument.Value)
        End If
        If protocol.IdAttachments.HasValue Then
            result.AddBiblosAttachment(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB, protocol.IdAttachments)
        End If
        If Not protocol.IdAnnexed.IsEmpty() Then
            result.AddBiblosAnnexed(protocol.Location.DocumentServer, protocol.IdAnnexed)
        End If

        Return result
    End Function

    Private Function PurgeText(text As String) As String
        Dim purged As String = text

        purged = StringHelper.ReplaceCrLf(purged)
        If Not String.IsNullOrWhiteSpace(DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString) Then
            purged = StringHelper.Clean(purged, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If

        Return purged
    End Function

    Public Function GetContact(dto As API.IContactDTO) As Contact
        Return GetContact(dto, Nothing)
    End Function

    Public Function GetContact(dto As API.IContactDTO, parent As Contact) As Contact

        If Not String.IsNullOrWhiteSpace(dto.Code) Then
            Dim foundBySearchCode As IList(Of Contact) = FacadeFactory.Instance.ContactFacade.GetContactBySearchCode(dto.Code, CShort(1))
            If Not foundBySearchCode.IsNullOrEmpty() Then
                Return foundBySearchCode.OrderByDescending(Function(f) f.Id).First()
            End If
        End If

        If Not String.IsNullOrWhiteSpace(dto.EmailAddress) Then
            Dim foundByMail As IList(Of Contact) = FacadeFactory.Instance.ContactFacade.GetByMail(dto.EmailAddress)
            If Not foundByMail.IsNullOrEmpty() Then
                Return foundByMail.OrderByDescending(Function(f) f.Id).First()
            End If
        End If

        If Not String.IsNullOrWhiteSpace(dto.Code) AndAlso DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderIPAContactEnabled Then
            Dim foundByIpa As IList(Of IPA) = IPARetriever.GetIpaEntities(DocSuiteContext.Current.ProtocolEnv.LdapIndicePa, dto.Code)
            If foundByIpa.Count() = 1 Then
                Return ContactFacade.CreateFromIpa(foundByIpa.Single())
            End If
        End If

        'Ritorno un nuovo contatto
        Dim contact As New Contact()
        FacadeFactory.Instance.ContactFacade.CreateFromDto(contact, dto, parent)

        Return contact
    End Function

    ''' <summary>
    ''' Trasforma il dto in Contact. Per mantenere la compatibilità con <see cref="GetContact(IContactDTO, Contact)"/>
    ''' </summary>
    ''' <param name="dto"></param>
    ''' <param name="parent"></param>
    ''' <returns></returns>
    Public Function GetContactManual(dto As API.IContactDTO, parent As Contact) As Contact
        'Ritorno un nuovo contatto
        Dim contact As New Contact()
        contact.Code = dto.Code
        contact.ContactType = New ContactType(ContactType.Person)
        contact.CertifiedMail = dto.EmailAddress
        contact.EmailAddress = contact.CertifiedMail
        contact.Description = dto.Description
        If String.IsNullOrWhiteSpace(contact.Description) Then
            contact.Description = contact.CertifiedMail
        End If
        If Not String.IsNullOrEmpty(dto.FiscalCode) Then
            contact.FiscalCode = dto.FiscalCode
        End If
        If parent IsNot Nothing Then
            contact.Parent = parent
        End If
        contact.IsActive = 1

        Return contact
    End Function

    ''' <summary> Calcola il numero di protocollo </summary>
    ''' <remarks>
    ''' TODO: Questo codice è presente da mille altre parti, centralizzarlo in <see cref="Protocol"/>
    ''' </remarks>
    Public Shared Function ProtocolFullNumber(ByVal year As Short, ByVal number As Integer, Optional ByVal slash As String = "/") As String
        Return String.Format("{0}{1}{2:0000000}", year, slash, number)
    End Function

    Public Shared Sub ProtNumber(ByVal s As String, ByRef year As Short, ByRef number As Integer)
        Dim i As Integer = InStr(s, "/")
        Dim b As Integer = InStr(s, " ")
        year = Short.Parse(Left(s, i - 1))
        If b = 0 Then
            number = Integer.Parse(Mid(s, i + 1))
        Else
            number = Integer.Parse(Mid(s, i + 1, b - i - 1))
        End If
    End Sub

    Public Sub AddDocument(protocol As Protocol, document As DocumentInfo)
        AddDocument(protocol, document, New ProtocolSignatureInfo(), New Dictionary(Of String, String))
    End Sub

    Public Sub AddDocument(protocol As Protocol, document As DocumentInfo, info As ProtocolSignatureInfo)
        AddDocument(protocol, document, info, New Dictionary(Of String, String))
    End Sub

    Public Sub AddDocument(protocol As Protocol, document As DocumentInfo, info As ProtocolSignatureInfo, attributes As Dictionary(Of String, String))

        info.DocumentType = ProtocolDocumentType.Main

        Dim args As New ProtocolEventArgs(protocol)
        args.Tag = document
        OnBeforeAddDocument(args)
        If Not args.Cancel Then
            document.AddAttributes(attributes)
            document.Signature = GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, info)
            ' Aggiungo sempre su nuova catena: il protocollo ha solo un documento.
            Dim loc As UIDLocation = GetDocumentLocation(protocol)
            Dim documentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(loc.Server, loc.Archive, protocol.IdDocument.GetValueOrDefault(0))
            Dim idDocument As Guid = documentInfo.DocumentId
            protocol.IdDocument = documentInfo.BiblosChainId
            If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled Then
                document.AddAttribute("documentId", documentInfo.DocumentId.ToString())
                document.AddAttribute("chainId", documentInfo.ChainId.ToString())
                document.AddAttribute("archiveName", loc.Archive)
            End If

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso document.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.LP, String.Format("Associato livello privacy {0} al documento {1} [{2}]", document.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), document.Name, idDocument))
            End If
            Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, String.Format("Documento (Add): {0}", document.Name))

            args = New ProtocolEventArgs(protocol)
            args.Tag = document
            OnAfterAddDocument(args)
        End If
    End Sub

    Public Function GetInvoiceAttributes(protocol As Protocol, contact As Contact) As Dictionary(Of String, String)
        Dim tor As New Dictionary(Of String, String)

        If Not contact Is Nothing Then
            If Not String.IsNullOrEmpty(contact.Description) Then

                If InStr(contact.Description, "|") = 0 Then
                    tor.Add("Denominazione", contact.Description)
                Else
                    Dim vNome() As String = contact.Description.Split("|"c)
                    tor.Add("Cognome", vNome(0))
                    tor.Add("Nome", vNome(1))
                End If

            End If

            If Not String.IsNullOrEmpty(contact.FiscalCode) Then

                Dim vFiscalCode As String = contact.FiscalCode.Trim()
                Select Case DocSuiteContext.Current.ProtocolEnv.CorporateAcronym
                    Case "Itw"
                        tor.Add("PIVA", vFiscalCode)
                        tor.Add("CFIS", vFiscalCode)
                        Exit Select
                    Case Else
                        If vFiscalCode.Length = 11 Then
                            tor.Add("PartitaIVA", vFiscalCode)
                        Else
                            tor.Add("CodiceFiscale", vFiscalCode)
                        End If
                        Exit Select
                End Select
            End If
        End If

        If Not String.IsNullOrEmpty(protocol.InvoiceNumber) Then

            Dim vInvoiceNumber As String = protocol.InvoiceNumber.ToString.Trim()
            vInvoiceNumber = vInvoiceNumber.PadLeft(7, "0"c)
            tor.Add("NumeroFattura", vInvoiceNumber)
        End If

        tor.Add("DataFattura", If(protocol.InvoiceDate.HasValue, protocol.InvoiceDate.Value.ToString("dd/MM/yyyy"), String.Empty))

        Dim extensionsByRI As IList(Of ContainerExtension) = New ContainerExtensionFacade("ProtDB").GetByContainerAndKey(protocol.Container.Id, ContainerExtensionType.RI)
        Dim registroIva As String = extensionsByRI.Select(Function(s) s.KeyValue).SingleOrDefault()
        tor.Add("RegistroIVA", registroIva)

        If protocol.AccountingSectionalNumber.HasValue Then
            tor.Add("SezionaleNumerico", protocol.AccountingSectionalNumber.ToString())
        End If

        tor.Add("AnnoIVA", If(protocol.AccountingYear.HasValue, protocol.AccountingYear.ToString(), String.Empty))
        If protocol.AccountingDate.HasValue Then
            tor.Add("DataIVA", protocol.AccountingDate.Value.ToString("dd/MM/yyyy"))
        End If

        If protocol.AccountingNumber.HasValue Then
            Dim vProtocolloIva As String = protocol.AccountingNumber.ToString.Trim
            vProtocolloIva = vProtocolloIva.PadLeft(7, "0"c)
            tor.Add("ProtocolloIVA", vProtocolloIva)
        End If

        Return tor
    End Function

    ''' <summary> Aggiorna l'idBiblos del documento di un protocollo. </summary>
    ''' <param name="protocol">Protocollo da aggiornare</param>
    ''' <param name="idDocument">Nuovo idBiblos</param>
    ''' <param name="idStatus">Nuovo Stato</param>
    ''' <param name="fileName">Nuovo filename</param>
    Public Sub UpdateDocument(ByRef protocol As Protocol, ByVal idDocument As Integer, ByVal idStatus As Short, Optional ByVal fileName As String = "")
        protocol.IdDocument = idDocument
        protocol.IdStatus = idStatus
        If Not String.IsNullOrEmpty(fileName) Then
            protocol.DocumentCode = fileName
        End If
        Me.UpdateOnly(protocol)
    End Sub

    ''' <summary> Aggiorna l'idBiblos degli allegati di un protocollo. </summary>
    ''' <param name="protocol">Protocollo da aggiornare</param>
    ''' <param name="idAttachments">Nuovo idBiblos</param>
    Public Sub UpdateAttachments(ByRef protocol As Protocol, ByVal idAttachments As Integer)
        protocol.IdAttachments = idAttachments
        UpdateOnly(protocol)
    End Sub

    Public Function GetAttachmentLocation(protocol As Protocol) As UIDLocation
        Dim archiveName As String = protocol.Location.ProtBiblosDSDB
        If DocSuiteContext.Current.ProtocolEnv.IsProtocolAttachLocationEnabled AndAlso protocol.AttachLocation IsNot Nothing Then
            archiveName = protocol.AttachLocation.ProtBiblosDSDB
        End If
        Return New UIDLocation() With {.Archive = archiveName, .Server = protocol.Location.DocumentServer}
    End Function

    Public Function GetDocumentLocation(protocol As Protocol) As UIDLocation
        Dim location As New UIDLocation() With {.Archive = protocol.Location.ProtBiblosDSDB, .Server = protocol.Location.DocumentServer}
        Return location
    End Function

    Public Overloads Sub AddAttachments(ByRef prot As Protocol, ByRef attachments As IList(Of DocumentInfo))
        AddAttachments(prot, attachments, New ProtocolSignatureInfo())
    End Sub
    Public Overloads Sub AddAttachments(ByRef prot As Protocol, ByRef attachments As IList(Of DocumentInfo), info As ProtocolSignatureInfo)

        If (attachments Is Nothing) OrElse attachments.Count <= 0 Then
            Exit Sub
        End If

        Dim args As New ProtocolEventArgs(prot)
        args.Tag = attachments
        OnBeforeAddAttachments(args)

        If args.Cancel Then
            Exit Sub
        End If

        info.DocumentType = ProtocolDocumentType.Attachment
        info.DocumentNumber = 0

        For Each attachment As DocumentInfo In attachments

            info.DocumentNumber = info.DocumentNumber + 1
            attachment.Signature = GenerateSignature(prot, prot.RegistrationDate.ToLocalTime().DateTime, info)

            Dim loc As UIDLocation = GetAttachmentLocation(prot)
            Dim documentInfo As BiblosDocumentInfo = attachment.ArchiveInBiblos(loc.Server, loc.Archive, prot.IdAttachments.GetValueOrDefault(0))
            Dim idDocument As Guid = documentInfo.DocumentId
            If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled Then
                attachment.AddAttribute("documentId", documentInfo.DocumentId.ToString())
                attachment.AddAttribute("chainId", documentInfo.ChainId.ToString())
                attachment.AddAttribute("archiveName", loc.Archive)
            End If
            prot.IdAttachments = documentInfo.BiblosChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso attachment.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                Factory.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.LP, String.Format("Associato livello privacy {0} all'allegato {1} [{2}]", attachment.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), attachment.Name, idDocument))
            End If

            Factory.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, String.Format("Allegati (Add): {0}", attachment.Name))
        Next

        args = New ProtocolEventArgs(prot)
        args.Tag = attachments
        OnAfterAddAttachments(args)
    End Sub

    Public Overloads Sub AddAnnexes(ByVal protocol As Protocol, ByVal stream As Byte(), ByVal documentName As String, ByVal username As String)
        Dim annexes As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        annexes.Add(New MemoryDocumentInfo(stream, documentName))
        AddAnnexes(protocol, annexes)
    End Sub

    Public Overloads Sub AddAnnexes(ByRef protocol As Protocol, ByRef annexes As IList(Of DocumentInfo))
        AddAnnexes(protocol, annexes, New ProtocolSignatureInfo())
    End Sub

    ''' <summary> Permette di inserire in biblos allegati del tipo annexed. </summary>
    ''' <param name="prot">Protocollo nel quale si vogliono inserire gli annexed</param>
    ''' <param name="annexes">list di annexed incapsulati nel oggetto DocumentInfo</param>
    Public Overloads Sub AddAnnexes(ByRef prot As Protocol, ByRef annexes As IList(Of DocumentInfo), info As ProtocolSignatureInfo)
        If (annexes Is Nothing) OrElse annexes.Count <= 0 Then
            Exit Sub
        End If

        Dim args As New ProtocolEventArgs(prot)
        args.Tag = annexes
        OnBeforeAddAnnexes(args)

        If args.Cancel Then
            Exit Sub
        End If

        info.DocumentType = ProtocolDocumentType.Annexed
        info.DocumentNumber = 0

        For Each annexed As DocumentInfo In annexes
            annexed.Signature = GenerateSignature(prot, prot.RegistrationDate.ToLocalTime().DateTime, info)
            Dim loc As UIDLocation = GetAttachmentLocation(prot)
            prot.IdAnnexed = annexed.ArchiveInBiblos(loc.Server, loc.Archive, prot.IdAnnexed).ChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso annexed.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                Factory.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.LP, String.Format("Associato livello privacy {0} all'annesso {1} [{2}]", annexed.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), annexed.Name, prot.IdAnnexed))
            End If

            Factory.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, String.Format("Annesso (Add): {0}", annexed.Name))
        Next

        args = New ProtocolEventArgs(prot)
        args.Tag = annexes
        OnAfterAddAttachments(args)
    End Sub

    Public Function GetDocumentAttributes(protocol As Protocol) As Dictionary(Of String, String)
        Dim invoice As Boolean = DocSuiteContext.Current.ProtocolEnv.IsInvoiceEnabled AndAlso Not String.IsNullOrEmpty(protocol.InvoiceNumber)
        Return GetDocumentAttributes(protocol, invoice)
    End Function

    Private Function GetInvoiceContact(protocol As Protocol) As Contact
        If Not DocSuiteContext.Current.ProtocolEnv.IsInvoiceEnabled Then
            Return Nothing
        End If

        Dim senders As New List(Of Contact)
        Dim recipients As New List(Of Contact)
        FileLogger.Debug(LoggerName, String.Concat("GetInvoiceContact.Contacts ->", protocol.Contacts.IsNullOrEmpty()))
        FileLogger.Debug(LoggerName, String.Concat("GetInvoiceContact.ManualContacts ->", protocol.ManualContacts.IsNullOrEmpty()))
        If Not protocol.Contacts.IsNullOrEmpty() Then
            Dim protocolSenders As List(Of ProtocolContact) = protocol.Contacts.Where(Function(r) r.ComunicationType.Eq(ProtocolContactCommunicationType.Sender)).ToList()
            senders.AddRange(protocolSenders.Select(Function(s) s.Contact))

            Dim protocolRecipients As List(Of ProtocolContact) = protocol.Contacts.Where(Function(r) r.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)).ToList()
            recipients.AddRange(protocolRecipients.Select(Function(s) s.Contact))
        End If
        If Not protocol.ManualContacts.IsNullOrEmpty() Then
            Dim protocolManualSenders As List(Of ProtocolContactManual) = protocol.ManualContacts.Where(Function(m) m.ComunicationType.Eq(ProtocolContactCommunicationType.Sender)).ToList()
            senders.AddRange(protocolManualSenders.Select(Function(s) s.Contact))

            Dim protocolManualRecipients As List(Of ProtocolContactManual) = protocol.ManualContacts.Where(Function(m) m.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)).ToList()
            recipients.AddRange(protocolManualRecipients.Select(Function(s) s.Contact))
        End If

        Select Case protocol.Type.Id
            Case -1
                Return senders.FirstOrDefault()
            Case 1
                Return recipients.FirstOrDefault()
        End Select

        Return Nothing
    End Function

    Public Function GetDocumentAttributes(protocol As Protocol, invoice As Boolean) As Dictionary(Of String, String)
        Dim invoiceContact As Contact = Nothing
        If invoice Then
            invoiceContact = Me.GetInvoiceContact(protocol)
        End If

        If invoiceContact Is Nothing Then
            Return New Dictionary(Of String, String)
        End If

        Return Me.GetInvoiceAttributes(protocol, invoiceContact)
    End Function

    Public Overrides Sub Save(ByRef protocol As Protocol)
        Dim args As New ProtocolEventArgs(protocol)
        OnBeforeSave(args)
        If Not args.Cancel Then
            'codifica oggetto
            protocol.ProtocolObject = StringHelper.ReplaceCrLf(protocol.ProtocolObject)
            'Salvataggio protcollo
            MyBase.Save(protocol)

            OnAfterSave(New ProtocolEventArgs(protocol))
        End If
    End Sub

    Private Function ProtocolNeedAuditable(ByRef protocol As Protocol) As Boolean
        Dim needAuditable As Boolean = True
        Dim updateModeType As ProtocolUpdateModeType = ProtocolUpdateModeType.UpdateAuditable

        If Not ProtocolUpdateModeType.IsDefined(GetType(ProtocolUpdateModeType), DocSuiteContext.Current.ProtocolEnv.LastUserProtocolUpdateMode) Then
            updateModeType = DirectCast(DocSuiteContext.Current.ProtocolEnv.LastUserProtocolUpdateMode, ProtocolUpdateModeType)
        End If

        If (updateModeType = ProtocolUpdateModeType.UpdateAuditableIfOnlyContainer) Then
            needAuditable = FacadeFactory.Instance.ContainerFacade.CheckContainerRight(protocol.Container.Id, DSWEnvironment.Protocol, Nothing, True)
        End If
        Return needAuditable

    End Function

    Public Overrides Sub UpdateWithoutTransaction(ByRef protocol As Protocol)
        Dim args As New ProtocolEventArgs(protocol)
        OnBeforeUpdate(args)
        Dim needAuditable As Boolean = True

        If Not args.Cancel Then
            MyBase.UpdateNeedAuditable(protocol, _dbName, needAuditable:=ProtocolNeedAuditable(protocol), needTransaction:=False)
            OnAfterUpdate(New ProtocolEventArgs(protocol))
        End If
    End Sub

    Public Overrides Sub Update(ByRef protocol As Protocol)
        Dim args As New ProtocolEventArgs(protocol)
        OnBeforeUpdate(args)
        Dim needAuditable As Boolean = True

        If Not args.Cancel Then
            MyBase.UpdateNeedAuditable(protocol, ProtocolNeedAuditable(protocol))
            OnAfterUpdate(New ProtocolEventArgs(protocol))
        End If
    End Sub


    Public Sub Activation(protocol As Protocol)

        Dim args As New ProtocolEventArgs(protocol)
        OnBeforeActivation(args)
        If Not args.Cancel Then
            protocol.IdStatus = ProtocolStatusId.Attivo
            UpdateNoLastChange(protocol)
            Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PF, "Impostazione stato attivo")

            OnAfterActivation(New ProtocolEventArgs(protocol))
        End If

    End Sub

    ''' <summary>
    ''' Recupera un protocollo dato l'anno e il numero
    ''' </summary>
    ''' <param name="year">Anno di protocollazione</param>
    ''' <param name="number">Numero del protocollo</param>
    ''' <param name="shoudLock">Default false, indica se fare il lock dell'elemento</param>
    Public Overloads Function GetById(ByVal year As Short, ByVal number As Integer, Optional ByVal shoudLock As Boolean = False) As Protocol
        Dim id As YearNumberCompositeKey = New YearNumberCompositeKey(year, number)
        Return GetById(id, shoudLock)
    End Function

#Region "ProtocolLink: Methods for create/remove Links to a protocol"

    Public Overloads Sub RemoveProtocolLink(ByVal protocolFather As Protocol, ByVal protocolChild As Protocol)
        Dim dao As New NHibernateProtocolLinkDao(_dbName)
        Dim link As ProtocolLink = New ProtocolLink()
        Dim id As ProtocolLinkCompositeKey = New ProtocolLinkCompositeKey()
        id.Year = protocolFather.Id.Year.Value
        id.Number = protocolFather.Id.Number.Value
        id.NumberSon = protocolChild.Id.Number.Value
        id.YearSon = protocolChild.Id.Year.Value
        dao.ConnectionName = _dbName
        link = dao.GetById(id, False)
        dao.Delete(link)
    End Sub

    ''' <summary>
    ''' Collega il protocollo padre al protocollo figlio
    ''' </summary>
    ''' <param name="protocolFather">Protocollo padre</param>
    ''' <param name="protocolChild">Protocollo figlio</param>
    ''' <param name="linkType">Tipo del collegamento</param>
    Public Overloads Sub AddProtocolLink(ByVal protocolFather As Protocol, ByVal protocolChild As Protocol,
                                         ByVal linkType As ProtocolLinkType, lambda As Action(Of Guid, Guid))
        Dim dao As New NHibernateProtocolLinkDao(_dbName)
        Dim link As ProtocolLink = New ProtocolLink()
        Dim id As ProtocolLinkCompositeKey = New ProtocolLinkCompositeKey()
        id.Year = protocolFather.Id.Year.Value
        id.Number = protocolFather.Id.Number.Value
        id.NumberSon = protocolChild.Id.Number.Value
        id.YearSon = protocolChild.Id.Year.Value
        link.Id = id
        link.LinkType = linkType
        link.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        link.RegistrationDate = DateTimeOffset.UtcNow
        link.Protocol = protocolFather
        link.ProtocolLinked = protocolChild
        link.UniqueIdProtocolParent = protocolFather.UniqueId
        link.UniqueIdProtocolSon = protocolChild.UniqueId
        dao.ConnectionName = _dbName
        dao.Save(link)
        If lambda IsNot Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.FascicleEnabled Then
            lambda(protocolFather.UniqueId, protocolChild.UniqueId)
        End If
    End Sub

#End Region

#Region "ContactIssue: Methods for create/remove ContactsIssue to a Protocol"

    Public Sub AddProtocolContactIssue(ByRef protocol As Protocol, ByVal contact As Contact)
        Dim dao As New NHibernateProtocolContactIssueDao(_dbName)
        Dim pc As New ProtocolContactIssue()
        pc.Contact = contact
        pc.Incremental = protocol.ContactIssues.Count + 1
        pc.Protocol = protocol
        dao.ConnectionName = _dbName
        dao.Save(pc)
        protocol.ContactIssues.Add(pc)
    End Sub

    Public Sub RemoveProtocolContactIssue(ByRef protocol As Protocol)
        Dim dao As New NHibernateProtocolContactIssueDao(_dbName)
        dao.ConnectionName = _dbName
        If protocol.ContactIssues IsNot Nothing Then
            For Each pc As ProtocolContactIssue In protocol.ContactIssues
                dao.Delete(pc)
            Next
        End If
    End Sub

#End Region

#Region "Invoices: Methods for manage Invoices"

    Public Sub AddProtocolInvoice(ByRef protocol As Protocol, ByVal invoiceNumber As String,
              ByVal invoiceDate As Nullable(Of Date), ByVal accountingSectional As String,
              ByVal accountingYear As Short?, ByVal accountingDate As Nullable(Of Date),
              ByVal accountingNumber As Integer?, ByVal accountingSectionalNumber As Integer?)

        protocol.InvoiceNumber = invoiceNumber
        'protocol.InvoiceNumber = If(invoiceNumber = "", Nothing, Integer.Parse(invoiceNumber))
        protocol.InvoiceDate = invoiceDate
        protocol.AccountingDate = accountingDate
        protocol.AccountingYear = accountingYear
        protocol.AccountingSectional = If(String.IsNullOrEmpty(accountingSectional), Nothing, accountingSectional)
        protocol.AccountingSectionalNumber = accountingSectionalNumber
        protocol.AccountingNumber = accountingNumber
    End Sub

#End Region

#Region "Package: Methods for manage Packages"
    Public Sub UpdateProtocolPackage(ByRef protocol As Protocol, ByVal package As String,
                ByVal lot As String, ByVal incremental As String, ByVal origin As Char)

        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try
            'aggiornamento campi AdvancedProtocol
            Dim iPackage As Integer? = If(String.IsNullOrEmpty(package), Nothing, Integer.Parse(package))
            Dim iIncremental As Integer? = If(String.IsNullOrEmpty(incremental), Nothing, Integer.Parse(incremental))

            protocol.Package = iPackage
            protocol.PackageLot = If(String.IsNullOrEmpty(lot), Nothing, Integer.Parse(lot))
            protocol.PackageIncremental = iIncremental
            protocol.PackageOrigin = origin
            _dao.UpdateWithoutTransaction(protocol)

            'aggiornamento TablePackage
            Factory.PackageFacade.UpdatePackage(origin, iPackage.Value, iIncremental.Value)

            'commit
            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            Throw New DocSuiteException("Inserimento in Scatolone", ex) With {.Descrizione = "Errore in fase di inserimento protocollo in Scatolone", .User = DocSuiteContext.Current.User.FullUserName}
        End Try
    End Sub
#End Region

#Region "Duplicate a Protocol"

    ''' <summary>
    ''' Duplica il protocollo indicato.
    ''' </summary>
    ''' <param name="pYear">Anno di protocollazione</param>
    ''' <param name="pNumber">Numero di protocollazione</param>
    ''' <param name="pDuplicateContainer">Indica se duplicare contenitore e locazione</param>
    ''' <param name="pDuplicateSender">Indica se duplicare i mittenti</param>
    ''' <param name="pDuplicateReceipt">Indica se duplicare i destinatari</param>
    ''' <param name="pDuplicateObject">Indica se duplicare l'oggetto</param>
    ''' <param name="pDuplicateClassification">Indica se duplicare il classificatore</param>
    ''' <param name="pDuplicateOther">Indica se duplicare Note, Assegnatario/Proponente, Categoria di servizio</param>
    ''' <param name="pDuplicateDocType">Indica se duplicare il tipo di documento</param>
    ''' <param name="pDuplicateRoles">Indica se duplicare i settori</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Duplicate(pYear As Short, pNumber As Integer, ByRef hasDisabledElements As Boolean,
               Optional ByVal pDuplicateContainer As Boolean = False,
               Optional ByVal pDuplicateSender As Boolean = False,
               Optional ByVal pDuplicateReceipt As Boolean = False,
               Optional ByVal pDuplicateObject As Boolean = False,
               Optional ByVal pDuplicateClassification As Boolean = False,
               Optional ByVal pDuplicateOther As Boolean = False,
               Optional ByVal pDuplicateDocType As Boolean = False,
               Optional ByVal pDuplicateRoles As Boolean = False
               ) As Protocol

        hasDisabledElements = False
        Dim vProtocol As Protocol = GetById(pYear, pNumber, False)
        Dim vDuplicate As Protocol = New Protocol()

        'assegno temporaneamente Year e Number al protocollo
        vDuplicate.Year = vProtocol.Year
        vDuplicate.Number = vProtocol.Number

        'Contenitore
        If pDuplicateContainer Then
            vDuplicate.Container = vProtocol.Container
            vDuplicate.Location = vProtocol.Container.ProtLocation
        End If

        'Mittenti
        If pDuplicateSender Then
            For Each pc As ProtocolContact In vProtocol.Contacts
                If pc.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) AndAlso pc.Contact.IsActive = 1 Then
                    vDuplicate.Contacts.Add(pc)
                End If
            Next

            For Each pcm As ProtocolContactManual In vProtocol.ManualContacts
                If pcm.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    vDuplicate.ManualContacts.Add(pcm)
                End If
            Next

            If Not hasDisabledElements AndAlso vProtocol.Contacts.Where(Function(pc) pc.Contact.IsActive = 0).Any() Then
                hasDisabledElements = True
            End If

        End If
        'Destinatari
        If pDuplicateReceipt Then
            For Each pc As ProtocolContact In vProtocol.Contacts
                If pc.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) AndAlso pc.Contact.IsActive = 1 Then
                    vDuplicate.Contacts.Add(pc)
                End If
            Next

            For Each pcm As ProtocolContactManual In vProtocol.ManualContacts
                If pcm.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                    vDuplicate.ManualContacts.Add(pcm)
                End If
            Next
        End If

        'Oggetto
        If pDuplicateObject Then
            vDuplicate.ProtocolObject = vProtocol.ProtocolObject
        End If

        'Classificatore
        If pDuplicateClassification AndAlso vProtocol.Category.IsActive = 1 Then
            vDuplicate.Category = vProtocol.Category
        End If
        If Not hasDisabledElements AndAlso pDuplicateClassification AndAlso vProtocol.Category.IsActive = 0 Then
            hasDisabledElements = True
        End If

        'Altre (Note, Assegnatario/Proponente, Categoria di servizio)
        If pDuplicateOther Then
            vDuplicate.Note = vProtocol.Note
            vDuplicate.Subject = vProtocol.Subject
            vDuplicate.ServiceCategory = vProtocol.ServiceCategory
        End If

        'Tipo(Documento)
        If pDuplicateDocType Then
            vDuplicate.DocumentType = vProtocol.DocumentType
        End If

        'Settori espliciti
        If pDuplicateRoles Then
            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                vDuplicate.Roles = vProtocol.Roles _
                    .Where(Function(r) Not String.IsNullOrWhiteSpace(r.DistributionType) _
                               AndAlso r.DistributionType.Equals(ProtocolDistributionType.Explicit) AndAlso r.Role IsNot Nothing AndAlso r.Role.IsActive = 1) _
                    .ToList()
            Else
                vDuplicate.Roles = vProtocol.Roles _
                    .Where(Function(pr) pr.Role IsNot Nothing AndAlso pr.Role.IsActive = 1).ToList()
            End If

            If Not hasDisabledElements AndAlso vProtocol.Roles.Where(Function(r) r.Role IsNot Nothing AndAlso r.Role.IsActive = 0).Any() Then
                hasDisabledElements = True
            End If
        End If

        'Tipo Protocollo
        vDuplicate.Type = vProtocol.Type

        'elimino Year e Number dal protocollo duplicato perchè verranno riassegnati successivamente
        vDuplicate.Year = Nothing
        vDuplicate.Number = Nothing

        Return vDuplicate
    End Function

#End Region

    Public Function GetProtocolNumber(ByVal pYear As Short, ByVal pNumber As Integer) As String
        Return pYear & "/" & pNumber.ToString().PadLeft(7, "0"c)
    End Function

    Public Function CreateProtocol() As Protocol
        Dim args As New ProtocolEventArgs()
        OnBeforeProtocolCreate(args)

        If Not args.Cancel Then
            Dim vParameter As Parameter
            Dim vProtocol As New Protocol

            vParameter = New Parameter

            Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB")
            Dim sqlQuery As IQuery = session.CreateSQLQuery("select * from Parameter with (xlock, rowlock) where incremental=1").AddEntity(vParameter.GetType).SetMaxResults(1)
            Dim tx As ITransaction = session.BeginTransaction(IsolationLevel.Serializable)
            Try
                vParameter = CType(sqlQuery.UniqueResult, Parameter)
                session.Refresh(vParameter) ' Forzo un refresh del dato da SQL
                vProtocol.Year = vParameter.LastUsedYear
                vProtocol.Number = vParameter.LastUsedNumber
                vParameter.LastUsedNumber += 1
                session.Update(vParameter)
                tx.Commit()
            Catch ex As Exception
                tx.Rollback()
                Throw
            Finally
                session.Flush()
            End Try

            args = New ProtocolEventArgs()
            args.Protocol = vProtocol
            OnAfterProtocolCreate(args)

            Return vProtocol
        Else
            Return args.Protocol
        End If

    End Function

    Public Function IsUsedDocType(ByVal DocType As DocumentType) As IList(Of Protocol)
        Return _dao.IsUsedDocType(DocType)
    End Function

    Public Function FinderProtocolByMetadati(year As Short, accountingSectional As String, container As String, vatRegistrationNumber As Integer) As Protocol
        Try
            Return _dao.FinderProtocolByMetadati(year, accountingSectional, container, vatRegistrationNumber)
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("Protocol not found", ex.Message), ex)
            Return Nothing
        End Try
    End Function
    Public Function GetByUniqueId(uniqueId As Guid) As Protocol
        Return _dao.GetByUniqueId(uniqueId)
    End Function

    Public Overloads Function SecurityGroupsUserRight(ByVal protocol As Protocol, ByVal securityGroups As IList(Of SecurityGroups), ByVal rightsToCheck As Integer, ByVal prevRights As Integer, ByVal viewRights As Integer, userName As String) As Boolean
        Return SecurityGroupsUserRight(protocol, securityGroups, rightsToCheck, prevRights, viewRights, False, userName)
    End Function

    Public Overloads Function SecurityGroupsUserRight(ByVal protocol As Protocol, ByVal securityGroups As IList(Of SecurityGroups), ByVal rightsToCheck As Integer, ByVal prevRights As Integer, ByVal viewRights As Integer, ByVal statusCancel As Boolean, userName As String) As Boolean
        Return SecurityGroupsUserRight(protocol, securityGroups, rightsToCheck, prevRights, viewRights, statusCancel, False, True, userName)
    End Function

    Public Overloads Function SecurityGroupsUserRight(ByVal protocol As Protocol, ByVal securityGroups As IList(Of SecurityGroups), ByVal rightsToCheck As Integer, ByVal prevRights As Integer, ByVal viewRights As Integer, ByVal statusCancel As Boolean, ByVal statusError As Boolean, ByVal checkContainers As Boolean, userName As String) As Boolean
        Dim checkRights As Boolean = protocol.IdStatus.Value >= ProtocolStatusId.Attivo OrElse protocol.IdStatus.Value = ProtocolStatusId.Incompleto OrElse protocol.IdStatus.Value = ProtocolStatusId.Rejected
        If statusCancel Then
            checkRights = checkRights OrElse protocol.IdStatus.Value = ProtocolStatusId.Annullato
        End If
        If statusError Then
            checkRights = checkRights OrElse protocol.IdStatus.Value = ProtocolStatusId.Errato
        End If

        If checkContainers AndAlso checkRights AndAlso protocol.Container.ContainerGroups.
                Where(Function(f) f.SecurityGroup IsNot Nothing).
                Any(Function(f) f.ProtocolRightsString.Substring(rightsToCheck - 1, 1).Eq("1") AndAlso securityGroups.Any(Function(x) x.Id = f.SecurityGroup.Id)) Then
            Return True
        End If

        If Not rightsToCheck.Equals(prevRights) AndAlso Not rightsToCheck.Equals(viewRights) Then
            Return False
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            Return SecurityGroupsUserRole(protocol, securityGroups, "1")
        End If

        Dim temp As Boolean = SecurityGroupsUserRole(protocol, securityGroups, "11")
        If Not temp Then
            temp = SecurityGroupsUserAuthoriz(protocol, securityGroups, userName)
        End If
        Return temp
    End Function

    Public Function SecurityGroupsUserRole(ByVal protocol As Protocol, ByVal securityGroups As IList(Of SecurityGroups), ByVal rights As String) As Boolean
        For Each pr As ProtocolRole In protocol.Roles
            For Each sg As SecurityGroups In securityGroups
                For Each rg As RoleGroup In pr.Role.RoleGroups
                    If (DocSuiteContext.Current.ProtocolEnv.DisabledRolesRights OrElse pr.Role.IsActive = 1S) _
                        AndAlso rg.ProtocolRights.ToString().StartsWith(rights) AndAlso rg.SecurityGroup.Id.Equals(sg.Id) Then
                        Return True
                    End If
                Next
            Next
        Next
        Return False
    End Function

    Public Function SecurityGroupsUserAuthoriz(protocol As Protocol, securityGroups As IList(Of SecurityGroups), userName As String) As Boolean
        If protocol.RoleUsers Is Nothing Then
            Return False
        End If

        Dim evaluate As Boolean = protocol.RoleUsers.Any(Function(ru) ru.Account.Eq(userName) AndAlso ru.Role.RoleGroups.Any(Function(rg) securityGroups.Any(Function(sg) sg.Id = rg.SecurityGroup.Id)))

        Return evaluate
    End Function


    Public Function CheckProtocolRoleCC(ByVal protocol As Protocol, ByVal userGroups As String(), ByVal cc As Boolean) As Boolean
        ' Verifico che il protocollo corrente sia abbinato ad un settore del quale ho determinati permessi.
        For Each pr As ProtocolRole In protocol.Roles
            For Each group As String In userGroups
                For Each rg As RoleGroup In pr.Role.RoleGroups
                    If Not DocSuiteContext.Current.ProtocolEnv.DisabledRolesRights AndAlso pr.Role.IsActive <> 1S OrElse Not rg.Name.Eq(group) Then
                        Continue For
                    End If

                    If cc Then
                        If pr.Type IsNot Nothing AndAlso pr.Type.Eq(ProtocolRoleTypes.CarbonCopy) Then
                            Return True
                        End If
                    Else
                        If String.IsNullOrEmpty(pr.Type) OrElse Not pr.Type.Eq(ProtocolRoleTypes.CarbonCopy) Then
                            Return True
                        End If
                    End If

                Next
            Next
        Next

        Return False
    End Function

    Public Sub UpdateProtocolsForContainerOrCategory(ByVal idnewcontainer As Integer, ByVal idnewcategory As String, ByVal listprot As IList(Of Protocol), ByVal Action As String)
        Dim container As Container
        Dim category As Category
        Dim transazione As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try

            Select Case Action
                Case "Container"
                    container = Factory.ContainerFacade.GetById(idnewcontainer, False, "ProtDB")
                    If idnewcontainer <> 0 Then
                        For Each protocol As Protocol In listprot
                            protocol.Container = container
                            UpdateWithoutTransaction(protocol)
                        Next
                    End If

                Case "Category"
                    If Not String.IsNullOrEmpty(idnewcategory) Then
                        category = Factory.CategoryFacade.GetById(Integer.Parse(idnewcategory))
                        For Each protocol As Protocol In listprot
                            protocol.Category = category
                            UpdateWithoutTransaction(protocol)

                        Next
                    End If
            End Select

            transazione.Commit()

        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            transazione.Rollback()
        End Try
    End Sub

    Public Sub UpdateProtocolRoles(ByVal roles As IList(Of Role), ByVal protocols As IList(Of Protocol))
        If protocols.IsNullOrEmpty() Then
            Exit Sub
        End If

        Dim transazione As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try
            For Each protocol As Protocol In protocols
                protocol.Roles.Clear()
                Factory.ProtocolFacade.UpdateWithoutTransaction(protocol)
                Dim userName As String = DocSuiteContext.Current.User.FullUserName
                For Each ruolo As Role In roles
                    protocol.AddRole(ruolo, userName, DateTimeOffset.UtcNow)
                    protocol.LastChangedUser = userName
                    protocol.LastChangedDate = Date.Now

                    If Not DocSuiteContext.Current.ProtocolEnv.IsLogEnabled Then
                        Continue For
                    End If

                    Dim protocollog As New ProtocolLog
                    With protocollog
                        .Id = Factory.ProtocolLogFacade.GetMaxId()
                        .Year = protocol.Id.Year.Value
                        .Number = protocol.Id.Number.Value
                        .LogType = ProtocolLogEvent.PZ.ToString()
                        .LogDescription = String.Format("Autorizzazioni:(Add) {0} {1}.", ruolo.Id, ruolo.Name)
                        .SystemComputer = DocSuiteContext.Current.UserComputer
                        .SystemUser = DocSuiteContext.Current.User.FullUserName
                        .Program = DocSuiteContext.Program
                        .LogDate = Date.Now
                        .UniqueIdProtocol = protocol.UniqueId
                    End With
                    Factory.ProtocolLogFacade.SaveWithoutTransaction(protocollog)

                Next
            Next
            transazione.Commit()

        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            transazione.Rollback()
        End Try
    End Sub

    Public Function GetFirstProtocolSuspended(year As Short, from As DateTime, [to] As DateTime) As Protocol
        Return _dao.GetFirstProtocolSuspended(year, from, [to])
    End Function

    Public Function HasProtSuspended() As Boolean
        Return _dao.HasProtSuspended()
    End Function

    Public Function CountProtSuspended(year As Short, from As DateTime, [to] As DateTime) As Integer
        Return _dao.CountProtSuspended(year, from, [to])
    End Function

    Public Overloads Function Suspend(ByVal suspendNumber As Integer, ByVal suspendDate As Date, suspendYear As Short?) As List(Of String)
        Dim currentParameter As Parameter = FacadeFactory.Instance.ParameterFacade.GetCurrentAndRefresh()
        If suspendYear.HasValue AndAlso currentParameter.LastUsedYear = suspendYear Then
            Return _dao.Suspend(suspendNumber, suspendDate, Nothing)
        End If
        Return _dao.Suspend(suspendNumber, suspendDate, suspendYear)
    End Function

    ''' <summary> Metodo che ritira protocolli e anagrafiche dei concorsi nel contenitore richiesto. </summary>
    ''' <param name="dateFrom">Data di inizio ricerca</param>
    ''' <param name="dateto">Data di fine ricerca</param>
    ''' <param name="categoryPath">Contenitore</param>
    ''' <remarks>Metodo per la stampa dei concorsi</remarks>
    Public Function GetProtocolForConcourse(ByVal dateFrom As Date?, ByVal dateto As Date?, ByVal categoryPath As String) As IList(Of Protocol)
        Return _dao.GetProtocolForConcourse(dateFrom, dateto, categoryPath)
    End Function

    Function GetLastProtocolInDuplicateMetadatas(ByVal documentCode As String, ByVal [date] As Date?, ByVal subject As String) As Protocol
        Return _dao.GetLastProtocolInDuplicateMetadatas(documentCode, [date], subject)
    End Function

    Public Sub UpdateProtocolObject(ByVal Year As Short, ByVal Number As Integer, ByVal oldObject As String, ByVal newObject As String)
        Dim prot As Protocol = GetById(Year, Number)

        UpdateProtocolObject(prot, oldObject, newObject)
    End Sub

    Public Sub UpdateProtocolObject(ByVal Year As Short, ByVal Number As Integer, ByVal oldObject As String, ByVal newObject As String, ByVal oldChangeReason As String, ByVal newChangeReason As String)
        Dim prot As Protocol = GetById(Year, Number)
        UpdateProtocolObject(prot, oldObject, newObject, oldChangeReason, newChangeReason)
    End Sub

    Public Sub UpdateProtocolObject(ByRef protocol As Protocol, ByVal oldObject As String, ByVal newObject As String, Optional ByVal oldChangeReason As String = "", Optional ByVal newChangeReason As String = "")
        protocol.ProtocolObject = newObject
        If Not String.IsNullOrEmpty(newChangeReason) Then
            protocol.ObjectChangeReason = newChangeReason
        End If
        Update(protocol)

        Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, $"Oggetto (old): {oldObject}")
        Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, $"Oggetto (new): {newObject}")
        If Not String.IsNullOrEmpty(newChangeReason) Then
            Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, $"Motivazione Cambio Oggetto (old): {oldChangeReason}")
            Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, $"Motivazione Cambio Oggetto (new): {newChangeReason}")
        End If
    End Sub

    Public Function SerachInvoiceAccountingDouble(ByVal idContainer As Integer, ByVal accountingSectional As String, ByVal accountinYear As Short, ByVal accountingNumber As Integer) As IList(Of Protocol.AdvancedProtocol)
        Dim advancedDao As NHibernateAdvancedProtocolDao = New NHibernateAdvancedProtocolDao(Me._dbName)

        Return advancedDao.SearchInvoiceAccountingDouble(idContainer, accountingSectional, accountinYear, accountingNumber)
    End Function

    ''' <summary>
    ''' Recupera una lista di protocolli a partire dalla lista delle loro YNCK secondo una precisa strategia di fetching.
    ''' </summary>
    ''' <param name="keys">Lista di YNCK.</param>
    Public Function GetProtocols(keys As IList(Of YearNumberCompositeKey), strategy As NHibernateProtocolDao.FetchingStrategy) As IList(Of Protocol)
        Return _dao.GetProtocols(keys, strategy)
    End Function

    ''' <summary>
    ''' Recupera una lista di protocolli a partire dalla lista delle loro YNCK secondo la strategia di fetching di default.
    ''' </summary>
    ''' <param name="keys">Lista di YNCK.</param>
    Public Function GetProtocols(ByVal keys As IList(Of YearNumberCompositeKey)) As IList(Of Protocol)
        Return GetProtocols(keys, NHibernateProtocolDao.FetchingStrategy.Common)
    End Function

    Public Function GetFirstContact(ByVal Year As Short, ByVal Number As Integer, ByVal Type As String, Optional ByVal Space As String = " ", Optional ByVal SearchCode As Boolean = False) As String
        Dim dto As ProtocolContactDTO = _dao.GetFirstContact(Year, Number, Type)

        If dto Is Nothing Then
            Return String.Empty
        End If

        If String.IsNullOrEmpty(dto.Type) Then
            Return String.Empty
        End If
        Dim sret As String = String.Empty
        If SearchCode Then
            If Not String.IsNullOrEmpty(dto.SearchCode) Then
                sret = String.Concat(dto.SearchCode.Trim(), Space)
            End If
        End If
        sret &= dto.Description.Replace("|", " ")
        Return sret
    End Function

    Public Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of Protocol)
        Return _dao.GetJournalPrint(idContainers, dateFrom, dateTo, idStatus)
    End Function

    Public Function GetLinkedDocumentCount(ByVal year As Short, ByVal number As Integer) As Integer
        Return _dao.GetLinkedDocumentCount(year, number)
    End Function

    Public Sub AddRoleAuthorization(protocol As Protocol, idRole As Integer, distributionType As String)
        Dim rolefacade As New RoleFacade("ProtDB")
        Dim role As Role = rolefacade.GetById(idRole)
        AddRoleAuthorization(protocol, role, distributionType)
    End Sub

    Public Sub RemoveRoleAuthorization(protocol As Protocol, idRole As Integer)
        Dim rolefacade As New RoleFacade("ProtDB")
        Dim role As Role = rolefacade.GetById(idRole)
        RemoveRoleAuthorization(protocol, role)
    End Sub

    ''' <summary> Aggiorna il tipo di autorizzazione ruolo/protocollo </summary>
    ''' <param name="protocol"> Protocollo di cui verificare le autorizzazioni </param>
    ''' <param name="rolesToUpdate"> Tutti i settori da verificare </param>
    ''' <param name="cc"> Indica se impostare o disattivare la copia conoscenza </param>
    Public Sub UpdateRoleAuthorization(protocol As Protocol, rolesToUpdate As IList(Of KeyValuePair(Of String, String)), cc As Boolean)
        If rolesToUpdate Is Nothing Then
            Exit Sub
        End If

        Dim proles As IList(Of ProtocolRole) = protocol.Roles.Where(Function(x) rolesToUpdate.Any(Function(xx) xx.Key.Eq(x.Id.Id.ToString()))).ToList()
        If Not proles.Any() Then
            Exit Sub
        End If

        For Each prole As ProtocolRole In proles
            If cc Then
                prole.Type = ProtocolRoleTypes.CarbonCopy
            Else
                prole.Type = Nothing
            End If
        Next
    End Sub

    Public Sub AddRoleAuthorizations(protocol As Protocol, roles As ICollection(Of Integer), distributionType As String)
        If Not roles Is Nothing Then
            For Each id As Integer In roles
                AddRoleAuthorization(protocol, id, distributionType)
            Next
        End If
    End Sub

    Public Sub AddRoleAuthorization(protocol As Protocol, role As Role, distributionType As String)
        ' Recupero i diritti sul protocollo
        Dim rights As New ProtocolRights(protocol)
        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            If Not rights.IsDistributable Then
                Throw New DocSuiteException("Aggiungi Autorizzazioni", "Operatore senza diritti di Distribuzione")
            End If
        Else
            If Not rights.IsAuthorizable Then
                Throw New DocSuiteException("Aggiungi Autorizzazioni", "Operatore senza diritti di Autorizzazione")
            End If
        End If

        ' Verifico se il settore è già presente
        Dim compareKey As YearNumberIdCompositeKey = New YearNumberIdCompositeKey()
        compareKey.Id = role.Id
        compareKey.Year = protocol.Year
        compareKey.Number = protocol.Number

        Dim proleExist As Boolean = protocol.Roles.Any(Function(x) x.Id.Equals(compareKey))

        If proleExist Then
            If distributionType.Eq("E") Then
                Dim proleToUpdate As ProtocolRole = protocol.Roles.FirstOrDefault(Function(x) x.Id.Equals(compareKey))
                proleToUpdate.DistributionType = distributionType
            End If
            ' TODO: segnare in LOG
        Else
            ' Aggiungere il settore al protocollo
            protocol.AddRole(role, DocSuiteContext.Current.User.FullUserName, DateTimeOffset.UtcNow, distributionType)
            ' Aggiungo riga in LOG
            Factory.ProtocolLogFacade.AddRoleAuthorization(protocol, role)

            ' TODO: verificare (che cosa sia) IsAuthorizFullEnabled

            ' TODO: riportare codice TopMedia per Torino (si, magari un giorno...)

            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not role.Father Is Nothing Then
                AddRoleAuthorization(protocol, role.Father, "I")
            End If
        End If
    End Sub

    Public Sub RemoveRoleAuthorization(protocol As Protocol, role As Role)
        If Not protocol.Contains(role) Then
            Exit Sub
        End If

        If protocol.RoleUsers IsNot Nothing Then
            Dim temp As New List(Of ProtocolRoleUser)(protocol.RoleUsers)
            For Each item As ProtocolRoleUser In temp
                If item.Role.FullIncrementalPath.StartsWith(role.FullIncrementalPath) Then
                    protocol.RoleUsers.Remove(item)
                End If
            Next
        End If
        protocol.RemoveRole(role)
        ' Aggiungo riga in LOG
        Factory.ProtocolLogFacade.DelRoleAuthorization(protocol, role)
    End Sub

    Public Sub RemoveRoleAuthorizations(protocol As Protocol, roles As ICollection(Of Integer))
        If Not roles Is Nothing Then
            For Each id As Integer In roles
                RemoveRoleAuthorization(protocol, id)
            Next
        End If
    End Sub

    Private Enum ProtocolRoleUserColumns
        IdRole = 0
        GroupName = 1
        UserName = 2
        Account = 3
    End Enum

    Public Sub AddRoleUserAuthorizations(protocol As Protocol, ids As IList(Of String))
        If ids Is Nothing Then
            Exit Sub
        End If

        For Each selectedId As String In ids
            Dim roleUserNodeValue As String() = selectedId.Split("|"c)
            Dim idRole As Integer = Integer.Parse(roleUserNodeValue(ProtocolRoleUserColumns.IdRole))
            AddRoleUserAuthorization(protocol, idRole, roleUserNodeValue(ProtocolRoleUserColumns.GroupName), roleUserNodeValue(ProtocolRoleUserColumns.UserName), roleUserNodeValue(ProtocolRoleUserColumns.Account))
        Next
    End Sub

    Public Sub AddRoleUserAuthorization(protocol As Protocol, idRole As Integer, groupName As String, userName As String, account As String)
        'Verifica se il settore è autorizzato
        If Not protocol.ContainsRole(idRole) Then
            Throw New DocSuiteException("Errore autorizzazione", String.Format("Errore per l'utente [{0}]: Settore [{1}] non autorizzato.", userName, idRole))
        End If

        Dim pruk As New ProtocolRoleUserKey
        pruk.Year = protocol.Year
        pruk.Number = protocol.Number
        pruk.IdRole = idRole
        pruk.GroupName = groupName
        pruk.UserName = userName

        If protocol.RoleUsers Is Nothing Then
            protocol.RoleUsers = New List(Of ProtocolRoleUser)
        End If
        ' Verifico se l'utente è già autorizzato.
        If Not protocol.Contains(pruk) Then
            Dim pru As New ProtocolRoleUser
            pru.Id = pruk
            pru.Account = account
            pru.IsActive = 1
            pru.UniqueIdProtocol = protocol.UniqueId
            pru.Role = Factory.RoleFacade.GetById(idRole)
            pru.Protocol = protocol

            protocol.RoleUsers.Add(pru)

            Factory.ProtocolLogFacade.AddRoleUserAuthorization(protocol, pru)
        End If

    End Sub

    Public Sub RemoveRoleUserAuthorizations(protocol As Protocol, ids As IList(Of String))
        If ids Is Nothing Then
            Exit Sub
        End If

        For Each selectedId As String In ids
            Dim roleUserNodeValue As String() = selectedId.Split("|"c)
            Dim idRole As Integer = Integer.Parse(roleUserNodeValue(ProtocolRoleUserColumns.IdRole))
            RemoveRoleUserAuthorization(protocol, idRole, roleUserNodeValue(ProtocolRoleUserColumns.GroupName), roleUserNodeValue(ProtocolRoleUserColumns.UserName), roleUserNodeValue(ProtocolRoleUserColumns.Account))
        Next
    End Sub

    Public Sub RemoveRoleUserAuthorization(protocol As Protocol, idRole As Integer, groupName As String, userName As String, account As String)
        Dim pruk As New ProtocolRoleUserKey
        pruk.Year = protocol.Year
        pruk.Number = protocol.Number
        pruk.IdRole = idRole
        pruk.GroupName = groupName
        pruk.UserName = userName

        If protocol.Contains(pruk) Then
            ' Cancello l'elemento solo se presente in elenco
            Dim pru As New ProtocolRoleUser
            pru.Id = pruk
            pru.Account = account
            pru.IsActive = 1

            protocol.RoleUsers.Remove(pru)
            ' log
            Factory.ProtocolLogFacade.DelRoleUserAuthorization(protocol, pru)
        End If
    End Sub

    ''' <summary>
    ''' Ritorna il Number massimo dei protocolli dato un year
    ''' </summary>
    Public Function GetMaxProtocolNumber(ByVal year As Short) As Integer
        Return _dao.GetMaxProtocolNumber(year)
    End Function

    ''' <summary> Recupera una lista di YNCK da una lista di ProtocolHeader. </summary>
    ''' <param name="headers">Lista di ProtocolHeader</param>
    Private Function GetYNCKByHeader(headers As IList(Of ProtocolHeader)) As IList(Of YearNumberCompositeKey)
        If headers IsNot Nothing AndAlso headers.Count > 0 Then
            Dim keys As New List(Of YearNumberCompositeKey)
            For Each header As ProtocolHeader In headers
                If Not keys.Contains(header.ProtocolCompositeKey) Then
                    keys.Add(header.ProtocolCompositeKey)
                End If
            Next
            Return keys
        End If
        Return Nothing
    End Function

    ''' <summary> Ritorna un dizionario di Contatti per i principali contatti di un protocollo. </summary>
    ''' <param name="headers">Lista di ProtocolHeader</param>
    Public Function GetProtocolContactDictionary(headers As IList(Of ProtocolHeader)) As IDictionary(Of YearNumberCompositeKey, Contact)
        Dim keys As IList(Of YearNumberCompositeKey) = GetYNCKByHeader(headers)
        Return GetProtocolContactDictionary(keys)
    End Function

    ''' <summary> Ritorna un dizionario di Contatti per i principali contatti di un protocollo. </summary>
    ''' <param name="keys">Lista di chiavi di Protocol</param>
    Public Function GetProtocolContactDictionary(keys As IList(Of YearNumberCompositeKey)) As IDictionary(Of YearNumberCompositeKey, Contact)
        If (keys Is Nothing) OrElse keys.Count <= 0 Then
            Return Nothing
        End If

        Dim protocolContacts As IDictionary(Of YearNumberCompositeKey, Object) = _dao.GetMainContacts(keys)
        If (protocolContacts Is Nothing) OrElse protocolContacts.Count <= 0 Then
            Return Nothing
        End If

        Dim contactLabels As New Dictionary(Of YearNumberCompositeKey, Contact)
        For Each item As KeyValuePair(Of YearNumberCompositeKey, Object) In protocolContacts
            If TypeName(item.Value).Eq("ProtocolContact") Then
                With DirectCast(item.Value, ProtocolContact)
                    contactLabels.Add(item.Key, .Contact)
                    Continue For
                End With
            ElseIf TypeName(item.Value).Eq("ProtocolContactManual") Then
                With DirectCast(item.Value, ProtocolContactManual)
                    contactLabels.Add(item.Key, .Contact)
                    Continue For
                End With
            End If
            Throw New InvalidCastException(String.Format("ProtocolFacade.GetMainContacts: tipo oggetto non valido per il protocollo {0}", item.Key.ToString()))
        Next
        Return contactLabels
    End Function

    ''' <summary> Ritorna un dizionario di etichette per i principali contatti di un protocollo. </summary>
    ''' <param name="headers">Lista di ProtocolHeader</param>
    Public Function GetProtocolContactLabelsDictionary(headers As IList(Of ProtocolHeader)) As IDictionary(Of YearNumberCompositeKey, String)
        Dim keys As IList(Of YearNumberCompositeKey) = GetYNCKByHeader(headers)
        Return GetProtocolContactLabelsDictionary(keys)
    End Function

    ''' <summary> Ritorna un dizionario di etichette per i principali contatti di un protocollo. </summary>
    ''' <param name="keys">Lista di chiavi di Protocol</param>
    Public Function GetProtocolContactLabelsDictionary(keys As IList(Of YearNumberCompositeKey)) As IDictionary(Of YearNumberCompositeKey, String)
        If (keys Is Nothing) OrElse keys.Count <= 0 Then
            Return Nothing
        End If

        Dim protocolContacts As IDictionary(Of YearNumberCompositeKey, Object) = _dao.GetMainContacts(keys)
        If (protocolContacts Is Nothing) OrElse protocolContacts.Count <= 0 Then
            Return Nothing
        End If

        Dim contactLabels As New Dictionary(Of YearNumberCompositeKey, String)
        For Each item As KeyValuePair(Of YearNumberCompositeKey, Object) In protocolContacts
            If TypeName(item.Value).Eq("ProtocolContact") Then
                With DirectCast(item.Value, ProtocolContact)
                    Dim label As String = .Contact.FullDescription
                    contactLabels.Add(item.Key, label)
                    Continue For
                End With
            ElseIf TypeName(item.Value).Eq("ProtocolContactManual") Then
                With DirectCast(item.Value, ProtocolContactManual)
                    Dim label As String = .Contact.Description
                    contactLabels.Add(item.Key, label)
                    Continue For
                End With
            End If
            Throw New InvalidCastException(String.Format("ProtocolFacade.GetMainContacts: tipo oggetto non valido per il protocollo {0}", item.Key.ToString()))
        Next
        Return contactLabels
    End Function

    ''' <summary> Ritorna un dizionario di ProtocolRights per i ProtocolHeaders specificati. </summary>
    ''' <param name="headers">Lista di ProtocolHeader</param>
    Public Function GetProtocolRightsDictionary(headers As IList(Of ProtocolHeader)) As IDictionary(Of YearNumberCompositeKey, ProtocolRights)
        Dim keys As IList(Of YearNumberCompositeKey) = GetYNCKByHeader(headers)
        Dim protocols As IList(Of Protocol) = GetProtocols(keys, NHibernateProtocolDao.FetchingStrategy.BasicDataAndPermissions)
        Dim retval As New Dictionary(Of YearNumberCompositeKey, ProtocolRights)
        Dim pr As ProtocolRights = Nothing
        For Each p As Protocol In protocols
            If Not retval.ContainsKey(p.Id) Then
                pr = New ProtocolRights(p)
                pr.ContainerRightDictionary = CommonShared.UserContainerRightDictionary
                pr.RoleRightDictionary = CommonShared.UserRoleRightDictionary
                retval.Add(p.Id, pr)
            End If
        Next
        Return retval
    End Function

    Public Function RecoverProtocol(key As YearNumberCompositeKey) As Protocol
        Dim protocol As Protocol = GetById(key)

        If protocol.Documents IsNot Nothing Then
            protocol.Documents.Clear()
        End If

        If protocol.Roles IsNot Nothing Then
            protocol.Roles.Clear()
        End If

        If protocol.RoleUsers IsNot Nothing Then
            protocol.RoleUsers.Clear()
        End If

        If protocol.ProtocolLinked IsNot Nothing Then
            protocol.ProtocolLinked.Clear()
        End If

        If protocol.ProtocolLinkedTo IsNot Nothing Then
            protocol.ProtocolLinkedTo.Clear()
        End If

        If protocol.Recipients IsNot Nothing Then
            protocol.Recipients.Clear()
        End If

        If protocol.Contacts IsNot Nothing Then
            protocol.Contacts.Clear()
        End If

        If protocol.ManualContacts IsNot Nothing Then
            protocol.ManualContacts.Clear()
        End If

        If protocol.ContactIssues IsNot Nothing Then
            protocol.ContactIssues.Clear()
        End If

        If protocol.PecMails IsNot Nothing Then
            protocol.PecMails.Clear()
        End If

        Update(protocol)

        Return protocol
    End Function

    Public Function GetCategoryDescription(prot As Protocol) As String
        Dim description As String = Factory.CategoryFacade.GetFullIncrementalName(prot.Category)
        Return description
    End Function

    Public Function GetCategoryCode(prot As Protocol) As String
        Return Factory.CategoryFacade.GetCodeDotted(prot.Category)
    End Function

    Public Function GetCategory(prot As Protocol) As Category
        Return prot.Category
    End Function

    Public Function GetCategoryString(prot As ProtocolHeader) As String
        If DocSuiteContext.Current.ProtocolEnv.ShowProtocolCategoryFullCode Then
            Return $"{Factory.CategoryFacade.GetCodeDotted(prot.CategoryFullCode)}.{prot.CategoryName}"
        End If
        Return prot.CategoryName
    End Function

    Public Shared Function GetCalculatedLink(prot As Protocol) As String
        Return String.Format("{0}|{1:0000000}|{2}|{3:dd/MM/yyyy}", prot.Year, prot.Number, prot.Location.Id, prot.RegistrationDate.ToLocalTime())
    End Function

    Public Function GetProtocolFromCalculatedLink(calculatedLink As String) As Protocol
        Dim protocolParts As String() = calculatedLink.Split("|"c)
        If protocolParts.Length < 4 Then Throw New Exception(String.Format("Il parametro {0} ricevuto non è un link di protocollo valido. Il numero dei campi è minore di 4.", calculatedLink))
        Return GetById(Short.Parse(protocolParts(0)), Integer.Parse(protocolParts(1)))
    End Function

    Public Shared Function RecuperoProtocollo(ByVal prot As Protocol, ByVal letteraName As String) As BiblosPdfDocumentInfo
        Dim doc As New BiblosPdfDocumentInfo(New BiblosDocumentInfo(prot.Location.DocumentServer, prot.Location.ProtBiblosDSDB, prot.IdDocument.Value, 0))
        doc.Caption = String.Concat(letteraName, "_", prot.Year, "_", prot.Number, FileHelper.PDF)
        Return doc
    End Function

    Public Function GetRecoveringProtocolsFinder() As NHibernateProtocolFinder
        Dim finder As NHibernateProtocolFinder = New NHibernateProtocolFinder("ProtDB")
        finder.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        finder.IdStatus = ProtocolStatusId.Errato
        finder.Year = CType(Now.Year, Short?)
        Return finder
    End Function

    Public Function GetInvoicePaProtocolsByStatuses(ByVal statuses As List(Of Integer)) As IEnumerable(Of ProtocolDTO)
        Dim protocols As IList(Of Protocol) = _dao.GetProtocolsByStatuses(statuses)
        'Mapping entità protocollo con oggetto DTO
        Return protocols.Select(Function(s) New ProtocolDTO().CopyFrom(s))
    End Function

    Public Function GetInvoicePaProtocolsFinder(status As List(Of Integer)) As NHibernateProtocolFinder
        Return GetInvoicePaProtocolsFinder(status, Nothing, Nothing)
    End Function

    Public Function GetInvoicePaProtocolsFinder(status As List(Of Integer), dateFrom As DateTime?, dateTo As DateTime?) As NHibernateProtocolFinder
        'Base
        Dim finder As NHibernateProtocolFinder = New NHibernateProtocolFinder("ProtDB")
        finder.IdProtocolKind = CType(ProtocolKind.FatturePA, Short)
        finder.ApplyProtocolKindCriteria = True
        finder.StatusList = status

        'Filtro per data
        If dateFrom.HasValue Then
            finder.RegistrationDateFrom = dateFrom
        End If
        If dateTo.HasValue Then
            finder.RegistrationDateTo = dateTo
        End If
        Return finder
    End Function

    Public Sub RaiseAfterInsert(protocol As Protocol)
        Dim args As New ProtocolEventArgs(protocol)
        If Not args.Cancel AndAlso Observer IsNot Nothing Then
            Observer.Observe(Me)
            OnAfterInsert(args)
            Observer.Disregard(Me)
        End If
    End Sub
    Public Sub RaiseAfterEdit(protocol As Protocol, documents As IList(Of DocumentInfo))
        Dim args As New ProtocolEventArgs(protocol)
        args.Tag = documents
        If Not args.Cancel AndAlso Observer IsNot Nothing Then
            Observer.Observe(Me)
            OnAfterEdit(args)
            Observer.Disregard(Me)
        End If
    End Sub
    Public Sub RaiseAfterCancel(protocol As Protocol)
        Dim args As New ProtocolEventArgs(protocol)
        If Not args.Cancel AndAlso Observer IsNot Nothing Then
            Observer.Observe(Me)
            OnAfterCancel(args)
            Observer.Disregard(Me)
        End If
    End Sub

    ''' <summary> Rigetta un protocollo. </summary>
    Public Sub Reject(ByVal protocol As Protocol, ByVal motivation As String)
        protocol.IdStatus = ProtocolStatusId.Rejected
        protocol.LastChangedReason = motivation
        protocol.Container = RejectionContainer
        Update(protocol)

        RegenerateSignatures(protocol)

        Factory.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PR, String.Format("Protocollo rigettato da {0} con motivazione [{1}]", DocSuiteContext.Current.User.FullUserName, motivation))
    End Sub

    ''' <summary> Rigenera le signatures del protocollo in biblos. </summary>
    Public Sub RegenerateSignatures(ByVal protocol As Protocol)
        ' modifica signature documento
        Dim document As DocumentInfo = GetDocument(protocol)
        document.Signature = GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Main})
        protocol.IdDocument = document.ArchiveInBiblos(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB).BiblosChainId

        ' modifica signature allegati
        Dim newAttachmentsChain As New BiblosChainInfo()
        For Each attachment As BiblosDocumentInfo In GetAttachments(protocol)
            attachment.Signature = GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Attachment})
            newAttachmentsChain.AddDocument(attachment)
        Next
        newAttachmentsChain.ArchiveInBiblos(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB)
        If Not newAttachmentsChain.Documents.IsNullOrEmpty() Then
            protocol.IdAttachments = DirectCast(newAttachmentsChain.Documents(0), BiblosDocumentInfo).BiblosChainId
        End If

        ' modifica signature annessi
        Dim newAnnexesChain As New BiblosChainInfo()
        For Each annex As BiblosDocumentInfo In GetAnnexes(protocol)
            annex.Signature = GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Annexed})
            newAnnexesChain.AddDocument(annex)
        Next
        protocol.IdAnnexed = newAnnexesChain.ArchiveInBiblos(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB)

        UpdateNoLastChange(protocol)

        ' INIZIO ####################
        ' TODO: usare questo approccio appena il bug di biblos inerente il checkin in transito viene risolto
        '
        'Dim documentChainInfo As New BiblosChainInfo(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB, protocol.IdDocument.Value)
        'For Each document As DocumentInfo In documentChainInfo.Documents
        '    document.Signature = GenerateSignature(protocol, protocol.RegistrationDate.GetValueOrDefault(Nothing), New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Main})
        'Next
        'documentChainInfo.Update("DSW8")
        '
        'Dim annexesChainInfo As New BiblosChainInfo(protocol.Location.DocumentServer, protocol.IdAnnexed)
        'For Each document As DocumentInfo In annexesChainInfo.Documents
        '    document.Signature = GenerateSignature(protocol, protocol.RegistrationDate.GetValueOrDefault(Nothing), New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Annexed})
        'Next
        'annexesChainInfo.Update("DSW8")
        '
        ' FINE #####################
    End Sub

    ''' <summary> Ritorna la descrizione per l'IdStatus specificato. </summary>
    Public Shared Function GetStatusDescription(idStatus As Integer) As String
        Try
            Dim statusVal As ProtocolStatusId = CType([Enum].Parse(GetType(ProtocolStatusId), idStatus.ToString()), ProtocolStatusId)
            If [Enum].IsDefined(GetType(ProtocolStatusId), statusVal) Then
                Return statusVal.GetDescription()
            Else
                Return String.Format("Status {0} non previsto", idStatus)
            End If
        Catch ex As Exception
            Return String.Format("Status {0} non previsto", idStatus)
        End Try
    End Function

    Public Sub SetImplicitProtocolRoles(ByRef protocol As Protocol, implicitRoles As IEnumerable(Of Integer))
        If protocol.Roles.IsNullOrEmpty() Then
            Return
        End If

        ' Popolo una lista con gli id dei settori autorizzati esplicitamente.
        Dim explicits As IEnumerable(Of Integer) = protocol.Roles.Where(Function(pr) pr.DistributionType.GetValueOrDefault(ProtocolDistributionType.Explicit).Eq(ProtocolDistributionType.Explicit)) _
                                                   .Select(Function(pr) pr.Id.Id)

        Dim missing As IEnumerable(Of Integer) = implicitRoles.Where(Function(id) Not explicits.Contains(id))
        For Each id As Integer In missing
            Dim role As Role = Factory.RoleFacade.GetById(id)
            protocol.AddRole(role, DocSuiteContext.Current.User.FullUserName, DateTimeOffset.UtcNow, ProtocolDistributionType.Implicit)
            Factory.ProtocolLogFacade.AddRoleAuthorization(protocol, role)
        Next
    End Sub

    Private Function IsChild(protocolRole As ProtocolRole, idParent As Integer) As Boolean
        ' TODO: verificare che il contains non possa dare falsi positivi
        Return protocolRole.Id.Id = idParent OrElse protocolRole.Role.FullIncrementalPath.Contains(idParent.ToString())
    End Function
    Public Sub SetProtocolRoleDistributionType(ByRef protocol As Protocol, ccRoles As IEnumerable(Of Integer))
        If protocol.Roles.IsNullOrEmpty() Then
            Return
        End If

        protocol.Roles.Where(Function(pr) ccRoles.Any(Function(id) IsChild(pr, id))) _
            .ToList().ForEach(Sub(pr) pr.Type = ProtocolRoleTypes.CarbonCopy)
    End Sub

    ''' <summary> Separa le PEC da un protocollo in modo che possano essere riprotocollate </summary>
    ''' <param name="prot">Il protocollo che deve essere disconnesso dalle sue PEC</param>
    Public Sub PecUnlink(ByVal prot As Protocol)
        For Each pec As PECMail In prot.PecMails
            Factory.PECMailFacade.Reset(pec)

            'Log
            If DocSuiteContext.Current.ProtocolEnv.IsLogEnabled Then
                ' Log della Pec
                Factory.PECMailLogFacade.InsertLog(pec, String.Format("PEC scollegata dal protocollo {0} per motivi di annullamento: {1}", prot.Id.ToString(), prot.LastChangedReason), PECMailLogType.Unlinked)
                ' Log del Protocollo
                Factory.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PA, String.Format("Scollegata PEC n.{0} del {1} con oggetto ""{2}""", pec.Id, pec.RegistrationDate.ToLocalTime().Date.ToShortDateString(), pec.MailSubject))
            End If
        Next
    End Sub

    Public Function GetExportFileName(ByVal prot As Protocol, ByVal tipo As String, ByVal docName As String) As String
        Dim vType As String = String.Empty
        Select Case prot.Type.Id
            Case -1 : vType = "M"
            Case 1 : vType = "D"
        End Select

        Dim sRet As New StringBuilder()
        'recupero il contatto mitt/dest
        sRet.AppendFormat("{0}-", GetFirstContact(prot.Year, prot.Number, vType, "-", True).Trim)
        sRet.AppendFormat("{0}{1:0000000}-", prot.Year, prot.Number)
        sRet.AppendFormat("{0}-", prot.RegistrationDate.ToLocalTime().DateTime.ToShortDateString().Replace("/", " "))
        sRet.AppendFormat("{0}-", prot.Type.ShortDescription.Replace("/", ""))
        sRet.AppendFormat("{0}-", tipo)
        sRet.Append(docName)

        Const replacewith As String = "_"
        sRet.Replace("\", replacewith)
        sRet.Replace("/", replacewith)
        sRet.Replace(":", replacewith)
        sRet.Replace("*", replacewith)
        sRet.Replace("?", replacewith)
        sRet.Replace("""", replacewith)
        sRet.Replace("<", replacewith)
        sRet.Replace(">", replacewith)
        sRet.Replace("|", replacewith)
        Return sRet.ToString()
    End Function

    Public Function AvailableProtocolKinds(ByVal idContainer As Integer) As IList(Of ProtocolKind)
        Dim currentContainer As Container = FacadeFactory.Instance.ContainerFacade.GetById(idContainer, False)
        Dim currentContainerEnv As ContainerEnv = New ContainerEnv(DocSuiteContext.Current, currentContainer)

        Dim paramsGroup As String() = currentContainerEnv.ProtocolKindContainers.Split("|"c)
        Dim availableKinds As New List(Of ProtocolKind)

        'Aggiungo la tipologia Standard
        availableKinds.Add(ProtocolKind.Standard)

        If paramsGroup Is Nothing Then
            Return Nothing
        End If

        For Each group As String In paramsGroup
            If Not group.IsNullOrEmpty Then
                Dim kind As ProtocolKind = CType(group, ProtocolKind)
                If CheckProtocolKindEnable(kind) AndAlso Not availableKinds.Any(Function(x) x.Equals(kind)) Then
                    availableKinds.Add(kind)
                End If
            End If
        Next

        Return availableKinds
    End Function

    Public Function CheckProtocolKindEnable(ByVal kind As ProtocolKind) As Boolean
        Select Case kind
            Case ProtocolKind.FatturePA
                Return DocSuiteContext.Current.ProtocolEnv.InvoicePAEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.IsInvoiceEnabled
            Case Else
                Return True
        End Select
    End Function

    Public Sub FlushAnnexed(ByVal protocol As Protocol)
        If protocol.IdAnnexed.Equals(Guid.Empty) Then
            FileLogger.Warn(LoggerName, String.Format("Nessun annesso da eliminare per il Protocollo {0}", protocol.FullNumber))
            Exit Sub
        End If

        Service.DetachDocument(protocol.Location.DocumentServer, protocol.IdAnnexed)
        protocol.IdAnnexed = Guid.Empty
        MyBase.UpdateOnly(protocol)

        'Invio comando di modifica alle WebApi
        FacadeFactory.Instance.ProtocolFacade.SendUpdateProtocolCommand(protocol)

        Const message As String = "Catena Annessi svuotata."
        FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, message)
    End Sub

    Public Function GetProtocolDocument(ByVal protocol As Protocol) As List(Of ProtocolDocumentDTO)
        Dim docs As BiblosDocumentInfo = GetDocument(protocol)

        If docs Is Nothing Then
            Return Nothing
        End If

        Dim documents As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo) From {docs}
        Dim gridList As List(Of ProtocolDocumentDTO) = New List(Of ProtocolDocumentDTO)

        If documents.Any() Then
            Dim doc As ProtocolDocumentDTO = Nothing
            For Each document As BiblosDocumentInfo In documents
                doc = New ProtocolDocumentDTO
                doc.Serialized = document.Serialized
                doc.DocumentType = "Documento"
                doc.DocumentName = document.Name
                gridList.Add(doc)
            Next
        End If

        ' Carico e visualizzo gli allegati del protocollo
        Dim attachments As BiblosDocumentInfo() = ProtocolFacade.GetAttachments(protocol)
        If attachments.Any() Then
            Dim att As ProtocolDocumentDTO = Nothing
            For Each attachment As BiblosDocumentInfo In attachments
                att = New ProtocolDocumentDTO
                att.Serialized = attachment.Serialized
                att.DocumentType = "Allegati"
                att.DocumentName = attachment.Name
                gridList.Add(att)
            Next
        End If

        Dim annexed As BiblosDocumentInfo() = ProtocolFacade.GetAnnexes(protocol)
        If annexed.Any() Then
            Dim ann As ProtocolDocumentDTO = Nothing
            For Each annex As BiblosDocumentInfo In annexed
                ann = New ProtocolDocumentDTO
                ann.Serialized = annex.Serialized
                ann.DocumentType = "Annessi"
                ann.DocumentName = annex.Name
                gridList.Add(ann)
            Next
        End If
        Return gridList
    End Function

    Public Shared Function GetProtocolDocuments(prot As Protocol) As DocumentInfo
        Dim mainFolder As New FolderInfo() With {.Name = prot.ToString("S"), .ID = prot.Id.ToString()}

        ' Documento principale
        Dim doc As BiblosDocumentInfo = GetDocument(prot, includeUniqueId:=True)
        If doc IsNot Nothing Then
            Dim folderDoc As New FolderInfo() With {.Name = "Documento", .Parent = mainFolder}
            folderDoc.AddChild(doc)
        End If

        ' Allegati
        Dim attachments() As BiblosDocumentInfo = GetAttachments(prot, includeUniqueId:=True)
        If attachments.Count > 0 Then
            Dim folderAtt As New FolderInfo() With {.Name = "Allegati (parte integrante)", .Parent = mainFolder}
            folderAtt.AddChildren(attachments)
        End If

        ' Allegati non parte integrante (Annessi)
        Dim annexes() As BiblosDocumentInfo = GetAnnexes(prot, includeUniqueId:=True)
        If annexes.Count > 0 Then
            Dim folderAtt As New FolderInfo() With {.Name = "Annessi (non parte integrante)", .Parent = mainFolder}
            folderAtt.AddChildren(annexes)
        End If

        ' Documenti "Attestazione Conformità"
        If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled Then
            Dim dematerialisation() As BiblosDocumentInfo = GetDematerialised(prot, includeUniqueId:=True)

            If dematerialisation.Count > 0 Then
                Dim folderAtt As New FolderInfo() With {.Name = "Attestazione di conformità", .Parent = mainFolder}
                folderAtt.AddChildren(dematerialisation)
            End If
        End If
        Return mainFolder
    End Function

    ''' <summary>
    ''' Invia un nuovo comando di inserimento alle web api
    ''' </summary>
    ''' <param name="protocol"></param>
    ''' <returns>Command ID</returns>
    Public Function SendInsertProtocolCommand(protocol As Protocol) As Guid?
        Return SendInsertProtocolCommand(protocol, FacadeFactory.Instance.CollaborationFacade.GetByProtocol(protocol), Nothing)
    End Function

    Public Function SendInsertProtocolCommand(protocol As Protocol, collaboration As Collaboration, toIdfascicle As Guid?) As Guid?
        Try
            Dim commandInsert As ICommandCreateProtocol = PrepareProtocolCommand(Of ICommandCreateProtocol)(protocol, collaboration, toIdfascicle,
                Function(tenantName, tenantId, collaborationUniqueId, collaborationId, collaborationTemplateName, identity, apiPRotocol, apiCategoryFascicle, documentUnit)
                    Dim command As CommandCreateProtocol = New CommandCreateProtocol(tenantName, tenantId, collaborationUniqueId, collaborationId, collaborationTemplateName, identity, apiPRotocol, apiCategoryFascicle, documentUnit)
                    If apiPRotocol.WorkflowActions IsNot Nothing Then
                        For Each workflowAction As IWorkflowAction In apiPRotocol.WorkflowActions
                            command.WorkflowActions.Add(workflowAction)
                        Next
                    End If
                    Return command
                End Function)
            CommandInsertFacade.Push(commandInsert)
            Return commandInsert.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendInsertProtocolCommand => ", ex.Message), ex)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Invia un nuovo comando di modifica alle web api
    ''' </summary>
    ''' <param name="protocol"></param>
    ''' <returns>Command ID</returns>
    Public Function SendUpdateProtocolCommand(protocol As Protocol) As Guid?
        Return SendUpdateProtocolCommand(protocol, FacadeFactory.Instance.CollaborationFacade.GetByProtocol(protocol), Nothing)
    End Function

    Public Function SendUpdateProtocolCommand(protocol As Protocol, collaboration As Collaboration, toIdfascicle As Guid?) As Guid?
        Try
            Dim commandUpdate As ICommandUpdateProtocol = PrepareProtocolCommand(Of ICommandUpdateProtocol)(protocol, collaboration, toIdfascicle,
                Function(tenantName, tenantId, collaborationUniqueId, collaborationId, collaborationTemplateName, identity, apiProtocol, apiCategoryFascicle, documentUnit)
                    Dim command As CommandUpdateProtocol = New CommandUpdateProtocol(tenantName, tenantId, collaborationUniqueId, collaborationId, collaborationTemplateName, identity, apiProtocol, apiCategoryFascicle, documentUnit)
                    If apiProtocol.WorkflowActions IsNot Nothing Then
                        For Each workflowAction As IWorkflowAction In apiProtocol.WorkflowActions
                            Command.WorkflowActions.Add(workflowAction)
                        Next
                    End If
                    Return command
                End Function)
            CommandUpdateFacade.Push(commandUpdate)
            Return commandUpdate.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendUpdateProtocolCommand => ", ex.Message), ex)
        End Try
        Return Nothing
    End Function

    Public Function PrepareProtocolCommand(Of T As {ICommand})(protocol As Protocol, collaboration As Collaboration, toIdfascicle As Guid?,
                                                               commandInitializeFunc As Func(Of String, Guid, Guid?, Integer?, String, IdentityContext, APIProtocol.Protocol, APICommons.CategoryFascicle, DocumentUnit, T)) As T
        Dim apiProtocol As APIProtocol.Protocol = MapperProtocolEntity.MappingDTO(protocol)
        If toIdfascicle.HasValue Then
            apiProtocol.WorkflowActions.Add(New WorkflowActionFascicleModel(New FascicleModel() With {.UniqueId = toIdfascicle.Value}, New DocumentUnitModel() With {.UniqueId = apiProtocol.UniqueId}, Nothing))
        End If
        Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
        Dim tenantName As String = DocSuiteContext.Current.CurrentTenant.TenantName
        Dim tenantId As Guid = DocSuiteContext.Current.CurrentTenant.TenantId
        Dim collaborationId As Integer? = Nothing
        Dim colalborationTemplateName As String = String.Empty
        Dim collaborationUniqueId As Guid?
        Dim dswCategoryFascicle As CategoryFascicle = CategoryFascicleDao.GetByIdCategory(apiProtocol.Category.EntityShortId, DSWEnvironment.Protocol)
        Dim apiCategoryFascicle As APICommons.CategoryFascicle = Nothing
        If dswCategoryFascicle IsNot Nothing Then
            apiCategoryFascicle = MapperCategoryFascicle.MappingDTO(dswCategoryFascicle)
        End If

        If collaboration IsNot Nothing AndAlso collaboration.HasProtocol AndAlso collaboration.Year.Value = protocol.Year AndAlso collaboration.Number.Value = protocol.Number Then
            collaborationId = collaboration.Id
            colalborationTemplateName = collaboration.TemplateName
            collaborationUniqueId = collaboration.UniqueId
        End If
        Return commandInitializeFunc(tenantName, tenantId, collaborationUniqueId, collaborationId, colalborationTemplateName, identity, apiProtocol, apiCategoryFascicle, Nothing)
    End Function

    Public Function GetDistributionProtocolRole(protocol As Protocol) As ProtocolRole
        For Each protocolRole As ProtocolRole In protocol.Roles
            If Factory.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Protocol, protocolRole.Role) Then
                Return protocolRole
            End If

        Next
        Return Nothing
    End Function

    Public Function GetDistributionGroupName(protocol As Protocol) As String
        Dim groupName As String = String.Empty
        Dim rg As RoleGroup
        For Each protocolRole As ProtocolRole In protocol.Roles
            rg = protocolRole.Role.RoleGroups.Where(Function(f) Not f.ProtocolRights.IsRoleManager).FirstOrDefault
            If Factory.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Protocol, protocolRole.Role) Then
                groupName = rg.SecurityGroup.GroupName
                Exit For
            End If
        Next
        Return groupName
    End Function

    Public Function GetProtocolMainDocumentPdfConverted(protocol As Protocol) As ProtocolDocumentDTO
        Dim doc As BiblosDocumentInfo = GetDocument(protocol)
        If doc Is Nothing Then
            Return Nothing
        End If

        Dim convertedDocument As BiblosPdfDocumentInfo = New BiblosPdfDocumentInfo(doc)
        Dim dto As ProtocolDocumentDTO = New ProtocolDocumentDTO With {
            .Serialized = Convert.ToBase64String(convertedDocument.GetPdfStream()),
            .DocumentType = "Documento",
            .DocumentName = convertedDocument.Name
        }
        Return dto
    End Function

    Public Function GetLastProtocolByYear(year As Short) As Protocol
        Return _dao.GetLastProtocolByYear(year)
    End Function

    Public Sub SendUserAuthorizationEmail(protocol As Protocol, user As AccountModel)
        Dim contacts As List(Of MessageContactEmail) = New List(Of MessageContactEmail)
        contacts.Add(FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(CommonUtil.GetInstance().UserDescription, DocSuiteContext.Current.User.FullUserName, FacadeFactory.Instance.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True), MessageContact.ContactPositionEnum.Sender))
        contacts.Add(FacadeFactory.Instance.MessageContactEmailFacade.CreateEmailContact(user.GetFullUserName(), user.GetFullUserName(), Factory.UserLogFacade.EmailOfUser(user.Account, user.Domain, True), MessageContact.ContactPositionEnum.Recipient))
        If contacts.Any(Function(f) String.IsNullOrEmpty(f.Email)) Then
            FileLogger.Warn(LoggerName, String.Concat("ProtocolFacade.SendEmail - Protocollo ", protocol.FullNumber, " per ", user.GetFullUserName(), ": La email non può essere inviata in quanto il mittente/destinatario non ha un indirizzo email valido"))
            Return
        End If

        Dim mailSubject As String = String.Concat("Autorizzazione al protocollo aziendale n. ", protocol.FullNumber)
        Dim mailBody As String = String.Concat("Azienda unità sanitaria locale di Reggio Emilia<br/>Archivio DocSuite: ", protocol.Container.Name, "<br/>Autorizzazione di ", DocSuiteContext.Current.User.FullUserName, " alla lettura del seguente documento ", protocol.FullNumber, "<br/><br/>Per accedere utilizzare il seguente indizzo: ", DocSuiteContext.Current.ProtocolEnv.ExternalViewerMyDocuments)
        Dim email As MessageEmail = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(contacts, mailSubject, mailBody, False)

        Dim idMessage As Integer = FacadeFactory.Instance.MessageEmailFacade.SendEmailMessage(email)
        FileLogger.Info(LoggerName, String.Concat("ProtocolFacade.SendEmail - Protocollo ", protocol.FullNumber, " per ", user.GetFullUserName(), ": Mail inserita in coda di invio [id ", idMessage, " ]"))
    End Sub

End Class