Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class SegnaturaReader

#Region " Constructors "

    Public Sub New(xml As String)
        Me.XmlSource = xml
        Me.XmlCleaned = xml
    End Sub

    Public Sub New(xml As String, baseParentIdContact As Integer)
        Me.New(xml)
        Me.BaseParentContact = FacadeFactory.Instance.ContactFacade.GetById(baseParentIdContact)
    End Sub

#End Region

#Region " Fields "

    Private _loadingError As Exception

    Private _dtdPath As String

#End Region

#Region " Properties "

    Public Property XmlSource As String

    Public Property XmlCleaned As String

    Public Property XmlDoc As XmlDocument

    Public ReadOnly Property IsValid As Boolean
        Get
            Return XmlCleaned.Eq(XmlSource)
        End Get
    End Property

    Public ReadOnly Property HasErrors As Boolean
        Get
            Return _loadingError IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property LoadingError As Exception
        Get
            Return _loadingError
        End Get
    End Property

    Public Property DtdPath As String
        Get
            If String.IsNullOrEmpty(_dtdPath) Then
                _dtdPath = DocSuiteContext.PecSegnature
            End If

            Return _dtdPath
        End Get
        Set(value As String)
            _dtdPath = value
        End Set
    End Property

    Public Property IsWithoutDTD As Boolean

    Public Property HasInvalidChar As Boolean

    Public Property BaseParentContact As Contact

    Public ReadOnly Property IndirizzoTelematico As String
        Get
            'Eseguo la verifica se l'indirizzo telematico è valorizzato
            Dim origineNode As XmlNode = Me.XmlDoc.SelectSingleNode("//Origine")
            If origineNode IsNot Nothing Then
                Dim indirizzoNode As XmlNode = origineNode.ChildNodes.Cast(Of XmlNode).SingleOrDefault(Function(s) s.Name.Eq("IndirizzoTelematico"))
                If indirizzoNode IsNot Nothing Then
                    Return indirizzoNode.InnerText
                End If
            End If

            Return String.Empty
        End Get
    End Property
#End Region

