Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.Compress

Public Class PECViewFromFile
    Inherits PECBasePage
    Implements IHaveViewerLight
    Implements IHavePecMail

#Region " Properties "

    Public ReadOnly Property CheckedDocuments() As List(Of DocumentInfo) Implements IHaveViewerLight.CheckedDocuments
        Get
            Return viewer.CheckedDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedDocument() As DocumentInfo Implements IHaveViewerLight.SelectedDocument
        Get
            Return viewer.SelectedDocument
        End Get
    End Property

    Public ReadOnly Property PecMails() As IEnumerable(Of PECMail) Implements IHavePecMail.PecMails
        Get
            Dim pec As New PECMail()
            pec.MailSubject = txtObject.Text
            Dim mailDate As DateTime
            If DateTime.TryParse(txtDate.Text, mailDate) Then
                pec.MailDate = mailDate
            End If
            pec.MailSenders = lblFrom.Text
            pec.MailRecipients = lblTo.Text
            pec.Direction = PECMailDirection.Ingoing
            Return {pec}
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            Title = String.Format("{0} - Apertura da File", PecLabel)
            uscUploadDocumenti.AllowedExtensions = {FileHelper.EML}.ToList()

            Dim source As String = Request.QueryString.GetValueOrDefault(Of String)("source", String.Empty)
            If Not String.IsNullOrEmpty(source) Then
                Dim doc As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(HttpUtility.UrlDecode(source))), TempFileDocumentInfo)
                uscUploadDocumenti.LoadDocumentInfo(doc)
                Try
                    LoadFile(doc)
                Catch ex As ExtractException
                    AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
                End Try
            Else
                UnloadFile()
            End If

        End If

    End Sub

    Private Sub uscUploadDocumenti_DocumentRemoved(sender As Object, e As DocumentEventArgs) Handles uscUploadDocumenti.DocumentRemoved
        UnloadFile()
    End Sub

    Private Sub uscUploadDocumenti_DocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscUploadDocumenti.DocumentUploaded

        Dim documents As List(Of DocumentInfo) = uscUploadDocumenti.DocumentInfos
        If documents.IsNullOrEmpty() OrElse documents.Count <> 1 Then
            UnloadFile()
            Exit Sub
        End If
        Try
            Dim doc As TempFileDocumentInfo = CType(uscUploadDocumenti.DocumentInfos(0), TempFileDocumentInfo)
            LoadFile(doc)
        Catch ex As ExtractException
            AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlHeader, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, viewer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadFile(doc As TempFileDocumentInfo)
        If Not FileHelper.MatchExtension(doc.Name, FileHelper.EML) Then
            AjaxAlert("Il documento caricato non è supportato. Caricare un documento di tipo EML.")
            UnloadFile()
            Exit Sub
        End If

        Dim pec As PECMail = Tools.PecMailFactory(doc)
        lblFrom.Text = HttpUtility.HtmlEncode(pec.MailSenders)
        lblTo.Text = HttpUtility.HtmlEncode(pec.MailRecipients)
        txtObject.Text = pec.MailSubject
        If (txtObject.Text.Length > 90) Then
            txtObject.TextMode = TextBoxMode.MultiLine
            If (txtObject.Text.Length > 250) Then
                txtObject.Rows = 3
            End If
        End If

        If pec.MailDate.HasValue Then
            txtDate.Text = pec.MailDate.Value.ToString("dd/MM/yyyy")
        End If

        txtEmlSize.Text = pec.Size.Value.ToByteFormattedString(2)

        Dim source As List(Of DocumentInfo) = Tools.GetAttachments(doc)
        viewer.DataSource = source
        viewer.DataBind()

        AjaxManager.ResponseScripts.Add(ViewerLight.SHOW_ACTIVEX_SCRIPT)

        Dim backurl As String = String.Format("~/PEC/PECViewFromFile.aspx?source={0}", HttpUtility.UrlEncode(doc.ToQueryString().AsEncodedQueryString()))

        cmdProtocol.PostBackUrl = AddBackUrl("~/Pec/PECToDocumentUnit.aspx", backurl)
        cmdAttach.PostBackUrl = AddBackUrl("~/PEC/PECAttachToDocumentUnit.aspx", backurl)

        cmdProtocol.Enabled = True
        cmdAttach.Enabled = True
    End Sub

    Private Sub UnloadFile()
        lblFrom.Text = ""
        lblTo.Text = ""
        txtObject.Text = ""
        txtDate.Text = ""
        txtEmlSize.Text = ""

        AjaxManager.ResponseScripts.Add(ViewerLight.HIDE_ACTIVEX_SCRIPT)
        cmdProtocol.Enabled = False
        cmdAttach.Enabled = False
    End Sub

    Private Function AddBackUrl(url As String, args As String) As String
        Dim abs As String = ConvertRelativeUrlToAbsoluteUrl(url)
        Dim uri As New Uri(abs, UriKind.Absolute)
        Return String.Format("{0}?backurl={1}", uri.AbsolutePath, HttpUtility.UrlEncode(args))
    End Function

#End Region

End Class