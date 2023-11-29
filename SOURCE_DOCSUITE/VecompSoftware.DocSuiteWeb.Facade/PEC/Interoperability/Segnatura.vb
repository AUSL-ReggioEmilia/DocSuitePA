Imports System.Xml.Schema
Imports System.IO
Imports System.Xml
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text.RegularExpressions
Imports VecompSoftware.Services.Biblos.Models

Public Class Segnatura

    Private Delegate Function CreateXElementHandler(ByVal contact As Contact) As XmlElement
    Private Delegate Function CreateContactHandler(ByVal contactElementKey As String, ByVal contactElement As XmlElement) As Contact
    Private Delegate Function CreateContactWithAddressesHandler(ByVal contactElementKey As String, ByVal contactElement As XmlElement, ByVal withAddressInfos As Boolean) As Contact

#Region " Fields "

    Private _segnaturaValidationResult As SegnaturaValidationResult
    Private _admCode As String = ""
    Private _aooCode As String = ""
    Private _contactCache As New Dictionary(Of Integer, XmlElement)

    ''' <summary> Collezione dei mittenti. </summary>
    Private _senders As IList(Of ContactDTO)

    ''' <summary> Collezione dei destinatari. </summary>
    Private _recipients As IList(Of ContactDTO)

    ''' <summary> Protocollo a cui si riferisce la Segnatura. </summary>
    Private ReadOnly _protocol As Protocol

    ''' <summary> Segnatura Xml. </summary>
    Private Property _xmlSignature As XmlDocument

    Private _validationException As Exception
    Private _sendingAddress As String

#End Region

