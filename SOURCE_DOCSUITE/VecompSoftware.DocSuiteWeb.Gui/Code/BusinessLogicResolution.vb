Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports System.Collections.Generic
Imports System.IO
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports System.Linq

Public Class BusinessLogicResolution

    Private _resolution As Resolution
    Private _fileresolution As FileResolution


    Public Sub New(ByVal resolution As Resolution)
        _resolution = resolution
        Dim temp As IList(Of FileResolution) = (New FileResolutionFacade).GetByResolution(resolution)
        If temp.Count = 1 Then
            _fileresolution = temp(0)
        Else
            Throw New Exception("Errore in costruzione BusinessLogicResolution: fileresolution")
        End If
    End Sub

    ' METODI IMPLEMENTATI PER TORINO
    Public Sub RemoveFrontalino()
        Dim facade As New ResolutionFacade()
        If HasFrontalino() Then
            Dim oldchain As Integer = _fileresolution.IdResolutionFile.GetValueOrDefault(-1)
            Dim docs As IList(Of DocumentInfo) = CType(BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdResolutionFile.GetValueOrDefault(-1)), IList(Of DocumentInfo))
            ' Rimuovo il frontalino
            docs.RemoveAt(0)
            Dim idCatena As Integer = 0
            Dim signature As String = facade.SqlResolutionGetNumber(_resolution.Id, , , , True)
            ResolutionFacade.SaveBiblosDocuments(_resolution, docs, Nothing, signature, idCatena, 0)
            ' registrazione del id catena in db
            facade.SqlResolutionDocumentUpdate(_resolution.Id, idCatena, ResolutionFacade.DocType.Disposizione)

            Dim logger As New ResolutionLogFacade()
            logger.Log(_resolution, ResolutionLogType.RP, String.Format("Rimozione Frontalino. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, idCatena))
        End If

        'Se esistono allegati
        'Se questi allegati hanno il frontalino
        'rimuoverlo
        If IdAttachmentsHasFrontalino() Then
            Dim oldchain As Integer = _fileresolution.IdAttachements

            Dim docs As IList(Of DocumentInfo) = CType(BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdAttachements.GetValueOrDefault(-1)), IList(Of DocumentInfo))
            ' Rimuovo il frontalino
            docs.RemoveAt(0)
            Dim idCatena As Integer = 0
            Dim signature As String = facade.SqlResolutionGetNumber(_resolution.Id, , , , True)
            ResolutionFacade.SaveBiblosDocuments(_resolution, docs, Nothing, signature, idCatena, 0)
            ' registrazione del id catena in db
            facade.SqlResolutionDocumentUpdate(_resolution.Id, idCatena, ResolutionFacade.DocType.Allegati)

            Dim logger As New ResolutionLogFacade()
            logger.Log(_resolution, ResolutionLogType.RP, String.Format("Rimozione Frontalino da allegato. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, idCatena))
        End If

    End Sub

    Public Sub AddFrontalino(ByVal files As ICollection(Of ResolutionFrontispiece), Optional ByVal frontalinoAllegati As Boolean = False)
        If files.IsNullOrEmpty() Then
            Return
        End If

        Dim frontispieces As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        Dim privacyFrontispieces As IList(Of DocumentInfo) = New List(Of DocumentInfo)

        Dim signature As String = FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(_resolution.Id, , , , True)
        Dim info As FileInfo = New FileInfo(files.FirstOrDefault(Function(x) Not x.IsPrivacy).Path)
        Dim item As New TempFileDocumentInfo(info) With {.Name = ReslFrontalinoPrintPdfTO.FRONTALINO_NAME, .Signature = signature}
        frontispieces.Add(item)

        Dim privacyFront As ResolutionFrontispiece = files.FirstOrDefault(Function(x) x.IsPrivacy)
        If privacyFront IsNot Nothing Then
            Dim privacyItem As New TempFileDocumentInfo(New FileInfo(privacyFront.Path)) With {.Name = ReslFrontalinoPrintPdfTO.FRONTALINO_PRIVACY_NAME, .Signature = signature}
            privacyFrontispieces.Add(privacyItem)
        End If

        If (Not frontalinoAllegati) Then
            ' Verifico se la resolution ha già il documento
            If _fileresolution.IdResolutionFile.HasValue Then
                Dim oldchain As Integer = _fileresolution.IdResolutionFile.Value
                Dim frontspieceChain As Integer = 0

                Dim mainDocs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, oldchain)
                mainDocs = mainDocs.Where(Function(x) Not x.Name.Eq(ReslFrontalinoPrintPdfTO.FRONTALINO_NAME)).ToList()
                For Each mainDoc As DocumentInfo In mainDocs
                    frontispieces.Add(mainDoc)
                Next
                ' Creo catena con frontalino
                frontspieceChain = DocumentInfoFactory.ArchiveDocumentsInBiblos(frontispieces, _resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, frontspieceChain)
                FacadeFactory.Instance.ResolutionFacade.SqlResolutionDocumentUpdate(_resolution.Id, frontspieceChain, ResolutionFacade.DocType.Disposizione)
                FacadeFactory.Instance.ResolutionLogFacade.Log(_resolution, ResolutionLogType.RP, String.Format("Aggiunta Frontalino. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, frontspieceChain))
            End If

            If Not _fileresolution.IdMainDocumentsOmissis.Equals(Guid.Empty) Then
                Dim oldchain As Guid = _fileresolution.IdMainDocumentsOmissis
                Dim frontspieceChainPrivacyChain As Guid = Guid.Empty

                Dim mainOmissisDocs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, oldchain)
                mainOmissisDocs = mainOmissisDocs.Where(Function(x) Not x.Name.Eq(ReslFrontalinoPrintPdfTO.FRONTALINO_PRIVACY_NAME)).ToList()
                For Each mainOmissisDoc As DocumentInfo In mainOmissisDocs
                    privacyFrontispieces.Add(mainOmissisDoc)
                Next
                ' Creo catena con frontalino
                frontspieceChainPrivacyChain = DocumentInfoFactory.ArchiveDocumentsInBiblos(privacyFrontispieces, _resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, frontspieceChainPrivacyChain)
                FacadeFactory.Instance.ResolutionFacade.SqlResolutionDocumentUpdate(_resolution.Id, frontspieceChainPrivacyChain, ResolutionFacade.DocType.DocumentoPrincipaleOmissis)
                FacadeFactory.Instance.ResolutionLogFacade.Log(_resolution, ResolutionLogType.RP, String.Format("Aggiunta Frontalino Omissis. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, frontspieceChainPrivacyChain))
            End If
        Else
            ' Verifico se la resolution ha già il documento
            If _fileresolution.IdAttachements.HasValue Then
                Dim oldchain As Integer = _fileresolution.IdAttachements.Value
                Dim attachmentChain As Integer = 0

                Dim attachments As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, oldchain)
                attachments = attachments.Where(Function(x) Not x.Name.Eq(ReslFrontalinoPrintPdfTO.FRONTALINO_NAME)).ToList()
                For Each attachment As DocumentInfo In attachments
                    frontispieces.Add(attachment)
                Next
                ' Creo catena con frontalino
                attachmentChain = DocumentInfoFactory.ArchiveDocumentsInBiblos(frontispieces, _resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, attachmentChain)
                FacadeFactory.Instance.ResolutionFacade.SqlResolutionDocumentUpdate(_resolution.Id, attachmentChain, ResolutionFacade.DocType.Allegati)
                FacadeFactory.Instance.ResolutionLogFacade.Log(_resolution, ResolutionLogType.RP, String.Format("Aggiunta Frontalino. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, attachmentChain))
            End If
        End If
    End Sub

    Public Sub AddUltimaPagina(ByVal file As String)
        'Dim docs As New List(Of UploadDocument)
        If Not IO.File.Exists(file) Then
            Throw New DocSuiteException("Errore aggiunta pagina", "Il documento non è presente: " + file)
        End If
        Dim info As New FileInfo(file)
        'Dim item As New UploadDocument("UltimaPagina.pdf", info)
        'docs.Add(item)

        Dim facade As New ResolutionFacade()
        Dim signature As String = facade.SqlResolutionGetNumber(_resolution.Id, , , , True)

        If _fileresolution.IdResolutionFile.HasValue Then
            If HasUltimaPagina() Then
                ' Sostituisco quella presente
                ReplaceUltimaPagina(file)
            Else
                Dim doc As New FileDocumentInfo(info)
                doc.Signature = signature
                doc.ArchiveInBiblos(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, CType(_fileresolution.IdResolutionFile, Integer))
            End If
        Else
            ' Non c'è il documento, non posso inserire nulla... eccezione
            Throw New DocSuiteException("Errore aggiunta pagina", "Catena documento non presente")
        End If

    End Sub

    Public Sub ReplaceUltimaPagina(ByVal path As String)
        If HasUltimaPagina() Then
            Dim oldchain As Integer = _fileresolution.IdResolutionFile.GetValueOrDefault(-1)

            Dim docs As IList(Of DocumentInfo) = CType(BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdResolutionFile.GetValueOrDefault(-1)), IList(Of DocumentInfo))

            ' Rimuovo l'ultima pagina
            Dim doc As DocumentInfo = New TempFileDocumentInfo(New FileInfo(path))
            doc.Name = docs(docs.Count - 1).Name
            docs(docs.Count - 1) = doc
            Dim idCatena As Integer = 0
            Dim facade As New ResolutionFacade()
            Dim signature As String = facade.SqlResolutionGetNumber(_resolution.Id, , , , True)
            ResolutionFacade.SaveBiblosDocuments(_resolution, docs, Nothing, signature, idCatena, 0)
            ' registrazione del id catena in db
            facade.SqlResolutionDocumentUpdate(_resolution.Id, idCatena, ResolutionFacade.DocType.Disposizione)

            Dim logger As New ResolutionLogFacade()
            logger.Log(_resolution, ResolutionLogType.RP, String.Format("Sostituzione Ultima Pagina. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, idCatena))
        End If
    End Sub

    Public Sub RemoveUltimaPagina()
        If HasUltimaPagina() Then
            Dim oldchain As Integer = _fileresolution.IdResolutionFile.GetValueOrDefault(-1)

            Dim docs As IList(Of DocumentInfo) = BiblosDocumentInfo.GetDocuments(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdResolutionFile)

            ' Rimuovo l'ultima pagina
            docs.RemoveAt(docs.Count - 1)
            Dim idCatena As Integer = 0
            Dim facade As New ResolutionFacade()
            Dim signature As String = facade.SqlResolutionGetNumber(_resolution.Id, , , , True)
            ResolutionFacade.SaveBiblosDocuments(_resolution, docs, Nothing, signature, idCatena, 0)
            ' registrazione del id catena in db
            facade.SqlResolutionDocumentUpdate(_resolution.Id, idCatena, ResolutionFacade.DocType.Disposizione)

            Dim logger As New ResolutionLogFacade()
            logger.Log(_resolution, ResolutionLogType.RP, String.Format("Rimozione Ultima Pagina. Vecchia catena: {0} - Nuova catena: {1}.", oldchain, idCatena))
        End If
    End Sub

    Public Sub ParcheggiaFrontalino(frontispieces As ICollection(Of ResolutionFrontispiece))
        Dim frontalini As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each frontispiece As ResolutionFrontispiece In frontispieces
            frontalini.Add(New TempFileDocumentInfo(New FileInfo(frontispiece.Path)) With {.Name = If(frontispiece.IsPrivacy, ReslFrontalinoPrintPdfTO.FRONTALINO_PRIVACY_NAME, ReslFrontalinoPrintPdfTO.FRONTALINO_NAME)})
        Next

        Dim idCatena As Integer = 0
        ResolutionFacade.SaveBiblosDocuments(_resolution, frontalini, Nothing, String.Empty, idCatena, 0)
        ' registrazione del id catena in db
        FacadeFactory.Instance.ResolutionFacade.SqlResolutionDocumentUpdate(_resolution.Id, idCatena, ResolutionFacade.DocType.Frontespizio)
        FacadeFactory.Instance.ResolutionLogFacade.Log(_resolution, ResolutionLogType.RP, String.Format("Frontalino Parcheggiato in catena {0}.", idCatena))
    End Sub

    Public Sub ParcheggiaUltimaPagina(ByVal file As String)
        Dim items As New List(Of DocumentInfo)
        Dim info As New FileInfo(file)
        Dim doc As New TempFileDocumentInfo(info) With {.Name = info.Name}
        items.Add(doc)
        Dim idCatena As Integer = 0
        ResolutionFacade.SaveBiblosDocuments(_resolution, items, Nothing, String.Empty, idCatena, 0)
        ' registrazione del id catena in db
        Dim facade As New ResolutionFacade()
        facade.SqlResolutionDocumentUpdate(_resolution.Id, idCatena, ResolutionFacade.DocType.UltimaPagina)

        Dim logger As New ResolutionLogFacade()
        logger.Log(_resolution, ResolutionLogType.RP, String.Format("Ultima Pagina Parcheggiata in catena {0}.", idCatena))
    End Sub

    ''' <summary> Accoda i documenti presenti nella catena lIdCatenaFrom alla catena lIdCatenaTo. </summary>
    ''' <param name="idCatenaFrom">Catena di lettura.</param>
    ''' <param name="idCatenaTo">Catena di scrittura</param>
    Private Sub AddToEndDocument(ByVal idCatenaFrom As Long, ByRef idCatenaTo As Long, ByVal first As Boolean)
        Dim chainFrom As New UIDChain(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, idCatenaFrom)
        Dim sources As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(chainFrom)
        Dim start As Integer = If(first, 0, 1)

        For i As Integer = start To sources.Count - 1
            Dim doc As BiblosDocumentInfo = sources(i)
            doc.ArchiveInBiblos(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, idCatenaTo)
        Next

    End Sub

    Public Function HasDocument() As Boolean
        Return _fileresolution.IdResolutionFile.HasValue
    End Function

    Public Function HasAttachments() As Boolean
        Return _fileresolution.IdAttachements.HasValue
    End Function

    Public Function HasFrontalino() As Boolean
        If Not _fileresolution.IdResolutionFile.HasValue Then
            Return False
        End If

        Dim documentName As String = Service.GetDocumentName(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdResolutionFile.Value, 0)

        Return documentName.ContainsIgnoreCase("frontalino.pdf")
    End Function

    Public Function IdAttachmentsHasFrontalino() As Boolean
        If Not _fileresolution.IdAttachements.HasValue Then
            Return False
        End If

        Dim documentName As String = Service.GetDocumentName(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdAttachements.Value, 0)

        Return documentName.ContainsIgnoreCase("frontalino.pdf")
    End Function

    Public Function HasUltimaPagina() As Boolean
        If Not _fileresolution.IdResolutionFile.HasValue Then
            Return False
        End If

        Dim documentName As String = Service.GetDocumentName(_resolution.Location.DocumentServer, _resolution.Location.ReslBiblosDSDB, _fileresolution.IdAttachements.Value, 0)

        Return documentName.ContainsIgnoreCase("ultimapagina")
    End Function

End Class
