Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Collaborations
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class DeskToCollaboration
    Inherits DeskBasePage
    Implements ICollaborationInitializer

#Region "Fields"
    Private Const PAGE_TITLE As String = "Tavoli - Inserimento Collaborazione"
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const ATTACHMENT_CODE As String = "ALL"
    Private Const ANNEXED_CODE As String = "ANX"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"
    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentCollaborationDomainType As String
        Get
            If CurrentCollaborationType.Equals(Nothing) Then
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
                Case Else
                    Return String.Empty
            End Select
        End Get
    End Property

    Public Property CollaborationInitializerSource As CollaborationInitializer
        Get
            Return TryCast(Session("collaborationInitializerSource"), CollaborationInitializer)
        End Get
        Set(value As CollaborationInitializer)
            If value Is Nothing Then
                Session.Remove("collaborationInitializerSource")
            Else
                Session("collaborationInitializerSource") = value
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentCollaborationType As CollaborationDocumentType
        Get
            Dim collType As CollaborationDocumentType
            If [Enum].TryParse(ddlCollaborationType.SelectedValue, collType) Then
                Return collType
            End If
        End Get
    End Property

    Public ReadOnly Property SelectedMainDocument As DocumentInfo
        Get
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim radio As RadioButton = CType(item.FindControl("rdbMainDocument"), RadioButton)
                If radio IsNot Nothing AndAlso radio.Checked Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
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
            Dim attachments As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                Dim radio As RadioButton = CType(item.FindControl("rdbMainDocument"), RadioButton)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(ATTACHMENT_CODE) AndAlso Not radio.Checked Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    attachments.Add(documentInfo)
                End If
            Next
            Return attachments
        End Get
    End Property

    Public ReadOnly Property SelectedAnnexed As IList(Of DocumentInfo)
        Get
            Dim attachments As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                Dim radio As RadioButton = CType(item.FindControl("rdbMainDocument"), RadioButton)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(ANNEXED_CODE) AndAlso Not radio.Checked Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    attachments.Add(documentInfo)
                End If
            Next
            Return attachments
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

