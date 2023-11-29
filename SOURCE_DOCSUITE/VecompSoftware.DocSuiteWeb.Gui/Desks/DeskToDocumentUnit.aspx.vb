Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Collaborations
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class DeskToDocumentUnit
    Inherits DeskBasePage
    Implements IProtocolInitializer
    Implements ICollaborationInitializer
    Implements IResolutionInitializer

#Region " Fields "
    Private Const PAGE_TITLE As String = "Tavoli - Gestione"
    Private Const MAINDOCUMENT_CODE As String = "MAIN"
    Private Const MAINDOCUMENTOMISSIS_CODE As String = "MAINOMISSIS"
    Private Const ATTACHMENT_CODE As String = "ALL"
    Private Const ATTACHMENTOMISSIS_CODE As String = "ALLOMISSIS"
    Private Const ANNEXED_CODE As String = "ANX"
    Private Const INITIALIZE_CALLBACK As String = "deskToDocumentUnit.initializeDataCallback();"
    Private Const SET_POSTBACKURL_CALLBACK As String = "deskToDocumentUnit.setPostbackUrlCallback();"
    Private Const OPEN_WINDOW_SCRIPT As String = "deskToDocumentUnit.openPreviewDocumentWindow('{0}');"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"

    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder
#End Region

#Region " Properties "
    Public ReadOnly Property SelectedMainDocument As DocumentInfo
        Get
            Return GetSelectedDocumentsByDocumentTypeCode(MAINDOCUMENT_CODE).SingleOrDefault()
        End Get
    End Property

    Public ReadOnly Property SelectedMainDocumentOmissis As DocumentInfo
        Get
            Return GetSelectedDocumentsByDocumentTypeCode(MAINDOCUMENTOMISSIS_CODE).SingleOrDefault()
        End Get
    End Property

    Public ReadOnly Property SelectedAttachments As ICollection(Of DocumentInfo)
        Get
            Return GetSelectedDocumentsByDocumentTypeCode(ATTACHMENT_CODE)
        End Get
    End Property

    Public ReadOnly Property SelectedAttachmentsOmissis As ICollection(Of DocumentInfo)
        Get
            Return GetSelectedDocumentsByDocumentTypeCode(ATTACHMENTOMISSIS_CODE)
        End Get
    End Property

    Public ReadOnly Property SelectedAnnexed As ICollection(Of DocumentInfo)
        Get
            Return GetSelectedDocumentsByDocumentTypeCode(ANNEXED_CODE)
        End Get
    End Property

    Public ReadOnly Property ResolutionMainDocumentOmissisEnabled As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso ResolutionEnv.MainDocumentOmissisEnable
        End Get
    End Property

    Public ReadOnly Property ResolutionAttachmentsOmissisEnabled As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso ResolutionEnv.AttachmentOmissisEnable
        End Get
    End Property

    Public ReadOnly Property CurrentCollaborationDomainType As String
        Get
            If Not CurrentCollaborationType.HasValue Then
                Return String.Empty
            End If

            Select Case CurrentCollaborationType
                Case CollaborationDocumentType.P,
                     CollaborationDocumentType.U
                    Return "Prot"
                Case CollaborationDocumentType.A
                    Return "Resl"
                Case CollaborationDocumentType.D
                    Return "Resl"
                Case CollaborationDocumentType.S
                    Return "Series"
                Case CollaborationDocumentType.UDS
                    Return "UDS"
            End Select

            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property CurrentCollaborationType As CollaborationDocumentType?
        Get
            Dim collType As CollaborationDocumentType
            If [Enum].TryParse(ddlCollaborationType.SelectedValue, collType) Then
                Return collType
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property CurrentTemplateCollaborationFinder As TemplateCollaborationFinder
        Get
            If _currentTemplateCollaborationFinder Is Nothing Then
                _currentTemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                _currentTemplateCollaborationFinder.ResetDecoration()
                _currentTemplateCollaborationFinder.EnablePaging = False
            End If
            Return _currentTemplateCollaborationFinder
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub DgvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim dto As DeskDocumentResult = DirectCast(e.Item.DataItem, DeskDocumentResult)
        With DirectCast(e.Item.FindControl("imgDocumentExtensionType"), ImageButton)
            If dto.IsSigned AndAlso FileHelper.MatchExtension(dto.Name, FileHelper.PDF) Then
                .ImageUrl = ImagePath.FormatImageName("file_extension_pdf_signed", True)
            Else
                .ImageUrl = ImagePath.FromFile(dto.Name, True)
            End If

            Dim url As String = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(dto.BiblosSerializeKey))
            .OnClientClick = String.Format(OPEN_WINDOW_SCRIPT, url)
        End With

        With DirectCast(e.Item.FindControl("lblDocumentName"), Label)
            .Text = dto.Name
        End With

        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        documentTypeModControl.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        documentTypeModControl.Items.Add(New DropDownListItem("Documento principale", MAINDOCUMENT_CODE))
        documentTypeModControl.Items.Add(New DropDownListItem("Allegato", ATTACHMENT_CODE))
        documentTypeModControl.Items.Add(New DropDownListItem("Annesso", ANNEXED_CODE))

        e.Item.Selected = True
    End Sub

    Private Sub DeskToDocumentUnit_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Select Case ajaxModel.ActionName
            Case "initialize"
                LoadDocuments()
                LoadCollaborationTemplates()
                AjaxManager.ResponseScripts.Add(INITIALIZE_CALLBACK)
        End Select
    End Sub

    Private Sub RblDocumentUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDocumentUnit.SelectedIndexChanged
        btnConfirmPostback.PostBackUrl = GetDocumentUnitPostBackUrl()
        pnlCollaborationType.SetDisplay(rblDocumentUnit.SelectedValue = "0")
        AjaxManager.ResponseScripts.Add(SET_POSTBACKURL_CALLBACK)
    End Sub

    Private Sub DdlCollaborationType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlCollaborationType.SelectedIndexChanged
        btnConfirmPostback.PostBackUrl = GetCollaborationPostBackUrl()
        AjaxManager.ResponseScripts.Add(SET_POSTBACKURL_CALLBACK)
    End Sub

    Private Sub RblSendTo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSendTo.SelectedIndexChanged
        btnConfirmPostback.PostBackUrl = GetCollaborationPostBackUrl()
        AjaxManager.ResponseScripts.Add(SET_POSTBACKURL_CALLBACK)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf DeskToDocumentUnit_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgvDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ddlCollaborationType)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, pnlCollaborationType)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, btnConfirmPostback)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlCollaborationType, btnConfirmPostback)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlCollaborationType, ddlCollaborationType, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblSendTo, btnConfirmPostback)
    End Sub

    Private Sub Initialize()
        If CurrentDesk Is Nothing Then
            Throw New DocSuiteException(String.Format("Tavolo con ID {0} Inesistente", CurrentDeskId))
        End If

        If Not CurrentDeskRigths.IsProtocollable Then
            Throw New DocSuiteException(String.Format("Tavolo ID {0}", CurrentDeskId), "Non è possibile gestire il Tavolo selezionato. Verificare se si dispone di sufficienti autorizzazioni.")
        End If

        Title = PAGE_TITLE
        dgvDocuments.DataSource = Enumerable.Empty(Of String)
        rblDocumentUnit.Items(0).Selected = True
        pnlCollaborationType.SetDisplay(True)
        btnCancel.NavigateUrl = String.Format(DESK_SUMMARY_PATH, CurrentDeskId)
        btnConfirmPostback.PostBackUrl = GetDocumentUnitPostBackUrl()

        BindDeskInfos()
    End Sub

    Private Sub BindDeskInfos()
        lblDeskSubject.Text = CurrentDesk.Description
        lblDeskName.Text = CurrentDesk.Name
        If CurrentDesk.ExpirationDate.HasValue Then
            lblExpireDate.Text = CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy")
        End If

        If (DocSuiteContext.Current.IsResolutionEnabled) Then
            Dim resolutionListItem As ListItem = New ListItem With {
                .Text = Facade.TabMasterFacade.TreeViewCaption,
                .Value = DirectCast(DSWEnvironment.Resolution, Integer).ToString()
            }
            rblDocumentUnit.Items.Add(resolutionListItem)
        End If
    End Sub

    Private Sub LoadDocuments()
        Dim documentDtos As ICollection(Of DeskDocumentResult) = New Collection(Of DeskDocumentResult)
        For Each deskDocument As Data.Entity.Desks.DeskDocument In CurrentDesk.DeskDocuments.Where(Function(x) x.IsActive)
            Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(deskDocument.IdDocument.Value)
            If docInfos.Count = 0 Then
                Exit Sub
            End If
            Dim docInfo As BiblosDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
            Dim dto As DeskDocumentResult = CurrentDeskDocumentFacade.CreateDeskDocumentDto(deskDocument, docInfo)

            documentDtos.Add(dto)
        Next

        BindDeskDocuments(documentDtos)
    End Sub

    Public Sub BindDeskDocuments(deskDocuments As ICollection(Of DeskDocumentResult))
        dgvDocuments.DataSource = deskDocuments
        dgvDocuments.DataBind()
    End Sub

    Private Sub LoadCollaborationTemplates()
        ddlCollaborationType.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        Dim templates As ICollection(Of WebAPIDto(Of TemplateCollaboration)) = New List(Of WebAPIDto(Of TemplateCollaboration))

        Try
            templates = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.OnlyAuthorized = True
                        finder.Locked = True
                        finder.Status = TemplateCollaborationStatus.Active
                        finder.UserName = DocSuiteContext.Current.User.UserName
                        finder.Domain = DocSuiteContext.Current.User.Domain
                        Return finder.DoSearch()
                    End Function)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("E' avvenuto un errore durante la ricerca delle tipologie di collaborazione. Provare a ricaricare la pagina.")
            Exit Sub
        End Try

        Dim listItems As List(Of DropDownListItem) = New List(Of DropDownListItem)
        If templates IsNot Nothing AndAlso templates.Count > 0 Then
            listItems = templates.Select(Function(t) CreateCollaborationTemplateListItem(t.Entity)).ToList()
        End If

        listItems.ForEach(Sub(i) ddlCollaborationType.Items.Add(i))
    End Sub

    Private Function CreateCollaborationTemplateListItem(template As TemplateCollaboration) As DropDownListItem
        Dim listItem As DropDownListItem = New DropDownListItem(template.Name, template.DocumentType)

        Select Case template.DocumentType
            Case CollaborationDocumentType.P.ToString(),
                 CollaborationDocumentType.U.ToString()
                listItem.ImageUrl = "~/Comm/images/DocSuite/Protocollo16.png"
            Case CollaborationDocumentType.D.ToString()
                listItem.ImageUrl = "~/Comm/Images/DocSuite/Delibera16.png"
            Case CollaborationDocumentType.A.ToString()
                listItem.ImageUrl = "~/Comm/images/Docsuite/Atto16.png"
            Case CollaborationDocumentType.S.ToString(),
                 CollaborationDocumentType.UDS.ToString()
                listItem.ImageUrl = ImagePath.SmallDocumentSeries
        End Select

        Return listItem
    End Function

    Public Function GetProtocolInitializer() As ProtocolInitializer Implements IProtocolInitializer.GetProtocolInitializer
        Dim initializer As ProtocolInitializer = New ProtocolInitializer()
        initializer.Subject = CurrentDesk.Description
        initializer.MainDocument = SelectedMainDocument
        initializer.Attachments = SelectedAttachments
        initializer.Annexed = SelectedAnnexed
        Return initializer
    End Function

    Public Function GetCollaborationInitializer() As CollaborationInitializer Implements ICollaborationInitializer.GetCollaborationInitializer
        Dim initializer As CollaborationInitializer = New CollaborationInitializer()
        initializer.Subject = CurrentDesk.Description
        initializer.MainDocument = SelectedMainDocument
        initializer.MainDocumentOmissis = SelectedMainDocumentOmissis
        initializer.Attachments = SelectedAttachments
        initializer.AttachmentsOmissis = SelectedAttachmentsOmissis
        initializer.Annexed = SelectedAnnexed
        Return initializer
    End Function

    Public Function GetResolutionInitializer() As ResolutionInitializer Implements IResolutionInitializer.GetResolutionInitializer
        Dim initializer As ResolutionInitializer = New ResolutionInitializer()
        initializer.Subject = CurrentDesk.Description
        initializer.MainDocument = SelectedMainDocument
        initializer.MainDocumentOmissis = SelectedMainDocumentOmissis
        initializer.Attachments = SelectedAttachments
        initializer.AttachmentsOmissis = SelectedAttachmentsOmissis
        initializer.Annexed = SelectedAnnexed
        Return initializer
    End Function

    Private Function GetSelectedDocumentsByDocumentTypeCode(documentTypeCode As String) As ICollection(Of DocumentInfo)
        Dim selectedDocuments As ICollection(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
            Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
            If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(documentTypeCode) Then
                Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                selectedDocuments.Add(documentInfo)
            End If
        Next
        Return selectedDocuments
    End Function

    Private Function GetDocumentUnitPostBackUrl() As String
        Select Case rblDocumentUnit.SelectedValue
            Case "0"
                Return GetCollaborationPostBackUrl()
            Case "1"
                Return GetProtocolPostBackUrl()
            Case "2"
                Return GetResolutionPostBackUrl()
        End Select
        Return String.Empty
    End Function

    Private Function GetProtocolPostBackUrl() As String
        Dim params As New StringBuilder()
        params.Append("Type=Prot")
        params.AppendFormat("&IdDesk={0}", CurrentDesk.Id)
        params.AppendFormat("&FromDesk={0}", True)
        Dim url As String = String.Format("~/Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        Return url
    End Function

    Private Function GetCollaborationPostBackUrl() As String
        Dim params As New StringBuilder()
        If (rblSendTo.SelectedValue.Eq("0")) Then
            params.Append("Titolo=Inserimento&Action=Add&Title2=Alla Visione/Firma&Action2=CI&FromDesk=True")
        Else
            params.Append("Titolo=Inserimento&Action=Apt&Title2=Al Protocollo/Segreteria&Action2=CA&FromDesk=True")
        End If
        params.AppendFormat("&IdDesk={0}", CurrentDesk.Id)
        params.AppendFormat("&Type={0}", CurrentCollaborationDomainType)
        If CurrentCollaborationType.HasValue Then
            params.AppendFormat("&docType={0}", CurrentCollaborationType.Value)
        End If
        Dim url As String = String.Format("~/User/UserCollGestione.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        Return url
    End Function

    Private Function GetResolutionPostBackUrl() As String
        Dim params As New StringBuilder()
        params.Append("Type=Resl")
        params.AppendFormat("&IdDesk={0}", CurrentDesk.Id)
        params.Append("&Action=FromDesk")
        Dim url As String = String.Format("~/Resl/ReslInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        Return url
    End Function
#End Region

End Class