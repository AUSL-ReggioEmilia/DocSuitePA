Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models
Imports Newtonsoft.Json

<ComponentModel.DataObject()>
Public Class CollaborationVersioningFacade
    Inherits BaseProtocolFacade(Of CollaborationVersioning, Guid, NHibernateCollaborationVersioningDao)

#Region " Fields "

    Public Const CheckedOutFormat As String = "{0}.{1}{2}"

#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ' Discriminatori di raggruppamento CV.

    ''' <summary> Indica se il CV si riferisce ad un particolare raggruppamento </summary>
    ''' <param name="versioning">CV di riferimento.</param>
    ''' <param name="documentGroup">DocumentGroup di riferimento.</param>
    ''' <remarks>Questo metodo è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Shared Function IsBelongingTo(versioning As CollaborationVersioning, documentGroup As String) As Boolean
        Select Case documentGroup
            Case VersioningDocumentGroup.MainDocument
                Return IsMainDocumentVersioning(versioning)
            Case VersioningDocumentGroup.MainDocumentOmissis
                Return IsMainDocumentsOmissisVersioning(versioning)
            Case VersioningDocumentGroup.Attachment
                Return IsAttachmentVersioning(versioning)
            Case VersioningDocumentGroup.AttachmentOmissis
                Return IsAttachmentOmissisVersioning(versioning)
            Case VersioningDocumentGroup.Annexed
                Return IsAnnexedVersioning(versioning)
            Case Else
                Throw New InvalidOperationException("VersioningDocumentGroup mancante o non valido.")
        End Select
    End Function

    ''' <summary> Indica se il CV si riferisce al documento principale della collaborazione </summary>
    ''' <param name="versioning">CV da verificare.</param>
    ''' <remarks>Questo metodo è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Shared Function IsMainDocumentVersioning(versioning As CollaborationVersioning) As Boolean
        Return versioning.CollaborationIncremental = 0
    End Function

    ''' <summary> Indica se il CV si riferisce ad un allegato della collaborazione </summary>
    ''' <remarks>Questo metodo è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Shared Function IsMainDocumentsOmissisVersioning(versioning As CollaborationVersioning) As Boolean
        Return Not IsMainDocumentVersioning(versioning) AndAlso versioning.DocumentGroup.Eq(VersioningDocumentGroup.MainDocumentOmissis)
    End Function

    ''' <summary> Indica se il CV si riferisce ad un allegato della collaborazione </summary>
    ''' <remarks>Questo metodo è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Shared Function IsAttachmentVersioning(versioning As CollaborationVersioning) As Boolean
        Return Not IsMainDocumentVersioning(versioning) AndAlso (String.IsNullOrEmpty(versioning.DocumentGroup) OrElse versioning.DocumentGroup.Eq(VersioningDocumentGroup.Attachment))
    End Function

    ''' <summary> Indica se il CV si riferisce ad un allegato della collaborazione </summary>
    ''' <remarks>Questo metodo è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Shared Function IsAttachmentOmissisVersioning(versioning As CollaborationVersioning) As Boolean
        Return Not IsMainDocumentVersioning(versioning) AndAlso versioning.DocumentGroup.Eq(VersioningDocumentGroup.AttachmentOmissis)
    End Function

    ''' <summary> Indica se il CV si riferisce ad un allegato di tipo "Annexed" della collaborazione </summary>
    ''' <remarks>Questo metodo è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Shared Function IsAnnexedVersioning(versioning As CollaborationVersioning) As Boolean
        Return Not IsMainDocumentVersioning(versioning) AndAlso versioning.DocumentGroup.Eq(VersioningDocumentGroup.Annexed)
    End Function


    ' Recupero i CV.

    ''' <summary> Recupera tutti i CV di una collaborazione in ordine ascendente per CollaborationIncremental, Incremental </summary>
    ''' <param name="idCollaboration">Id della collaborazione.</param>
    Public Function GetByCollaboration(ByVal idCollaboration As Integer) As IList(Of CollaborationVersioning)
        Return _dao.GetByCollaboration(idCollaboration)
    End Function

    ''' <summary> Recupera l'ultima versione dei documenti principali di uno specifico utente </summary>
    ''' <param name="idCollaborations">Lista delle collaborazioni di riferimento.</param>
    ''' <param name="user">Utente di riferimento.</param>
    ''' <remarks>Questo metodo è stato implementato a fini di verifica firme.</remarks>
    Public Function GetLastMainVersioningsByUser(idCollaborations As Integer(), user As String) As IList(Of CollaborationVersioning)
        Return _dao.GetLastVersioningsByUser(idCollaborations, user).Where(Function(v) IsMainDocumentVersioning(v)).ToList()
    End Function

    ' Recupero ultimi CV disponibili.

    ''' <summary> Recupera l'ultimo discriminatore di documento inserito per la collaborazione specificata </summary>
    ''' <param name="idCollaboration">Id della collaborazione.</param>
    ''' <remarks>Considera anche i CV con isActive = 0 per non violare la chiave primaria.</remarks>
    Public Function GetLastCollaborationIncremental(idCollaboration As Integer) As Short?
        Return _dao.GetLastCollaborationIncremental(idCollaboration)
    End Function

    ''' <summary> Recupera l'ultimo discriminatore di documento inserito per la collaborazione specificata </summary>
    ''' <param name="collaboration">Collaborazione di riferimento.</param>
    ''' <remarks>Considera anche i CV con isActive = 0 per non violare la chiave primaria.</remarks>
    Public Function GetLastCollaborationIncremental(collaboration As Collaboration) As Short?
        Return GetLastCollaborationIncremental(collaboration.Id)
    End Function

    ''' <summary> Recupera l'ultimo incrementale inserito per uno specifico documento della collaborazione </summary>
    ''' <param name="idCollaboration">Id della collaborazione.</param>
    ''' <param name="collaborationIncremental">Incrementale del documento.</param>
    ''' <remarks>Considera anche i CV con isActive = 0 per non violare la chiave primaria.</remarks>
    Public Function GetLastIncremental(idCollaboration As Integer, collaborationIncremental As Short) As Short?
        Return _dao.GetLastIncremental(idCollaboration, collaborationIncremental)
    End Function

    Public Function GetByAccount(username As String) As IList(Of CollaborationVersioning)
        Return _dao.GetByAccount(username)
    End Function


    ''' <summary> Recupera l'ultimo incrementale inserito per uno specifico documento della collaborazione </summary>
    ''' <param name="versioning">CV di cui recuperare l'ultimo incrementale.</param>
    ''' <remarks>Considera anche i CV con isActive = 0 per non violare la chiave primaria.</remarks>
    Public Function GetLastIncremental(versioning As CollaborationVersioning) As Short?
        Return GetLastIncremental(versioning.Collaboration.Id, versioning.CollaborationIncremental)
    End Function

    ''' <summary> Recupera i CV più aggiornati della collaborazione </summary>
    ''' <param name="idCollaboration">Id della collaborazione.</param>
    ''' <remarks>Considera le sole versioni con isActive = 1.</remarks>
    Public Function GetLastVersionings(idCollaboration As Integer) As IList(Of CollaborationVersioning)
        Return _dao.GetLastVersionings(idCollaboration)
    End Function

    ''' <summary> Recupera i CV più aggiornati della collaborazione </summary>
    ''' <param name="collaboration">Collaborazione di riferimento.</param>
    ''' <returns></returns>
    ''' <remarks>Considera le sole versioni con isActive = 1.</remarks>
    Public Function GetLastVersionings(collaboration As Collaboration) As IList(Of CollaborationVersioning)
        Return GetLastVersionings(collaboration.Id)
    End Function

    ''' <summary> Recupera i CV più aggiornati di un determinato DocumentGroup </summary>
    ''' <param name="collaboration">Collaborazione di riferimento.</param>
    ''' <param name="documentGroup">Gruppo documentale di riferimento.</param>
    Public Function GetLastVersionings(collaboration As Collaboration, documentGroup As String) As IList(Of CollaborationVersioning)
        Dim lastVersionings As IList(Of CollaborationVersioning) = GetLastVersionings(collaboration)
        If String.IsNullOrEmpty(documentGroup) Then
            Return lastVersionings
        End If
        Return lastVersionings.Where(Function(v) IsBelongingTo(v, documentGroup)).ToList()
    End Function

    ''' <summary>
    ''' Ritorna il documento più aggiornato di uno specifico collaborationincremental
    ''' </summary>
    ''' <param name="idCollaboration"></param>
    ''' <param name="collaborationIncremental"></param>
    ''' <returns></returns>
    Public Function GetLastVersionDocumentByIncremental(idCollaboration As Integer, collaborationIncremental As Short) As BiblosDocumentInfo
        Dim lastVersioning As CollaborationVersioning = _dao.GetLastVersioningByIncremental(idCollaboration, collaborationIncremental)
        If lastVersioning IsNot Nothing Then
            Return GetBiblosDocument(lastVersioning)
        End If
        Return Nothing
    End Function

    ' Recupero e formattazione documenti da CV.

    ''' <summary> Formatta la Caption del documento con i dati del CV </summary>
    ''' <param name="document">Documento di cui formattare l'etichetta.</param>
    ''' <param name="versioning">CV di riferimento.</param>
    Public Function SetDocumentCaption(Of T As DocumentInfo)(document As DocumentInfo, versioning As CollaborationVersioning) As T
        document.Caption = String.Format("{0} (v.{1}) del {2:dd/MM/yyyy}", document.Name, versioning.Incremental, versioning.RegistrationDate)
        If versioning.CheckedOut Then
            document.Caption = String.Format("{0} (v.{1}) *{2} dal {3:dd/MM/yyyy}", document.Name, versioning.Incremental, versioning.CheckOutUser, versioning.CheckOutDate)
        End If
        Return DirectCast(document, T)
    End Function

    ''' <summary> Recupera il BiblosDocumentInfo relativo del CV </summary>
    ''' <param name="versioning">CV di cui recuperare il documento.</param>
    ''' <remarks>Imposta anche la caption col CV relativo.</remarks>
    Public Function GetBiblosDocument(versioning As CollaborationVersioning, Optional ignoreState As Boolean = False) As BiblosDocumentInfo
        Dim location As Location = versioning.Collaboration.Location
        Dim uid As New UIDDocument(location.ProtBiblosDSDB, versioning.IdDocument, 0)
        Dim chainId As Guid = Service.GetChainGuid(uid)
        Dim document As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentChildren(chainId, ignoreState).First()
        If document.IsRemoved Then
            document = New BiblosDeletedDocumentInfo(document.DocumentId)
        End If
        Return SetDocumentCaption(Of BiblosDocumentInfo)(document, versioning)
    End Function

    ''' <summary> Recupera il BiblosDocumentInfo relativo del CV </summary>
    ''' <param name="cvck">CVCK di cui recuperare il documento.</param>
    ''' <remarks>Imposta anche la caption col CV relativo.</remarks>
    Public Function GetBiblosDocument(cvck As Guid) As BiblosDocumentInfo
        Dim cv As CollaborationVersioning = GetById(cvck)
        Return GetBiblosDocument(cv)
    End Function

    ''' <summary> Lista piatta dei documenti della collaborazione all'ultima versione. </summary>
    Public Function GetLastVersionFlatList(collaboration As Collaboration) As List(Of DocumentInfo)
        Dim lastVersionings As IList(Of CollaborationVersioning) = GetLastVersionings(collaboration)
        Dim documents As New List(Of DocumentInfo)
        For Each cv As CollaborationVersioning In lastVersionings
            documents.Add(GetBiblosDocument(cv))
        Next
        Return documents
    End Function

    ''' <summary> Recupera un dizionario in chiave CVCK valorizzato con l'ultima versione dei documenti </summary>
    ''' <param name="collaboration">Collaborazione di cui recuperare i documenti.</param>
    ''' <param name="documentGroup">Raggruppamento di appartenenza dei documenti.</param>
    ''' <remarks>Ritorna Nothing se nessun documento disponibile.</remarks>
    Public Function GetLastVersionDocuments(collaboration As Collaboration, documentGroup As String) As IDictionary(Of Guid, BiblosDocumentInfo)
        Dim lastVersionings As IList(Of CollaborationVersioning) = GetLastVersionings(collaboration, documentGroup)
        If lastVersionings Is Nothing Then
            Return Nothing
        End If
        Dim documents As Dictionary(Of Guid, BiblosDocumentInfo) = lastVersionings.ToDictionary(Of Guid, BiblosDocumentInfo)(Function(v) v.Id, Function(v) GetBiblosDocument(v))
        If documents Is Nothing OrElse documents.Count = 0 Then
            Return Nothing
        End If
        Return documents
    End Function

    ''' <summary> Recupera un dizionario in chiave CVCK valorizzato con l'ultima versione dei documenti </summary>
    ''' <param name="collaboration">Collaborazione di cui recuperare i documenti.</param>
    ''' <remarks>Ritorna Nothing se nessun documento disponibile.</remarks>
    Public Function GetLastVersionDocuments(collaboration As Collaboration) As IDictionary(Of Guid, BiblosDocumentInfo)
        Return GetLastVersionDocuments(collaboration, Nothing)
    End Function

    Public Function GetLastVersionDocumentDtos(collaboration As Collaboration, documentGroup As String) As ICollection(Of CollaborationDocument)
        Dim lastVersioningDictionary As IDictionary(Of Guid, BiblosDocumentInfo) = GetLastVersionDocuments(collaboration, documentGroup)
        Dim dtos As ICollection(Of CollaborationDocument) = New List(Of CollaborationDocument)

        If lastVersioningDictionary Is Nothing Then
            Return dtos
        End If

        For Each documentInfo As KeyValuePair(Of Guid, BiblosDocumentInfo) In lastVersioningDictionary
            Dim dto As CollaborationDocument = New CollaborationDocument()
            dto.BiblosSerializeKey = documentInfo.Value.Serialized
            dto.DocumentName = documentInfo.Value.Name
            dto.IdBiblosDocument = documentInfo.Value.DocumentId
            dto.IdCollaboration = collaboration.Id
            dto.VersioningDocumentGroup = documentGroup

            dtos.Add(dto)
        Next

        Return dtos
    End Function

    ''' <summary> Recupera il DataSource del CollaborationViewerLight </summary>
    ''' <param name="collaboration">Collaborazione di riferimento.</param>
    Public Function GetCollaborationViewerSource(collaboration As Collaboration) As DocumentInfo
        Dim lastVersionings As IList(Of CollaborationVersioning) = GetLastVersionings(collaboration)
        If lastVersionings Is Nothing OrElse lastVersionings.Count = 0 Then
            Dim message As String = String.Format("Nessun documento disponibile per la collaborazione {0}.", collaboration.Id)
            Throw New InvalidOperationException(message)
        End If

        ' Creo la struttura ad albero del visualizzatore.
        Dim source As New FolderInfo()
        source.ID = collaboration.Id.ToString()
        source.Name = Factory.CollaborationFacade.GetCaption(collaboration)

        Dim mainDocuments As New FolderInfo() With {.Name = "Documento"}
        Dim mainDocumentsOmissis As New FolderInfo() With {.Name = "Documento Omissis"}
        Dim attachments As New FolderInfo() With {.Name = "Allegati (parte integrante)"}
        Dim attachmentsOmissis As New FolderInfo() With {.Name = "Allegati Omissis (parte integrante)"}
        Dim annexed As New FolderInfo() With {.Name = "Annessi (non parte integrante)"}
        Dim others As New FolderInfo() With {.Name = "Altro"}

        For Each cv As CollaborationVersioning In lastVersionings
            Dim document As BiblosDocumentInfo = GetBiblosDocument(cv)
            If IsMainDocumentVersioning(cv) Then
                mainDocuments.AddChild(document)
            ElseIf IsMainDocumentsOmissisVersioning(cv) Then
                mainDocumentsOmissis.AddChild(document)
            ElseIf IsAttachmentVersioning(cv) Then
                attachments.AddChild(document)
            ElseIf IsAttachmentOmissisVersioning(cv) Then
                attachmentsOmissis.AddChild(document)
            ElseIf IsAnnexedVersioning(cv) Then
                annexed.AddChild(document)
            Else
                others.AddChild(document)
            End If
        Next
        If mainDocuments.Children.Count > 0 Then
            source.AddChild(mainDocuments)
        End If
        If mainDocumentsOmissis.Children.Count > 0 Then
            source.AddChild(mainDocumentsOmissis)
        End If
        If attachments.Children.Count > 0 Then
            source.AddChild(attachments)
        End If
        If attachmentsOmissis.Children.Count > 0 Then
            source.AddChild(attachmentsOmissis)
        End If
        If annexed.Children.Count > 0 Then
            source.AddChild(annexed)
        End If
        If others.Children.Count > 0 Then
            source.AddChild(others)
        End If

        Return source
    End Function

    ' Inserimento primo CV documento.

    ''' <summary> Inserisce la prima versione del documento nella collaborazione </summary>
    ''' <param name="collaboration">Collaborazione di riferimento.</param>
    ''' <param name="document">Documento da versionare.</param>
    ''' <param name="documentGroup">Tipologia del documento.</param>
    ''' <remarks>Il parametro documentGroup è stato introdotto nell'ottica futura di poter gestire più tipologie di documenti.</remarks>
    Public Function AddDocumentToVersioning(collaboration As Collaboration, document As DocumentInfo, documentGroup As String) As CollaborationVersioning
        Dim cv As New CollaborationVersioning()
        cv.Collaboration = collaboration
        Dim collaborationIncremental As Short = 0
        Select Case documentGroup
            Case VersioningDocumentGroup.MainDocument
                Dim mainVersioning As CollaborationVersioning = GetLastVersionings(collaboration).FirstOrDefault(Function(v) IsMainDocumentVersioning(v))
                If mainVersioning IsNot Nothing Then
                    Throw New InvalidOperationException("Versioning del documento principale già presente nella collaborazione.")
                End If

                'Procedura da rivedere per l'inserimento di collaborazioni con più documenti principali
                'If DocSuiteContext.Current.ProtocolEnv.CollaborationMultiDocument AndAlso Not collaboration.DocumentType = CollaborationDocumentType.P.ToString() Then
                '    Dim last As Short? = GetLastCollaborationIncremental(collaboration)
                '    collaborationIncremental = last.GetValueOrDefault(0) + 1S
                'Else
                '    Dim mainVersioning As CollaborationVersioning = GetLastVersionings(collaboration).FirstOrDefault(Function(v) IsMainDocumentVersioning(v))
                '    If mainVersioning IsNot Nothing Then
                '        Throw New InvalidOperationException("Versioning del documento principale già presente nella collaborazione.")
                '    End If
                'End If
            Case VersioningDocumentGroup.MainDocumentOmissis, VersioningDocumentGroup.Attachment, VersioningDocumentGroup.AttachmentOmissis, VersioningDocumentGroup.Annexed
                Dim last As Short? = GetLastCollaborationIncremental(collaboration)
                collaborationIncremental = last.GetValueOrDefault(0) + 1S
            Case Else
                Throw New InvalidOperationException("VersioningDocumentGroup mancante o non valido.")
        End Select
        cv.CollaborationIncremental = collaborationIncremental
        cv.Incremental = GetLastIncremental(cv).GetValueOrDefault(0) + 1S

        Dim saved As Integer = document.ArchiveInBiblos(collaboration.Location.ProtBiblosDSDB).BiblosChainId

        cv.IdDocument = saved
        cv.DocumentName = document.Name
        cv.IsActive = 1
        cv.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        cv.RegistrationDate = _dao.GetServerDate()
        cv.DocumentChecksum = Nothing ' todo: non ancora implementato. - FG
        cv.DocumentGroup = documentGroup

        Save(cv)
        Return cv
    End Function

    ''' <summary>
    ''' Scarta il CV.
    ''' </summary>
    ''' <param name="versioning">CV di riferimento.</param>
    ''' <remarks></remarks>
    Public Sub DiscardVersioning(versioning As CollaborationVersioning)
        versioning.IsActive = 0
        UpdateOnly(versioning)
    End Sub

    ' Funzionalità di Check In/Check Out.

    ''' <summary> Esegue l'estrazione del CV di un documento </summary>
    ''' <param name="versioning">CV da estrarre.</param>
    ''' <param name="user">Utente a cui attribuire l'estrazione.</param>
    Public Function CheckOut(versioning As CollaborationVersioning, user As String) As String
        If versioning.CheckedOut AndAlso versioning.CheckOutUser.Eq(user) Then
            Return versioning.CheckOutSessionId
        End If
        If versioning.CheckedOut Then
            Dim message As String = String.Format("La versione con chiave ""{0}"" del documento ""{1}"" risulta essere già estratta dall'utente ""{2}"", impossibile procedere.",
                                    versioning.Id, versioning.DocumentName, user)
            Throw New InvalidOperationException(message)
        End If

        FileLogger.Info(LoggerName, String.Format("CheckOut: {0} User: {1}", versioning.Id, user))
        versioning.CheckedOut = True
        versioning.CheckOutUser = user
        versioning.CheckOutSessionId = Guid.NewGuid.ToString()
        versioning.CheckOutDate = _dao.GetServerDate()
        Update(versioning)
        FileLogger.Info(LoggerName, "CheckOut completato.")
        Return versioning.CheckOutSessionId
    End Function

    Public Function IsMine(versioning As CollaborationVersioning) As Boolean
        Return versioning.CheckedOut.GetValueOrDefault(False) AndAlso versioning.CheckOutUser.Eq(DocSuiteContext.Current.User.FullUserName)
    End Function

    ''' <summary> Indica se la collaborazione ha dei documenti estratti </summary>
    ''' <param name="idCollaboration">Id della collaborazione da verificare.</param>
    ''' <remarks>Considera solo le estrazioni effettuate su CV con isActive = 1.</remarks>
    Public Function HasCheckOut(idCollaboration As Integer) As Boolean
        Return _dao.HasCheckOut(idCollaboration)
    End Function

    ''' <summary> Indica se la collaborazione ha dei documenti estratti </summary>
    ''' <param name="collaboration">Collaborazione da verificare.</param>
    ''' <remarks>Considera solo le estrazioni effettuate su CV con isActive = 1.</remarks>
    Public Function HasCheckOut(collaboration As Collaboration) As Boolean
        Return HasCheckOut(collaboration.Id)
    End Function

    Public Function GetDocumentInCheckout(collaboration As Collaboration) As IList(Of CollaborationVersioning)
        Return _dao.GetDocumentInCheckout(collaboration.Id)
    End Function

    Public Function GetLastCheckout(collaboration As Collaboration) As CollaborationVersioning
        Return _dao.GetLastCheckout(collaboration.Id)
    End Function

    ''' <summary> Recupera il CV dal guid dell'estrazione </summary>
    ''' <param name="sessionId">Guid dell'estrazione.</param>
    ''' <remarks>Considera solo le estrazioni effettuate su CV con isActive = 1.</remarks>
    Public Function GetVersioningByCheckOutSessionId(sessionId As String) As CollaborationVersioning
        Return _dao.GetVersioningByCheckOutSessionId(sessionId)
    End Function

    ''' <summary> Formatta il nome del documento per identificare l'estrazione </summary>
    ''' <param name="versioning">CV di cui recuperare il nome del documento.</param>
    Public Shared Function CheckedOutFileNameFormat(versioning As CollaborationVersioning) As String
        If Not versioning.CheckedOut.GetValueOrDefault(False) Then
            Dim message As String = String.Format("La versione con chiave ""{0}"" del documento ""{1}"" risulta non essere stata estratta, impossibile procedere.",
                                    versioning.Id, versioning.DocumentName)
            Throw New InvalidOperationException(message)
        End If
        If String.IsNullOrEmpty(versioning.CheckOutSessionId) Then
            Dim message As String = String.Format("La versione con chiave ""{0}"" del documento ""{1}"" risulta non avere CheckOutSessionId valorizzato, impossibile procedere.",
                                    versioning.Id, versioning.DocumentName)
            Throw New InvalidOperationException(message)
        End If
        Return CheckedOutFileNameFormat(versioning.DocumentName, versioning.CheckOutSessionId)
    End Function

    ''' <summary> Formatta il nome del documento per identificare l'estrazione </summary>
    ''' <param name="fileName">Nome del documento.</param>
    ''' <param name="sessionId">Guid dell'estrazione.</param>
    ''' <remarks>ATTENZIONE! Non esegue nessun controllo che il guid sia effettivamente abbinato ad un'estrazione.</remarks>
    Public Shared Function CheckedOutFileNameFormat(fileName As String, sessionId As String) As String
        Return String.Format(CheckedOutFormat, Path.GetFileNameWithoutExtension(fileName), sessionId, Path.GetExtension(fileName))
    End Function

    ''' <summary> Recupera il FileDocumentInfo locale corrispondente all'estrazione </summary>
    ''' <param name="versioning">CV di cui recuperare il documento estratto.</param>
    Public Function GetLocalCheckedOutDocument(versioning As CollaborationVersioning, friendlyName As Boolean) As FileDocumentInfo
        If Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled Then
            Return Nothing
        End If

        Dim name As String = CollaborationVersioningFacade.CheckedOutFileNameFormat(versioning)
        Dim local As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, name)
        If Not File.Exists(local) Then
            Throw New DocSuiteException("Apertura file locale", String.Format("Impossibile trovare il file [{0}]", local))
        End If

        Dim result As New FileDocumentInfo(New FileInfo(local))

        If friendlyName Then
            result.Name = versioning.DocumentName
        End If
        Return result
    End Function
    Public Function GetLocalCheckedOutDocument(versioning As CollaborationVersioning) As FileDocumentInfo
        Return GetLocalCheckedOutDocument(versioning, False)
    End Function

    ''' <summary> Annulla l'estrazione del CV di un documento </summary>
    ''' <param name="versioning">CV di cui annullare l'estrazione.</param>
    ''' <param name="user">Utente a cui attribuire l'annullamento.</param>
    Public Sub UndoCheckOut(versioning As CollaborationVersioning, user As String, Optional forceUndoCheckOut As Boolean = False)
        If Not versioning.CheckedOut.GetValueOrDefault(False) Then
            Return
        End If
        If Not versioning.CheckOutUser.Eq(user) AndAlso Not forceUndoCheckOut Then
            Dim message As String = String.Format("La versione con chiave ""{0}"" del documento ""{1}"" risulta essere stata estratta dall'utente ""{2}"", impossibile procedere.",
                                    versioning.Id, versioning.DocumentName, versioning.CheckOutUser)
            Throw New InvalidOperationException(message)
        End If
        Dim logString As String = $"{(If(forceUndoCheckOut, "", "Forced "))} UndoCheckOut: {versioning.Id} User: {user} ,Checkout from user {versioning.CheckOutUser}"
        FileLogger.Info(LoggerName, logString)

        versioning.CheckedOut = False
        versioning.CheckOutUser = Nothing
        versioning.CheckOutSessionId = Nothing
        versioning.CheckOutDate = Nothing
        Update(versioning)
        FileLogger.Info(LoggerName, "UndoCheckOut completato.")
    End Sub

    ''' <summary>
    ''' Esegue l'archiaviazione di una nuove versione del documento.
    ''' </summary>
    ''' <param name="versioning">CV da aggiornare.</param>
    ''' <param name="user">Utente a cui attribuire l'archiviazione.</param>
    ''' <param name="document">Nuovo documento.</param>
    ''' <remarks></remarks>
    Public Sub CheckIn(versioning As CollaborationVersioning, user As String, document As DocumentInfo)
        If Not versioning.CheckedOut.GetValueOrDefault(False) Then
            Dim message As String = String.Format("La versione con chiave ""{0}"" del documento ""{1}"" risulta non essere stata estratta, impossibile procedere.",
                                    versioning.Id, versioning.DocumentName)
            Throw New InvalidOperationException(message)
        End If
        If Not versioning.CheckOutUser.Eq(user) Then
            Dim message As String = String.Format("La versione con chiave ""{0}"" del documento ""{1}"" risulta essere stata estratta dall'utente ""{2}"", impossibile procedere.",
                                    versioning.Id, versioning.DocumentName, versioning.CheckOutUser)
            Throw New InvalidOperationException(message)
        End If
        ' todo: test di versioning.DocumentChecksum non ancora implementato. - FG

        FileLogger.Info(LoggerName, String.Format("CheckIn: {0} User: {1}", versioning.Id, user))
        FileLogger.Info(LoggerName, String.Format("Da: {0} A: {1}", versioning.DocumentName, document.Name))

        ' Salvo il documento in Biblos.
        Dim signature As String = Factory.CollaborationFacade.GetSignature(versioning.Collaboration.Id)
        FileLogger.Info(LoggerName, "GetSignature: " & signature)
        document.Signature = signature
        FileLogger.Info(LoggerName, String.Format("SaveToBiblos: {0}", versioning.Collaboration.Location.ProtBiblosDSDB))

        Dim checkedInChainId As Integer = document.ArchiveInBiblos(versioning.Collaboration.Location.ProtBiblosDSDB).BiblosChainId

        Dim cv As New CollaborationVersioning()
        cv.Collaboration = versioning.Collaboration
        cv.CollaborationIncremental = versioning.CollaborationIncremental
        cv.Incremental = GetLastIncremental(cv).GetValueOrDefault(0) + 1S

        cv.IdDocument = checkedInChainId
        cv.DocumentName = document.Name
        cv.IsActive = 1
        cv.RegistrationUser = user
        cv.RegistrationDate = _dao.GetServerDate()
        cv.CheckedOut = False
        cv.DocumentChecksum = Nothing ' todo: non ancora implementato. - FG
        cv.DocumentGroup = versioning.DocumentGroup
        FileLogger.Info(LoggerName, String.Format("Salvataggio nuovo Versioning: {0}", cv.Id))
        Save(cv)
        FileLogger.Info(LoggerName, "Salvataggio nuovo Versioning completato.")
        Factory.CollaborationLogFacade.Insert(versioning.Collaboration, versioning.CollaborationIncremental, cv.Incremental, cv.IdDocument, CollaborationLogType.MS, $"File modificato [{document.Name}]")

        ' Annullo il checkout della versione precedentemente estratta.
        UndoCheckOut(versioning, user)
        FileLogger.Info(LoggerName, "CheckIn completato.")
    End Sub


    Public Function InsertDocument(ByVal idCollaboration As Integer, ByVal idDocument As Integer, ByVal documentName As String, ByVal registrationUser As String, ByVal registrationName As String, ByVal collaborationIncremental As Short) As CollaborationVersioning
        Dim cv As New CollaborationVersioning()
        cv.Collaboration = FacadeFactory.Instance.CollaborationFacade.GetById(idCollaboration)
        cv.CollaborationIncremental = collaborationIncremental
        cv.Incremental = GetLastIncremental(cv).GetValueOrDefault(0) + 1S

        cv.IdDocument = idDocument
        cv.DocumentName = documentName
        cv.IsActive = 1S
        cv.RegistrationUser = registrationUser
        cv.RegistrationDate = _dao.GetServerDate()

        Save(cv)
        Return cv
    End Function

    Public Function InsertDocument(ByVal idCollaboration As Integer, ByVal idDocument As Integer, ByVal documentName As String, ByVal registrationUser As String, ByVal registrationName As String) As CollaborationVersioning
        Return InsertDocument(idCollaboration, idDocument, documentName, registrationUser, registrationName, 0)
    End Function

    Public Function InsertDocument(ByVal idCollaboration As Integer, ByVal idDocument As Integer, ByVal documentName As String, ByVal collaborationIncremental As Short) As CollaborationVersioning
        Return InsertDocument(idCollaboration, idDocument, documentName, DocSuiteContext.Current.User.FullUserName, CommonUtil.GetInstance.UserDescription, collaborationIncremental)
    End Function

    Public Function InsertDocument(ByVal idCollaboration As Integer, ByVal idDocument As Integer, ByVal documentName As String) As CollaborationVersioning
        Return InsertDocument(idCollaboration, idDocument, documentName, DocSuiteContext.Current.User.FullUserName, CommonUtil.GetInstance.UserDescription, 0)
    End Function

    Public Function CheckUserDocumentsSign(ByVal idCollaborations As Integer(), ByVal user As String) As Boolean
        Dim versionings As IList(Of CollaborationVersioning) = Nothing
        If String.IsNullOrEmpty(user) Then
            user = DocSuiteContext.Current.User.FullUserName
        End If
        For Each idCollaboration As Integer In idCollaborations
            versionings = GetLastMainVersioningsByUser(New Integer() {idCollaboration}, user)
            If versionings.Count <= 0 Then
                Return False
            End If
            For Each collVers As CollaborationVersioning In versionings
                Dim doc As BiblosDocumentInfo = GetBiblosDocument(collVers)
                If Not doc.IsSigned Then
                    Return False
                End If
            Next
        Next
        Return True
    End Function

    Public Function CheckUserDocumentsSign(ByVal idCollList As Integer()) As Boolean
        Return CheckUserDocumentsSign(idCollList, String.Empty)
    End Function

    ''' <summary> Ritorna il versioning collegato al documento o nothing </summary>
    Public Shared Function GetVersioningByDocumentInfo(versionings As IList(Of CollaborationVersioning), document As DocumentInfo) As CollaborationVersioning
        If document.GetType() IsNot GetType(BiblosDocumentInfo) OrElse versionings Is Nothing Then
            Return Nothing
        End If

        Dim bdi As BiblosDocumentInfo = DirectCast(document, BiblosDocumentInfo)
        Dim idBiblos As Integer = bdi.BiblosChainId
        Return versionings.FirstOrDefault(Function(v) v.IdDocument = idBiblos)
    End Function

    ''' <summary> Esegue il checkout/checkin di tutti i document info passati. </summary>
    Public Sub CheckSignedDocuments(ByVal signedDocuments As List(Of MultiSignDocumentInfo))
        Dim serverDate As DateTime = DateTime.Now

        For Each document As MultiSignDocumentInfo In signedDocuments

            Dim collaborationVersioning As CollaborationVersioning = GetById(Guid.Parse(document.IdOwner))
            Dim coll As Collaboration = collaborationVersioning.Collaboration

            Dim rollback As Boolean = False
            Try
                document.DocumentInfo.Signature = Factory.CollaborationFacade.GenerateSignature(coll, serverDate, String.Empty)

                CheckOut(collaborationVersioning, DocSuiteContext.Current.User.FullUserName)
                CheckIn(collaborationVersioning, DocSuiteContext.Current.User.FullUserName, document.DocumentInfo)
                rollback = True

                Factory.CollaborationFacade.SetSignedByUser(coll, DocSuiteContext.Current.User.FullUserName, serverDate)

                Dim chainId As Integer = GetLastVersionings(coll.Id).Where(Function(x) x.CollaborationIncremental = collaborationVersioning.CollaborationIncremental).FirstOrDefault().IdDocument

                Factory.CollaborationLogFacade.Insert(coll, collaborationVersioning.CollaborationIncremental, collaborationVersioning.Incremental, chainId, CollaborationLogType.MF, String.Format("Firma Documento [{0}].", document.DocumentInfo.Name))

            Catch ex As Exception
                ' Accerto che non stia checked out da me
                If collaborationVersioning.CheckedOut AndAlso collaborationVersioning.CheckOutUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    UndoCheckOut(collaborationVersioning, DocSuiteContext.Current.User.FullUserName)
                End If
                If rollback Then
                    DiscardVersioning(collaborationVersioning)
                End If
            End Try
        Next

    End Sub

#End Region

End Class