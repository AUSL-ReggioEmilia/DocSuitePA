Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class ProtocolToUDS
    Inherits UDSBasePage
    Implements IUDSInitializer

#Region "Fields"
    Private _currentProtocol As Protocol
    Private Const OPEN_WINDOW_SCRIPT As String = "return openWindow('{0}', '{1}', '{2}');"
    Private Const PROTOCOL_ANNEXED_CODE As String = "ANX"
    Private Const PROTOCOL_ATTACHMENT_CODE As String = "ATT"
    Private _protocolDocumentSource As List(Of ProtocolDocumentDTO)
#End Region

#Region "Properties"
    Public ReadOnly Property ProtocolUniqueId As Guid?
        Get
            Return GetKeyValue(Of Guid?)("ProtocolUniqueId")
        End Get
    End Property

    Public ReadOnly Property CurrentProtocol As Protocol
        Get
            If _currentProtocol Is Nothing AndAlso ProtocolUniqueId.HasValue Then
                _currentProtocol = Facade.ProtocolFacade.GetByUniqueId(ProtocolUniqueId.Value)
            End If
            Return _currentProtocol
        End Get
    End Property

    Public ReadOnly Property SelectedUDSId As Guid?
        Get
            If Not String.IsNullOrEmpty(ddlUDSs.SelectedValue) Then
                Return Guid.Parse(ddlUDSs.SelectedValue)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property ProtocolDocumentSource As IList(Of ProtocolDocumentDTO)
        Get
            If _protocolDocumentSource Is Nothing Then
                _protocolDocumentSource = Facade.ProtocolFacade.GetProtocolDocument(CurrentProtocol)
            End If
            Return _protocolDocumentSource
        End Get
    End Property

    Public ReadOnly Property SelectedMainDocument As DocumentInfo
        Get
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim chkbox As CheckBox = CType(item.FindControl("chkSelectMainDocument"), CheckBox)
                If chkbox IsNot Nothing AndAlso chkbox.Checked Then
                    Dim key As String = item.GetDataKeyValue("Serialized").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    Return documentInfo
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property SelectedAttachments As IList(Of DocumentInfo)
        Get
            Return GetSelectedDocuments(PROTOCOL_ATTACHMENT_CODE)
        End Get
    End Property

    Public ReadOnly Property SelectedAnnexed As IList(Of DocumentInfo)
        Get
            Return GetSelectedDocuments(PROTOCOL_ANNEXED_CODE)
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub DgvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        e.Item.SelectableMode = GridItemSelectableMode.ServerAndClientSide
        Dim dto As ProtocolDocumentDTO = DirectCast(e.Item.DataItem, ProtocolDocumentDTO)
        With DirectCast(e.Item.FindControl("imgDocumentExtensionType"), ImageButton)
            .ImageUrl = ImagePath.FromFile(dto.DocumentName, True)

            Dim url As String = ResolveUrl(String.Concat("~/Viewers/DocumentInfoViewer.aspx?", CommonShared.AppendSecurityCheck(dto.Serialized)))
            .OnClientClick = String.Format(OPEN_WINDOW_SCRIPT, url, windowPreviewDocument.ClientID, "")
        End With

        With DirectCast(e.Item.FindControl("lblDocumentName"), Label)
            .Text = dto.DocumentName
        End With

        With DirectCast(e.Item.FindControl("chkSelectMainDocument"), CheckBox)
            .Attributes.Add("onclick", String.Format("javascript:mutuallyExclusive(this, {0})", e.Item.ItemIndex))
            .InputAttributes.Add("class", "selectmaindocument")
        End With

        Dim selectable As Boolean = False
        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        Dim chkMainDocument As CheckBox = DirectCast(e.Item.FindControl("chkSelectMainDocument"), CheckBox)
        documentTypeModControl.Items.Clear()
        documentTypeModControl.Visible = False
        chkMainDocument.Enabled = False
        chkMainDocument.ToolTip = "L'archivio selezionato non contiene un controllo per il documento principale"

        If (SelectedUDSId.HasValue) Then
            Dim selectedUDS As UDSRepository = CurrentUDSRepositoryFacade.GetById(SelectedUDSId.Value)
            If selectedUDS IsNot Nothing AndAlso Not String.IsNullOrEmpty(selectedUDS.ModuleXML) Then
                Dim model As UDSModel = UDSModel.LoadXml(selectedUDS.ModuleXML)
                If model.Model.Documents IsNot Nothing Then
                    If model.Model.Documents.Document IsNot Nothing Then
                        chkMainDocument.Enabled = True
                        chkMainDocument.ToolTip = String.Empty
                        chkMainDocument.Checked = dto.DocumentType.Eq("Documento")
                        If chkMainDocument.Checked Then
                            With DirectCast(e.Item, GridDataItem)("typeDoc")
                                DirectCast(.Controls(1), RadDropDownList).SetDisplay(False)
                            End With
                        End If
                        selectable = True
                    End If

                    If model.Model.Documents.DocumentAttachment IsNot Nothing Then
                        selectable = True
                        documentTypeModControl.Visible = True
                        documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAttachment.Label, PROTOCOL_ATTACHMENT_CODE))
                    End If

                    If model.Model.Documents.DocumentAnnexed IsNot Nothing Then
                        selectable = True
                        documentTypeModControl.Visible = True
                        documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAnnexed.Label, PROTOCOL_ANNEXED_CODE))
                    End If
                End If
            End If
        End If

        e.Item.Selected = selectable
        If Not selectable Then
            e.Item.ToolTip = "L'archvio selezionato non contiene controlli per la gestione dei documenti"
            e.Item.SelectableMode = GridItemSelectableMode.None
        End If
    End Sub

    Protected Sub DdlSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlUDSs.SelectedIndexChanged
        BindProtocolDocumentsGrid()
    End Sub

    Protected Sub DgvDocuments_NeedDataSource(sender As Object, e As EventArgs) Handles dgvDocuments.NeedDataSource
        BindProtocolDocumentsGrid(needBinding:=False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSs, ddlUDSs, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSs, pnlProtocolDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Public Sub Initialize()
        If CurrentProtocol Is Nothing Then
            Throw New DocSuiteException(String.Format("Protocollo {0}", ProtocolUniqueId), "Nessun protocollo trovato per l'ID passato")
        End If

        windowPreviewDocument.Height = Unit.Pixel(ProtocolEnv.DocumentPreviewHeight)
        windowPreviewDocument.Width = Unit.Pixel(ProtocolEnv.DocumentPreviewWidth)

        btnConfirm.PostBackUrl = String.Concat("../UDS/UDSInsert.aspx?Type=UDS&Action=Insert&IdProtocol=", ProtocolUniqueId)

        'Inizializzo dropdownlist archivi
        BindProtocollableArchives()

        'Inizializzo usercontrol preview Protocollo
        uscProtocolPreview.CurrentProtocol = CurrentProtocol
        uscProtocolPreview.Initialize()
    End Sub

    Private Sub BindProtocollableArchives()
        Dim archives As IList(Of UDSRepository) = CurrentUDSRepositoryFacade.GetProtocollable()
        If archives.IsNullOrEmpty() Then
            FileLogger.Warn(LoggerName, "Nessun archvio con protocollazione abilitata trovato")
            Exit Sub
        End If

        Dim authorizedArchives As IList(Of UDSRepository) = New List(Of UDSRepository)()
        Dim currentRepositoryRights As UDSRepositoryRightsUtil
        Dim dto As UDSDto
        For Each archive As UDSRepository In archives
            dto = New UDSDto() With {.UDSModel = UDSModel.LoadXml(archive.ModuleXML)}
            currentRepositoryRights = New UDSRepositoryRightsUtil(archive, DocSuiteContext.Current.User.FullUserName, dto)
            If currentRepositoryRights.IsInsertable Then
                authorizedArchives.Add(archive)
            End If
        Next

        ddlUDSs.DataTextField = "Name"
        ddlUDSs.DataValueField = "Id"
        ddlUDSs.DataSource = authorizedArchives
        ddlUDSs.DataBind()

        If (authorizedArchives.Count = 1) Then
            ddlUDSs.SelectedIndex = 0
            DdlSeries_SelectedIndexChanged(Me, New EventArgs())
        Else
            ddlUDSs.Items.Insert(0, New DropDownListItem(String.Empty, String.Empty))
        End If
    End Sub

    Private Sub BindProtocolDocumentsGrid(Optional needBinding As Boolean = True)
        dgvDocuments.DataSource = ProtocolDocumentSource
        If needBinding Then
            dgvDocuments.DataBind()
        End If
    End Sub

    Public Function GetUDSInitializer() As UDSDto Implements IUDSInitializer.GetUDSInitializer
        If (Not SelectedUDSId.HasValue) Then
            Throw New DocSuiteException("Impossibile proseguire con la gestione dell'archivio per un problema applicativo interno. Contattare l'assistenza.")
        End If

        Dim dto As UDSDto = New UDSDto With {
            .Id = SelectedUDSId.Value
        }

        Dim selectedUDS As UDSRepository = CurrentUDSRepositoryFacade.GetById(dto.Id)
        dto.UDSRepository = New UDSEntityRepositoryDto() With {.UniqueId = selectedUDS.Id, .Name = selectedUDS.Name}
        Dim udsModel As UDSModel = UDSModel.LoadXml(selectedUDS.ModuleXML)
        Dim model As UnitaDocumentariaSpecifica = udsModel.Model
        model.Subject.Value = CurrentProtocol.ProtocolObject
        model.Category = New UDS.Category() With {.IdCategory = CurrentProtocol.Category.Id.ToString()}

        'Se il documento principale non è stato valorizzato significa che non si tratta di caricamento automatico da organigramma
        If model.Documents Is Nothing OrElse model.Documents.Document Is Nothing OrElse model.Documents.Document.Instances Is Nothing Then
            If model.Documents IsNot Nothing Then
                If model.Documents.Document IsNot Nothing Then
                    Dim mainDocument As DocumentInfo = SelectedMainDocument
                    If mainDocument IsNot Nothing Then
                        model.Documents.Document.Instances = UDSFacade.GetDocumentInstances(New List(Of DocumentInfo) From {mainDocument})
                    End If
                End If

                If model.Documents.DocumentAttachment IsNot Nothing Then
                    Dim attachments As IList(Of DocumentInfo) = SelectedAttachments
                    If attachments IsNot Nothing Then
                        model.Documents.DocumentAttachment.Instances = UDSFacade.GetDocumentInstances(attachments)
                    End If
                End If

                If model.Documents.DocumentAnnexed IsNot Nothing Then
                    Dim annexed As IList(Of DocumentInfo) = SelectedAnnexed
                    If annexed IsNot Nothing Then
                        model.Documents.DocumentAnnexed.Instances = UDSFacade.GetDocumentInstances(annexed)
                    End If
                End If
            End If
        End If

        If model.Contacts IsNot Nothing Then
            Dim senders As IList(Of ContactDTO) = Facade.ProtocolFacade.GetContacts(CurrentProtocol, ProtocolContactCommunicationType.Sender)
            Dim recipients As IList(Of ContactDTO) = Facade.ProtocolFacade.GetContacts(CurrentProtocol, ProtocolContactCommunicationType.Recipient)
            For Each contactField As Contacts In model.Contacts
                If Not contactField.ContactType.Equals(UDS.ContactType.None) Then
                    Select Case model.ProtocolDirection
                        Case ProtocolDirectionType.In
                            ConctactDiscriminator(contactField, senders, recipients, UDS.ContactType.Sender)
                        Case ProtocolDirectionType.Out
                            ConctactDiscriminator(contactField, senders, recipients, UDS.ContactType.Recipient)
                        Case ProtocolDirectionType.None
                            ConctactDiscriminator(contactField, senders, recipients, UDS.ContactType.Sender)
                    End Select
                End If
            Next
        End If
        dto.UDSModel = New UDSModel(model)
        Return dto
    End Function


    Private Sub ConctactDiscriminator(contactField As Contacts, senders As IList(Of ContactDTO), recipients As IList(Of ContactDTO), contactType As UDS.ContactType)
        Select Case CurrentProtocol.Type.Id
            Case 1
                contactField.ContactInstances = UDSFacade.GetContactInstances(If(contactField.ContactType.Equals(contactType), recipients, senders))
                contactField.ContactManualInstances = UDSFacade.GetManualContactInstances(If(contactField.ContactType.Equals(contactType), recipients, senders))
            Case -1
                contactField.ContactInstances = UDSFacade.GetContactInstances(If(contactField.ContactType.Equals(contactType), senders, recipients))
                contactField.ContactManualInstances = UDSFacade.GetManualContactInstances(If(contactField.ContactType.Equals(contactType), senders, recipients))
            Case Else
                contactField.ContactInstances = UDSFacade.GetContactInstances(If(contactField.ContactType.Equals(contactType), senders, recipients))
                contactField.ContactManualInstances = UDSFacade.GetManualContactInstances(If(contactField.ContactType.Equals(contactType), senders, recipients))
        End Select
    End Sub

    Private Function GetSelectedDocuments(documentType As String) As IList(Of DocumentInfo)
        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
            Dim ddlDocumentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
            Dim chkbox As CheckBox = CType(item.FindControl("chkSelectMainDocument"), CheckBox)
            If ddlDocumentType IsNot Nothing AndAlso chkbox IsNot Nothing AndAlso ddlDocumentType.SelectedValue.Eq(documentType) AndAlso Not chkbox.Checked Then
                Dim key As String = item.GetDataKeyValue("Serialized").ToString()
                Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                documents.Add(documentInfo)
            End If
        Next
        Return documents
    End Function
#End Region

End Class