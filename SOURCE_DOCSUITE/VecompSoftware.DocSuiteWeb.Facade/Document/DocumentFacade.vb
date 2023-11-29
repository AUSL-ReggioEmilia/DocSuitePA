Imports System
Imports System.Linq
Imports VecompSoftware.Helpers
Imports NHibernate.Util
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class DocumentFacade
    Inherits BaseDocumentFacade(Of Document, YearNumberCompositeKey, NHibernateDocumentDao)

    Public Const BIBLOS_ATTRIBUTE_IsPublic As String = "IsPublic"
#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Overloads Function GetById(ByVal year As Short, ByVal number As Integer, Optional ByVal shoudLock As Boolean = False) As Document
        Dim id As New YearNumberCompositeKey(year, number)
        Return GetById(id, shoudLock)
    End Function

    Public Function GenerateSignature(ByRef document As Document) As String
        Dim vEnv As DocumentEnv = DocSuiteContext.Current.DocumentEnv
        Dim vSignature As String = String.Format("{0} {1} del {2:dd/MM/yyyy}", vEnv.Signature, document.Id.ToString(), document.RegistrationDate)

        Select Case vEnv.SignatureType
            Case 0
            Case 1
                vSignature = vEnv.CorporateAcronym & " " & vSignature
            Case 2
                vSignature = vEnv.CorporateAcronym & " " & vSignature & " " & document.Container.Name
            Case Else
                vSignature = ""
        End Select

        Return vSignature
    End Function

    Public Sub AddDocumentContact(ByRef currentDocument As Document, ByVal contact As Contact)
        Dim docDao As New NHibernateDocumentContactDao(_dbName)

        Dim docContact As New DocumentContact()
        docContact.Contact = contact
        docContact.Incremental = CType(currentDocument.Contacts.Count(), Short)
        docContact.Document = currentDocument
        docDao.ConnectionName = _dbName
        docDao.Save(docContact)
        currentDocument.Contacts.Add(docContact)
    End Sub

    Public Sub RemoveDocumentContact(ByRef currentDocument As Document)

        Dim docDao As New NHibernateDocumentContactDao(_dbName)
        docDao.ConnectionName = _dbName

        If currentDocument.Contacts IsNot Nothing Then
            For Each dc As DocumentContact In currentDocument.Contacts
                docDao.Delete(dc)
            Next
        End If
    End Sub

    Function GetDocumentProtocol(ByVal arrContacts As String) As IList(Of DocumentContact)
        Return _dao.GetDocumentProtocol(arrContacts)
    End Function

    Function GetDocuments(ByVal keyList As IList(Of YearNumberCompositeKey)) As IList(Of Document)
        Return _dao.GetDocuments(keyList)
    End Function

    Function GetExpiryFolders(ByRef document As Document) As IList(Of DocumentFolder)
        Return _dao.GetExpiryFolders(document)
    End Function

    ''' <summary> Restituisce le pratiche per la conservazione nell'archivio Biblos preposto</summary>
    Public Function GetConservationDocumentsToExport(ByVal ToDate As DateTime) As IList(Of Document)
        Return _dao.GetConservationDocuments(ToDate)
    End Function

    Public Function GetByServiceNumber(ByVal serviceNumber As String) As IList(Of Document)
        Return _dao.GetByServiceNumber(serviceNumber)
    End Function

    ''' <summary> Chiusura pratica </summary>
    Public Sub Close(ByRef currentDocument As Document, ByVal endDate As Date?, ByVal note As String)
        Dim factory As FacadeFactory = New FacadeFactory("DocmDB")
        'Aggiornamento dello stato
        Dim status As DocumentTabStatus = factory.DocumentTabStatusFacade.GetById("CP")
        If status IsNot Nothing Then
            currentDocument.Status = status
            currentDocument.EndDate = If(endDate.HasValue, endDate.Value, currentDocument.EndDate)
            currentDocument.Note = note
            Update(currentDocument)

            'inserimento documenttoken
            Dim documenttokens As IList(Of DocumentToken) = factory.DocumentTokenFacade.GetDocumentTokenRoleP(currentDocument.Year, currentDocument.Number)
            Dim DocumentToken As DocumentToken = factory.DocumentTokenFacade.CreateDocumentToken(currentDocument.Year, currentDocument.Number)
            With DocumentToken
                .IncrementalOrigin = 0
                .IsActive = False
                .Response = String.Empty
                .DocStep = documenttokens(0).DocStep
                .SubStep = documenttokens(0).SubStep
                .DocumentTabToken = factory.DocumentTabTokenFacade.GetById("MC")
                .RoleSource = factory.RoleFacade.GetById(documenttokens(0).RoleSource.Id)
                .RoleDestination = factory.RoleFacade.GetById(documenttokens(0).RoleDestination.Id)
                .OperationDate = Date.Now
                .Note = note
            End With
            factory.DocumentTokenFacade.Save(DocumentToken)
        Else
            Throw New DocSuiteException("Chiusura pratica", "Stato [Chiusura] non trovato su Database.")
        End If
    End Sub

    ''' <summary> Riapertura pratica </summary>
    Public Sub ReOpen(ByRef currentDocument As Document, ByVal restartDate As Date?, ByVal note As String)
        Dim factory As FacadeFactory = New FacadeFactory("DocmDB")
        'Aggiornamento dello stato
        Dim status As DocumentTabStatus = factory.DocumentTabStatusFacade.GetById("RP")
        If status IsNot Nothing Then
            currentDocument.Status = status
            currentDocument.ReStartDate = If(restartDate.HasValue, restartDate.Value, currentDocument.ReStartDate)
            currentDocument.Note = note
            Update(currentDocument)

            'inserimento documenttoken
            Dim documenttokens As IList(Of DocumentToken) = factory.DocumentTokenFacade.GetDocumentTokenRoleP(currentDocument.Year, currentDocument.Number)
            Dim documentToken As DocumentToken = factory.DocumentTokenFacade.CreateDocumentToken(currentDocument.Year, currentDocument.Number)
            With documentToken
                .IncrementalOrigin = 0
                .IsActive = False
                .Response = String.Empty
                .DocStep = documenttokens(0).DocStep
                .SubStep = documenttokens(0).SubStep
                .DocumentTabToken = factory.DocumentTabTokenFacade.GetById("MT")
                .RoleSource = documenttokens(0).RoleSource
                .RoleDestination = documenttokens(0).RoleDestination
                .OperationDate = Date.Now
                .Note = note
            End With
            factory.DocumentTokenFacade.Save(documentToken)
        Else
            Throw New DocSuiteException("Riapertura pratica", "Stato [RiApertura] non trovato su Database.")
        End If
    End Sub

    ''' <summary> Annulla una pratica </summary>
    ''' <param name="currentDocument">Pratica da annullare</param>
    ''' <param name="lastChangedReason">Causale di annullamento</param>
    Public Sub Cancel(ByRef currentDocument As Document, ByVal lastChangedReason As String)
        Dim factory As FacadeFactory = New FacadeFactory("DocmDB")
        Dim status As DocumentTabStatus = factory.DocumentTabStatusFacade.GetById("PA")
        If status IsNot Nothing Then
            currentDocument.Status = status
            currentDocument.LastChangedReason = lastChangedReason
            UpdateOnly(currentDocument)
            factory.DocumentLogFacade.Insert(currentDocument.Year, currentDocument.Number, "PA", lastChangedReason)
        Else
            Throw New DocSuiteException("Annullamento pratica", "Stato [Annullamento] non trovato su Database.")
        End If
    End Sub

    ''' <summary> Archivia una pratica </summary>
    ''' <param name="currentDocument">Pratica da archiviare</param>
    ''' <param name="lastChangedReason">Causale di annullamento</param>
    Public Sub Archive(ByRef currentDocument As Document, ByVal archiveDate As Date?, ByVal lastChangedReason As String)
        Dim factory As FacadeFactory = New FacadeFactory("DocmDB")
        'Aggiornamento dello stato
        Dim status As DocumentTabStatus = factory.DocumentTabStatusFacade.GetById("AR")
        If status IsNot Nothing Then
            currentDocument.Status = status
            currentDocument.ArchiveDate = archiveDate
            currentDocument.Note = lastChangedReason
            UpdateOnly(currentDocument)
            factory.DocumentLogFacade.Insert(currentDocument.Year, currentDocument.Number, "AR", lastChangedReason)
        Else
            Throw New DocSuiteException("Archiviazione pratica", "Stato [Archiviazione] non trovato su Database.")
        End If
    End Sub


    ''' <summary> Permette di cambiare di stato una Pratica </summary>
    Public Function UpdateStatus(ByRef currentDocument As Document, ByVal newStatus As String, ByVal statusDate As DateTime, ByVal lastChangedReason As String, Optional ByVal appendReason As Boolean = False) As Boolean
        If appendReason Then
            lastChangedReason = If(newStatus = "PA", currentDocument.LastChangedReason, currentDocument.Note) & " - " & lastChangedReason
        End If
        Select Case newStatus
            Case "PA"
                Cancel(currentDocument, lastChangedReason)
            Case "CP"
                Close(currentDocument, statusDate, lastChangedReason)
            Case "RP"
                ReOpen(currentDocument, statusDate, lastChangedReason)
            Case "AR"
                Archive(currentDocument, statusDate, lastChangedReason)
            Case Else
                Throw New DocSuiteException("Aggiornamento stato pratica", String.Format("Stato [{0}] non disponibile.", newStatus))
        End Select
    End Function

    Public Function CheckPrivacy(biblosDocInfo As DocumentInfo, idContainer As Integer, roleIds As Guid(), privacyRoleIds As Guid(), env As DSWEnvironment, isUserAuthorized As Boolean) As Boolean
        If Not DocSuiteContext.Current.PrivacyEnabled Then
            Return True
        End If

        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso env.Equals(DSWEnvironment.Protocol) Then

            If Factory.ContainerGroupFacade.HasContainerRight(idContainer, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName, ProtocolContainerRightPositions.View, env) OrElse
                isUserAuthorized OrElse (((roleIds IsNot Nothing AndAlso roleIds.Count > 0) OrElse (privacyRoleIds IsNot Nothing AndAlso privacyRoleIds.Count > 0)) AndAlso
                                          Factory.RoleFacade.CheckCurrentUserPrivacyRoles(roleIds, privacyRoleIds, env)) Then
                Return True
            End If

            Return False
        End If

        Dim privacyLevel As String = biblosDocInfo.Attributes.Where(Function(x) x.Key.Eq(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)).FirstOrDefault().Value
        Dim documentPrivacyLevel As Integer = Convert.ToInt32(privacyLevel)
        Dim maxPrivacyLevel As Integer = Factory.ContainerGroupFacade.GetMaxPrivacyLevel(idContainer, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName)
        If maxPrivacyLevel >= documentPrivacyLevel Then
            Return True
        End If
        Return False
    End Function

    Public Function CheckPrivacyLevel(biblosDocInfo As List(Of DocumentInfo), container As Container) As Boolean
        If Not DocSuiteContext.Current.PrivacyLevelsEnabled OrElse (container IsNot Nothing AndAlso Not container.PrivacyEnabled) Then
            Return False
        End If
        Return biblosDocInfo.SelectMany(Function(f) f.Attributes.Where(Function(x) x.Key.Eq(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) AndAlso x.Value IsNot Nothing AndAlso Not x.Value.Eq("0"))).
                             Any(Function(f) container.PrivacyLevel > Convert.ToInt32(f.Value))

    End Function

    Public Function FilterAllowedDocuments(documents As IList(Of DocumentInfo), idContainer As Integer, roleIds As Guid(), privacyRoleIds As Guid(), env As DSWEnvironment, isUserAthorized As Boolean) As List(Of DocumentInfo)
        If Not DocSuiteContext.Current.PrivacyEnabled Then
            Return documents.ToList()
        End If

        Dim allowedDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each doc As DocumentInfo In documents
            If doc.Attributes.Any(Function(a) a.Key.Eq(BIBLOS_ATTRIBUTE_IsPublic)) AndAlso doc.Attributes(BIBLOS_ATTRIBUTE_IsPublic).Eq("True") Then
                allowedDocuments.Add(doc)
                Continue For
            End If

            If CheckPrivacy(doc, idContainer, roleIds, privacyRoleIds, env, isUserAthorized) Then
                allowedDocuments.Add(doc)
            End If
        Next
        Return allowedDocuments
    End Function

#Region "WsDocm"
    Public Function GetDataForWs(ByVal filters As Dictionary(Of String, Object), ByVal maxResults As Integer) As IList(Of DocumentHeader)
        'Inizializzo il Finder
        Dim finder As New NHibernateDocumentFinder("DocmDB")

        'Aggiungo i filtri al finder
        If filters.Any() Then
            For Each filter As KeyValuePair(Of String, Object) In filters
                ReflectionHelper.SetProperty(finder, filter.Key, filter.Value)
            Next
        End If

        'Setto le impostazioni di default del finder
        finder.EnablePaging = maxResults > 0
        finder.PageSize = maxResults
        finder.IsWebService = True
        finder.EnableTableJoin = True

        'Chiamata al finder che effettua la ricerca
        Return CType(finder.DoSearchHeader(), IList(Of DocumentHeader))
    End Function
#End Region

#End Region
End Class