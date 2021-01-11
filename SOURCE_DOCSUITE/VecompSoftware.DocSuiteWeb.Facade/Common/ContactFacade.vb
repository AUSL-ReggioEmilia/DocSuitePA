Imports System.Xml
Imports System.Linq
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data
Imports NHibernate
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

<ComponentModel.DataObject()>
Public Class ContactFacade
    Inherits CommonFacade(Of Contact, Integer, NHibernateContactDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

    Public Overrides Sub Save(ByRef obj As Contact)
        Dim id As Integer = _dao.GetMaxId()

        obj.Id = id + 1
        obj.CertifiedMail = If(String.IsNullOrEmpty(obj.CertifiedMail), String.Empty, obj.CertifiedMail.Trim())
        obj.EmailAddress = If(String.IsNullOrEmpty(obj.EmailAddress), String.Empty, obj.EmailAddress.Trim())
        'Calcola FullIncrementalPath
        CalculateFullIncremental(obj)

        MyBase.Save(obj)
    End Sub

    Private Sub CalculateFullIncremental(ByRef obj As Contact)
        If obj.Parent Is Nothing Then
            obj.FullIncrementalPath = obj.Id.ToString()
        Else
            obj.FullIncrementalPath = String.Format("{0}|{1}", obj.Parent.FullIncrementalPath, obj.Id.ToString())
        End If
    End Sub
    Private Sub CalculateFullIncremental(ByRef childs As IList(Of Contact))
        If (childs Is Nothing) Then
            Return
        End If
        For Each contact As Contact In childs
            Dim obj As Contact = GetById(contact.Id, False)
            If obj.Parent Is Nothing Then
                obj.FullIncrementalPath = obj.Id.ToString()
            Else
                obj.FullIncrementalPath = String.Format("{0}|{1}", obj.Parent.FullIncrementalPath, obj.Id.ToString())
            End If
            Me.UpdateOnly(obj)
            obj = GetById(contact.Id, False)
            CalculateFullIncremental(obj.Children)
        Next
    End Sub
    Public Function GetRootContact(ByVal searchAll As Boolean, ByVal onlyMyRoles As Boolean, editMode As Boolean,
                                   Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                   Optional excludeParentId As List(Of Integer) = Nothing,
                                   Optional onlyParentId As Integer? = Nothing, Optional excludeRoleRoot As Boolean? = Nothing, Optional procedureType As String = Nothing,
                                   Optional idRole As Integer? = Nothing, Optional roleType As String = Nothing, Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Dim contacts As IList(Of Contact) = New List(Of Contact)

        ' Verifico se devo caricare i nodi di settore
        If DocSuiteContext.Current.ProtocolEnv.RoleContactEnabled AndAlso Not excludeRoleRoot Then
            ' Verifico se sono amministratore di rubrica
            If onlyMyRoles Then
                Dim roles As IList(Of Role) = Factory.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, True)

                If Not roles.IsNullOrEmpty() Then

                    Dim iRoles As List(Of Integer) = roles.Select(Function(f) f.Id).ToList()
                    Dim tmp As IList(Of Contact) = _dao.GetRoleRootContact(iRoles, searchAll, excludeParentIds:=excludeParentId, onlyParentId:=onlyParentId, currentTenant:=currentTenant)
                    For Each ct As Contact In tmp
                        contacts.Add(ct)
                    Next
                End If
            Else
                Dim tmp As IList(Of Contact) = _dao.GetRoleRootContact(searchAll, currentTenant:=currentTenant)
                For Each ct As Contact In tmp
                    contacts.Add(ct)
                Next
            End If

        End If

        If Not editMode OrElse Not onlyMyRoles Then
            Dim roleIds As IList(Of Integer) = Nothing

            Dim temp As IList(Of Contact) = _dao.GetRootContact(searchAll, categoryFascicleRightRoles:=categoryFascicleRightRoles, excludeParentId:=excludeParentId, onlyParentId:=onlyParentId, procedureType:=procedureType, idRole:=idRole, currentTenant:=currentTenant)
            For Each c As Contact In temp
                If contacts.Contains(c) Then
                    Continue For
                End If

                If DocSuiteContext.Current.ProtocolEnv.RoleContactEnabled AndAlso Not excludeRoleRoot Then
                    contacts.Add(c)
                Else
                    If c.RoleRootContact Is Nothing Then
                        contacts.Add(c)
                    End If
                End If
            Next
        End If

        Return contacts

    End Function

    Public Function GetContactByParentId(ByVal parentId As Integer, ByVal searchAll As Boolean,
                                         Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                         Optional excludeParentIds As List(Of Integer) = Nothing,
                                         Optional onlyParentId As Integer? = Nothing,
                                         Optional contactListId As Guid? = Nothing,
                                         Optional procedureType As String = Nothing,
                                         Optional idRole As Integer? = Nothing,
                                         Optional roleType As String = Nothing,
                                         Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Return _dao.GetContactByParentId(parentId, searchAll, categoryFascicleRightRoles:=categoryFascicleRightRoles, excludeParentIds:=excludeParentIds, onlyParentId:=onlyParentId,
                                         contactListId:=contactListId, procedureType:=procedureType, idRole:=idRole, roleType:=roleType, currentTenant:=currentTenant)
    End Function

    Public Function GetContactWithId(ByVal idContactList As Integer()) As IList(Of Contact)
        Return _dao.GetContactWithId(idContactList)
    End Function

    Public Function GetContactWithEmail(ByVal eMailList As String()) As IList(Of Contact)
        Return _dao.GetContactWithEmail(eMailList)
    End Function

    Public Function GetContactWithCertifiedEmail(ByVal eMailList As String()) As IList(Of Contact)
        Return _dao.GetContactWithCertifiedEmail(eMailList)
    End Function

    Public Function GetContactWithCertifiedAndClassicEmail(ByVal eMailList As String()) As IList(Of Contact)
        Return _dao.GetContactWithCertifiedAndClassicEmail(eMailList)
    End Function

    Public Function GetContactByDescription(ByVal description As String, ByVal searchType As NHibernateContactDao.DescriptionSearchType,
                                            ByVal contactRootRoles As List(Of Integer),
                                            Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                            Optional ByVal rootFullIncrementalPath As String = "",
                                            Optional excludeParentId As List(Of Integer) = Nothing,
                                            Optional onlyParentId As Integer? = Nothing,
                                            Optional roleType As String = Nothing) As IList(Of Contact)
        Return GetContactByDescription(description, searchType, False, contactRootRoles, categoryFascicleRightRoles:=categoryFascicleRightRoles, rootFullIncrementalPath:=rootFullIncrementalPath,
                                       excludeParentId:=excludeParentId, onlyParentId:=onlyParentId, roleType:=roleType)
    End Function

    Public Function GetContactByDescription(ByVal description As String, ByVal searchType As NHibernateContactDao.DescriptionSearchType, ByVal searchAll As Boolean,
                                            ByVal contactRootRoles As List(Of Integer),
                                            Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                            Optional ByVal rootFullIncrementalPath As String = "",
                                            Optional excludeParentId As List(Of Integer) = Nothing,
                                            Optional onlyParentId As Integer? = Nothing,
                                            Optional contactListId As Guid? = Nothing,
                                            Optional procedureType As String = Nothing,
                                            Optional idRole As Integer? = Nothing,
                                            Optional roleType As String = Nothing,
                                            Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Return _dao.GetContactByDescription(description, searchType, searchAll, contactRootRoles, categoryFascicleRightRoles:=categoryFascicleRightRoles, rootFullIncrementalPath:=rootFullIncrementalPath,
                                            exludeParentId:=excludeParentId, onlyParentId:=onlyParentId, contactListId:=contactListId, procedureType:=procedureType,
                                            idRole:=idRole, roleType:=roleType, currentTenant:=currentTenant)
    End Function

    Public Function GetContactByFiscalCode(ByVal pFiscalCode As String) As IList(Of Contact)
        Return _dao.GetContactByFiscalCode(pFiscalCode)
    End Function

    Public Function GetContactByDescriptionAndContactType(ByVal description As String, ByVal contactType As Char) As IList(Of Contact)
        Return _dao.GetContactByDescriptionAndContactType(description, contactType)
    End Function

    Public Function GetContactBySearchCode(ByVal searchCode As String, ByVal isActive As Short,
                                           Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                           Optional excludeParentIds As List(Of Integer) = Nothing,
                                           Optional onlyParentId As Integer? = Nothing, Optional contactListId As Guid? = Nothing,
                                           Optional idCategory As Integer? = Nothing, Optional procedureType As String = Nothing, Optional idRole As Integer? = Nothing, Optional roleType As String = Nothing,
                                           Optional currentTenantId? As Guid = Nothing) As IList(Of Contact)
        Dim contactList As IList(Of Contact) = _dao.GetContactBySearchCode(searchCode, isActive, categoryFascicleRightRoles:=categoryFascicleRightRoles, excludeParentId:=excludeParentIds,
                                                                           onlyParentId:=onlyParentId, contactListId:=contactListId, procedureType:=procedureType, idRole:=idRole, roleType:=roleType, currentTenantId:=currentTenantId)
        ''Verifico che tutto il ramo sia active
        If isActive = 1 Then
            Return contactList.Where(Function(f) IsTreeActive(f)).ToList()
        End If
        ''Altrimenti ritorno il risultato calcolato
        Return contactList
    End Function


    Public Function GetContactByIncrementalFather(ByVal incrementalFather As Integer, Optional ByVal isActive As Boolean = False) As IList(Of Contact)
        Return _dao.GetContactByIncrementalFather(incrementalFather, isActive)
    End Function

    ''' <summary> Verifica l'esatezza e correge i FullIncrementalPath di tutti i contatti. </summary>
    Public Function FullIncrementalUtility() As IList(Of Contact)
        Dim modified As New List(Of Contact)
        Using transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
            Try
                Dim contacts As IList(Of Contact) = GetAll()
                For Each contact As Contact In contacts
                    Dim sFullPath As String = String.Empty
                    _dao.GetFullIncrementalPath(contact.Parent, sFullPath)
                    If Not String.IsNullOrEmpty(sFullPath) Then
                        sFullPath = sFullPath & "|"
                    End If
                    sFullPath = sFullPath & contact.Id.ToString()
                    If contact.FullIncrementalPath <> sFullPath Then
                        contact.FullIncrementalPath = sFullPath
                        UpdateWithoutTransaction(contact)
                        modified.Add(contact)
                    End If
                Next
                transaction.Commit()
            Catch exception As ObjectNotFoundException
                transaction.Rollback()
                Throw New DocSuiteException("Errore verifica", String.Format("Contatto non più esistente [{0}]", exception.Message), exception)
            Catch ex As Exception
                transaction.Rollback()
                Throw New DocSuiteException("Errore verifica", "Impossibile calcolare FullIncrementalPath dei contatti", ex)
            End Try
        End Using

        Return modified
    End Function

    Public Function GetContactByGroups(ByVal groups As IList(Of String)) As IList(Of Contact)
        Return _dao.GetContactByGroups(groups)
    End Function

    Public Function GetContactByFullPath(ByVal fullIncremental As String) As IList(Of Contact)
        Return _dao.GetContactByFullPath(fullIncremental)
    End Function

    Public Function GetFullPath(ByVal fullIncremental As String) As String
        Dim treeS As String = String.Empty

        Dim contacts As IList(Of Contact) = GetContactByFullPath(fullIncremental)
        If contacts.Count > 0 Then
            treeS = contacts.Aggregate(treeS, Function(current, contactItem) current + String.Format("{0} \", contactItem.Description))
            treeS = treeS.Remove(treeS.Length - 1, 1)
        End If
        Return treeS
    End Function

    Public Overrides Function IsUsed(ByRef obj As Contact) As Boolean
        If DocSuiteContext.Current.IsProtocolEnabled Then
            If obj.Protocols.Count > 0 Then
                Return True
            End If
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            If obj.Documents.Count > 0 Then
                Return True
            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            'TODO
        End If

        If obj.Children.Count > 0 Then
            Return obj.Children.Aggregate(True, Function(current, c) current AndAlso (c.IsActive = 0S))
        End If

        Return False
    End Function

    Public Function ImportFromXml(ByVal xmlDoc As XmlDocument) As IList(Of ContactDTO)
        Dim listDto As New List(Of ContactDTO)

        'verifica nodo ROOT
        If Not xmlDoc.FirstChild.Name.Eq("ELENCOCONTATTI") Then
            Throw New DocSuiteException("Errore importazione", String.Format("Formato del file non corretto.{0}Manca TAG <ElencoContatti>.{0}Importazione Interrotta", Environment.NewLine))
        End If
        'verifica nodi foglia
        For Each node As XmlNode In xmlDoc.FirstChild.ChildNodes
            If Not node.Name.Eq("SEARCHCODE") Then
                Continue For
            End If

            Dim contacts As IList(Of Contact) = GetContactBySearchCode(node.InnerText, -1)
            Select Case contacts.Count
                Case 0
                    Throw New DocSuiteException("Errore importazione", String.Format("Codice {0} non trovato in tabella contatti.{1}Importazione Interrotta", node.InnerText, Environment.NewLine))
                Case 1
                    Dim contactDto As New ContactDTO()
                    contactDto.Contact = contacts(0)
                    contactDto.Id = contacts(0).Id
                    contactDto.Type = ContactDTO.ContactType.Address
                    listDto.Add(contactDto)
                Case Is > 1
                    Throw New DocSuiteException("Errore importazione", String.Format("Codice {0} non univoco in tabella contatti.{1}Importazione Interrotta", node.InnerText, Environment.NewLine))
            End Select

        Next
        If listDto.Count = 0 Then
            Throw New DocSuiteException("Errore importazione", "Il File non contiene Codici Contatti\nImportazione Interrotta")
        End If

        Return listDto
    End Function

    Public Function GetContacts(ByVal code As String, ByVal contactType As Char?, ByVal isActive As Short?) As IList(Of Contact)
        Return _dao.GetContacts(code, contactType, isActive)
    End Function

    Public Function GetContactMyRoles() As List(Of Integer)
        Dim sRole As List(Of Integer) = Nothing
        If DocSuiteContext.Current.ProtocolEnv.RoleContactEnabled Then
            Dim roles As IList(Of Role) = FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, True)
            If roles Is Nothing Then
                roles = New List(Of Role)
            End If
            sRole = roles.Select(Function(f) f.Id).ToList()
        End If
        Return sRole
    End Function


    ''' <summary> Se necessario, controlla se il contatto è duplicato. </summary>
    ''' <param name="contact">Contatto da verificare</param>
    ''' <exception cref="DocSuiteException"> Se il contatto è già presente in rubrica. </exception>
    Public Sub DuplicationCheck(ByVal contact As Contact)
        ' se la verifica 
        If Not DocSuiteContext.Current.ProtocolEnv.RubricaCheckDuplicati Then
            Exit Sub
        End If

        Dim existingContacts As IList(Of Contact) = Nothing
        Dim sRole As List(Of Integer) = FacadeFactory.Instance.ContactFacade.GetContactMyRoles()
        Select Case contact.ContactType.Id
            Case ContactType.Aoo, ContactType.Person
                If Not String.IsNullOrEmpty(contact.FiscalCode) Then
                    existingContacts = GetContactByFiscalCode(contact.FiscalCode)
                End If
                If existingContacts.IsNullOrEmpty() Then
                    existingContacts = GetContactByDescription(contact.Description, NHibernateContactDao.DescriptionSearchType.Equal, True, sRole)
                End If
            Case Else
                existingContacts = GetContactByDescription(contact.Description, NHibernateContactDao.DescriptionSearchType.Equal, True, sRole)
        End Select

        If Not existingContacts.IsNullOrEmpty() Then

            Throw New DocSuiteException("Controllo Contatti", String.Format("Contatto [{0}] già presente in rubrica.", existingContacts.Item(0).DescriptionFormatByContactType))
        End If

    End Sub

    Public Function ContactDuplicationCheck(ByVal contact As Contact) As Boolean
        If Not DocSuiteContext.Current.ProtocolEnv.RubricaCheckDuplicati Then
            Return False
        End If

        Dim existingContacts As IList(Of Contact) = Nothing
        Dim sRole As List(Of Integer) = FacadeFactory.Instance.ContactFacade.GetContactMyRoles()
        Select Case contact.ContactType.Id
            Case ContactType.Aoo, ContactType.Person
                If Not String.IsNullOrEmpty(contact.FiscalCode) Then
                    existingContacts = GetContactByFiscalCode(contact.FiscalCode)
                End If
                If existingContacts.IsNullOrEmpty() Then
                    existingContacts = GetContactByDescription(contact.Description, NHibernateContactDao.DescriptionSearchType.Equal, True, sRole)
                End If
            Case Else
                existingContacts = GetContactByDescription(contact.Description, NHibernateContactDao.DescriptionSearchType.Equal, True, sRole)
        End Select

        If Not existingContacts.IsNullOrEmpty() Then
            Return False
        End If
        Return True

    End Function

#Region "Hierarchy Search Optimizations"
    Public Function GetHierarchyOfContacts(ByRef contactList As List(Of Contact)) As List(Of List(Of Contact))
        Dim hierarchy As New List(Of List(Of Contact))
        Dim level As Integer = GetContactHierarchy(contactList, hierarchy)
        hierarchy.Add(contactList)
        While level > 0
            OrderContactsResult(hierarchy(level), hierarchy(level - 1))
            level -= 1
        End While

        Return hierarchy
    End Function

    Protected Function GetContactHierarchy(ByRef contactList As List(Of Contact), ByRef hierarchy As List(Of List(Of Contact))) As Integer
        Dim lst As IList(Of Contact) = _dao.GetContactsForContactList(ConvertsContactToIds(contactList))
        If Not lst.IsNullOrEmpty() Then
            hierarchy.Insert(0, lst.ToList())
            GetContactHierarchy(lst.ToList(), hierarchy)
        End If

        Return hierarchy.Count
    End Function

    Protected Function ConvertsContactToIds(ByRef contacts As List(Of Contact)) As Integer()
        Return (From contact In contacts Where contact.Parent IsNot Nothing Select contact.Parent.Id).ToArray()
    End Function

    Protected Sub OrderContactsResult(ByRef masterList As List(Of Contact), ByRef toOrderList As List(Of Contact))
        Dim resContact As Contact
        Dim tmpList As New List(Of Contact)
        Dim masterToRemove As New List(Of Contact)

        For Each contact As Contact In masterList
            If contact Is Nothing Then
                Continue For
            End If

            If contact.Parent IsNot Nothing Then
                Dim predicate As New ContactPredicate(contact)
                resContact = toOrderList.Find(New Predicate(Of Contact)(AddressOf predicate.CompareParentId))
                If Not tmpList.Contains(resContact) Then
                    tmpList.Add(resContact)
                End If
            Else
                tmpList.Add(contact)
                masterToRemove.Add(contact)
            End If

        Next
        For Each c As Contact In masterToRemove
            masterList.Remove(c)
        Next
        toOrderList = tmpList
    End Sub

    Public Sub DeleteSameDTO(ByRef sourceArray As IList(Of ContactDTO), ByRef destArray As List(Of ContactDTO))
        For i As Integer = sourceArray.Count - 1 To 0 Step -1
            Dim predicate As New ContactPredicate(sourceArray(i))
            Dim destIndex As Integer = destArray.FindIndex(New Predicate(Of ContactDTO)(AddressOf predicate.CompareContactDtoId))
            If destIndex <> -1 Then
                destArray.RemoveAt(destIndex)
                sourceArray.RemoveAt(i)
            End If
        Next
    End Sub

#End Region

    ''' <summary> Formatta un contatto per la sua visualizzazione nelle tabelle grafiche web </summary>
    ''' <param name="contact">Il contatto da formattare</param>
    ''' <param name="code">Identifica se utilizzare il codice del contatto (e il codice di ricerca)</param>
    Public Shared Function FormatContact(ByRef contact As Contact, Optional ByVal code As Boolean = True) As String
        Dim text As String = String.Empty
        If code Then
            If Not String.IsNullOrEmpty(contact.Code) Then
                text = String.Format("{0} - {1}", contact.Code, contact.SearchCode)
            Else
                text = contact.SearchCode
            End If
            If Not String.IsNullOrEmpty(text) Then
                text = String.Format(" ({0})", text)
            End If
        End If
        text = Replace(contact.Description, "|", " ") & text

        Return text
    End Function

    ''' <summary> Funzione ricorsiva utilizzata per calacolare la grafica dei contatti per l'esportazione in PDF </summary>
    ''' <param name="contacts">Lista di contatti o contatto semplice utilizzato per ricavare le informazioni</param>
    ''' <param name="s">Stringa passata per riferimento che viene caricata progressivamente con l'esportazione del contatto</param>
    Public Shared Sub FormatContacts(ByVal contacts As Object, ByRef s As String, Optional ByVal communicationType As String = "")
        ' TODO: trasformare s in uno StringBuilder
        Dim spazio As String = ""
        If TypeOf contacts Is IList(Of ProtocolContactManual) Then
            For Each contatto As ProtocolContactManual In DirectCast(contacts, IList(Of ProtocolContactManual))
                If Not contatto.Contact.Parent Is Nothing Then
                    FormatContacts(contatto.Contact.Parent, s)
                End If
                If s <> "" Then s &= vbCrLf
                spazio = spazio.PadLeft(StringHelper.CountChar(contatto.Contact.FullIncrementalPath, "|"c), "."c)
                s &= spazio & Replace(contatto.Contact.Description, "|", " ")
            Next
        ElseIf TypeOf contacts Is Contact Then
            If Not contacts.Parent Is Nothing Then
                FormatContacts(contacts.Parent, s)
            End If
            If s <> "" Then s &= vbCrLf
            spazio = spazio.PadLeft(StringHelper.CountChar(CType(contacts.FullIncrementalPath, String), "|"c), "."c)
            s &= spazio & Replace(CType(contacts.Description, String), "|", " ")
        ElseIf TypeOf contacts Is IList(Of ProtocolContact) Then
            For Each contatto As ProtocolContact In DirectCast(contacts, IList(Of ProtocolContact))
                If Not contatto.Contact.Parent Is Nothing Then
                    FormatContacts(contatto.Contact.Parent, s)
                End If
                If s <> "" Then s &= vbCrLf
                spazio = spazio.PadLeft(StringHelper.CountChar(contatto.Contact.FullIncrementalPath, "|"c), "."c)
                s &= spazio & Replace(contatto.Contact.Description, "|", " ")
            Next
        ElseIf TypeOf contacts Is IList(Of ResolutionContact) Then
            For Each contatto As ResolutionContact In DirectCast(contacts, IList(Of ResolutionContact))
                Dim s1 As String = String.Empty
                If String.IsNullOrEmpty(communicationType) Or contatto.ComunicationType = communicationType Then
                    spazio = spazio.PadLeft(StringHelper.CountChar(contatto.Contact.FullIncrementalPath, "|"c), "."c)
                    s1 &= spazio & Replace(contatto.Contact.Description, "|", " ")
                    If contatto.Contact.Parent IsNot Nothing Then FormatContacts(contatto.Contact.Parent, s1)
                End If
                If s <> "" Then s &= "#"
                s &= s1
            Next
        Else
            spazio = spazio.PadLeft(StringHelper.CountChar(CType(contacts.FullIncrementalPath, String), "|"c), "."c)
            s = spazio & Replace(CType(contacts.Description, String), "|", " ") & "#" & s
            If Not contacts.Parent Is Nothing Then FormatContacts(contacts.Parent, s)
        End If
    End Sub

    ''' <summary> Verifica se una gerarchia di contatti è attiva oppure no </summary>
    ''' <param name="contact">Foglia iniziale da verificare</param>
    Private Function IsTreeActive(ByVal contact As Contact) As Boolean
        ''Se sono arrivato all'ultimo nodo ritorno il suo valore
        If contact.Parent Is Nothing Then
            Return contact.IsActive = 1
        End If
        ''Altrimenti metto in And il valore corrente con quello ritornato dalla ricorsione
        Return contact.IsActive = 1 AndAlso IsTreeActive(contact.Parent)
    End Function

    Public Function GetByDescription(description As String, contactType As Char, parentId As Integer?) As IList(Of Contact)
        Return _dao.GetByDescription(description, contactType, parentId)
    End Function
    Public Function GetLikeDescription(description As String, contactType As Char, parentId As Integer?) As IList(Of Contact)
        Return _dao.GetLikeDescription(description, contactType, parentId)
    End Function

    Public Function GetByFiscalCodes(fiscalCodes As ICollection(Of String), contactType As Char, parentId As Integer?) As IList(Of Contact)
        Return _dao.GetByFiscalCodes(fiscalCodes, contactType, parentId)
    End Function

    Public Function CreateDTO(source As ProtocolContactIssue) As ContactDTO
        If source Is Nothing Then
            Return Nothing
        End If

        Dim dto As New ContactDTO()
        dto.Contact = source.Contact
        dto.Type = ContactDTO.ContactType.Address

        Return dto
    End Function

    Public Function GetByMail(mailAddress As String) As IList(Of Contact)
        Return _dao.GetByMail(mailAddress)
    End Function

    Public Sub CreateFromDto(ByRef contact As Contact, dto As API.IContactDTO)
        CreateFromDto(contact, dto, Nothing)
    End Sub

    Public Sub CreateFromDto(ByRef contact As Contact, dto As API.IContactDTO, ByVal parent As Contact)
        If dto IsNot Nothing AndAlso Not String.IsNullOrEmpty(dto.Description) AndAlso Not String.IsNullOrWhiteSpace(dto.Description) Then
            contact.Code = dto.Code
            contact.SearchCode = dto.Code
            contact.ContactType = New ContactType(ContactType.Person)
            contact.CertifiedMail = dto.EmailAddress
            contact.EmailAddress = dto.EmailAddress
            contact.Description = dto.Description
            If dto.BirthDate.HasValue AndAlso Not dto.BirthDate.Value.Equals(DateTime.MinValue) Then
                contact.BirthDate = dto.BirthDate
            End If

            contact.Address = New Address()
            contact.Address.Address = dto.Address
            contact.Address.City = dto.City
            contact.Address.CityCode = dto.CityCode
            contact.Address.CivicNumber = dto.CivicNumber
            contact.Address.ZipCode = dto.ZipCode

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

            Save(contact)
        End If
    End Sub

    Public Function IsChildContact(parentId As Integer, childId As Integer) As Boolean
        Dim childContact As Contact = GetById(childId)
        If childContact Is Nothing Then
            Return False
        End If

        Dim fullIncrementalPaths As String() = childContact.FullIncrementalPath.Split("|"c)
        Return fullIncrementalPaths.Any(Function(x) x.Eq(parentId.ToString()))
    End Function

    ''' <summary> Restituisce un oggetto normalizzato per la serializzazione </summary>
    ''' <remarks> I campi sono troncati come sul DB </remarks>
    Public Shared Function CreateFromIpa(ipa As IPA) As Contact
        Dim contact As New Contact
        contact.EmailAddress = StringHelper.Truncate(ipa.Mail, 256)
        contact.CertifiedMail = StringHelper.Truncate(ipa.Mail, 256)
        contact.Description = ipa.Description
        contact.Address = New Address
        With contact.Address
            .ZipCode = StringHelper.Truncate(ipa.PostalCode, 20)
            .CityCode = StringHelper.Truncate(ipa.Provincia, 2)
            .Address = StringHelper.Truncate(ipa.Indirizzo, 60)
        End With
        contact.TelephoneNumber = StringHelper.Truncate(ipa.TelephoneNumber, 50)
        contact.Note = StringHelper.Truncate(ipa.ADSPath, 255)
        contact.ContactType = New ContactType(ContactType.Ipa)
        Return contact
    End Function

    Public Sub Move(toMoveContact As Contact, destinationContact As Contact)

        destinationContact.Children.Add(toMoveContact)
        toMoveContact.Parent = destinationContact
        CalculateFullIncremental(toMoveContact)
        Me.UpdateOnly(destinationContact)
        Me.UpdateOnly(toMoveContact)
        toMoveContact = GetById(toMoveContact.Id)
        CalculateFullIncremental(toMoveContact.Children)
    End Sub

    Public Function GetByList(contactListId As Guid, showAll As Boolean, roleContactIds As List(Of Integer), excludeParentalIds As List(Of Integer), onlyParentId As Integer?) As IList(Of Contact)
        Return _dao.GetByList(contactListId, showAll, roleContactIds, excludeParentalIds, onlyParentId)
    End Function

    Public Function GetContactByRole(ByVal searchCode As String, ByVal isActive As Short, Optional parentId As Integer? = Nothing, Optional idRole As Integer? = Nothing) As IList(Of Contact)
        Dim contactList As IList(Of Contact) = _dao.GetContactByRole(searchCode, isActive, parentId:=parentId, idRole:=idRole)
        Return contactList
    End Function

    Public Function GetByIdRole(ByVal idRole As Integer) As Contact
        Dim contact As Contact = _dao.GetByIdRole(idRole)
        Return contact
    End Function

    Public Function GetContactByIncrementalFatherAndSearchCode(ByVal incrementalFather As Integer, ByVal searchCode As String, ByVal isActive As Boolean) As Contact
        Return _dao.GetContactByIncrementalFatherAndSearchCode(incrementalFather, searchCode, isActive)
    End Function

    Public Sub ActivateContact(contact As Contact)
        ActivateContact(contact, False)
    End Sub

    Public Sub ActivateContact(contact As Contact, activateAllChildren As Boolean)
        ChangeContactActiveState(contact, True, activateAllChildren)
    End Sub

    Public Sub DisableContact(contact As Contact)
        DisableContact(contact, False)
    End Sub

    Public Sub DisableContact(contact As Contact, disableAllChildren As Boolean)
        ChangeContactActiveState(contact, False, disableAllChildren)
    End Sub

    Private Sub ChangeContactActiveState(contact As Contact, isActive As Boolean, recursiveChildren As Boolean)
        CommonTransactionalActions(Sub() _dao.BatchChangeContactActiveState(contact, isActive, recursiveChildren),
                                   Sub()
                                       _dao.ConnectionName = ReslDB
                                       _dao.BatchChangeContactActiveState(contact, isActive, recursiveChildren)
                                   End Sub,
                                   Sub()
                                       _dao.ConnectionName = DocmDB
                                       _dao.BatchChangeContactActiveState(contact, isActive, recursiveChildren)
                                   End Sub)
    End Sub

    Public Sub Clone(contactToClone As Contact, name As String)
        CommonTransactionalSingleAction(Sub() RecursiveClone(New List(Of Contact) From {contactToClone}, contactToClone.Parent, name))
    End Sub

    Private Sub RecursiveClone(contactsToClone As ICollection(Of Contact), parentContact As Contact, Optional newName As String = "")
        If contactsToClone.IsNullOrEmpty() Then
            Exit Sub
        End If

        Dim contactSaved As Contact
        Dim ochartContact As OChartItemContact
        Dim ochartContactItems As ICollection(Of OChartItem)
        For Each contactToClone As Contact In contactsToClone.Where(Function(f) f.IsActive = 1)
            contactSaved = CloneContactRelations(contactToClone, parentContact, newName)
            ochartContactItems = FacadeFactory.Instance.OChartItemFacade.GetByContact(contactToClone)
            If Not ochartContactItems.IsNullOrEmpty() Then
                For Each ochartContactItem As OChartItem In ochartContactItems.Where(Function(f) f.Enabled)
                    ochartContact = New OChartItemContact() With {.Item = ochartContactItem, .Contact = contactSaved}
                    FacadeFactory.Instance.OChartItemContactFacade.SaveWithoutTransaction(ochartContact)
                Next
            End If

            RecursiveClone(contactToClone.Children, contactSaved)
        Next
    End Sub

    Private Function CloneContactRelations(contactToClone As Contact, parentContact As Contact, Optional newName As String = "") As Contact
        Dim contactToSave As Contact = InitializeNewInstanceFromExistingContact(contactToClone)
        contactToSave.Id = _dao.GetMaxId() + 1
        If Not String.IsNullOrEmpty(newName) Then
            contactToSave.Description = newName
        End If

        If contactToClone.ContactType IsNot Nothing Then
            contactToSave.ContactType = FacadeFactory.Instance.ContactTypeFacade.GetById(contactToClone.ContactType.Id)
        End If

        If contactToClone.StudyTitle IsNot Nothing Then
            contactToSave.StudyTitle = FacadeFactory.Instance.ContactTitleFacade.GetById(contactToClone.StudyTitle.Id)
        End If

        If contactToClone.Role IsNot Nothing Then
            contactToSave.Role = FacadeFactory.Instance.RoleFacade.GetById(contactToClone.Role.Id)
        End If

        If contactToClone.RoleRootContact IsNot Nothing Then
            contactToSave.RoleRootContact = FacadeFactory.Instance.RoleFacade.GetById(contactToClone.RoleRootContact.Id)
        End If

        If parentContact IsNot Nothing Then
            contactToSave.Parent = parentContact
        End If

        CalculateFullIncremental(contactToSave)
        SaveWithoutTransaction(contactToSave)
        Return contactToSave
    End Function

    Public Shared Function InitializeNewInstanceFromExistingContact(contact As Contact) As Contact
        Dim newInstanceContact As Contact = New Contact With {
            .IsActive = Convert.ToInt16(False),
            .ActiveFrom = contact.ActiveFrom,
            .ActiveTo = contact.ActiveTo,
            .Address = contact.Address,
            .BirthDate = contact.BirthDate,
            .BirthPlace = contact.BirthPlace,
            .CertifiedMail = contact.CertifiedMail,
            .Code = contact.Code,
            .Description = contact.Description,
            .EmailAddress = contact.EmailAddress,
            .FaxNumber = contact.FaxNumber,
            .FiscalCode = contact.FiscalCode,
            .isLocked = contact.isLocked,
            .isNotExpandable = contact.isNotExpandable,
            .Note = contact.Note,
            .RoleUserIdRole = contact.RoleUserIdRole,
            .SearchCode = contact.SearchCode,
            .TelephoneNumber = contact.TelephoneNumber
        }
        Return newInstanceContact
    End Function
End Class

