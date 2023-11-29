Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI

Partial Public Class uscProtGrid
    Inherits GridControl

#Region " Fields "

    Public Const COLUMN_CLIENT_SELECT As String = "colClientSelect"
    Public Const COLUMN_SELECTION As String = "colSelection"
    Public Const COLUMN_ACTIONS As String = "colActions"
    Public Const COLUMN_PARTIAL_ACTIONS As String = "colPartialActions"
    Public Const COLUMN_HAS_READ As String = "colHasRead"
    Public Const COLUMN_PROTOCOL_TYPE As String = "colProtocolType"
    Public Const COLUMN_VIEW_DOCUMENTS As String = "colViewDocuments"
    Public Const COLUMN_VIEW_LINKS As String = "colViewLinks"
    Public Const COLUMN_VIEW_FASCICLES As String = "colViewFascicles"
    Public Const COLUMN_VIEW_PROTOCOL As String = "Id"
    Public Const COLUMN_FULL_PROTOCOL_NUMBER As String = "colFullProtocolNumber"
    Public Const COLUMN_REGISTRATION_DATE As String = "RegistrationDate"
    Public Const COLUMN_REGISTRATION_USER As String = "RegistrationUser"
    Public Const COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION As String = "Type.Id"
    Public Const COLUMN_CONTAINER_NAME As String = "Container.Name"
    Public Const COLUMN_CATEGORY_NAME As String = "Category.Name"
    Public Const COLUMN_PROTOCOL_CONTACT As String = "colProtocolContact"
    Public Const COLUMN_PROTOCOL_OBJECT As String = "ProtocolObject"
    Public Const COLUMN_PROTOCOL_STATUS As String = "Status.Description"
    Public Const COLUMN_SUBJECT As String = "colSubject"
    Public Const ingoingPecColumnName As String = "ingoingPec"
    Public Const COLUMN_ACCEPTANCE_ROLES As String = "colAcceptanceRoles"
    Public Const COLUMN_PROTOCOL_SECTIONAL As String = "AP.AccountingSectional"
    Public Const COLUMN_PROTOCOL_INVOICE_NUMBER As String = "AP.InvoiceNumber"
    Public Const COLUMN_PROTOCOL_INVOICE_YEAR As String = "AP.InvoiceYear"
    Public Const COLUMN_PROTOCOL_ACCOUNTING_NUMBER As String = "AP.AccountingNumber"
    Public Const COLUMN_HIGHLIGHT_NOTE As String = "PU.Note"
    Public Const COLUMN_HIGHLIGHT_REGISTRATION_USER As String = "PU.RegistrationUser"

    Protected Const GridPageHistoryKey As String = "PageHistoryKey"

    Private _gridDataSource As IList(Of ProtocolHeader)
    Private _protocolRightsDictionary As IDictionary(Of Guid, ProtocolRights)
    Private _protocolContactLabelsDictionary As IDictionary(Of Guid, String)

#End Region

