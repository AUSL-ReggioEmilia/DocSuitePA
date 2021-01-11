Imports System.Collections.Generic
Imports System.IO
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.Compress

Public Class CommExport
    Inherits BaseCommExport

    Private _protocolIds As IList(Of Guid)
    Public Shared Function InitializeExportTask(listID As IList(Of Guid)) As String

        If listID.Count <= 0 Then
            Return "Nessun protocollo selezionato"
        End If

        Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask

        If task.Running Then
            Return String.Concat("Task già in esecuzione dall'utente '", task.TaskUser, "'")
        End If

        'Recupero il path dalla ParamenterEnv
        Dim path As String = DocSuiteContext.Current.ProtocolEnv.ExportPath
        path = String.Concat(path.TrimEnd("\"c), "\", DocSuiteContext.Current.User.UserName, "\")

        'Impersonifico l'amministratore
        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()

            'Se non esiste già, la creo
            If Not Directory.Exists(path) Then
                Directory.CreateDirectory(path)

                Dim fACL As New FileACL.Security
                fACL.SetFileACL(path, String.Concat(CommonUtil.UserFullName, ";", DocSuiteContext.Current.CurrentTenant.DomainUser))

            End If

        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, ex.Message, ex)
            Throw
        Finally
            impersonator.ImpersonationUndo()
        End Try


        'Esporto i protocolli
        Dim export As New CommExport(path)
        If Not export.ExportDocuments(listID) Then
            Return "Nessun protocollo selezionato"
        End If
        Return String.Empty
    End Function
    Public Function ExportDocuments(ByVal ids As IList(Of Guid)) As Boolean
        _protocolIds = ids

        If _protocolIds.Count > 0 Then
            CommonUtil.SaveThreadContext()

            _exported = 0
            _errors = 0
            Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask
            task.TaskUser = DocSuiteContext.Current.User.UserName
            task.StepsCount = _protocolIds.Count
            task.SetCurrentFileName = New MultiStepLongRunningTask.SetCurrentFileNameDelegate(AddressOf SetCurrentYearNumber)
            task.TaskToExecute = New MultiStepLongRunningTask.TaskToExec(AddressOf ExportProtocolDocument)
            task.RunTask()

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub ExportDocuments(protocol As Protocol, documents As ICollection(Of BiblosDocumentInfo), documentAliasType As String)
        Try
            If documents Is Nothing Then
                Return
            End If

            Dim docName As String = String.Empty
            Dim index As Integer = 0
            For Each doc As BiblosDocumentInfo In documents
                Try
                    index += 1
                    docName = FacadeFactory.Instance.ProtocolFacade.GetExportFileName(protocol, $"{documentAliasType}-{index}", doc.Name)
                    docName = String.Concat(docName, doc.Extension)
                    Dim directoryOutput As DirectoryInfo = New DirectoryInfo(_dirOutput)
                    Try
                        doc.SavePdf(directoryOutput, String.Concat(docName, FileHelper.PDF), String.Empty)
                    Catch ex As Exception
                        doc.SaveToDisk(directoryOutput, docName)
                    End Try
                    _exported += 1
                Catch ex As Exception
                    FacadeFactory.Instance.UserErrorFacade.Insert("BiblosDSExtract", $"File: {doc.Name}{vbCrLf}{ex.Message}") 'Inserisco il record nella tabella degli errori
                    FileLogger.Warn(LogName.FileLog, "BiblosDSExtract", ex)
                    _errors += 1
                End Try
            Next

        Catch exx As Exception
            FacadeFactory.Instance.UserErrorFacade.Insert("BiblosDSExtract", exx.Message) 'Inserisco il record nella tabella degli errori
            FileLogger.Warn(LogName.FileLog, "BiblosDSExtract", exx)
            _errors += 1
        End Try
    End Sub

    Private Sub ExportProtocolDocument(ByVal CurrentStep As Integer)
        Dim protocolId As Guid = _protocolIds(CurrentStep)
        Dim prot As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(protocolId)
        If prot.IdDocument.HasValue Then
            ExportMainDocument(prot)

            'Esporto gli allegati
            If prot.IdAttachments.HasValue AndAlso prot.IdAttachments.Value > 0 Then
                ExportProtocolAttachments(prot)
            End If

            If Not prot.IdAnnexed.Equals(Guid.Empty) Then
                ExportProtocolAnnexed(prot)
            End If
        End If
    End Sub

    Private Sub ExportMainDocument(prot As Protocol)
        Dim archive As String = prot.Location.ProtBiblosDSDB
        Dim doc As BiblosDocumentInfo = New BiblosDocumentInfo(archive, prot.IdDocument.Value)
        ExportDocuments(prot, New List(Of BiblosDocumentInfo) From {doc}, "P")
    End Sub

    Private Sub ExportProtocolAttachments(prot As Protocol)
        Dim archive As String = prot.Location.ProtBiblosDSDB
        Dim idAttachments As Integer = prot.IdAttachments.Value
        Dim attachments As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(archive, idAttachments)
        ExportDocuments(prot, attachments, "P-ALL")
    End Sub

    Private Sub ExportProtocolAnnexed(prot As Protocol)
        Dim idAnnexed As Guid = prot.IdAnnexed
        Dim annexed As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(idAnnexed)
        ExportDocuments(prot, annexed, "P-ANX")
    End Sub

    ''' <summary>
    ''' Routine che recupera il numero di protocollo
    ''' dato il suo numero nella coda
    ''' </summary>
    ''' <param name="currentStep">Numero nella coda</param>
    ''' <returns>Nome del file</returns>
    Private Function SetCurrentYearNumber(ByVal currentStep As Integer) As String

        If (currentStep < 0 Or currentStep > _protocolIds.Count - 1) Then
            Return String.Empty
        End If

        Return _protocolIds(currentStep).ToString()

    End Function

#Region "ctor"
    Public Sub New(Optional ByVal dirOutput As String = "")
        MyBase.New(dirOutput)
    End Sub
#End Region

End Class

