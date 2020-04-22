Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()>
Partial Public Class Protocol
    Inherits DomainObject(Of YearNumberCompositeKey)
    Implements IAuditable
    Implements IFormattable

#Region " Fields "

    Private _location As Location
    Private _attachLocation As Location
    Private _container As Container
    Private _category As Category
    Private _type As ProtocolType
    Private _docType As DocumentType
    Private _object As String
    Private _documentDate As Date?
    Private _documentProtocol As String
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _idDocument As Integer?
    Private _idAttachments As Integer?
    Private _documentCode As String
    Private _documents As IList(Of Document)
    Private _lastChangedReason As String
    Private _alternativeRecipient As String
    Private _checkPublication As String
    Private _roles As IList(Of ProtocolRole)
    Private _roleUsers As IList(Of ProtocolRoleUser)
    Private _protocolLinked As IList(Of Protocol)
    Private _protocolLinkedTo As IList(Of Protocol)
    Private _recipients As IList(Of Recipient)
    Private _contacts As IList(Of ProtocolContact)
    Private _manualContacts As IList(Of ProtocolContactManual)
    Private _contactIssues As IList(Of ProtocolContactIssue)
    Private _logs As IList(Of ProtocolLog)
    Private _pecMails As IList(Of PECMail)
    Private _advancedProtocol As AdvancedProtocol
    Private _docAreaImportStatus As DocAreaImportStatus
    Private _handlerDate As Date?
    'DOCAREA
    Private _isModified As Nullable(Of Boolean)
    Private _idHummingbird As Nullable(Of Long)
    'Change Object
    Private _objectChangeReason As String
    'Journal
    Private _journalDate As Date?
    Private _journalLog As ProtocolJournalLog
    'Check
    Private _checkDate As Date?
    'Conservation
    Private _conservationStatus As Char?
    Private _lastConservationDate As Date?
    Private _hasConservatedDocs As Boolean = False
    'TOPMEDIA
    Private _tdIdDocument As Integer?
    Private _tdError As String

    Private _idProtocolKind As Short

#End Region

