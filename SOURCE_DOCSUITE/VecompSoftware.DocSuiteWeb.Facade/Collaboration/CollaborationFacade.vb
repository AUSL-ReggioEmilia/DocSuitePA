Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Net.Mail
Imports System.Text
Imports System.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.Core.Command
Imports VecompSoftware.Core.Command.CQRS.Commands.Entities.Collaborations
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Command.CQRS.Commands.Entities.Collaborations
Imports VecompSoftware.Services.Logging

<ComponentModel.DataObject()>
Public Class CollaborationFacade
    Inherits BaseProtocolFacade(Of Collaboration, Integer, NHibernateCollaborationDao)

#Region " Fields "
    Private _commandUpdateFacade As CommandFacade(Of ICommandDeleteCollaboration)
#End Region

#Region " Properties "

    Public Overrides ReadOnly Property LoggerName As String
        Get
            Return "Collaboration"
        End Get
    End Property

    Private ReadOnly Property CommandUpdateFacade As CommandFacade(Of ICommandDeleteCollaboration)
        Get
            If _commandUpdateFacade Is Nothing Then
                _commandUpdateFacade = New CommandFacade(Of ICommandDeleteCollaboration)
            End If
            Return _commandUpdateFacade
        End Get
    End Property
#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region


#Region " Insert/Update/Delete Collaboration "
    ''' <summary> Inserisce un documento principale in biblos SSE la collaborazione è in stato Bozza. </summary>
    Public Sub BiblosInsert(ByVal collNumber As Integer, ByVal stream As Byte(), ByVal documentName As String, ByVal username As String)
        Dim coll As Collaboration = _dao.GetById(collNumber, False)
        ' Validazione
        If Not coll.IdStatus.Eq(CollaborationStatusType.BZ.ToString()) Then
            Throw New InvalidOperationException("Attenzione il documento può essere inserito solo se la collaborazione è in stato Bozza")
        End If

        ' Verifico che il primo documento aggiunto sia il Documento Principale, ed evito che esso possa essere duplicato
        Dim lstCollVersioning As IList(Of CollaborationVersioning) = Factory.CollaborationVersioningFacade.GetLastVersionings(coll)
        If lstCollVersioning IsNot Nothing AndAlso lstCollVersioning.Count <> 0 Then
            If lstCollVersioning(0).CollaborationIncremental = 0S Then
                Throw New InvalidOperationException("Attenzione la Collaborazione possiede già il documento principale")
            End If
        End If

        ' Ottengo la signature
        Dim signature As String = GetSignature(collNumber, DateTime.Now, Nothing)

        ' Ottengo la location dove inserire il documento

        Dim doc As New MemoryDocumentInfo(stream, documentName)
        doc.Signature = signature

        Dim location As Location = Factory.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.CollaborationLocation)
        Dim archivedBiblosChainId As Integer = doc.ArchiveInBiblos(location.ProtBiblosDSDB).BiblosChainId
        Factory.CollaborationVersioningFacade.InsertDocument(collNumber, archivedBiblosChainId, documentName, username, username)
    End Sub

    ''' <summary> Se la collaborazione e in stato Bozza (BZ) la mette in stato IN per renderla utilizzabile dai firmatari. </summary>
    ''' <param name="collNumber">Identificativo della collaborazione</param>
    Public Sub StartCollaboration(collNumber As Integer, idTenantAOO As Guid)
        Dim coll As Collaboration = GetById(collNumber)

        If coll Is Nothing Then
            Throw New ArgumentException("Collaborazione non presente, verificare l'identificativo della collaborazione", "collNumber")
        End If

        If Not coll.IdStatus.Eq(CollaborationStatusType.BZ.ToString()) Then
            Throw New InvalidOperationException("Attenzione la Collaborazione non è in stato bozza")
        End If

        Dim lstCollVersioning As IList(Of CollaborationVersioning) = Factory.CollaborationVersioningFacade.GetLastVersionings(coll)
        If lstCollVersioning Is Nothing OrElse lstCollVersioning.Count = 0 Then
            Throw New InvalidOperationException("Attenzione la Collaborazione non possiede il documento principale")
        End If

        coll.IdStatus = CollaborationStatusType.IN.ToString()
        UpdateOnly(coll)

        'Mando la mail
        SendMail(coll, coll.IdStatus, idTenantAOO)
    End Sub

    Private Function GetCollaborationContactFromAD(account As String, errorMessage As String) As CollaborationContact
        Dim user As AccountModel = CommonAD.GetAccount(account)
        If user Is Nothing Then
            Throw New Exception(String.Format("Non è stato possibile caricare l'utente {0} come destinatario {1} in quanto non trovato nel dominio ", account, errorMessage))
        End If
        Return New CollaborationContact(user.GetFullUserName(), Nothing, user.Name, user.Email, True)
    End Function

    ''' <summary> Inserisce i metadati di una nuova collaborazione ed eventualmente dati aggiuntivi per la successiva gestione. </summary>
    ''' <returns> Restituisce il numero della collaborazione come intero. </returns>
    Public Function InsertCollaboration(collaborazioneXml As CollaborationXML, username As String, idTenantAOO As Guid) As Int32
        ' Verifico la modalità di inserimento almeno un firmatario deve sempre essere presente
        If collaborazioneXml.Signers.Count = 0 Then
            Throw New ArgumentException("Attenzione inserire almeno un firmatario", "collaborazioneXml")
        End If

        If String.IsNullOrEmpty(collaborazioneXml.Type) Then
            Throw New ArgumentException("Attenzione inserire la tipologia di collaborazione", "collaborazioneXml")
        End If

        If String.IsNullOrEmpty(collaborazioneXml.Priority) Then
            Throw New ArgumentException("Attenzione inserire la priorità della collaborazione", "collaborazioneXml")
        End If

        ' Recupero il numero progressivo
        Dim progressivo As Integer = FacadeFactory.Instance.IncrementalFacade.GetFor(Of Collaboration)().Incremental.Value
        If progressivo <= 0 Then
            Throw New Exception("Errore in calcolo progressivo")
        End If

        ' Recupero la location delle collaborazioni per il database della tipologia indicata
        Dim locFacade As New LocationFacade(_dbName)
        Dim loc As Location = locFacade.GetById(DocSuiteContext.Current.ProtocolEnv.CollaborationLocation)
        If loc.ProtBiblosDSDB Is Nothing Then
            Throw New Exception("Inserire il valore ProtBiblosDSDB nella tabella Location per il DB: ")
        End If

        ' Creo la collaborazione
        Dim coll As Collaboration = New Collaboration()
        coll.Id = progressivo
        coll.DocumentType = collaborazioneXml.Type
        coll.IdPriority = collaborazioneXml.Priority

        Dim memorandumDate As Date
        If Date.TryParseExact(collaborazioneXml.ReminderDate, "d/M/yyyy", Nothing, DateTimeStyles.None, memorandumDate) Then
            coll.MemorandumDate = memorandumDate
        Else
            coll.MemorandumDate = Nothing
        End If

        coll.CollaborationObject = collaborazioneXml.Subject

        coll.Note = collaborazioneXml.Notes
        coll.Location = loc

        coll.RegistrationDate = DateTimeOffset.UtcNow
        coll.RegistrationUser = username
        coll.RegistrationName = username
        coll.RegistrationEMail = FacadeFactory.Instance.UserLogFacade.EmailOfUser(username, True)
        coll.LastChangedUser = username
        coll.LastChangedDate = DateTimeOffset.UtcNow

        'TODO: Da rivedere con gestione corretta del template
        Dim templateName As String = collaborazioneXml.Type
        Select Case coll.DocumentType
            Case CollaborationDocumentType.P.ToString()
                templateName = "Protocollo"
                If Not String.IsNullOrEmpty(collaborazioneXml.TemplateName) Then
                    templateName = collaborazioneXml.TemplateName
                End If
            Case CollaborationDocumentType.U.ToString()
                templateName = "Uoia"
            Case CollaborationDocumentType.A.ToString()
                templateName = FacadeFactory.Instance.ResolutionTypeFacade.DeterminaCaption
            Case CollaborationDocumentType.D.ToString()
                templateName = FacadeFactory.Instance.ResolutionTypeFacade.DeliberaCaption
        End Select
        coll.TemplateName = templateName

        ' Stato Collaborazione - BOZZA
        coll.IdStatus = CollaborationStatusType.BZ.ToString()

        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Try
            unitOfWork.BeginTransaction()
            SaveWithoutTransaction(coll)
            FacadeFactory.Instance.CollaborationLogFacade.Insert(coll, "Restituzione della collaborazione")

            ' Addetti protocollazione
            Dim collaborationUsers As List(Of CollaborationContact) = New List(Of CollaborationContact)

            ' Firmatari
            Dim collaborationSigns As List(Of CollaborationContact) = New List(Of CollaborationContact)

            ' Estrapolazione firmatari e addetti protocollazione -
            ' 20160906 : FL -> vices è un tipo di firmatario che abbiamo introdotto nella gestione per FALCK. 
            ' Attualmente gli ignoriamo ma sono i firmatari vicari che entrerebbero in gioco se la segreteria fa il cambio di responsabile.
            ' quindi vanno in estensione oltre ai normali ruoli di firma sui settori
            For Each item As Signer In collaborazioneXml.Signers.Where(Function(f) String.IsNullOrEmpty(f.Type) OrElse Not f.Type.Eq("vices"))
                Dim collaborationSign As CollaborationContact = Nothing

                If Not String.IsNullOrEmpty(item.UserName) AndAlso Not item.UserName.Contains("\") Then
                    item.UserName = String.Concat(DocSuiteContext.Current.CurrentTenant.DomainName, "\", item.UserName)
                End If

                If item.Role <> 0 Then
                    '' aggiungo l'utente come firmatario
                    '' Calcolo il destinatario di firma tramite userName leggendo su DB
                    '' Vincolo base: compilazione della descrizione.
                    '' L'ordinamento è fatto a monte per e-mail in modo tale che, se presente la mail il primo risultato l'avrà
                    collaborationSign = Factory.RoleUserFacade.GetByAccountsAndNotType(item.UserName, Nothing, True) _
                        .Where(Function(ru) Not String.IsNullOrEmpty(ru.Description)) _
                        .Select(Function(ru1) New CollaborationContact(ru1.Account, Convert.ToInt16(ru1.Id), ru1.Description, ru1.Email, item.SignRequired)) _
                        .FirstOrDefault()

                    If collaborationSign Is Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.CorporateAcronym.ContainsIgnoreCase("AUSLRE") Then
                        collaborationSign = GetCollaborationContactFromAD(item.UserName, "di firma")
                    End If

                    '' Ho già il settore addetto alla protocollazione, effettuo le verifiche di esistenza
                    Dim role As Role = Factory.RoleFacade.GetById(item.Role)
                    Dim roleUsers As IList(Of RoleUser) = Factory.RoleUserFacade.GetByRoleIdAndAccount(item.Role, item.UserName, True)
                    If (roleUsers.IsNullOrEmpty() AndAlso DocSuiteContext.Current.ProtocolEnv.CorporateAcronym.ContainsIgnoreCase("AUSLRE")) Then
                        FileLogger.Warn(LoggerName, String.Concat("RoleUsers.IsNullOrEmpty() AND AUSLRE - ", item.Role, "-", Convert.ToInt16(item.Role)))
                        collaborationUsers.Add(New CollaborationContact(Nothing, Convert.ToInt16(item.Role), role.Name, role.EMailAddress, True))
                    End If

                    '' Se non trovo corrispondenza significa che l'utente cercato non è presente nel settore indicato, quindi ritorno errore
                    If Not roleUsers.IsNullOrEmpty() Then
                        '' Se sono qui significa che ho 1 o 2 risultati (2 nel caso in cui sia impostato sia come firmatario principale che come vice)
                        '' Carico automaticamente la segreteria impostando il Role ricevuto
                        FileLogger.Warn(LoggerName, String.Concat("Not roleUsers.IsNullOrEmpty() - ", item.Role, "-", Convert.ToInt16(item.Role)))
                        collaborationUsers.Add(New CollaborationContact(Nothing, Convert.ToInt16(item.Role), role.Name, role.EMailAddress, True))
                    ElseIf collaborationUsers.IsNullOrEmpty() AndAlso DocSuiteContext.Current.ProtocolEnv.WsCollCheckUserRole Then
                        '' Comportamento restrittivo, lancio eccezione
                        Throw New Exception(String.Format("L'utente {0} ({1}) non è presente nel Settore {2} ({3})", CommonAD.GetDisplayName(item.UserName), item.UserName, role.Name, role.Id))
                    Else
                        '' Comportamento largo, resetto il Role ed eseguo il caricamento con il metodo successivo
                        item.Role = 0
                    End If
                End If

                If item.Role = 0 Then
                    '' Nel caso di scelta da AD, se già non ho trovato un risultato su DB, effettuo una ricerca su AD
                    If collaborationSign Is Nothing Then
                        collaborationSign = GetCollaborationContactFromAD(item.UserName, "di firma")
                    End If

                    '' Ho l'utente firmatario ma non ho il suo settore di riferimento
                    '' Carico tutte le segreterie dell'utente dove questo è impostato come principale
                    collaborationUsers.AddRange(Factory.RoleUserFacade.GetSecretaryRoles(item.UserName, True, idTenantAOO).Select(Function(role) New CollaborationContact(Nothing, Convert.ToInt16(role.Id), role.Name, role.EMailAddress, True)))
                End If

                If collaborationSign Is Nothing Then
                    Throw New Exception(String.Format("Non è stato possibile caricare l'utente {0} ({1}) come destinatario di firma.", CommonAD.GetDisplayName(item.UserName), item.UserName))
                End If
                collaborationSigns.Add(collaborationSign)
            Next

            'Ottengo i worker se presenti dal Object rivacavato dal xml - Protocollatori diretti
            Dim restitutionUsers As New List(Of CollaborationContact) ' Addetti protocollazione Worker (Utenti)
            If collaborazioneXml.Workers IsNot Nothing Then
                restitutionUsers.AddRange(collaborazioneXml.Workers.Select(Function(f) GetCollaborationContactFromAD(f.UserName, "del protocollo")))
            End If

            '' Verifico che ci sia almeno 1 destinatario in grado di gestire
            If collaborationUsers.Count = 0 AndAlso restitutionUsers.Count = 0 Then
                Throw New Exception("Non sono state definite utenze per la protocollazione. E' necessario che sia presente almeno 1 utente oppure una segreteria.")
            End If

            Dim incremental As Short = 0

            ' Aggiungo i Firmatari
            If collaborationSigns IsNot Nothing Then
                InsertDistribution(collaborationSigns, coll.Id, username)

                ' Numero firmatari collaborazione
                coll.SignCount = CType(collaborationSigns.Count, Short)
                Update(coll)
            End If

            ' Aggiungo i workers diretti
            CheckRestitution(restitutionUsers)
            UpdateRestituition(restitutionUsers, DestinatonType.P, incremental, coll.Id, username)

            ' Aggiungo le segreterie
            CheckRestitution(collaborationUsers)
            UpdateRestituition(collaborationUsers, DestinatonType.S, incremental, coll.Id, username)

            ' Salva bozza di protocollo
            If collaborazioneXml.Attributes IsNot Nothing AndAlso collaborazioneXml.Attributes.Count > 0 Then
                Factory.CollaborationDraftFacade.AddCollaborationDraft(coll, "Bozza di Collaborazione", collaborazioneXml.Attributes(0))
            End If

            'Commit Transaction
            unitOfWork.Commit()

            'Mando la mail
            SendMail(coll, coll.IdStatus, idTenantAOO)

            Return coll.Id
        Catch ex As Exception
            'if there is an error Rollback
            unitOfWork.Rollback()
            Throw New Exception("WSColl - Errore Inserimento nuova Collaborazione: " & progressivo, ex)
        End Try
    End Function

    Public Function CreateCollaboration() As Collaboration
        Dim result As New Collaboration()

        result.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        result.LastChangedUser = DocSuiteContext.Current.User.FullUserName

        result.RegistrationName = CommonUtil.GetInstance().UserDescription
        result.RegistrationEMail = CommonUtil.GetInstance().UserMail

        Return result
    End Function

    Private Function Insert(collaboration As Collaboration) As Collaboration
        collaboration.CollaborationObject = StringHelper.Clean(collaboration.CollaborationObject, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        collaboration.CollaborationObject = StringHelper.ReplaceCrLf(collaboration.CollaborationObject)
        collaboration.CollaborationObject = StringHelper.ConvertFromWord(collaboration.CollaborationObject)

        collaboration.Note = StringHelper.Clean(collaboration.Note, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        collaboration.Note = StringHelper.ReplaceCrLf(collaboration.Note)
        collaboration.Note = StringHelper.ConvertFromWord(collaboration.Note)

        Dim serverDate As Date = _dao.GetServerDate()
        collaboration.RegistrationDate = serverDate
        collaboration.LastChangedDate = serverDate

        collaboration.Location = FacadeFactory.Instance.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.CollaborationLocation)
        collaboration.Id = FacadeFactory.Instance.IncrementalFacade.GetFor(Of Collaboration)().Incremental.Value
        If Not collaboration.Id > 0 Then
            Throw New DocSuiteException("Errore inserimento", "Errore in Calcolo Progressivo")
        End If

        Me.Save(collaboration)

        Return collaboration
    End Function

    Public Function Insert(collaboration As Collaboration,
                           ByVal distributionUsers As List(Of CollaborationContact),
                           ByVal restitutionUsers As List(Of CollaborationContact), ByVal restitutionRoles As List(Of CollaborationUser)) As Collaboration

        If distributionUsers Is Nothing Then
            collaboration.SignCount = 0S
        Else
            collaboration.SignCount = CType(distributionUsers.Count, Short)
        End If

        Dim unitOfWork As New NHibernateUnitOfWork(ProtDB)
        Try
            unitOfWork.BeginTransaction()

            Insert(collaboration)
            FacadeFactory.Instance.CollaborationLogFacade.Insert(collaboration, "Inserimento della collaborazione")

            If distributionUsers IsNot Nothing Then
                InsertDistribution(distributionUsers, collaboration.Id, DocSuiteContext.Current.User.FullUserName)
            End If
            'Lista Restituzioni
            Dim incremental As Short = 0
            If restitutionUsers IsNot Nothing Then
                UpdateRestituition(restitutionUsers, DestinatonType.P, incremental, collaboration.Id, DocSuiteContext.Current.User.FullUserName)
            End If
            If restitutionRoles IsNot Nothing Then
                For Each collUser As CollaborationUser In restitutionRoles
                    incremental += 1S

                    collUser.IdCollaboration = collaboration.Id
                    collUser.Incremental = incremental
                    collUser.DestinationType = DestinatonType.S.ToString()

                    Factory.CollaborationUsersFacade.Save(collUser)
                Next
            End If

            'Commit Transaction
            unitOfWork.Commit()
            'Refresh di tutte le relazioni
            NHibernateSessionManager.Instance.GetSessionFrom(ProtDB).Refresh(collaboration)
        Catch ex As Exception
            'if there is an error Rollback
            unitOfWork.Rollback()
            FileLogger.Warn(LoggerName, "Errore su CollaborationFacade.Insert : " & collaboration.Id, ex)
            Throw New DocSuiteException("Errore Inserimento Record Collaboration: " & collaboration.Id, ex)
        End Try

        Return collaboration
    End Function


    Public Overloads Sub Update(ByRef collaboration As Collaboration, ByVal idPriority As String,
                                ByVal memorandumDate As DateTime?, ByVal [object] As String,
                                ByVal note As String, ByVal idStatus As CollaborationStatusType?,
                                ByVal contactsDistribution As List(Of CollaborationContact),
                                ByVal contactsRestitution As List(Of CollaborationContact),
                                ByVal restitutionRoles As List(Of CollaborationUser),
                                ByVal contactsForward As List(Of CollaborationContact), ByVal year As Short?,
                                ByVal number As Integer?, ByVal idResolution As Integer?, ByVal signUser As String,
                                ByVal incremental As Short, ByVal respChange As Boolean)

        If String.IsNullOrEmpty(signUser) Then
            signUser = DocSuiteContext.Current.User.FullUserName
        End If

        If Not String.IsNullOrEmpty(idPriority) Then
            collaboration.IdPriority = idPriority
        End If
        If Not String.IsNullOrEmpty([object]) Then
            collaboration.CollaborationObject = [object]
        End If
        If note IsNot Nothing Then
            collaboration.Note = note
        End If
        If memorandumDate.HasValue Then
            collaboration.MemorandumDate = memorandumDate
        End If
        If year.HasValue Then
            collaboration.Year = year
        End If
        If number.HasValue Then
            collaboration.Number = number
        End If
        If idResolution.HasValue Then
            collaboration.Resolution = (New ResolutionFacade().GetById(idResolution.Value))
        End If
        Dim serverDate As DateTime = _dao.GetServerDate()
        If idStatus.HasValue Then
            collaboration.IdStatus = idStatus.ToString()

            If idStatus.Value = CollaborationStatusType.PT Then
                collaboration.PublicationUser = signUser
                collaboration.PublicationDate = serverDate
            End If
        End If
        collaboration.LastChangedUser = signUser
        collaboration.LastChangedDate = serverDate

        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Try
            unitOfWork.BeginTransaction()

            Update(collaboration)
            UpdateCollaborationSignsDl(serverDate, collaboration.Id, signUser)

            'Lista Restituzioni
            Dim tempIncremental As Short = 0
            If contactsRestitution IsNot Nothing AndAlso contactsRestitution.Count > 0 Then
                UpdateRestituition(contactsRestitution, DestinatonType.P, tempIncremental, collaboration.Id, DocSuiteContext.Current.User.FullUserName)
            End If
            If restitutionRoles IsNot Nothing Then
                For Each collUser As CollaborationUser In restitutionRoles
                    tempIncremental += 1S

                    collUser.IdCollaboration = collaboration.Id
                    collUser.Incremental = tempIncremental
                    collUser.DestinationType = DestinatonType.S.ToString()

                    Factory.CollaborationUsersFacade.Save(collUser)
                Next
            End If
            If contactsForward IsNot Nothing AndAlso contactsForward.Count > 0 Then
                UpdateForward(contactsForward, incremental, respChange, signUser, collaboration.Id)
            End If
            If contactsDistribution IsNot Nothing AndAlso contactsDistribution.Count > 0 Then
                UpdateDistribution(contactsDistribution, collaboration.Id)
            End If

            unitOfWork.Commit()
        Catch ex As Exception
            unitOfWork.Rollback()
            Dim message As String = String.Format("Impossibile aggiornare collaborazione [{0}]", collaboration.Id)
            FileLogger.Warn(LoggerName, message, ex)
            Throw New DocSuiteException("Errore collaborazione", message, ex)
        End Try
    End Sub

    Private Sub UpdateRestituition(ByRef contactRestitution As List(Of CollaborationContact), ByVal destType As DestinatonType, ByRef incremental As Short, ByVal idCollaboration As Integer, ByVal userConnected As String)
        For Each item As CollaborationContact In contactRestitution
            incremental += 1S

            Dim collUser As New CollaborationUser()
            collUser.IdCollaboration = idCollaboration
            collUser.Incremental = incremental
            collUser.DestinationFirst = item.DestinationFirst
            collUser.Account = item.Account
            collUser.IdRole = Nothing
            If item.IdRole <> 0 Then
                collUser.IdRole = item.IdRole
            End If
            collUser.DestinationName = item.DestinationName
            collUser.DestinationEMail = item.DestinationEMail
            collUser.DestinationType = destType.ToString()
            collUser.RegistrationUser = userConnected
            collUser.RegistrationDate = DateTimeOffset.UtcNow

            Factory.CollaborationUsersFacade.Save(collUser)
        Next
    End Sub

    Private Shared Sub CheckRestitution(ByRef contactRestitution As List(Of CollaborationContact))
        '' Rimuovo i duplicati
        contactRestitution = contactRestitution.GroupBy(Function(cr) New With {Key cr.Account, cr.IdRole}).Select(Function(c) c.First()).ToList()

        '' Se non esiste almeno 1 segreteria con il flag a 1 allora le imposto di default tutte
        If Not contactRestitution.Where(Function(c) c.DestinationFirst = True).Any() Then
            contactRestitution.ForEach(Function(f) f.DestinationFirst = True)
        End If
    End Sub

    ''' <summary> Passa la palla al firmatario successivo alias setta isActive = 0 al vecchio firmatario e aggiunge quello nuovo </summary>
    Private Sub UpdateForward(ByRef contactForward As List(Of CollaborationContact), ByRef incremental As Short, ByVal respChange As Boolean, ByVal signUser As String, ByVal idCollaboration As Integer)
        'isactive visione/firma
        Dim isRequired As Boolean = False
        For Each item As CollaborationSign In Factory.CollaborationSignsFacade.GetCollaborationSignsBy(idCollaboration, signUser, 1S)
            item.IsActive = False
            If item.IsRequired.HasValue Then
                isRequired = item.IsRequired.Value
            End If
            If respChange Then
                item.SignName &= " (D)"
            End If
            Factory.CollaborationSignsFacade.UpdateOnly(item)
        Next
        'Inserimento firmatari
        InsertCollaborationSigns(contactForward(0), incremental, idCollaboration, isRequired)
    End Sub

    Private Sub UpdateDistribution(ByRef contactDistribution As List(Of CollaborationContact), ByVal idCollaboration As Integer)
        'isActive visione/firma
        For Each item As CollaborationSign In Factory.CollaborationSignsFacade.GetCollaborationSignsBy(idCollaboration, True)
            item.IsActive = False
            Factory.CollaborationSignsFacade.UpdateOnly(item)
        Next
        'Inserimento firmatari
        InsertCollaborationSigns(contactDistribution(0), -1, idCollaboration, False)
    End Sub

    ''' <summary> Inserimento di un nuovo firmatario </summary>
    Private Sub InsertCollaborationSigns(ByVal signer As CollaborationContact, ByVal incremental As Short, ByVal idCollaboration As Integer, ByVal isRequired As Boolean)
        'Ottengo il prossimo incremental
        Dim sqlIncremental As Short = Factory.CollaborationSignsFacade.GetNextIncremental(idCollaboration)

        Dim collSign As New CollaborationSign()
        collSign.IdCollaboration = idCollaboration
        collSign.Incremental = If(incremental > 0, incremental, sqlIncremental)
        collSign.IsActive = True
        collSign.IdStatus = String.Empty
        collSign.SignUser = signer.Account
        collSign.SignName = signer.DestinationName
        collSign.SignEMail = signer.DestinationEMail
        collSign.IsRequired = isRequired
        collSign.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        collSign.RegistrationDate = DateTimeOffset.UtcNow
        Factory.CollaborationSignsFacade.Save(collSign)
    End Sub

    ''' <summary> Aggiorna CollaborationSigns SSE(se solo se) sono il Dirigente del settore </summary>
    Private Sub UpdateCollaborationSignsDl(ByVal lastChangedDate As DateTime, ByVal idCollaboration As Integer, ByVal signUser As String)
        For Each colSign As CollaborationSign In Factory.CollaborationSignsFacade.GetCollaborationSignsBy(idCollaboration, signUser, True)
            colSign.LastChangedDate = lastChangedDate
            colSign.LastChangedUser = signUser
            colSign.IdStatus = CollaborationStatusType.DL.ToString()
            Factory.CollaborationSignsFacade.Update(colSign)
        Next
    End Sub

    Public Function InsertDistribution(ByVal idCollaboration As Integer, ByRef contactDistribution As List(Of CollaborationContact)) As Boolean
        Try
            InsertDistribution(contactDistribution, idCollaboration)
            Return True
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore Inserimento CollaborationSigns della Collaboration: " & idCollaboration, ex)
            Return False
        End Try
    End Function

    Private Sub InsertDistribution(ByRef contactDistribution As List(Of CollaborationContact), ByVal idCollaboration As Integer)
        InsertDistribution(contactDistribution, idCollaboration, DocSuiteContext.Current.User.FullUserName)
    End Sub

    Private Sub InsertDistribution(ByRef contactDistribution As List(Of CollaborationContact), ByVal idCollaboration As Integer, ByVal userConnected As String)
        Dim index As Short = 0
        For Each item As CollaborationContact In contactDistribution
            Dim collSign As New CollaborationSign()
            collSign.IdCollaboration = idCollaboration
            collSign.Incremental = index + 1S
            collSign.IsActive = If(index = 0, True, False)
            collSign.IsRequired = IsManagerContact(item)
            collSign.IdStatus = String.Empty
            collSign.SignUser = item.Account
            collSign.SignName = item.DestinationName
            collSign.SignEMail = item.DestinationEMail

            collSign.RegistrationUser = userConnected
            collSign.RegistrationDate = DateTimeOffset.UtcNow

            Factory.CollaborationSignsFacade.Save(collSign)
            index += 1S
        Next
    End Sub

    ''' <summary> Elimina la collaborazione e tutte le dipendenze. </summary>
    ''' <remarks> Transazionale - in caso di errore non apporta le modifiche. </remarks>
    Public Overloads Sub Delete(ByVal idCollaboration As Integer)
        'Get session instance by name db 
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Try
            'Init Transaction
            unitOfWork.BeginTransaction()

            'Cancello la tupla Collaboration e dalle impostazioni del mapping elimino a cascata tutte le dipendenze
            Dim coll As Collaboration = GetById(idCollaboration)
            Delete(coll)

            'Commit Transaction
            unitOfWork.Commit()
        Catch ex As Exception
            'if there is an error rollback
            unitOfWork.Rollback()
            FileLogger.Warn(LoggerName, "Errore su CollaborationFacade.Delete : " & idCollaboration, ex)
            Throw New Exception("Errore Eliminazione Record Collaboration", ex)
        End Try
    End Sub

    ''' <summary> Elimina il Collaboratore dato l'id della collaborazione - idCollaboration </summary>
    Public Sub DeleteUsers(ByVal idCollaboration As Integer)
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Try
            unitOfWork.BeginTransaction()
            For Each item As CollaborationUser In Factory.CollaborationUsersFacade.GetByCollaboration(idCollaboration, Nothing, Nothing)
                Factory.CollaborationUsersFacade.Delete(item)
            Next
            unitOfWork.Commit()
        Catch ex As Exception
            unitOfWork.Rollback()
            FileLogger.Warn(LoggerName, "Errore su CollaborationFacade.DeleteUsers : " & idCollaboration, ex)
            Throw New DocSuiteException("Errore nell'eliminazione Record CollaborationUsers", ex)
        End Try
    End Sub

    ''' <summary> Elimina i firmatari dato l'id della collaborazione </summary>
    Public Function DeleteSigns(collaboration As Collaboration) As Boolean
        Dim unitOfWork As NHibernateUnitOfWork = New NHibernateUnitOfWork(ProtDB)
        Try
            unitOfWork.BeginTransaction()
            collaboration.CollaborationSigns.Clear()
            unitOfWork.Commit()
            Return True
        Catch ex As Exception
            unitOfWork.Rollback()
            FileLogger.Warn(LoggerName, String.Concat("Errore eliminazione CollaborationSigns della Collaboration: ", collaboration.Id), ex)
            Return False
        End Try
    End Function


