Imports System.Text
Imports System.IO
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.Signer
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Compress
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Model.Documents
Imports System.Linq
Imports SignModel = VecompSoftware.Services.SignService.Models
Imports System.Net.Http
Imports VecompSoftware.Services.SignService.ProxySignService.Models
Imports VecompSoftware.DocSuiteWeb.Model.Documents.Signs
Imports VecompSoftware.Services.SignService.Services
Imports VecompSoftware.Services.SignService.ArubaSignService.Models

Public Class SingleSign
    Inherits CommonBasePage

#Region " Fields "

    Private _originalDocument As DocumentInfo
    Private _signedDocument As TempFileDocumentInfo
    Private _currentUserLog As UserLog
    Private _currentUserProfile As Model.Documents.Signs.UserProfile = Nothing
    Private _showViewer As Boolean = True

#End Region

#Region " Properties "

    ''' <summary> Visibilità del componente di firma. </summary>
    Public Property ShowViewer() As Boolean
        Get
            Return _showViewer
        End Get
        Set(value As Boolean)
            _showViewer = value
        End Set
    End Property

    ''' <summary> Documento da firmare. </summary>
    ''' <remarks> Salva nel viewstate. </remarks>
    Public Property OriginalDocument() As DocumentInfo
        Private Get
            If _originalDocument Is Nothing Then
                If String.IsNullOrEmpty(serializedDocumentSource.Value) Then
                    Throw New DocSuiteException("Errore di firma", "Impossibile trovare il Documento da firmare.")
                End If
                _originalDocument = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(serializedDocumentSource.Value))
            End If
            Return _originalDocument
        End Get
        Set(value As DocumentInfo)
            serializedDocumentSource.Value = value.ToQueryString().AsEncodedQueryString()
            _originalDocument = value
        End Set
    End Property

    ''' <summary> Documento firmato. </summary>
    Public Property SignedDocument As TempFileDocumentInfo
        Get
            Return _signedDocument
        End Get
        Private Set(value As TempFileDocumentInfo)
            _signedDocument = value
        End Set
    End Property

    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            End If
            Return _currentUserLog
        End Get
    End Property

    Private ReadOnly Property OriginalButton() As RadToolBarButton
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("original"), RadToolBarButton)
        End Get
    End Property

    Private ReadOnly Property PdfButton() As RadToolBarButton
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("pdfConverted"), RadToolBarButton)
        End Get
    End Property

    Private ReadOnly Property CAdESButton() As RadToolBarButton
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("CAdES"), RadToolBarButton)
        End Get
    End Property

    Private ReadOnly Property PAdESButton() As RadToolBarButton
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("PAdES"), RadToolBarButton)
        End Get
    End Property

    Public ReadOnly Property PinTextbox() As RadTextBox
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("pinContainer").FindControl("pin"), RadTextBox)
        End Get
    End Property

    Private ReadOnly Property OtpTextbox() As RadTextBox
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("otpContainer2").FindControl("proxyOtp"), RadTextBox)
        End Get
    End Property

    Private ReadOnly Property signTypeDropdown() As RadDropDownList
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("signTypeDropdown").FindControl("signTypeDropdown"), RadDropDownList)
        End Get
    End Property

    Private ReadOnly Property CloseAfterPdfConversion() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("CloseAfterPdfConversion", False)
        End Get
    End Property

    Private ReadOnly Property CurrentUserProfile As Model.Documents.Signs.UserProfile
        Get
            If _currentUserProfile Is Nothing Then
                If Not String.IsNullOrEmpty(CurrentUserLog.UserProfile) Then
                    _currentUserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(CurrentUserLog.UserProfile)
                End If
            End If
            Return _currentUserProfile
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        MasterDocSuite.TitleVisible = False
        If Not Page.ClientScript.IsStartupScriptRegistered("signJs") Then
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "signJS", "<script type=""text/javascript"" src=""../js/DigitalSign.js""></script>")
        End If

        If Not Page.IsPostBack Then
            Dim defaultSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = SetDefaultSignOption()
            If Not ProtocolEnv.EnableSinglePades Then
                PAdESButton.Visible = False
                CAdESButton.Checked = True
            End If

            OriginalDocument = DocumentInfoFactory.BuildDocumentInfo(Context.Request.QueryString)
            ' Carico il visualizzatore
            uscViewerLight.DataSource = New List(Of DocumentInfo)
            uscViewerLight.DataSource.Add(OriginalDocument)
            Try
                uscViewerLight.DataBind()
            Catch ex As ExtractException
                AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
            End Try
            ' Se è un pdf o un file firmato tolgo la possibilità di convertirlo
            ' macello, ma non conviene metterci testa perchè é in divenire
            If FileHelper.MatchExtension(OriginalDocument.Name, FileHelper.PDF) Then
                OriginalButton.Checked = True
                PdfButton.Enabled = False
                PAdESButton.Enabled = True
            ElseIf FileHelper.MatchExtension(OriginalDocument.Name, FileHelper.P7M) Then
                OriginalButton.Checked = True
                PdfButton.Enabled = False
                CAdESButton.Checked = True
                PAdESButton.Enabled = False
            Else
                PdfButton.Checked = True
                PdfButton.Enabled = True
                PAdESButton.Enabled = True
            End If
            If ProtocolEnv.DefaultSignType.Eq(SignType.CAdES.ToString()) Then
                OriginalButton.Checked = True
                CAdESButton.Checked = True
            End If

            HideSignTypeDropdownToolbarControl()
            If ProtocolEnv.RemoteSignEnabled Then
                ShowSignTypeDropdownToolbarControl()
                PopulateBySelectedSignOption(defaultSignOption)
                If Not DocSuiteContext.Current.HasInfocertProxySign Then
                    signTypeDropdown.FindItemByValue("2").Remove()
                    signTypeDropdown.FindItemByValue("4").Remove()
                End If
                If Not DocSuiteContext.Current.HasArubaActalisSign Then
                    signTypeDropdown.FindItemByValue("1").Remove()
                    signTypeDropdown.FindItemByValue("3").Remove()
                End If
            End If

            LoadDocumentSource()
            ' Carico il componente di firma con lo stream di default
            AjaxManager.ResponseScripts.Add("openDefault();")

        End If

    End Sub

    Private Sub PopulatePin(pin As String)

        Dim ss As String = String.Format("document.getElementById('{0}').value = '{1}';", PinTextbox.ClientID, pin)
        AjaxManager.ResponseScripts.Add(ss)
    End Sub

    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim selectedProvider As Signs.ProviderSignType = ProviderSignType.Smartcard
        If ProtocolEnv.RemoteSignEnabled AndAlso Not String.IsNullOrEmpty(signTypeDropdown.SelectedValue) Then
            selectedProvider = CType(Integer.Parse(signTypeDropdown.SelectedValue), Signs.ProviderSignType)
        End If

        Select Case e.Item.Value
            Case "original", "pdfConverted"
                LoadDocumentSource()
                ' Carico lo stream di default
                AjaxManager.ResponseScripts.Add("loadSelectedDocument();")
            Case "requestOtp"
                RequestOTP(selectedProvider)
            Case "sign"
                If ValidateSubmit(selectedProvider) Then
                    Sign(selectedProvider)
                End If
        End Select
    End Sub

    Protected Sub signTypeDropdown_SelectedIndexChanged(sender As Object, e As DropDownListEventArgs)
        Dim selectedProvider As Signs.ProviderSignType = CType(Integer.Parse(e.Value), Signs.ProviderSignType)
        If CurrentUserProfile IsNot Nothing Then
            Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = GetSelectedUserProfile(selectedProvider)
            PopulateBySelectedSignOption(selectedSignOption)
        End If

        Dim ss As String = String.Format("hideAjaxLoadingPanel('{0}');", ToolBar.ClientID)
        AjaxManager.ResponseScripts.Add(ss)
    End Sub

