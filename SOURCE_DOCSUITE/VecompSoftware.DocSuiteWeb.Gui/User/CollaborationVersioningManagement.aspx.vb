Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Imports VecompSoftware.Services.Logging
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.Compress

Public Class CollaborationVersioningManagement
    Inherits CommonBasePage

#Region " Fields "

    Private Const GuidPattern As String = "[a-fA-F\d]{8}-([a-fA-F\d]{4}-){3}[a-fA-F\d]{12}"
    Private Const PurgeZipPattern As String = "^doc_[0-9]{3}_"
    Private Const PurgeCheckOutPattern As String = "[.]{1}[a-fA-F\d]{8}-([a-fA-F\d]{4}-){3}[a-fA-F\d]{12}"

    Private Const CheckedOutZipName As String = "CheckedOutDocuments.zip"

    Private Const IdentifiersFieldName As String = "_collaborationIdentifiers"

    Private _collaborationId As Integer?
    Private _collaborationIdentifiers As IList(Of Integer)
    Private _collaborationList As IList(Of Collaboration)
    Private _lastVersionings As IList(Of CollaborationVersioning)

    Private _backUrl As String

#End Region

#Region " Properties "


    Private ReadOnly Property CollaborationId As Integer?
        Get
            If Not _collaborationId.HasValue Then
                _collaborationId = Request.QueryString.GetValueOrDefault(Of Integer?)("collaborationid", Nothing)
            End If
            Return _collaborationId
        End Get
    End Property

    Private ReadOnly Property CollaborationIdentifiers As IList(Of Integer)
        Get
            If _collaborationIdentifiers Is Nothing Then
                Dim ids As IList(Of Integer)
                If Not String.IsNullOrEmpty(Request.QueryString.Item("ids")) Then
                    Dim queryStringValue As String = Server.UrlDecode(Request.QueryString.GetValue(Of String)("ids"))
                    ids = JsonConvert.DeserializeObject(Of List(Of Integer))(queryStringValue)
                ElseIf Not String.IsNullOrEmpty(ViewState.Item(IdentifiersFieldName)) Then
                    Dim viewStateValue As String = Server.HtmlDecode(ViewState.Item(IdentifiersFieldName))
                    ids = JsonConvert.DeserializeObject(Of List(Of Integer))(viewStateValue)
                End If
                If ids IsNot Nothing AndAlso ids.Count > 0 Then
                    ' Inizializzo solo se mi è stato fornito un elenco valido.
                    _collaborationIdentifiers = ids
                End If
            End If
            Return _collaborationIdentifiers
        End Get
    End Property

    Private ReadOnly Property CollaborationList As IList(Of Collaboration)
        Get
            If _collaborationList Is Nothing Then
                If CollaborationId.HasValue Then
                    ' Valorizzo a partire dal particolare.
                    _collaborationList = New List(Of Collaboration) From {Facade.CollaborationFacade.GetById(CollaborationId.Value)}
                ElseIf CollaborationIdentifiers IsNot Nothing Then
                    ' Altrimenti dallo specifico parametro.
                    _collaborationList = Facade.CollaborationFacade.GetListByIds(CollaborationIdentifiers)
                End If
            End If
            Return _collaborationList
        End Get
    End Property

    Private ReadOnly Property LastVersionings As IList(Of CollaborationVersioning)
        Get
            If _lastVersionings Is Nothing AndAlso CollaborationList IsNot Nothing AndAlso CollaborationList.Count > 0 Then
                Dim merged As New List(Of CollaborationVersioning)
                For Each item As Collaboration In CollaborationList
                    Dim list As IList(Of CollaborationVersioning) = Facade.CollaborationVersioningFacade.GetLastVersionings(item)
                    If list IsNot Nothing AndAlso list.Count > 0 Then
                        merged.AddRange(list)
                    End If
                Next
                If merged.Count > 0 Then
                    _lastVersionings = merged
                End If
            End If
            Return _lastVersionings
        End Get
    End Property

    Private ReadOnly Property BackUrl As String
        Get
            If String.IsNullOrEmpty(_backUrl) Then
                _backUrl = Request.QueryString.GetValueOrDefault(Of String)("backurl", Nothing)
            End If
            Return _backUrl
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            cmdMonitor.Attributes("onclick") = ViewerLight.HIDE_ACTIVEX_SCRIPT & cmdMonitor.Attributes("onclick")
            wndMonitor.OnClientClose = ViewerLight.SHOW_ACTIVEX_SCRIPT.Replace("();", "")
            Try
                BindViewerLight()
            Catch ex As ExtractException
                AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
            End Try
            BindCheckInDocuments()
            InitializeButtons()
        End If

    End Sub

    Private Sub CollaborationVersioningManagement_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        uscCheckInDocuments.ButtonAddDocument.OnClientClick = ViewerLight.HIDE_ACTIVEX_SCRIPT & uscCheckInDocuments.ButtonAddDocument.OnClientClick
    End Sub

    Private Sub cmdClearMonitor_Click(sender As Object, e As EventArgs) Handles cmdClearMonitor.Click
        txtMonitor.Text = String.Empty
    End Sub

    Private Sub cmdCheckOut_Click(sender As Object, e As EventArgs) Handles cmdCheckOut.Click

        Dim selected As IList(Of DocumentInfo) = GetSelectedDocuments()
        If selected Is Nothing OrElse selected.Count = 0 Then
            AjaxAlert("È necessario selezionare dal visualizzatore il/i documento/i di cui eseguire il Check Out.")
            Return
        End If
        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso Not selected.HasSingle() Then
            AjaxAlert("Non è possibile estrarre più di un documento per volta.")
            Return
        End If

        Dim skipped As New List(Of DocumentInfo)
        Dim sessions As New List(Of String)
        Dim checkedOut As New List(Of DocumentInfo)
        For Each item As DocumentInfo In selected
            Dim versioning As CollaborationVersioning = GetVersioningByDocumentInfo(item)
            If versioning Is Nothing Then
                Dim message As String = PromptToScreen("CollaborationVersioning per {0} mancante o non valido.", item.Name)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            Try
                Dim sessionId As String = Facade.CollaborationVersioningFacade.CheckOut(versioning, DocSuiteContext.Current.User.FullUserName)
                Dim fullName As String = CollaborationVersioningFacade.CheckedOutFileNameFormat(item.Name, sessionId)
                item.Name = fullName
                checkedOut.Add(item)

                sessions.Add(sessionId)
            Catch ex As Exception
                Dim message As String = PromptToScreen("Check Out {0} ({1}): {2}", item.Name, versioning.CheckOutSessionId, ex.Message)
                FileLogger.Error(LoggerName, message, ex)
                skipped.Add(item)
                Continue For
            End Try
        Next
        ResetLastVersionings()
        FileLogger.Info(LoggerName, "Check Out completato:" & String.Join(", ", sessions))
        Dim content As DocumentInfo = GetCheckedOutContent(checkedOut)
        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled Then
            OpenLocalFile(content)
        Else
            StartDownload(content)
        End If
        Dim checkedCount As Integer = selected.Count
        Try
            BindViewerLight()
        Catch ex As ExtractException
            AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
        End Try
        BindCheckInDocuments()
        If skipped.Count > 0 Then
            Dim message As String = PromptToScreen("Non è stato possibile eseguire il Check Out di {0} documento/i.", skipped.Count)
        End If
        Dim summary As String = PromptToScreen("Check Out completato di {0} documenti su {1}.", sessions.Count, checkedCount)

    End Sub

    Private Sub cmdUndoCheckOut_Click(sender As Object, e As EventArgs) Handles cmdUndoCheckOut.Click
        Dim selected As IList(Of DocumentInfo) = GetSelectedDocuments()
        If selected Is Nothing OrElse selected.Count = 0 Then
            AjaxAlert("È necessario selezionare dal visualizzatore il/i documento/i di cui eseguire l' Annulla Check Out.")
            Return
        End If
        Dim skipped As New List(Of DocumentInfo)
        For Each item As DocumentInfo In selected
            Dim versioning As CollaborationVersioning = GetVersioningByDocumentInfo(item)
            If versioning Is Nothing Then
                Dim message As String = PromptToScreen("CollaborationVersioning per {0} mancante o non valido.", item.Name)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            Try
                Dim local As FileDocumentInfo = FacadeFactory.Instance.CollaborationVersioningFacade.GetLocalCheckedOutDocument(versioning)
                Facade.CollaborationVersioningFacade.UndoCheckOut(versioning, DocSuiteContext.Current.User.FullUserName)
                DeleteLocalFile(local)
            Catch ex As Exception
                Dim message As String = PromptToScreen("Annulla Check Out {0} ({1}): {2}", item.Name, versioning.CheckOutSessionId, ex.Message)
                FileLogger.Error(LoggerName, message, ex)
                skipped.Add(item)
                Continue For
            End Try
        Next
        ResetLastVersionings()
        Dim checkedCount As Integer = selected.Count
        Try
            BindViewerLight()
        Catch ex As ExtractException
            AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
        End Try
        BindCheckInDocuments()
        If skipped.Count > 0 Then
            Dim message As String = PromptToScreen("Non è stato possibile eseguire l'annullamento del Check Out di {0} documento/i.", skipped.Count)
        End If
        Dim successful As Integer = checkedCount - skipped.Count
        Dim summary As String = PromptToScreen("Annulla Check Out completato di {0} documenti su {1}.", successful, checkedCount)

    End Sub

    Private Sub cmdCheckIn_Click(sender As Object, e As EventArgs) Handles cmdCheckIn.Click
        Dim skipped As New List(Of DocumentInfo)
        Dim updated As New List(Of CollaborationVersioning)
        Dim checkedIn As New List(Of String)
        For Each item As DocumentInfo In uscCheckInDocuments.DocumentInfos
            If Not Regex.IsMatch(item.Name, GuidPattern) Then
                Dim message As String = PromptToScreen("CheckOutSessionId per {0} mancante o non valido.", item.Name)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            Dim sessionId As String = Regex.Match(item.Name, GuidPattern).ToString()
            Dim versioning As CollaborationVersioning = Facade.CollaborationVersioningFacade.GetVersioningByCheckOutSessionId(sessionId)
            If LastVersionings IsNot Nothing AndAlso Not LastVersionings.Contains(versioning) Then
                Dim message As String = PromptToScreen("CollaborationVersioning per {0} non appartiene a nessuna delle versioni correntemente visualizzate.", sessionId)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            If versioning Is Nothing Then
                Dim message As String = PromptToScreen("CollaborationVersioning per {0} mancante o non valido.", sessionId)
                FileLogger.Warn(LoggerName, message)
                skipped.Add(item)
                Continue For
            End If
            item.Name = Regex.Replace(item.Name, PurgeCheckOutPattern, String.Empty)
            item.Name = Regex.Replace(item.Name, PurgeZipPattern, String.Empty)
            Try
                Dim local As FileDocumentInfo = FacadeFactory.Instance.CollaborationVersioningFacade.GetLocalCheckedOutDocument(versioning)
                Facade.CollaborationVersioningFacade.CheckIn(versioning, DocSuiteContext.Current.User.FullUserName, item)
                updated.Add(versioning)
                checkedIn.Add(sessionId)
                DeleteLocalFile(local)
            Catch ex As Exception
                Dim message As String = PromptToScreen("Check In {0}: {1}", sessionId, ex.Message)
                FileLogger.Error(LoggerName, message, ex)
                skipped.Add(item)
                Continue For
            End Try
        Next
        ResetLastVersionings()
        SetCollaborationIdentifiersByVersioningList(updated)
        Try
            BindViewerLight()
        Catch ex As ExtractException
            AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
        End Try
        Dim uploadedCount As Integer = uscCheckInDocuments.DocumentInfos.Count
        PurgeUploaded(checkedIn)
        InitializeButtons()
        If skipped.Count > 0 Then
            Dim message As String = PromptToScreen("Non è stato possibile eseguire il Check In automatico di {0} documento/i, occorre procedere manualmente.", skipped.Count)
        End If
        Dim summary As String = PromptToScreen("Check In completato di {0} documenti su {1}.", checkedIn.Count, uploadedCount)
    End Sub

    Private Sub cmdCheckInManual_Click(sender As Object, e As EventArgs) Handles cmdCheckInManual.Click
        Dim selected As IList(Of DocumentInfo) = GetSelectedDocuments()
        If selected Is Nothing OrElse selected.Count = 0 Then
            AjaxAlert("E' necessario selezionare dal visualizzatore il documento di cui eseguire l'aggiornamento.")
            Return
        End If
        If selected.Count > 1 Then
            AjaxAlert("E' necessario selezionare dal visualizzatore un solo documento per volta.")
            Return
        End If
        If uscCheckInDocuments.DocumentInfos Is Nothing OrElse uscCheckInDocuments.DocumentInfos.Count = 0 Then
            AjaxAlert("E' necessario caricare il documento aggiornato.")
            Return
        End If
        If uscCheckInDocuments.SelectedDocumentInfo Is Nothing Then
            AjaxAlert("E' necessario selezionare il documento fra quelli caricati di cui effettualre il Check In.")
            Return
        End If

        Dim oldVersion As DocumentInfo = selected.First()
        Dim versioning As CollaborationVersioning = GetVersioningByDocumentInfo(oldVersion)
        If versioning Is Nothing Then
            FileLogger.Warn(LoggerName, String.Format("CollaborationVersioning per {0} mancante o non valido.", oldVersion.Name))
            AjaxAlert("CollaborationVersioning per {0} mancante o non valido.", oldVersion.Name)
            Return
        End If
        Dim purged As DocumentInfo = uscCheckInDocuments.SelectedDocumentInfo
        purged.Name = Regex.Replace(purged.Name, PurgeCheckOutPattern, String.Empty)
        purged.Name = Regex.Replace(purged.Name, PurgeZipPattern, String.Empty)
        Try
            Facade.CollaborationVersioningFacade.CheckIn(versioning, DocSuiteContext.Current.User.FullUserName, purged)
            uscCheckInDocuments.SelectedNode.Remove()
        Catch ex As Exception
            Dim message As String = PromptToScreen("Check In Manuale {0}: {1}", uscCheckInDocuments.SelectedDocumentInfo.Name, ex.Message)
            FileLogger.Error(LoggerName, message, ex)
        Finally
            ResetLastVersionings()
            Try
                BindViewerLight()
            Catch ex As ExtractException
                AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
            End Try
        End Try
        Dim summary As String = PromptToScreen("Check In Manuale di {0} completato.", purged.Name)

    End Sub

    Private Sub cmdViewCollaboration_Click(sender As Object, e As EventArgs) Handles cmdViewCollaboration.Click
        Response.Redirect(BackUrl)
    End Sub

    Private Sub uscCheckInDocuments_DocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscCheckInDocuments.DocumentUploaded
        If CollaborationList IsNot Nothing Then
            AjaxManager.ResponseScripts.Add(ViewerLight.SHOW_ACTIVEX_SCRIPT)
        End If
        InitializeButtons()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearMonitor, txtMonitor)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscCheckInDocuments)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckOut, uscViewerLight)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckOut, txtMonitor)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdUndoCheckOut, uscViewerLight)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdUndoCheckOut, txtMonitor)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckIn, uscViewerLight)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckIn, cmdCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckIn, cmdUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckIn, cmdCheckInManual)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckIn, uscCheckInDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckIn, txtMonitor)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckInManual, uscViewerLight)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckInManual, cmdCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckInManual, cmdUndoCheckOut)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckInManual, cmdCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckInManual, uscCheckInDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckInManual, txtMonitor)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdCheckIn)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdCheckInManual)

        If DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckOut, uscCheckInDocuments)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckOut, cmdCheckIn)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdCheckOut, cmdCheckInManual)

            AjaxManager.AjaxSettings.AddAjaxSetting(cmdUndoCheckOut, uscCheckInDocuments)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdUndoCheckOut, cmdCheckIn)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdUndoCheckOut, cmdCheckInManual)
        End If
    End Sub

    Private Sub InitializeButtons()
        Dim hasCollaborations As Boolean = CollaborationList IsNot Nothing AndAlso CollaborationList.Count > 0
        Dim hasUploadedDocuments As Boolean = uscCheckInDocuments.DocumentInfos IsNot Nothing AndAlso uscCheckInDocuments.DocumentInfos.Count > 0
        Dim hasBackUrl As Boolean = Not String.IsNullOrEmpty(BackUrl)

        cmdCheckOut.Enabled = hasCollaborations
        cmdUndoCheckOut.Enabled = hasCollaborations
        cmdCheckInManual.Enabled = hasCollaborations AndAlso hasUploadedDocuments
        cmdCheckIn.Enabled = hasUploadedDocuments
        cmdViewCollaboration.Enabled = hasBackUrl

        uscCheckInDocuments.SignButtonEnabled = False
        uscCheckInDocuments.ButtonPreviewEnabled = False
        uscCheckInDocuments.ButtonScannerEnabled = False
    End Sub

    Private Sub PurgeUploaded(checkedIn As IList(Of String))
        If checkedIn Is Nothing OrElse checkedIn.Count = 0 Then
            Return
        End If

        Dim purged As New List(Of DocumentInfo)(uscCheckInDocuments.DocumentInfos)
        For Each item As DocumentInfo In uscCheckInDocuments.DocumentInfos
            For Each sessionId As String In checkedIn
                If Regex.IsMatch(item.Name, GuidPattern) AndAlso Regex.Match(item.Name, GuidPattern).ToString() = sessionId Then
                    purged.Remove(item)
                    Continue For
                End If
            Next
        Next
        uscCheckInDocuments.LoadDocumentInfo(purged)
        uscCheckInDocuments.InitializeNodesAsAdded(True)
    End Sub
    Private Sub ResetLastVersionings()
        _lastVersionings = Nothing
    End Sub

    Private Function PromptToScreen(message As String) As String
        txtMonitor.Text = String.Format("{0} - {1}{2}{3}", DateTime.Now, message, Environment.NewLine, txtMonitor.Text)
        Return message
    End Function
    Private Function PromptToScreen(format As String, ParamArray args() As Object) As String
        Return PromptToScreen(String.Format(format, args))
    End Function

    Private Sub BindViewerLight()
        If CollaborationList Is Nothing Then
            AjaxManager.ResponseScripts.Add(ViewerLight.HIDE_ACTIVEX_SCRIPT)
            Exit Sub
        End If

        Dim dataSource As New List(Of DocumentInfo)()
        For Each item As Collaboration In CollaborationList
            dataSource.Add(Facade.CollaborationVersioningFacade.GetCollaborationViewerSource(item))
        Next
        uscViewerLight.DataSource = dataSource
        uscViewerLight.DataBind()
        AjaxManager.ResponseScripts.Add(ViewerLight.SHOW_ACTIVEX_SCRIPT)
    End Sub

    Private Sub SetCollaborationIdentifiersByVersioningList(versioningList As IList(Of CollaborationVersioning))
        ' Se è già impostato mantengo la visualizzazione delle collaborazioni correnti.
        If CollaborationIdentifiers IsNot Nothing AndAlso CollaborationIdentifiers.Count > 0 Then
            Return
        End If
        Dim ids As New List(Of Integer)
        For Each item As CollaborationVersioning In versioningList
            If ids.Contains(item.Collaboration.Id) Then
                Continue For
            End If
            ids.Add(item.Collaboration.Id)
        Next
        If ids.Count > 0 Then
            _collaborationIdentifiers = ids
            ViewState.Item(IdentifiersFieldName) = Server.HtmlEncode(JsonConvert.SerializeObject(ids))
        End If
        InitializeButtons()
    End Sub

    Private Sub BindCheckInDocuments()
        If Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled Then
            Return
        End If

        Dim documents As IEnumerable(Of FileDocumentInfo) = LastVersionings.Where(Function(v) FacadeFactory.Instance.CollaborationVersioningFacade.IsMine(v)) _
                                                            .Select(Function(v) FacadeFactory.Instance.CollaborationVersioningFacade.GetLocalCheckedOutDocument(v))

        uscCheckInDocuments.LoadDocumentInfo(New List(Of DocumentInfo)(documents))
        uscCheckInDocuments.InitializeNodesAsAdded(True)

        InitializeButtons()
    End Sub

    Private Function GetSelectedDocuments() As IList(Of DocumentInfo)
        If uscViewerLight.CheckedDocuments IsNot Nothing AndAlso uscViewerLight.CheckedDocuments.Count > 0 Then
            Return uscViewerLight.CheckedDocuments
        End If

        If uscViewerLight.AllDocuments.Count = 1 Then
            Return uscViewerLight.AllDocuments
        End If

        Return Nothing
    End Function

    Private Function GetVersioningByDocumentInfo(document As DocumentInfo) As CollaborationVersioning
        Dim bdi As BiblosDocumentInfo = DirectCast(document, BiblosDocumentInfo)
        Dim idBiblos As Integer = bdi.BiblosChainId
        Return LastVersionings.FirstOrDefault(Function(f) f.IdDocument = idBiblos)
    End Function
    Private Function GetCheckedOutContent(files As List(Of DocumentInfo)) As DocumentInfo
        If files.Count = 1 Then
            Return files.First()
        End If

        Dim tempFileName As String = FileHelper.UniqueFileNameFormat(CheckedOutZipName, DocSuiteContext.Current.User.UserName)
        Dim destination As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)
        Dim content As List(Of KeyValuePair(Of String, Byte())) = files.Select(Function(d) New KeyValuePair(Of String, Byte())(d.Name, d.Stream)).ToList()
        Dim compressManager As ICompress = New ZipCompress()
        compressManager.Compress(content, destination)
        Dim zipped As New FileInfo(destination)
        Dim tempInfo As New TempFileDocumentInfo(zipped) With {.Name = CheckedOutZipName}
        FileLogger.Info(LoggerName, String.Format("{0} inclusi in {1}", files.Count, tempInfo.Name))
        Return tempInfo
    End Function

    Private Sub StartDownload(content As DocumentInfo)
        Dim queryString As NameValueCollection = content.ToQueryString()
        queryString.Add("Download", "True")
        queryString.Add("Original", "True")
        Dim url As String = String.Format("{0}/Viewers/Handlers/DocumentInfoHandler.ashx/{1}?{2}", DocSuiteContext.Current.CurrentTenant.DSWUrl, FileHelper.FileNameToUrl(content.Name), CommonShared.AppendSecurityCheck(queryString.AsEncodedQueryString()))
        AjaxManager.ResponseScripts.Add(String.Format("StartDownload('{0}');", url))
    End Sub

    Private Const excel_ext As String = "|.xlsx|.xlsm|.xlsb|.xltx|.xltm|.xlt|.xls|.xlam|.xla|.xlw|.xlr|.csv|.dif|.slk|"
    Private Const word_ext As String = "|.doc|.docm|.docx|.docx|.dot|.dotm|.dotx|.htm|.html|.mht|.mhtml|.odt|.pdf|.rtf|.txt|.wps|.xml|.xps"

    ''' <summary>
    ''' Downlaod file checkout in share di rete
    ''' </summary>
    ''' <param name="content"></param>
    ''' <remarks></remarks>
    Private Sub OpenLocalFile(content As DocumentInfo)
        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, content.Name)
        If Not File.Exists(destination) Then
            File.WriteAllBytes(destination, content.Stream)
        End If
        Dim fileInfo As FileInfo = New FileInfo(destination)

        Dim functionName As String = "OpenAlert"
        If excel_ext.Contains(String.Concat("|", fileInfo.Extension, "|")) Then
            functionName = "OpenExcel"
        End If
        If word_ext.Contains(String.Concat("|", fileInfo.Extension, "|")) Then
            functionName = "OpenWord"
        End If

        If functionName.Eq("OpenAlert") Then
            destination = fileInfo.Extension
        End If

        Dim escaped As String = HttpUtility.JavaScriptStringEncode(destination)
        Dim jsOpenLocalFile As String = String.Format("{0}('{1}');", functionName, escaped)
        AjaxManager.ResponseScripts.Add(jsOpenLocalFile)
    End Sub

    Private Sub DeleteLocalFile(content As FileDocumentInfo)
        If content Is Nothing Then
            Return
        End If

        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.VersioningShare, content.FileInfo.FullName)
        If Not File.Exists(destination) Then
            Return
        End If

        Try
            File.Delete(destination)
        Catch ex As Exception
            Dim message As String = String.Format("Non è stato possibile cancellare il file dal percorso: {0}", destination)
            FileLogger.Warn(LoggerName, message, ex)
        End Try
    End Sub

#End Region

End Class