Imports VecompSoftware.Helpers.ExtensionMethods
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Web
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Services.Logging

Partial Public Class CommonUploadDocument
    Inherits CommBasePage

#Region " Fields "

    Public Const AllowedExtensionsQueryItem As String = "allowedextensions"
    Public Const ExcelImportQueryItem As String = "Excel"
    Private Const MultipleSelectionQueryItem As String = "MultiDoc"
    Private Const AllowZipDocQueryItem As String = "allowzipdoc"
    Private Const AllowUnlimitFileSizeQueryItem As String = "allowunlimitfilesize"

    Private _allowedExtensions As List(Of String)
    Private _fileExtensionWhiteList As List(Of String)
    Private _fileExtensionBlackList As List(Of String)
    Private _clientSideFileExtensionBlackList As String
    Private _backurl As String

#End Region

#Region " Properties "

    Public ReadOnly Property ExcelImport() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(ExcelImportQueryItem, False)
        End Get
    End Property

    Private ReadOnly Property MultipleSelection As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(MultipleSelectionQueryItem, False)
        End Get
    End Property

    Private ReadOnly Property IsZipAllowed As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(AllowZipDocQueryItem, False)
        End Get
    End Property

    Private ReadOnly Property IsAllowUnlimitFileSize As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(AllowUnlimitFileSizeQueryItem, False)
        End Get
    End Property

    Public ReadOnly Property WarningUploadThreshold As Long
        Get
            Return ProtocolEnv.WarningUploadThreshold
        End Get
    End Property

    Public ReadOnly Property WarningUploadThresholdType As String
        Get
            Return ProtocolEnv.WarningUploadThresholdType
        End Get
    End Property

    Public ReadOnly Property MaxUploadThreshold As Integer
        Get
            If IsAllowUnlimitFileSize Then
                Return 0
            End If
            Return ProtocolEnv.MaxUploadThreshold
        End Get
    End Property

    Private ReadOnly Property AllowedExtensions As List(Of String)
        Get
            If _allowedExtensions Is Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString.GetValueOrDefault(Of String)(AllowedExtensionsQueryItem, Nothing)) Then
                _allowedExtensions = New List(Of String)(Request.Item(AllowedExtensionsQueryItem).ToLowerInvariant().Split(","c))
            End If
            Return _allowedExtensions
        End Get
    End Property

    Private ReadOnly Property FileExtensionWhiteList As List(Of String)
        Get
            If _fileExtensionWhiteList Is Nothing Then
                If AllowedExtensions IsNot Nothing Then
                    _fileExtensionWhiteList = New List(Of String)(AllowedExtensions)
                    If _fileExtensionWhiteList.Contains(FileHelper.ZIP) AndAlso DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso Not MultipleSelection Then
                        _fileExtensionWhiteList.Remove(FileHelper.ZIP)
                    End If

                ElseIf Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.FileExtensionWhiteList) Then
                    Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.FileExtensionWhiteList.ToLowerInvariant().Split("|"c)
                    _fileExtensionWhiteList = New List(Of String)(splitted)

                    ' Considero le sole estensioni non menzionate in BlackList.
                    _fileExtensionWhiteList = _fileExtensionWhiteList.Where(Function(white) Not FileExtensionBlackList.Contains(white)).ToList()

                    If DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso MultipleSelection Then
                        _fileExtensionWhiteList.Add(FileHelper.ZIP)
                    End If
                End If
            End If
            Return _fileExtensionWhiteList
        End Get
    End Property

    Private ReadOnly Property FileExtensionBlackList As List(Of String)
        Get
            If _fileExtensionBlackList Is Nothing AndAlso AllowedExtensions Is Nothing AndAlso Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList) Then

                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList.ToLowerInvariant().Split("|"c)
                _fileExtensionBlackList = New List(Of String)(splitted)
            End If
            Return _fileExtensionBlackList
        End Get
    End Property

    Public ReadOnly Property ClientSideFileExtensionBlackList As String
        Get
            If String.IsNullOrEmpty(_clientSideFileExtensionBlackList) Then
                _clientSideFileExtensionBlackList = String.Empty
                If FileExtensionBlackList IsNot Nothing Then
                    Dim bl As New List(Of String)(FileExtensionBlackList)
                    If DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso MultipleSelection Then
                        bl.Remove(FileHelper.ZIP)
                    End If
                    _clientSideFileExtensionBlackList = String.Join("|", bl)
                End If
            End If
            Return _clientSideFileExtensionBlackList
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        ' Disabilita Silverlight.
        If Not DocSuiteContext.Current.ProtocolEnv.UploadSilverlightEnabled Then
            Const script As String = "Telerik.Web.UI.RadAsyncUpload.Modules.Flash.isAvailable = function () { return false; }; Telerik.Web.UI.RadAsyncUpload.Modules.Silverlight.isAvailable = function () { return false; };"
            ' ATTENZIONE: lo script di disabilitazione di SilverLight deve essere rigistrato in Startup.
            ' AjaxManager.ResponseScripts.Add(script) <== QUESTO NON FUNZIONA
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SilverlightUnavailable", script, True)
        End If
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            ' Inizializzo le funzionalità di verifica su FileExtensionBlackList.
            InitializeAsyncUploadDocument()

            chkDisableFileExtensionWhiteList.Checked = False
            chkDisableFileExtensionWhiteList.Visible = AllowedExtensions Is Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.EnableGrayList
        End If

        excelModel.Visible = ExcelImport

        MasterDocSuite.TitleVisible = False
        lnkModel.NavigateUrl = String.Format("{0}/Comm/Download/modello.xls", CommonUtil.GetInstance().HomeDirectory)
    End Sub

    ''' <summary> Disattiva la verifica di FileExtensionWhiteList. </summary>
    Protected Sub chkDisableFileExtensionWhiteList_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles chkDisableFileExtensionWhiteList.CheckedChanged
        cmdConfirm.OnClientClicked = "cmdConfirm_OnClick"
        If chkDisableFileExtensionWhiteList.Checked OrElse FileExtensionWhiteList Is Nothing Then
            AsyncUploadDocument.AllowedFileExtensions = Nothing
            Exit Sub
        End If

        AsyncUploadDocument.AllowedFileExtensions = FileExtensionWhiteList.ToArray()

        AjaxManager.ResponseScripts.Add("purgeBlackListed();")
    End Sub

    Private Sub cmdConfirm_Click(sender As Object, e As EventArgs) Handles cmdConfirm.Click
        If AsyncUploadDocument Is Nothing OrElse AsyncUploadDocument.UploadedFiles Is Nothing OrElse AsyncUploadDocument.UploadedFiles.Count = 0 Then
            AjaxAlert("Nessun documento selezionato.")
            Exit Sub
        End If

        Dim serializeMe As New Dictionary(Of String, String)
        Dim invalidEntries As New List(Of String)
        Dim warningSizeEntries As New List(Of String)

        For Each item As UploadedFile In AsyncUploadDocument.UploadedFiles
            Dim fileName As String = Path.GetFileName(FileHelper.ReplaceUnicode(item.FileName))
            Dim extension As String = Path.GetExtension(item.FileName).ToLowerInvariant()
            If FileExtensionBlackList IsNot Nothing AndAlso FileExtensionBlackList.Contains(extension) Then
                invalidEntries.Add(fileName)
                Continue For
            End If
            Dim targetFileName As String = FileHelper.UniqueFileNameFormat(fileName, DocSuiteContext.Current.User.UserName)
            Dim targetPath As String = Path.Combine(CommonUtil.GetInstance().AppTempPath, targetFileName)
            Try
                item.SaveAs(targetPath)
            Catch ex As PathTooLongException
                ' TODO: rimuovere appena possibile questo metodo di salvataggio e conteggio spannometrico dei caratteri possibili
                Dim theoricalMaxLenght As Integer = FileHelper.MaxFullyQualifiedNameLength - CommonUtil.GetInstance().AppTempPath.Length - FileHelper.UniqueFileNameFormat("", DocSuiteContext.Current.User.UserName).Length
                Throw New DocSuiteException("Errore inserimento", String.Format("Il nome del file è troppo lungo: [{0}] caratteri di [{1}] possibili.", fileName.Length, theoricalMaxLenght), ex)
            End Try

            If (FileHelper.MatchExtension(fileName, FileHelper.ZIP) OrElse FileHelper.MatchExtension(fileName, FileHelper.RAR)) AndAlso DocSuiteContext.Current.ProtocolEnv.UploadZipManaged AndAlso MultipleSelection Then
                If DocSuiteContext.Current.ProtocolEnv.ZipUploadEnabled AndAlso chkDisableFileExtensionWhiteList.Checked AndAlso IsZipAllowed Then
                    serializeMe.Add(targetFileName, fileName)
                    If WarningUploadThresholdType = "Alert" AndAlso item.ContentLength > WarningUploadThreshold Then
                        warningSizeEntries.Add(String.Format("{0} ({1})", fileName, item.ContentLength.ToByteFormattedString()))
                    End If
                Else

                    Dim prefix As String = FileHelper.UniquePrefixFormat(DocSuiteContext.Current.User.UserName)
                    Dim compressManager As ICompress = New ZipCompress()
                    If FileHelper.MatchExtension(fileName, FileHelper.RAR) Then
                        compressManager = New RarCompress()
                    End If
                    Dim compressedItems As ICollection(Of CompressItem) = New List(Of CompressItem)
                    Try
                        compressedItems = compressManager.InMemoryExtract(targetPath)
                    Catch ex As BadPasswordException
                        FileLogger.Warn(LogName.FileLog, "Non è stato possibile aprire il file compresso protetto da password. Lato applicativo verrà gestito il file senza l'apertura automatica del contenuto compresso.", ex)
                        serializeMe.Add(targetFileName, fileName)
                        Continue For
                    Catch ex As ExtractException
                        AjaxAlert(String.Format(EXTRACT_COMPRESSFILE_ERROR))
                        Exit Sub
                    End Try

                    For Each compressItem As CompressItem In compressedItems
                        Dim uniqueFileName As String = compressItem.FullFilename.Replace("/", "_")
                        extension = Path.GetExtension(compressItem.Filename).ToLowerInvariant()
                        If FileExtensionWhiteList IsNot Nothing AndAlso Not chkDisableFileExtensionWhiteList.Checked AndAlso Not FileExtensionWhiteList.Contains(extension) Then
                            invalidEntries.Add(compressItem.FullFilename)
                            Continue For
                        End If
                        If FileExtensionBlackList IsNot Nothing AndAlso FileExtensionBlackList.Contains(extension) Then
                            invalidEntries.Add(compressItem.FullFilename)
                            Continue For
                        End If
                        File.WriteAllBytes(Path.Combine(CommonUtil.GetInstance().AppTempPath, String.Concat(prefix, FileHelper.ReplaceUnicode(uniqueFileName))), compressItem.Data)
                        serializeMe.Add(prefix & FileHelper.ReplaceUnicode(uniqueFileName), FileHelper.ReplaceUnicode(compressItem.Filename))
                    Next
                End If
            ElseIf (FileHelper.MatchExtension(fileName, FileHelper.ZIP) AndAlso Not IsZipAllowed) OrElse FileHelper.MatchExtension(fileName, FileHelper.RAR) Then
                invalidEntries.Add(fileName)
            Else
                serializeMe.Add(targetFileName, fileName)
                If WarningUploadThresholdType = "Alert" AndAlso item.ContentLength > WarningUploadThreshold Then
                    warningSizeEntries.Add(String.Format("{0} ({1})", fileName, item.ContentLength.ToByteFormattedString()))
                End If
            End If
        Next

        If invalidEntries.Count > 0 Then
            AjaxAlert(String.Format("I seguenti file non presentano un'estensione valida al caricamento: {0}", String.Join(", ", invalidEntries)))
        End If

        If WarningUploadThresholdType = "Alert" AndAlso warningSizeEntries.Count > 0 Then
            AjaxAlert(String.Format("I seguenti file risultano avere una grandezza elevata e potrebbero rallentare i servizi:{0}{1}{0}{0}Si consiglia, se possibile, di utilizzare versioni con dimensione minore.", Environment.NewLine, String.Join(Environment.NewLine, warningSizeEntries)))
        End If

        Dim serialized As String = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(serializeMe))
        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", serialized))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AsyncUploadDocument.PostbackTriggers = New String() {cmdConfirm.ID}
        AjaxManager.AjaxSettings.AddAjaxSetting(chkDisableFileExtensionWhiteList, AsyncUploadDocument)
    End Sub

    Private Sub InitializeAsyncUploadDocument()

        AsyncUploadDocument.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Disabled

        If MultipleSelection Then
            AsyncUploadDocument.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Automatic
            ' AsyncUploadDocument.MaxFileInputsCount = 0
        Else
            AsyncUploadDocument.MaxFileInputsCount = 1
        End If

        ' Inizializzo le funzionalità di verifica sulle estensioni dei file caricati.
        AsyncUploadDocument.AllowedFileExtensions = Nothing
        If FileExtensionWhiteList IsNot Nothing Then
            AsyncUploadDocument.AllowedFileExtensions = FileExtensionWhiteList.ToArray()
        End If

        AsyncUploadDocument.MaxFileSize = MaxUploadThreshold
    End Sub

#End Region

End Class