#Region " Properties "

    ''' <summary> Contiene eventuali eccezioni non gestite. </summary>
    Public Property ValidationException() As Exception
        Get
            Return _validationException
        End Get
        Set(ByVal value As Exception)
            _validationException = value
        End Set
    End Property

    ''' <summary> Facade Factory per l'esecuzione delle query di lookup. </summary>
    Public Property LookupFacadeFactory() As FacadeFactory

    ''' <summary> L'indirizzo PEC mittente. </summary>
    Public Property SendingAddress() As String
        Get
            Return _sendingAddress
        End Get
        Set(ByVal value As String)
            _sendingAddress = value
        End Set
    End Property

    Private _signatureString As String
    Public Property SignatureString() As String
        Get
            Return _signatureString
        End Get
        Private Set(value As String)
            _signatureString = value
        End Set
    End Property

    Private _signatureDtdFilePath As String
    Public Property SignatureDtdFilePath As String
        Get
            Return _signatureDtdFilePath
        End Get
        Private Set(value As String)
            _signatureDtdFilePath = value
        End Set
    End Property

    Private _isValid As Boolean
    Public ReadOnly Property IsValid As Boolean
        Get
            Return _isValid
        End Get
    End Property


#End Region

#Region " Constructors "

    ''' <summary> Costruttore della classe. </summary>
    ''' <param name="protocol">Protocollo a cui si riferisce la Segnatura.</param>
    ''' <param name="signatureDtdFilePath">Path del file DTD ministeriale di validazione della Segnatura.</param>
    Public Sub New(ByVal protocol As Protocol, ByVal signatureDtdFilePath As String)
        If protocol Is Nothing Then
            Throw New ArgumentException("Il parametro 'protocol' della classe Segnatura non è inizializzato")
        End If
        _protocol = protocol
        Me.SignatureDtdFilePath = signatureDtdFilePath
        _xmlSignature = New XmlDocument()
        _xmlSignature.XmlResolver = New SignatureDTDResolver(Me.SignatureDtdFilePath)
    End Sub


    ''' <summary> Costruttore della classe. </summary>
    ''' <param name="xml">Segnatura Xml.</param>
    ''' <param name="signatureDtdFilePath">Path del file DTD ministeriale di validazione della Segnatura.</param>
    Public Sub New(ByVal xml As String, ByVal signatureDtdFilePath As String)
        Me.SignatureDtdFilePath = signatureDtdFilePath
        Me.SignatureString = xml


        _xmlSignature = New XmlDocument()
        ' resolver
        _xmlSignature.XmlResolver = New SignatureDTDResolver(Me.SignatureDtdFilePath)
        Try
            ' WORKAROUND per risolvere il problema delle segnature senza DTD
            If xml.StartsWith("<Segnatura>") Then
                xml = xml.Replace("<Segnatura>", "<?xml version=""1.0"" encoding=""ISO-8859-1""?><!DOCTYPE Segnatura SYSTEM ""Segnatura.dtd""[]><Segnatura versione=""2001-05-07"" xml:lang=""it"">")
            End If

            _xmlSignature.LoadXml(xml)
        Catch ex As Exception

            Dim temp As String = Regex.Replace(xml, "[^a-zA-Z0-9_.]", "", RegexOptions.Compiled)

            Throw New ArgumentException("Il parametro 'xmlSignature' della classe Segnatura non è inizializzato correttamente: " & ex.Message, ex)
        End Try
    End Sub

    Private Function LoadSignature(signature As String) As Boolean
        Try
            _xmlSignature = New XmlDocument()

        Catch ex As Exception
            Return False ' CARICAMENTO NON RIUSCITO
        End Try


        Return True ' CARICAMENTO RIUSCITO
    End Function

#End Region

#Region "Private Methods"

    ''' <summary> Gestore errori di validazione rispetto alla DTD </summary>
    Private Sub SignatureValidationEventHandler(ByVal sender As Object, ByVal args As ValidationEventArgs)
        'anche nodi e attributi sbagliati risultano warning
        If (args.Severity = XmlSeverityType.Warning OrElse args.Severity = XmlSeverityType.Error) Then
            _segnaturaValidationResult.IsValid = False
            _segnaturaValidationResult.NotValidMessage = args.Message
        End If
    End Sub

    ''' <summary> Controlla se il padre del nodo corrente è di tipo supportato dalla gerarchia della DTD </summary>
    ''' <param name="contactType">Tipo del contatto corrente.</param>
    ''' <param name="parentContact">Contatto padre.</param>
    Private Function CheckAddressContactHierarchy(ByVal contactType As Char, ByVal parentContact As Contact) As Boolean
        Dim correctParent As Boolean = False

        Select Case contactType
            Case Data.ContactType.Administration
            Case Data.ContactType.Aoo
                If parentContact.ContactType.Id = Data.ContactType.Administration Then
                    correctParent = True
                End If
            Case Data.ContactType.OrganizationUnit
                If parentContact.ContactType.Id = Data.ContactType.OrganizationUnit OrElse parentContact.ContactType.Id = Data.ContactType.Aoo Then
                    correctParent = True
                End If
            Case Data.ContactType.Role
                If parentContact.ContactType.Id = Data.ContactType.OrganizationUnit OrElse parentContact.ContactType.Id = Data.ContactType.Aoo Then
                    correctParent = True
                End If
            Case Data.ContactType.Person
                If parentContact.ContactType.Id = Data.ContactType.Role OrElse parentContact.ContactType.Id = Data.ContactType.OrganizationUnit OrElse parentContact.ContactType.Id = Data.ContactType.Aoo Then
                    correctParent = True
                End If
        End Select

        Return correctParent
    End Function

    Private Function ValidateEmailAddress(ByVal contact As Contact) As String
        Dim certifiedMail As String = contact.CertifiedMail

        If String.IsNullOrEmpty(certifiedMail) Then
            Throw New DocSuiteException("Validazione indirizzo PEC") With {.Descrizione = String.Format("Il contatto '{0}' selezionato non possiede un indirizzo PEC.", contact.FullDescription)}
        End If

        If Not RegexHelper.IsValidEmail(certifiedMail) Then
            Throw New DocSuiteException("Validazione indirizzo PEC") With {.Descrizione = String.Format("Il contatto '{0}' selezionato ha un indirizzo PEC non valido.", contact.FullDescription)}
        End If

        Return certifiedMail
    End Function

#Region "Costruzione ramo xml Mittente/Destinatario"

    ''' <summary> Costruisce il ramo del Mittente </summary>
    ''' <returns>Elemento xml Origine.</returns>
    Private Function BuildSender() As XmlElement
        Dim senderElement As XmlElement = Nothing

        If Not _senders.IsNullOrEmpty() Then
            Dim contactDto As ContactDTO = _senders(0)
            If contactDto.Type = ContactDTO.ContactType.Address Then
                ' contatto da rubrica
                senderElement = Me.BuildAddressContactPath(contactDto.Contact.ContactType.Id, contactDto.Contact, Me._xmlSignature.CreateElement("Mittente"), True).BranchBase.ParentNode ' il padre del contatto base è il contenitore
            Else
                ' contatto manuale
                senderElement = Me.BuildManualContactPath(contactDto.Contact, Me._xmlSignature.CreateElement("Mittente"), True).ParentNode
            End If

            If senderElement IsNot Nothing Then
                senderElement = BuildOrigin(senderElement, contactDto.Contact, SendingAddress)
            End If
        End If

        Return senderElement
    End Function

    ''' <summary> Costruisce il ramo del Destinatario </summary>
    ''' <returns>Elemento xml Destinatario.</returns>
    Private Function BuildRecipients() As IList(Of XmlElement)
        Dim recipientsElements As New List(Of XmlElement)

        For Each contactDto As ContactDTO In _recipients
            If contactDto Is Nothing Then
                Throw New ArgumentException("La lista dei destinatari contiene un elemento non inizializzato correttamente")
            End If

            Dim recipientElement As XmlElement
            If contactDto.Type = ContactDTO.ContactType.Address Then
                ' contatto da rubrica
                ' il padre del contatto base è il contenitore
                recipientElement = BuildAddressContactPath(contactDto.Contact.ContactType.Id, contactDto.Contact, _xmlSignature.CreateElement("Destinatario")).BranchBase.ParentNode
            Else
                ' contatto manuale
                recipientElement = BuildManualContactPath(contactDto.Contact, _xmlSignature.CreateElement("Destinatario")).ParentNode
            End If

            If recipientElement IsNot Nothing Then
                recipientElement = BuildDestination(recipientElement, contactDto.Contact)
                recipientsElements.Add(recipientElement)
            End If
        Next

        Return recipientsElements
    End Function

    ''' <summary> Costruisce un ramo xml valido secondo la DTD nel caso di 'contatto foglia' da rubrica. </summary>
    ''' <param name="contact">Contatto di DocSuite.</param>
    ''' <param name="containerElement">Elemento contenitore di tutto il ramo.</param>
    ''' <param name="isSender">Indica se il contatto è mittente.</param>
    ''' <returns>Un elemento xml.</returns>
    Private Function BuildAddressContactPath(ByVal contactType As Char, ByVal contact As Contact, ByVal containerElement As XmlElement, Optional ByVal isSender As Boolean = False, Optional ByVal isLeaf As Boolean = True) As SegnaturaContactsBranch
        If contact Is Nothing Then
            Throw New ArgumentException("Il parametro 'leafContact' non è correttamente inizializzato")
        End If

        Dim parentElement As XmlElement = Nothing
        Dim currentElement As XmlElement = Nothing
        Dim insertionPointElement As XmlElement = Nothing
        Dim branch As SegnaturaContactsBranch = Nothing

        Select Case contactType
            Case Data.ContactType.Administration
                If isSender Then
                    _admCode = GetContactCode(isSender, contact)
                End If

                currentElement = GetOrCreateXElementFromContact(contact, AddressOf BuildAmministrazione)
                If isLeaf Then
                    AppendAddressElementsToElement(currentElement, contact, False)
                End If
                containerElement.AppendChild(currentElement)
                branch = New SegnaturaContactsBranch(currentElement, currentElement)

            Case Data.ContactType.Aoo
                If isSender Then
                    _aooCode = GetContactCode(isSender, contact)
                End If

                If contact.Parent IsNot Nothing Then
                    branch = GetValidParentBranch(contact, "M"c, containerElement, isSender, isLeaf)
                    parentElement = branch.Leaf 'l'elemento foglia restituito dalla costruzione dell'elemento padre è l'elemento padre stesso
                    currentElement = branch.Leaf ' l'AOO non ha figli, il ramo prosegue attaccato all' Amministrazione

                    ' i mittenti hanno il nodo Aoo oltre a quello dell' Amministrazione
                    If parentElement IsNot Nothing AndAlso containerElement IsNot Nothing AndAlso isSender Then
                        containerElement.InsertAfter(GetOrCreateXElementFromContact(contact, AddressOf BuildAoo), parentElement)
                    End If
                End If

            Case Data.ContactType.OrganizationUnit
                currentElement = GetOrCreateXElementFromContact(contact, AddressOf BuildOrganizationUnit)
                If isLeaf Then AppendAddressElementsToElement(currentElement, contact, Not isSender)
                If currentElement IsNot Nothing AndAlso contact.Parent IsNot Nothing Then
                    branch = GetValidParentBranch(contact, "M"c, containerElement, isSender, isLeaf)
                    parentElement = branch.Leaf 'l'elemento foglia restituito dalla costruzione dell'elemento padre è l'elemento padre stesso

                    If parentElement.Name = "Amministrazione" Then
                        insertionPointElement = parentElement.GetElementsByTagName("CodiceAmministrazione")(0)
                    End If
                End If

            Case Data.ContactType.Role
                currentElement = GetOrCreateXElementFromContact(contact, AddressOf BuildRuolo)
                If currentElement IsNot Nothing AndAlso contact.Parent IsNot Nothing Then
                    branch = GetValidParentBranch(contact, "U"c, containerElement, isSender, isLeaf)
                    parentElement = branch.Leaf

                    Select Case parentElement.Name
                        Case "UnitaOrganizzativa"
                            insertionPointElement = parentElement.GetElementsByTagName("Denominazione")(0)
                            AppendAddressElementsToElement(parentElement, contact.Parent, Not isSender)
                        Case "Amministrazione"
                            insertionPointElement = parentElement.GetElementsByTagName("CodiceAmministrazione")(0)
                            AppendAddressElementsToElement(parentElement, contact.Parent, False)
                    End Select
                End If
            Case Data.ContactType.Person
                currentElement = GetOrCreateXElementFromContact(contact, AddressOf BuildPersona)
                If currentElement IsNot Nothing AndAlso contact.Parent IsNot Nothing Then
                    branch = GetValidParentBranch(contact, "U"c, containerElement, isSender, isLeaf)
                    parentElement = branch.Leaf

                    Select Case parentElement.Name
                        Case "UnitaOrganizzativa"
                            insertionPointElement = parentElement.GetElementsByTagName("Denominazione")(0)
                            AppendAddressElementsToElement(parentElement, contact.Parent, Not isSender)
                        Case "Amministrazione"
                            insertionPointElement = parentElement.GetElementsByTagName("CodiceAmministrazione")(0)
                            AppendAddressElementsToElement(parentElement, contact.Parent, False)
                    End Select
                End If
        End Select

        If currentElement IsNot Nothing AndAlso parentElement IsNot Nothing AndAlso Not parentElement.Equals(currentElement) Then
            If insertionPointElement IsNot Nothing Then
                parentElement.InsertAfter(currentElement, insertionPointElement)
            Else
                parentElement.AppendChild(currentElement)
            End If
        End If

        Return New SegnaturaContactsBranch(branch.BranchBase, currentElement)
    End Function

    Private Function GetOrCreateXElementFromContact(ByVal contact As Contact, ByVal createElementHandler As CreateXElementHandler) As XmlElement
        'If Me._contactCache.ContainsKey(contact.Id) Then
        '    Return Me._contactCache(contact.Id)
        'End If
        ' temporaneamente disabilitato il riutilizzo di nodi già creati perchè nel caso di amministrazioni 
        ' o UO multi-ruolo o multi-persona non è chiaro quale IndirizzoTelematico utilizzare per il nodo Destinazione

        Dim element As XmlElement = createElementHandler(contact)
        'Me._contactCache.Add(contact.Id, element)
        Return element
    End Function

    Private Function GetContactCode(ByVal isSender As Boolean, ByVal contact As Contact) As String
        If isSender AndAlso contact IsNot Nothing AndAlso Not String.IsNullOrEmpty(contact.Code) Then Return contact.Code
        Return String.Empty
    End Function

    ''' <summary> Restituisce gli estremi (base e foglia) del ramo di nodi del padre di un contatto dato. </summary>
    ''' <param name="contact">Contatto di cui si vuole ottenere il ramo di nodi del padre.</param>
    ''' <param name="alternativeType">Tipo del nodo da autogenerare qualora il padre del contatto non sia valido secondo la DTD.</param>
    ''' <param name="containerElement">Elemento contenitore del ramo.</param>
    ''' <param name="isSender">Indica se il contatto è mittente.</param>
    ''' <remarks>Il ramo è sempre valido per la DTD perchè vengono autocompilati eventuali nodi mancanti.</remarks>
    Private Function GetValidParentBranch(ByVal contact As Contact, ByVal alternativeType As Char, ByVal containerElement As XmlElement, ByVal isSender As Boolean, ByVal isLeaf As Boolean) As SegnaturaContactsBranch
        Dim branch As SegnaturaContactsBranch = Nothing
        isLeaf = (isLeaf AndAlso contact.ContactType.Id = ContactType.Aoo) 'la AOO non si conta come elemento della gerarchia

        ' controllo che sia rispettata una gerarchia compatibile con quella imposta dalla DTD ministeriale
        If CheckAddressContactHierarchy(contact.ContactType.Id, contact.Parent) Then
            ' continuo a risalire il ramo
            branch = BuildAddressContactPath(contact.Parent.ContactType.Id, contact.Parent, containerElement, isSender, isLeaf)
        Else
            ' forzo la creazione del nodo superiore, per rispettare la gerarchia
            branch = BuildAddressContactPath(alternativeType, contact, containerElement, isSender, isLeaf)
        End If

        Return branch
    End Function

    ''' <summary> Costruisce un ramo xml valido secondo la DTD nel caso di 'contatto foglia' manuale </summary>
    ''' <param name="leafContact">Contatto di livello più basso.</param>
    ''' <param name="containerElement">Elemento contenitore di tutto il ramo.</param>
    ''' <param name="createAoo">Indica se creare il nodo Aoo.</param>
    ''' <returns>Un elemento xml.</returns>
    Private Function BuildManualContactPath(ByVal leafContact As Contact, ByVal containerElement As XmlElement, Optional ByVal createAoo As Boolean = False) As XmlElement
        If leafContact Is Nothing Then
            Throw New ArgumentException("Il parametro 'leafContact' non è correttamente inizializzato")
        End If

        Dim ouElement As XmlElement
        Dim aooElement As XmlElement
        Dim ammElement As XmlElement

        ouElement = BuildOrganizationUnit(leafContact)
        ouElement = AppendAddressElementsToElement(ouElement, leafContact, True)
        aooElement = BuildAoo(leafContact)
        ammElement = BuildAmministrazione(leafContact)

        ammElement.AppendChild(ouElement)
        containerElement.AppendChild(ammElement)
        If createAoo Then containerElement.InsertAfter(aooElement, ammElement)

        Return ammElement
    End Function

#End Region

#Region "Costruzione dei nodi specifici previsti dalla DTD Ministeriale"

    ''' <summary> Costruisce il nodo Identificatore </summary>
    ''' <param name="administrationCode">Codice dell' Amministrazione.</param>
    ''' <param name="aooCode">Codice AOO.</param>
    ''' <param name="protocol">Protocollo a cui si riferisce la segnatura.</param>
    ''' <returns>Elemento xml Identificatore.</returns>
    Private Function BuildIdentificatore(ByVal administrationCode As String, ByVal aooCode As String, ByVal protocol As Protocol) As XmlElement
        '<Identificatore>
        '	<CodiceAmministrazione/>
        '	<CodiceAOO>ASMN-RE</CodiceAOO>
        '	<NumeroRegistrazione>0000030</NumeroRegistrazione>
        '	<DataRegistrazione>2009-11-17</DataRegistrazione>
        '</Identificatore>
        Dim identifElement As XmlElement = _xmlSignature.CreateElement("Identificatore")
        identifElement.AppendChild(_xmlSignature.CreateElement("CodiceAmministrazione")).InnerText = administrationCode
        identifElement.AppendChild(_xmlSignature.CreateElement("CodiceAOO")).InnerText = aooCode
        identifElement.AppendChild(_xmlSignature.CreateElement("NumeroRegistrazione")).InnerText = protocol.Number.ToString().PadLeft(7)
        Dim dataElement As XmlElement = identifElement.AppendChild(Me._xmlSignature.CreateElement("DataRegistrazione"))
        dataElement.InnerText = protocol.RegistrationDate.ToLocalTime().ToString("yyyy-MM-dd")
        Return identifElement
    End Function

    ''' <summary> Costruisce il nodo Origine </summary>
    ''' <param name="mittenteElement">Elemento xml Mittente.</param>
    ''' <param name="emailAddress">Indirizzo e-mail della casella PEC con cui si sta inviando il messaggio.</param>
    ''' <returns>Elemento xml Origine.</returns>
    Private Function BuildOrigin(ByVal mittenteElement As XmlElement, ByVal contact As Contact, Optional ByVal emailAddress As String = "") As XmlElement
        '<Origine>
        '	<IndirizzoTelematico tipo="smtp"/>
        '</Origine>
        Dim origElement As XmlElement = _xmlSignature.CreateElement("Origine")

        Dim emailAddressElement As XmlElement = origElement.AppendChild(Me._xmlSignature.CreateElement("IndirizzoTelematico"))
        emailAddressElement.SetAttribute("tipo", "smtp")
        If String.IsNullOrEmpty(emailAddress) Then
            emailAddressElement.InnerText = ValidateEmailAddress(contact)
        Else
            emailAddressElement.InnerText = emailAddress
        End If
        origElement.AppendChild(mittenteElement)

        Return origElement
    End Function

    ''' <summary> Costruisce il nodo Destinazione </summary>
    ''' <param name="destinatarioElement">Elemento xml Destinatario.</param>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Destinazione.</returns>
    Private Function BuildDestination(ByVal destinatarioElement As XmlElement, ByVal contact As Contact) As XmlElement
        '<Destinazione>
        '	<IndirizzoTelematico tipo="smtp"/>
        '</Destinazione>

        Dim destElement As XmlElement = _xmlSignature.CreateElement("Destinazione")

        Dim emailAddressElement As XmlElement = destElement.AppendChild(Me._xmlSignature.CreateElement("IndirizzoTelematico"))
        emailAddressElement.SetAttribute("tipo", "smtp")
        emailAddressElement.InnerText = ValidateEmailAddress(contact)
        destElement.AppendChild(destinatarioElement)

        Return destElement
    End Function

    ''' <summary> Costruisce il nodo Oggetto </summary>
    ''' <param name="protocol">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Oggetto.</returns>
    Private Function BuildObject(ByVal protocol As Protocol) As XmlElement
        '<Oggetto>Oggetto prot</Oggetto>

        Dim objectElement As XmlElement = _xmlSignature.CreateElement("Oggetto")
        objectElement.InnerText = protocol.ProtocolObject
        Return objectElement
    End Function

    ''' <summary> Costruisce il nodo Aoo dell' Origine </summary>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Aoo.</returns>
    Private Function BuildAoo(ByVal contact As Contact) As XmlElement
        '<AOO>
        '	<Denominazione>ASMN-TEST</Denominazione>
        '</AOO>
        Dim aooElement As XmlElement = _xmlSignature.CreateElement("AOO")
        aooElement.AppendChild(_xmlSignature.CreateElement("Denominazione")).InnerText = contact.FullDescription
        aooElement.AppendChild(_xmlSignature.CreateElement("CodiceAOO")).InnerText = contact.Code

        Return aooElement
    End Function

    ''' <summary> Costruisce il contatto Aoo </summary>
    ''' <param name="contactElementKey">Elemento Xml del file Segnatura che costituisce la chiave logica del contatto.</param>
    ''' <param name="contactElement">Elemento Xml del file Segnatura.</param>
    ''' <returns>Contatto Aoo.</returns>
    Private Function BuildAooContact(ByVal contactElementKey As String, ByVal contactElement As XmlElement) As Contact
        '<!ELEMENT AOO (Denominazione, CodiceAOO?)>

        Dim denominazione As String = ""
        Dim codiceAoo As String = ""
        Dim node As XmlNode

        Dim aoo As New Contact
        aoo.ContactType = New ContactType(ContactType.Aoo)

        If Not String.IsNullOrEmpty(contactElementKey) Then
            denominazione = contactElementKey

            node = contactElement.SelectSingleNode("CodiceAOO")
            If node IsNot Nothing Then codiceAoo = node.InnerText

            aoo.Description = denominazione
            aoo.Code = codiceAoo

            Return aoo
        End If

        Return Nothing
    End Function

    ''' <summary> Costruisce il nodo Amministrazione </summary>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Amministrazione.</returns>
    Private Function BuildAmministrazione(ByVal contact As Contact) As XmlElement
        '<Amministrazione>
        '	<Denominazione>TEST AOO</Denominazione>
        '	<CodiceAmministrazione/>
        '</Amministrazione>
        Dim ammElement As XmlElement = _xmlSignature.CreateElement("Amministrazione")
        ammElement.AppendChild(_xmlSignature.CreateElement("Denominazione")).InnerText = contact.FullDescription
        ammElement.AppendChild(_xmlSignature.CreateElement("CodiceAmministrazione")).InnerText = contact.Code

        Return ammElement
    End Function

    ''' <summary> Costruisce il contatto Amministrazione </summary>
    ''' <param name="contactElementKey">Elemento Xml del file Segnatura che costituisce la chiave logica del contatto.</param>
    ''' <param name="contactElement">Elemento Xml del file Segnatura.</param>
    ''' <param name="withAddressInfos">Indica se popolare o meno i campi relativi all' indirizzo postale, elettronico, etc.</param>
    ''' <returns>Contatto Amministrazione.</returns>
    Private Function BuildAmministrazioneContact(ByVal contactElementKey As String, ByVal contactElement As XmlElement, ByVal withAddressInfos As Boolean) As Contact
        '<!ELEMENT Amministrazione (Denominazione, CodiceAmministrazione?,
        '(UnitaOrganizzativa | ((Ruolo | Persona)*, IndirizzoPostale,
        'IndirizzoTelematico*, Telefono*, Fax*)))>

        Dim denominazione As String = ""
        Dim id As String = ""
        Dim node As XmlNode

        Dim adm As New Contact
        adm.ContactType = New ContactType(ContactType.Administration)

        If Not String.IsNullOrEmpty(contactElementKey) Then
            denominazione = contactElementKey

            node = contactElement.SelectSingleNode("CodiceAmministrazione")
            If node IsNot Nothing Then id = node.InnerText

            adm.Description = denominazione
            adm.Code = id

            If withAddressInfos Then Me.PopulateAddressContactFields(contactElement, adm)

            Return adm
        End If

        Return Nothing
    End Function

    ''' <summary> Costruisce il nodo Unità Organizzativa </summary>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml UnitaOrganizzativa.</returns>
    Private Function BuildOrganizationUnit(ByVal contact As Contact) As XmlElement
        '<UnitaOrganizzativa tipo="permanente">
        '	<Denominazione>ASMN-TEST</Denominazione>
        '   <UnitaOrganizzativa> or <Persona> or <Ruole>
        '   ... altri elementi propri di Persona o Ruolo
        '</UnitaOrganizzativa>

        Dim ouElement As XmlElement = _xmlSignature.CreateElement("UnitaOrganizzativa")
        ouElement.SetAttribute("tipo", "permanente")
        ouElement.AppendChild(_xmlSignature.CreateElement("Denominazione")).InnerText = contact.FullDescription

        Return ouElement
    End Function

    ''' <summary> Costruisce il contatto Unità Organizzativa </summary>
    ''' <param name="contactElementKey">Elemento Xml del file Segnatura che costituisce la chiave logica del contatto.</param>
    ''' <param name="contactElement">Elemento Xml del file Segnatura.</param>
    ''' <param name="withAddressInfos">Indica se popolare o meno i campi relativi all' indirizzo postale, elettronico, etc.</param>
    ''' <returns>Contatto Unità Organizzativa.</returns>
    Private Function BuildOrganizationUnitContact(ByVal contactElementKey As String, ByVal contactElement As XmlElement, ByVal withAddressInfos As Boolean) As Contact
        '<!ELEMENT UnitaOrganizzativa (Denominazione, Identificativo?,
        '(UnitaOrganizzativa | ((Ruolo | Persona)*, IndirizzoPostale,
        'IndirizzoTelematico*, Telefono*, Fax*)))>

        Dim denominazione As String = ""
        Dim id As String = ""
        Dim node As XmlNode

        Dim ou As New Contact
        ou.ContactType = New ContactType(ContactType.OrganizationUnit)

        If Not String.IsNullOrEmpty(contactElementKey) Then
            denominazione = contactElementKey

            node = contactElement.SelectSingleNode("Identificativo")
            If node IsNot Nothing Then id = node.InnerText

            ou.Description = denominazione
            ou.Code = id

            If withAddressInfos Then
                PopulateAddressContactFields(contactElement, ou)
            End If

            Return ou
        End If

        Return Nothing
    End Function

    ''' <summary> Costruisce il nodo Ruolo </summary>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Ruolo.</returns>
    Private Function BuildRuolo(ByVal contact As Contact) As XmlElement
        '<Ruolo>
        '	<Denominazione>Direttore Sanitario</Denominazione>
        '</Ruolo>

        Dim ammElement As XmlElement = _xmlSignature.CreateElement("Ruolo")
        ammElement.AppendChild(_xmlSignature.CreateElement("Denominazione")).InnerText = contact.FullDescription
        Return ammElement
    End Function

    ''' <summary> Costruisce il contatto Ruolo </summary>
    ''' <param name="contactElementKey">Elemento Xml del file Segnatura che costituisce la chiave logica del contatto.</param>
    ''' <param name="contactElement">Elemento Xml del file Segnatura.</param>
    ''' <returns>Contatto Ruolo.</returns>
    Private Function BuildRuoloContact(ByVal contactElementKey As String, ByVal contactElement As XmlElement) As Contact
        '<!ELEMENT Ruolo (Denominazione, Identificativo?, Persona?)>

        Dim denominazione As String = ""
        Dim id As String = ""
        Dim node As XmlNode

        Dim ruolo As New Contact
        ruolo.ContactType = New ContactType(ContactType.Role)

        If Not String.IsNullOrEmpty(contactElementKey) Then
            denominazione = contactElementKey

            node = contactElement.SelectSingleNode("Identificativo")
            If node IsNot Nothing Then
                id = node.InnerText
            End If

            ruolo.Description = denominazione
            ruolo.Code = id

            Return ruolo
        End If

        Return Nothing
    End Function

    ''' <summary> Costruisce il nodo Persona </summary>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Persona.</returns>
    Private Function BuildPersona(ByVal contact As Contact) As XmlElement
        '<Persona>
        '	<Nome/>
        '	<Cognome>Bianchi</Cognome>
        '	<Titolo/>
        '	<CodiceFiscale/>
        '</Persona>

        Dim personaElement As XmlElement = _xmlSignature.CreateElement("Persona")
        Dim nomeArr As String() = contact.Description.Split("|"c)

        If nomeArr.Length = 0 Then
            Throw New ArgumentException("Il cognome è obbligatorio per un contatto di tipo Persona")
        End If

        Dim nomeElemant As XmlElement = personaElement.AppendChild(Me._xmlSignature.CreateElement("Nome"))
        If nomeArr.Length > 1 Then
            nomeElemant.InnerText = nomeArr(1)
        End If

        personaElement.AppendChild(_xmlSignature.CreateElement("Cognome")).InnerText = nomeArr(0)

        personaElement.AppendChild(_xmlSignature.CreateElement("CodiceFiscale")).InnerText = contact.FiscalCode

        Return personaElement
    End Function

    ''' <summary> Costruisce il contatto Persona </summary>
    ''' <param name="contactElementKey">Elemento Xml del file Segnatura che costituisce la chiave logica del contatto.</param>
    ''' <param name="contactElement">Elemento Xml del file Segnatura.</param>
    ''' <returns>Contatto Persona.</returns>
    Private Function BuildPersonaContact(ByVal contactElementKey As String, ByVal contactElement As XmlElement) As Contact
        '<!ELEMENT Persona ((Denominazione | (Nome?, Cognome, Titolo?,
        'CodiceFiscale?)), Identificativo?)>

        Dim cognome As String = ""
        Dim nome As String = ""
        Dim cf As String = ""
        Dim id As String = ""
        Dim node As XmlNode

        Dim persona As New Contact
        persona.ContactType = New ContactType(ContactType.Person)

        If Not String.IsNullOrEmpty(contactElementKey) Then
            If contactElementKey.IndexOf("|"c) = -1 Then
                persona.Description = contactElementKey
                Return persona
            Else
                Dim aTemp() As String = contactElementKey.Split("|"c)
                cognome = aTemp(0)
                nome = aTemp(1)

                node = contactElement.SelectSingleNode("CodiceFiscale")
                If node IsNot Nothing Then cf = node.InnerText

                node = contactElement.SelectSingleNode("Identificativo")
                If node IsNot Nothing Then id = node.InnerText

                persona.Description = String.Format("{0}|{1}", cognome, nome)
                persona.FiscalCode = cf
                persona.Code = id
                Return persona
            End If
        End If

        Return Nothing
    End Function

    Private Sub CopyAddressInfos(ByRef targetContact As Contact, ByRef sourceContact As Contact)
        If (sourceContact Is Nothing) OrElse (targetContact Is Nothing) OrElse (sourceContact.Address Is Nothing) Then
            Exit Sub
        End If

        Dim targetAddress As New Address
        targetAddress.Address = sourceContact.Address.Address
        targetAddress.City = sourceContact.Address.City
        targetAddress.CityCode = sourceContact.Address.CityCode
        targetAddress.CivicNumber = sourceContact.Address.CivicNumber

        If sourceContact.Address.PlaceName IsNot Nothing Then
            Dim targetPlace As New ContactPlaceName

            targetPlace.Id = sourceContact.Address.PlaceName.Id
            targetPlace.Description = sourceContact.Address.PlaceName.Description

            targetAddress.PlaceName = targetPlace
        End If

        targetContact.Address = targetAddress
    End Sub

    Private Function GetOrCreateContact(ByVal logicalKey As String, ByVal contactType As Char, ByVal elementContact As XmlElement, ByVal createContactHandl As CreateContactHandler) As Contact
        Dim newContact As Boolean
        Dim retContact As Contact

        If LookupFacadeFactory IsNot Nothing Then
            Dim contacts As IList(Of Contact) = LookupFacadeFactory.ContactFacade.GetContactByDescriptionAndContactType(logicalKey, contactType)
            If contacts.Count > 0 Then
                retContact = contacts(0)
            Else
                retContact = createContactHandl(logicalKey, elementContact)
                newContact = True
            End If
        Else
            retContact = createContactHandl(logicalKey, elementContact)
            newContact = True
        End If

        If newContact AndAlso retContact IsNot Nothing Then
            retContact.IsActive = True
            retContact.isLocked = 0
        End If

        Return retContact
    End Function

    Private Function GetOrCreateContact(ByVal logicalKey As String, ByVal contactType As Char, ByVal elementContact As XmlElement, ByVal needAddresses As Boolean, ByVal createContactHandl As CreateContactWithAddressesHandler) As Contact
        Dim newContact As Boolean
        Dim retContact As Contact

        If LookupFacadeFactory IsNot Nothing Then
            Dim contacts As IList(Of Contact) = LookupFacadeFactory.ContactFacade.GetContactByDescriptionAndContactType(logicalKey, contactType)
            If contacts.Count > 0 Then
                retContact = contacts(0)
            Else
                retContact = createContactHandl(logicalKey, elementContact, needAddresses)
                newContact = True
            End If
        Else
            retContact = createContactHandl(logicalKey, elementContact, needAddresses)
            newContact = True
        End If

        If newContact AndAlso retContact IsNot Nothing Then
            retContact.IsActive = True
            retContact.isLocked = 0
        End If

        Return retContact
    End Function

    Private Function GetOrCreateParentOrganizationUnits(ByVal parentContactElement As XmlElement, ByVal childContact As Contact, ByVal createContactHandl As CreateContactWithAddressesHandler) As Contact
        Dim contact As Contact = Nothing

        Dim keyNode As XmlNode = parentContactElement.SelectSingleNode("Denominazione")
        If keyNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(keyNode.InnerText) Then
            Dim nodeKey As String = keyNode.InnerText

            contact = GetOrCreateContact(nodeKey, ContactType.OrganizationUnit, parentContactElement, False, createContactHandl)
            childContact.Parent = contact

            If parentContactElement.ParentNode IsNot Nothing AndAlso parentContactElement.ParentNode.Name = "UnitaOrganizzativa" Then
                Return GetOrCreateParentOrganizationUnits(parentContactElement.ParentNode, contact, createContactHandl)
            End If
        End If

        Return contact
    End Function

    ''' <summary> Costruisce il nodo Documento </summary>
    ''' <param name="documentName">Nome del file.</param>
    ''' <param name="protocolObject">Oggetto del relativo protocollo.</param>
    ''' <returns>Elemento xml Documento.</returns>
    Private Function BuildDocumento(ByVal documentName As String, Optional ByVal protocolObject As String = "") As XmlElement
        '<Documento nome="_02-BM-00061 Azienda Pagg111 &amp; 'pippo'.pdf" tipoRiferimento="MIME">
        '	<Oggetto>Oggettino</Oggetto>
        '</Documento>

        Dim docElement As XmlElement = _xmlSignature.CreateElement("Documento")
        docElement.SetAttribute("nome", documentName)
        docElement.SetAttribute("tipoRiferimento", "MIME")
        If Not String.IsNullOrEmpty(protocolObject) Then
            docElement.AppendChild(_xmlSignature.CreateElement("Oggetto")).InnerText = protocolObject
        End If

        Return docElement
    End Function

    ''' <summary> Costruisce il nodo Descrizione </summary>
    ''' <returns>Elemento xml Descrizione.</returns>
    Private Function BuildDescrizione() As XmlElement
        '<Descrizione>
        '	<Documento nome="_02-BM-00061 Azienda Pagg111 &amp; 'pippo'.pdf" tipoRiferimento="MIME">
        '		<Oggetto>Oggettino</Oggetto>
        '	</Documento>
        '	<Allegati>
        '		<Documento nome="_02-BM-00061 Azienda Pagg222.pdf" tipoRiferimento="MIME"/>
        '	</Allegati>
        '</Descrizione>

        Dim descrizioneElement As XmlElement = _xmlSignature.CreateElement("Descrizione")

        Dim doc As New BiblosDocumentInfo(_protocol.Location.ProtBiblosDSDB, _protocol.IdDocument.Value)
        If Not IsNothing(doc) Then
            descrizioneElement.AppendChild(BuildDocumento(doc.Name, _protocol.ProtocolObject))
            If _protocol.IdAttachments.HasValue And _protocol.IdAttachments.Value > 0 Then
                Dim attachments As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(_protocol.Location.ProtBiblosDSDB, _protocol.IdAttachments.Value)
                If attachments.Count > 0 Then
                    Dim attachsElement As XmlNode = descrizioneElement.AppendChild(_xmlSignature.CreateElement("Allegati"))
                    For Each attachment As BiblosDocumentInfo In attachments
                        attachsElement.AppendChild(BuildDocumento(attachment.Name, String.Empty))
                    Next
                End If
            End If
        End If

        Return descrizioneElement
    End Function
    ''' <summary> Il nodo di livello più basso che non sia Persona e Ruolo va decorato con informazioni sull' indirizzo. </summary>
    ''' <param name="element">Elemento xml in cui vanno inseriti gli elementi di indirizzo aggiuntivi.</param>
    ''' <param name="contact">Contatto di Docsuite.</param>
    ''' <returns>Elemento xml Padre dell' elemento Persona o Ruolo.</returns>
    Private Function AppendAddressElementsToElement(ByVal element As XmlElement, ByVal contact As Contact, Optional ByVal isEmailAddressMandatory As Boolean = False) As XmlElement
        '<UnitaOrganizzativa tipo="permanente"> or <Amministrazione>
        '	<Denominazione>ASMN-TEST</Denominazione>
        '       ... Persona o Ruolo ...
        '	<IndirizzoPostale>
        '       ...
        '	</IndirizzoPostale>
        '	<IndirizzoTelematico tipo="smtp">protocollo@pec.asmn.re.it</IndirizzoTelematico>
        '	<Telefono/>
        '	<Fax/>
        '</UnitaOrganizzativa>

        Dim civicNumber As String = ""
        Dim zipCode As String = ""
        Dim city As String = ""
        Dim cityCode As String = ""
        Dim placeName As String = ""

        ' IndirizzoPostale
        If contact.Address IsNot Nothing Then
            civicNumber = contact.Address.CivicNumber
            zipCode = contact.Address.ZipCode
            city = contact.Address.City
            cityCode = contact.Address.CityCode
            If contact.Address.PlaceName IsNot Nothing Then placeName = contact.Address.PlaceName.Description
        End If

        Dim postalAddressElement As XmlElement = element.AppendChild(Me._xmlSignature.CreateElement("IndirizzoPostale"))
        postalAddressElement.AppendChild(_xmlSignature.CreateElement("Toponimo")).InnerText = placeName
        postalAddressElement.AppendChild(_xmlSignature.CreateElement("Civico")).InnerText = civicNumber
        postalAddressElement.AppendChild(_xmlSignature.CreateElement("CAP")).InnerText = zipCode
        postalAddressElement.AppendChild(_xmlSignature.CreateElement("Comune")).InnerText = city
        postalAddressElement.AppendChild(_xmlSignature.CreateElement("Provincia")).InnerText = cityCode

        Dim emailAddressElement As XmlElement = element.AppendChild(Me._xmlSignature.CreateElement("IndirizzoTelematico"))
        emailAddressElement.SetAttribute("tipo", "smtp")
        If isEmailAddressMandatory Then
            emailAddressElement.InnerText = ValidateEmailAddress(contact)
        Else

            Dim emailAddress As String = String.Empty
            If Not String.IsNullOrEmpty(contact.CertifiedMail) Then
                emailAddress = contact.CertifiedMail
            End If
            emailAddressElement.InnerText = emailAddress
        End If
        element.AppendChild(_xmlSignature.CreateElement("Telefono")).InnerText = contact.TelephoneNumber
        element.AppendChild(_xmlSignature.CreateElement("Fax")).InnerText = contact.FaxNumber

        Return element
    End Function

    ''' <summary>
    ''' Compilazione dei campi relativi all'indirizzo postale ed elettronico del Contatto di DocSuite.
    ''' </summary>
    ''' <param name="element">Elemento xml da cui vengono desunte le informazioni.</param>
    ''' <param name="contact">Contatto di Docsuite da popolare.</param>
    ''' <remarks></remarks>
    Private Sub PopulateAddressContactFields(ByVal element As XmlElement, ByRef contact As Contact)
        '<!ELEMENT IndirizzoPostale (Denominazione | (Toponimo, Civico, CAP, Comune,
        'Provincia, Nazione?))>

        '(UnitaOrganizzativa | ((Ruolo | Persona)*, IndirizzoPostale,
        'IndirizzoTelematico*, Telefono*, Fax*)))>

        Dim indirizzoPostale As String = ""
        Dim toponimo As String = ""
        Dim toponimoDug As String = ""
        Dim civico As String = ""
        Dim cap As String = ""
        Dim comune As String = ""
        Dim provincia As String = ""
        Dim indirizzoTelematico As String = ""
        Dim telefono As String = ""
        Dim fax As String = ""

        Dim address As New Address
        Dim place As New ContactPlaceName
        Dim node As XmlNode

        If element IsNot Nothing AndAlso contact IsNot Nothing Then
            Dim ipNode As XmlNode = element.SelectSingleNode("IndirizzoPostale")
            If ipNode IsNot Nothing Then
                node = ipNode.SelectSingleNode("Denominazione")
                If node IsNot Nothing Then
                    indirizzoPostale = node.InnerText
                Else
                    node = ipNode.SelectSingleNode("Toponimo")
                    If node IsNot Nothing Then toponimo = node.InnerText

                    node = ipNode.SelectSingleNode("Toponimo/@dug")
                    If node IsNot Nothing Then toponimoDug = node.InnerText

                    node = ipNode.SelectSingleNode("Civico")
                    If node IsNot Nothing Then civico = node.InnerText

                    node = ipNode.SelectSingleNode("CAP")
                    If node IsNot Nothing Then cap = node.InnerText

                    node = ipNode.SelectSingleNode("Comune")
                    If node IsNot Nothing Then comune = node.InnerText

                    node = ipNode.SelectSingleNode("Provincia")
                    If node IsNot Nothing Then provincia = node.InnerText
                End If
            End If

            node = element.SelectSingleNode("IndirizzoTelematico")
            If node IsNot Nothing Then indirizzoTelematico = node.InnerText

            node = element.SelectSingleNode("Telefono")
            If node IsNot Nothing Then telefono = node.InnerText

            node = element.SelectSingleNode("Fax")
            If node IsNot Nothing Then fax = node.InnerText

            If String.IsNullOrEmpty(indirizzoPostale) Then
                address.Address = toponimo
                address.CivicNumber = civico
                address.ZipCode = cap
                address.City = comune
                address.CityCode = provincia

                If Not String.IsNullOrEmpty(toponimoDug) AndAlso LookupFacadeFactory IsNot Nothing Then
                    Dim toponimi As IList(Of ContactPlaceName) = LookupFacadeFactory.ContactPlaceNameFacade.GetByDescription(toponimoDug)
                    If toponimi IsNot Nothing AndAlso toponimi.Count = 1 Then
                        place.Id = toponimi(0).Id
                        place.Description = toponimi(0).Description

                        address.PlaceName = place
                    End If
                End If
            Else
                address.Address = indirizzoPostale
            End If

            contact.Address = address
            contact.CertifiedMail = indirizzoTelematico
            contact.EmailAddress = indirizzoTelematico
            contact.TelephoneNumber = telefono
            contact.FaxNumber = fax
        End If
    End Sub

    ''' <summary> Ripercorre un ramo di contatti seguendo i contatti padre a partire da un contatto foglia, e li inserisce in db. </summary>
    ''' <param name="leafContact">Contatto foglia.</param>
    ''' <returns>True se l'inserimento è avvenuto con successo.</returns>
    Private Function InsertContactBranchInDb(ByVal leafContact As Contact) As Boolean
        Dim contact As Contact = leafContact
        Dim contactsToInsert As New List(Of Contact)

        If LookupFacadeFactory IsNot Nothing Then
            Do
                contactsToInsert.Add(contact)
                contact = contact.Parent
            Loop While (contact IsNot Nothing)
            contactsToInsert.Reverse()

            ' non è possibile utilizzare una transazione tradizionale perchè è un'operazione multi db
            For Each contact In contactsToInsert
                If contact.Id = 0 Then
                    LookupFacadeFactory.ContactFacade.Save(contact)
                End If
            Next
        End If
    End Function