#Region " Methods "

    ''' <summary>
    ''' Recupera dal nodo "Mittente" i contatti come contatti da rubrica.
    ''' </summary>
    ''' <param name="persist">Indica se salvare o meno i contatti mancanti.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMittenteForAddressBook(persist As Boolean) As List(Of ContactDTO)
        Dim mittente As List(Of Contact) = Me.GetMittenteNew(persist).ToList()
        Dim leaves As List(Of Contact) = Me.GetLeafContacts(mittente)
        Return leaves.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Address)).ToList()
    End Function

    ''' <summary>
    ''' Recupera dal nodo "Mittente" i contatti come contatti da rubrica.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMittenteForAddressBook() As List(Of ContactDTO)
        Return Me.GetMittenteForAddressBook(False)
    End Function

    ''' <summary>
    ''' Recupera dal nodo "Mittente" i contatti come contatti manuali.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMittenteForManual() As List(Of ContactDTO)
        Dim mittente As List(Of Contact) = Me.GetMittenteNew(False).ToList()
        Dim leaves As List(Of Contact) = Me.GetLeafContacts(mittente)
        Return leaves.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Manual)).ToList()
    End Function


    ''' <summary>
    ''' Recupera dal nodo "IndirizzoPostale" un oggetto Address.
    ''' </summary>
    ''' <param name="node">nodo IndirizzoPostale</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAddressFromIndirizzoPostale(node As XmlNode) As Address
        If node Is Nothing OrElse Not node.Name.Equals("IndirizzoPostale") Then
            Return Nothing
        End If

        Dim denominazione As String = node.SelectSingleNodeInnerTextOrDefault("Denominazione")
        If denominazione IsNot Nothing Then
            Return New Address() With {.Address = denominazione}
        End If

        Dim address As New Address()
        address.Address = node.SelectSingleNodeInnerTextOrDefault("Toponimo")
        address.CivicNumber = node.SelectSingleNodeInnerTextOrDefault("Civico")
        address.ZipCode = node.SelectSingleNodeInnerTextOrDefault("CAP")
        address.City = node.SelectSingleNodeInnerTextOrDefault("Comune")
        address.CityCode = node.SelectSingleNodeInnerTextOrDefault("Provincia")

        Dim toponimoDug As String = node.SelectSingleNodeInnerTextOrDefault("Toponimo/@dug")
        If Not String.IsNullOrEmpty(toponimoDug) Then
            Dim placeNames As IList(Of ContactPlaceName) = FacadeFactory.Instance.ContactPlaceNameFacade.GetByDescription(toponimoDug)
            If placeNames.HasSingle() Then
                address.PlaceName = placeNames.First()
            End If
        End If

        Return address
    End Function

    ''' <summary>
    ''' Recupera dal nodo "Amministrazione" un oggetto Contact.
    ''' </summary>
    ''' <param name="node">nodo Amministrazione</param>
    ''' <param name="parent">contatto padre</param>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactFromAmministrazione(node As XmlNode, parent As Contact, persist As Boolean) As Contact
        Dim contact As New Contact() With {.IsActive = 1S}

        contact.Description = node.SelectSingleNodeInnerTextOrDefault("Denominazione")
        contact.Code = node.SelectSingleNodeInnerTextOrDefault("CodiceAmministrazione")
        contact.ContactType = New ContactType(ContactType.Administration)
        contact.Parent = parent

        contact.Address = Me.GetAddressFromIndirizzoPostale(node.SelectSingleNode("IndirizzoPostale"))
        contact.CertifiedMail = node.SelectSingleNodeInnerTextOrDefault("IndirizzoTelematico")
        contact.EmailAddress = contact.CertifiedMail
        contact.TelephoneNumber = node.SelectSingleNodeInnerTextOrDefault("Telefono")
        contact.FaxNumber = node.SelectSingleNodeInnerTextOrDefault("Fax")

        contact = Me.GetInteroperableContact(contact, persist)

        For Each item As XmlNode In node.ChildNodes
            Dim child As Contact = Me.GetContactByXmlNode(item, contact, persist)
            If child Is Nothing Then
                Continue For
            End If

            If contact.Children Is Nothing Then
                contact.Children = New List(Of Contact)
            End If

            If Not contact.Children.Any(Function(c) c.Id.Equals(child.Id)) Then
                contact.Children.Add(child)
            End If
        Next

        Return contact
    End Function

    ''' <summary>
    ''' Recupera dal nodo "AOO" un oggetto Contact.
    ''' </summary>
    ''' <param name="node">nodo Amministrazione</param>
    ''' <param name="parent">contatto padre</param>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactFromAOO(node As XmlNode, parent As Contact, persist As Boolean) As Contact
        Dim contact As New Contact() With {.IsActive = 1S}

        contact.Description = node.SelectSingleNodeInnerTextOrDefault("Denominazione")
        contact.Code = node.SelectSingleNodeInnerTextOrDefault("CodiceAOO")
        contact.ContactType = New ContactType(ContactType.Aoo)
        contact.Parent = parent

        If Not CheckIndirizzoTelematico(New List(Of Contact)) Then
            contact.CertifiedMail = IndirizzoTelematico
        End If
        contact = Me.GetInteroperableContact(contact, persist)

        For Each item As XmlNode In node.ChildNodes
            Dim child As Contact = Me.GetContactByXmlNode(item, contact, persist)
            If child Is Nothing Then
                Continue For
            End If

            If contact.Children Is Nothing Then
                contact.Children = New List(Of Contact)
            End If

            If Not contact.Children.Any(Function(c) c.Id.Equals(child.Id)) Then
                contact.Children.Add(child)
            End If
        Next

        Return contact
    End Function

    ''' <summary>
    ''' Recupera dal nodo "Persona" un oggetto Contact.
    ''' </summary>
    ''' <param name="node">nodo Amministrazione</param>
    ''' <param name="parent">contatto padre</param>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactFromPersona(node As XmlNode, parent As Contact, persist As Boolean) As Contact
        Dim contact As New Contact() With {.IsActive = 1S}

        Dim denominazione As String = node.SelectSingleNodeInnerTextOrDefault("Denominazione")
        If denominazione IsNot Nothing Then
            contact.Description = denominazione
        Else
            Dim cognome As String = node.SelectSingleNodeInnerTextOrDefault("Cognome")
            Dim nome As String = node.SelectSingleNodeInnerTextOrDefault("Nome")

            If Not String.IsNullOrWhiteSpace(cognome) OrElse Not String.IsNullOrWhiteSpace(nome) Then
                contact.Description = String.Format("{0}|{1}", cognome, nome)
            End If
        End If
        contact.Code = node.SelectSingleNodeInnerTextOrDefault("Identificativo")
        contact.ContactType = New ContactType(ContactType.Person)
        contact.Parent = parent

        contact.FiscalCode = node.SelectSingleNodeInnerTextOrDefault("CodiceFiscale")

        contact = Me.GetInteroperableContact(contact, persist)

        For Each item As XmlNode In node.ChildNodes
            Dim child As Contact = Me.GetContactByXmlNode(item, contact, persist)
            If child Is Nothing Then
                Continue For
            End If

            If contact.Children Is Nothing Then
                contact.Children = New List(Of Contact)
            End If

            If Not contact.Children.Any(Function(c) c.Id.Equals(child.Id)) Then
                contact.Children.Add(child)
            End If
        Next

        Return contact
    End Function

    ''' <summary>
    ''' Recupera dal nodo "Ruolo" un oggetto Contact.
    ''' </summary>
    ''' <param name="node">nodo Amministrazione</param>
    ''' <param name="parent">contatto padre</param>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactFromRuolo(node As XmlNode, parent As Contact, persist As Boolean) As Contact
        Dim contact As New Contact() With {.IsActive = 1S}

        contact.Description = node.SelectSingleNodeInnerTextOrDefault("Denominazione")
        contact.Code = node.SelectSingleNodeInnerTextOrDefault("Identificativo")
        contact.ContactType = New ContactType(ContactType.Role)
        contact.Parent = parent

        contact = Me.GetInteroperableContact(contact, persist)

        For Each item As XmlNode In node.ChildNodes
            Dim child As Contact = Me.GetContactByXmlNode(item, contact, persist)
            If child Is Nothing Then
                Continue For
            End If

            If contact.Children Is Nothing Then
                contact.Children = New List(Of Contact)
            End If

            If Not contact.Children.Any(Function(c) c.Id.Equals(child.Id)) Then
                contact.Children.Add(child)
            End If
        Next

        Return contact
    End Function

    ''' <summary>
    ''' Recupera dal nodo "UnitaOrganizzativa" un oggetto Contact.
    ''' </summary>
    ''' <param name="node">nodo Amministrazione</param>
    ''' <param name="parent">contatto padre</param>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactFromUnitaOrganizzativa(node As XmlNode, parent As Contact, persist As Boolean) As Contact
        Dim contact As New Contact() With {.IsActive = 1S}

        contact.Description = node.SelectSingleNodeInnerTextOrDefault("Denominazione")
        contact.Code = node.SelectSingleNodeInnerTextOrDefault("Identificativo")
        contact.ContactType = New ContactType(ContactType.OrganizationUnit)
        contact.Parent = parent

        contact.Address = Me.GetAddressFromIndirizzoPostale(node.SelectSingleNode("IndirizzoPostale"))
        contact.CertifiedMail = node.SelectSingleNodeInnerTextOrDefault("IndirizzoTelematico")
        contact.EmailAddress = contact.CertifiedMail
        contact.TelephoneNumber = node.SelectSingleNodeInnerTextOrDefault("Telefono")
        contact.FaxNumber = node.SelectSingleNodeInnerTextOrDefault("Fax")

        contact = Me.GetInteroperableContact(contact, persist)

        For Each item As XmlNode In node.ChildNodes
            Dim child As Contact = Me.GetContactByXmlNode(item, contact, persist)
            If child Is Nothing Then
                Continue For
            End If

            If contact.Children Is Nothing Then
                contact.Children = New List(Of Contact)
            End If

            If Not contact.Children.Any(Function(c) c.Id.Equals(child.Id)) Then
                contact.Children.Add(child)
            End If
        Next

        Return contact
    End Function

    ''' <summary>
    ''' Recupera da un nodo un oggetto Contact.
    ''' </summary>
    ''' <param name="node">nodo Amministrazione</param>
    ''' <param name="parent">contatto padre</param>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactByXmlNode(node As XmlNode, parent As Contact, persist As Boolean) As Contact
        Select Case node.Name
            Case "Persona"
                Return Me.GetContactFromPersona(node, parent, persist)

            Case "Ruolo"
                Return Me.GetContactFromRuolo(node, parent, persist)

            Case "UnitaOrganizzativa"
                Return Me.GetContactFromUnitaOrganizzativa(node, parent, persist)

        End Select

        Return Nothing
    End Function


    ''' <summary>
    ''' Recupera dalla rubrica il contatto equivalente.
    ''' </summary>
    ''' <param name="contact">contatto recuperato</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetInteroperableContact(contact As Contact, persist As Boolean) As Contact
        Dim found As IList(Of Contact) = Nothing
        Dim parentId As Integer? = Nothing
        If contact.Parent IsNot Nothing Then
            parentId = contact.Parent.Id
        End If

        Select Case DocSuiteContext.Current.ProtocolEnv.PECInteropContactSearch
            Case 1
                ' Ricerca per descrizione.
                found = FacadeFactory.Instance.ContactFacade.GetByDescription(contact.Description, contact.ContactType.Id, parentId)

            Case 2
                ' Ricerca per codice, qualora mancante per descrizione.
                If Not String.IsNullOrWhiteSpace(contact.Code) Then
                    found = FacadeFactory.Instance.ContactFacade.GetContacts(contact.Code, contact.ContactType.Id, 1S)
                End If
                If found.IsNullOrEmpty() Then
                    found = FacadeFactory.Instance.ContactFacade.GetByDescription(contact.Description, contact.ContactType.Id, parentId)
                End If

            Case Else
                ' Ricerca per codice.
                If Not String.IsNullOrWhiteSpace(contact.Code) Then
                    found = FacadeFactory.Instance.ContactFacade.GetContacts(contact.Code, contact.ContactType.Id, 1S)
                End If
        End Select

        If Not found.IsNullOrEmpty() Then
            Dim first As Contact = found.First()
            ' Svuoto i figli per ignorare quelli già presenti.
            first.Children = New List(Of Contact)
            Return first
        End If

        If persist Then
            FacadeFactory.Instance.ContactFacade.Save(contact)
        End If

        Return contact
    End Function

    ''' <summary>
    ''' Recupera i contatti.
    ''' </summary>
    ''' <param name="persist">salva se mancante</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMittenteNew(persist As Boolean) As IEnumerable(Of Contact)
        Dim contacts As IList(Of Contact) = New List(Of Contact)
        CheckIndirizzoTelematico(contacts)

        Dim mittente As XmlNode = Me.XmlDoc.SelectSingleNode("//Mittente")
        If mittente Is Nothing _
            OrElse mittente.ChildNodes Is Nothing OrElse mittente.ChildNodes.Count = 0 Then
            Return New List(Of Contact)()
        End If

        Dim baseContacts As IList(Of Contact) = GetBaseInteropContacts(mittente, persist)
        If baseContacts.Any() Then
            For Each contact As Contact In baseContacts
                If Not contacts.Any(Function(x) x.Id.Equals(contact.Id) AndAlso x.Description.Eq(contact.Description) AndAlso x.Code.Eq(contact.Code)) Then
                    contacts.Add(contact)
                End If
            Next
        End If

        Dim interopContacts As IList(Of Contact) = mittente.ChildNodes.Cast(Of XmlNode).Where(Function(x) Not x.Name.Eq("Amministrazione") AndAlso Not x.Name.Eq("AOO")) _
            .Select(Function(n) Me.GetContactByXmlNode(n, BaseParentContact, persist)).ToList()

        If interopContacts.Any() Then
            For Each contact As Contact In interopContacts
                If Not contacts.Any(Function(x) x.Id.Equals(contact.Id) AndAlso x.Description.Eq(contact.Description) AndAlso x.Code.Eq(contact.Code)) Then
                    contacts.Add(contact)
                End If
            Next
        End If

        Return contacts
    End Function

    ''' <summary>
    ''' Recupera dall'alberatura i soli contatti figli.
    ''' </summary>
    ''' <param name="contacts">ricorsione</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetLeafContacts(contacts As IList(Of Contact)) As List(Of Contact)
        Dim result As New List(Of Contact)
        For Each item As Contact In contacts
            Dim childrens As IList(Of Contact) = Nothing
            Dim contact As Contact = Nothing
            Try
                childrens = item.Children
                If (childrens IsNot Nothing) Then
                    childrens.FirstOrDefault()
                End If
            Catch ex As Exception
                contact = FacadeFactory.Instance.ContactFacade.GetById(item.Id)
                childrens = Nothing
                If (contact.Children.Any()) Then
                    childrens = contact.Children
                End If
            End Try

            If childrens.IsNullOrEmpty() Then
                result.Add(item)
                Continue For
            End If

            Dim recursion As List(Of Contact) = Me.GetLeafContacts(childrens)
            If recursion.IsNullOrEmpty() Then
                Continue For
            End If

            result.AddRange(recursion)
        Next
        Return result
    End Function

