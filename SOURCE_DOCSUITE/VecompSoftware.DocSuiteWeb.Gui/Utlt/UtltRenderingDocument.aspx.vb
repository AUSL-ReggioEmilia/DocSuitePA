Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web

Partial Class UtltRenderingDocument
    Inherits CommBasePage

#Region " Properties "

    Private ReadOnly Property BytesKey() As String
        Get
            Return Session.SessionID + AsyncUploadDocument.UniqueID
        End Get
    End Property

#End Region

#Region " Events  "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, AsyncUploadDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(AsyncUploadDocument, AsyncUploadDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblFileName)
        AjaxManager.AjaxSettings.AddAjaxSetting(AsyncUploadDocument, lblFileName)
        If (Not Page.IsPostBack) Then
            LoadSupportedFormats()
        End If
    End Sub

    Private Sub AsyncUploadDocument_FileUploaded(sender As Object, e As FileUploadedEventArgs) Handles AsyncUploadDocument.FileUploaded
        lblFileName.Text = ""
        Context.Cache.Remove(BytesKey)
        If AsyncUploadDocument.UploadedFiles().Count = 0 Then
            AjaxAlert("Nessun documento selezionato.")
            Exit Sub
        End If
        Dim file As UploadedFile = AsyncUploadDocument.UploadedFiles(0)
        If file.ContentLength = 0 Then
            AjaxAlert("Documento non valido")
            Exit Sub
        End If

        Dim extension As String = file.GetExtension().Remove(0, 1)
        If Not DocSuiteContext.Current.ProtocolEnv.SignaturePrintExt.ContainsIgnoreCase(extension) Then
            AjaxAlert("Fomato '{0}' non supportato", extension)
            Exit Sub
        End If

        Dim fileName As String = file.GetName()
        If FileHelper.MatchExtension(fileName, FileHelper.PDF) Then
            AjaxAlert("Il Documento è già nel Formato Richiesto")
            Exit Sub
        End If
        If Not DocSuiteContext.Current.ProtocolEnv.IsFDQEnabled AndAlso FileHelper.MatchExtension(fileName, FileHelper.P7M) Then
            AjaxAlert("Fomato Documento non Gestito per la Conversione")
            Exit Sub
        End If

        lblFileName.Text = fileName
        ' Uso il context per avere il postback ajaxificato e poter pulire il controllo
        Context.Cache.Insert(BytesKey, file.InputStream.ToByteArray(), Nothing, DateTime.Now.AddMinutes(20), TimeSpan.Zero)
    End Sub

    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnExport.Click
        If Context.Cache.Get(BytesKey) Is Nothing Then
            AjaxAlert("Nessun documento da convertire.")
            Exit Sub
        End If
        Dim bytes As Byte() = DirectCast(Context.Cache.Get(BytesKey), Byte())
        ConvertFile(bytes, lblFileName.Text)
    End Sub

#End Region

#Region " Methods "

    Private Sub LoadSupportedFormats()
        AsyncUploadDocument.AllowedFileExtensions = DocSuiteContext.Current.ProtocolEnv.SignaturePrintExt.Split("|"c)
        Dim tblHlp As New DataTable
        tblHlp.Columns.Add("Url", GetType(String))
        tblHlp.Columns.Add("Tipo", GetType(String))

        For Each extension As String In AsyncUploadDocument.AllowedFileExtensions
            If extension.Eq("PDF") OrElse String.IsNullOrEmpty(extension) Then
                Continue For
            End If

            Dim rwHlp As DataRow = tblHlp.NewRow
            rwHlp("Url") = ImagePath.FromFile("." & extension)
            rwHlp("Tipo") = extension
            tblHlp.Rows.Add(rwHlp)
        Next

        rptFormati.DataSource = tblHlp
        rptFormati.DataBind()
    End Sub

    Private Sub ConvertFile(ByRef bytes As Byte(), ByVal fileName As String)
        Dim output As Byte() = VecompSoftware.Services.StampaConforme.Service.ConvertToSimplePdf(bytes, fileName)

        Response.Clear()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(HttpCacheability.Private)
        Response.Expires = -1
        Response.Buffer = True
        Response.AddHeader("Content-Disposition", String.Format("{0};FileName=""{1}""", "attachment", fileName & FileHelper.PDF))
        Response.BinaryWrite(output)
        Response.End()
    End Sub

#End Region

End Class