#Region " Properties "
    Private ReadOnly Property GridDataSource As IList(Of ProtocolHeader)
        Get
            If _gridDataSource Is Nothing Then
                _gridDataSource = DirectCast(grdProtocols.DataSource, IList(Of ProtocolHeader))
            End If
            Return _gridDataSource
        End Get
    End Property
    Private ReadOnly Property protocolRightsDictionary As IDictionary(Of Guid, ProtocolRights)
        Get
            If _protocolRightsDictionary Is Nothing Then
                _protocolRightsDictionary = Facade.ProtocolFacade.GetProtocolRightsDictionary(GridDataSource, False)
            End If
            Return _protocolRightsDictionary
        End Get
    End Property
    Private ReadOnly Property protocolContactLabelsDictionary As IDictionary(Of Guid, String)
        Get
            If _protocolContactLabelsDictionary Is Nothing Then
                _protocolContactLabelsDictionary = Facade.ProtocolFacade.GetProtocolContactLabelsDictionary(GridDataSource)
            End If
            Return _protocolContactLabelsDictionary
        End Get
    End Property

    Public ReadOnly Property Grid As BindGrid
        Get
            Return grdProtocols
        End Get
    End Property

    Public Property ColumnClientSelectVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_CLIENT_SELECT).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_CLIENT_SELECT).Visible = value
        End Set
    End Property

    Public Property ColumnSelectionVisible As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_SELECTION).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_SELECTION).Visible = value
        End Set
    End Property
    Public Property ColumnActionsVisible As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_ACTIONS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_ACTIONS).Visible = value
        End Set
    End Property
    Public Property ColumnPartialActionsVisible As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PARTIAL_ACTIONS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PARTIAL_ACTIONS).Visible = value
        End Set
    End Property
    Public Property ColumnHasReadVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_HAS_READ).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_HAS_READ).Visible = value
        End Set
    End Property
    Public Property ColumnProtocolTypeVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_TYPE).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_TYPE).Visible = value
        End Set
    End Property
    Public Property ColumnViewDocumentsVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_DOCUMENTS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_DOCUMENTS).Visible = value
        End Set
    End Property
    Public Property ColumnViewLinksVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_LINKS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_LINKS).Visible = value
        End Set
    End Property

    Public Property ColumnViewProtocolVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_PROTOCOL).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_PROTOCOL).Visible = value
        End Set
    End Property
    Public Property ColumnFullProtocolNumberVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_FULL_PROTOCOL_NUMBER).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_FULL_PROTOCOL_NUMBER).Visible = value
        End Set
    End Property
    Public Property ColumnRegistrationDateVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_REGISTRATION_DATE).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_REGISTRATION_DATE).Visible = value
        End Set
    End Property
    Public Property ColumnProtocolTypeShortDescriptionVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION).Visible = value
        End Set
    End Property

    Public Property ColumnProtocolTypeShortDescriptionAllowFiltering() As Boolean
        Get
            Return TryCast(grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION), GridBoundColumn).AllowFiltering
        End Get
        Set(ByVal value As Boolean)
            TryCast(grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION), GridBoundColumn).AllowFiltering = value
        End Set
    End Property

    Public Property ColumnContainerNameVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_CONTAINER_NAME).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_CONTAINER_NAME).Visible = value
        End Set
    End Property
    Public Property ColumnCategoryNameVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_CATEGORY_NAME).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_CATEGORY_NAME).Visible = value
        End Set
    End Property
    Public Property ColumnHighlightNoteVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_HIGHLIGHT_NOTE).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_HIGHLIGHT_NOTE).Visible = value
        End Set
    End Property
    Public Property ColumnHighlightRegistrationUserVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_HIGHLIGHT_REGISTRATION_USER).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_HIGHLIGHT_REGISTRATION_USER).Visible = value
        End Set
    End Property
    Public Property ColumnProtocolContactVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_CONTACT).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_CONTACT).Visible = value
        End Set
    End Property
    Public Property ColumnProtocolRegistrationUserVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_REGISTRATION_USER).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_REGISTRATION_USER).Visible = value
        End Set
    End Property
    Public Property ColumnProtocolObjectVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_OBJECT).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_OBJECT).Visible = value
        End Set
    End Property
    Public Property ColumnProtocolStatusVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_STATUS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_STATUS).Visible = value
        End Set
    End Property
    Public Property ColumnIngoingPecVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(ingoingPecColumnName).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(ingoingPecColumnName).Visible = value
        End Set
    End Property

    Public Property ColumnAcceptanceRolesVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_ACCEPTANCE_ROLES).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_ACCEPTANCE_ROLES).Visible = value
        End Set
    End Property

    Public Property EnableGridScrolling As Boolean = True

    Public Property ColumnProtocolSectionalVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_SECTIONAL).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_SECTIONAL).Visible = value
        End Set
    End Property
    Public Property ColumnInvoiceNumberVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_INVOICE_NUMBER).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_INVOICE_NUMBER).Visible = value
        End Set
    End Property
    Public Property ColumnInvoiceYearVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_INVOICE_YEAR).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_INVOICE_YEAR).Visible = value
        End Set
    End Property
    Public Property ColumnAccountingNumberVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_ACCOUNTING_NUMBER).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_ACCOUNTING_NUMBER).Visible = value
        End Set
    End Property
