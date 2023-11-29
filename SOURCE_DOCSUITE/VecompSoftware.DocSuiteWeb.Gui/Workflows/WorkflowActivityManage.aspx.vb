Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Collaborations
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class WorkflowActivityManage
    Inherits CommonBasePage
    Implements IProtocolInitializer
    Implements IUDSInitializer
    Implements ICollaborationInitializer

#Region " Fields "
    Private _whiteList() As String
    Private _blackList() As String
    Private Const CONFIRM_CALLBACK As String = "workflowActivityManage.confirmCallback('{0}','{1}',{2},'{3}','{4}');"
    Private Const LOAD_CALLBACK As String = "workflowActivityManage.loadCallback('{0}');"
    Private Const INSERT_MISCELLANEA As String = "InsertMiscellanea"
    Private _allAttachmentDocuments As List(Of DocumentInfo)
    Private _fascMiscellaneaLocation As Location = Nothing
    Private _currentFascicleFolderFinder As FascicleFolderFinder
    Private _currentWorkflowPropertyFacade As Facade.WebAPI.Workflows.WorkflowPropertyFacade
#End Region

#Region " Properties "
    Protected ReadOnly Property CurrentUser As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property
    Protected ReadOnly Property CurrentWorkflowActivityId As Guid
        Get
            Return Guid.Parse(Request.QueryString("IdWorkflowActivity"))
        End Get
    End Property

    Public ReadOnly Property FascMiscellaneaLocation() As Location
        Get
            If _fascMiscellaneaLocation Is Nothing Then
                _fascMiscellaneaLocation = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
            End If
            Return _fascMiscellaneaLocation
        End Get
    End Property

    Public ReadOnly Property ArchiveName() As String
        Get
            Return FascMiscellaneaLocation.ProtBiblosDSDB
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleFolderFinder() As FascicleFolderFinder
        Get
            If _currentFascicleFolderFinder Is Nothing Then
                _currentFascicleFolderFinder = New FascicleFolderFinder(DocSuiteContext.Current.CurrentTenant)
                _currentFascicleFolderFinder.EnablePaging = False
            End If
            Return _currentFascicleFolderFinder
        End Get
    End Property

    Public ReadOnly Property AllAttachmentDocuments As List(Of DocumentInfo)
        Get
            _allAttachmentDocuments = New List(Of DocumentInfo)
            Dim radio As RadioButton
            Dim documentInfo As DocumentInfo
            For Each selectedItem As GridDataItem In grdUD.MasterTableView.GetSelectedItems()
                radio = CType(selectedItem.FindControl("rbtMainDocument"), RadioButton)
                If radio Is Nothing OrElse Not radio.Checked Then
                    documentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(selectedItem.GetDataKeyValue("MainDocumentName").ToString()))
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    _allAttachmentDocuments.Add(documentInfo)
                End If
            Next
            Return _allAttachmentDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedDocument As DocumentInfo
        Get
            Dim radio As RadioButton
            Dim documentInfo As DocumentInfo
            For Each item As GridDataItem In grdUD.Items
                radio = CType(item.FindControl("rbtMainDocument"), RadioButton)
                If radio IsNot Nothing AndAlso radio.Checked Then
                    documentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(item.GetDataKeyValue("MainDocumentName").ToString()))
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    Return documentInfo
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property WhiteList As String()
        Get
            If _whiteList Is Nothing Then
                _whiteList = ProtocolEnv.FileExtensionWhiteList.Split("|"c)
            End If
            Return _whiteList
        End Get
    End Property

    Public ReadOnly Property BlackList As String()
        Get
            If _blackList Is Nothing Then
                _blackList = ProtocolEnv.FileExtensionBlackList.Split("|"c)
            End If
            Return _blackList
        End Get
    End Property
    Public ReadOnly Property FolderSelectionEnabled As Boolean
        Get
            Return GetKeyValueOrDefault("FolderSelectionEnabled", True)
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowPropertyFacade As Facade.WebAPI.Workflows.WorkflowPropertyFacade
        Get
            If _currentWorkflowPropertyFacade Is Nothing Then
                _currentWorkflowPropertyFacade = New WebAPI.Workflows.WorkflowPropertyFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
            End If
            Return _currentWorkflowPropertyFacade
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            uscFascicleSearch.FolderSelectionEnabled = FolderSelectionEnabled
            grdUD.DataSource = New List(Of String)
            InitializeButton(True)
        End If
    End Sub

    Protected Sub GrdUD_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdUD.ItemDataBound
        If TypeOf e.Item Is GridGroupHeaderItem Then
            Dim GroupHeader As GridGroupHeaderItem = DirectCast(e.Item, GridGroupHeaderItem)
            GroupHeader.DataCell.Text = GroupHeader.DataCell.Text.Replace(":", "").Trim()
        End If

        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As DocumentUnitModel = DirectCast(e.Item.DataItem, DocumentUnitModel)
        Dim lblFileName As Label = DirectCast(e.Item.FindControl("lblFileName"), Label)
        lblFileName.Text = boundHeader.Title

        Dim imageFile As Image = DirectCast(e.Item.FindControl("ImageFile"), Image)
        imageFile.ImageUrl = ImagePath.FromFile(boundHeader.Title)

        Dim lblUDRegistrationDate As Label = DirectCast(e.Item.FindControl("lblUDRegistrationDate"), Label)
        If (boundHeader.RegistrationDate.HasValue) Then
            lblUDRegistrationDate.Text = boundHeader.RegistrationDate.Value.Date.ToShortDateString()
        Else
            lblUDRegistrationDate.Text = String.Empty
        End If

        Dim lblUDObject As Label = DirectCast(e.Item.FindControl("lblUDObject"), Label)
        lblUDObject.Text = boundHeader.Subject
    End Sub

    Protected Sub RblDocumentUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDocumentUnit.SelectedIndexChanged
        InitializeButton()
    End Sub

    Protected Sub DdlUDSArchives_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles ddlUDSArchives.SelectedIndexChanged
        InitializeButton()
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf WorkFlowManageAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdUD)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSArchives, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(pnlFascicleSelect, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Protected Sub WorkFlowManageAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        Select Case ajaxModel.ActionName
            Case "LoadWorkFlowDocument"
                Try
                    Dim chainId As Guid = JsonConvert.DeserializeObject(Of Guid)(ajaxModel.Value(0))
                    LoadUD(LoadWorkFlowDocument(chainId))
                    AjaxManager.ResponseScripts.Add(String.Format(LOAD_CALLBACK, String.Empty))
                Catch ex As Exception
                    FileLogger.Error(LoggerName, "Errore nel caricamento dei documenti dell'attività", ex)
                    AjaxManager.ResponseScripts.Add(String.Format(LOAD_CALLBACK, "Errore nel caricamento dei documenti dell'attività"))
                End Try

            Case INSERT_MISCELLANEA
                Dim idFascicle As Guid = Guid.Parse(ajaxModel.Value(0))
                Dim idFascicleFolder As Guid = Guid.Parse(ajaxModel.Value(1))
                ManageDocuments(idFascicle, idFascicleFolder)
        End Select
    End Sub



    Private Sub LoadUD(models As IList(Of DocumentUnitModel))
        grdUD.DataSource = models
        grdUD.DataBind()
    End Sub

    Public Function GetProtocolInitializer() As ProtocolInitializer Implements IProtocolInitializer.GetProtocolInitializer
        Dim tor As New ProtocolInitializer()
        Dim attachments As List(Of DocumentInfo) = AllAttachmentDocuments
        ' Solamente un elemento può essere MAIN
        tor.MainDocument = SelectedDocument
        tor.Attachments = New List(Of DocumentInfo)()
        For Each attachment As DocumentInfo In attachments
            If CheckDocument(attachment.Name).GetValueOrDefault(True) Then
                tor.Attachments.Add(attachment)
            End If
        Next
        Return tor
    End Function

    Public Function GetCollaborationInitializer() As CollaborationInitializer Implements ICollaborationInitializer.GetCollaborationInitializer
        Dim initializer As CollaborationInitializer = New CollaborationInitializer With {
            .MainDocument = SelectedDocument,
            .Attachments = AllAttachmentDocuments
        }
        Return initializer
    End Function

    Public Function GetUDSInitializer() As UDSDto Implements IUDSInitializer.GetUDSInitializer
        Dim udsSelected As UDSRepository = CurrentUDSRepositoryFacade.GetById(Guid.Parse(ddlUDSArchives.SelectedValue))
        Dim dto As UDSDto = New UDSDto()
        dto.Id = udsSelected.Id
        dto.UDSRepository = New UDSEntityRepositoryDto() With {.UniqueId = udsSelected.Id, .Name = udsSelected.Name}
        Dim model As UnitaDocumentariaSpecifica = UDSModel.LoadXml(udsSelected.ModuleXML).Model
        Dim allSelectedDocument As List(Of DocumentInfo) = AllAttachmentDocuments
        If model.Documents Is Nothing OrElse model.Documents.Document Is Nothing OrElse model.Documents.Document.Instances Is Nothing Then
            If model.Documents IsNot Nothing Then
                If model.Documents.Document IsNot Nothing Then
                    Dim mainDocument As DocumentInfo = SelectedDocument
                    If mainDocument IsNot Nothing Then
                        Dim documents As IList(Of DocumentInstance) = New List(Of DocumentInstance)
                        model.Documents.Document.Instances = GetDocumentInstances(New List(Of DocumentInfo) From {mainDocument})
                    End If
                End If
                If model.Documents.DocumentAttachment IsNot Nothing Then
                    Dim attachments As IList(Of DocumentInfo) = allSelectedDocument
                    If attachments IsNot Nothing Then
                        Dim documents As IList(Of DocumentInstance) = New List(Of DocumentInstance)
                        model.Documents.DocumentAttachment.Instances = GetDocumentInstances(attachments.Where(Function(x) CheckDocument(x.Name).GetValueOrDefault(True)).ToList())
                    End If
                End If
            End If
        End If
        dto.UDSModel = New UDSModel(model)
        Return dto
    End Function

    Private Function GetDocumentInstances(documents As IList(Of DocumentInfo)) As DocumentInstance()
        Dim documentInstances As IList(Of DocumentInstance) = New List(Of DocumentInstance)
        If documents.Any() Then
            Dim documentStored As BiblosDocumentInfo = Nothing
            For Each document As DocumentInfo In documents
                If TypeOf document Is BiblosPdfDocumentInfo Then
                    documentStored = document.ArchiveInBiblos(CommonShared.CurrentWorkflowLocation.ProtBiblosDSDB, Guid.Empty)
                    documentInstances.Add(New DocumentInstance() With {.IdDocumentToStore = documentStored.DocumentId.ToString(), .DocumentName = document.Name})
                Else
                    documentInstances.Add(New DocumentInstance() With {.StoredChainId = DirectCast(document, BiblosDocumentInfo).DocumentId.ToString()})
                End If
            Next
        End If
        Return documentInstances.ToArray()
    End Function

    Private Function LoadWorkFlowDocument(idArchiveChains As Guid) As List(Of DocumentUnitModel)
        Dim documentModels As List(Of DocumentUnitModel) = New List(Of DocumentUnitModel)
        Dim documents As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(idArchiveChains)
        Dim mappedDoc As DocumentUnitModel
        Dim noteAttribute As KeyValuePair(Of String, String)
        Dim items As NameValueCollection
        For Each doc As BiblosDocumentInfo In documents
            mappedDoc = New DocumentUnitModel()
            mappedDoc.DocumentUnitName = "Documenti"
            If doc.DateCreated.HasValue Then
                mappedDoc.RegistrationDate = doc.DateCreated.Value
            End If
            mappedDoc.UniqueId = doc.DocumentId
            mappedDoc.Title = doc.Name
            mappedDoc.Subject = String.Empty
            noteAttribute = doc.Attributes.SingleOrDefault(Function(f) f.Key.Equals(BiblosFacade.NOTE_ATTRIBUTE))
            If Not String.IsNullOrEmpty(noteAttribute.Value) Then
                mappedDoc.Subject = noteAttribute.Value
            End If
            mappedDoc.Number = ImagePath.FromFile(doc.Name)
            items = doc.ToQueryString()
            mappedDoc.MainDocumentName = items.AsEncodedQueryString()
            mappedDoc.Environment = DSWEnvironment.Document
            documentModels.Add(mappedDoc)
        Next
        Return documentModels
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

    Private Sub ManageDocuments(idFascicle As Guid, idFascicleFolder As Guid)
        If idFascicle.Equals(Guid.Empty) Then
            Throw New ArgumentNullException("Nessun fascicolo definito per l'inserimento")
        End If

        Dim allSelectedDocument As List(Of DocumentInfo) = AllAttachmentDocuments
        If allSelectedDocument.Count = 0 Then
            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, Guid.Empty, idFascicle, False.ToString().ToLower(), "Nessun documento selezionato per la fascicolazione"))
            Exit Sub
        End If

        Dim currentFascicleFolder As FascicleFolder = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFascicleFolderFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.ExpandProperties = True
                                finder.IdFascicle = idFascicle
                                finder.ReadDefaultFolder = False
                                finder.UniqueId = idFascicleFolder
                                Return finder.DoSearch().Select(Function(f) f.Entity).SingleOrDefault()
                            End Function)

        Dim chainId As Guid = Guid.Empty
        If currentFascicleFolder.FascicleDocuments.Count > 0 Then
            chainId = currentFascicleFolder.FascicleDocuments.Where(Function(x) x.ChainType = Entity.DocumentUnits.ChainType.Miscellanea).Select(Function(s) s.IdArchiveChain).SingleOrDefault()
        End If
        Dim isNewIdArchiveChain As Boolean = chainId.Equals(Guid.Empty)

        For Each addedDocument As DocumentInfo In allSelectedDocument
            addedDocument.AddAttribute(BiblosFacade.REGISTRATION_USER_ATTRIBUTE, DocSuiteContext.Current.User.FullUserName)
            Dim biblosFunc As Func(Of DocumentInfo, BiblosDocumentInfo) = Function(d As DocumentInfo) SaveBiblosDocument(d, chainId)
            Dim savedtemplate As BiblosDocumentInfo = biblosFunc(addedDocument)
            chainId = savedtemplate.ChainId
        Next
        AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, chainId, idFascicle, isNewIdArchiveChain.ToString().ToLower(), idFascicleFolder, String.Empty))
    End Sub

    Private Function SaveBiblosDocument(document As DocumentInfo, chainId As Guid) As BiblosDocumentInfo
        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(ArchiveName, chainId)
        Return storedBiblosDocumentInfo
    End Function

    Private Sub InitializeButton(Optional firstLoad As Boolean = False)
        Dim url As String = Nothing
        Dim causesValidation As Boolean = False
        Dim onClickAction As String = "showLoadingPanel"
        Dim environment As DSWEnvironmentType = DSWEnvironmentType.Any
        Dim entityId As Integer = 0

        If firstLoad Then
            rblDocumentUnit.ClearSelection()
            rblDocumentUnit.SelectedValue = "Collaborazione"
        End If

        Dim workflowProperty As WorkflowProperty = CurrentWorkflowPropertyFacade.FindPropertyByActivityIdAndName(CurrentWorkflowActivityId, WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL)
        If workflowProperty IsNot Nothing AndAlso String.IsNullOrEmpty(workflowProperty.ValueString) = False Then
            Dim workflowReferenceModel As WorkflowReferenceModel = JsonConvert.DeserializeObject(Of WorkflowReferenceModel)(workflowProperty.ValueString)
            If workflowReferenceModel IsNot Nothing Then
                environment = workflowReferenceModel.ReferenceType
            End If
            If environment = DSWEnvironmentType.PECMail Then
                rblDocumentUnit.Items.Add(New ListItem("Protocolla PEC", "PEC"))
                Dim pecMail As Entity.PECMails.PECMail = JsonConvert.DeserializeObject(Of Entity.PECMails.PECMail)(workflowReferenceModel.ReferenceModel)
                entityId = pecMail.EntityId
                If pecMail.ProcessStatus = Entity.PECMails.PECMailProcessStatus.StoredInDocumentManager AndAlso Not String.IsNullOrEmpty(pecMail.MailContent) Then
                    Dim results As String = New FacadeElsaWebAPI(ProtocolEnv.DocSuiteNextElsaBaseURL).StartPreparePECMailDocumentsWorkflow(pecMail.UniqueId, JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(pecMail.MailContent))
                End If

                If firstLoad Then
                    rblDocumentUnit.ClearSelection()
                    rblDocumentUnit.SelectedValue = "PEC"
                End If
            End If
        End If

        Select Case rblDocumentUnit.SelectedValue
            Case "Protocollo"
                url = String.Format("~/Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck($"Type=Prot&Action=Insert&IdWorkflowActivity={CurrentWorkflowActivityId}"))

            Case "Archivi"
                onClickAction = "onArchiveClick"
                causesValidation = True
                url = String.Format("~/UDS/UDSInsert.aspx?{0}", CommonShared.AppendSecurityCheck($"Type=UDS&Action=Insert&IdWorkflowActivity={CurrentWorkflowActivityId}&ArchiveTypeId={ddlUDSArchives.SelectedValue}"))

            Case "Collaborazione"
                url = String.Format("~/User/UserCollGestione.aspx?{0}", CommonShared.AppendSecurityCheck($"Titolo=Inserimento&Action=Add&Title2=Alla Visione/Firma&Action2=CI&FromWorkFlow=True&IdWorkflowActivity={CurrentWorkflowActivityId}"))

            Case "Fascicolo"
                onClickAction = "onFascicleMiscellaneaClick"

            Case "PEC"
                url = String.Format("~/Pec/PECToDocumentUnit.aspx?{0}", CommonShared.AppendSecurityCheck($"isInWindow=true&Type=Pec&PECId={entityId}"))
        End Select

        btnConfirm.OnClientClicking = onClickAction
        btnConfirm.CausesValidation = causesValidation
        btnConfirm.AutoPostBack = Not String.IsNullOrEmpty(url)
        btnConfirm.PostBackUrl = url
    End Sub

    Private Function GetIdFascicle() As Guid? Implements IUDSInitializer.GetIdFascicle
        Return Nothing
    End Function
#End Region

End Class