Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()>
Partial Public Class Protocol
    Inherits DomainObject(Of Guid)
    Implements IAuditable
    Implements IFormattable
    Implements ISupportTenant

#Region " Fields "

    Private _checkPublication As String
    Private _advancedProtocol As AdvancedProtocol
    Private _conservationStatus As Char?

#End Region

#Region " Properties "

    Public Overridable Property AccountingDate As Date?
        Get
            Return AdvanceProtocol.AccountingDate
        End Get
        Set(ByVal value As Date?)
            AdvanceProtocol.AccountingDate = value
        End Set
    End Property

    Public Overridable Property AccountingNumber As Integer?
        Get
            Return AdvanceProtocol.AccountingNumber
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.AccountingNumber = value
        End Set
    End Property

    Public Overridable Property AccountingYear As Short?
        Get
            Return AdvanceProtocol.AccountingYear
        End Get
        Set(ByVal value As Short?)
            AdvanceProtocol.AccountingYear = value
        End Set
    End Property

    Public Overridable Property AccountingSectional As String
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

    Public Overridable Property Package As Integer?
        Get
            Return AdvanceProtocol.Package
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.Package = value
        End Set
    End Property

    Public Overridable Property PackageOrigin As Char
        Get
            Return AdvanceProtocol.PackageOrigin
        End Get
        Set(ByVal value As Char)
            AdvanceProtocol.PackageOrigin = value
        End Set
    End Property

    Public Overridable Property PackageIncremental As Integer?
        Get
            Return AdvanceProtocol.PackageIncremental
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.PackageIncremental = value
        End Set
    End Property

    Public Overridable Property PackageLot As Integer?
        Get
            Return AdvanceProtocol.PackageLot
        End Get
        Set(ByVal value As Integer?)
            AdvanceProtocol.PackageLot = value
        End Set
    End Property

    Public Overridable Property InvoiceDate As Date?
        Get
            Return AdvanceProtocol.InvoiceDate
        End Get
        Set(ByVal value As Date?)
            AdvanceProtocol.InvoiceDate = value
        End Set
    End Property

    Public Overridable Property InvoiceNumber As String
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

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property AdvanceProtocol() As AdvancedProtocol
        Get
            If _advancedProtocol Is Nothing Then
                _advancedProtocol = New AdvancedProtocol()
                _advancedProtocol.Protocol = Me
                _advancedProtocol.Year = Year
                _advancedProtocol.Number = Number
            End If
            Return _advancedProtocol
        End Get
        Set(ByVal value As AdvancedProtocol)
            _advancedProtocol = value
        End Set
    End Property

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property AttachLocation As Location

    Public Overridable Property Location As Location

    Public Overridable Property Container As Container

    Public Overridable Property Category As Category

    Public Overridable Property Roles As IList(Of ProtocolRole)

    Public Overridable Property RoleUsers As IList(Of ProtocolRoleUser)

    Public Overridable Property ProtocolObject As String

    Public Overridable Property DocumentDate As Date?

    Public Overridable Property DocumentProtocol As String

    Public Overridable Property Type As ProtocolType

    Public Overridable Property DocumentType As DocumentType

    Public Overridable Property Documents As IList(Of Document)

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property IdDocument As Integer?

    Public Overridable Property IdAttachments As Integer?

    Public Overridable Property DocumentCode As String

    Public Overridable Property Status As ProtocolStatus
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
    Public Overridable Property IdStatus As Integer?

    Public Overridable Property LastChangedReason As String

    Public Overridable Property AlternativeRecipient As String

    Public Overridable Property CheckPublication As Boolean
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

    Public Overridable Property ServiceCategory As String
        Get
            Return AdvanceProtocol.ServiceCategory
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.ServiceCategory = value
        End Set
    End Property

    Public Overridable Property Subject As String
        Get
            Return AdvanceProtocol.Subject
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.Subject = value
        End Set
    End Property

    Public Overridable Property ServiceField As String
        Get
            Return AdvanceProtocol.ServiceField
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.ServiceField = value
        End Set
    End Property

    Public Overridable Property Note As String
        Get
            Return AdvanceProtocol.Note
        End Get
        Set(ByVal value As String)
            AdvanceProtocol.Note = value
        End Set
    End Property

    Public Overridable Property IsClaim As Boolean?
        Get
            Return AdvanceProtocol.IsClaim
        End Get
        Set(ByVal value As Boolean?)
            AdvanceProtocol.IsClaim = value
        End Set
    End Property

    Public Overridable Property ProtocolLinks As IList(Of ProtocolLink)

    Public Overridable Property ProtocolParentLinks As IList(Of ProtocolLink)

    Public Overridable Property Recipients As IList(Of Recipient)

    Public Overridable Property Contacts As IList(Of ProtocolContact)

    Public Overridable Property ContactIssues As IList(Of ProtocolContactIssue)

    Public Overridable Property ProtocolLogs As IList(Of ProtocolLog)

    Public Overridable Property DocAreaImportStatus As DocAreaImportStatus

    Public Overridable Property ManualContacts As IList(Of ProtocolContactManual)

    Public Overridable Property PecMails As IList(Of PECMail)

    Public Overridable ReadOnly Property OutgoingPecMailsActive As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If

            Return (From mail In PecMails Where mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active) AndAlso mail.Direction = PECMailDirection.Outgoing).ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property OutgoingPecMailsProcessingError As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If

            Return (From mail In PecMails Where (mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Processing)) Or (mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error))).ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property OutgoingPecMailAll As IList(Of PECMail)
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

    Public Overridable ReadOnly Property IngoingPecMails As IList(Of PECMail)
        Get
            If PecMails Is Nothing Then
                Return Nothing
            End If
            Return (From mail In PecMails Where mail.Direction = PECMailDirection.Ingoing).ToList()
        End Get
    End Property

    Public Overridable ReadOnly Property FullNumber As String
        Get
            Return String.Format("{0}/{1:0000000}", Year, Number)
        End Get
    End Property

    Public Overridable Property HandlerDate As Date?

    ''DOCAREA
    Public Overridable Property IsModified As Boolean?

    'DOCAREA
    Public Overridable Property IdHummingbird As Long?

    'Change Object
    Public Overridable Property ObjectChangeReason As String

    'Journal Log
    Public Overridable Property JournalDate As Date?

    Public Overridable Property JournalLog As ProtocolJournalLog

    Public Overridable Property CheckDate As Date?

    'Conservation
    Public Overridable Property ConservationStatus As Char?
        Get
            Return Me._conservationStatus.GetValueOrDefault("M"c)
        End Get
        Set(ByVal value As Char?)
            Me._conservationStatus = value
        End Set
    End Property

    Public Overridable Property LastConservationDate As Date?

    Public Overridable Property HasConservatedDocs As Boolean

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

    Public Overridable Property DematerialisationChainId As Guid?

    Public Overridable Property ProtocolDocumentSeriesItems As IList(Of ProtocolDocumentSeriesItem)

    Public Overridable Property TDIdDocument As Integer?

    Public Overridable Property TDError As String

    Public Overridable Property IdTenantAOO As Guid? Implements ISupportTenant.IdTenantAOO

