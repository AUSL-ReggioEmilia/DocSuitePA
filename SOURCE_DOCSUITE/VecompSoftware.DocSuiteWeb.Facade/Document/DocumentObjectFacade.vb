Imports System
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

<ComponentModel.DataObject()> _
Public Class DocumentObjectFacade
    Inherits BaseDocumentFacade(Of DocumentObject, YearNumberIncrCompositeKey, NHibernateDocumentObjectDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function FolderHasDocumentObjectActive(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Boolean
        Return _dao.FolderHasDocumentObjectActive(YearNumberInc)
    End Function

    Public Function GetMaxId(ByVal year As Short, ByVal number As Integer) As Short
        Return _dao.GetMaxId(year, number)
    End Function

    ''' <summary> Lista di documenti con i versioning </summary>
    ''' <param name="id"> Chiave </param>
    Public Function GetObjectsWithVersioning(ByVal id As YearNumberIncrCompositeKey, ByVal listAllDocs As Boolean) As IList(Of DocumentObject)
        Return _dao.GetObjectsWithVersioning(id, listAllDocs)
    End Function

    ''' <summary> Numero dei DocumentObjects associati ad una DocumentFolder </summary>
    ''' <param name="YearNumberInc">Parametro di tipo YearNumberIncrCompositeKey della folder</param>
    Public Function FolderDocumentObjectCount(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer
        Return _dao.FolderDocumentObjectCount(YearNumberInc)
    End Function

    ''' <summary>
    ''' Scambia la posizione di due documentObject consecutivi
    ''' </summary>
    ''' <param name="SwapPosition">Indica se scambiare un docObject con quello in posizione precedente o successiva; valori ammessi: "MOVEUP" o "MOVEDOWN" </param>
    ''' <param name="YearNumberInc">Parametro di tipo YearNumberIncrCompositeKey della folder</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SwapDocObjectPosition(ByVal SwapPosition As String, ByVal YearNumberInc As YearNumberIncrCompositeKey) As Boolean

        Dim docObjOrigin As New DocumentObject
        Dim docObjDest As New DocumentObject
        Dim YearNumberIncDest As New YearNumberIncrCompositeKey

        Dim ordinalPositionOrigine As Short
        Dim ordinalPositionDestinazione As Short


        docObjOrigin = _dao.GetById(YearNumberInc, True)
        ordinalPositionOrigine = docObjOrigin.OrdinalPosition

        With YearNumberIncDest
            .Year = YearNumberInc.Year
            .Number = YearNumberInc.Number
            .Incremental = docObjOrigin.IncrementalFolder
        End With

        docObjDest = _dao.GetNextDocObjectPosition(SwapPosition, YearNumberIncDest, ordinalPositionOrigine)
        ordinalPositionDestinazione = docObjDest.OrdinalPosition

        Try
            docObjDest.OrdinalPosition = ordinalPositionOrigine
            docObjOrigin.OrdinalPosition = ordinalPositionDestinazione
            MyBase.Update(docObjOrigin)
            MyBase.Update(docObjDest)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Function GetDocumentCountOfDocumentFolder(ByVal YearNumberInc As YearNumberIncrCompositeKey) As Integer

        Return _dao.GetDocumentCountOfDocumentFolder(YearNumberInc)

    End Function

    Function GetDocumentObjectLink(ByVal idObjectType As String, ByVal link As String) As IList(Of DocumentObject)
        Return _dao.GetDocumentObjectLink(idObjectType, link)
    End Function

    ''' <summary>
    ''' Aggiorna lo stato del DocumentObject impostanto IdObjectStatus al valore "A"
    ''' </summary>
    ''' <param name="documentObject">DocumentObject di cui aggiornare lo stato</param>
    Public Sub UpdateStatus(ByRef documentObject As DocumentObject)
        documentObject.idObjectStatus = "A"

        Me.Update(documentObject)
    End Sub

    ''' <summary> Aggiorna la descrizione del DocumentObject </summary>
    Public Sub UpdateDescription(ByRef documentObject As DocumentObject, ByVal documentDate As Nullable(Of Date), ByVal [object] As String, ByVal reason As String, ByVal note As String)
        documentObject.DocObject = [object]
        documentObject.Reason = reason
        documentObject.Note = note
        documentObject.DocumentDate = documentDate

        Me.Update(documentObject)
    End Sub

    ''' <summary> Aggiorna l'incrementalFolder del DocumentObject </summary>
    ''' <param name="documentObject">DocumentObject da aggiornare</param>
    ''' <param name="newIncrementalFolder">Nuova IncrementalFolder</param>
    ''' <remarks></remarks>
    Public Sub UpdateFolder(ByRef documentObject As DocumentObject, ByVal newIncrementalFolder As Short)
        documentObject.IncrementalFolder = newIncrementalFolder

        Me.Update(documentObject)
    End Sub

    Function GetVersionedDocumentObjects(ByVal year As Short, ByVal number As Integer, ByVal incremental As Short) As IList(Of DocumentObject)
        Return _dao.GetVersionedDocumentObjects(year, number, incremental)
    End Function

    ''' <summary>
    ''' Verifica l'esistenza di link associati al documentObject
    ''' </summary>
    ''' <param name="Link">il link da controllare</param>
    ''' <param name="IncrementalFolder"></param>
    ''' <returns>true se trova dei link, false othewise</returns>
    ''' <remarks></remarks>
    Function DocumentObjectVerifyLink(ByVal Year As Short, ByVal Number As Integer, ByVal Link As String, Optional ByVal IncrementalFolder As Short = 0) As Boolean
        Return _dao.DocumentObjectVerifyLink(Year, Number, Link, IncrementalFolder)
    End Function
    ''' <summary>
    ''' Ritorna un document object in checkout
    ''' </summary>
    ''' <param name="year"></param>
    ''' <param name="number"></param>
    ''' <param name="incremental"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CheckDocObjectCheckedOut(ByVal year As Short, ByVal number As Integer, ByVal incremental As Integer) As Boolean
        Return _dao.CheckDocObjectCheckedOut(year, number, incremental)
    End Function

    ''' <summary>
    ''' POTREBBE ESSERE UNA CAGATA PAZZESCA MA PROVIAMO (MP)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetNHibernateDomainObjectFinder(ByVal dbType As String) As NHibernateDomainObjectFinder(Of DocumentObject)
        Return New NHibernateDomainObjectFinder(Of DocumentObject)(Me.CurrentCriteria, dbType)
    End Function

    ''' <summary> Inserisce il documento nella pratica corrente </summary>
    Public Function InsertDocumentObject(documentObject As DocumentObject, location As Location, ByVal doc As DocumentInfo, documentToken As IList(Of DocumentToken)) As DocumentObject
        If Not doc.Exists() Then
            Throw New DocSuiteException("Inserimento Documento", String.Format("Documento [{0}] non valido. Reinserire il Documento", doc.Name))
        End If

        If documentToken.IsNullOrEmpty() Then
            Throw New DocSuiteException("Inserimento Documento", "Errore in lettura Step attivo della Pratica.")
        End If

        Dim idBiblos As Integer
        Try
            idBiblos = doc.ArchiveInBiblos(location.DocmBiblosDSDB).BiblosChainId
        Catch ex As Exception
            Throw New DocSuiteException("Inserimento Documento", String.Format("Impossibile inserire documento nella pratica [{0}] su archivio [{1}-{2}]", documentObject.Id.ToString(), location.DocmBiblosDSDB), ex)
        End Try

        ' Registrazione
        With documentObject
            .Incremental = GetMaxId(.Year, .Number)
            .OrdinalPosition = .Incremental
            .Description = doc.Name
            .idBiblos = idBiblos
            .RegistrationDate = DateTime.Now
            .Link = ""
        End With
        Save(documentObject)
        Return documentObject
    End Function

End Class