#Region " Properties "

    Public Overridable Property AccountingDate() As Date?
        Get
            Return AdvanceProtocol.AccountingDate
        End Get
        Set(ByVal value As Date?)
            AdvanceProtocol.AccountingDate = value
        End Set
    End Property

    Public Overridable Property AccountingNumber() As Integer?
        Get
            Return AdvanceProtocol.AccountingNumber
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.AccountingNumber = value
        End Set
    End Property

    Public Overridable Property AccountingYear() As Nullable(Of Short)
        Get
            Return AdvanceProtocol.AccountingYear
        End Get
        Set(ByVal value As Nullable(Of Short))
            AdvanceProtocol.AccountingYear = value
        End Set
    End Property

    Public Overridable Property AccountingSectional() As String
        Get
            Return AdvanceProtocol.AccountingSectional
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.AccountingSectional = value
        End Set
    End Property

    Public Overridable Property AccountingSectionalNumber As Integer?
        Get
            Return AdvanceProtocol.AccountingSectionalNumber
        End Get
        Set(value As Integer?)
            AdvanceProtocol.AccountingSectionalNumber = value
        End Set
    End Property

    Public Overridable Property Package() As Integer?
        Get
            Return AdvanceProtocol.Package
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.Package = value
        End Set
    End Property

    Public Overridable Property PackageOrigin() As Char
        Get
            Return AdvanceProtocol.PackageOrigin
        End Get
        Set(ByVal value As Char)
            AdvanceProtocol.PackageOrigin = value
        End Set
    End Property

    Public Overridable Property PackageIncremental() As Integer?
        Get
            Return AdvanceProtocol.PackageIncremental
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.PackageIncremental = value
        End Set
    End Property

    Public Overridable Property PackageLot() As Integer?
        Get
            Return AdvanceProtocol.PackageLot
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.PackageLot = value
        End Set
    End Property

    Public Overridable Property InvoiceDate() As Date?
        Get
            Return AdvanceProtocol.InvoiceDate
        End Get
        Set(ByVal value As Date?)
            AdvanceProtocol.InvoiceDate = value
        End Set
    End Property

    Public Overridable Property InvoiceNumber() As String
        Get
            Return AdvanceProtocol.InvoiceNumber
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.InvoiceNumber = value
        End Set
    End Property

    Public Overridable Property IdentificationSDI As String
        Get
            Return AdvanceProtocol.IdentificationSDI
        End Get
        Set(value As String)
            AdvanceProtocol.IdentificationSDI = value
        End Set
    End Property

    Public Overridable Property Year() As Short
        Get
            Return Id.Year.Value
        End Get
        Set(ByVal value As Short)
            Id.Year = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return Id.Number.Value
        End Get
        Set(ByVal value As Integer)
            Id.Number = value
        End Set
    End Property

    Public Overridable Property AdvanceProtocol() As AdvancedProtocol
        Get
            If _advancedProtocol Is Nothing Then
                _advancedProtocol = New AdvancedProtocol()
                _advancedProtocol.Id = New YearNumberCompositeKey(Year, Number)
                _advancedProtocol.UniqueIdProtocol = Me.UniqueId
            End If
            Return _advancedProtocol
        End Get
        Set(ByVal value As AdvancedProtocol)
            _advancedProtocol = value
        End Set
    End Property

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Public Overridable Property AttachLocation() As Location
        Get
            Return _attachLocation
        End Get
        Set(ByVal value As Location)
            _attachLocation = value
        End Set
    End Property

    Public Overridable Property Location() As Location
        Get
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
        End Set
    End Property

    Public Overridable Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property

    Public Overridable Property Category() As Category
        Get
            Return _category
        End Get
        Set(ByVal value As Category)
            _category = value
        End Set
    End Property

    Public Overridable Property Roles() As IList(Of ProtocolRole)
        Get
            Return _roles
        End Get
        Set(ByVal value As IList(Of ProtocolRole))
            _roles = value
        End Set
    End Property

    Public Overridable Property RoleUsers() As IList(Of ProtocolRoleUser)
        Get
            Return _roleUsers
        End Get
        Set(ByVal value As IList(Of ProtocolRoleUser))
            _roleUsers = value
        End Set
    End Property

    Public Overridable Property ProtocolObject() As String
        Get
            Return _object
        End Get
        Set(ByVal value As String)
            _object = value
        End Set
    End Property

    Public Overridable Property DocumentDate() As Date?
        Get
            Return _documentDate
        End Get
        Set(ByVal value As Date?)
            _documentDate = value
        End Set
    End Property

    Public Overridable Property DocumentProtocol() As String
        Get
            Return _documentProtocol
        End Get
        Set(ByVal value As String)
            _documentProtocol = value
        End Set
    End Property

    Public Overridable Property Type() As ProtocolType
        Get
            Return _type
        End Get
        Set(ByVal value As ProtocolType)
            _type = value
        End Set
    End Property

    Public Overridable Property DocumentType() As DocumentType
        Get
            Return _docType
        End Get
        Set(ByVal value As DocumentType)
            _docType = value
        End Set
    End Property

    Public Overridable Property Documents() As IList(Of Document)
        Get
            Return _documents
        End Get
        Set(ByVal value As IList(Of Document))
            _documents = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property IdDocument() As Integer?
        Get
            Return _idDocument
        End Get
        Set(ByVal value As Integer?)
            _idDocument = value
        End Set
    End Property

    Public Overridable Property IdAttachments() As Integer?
        Get
            Return _idAttachments
        End Get
        Set(ByVal value As Integer?)
            _idAttachments = value
        End Set
    End Property

    Public Overridable Property DocumentCode() As String
        Get
            Return _documentCode
        End Get
        Set(ByVal value As String)
            _documentCode = value
        End Set
    End Property

    Public Overridable Property Status() As ProtocolStatus
        Get
            Return AdvanceProtocol.Status
        End Get
        Set(ByVal value As ProtocolStatus)
            AdvanceProtocol.Status = value
        End Set
    End Property

    ''' <summary>  </summary>
    ''' <remarks>
    ''' TODO: nel db è uno short
    ''' </remarks>
    Public Overridable Property IdStatus() As Integer?

    Public Overridable Property LastChangedReason() As String
        Get
            Return _lastChangedReason
        End Get
        Set(ByVal value As String)
            _lastChangedReason = value
        End Set
    End Property

    Public Overridable Property AlternativeRecipient() As String
        Get
            Return _alternativeRecipient
        End Get
        Set(ByVal value As String)
            _alternativeRecipient = value
        End Set
    End Property

    Public Overridable Property CheckPublication() As Boolean
        Get
            If String.IsNullOrEmpty(_checkPublication) OrElse _checkPublication.Eq("0"c) Then
                Return False
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                _checkPublication = "1"
            Else
                _checkPublication = "0"
            End If
        End Set
    End Property

    Public Overridable Property ServiceCategory() As String
        Get
            Return AdvanceProtocol.ServiceCategory
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.ServiceCategory = value
        End Set
    End Property

    Public Overridable Property Subject() As String
        Get
            Return AdvanceProtocol.Subject
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.Subject = value
        End Set
    End Property

    Public Overridable Property ServiceField() As String
        Get
            Return AdvanceProtocol.ServiceField
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.ServiceField = value
        End Set
    End Property

    Public Overridable Property Note() As String
        Get
            Return AdvanceProtocol.Note
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.Note = value
        End Set
    End Property

    Public Overridable Property IsClaim() As Nullable(Of Boolean)
        Get
            Return AdvanceProtocol.IsClaim
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            AdvanceProtocol.IsClaim = value
        End Set
    End Property

    Public Overridable Property ProtocolLinked() As IList(Of Protocol)
        Get
            Return _protocolLinked
        End Get
        Protected Set(ByVal value As IList(Of Protocol))
            _protocolLinked = value
        End Set
    End Property

    Public Overridable Property ProtocolLinkedTo() As IList(Of Protocol)
        Get
            Return _protocolLinkedTo
        End Get
        Protected Set(ByVal value As IList(Of Protocol))
            _protocolLinkedTo = value
        End Set
    End Property

    Public Overridable Property Recipients() As IList(Of Recipient)
        Get
            Return _recipients
        End Get
        Protected Set(ByVal value As IList(Of Recipient))
            _recipients = value
        End Set
    End Property

    Public Overridable Property Contacts() As IList(Of ProtocolContact)
        Get
            Return _contacts
        End Get
        Protected Set(ByVal value As IList(Of ProtocolContact))
            _contacts = value
        End Set
    End Property

    Public Overridable Property ContactIssues() As IList(Of ProtocolContactIssue)
        Get
            Return _contactIssues
        End Get
        Protected Set(ByVal value As IList(Of ProtocolContactIssue))
            _contactIssues = value
        End Set
    End Property


    Public Overridable Property ProtocolLogs() As IList(Of ProtocolLog)
        Get
            Return _logs
        End Get
        Protected Set(ByVal value As IList(Of ProtocolLog))
            _logs = value
        End Set
    End Property

    'DOCAREA
    Public Overridable Property DocAreaImportStatus() As DocAreaImportStatus
        Get
            Return _docAreaImportStatus
        End Get
        Set(ByVal value As DocAreaImportStatus)
            _docAreaImportStatus = value
        End Set
    End Property

    Public Overridable Property ManualContacts() As IList(Of ProtocolContactManual)
        Get
            Return _manualContacts
        End Get
        Protected Set(ByVal value As IList(Of ProtocolContactManual))
            _manualContacts = value
        End Set
    End Property

    Public Overridable Property PecMails() As IList(Of PECMail)
        Get
            Return _pecMails
        End Get
        Protected Set(ByVal value As IList(Of PECMail))
            _pecMails = value
        End Set
    End Property

    Public Overridable ReadOnly Property OutgoingPecMailsActive() As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If

            Return (From mail In PecMails Where mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active) AndAlso mail.Direction = PECMailDirection.Outgoing).ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property OutgoingPecMailsProcessingError() As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If

            Return (From mail In PecMails Where (mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Processing)) Or (mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error))).ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property OutgoingPecMailAll() As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If

            Return (From mail In PecMails.Where(Function(x) x.Direction.Equals(PECMailDirection.Outgoing))
                    Where mail.IsActive.Equals(ActiveType.Cast(ActiveType.PECMailActiveType.Active)) _
                        OrElse mail.IsActive.Equals(ActiveType.Cast(ActiveType.PECMailActiveType.Processing)) _
                        OrElse mail.IsActive.Equals(ActiveType.Cast(ActiveType.PECMailActiveType.Error))) _
                .ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property IngoingPecMails() As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If
            Return (From mail In PecMails Where mail.Direction = PECMailDirection.Ingoing).ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property FullNumber() As String
        Get
            Return String.Format("{0}/{1:0000000}", Year, Number)
        End Get
    End Property

    Public Overridable Property HandlerDate() As Date?
        Get
            Return _handlerDate
        End Get
        Set(value As Date?)
            _handlerDate = value
        End Set
    End Property

    ''DOCAREA
    Public Overridable Property IsModified() As Nullable(Of Boolean)
        Get
            Return _isModified
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _isModified = value
        End Set
    End Property

    'DOCAREA
    Public Overridable Property IdHummingbird() As Nullable(Of Long)
        Get
            Return _idHummingbird
        End Get
        Set(ByVal value As Nullable(Of Long))
            _idHummingbird = value
        End Set
    End Property

    'Change Object
    Public Overridable Property ObjectChangeReason() As String
        Get
            Return _objectChangeReason
        End Get
        Set(ByVal value As String)
            _objectChangeReason = value
        End Set
    End Property

    'Journal Log
    Public Overridable Property JournalDate() As Date?
        Get
            Return _journalDate
        End Get
        Set(ByVal value As Date?)
            _journalDate = value
        End Set
    End Property

    Public Overridable Property JournalLog() As ProtocolJournalLog
        Get
            Return _journalLog
        End Get
        Set(ByVal value As ProtocolJournalLog)
            _journalLog = value
        End Set
    End Property

    Public Overridable Property CheckDate() As Date?
        Get
            Return _checkDate
        End Get
        Set(ByVal value As Date?)
            _checkDate = value
        End Set
    End Property

    'Conservation
    Public Overridable Property ConservationStatus As Char?
        Get
            Return Me._conservationStatus.GetValueOrDefault("M"c)
        End Get
        Set(ByVal value As Char?)
            Me._conservationStatus = value
        End Set
    End Property

    Public Overridable Property LastConservationDate() As Date?
        Get
            Return _lastConservationDate
        End Get
        Set(ByVal value As Date?)
            _lastConservationDate = value
        End Set
    End Property

    Public Overridable Property HasConservatedDocs() As Boolean
        Get
            Return _hasConservatedDocs
        End Get
        Set(ByVal value As Boolean)
            _hasConservatedDocs = value
        End Set
    End Property

    ' ProtocolParer
    Public Overridable Property ProtocolParer As ProtocolParer

    ' Guid catena allegati annexed
    Public Overridable Property IdAnnexed As Guid

    ''Collegamento ai messaggi
    Public Overridable Property Messages As IList(Of ProtocolMessage)

    Public Overridable Property TaskHeader As IList(Of TaskHeader)

    Public Overridable Property Users As IList(Of ProtocolUser)

    Public Overridable Property RejectedRoles As IList(Of ProtocolRejectedRole)


    Public Overridable Property IdProtocolKind As Short
        Get
            Return _idProtocolKind
        End Get
        Set(value As Short)
            _idProtocolKind = value
        End Set
    End Property

    Public Overridable Property DematerialisationChainId As Guid?

    Public Overridable Property ProtocolDocumentSeriesItems As IList(Of ProtocolDocumentSeriesItem)