#End Region

#Region " Constructor "

    Public Sub New()
        Id = Guid.NewGuid()
        Roles = New List(Of ProtocolRole)
        ProtocolLinks = New List(Of ProtocolLink)
        ProtocolParentLinks = New List(Of ProtocolLink)
        Recipients = New List(Of Recipient)
        Contacts = New List(Of ProtocolContact)
        ManualContacts = New List(Of ProtocolContactManual)
        ContactIssues = New List(Of ProtocolContactIssue)
        DocAreaImportStatus = New DocAreaImportStatus()
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
            ManualContacts.Add(pcm)
            pcm.Incremental = ManualContacts.Count
            pcm.Contact.FullIncrementalPath = pcm.Incremental.ToString()
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
            ManualContacts.Add(pcm)
            pcm.Incremental = ManualContacts.Count
            pcm.Contact.FullIncrementalPath = pcm.Incremental.ToString()
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
        Roles.Add(pr)
    End Sub

    Public Overridable Overloads Sub AddUser(user As KeyValuePair(Of String, String))
        Dim protocolUser As ProtocolUser = New ProtocolUser()
        protocolUser.Account = user.Key
        protocolUser.Type = ProtocolUserType.Authorization
        protocolUser.Protocol = Me
        Users.Add(protocolUser)
    End Sub

    Public Overridable Overloads Sub AddRole(ByRef role As Role, ByVal rights As String)
        Dim protRole As New ProtocolRole()
        protRole.Role = role
        protRole.Rights = rights
        protRole.Protocol = Me
        Roles.Add(protRole)
    End Sub

    Public Overridable Sub RemoveRole(role As Role)
        If Roles Is Nothing OrElse Roles.IsNullOrEmpty() Then
            Exit Sub
        End If

        Dim toRemove As ProtocolRole = Roles.Single(Function(x) x.Role.Id = role.Id)
        Roles.Remove(toRemove)
    End Sub

    Public Overridable Overloads Function Contains(role As Role) As Boolean
        If Roles Is Nothing OrElse Roles.IsNullOrEmpty() Then
            Return False
        End If

        Return Roles.Any(Function(x) x.Role.Id = role.Id)
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
                Return String.Format("{0} del {1} ({2})", FullNumber, RegistrationDate.ToLocalTime().ToString, ProtocolObject)
            Case "s"
                Return String.Format("Protocollo {0} del {1:dd/MM/yyyy}", FullNumber, RegistrationDate.ToLocalTime())
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