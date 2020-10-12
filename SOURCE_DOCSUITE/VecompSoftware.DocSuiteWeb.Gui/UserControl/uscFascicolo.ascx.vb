Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Partial Public Class uscFascicolo
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const EXTERNAL_DATA_INITIALIZE_CALLBACK As String = "uscFascicolo.loadExternalDataCallback({0});"
    Private Const GRID_REFRESH_CALLBACK As String = "uscFascicolo.refreshGridUDCallback({0},{1});"
    Private Const INSERTS_DOCUMENTUNIT_NAME As String = "Inserto"

    Private Const WORD_TABLE_COLUMN_FDU_SUBJECT As String = "Oggetto"
    Private Const WORD_TABLE_COLUMN_FDU_CATEGORY As String = "Classificatore"
    Private Const WORD_TABLE_COLUMN_FDU_REGISTRATION_DATE As String = "Data di registrazione"
    Private Const WORD_TABLE_COLUMN_FDU_FASCICLE_REGISTRATION_DATE As String = "Data di registrazione nel fascicolo"
    Private Const WORD_TABLE_COLUMN_FDU_FASCICLE_REGISTRATION_USER As String = "Utente di registrazione nel fascicolo"
    Private Const WORD_TABLE_COLUMN_FD_DOCUMENT_NAME As String = "Nome file"
    Private Const WORD_TABLE_COLUMN_FD_DOCUMENT_CREATED_DATE As String = "Data di creazione"
    Private Const WORD_TABLE_COLUMN_FD_DOCUMENT_REGISTRATION_USER As String = "Utente di registrazione nel documento"
