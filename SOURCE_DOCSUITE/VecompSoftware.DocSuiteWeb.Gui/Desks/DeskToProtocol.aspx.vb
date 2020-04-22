Imports System.Collections.Generic
Imports System.Linq
Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class DeskToProtocol
    Inherits DeskBasePage
    Implements IProtocolInitializer

#Region "Fields"
    Private Const PAGE_TITLE As String = "Tavoli - Inserimento Protocollo"
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const ATTACHMENT_CODE As String = "ALL"
    Private Const ANNEXED_CODE As String = "ANX"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"
    Private Const TYPE_PROTOCOL As String = "Prot"
#End Region

#Region "Properties"
    Public Property ProtocolInitializerSource As ProtocolInitializer
        Get
            Return TryCast(Session("protocolInitializerSource"), ProtocolInitializer)
        End Get
        Set(value As ProtocolInitializer)
            If value Is Nothing Then
                Session.Remove("protocolInitializerSource")
            Else
                Session("protocolInitializerSource") = value
            End If
        End Set
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
#End Region

#Region "Events"
    Protected Sub DgvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemDataBound
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

        With DirectCast(e.Item.FindControl("lblDocumentName"), Label)
            .Text = dto.Name
        End With

        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        documentTypeModControl.Items.Add(New DropDownListItem("Allegato", ATTACHMENT_CODE))
        documentTypeModControl.Items.Add(New DropDownListItem("Annesso", ANNEXED_CODE))

        e.Item.Selected = True
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
        Dim url As String = GetProtocolToSignPostBackUrl()
        Server.Transfer(url)
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
        ProtocolInitializerSource = Nothing

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

        BindDeskDocuments(documentDtos)
    End Sub

    Private Sub InitializePage()
        lblDeskSubject.Text = CurrentDesk.Description
        lblDeskName.Text = CurrentDesk.Name
        lblExpireDate.Text = String.Empty
        If CurrentDesk.ExpirationDate.HasValue Then
            lblExpireDate.Text = CurrentDesk.ExpirationDate.Value.ToString("dd/MM/yyyy")
        End If
    End Sub

    'Setta i dto nella griglia di gestione documenti
    Public Sub BindDeskDocuments(deskDocuments As ICollection(Of DeskDocumentResult))
        dgvDocuments.DataSource = deskDocuments
        dgvDocuments.DataBind()
    End Sub

    Private Function GetProtocolToSignPostBackUrl() As String
        Dim params As New StringBuilder()
        params.Append("Type=Prot")
        params.AppendFormat("&IdDesk={0}", CurrentDesk.Id)
        params.AppendFormat("&FromDesk={0}", True)
        Dim url As String = String.Format("~/Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        Return url
    End Function

    Public Function GetProtocolInitializer() As ProtocolInitializer Implements IProtocolInitializer.GetProtocolInitializer
        Dim initializer As ProtocolInitializer = New ProtocolInitializer()
        initializer.Subject = lblDeskSubject.Text
        initializer.MainDocument = SelectedMainDocument
        initializer.Attachments = SelectedAttachments
        initializer.Annexed = SelectedAnnexed
        Return initializer
    End Function

#End Region
End Class