#End Region

    ' FG20140704: Ho iniziato a fare refactoring di questa classe anche se resta ancora parecchio lavoro da fare...

    Public Sub LoadDocument()
        Try
            CleanXml()

            XmlDoc = New XmlDocument()
            XmlDoc.XmlResolver = New SignatureDTDResolver(DtdPath)

            Try
                XmlDoc.LoadXml(XmlCleaned)
            Catch ex As Exception
                Throw New DocSuiteException("Errore generico in fase di caricamento file di Segnatura.", ex)
            End Try
        Catch ex As Exception
            _loadingError = ex
        End Try

    End Sub

    Private Sub CleanXml()

        ' Clean XSD
        If XmlCleaned.StartsWith("<Segnatura>") Then
            IsWithoutDTD = True
            XmlCleaned = XmlCleaned.Replace("<Segnatura>", "<?xml version=""1.0"" encoding=""ISO-8859-1""?><!DOCTYPE Segnatura SYSTEM ""Segnatura.dtd""[]><Segnatura versione=""2001-05-07"" xml:lang=""it"">")
        End If

        Dim temp As String = Regex.Replace(XmlCleaned, DocSuiteContext.Current.ProtocolEnv.SegnaturaClearRegex, "_", RegexOptions.Compiled)
        If Not temp.Eq(XmlCleaned) Then
            XmlCleaned = temp
            HasInvalidChar = True
        End If

    End Sub

    Public Function GetOggetto() As String

        Dim node As XmlNode = XmlDoc.SelectSingleNode(DocSuiteContext.Current.ProtocolEnv.InteropOggettoXPath)
        If node Is Nothing Then
            Return String.Empty
        End If
        Return node.InnerText

    End Function

    Public Function GetNomeDocumentoPrincipale() As String

        Dim node As XmlNode = XmlDoc.SelectSingleNode("//Descrizione/Documento/@nome")
        If node Is Nothing Then
            Return String.Empty
        End If
        Return node.InnerText

    End Function

    Public Function GetNumeroRegistrazione() As String

        Dim node As XmlNode = XmlDoc.SelectSingleNode("//Identificatore/NumeroRegistrazione")
        If node Is Nothing Then
            Return String.Empty
        End If
        Return node.InnerText
    End Function


    Public Function GetDataRegistrazione() As DateTime?

        Dim node As XmlNode = XmlDoc.SelectSingleNode("//Identificatore/DataRegistrazione")
        If node Is Nothing OrElse String.IsNullOrEmpty(node.InnerText) Then
            Return Nothing
        End If

        Return Date.Parse(node.InnerText, Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Date
    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei nodi che rappresentano contatti Mittente
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetContactsNodes() As List(Of XmlNode)

        Dim tor As New List(Of XmlNode)

        For Each node As XmlNode In XmlDoc.SelectNodes("//Mittente/Amministrazione")
            tor.Add(node)
        Next

        For Each node As XmlNode In XmlDoc.SelectNodes("//Mittente/AOO")
            tor.Add(node)
        Next

        For Each node As XmlNode In XmlDoc.SelectNodes("//Mittente//UnitaOrganizzativa[IndirizzoPostale]")
            tor.Add(node)
        Next

        For Each node As XmlNode In XmlDoc.SelectNodes("//Mittente//Persona")
            tor.Add(node)
        Next

        For Each node As XmlNode In XmlDoc.SelectNodes("//Mittente//Ruolo")
            tor.Add(node)
        Next

        Return tor

    End Function

    Private Function GetMittente(tagName As String) As Contact
        Dim node As XmlNode = XmlDoc.SelectSingleNode(String.Concat("//Mittente//", tagName))
        If node Is Nothing Then
            Return Nothing
        End If
        Return GetContactOrBuild(node)
    End Function

    Public Sub AddParent(source As Contact, parent As Contact)


        If source.Parent Is Nothing Then
            source.Parent = parent
        Else
            If source.Parent.Description.Eq(parent.Description) Then
                Return
            End If
            AddParent(source.Parent, parent)
        End If

    End Sub

    Public Sub SaveContact(contact As Contact)
        If contact.Id <> 0 Then
            Return ' Contatto già salvato in DB
        End If
        ' Risalgo al primo padre non salvato
        If contact.Parent IsNot Nothing Then
            SaveContact(contact.Parent)
        End If

        FacadeFactory.Instance.ContactFacade.Save(contact)

    End Sub

    Public Function GetMittente(hierarchical As Boolean) As ContactDTO
        ' Se non GERARCHICA restituisco il primo nodo trovato
        ' Altrimenti restituisco quello di più basso livello con annessa gerarchia...

        Dim tor As ContactDTO = Nothing

        ' Cerco PERSONA
        Dim persona As Contact = GetMittente("Persona")
        If persona IsNot Nothing Then
            If hierarchical Then
                ' Nodo da restituire
                tor = New ContactDTO(persona, ContactDTO.ContactType.Address)
            Else
                ' Restituisco direttamente il nodo trovato
                tor = New ContactDTO(persona, ContactDTO.ContactType.Manual)
            End If
        End If

        ' Cerco RUOLO
        Dim ruolo As Contact = GetMittente("Ruolo")
        If ruolo IsNot Nothing Then
            If hierarchical Then
                If tor Is Nothing Then
                    ' PERSONA non trovata
                    tor = New ContactDTO(ruolo, ContactDTO.ContactType.Address)
                Else
                    ' Aggiungo Ruolo come Parent
                    AddParent(tor.Contact, ruolo)
                End If
            Else
                If tor Is Nothing Then
                    ' Restituisco direttamente il nodo trovato
                    ruolo.ContactType = New ContactType(ContactType.Person)
                    tor = New ContactDTO(ruolo, ContactDTO.ContactType.Manual)
                Else
                    ' noop
                    ' Se ho già trovato PERSONA il ruolo va ignorato 
                End If
            End If
        End If

        Dim uo As Contact = GetMittente("UnitaOrganizzativa[IndirizzoPostale]")
        If uo IsNot Nothing Then
            If hierarchical Then
                ' TODO: verificare possibile presenza id nodo padre di tipo UnitaOrganizzativa
                If tor Is Nothing Then
                    ' PERSONA non trovata
                    tor = New ContactDTO(uo, ContactDTO.ContactType.Address)
                Else
                    ' Aggiungo Ruolo come Parent
                    AddParent(tor.Contact, uo)
                End If
            Else
                uo.ContactType = New ContactType(ContactType.Aoo)
                Return New ContactDTO(uo, ContactDTO.ContactType.Manual)
            End If
        End If

        If hierarchical Then
            Dim aoo As Contact = GetMittente("AOO")
            If aoo IsNot Nothing Then
                ' TODO: verificare possibile presenza id nodo padre di tipo UnitaOrganizzativa
                If tor Is Nothing Then
                    ' PERSONA non trovata
                    tor = New ContactDTO(aoo, ContactDTO.ContactType.Address)
                Else
                    ' Aggiungo Ruolo come Parent
                    AddParent(tor.Contact, aoo)
                End If
            End If
        Else
            ' noop
        End If

        Dim amministrazione As Contact = GetMittente("Amministrazione")
        If amministrazione IsNot Nothing Then
            If hierarchical Then
                If tor Is Nothing Then
                    ' PERSONA non trovata
                    tor = New ContactDTO(amministrazione, ContactDTO.ContactType.Address)
                Else
                    ' Aggiungo Ruolo come Parent
                    AddParent(tor.Contact, amministrazione)
                End If
            Else
                If tor Is Nothing Then
                    tor = New ContactDTO(amministrazione, ContactDTO.ContactType.Address)
                Else
                    ' Copio l'indirizzo 
                    CopyAddressInfos(tor.Contact, amministrazione)
                End If
            End If
        End If

        Return tor

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

    Private Function GetContactOrBuild(node As XmlNode) As Contact
        Dim contact As Contact = GetContact(node)

        If contact Is Nothing Then
            ' Contatto non trovato in DB
            contact = ContactBuilder(node)
        End If

        Return contact
    End Function

    Public Function GetContacts() As IList(Of Contact)
        Dim tor As New List(Of Contact)
        For Each node As XmlNode In GetContactsNodes()

            ' Cerco il contatto in DB
            Dim contact As Contact = GetContact(node)

            If contact Is Nothing Then
                ' Contatto non trovato in DB
                contact = ContactBuilder(node)
            End If

            tor.Add(contact)

        Next
        Return tor
    End Function

    Private Function GetContactCode(node As XmlNode) As String
        Select Case node.Name
            Case "Persona"
                Return node.GetIdentificativo()
            Case "Ruolo", "UnitaOrganizzativa"
                Return node.GetIdentificativo()
            Case "AOO"
                Return node.GetCodiceAOO()
            Case "Amministrazione"
                Return node.GetCodiceAmministrazione()
        End Select

        Throw New Exception("XmlNode non riconosciuto: " + node.Name)
    End Function

    Private Function ContactBuilder(node As XmlNode) As Contact
        Dim contact As New Contact
        contact.Description = GetContactKey(node)
        contact.Code = GetContactCode(node)

        Select Case node.Name
            Case "Persona"
                contact.ContactType = New ContactType(ContactType.Person)
                contact.FiscalCode = node.GetCodiceFiscale()
            Case "Ruolo"
                contact.ContactType = New ContactType(ContactType.Role)
            Case "UnitaOrganizzativa"
                contact.ContactType = New ContactType(ContactType.OrganizationUnit)

                contact.Address = GetAddress(node)
                contact.CertifiedMail = node.GetIndirizzoTelematico()
                contact.EmailAddress = contact.CertifiedMail
                contact.TelephoneNumber = node.GetTelefono()
                contact.FaxNumber = node.GetFax()
            Case "AOO"
                contact.ContactType = New ContactType(ContactType.Aoo)
            Case "Amministrazione"
                contact.ContactType = New ContactType(ContactType.Administration)

                contact.Address = GetAddress(node)
                contact.CertifiedMail = node.GetIndirizzoTelematico()
                contact.EmailAddress = contact.CertifiedMail
                contact.TelephoneNumber = node.GetTelefono()
                contact.FaxNumber = node.GetFax()
            Case Else
                Throw New Exception("XmlNode non riconosciuto: " + node.Name)
        End Select

        Return contact

    End Function

    Private Function GetAddress(node As XmlNode) As Address
        Dim address As New Address

        Dim indirizzoPostaleNode As XmlNode = node.SelectSingleNode("IndirizzoPostale")

        If indirizzoPostaleNode IsNot Nothing Then

            Dim denominazione As String = indirizzoPostaleNode.GetDenominazione()

            If Not String.IsNullOrEmpty(denominazione) Then
                address.Address = denominazione
            Else
                address.Address = indirizzoPostaleNode.GetToponimo()
                address.CivicNumber = indirizzoPostaleNode.GetCivico()
                address.ZipCode = indirizzoPostaleNode.GetCAP()
                address.City = indirizzoPostaleNode.GetComune()
                address.CityCode = indirizzoPostaleNode.GetProvincia()

                Dim toponimoDug As String = indirizzoPostaleNode.GetToponimoDug()
                If Not String.IsNullOrEmpty(toponimoDug) Then
                    Dim toponimi As IList(Of ContactPlaceName) = FacadeFactory.Instance.ContactPlaceNameFacade.GetByDescription(toponimoDug)
                    If toponimi IsNot Nothing AndAlso toponimi.Count = 1 Then
                        address.PlaceName = toponimi(0)
                    End If
                End If
            End If
        End If
        Return address
    End Function




    Private Function GetContact(node As XmlNode) As Contact
        Dim nodeKey As String = GetContactKey(node)
        Dim nodeCode As String = GetContactCode(node)
        Dim contactType As Char = GetContactType(node)

        Dim contacts As IList(Of Contact) = Nothing
        If DocSuiteContext.Current.ProtocolEnv.PECInteropContactSearch <> 1 AndAlso
            Not String.IsNullOrEmpty(nodeCode) Then
            contacts = FacadeFactory.Instance.ContactFacade.GetContacts(nodeCode, contactType, 1S)
        End If
        If DocSuiteContext.Current.ProtocolEnv.PECInteropContactSearch = 1 OrElse
            (DocSuiteContext.Current.ProtocolEnv.PECInteropContactSearch = 2 AndAlso contacts.IsNullOrEmpty()) Then
            contacts = FacadeFactory.Instance.ContactFacade.GetContactByDescriptionAndContactType(nodeKey, contactType)
        End If
        If Not contacts.IsNullOrEmpty() Then
            Return contacts(0)
        End If

        ' Non trovato
        Return Nothing

    End Function

    Private Function GetContactType(node As XmlNode) As Char
        Select Case node.Name
            Case "Persona"
                Return ContactType.Person
            Case "Ruolo"
                Return ContactType.Role
            Case "UnitaOrganizzativa"
                Return ContactType.OrganizationUnit
            Case "AOO"
                Return ContactType.Aoo
            Case "Amministrazione"
                Return ContactType.Administration
        End Select

        Throw New Exception("XmlNode non riconosciuto: " + node.Name)

    End Function

    Private Function GetContactKey(node As XmlNode) As String
        Select Case node.Name
            Case "Persona"
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
                Return nodeKey
            Case "Ruolo", "UnitaOrganizzativa", "AOO", "Amministrazione"
                Dim keyNode As XmlNode = node.SelectSingleNode("Denominazione")
                Dim nodeKey As String = ""
                If keyNode IsNot Nothing Then
                    nodeKey = keyNode.InnerText
                End If
                Return nodeKey
        End Select

        Throw New Exception("XmlNode non riconosciuto: " + node.Name)
    End Function

    Private Function CheckIndirizzoTelematico(ByRef contacts As IList(Of Contact)) As Boolean
        'Eseguo la verifica se l'indirizzo telematico è valorizzato
        If Not String.IsNullOrEmpty(IndirizzoTelematico) Then
            Dim certifiedMail As String = IndirizzoTelematico
            contacts = FacadeFactory.Instance.ContactFacade.GetContactWithCertifiedAndClassicEmail(New String() {certifiedMail})
            If contacts.Any() Then
                Return True
            End If
        End If

        Return False
    End Function

    Private Function GetBaseInteropContacts(ByVal mittenteNode As XmlNode, ByVal persist As Boolean) As IList(Of Contact)
        Dim contacts As IList(Of Contact) = New List(Of Contact)
        Dim nodeAmministrazione As XmlNode = mittenteNode.ChildNodes.Cast(Of XmlNode).SingleOrDefault(Function(s) s.Name.Eq("Amministrazione"))
        If nodeAmministrazione IsNot Nothing Then
            Dim contactAmministrazione As Contact = Me.GetContactFromAmministrazione(nodeAmministrazione, BaseParentContact, persist)
            contacts.Add(contactAmministrazione)
            Dim nodeAoo As XmlNode = mittenteNode.ChildNodes.Cast(Of XmlNode).SingleOrDefault(Function(s) s.Name.Eq("AOO"))
            If nodeAoo IsNot Nothing Then
                Dim contactAoo As Contact = Me.GetContactFromAOO(nodeAoo, contactAmministrazione, persist)
                contacts.Add(contactAoo)
            End If
        End If

        Return contacts
    End Function

End Class

Public Module XmlNodeExtensionMethods

    <System.Runtime.CompilerServices.Extension> _
    Public Function IsIndirizzoPostale(source As XmlNode) As Boolean
        Return source.Name.Eq("IndirizzoPostale")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetField(source As XmlNode, tag As String) As String
        Dim node As XmlNode = source.SelectSingleNode(tag)
        If node IsNot Nothing Then Return node.InnerText
        Return Nothing
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetDenominazione(source As XmlNode) As String
        Return source.GetField("Denominazione")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetIdentificativo(source As XmlNode) As String
        Return source.GetField("Identificativo")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetCodiceFiscale(source As XmlNode) As String
        Return source.GetField("CodiceFiscale")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetIndirizzoTelematico(source As XmlNode) As String
        Return source.GetField("IndirizzoTelematico")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetCodiceAOO(source As XmlNode) As String
        Return source.GetField("CodiceAOO")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetCodiceAmministrazione(source As XmlNode) As String
        Return source.GetField("CodiceAmministrazione")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetTelefono(source As XmlNode) As String
        Return source.GetField("Telefono")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetFax(source As XmlNode) As String
        Return source.GetField("Fax")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetToponimo(source As XmlNode) As String
        If Not source.IsIndirizzoPostale Then
            Throw New InvalidOperationException("Campo Toponimo disponibile solo per nodi di tipo IndirizzoPostale")
        End If
        Return source.GetField("Toponimo")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetToponimoDug(source As XmlNode) As String
        If Not source.IsIndirizzoPostale Then
            Throw New InvalidOperationException("Campo ToponimoDug disponibile solo per nodi di tipo IndirizzoPostale")
        End If
        Return source.GetField("Toponimo/@dug")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetCivico(source As XmlNode) As String
        If Not source.IsIndirizzoPostale Then
            Throw New InvalidOperationException("Campo Civico disponibile solo per nodi di tipo IndirizzoPostale")
        End If
        Return source.GetField("Civico")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetCAP(source As XmlNode) As String
        If Not source.IsIndirizzoPostale Then
            Throw New InvalidOperationException("Campo CAP disponibile solo per nodi di tipo IndirizzoPostale")
        End If
        Return source.GetField("CAP")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetComune(source As XmlNode) As String
        If Not source.IsIndirizzoPostale Then
            Throw New InvalidOperationException("Campo Comune disponibile solo per nodi di tipo IndirizzoPostale")
        End If
        Return source.GetField("Comune")
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function GetProvincia(source As XmlNode) As String
        If Not source.IsIndirizzoPostale Then
            Throw New InvalidOperationException("Campo Provincia disponibile solo per nodi di tipo IndirizzoPostale")
        End If
        Return source.GetField("Provincia")
    End Function


End Module