#End Region

#Region " Methods "
    Private Function GetSelectedUserProfile(selectedProvider As Signs.ProviderSignType) As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)?
        Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = Nothing
        If CurrentUserProfile IsNot Nothing AndAlso CurrentUserProfile.Value.Any(Function(x) x.Key = selectedProvider) Then
            selectedSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Key = selectedProvider)
        End If
        Return selectedSignOption
    End Function

    Private Function ValidateSubmit(ByRef selectedProvider As Signs.ProviderSignType) As Boolean

        If Not ProtocolEnv.RemoteSignEnabled Then
            Return True
        End If

        Dim validationErrors As StringBuilder = New StringBuilder()
        selectedProvider = Signs.ProviderSignType.Smartcard

        If ProtocolEnv.RemoteSignEnabled AndAlso Not String.IsNullOrEmpty(signTypeDropdown.SelectedValue) Then
            selectedProvider = CType(Integer.Parse(signTypeDropdown.SelectedValue), Signs.ProviderSignType)
        End If

        If String.IsNullOrEmpty(PinTextbox.Text) Then
            validationErrors.AppendLine("Inserire PIN.")
        End If
        If selectedProvider = Signs.ProviderSignType.InfocertRemote OrElse selectedProvider = Signs.ProviderSignType.ArubaRemote Then
            If String.IsNullOrEmpty(OtpTextbox.Text) Then
                validationErrors.AppendLine("Inserire OTP.")
            End If
        End If
        If selectedProvider = Signs.ProviderSignType.InfocertAutomatic OrElse selectedProvider = Signs.ProviderSignType.ArubaAutomatic Then
            If String.IsNullOrEmpty(PinTextbox.Text) Then
                validationErrors.AppendLine("Inserire la password.")
            End If
        End If

        If validationErrors.Length > 0 Then
            AjaxAlert(validationErrors.ToString())
            Return False
        End If
        Return True
    End Function

    Private Sub PopulateBySelectedSignOption(selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)?)
        If selectedSignOption.HasValue Then

            If selectedSignOption.Value.Key = Signs.ProviderSignType.ArubaRemote Or selectedSignOption.Value.Key = Signs.ProviderSignType.InfocertRemote Then
                Select Case selectedSignOption.Value.Value.StorageInformationType
                    Case Signs.StorageInformationType.Forever
                        PopulatePin(selectedSignOption.Value.Value.PIN)
                    Case Signs.StorageInformationType.Session
                        Dim signOptionPin As IDictionary(Of String, String)
                        If Session("signOptionPin") IsNot Nothing Then
                            signOptionPin = JsonConvert.DeserializeObject(Of IDictionary(Of String, String))(Session("signOptionPin"))
                            Dim existingPin As String = signOptionPin(selectedSignOption.Value.Key.ToString("D"))
                            If Not String.IsNullOrEmpty(existingPin) Then
                                PopulatePin(existingPin)
                            End If

                        End If
                End Select

            Else
                If selectedSignOption.Value.Key = Signs.ProviderSignType.ArubaAutomatic Or selectedSignOption.Value.Key = Signs.ProviderSignType.InfocertAutomatic Then
                    PopulatePin(selectedSignOption.Value.Value.Password)

                    Select Case selectedSignOption.Value.Value.StorageInformationType
                        Case Signs.StorageInformationType.Forever
                            PopulatePin(selectedSignOption.Value.Value.Password)
                        Case Signs.StorageInformationType.Session
                            Dim signOptionPassword As IDictionary(Of String, String)
                            If Session("signOptionPassword") IsNot Nothing AndAlso Session("signOptionPassword") <> "null" Then
                                signOptionPassword = JsonConvert.DeserializeObject(Of IDictionary(Of String, String))(Session("signOptionPassword"))
                                Dim existingPin As String = signOptionPassword(selectedSignOption.Value.Key.ToString("D"))
                                If Not String.IsNullOrEmpty(existingPin) Then
                                    PopulatePin(existingPin)
                                End If

                            End If
                    End Select
                End If
            End If
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, startStream)
        'AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, signContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary> Carica con <see cref="OriginalDocument"/> il componente di firma. </summary>
    Public Sub LoadDocumentSource()
        ' Pulisco eventuali stream precedenti
        endStream.Value = ""

        Try
            Dim stream As Byte()
            If PdfButton.Checked Then
                If Not CloseAfterPdfConversion Then
                    stream = OriginalDocument.GetPdfStreamNoSignature()
                Else
                    Dim fi As FileInfo = BiblosFacade.SaveUniquePdfToTempNoSignature(OriginalDocument)
                    Dim convertedDocument As TempFileDocumentInfo = New TempFileDocumentInfo(OriginalDocument.PDFName, fi)
                    AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", convertedDocument.ToQueryString().AsEncodedQueryString()))
                    Exit Sub
                End If
            Else
                stream = OriginalDocument.Stream()
            End If

            startStream.Value = Convert.ToBase64String(stream, Base64FormattingOptions.InsertLineBreaks)
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore creazione documento [{0}].", OriginalDocument.Name), ex)
            Throw New DocSuiteException("Errore firma singola", "Errore nella preparazione del documento.", ex)
        End Try
    End Sub

    Private Sub SignCard()
        Dim newName As String = Nothing
        Try
            Dim noloadedStrem As Byte()
            ' Predispongo lo stream
            Dim stream As StringBuilder = New StringBuilder(endStream.Value)
            Dim extension As String = Nothing
            If stream.Length = 0 Then
                Throw New DocSuiteException("Errore firma singola", "Impossibile trovare il file firmato.")
            End If
            stream.Replace("-----BEGIN CMS-----", "")
            stream.Replace("-----END CMS-----", "")

            ' Percorso nuovo file temporaneo, in caso di conversione PDF cancello l'ultima estensione
            InitializeSign(noloadedStrem, newName, extension, loadStream:=False)
            Dim tempFileName As String = $"{FileHelper.UniqueFileNameFormat(OriginalDocument.Name, DocSuiteContext.Current.User.UserName)}{extension}"
            Dim tempPath As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)
            ' Salvataggio stream
            File.WriteAllBytes(tempPath, Convert.FromBase64String(stream.ToString()))
            SignedDocument = New TempFileDocumentInfo(newName, New FileInfo(tempPath), True, stream.Length)

            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", SignedDocument.ToQueryString().AsEncodedQueryString()))
        Catch ex As PathTooLongException
            FileLogger.Warn(LoggerName, "Nome documento da firmare troppo lungo", ex)
            AjaxAlert("Nome del documento troppo lungo.")
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore firma del documento [{0}] [{1}].", OriginalDocument.Name, newName), ex)
            AjaxAlert("Errore generico nella firma del documento, contattare l'assistenza.")
        End Try
    End Sub

    Private Sub RequestOTP(selectedSignType As Signs.ProviderSignType)
        If selectedSignType = ProviderSignType.ArubaAutomatic Then
            AjaxAlert("La firma automatica di ARUBA non prevete OTP")
            Return
        End If

        If selectedSignType = ProviderSignType.InfocertAutomatic Then
            AjaxAlert("La firma automatica di Infocert non prevete OTP")
            Return
        End If

        If selectedSignType = ProviderSignType.ArubaRemote Then
            Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = GetSelectedUserProfile(selectedSignType)
            If Not selectedSignOption.HasValue OrElse (selectedSignOption.HasValue AndAlso String.IsNullOrEmpty(selectedSignOption.Value.Value.Alias)) Then
                AjaxAlert("Profilo di firma non configurato correttamente. Verificare di aver impostato l'Alias del specificato mediante la pagina configurazione utente.")
                Return
            End If

            Try
                Dim signService As SignService = New SignService(Sub(f) FileLogger.Info(LoggerName, f), Sub(f) FileLogger.Error(LoggerName, f))
                Dim arubaSignModel As ArubaSignModel = New ArubaSignModel With {
                    .User = selectedSignOption.Value.Value.Alias,
                    .OTPPassword = selectedSignOption.Value.Value.OTP,
                    .OTPAuthType = selectedSignOption.Value.Value.CustomProperties(RemoteSignProperty.ARUBA_OTP_AUTHTYPE),
                    .CertificateId = selectedSignOption.Value.Value.CustomProperties(RemoteSignProperty.ARUBA_CERTIFICATEID),
                    .SignType = SignModel.SignType.Remote
                }
                signService.RequestOTP(arubaSignModel, SignModel.SignatureType.ArubaSign)
                AjaxAlert("OTP richiesto correttamente.")
            Catch ex As Exception
                AjaxAlert("Si è verificato un errore durante l'invio della richiesta OTP")
            End Try
        End If

        If selectedSignType = ProviderSignType.InfocertRemote Then
            Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = GetSelectedUserProfile(selectedSignType)
            If Not selectedSignOption.HasValue OrElse (selectedSignOption.HasValue AndAlso String.IsNullOrEmpty(selectedSignOption.Value.Value.Alias)) Then
                AjaxAlert("Profilo di firma non configurato correttamente. Verificare di aver impostato l'Alias del specificato mediante la pagina configurazione utente.")
                Return
            End If

            Try
                Dim signService As SignService = New SignService(Sub(f) FileLogger.Info(LoggerName, f), Sub(f) FileLogger.Error(LoggerName, f))
                Dim proxySignModel As ProxySignModel = New ProxySignModel With {
                    .Alias = selectedSignOption.Value.Value.Alias,
                    .SignType = SignModel.SignType.Remote
                }
                signService.RequestOTP(proxySignModel, SignModel.SignatureType.ProxySign)
                AjaxAlert("OTP richiesto correttamente.")
            Catch ex As Exception
                AjaxAlert("Si è verificato un errore durante l'invio della richiesta OTP")
            End Try
        End If

    End Sub
    Private Sub SignAutomatic(selectedSignType As Signs.ProviderSignType)
        Dim newName As String = String.Empty
        Dim stream As Byte() = Nothing
        Dim extension As String = String.Empty
        Dim currentFilename As String = String.Empty

        Try
            currentFilename = OriginalDocument.Name
            InitializeSign(stream, newName, extension)

            Dim signService As SignService = New SignService(Sub(f) FileLogger.Info(LoggerName, f), Sub(f) FileLogger.Error(LoggerName, f))

            If selectedSignType = ProviderSignType.ArubaAutomatic Then
                If Not CurrentUserProfile.Value.Keys.Any(Function(f) f = Signs.ProviderSignType.ArubaAutomatic) Then
                    AjaxAlert("Profilo di firma automatica Aruba non configurato correttamente. Verificare la pagina configurazione utente.")
                    Return
                End If

                Dim remoteSignProperty As RemoteSignProperty = CurrentUserProfile.Value.Item(Signs.ProviderSignType.ArubaAutomatic)
                Dim arubaSignModel As ArubaSignModel = New ArubaSignModel With {
                    .DelegatedDomain = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_DOMAIN),
                    .DelegatedPassword = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_PASSWORD),
                    .DelegatedUser = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_USER),
                    .OTPPassword = remoteSignProperty.OTP,
                    .OTPAuthType = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_OTP_AUTHTYPE),
                    .User = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_USER),
                    .CertificateId = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_CERTIFICATEID),
                    .RequestType = SignModel.SignRequestType.Pades
                }
                If CAdESButton.Checked Then
                    arubaSignModel.RequestType = SignModel.SignRequestType.Cades
                End If
                arubaSignModel.SignType = SignModel.SignType.Automatic
                stream = signService.SignDocument(arubaSignModel, stream, currentFilename, SignModel.SignatureType.ArubaSign)
            End If

            If selectedSignType = ProviderSignType.InfocertAutomatic Then
                If Not CurrentUserProfile.Value.Keys.Any(Function(f) f = Signs.ProviderSignType.InfocertAutomatic) Then
                    AjaxAlert("Profilo di firma automatica Aruba non configurato correttamente. Verificare la pagina configurazione utente.")
                    Return
                End If
                Dim remoteSignProperty As RemoteSignProperty = CurrentUserProfile.Value.Item(Signs.ProviderSignType.InfocertAutomatic)
                If String.IsNullOrEmpty(remoteSignProperty.Alias) Then
                    AjaxAlert("Profilo di firma non configurato correttamente. Verificare di aver impostato l'Alias del specificato mediante la pagina configurazione utente.")
                    Return
                End If
                Dim proxySignModel As ProxySignModel = New ProxySignModel With {
                    .Alias = remoteSignProperty.Alias,
                    .OTPPassword = OtpTextbox.Text,
                    .PINPassword = PinTextbox.Text,
                    .SignType = SignModel.SignType.Automatic
                }

                proxySignModel.RequestType = SignModel.SignRequestType.Pades
                If CAdESButton.Checked Then
                    proxySignModel.RequestType = SignModel.SignRequestType.Cades
                End If

                stream = signService.SignDocument(proxySignModel, stream, currentFilename, SignModel.SignatureType.ProxySign)

            End If

            Dim tempFileName As String = $"{FileHelper.UniqueFileNameFormat(currentFilename, DocSuiteContext.Current.User.UserName)}{extension}"
            Dim tempPath As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)
            File.WriteAllBytes(tempPath, stream)
            SignedDocument = New TempFileDocumentInfo(newName, New FileInfo(tempPath), True, stream.Length)
            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", SignedDocument.ToQueryString().AsEncodedQueryString()))

        Catch ex As SignerException
            AjaxAlert(ex.Message)
        Catch ex As PathTooLongException
            FileLogger.Warn(LoggerName, "Nome documento da firmare troppo lungo", ex)
            AjaxAlert("Nome del documento troppo lungo.")
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore firma del documento [{0}] [{1}].", currentFilename, newName), ex)
            AjaxAlert("Errore generico nella firma del documento, contattare l'assistenza.")
        End Try
    End Sub

    Private Sub SignRemote(selectedSignType As Signs.ProviderSignType)
        Dim newName As String = String.Empty
        Dim stream As Byte() = Nothing
        Dim extension As String = String.Empty
        Dim currentFilename As String = String.Empty
        Try
            currentFilename = OriginalDocument.Name
            InitializeSign(stream, newName, extension)

            Dim selectedProvider As Signs.ProviderSignType = CType(Integer.Parse(signTypeDropdown.SelectedValue), Signs.ProviderSignType)
            Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = GetSelectedUserProfile(selectedProvider)

            If Not selectedSignOption.HasValue OrElse (selectedSignOption.HasValue AndAlso String.IsNullOrEmpty(selectedSignOption.Value.Value.Alias)) Then
                AjaxAlert("Profilo di firma non configurato correttamente. Verificare di aver impostato l'Alias del specificato mediante la pagina configurazione utente.")
                Return
            End If
            Dim signService As SignService = New SignService(Sub(f) FileLogger.Info(LoggerName, f), Sub(f) FileLogger.Error(LoggerName, f))

            If selectedSignType = ProviderSignType.ArubaRemote Then
                Dim remoteSignProperty As RemoteSignProperty = selectedSignOption.Value.Value
                Dim arubaSignModel As ArubaSignModel = New ArubaSignModel()
                arubaSignModel.OTPPassword = OtpTextbox.Text
                arubaSignModel.OTPAuthType = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_OTP_AUTHTYPE)
                arubaSignModel.User = selectedSignOption.Value.Value.Alias
                arubaSignModel.UserPassword = PinTextbox.Text
                arubaSignModel.CertificateId = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_CERTIFICATEID)
                arubaSignModel.SignType = SignModel.SignType.Remote
                arubaSignModel.RequestType = SignModel.SignRequestType.Pades
                If CAdESButton.Checked Then
                    arubaSignModel.RequestType = SignModel.SignRequestType.Cades
                End If
                stream = signService.SignDocument(arubaSignModel, stream, currentFilename, SignModel.SignatureType.ArubaSign)

            End If

            If selectedSignType = ProviderSignType.InfocertRemote Then
                Dim proxySignModel As ProxySignModel = New ProxySignModel With {
                    .Alias = selectedSignOption.Value.Value.Alias,
                    .OTPPassword = OtpTextbox.Text,
                    .PINPassword = PinTextbox.Text,
                    .SignType = SignModel.SignType.Remote
                }

                proxySignModel.RequestType = SignModel.SignRequestType.Pades
                If CAdESButton.Checked Then
                    proxySignModel.RequestType = SignModel.SignRequestType.Cades
                End If

                stream = signService.SignDocument(proxySignModel, stream, currentFilename, SignModel.SignatureType.ProxySign)
            End If

            ' Salvataggio stream
            Dim tempFileName As String = $"{FileHelper.UniqueFileNameFormat(currentFilename, DocSuiteContext.Current.User.UserName)}{extension}"
            Dim tempPath As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)
            File.WriteAllBytes(tempPath, stream)
            SignedDocument = New TempFileDocumentInfo(newName, New FileInfo(tempPath), True, stream.Length)
            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", SignedDocument.ToQueryString().AsEncodedQueryString()))

        Catch ex As SignerException
            AjaxAlert(ex.Message)
        Catch ex As PathTooLongException
            FileLogger.Warn(LoggerName, "Nome documento da firmare troppo lungo", ex)
            AjaxAlert("Nome del documento troppo lungo.")
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore firma del documento [{0}] [{1}].", currentFilename, newName), ex)
            AjaxAlert("Errore generico nella firma del documento, contattare l'assistenza.")
        End Try
    End Sub

    Private Sub InitializeSign(ByRef stream As Byte(), ByRef newName As String, ByRef extension As String, Optional loadStream As Boolean = True)
        extension = OriginalDocument.Extension
        stream = Nothing
        If CAdESButton.Checked Then
            ' CAdES
            newName = OriginalDocument.Name
            extension = FileHelper.P7M
            If PdfButton.Checked Then
                ' Se è stato convertito sostituisco la vecchia estensione con PDF
                newName = $"{OriginalDocument.Name.Substring(0, OriginalDocument.Name.Length - OriginalDocument.Extension.Length)}{FileHelper.PDF}"
                extension = FileHelper.PDF
                If loadStream Then
                    stream = OriginalDocument.GetPdfStreamNoSignature()
                End If
            End If
            newName = $"{newName}{FileHelper.P7M}"
            extension = FileHelper.P7M
        Else
            ' PAdES
            newName = OriginalDocument.Name
            If Not FileHelper.MatchExtension(OriginalDocument.Name, FileHelper.PDF) Then
                newName = $"{OriginalDocument.Name.Substring(0, OriginalDocument.Name.Length - OriginalDocument.Extension.Length)}{FileHelper.PDF}"
            End If
            extension = FileHelper.PDF
            If loadStream Then
                stream = OriginalDocument.GetPdfStreamNoSignature()
            End If
        End If
        If stream Is Nothing AndAlso loadStream Then
            stream = OriginalDocument.Stream()
        End If
    End Sub

    Private Sub Sign(selectedSignType As Signs.ProviderSignType)
        Select Case selectedSignType
            Case Signs.ProviderSignType.Smartcard
                SignCard()
            Case Signs.ProviderSignType.ArubaRemote, Signs.ProviderSignType.InfocertRemote
                SignRemote(selectedSignType)
            Case Signs.ProviderSignType.InfocertAutomatic, Signs.ProviderSignType.ArubaAutomatic
                SignAutomatic(selectedSignType)
        End Select

    End Sub

    Function SetDefaultSignOption() As KeyValuePair(Of ProviderSignType, RemoteSignProperty)?
        Dim defaultSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = Nothing
        If ProtocolEnv.RemoteSignEnabled AndAlso CurrentUserProfile IsNot Nothing Then
            If CurrentUserProfile.Value.Any(Function(x) x.Value.IsDefault = True) Then
                defaultSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Value.IsDefault = True)
            Else
                defaultSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Key = CurrentUserProfile.DefaultProvider)
            End If
            If defaultSignOption.HasValue Then
                signTypeDropdown.SelectedValue = defaultSignOption.Value.Key.ToString("D")
            End If
        Else
            signTypeDropdown.SelectedIndex = 0
        End If
        Return defaultSignOption
    End Function

    Private Sub ShowSignTypeDropdownToolbarControl()
        ToolBar.Items.FindItemByValue("signTypeDropdown").SetDisplay(True)
        ToolBar.Items.FindItemByValue("signTypeSeparator").SetDisplay(True)
    End Sub

    Private Sub HideSignTypeDropdownToolbarControl()
        ToolBar.Items.FindItemByValue("signTypeDropdown").SetDisplay(False)
        ToolBar.Items.FindItemByValue("signTypeSeparator").SetDisplay(False)
    End Sub

#End Region

End Class