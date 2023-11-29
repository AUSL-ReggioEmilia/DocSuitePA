Imports System
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports VecompSoftware.Services.Biblos

<DataObject()>
Public Class ResolutionWorkflowFacade
    Inherits BaseResolutionFacade(Of ResolutionWorkflow, ResolutionWorkflowCompositeKey, NHibernateResolutionWorkflowDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetMaxIncremental(ByVal idresolution As Integer) As Short
        Return _dao.GetMaxIncremental(idresolution) + 1S
    End Function

    ''' <summary> Torna l'incrementale massimo correntemente legato all'atto </summary>
    ''' <param name="idresolution">Atto per il quale ritirare l'incrementale</param>
    ''' <param name="isactive">Indica se cercare tra gli incrementali attivi o meno</param>
    ''' <returns>Se non trova nulla torna 0</returns>
    Public Function GetActiveIncremental(ByVal idresolution As Integer, ByVal isactive As Short) As Short
        Return _dao.GetActiveIncremental(idresolution, isactive)
    End Function

    ''' <summary> Torna lo step massimo legato all'atto. </summary>
    ''' <param name="idresolution">Atto per il quale ritirare l'incrementale</param>
    Public Function GetActiveStep(ByVal idResolution As Integer) As Short
        Return _dao.GetActiveStep(idResolution)
    End Function

    Public Function SqlResolutionWorkflowSearch(ByVal idResolution As Integer, ByVal searchStep As Short, Optional ByVal activeStep As Boolean = True) As ResolutionWorkflow
        Return _dao.SqlResolutionWorkflowSearch(idResolution, searchStep, activeStep)
    End Function

    Public Function GetAllByResolution(ByVal idResolution As Integer, Optional ByVal activeStep As Boolean = False) As IList(Of ResolutionWorkflow)
        Return _dao.GetAllByResolution(idResolution, activeStep)
    End Function

    Public Function GetDocumentName(header As ResolutionHeader) As String
        If Not String.IsNullOrEmpty(header.AdoptedDocumentName) Then
            Return header.AdoptedDocumentName
        End If
        If Not header.AdoptedDocument.HasValue Then
            Return String.Empty
        End If
        Try
            Dim biblosDocumentName As String = Service.GetDocumentName(header.ProxiedLocation.ReslBiblosDSDB, header.AdoptedDocument.Value, 0)
            If String.IsNullOrEmpty(biblosDocumentName) Then
                Return String.Empty
            End If

            Dim rw As ResolutionWorkflow = GetByResolutionAndDescription(header.Id, header.WorkflowType, WorkflowStep.ADOZIONE)
            If rw IsNot Nothing Then
                rw.DocumentName = biblosDocumentName
                UpdateOnly(rw)
            End If
            Return biblosDocumentName
        Catch ex As Exception
            ' Se non riesco a recuperare il nome del documento da biblos evito comunque di bloccare l'esecuzione.
            FileLogger.Warn(LoggerName, String.Format("Errore ritiro nome documento adottato [{0}].", header.AdoptedDocument), ex)
            Return String.Empty
        End Try
    End Function

    Public Function GetDocumentName(ByVal idResolution As Integer, ByVal workflowType As String) As String
        Dim rw As ResolutionWorkflow = GetByResolutionAndDescription(idResolution, workflowType, WorkflowStep.ADOZIONE)
        If rw Is Nothing Then
            Return String.Empty
        End If

        If Not String.IsNullOrEmpty(rw.DocumentName) Then
            Return rw.DocumentName
        End If
        If Not rw.Document.HasValue Then
            Return String.Empty
        End If

        Dim biblosDocumentName As String = Service.GetDocumentName(rw.Resolution.Location.ReslBiblosDSDB, rw.Document.Value, 0)
        If String.IsNullOrEmpty(biblosDocumentName) Then
            Return String.Empty
        End If

        If rw IsNot Nothing Then
            rw.DocumentName = biblosDocumentName
            UpdateOnly(rw)
        End If
        Return biblosDocumentName
    End Function

    Public Function GetByResolutionAndDescription(idResolution As Integer, workflowType As String, workflowStep As String) As ResolutionWorkflow
        Return _dao.GetByResolutionAndDescription(idResolution, workflowType, workflowStep)
    End Function

    ''' <summary> Aggiorna lo step corrente e aggiunge il successivo aggiornando il log </summary>
    Public Overloads Function InsertNextStep(ByVal idResolution As Integer, ByVal activeStep As Short, ByVal idDocument As Integer, ByVal idAttachment As Integer, ByVal idPrivacyAttachments As Integer, ByVal idAnnexed As Guid, ByVal idDocumentsOmissis As Guid, ByVal idAttachmentsOmissis As Guid, registrationUser As String, Optional documentName As String = "") As Boolean
        Dim incFather As Short = GetActiveIncremental(idResolution, 1)
        Dim idwf As New ResolutionWorkflowCompositeKey

        '--update
        idwf.IdResolution = idResolution
        idwf.Incremental = incFather
        Dim rw As ResolutionWorkflow = GetById(idwf)
        If rw IsNot Nothing Then
            rw.IsActive = 0S
            UpdateOnly(rw)
        End If

        '--insert
        Dim rwNext As New ResolutionWorkflow
        rwNext.Id.IdResolution = idResolution
        rwNext.Id.Incremental = GetMaxIncremental(idResolution)
        rwNext.ResStep = activeStep + 1S
        rwNext.IsActive = 1S

        ' Documento
        If Not idDocument.Equals(0) Then
            rwNext.Document = idDocument
        End If
        If Not String.IsNullOrEmpty(documentName) Then
            rwNext.DocumentName = documentName
        End If

        ' Documento Omissis
        If Not idDocumentsOmissis.Equals(Guid.Empty) Then
            rwNext.DocumentsOmissis = idDocumentsOmissis
        End If

        ' Allegati
        If Not idAttachment.Equals(0) Then
            rwNext.Attachment = idAttachment
        End If

        ' Allegati Omissis
        If Not idAttachmentsOmissis.Equals(Guid.Empty) Then
            rwNext.AttachmentsOmissis = idAttachmentsOmissis
        End If

        ' Allegati Privacy
        If Not idPrivacyAttachments.Equals(0) Then
            rwNext.PrivacyAttachment = idPrivacyAttachments
        End If

        ' Annessi
        If Not idAnnexed.Equals(Guid.Empty) Then
            rwNext.Annexed = idAnnexed
        End If

        rwNext.RegistrationUser = registrationUser
        rwNext.RegistrationDate = DateTime.Now
        Save(rwNext)
        If rw IsNot Nothing Then
            rwNext.Parent = rw
            rwNext.IncrementalFather = rw.Id.Incremental
            Update(rwNext)

            Dim activeWorkflowStep As TabWorkflow = Nothing
            Dim nextWorkflowStep As TabWorkflow = Nothing
            Dim currentResolution As Resolution
            Try
                currentResolution = Factory.ResolutionFacade.GetById(idResolution)
                Dim facade As New FacadeFactory("ReslDB")
                facade.TabWorkflowFacade.GetByStep(currentResolution.WorkflowType, activeStep, activeWorkflowStep)
                facade.TabWorkflowFacade.GetByStep(currentResolution.WorkflowType, activeStep + 1S, nextWorkflowStep)
                Factory.ResolutionLogFacade.Insert(Factory.ResolutionFacade.GetById(idResolution), ResolutionLogType.RF, String.Format("ATTI.STEP: Avanzamento del workflow, da ""{0}"" a ""{1}"", step attivo:{2} catene DOC:{3}, ALL:{4}, RIS:{5}, ANNESSI:{6}", activeWorkflowStep.CustomDescription, nextWorkflowStep.CustomDescription, activeStep, idDocument, idAttachment, idPrivacyAttachments, idAnnexed))
            Catch ex As Exception
                FileLogger.Warn(LoggerName, String.Format("ATTI.STEP: Avanzamento del workflow, da ""{0}"" a ""{1}"", step attivo:{2} catene DOC:{3}, ALL:{4}, RIS:{5}, ANNESSI:{6}", activeWorkflowStep.CustomDescription, nextWorkflowStep.CustomDescription, activeStep, idDocument, idAttachment, idPrivacyAttachments, idAnnexed), ex)
            End Try
        End If
        NHibernateSessionManager.Instance.GetSessionFrom(_dbName).Evict(rwNext)

        Return True
    End Function

    ''' <summary> Riabilita lo step precedente </summary>
    Public Function EnablePreviousStep(ByVal idResolution As Integer, ByVal activeStep As Short) As Boolean
        Dim activeInc As Short = GetActiveIncremental(idResolution, 1S)
        If activeInc = 0S Then
            Return False
        End If

        Dim idwf As New ResolutionWorkflowCompositeKey
        idwf.IdResolution = idResolution
        idwf.Incremental = activeInc
        Dim rw As ResolutionWorkflow = GetById(idwf)
        If rw Is Nothing Then
            Return False
        End If

        rw.IsActive = 2S
        rw.LastChangedUser = DocSuiteContext.Current.User.FullUserName
        rw.LastChangedDate = DateTimeOffset.UtcNow
        Factory.ResolutionWorkflowFacade.Update(rw)

        idwf.IdResolution = idResolution
        idwf.Incremental = rw.Parent.Id.Incremental

        Dim rw2 As ResolutionWorkflow = GetById(idwf)
        Dim bRet As Boolean = False
        If Not rw2 Is Nothing Then

            rw2.IsActive = 1S

            rw2.LastChangedUser = DocSuiteContext.Current.User.FullUserName
            rw2.LastChangedDate = DateTimeOffset.UtcNow
            Factory.ResolutionWorkflowFacade.Save(rw2)

            bRet = True

            Factory.ResolutionLogFacade.Log(Factory.ResolutionFacade.GetById(idResolution), ResolutionLogType.RU, "ATTI.STEP.DEL: Retrocessione di step.")

        End If

        Return bRet
    End Function

    ''' <summary> Salvataggio in ResolutionWorkflow </summary>
    Public Overloads Function SqlResolutionWorkflowUpdate(ByVal idResolution As Integer, ByVal idDocument As Integer, ByVal idAttachment As Integer) As Boolean
        Return SqlResolutionWorkflowUpdate(idResolution, idDocument, idAttachment, 0)
    End Function

    ''' <summary> Salvataggio in ResolutionWorkflow </summary>
    Public Overloads Function SqlResolutionWorkflowUpdate(ByVal idResolution As Integer, ByVal idDocument As Integer, ByVal idAttachment As Integer, ByVal idPrivacyAttachment As Integer) As Boolean
        Return SqlResolutionWorkflowUpdate(idResolution, idDocument, idAttachment, idPrivacyAttachment, Guid.Empty, Guid.Empty, Guid.Empty)
    End Function

    ''' <summary> Salvataggio in ResolutionWorkflow </summary>
    Public Overloads Function SqlResolutionWorkflowUpdate(ByVal idResolution As Integer, ByVal idDocument As Integer, ByVal idAttachment As Integer, ByVal idPrivacyAttachment As Integer, ByVal idAnnexes As Guid, ByVal idDocumentsOmissis As Guid, ByVal idAttachmentsOmissis As Guid) As Boolean
        Dim activeInc As Short = GetActiveIncremental(idResolution, 1S)
        Dim idwf As New ResolutionWorkflowCompositeKey

        If activeInc = 0S Then
            Return True
        End If

        idwf.IdResolution = idResolution
        idwf.Incremental = activeInc
        Dim rw As ResolutionWorkflow = GetById(idwf)
        If rw Is Nothing Then
            Return True
        End If

        ' Documento
        rw.Document = Nothing
        If Not idDocument.Equals(0) Then
            rw.Document = idDocument
        End If

        ' Documenti Omissis
        rw.DocumentsOmissis = Nothing
        If Not idDocumentsOmissis.Equals(Guid.Empty) Then
            rw.DocumentsOmissis = idDocumentsOmissis
        End If

        ' Allegati
        rw.Attachment = Nothing
        If Not idAttachment.Equals(0) Then
            rw.Attachment = idAttachment
        End If

        ' Allegati Omissis
        rw.AttachmentsOmissis = Nothing
        If Not idAttachmentsOmissis.Equals(Guid.Empty) Then
            rw.AttachmentsOmissis = idAttachmentsOmissis
        End If

        ' Allegati Privacy
        rw.PrivacyAttachment = Nothing
        If Not idPrivacyAttachment.Equals(0) Then
            rw.PrivacyAttachment = idPrivacyAttachment
        End If

        ' Annessi
        rw.Annexed = Guid.Empty
        If Not idAnnexes.Equals(Guid.Empty) Then
            rw.Annexed = idAnnexes
        End If

        rw.LastChangedUser = DocSuiteContext.Current.User.FullUserName
        rw.LastChangedDate = DateTimeOffset.UtcNow

        Save(rw)

        Factory.ResolutionLogFacade.Log(Factory.ResolutionFacade.GetById(idResolution), ResolutionLogType.RM, String.Format("ATTI.STEP.UPDATE: Aggiornato il workflow con le catene DOC:{0}, ALL:{1}, RIS:{2}.", idDocument, idAttachment, idAttachment))

        Return True
    End Function

#End Region

End Class