#End Region

#Region " Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        InitializeColumns()
    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' DO Nothing
    End Sub
    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        If IsPostBack Then
            FilterHelper.WriteFilterClientState(Session, CommonInstance.AppTempPath, grdProtocols.CustomPageIndex + 1)
        End If
    End Sub

    Private Sub grdProtocols_Init(ByVal sender As Object, ByVal e As EventArgs) Handles grdProtocols.Init
        grdProtocols.EnableScrolling = EnableGridScrolling
        InitializeFilterColumns()
    End Sub

    Private Sub grdProtocols_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdProtocols.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As ProtocolHeader = DirectCast(e.Item.DataItem, ProtocolHeader)
        Dim hiddenId As String = boundHeader.UniqueId.ToString()
        If ColumnActionsVisible Then
            With DirectCast(e.Item.FindControl("lbtRepair"), LinkButton)
                .Visible = False
                If DocSuiteContext.Current.ProtocolEnv.IsRepairEnabled AndAlso boundHeader.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    .Visible = True
                    .CommandArgument = hiddenId
                End If
            End With
            With DirectCast(e.Item.FindControl("lbtRedo"), LinkButton)
                .Visible = False
                If DocSuiteContext.Current.ProtocolEnv.IsRedoEnabled AndAlso boundHeader.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    .Visible = True
                    .CommandArgument = hiddenId
                End If
            End With
            With DirectCast(e.Item.FindControl("lbtRecover"), LinkButton)
                .Visible = False
                If DocSuiteContext.Current.ProtocolEnv.IsRecoverEnabled AndAlso boundHeader.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    .Visible = True
                    .CommandArgument = hiddenId
                End If
            End With
            With DirectCast(e.Item.FindControl("lbtPresaCarico"), LinkButton)
                .Visible = False
                If (DocSuiteContext.Current.ProtocolEnv.IsRepairEnabled OrElse DocSuiteContext.Current.ProtocolEnv.IsRedoEnabled OrElse DocSuiteContext.Current.ProtocolEnv.IsRecoverEnabled) _
                    AndAlso Not boundHeader.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    .Visible = True
                    .CommandArgument = hiddenId
                End If
            End With
        End If
        If ColumnPartialActionsVisible Then
            With DirectCast(e.Item.FindControl("lbtCompleta"), LinkButton)
                .Visible = False
                If DocSuiteContext.Current.ProtocolEnv.ProtParzialeEnabled AndAlso boundHeader.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    .Visible = True
                    .CommandArgument = hiddenId
                End If
            End With
            With DirectCast(e.Item.FindControl("lbtAnnulla"), LinkButton)
                .Visible = False
                If DocSuiteContext.Current.ProtocolEnv.ProtParzialeEnabled AndAlso boundHeader.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName) Then
                    .Visible = True
                    .CommandArgument = hiddenId
                End If
            End With
        End If

        If ColumnHasReadVisible Then
            With DirectCast(e.Item.FindControl("imgHasRead"), Image)
                If DocSuiteContext.Current.ProtocolEnv.IsLogStatusEnabled AndAlso Not boundHeader.HasRead.GetValueOrDefault(False) Then
                    .ImageUrl = "~/App_Themes/DocSuite2008/imgset16/mail.png"
                Else
                    .Visible = False
                End If
            End With
        End If

        If ColumnProtocolTypeVisible Then
            DirectCast(e.Item.FindControl("imgProtocolType"), Image).ImageUrl = getHeaderImageUrl(boundHeader, "ProtocolType")
        End If
        If ColumnViewDocumentsVisible Then
            Dim protocolRights As ProtocolRights = protocolRightsDictionary.Item(boundHeader.UniqueId)
            With DirectCast(e.Item.FindControl("ibtViewDocuments"), ImageButton)
                .CommandArgument = hiddenId
                .ImageUrl = getHeaderImageUrl(boundHeader, .CommandName)
                .OnClientClick = getHeaderClientScript(boundHeader, .CommandName)
                .PostBackUrl = getHeaderPostBackUrl(boundHeader, .CommandName)
                If Not protocolRights.IsDocumentReadable Then
                    .CssClass = "TransparentControl"
                End If

                .Attributes.Add("onmouseover", "this.style.cursor='hand';")
                .Attributes.Add("onmouseout", "this.style.cursor='default';")
            End With
        End If
        If ColumnViewLinksVisible Then
            With DirectCast(e.Item.FindControl("cmdViewLinks"), RadButton)
                If boundHeader.Links.HasValue AndAlso boundHeader.Links > 0 Then
                    .Image.ImageUrl = "~/comm/images/docsuite/Link16.png"
                    .NavigateUrl = $"~/Prot/ProtCollegamenti.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={boundHeader.UniqueId}&Type=Prot")}"
                Else
                    .Visible = False
                End If
            End With
        End If
        With DirectCast(e.Item.FindControl("hf_protocol_unique"), HiddenField)
            .Value = boundHeader.UniqueId.ToString()
        End With
        If ColumnViewProtocolVisible Then
            With DirectCast(e.Item.FindControl("lbtViewProtocol"), LinkButton)
                .Text = boundHeader.FullProtocolNumber
                .CommandArgument = hiddenId

                If RedirectOnParentPage Then
                    Dim parameters As String = String.Format("UniqueId={0}&Type=Prot", boundHeader.UniqueId)
                    parameters = CommonShared.AppendSecurityCheck(parameters)

                    Dim parentPageUrl As String = "../Prot/ProtVisualizza.aspx?" & parameters
                    Dim parentPageScript As String = grdProtocols.GetRedirectParentPageScript(parentPageUrl)

                    .OnClientClick = parentPageScript
                End If
            End With
        End If
        If ColumnFullProtocolNumberVisible Then
            DirectCast(e.Item.FindControl("lblFullProtocolNumber"), Label).Text = boundHeader.FullProtocolNumber
        End If
        If ColumnProtocolContactVisible Then
            DirectCast(e.Item.FindControl("lblProtocolContact"), Label).Text = getTextByHeader(boundHeader, COLUMN_PROTOCOL_CONTACT)
        End If
        If ColumnProtocolObjectVisible Then
            DirectCast(e.Item.FindControl("lblProtocolObject"), Label).Text = getProtocolObject(boundHeader)
        End If
        If ColumnCategoryNameVisible Then
            DirectCast(e.Item.FindControl("lblCategoryProjection"), Label).Text = getTextByHeader(boundHeader, COLUMN_CATEGORY_NAME)
        End If

        If ColumnHighlightNoteVisible = True Then
            Dim protocolUser As ProtocolUser = Facade.ProtocolUserFacade.GetProtocolUsersByProtocol(boundHeader.UniqueId, DocSuiteContext.Current.User.FullUserName)
            If protocolUser IsNot Nothing Then
                Dim lblProtocolNote As Label = DirectCast(e.Item.FindControl("lblProtocolNote"), Label)
                lblProtocolNote.Visible = True
                lblProtocolNote.Text = protocolUser.Note
                If ColumnHighlightRegistrationUserVisible = True Then
                    Dim data As Date = protocolUser.RegistrationDate.Date
                    Dim user As String = protocolUser.RegistrationUser
                    If protocolUser.LastChangedDate.HasValue Then
                        data = protocolUser.LastChangedDate.Value.Date
                        user = protocolUser.LastChangedUser
                    End If
                    Dim lblHighlightRegistrationUser As Label = DirectCast(e.Item.FindControl("lblHighlightRegistrationUser"), Label)
                    lblHighlightRegistrationUser.Visible = True
                    lblHighlightRegistrationUser.Text = $"{CommonAD.GetDisplayName(user)} il {data.ToShortDateString()}"
                End If
            End If
        End If

        If ColumnProtocolStatusVisible Then
            If Not DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
                ' Fare due colonne distinte per IdStatus e ProtocolStatus e nasconderle programmaticamente pareva brutto... - FG
                If boundHeader.IdStatus.HasValue Then
                    DirectCast(e.Item.FindControl("lblProtocolStatus"), Label).Text = ProtocolFacade.GetStatusDescription(boundHeader.IdStatus.Value)
                End If

            ElseIf boundHeader.ProtocolStatus IsNot Nothing Then
                If Not String.IsNullOrEmpty(boundHeader.ProtocolStatus.BackColor) Then
                    e.Item.Style.Add("background-color", boundHeader.ProtocolStatus.BackColor)
                End If
                If Not String.IsNullOrEmpty(boundHeader.ProtocolStatus.ForeColor) Then
                    e.Item.Style.Add("color", boundHeader.ProtocolStatus.ForeColor)
                End If

                DirectCast(e.Item.FindControl("lbtViewProtocol"), LinkButton).Style.Add("color", boundHeader.ProtocolStatus.ForeColor)
                DirectCast(e.Item.FindControl("lblProtocolStatus"), Label).Text = boundHeader.Status.Description
            End If
        End If

        Dim imgIngoingPec As Image = TryCast(e.Item.FindControl("imgIngoingPec"), Image)
        If imgIngoingPec IsNot Nothing Then
            ' Coccarde colorate
            imgIngoingPec.Visible = False
            If boundHeader.IngoingPecId.HasValue Then
                Dim coccarda As KeyValuePair(Of String, String) = CoccardaManager.GetImage(boundHeader, ProtocolEnv.CoccardaProtocolEnabled)
                If Not String.IsNullOrEmpty(coccarda.Key) Then
                    imgIngoingPec.Visible = True
                    imgIngoingPec.ImageUrl = coccarda.Key
                    imgIngoingPec.ToolTip = coccarda.Value
                End If
            End If
        End If

        Dim btnToEvaluateRoles As RadButton = TryCast(e.Item.FindControl("btnToEvaluateRoles"), RadButton)
        If btnToEvaluateRoles IsNot Nothing AndAlso boundHeader.CountToEvaluateRoles.HasValue AndAlso boundHeader.CountToEvaluateRoles.Value > 0 Then
            btnToEvaluateRoles.Visible = True
            btnToEvaluateRoles.Text = boundHeader.CountToEvaluateRoles.Value.ToString()
            'btnToEvaluateRoles.OnClientClicking = String.Format("OpenToEvaluateRoles({0},{1},'{2}')", ProtocolEnv.DocumentPreviewWidth, ProtocolEnv.DocumentPreviewHeight, boundHeader.UniqueId.ToString())
        End If

        If ColumnProtocolRegistrationUserVisible Then
            DirectCast(e.Item.FindControl("lblRegistrationUser"), Label).Text = CommonAD.GetDisplayName(boundHeader.RegistrationUser)
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsInvoiceDataGridResultEnabled Then
            DirectCast(e.Item.FindControl("lblSectional"), Label).Text = boundHeader.AccountingSectional
            DirectCast(e.Item.FindControl("lblInvoiceNumber"), Label).Text = If(String.IsNullOrEmpty(boundHeader.InvoiceNumber), String.Empty, boundHeader.InvoiceNumber.PadLeft(7, "0"c))
            DirectCast(e.Item.FindControl("lblInvoiceYear"), Label).Text = If(boundHeader.InvoiceYear.HasValue, boundHeader.InvoiceYear.Value.ToString(), String.Empty)
            DirectCast(e.Item.FindControl("lblAccountingNumber"), Label).Text = If(boundHeader.AccountingNumber.HasValue, boundHeader.AccountingNumber.Value.ToString(), String.Empty)
        End If
    End Sub

    Private Sub grdProtocols_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles grdProtocols.ItemCommand
        Select Case e.CommandName
            Case "Repair", "Complete"
                Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(uniqueIdProtocol)
                If currentProtocol.ProtocolObject.Eq("dummy") Then
                    currentProtocol.IdStatus = ProtocolStatusId.Sospeso
                    Facade.ProtocolFacade.Update(currentProtocol)
                    Facade.ProtocolLogFacade.Insert(currentProtocol, ProtocolLogEvent.PM, "Protocollo rimesso in status Sospeso")
                Else
                    Dim parameters As String = CommonShared.AppendSecurityCheck($"Action=Repair&UniqueId={uniqueIdProtocol}&Type=Prot")
                    RedirectOnPage($"~/Prot/ProtModifica.aspx?{parameters}")
                End If

            Case "Cancel"
                Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                Dim parameters As String = CommonShared.AppendSecurityCheck($"UniqueId={uniqueIdProtocol}&Type=Prot")
                RedirectOnPage($"~/Prot/ProtAnnulla.aspx?{parameters}")

            Case "Recover"
                Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                Dim parameters As String = CommonShared.AppendSecurityCheck($"Action=Recover&UniqueId={uniqueIdProtocol}&Type=Prot")
                RedirectOnPage($"~/Prot/ProtInserimento.aspx?{parameters}")

            Case "Steal"
                Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(uniqueIdProtocol)
                Dim previousOwner As String = currentProtocol.RegistrationUser
                currentProtocol.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                Facade.ProtocolFacade.Update(currentProtocol)
                Facade.ProtocolLogFacade.Insert(currentProtocol, ProtocolLogEvent.PM, String.Format("Protocollo preso in carico da [{0}]. Il proprietario precedente era [{1}].", currentProtocol.RegistrationUser, previousOwner))
                grdProtocols.DataBindFinder()

            Case "Redo"
                Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                Dim parameters As String = CommonShared.AppendSecurityCheck($"Action=Redo&UniqueId={uniqueIdProtocol}&Type=Prot")
                RedirectOnPage($"~/Prot/ProtInserimento.aspx?{parameters}")

            Case "ViewProtocol"
                ' TODO: questo codice dovrebbe stare nella pagina, non nel controllo
                Select Case BasePage.Action
                    Case "CopyProtocolDocuments", "Fasc", "Resl"
                        Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                        Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(uniqueIdProtocol)
                        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}|{1}');", currentProtocol.Year, currentProtocol.Number))

                    Case Else
                        Dim uniqueIdProtocol As Guid = Guid.Parse(e.CommandArgument.ToString())
                        RedirectOnPage($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={uniqueIdProtocol}&Type=Prot")}")
                End Select

        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeColumns()
        If Not DocSuiteContext.Current.IsProtocolEnabled Then
            Exit Sub
        End If

        ColumnProtocolRegistrationUserVisible = False
        ColumnHighlightNoteVisible = False
        ColumnHighlightRegistrationUserVisible = False
        ColumnProtocolSectionalVisible = False
        ColumnInvoiceNumberVisible = False
        ColumnAccountingNumberVisible = False
        ColumnInvoiceYearVisible = False

        If Not DocSuiteContext.Current.ProtocolEnv.IsLogStatusEnabled Then
            ColumnHasReadVisible = False
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsSearchContactEnabled Then
            ColumnProtocolContactVisible = False
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
            ColumnProtocolStatusVisible = False
        End If

        Dim col As GridColumn = grdProtocols.Columns.FindByUniqueName(ingoingPecColumnName)
        If Not DocSuiteContext.Current.ProtocolEnv.IsPECEnabled Then
            col.Visible = False
        Else
            col.Visible = True
            col.HeaderImageUrl = ImagePath.SmallDocumentSignature
        End If

        If DocSuiteContext.Current.ProtocolEnv.SimplifiedProtocolGridResultEnabled AndAlso
            (BasePage.Action.Eq("CopyProtocolDocuments") OrElse BasePage.Action.Eq("Fasc") OrElse BasePage.Action.Eq("Resl")) Then
            ColumnHasReadVisible = False
            ColumnIngoingPecVisible = False
            ColumnViewLinksVisible = False
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsInvoiceDataGridResultEnabled Then
            ColumnProtocolSectionalVisible = True
            ColumnInvoiceNumberVisible = True
            ColumnAccountingNumberVisible = True
            ColumnInvoiceYearVisible = True
        End If

        If DocSuiteContext.Current.ProtocolEnv.ProtocolGridOrderColumns IsNot Nothing Then
            Dim protocolGridOrder As IDictionary(Of String, Integer) = ProtocolEnv.ProtocolGridOrderColumns
            For Each colx As Telerik.Web.UI.GridColumn In grdProtocols.MasterTableView.RenderColumns
                If protocolGridOrder.Keys.Contains(colx.UniqueName) Then
                    colx.OrderIndex = protocolGridOrder.Item(colx.UniqueName)
                End If
            Next
            grdProtocols.Rebind()
            _protocolContactLabelsDictionary = Nothing
            _protocolRightsDictionary = Nothing
            _gridDataSource = Nothing
        End If
    End Sub

    Private Sub InitializeFilterColumns()
        If ColumnProtocolTypeShortDescriptionVisible Then
            With DirectCast(grdProtocols.Columns.FindByUniqueNameSafe(COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION), SuggestFilteringColumn)
                .DataSourceCombo = Facade.ProtocolTypeFacade.GetAllSearch()
                .DataTextCombo = "ShortDescription"
                .DataFieldCombo = "Id"
                .DataType = GetType(Integer)
            End With
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled AndAlso ColumnProtocolStatusVisible Then
            With DirectCast(grdProtocols.Columns.FindByUniqueNameSafe(COLUMN_PROTOCOL_STATUS), SuggestFilteringTemplateColumn)
                .DataSourceCombo = Facade.ProtocolStatusFacade.GetAll()
                .DataTextCombo = "Description"
                .DataFieldCombo = "Description"
                .DataType = GetType(String)
            End With
        End If

        Dim columnCategory As CompositeTemplateColumnSqlExpression = TryCast(grdProtocols.Columns.FindByUniqueNameSafe(COLUMN_CATEGORY_NAME), CompositeTemplateColumnSqlExpression)
        If columnCategory IsNot Nothing Then
            columnCategory.BindingType = CompositeTemplateColumnSqlExpression.ColumnBinding.CustomBinding
            columnCategory.CustomExpressionDelegate = New CompositeTemplateColumnSqlExpression.SqlExpressionDelegate(AddressOf Facade.CategoryFacade.CategoryNameFullCodeFilter)
        End If


    End Sub

    Private Function getProtocolObject(header As ProtocolHeader) As String
        Dim protocolObject As String = String.Empty
        If Not String.IsNullOrEmpty(header.Protocol) Then
            protocolObject = header.ProtocolObject
        End If
        Dim privacyObject As String = DocSuiteContext.Current.ProtocolEnv.SecurityProtocolObject

        If String.IsNullOrEmpty(privacyObject) Then
            Return protocolObject
        Else
            Dim pr As ProtocolRights = protocolRightsDictionary(header.UniqueId)
            If pr IsNot Nothing AndAlso pr.IsPreviewable Then
                Return protocolObject
            End If
            Return privacyObject
        End If
    End Function

    Private Function getHeaderImageUrl(header As ProtocolHeader, discriminator As String) As String
        Select Case discriminator
            Case "ViewDocuments"
                Dim icon As String
                If header.IdStatus = ProtocolStatusId.Incompleto Then
                    icon = ImagePath.SmallEmpty
                ElseIf String.IsNullOrWhiteSpace(header.DocumentCode) OrElse header.IdDocument.GetValueOrDefault(0) = 0 Then
                    icon = ImagePath.SmallPageError
                ElseIf ProtocolEnv.SignedIconRenderingModality.ProtocolIconModality = IconModality.OriginalIcon Then
                    icon = ImagePath.FromFileNoSignature(header.DocumentCode, True)
                Else
                    icon = ImagePath.FromFile(header.DocumentCode, True)
                End If
                Return icon

            Case "ProtocolType"
                If header.IdStatus.HasValue AndAlso header.IdStatus = ProtocolStatusId.Annullato Then
                    Return "~/comm/images/remove16.gif"
                End If
                Select Case header.Type.Id
                    Case -1
                        Return "~/App_Themes/DocSuite2008/imgset16/ingoing.png"
                    Case 0
                        Return "~/App_Themes/DocSuite2008/imgset16/inout.png"
                    Case 1
                        Return "~/App_Themes/DocSuite2008/imgset16/outgoing.png"
                    Case Else
                        Return String.Empty
                End Select

            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function getHeaderPostBackUrl(header As ProtocolHeader, discriminator As String) As String
        Select Case discriminator
            Case "ViewProtocol"
                If BasePage.Action.Eq("Fasc") OrElse BasePage.Action.Eq("Resl") Then
                    Return String.Empty
                End If
                Dim parameters As String = $"UniqueId={header.UniqueId}&Type=Prot"
                Return $"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck(parameters)}"

            Case "ViewDocuments"
                ' Verifico permessi di lettura per la visualizzazione selezionata.
                Dim protocolRights As ProtocolRights = protocolRightsDictionary.Item(header.UniqueId)
                If Not protocolRights.IsDocumentReadable Then
                    Return String.Empty
                End If

                Dim parameters As String = $"UniqueId={header.UniqueId}&Type=Prot"
                Return $"~/Viewers/ProtocolViewer.aspx?{CommonShared.AppendSecurityCheck(parameters)}"

            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function getHeaderClientScript(header As ProtocolHeader, discriminator As String) As String
        Select Case discriminator
            Case "ViewDocuments"
                Dim protocolRights As ProtocolRights = protocolRightsDictionary.Item(header.UniqueId)
                If Not protocolRights.IsDocumentReadable Then
                    Dim script As String = "alert('Protocollo: {0}\nDiritti insufficienti per la visualizzazione del documento.'); return false;"
                    script = String.Format(script, header.FullProtocolNumber)
                    Return script
                End If
                Return String.Empty

        End Select
        Return String.Empty
    End Function

    Private Function getTextByHeader(header As ProtocolHeader, discriminator As String) As String
        Select Case discriminator
            Case COLUMN_PROTOCOL_CONTACT
                If protocolContactLabelsDictionary IsNot Nothing AndAlso protocolContactLabelsDictionary.Keys IsNot Nothing _
                                       AndAlso header IsNot Nothing AndAlso protocolContactLabelsDictionary.Keys.Contains(header.UniqueId) Then
                    Return protocolContactLabelsDictionary.Item(header.UniqueId).Replace("|"c, " ")
                End If
                Return "MANCANTE!"
            Case COLUMN_CATEGORY_NAME
                Return Facade.ProtocolFacade.GetCategoryString(header)
        End Select
        Return String.Empty
    End Function

    Public Sub RemoveItemCommandHandler()
        RemoveHandler grdProtocols.ItemCommand, AddressOf grdProtocols_ItemCommand
    End Sub

    Public Sub DisableColumn(ByVal columnUniqueName As String)
        Dim column As GridColumn = grdProtocols.Columns.FindByUniqueNameSafe(columnUniqueName)
        If column Is Nothing Then
            Exit Sub
        End If

        Try
            column.Visible = False
            Select Case TypeName(column)
                Case "GridTemplateColumn"
                    DirectCast(column, GridTemplateColumn).ItemTemplate = Nothing
                Case "GridBoundColumn"
                    DirectCast(column, GridBoundColumn).DataField = String.Empty
            End Select

        Catch ex As Exception
            Throw New GridBindingException(String.Format("Non è stato possibile disabilitare la colonna [{0}].", column.UniqueName))
        End Try
    End Sub

    Sub EnableColumn(ByVal columnUniqueName As String)
        Dim column As GridColumn = grdProtocols.Columns.FindByUniqueNameSafe(columnUniqueName)
        If column Is Nothing Then
            Exit Sub
        End If

        column.Visible = True
    End Sub

    Public Sub SetDataField(ByVal columnUniqueName As String, ByVal dataField As String)
        Dim column As GridColumn = grdProtocols.Columns.FindByUniqueNameSafe(columnUniqueName)
        If column IsNot Nothing AndAlso TypeName(column).Equals("GridBoundColumn") Then
            DirectCast(column, GridBoundColumn).DataField = dataField
        End If
    End Sub

#End Region

End Class