#End Region

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public ReadOnly Property UscFascicleFolder As uscFascicleFolders
        Get
            Return uscFascicleFolders
        End Get
    End Property

    Public ReadOnly Property UscDocumentReference As uscDocumentUnitReferences
        Get
            Return uscDocumentUnitReferences
        End Get
    End Property

    Public Property IsEditPage As Boolean
    Public Property IsAuthorizePage As Boolean

    Public ReadOnly Property GridUD As RadGrid
        Get
            Return grdUD
        End Get
    End Property

    Public ReadOnly Property PanelUD As LayoutRow
        Get
            Return pnlFoldersAndUD
        End Get
    End Property

    Public ReadOnly Property DeterminaCaption As String
        Get
            If (DocSuiteContext.Current.IsResolutionEnabled) Then
                Return Facade.ResolutionTypeFacade.DeterminaCaption()
            End If
            Return String.Empty

        End Get
    End Property

    Public ReadOnly Property DeliberaCaption As String
        Get
            If (DocSuiteContext.Current.IsResolutionEnabled) Then
                Return Facade.ResolutionTypeFacade.DeliberaCaption()
            End If
            Return String.Empty
        End Get
    End Property

    Public Property CurrentFascicleId As Guid
        Get
            If ViewState(String.Format("{0}_CurrentFascicleId", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_CurrentFascicleId", ID)), Guid)
            End If
            Return Guid.Empty
        End Get
        Set(value As Guid)
            ViewState(String.Format("{0}_CurrentFascicleId", ID)) = value
        End Set
    End Property

    Public ReadOnly Property FasciclesPanelVisibilities As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.FasciclesPanelVisibilities)
        End Get
    End Property

    Public Property CurrentWorkflowActivityId As String

    Private Property GrdUDFilters As IDictionary(Of String, String)

#End Region

#Region " Events "

    Private Sub uscFascicolo_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        GrdUDFilters = Nothing
        uscFascSummary.IsEditPage = IsEditPage
        uscFascicleFolders.IsVisibile = Not IsEditPage AndAlso Not IsAuthorizePage
        uscResponsabili.TreeViewCaption = DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel
        uscCustomActionsRest.IsSummary = Not IsEditPage
        If Not IsPostBack Then
            grdUD.DataSource = New List(Of String)
            uscResponsabili.Visible = IsEditPage
            uscRole.Visible = IsAuthorizePage
            uscRole.MultipleRoles = IsAuthorizePage
            uscRole.FascicleVisibilityTypeButtonEnabled = False
            If Not IsEditPage AndAlso Not IsAuthorizePage Then
                uscRole.ReadOnlyMode = True
            End If
            If IsEditPage Then
                uscDynamicMetadataSummaryRest.Visible = False
            End If
            rowAuthorizations.SetDisplay(Not (IsEditPage OrElse IsAuthorizePage))
        End If
    End Sub

    Protected Sub UscFascicoloAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Try
                Select Case ajaxModel.ActionName
                    Case "LoadExternalData"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
                            Dim fascicle As Entity.Fascicles.Fascicle = JsonConvert.DeserializeObject(Of Entity.Fascicles.Fascicle)(ajaxModel.Value(0))

                            Dim workflowHandler As String = String.Empty
                            Dim workflowRole As Integer = 0
                            Dim isActiveWorkflow As Boolean = False
                            If ajaxModel.Value.Count > 1 Then
                                isActiveWorkflow = JsonConvert.DeserializeObject(Of Boolean)(ajaxModel.Value(1))
                                workflowHandler = ajaxModel.Value(2)
                            End If

                            uscRole.FascicleVisibilityTypeButtonEnabled = IsAuthorizePage AndAlso (fascicle.FascicleType = Entity.Fascicles.FascicleType.Procedure)
                            LoadAuthorizations(fascicle, workflowHandler)
                            LoadRoles(fascicle)
                            If IsEditPage Then
                                uscResponsabili.Visible = True
                                LoadRiferimenti(fascicle)
                            End If
                            If Not IsEditPage Then
                                Dim metadata As List(Of BaseDocumentGeneratorParameter) = New List(Of BaseDocumentGeneratorParameter)()
                                If ProtocolEnv.MetadataRepositoryEnabled AndAlso Not String.IsNullOrEmpty(fascicle.MetadataValues) Then
                                    Dim metadataDesigner As MetadataDesignerModel = JsonConvert.DeserializeObject(Of MetadataDesignerModel)(fascicle.MetadataDesigner)
                                    Dim metadataValues As ICollection(Of MetadataValueModel) = JsonConvert.DeserializeObject(Of ICollection(Of MetadataValueModel))(fascicle.MetadataValues)
                                    For Each item As MetadataValueModel In metadataValues.Where(Function(x) Not metadataDesigner.DiscussionFields.Any(Function(xx) xx.KeyName = x.KeyName))
                                        metadata.Add(New StringParameter(item.KeyName, Server.HtmlEncode(item.Value)))
                                    Next
                                    For Each item As DiscussionFieldModel In metadataDesigner.DiscussionFields
                                        metadata.Add(New StringParameter(item.Label, Server.HtmlEncode(String.Join(", ", item.Comments.Select(Function(f) f.Comment).ToArray()))))
                                    Next
                                End If
                                AddDocumentGeneratorParameters(metadata, fascicle)
                                AjaxManager.ResponseScripts.Add(String.Concat("$(document).ready(function() {SetMetadataSessionStorage(`", JsonConvert.SerializeObject(metadata, DocSuiteContext.DefaultWebAPIJsonSerializerSettings), "`)});"))
                            End If
                            AjaxManager.ResponseScripts.Add(String.Format(EXTERNAL_DATA_INITIALIZE_CALLBACK, uscRole.FascicleVisibilityTypeButtonEnabled.ToString().ToLower()))
                        End If

                    Case "ReloadGrid"
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 1 Then
                            GrdUDFilters = JsonConvert.DeserializeObject(Of IDictionary(Of String, String))(ajaxModel.Value(1))
                            Dim insertsArchiveChains As ICollection(Of Guid) = JsonConvert.DeserializeObject(Of ICollection(Of Guid))(ajaxModel.Value(2))
                            Dim documentUnits As IList(Of DocumentUnitModel) = JsonConvert.DeserializeObject(Of IList(Of DocumentUnitModel))(ajaxModel.Value(0))
                            Dim orders As ICollection(Of JObject) = JsonConvert.DeserializeObject(Of ICollection(Of JObject))(ajaxModel.Value(3))
                            Dim docs As IList(Of DocumentUnitModel) = New List(Of DocumentUnitModel)()
                            If (insertsArchiveChains IsNot Nothing AndAlso insertsArchiveChains.Count > 0) AndAlso GrdUDFilters IsNot Nothing _
                                AndAlso (GrdUDFilters.Count = 0 OrElse
                                            (GrdUDFilters.ContainsKey("DocumentUnitName") AndAlso (GrdUDFilters("DocumentUnitName").Equals(INSERTS_DOCUMENTUNIT_NAME) _
                                             OrElse String.IsNullOrEmpty(GrdUDFilters("DocumentUnitName")))) OrElse GrdUDFilters.ContainsKey("Title")) Then
                                docs = LoadDocuments(insertsArchiveChains)
                                If GrdUDFilters.ContainsKey("Title") Then
                                    docs = docs.Where(Function(d) d.Subject.Contains(GrdUDFilters("Title"))).ToList()
                                End If
                            End If

                            documentUnits = documentUnits.Union(docs).ToList()
                            Select Case orders.Count
                                Case 0
                                    Dim defaultSorting As JObject = New JObject(New JProperty("FieldName", "RegistrationDate"), New JProperty("SortOrder", "2"))
                                    documentUnits = OrderList(documentUnits, defaultSorting, Nothing)
                                Case 1
                                    documentUnits = OrderList(documentUnits, orders(0), Nothing)
                                Case 2
                                    Dim orderBy As JObject = orders(0)
                                    Dim thenBy As JObject = orders(1)
                                    documentUnits = OrderList(documentUnits, orderBy, thenBy)
                            End Select

                            LoadUD(documentUnits)
                            AjaxManager.ResponseScripts.Add(String.Format(GRID_REFRESH_CALLBACK, ajaxModel.Value(1), ajaxModel.Value(3)))
                        End If
                End Select
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore nel caricamento dei dati del fascicolo.", ex)
                AjaxManager.Alert("Errore nel caricamento dei dati del fascicolo.")
                Return
            End Try

        End If

    End Sub

    Private Function OrderList(docs As IList(Of DocumentUnitModel), OrderBy As JObject, ThenBy As JObject) As IList(Of DocumentUnitModel)
        If OrderBy("SortOrder").ToString() = "1" Then
            docs = docs.OrderBy(Function(d) d.GetType().GetProperty(OrderBy("FieldName").ToString()).GetValue(d)).ToList()
        Else
            docs = docs.OrderByDescending(Function(d) d.GetType().GetProperty(OrderBy("FieldName").ToString()).GetValue(d)).ToList()
        End If
        If ThenBy IsNot Nothing Then
            If OrderBy("SortOrder").ToString() = "1" Then
                If ThenBy("SortOrder").ToString() = "1" Then
                    docs = docs.OrderBy(Function(d) d.GetType().GetProperty(OrderBy("FieldName").ToString()).GetValue(d)).ThenBy(Function(d) d.GetType().GetProperty(ThenBy("FieldName").ToString()).GetValue(d)).ToList()
                Else
                    docs = docs.OrderBy(Function(d) d.GetType().GetProperty(OrderBy("FieldName").ToString()).GetValue(d)).ThenByDescending(Function(d) d.GetType().GetProperty(ThenBy("FieldName").ToString()).GetValue(d)).ToList()
                End If
            Else
                If ThenBy("SortOrder").ToString() = "1" Then
                    docs = docs.OrderByDescending(Function(d) d.GetType().GetProperty(OrderBy("FieldName").ToString()).GetValue(d)).ThenBy(Function(d) d.GetType().GetProperty(ThenBy("FieldName").ToString()).GetValue(d)).ToList()
                Else
                    docs = docs.OrderByDescending(Function(d) d.GetType().GetProperty(OrderBy("FieldName").ToString()).GetValue(d)).ThenByDescending(Function(d) d.GetType().GetProperty(ThenBy("FieldName").ToString()).GetValue(d)).ToList()
                End If
            End If
        End If
        Return docs
    End Function

    Protected Sub GrdUD_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdUD.ItemDataBound
        If TypeOf e.Item Is GridGroupHeaderItem Then
            Dim GroupHeader As GridGroupHeaderItem = DirectCast(e.Item, GridGroupHeaderItem)
            GroupHeader.DataCell.Text = GroupHeader.DataCell.Text.Replace(":", "").Trim()
        End If

        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
            If GrdUDFilters IsNot Nothing Then
                For Each filterSet As KeyValuePair(Of String, String) In GrdUDFilters
                    Dim controlName As String = String.Empty
                    Select Case filterSet.Key
                        Case "ReferenceType"
                            controlName = "rcbFilterReferenceType"
                            Exit Select
                        Case "Title"
                            controlName = "subjectFilter"
                            Exit Select
                    End Select
                    Dim control As Control = filterItem.FindControl(controlName)
                    If control IsNot Nothing Then
                        Dim value As String = filterSet.Value
                        FilterHelper.SetFilterValue(control, value)
                    End If
                Next
            End If

        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As DocumentUnitModel = DirectCast(e.Item.DataItem, DocumentUnitModel)
        Dim btnUDLink As RadButton = DirectCast(e.Item.FindControl("btnUDLink"), RadButton)
        btnUDLink.Attributes.Add("DocumentUnitName", boundHeader.DocumentUnitName)
        Dim env As DSWEnvironment = DirectCast(If(boundHeader.Environment < 100, boundHeader.Environment, DSWEnvironment.UDS), DSWEnvironment)
        btnUDLink.Attributes.Add("Environment", Convert.ToInt32(env).ToString())
        If env = DSWEnvironment.UDS AndAlso boundHeader.IdUDSRepository IsNot Nothing Then
            btnUDLink.Attributes.Add("UDSRepositoryId", boundHeader.IdUDSRepository.ToString())
        End If

        btnUDLink.Text = boundHeader.Title

        Dim urlIcon As String = String.Empty
        Dim lblReferenceType As Image = DirectCast(e.Item.FindControl("imgReferenceType"), Image)
        If Not boundHeader.DocumentUnitName.Equals(INSERTS_DOCUMENTUNIT_NAME) Then

            btnUDLink.Attributes.Add("EntityId", boundHeader.EntityId.ToString())
            btnUDLink.Attributes.Add("Year", boundHeader.Year.ToString())
            btnUDLink.Attributes.Add("Number", boundHeader.Number.ToString())


            If boundHeader.ReferenceType = Data.ReferenceType.Reference Then
                lblReferenceType.ImageUrl = ImagePath.SmallLink
                lblReferenceType.ToolTip = "Per riferimento nel fascicolo corrente"
            Else
                lblReferenceType.ImageUrl = ImagePath.SmallFolderDocument
                lblReferenceType.ToolTip = "Fascicolato nel fascicolo corrente"
            End If

            Dim lblUDCategory As Label = DirectCast(e.Item.FindControl("lblCategory"), Label)
            lblUDCategory.Text = boundHeader.Category.Name

            urlIcon = GetIconUrl(env)
        Else
            lblReferenceType.ImageUrl = ImagePath.SmallDocument
            urlIcon = boundHeader.Number
            btnUDLink.Attributes.Add("SerializedDoc", boundHeader.MainDocumentName)
            Dim results As NameValueCollection = HttpUtility.ParseQueryString(boundHeader.MainDocumentName)
            btnUDLink.Attributes.Add("BiblosChainId", results("ChainId"))
            btnUDLink.Attributes.Add("BiblosDocumentId", results("Guid"))
            btnUDLink.Attributes.Add("BiblosDocumentName", results("Name"))
        End If

        btnUDLink.Attributes.Add("Title", boundHeader.Title.ToString())
        btnUDLink.Attributes.Add("UniqueId", boundHeader.UniqueId.ToString())
        btnUDLink.Icon.PrimaryIconUrl = urlIcon
        btnUDLink.Icon.PrimaryIconHeight = Unit.Pixel(16)
        btnUDLink.Icon.PrimaryIconWidth = Unit.Pixel(16)

        If ProtocolEnv.MultiAOOFascicleEnabled AndAlso boundHeader.TenantAOO IsNot Nothing AndAlso boundHeader.TenantAOO.IdTenantAOO <> CurrentTenant.TenantAOO.UniqueId Then
            btnUDLink.Enabled = False
            e.Item.BackColor = Drawing.Color.LightGray
            e.Item.SelectableMode = GridItemSelectableMode.None
            e.Item.Enabled = False
        End If

        Dim lblUDRegistrationDate As Label = DirectCast(e.Item.FindControl("lblUDRegistrationDate"), Label)
        If boundHeader.RegistrationDate.HasValue Then
            lblUDRegistrationDate.Text = boundHeader.RegistrationDate.Value.Date.ToShortDateString()
        Else
            lblUDRegistrationDate.Text = String.Empty
        End If

        Dim imgUDFascicle As RadButton = DirectCast(e.Item.FindControl("imgUDFascicle"), RadButton)
        If boundHeader.IdFascicle.HasValue AndAlso Not boundHeader.IdFascicle.Value.Equals(CurrentFascicleId) Then
            imgUDFascicle.Visible = True
            imgUDFascicle.Attributes.Add("IdFascicle", boundHeader.IdFascicle.Value.ToString())
            imgUDFascicle.Image.EnableImageButton = True
            imgUDFascicle.Image.IsBackgroundImage = True
            imgUDFascicle.Image.ImageUrl = ImagePath.SmallLinkedFolder
            imgUDFascicle.ToolTip = "Fascicolato in altro fascicolo"
        Else
            imgUDFascicle.Visible = False
        End If

        Dim lblUDObject As Label = DirectCast(e.Item.FindControl("lblUDObject"), Label)
        lblUDObject.Text = boundHeader.Subject

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscFascicoloAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdUD)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscResponsabili)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAuthorizations)
    End Sub

    Private Sub LoadUD(models As IList(Of DocumentUnitModel))
        grdUD.DataSource = models
        grdUD.DataBind()
    End Sub

    Private Sub LoadRiferimenti(fascicle As Entity.Fascicles.Fascicle)
        Dim riferimenti As IList(Of ContactDTO) = New List(Of ContactDTO)
        For Each fascicleContact As Entity.Commons.Contact In fascicle.Contacts
            Dim tmpDto As ContactDTO = New ContactDTO()
            Dim contact As Data.Contact = Facade.ContactFacade.GetById(fascicleContact.EntityId)

            tmpDto.Contact = contact
            tmpDto.Type = ContactDTO.ContactType.Address
            tmpDto.Id = contact.Id
            riferimenti.Add(tmpDto)
        Next
        uscResponsabili.DataSource = riferimenti
        uscResponsabili.DataBind()
    End Sub

    Private Sub LoadAuthorizations(fascicle As Entity.Fascicles.Fascicle, handler As String)
        Dim roleCaption As String = "Settori autorizzati"
        uscAuthorizations.MasterRoles = New List(Of Data.Role)()
        If Not String.IsNullOrEmpty(handler) Then
            uscAuthorizations.WorkflowHandler = handler
        End If

        If (Not String.IsNullOrEmpty(ProtocolEnv.FascicleAuthorizedRoleCaption)) Then
            roleCaption = ProtocolEnv.FascicleAuthorizedRoleCaption.ToString()
        End If
        If fascicle.FascicleRoles IsNot Nothing AndAlso fascicle.FascicleRoles.Count > 0 Then

            If (fascicle.VisibilityType.Equals(Entity.Fascicles.VisibilityType.Accessible)) Then
                roleCaption = "Settori autorizzati (documenti disponibili ai settori autorizzati)"
            End If

            Dim responsibleRoles As List(Of Data.Role) = New List(Of Data.Role)
            Dim accountedRoles As List(Of Data.Role) = New List(Of Data.Role)
            Dim workflowRole As List(Of Data.Role) = New List(Of Data.Role)
            Dim role As Data.Role = New Data.Role()
            For Each item As Entity.Fascicles.FascicleRole In fascicle.FascicleRoles
                role = Facade.RoleFacade.GetById(item.Role.EntityShortId)
                If item.AuthorizationRoleType = Entity.Commons.AuthorizationRoleType.Accounted Then
                    accountedRoles.Add(role)
                End If
                If item.AuthorizationRoleType = Entity.Commons.AuthorizationRoleType.Responsible AndAlso Not item.IsMaster Then
                    workflowRole.Add(role)
                End If
                If item.AuthorizationRoleType = Entity.Commons.AuthorizationRoleType.Responsible AndAlso item.IsMaster Then
                    responsibleRoles.Add(role)
                End If
            Next
            If responsibleRoles.Any() Then
                uscAuthorizations.ResponsibleRoles = responsibleRoles
            End If
            If workflowRole.Any() Then
                uscAuthorizations.WorkflowRole = workflowRole
            End If
            If accountedRoles.Any() Then
                uscAuthorizations.AccountedRoleCaption = roleCaption
                uscAuthorizations.AccountedRoles = accountedRoles.OrderBy(Function(x) x.Name).ToList()
            End If
        End If
        uscAuthorizations.ResponsibleContacts = fascicle.Contacts
        uscAuthorizations.BindData()

    End Sub

    Private Sub LoadRoles(fascicle As Entity.Fascicles.Fascicle)

        If (Not String.IsNullOrEmpty(ProtocolEnv.FascicleAuthorizedRoleCaption)) Then
            uscRole.Caption = ProtocolEnv.FascicleAuthorizedRoleCaption.ToString()
        End If
        If fascicle.FascicleRoles IsNot Nothing AndAlso fascicle.FascicleRoles.Count > 0 Then
            If (fascicle.VisibilityType.Equals(Entity.Fascicles.VisibilityType.Accessible)) Then
                uscRole.Caption = String.Format("Autorizzazioni ({0})", "documenti disponibili ai settori autorizzati")
            End If
            If fascicle.FascicleRoles.Any(Function(r) Not r.IsMaster) Then
                uscRole.Required = False
                uscRole.Visible = True
                uscRole.MultipleRoles = True
            End If
        End If
    End Sub

    Private Function GetIconUrl(environment As Integer) As String
        Dim env As DSWEnvironment = DirectCast([Enum].Parse(GetType(DSWEnvironment), environment.ToString()), DSWEnvironment)
        Select Case env
            Case DSWEnvironment.Protocol
                Return "../Comm/Images/DocSuite/Protocollo16.gif"
            Case DSWEnvironment.Resolution
                Return "../Comm/Images/DocSuite/Atti16.gif"
            Case DSWEnvironment.DocumentSeries
                Return "../App_Themes/DocSuite2008/imgset16/document_copies.png"
            Case DSWEnvironment.UDS
                Return "../App_Themes/DocSuite2008/imgset16/document_copies.png"
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function LoadDocuments(idArchiveChains As ICollection(Of Guid)) As List(Of DocumentUnitModel)

        Dim documentInfoList As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim location As Location = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
        Dim documentModels As List(Of DocumentUnitModel) = New List(Of DocumentUnitModel)

        If location IsNot Nothing Then
            For Each idArchiveChain As Guid In idArchiveChains
                documentInfoList.AddRange(BiblosDocumentInfo.GetDocumentsLatestVersion(idArchiveChain).ToList())
            Next

            Dim mappedDoc As DocumentUnitModel
            Dim noteAttribute As String = String.Empty
            Dim items As NameValueCollection
            For Each doc As BiblosDocumentInfo In documentInfoList
                mappedDoc = New DocumentUnitModel With {
                    .DocumentUnitName = INSERTS_DOCUMENTUNIT_NAME
                }
                If doc.DateCreated.HasValue Then
                    mappedDoc.RegistrationDate = doc.DateCreated.Value
                End If
                mappedDoc.UniqueId = doc.DocumentId
                mappedDoc.Title = doc.Name

                noteAttribute = String.Empty
                If doc.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.NOTE_ATTRIBUTE)) Then
                    noteAttribute = doc.Attributes(BiblosFacade.NOTE_ATTRIBUTE)
                End If
                mappedDoc.Subject = noteAttribute
                mappedDoc.Number = ImagePath.FromDocumentInfo(doc)

                items = doc.ToQueryString()
                mappedDoc.MainDocumentName = items.AsEncodedQueryString()
                mappedDoc.Environment = DSWEnvironment.Document
                documentModels.Add(mappedDoc)
            Next
        End If
        Return documentModels
    End Function

    Private Sub AddDocumentGeneratorParameters(metadata As List(Of BaseDocumentGeneratorParameter), fascicle As Entity.Fascicles.Fascicle)
        metadata.Add(New StringParameter("_filename", fascicle.Title.Replace(".", "_").Replace("-", "_")))
        metadata.Add(New IntParameter("Year", fascicle.Year))
        metadata.Add(New IntParameter("Number", fascicle.Number))
        metadata.Add(New DateTimeParameter("StartDate", fascicle.StartDate.Date))
        metadata.Add(New StringParameter("Title", fascicle.Title))
        metadata.Add(New StringParameter("FascicleObject", Server.HtmlEncode(fascicle.FascicleObject)))
        metadata.Add(New StringParameter("Contact", If(fascicle.Contacts.Any(), fascicle.Contacts.FirstOrDefault().Description, String.Empty)))
        metadata.Add(New StringParameter("FDU_Subject", WORD_TABLE_COLUMN_FDU_SUBJECT))
        metadata.Add(New StringParameter("FDU_Category", WORD_TABLE_COLUMN_FDU_CATEGORY))
        metadata.Add(New StringParameter("FDU_RegistrationDate", WORD_TABLE_COLUMN_FDU_REGISTRATION_DATE))
        metadata.Add(New StringParameter("FDU_FascicleRegistrationDate", WORD_TABLE_COLUMN_FDU_FASCICLE_REGISTRATION_DATE))
        metadata.Add(New StringParameter("FDU_FascicleRegistrationUser", WORD_TABLE_COLUMN_FDU_FASCICLE_REGISTRATION_USER))
        metadata.Add(New StringParameter("FD_DocumentName", WORD_TABLE_COLUMN_FD_DOCUMENT_NAME))
        metadata.Add(New StringParameter("FD_DocumentCreatedDate", WORD_TABLE_COLUMN_FD_DOCUMENT_CREATED_DATE))
        metadata.Add(New StringParameter("FD_DocumentRegistrationUser", WORD_TABLE_COLUMN_FD_DOCUMENT_REGISTRATION_USER))
    End Sub
#End Region

End Class