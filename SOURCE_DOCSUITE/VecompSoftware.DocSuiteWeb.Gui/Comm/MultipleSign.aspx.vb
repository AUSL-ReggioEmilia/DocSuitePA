Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Documents
Imports VecompSoftware.DocSuiteWeb.Model.Documents.Signs
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.SignService.ArubaSignService.Models
Imports VecompSoftware.Services.SignService.Models
Imports VecompSoftware.Services.SignService.ProxySignService.Models
Imports VecompSoftware.Services.SignService.Services
Imports SignModel = VecompSoftware.Services.SignService.Models

Public Class MultipleSign
    Inherits MultipleSignBasePage

#Region " Fields "

    Public Const MultiSignUndoQuerystring As String = "msu"
    Private _currentUserLog As UserLog
    Private _currentUserProfile As Model.Documents.Signs.UserProfile = Nothing

#End Region

#Region " Properties "

    Protected ReadOnly Property DocumentCount As Integer
        Get
            Return OriginalDocuments.Count
        End Get
    End Property

    Public ReadOnly Property TempDirectoryName As String
        Get
            Dim tempDir As String = CType(ViewState("TempDir"), String)
            If String.IsNullOrEmpty(tempDir) Then
                tempDir = String.Format("{0}{1}{2:hhmmss}", DocSuiteContext.Current.ProtocolEnv.FDQMultipleShare.Replace("%ServerName%", Server.MachineName), DocSuiteContext.Current.User.UserName, DateTime.Now)
                ViewState("TempDir") = tempDir
            End If
            Return tempDir
        End Get
    End Property

    Private ReadOnly Property ChkAddComment As RadButton
        Get
            Dim controlContainer As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("AddComment"), RadToolBarButton)
            Return DirectCast(controlContainer.FindControl("chkAddComment"), RadButton)
        End Get
    End Property

    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentUserLog
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

    Public ReadOnly Property PinTextbox() As RadTextBox
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("pinContainer").FindControl("pin"), RadTextBox)
        End Get
    End Property

    Private ReadOnly Property signTypeDropdown() As RadDropDownList
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("signTypeDropdown").FindControl("signTypeDropdown"), RadDropDownList)
        End Get
    End Property

    Private Property OriginalDocumentInfos As List(Of DocumentInfo)
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not Page.IsPostBack Then
            Initialize()

            HideSignTypeDropdownToolbarControl()
            HidePinToolbarControls()
            If ProtocolEnv.RemoteSignEnabled Then
                ShowSignTypeDropdownToolbarControl()
                PopulateBySelectedSignOption(SetDefaultSignOption())
                If Not DocSuiteContext.Current.HasInfocertProxySign Then
                    signTypeDropdown.FindItemByValue("4").Remove()
                End If
                If Not DocSuiteContext.Current.HasArubaActalisSign Then
                    signTypeDropdown.FindItemByValue("3").Remove()
                End If
            End If
        End If
    End Sub

    Private Sub DocumentListGrid_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If Not e.CommandName.Eq("preview") Then
            Exit Sub
        End If

        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(DirectCast(e.Item, GridDataItem).GetDataKeyValue("Serialized").ToString()))
        AjaxManager.ResponseScripts.Add(String.Format("OpenGenericWindow('../Viewers/DocumentInfoViewer.aspx?{0}')", document.ToQueryString().AsEncodedQueryString()))
    End Sub

    Protected Sub DocumentListGrid_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim bound As MultiSignDocumentInfo = DirectCast(e.Item.DataItem, MultiSignDocumentInfo)
        Dim info As DocumentInfo = bound.DocumentInfo

        If bound.Mandatory Then
            e.Item.Selected = True
            Dim cb As CheckBox = CType(CType(e.Item, GridDataItem)("SelectColumn").Controls(0), CheckBox)
            cb.Checked = True
            cb.Enabled = bound.MandatorySelectable
        End If

        Dim documentImage As ImageButton = DirectCast(e.Item.FindControl("documentType"), ImageButton)
        documentImage.ImageUrl = ImagePath.FromDocumentInfo(info, True)

        Dim documentName As Label = DirectCast(e.Item.FindControl("lblFileName"), Label)
        documentName.Text = info.Name

        Dim lblDocType As Label = DirectCast(e.Item.FindControl("lblDocType"), Label)
        lblDocType.Text = bound.DocType

        Dim originalToggle As RadButton = DirectCast(e.Item.FindControl("originalToggle"), RadButton)
        Dim pdfToggle As RadButton = DirectCast(e.Item.FindControl("pdfToggle"), RadButton)
        Dim cadesToggle As RadButton = DirectCast(e.Item.FindControl("cadesToggle"), RadButton)
        Dim padesToggle As RadButton = DirectCast(e.Item.FindControl("padesToggle"), RadButton)
        ' macello, ma non conviene metterci testa perchè é in divenire, i radiobutton funzionano male quindi ho dovuto specificare per entrambi i toggle :(
        If FileHelper.MatchExtension(info.Name, FileHelper.PDF) Then
            originalToggle.Checked = True
            pdfToggle.Checked = False
            pdfToggle.Enabled = False
            If ProtocolEnv.EnableMultiPades Then
                cadesToggle.Checked = False
                padesToggle.Checked = True
                padesToggle.Enabled = True
            Else
                cadesToggle.Checked = True
                padesToggle.Checked = False
                padesToggle.Visible = False
            End If
        ElseIf FileHelper.MatchExtension(info.Name, FileHelper.P7M) Then
            originalToggle.Checked = True
            pdfToggle.Checked = False
            pdfToggle.Enabled = False
            cadesToggle.Checked = True
            padesToggle.Checked = False
            padesToggle.Enabled = False
            If Not ProtocolEnv.EnableMultiPades Then
                padesToggle.Visible = False
            End If
        Else
            If ProtocolEnv.EnableMultiPades Then
                originalToggle.Checked = False
                pdfToggle.Checked = True
                cadesToggle.Checked = False
                padesToggle.Checked = True
                padesToggle.Enabled = True
            Else
                originalToggle.Checked = True
                pdfToggle.Checked = False
                cadesToggle.Checked = True
                padesToggle.Checked = False
                padesToggle.Visible = False
            End If
        End If
        If ProtocolEnv.DefaultSignType.Eq(Helpers.Signer.CAdES.SignType.CAdES.ToString()) Then
            originalToggle.Checked = True
            pdfToggle.Checked = False
            cadesToggle.Checked = True
            padesToggle.Checked = False
        End If
    End Sub

    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case sourceControl.CommandName.ToLower()
            Case "sign"

                Dim selectedProvider As Signs.ProviderSignType = Signs.ProviderSignType.Smartcard
                If ProtocolEnv.RemoteSignEnabled Then
                    selectedProvider = CType(Convert.ToInt32(signTypeDropdown.SelectedValue), Signs.ProviderSignType)
                End If

                FileLogger.Debug(LoggerName, "Begin multiple sign")
                If selectedProvider <> Signs.ProviderSignType.Smartcard AndAlso Not SignAction.Eq(CollaborationMainAction.DaFirmareInDelega) AndAlso Not SignAction.Eq(CollaborationSubAction.DaFirmareInDelega) AndAlso Not ValidateSubmit(selectedProvider) Then
                    Return
                End If

                ' Preparo i documenti per la firma
                Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()

                'Creazione Directory esportazione
                Dim tempDir As New DirectoryInfo(TempDirectoryName)
                If tempDir.Exists Then
                    tempDir.Delete(True)
                End If
                tempDir.Create()
                Dim inDir As DirectoryInfo = tempDir.CreateSubdirectory("In")

                If Not inDir.Exists Then
                    impersonator.ImpersonationUndo()
                    Throw New DocSuiteException("Errore di firma", "Impossibile creare directory temporanea per i file da firmare, controllare autorizzazioni.")
                End If

                'Estrazione documenti per la firma
                OriginalDocumentInfos = New List(Of DocumentInfo)()
                Dim signTypes As List(Of String) = New List(Of String)
                Dim inDocuments As List(Of FileInfo) = New List(Of FileInfo)()
                Dim document As MultiSignDocumentInfo
                Dim cadesToggle As RadButton
                Dim pdfToggle As RadButton
                Dim originalToggle As RadButton
                Dim newName As String
                Dim inFilename As String
                Dim documentTosigns As Dictionary(Of String, List(Of FileInfo)) = New Dictionary(Of String, List(Of FileInfo))()
                Dim docFileInfo As FileInfo
                For Each dataGridItem As GridDataItem In DocumentListGrid.SelectedItems
                    ' Documento interessato
                    document = New MultiSignDocumentInfo(HttpUtility.ParseQueryString(dataGridItem.GetDataKeyValue("Serialized").ToString()))
                    ' Pulsanti griglia
                    cadesToggle = DirectCast(dataGridItem.FindControl("cadesToggle"), RadButton)
                    pdfToggle = DirectCast(dataGridItem.FindControl("pdfToggle"), RadButton)
                    originalToggle = DirectCast(dataGridItem.FindControl("originalToggle"), RadButton)

                    ' Creo il nuovo nome
                    newName = document.DocumentInfo.Name
                    If pdfToggle.Checked Then
                        ' Se si vuole firmare la versione pdf sostituisco la vecchia estensione con .pdf
                        newName = $"{document.DocumentInfo.Name.Substring(0, document.DocumentInfo.Name.Length - document.DocumentInfo.Extension.Length)}{FileHelper.PDF}"
                    End If
                    document.DocumentInfo.Name = newName

                    inDocuments = New List(Of FileInfo)()
                    ' Salvo nella cartella
                    If originalToggle.Checked Then
                        inFilename = $"{document.IdOwner}§{document.DocumentInfo.Name}"
                        document.DocumentInfo.SaveToDisk(inDir, inFilename)
                    Else
                        inFilename = $"{document.IdOwner}§{newName}"
                        document.DocumentInfo.SavePdfNoSignature(inDir, inFilename)
                    End If

                    OriginalDocumentInfos.Add(document.DocumentInfo)
                    If Not documentTosigns.ContainsKey(document.EffectiveSigner) Then
                        documentTosigns.Add(document.EffectiveSigner, New List(Of FileInfo)())
                    End If
                    docFileInfo = New FileInfo(Path.Combine(inDir.FullName, inFilename))
                    inDocuments.Add(docFileInfo)
                    documentTosigns(document.EffectiveSigner).Add(docFileInfo)
                    ' Compongo la lista delle tipologie di firma da effettuare
                    signTypes.Add(If(cadesToggle.Checked, Helpers.Signer.CAdES.SignType.CAdES.ToString("D"), Helpers.Signer.CAdES.SignType.PAdES.ToString("D")))
                Next

                Dim outDir As DirectoryInfo = tempDir.CreateSubdirectory("Out")

                impersonator.ImpersonationUndo()

                Dim comment As String = If(ChkAddComment.Checked, ProtocolEnv.DefaultFVicario, "")

                If selectedProvider = ProviderSignType.Smartcard Then
                    'Use ActiveX for smartcard sign
                    AjaxManager.ResponseScripts.Add(String.Format("riseSign('{0}','{1}','{2}','{3}')", inDir.FullName.Replace("\", "\\"), outDir.FullName.Replace("\", "\\"), String.Join(",", signTypes), comment))
                Else
                    Dim defaultRemoteSignProperty As RemoteSignProperty = GetSelectedUserProfile(selectedProvider).Value.Value
                    Dim selectRemoteSignProperty As RemoteSignProperty
                    Dim pinPassword As String = PinTextbox.Text
                    For Each listdocuments As KeyValuePair(Of String, List(Of FileInfo)) In documentTosigns
                        selectRemoteSignProperty = defaultRemoteSignProperty
                        If Not (String.IsNullOrEmpty(listdocuments.Key)) Then
                            selectRemoteSignProperty = GetSpecificUserProfile(listdocuments.Key)
                            pinPassword = selectRemoteSignProperty.Password
                        End If
                        Sign(selectedProvider, selectRemoteSignProperty, signTypes, listdocuments.Value, outDir, pinPassword)
                    Next
                End If

            Case "undo"
                SignedDocuments = New List(Of MultiSignDocumentInfo)
                Response.Redirect(String.Format("{0}&{1}=true", PostBackUrl, MultiSignUndoQuerystring))

            Case "originaltoggle"
                ToggleSignDocumentType(sourceControl.CommandName)

            Case "pdftoggle"
                ToggleSignDocumentType(sourceControl.CommandName)

            Case "cadestoggle"
                ToggleSignType(sourceControl.CommandName)

            Case "padestoggle"
                ToggleSignType(sourceControl.CommandName)
        End Select
    End Sub

    Protected Sub Page_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        If Not e.Argument.Eq("Signed") Then
            Exit Sub
        End If

        If Integer.Parse(hdnHasError.Value) > 0 Then
            hdnHasError.Value = "0"
            AjaxAlert("Sono presenti errori")
            Exit Sub
        End If

        Dim outputDirectory As New DirectoryInfo(Path.Combine(TempDirectoryName, "Out"))
        Dim documents As New List(Of MultiSignDocumentInfo)
        For Each fileInfo As FileInfo In outputDirectory.GetFiles()
            documents.Add(New MultiSignDocumentInfo(fileInfo))
        Next

        SignedDocuments = documents
        MyBase.SignedComplete = False
        If MyBase.SaveResponseToSession Then
            MyBase.SignedComplete = True
        End If

        ' Trasferisco la response sulla pagina che dovrà occuparsi di processare i documenti firmati
        Server.Transfer(PostBackUrl, False)

    End Sub

    Protected Sub signTypeDropdown_SelectedIndexChanged(sender As Object, e As DropDownListEventArgs)
        Dim selectedProvider As Signs.ProviderSignType = CType(Convert.ToInt32(e.Value), Signs.ProviderSignType)

        If CurrentUserProfile IsNot Nothing Then
            Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = Nothing
            If CurrentUserProfile.Value.Any(Function(x) x.Key = selectedProvider) Then
                selectedSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Key = selectedProvider)
            End If
            PopulateBySelectedSignOption(selectedSignOption)
        End If

        signTypeDropdown.SelectedValue = e.Value

        Dim ss As String = String.Format("hideAjaxLoadingPanel('{0}');", ToolBar.ClientID)
        AjaxManager.ResponseScripts.Add(ss)

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf Page_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, ToolBar, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Dim sourcePage As ISignMultipleDocuments = TryCast(PreviousPage, ISignMultipleDocuments)
        If sourcePage Is Nothing Then
            Throw New DocSuiteException("Firma Multipla", "Pagina di provenienza errata.")
        End If

        MyBase.SignedComplete = Nothing

        DocumentListGrid.MasterTableView.GetColumn("SignType").Visible = ProtocolEnv.EnableMultiPades
        RadToolBarVisibility()

        If ProtocolEnv.DefaultFVicario.IsNullOrEmpty() Then
            ChkAddComment.Visible = False
        Else
            ChkAddComment.Checked = Facade.RoleUserFacade.GetHighestUserType() = RoleUserType.V
        End If

        Dim documentsToSign As ICollection(Of MultiSignDocumentInfo) = sourcePage.DocumentsToSign
        OriginalDocuments = documentsToSign.Where(Function(x) Not x.Signers.Any(Function(xx) xx.Eq(DocSuiteContext.Current.User.FullUserName) OrElse xx.Eq(x.EffectiveSigner))).ToList()
        If OriginalDocuments.IsNullOrEmpty() Then
            Throw New InformationException("Firma Multipla", "Nessun documento da firmare.")
        End If

        If ((OriginalDocuments.Count - documentsToSign.Count) < 0) Then
            AjaxAlert("Alcuni documenti risultano già firmati dall'utente e sono stati scartati")
        End If

        SignAction = sourcePage.SignAction
        DocumentListGrid.DataSource = OriginalDocuments
        DocumentListGrid.DataBind()

        PostBackUrl = sourcePage.ReturnUrl
    End Sub

    Private Sub RadToolBarVisibility()
        ToolBar.FindItemByValue("signSeparator").Visible = ProtocolEnv.EnableMultiPades
        ToolBar.FindItemByValue("signDescription").Visible = ProtocolEnv.EnableMultiPades
        ToolBar.FindItemByValue("cadesSign").Visible = ProtocolEnv.EnableMultiPades
        ToolBar.FindItemByValue("padesSign").Visible = ProtocolEnv.EnableMultiPades
    End Sub

    Private Sub ToggleSignDocumentType(documentType As String)
        Dim document As MultiSignDocumentInfo
        Dim pdfToggle As RadButton
        Dim originalToggle As RadButton
        For Each dataGridItem As GridDataItem In DocumentListGrid.Items
            document = New MultiSignDocumentInfo(HttpUtility.ParseQueryString(dataGridItem.GetDataKeyValue("Serialized").ToString()))
            pdfToggle = DirectCast(dataGridItem.FindControl("pdfToggle"), RadButton)
            originalToggle = DirectCast(dataGridItem.FindControl("originalToggle"), RadButton)

            pdfToggle.Checked = pdfToggle.Enabled AndAlso documentType.Eq("pdftoggle")
            originalToggle.Checked = (originalToggle.Enabled AndAlso documentType.Eq("originaltoggle")) _
                                            OrElse (originalToggle.Enabled AndAlso documentType.Eq("pdftoggle") _
                                                        AndAlso (FileHelper.MatchExtension(document.DocumentInfo.Name, FileHelper.PDF) _
                                                                        OrElse FileHelper.MatchExtension(document.DocumentInfo.Name, FileHelper.P7M)))
        Next
    End Sub

    Private Sub ToggleSignType(signType As String)
        Dim document As MultiSignDocumentInfo
        Dim cadesToggle As RadButton
        Dim padesToggle As RadButton
        For Each dataGridItem As GridDataItem In DocumentListGrid.Items
            document = New MultiSignDocumentInfo(HttpUtility.ParseQueryString(dataGridItem.GetDataKeyValue("Serialized").ToString()))
            cadesToggle = DirectCast(dataGridItem.FindControl("cadesToggle"), RadButton)
            padesToggle = DirectCast(dataGridItem.FindControl("padesToggle"), RadButton)

            cadesToggle.Checked = (cadesToggle.Enabled AndAlso signType.Eq("cadestoggle") _
                                            OrElse (cadesToggle.Enabled AndAlso signType.Eq("padestoggle") _
                                                        AndAlso FileHelper.MatchExtension(document.DocumentInfo.Name, FileHelper.P7M)))
            padesToggle.Checked = padesToggle.Enabled AndAlso signType.Eq("padestoggle")
        Next
    End Sub

    Function SetDefaultSignOption() As Nullable(Of KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty))
        Dim defaultSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = Nothing
        If CurrentUserProfile IsNot Nothing Then
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

    Private Sub PopulateBySelectedSignOption(selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)?)
        If selectedSignOption.HasValue Then
            HidePinToolbarControls()
            If selectedSignOption.Value.Key = Signs.ProviderSignType.ArubaAutomatic OrElse selectedSignOption.Value.Key = Signs.ProviderSignType.InfocertAutomatic Then
                ShowPinToolbarControls()
                PopulatePin(selectedSignOption.Value.Value.Password)

                Select Case selectedSignOption.Value.Value.StorageInformationType
                    Case Signs.StorageInformationType.Forever
                        PopulatePin(selectedSignOption.Value.Value.Password)
                    Case Signs.StorageInformationType.Session
                        Dim signOptionPassword As IDictionary(Of String, String)
                        If Session("signOptionPassword") IsNot Nothing AndAlso Session("signOptionPassword") <> "null" Then
                            signOptionPassword = JsonConvert.DeserializeObject(Of IDictionary(Of String, String))(CType(Session("signOptionPassword"), String))
                            Dim existingPin As String = signOptionPassword(selectedSignOption.Value.Key.ToString("D"))
                            If Not String.IsNullOrEmpty(existingPin) Then
                                PopulatePin(existingPin)
                            End If

                        End If
                End Select
            End If
        End If

        If SignAction.Eq(CollaborationMainAction.DaFirmareInDelega) OrElse SignAction.Eq(CollaborationSubAction.DaFirmareInDelega) Then
            PopulatePin("")
            PinTextbox.Visible = False
            ChkAddComment.Checked = True
            ChkAddComment.Enabled = False
            signTypeDropdown.Enabled = False

            If DocSuiteContext.Current.HasInfocertProxySign Then
                signTypeDropdown.SelectedValue = Convert.ToString(ProviderSignType.InfocertAutomatic)
            End If
            If DocSuiteContext.Current.HasArubaActalisSign Then
                signTypeDropdown.SelectedValue = Convert.ToString(ProviderSignType.ArubaAutomatic)
            End If
        End If
    End Sub

    Private Sub PopulatePin(pin As String)
        Dim ss As String = String.Format("document.getElementById('{0}').value = '{1}';", PinTextbox.ClientID, pin)
        AjaxManager.ResponseScripts.Add(ss)
    End Sub

    Private Function GetSelectedUserProfile(selectedProvider As Signs.ProviderSignType) As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)?
        Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = Nothing
        If CurrentUserProfile IsNot Nothing AndAlso CurrentUserProfile.Value.Any(Function(x) x.Key = selectedProvider) Then
            selectedSignOption = CurrentUserProfile.Value.FirstOrDefault(Function(x) x.Key = selectedProvider)
        End If
        Return selectedSignOption
    End Function

    Private Function ValidateSubmit(selectedProvider As Signs.ProviderSignType) As Boolean

        Dim selectedSignOption As KeyValuePair(Of Signs.ProviderSignType, Signs.RemoteSignProperty)? = GetSelectedUserProfile(selectedProvider)

        If selectedProvider = Signs.ProviderSignType.ArubaRemote OrElse selectedProvider = Signs.ProviderSignType.ArubaAutomatic Then
            If Not selectedSignOption.HasValue OrElse (selectedSignOption.HasValue AndAlso String.IsNullOrEmpty(selectedSignOption.Value.Value.Alias)) Then
                AjaxAlert("Profilo di firma non configurato correttamente. Verificare di aver impostato l'Alias del certificato mediante la pagina configurazione utente.")
                Return False
            End If
        End If

        If selectedProvider = Signs.ProviderSignType.InfocertRemote Then
            AjaxAlert("La firma remota di Infocert non è supportata nella modalità firma massiva.")
            Return False
        End If

        If selectedProvider = Signs.ProviderSignType.InfocertAutomatic Then
            If Not selectedSignOption.HasValue OrElse (selectedSignOption.HasValue AndAlso String.IsNullOrEmpty(selectedSignOption.Value.Value.Alias)) Then
                AjaxAlert("Profilo di firma non configurato correttamente. Verificare di aver impostato l'Alias del certificato mediante la pagina configurazione utente.")
                Return False
            End If
        End If

        If String.IsNullOrEmpty(PinTextbox.Text) Then
            AjaxAlert("Inserire Password.")
            Return False
        End If

        Return True
    End Function

    Private Sub Sign(selectedSignType As ProviderSignType, remoteSignProperty As RemoteSignProperty, signTypes As List(Of String), inDocuments As List(Of FileInfo), outDir As DirectoryInfo, pinPassword As String)
        Dim documents As New List(Of MultiSignDocumentInfo)
        Dim newFilename As String
        Dim pathFilename As String
        Dim extension As String
        Dim signService As SignService = New SignService(Sub(f) FileLogger.Info(LoggerName, f), Sub(f) FileLogger.Error(LoggerName, f))

        Dim signParameter As ISignParameter = Nothing
        Dim signatureType As SignatureType = SignatureType.ArubaSign

        If selectedSignType = ProviderSignType.ArubaAutomatic OrElse selectedSignType = ProviderSignType.ArubaRemote Then
            signParameter = New ArubaSignModel With {
                        .DelegatedDomain = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_DOMAIN),
                        .DelegatedPassword = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_PASSWORD),
                        .DelegatedUser = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_DELEGATED_USER),
                        .OTPPassword = remoteSignProperty.OTP,
                        .OTPAuthType = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_OTP_AUTHTYPE),
                        .User = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_USER),
                        .CertificateId = remoteSignProperty.CustomProperties(RemoteSignProperty.ARUBA_CERTIFICATEID),
                        .SignType = If(selectedSignType = ProviderSignType.ArubaRemote, SignModel.SignType.Remote, SignModel.SignType.Automatic),
                        .RequestType = SignModel.SignRequestType.Cades
                    }
        End If

        If selectedSignType = ProviderSignType.InfocertAutomatic OrElse selectedSignType = ProviderSignType.InfocertRemote Then
            signatureType = SignatureType.ProxySign
            signParameter = New ProxySignModel With {
                    .Alias = remoteSignProperty.Alias,
                    .OTPPassword = remoteSignProperty.OTP,
                    .PINPassword = pinPassword,
                    .SignType = If(selectedSignType = ProviderSignType.InfocertRemote, SignModel.SignType.Remote, SignModel.SignType.Automatic),
                    .RequestType = SignModel.SignRequestType.Cades
                    }
        End If

        For Each document As FileInfo In inDocuments
            Try
                newFilename = $"{document.Name}{FileHelper.P7M}"
                pathFilename = Path.Combine(outDir.FullName, newFilename)
                extension = document.Extension
                File.WriteAllBytes(pathFilename, signService.SignDocument(signParameter, File.ReadAllBytes(document.FullName), newFilename, signatureType))
                documents.Add(New MultiSignDocumentInfo(New FileInfo(pathFilename)))

            Catch ex As PathTooLongException
                FileLogger.Warn(LoggerName, "Nome documento da firmare troppo lungo", ex)
                AjaxAlert("Nome del documento troppo lungo.")
            Catch ex As Exception
                FileLogger.Error(LoggerName, String.Format("Errore firma del documento [{0}].", document.Name), ex)
                AjaxAlert("Errore generico nella firma del documento, contattare l'assistenza.")
            End Try
        Next

        If selectedSignType = ProviderSignType.ArubaRemote Then
            signService.CloseMultiSignSession(signParameter, SignModel.SignatureType.ArubaSign)
        End If

        SignedDocuments = documents
        MyBase.SignedComplete = False
        If MyBase.SaveResponseToSession Then
            MyBase.SignedComplete = True
        End If

        ' Trasferisco la response sulla pagina che dovrà occuparsi di processare i documenti firmati
        Server.Transfer(PostBackUrl, False)

    End Sub

    Private Sub ShowPinToolbarControls()
        ToolBar.Items.FindItemByValue("pinContainer").SetDisplay(True)
        ToolBar.Items.FindItemByValue("pinContainer2").SetDisplay(True)
        ToolBar.Items.FindItemByValue("pinSeparator").SetDisplay(True)
    End Sub

    Private Sub HidePinToolbarControls()
        ToolBar.Items.FindItemByValue("pinContainer").SetDisplay(False)
        ToolBar.Items.FindItemByValue("pinContainer2").SetDisplay(False)
        ToolBar.Items.FindItemByValue("pinSeparator").SetDisplay(False)
    End Sub

    Private Sub ShowSignTypeDropdownToolbarControl()
        ToolBar.Items.FindItemByValue("signTypeDropdown").SetDisplay(True)
    End Sub

    Private Sub HideSignTypeDropdownToolbarControl()
        ToolBar.Items.FindItemByValue("signTypeDropdown").SetDisplay(False)
    End Sub

    Private Function GetSpecificUserProfile(effectiveSigner As String) As RemoteSignProperty
        Dim selectedSignOption As Signs.RemoteSignProperty = Nothing
        Dim effectiveUserLog As UserLog = Facade.UserLogFacade.GetByUser(effectiveSigner)
        If Not String.IsNullOrEmpty(effectiveUserLog.UserProfile) Then
            Dim specificUserProfile As Model.Documents.Signs.UserProfile = JsonConvert.DeserializeObject(Of Model.Documents.Signs.UserProfile)(effectiveUserLog.UserProfile)
            ' da migliorare
            If DocSuiteContext.Current.HasInfocertProxySign Then
                selectedSignOption = specificUserProfile.Value.FirstOrDefault(Function(x) x.Key = ProviderSignType.InfocertAutomatic).Value
            End If
            If DocSuiteContext.Current.HasArubaActalisSign Then
                selectedSignOption = specificUserProfile.Value.FirstOrDefault(Function(x) x.Key = ProviderSignType.ArubaAutomatic).Value
            End If
        End If
        Return selectedSignOption
    End Function
#End Region

End Class