#End Region

#Region " Gestione Mail "

    ''' <summary> Invia una mail inerente l'azione compiuta sulla collaborazione. </summary>
    ''' <param name="collaboration">Collaborazione</param>
    ''' <param name="action">Azione compiuta sulla collaborazione</param>
    Public Sub SendMail(collaboration As Collaboration, action As String, idTenantAOO As Guid)
        Try
            'Carico la descrizione dell'azione
            Dim currentCollaborationStatus As CollaborationStatus = Factory.CollaborationStatusFacade.GetById(action)

            'Check delle precondizioni
            If Not CheckSendMailPrecondition(collaboration, currentCollaborationStatus) Then
                Return
            End If

            FileLogger.Info(LoggerName, String.Format("{0}: CollaborationFacade.SendMail - Collaborazione [{1}] per {2}-{3} : Preparazione invio", DocSuiteContext.Current.User.FullUserName, collaboration.Id, currentCollaborationStatus.Id, currentCollaborationStatus.Description))

            NHibernateSessionManager.Instance.GetSessionFrom(ProtDB).Refresh(collaboration)
            Dim idMessage As Integer = Factory.MessageEmailFacade.SendEmailMessage(CreateMailMessage(collaboration, currentCollaborationStatus, False, idTenantAOO))

            FileLogger.Info(LoggerName, String.Format("{0}: CollaborationFacade.SendMail - Collaborazione [{1}] per {2}-{3} : Mail inserita in coda di invio [id {4}]", DocSuiteContext.Current.User.FullUserName, collaboration.Id, currentCollaborationStatus.Id, currentCollaborationStatus.Description, idMessage))

        Catch ex As Exception
            '' Intercetto tutte le eccezioni in maniera silenziosa in quanto la funzionalità di invio mail non deve interrompere il flusso di lavoro
            FileLogger.Error(LoggerName, String.Format("{0}: CollaborationFacade.SendMail - Collaborazione [{1}] per {2} : Errore in fase di generazione notifica.", DocSuiteContext.Current.User.FullUserName, collaboration.Id, action), ex)
        End Try
    End Sub

    ''' <summary>
    ''' Verifica in modo silente le pre-condizioni dell'invio Mail.
    ''' Se non risulta necessario la mail non viene inviata.
    ''' Se vengono rilevati errori la mail non viene inviata e viene segnato l'errore.
    ''' </summary>
    ''' <param name="collaboration"></param>
    ''' <param name="collaborationStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckSendMailPrecondition(ByVal collaboration As Collaboration, ByVal collaborationStatus As CollaborationStatus) As Boolean
        ' Verifico sia abilitata la gestione dell'invio mail.
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationMail Then
            Return False
        End If

        ' Verifico di aver ricevuto effettivamente una collaborazione
        If collaboration Is Nothing Then
            FileLogger.Error(LoggerName, String.Format("{0}: CollaborationFacade.SendMail - Ricevuta collaborazione nulla.", DocSuiteContext.Current.User.FullUserName))
            Return False
        End If

        ''Verifico sia presente un CollaborationStatus
        If collaborationStatus Is Nothing Then
            FileLogger.Error(LoggerName, String.Format("{0}: CollaborationFacade.SendMail - Ricevuta collaborazione nulla.", DocSuiteContext.Current.User.FullUserName))
            Return False
        End If

        ''Se non è previsto l'invio mail allora lo salto
        If Not collaborationStatus.MailEnable Then
            Return False
        End If

        ''Se non c'è il mittente annullo l'invio
        If String.IsNullOrEmpty(collaborationStatus.MailSender) Then
            FileLogger.Error(LoggerName, String.Format("{0}: CollaborationFacade.SendMail di collaborazione n.{1} per azione {2} - Nessun mittente impostato.", DocSuiteContext.Current.User.FullUserName, collaboration.Id, collaborationStatus.Id))
            Return False
        End If

        ''Se tutti e 3 i destinatari sono vuoti annullo l'invio
        If String.IsNullOrEmpty(collaborationStatus.MailRecipientsTo) AndAlso String.IsNullOrEmpty(collaborationStatus.MailRecipientsCc) AndAlso String.IsNullOrEmpty(collaborationStatus.MailRecipientsCcn) Then
            FileLogger.Error(LoggerName, String.Format("{0}: CollaborationFacade.SendMail di collaborazione n.{1} per azione {2} - Nessun destinatario impostato.", DocSuiteContext.Current.User.FullUserName, collaboration.Id, collaborationStatus.Id))
            Return False
        End If

        ''Se tutte le precedenti condizioni non hanno problemi allora ritorna precondizioni positive
        Return True
    End Function

    ''' <summary> Genera la mail da spedire </summary>
    ''' <param name="currentCollaborationStatus">Azione compiuta sulla collaborazione</param>
    Private Function CreateMailMessage(collaboration As Collaboration, currentCollaborationStatus As CollaborationStatus, isDispositionNotification As Boolean, idTenantAOO As Guid) As MessageEmail
        Dim signature As String = GetSignature(collaboration.Id, collaboration.RegistrationDate.DateTime, DirectCast([Enum].Parse(GetType(CollaborationDocumentType), collaboration.DocumentType), CollaborationDocumentType))
        Dim contacts As IList(Of MessageContactEmail) = GetContacts(collaboration, currentCollaborationStatus, idTenantAOO)
        If contacts.Count < 2 Then
            Throw New Exception("Contatti insufficienti per spedire la notifica.")
        End If

        Dim mailSubject As String = GetSubject(collaboration, currentCollaborationStatus, signature, idTenantAOO)
        Dim mailBody As String = GetBody(collaboration, currentCollaborationStatus, signature, contacts, idTenantAOO)
        Dim email As MessageEmail = FacadeFactory.Instance.MessageEmailFacade.CreateEmailMessage(contacts, mailSubject, mailBody, isDispositionNotification)

        ''Imposto la priorità
        Select Case collaboration.IdPriority
            Case "A"
                email.Priority = MailPriority.High
            Case "B"
                email.Priority = MailPriority.Low
            Case Else
                email.Priority = MailPriority.Normal
        End Select
        Return email
    End Function

    ''' <summary> Genera e restituisce l'oggetto della mail </summary>
    Private Function GetSubject(collaboration As Collaboration, currentCollaborationStatus As CollaborationStatus, signature As String, idTenantAOO As Guid) As String
        Dim subject As String = GetCurrentCollaborationStatusSubject(currentCollaborationStatus, collaboration.Id, idTenantAOO)
        ' Dicitura Oggetto Mail
        subject = String.Format("{0} {1} - {2} - {3}", DocSuiteContext.ProductName, signature, GetModuleName(collaboration.DocumentType, "1"), subject)
        Return subject
    End Function

    Private Function GetCurrentCollaborationStatusSubject(currentCollaborationStatus As CollaborationStatus, idCollaboration As Integer, idTenantAOO As Guid) As String
        '' Gestione Oggetto
        Dim subject As String = currentCollaborationStatus.MailSubject
        ''Sostituzione variabili Subject
        If Not String.IsNullOrEmpty(currentCollaborationStatus.MailSubjectVars) Then
            subject = String.Format(subject, (From var In currentCollaborationStatus.MailSubjectVars.Split("|"c) Select Factory.CollaborationStatusRecipientFacade.ResolveAddresses(var, idCollaboration, False, idTenantAOO)(0).Description).ToArray)
        End If
        Return subject
    End Function

    ''' <summary> Calcola e restituisce i contatti della mail da spedire </summary>
    Private Function GetContacts(ByRef collaboration As Collaboration, currentCollaborationStatus As CollaborationStatus, idTenantAOO As Guid) As IList(Of MessageContactEmail)
        Dim contacts As New List(Of MessageContactEmail)

        '' Aggiungo il mittente
        Dim sender As MessageContactEmail = Factory.CollaborationStatusFacade.GetSender(currentCollaborationStatus, collaboration.Id, idTenantAOO)
        If sender IsNot Nothing Then
            contacts.Add(sender)
        Else
            Throw New Exception("Nessun mittente caricato. Impossibile generare la notifica.")
        End If

        '' Aggiungo i destinatari
        contacts.AddRange(Factory.CollaborationStatusFacade.GetRecipients(currentCollaborationStatus, collaboration.Id, idTenantAOO))

        Return contacts
    End Function

    Private Function GetUrlNotificationMessage(ByRef collaboration As Collaboration, ByVal status As CollaborationStatus) As String
        Dim url As String = String.Empty
        If DocSuiteContext.Current.ProtocolEnv.EnableLinkToProtocolInNotificationCollaboration Then
            Select Case collaboration.DocumentType
                Case CollaborationDocumentType.P.ToString()
                    If status.Id.Eq(CollaborationMainAction.ProtocollatiGestiti) Then
                        url = String.Format("{0}?Tipo=Prot&Azione=Apri&Anno={1}&Numero={2}", DocSuiteContext.Current.CurrentTenant.DSWUrl, collaboration.Year, collaboration.Number)
                    End If
                Case CollaborationDocumentType.A.ToString(), CollaborationDocumentType.D.ToString()
                    If status.Id.Eq(CollaborationMainAction.ProtocollatiGestiti) Then
                        url = String.Format("{0}?Tipo=Resl&Azione=Apri&Identificativo={1}", DocSuiteContext.Current.CurrentTenant.DSWUrl, collaboration.Resolution.Id)
                    End If
            End Select
        End If

        If String.IsNullOrEmpty(url) Then
            url = String.Format("{0}?Tipo=Coll&Azione={1}&Identificativo={2}&Stato={1}", DocSuiteContext.Current.CurrentTenant.DSWUrl, status.MailStatus, collaboration.Id)
        End If
        Return url
    End Function


    ''' <summary> Calcola il contenuto della mail </summary>
    ''' TODO: potrebbe essere il caso di dare la possibilità di gestire il contenuto del testo tramite un template per gestioni future
    Private Function GetBody(collaboration As Collaboration, currentCollaborationStatus As CollaborationStatus, signature As String,
                             contacts As IList(Of MessageContactEmail), idTenantAOO As Guid) As String
        'Genero il link corretto
        Dim url As String = GetUrlNotificationMessage(collaboration, currentCollaborationStatus)

        Dim sw As New StringWriter()
        Dim body As New HtmlTextWriter(sw)

        Dim moduleName As String = GetModuleName(collaboration.DocumentType, "1")
        'Tipologia richiesta
        Dim statusSbj As String = GetCurrentCollaborationStatusSubject(currentCollaborationStatus, collaboration.Id, idTenantAOO)
        body.Write("Tipologia Richiesta:&nbsp;")
        body.RenderBeginTag("b")
        body.WriteEncodedText(String.Format("{0} - {1}", moduleName, statusSbj))
        body.RenderEndTag()

        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)
        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)

        'Da
        Dim contact As String = contacts.Where(Function(x) x.MessageContact.ContactPosition = MessageContact.ContactPositionEnum.Sender).Select(Function(s) s.Description).Single()
        body.Write("Da:&nbsp;")
        body.RenderBeginTag("b")
        body.WriteEncodedText(contact)
        body.RenderEndTag()

        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)

        'A
        Dim recipients As IList(Of String) = contacts.Where(Function(x) x.MessageContact.ContactPosition = MessageContact.ContactPositionEnum.Recipient).Select(Function(s) s.Description).ToList()
        body.Write("A:&nbsp;")
        body.RenderBeginTag("b")
        body.WriteEncodedText(String.Join("; ", recipients))
        body.RenderEndTag()

        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)
        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)

        'Tipologia documento
        body.Write("Tipologia Documento:&nbsp;")
        body.RenderBeginTag("b")
        body.WriteEncodedText(moduleName)
        body.RenderEndTag()

        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)

        'Oggetto
        body.Write("Oggetto:&nbsp;")
        body.RenderBeginTag("b")
        body.WriteEncodedText(collaboration.CollaborationObject)
        body.RenderEndTag()

        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)

        If Not String.IsNullOrEmpty(collaboration.Note) Then
            'Note
            body.Write("Note:&nbsp;")
            body.RenderBeginTag("b")
            body.WriteEncodedText(collaboration.Note)
            body.RenderEndTag()
        End If

        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)
        body.WriteBeginTag("br")
        body.Write(HtmlTextWriter.SelfClosingTagEnd)

        'Link
        If String.IsNullOrEmpty(signature) Then
            signature = "Collegamento"
        End If
        If Not currentCollaborationStatus.Id.Eq(CollaborationMainAction.CancellazioneDocumento) Then
            body.AddAttribute(HtmlTextWriterAttribute.Href, url)
            body.RenderBeginTag(HtmlTextWriterTag.A)
            body.Write(signature)
            body.RenderEndTag()
        End If

        body.Flush()
        Return sw.ToString()
    End Function

