Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports VecompSoftware.Services.Biblos
Imports System.Linq
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.UDSDesigner
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Compress

Public Class PECAttachToDocumentUnit
    Inherits PECBasePage

#Region " Fields "
    Private _whiteList() As String
    Private _blackList() As String
    Private Const ODATA_EQUAL_UDSYEARNUMBER As String = "$filter=_year eq {0} and _number eq {1}"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private Const ON_ERROR_FUNCTION As String = "onError('{0}')"
    Public Const NOTIFICATION_SUCCESS_ICON As String = "ok"
    Public Const NOTIFICATION_ERROR_ICON As String = "delete"
    Public Const CONFIRM_UDS_CLICK As String = "confirmUds"
    Public Const COMMAND_SUCCESS As String = "Attendere il termine dell'attività di {0}."
#End Region

#Region " Properties "

    Public ReadOnly Property WhiteList() As String()
        Get
            If _whiteList Is Nothing Then
                If Not String.IsNullOrEmpty(ProtocolEnv.FileExtensionWhiteList) Then
                    _whiteList = ProtocolEnv.FileExtensionWhiteList.Split("|"c)
                Else
                    _whiteList = New String() {}
                End If
            End If
            Return _whiteList
        End Get
    End Property

    Public ReadOnly Property BlackList() As String()
        Get
            If _blackList Is Nothing Then
                If Not String.IsNullOrEmpty(ProtocolEnv.FileExtensionBlackList) Then
                    _blackList = ProtocolEnv.FileExtensionBlackList.Split("|"c)
                Else
                    _blackList = New String() {}
                End If
            End If
            Return _blackList
        End Get
    End Property

    Public ReadOnly Property AllDocuments As List(Of DocumentInfo)
        Get

            Dim list As New List(Of DocumentInfo)
            For Each selectedItem As GridDataItem In DocumentListGrid.MasterTableView.GetSelectedItems()
                list.Add(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(selectedItem.GetDataKeyValue("Serialized"), String))))
            Next
            Return list

        End Get
    End Property

    Public ReadOnly Property DocumentsSource As List(Of DocumentInfo)
        Get
            Dim source As New List(Of DocumentInfo)
            For Each selectedItem As GridDataItem In DocumentListGrid.MasterTableView.Items
                source.Add(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(selectedItem.GetDataKeyValue("Serialized"), String))))
            Next
            Return source
        End Get
    End Property

    Private ReadOnly Property SelectedEnvironmentType As DSWEnvironment
        Get
            Return DirectCast([Enum].Parse(GetType(DSWEnvironment), rblDocumentUnit.SelectedValue), DSWEnvironment)
        End Get
    End Property

    Private ReadOnly Property SelectedRepositoryId As Guid?
        Get
            If Not String.IsNullOrEmpty(ddlAvailableUDS.SelectedValue) Then
                Return Guid.Parse(ddlAvailableUDS.SelectedValue)
            End If
            Return Nothing
        End Get
    End Property

    Protected ReadOnly Property SignalRServerAddress As String
        Get
            Return DocSuiteContext.SignalRServerAddress
        End Get
    End Property

    Private Property CurrentUDSSelected As UDSKeysDto
        Get
            If ViewState("CurrentUDSSelected") IsNot Nothing Then
                Return DirectCast(ViewState("CurrentUDSSelected"), UDSKeysDto)
            End If
            Return Nothing
        End Get
        Set(value As UDSKeysDto)
            ViewState("CurrentUDSSelected") = value
        End Set
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        uscPECInfo.PecLabel = PecLabel.ToLower()
        InitializeAjax()

        If Not IsPostBack Then
            Title = String.Format("{0} - Allega", PecLabel)
            cmdCancel.PostBackUrl = PreviousPageUrl

            txtYear.Text = DateTime.Today.Year.ToString()
            txtNumber.Focus()
            lbSelectedDocumentUnit.Text = "Selezione Protocollo"

            Dim protocolItem As ListItem = New ListItem("Protocollo", DSWEnvironment.Protocol.ToString())
            Dim UDSItem As ListItem = New ListItem("Archivio", DSWEnvironment.UDS.ToString())
            protocolItem.Selected = True
            rblDocumentUnit.Items.Add(protocolItem)
            rblDocumentUnit.Items.Add(UDSItem)

            Dim availableContainers As IList(Of Data.Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.UDS, UDSRightPositions.PECIngoing, True)
            If ProtocolEnv.UDSEnabled AndAlso availableContainers.Count > 0 Then
                tbDocumentUnitSelect.Visible = True
            End If

            If CurrentPecMailList IsNot Nothing Then
                If CurrentPecMailList.Count = 1 Then
                    uscPECInfo.PECMail = CurrentPecMail
                    uscPECInfo.BindData(CurrentPecMail)
                Else
                    uscPECInfo.Visible = False
                End If
            End If

            Dim docs As List(Of DocumentInfo) = InitializeDocuments()

            DocumentListGrid.DataSource = docs
            DocumentListGrid.DataBind()
        End If

    End Sub

    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        Dim vYear As Short
        Dim vNumber As Integer
        If Not Short.TryParse(txtYear.Text, vYear) OrElse Not Integer.TryParse(txtNumber.Text, vNumber) Then
            AjaxAlert(String.Format("Protocollo [{0}/{1}] non trovato.", txtYear.Text, txtNumber.Text))
            Exit Sub
        End If

        If SelectedEnvironmentType = DSWEnvironment.UDS Then
            FindUDS(vYear, vNumber)
        Else
            FindProtocol(vYear, vNumber)
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click
        If SelectedEnvironmentType = DSWEnvironment.UDS Then
            AttachToUDS()
        Else
            AttachToProtocol()
        End If
    End Sub

    Private Sub DocumentListGridCommandDataBound(sender As Object, e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If e.CommandName.Eq("unzip") AndAlso Not String.IsNullOrEmpty(CType(e.CommandArgument, String)) Then
            Dim documentId As Guid = Guid.Parse(e.CommandArgument.ToString())
            Dim txtPassword As HiddenField = CType(e.Item.FindControl("txtPassword"), HiddenField)
            Dim key As String = DocumentListGrid.Items(e.Item.ItemIndex).GetDataKeyValue("Serialized").ToString()
            Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
            Dim zipDoc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
            If DocumentsSource Is Nothing Then
                AjaxAlert("Errore nella gestione degli allegati.")
                Exit Sub
            End If
            Dim documenti As List(Of DocumentInfo) = DocumentsSource
            Try
                Dim compressManager As ICompress = New ZipCompress()
                If FileHelper.MatchExtension(zipDoc.Name, FileHelper.RAR) Then
                    compressManager = New RarCompress()
                End If

                Dim extracted As List(Of CompressItem) = compressManager.InMemoryExtract(New MemoryStream(zipDoc.Stream), password:=txtPassword.Value)
                For Each item As CompressItem In extracted
                    Dim memoryInfo As New MemoryDocumentInfo(item.Data, item.Filename)
                    Dim file As New TempFileDocumentInfo(BiblosFacade.SaveUniqueToTemp(memoryInfo))
                    file.Name = item.Filename
                    documenti.Add(file)
                Next
                Dim zip As DocumentInfo = documenti.FirstOrDefault(Function(f) (TypeOf f Is BiblosDocumentInfo) AndAlso DirectCast(f, BiblosDocumentInfo).DocumentId = documentId)
                If zip IsNot Nothing Then
                    documenti.Remove(zip)
                End If
                DocumentListGrid.DataSource = documenti.ToList()
                DocumentListGrid.DataBind()
            Catch ex As BadPasswordException
                AjaxAlert("Password errata. Specificare una nuova password.")
            Catch ex As ExtractException
                AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
            End Try
        End If
    End Sub

    Private Sub DocumentListGridItemDataBound(sender As Object, e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem Then
            Dim txtPassword As HiddenField = CType(e.Item.FindControl("txtPassword"), HiddenField)
            Dim imgDecrypt As ImageButton = CType(e.Item.FindControl("imgDecrypt"), ImageButton)
            Dim btnUnzip As Button = CType(e.Item.FindControl("btnUnzip"), Button)
            Dim bound As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)
            Dim chk As Boolean? = CheckDocument(bound.Name)
            If chk.HasValue Then
                If chk.Value = True Then
                    e.Item.Selected = True
                Else
                    e.Item.SelectableMode = GridItemSelectableMode.None
                End If
            Else
                e.Item.Selected = False
            End If
            If FileHelper.MatchExtension(bound.Name, FileHelper.ZIP) OrElse FileHelper.MatchExtension(bound.Name, FileHelper.RAR) Then
                Try
                    Dim compressManager As ICompress = New ZipCompress()
                    If FileHelper.MatchExtension(bound.Name, FileHelper.RAR) Then
                        compressManager = New RarCompress()
                    End If
                    If compressManager.HasPassword(bound.Stream) Then
                        imgDecrypt.Visible = True
                        imgDecrypt.OnClientClick = String.Format("showDialogInitially('{0}','{1}');return false;", txtPassword.ClientID, btnUnzip.ClientID)
                        btnUnzip.Visible = True
                        If (TypeOf bound Is BiblosDocumentInfo) Then
                            btnUnzip.CommandArgument = DirectCast(bound, BiblosDocumentInfo).DocumentId.ToString()
                        End If
                    End If
                Catch ex As ExtractException
                    AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
                End Try
            End If
        End If

        If e.Item.ItemType = GridItemType.Header Then
            CType(CType(e.Item, GridHeaderItem)("Select").Controls(0), CheckBox).ToolTip = "Selezione documenti da Allegare al Protocollo."
        End If
    End Sub

    Protected Sub rblDocumentUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDocumentUnit.SelectedIndexChanged
        uscProtocolPreview.Visible = False
        udsPanel.Visible = False
        btnAdd.Enabled = False
        If SelectedEnvironmentType = DSWEnvironment.UDS Then
            btnAdd.AutoPostBack = False
            btnAdd.OnClientClicked = CONFIRM_UDS_CLICK
            pnlUDSSelected.Visible = True
            lbSelectedDocumentUnit.Text = "Selezione Archivio"
        Else
            btnAdd.AutoPostBack = True
            btnAdd.OnClientClicked = Nothing
            pnlUDSSelected.Visible = False
            lbSelectedDocumentUnit.Text = "Selezione Protocollo"
        End If
    End Sub

    Protected Sub PECAttachToDocumentUnit_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As Object() = arg.Split("|"c)
        If arguments.Length = 0 Then
            Exit Sub
        End If

        Dim argumentName As String = arguments(0).ToString()

        Select Case argumentName
            Case "udscallback"
                Dim udsId As Guid = Guid.Parse(arguments(1).ToString())
                Dim udsRepositoryId As Guid = Guid.Parse(arguments(2).ToString())
                Response.Redirect(String.Format("~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}", udsId, udsRepositoryId))
        End Select
    End Sub
#End Region

#Region " Methods "

    Private Function InitializeDocuments() As List(Of DocumentInfo)
        Dim docs As New List(Of DocumentInfo)
        If (PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is IHaveViewerLight AndAlso PreviousPage.IsCrossPagePostBack) Then
            Dim chkDocs As List(Of DocumentInfo) = CType(PreviousPage, IHaveViewerLight).CheckedDocuments
            docs = Facade.PECMailFacade.GetDocumentList(chkDocs)
        Else
            If CurrentPecMailList.Count = 1 Then
                ' Nel caso non ci siano documenti sulla PreviousPage carico tutti i documenti della PEC
                docs = Facade.PECMailFacade.GetDocumentList(CurrentPecMailWrapper)
            End If
        End If
        Return docs
    End Function

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf PECAttachToDocumentUnit_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentListGrid)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAdd, ContentPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAdd, btnAdd, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscProtocolPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, tblFindDocumentUnit, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, udsPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, btnAdd)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, btnAdd)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, udsPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, uscProtocolPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, tbDocumentUnitSelect, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, lbSelectedDocumentUnit)
        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
    End Sub

    Public Function VerifyNotInclusion(file As String) As Boolean
        If String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList) Then
            Return True
        End If

        Dim ext As String = Path.GetExtension(file)
        Return Not StringHelper.ExistsIn(DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList, ext, "|"c, String.Empty)
    End Function

    Protected Function GetNotes(name As String) As String
        Dim v As Boolean? = CheckDocument(name)
        If v.HasValue Then
            If v.Value Then
                Return "OK"
            Else
                Return "Impossibile protocollare questo tipo di documento."
            End If
        Else
            Return "Documento non previsto"
        End If
    End Function

    Private Function CheckDocument(name As String) As Boolean?
        If WhiteList.Count = 0 AndAlso BlackList.Count = 0 Then
            Return True ' Nessun controllo richiesto
        End If
        If FileHelper.MatchExtension(name, BlackList) Then
            Return False ' File in Black List 
        End If
        If FileHelper.MatchExtension(name, WhiteList) Then
            Return True ' File in White List
        End If
        If ProtocolEnv.EnableGrayList Then
            Return Nothing ' Gray List
        End If
        Return False ' File non in WhiteList e GrayList disabilitata
    End Function

    Private Sub FindProtocol(year As Short, number As Integer)
        Dim selectedProtocol As Protocol = Facade.ProtocolFacade.GetById(year, number, False)
        If selectedProtocol Is Nothing Then
            AjaxAlert(String.Format("Protocollo [{0}/{1}] non trovato.", year, number))
            Exit Sub
        End If

        ' L'utente non ha i diritti di modifica sul protocollo selezionato
        If Not (New ProtocolRights(selectedProtocol).IsEditable) Then
            AjaxAlert(String.Format("Protocollo [{0}]{1}Mancano i diritti necessari", ProtocolFacade.ProtocolFullNumber(year, number), Environment.NewLine))
            Exit Sub
        End If

        uscProtocolPreview.CurrentProtocol = selectedProtocol
        uscProtocolPreview.Visible = True
        uscProtocolPreview.Initialize()
        btnAdd.Enabled = True
        txtYear.Text = selectedProtocol.Year.ToString()
        txtNumber.Text = selectedProtocol.Number.ToString()
    End Sub

    Private Sub FindUDS(year As Short, number As Integer)
        If Not SelectedRepositoryId.HasValue Then
            AjaxAlert("Nessun archivio selezionato")
            Exit Sub
        End If

        Dim selectedRepository As UDSRepository = CurrentUDSRepositoryFacade.GetById(SelectedRepositoryId.Value)
        Dim selectedUDS As UDSDto = CurrentUDSFacade.GetUDSSource(selectedRepository, String.Format(ODATA_EQUAL_UDSYEARNUMBER, year, number))
        If selectedUDS Is Nothing Then
            AjaxAlert(String.Format("Archivio [{0}/{1}] non trovato.", year, number))
            Exit Sub
        End If

        ' L'utente non ha i diritti di modifica sul protocollo selezionato
        If Not Facade.ContainerFacade.CheckContainerRight(selectedRepository.Container.Id, DSWEnvironment.UDS, UDSRightPositions.Modify, True) Then
            AjaxAlert(String.Format("Archivio [{0}]{1}Mancano i diritti necessari", selectedRepository.Name, Environment.NewLine))
            Exit Sub
        End If

        udsPanel.Visible = True
        lblObject.Text = selectedUDS.UDSModel.Model.Subject.Value
        Dim category As Data.Category = Facade.CategoryFacade.GetById(Integer.Parse(selectedUDS.UDSModel.Model.Category.IdCategory))
        lblCategoryDescription.Text = category.Name
        lblCategoryCode.Text = category.FullCodeDotted
        lblContainer.Text = selectedRepository.Container.Name
        cmdUDS.Icon.PrimaryIconUrl = ImagePath.SmallDocumentSeries
        cmdUDS.Icon.PrimaryIconHeight = Unit.Pixel(16)
        cmdUDS.Icon.PrimaryIconWidth = Unit.Pixel(16)
        cmdUDS.Text = String.Format("{0}/{1:0000000} del {2}", selectedUDS.Year, selectedUDS.Number, selectedUDS.RegistrationDate.ToString("dd/MM/yyyy"))
        cmdUDS.NavigateUrl = String.Format("~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}", selectedUDS.Id, selectedRepository.Id)
        btnAdd.Enabled = True
        txtYear.Text = selectedUDS.Year.ToString()
        txtNumber.Text = selectedUDS.Number.ToString()
        CurrentUDSSelected = New UDSKeysDto() With {.UDSId = selectedUDS.Id, .IdUDSRepository = selectedRepository.Id}
    End Sub

    Public Function GetRepositories() As ICollection(Of RadComboBoxItem)
        Dim repositories As IList(Of UDSRepository) = CurrentUDSRepositoryFacade.GetByPecAnnexedEnabled(String.Empty)

        Dim rcbItems As ICollection(Of RadComboBoxItem) = New List(Of RadComboBoxItem)
        For Each repository As UDSRepository In repositories
            If Facade.ContainerFacade.CheckContainerRight(repository.Container.Id, DSWEnvironment.UDS, UDSRightPositions.PECIngoing, True) Then
                rcbItems.Add(New RadComboBoxItem(repository.Name, repository.Id.ToString()))
            End If
        Next
        Return rcbItems
    End Function

    Private Sub AttachToProtocol()
        Dim selectedProtocol As Protocol = uscProtocolPreview.CurrentProtocol
        If selectedProtocol Is Nothing Then
            Exit Sub
        End If


        'Inserisco ogni documento nella catena degli annessi del protocollo selezionato
        For Each doc As DocumentInfo In From doc1 In AllDocuments Where CheckDocument(doc1.Name).GetValueOrDefault(True)
            Facade.ProtocolFacade.AddAnnexes(selectedProtocol, New List(Of DocumentInfo) From {doc})
        Next

        For Each pec As PECMail In CurrentPecMailList
            pec.Year = selectedProtocol.Year
            pec.Number = selectedProtocol.Number
            pec.DocumentUnit = Facade.DocumentUnitFacade.GetById(selectedProtocol.Id)
            pec.RecordedInDocSuite = CType(1, Short)

            Facade.PECMailLogFacade.InsertLog(pec, String.Format("Pec collegata al protocollo {0}", selectedProtocol.Id.ToString()), PECMailLogType.Linked)
            'Log del protocollo
            Facade.ProtocolLogFacade.Insert(selectedProtocol, ProtocolLogEvent.PM, String.Format("Collegata PEC n.{0} del {1} con oggetto ""{2}""", pec.Id, pec.RegistrationDate.ToLocalTime().DateTime.ToShortDateString(), pec.MailSubject))

            Facade.PECMailFacade.Update(pec)
        Next

        Dim parameters As String = String.Format("Type=Prot&UniqueId={0}", selectedProtocol.Id)
        Response.Redirect(String.Concat("../Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(parameters)), True)
    End Sub

    Private Sub AttachToUDS()
        Try
            If CurrentUDSSelected Is Nothing Then
                AjaxAlert("Nessun archivio selezionato")
                Exit Sub
            End If

            Dim correlationId As Guid = Guid.Empty
            If (Not Guid.TryParse(HFcorrelatedCommandId.Value, correlationId)) Then
                AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, "Errore generale, contattare assistenza : CorrelationId is not Valid."))
                Return
            End If

            Dim selectedRepository As UDSRepository = CurrentUDSRepositoryFacade.GetById(CurrentUDSSelected.IdUDSRepository)
            Dim selectedUDS As UDSDto = CurrentUDSFacade.GetUDSSource(selectedRepository, String.Format(ODATA_EQUAL_UDSID, CurrentUDSSelected.UDSId))
            Dim model As UDSModel = selectedUDS.UDSModel
            'Inserisco ogni documento nella catena degli annessi del protocollo selezionato
            Dim docs As ICollection(Of DocumentInfo) = AllDocuments.Where(Function(x) CheckDocument(x.Name).GetValueOrDefault(True)).ToList()
            Dim documents As ICollection(Of DocumentInstance) = New List(Of DocumentInstance)
            Dim documentStored As BiblosDocumentInfo = Nothing
            For Each document As DocumentInfo In docs
                documentStored = document.ArchiveInBiblos(CommonShared.CurrentWorkflowLocation.ProtBiblosDSDB, Guid.Empty)
                documents.Add(New DocumentInstance() With {.IdDocumentToStore = documentStored.DocumentId.ToString(), .DocumentName = document.Name})
            Next

            Dim pecInstances As List(Of PECInstance) = New List(Of PECInstance)
            Dim pecMails As PECMails = model.Model.PECMails
            If pecMails IsNot Nothing Then
                pecInstances.AddRange(pecMails.Instances)
            End If
            pecInstances.AddRange(CurrentPecMailIdList.Select(Function(s) New PECInstance() With {.IdPECMail = s}))

            If pecMails Is Nothing Then
                pecMails = New PECMails() With {.Label = SectionManager.DefaultSectionName}
                model.Model.PECMails = pecMails
            End If
            pecMails.Instances = pecInstances.ToArray()

            model.Model.Documents.DocumentAnnexed.Instances = If(model.Model.Documents.DocumentAnnexed.Instances, Enumerable.Empty(Of DocumentInstance)).Concat(documents).ToArray()
            Dim sendedCommandId As Guid = CurrentUDSRepositoryFacade.SendCommandUpdateData(selectedRepository.Id, selectedUDS.Id, correlationId, model)

            FileLogger.Info(LoggerName, String.Format("Command sended with Id {0} and CorrelationId {0}", sendedCommandId, correlationId))
            btnAdd.Enabled = False
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Dim exceptionMessage As String = String.Format("Errore nella fase di salvataggio: {0}", ProtocolEnv.DefaultErrorMessage)
            AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(exceptionMessage)))
        End Try
    End Sub
#End Region

End Class