#Region "Events"
    Protected Sub DgvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemDataBound

        If e.Item.ItemType = GridItemType.Header Then
            CType(CType(e.Item, GridHeaderItem)("Select").Controls(0), CheckBox).Visible = False
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        ' imgDocumentExtensionType
        Dim dto As DeskDocumentResult = DirectCast(e.Item.DataItem, DeskDocumentResult)
        With DirectCast(e.Item.FindControl("imgDocumentExtensionType"), ImageButton)
            If dto.IsSigned AndAlso FileHelper.MatchExtension(dto.Name, FileHelper.PDF) Then
                .ImageUrl = ImagePath.FormatImageName("file_extension_pdf_signed", True)
            Else
                .ImageUrl = ImagePath.FromFile(dto.Name, True)
            End If

            Dim url As String = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(dto.BiblosSerializeKey))
            .OnClientClick = String.Format(OPEN_WINDOW_SCRIPT, ID, url, windowPreviewDocument.ClientID, "")
        End With


        If dto.IsJustInCollaboration Then
            Dim currentCheckBox As CheckBox = CType(CType(e.Item, GridDataItem)("Select").Controls(0), CheckBox)
            currentCheckBox.Enabled = False
            currentCheckBox.ToolTip = "La scelta del documento è disabilitata in quanto sul documento è stata avviata una collaborazione non ancora conclusa."
        End If

        With DirectCast(e.Item.FindControl("lblDocumentName"), Label)
            .Text = dto.Name
        End With

        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        documentTypeModControl.Items.Add(New DropDownListItem("Allegato", ATTACHMENT_CODE))
        documentTypeModControl.Items.Add(New DropDownListItem("Annesso", ANNEXED_CODE))
        If Not dto.IsJustInCollaboration Then
            e.Item.Selected = True
        End If

    End Sub

    Protected Sub DgvDocuments_ItemCreated(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemCreated
        If (TypeOf e.Item Is GridDataItem) Then
            Dim rdb As RadioButton = CType(e.Item.FindControl("rdbMainDocument"), RadioButton)
            rdb.Attributes.Add("OnClick", "SelectMeOnly(" & rdb.ClientID & ", " & "'dgvDocuments'" & ")")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub BtnToSign_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnToSign.Click
        CollaborationInitializerSource = GetCollaborationInitializer()
        Dim url As String = GetCollaborationToSignPostBackUrl()
        Response.Redirect(url)
    End Sub

    Protected Sub BtnToProtocol_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnToProtocol.Click
        Dim allDocumentSigned As Boolean = dgvDocuments.SelectedItems.Cast(Of GridDataItem).All(Function(x) Convert.ToBoolean(x("IsSigned").Text))
        If Not allDocumentSigned Then
            AjaxAlert("Tutti i documenti selezionati devono essere firmati")
            Exit Sub
        End If
        CollaborationInitializerSource = GetCollaborationInitializer()
        Dim url As String = GetCollaborationToProtocolPostBackUrl()
        Response.Redirect(url)
    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        Response.Redirect(String.Format(DESK_SUMMARY_PATH, CurrentDesk.Id))
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        If CurrentDesk Is Nothing Then
            Throw New DocSuiteException(String.Format("Tavolo con ID {0} Inesistente", CurrentDeskId))
        End If

        If Not CurrentDeskRigths.IsProtocollable Then
            Throw New DocSuiteException(String.Format("Tavolo ID {0}", CurrentDeskId), "Non è possibile creare una collaborazione dal Tavolo selezionato. Verificare se si dispone di sufficienti autorizzazioni.")
        End If
        windowPreviewDocument.Height = Unit.Pixel(ProtocolEnv.DocumentPreviewHeight)
        windowPreviewDocument.Width = Unit.Pixel(ProtocolEnv.DocumentPreviewWidth)

        Title = PAGE_TITLE

        'Svuota la sessione
        CollaborationInitializerSource = Nothing

        InitializePage()
        InitializeDocuments()
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvDocuments, dgvDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
    End Sub

    Private Sub InitializeDocuments()
        Dim documentDtos As ICollection(Of DeskDocumentResult) = New Collection(Of DeskDocumentResult)
        For Each deskDocument As DeskDocument In CurrentDesk.DeskDocuments.Where(Function(x) x.IsActive = 0)
            Dim docInfos As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentDesk.Container.DeskLocation.DocumentServer, deskDocument.IdDocument.Value)
            If Not docInfos.Any() Then
                Exit Sub
            End If
            Dim docInfo As BiblosDocumentInfo = docInfos.OrderByDescending(Function(f) f.Version).First()
            Dim dto As DeskDocumentResult = CurrentDeskDocumentFacade.CreateDeskDocumentDto(deskDocument, docInfo)
            documentDtos.Add(dto)
        Next
        'Se tutti i documenti sono firmati allora abilito la possibilità di creare
        'collaborazione direttamente al Protocollo/Segreteria
        If documentDtos.All(Function(x) x.IsSigned) Then
            btnToProtocol.Enabled = True
        End If

        BindDeskDocuments(documentDtos)
    End Sub

    Private Sub InitializePage()
        lblDeskSubject.Text = CurrentDesk.Description
        lblDeskName.Text = CurrentDesk.Name
        lblExpireDate.Text = String.Empty
        If CurrentDesk.ExpirationDate.HasValue Then
            lblExpireDate.Text = CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy")
        End If

        BindDdlDocType()
    End Sub

    Private Function GetDropDownListItem(template As TemplateCollaboration) As DropDownListItem
        Dim listItem As New DropDownListItem(template.Name, template.DocumentType)

        Select Case template.DocumentType
            Case CollaborationDocumentType.P.ToString(),
                 CollaborationDocumentType.U.ToString()
                listItem.ImageUrl = "~/Comm/Images/DocSuite/Protocollo16.gif"
            Case CollaborationDocumentType.D.ToString()
                listItem.ImageUrl = "~/Comm/images/Docsuite/Delibera16.gif"
            Case CollaborationDocumentType.A.ToString()
                listItem.ImageUrl = "~/Comm/images/Docsuite/Atto16.gif"
            Case CollaborationDocumentType.S.ToString(),
                 CollaborationDocumentType.UDS.ToString()
                listItem.ImageUrl = ImagePath.SmallDocumentSeries
        End Select

        Return listItem
    End Function

    Private Sub BindDdlDocType()
        ddlCollaborationType.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        Dim templates As ICollection(Of WebAPIDto(Of TemplateCollaboration)) = New List(Of WebAPIDto(Of TemplateCollaboration))

        Try
            CurrentTemplateCollaborationFinder.ResetDecoration()
            CurrentTemplateCollaborationFinder.OnlyAuthorized = True
            CurrentTemplateCollaborationFinder.Locked = True
            CurrentTemplateCollaborationFinder.Status = TemplateCollaborationStatus.Active
            CurrentTemplateCollaborationFinder.UserName = DocSuiteContext.Current.User.UserName
            CurrentTemplateCollaborationFinder.Domain = DocSuiteContext.Current.User.Domain
            templates = CurrentTemplateCollaborationFinder.DoSearch()
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("E' avvenuto un errore durante la ricerca delle tipologie di collaborazione. Provare a ricaricare la pagina.")
            Exit Sub
        End Try

        Dim listItems As List(Of DropDownListItem) = New List(Of DropDownListItem)
        If templates IsNot Nothing AndAlso templates.Count > 0 Then
            listItems = templates.Select(Function(t) Me.GetDropDownListItem(t.Entity)).ToList()
        End If

        listItems.ForEach(Sub(i) ddlCollaborationType.Items.Add(i))
    End Sub

    'Setta i dto nella griglia di gestione documenti
    Public Sub BindDeskDocuments(deskDocuments As ICollection(Of DeskDocumentResult))
        dgvDocuments.DataSource = deskDocuments
        dgvDocuments.DataBind()
    End Sub

    Private Function GetCollaborationToSignPostBackUrl() As String
        Dim params As New StringBuilder()
        params.Append("Titolo=Inserimento&Action=Add&Title2=Alla Visione/Firma&Action2=CI&FromDesk=True")
        params.AppendFormat("&IdDesk={0}", CurrentDesk.Id)
        params.AppendFormat("&Type={0}", CurrentCollaborationDomainType)
        If Not CurrentCollaborationType.Equals(Nothing) Then
            params.AppendFormat("&docType={0}", CurrentCollaborationType)
        End If

        Dim url As String = String.Format("~/User/UserCollGestione.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        Return url
    End Function

    Private Function GetCollaborationToProtocolPostBackUrl() As String
        Dim params As New StringBuilder()
        params.Append("Titolo=Inserimento&Action=Apt&Title2=Al Protocollo/Segreteria&Action2=CA&FromDesk=True")
        params.AppendFormat("&IdDesk={0}", CurrentDesk.Id)
        params.AppendFormat("&Type={0}", CurrentCollaborationDomainType)
        If Not CurrentCollaborationType.Equals(Nothing) Then
            params.AppendFormat("&docType={0}", CurrentCollaborationType)
        End If

        Dim url As String = String.Format("~/User/UserCollGestione.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        Return url
    End Function

    Public Function GetCollaborationInitializer() As CollaborationInitializer Implements ICollaborationInitializer.GetCollaborationInitializer
        Dim initializer As CollaborationInitializer = New CollaborationInitializer()
        initializer.Subject = lblDeskSubject.Text
        initializer.MainDocument = SelectedMainDocument
        initializer.Attachments = SelectedAttachments
        initializer.Annexed = SelectedAnnexed

        Return initializer
    End Function

#End Region
End Class