#End Region

    Public Sub ChangeSigner(coll As Collaboration, changeSigner As ChangeSignerDTO, ByRef countChanged As Integer, ByRef countTotal As Integer, ByRef pushNotify As Boolean, idTenantAOO As Guid)
        Dim roleUserFacade As New RoleUserFacade(ProtDB)
        Dim roleFacade As New RoleFacade(ProtDB)
        pushNotify = False
        countTotal += 1
        Dim collSign As CollaborationSign = coll.GetFirstCollaborationSignActive()

        If DocSuiteContext.Current.ProtocolEnv.StrictManagerChange Then
            Dim roleIds As List(Of Integer) = roleUserFacade.GetManagersByCollaboration(coll.Id, collSign.SignUser, idTenantAOO).Select(Function(x) x.Role.Id).ToList()
            Dim collSecretary As IList(Of RoleUser) = roleUserFacade.GetByRoleIdsAndAccount(roleIds, DocSuiteContext.Current.User.FullUserName, RoleUserType.S.ToString())
            If Not collSign.SignUser.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso (collSecretary Is Nothing OrElse (collSecretary IsNot Nothing AndAlso collSecretary.Count = 0)) Then
                Exit Sub
            End If
        End If

        'Se in stato da protocollare la collaborazione non può cambiare responsabile
        If coll.IdStatus.Eq(CollaborationStatusType.DP.ToString()) Then
            Factory.CollaborationLogFacade.Insert(coll, 0, 0, 0, CollaborationLogType.CR, "Cambio Responsabile Non possibile collaborazione in stato Da Protocollare")
            Exit Sub
        End If

        Dim contactForward As New List(Of CollaborationContact)
        Dim accountFw As CollaborationContact = Nothing
        Dim account As IList(Of RoleUser) = roleUserFacade.GetByRoleIdAndAccount(changeSigner.RoleId, changeSigner.Destination, String.Empty)
        If account.Count > 0 Then
            Dim description As String = GetSignerDescription(account(0).Description, changeSigner.Destination, coll.DocumentType)
            accountFw = New CollaborationContact(changeSigner.Destination, Nothing, description, account(0).Email, False)
            contactForward.Add(accountFw)
        End If

        If (collSign Is Nothing) OrElse Not collSign.SignUser.Eq(changeSigner.Origin) Then
            Factory.CollaborationLogFacade.Insert(coll, collSign.Incremental, collSign.Incremental, 0, CollaborationLogType.CR, String.Format("Cambio Responsabile Non possibile non è ancora il turno di [{0}].", collSign.SignUser))
            Exit Sub
        End If

        Factory.CollaborationLogFacade.Insert(coll, collSign.Incremental, collSign.Incremental, 0, CollaborationLogType.CR, String.Format("Cambio Responsabile [{0}].", collSign.SignUser))
        If coll.SignCount.HasValue AndAlso (coll.SignCount.Value > 1) Then
            Factory.CollaborationSignsFacade.ShiftUsers(coll.Id, accountFw, changeSigner.Destination, DocSuiteContext.Current.User.FullUserName)
        Else
            Update(coll, String.Empty, Nothing, String.Empty, Nothing, Nothing, Nothing, Nothing, Nothing, contactForward, Nothing, Nothing, Nothing, collSign.SignUser, 0, True)
        End If

        SendMail(coll, CollaborationMainAction.CambioResponsabile, idTenantAOO)
        countChanged += 1
        pushNotify = True
    End Sub

    Public Sub NextStep(idList As IList(Of Integer), idTenantAOO As Guid, Optional managersAccounts As AbsentManager() = Nothing)
        If idList Is Nothing Then
            Exit Sub
        End If

        'per ogni collaborazione
        For Each idColl As Integer In idList
            Try
                FileLogger.Info(LoggerName, String.Format("CollaborationFacade.NextStep di [{0}]", idColl))

                Dim coll As Collaboration = GetById(idColl)
                If coll Is Nothing Then
                    Throw New DocSuiteException("Errore Collaborazione", "Impossibile recuperare Collaborazione numero " & idColl)
                End If

                If managersAccounts IsNot Nothing AndAlso Not coll.CollaborationSigns.Any(Function(s) managersAccounts.Any(Function(m) m.Manager.Eq(s.SignUser))) Then
                    Exit Sub
                End If

                If coll.SignCount.GetValueOrDefault(0) > 0 AndAlso DocSuiteContext.Current.ProtocolEnv.ForceProsecutable Then
                    FacadeFactory.Instance.CollaborationSignsFacade.SkipRequiredSigns(coll)
                End If

                If coll.SignCount.GetValueOrDefault(0) > 1 AndAlso Factory.CollaborationSignsFacade.HasNext(coll.Id) Then
                    ' Non è ultima firma, eseguo un Forward al destinatario successivo
                    Factory.CollaborationSignsFacade.UpdateForward(coll)
                    SendMail(coll, CollaborationMainAction.DaVisionareFirmare, idTenantAOO)
                    Factory.CollaborationLogFacade.Insert(coll, "Prosegui al destinatario successivo")
                Else
                    'Aggiorno
                    If DocSuiteContext.Current.ProtocolEnv.ForceCollaborationSignDateEnabled Then
                        Dim activeSigner As CollaborationSign = coll.GetFirstCollaborationSignActive()
                        If Not activeSigner.SignDate.HasValue Then
                            activeSigner.SignDate = _dao.GetServerDate()
                            Factory.CollaborationSignsFacade.UpdateOnly(activeSigner)
                        End If
                    End If
                    ' Ultimo destinatario di firma, metto in da protocollare
                    If Factory.ResolutionFacade.UpdateResolutionFromCollaboration(coll) Then
                        Update(coll, String.Empty, Nothing, String.Empty, Nothing, CollaborationStatusType.WM, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, String.Empty, 0, False)
                        Factory.CollaborationLogFacade.Insert(coll, "Collaborazione automaticamente gestita da attività di workflow")
                    Else
                        Update(coll, String.Empty, Nothing, String.Empty, Nothing, CollaborationStatusType.DP, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, String.Empty, 0, False)
                        SendMail(coll, CollaborationMainAction.DaProtocollareGestire, idTenantAOO)
                        Factory.CollaborationLogFacade.Insert(coll, "Avanzamento al protocollo/segreteria")
                    End If
                End If
                UpdateBiblosSignsModel(coll)
            Catch ex As DocSuiteException
                Throw
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore su NextStep di CollaborationFacade: " & idColl, ex)
                Throw New DocSuiteException("Errore Collaborazione", "Errore non previsto in NextStep.", ex)
            End Try
        Next
    End Sub

    Public Sub UpdateBiblosSignsModel(ByRef coll As Collaboration)
        If DocSuiteContext.Current.ProtocolEnv.ForceCollaborationSignDateEnabled Then
            Dim documentsDictionary As IDictionary(Of Guid, BiblosDocumentInfo)
            Dim collVersiongs As IList(Of CollaborationVersioning) = Factory.CollaborationVersioningFacade.GetLastVersionings(coll)
            Dim documentDateDictionary As New List(Of CollaborationVersioningModel)
            Dim collaborationVersioningDate As DateTimeOffset
            For Each collvers As CollaborationVersioning In collVersiongs
                collaborationVersioningDate = New DateTime(collvers.RegistrationDate.Year, collvers.RegistrationDate.Month, collvers.RegistrationDate.Day, collvers.RegistrationDate.Hour, collvers.RegistrationDate.Minute, collvers.RegistrationDate.Second)
                documentDateDictionary.Add(New CollaborationVersioningModel() With {.IdDocument = collvers.IdDocument, .RegistrationDate = collaborationVersioningDate})
            Next

            Dim signerModels As List(Of CollaborationSignModel) = Factory.CollaborationSignsFacade.GetCollaborationSignModel(coll.Id, documentDateDictionary)
            documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.MainDocument)
            StoreDocumentMetadatas(documentsDictionary, signerModels)
            'documento omissis
            documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.MainDocumentOmissis)
            StoreDocumentMetadatas(documentsDictionary, signerModels)
            'allegati
            documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.Attachment)
            StoreDocumentMetadatas(documentsDictionary, signerModels)
            'allegati omissis
            documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.AttachmentOmissis)
            StoreDocumentMetadatas(documentsDictionary, signerModels)
            'annessi
            documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.Annexed)
            StoreDocumentMetadatas(documentsDictionary, signerModels)
        End If
    End Sub

    Private Sub StoreDocumentMetadatas(ByVal documentsDictionary As IDictionary(Of Guid, BiblosDocumentInfo), ByVal signerModels As List(Of CollaborationSignModel))
        If Not documentsDictionary.IsNullOrEmpty() Then
            Dim documents As IList(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)(documentsDictionary.Values)
            Dim collaborationVersioningDate As New DateTimeOffset
            Dim signerModelstorig As List(Of CollaborationSignModel)
            If Not documents.IsNullOrEmpty() Then
                For Each document As BiblosDocumentInfo In documents
                    signerModelstorig = New List(Of CollaborationSignModel)
                    For Each dbl As CollaborationSignModel In signerModels.ToList()
                        Dim collSignModel As CollaborationSignModel = New CollaborationSignModel() With {
                            .Incremental = dbl.Incremental,
                            .IsRequired = dbl.IsRequired,
                            .SignDate = dbl.SignDate,
                            .SignEmail = dbl.SignEmail,
                            .SignName = dbl.SignName,
                            .SignUser = dbl.SignUser
                        }
                        collSignModel.CollaborationVersioningModels.AddRange(dbl.CollaborationVersioningModels.Where(Function(cv) cv.IdDocument = document.BiblosChainId).ToList())
                        signerModelstorig.Add(collSignModel)
                    Next
                    collaborationVersioningDate = signerModelstorig(0).CollaborationVersioningModels.Where(Function(cv) cv.IdDocument = document.BiblosChainId).FirstOrDefault().RegistrationDate
                    signerModelstorig = signerModelstorig.Where(Function(cs) cs.SignDate.HasValue AndAlso cs.SignDate.Value >= collaborationVersioningDate AndAlso cs.IsRequired.Value = False).ToList
                    document.AddAttribute(BiblosFacade.SING_MODELS_ATTRIBUTE, JsonConvert.SerializeObject(signerModelstorig))
                    Service.UpdateDocument(document, DocSuiteContext.Current.User.FullUserName)
                Next
            End If
        End If
    End Sub

    Public Function GetFdqAttachments(ByVal idCollList As List(Of Integer)) As IList(Of DocumentFDQDTO)
        Return _dao.GetFDQAttachments(idCollList)
    End Function

    Public Function GetSortedDocumentsToSign(ByVal selectedCollaborationIds As List(Of Integer), ByVal retrieveAttachments As Boolean) As IList(Of DocumentFDQDTO)
        Dim documents As IList(Of DocumentFDQDTO) = _dao.GetFDQDocuments(selectedCollaborationIds)
        If Not retrieveAttachments Then
            Return documents
        End If

        Dim retval As New List(Of DocumentFDQDTO)
        Dim attachments As IList(Of DocumentFDQDTO) = _dao.GetFDQAttachments(selectedCollaborationIds)
        For Each doc As DocumentFDQDTO In documents
            retval.Add(doc)
            For Each att As DocumentFDQDTO In attachments
                If att.Collaboration = doc.Collaboration Then
                    retval.Add(att)
                End If
            Next
        Next

        Return retval
    End Function

    Public Function GenerateSignature(ByRef coll As Collaboration, ByVal mydate As Date, ByVal type As String) As String
        If DocSuiteContext.Current.ProtocolEnv.CollaborationSignatureType.Equals(99) Then
            Return Nothing
        End If

        Dim signature As New StringBuilder

        If DocSuiteContext.Current.ProtocolEnv.CollaborationSignatureType = 1 Then
            signature.Append(DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
            signature.Append(" ")
        End If

        signature.Append("Collaborazione")

        Select Case type
            Case CollaborationDocumentType.P.ToString()
                signature.Append(" ")
                signature.Append("Protocollo")
            Case CollaborationDocumentType.D.ToString()
                signature.Append(" ")
                signature.Append("Delibera")
            Case CollaborationDocumentType.A.ToString()
                signature.Append(" ")
                signature.Append("Atto")
        End Select

        signature.AppendFormat(" n. {0} del {1:dd/MM/yyyy}", coll.Id, mydate)

        Return signature.ToString()
    End Function

    Public Sub Richiamo(ByVal coll As Collaboration, ByVal collSigns As CollaborationSign)
        coll.LastChangedDate = DateTimeOffset.UtcNow
        coll.LastChangedUser = DocSuiteContext.Current.User.FullUserName

        Try
            Update(coll)

            Dim sign As New List(Of CollaborationContact)
            sign.Add(New CollaborationContact(DocSuiteContext.Current.User.FullUserName, Nothing, CommonUtil.GetInstance().UserDescription, CommonUtil.GetInstance.UserMail, False))

            UpdateForward(sign, 0, False, collSigns.SignUser, coll.Id)

        Catch ex As Exception
            Throw New DocSuiteException("Errore Aggiornamento Record Collaboration. CollId = " & coll.Id, ex)
        End Try
    End Sub

    Public Shared Function GetPageTypeFromDocumentType(ByVal documentType As String) As String
        If documentType.Eq(CollaborationDocumentType.A.ToString()) OrElse documentType.Eq(CollaborationDocumentType.D.ToString()) Then
            Return "Resl"
        End If
        If documentType.Eq(CollaborationDocumentType.S.ToString()) Then
            Return "Series"
        End If
        If documentType.Eq(CollaborationDocumentType.UDS.ToString()) OrElse Integer.TryParse(documentType, 0) Then
            Return "UDS"
        End If

        Return "Prot"

    End Function

    Public Sub SetSignedByUser(collaboration As Collaboration, userName As String, signDate As DateTime)
        Dim sign As CollaborationSign = GetSignByUser(collaboration, userName)
        SetSigned(sign, signDate)
    End Sub

    Public Sub SetSignedByUser(collaboration As Collaboration, userName As String)
        Dim sign As CollaborationSign = GetSignByUser(collaboration, userName)
        SetSigned(sign, _dao.GetServerDate())
    End Sub

    Private Sub SetSigned(sign As CollaborationSign, signDate As DateTime)
        sign.SignDate = signDate
        Factory.CollaborationSignsFacade.Update(sign)
        Factory.CollaborationFacade.UpdateOnly(sign.Collaboration)
    End Sub

    Public Function GetSignByUser(collaboration As Collaboration, userName As String) As CollaborationSign
        Try
            Dim signs As IList(Of CollaborationSign) = Factory.CollaborationSignsFacade.GetByCollaboration(collaboration)
            If signs Is Nothing OrElse signs.Count = 0 Then
                Throw New DocSuiteException(String.Format("Nessuna CollaborationSigns per la collaborazione con id ""{0}"".", collaboration.Id))
            End If
            Dim activeSign As CollaborationSign = signs.Where(Function(s) s.SignUser.Eq(userName) AndAlso s.IsActive).FirstOrDefault()
            If activeSign Is Nothing Then
                Throw New DocSuiteException(String.Format("Firma non prevista per l'utente ""{0}"".", userName))
            End If
            Return activeSign
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Throw
        End Try
    End Function

    Public Function GetSignature(idCollaboration As Integer) As String
        Return GetSignature(idCollaboration, _dao.GetServerDate(), CollaborationDocumentType.P)
    End Function

    Public Function GetSignature(idCollaboration As Integer, dateTime As DateTime, ByVal collaborationType As CollaborationDocumentType?) As String
        If DocSuiteContext.Current.ProtocolEnv.CollaborationSignatureType.Equals(99) Then
            Return Nothing
        End If

        Dim docType As String = String.Empty
        If collaborationType.HasValue Then
            Select Case collaborationType.Value
                Case CollaborationDocumentType.P : docType = "Protocollo "
                Case CollaborationDocumentType.D : docType = "Delibera "
                Case CollaborationDocumentType.A : docType = "Atto "
            End Select
        End If
        Dim signature As String = String.Format("Collaborazione {0}n. {1} del {2:dd/MM/yyyy}", docType, idCollaboration, dateTime)
        If DocSuiteContext.Current.ProtocolEnv.CollaborationSignatureType = 1 Then
            signature = String.Format("{0} {1}", DocSuiteContext.Current.ProtocolEnv.CorporateAcronym, signature)
        End If
        Return signature
    End Function

    Public Function GetCaption(collaboration As Collaboration) As String
        Return String.Format("Collaborazione {0} del {1:dd/MM/yyyy}", collaboration.Id, collaboration.RegistrationDate)
    End Function

    Public Sub FinalizeToProtocol(collaboration As Collaboration, idTenantAOO As Guid)
        Update(collaboration, String.Empty, Nothing, String.Empty, Nothing, CollaborationStatusType.PT, Nothing, Nothing, Nothing, Nothing, collaboration.Year, collaboration.Number, Nothing, String.Empty, 0, False)
        SendMail(collaboration, CollaborationMainAction.ProtocollatiGestiti, idTenantAOO)
    End Sub

    ''' <summary>
    ''' Ritorna la collaborazione legata alla resolution.
    ''' </summary>
    ''' <param name="resolution"></param>
    ''' <returns>Nothing se non la trova.</returns>
    ''' <remarks></remarks>
    Public Function GetByResolution(ByVal resolution As Resolution) As Collaboration
        Return _dao.GetByResolution(resolution.Id)
    End Function

    Public Function GetByAccount(username As String) As IList(Of Collaboration)
        Return _dao.GetByAccount(username)
    End Function

    ''' <summary> Simpatica tabella di conversione. </summary>
    ''' <param name="type"> 1 - sostantivo, 2 - moto a luogo, 3 - azione. </param>
    ''' <remarks> Mah </remarks>
    Public Shared Function GetModuleName(ByVal documentType As String, ByVal type As String) As String
        Dim name As String = "Gestisci"
        Select Case documentType
            Case CollaborationDocumentType.P.ToString()
                Select Case type
                    Case "1"
                        name = "Protocollo"
                    Case "2"
                        name = "Al Protocollo"
                    Case "3"
                        name = "Protocolla"
                End Select
            Case CollaborationDocumentType.D.ToString()
                Select Case type
                    Case "1"
                        name = "Delibera"
                    Case "2"
                        name = "Alle Delibere"
                    Case "3"
                        name = "Delibera"
                End Select
            Case CollaborationDocumentType.A.ToString()
                Select Case type
                    Case "1"
                        name = "Atto"
                    Case "2"
                        name = "Agli Atti"
                    Case "3"
                        name = "Atto"
                End Select
        End Select
        Return name
    End Function

    ''' <summary> Estrae tutti i settori associabili automaticamente a partire da una collaborazione. </summary>
    Public Function GetSecretaryRoles(collaboration As Collaboration, userName As String, idTenantAOO As Guid) As IList(Of Role)
        Dim roles As IList(Of Role)
        ' TODO: Finire assolutamente
        Select Case DocSuiteContext.Current.ProtocolEnv.CollSecretaryRoleAllowance()
            Case CollaborationSecretaryRoleAllowance.EveryAssociatedRoles
                Dim collaborationUsers As IList(Of CollaborationUser) = Factory.CollaborationUsersFacade.GetByCollaboration(collaboration.Id, Nothing, DestinatonType.S)
                Dim roleIdList As New List(Of Integer)
                For Each collaborationUser As CollaborationUser In collaborationUsers
                    roleIdList.Add(collaborationUser.IdRole.Value)
                Next
                roles = Factory.RoleFacade.GetByIds(roleIdList)

            Case CollaborationSecretaryRoleAllowance.DestinationFirstRoles
                Dim collaborationUsers As IList(Of CollaborationUser) = Factory.CollaborationUsersFacade.GetByCollaboration(collaboration.Id, True, DestinatonType.S)
                Dim roleIdList As New List(Of Integer)
                For Each collaborationUser As CollaborationUser In collaborationUsers
                    roleIdList.Add(collaborationUser.IdRole.Value)
                Next
                roles = Factory.RoleFacade.GetByIds(roleIdList)

            Case CollaborationSecretaryRoleAllowance.UserDestinationFirstRoles
                Dim collaborationUsers As IList(Of CollaborationUser) = Factory.CollaborationUsersFacade.GetByCollaboration(collaboration.Id, True, DestinatonType.S)
                Dim roleIdList As New List(Of Integer)
                For Each collaborationUser As CollaborationUser In collaborationUsers
                    roleIdList.Add(collaborationUser.IdRole.Value)
                Next
                Dim userRoles As IList(Of RoleUser) = Factory.RoleUserFacade.GetByUserType(Nothing, userName, True, roleIdList, idTenantAOO)

                roles = New List(Of Role)()
                For Each userRole As RoleUser In userRoles
                    roles.Add(userRole.Role)
                Next

            Case CollaborationSecretaryRoleAllowance.SecretaryDestinationFirstRoles
                Dim collaborationUsers As IList(Of CollaborationUser) = Factory.CollaborationUsersFacade.GetByCollaboration(collaboration.Id, True, DestinatonType.S)
                Dim roleIdList As New List(Of Integer)
                For Each collaborationUser As CollaborationUser In collaborationUsers
                    roleIdList.Add(collaborationUser.IdRole.Value)
                Next
                Dim userRoles As IList(Of RoleUser) = Factory.RoleUserFacade.GetByUserType(RoleUserType.S, userName, True, roleIdList, idTenantAOO)

                roles = New List(Of Role)()
                For Each userRole As RoleUser In userRoles
                    roles.Add(userRole.Role)
                Next

            Case Else
                Throw New NotImplementedException("Caso non previsto.")

        End Select

        Return roles
    End Function

    Public Function GetByProtocol(year As Short, number As Integer) As Collaboration
        Return Me._dao.GetByProtocol(year, number)
    End Function

    Public Function GetByProtocol(protocol As Protocol) As Collaboration
        Return Me.GetByProtocol(protocol.Year, protocol.Number)
    End Function

    Public Function GetAvailableDocumentTypes() As List(Of CollaborationDocumentType)
        Dim documentTypes As New List(Of CollaborationDocumentType)
        If CollaborationRights.GetCollaborationProtocolEnabled(CurrentTenant.TenantAOO.UniqueId) OrElse DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible Then
            documentTypes.Add(CollaborationDocumentType.P)
        End If

        If CollaborationRights.GetCollaborationResolutionEnabled(CurrentTenant.TenantAOO.UniqueId) OrElse (DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible AndAlso DocSuiteContext.Current.IsResolutionEnabled) Then
            documentTypes.AddRange({CollaborationDocumentType.D, CollaborationDocumentType.A})
        End If

        If CollaborationRights.GetCollaborationSeriesEnabled(CurrentTenant.TenantAOO.UniqueId) OrElse (DocSuiteContext.Current.ProtocolEnv.CollaborationMenuAlwaysVisible AndAlso DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled) Then
            documentTypes.Add(CollaborationDocumentType.S)
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationAggregateEnabled Then
            documentTypes.Add(CollaborationDocumentType.U)
        End If

        If DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled Then
            documentTypes.Add(CollaborationDocumentType.W)
        End If

        If DocSuiteContext.Current.ProtocolEnv.UDSEnabled Then
            documentTypes.Add(CollaborationDocumentType.UDS)
        End If

        Return documentTypes
    End Function

    Public Function GetByIdDocumentSeriesItem(idDocumentSeriesItem As Integer) As Collaboration
        Return _dao.GetByDocumentSeriesItem(idDocumentSeriesItem)
    End Function
    Public Function GetByIdDocumentUnit(idDocumentUnit As Guid) As Collaboration
        Return _dao.GetByIdDocumentUnit(idDocumentUnit)
    End Function

    Public Function IsManagerContact(contact As CollaborationContact) As Boolean
        If DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates IsNot Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers IsNot Nothing AndAlso
            DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Any(Function(t) t.Value.Account.Eq(contact.Account)) Then
            Return True
        End If
        Return contact.DestinationFirst
    End Function


    ''' <summary>
    ''' Il metodo ritorna la description del contatto (destinatario di collaborazione) in base alla tipologia di destinatario, 
    ''' controlla se il contatto è un direttore, verificando la presenza dell'account nel parametro AbsentManagersCertificates.
    ''' </summary>
    ''' <param name="description"></param>
    ''' <param name="collaborationType"></param>
    ''' <returns></returns>
    Public Function GetSignerDescription(description As String, account As String, collaborationType As String) As String
        If DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates IsNot Nothing AndAlso collaborationType.Equals(CollaborationDocumentType.D.ToString()) AndAlso
            DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers IsNot Nothing AndAlso
            DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Where(Function(m) m.Value.IsAbsenceManaged).Any(Function(m) m.Value.Account.Eq(account)) Then
            Dim managerType As String = DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Where(Function(m) m.Value.IsAbsenceManaged AndAlso m.Value.Account.Eq(account)).FirstOrDefault().Value.Type
            description = String.Concat(description, " - ", managerType)
        End If
        Return description
    End Function

    Public Sub SendDeleteCollaborationCommand(collaboration As Entity.Collaborations.Collaboration)
        Try
            Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
            Dim tenantName As String = CurrentTenant.TenantName
            Dim tenantId As Guid = CurrentTenant.UniqueId
            Dim tenantAOOId As Guid = CurrentTenant.TenantAOO.UniqueId
            Dim commandUpdate As ICommandDeleteCollaboration = New CommandDeleteCollaboration(tenantName, tenantId, tenantAOOId, identity, collaboration)
            CommandUpdateFacade.Push(commandUpdate)
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendDeleteCollaborationCommand => ", ex.Message), ex)
        End Try
    End Sub
End Class