#Region "TOPMEDIA"

    Public Overridable Property TDIdDocument() As Integer?
        Get
            Return _tdIdDocument
        End Get
        Set(ByVal value As Integer?)
            _tdIdDocument = value
        End Set
    End Property

    Public Overridable Property TDError() As String
        Get
            Return _tdError
        End Get
        Set(ByVal value As String)
            _tdError = value
        End Set
    End Property

#End Region

#End Region

#Region " Constructor "

    Public Sub New()
        Id = New YearNumberCompositeKey()
        UniqueId = Guid.NewGuid()
        _roles = New List(Of ProtocolRole)
        _protocolLinked = New List(Of Protocol)
        _protocolLinkedTo = New List(Of Protocol)
        _recipients = New List(Of Recipient)
        _contacts = New List(Of ProtocolContact)
        _manualContacts = New List(Of ProtocolContactManual)
        _contactIssues = New List(Of ProtocolContactIssue)
        _docAreaImportStatus = New DocAreaImportStatus()
        Users = New List(Of ProtocolUser)
        RejectedRoles = New List(Of ProtocolRejectedRole)
        ProtocolDocumentSeriesItems = New List(Of ProtocolDocumentSeriesItem)
    End Sub

#End Region

#Region " Methods "

    Public Overridable Sub AppendSenders(ByVal contactList As IList(Of ContactDTO))
        For Each contact As ContactDTO In contactList
            If contact.ContactPart = ContactTypeEnum.AddressBook Then
                AddSender(contact.Contact, contact.IsCopiaConoscenza)
            Else
                AddSenderManual(contact.Contact, contact.IsCopiaConoscenza)
            End If
        Next
    End Sub

    Public Overridable Sub AppendRecipients(ByVal contactList As IList(Of ContactDTO))
        For Each contact As ContactDTO In contactList
            If contact.ContactPart = ContactTypeEnum.AddressBook Then
                AddRecipient(contact.Contact, contact.IsCopiaConoscenza)
            Else
                AddRecipientManual(contact.Contact, contact.IsCopiaConoscenza)
            End If
        Next
    End Sub

    Public Overridable Sub AddRecipientManual(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim contactVal As Contact = contact
        If String.IsNullOrEmpty(contactVal.FiscalCode) OrElse ManualContacts.FirstOrDefault(Function(f) f.Contact.FiscalCode IsNot Nothing AndAlso f.Contact.FiscalCode.Equals(contactVal.FiscalCode) AndAlso f.ComunicationType.Equals(ProtocolContactCommunicationType.Recipient)) Is Nothing Then
            Dim pcm As New ProtocolContactManual()
            pcm.ComunicationType = ProtocolContactCommunicationType.Recipient
            If copiaConoscenza Then
                pcm.Type = "CC"
            End If
            pcm.Contact = contact
            pcm.Protocol = Me
            pcm.UniqueIdProtocol = UniqueId
            ManualContacts.Add(pcm)
            pcm.Id.Id = ManualContacts.Count
            pcm.Contact.FullIncrementalPath = pcm.Id.Id.ToString()
        End If
    End Sub

    ''' <summary> Aggiunge un contatto </summary>
    ''' <remarks> Non verifica integrità dei dati, per inserire con controllo integrità usare <see cref="ProtocolContactFacade.BindContactToProtocol"/>. </remarks>
    Public Overridable Sub AddRecipient(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim contactVal As Contact = contact
        If Contacts.FirstOrDefault(Function(f) f.Contact.Id.Equals(contactVal.Id) AndAlso f.ComunicationType.Equals(ProtocolContactCommunicationType.Recipient)) Is Nothing Then
            Dim pc As New ProtocolContact()
            pc.Contact = contact
            pc.ComunicationType = ProtocolContactCommunicationType.Recipient
            If copiaConoscenza Then
                pc.Type = "CC"
            End If
            pc.Protocol = Me
            pc.UniqueIdProtocol = UniqueId
            'TODO: verificare Copia Conoscenza

            Contacts.Add(pc)
        End If
    End Sub

    Public Overridable Sub AddContactIssue(ByRef pContact As Contact)
        Dim pc As New ProtocolContactIssue()
        pc.Contact = pContact
        pc.Incremental = ContactIssues.Count + 1
        pc.Protocol = Me
        ContactIssues.Add(pc)
    End Sub

    Public Overridable Sub AddSender(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim contactVal As Contact = contact
        If Contacts.FirstOrDefault(Function(f) f.Contact.Id.Equals(contactVal.Id) AndAlso f.ComunicationType.Equals(ProtocolContactCommunicationType.Sender)) Is Nothing Then
            Dim pc As New ProtocolContact()
            pc.Contact = contact
            pc.ComunicationType = ProtocolContactCommunicationType.Sender
            If copiaConoscenza Then
                pc.Type = "CC"
            End If
            pc.Protocol = Me
            pc.UniqueIdProtocol = UniqueId
            Contacts.Add(pc)
        End If
    End Sub

    Public Overridable Sub AddSenderManual(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim contactVal As Contact = contact
        If String.IsNullOrEmpty(contactVal.FiscalCode) OrElse ManualContacts.FirstOrDefault(Function(f) f.Contact.FiscalCode.Equals(contactVal.FiscalCode) AndAlso f.ComunicationType.Equals(ProtocolContactCommunicationType.Sender)) Is Nothing Then
            Dim pcm As New ProtocolContactManual()
            pcm.ComunicationType = ProtocolContactCommunicationType.Sender
            If copiaConoscenza Then
                pcm.Type = "CC"
            End If
            pcm.Contact = contact
            pcm.Protocol = Me
            pcm.UniqueIdProtocol = UniqueId
            ManualContacts.Add(pcm)
            pcm.Id.Id = ManualContacts.Count
            pcm.Contact.FullIncrementalPath = pcm.Id.Id.ToString()
        End If
    End Sub

    Public Overridable Function GetRoles() As List(Of Role)
        Dim list As New List(Of Role)
        If Roles IsNot Nothing AndAlso Roles.Count > 0 Then
            list.AddRange(Roles.Select(Function(x) x.Role))
        End If
        Return list
    End Function

    Public Overridable Overloads Sub AddRole(ByRef role As Role, ByVal userName As String, ByVal regDate As DateTimeOffset)
        AddRole(role, userName, regDate, Nothing)
    End Sub

    Public Overridable Overloads Sub AddRole(ByRef role As Role, ByVal userName As String, ByVal regDate As DateTimeOffset, ByVal distributionType As String)
        Dim pr As ProtocolRole = New ProtocolRole()

        pr.Role = role
        pr.RegistrationUser = userName
        pr.RegistrationDate = regDate

        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            pr.DistributionType = distributionType
        End If

        pr.Protocol = Me
        pr.UniqueIdProtocol = UniqueId
        Roles.Add(pr)
    End Sub

    Public Overridable Overloads Sub AddUser(user As KeyValuePair(Of String, String))
        Dim protocolUser As ProtocolUser = New ProtocolUser()
        protocolUser.Account = user.Key
        protocolUser.Type = ProtocolUserType.Authorization
        protocolUser.UniqueIdProtocol = UniqueId
        protocolUser.Protocol = Me
        Users.Add(protocolUser)
    End Sub

    Public Overridable Overloads Sub AddRole(ByRef role As Role, ByVal rights As String)
        Dim protRole As New ProtocolRole()
        protRole.Role = role
        protRole.Rights = rights
        protRole.Protocol = Me
        protRole.UniqueIdProtocol = UniqueId
        Roles.Add(protRole)
    End Sub

    Public Overridable Sub RemoveRole(role As Role)
        Dim protRole As New ProtocolRole()
        protRole.Role = role
        protRole.Protocol = Me
        Roles.Remove(protRole)
    End Sub

    Public Overridable Overloads Function Contains(role As Role) As Boolean
        Dim pr As New ProtocolRole()
        pr.Role = role
        pr.Protocol = Me
        Return Roles.Contains(pr)
    End Function

    Public Overridable Overloads Function Contains(pruk As ProtocolRoleUserKey) As Boolean
        Dim pru As New ProtocolRoleUser
        pru.Id = pruk
        pru.Protocol = Me
        Return RoleUsers.Contains(pru)
    End Function

    Public Overridable Function ContainsRole(roleId As Integer) As Boolean
        Dim role As New Role()
        role.Id = roleId
        Return Contains(role)
    End Function

    Public Overridable Sub GetContacts(ByRef senderList As List(Of ProtocolContact), ByRef recipientList As List(Of ProtocolContact))
        For Each contact As ProtocolContact In Contacts
            If contact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                recipientList.Add(contact)
            ElseIf contact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                senderList.Add(contact)
            End If
        Next
    End Sub

    Public Overridable Function GetSenders() As IList(Of ContactDTO)
        Dim senderList As New List(Of ContactDTO)
        If Contacts IsNot Nothing Then
            senderList.AddRange(Contacts.Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Sender)).
                                        Select(Function(f) New ContactDTO(f.Contact, ContactDTO.ContactType.Address)))
        End If
        If ManualContacts IsNot Nothing Then
            senderList.AddRange(ManualContacts.Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Sender)).
                                Select(Function(f) New ContactDTO(f.Contact, ContactDTO.ContactType.Manual)))
        End If
        Return senderList
    End Function

    Public Overridable Function GetRecipients() As IList(Of ContactDTO)
        Dim recipientList As New List(Of ContactDTO)
        If Contacts IsNot Nothing Then
            recipientList.AddRange(Contacts.Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)).
                                   Select(Function(f) New ContactDTO(f.Contact, ContactDTO.ContactType.Address)))
        End If
        If ManualContacts IsNot Nothing Then
            recipientList.AddRange(ManualContacts.Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)).
                                   Select(Function(f) New ContactDTO(f.Contact, ContactDTO.ContactType.Manual)))
        End If
        Return recipientList
    End Function

    Public Overridable Sub GetManualContacts(ByRef senderList As List(Of ProtocolContactManual), ByRef recipientList As List(Of ProtocolContactManual))
        For Each contact As ProtocolContactManual In ManualContacts
            If contact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                recipientList.Add(contact)
            ElseIf contact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                senderList.Add(contact)
            End If
        Next
    End Sub

    Public Overridable Sub GetContacts(ByRef senderList As List(Of ContactDTO), ByRef recipientList As List(Of ContactDTO))
        For Each contact As ProtocolContact In Contacts
            If contact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                senderList.Add(New ContactDTO(contact.Contact, ContactDTO.ContactType.Address))
            ElseIf contact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                recipientList.Add(New ContactDTO(contact.Contact, ContactDTO.ContactType.Address))
            End If
        Next
    End Sub

    Public Overridable Sub GetManualContacts(ByRef senderList As List(Of ContactDTO), ByRef recipientList As List(Of ContactDTO))
        For Each contact As ProtocolContactManual In ManualContacts
            If contact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                senderList.Add(New ContactDTO(contact.Contact, contact.Id))
            ElseIf contact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                recipientList.Add(New ContactDTO(contact.Contact, contact.Id))
            End If
        Next
    End Sub

    Public Overrides Function ToString() As String
        Return ToString("g", Nothing)
    End Function

    Public Overridable Overloads Function ToString(format As String) As String
        Return ToString(format, Nothing)
    End Function

    Public Overridable Overloads Function ToString(formatProvider As IFormatProvider) As String
        Return ToString(Nothing, formatProvider)
    End Function

    Public Overridable Overloads Function ToString(ByVal format As String, ByVal formatProvider As IFormatProvider) As String Implements IFormattable.ToString
        If format Is Nothing Then format = "g"

        If formatProvider IsNot Nothing Then
            Dim formatter As ICustomFormatter = TryCast(formatProvider.GetFormat([GetType]()), ICustomFormatter)
            If formatter IsNot Nothing Then
                Return formatter.Format(format, Me, formatProvider)
            End If
        End If

        Select Case format.ToLowerInvariant()
            Case "g"
                Return String.Format("{0} del {1} ({2})", Id.ToString(), RegistrationDate.ToLocalTime().ToString, ProtocolObject)
            Case "s"
                Return String.Format("Protocollo {0} del {1:dd/MM/yyyy}", Id, RegistrationDate.ToLocalTime())
        End Select

        Return ToString("g")
    End Function

    ''' <summary>
    ''' Ritorna il numero totale di messaggi effettivi inviati (le e-mail vengono considerate multiple per ogni messaggio)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function MessagesCount() As Integer
        Return Messages.Sum(Function(protocolMessage) protocolMessage.Message.Emails.Count())
    End Function

#End Region
End Class