#End Region

    ''' <summary> Carica la lista dei contatti per la generazione della Segnatura xml.  </summary>
    ''' <param name="contacts">Lista di oggetti foglia ContactDTO da considerare per la generazione della Segnatura.</param>
    ''' <param name="contactDirection">Tipo di contatto: mittente o destinatario.</param>
    ''' <remarks>E' necessario chiamare questo metodo prima di invocare il metodo 'GetXmlSignature' di generazione della Segnatura da oggetto Protocol.</remarks>
    Public Sub LoadContactList(ByVal contacts As IList(Of ContactDTO), ByVal contactDirection As ContactDirection)
        'verifica il numero di contatti da caricare
        If contacts Is Nothing OrElse contacts.Count = 0 Then
            Throw New ArgumentException("Il parametro 'contacts' non è correttamente inizializzato")
        End If
        For Each contact As ContactDTO In contacts
            Dim contactType As Char = contact.Contact.ContactType.Id
            If contact.Type = ContactDTO.ContactType.Address AndAlso Not (contactType = Data.ContactType.Aoo OrElse contactType = Data.ContactType.OrganizationUnit OrElse contactType = Data.ContactType.Role OrElse contactType = Data.ContactType.Person) Then
                Throw New ArgumentException("I contatti da Rubrica utilizzabili per l'invio di messaggi di posta interoperabili possono essere solo di tipo: AOO, Unità Organizzativa, Ruolo o Persona")
            End If
        Next

        Select Case contactDirection
            Case ContactDirection.Sender
                If _senders Is Nothing Then
                    _senders = New List(Of ContactDTO)
                End If

                For Each contactDto As ContactDTO In contacts
                    _senders.Add(contactDto)
                Next
            Case ContactDirection.Recipient
                If _recipients Is Nothing Then
                    _recipients = New List(Of ContactDTO)
                End If

                For Each contactDto As ContactDTO In contacts
                    _recipients.Add(contactDto)
                Next
        End Select
    End Sub

    ''' <summary> Validazione della Segnatura secondo DTD Ministeriale. </summary>
    Public Function IsInteropSignatureValid() As SegnaturaValidationResult
        _segnaturaValidationResult = New SegnaturaValidationResult()

        If _xmlSignature Is Nothing Then
            Return _segnaturaValidationResult
        End If

        Try
            If Not File.Exists(SignatureDtdFilePath) Then
                Throw New FileNotFoundException(String.Format("Percorso SignatureDtdFilePath ""{0}"" non valido.", SignatureDtdFilePath))
            End If

            If _xmlSignature.DocumentType Is Nothing Then
                _xmlSignature.InsertBefore(_xmlSignature.CreateDocumentType("Segnatura", Nothing, "Segnatura.dtd", Nothing), _xmlSignature.DocumentElement)
            End If

            Dim settings As New XmlReaderSettings
            settings.XmlResolver = New SignatureDTDResolver(SignatureDtdFilePath)
            settings.ValidationType = ValidationType.DTD
            settings.ProhibitDtd = False
            AddHandler settings.ValidationEventHandler, AddressOf SignatureValidationEventHandler

            Dim reader As XmlReader = XmlReader.Create(New StringReader(_xmlSignature.OuterXml), settings)

            _segnaturaValidationResult.IsValid = True
            While (reader.Read())
                If (Not _segnaturaValidationResult.IsValid) Then
                    Exit While
                End If
            End While
        Catch ex As Exception
            ValidationException = ex
            _segnaturaValidationResult.IsValid = False
        End Try

        Return _segnaturaValidationResult
    End Function

    ''' <summary> Restituisce la Segnatura.xml come XmlDocument. </summary>
    Public Function GetXmlSignature() As XmlDocument
        If Not String.IsNullOrEmpty(_xmlSignature.OuterXml) Then
            Return _xmlSignature
        End If

        ' Mittente
        _contactCache.Clear()
        Dim senderElement As XmlElement = BuildSender()

        ' Destinatari
        _contactCache.Clear()
        Dim recipientsElements As IList(Of XmlElement) = BuildRecipients()

        ' dichiarazione
        Dim xmldecl As XmlDeclaration = _xmlSignature.CreateXmlDeclaration("1.0", "ISO-8859-1", "")
        Dim root As XmlElement = _xmlSignature.DocumentElement
        _xmlSignature.InsertBefore(xmldecl, root)

        ' Segnatura
        Dim segnaturaElement As XmlElement = _xmlSignature.CreateElement("Segnatura")
        _xmlSignature.AppendChild(segnaturaElement)
        segnaturaElement.SetAttribute("versione", "2001-05-07") ' ISO 8601 esteso (aaaa-mm-gg)
        segnaturaElement.SetAttribute("xml:lang", "it")

        ' intestazione
        Dim intestazioneElement As XmlElement = _xmlSignature.CreateElement("Intestazione")
        segnaturaElement.AppendChild(intestazioneElement)

        ' nodo Identificatore
        intestazioneElement.AppendChild(BuildIdentificatore(_admCode, _aooCode, _protocol))

        ' nodo Origine
        If senderElement IsNot Nothing Then
            intestazioneElement.AppendChild(senderElement)
        End If

        ' nodi Destinazione
        For Each recElement As XmlElement In recipientsElements
            If recElement IsNot Nothing Then
                intestazioneElement.AppendChild(recElement)
            End If
        Next

        ' nodo Oggetto
        intestazioneElement.AppendChild(BuildObject(_protocol))

        ' descrizione
        Dim descrizioneElement As XmlElement = BuildDescrizione()
        segnaturaElement.AppendChild(descrizioneElement)

        ' doctype
        Dim doctype As XmlDocumentType = _xmlSignature.CreateDocumentType("Segnatura", Nothing, "Segnatura.dtd", Nothing)
        _xmlSignature.InsertAfter(doctype, xmldecl)


        Return _xmlSignature
    End Function

    ''' <summary> Estrae i contatti dalla segnatura. </summary>
    ''' <param name="insertContactType">Tipo di contatto: da rubrica o manuale.</param>
    Public Function GetContactsFromSignature(ByVal insertContactType As ContactDTO.ContactType) As IList(Of ContactDTO)
        If String.IsNullOrEmpty(_xmlSignature.OuterXml) Then
            Return Nothing
        End If

        Dim contacts As New List(Of ContactDTO)
        Dim highestParent As New Contact

        Dim addressInfosCreated As Boolean = False

        Dim personList As XmlNodeList = _xmlSignature.SelectNodes("//Mittente//Persona")
        If personList IsNot Nothing AndAlso personList.Count > 0 Then
            For Each node As XmlNode In personList
                ' ottengo il contatto
                Dim keyNode As XmlNode = node.SelectSingleNode("Denominazione")
                Dim nodeKey As String = ""
                If keyNode IsNot Nothing Then
                    nodeKey = keyNode.InnerText
                Else
                    keyNode = node.SelectSingleNode("Cognome")
                    If keyNode IsNot Nothing Then
                        Dim nome As String = ""
                        Dim cognome As String = keyNode.InnerText

                        keyNode = node.SelectSingleNode("Nome")
                        If keyNode IsNot Nothing Then
                            nome = keyNode.InnerText
                        End If
                        nodeKey = String.Format("{0}|{1}", cognome, nome)
                    End If
                End If

                Dim contact As Contact = GetOrCreateContact(nodeKey, ContactType.Person, CType(node, XmlElement), AddressOf BuildPersonaContact)
                If contact Is Nothing Then
                    FileLogger.Warn(LogName.FileLog, String.Format("Impossibile generare contatto partendo da [{0}]", node.OuterXml))
                    Continue For
                End If
                highestParent = contact
                contacts.Add(New ContactDTO(contact, insertContactType))
            Next
        End If

        Dim roleList As XmlNodeList = _xmlSignature.SelectNodes("//Mittente//Ruolo")
        If roleList IsNot Nothing AndAlso roleList.Count > 0 Then
            For Each node As XmlNode In roleList
                ' ottengo il contatto
                Dim keyNode As XmlNode = node.SelectSingleNode("Denominazione")
                Dim nodeKey As String = ""
                If keyNode IsNot Nothing Then
                    nodeKey = keyNode.InnerText
                End If
                Dim contact As Contact = GetOrCreateContact(nodeKey, ContactType.Role, CType(node, XmlElement), AddressOf BuildRuoloContact)
                If contact Is Nothing Then
                    FileLogger.Warn(LogName.FileLog, String.Format("Impossibile generare contatto partendo da [{0}]", node.OuterXml))
                    Continue For
                End If

                If insertContactType = ContactDTO.ContactType.Address Then
                    ' mi aspetto un mittente univoco
                    If contacts.Count = 1 Then
                        highestParent.Parent = contact
                    ElseIf contacts.Count = 0 Then
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If
                    highestParent = contact
                Else
                    contact.ContactType = New ContactType(ContactType.Person)
                    If contacts.Count = 0 Then
                        ' se è presente il contatto persona il ruolo viene ignorato
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If
                End If
            Next
        End If

        Dim organizationUnitList As XmlNodeList = _xmlSignature.SelectNodes("//Mittente//UnitaOrganizzativa[IndirizzoPostale]")
        If organizationUnitList IsNot Nothing AndAlso organizationUnitList.Count > 0 Then
            For Each node As XmlNode In organizationUnitList
                ' ottengo il contatto
                Dim keyNode As XmlNode = node.SelectSingleNode("Denominazione")
                Dim nodeKey As String = ""
                If keyNode IsNot Nothing Then
                    nodeKey = keyNode.InnerText
                End If
                Dim contact As Contact = GetOrCreateContact(nodeKey, ContactType.OrganizationUnit, CType(node, XmlElement), True, AddressOf BuildOrganizationUnitContact)
                If contact Is Nothing Then
                    FileLogger.Warn(LogName.FileLog, String.Format("Impossibile generare contatto partendo da [{0}]", node.OuterXml))
                    Continue For
                End If

                addressInfosCreated = True
                If insertContactType = ContactDTO.ContactType.Address Then
                    ' mi aspetto un mittente univoco
                    If contacts.Count = 1 Then
                        highestParent.Parent = contact
                    ElseIf contacts.Count = 0 Then
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If

                    ' risalgo la gerarchia ricorsivamente
                    If node.ParentNode IsNot Nothing AndAlso node.ParentNode.Name = "UnitaOrganizzativa" Then
                        contact = GetOrCreateParentOrganizationUnits(node.ParentNode, contact, AddressOf BuildOrganizationUnitContact)
                    End If

                    highestParent = contact
                Else
                    contact.ContactType = New ContactType(ContactType.Aoo)

                    If contacts.Count = 1 Then
                        ' se è già stato caricato un contatto di tipo persona o ruolo gli aggiungo le informazioni di indirizzo dell'organization unit
                        CopyAddressInfos(contacts(0).Contact, contact)
                    ElseIf contacts.Count = 0 Then
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If
                    Return contacts
                End If
            Next
        End If

        Dim aooList As XmlNodeList = _xmlSignature.SelectNodes("//Mittente/AOO")
        If aooList IsNot Nothing AndAlso aooList.Count > 0 Then
            For Each node As XmlNode In aooList
                Dim keyNode As XmlNode = node.SelectSingleNode("Denominazione")
                Dim nodeKey As String = ""
                If keyNode IsNot Nothing Then
                    nodeKey = keyNode.InnerText
                End If
                Dim contact As Contact = GetOrCreateContact(nodeKey, ContactType.Aoo, CType(node, XmlElement), AddressOf BuildAooContact)
                If contact Is Nothing Then
                    FileLogger.Warn(LogName.FileLog, String.Format("Impossibile generare contatto partendo da [{0}]", node.OuterXml))
                    Continue For
                End If

                If insertContactType = ContactDTO.ContactType.Address Then
                    ' mi aspetto un mittente univoco
                    If contacts.Count = 1 Then
                        highestParent.Parent = contact
                    ElseIf contacts.Count = 0 Then
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If
                    highestParent = contact
                End If
            Next
        End If

        Dim administrationList As XmlNodeList = _xmlSignature.SelectNodes("//Mittente/Amministrazione")
        If administrationList IsNot Nothing AndAlso administrationList.Count > 0 Then
            For Each node As XmlNode In administrationList
                Dim keyNode As XmlNode = node.SelectSingleNode("Denominazione")
                Dim nodeKey As String = ""
                If keyNode IsNot Nothing Then
                    nodeKey = keyNode.InnerText
                End If
                Dim contact As Contact = GetOrCreateContact(nodeKey, ContactType.Administration, CType(node, XmlElement), Not addressInfosCreated, AddressOf BuildAmministrazioneContact)
                If contact Is Nothing Then
                    FileLogger.Warn(LogName.FileLog, String.Format("Impossibile generare contatto partendo da [{0}]", node.OuterXml))
                    Continue For
                End If

                If insertContactType = ContactDTO.ContactType.Address Then
                    If contacts.Count = 1 Then ' mi aspetto un mittente univoco
                        highestParent.Parent = contact
                    ElseIf contacts.Count = 0 Then
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If
                    highestParent = contact
                Else
                    If contacts.Count = 1 Then ' mi aspetto un mittente univoco
                        ' se è già stato caricato un contatto di tipo persona o ruolo gli aggiungo le informazioni di indirizzo dell'amministrazione
                        CopyAddressInfos(contacts(0).Contact, contact)
                    ElseIf contacts.Count = 0 Then
                        contacts.Add(New ContactDTO(contact, insertContactType))
                    End If
                End If
            Next
        End If

        Return contacts
    End Function

    ''' <summary> Estrae i contatti dalla segnatura mettendoli in DB se non sono presenti. </summary>
    ''' <param name="insertContactType">Tipo di contatto: da rubrica o manuale.</param>
    ''' <param name="lookupFacadeFactory">FacadeFactory per eseguire le lookup in Db.</param>
    Public Function GetOrCreateContactsFromSignature(ByVal insertContactType As ContactDTO.ContactType, Optional ByVal lookupFacadeFactory As FacadeFactory = Nothing) As IList(Of ContactDTO)
        ' Per ogni elemento a partire dall’ elemento-foglia e risalendo la gerarchia eseguo una lookup in db, se non presente lo creo.
        ' appena trovo una corrispondenza in db mi fermo nella ricerca: mi aggancio ai rami esistenti.
        Dim leafContacts As IList(Of ContactDTO) = GetContactsFromSignature(insertContactType)
        If leafContacts IsNot Nothing AndAlso leafContacts.Count > 0 Then
            For Each leafContact As ContactDTO In leafContacts
                InsertContactBranchInDb(leafContact.Contact)
            Next
        End If

        Return leafContacts
    End Function

    ''' <summary> Nome del documento principale della segnatura. </summary>
    ''' <remarks>
    ''' 'TODO: Da gestire la ricezione del nodo 'TestoDelMessaggio'
    ''' </remarks>
    Public Function GetMainDocumentNameFromSignature() As String
        If String.IsNullOrEmpty(_xmlSignature.OuterXml) Then
            Return Nothing
        End If

        Dim nodeList As XmlNodeList = _xmlSignature.SelectNodes("//Descrizione/Documento/@nome")
        If (nodeList Is Nothing) OrElse nodeList.Count <> 1 Then
            Return Nothing
        End If

        Return nodeList(0).InnerText
    End Function

    ''' <summary> Restituisce l'oggetto del protocollo leggendolo dal file Segnatura.xml. </summary>
    Public Function GetObjectFromSignature() As String
        If String.IsNullOrEmpty(_xmlSignature.OuterXml) Then
            Return Nothing
        End If

        Dim nodeList As XmlNodeList = _xmlSignature.SelectNodes("//Descrizione/Documento/Oggetto")
        If (nodeList Is Nothing) OrElse nodeList.Count <> 1 Then
            Return Nothing
        End If


        Return nodeList(0).InnerText
    End Function

    Public Sub Save(ByVal filePath As String)
        If _xmlSignature IsNot Nothing Then
            _xmlSignature.Save(filePath)
        End If
    End Sub

    Public Function GetOriginalProtocolNumber() As String
        Dim retval As String = String.Empty
        If Not String.IsNullOrEmpty(_xmlSignature.OuterXml) Then
            'Numero protocollo mittente
            Dim node As XmlNode = _xmlSignature.SelectSingleNode("//Identificatore/NumeroRegistrazione")
            If node IsNot Nothing Then
                retval = node.InnerText
            End If
        End If
        Return retval
    End Function

    Public Function GetOriginalProtocolData() As DateTime?
        If Not String.IsNullOrEmpty(_xmlSignature.OuterXml) Then
            'Data protocollo mittente
            Dim node As XmlNode = _xmlSignature.SelectSingleNode("//Identificatore/DataRegistrazione")
            If node IsNot Nothing Then
                Dim retval As String = node.InnerText
                Return Date.ParseExact(retval, "yyyy-MM-dd", Globalization.CultureInfo.CurrentCulture.DateTimeFormat)
            End If
        End If
        Return Nothing
    End Function

#End Region

End Class

