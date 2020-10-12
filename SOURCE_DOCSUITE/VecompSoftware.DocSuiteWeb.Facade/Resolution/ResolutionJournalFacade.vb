Imports System
Imports System.Collections.Generic
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports System.Text
Imports VecompSoftware.Services.Biblos.Models
Imports System.Linq

<ComponentModel.DataObject()>
Public Class ResolutionJournalFacade
    Inherits BaseResolutionFacade(Of ResolutionJournal, Integer, NHibernateResolutionJournalDao)

#Region " Fields "

    ''' <summary>
    ''' Separatore ufficiale per la docsuiteweb
    ''' </summary>
    ''' <remarks>
    ''' Temporaneamente e impropriamente usato anche da altre facade
    ''' TODO: metterlo al posto giusto :)
    ''' </remarks>
    Public Const Separator As Char = "§"c

#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " FacadeEntityBase overrides "

    Public Sub DetachDocuments(ByRef journal As ResolutionJournal)
        Service.DetachDocument(journal.Template.Location.ReslBiblosDSDB, journal.IDDocument)
        If journal.IDSignedDocument.HasValue Then
            Service.DetachDocument(journal.Template.Location.ConsBiblosDSDB, journal.IDSignedDocument)
        End If
    End Sub

#End Region

#Region " Methods "

    Public Function GetByYear(year As Integer) As IList(Of ResolutionJournal)
        Return _dao.GetByYear(year)
    End Function

    ''' <summary>
    ''' Ritorna tutte le resolution journal attive
    ''' </summary>
    ''' <returns></returns>
    Public Function GetActive() As IList(Of ResolutionJournal)
        Return _dao.GetActive()
    End Function

    ''' <summary>
    ''' Recupero l'ultimo Registro salvato per il Template indicato
    ''' </summary>
    Public Function GetLast(template As ResolutionJournalTemplate) As ResolutionJournal
        Return _dao.GetLast(template)
    End Function

    Public Function GetLastBeforeYearAndMonth(template As ResolutionJournalTemplate, year As Short, month As Integer) As ResolutionJournal
        Return _dao.GetLastBeforeYearAndMonth(template, year, month)
    End Function

    ''' <summary>
    ''' Crea il registro sequenzialmente successivo all'ultimo presente nel Template indicato
    ''' </summary>
    Public Function BuildNext(template As ResolutionJournalTemplate) As ResolutionJournal
        Return _dao.BuildNext(template)
    End Function

    Public Function ExtractToSign(idResolutionJournal As Integer) As DocumentInfo
        Dim resolutionJournal As ResolutionJournal = GetById(idResolutionJournal)
        If resolutionJournal Is Nothing Then Throw New DocSuiteException("Nessun registro trovato per l'ID passato")
        If resolutionJournal.IDDocument.Equals(0) Then Throw New DocSuiteException("Nessun documento presente per la firma")

        Dim biblosArchiveName As String = String.Empty
        Dim idChain As Integer = 0
        If resolutionJournal.IDSignedDocument.HasValue Then
            biblosArchiveName = resolutionJournal.Template.Location.ConsBiblosDSDB
            idChain = resolutionJournal.IDSignedDocument.Value
        Else
            biblosArchiveName = resolutionJournal.Template.Location.ReslBiblosDSDB
            idChain = resolutionJournal.IDDocument
        End If

        Return New BiblosDocumentInfo(biblosArchiveName, idChain)
    End Function

    ''' <summary>
    ''' Estrae il file relativo al registro dei provvedimenti
    ''' </summary>
    ''' <param name="resolutionJournal">Registro dei provvedimenti</param>
    ''' <returns>Nome del file generato</returns>
    ''' <remarks>Se un provvedimento non è firmato prende il documento originale, altrimenti prende l'ultimo firmato</remarks>
    Public Function Extract(ByVal resolutionJournal As ResolutionJournal) As String
        ' Nome del file
        'Dim fileName As String = String.Format("{0}-{1}-{2:HHmmss}", CommonUtil.UserDocumentName, GetDescription(resolutionJournal), DateTime.Now)
        ' Selezione file firmato o originale
        Dim doc As BiblosDocumentInfo
        If resolutionJournal.IDSignedDocument.HasValue Then
            doc = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ConsBiblosDSDB, resolutionJournal.IDSignedDocument.Value).LastOrDefault()
        Else
            doc = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ReslBiblosDSDB, resolutionJournal.IDDocument).FirstOrDefault()
        End If

        'Dim filename As String = String.Concat(GetDescription(resolutionJournal), doc.Extension)
        Dim fi As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)

        Return fi.Name
    End Function

    ''' <summary>
    ''' Estraggo il file info del documento originale.
    ''' Metodo utilizzato SOLO per la bonifica dei registri.
    ''' </summary>
    ''' <param name="resolutionJournal"></param>
    ''' <returns></returns>
    Public Function ExtractNotSignedFileInfo(ByVal resolutionJournal As ResolutionJournal) As FileInfo
        Dim doc As BiblosDocumentInfo = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ReslBiblosDSDB, resolutionJournal.IDDocument).FirstOrDefault()
        Return BiblosFacade.SaveUniqueToTemp(doc)
    End Function

    ''' <summary>
    ''' Estrae tutti i file relativi ai registri dei provvedimenti
    ''' </summary>
    ''' <param name="journalList">Lista dei registri dei provvedimenti</param>
    ''' <returns>Eventuali errori</returns>
    ''' <remarks>Se un provvedimento non è firmato prende il documento originale, altrimenti prende l'ultimo firmato</remarks>
    Public Function Extract(journalList As List(Of ResolutionJournal), ByRef tempDirectory As DirectoryInfo, ByRef inDirectory As DirectoryInfo, ByRef outDirectory As DirectoryInfo) As String

        Dim errorMessage As String = Nothing
        ' Prendo i permessi per manipolare cartelle sul server
        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()

            ' directory temporanea
            Dim shareDirectory As String = DocSuiteContext.Current.ProtocolEnv.FDQMultipleShare.Replace("%ServerName%", CommonShared.MachineName)
            If Not Directory.Exists(shareDirectory) Then
                Throw New DirectoryNotFoundException("Impossibile trovare la directory temporanea: reimpostare FDQMultipleShare.")
            End If

            ' Directory temporanea utente
            Dim tmpDirPath As String = Path.Combine(shareDirectory, String.Format("{0}{1:hhmmss}", DocSuiteContext.Current.User.UserName, Now))

            ' Mi assicuro che la directory sia vuota
            tempDirectory = New DirectoryInfo(tmpDirPath)
            If (tempDirectory.Exists) Then
                tempDirectory.Delete(True)
            End If

            tempDirectory.Create()

            ' Creo la sottodirectory per i documenti da salvare
            inDirectory = tempDirectory.CreateSubdirectory("In")
            If Not inDirectory.Exists Then
                Throw New IOException("Impossibile creare la directory temporanea: " & inDirectory.FullName)
            End If

            ' Creo la directory per i documenti firmati
            outDirectory = tempDirectory.CreateSubdirectory("Out")
            If Not outDirectory.Exists Then
                Throw New DirectoryNotFoundException("Impossibile trovare la directory: " & outDirectory.FullName)
            End If

            ' Estraggo i documenti per la firma
            For Each resolutionJournal As ResolutionJournal In journalList
                ' TODO: reimplementare appena si fa una nuova logica di gestione dei files temporanei
                Dim doc As BiblosDocumentInfo
                If resolutionJournal.IDSignedDocument.HasValue Then
                    doc = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ConsBiblosDSDB, resolutionJournal.IDSignedDocument.Value).LastOrDefault()
                Else
                    doc = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ReslBiblosDSDB, resolutionJournal.IDDocument).FirstOrDefault()
                End If

                ' Salvo nella directory temporanea il file rinominandolo nel suo id (per vecchie logiche di GUI)
                Dim filename As String = String.Format("{0}{1}{2}", resolutionJournal.Id, Separator, doc.Name)
                doc.SaveToDisk(inDirectory, filename)
            Next

        Catch ex As Exception
            ' Se esiste ancora la directory temporanea la cancello
            If tempDirectory IsNot Nothing Then
                tempDirectory.Refresh()
                If tempDirectory.Exists Then
                    tempDirectory.Delete(True)
                End If
            End If

            errorMessage = "Errore firma multipla: " & ex.Message

        Finally
            ' Rilascio i permessi per manipolare cartelle sul server
            impersonator.ImpersonationUndo()
        End Try

        Return errorMessage
    End Function

    ''' <summary>
    ''' Genera la firma per il Registro
    ''' </summary>
    ''' <param name="resolutionJournal">Registro di riferimento</param>
    ''' <param name="signDate">Data della firma</param>
    Private Function GenerateSignature(ByRef resolutionJournal As ResolutionJournal, ByVal signDate As Date) As String

        Dim format As String = resolutionJournal.Template.SignatureFormat
        If String.IsNullOrEmpty(format) Then
            format = "{0} {1:Description} n. {1:Id} del {2:dd/MM/yyyy}"
        End If

        Dim signature As New StringBuilder

        signature.AppendFormat(New ResolutionJournalFormatter(), format,
                               DocSuiteContext.Current.ResolutionEnv.CorporateAcronym,
                               resolutionJournal,
                               signDate)

        Return signature.ToString()
    End Function

    ''' <summary>
    ''' Salva il registro su Biblos
    ''' </summary>
    ''' <param name="resolutionJournal">Registro di riferimento</param>
    ''' <param name="physicalFile">File di cui fare l'upload</param>
    ''' <returns>Identificativo catena documentale biblos</returns>
    Public Function SaveDocument(ByRef resolutionJournal As ResolutionJournal, ByRef physicalFile As FileInfo) As Integer

        Dim location As Location = resolutionJournal.Template.Location
        Dim newChainId As Integer = 0
        Try
            If location Is Nothing Then
                Throw New Exception("Nessuna Location definita")
            End If
            Dim filename As String = GetDescription(resolutionJournal) & FileHelper.PDF
            Dim segnature As String = GenerateSignature(resolutionJournal, _dao.GetServerDate())

            Dim uid As UIDDocument = Services.Biblos.Service.AddFile(New UIDLocation() With {.Archive = location.ReslBiblosDSDB},
                                                         physicalFile, Service.GetBaseAttributes(filename, segnature))

            newChainId = uid.Chain.Id

        Catch ex As Exception
            Throw New DocSuiteException("ResolutionJournal", ex) With {.Descrizione = "Errore in salvataggio Registro.", .User = DocSuiteContext.Current.User.FullUserName}
        End Try

        Return newChainId
    End Function

    ''' <summary>
    ''' Salva il registro su Biblos
    ''' </summary>
    ''' <param name="resolutionJournal">Registro di riferimento</param>
    ''' <param name="physicalFile">File di cui fare l'upload</param>
    ''' <returns>chainId del documento salvato con firma su biblos</returns>
    Public Function SaveSignedDocument(ByRef resolutionJournal As ResolutionJournal, ByRef physicalFile As FileInfo) As Integer
        Try
            Dim location As Location = resolutionJournal.Template.Location
            If location Is Nothing Then
                Throw New Exception("Nessuna Location definita, Verificare ResolutionJournalTemplate.")
            End If

            ' Creazione del chainobject specifico per l'archivio della conservazione
            Dim attributes As New Dictionary(Of String, String)

            attributes.Add("Filename", GetDescription(resolutionJournal) & FileHelper.GetAllExtensions(physicalFile))
            attributes.Add("Signature", GenerateSignature(resolutionJournal, _dao.GetServerDate()))

            attributes.Add("anno", resolutionJournal.Year.ToString())
            attributes.Add("mese", resolutionJournal.Month.ToString())
            attributes.Add("data", _dao.GetServerDate().ToString("yyyyMMdd HH:mm:ss"))

            Dim contenitori As String = ResolutionJournalTemplateFacade.GetContainerAttributeByTemplate(resolutionJournal.Template)
            attributes.Add("contenitori", contenitori)
            Dim tipo As String = ResolutionJournalTemplateFacade.GetResolutionTypeAttributeByTemplate(resolutionJournal.Template)
            attributes.Add("tipo", tipo)
            Dim registro As String = ResolutionJournalTemplateFacade.GetServiceCodeAttributeByTemplate(resolutionJournal.Template)
            attributes.Add("registro", registro)

            ' Aggiungo il documento firmato all'archivio di conservazione, se è già stato firmato lo aggiungo in catena agli altri
            Dim chainId As Integer = 0
            'Aggiungo in coda solo se il parametro apposito è disattivo ovvero
            'se voglio tutta la serie (valore false) passo il precedente valore, altrimenti passo 0
            If resolutionJournal.IDSignedDocument.HasValue Then
                If Not DocSuiteContext.Current.ResolutionEnv.RegistroProvvedimentiAdottatiLastSignOnly Then
                    chainId = resolutionJournal.IDSignedDocument.Value
                Else
                    ''Se passo chanId = 0 ottengo un nuovo inserimento
                    ''Devo pertanto fare il detach dell'inserimento precedente
                    Services.Biblos.Service.DetachDocument(location.ConsBiblosDSDB, resolutionJournal.IDSignedDocument.Value)
                End If
            End If

            Dim uid As UIDDocument = Service.AddFile(New UIDLocation(location.ConsBiblosDSDB), chainId, physicalFile, attributes)
            Return uid.Chain.Id
        Catch ex As Exception
            Throw New DocSuiteException("ResolutionJournal error", "Errore in salvataggio Registro.", ex)
        End Try

    End Function

    ''' <summary>
    ''' Aggiorno la data firma per l'utente indicato in parametro e salva la modifica
    ''' </summary>
    ''' <param name="resolutionJournal">Registro di riferimento</param>
    ''' <param name="userName">Nome del firmatario</param>
    ''' <param name="signDate">Data della firma</param>
    Public Sub SetSign(resolutionJournal As ResolutionJournal, userName As String, signDate As DateTime)
        resolutionJournal.SignUser = userName
        resolutionJournal.Signdate = signDate
        Update(resolutionJournal)
    End Sub


    ''' <summary>
    ''' Verifica se il registro è cancellabile
    ''' </summary>
    ''' <param name="resolutionJournal">Se è l'ultimo del template allora è quello cancellabile</param>
    Public Function IsDeletable(resolutionJournal As ResolutionJournal) As Boolean
        Return _dao.GetLast(resolutionJournal.Template).Id = resolutionJournal.Id
    End Function

    ''' <summary>
    ''' Ottiene il nome di un registro formattato correttamente
    ''' </summary>
    Public Shared Function GetDescription(template As ResolutionJournalTemplate, month As Integer, year As Integer) As String
        Return String.Format("{0} {1} {2}", template.Description, StringHelper.UppercaseFirst(MonthName(month)), year)
    End Function

    ''' <summary>
    ''' Ottiene il nome di un registro formattato correttamente
    ''' </summary>
    Public Shared Function GetDescription(resolutionJournal As ResolutionJournal) As String
        Return GetDescription(resolutionJournal.Template, resolutionJournal.Month, resolutionJournal.Year)
    End Function

    Public Function ResolutionJournalViewerUrl(item As ResolutionJournal) As String
        Dim archive As String
        Dim chainId, chainEnum As Integer
        If item.Signdate.HasValue Then
            ' Documento firmato
            archive = item.Template.Location.ConsBiblosDSDB
            chainId = item.IDSignedDocument.Value

            Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(archive, chainId)
            chainEnum = docs.Count - 1
        Else
            ' Documento non firmato
            archive = item.Template.Location.ReslBiblosDSDB
            chainId = item.IDDocument
            chainEnum = 0
        End If

        Dim doc As New BiblosDocumentInfo(archive, chainId, chainEnum)

        Dim parameters As String = $"guid={doc.ChainId}&label={item.Template.Description}"
        Return $"~/Viewers/BiblosViewer.aspx?{CommonShared.AppendSecurityCheck(parameters)}"
    End Function

#End Region

End Class
