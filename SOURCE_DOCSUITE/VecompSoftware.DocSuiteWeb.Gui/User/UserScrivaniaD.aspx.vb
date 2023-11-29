Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class UserScrivaniaD
    Inherits UserBasePage

#Region " Fields "

#Region "Actions Protocols"
    Public Const ACTION_NAME_PROTOCOL_TO_READ As String = "PL"
    Public Const ACTION_NAME_PROTOCOL_INVOICE_TO_READ As String = "IPL"
    Public Const ACTION_NAME_PROTOCOL_TO_DISTRIBUTE As String = "PD"
    Public Const ACTION_NAME_PROTOCOL_TO_WORK As String = "PDL"
    Public Const ACTION_NAME_PROTOCOL_IN_ASSIGNMENT As String = "PA"
    Public Const ACTION_NAME_PROTOCOL_RECENT As String = "PR"
    Public Const ACTION_NAME_PROTOCOL_ASSIGNED As String = "PV"
    Public Const ACTION_NAME_PROTOCOL_IN_EVIDENCE As String = "PE"
    Public Const ACTION_NAME_PROTOCOL_UPDATED As String = "PU"
    Public Const ACTION_NAME_PROTOCOL_INSERT_TODAY As String = "PO"
    Public Const ACTION_NAME_PROTOCOL_INSERT_THIS_WEEK As String = "PS"
    Public Const ACTION_NAME_PROTOCOL_INSERT_THIS_MONTH As String = "PM"
    Public Const ACTION_NAME_PROTOCOL_NEVER_DISTRIBUTED As String = "PND"
#End Region

#Region "Actions Documents"
    Public Const ACTION_NAME_DOCUMENT_TO_READ As String = "DL"
    Public Const ACTION_NAME_DOCUMENT_OPEN_TO_READ As String = "DAL"
    Public Const ACTION_NAME_DOCUMENT_CHECKOUT As String = "DK"
    Public Const ACTION_NAME_DOCUMENT_TAKE_IN_CHARGE As String = "DR"
    Public Const ACTION_NAME_DOCUMENT_INSERT_TODAY As String = "DO"
    Public Const ACTION_NAME_DOCUMENT_INSERT_THIS_WEEK As String = "DS"
    Public Const ACTION_NAME_DOCUMENT_INSERT_THIS_MONTH As String = "DM"
    Public Const ACTION_NAME_DOCUMENT_TIMETABLE As String = "DE"
#End Region

#Region "Actions Resolutions"
    Public Const ACTION_NAME_RESOLUTION_NOT_COMPLIANT As String = "RNC"
    Public Const ACTION_NAME_RESOLUTION_CHECK_COMPLIANT As String = "RICC"
    Public Const ACTION_NAME_RESOLUTION_COMPLIANT_TO_ADOPTION As String = "RCDA"
    Public Const ACTION_NAME_RESOLUTION_PROPOSED As String = "RPO"
    Public Const ACTION_NAME_RESOLUTION_ADOPTED As String = "RA"
    Public Const ACTION_NAME_RESOLUTION_ACCOUNTING As String = "RVRP"
    Public Const ACTION_NAME_RESOLUTION_PUBLISHED As String = "RP"
    Public Const ACTION_NAME_RESOLUTION_EXECUTIVE As String = "RE"
    Public Const ACTION_NAME_RESOLUTION_VERIFY As String = "RAV"
    Public Const ACTION_NAME_RESOLUTION_AFF_GEN As String = "ZAFF"
    Public Const ACTION_NAME_RESOLUTION_AMM_TRASP As String = "Z"
    Public Const ACTION_NAME_RESOLUTION_INSERT_TODAY As String = "RO"
    Public Const ACTION_NAME_RESOLUTION_INSERT_THIS_WEEK As String = "RS"
    Public Const ACTION_NAME_RESOLUTION_INSERT_THIS_MONTH As String = "RM"
    Public Const ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL As String = "ROCR"
    Public Const ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED As String = "ROCP"
#End Region

    Public Const MultiDistribuzioneSessionName As String = "ProtMultiDistribuzione_ProtocolKeys"
    Private Const CLOSE_DISTRIBUTIONREJECTWINDOW_FUNCTION_NAME As String = "closeDistributionRejectNoteWindow()"

    Dim _titolo As String = String.Empty
    Dim _records As Integer = 0
    Dim _protocolFinder As NHibernateProtocolFinder
    Dim _documentFinder As NHibernateDocumentFinder
    Dim _resolutionFinder As NHibernateResolutionFinder


    Private _currentManageableRoleIds As ICollection(Of Integer)
    Private _currentManageableRoles As IList(Of Role)
    Private _currentManageableRoleId As Integer?
    Private _currentManageableRole As Role
    Private _isManageableRole As Boolean?
    Private _isManageableSubRole As Boolean?
    Private _currentUser As String
#End Region

#Region " Properties "
    Private ReadOnly Property CurrentManageableRoleIds As ICollection(Of Integer)
        Get
            If (_currentManageableRoleIds Is Nothing) Then
                Dim manageableRoleIds As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Manager, True, CurrentTenant.TenantAOO.UniqueId)
                _currentManageableRoleIds = manageableRoleIds.Select(Function(r) r.Id).ToList()
            End If

            Return _currentManageableRoleIds
        End Get
    End Property

    Private ReadOnly Property CurrentManageableRoles As IList(Of Role)
        Get
            If _currentManageableRoles Is Nothing Then
                _currentManageableRoles = Facade.RoleFacade.GetByIds(CurrentManageableRoleIds)
            End If
            Return _currentManageableRoles
        End Get
    End Property

    Private ReadOnly Property CurrentManageableRoleId As Integer?
        Get
            If Not _currentManageableRoleId.HasValue AndAlso Not String.IsNullOrEmpty(CommonUtil.GroupProtocolManagerSelected) Then
                If CommonUtil.GroupProtocolManagerSelected.Replace("|", String.Empty).Split(","c).Length = 1 Then
                    _currentManageableRoleId = CInt(CommonUtil.GroupProtocolManagerSelected.Replace("|", String.Empty))
                End If
            End If
            Return _currentManageableRoleId
        End Get
    End Property

    Private ReadOnly Property CurrentManageableRole As Role
        Get
            If _currentManageableRole Is Nothing Then
                _currentManageableRole = Facade.RoleFacade.GetById(CurrentManageableRoleId.Value)
            End If
            Return _currentManageableRole
        End Get
    End Property

    Private ReadOnly Property IsManageableRole() As Boolean
        Get
            If Not _isManageableRole.HasValue Then
                _isManageableRole = False
                For Each idRole As Integer In CurrentManageableRoleIds
                    If Not CurrentManageableRoleId.Equals(idRole) Then
                        Continue For
                    End If

                    _isManageableRole = True
                    Exit For
                Next
            End If
            Return _isManageableRole.Value
        End Get
    End Property

    Private ReadOnly Property IsManageableSubRole() As Boolean
        Get
            If Not _isManageableSubRole.HasValue Then
                _isManageableSubRole = False
                For Each r As Role In CurrentManageableRoles
                    If Not CurrentManageableRole.FullIncrementalPath.Contains(r.FullIncrementalPath) Then
                        Continue For
                    End If

                    _isManageableSubRole = True
                    Exit For
                Next
            End If
            Return _isManageableSubRole.Value
        End Get
    End Property

    Private Property RbCCSelectedValue As String
        Get
            Return CType(Session("RbCCSelectedValue"), String)
        End Get
        Set(value As String)
            Session("RbCCSelectedValue") = value
        End Set
    End Property

    Private Property ScrivaniaDaData As DateTime?
        Get
            Return CType(Session("ScrivaniaDaData"), DateTime?)
        End Get
        Set(value As DateTime?)
            Session("ScrivaniaDaData") = value
        End Set
    End Property

    Private Property ScrivaniaProtocolContainer As String
        Get
            Return CType(Session("ScrivaniaddlProtContainer"), String)
        End Get
        Set(value As String)
            Session("ScrivaniaddlProtContainer") = value
        End Set
    End Property

    Private Property ScrivaniaDocumentContainer As String
        Get
            Return CType(Session("ScrivaniaddlDocmContainer"), String)
        End Get
        Set(value As String)
            Session("ScrivaniaddlDocmContainer") = value
        End Set
    End Property

    Private Property ScrivaniaResolutionContainer As String
        Get
            Return CType(Session("ScrivaniaddlReslContainer"), String)
        End Get
        Set(value As String)
            Session("ScrivaniaddlReslContainer") = value
        End Set
    End Property

    Private ReadOnly Property CurrentUser As String
        Get
            _currentUser = DocSuiteContext.Current.User.FullUserName
            Return _currentUser
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        _titolo = Request.QueryString("Title")

        Loading()
        If Not IsPostBack Then
            Select Case Action
                Case ACTION_NAME_PROTOCOL_TO_READ, ACTION_NAME_PROTOCOL_INVOICE_TO_READ
                    rdpDateFilterFrom.SelectedDate = DateTime.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
                    ' Setto le ore 23:59:59
                    rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
                    If ProtocolEnv.ToReadProtocolTypeFinderEnabled Then
                        rowProtocolType.Visible = True
                        ddlProtocolTypes.Items.Add(New ListItem("Tutti", "All"))
                        ddlProtocolTypes.Items.Add(New ListItem("Ingresso", "-1"))
                        ddlProtocolTypes.Items.Add(New ListItem("Uscita", "1"))
                        If ProtocolEnv.IsInterOfficeEnabled Then
                            ddlProtocolTypes.Items.Add(New ListItem("Tra uffici", "0"))
                        End If
                        ddlProtocolTypes.SelectedValue = "All"
                    End If
                    rbCC.Items.Insert(0, New ListItem("Tutti", "All"))
                    rbCC.Items.Insert(1, New ListItem("Solo autorizzati (competenza)", "APC"))       'solo autorizzati per competenza
                    rbCC.Items.Insert(2, New ListItem("Solo autorizzati (CC)", "ACC"))  'solo autorizzati in copia conoscenza
                    rbCC.SelectedIndex = ProtocolEnv.ProtocolDistributionViewDefaultFilter
                    RbCCSelectedValue = rbCC.Items(ProtocolEnv.ProtocolDistributionViewDefaultFilter).Value
                Case ACTION_NAME_PROTOCOL_RECENT
                    rdpCalendar.SelectedDate = DateAdd(DateInterval.Month, -1, Date.Today)
                Case ACTION_NAME_PROTOCOL_IN_EVIDENCE
                    rdpDateFilterFrom.SelectedDate = Date.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
                    ' Setto le ore 23:59:59
                    rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
                    rbCC.Items.Add(New ListItem("Solo autorizzati (competenza)", "APC"))       'solo autorizzati per competenza
                    rbCC.Items.Add(New ListItem("Solo autorizzati (CC)", "ACC"))  'solo autorizzati in copia conoscenza
                Case ACTION_NAME_PROTOCOL_TO_WORK
                    rdpDateFilterFrom.SelectedDate = DateTime.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
                    ' Setto le ore 23:59:59
                    rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
                    rbCC.Items.Add(New ListItem("Solo autorizzati (competenza)", "APC"))       'solo autorizzati per competenza
                    rbCC.Items.Add(New ListItem("Solo autorizzati (CC)", "ACC"))  'solo autorizzati in copia conoscenza
                Case ACTION_NAME_PROTOCOL_TO_DISTRIBUTE
                    rdpDateFilterFrom.SelectedDate = DateTime.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
                    ' Setto le ore 23:59:59
                    rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
                    rbCC.Items.Insert(0, New ListItem("Tutti", "All"))
                    rbCC.Items.Insert(1, New ListItem("Solo autorizzati (competenza)", "APC"))       'solo autorizzati per competenza
                    rbCC.Items.Insert(2, New ListItem("Solo autorizzati (CC)", "ACC"))  'solo autorizzati in copia conoscenza
                    rbCC.SelectedIndex = ProtocolEnv.ProtocolDistributionViewDefaultFilter
                    RbCCSelectedValue = rbCC.Items(ProtocolEnv.ProtocolDistributionViewDefaultFilter).Value
                Case ACTION_NAME_PROTOCOL_IN_ASSIGNMENT, ACTION_NAME_PROTOCOL_UPDATED, ACTION_NAME_PROTOCOL_NEVER_DISTRIBUTED
                    rdpDateFilterFrom.SelectedDate = DateTime.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
                    ' Setto le ore 23:59:59
                    rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
                Case ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL, ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED
                    rdpDateFilterFrom.SelectedDate = DateTime.Today.AddDays(-ResolutionEnv.DesktopDayDiff).Date
                    ' Setto le ore 23:59:59
                    rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
                Case Else
                    rdpCalendar.SelectedDate = DateAdd(DateInterval.Day, 30, Date.Today)
            End Select
            InitializeContainer()
            If Action.Eq(UserDesktop.RESOLUTION_PROPOSED_BY_ROLE) Then
                InitializeProposedByRole()
            End If
            ' Valore default o ultimo valore utilizzato
            If Not String.IsNullOrEmpty(RbCCSelectedValue) Then
                rbCC.SelectedValue = RbCCSelectedValue
            Else
                rbCC.SelectedValue = "PC" 'Per Competenza
            End If

            ' Verifico se è stata usata una data
            If ScrivaniaDaData.HasValue Then
                Dim d As DateTime? = ScrivaniaDaData
                If d.HasValue Then
                    rdpCalendar.SelectedDate = d
                End If
            End If

            InitializeDataBind(CurrentTenant.TenantAOO.UniqueId)
            InitializeManageableSubRoleAlert()
        End If
    End Sub

    Private Sub cmdMultiAutorizza_Click(sender As Object, e As EventArgs) Handles cmdMultiAutorizza.Click
        Dim selectedProtocolKeys As String = String.Empty
        For Each gridItem As GridDataItem In uscProtocolGrid.Grid.Items
            Dim chkSelezione As CheckBox = CType(gridItem("colClientSelect").Controls(1), CheckBox)
            If (chkSelezione Is Nothing) OrElse Not chkSelezione.Checked Then
                Continue For
            End If

            Dim lbtViewProtocol As LinkButton = CType(gridItem.FindControl("lbtViewProtocol"), LinkButton)
            If lbtViewProtocol IsNot Nothing Then
                If Not selectedProtocolKeys.IsNullOrEmpty() Then
                    selectedProtocolKeys = $"{selectedProtocolKeys}|"
                End If
                selectedProtocolKeys = $"{selectedProtocolKeys}{lbtViewProtocol.CommandArgument}"
            End If
        Next

        If String.IsNullOrEmpty(selectedProtocolKeys) Then
            AjaxAlert("Selezionare i Protocolli per l'Autorizzazione")
            Exit Sub
        ElseIf Not CurrentManageableRoleId.HasValue AndAlso Not CurrentManageableRoleIds.Count = 1 Then
            AjaxAlert("E' necessario abilitare un solo settore da Utente>Settori abilitati.")
            Exit Sub
        End If

        Session("ProtMultiAutorizza_ProtocolList") = selectedProtocolKeys

        Response.Redirect("../Prot/ProtMultiAutorizza.aspx")
    End Sub

    Private Sub btnSegnaLetti_Click(ByVal sender As Object, ByVal e As SelectedEventArgs)
        If e.Selected Then
            InitializeDataBind(CurrentTenant.TenantAOO.UniqueId)
            Exit Sub
        End If
        If Not String.IsNullOrEmpty(e.ErrorMessage) Then
            AjaxAlert(e.ErrorMessage)
            Exit Sub
        End If
        Select Case Action
            Case ACTION_NAME_DOCUMENT_TO_READ
                AjaxAlert("Selezionare le Pratiche")
            Case ACTION_NAME_PROTOCOL_TO_READ
                AjaxAlert("Selezionare i Protocolli")
        End Select
    End Sub

    Private Sub btnRemoveHighlight_Click(sender As Object, e As EventArgs) Handles btnRemoveHighlight.Click
        Dim selectedItems As IList(Of GridDataItem) = New List(Of GridDataItem)
        For Each item As GridDataItem In uscProtocolGrid.Grid.SelectedItems
            Dim id As Guid = GetProtocolUniqueId(item).Value
            Facade.ProtocolUserFacade.RemoveHighlightUser(Facade.ProtocolFacade.GetById(id), DocSuiteContext.Current.User.FullUserName)
        Next

        If uscProtocolGrid.Grid.SelectedItems.Count = 0 Then
            AjaxAlert("Nessun protocollo selezionato.")
            Exit Sub
        Else
            BindProtocol(CurrentTenant.TenantAOO.UniqueId)
        End If
    End Sub

    Private Sub ddlProtContainer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlProtContainer.SelectedIndexChanged
        ScrivaniaProtocolContainer = ddlProtContainer.SelectedValue
        BindProtocol(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub ddlDocmContainer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles ddlDocmContainer.SelectedIndexChanged
        ScrivaniaDocumentContainer = ddlDocmContainer.SelectedValue
        BindDocument(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub ddlProtocolTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlProtocolTypes.SelectedIndexChanged
        BindProtocol(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub ddlReslContainer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlReslContainer.SelectedIndexChanged
        ScrivaniaResolutionContainer = ddlReslContainer.SelectedValue
        BindResolution(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub RoleProposerAction_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblResolutionTypes.SelectedIndexChanged, ddlRoles.SelectedIndexChanged
        BindResolution(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub rbCC_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rbCC.SelectedIndexChanged
        RbCCSelectedValue = rbCC.SelectedValue
        BindProtocol(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub RdpCalendarOnSelected(sender As Object, e As EventArgs) Handles rdpCalendar.SelectedDateChanged
        ScrivaniaDaData = rdpCalendar.SelectedDate
        InitializeDataBind(CurrentTenant.TenantAOO.UniqueId)
    End Sub
    Private Sub ChkChargeToMe_CheckedChanged(sender As Object, e As EventArgs) Handles chkChargeToMe.CheckedChanged
        BindResolution(CurrentTenant.TenantAOO.UniqueId)
    End Sub
    Private Sub cmdMultiDistribuzione_Click(sender As Object, e As EventArgs) Handles cmdMultiDistribuzione.Click
        Dim selectedKeys As List(Of Guid) = uscProtocolGrid.Grid.Items.Cast(Of GridDataItem)() _
                                                            .Where(Function(i) Me.GetChecked(i)) _
                                                            .Select(Function(i) Me.GetProtocolUniqueId(i)) _
                                                            .Where(Function(k) k.HasValue) _
                                                            .Select(Function(i) i.Value) _
                                                            .ToList()

        If selectedKeys.IsNullOrEmpty() Then
            Me.AjaxAlert("Nessun protocollo valido per la distribuzione selezionato.")
            Return
        End If

        Me.Session(UserScrivaniaD.MultiDistribuzioneSessionName) = selectedKeys
        Dim first As Guid = selectedKeys.First()
        Me.Response.Redirect($"../Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={first}&Type=Prot")}")
    End Sub

    Private Function GetChecked(item As GridDataItem) As Boolean
        Dim chk As CheckBox = CType(item("colClientSelect").Controls(1), CheckBox)
        Return chk IsNot Nothing AndAlso chk.Checked
    End Function
    Private Function GetProtocolUniqueId(item As GridDataItem) As Guid?
        Dim hf_protocol_unique As HiddenField = CType(item.FindControl("hf_protocol_unique"), HiddenField)
        Dim uniqueId As Guid = Guid.Empty
        If hf_protocol_unique Is Nothing OrElse Not Guid.TryParse(hf_protocol_unique.Value, uniqueId) OrElse uniqueId = Guid.Empty Then
            Return Nothing
        End If
        Return uniqueId
    End Function

    Private Sub rdpDateFilter_SelectedDateChanged(sender As Object, e As EventArgs) Handles rdpDateFilterFrom.SelectedDateChanged, rdpDateFilterTo.SelectedDateChanged
        Validations()

        Select Case Action
            Case UserDesktop.RESOLUTION_PROPOSED_BY_ROLE, ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL, ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED
                BindResolution(CurrentTenant.TenantAOO.UniqueId)
                Exit Sub
        End Select

        BindProtocol(CurrentTenant.TenantAOO.UniqueId)
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        Dim uniqueId As Guid?
        Dim checked As Boolean
        Dim account As String = DocSuiteContext.Current.User.FullUserName.ToLower()
        For Each myGridItem As GridDataItem In uscProtocolGrid.Grid.Items
            uniqueId = GetProtocolUniqueId(myGridItem)
            checked = GetChecked(myGridItem)
            If (checked OrElse myGridItem.Selected) AndAlso uniqueId.HasValue Then
                Dim protocolRoleUsers As List(Of ProtocolRoleUser) = Facade.ProtocolRoleUserFacade.GetByProtocolIdAndAccount(uniqueId.Value, account).ToList()
                For Each protocolRoleUser As ProtocolRoleUser In protocolRoleUsers
                    protocolRoleUser.Status = 1
                    Facade.ProtocolRoleUserFacade.Update(protocolRoleUser)
                Next
            End If
        Next
        uscProtocolGrid.Grid.Rebind() 'Refresh grid
    End Sub

    Private Sub btnDistributionRejectConfirm_Click(sender As Object, e As EventArgs) Handles btnDistributionRejectConfirm.Click
        Dim protocolUniqueIds As List(Of Guid) = uscProtocolGrid.Grid.Items.Cast(Of GridDataItem)() _
                                                            .Where(Function(i) Me.GetChecked(i)) _
                                                            .Select(Function(i) Me.GetProtocolUniqueId(i)) _
                                                            .Where(Function(k) k.HasValue) _
                                                            .Select(Function(i) i.Value) _
                                                            .ToList()

        If protocolUniqueIds.IsNullOrEmpty() Then
            AjaxAlert("Selezionare almeno un protocollo per l'attività di rifiuto.")
            Return
        End If

        If (String.IsNullOrEmpty(txtDistributionRejectNote.Text)) Then
            AjaxAlert("Le note sono obbligatorie per l'attività di rifiuto.")
            Return
        End If

        Dim roleIdsToReject As ICollection(Of Integer) = New List(Of Integer)
        If (CurrentManageableRoleId.HasValue) Then
            roleIdsToReject.Add(CurrentManageableRoleId.Value)
        End If

        If roleIdsToReject.Count = 0 Then
            roleIdsToReject.Add(CurrentManageableRoleIds.First())
        End If

        Dim errorBuilder As StringBuilder = New StringBuilder()
        Dim protocol As Protocol = Nothing
        Dim protocolRole As ProtocolRole
        For Each protocolUniqueId As Guid In protocolUniqueIds
            Try
                protocol = Nothing
                protocol = Facade.ProtocolFacade.GetById(protocolUniqueId)
                protocolRole = protocol.Roles.SingleOrDefault(Function(s) s.Role.Id.Equals(roleIdsToReject.First()))
                protocolRole.Note = txtDistributionRejectNote.Text
                Facade.ProtocolRejectedRoleFacade.RejectProtocols(protocol, roleIdsToReject)
                Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.AR, $"Prot.{protocol.FullNumber} - Settore rifiutato {protocolRole.Role.Id} ({protocolRole.Role.Name}) con Note: {txtDistributionRejectNote.Text}")
            Catch ex As Exception
                FileLogger.Error(LoggerName, $"Errore nel rifiuto del protocollo [{protocolUniqueId}]: {ex.Message}", ex)
                If protocol IsNot Nothing Then
                    errorBuilder.AppendLine($"E' avvenuto un errore nel rifiuto del protocollo [{protocol.FullNumber}]")
                Else
                    errorBuilder.AppendLine($"E' avvenuto un errore nella lettura del protocollo con id [{protocolUniqueId}]")
                End If
            End Try
        Next
        If (Not String.IsNullOrEmpty(errorBuilder.ToString())) Then
            AjaxAlert(errorBuilder.ToString())
        End If
        BindProtocol(CurrentTenant.TenantAOO.UniqueId)
        AjaxManager.ResponseScripts.Add(CLOSE_DISTRIBUTIONREJECTWINDOW_FUNCTION_NAME)
    End Sub

#End Region

#Region " Methods "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Validations() As Boolean
        If Not rdpDateFilterFrom.SelectedDate.HasValue Then
            AjaxAlert("Manca data Inizio")
            Return False
        End If
        If Not rdpDateFilterTo.SelectedDate.HasValue Then
            AjaxAlert("Manca data Fine")
            Return False
        End If
        Return True
    End Function

    ''' <summary>L'orrore continua</summary>
    ''' <remarks>L'IF e l'ELSE IF COSA VOGLIONO SIGNIFICARE??</remarks>
    Private Sub InitializeManageableSubRoleAlert()
        If CurrentManageableRoleId IsNot Nothing AndAlso Not IsManageableRole() AndAlso IsManageableSubRole() Then
            pnlAlert.Visible = True
            lblAlert.Text = String.Format("ATTENZIONE! La visualizzazione dei seguenti protocolli è relativa al settore: {0}.", CurrentManageableRole.Name)
        ElseIf CurrentManageableRoleId IsNot Nothing AndAlso Not IsManageableRole() AndAlso Not IsManageableSubRole() Then
            Throw New DocSuiteException("Permessi Scrivania", "Non si hanno i permessi necessari per la gestione del settore abilitato corrente.")
        End If
    End Sub

    Private Sub InitializeDataBind(idTenantAOO As Guid)
        Select Case Type
            Case "Prot"
                BindProtocol(idTenantAOO)
            Case "Docm"
                BindDocument(idTenantAOO)
            Case "Resl"
                BindResolution(idTenantAOO)
        End Select
    End Sub

    Private Sub Loading()
        MasterDocSuite.TitleVisible = False

        Select Case Type
            Case "Prot"
                pnlProtocollo.Visible = True
                lblHeader.Text = String.Format("Protocollo - {0}", _titolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlProtContainer, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlProtocolTypes, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpCalendar, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(rbCC, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFilterFrom, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFilterTo, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(btnRemoveHighlight, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(btnDistributionRejectConfirm, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)

                AjaxManager.AjaxSettings.AddAjaxSetting(rbCC, rbCC)

                AjaxManager.AjaxSettings.AddAjaxSetting(rbCC, divTitolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpCalendar, divTitolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlProtContainer, divTitolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlProtocolTypes, divTitolo)

                Select Case Action
                    Case ACTION_NAME_PROTOCOL_TO_READ, ACTION_NAME_PROTOCOL_INVOICE_TO_READ
                        SetPanelButtonBar()
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                    Case ACTION_NAME_PROTOCOL_IN_ASSIGNMENT
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                    Case ACTION_NAME_PROTOCOL_ASSIGNED
                        pnlButtonBar.Visible = True
                        uscProtocolGridBar.Grid = uscProtocolGrid.Grid
                        uscProtocolGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
                        uscDocumentGridBar.AjaxEnabled = True
                        uscProtocolGridBar.Visible = True
                        uscProtocolGridBar.SelectButton.Visible = True
                        uscProtocolGridBar.DeselectButton.Visible = True
                        uscProtocolGridBar.ShowDocumentButton()
                    Case ACTION_NAME_PROTOCOL_RECENT
                        pnlCalendar.Visible = True
                        lblCalendar.Text = "Dalla data: "
                    Case ACTION_NAME_PROTOCOL_TO_DISTRIBUTE
                        pnlButtonBar.Visible = True
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                        uscProtocolGridBar.Grid = uscProtocolGrid.Grid
                        uscProtocolGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
                        uscDocumentGridBar.AjaxEnabled = True
                        uscProtocolGridBar.Visible = True
                        uscProtocolGridBar.SelectButton.Visible = True
                        uscProtocolGridBar.DeselectButton.Visible = True
                        uscProtocolGridBar.SetReadButton.Visible = False
                        uscProtocolGridBar.ShowDocumentButton()
                    Case ACTION_NAME_PROTOCOL_IN_EVIDENCE
                        uscProtocolGrid.ColumnSelectionVisible = True
                        uscProtocolGrid.ColumnClientSelectVisible = False
                        uscProtocolGrid.ColumnHighlightNoteVisible = True
                        uscProtocolGrid.ColumnHighlightRegistrationUserVisible = True
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                        pnlHighlight.Visible = True
                    Case ACTION_NAME_PROTOCOL_TO_WORK
                        uscProtocolGridBar.SelectButton.Visible = True
                        uscProtocolGridBar.DeselectButton.Visible = True
                        pnlUpdate.Visible = True
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                        uscProtocolGridBar.SelectButton.Visible = True
                        uscProtocolGridBar.DeselectButton.Visible = True
                        uscProtocolGridBar.ShowDocumentButton()
                    Case ACTION_NAME_PROTOCOL_UPDATED
                        uscProtocolGridBar.SelectButton.Visible = False
                        uscProtocolGridBar.DeselectButton.Visible = False
                        uscProtocolGrid.ColumnClientSelectVisible = False
                        pnlUpdate.Visible = True
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                    Case UserDesktop.ActionNameProtocolliRigettati
                        uscProtocolGrid.DisableColumn(uscProtGrid.COLUMN_CLIENT_SELECT)
                    Case ACTION_NAME_PROTOCOL_NEVER_DISTRIBUTED
                        uscProtocolGridBar.SelectButton.Visible = False
                        uscProtocolGridBar.DeselectButton.Visible = False
                        uscProtocolGrid.ColumnClientSelectVisible = False
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                End Select

                uscProtocolGrid.RedirectOnParentPage = Not ProtocolEnv.MoveScrivaniaMenu
                _protocolFinder = Facade.ProtocolFinder

            Case "Docm"
                pnlPratiche.Visible = True
                lblHeader.Text = String.Format("Pratiche - {0}", _titolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscDocmGrid.Grid, uscDocmGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocmContainer, uscDocmGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocmContainer, divTitolo)

                Select Case Action
                    Case ACTION_NAME_DOCUMENT_TO_READ
                        SetPanelButtonBar()
                    Case ACTION_NAME_DOCUMENT_TIMETABLE
                        pnlCalendar.Visible = True
                        lblCalendar.Text = "Data scadenza: "
                        AjaxManager.AjaxSettings.AddAjaxSetting(rdpCalendar, uscDocmGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                        AjaxManager.AjaxSettings.AddAjaxSetting(rdpCalendar, lblHeader)
                End Select

                uscDocmGrid.RedirectOnParentPage = Not ProtocolEnv.MoveScrivaniaMenu
                _documentFinder = Facade.DocumentFinder

            Case "Resl"
                pnlAtti.Visible = True
                lblHeader.Text = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, _titolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlReslContainer, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlReslContainer, divTitolo)
                AjaxManager.AjaxSettings.AddAjaxSetting(chkChargeToMe, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)

                If ResolutionEnv.ViewResolutionProposedByRoleEnabled Then
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblResolutionTypes, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                    AjaxManager.AjaxSettings.AddAjaxSetting(ddlRoles, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblResolutionTypes, lblHeader)
                    AjaxManager.AjaxSettings.AddAjaxSetting(ddlRoles, lblHeader)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFilterFrom, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFilterFrom, lblHeader)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFilterTo, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFilterTo, lblHeader)
                End If

                Select Case Action
                    Case ACTION_NAME_RESOLUTION_AFF_GEN
                        pnlChargeToMe.Visible = True
                    Case ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL, ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED
                        pnlDateFilter.Visible = True
                        rdpDateFilterFrom.DateInput.Label = "Da:"
                        rdpDateFilterTo.DateInput.Label = "A:"
                End Select

                uscReslGrid.AutoHideColumns = False
                uscReslGrid.Grid.Columns.FindByUniqueName(uscReslGrid.COLUMN_DOCUMENT_TYPE).HeaderText = "Tipo"
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_CLIENT_SELECT)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_PROPOSER_CODE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_REGIONE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_CONTROLLER_STATUS)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_TIPOC)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_YEAR)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_DOCUMENT_SIGN)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ATTACH_SELECT)

                uscReslGrid.RedirectOnParentPage = Not ProtocolEnv.MoveScrivaniaMenu
                _resolutionFinder = Facade.ResolutionFinder

        End Select
    End Sub

    Private Sub SetPanelButtonBar()
        pnlButtonBar.Visible = True
        Select Case Type
            Case "Prot"
                uscProtocolGridBar.Grid = uscProtocolGrid.Grid
                uscProtocolGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
                uscDocumentGridBar.AjaxEnabled = True
                uscProtocolGridBar.Visible = True
                uscProtocolGridBar.SelectButton.Visible = True
                uscProtocolGridBar.DeselectButton.Visible = True
                uscProtocolGridBar.SetReadButton.Visible = True
                uscProtocolGridBar.ShowDocumentButton()
                AddHandler uscProtocolGridBar.SetRead, AddressOf btnSegnaLetti_Click
            Case "Docm"
                uscDocumentGridBar.Grid = uscDocmGrid.Grid
                uscDocumentGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
                uscDocumentGridBar.AjaxEnabled = True
                uscDocumentGridBar.Visible = True
                uscDocumentGridBar.SelectButton.Visible = True
                uscDocumentGridBar.DeselectButton.Visible = True
                uscDocumentGridBar.SetReadButton.Visible = True
                AddHandler uscDocumentGridBar.SetRead, AddressOf btnSegnaLetti_Click
        End Select
    End Sub

    Private Sub InitializeContainer()
        Dim ddl As RadDropDownList = Nothing
        Dim container As String = String.Empty
        Select Case Type
            Case "Prot"
                ddl = ddlProtContainer
                ddlDocmContainer.Visible = False
                ddlReslContainer.Visible = False
                container = ScrivaniaProtocolContainer
            Case "Docm"
                ddl = ddlDocmContainer
                ddlProtContainer.Visible = False
                ddlReslContainer.Visible = False
                container = ScrivaniaDocumentContainer
            Case "Resl"
                ddl = ddlReslContainer
                ddlDocmContainer.Visible = False
                ddlProtContainer.Visible = False
                container = ScrivaniaResolutionContainer
        End Select
        pnlContainer.Visible = True

        ddl.Items.Clear()
        Dim containers As IList(Of ContainerRightsDto)
        If ResolutionEnv.ApplyGeneralistUserDeskRight Then
            containers = Facade.ContainerFacade.GetAllRights(Type, True)
            Select Case Action
                Case ACTION_NAME_PROTOCOL_TO_READ
                    containers = containers.Where(Function(f) Not DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainerIdentifiers.Any(Function(c) c = f.ContainerId)).ToList()
                Case ACTION_NAME_PROTOCOL_INVOICE_TO_READ
                    containers = containers.Where(Function(f) DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainerIdentifiers.Any(Function(c) c = f.ContainerId)).ToList()
            End Select
        Else
            Dim right As ResolutionRightPositions = ResolutionRightPositions.Preview
            Select Case Action
                Case ACTION_NAME_RESOLUTION_PROPOSED
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_ADOPTED
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_ACCOUNTING
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_PUBLISHED
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_EXECUTIVE
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED
                    right = ResolutionRightPositions.Administration
                Case ACTION_NAME_RESOLUTION_VERIFY
                    right = ResolutionRightPositions.Insert
                Case ACTION_NAME_RESOLUTION_CHECK_COMPLIANT
                    right = ResolutionRightPositions.Executive
                Case ACTION_NAME_RESOLUTION_COMPLIANT_TO_ADOPTION
                    right = ResolutionRightPositions.Adoption
                Case ACTION_NAME_RESOLUTION_NOT_COMPLIANT
                    right = ResolutionRightPositions.Insert
            End Select
            containers = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, right, Nothing).Select(Function(c) New ContainerRightsDto() With {
            .ContainerId = c.Id,
            .LocationId = c.ReslLocation.Id,
            .Name = c.Name
            }).ToList()
        End If
        ddl.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        If containers.Count > 0 Then
            For Each cont As ContainerRightsDto In containers.Where(Function(f) f.LocationId >= 0)
                Dim containerId As String = cont.ContainerId.ToString()
                Dim name As String = cont.Name
                Dim locationId As String = cont.LocationId.ToString()
                ddl.Items.Add(New DropDownListItem(name, containerId))
            Next
        End If
        ddl.SelectedValue = container
    End Sub

    ''' <summary> Prepara il finder e restituisce il count dei risultati trovati </summary>
    ''' <remarks> 
    ''' TODO: Spostare nella facade
    ''' </remarks>
    Private Sub PrepareProtocolFinder(from As DateTime?, [to] As DateTime?, ByVal maxRecords As Integer, ByRef count As Integer, idTenantAOO As Guid)
        _protocolFinder.TopMaxRecords = maxRecords
        _protocolFinder.IdStatus = ProtocolStatusId.Attivo
        Dim commonUtil As CommonUtil = New CommonUtil()
        If Not String.IsNullOrEmpty(ddlProtContainer.SelectedValue) Then
            _protocolFinder.IdContainer = ddlProtContainer.SelectedValue
        End If

        Select Case Action
            Case ACTION_NAME_PROTOCOL_UPDATED 'aggiornati di recente
                If from.HasValue Then
                    _protocolFinder.LastChangedDateFrom = from.Value
                End If
                If [to].HasValue Then
                    _protocolFinder.LastChangedDateTo = [to].Value
                End If

            Case Else
                If from.HasValue Then
                    _protocolFinder.RegistrationDateFrom = from.Value
                End If
                If [to].HasValue Then
                    _protocolFinder.RegistrationDateTo = [to].Value
                End If
        End Select
        Select Case Action
            Case ACTION_NAME_PROTOCOL_TO_READ 'Da leggere
                ' Visualizzo solo protocolli a cui ho accesso e che non ho mai aperto
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                commonUtil.ExcludeInvoiceContainer(_protocolFinder)

                If rowProtocolType.Visible AndAlso Not String.IsNullOrEmpty(ddlProtocolTypes.SelectedValue) AndAlso Not ddlProtocolTypes.SelectedValue.Eq("All") Then
                    _protocolFinder.IdTypes = New List(Of Integer) From {Convert.ToInt32(ddlProtocolTypes.SelectedValue)}
                End If
                _protocolFinder.IncludeCountRead = False
                uscProtocolGrid.ColumnHasReadVisible = False
                _protocolFinder.ProtocolNotReaded = True
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                If ProtocolEnv.DaLeggereSubject Then
                    uscProtocolGrid.EnableColumn(uscProtGrid.COLUMN_SUBJECT)
                End If
            Case ACTION_NAME_PROTOCOL_INVOICE_TO_READ 'Fatture da Leggere
                ' Visualizzo solo protocolli di fattura a cui ho accesso e che non ho mai aperto
                _protocolFinder.LoadFetchModeFascicleEnabled = False
                _protocolFinder.LoadFetchModeProtocolLogs = False
                _protocolFinder.IncludeCountRead = False
                uscProtocolGrid.ColumnHasReadVisible = False
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                commonUtil.OnlyInvoiceContainer(_protocolFinder)

                If rowProtocolType.Visible AndAlso Not String.IsNullOrEmpty(ddlProtocolTypes.SelectedValue) AndAlso Not ddlProtocolTypes.SelectedValue.Eq("All") Then
                    _protocolFinder.IdTypes = New List(Of Integer) From {Convert.ToInt32(ddlProtocolTypes.SelectedValue)}
                End If

                _protocolFinder.ProtocolNotReaded = True
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                If ProtocolEnv.DaLeggereSubject Then
                    uscProtocolGrid.EnableColumn(uscProtGrid.COLUMN_SUBJECT)
                End If
            Case ACTION_NAME_PROTOCOL_UPDATED
                ' Visualizzo solo protocolli a cui ho accesso e sono già letti ma che sono stati almeno aggiornati 
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                _protocolFinder.IncludeCountRead = False
                uscProtocolGrid.ColumnHasReadVisible = False
                If rowProtocolType.Visible AndAlso Not String.IsNullOrEmpty(ddlProtocolTypes.SelectedValue) AndAlso Not ddlProtocolTypes.SelectedValue.Eq("All") Then
                    _protocolFinder.IdTypes = New List(Of Integer) From {Convert.ToInt32(ddlProtocolTypes.SelectedValue)}
                End If
                _protocolFinder.ProtocolOnlyLastChangedandRead = True
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled

            Case ACTION_NAME_PROTOCOL_TO_DISTRIBUTE ' Da Distribuire
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Distribute, idTenantAOO)
                _protocolFinder.NotDistributed = True
                _protocolFinder.IdTypes = ProtocolEnv.ProtocolDistributionTypologies
                ' Abilito la selezione lato client
                uscProtocolGrid.ColumnClientSelectVisible = True
                _protocolFinder.IncludeCountRead = True
                uscProtocolGrid.ColumnHasReadVisible = True
                pnlMultiAutorizza.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                pnlMultiDistribuzione.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                pnlDistributionReject.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.DistributionRejectableEnabled

                If (Not CurrentManageableRoleId.HasValue AndAlso CurrentManageableRoleIds.Count <> 1) Then
                    cmdMultiDistribuzione.Enabled = False
                    cmdMultiDistribuzione.ToolTip = "E' necessario selezionare un singolo settore manager"
                    cmdMultiAutorizza.Enabled = False
                    cmdMultiAutorizza.ToolTip = "E' necessario selezionare un singolo settore manager"
                    cmdDistributionReject.Enabled = False
                    cmdDistributionReject.ToolTip = "E' necessario selezionare un singolo settore manager"
                End If

            Case ACTION_NAME_PROTOCOL_TO_WORK 'Da lavorare
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                _protocolFinder.AssignedToWork = True
                _protocolFinder.Status = 0
                _protocolFinder.IncludeCountRead = True
                uscProtocolGrid.ColumnHasReadVisible = True
            Case ACTION_NAME_PROTOCOL_IN_ASSIGNMENT ' In Assegnazione  
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                _protocolFinder.Shunted = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                _protocolFinder.IncludeCountRead = True
                uscProtocolGrid.ColumnHasReadVisible = True
            Case ACTION_NAME_PROTOCOL_RECENT ' Recenti
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                _protocolFinder.IncludeCountRead = False
                uscProtocolGrid.ColumnHasReadVisible = False

            Case ACTION_NAME_PROTOCOL_ASSIGNED ' STATO ASSEGNATO
                If Not commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Write, idTenantAOO) Then
                    Exit Sub
                End If
                _protocolFinder.IncludeCountRead = True
                uscProtocolGrid.ColumnHasReadVisible = True
                _protocolFinder.EnablePaging = False
                _protocolFinder.AdvancedStatus = "A"
                uscProtocolGridBar.SetAssignButton.Visible = DocSuiteContext.Current.ProtocolEnv.SendProtocolMessageFromViewerEnabled
            Case ACTION_NAME_PROTOCOL_IN_EVIDENCE
                If Not ProtocolEnv.ProtocolHighlightSecurityEnabled Then
                    commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                End If
                pnlCC.Visible = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled
                _protocolFinder.ProtocolHighlightToMe = True
                _protocolFinder.IncludeCountRead = True
                uscProtocolGrid.ColumnHasReadVisible = True
            Case UserDesktop.ActionNameProtocolliRigettati
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                _protocolFinder.IdStatus = ProtocolStatusId.Rejected
                _protocolFinder.IncludeCountRead = False
                uscProtocolGrid.ColumnHasReadVisible = False
                If ProtocolEnv.DaLeggereSubject Then
                    uscProtocolGrid.EnableColumn(uscProtGrid.COLUMN_SUBJECT)
                End If

            Case ACTION_NAME_PROTOCOL_INSERT_TODAY, ACTION_NAME_PROTOCOL_INSERT_THIS_MONTH, ACTION_NAME_PROTOCOL_INSERT_THIS_WEEK
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                _protocolFinder.IncludeCountRead = True

            Case ACTION_NAME_PROTOCOL_NEVER_DISTRIBUTED
                commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO)
                _protocolFinder.NeverDistributed = True
                _protocolFinder.IdTypes = ProtocolEnv.ProtocolDistributionTypologies
                _protocolFinder.IncludeCountRead = True

            Case Else
                If Not commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, idTenantAOO) Then
                    Exit Sub
                End If
        End Select

        ' COPIA CONOSCENZA
        If pnlCC.Visible Then
            Select Case (rbCC.SelectedValue)
                Case "CC"
                    _protocolFinder.RoleCC = True
                Case "PC"
                    _protocolFinder.RoleCC = False
                Case "APC"
                    _protocolFinder.OnlyExplicitRoles = True
                    _protocolFinder.RoleDistributionType = "E"
                    _protocolFinder.RoleCC = False
                Case "ACC"
                    _protocolFinder.OnlyExplicitRoles = True
                    _protocolFinder.RoleDistributionType = "E"
                    _protocolFinder.RoleCC = True
                Case "All"
                    _protocolFinder.RoleCC = Nothing
            End Select

        End If

        count = _protocolFinder.Count()
    End Sub

    ''' <summary> Prepara il finder e restituisce il count dei risultati trovati </summary>
    ''' <remarks> 
    ''' TODO: Spostare nella facade
    ''' </remarks>
    Public Function PrepareDocumentFinder(ByVal from As DateTime?, ByVal [to] As DateTime?, ByVal maxRecords As Integer, ByRef count As Integer, idTenantAOO As Guid) As Boolean
        If Not String.IsNullOrEmpty(ddlDocmContainer.SelectedValue) Then
            _documentFinder.IDContainer = ddlDocmContainer.SelectedValue
        End If
        If from.HasValue Then
            _documentFinder.DataInizioFrom = from.Value
        End If
        If [to].HasValue Then
            _documentFinder.DataInizioTo = [to].Value
        End If

        'Verifica sicurezza
        If Not CommonInstance.ApplyDocumentFinderSecurity(_documentFinder, idTenantAOO) Then
            Throw New DocSuiteException("Permessi Scrivania", "Diritti insufficienti per la ricerca nel modulo Pratiche.")
        End If

        If Action.Eq(ACTION_NAME_DOCUMENT_CHECKOUT) Then
            Dim versAlias As IAliasFinder = New AliasFinder("DocumentVersionings", "DocumentVersionings")
            Dim objAlias As IAliasFinder = New AliasFinder("DocumentObjects", "DocumentObjects")
            Dim fldAlias As IAliasFinder = New AliasFinder("DocumentFolders", "DocumentFolders")
            objAlias.JoinAlias.Add(versAlias)
            fldAlias.JoinAlias.Add(objAlias)
            _documentFinder.SQLExpressions.Add(ACTION_NAME_DOCUMENT_CHECKOUT, New SQLExpression(String.Format("CheckOutUser='{0}' AND CheckStatus='O'", DocSuiteContext.Current.User.FullUserName), fldAlias))
            _documentFinder.LoadDocumentFolderInfo = True
        End If

        If Action.Eq(ACTION_NAME_DOCUMENT_TO_READ) Then
            'Da leggere
            _documentFinder.DocumentNotReaded = True
        End If

        If Action.Eq(ACTION_NAME_DOCUMENT_OPEN_TO_READ) Then
            'Da leggere
            _documentFinder.DocumentNotReaded = True
            _documentFinder.DataInizioTo = Date.Today
            _documentFinder.EndDateFrom = Date.Today
        End If

        If Action.Eq(ACTION_NAME_DOCUMENT_TAKE_IN_CHARGE) Then
            Dim rights As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, DossierRoleRightPositions.Workflow, True, CurrentTenant.TenantAOO.UniqueId)

            If rights.Count > 0 Then
                For Each role As Role In rights
                    _documentFinder.IdRoleDestination.Add(role.Id.ToString())
                Next
            Else
                Return False
            End If

            _documentFinder.PresaInCarico = True
        End If

        If Action.Eq(ACTION_NAME_DOCUMENT_TO_READ) OrElse Action.Eq(ACTION_NAME_DOCUMENT_TAKE_IN_CHARGE) Then
            uscDocmGrid.ColumnClientSelectVisible = True
        End If

        If Action.Eq(ACTION_NAME_DOCUMENT_TIMETABLE) Then
            _documentFinder.DataInizioFrom = Nothing
            _documentFinder.DocumentDate_To = Nothing
            _documentFinder.DocumentFolderExpired = True
            _documentFinder.DocumentFolderExpiredDate = rdpCalendar.SelectedDate
        End If

        _documentFinder.EnableTableJoin = True
        count = _documentFinder.Count()

        Return True
    End Function

    ''' <summary> Prepara il finder e restituisce il count dei risultati trovati </summary>
    ''' <remarks> 
    ''' TODO: Spostare nella facade
    ''' </remarks>
    Public Function PrepareResolutionFinder(ByVal from As DateTime?, ByVal [to] As DateTime?, ByVal maxRecords As Integer, ByRef count As Integer, idTenantAOO As Guid) As Boolean
        If Not String.IsNullOrEmpty(ddlReslContainer.SelectedValue) Then
            _resolutionFinder.ContainerIds = ddlReslContainer.SelectedValue
        End If

        If Not Action.Eq(UserDesktop.RESOLUTION_PROPOSED_BY_ROLE) Then
            If from.HasValue Then
                _resolutionFinder.RegistrationDateFrom = from.Value
            End If
            If [to].HasValue Then
                _resolutionFinder.RegistrationDateTo = [to].Value
            End If
        End If

        _resolutionFinder.EnableStatus = False
        Dim securityFinderContainerRight As ResolutionRightPositions = ResolutionRightPositions.Preview
        Select Case Action
            Case ACTION_NAME_RESOLUTION_PROPOSED
                Dim stepDescriptions As List(Of String) = GetStepDescriptionsFromFilterAction(ACTION_NAME_RESOLUTION_PROPOSED)
                If stepDescriptions IsNot Nothing AndAlso stepDescriptions.Count > 0 Then
                    _resolutionFinder.DescriptionSteps = stepDescriptions
                    _resolutionFinder.StepAttivo = True
                Else
                    _resolutionFinder.DescriptionStep = "Proposta"
                End If
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                End If
            Case ACTION_NAME_RESOLUTION_ADOPTED   'Per AUSL-RE: Da Pubblicare
                Dim stepDescriptions As List(Of String) = GetStepDescriptionsFromFilterAction(ACTION_NAME_RESOLUTION_ADOPTED)
                If stepDescriptions IsNot Nothing AndAlso stepDescriptions.Count > 0 Then
                    _resolutionFinder.DescriptionSteps = stepDescriptions
                    _resolutionFinder.StepAttivo = True
                Else
                    _resolutionFinder.DescriptionStep = "Adozione"
                End If
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                End If
                If ResolutionEnv.Configuration.Eq("AUSL-PC") Then
                    _resolutionFinder.IdStatus = 0
                    _resolutionFinder.SortExpressions.Add("R.InclusiveNumber", "ASC")
                End If
            Case ACTION_NAME_RESOLUTION_ACCOUNTING
                Dim stepDescriptions As List(Of String) = GetStepDescriptionsFromFilterAction(ACTION_NAME_RESOLUTION_ACCOUNTING)
                If stepDescriptions IsNot Nothing AndAlso stepDescriptions.Count > 0 Then
                    _resolutionFinder.DescriptionSteps = stepDescriptions
                    _resolutionFinder.StepAttivo = True
                Else
                    _resolutionFinder.DescriptionStep = "Controllo"
                End If
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                End If
            Case ACTION_NAME_RESOLUTION_PUBLISHED   'Per AUSL-RE: Pubblicati
                Dim stepDescriptions As List(Of String) = GetStepDescriptionsFromFilterAction(ACTION_NAME_RESOLUTION_PUBLISHED)
                If stepDescriptions IsNot Nothing AndAlso stepDescriptions.Count > 0 Then
                    _resolutionFinder.DescriptionSteps = stepDescriptions
                    _resolutionFinder.StepAttivo = True
                Else
                    If ResolutionEnv.ApplyGeneralistUserDeskRight Then
                        _resolutionFinder.DescriptionStep = "Pubblicazione"
                    Else
                        _resolutionFinder.HasPublishingDate = True
                    End If
                End If

                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                    _resolutionFinder.SortExpressions.Add("R.InclusiveNumber", "DESC")
                End If
                If ResolutionEnv.Configuration.Eq("AUSL-PC") Then
                    _resolutionFinder.IdStatus = 0
                    If Not _resolutionFinder.SortExpressions.ContainsKey("R.InclusiveNumber") Then
                        _resolutionFinder.SortExpressions.Add("R.InclusiveNumber", "ASC")
                    End If
                End If
            Case ACTION_NAME_RESOLUTION_EXECUTIVE
                Dim stepDescriptions As List(Of String) = GetStepDescriptionsFromFilterAction(ACTION_NAME_RESOLUTION_EXECUTIVE)
                If stepDescriptions IsNot Nothing AndAlso stepDescriptions.Count > 0 Then
                    _resolutionFinder.DescriptionSteps = stepDescriptions
                    _resolutionFinder.StepAttivo = True
                Else
                    _resolutionFinder.DescriptionStep = "Esecutività"
                End If
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                End If
                If ResolutionEnv.Configuration.Eq("AUSL-PC") Then
                    _resolutionFinder.IdStatus = 0
                    _resolutionFinder.SortExpressions.Add("R.InclusiveNumber", "ASC")
                End If
            Case ACTION_NAME_RESOLUTION_VERIFY  'Per AUSL-RE: Alla Verifica
                _resolutionFinder.DescriptionStep = "ControlloIn"
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Insert
                    _resolutionFinder.SortExpressions.Add("R.ProposeDate", "ASC")
                End If
                _resolutionFinder.IdStatus = 0
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ADOPTION_DATE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_STATUS)
                uscReslGrid.SetColumnWidth(uscReslGrid.COLUMN_NUMBER, 50)
            Case ACTION_NAME_RESOLUTION_CHECK_COMPLIANT 'per AUSL-PC : Atti da controllare, per AUSL-RE: Da verificare
                _resolutionFinder.DescriptionStep = "ControlloIn"
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Executive
                    _resolutionFinder.SortExpressions.Add("R.ProposeDate", "ASC")
                End If
                _resolutionFinder.IdStatus = 0
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ADOPTION_DATE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_STATUS)
                uscReslGrid.SetColumnWidth(uscReslGrid.COLUMN_NUMBER, 50)
            Case ACTION_NAME_RESOLUTION_COMPLIANT_TO_ADOPTION 'per AUSL-PC : Atti Conformi, per AUSL-RE: Da Adottare
                _resolutionFinder.DescriptionStep = "ControlloOut"
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Adoption
                    _resolutionFinder.SortExpressions.Add("R.ProposeDate", "ASC")
                End If
                _resolutionFinder.IdStatus = 0
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ADOPTION_DATE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_STATUS)
                uscReslGrid.SetColumnWidth(uscReslGrid.COLUMN_NUMBER, 50)
            Case ACTION_NAME_RESOLUTION_NOT_COMPLIANT 'per AUSL-PC : Atti non conformi
                _resolutionFinder.DescriptionStep = "Proposta"
                _resolutionFinder.HasCancelMotivation = True
                _resolutionFinder.IdStatus = 0
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Insert
                End If
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ADOPTION_DATE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_STATUS)
                uscReslGrid.ColumnDeclineNoteVisible = True
                uscReslGrid.SetColumnWidth(uscReslGrid.COLUMN_NUMBER, 50)
            Case ACTION_NAME_RESOLUTION_INSERT_TODAY, ACTION_NAME_RESOLUTION_INSERT_THIS_WEEK, ACTION_NAME_RESOLUTION_INSERT_THIS_MONTH 'EF 20120229 Gestione della vista "Inserimento oggi"
                _resolutionFinder.IdStatus = 0
                _resolutionFinder.SortExpressions.Add("InclusiveNumber", "ASC")

            Case ACTION_NAME_RESOLUTION_AMM_TRASP ' Filtro per AUSL-TO : Atti  con bozze da completare amm trasparente

                _resolutionFinder.DocumentSeriesItemIdentifier = Facade.DocumentSeriesItemFacade.GetDocumentSeriesItem()
                _resolutionFinder.HasDocumentalSeriesDraft = True
                _resolutionFinder.IdStatus = 0
                _resolutionFinder.User = CurrentUser
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ADOPTION_DATE)
                uscReslGrid.DisableColumn(uscReslGrid.COLUMN_STATUS)
                uscReslGrid.SetColumnWidth(uscReslGrid.COLUMN_NUMBER, 120)

            Case UserDesktop.ActionNameAdottateNonEsecutive
                CommonUtil.GetInstance().ApplyResolutionFinderSecurity(Page, _resolutionFinder, idTenantAOO)
                _resolutionFinder.IsAdopted = True
                _resolutionFinder.IsEffective = False

            Case UserDesktop.RESOLUTION_PROPOSED_BY_ROLE
                _resolutionFinder.IsAdopted = True
                _resolutionFinder.SortExpressions.Clear()
                If ddlRoles.SelectedValue IsNot Nothing Then
                    Dim selectedRole As Role = Facade.RoleFacade.GetById(Integer.Parse(ddlRoles.SelectedValue))
                    _resolutionFinder.Proposer = selectedRole.Name
                End If
                _resolutionFinder.ResolutionType = Facade.ResolutionTypeFacade.GetById(Short.Parse(rblResolutionTypes.SelectedValue))
                'Filtro per data adozione
                If from.HasValue Then
                    _resolutionFinder.AdoptionDateFrom = from.Value
                End If
                If [to].HasValue Then
                    _resolutionFinder.AdoptionDateTo = [to].Value
                End If

            Case ACTION_NAME_RESOLUTION_AFF_GEN
                _resolutionFinder.DescriptionStep = WorkflowStep.AFF_GEN_CHECK_STEP_DESCRIPTION
                _resolutionFinder.ResolutionType = Facade.ResolutionTypeFacade.GetById(ResolutionType.IdentifierDetermina)
                uscReslGrid.ColumnUserTakeChargeVisibile = True
                _resolutionFinder.UserTakeCharge = String.Empty
                If chkChargeToMe.Checked Then
                    _resolutionFinder.UserTakeCharge = DocSuiteContext.Current.User.FullUserName
                End If

            Case ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                End If
                _resolutionFinder.OCRegion = True
                _resolutionFinder.SortExpressions.Add("R.ProposeDate", "ASC")

            Case ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED
                If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                    securityFinderContainerRight = ResolutionRightPositions.Administration
                End If
                _resolutionFinder.OCSupervisoryBoard = True
                _resolutionFinder.SortExpressions.Add("R.ProposeDate", "ASC")

        End Select

        'Configurazione
        _resolutionFinder.Configuration = DocSuiteContext.Current.ResolutionEnv.Configuration

        'Verifica sicurezza
        If Not CommonInstance.ApplyResolutionFinderSecurity(Me, _resolutionFinder, idTenantAOO, securityFinderContainerRight) Then
            Return False
        End If

        count = _resolutionFinder.Count()

        Return True
    End Function

    Private Sub BindDocument(idTenantAOO As Guid)
        Dim dateFrom As DateTime? = Nothing
        Select Case Action
            Case ACTION_NAME_DOCUMENT_TO_READ
                dateFrom = DateTime.Today.AddDays(-DocumentEnv.DesktopDayDiff)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_DOCUMENT_OPEN_TO_READ, ACTION_NAME_DOCUMENT_CHECKOUT
                ' NOOP
            Case ACTION_NAME_DOCUMENT_TAKE_IN_CHARGE
                dateFrom = DateTime.Today.AddMonths(-1)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_DOCUMENT_INSERT_TODAY
                dateFrom = DateTime.Today
                lblHeader.Text &= String.Format(" del {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_DOCUMENT_INSERT_THIS_WEEK
                dateFrom = DateTime.Today.AddDays(-7)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_DOCUMENT_INSERT_THIS_MONTH
                dateFrom = DateTime.Today.AddMonths(-1)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_DOCUMENT_TIMETABLE
                dateFrom = DateTime.Today.AddMonths(-1)
        End Select

        Dim success As Boolean = PrepareDocumentFinder(dateFrom, Nothing, DocumentEnv.DesktopMaxRecords, _records, idTenantAOO)

        If Action.Eq(ACTION_NAME_DOCUMENT_TO_READ) OrElse Action.Eq(ACTION_NAME_DOCUMENT_OPEN_TO_READ) Then
            SetPanelButtonBar()
        End If

        If success Then
            uscDocmGrid.Grid.Finder = _documentFinder
            uscDocmGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
            'Inizializza colonnne
            Select Case Action
                Case ACTION_NAME_DOCUMENT_TIMETABLE
                    uscDocmGrid.Grid.DataBindFinderWithExtCount(_records)
                Case ACTION_NAME_DOCUMENT_CHECKOUT
                    uscDocmGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRYDATE)
                    uscDocmGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRY_DESCRIPTION)
                    uscDocmGrid.Grid.DataBindFinderWithExtCount(_records)
                Case Else
                    uscDocmGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_NAME)
                    uscDocmGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRYDATE)
                    uscDocmGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRY_DESCRIPTION)
                    uscDocmGrid.Grid.PageSize = _documentFinder.PageSize
                    uscDocmGrid.Grid.DataBindFinderWithExtCount(_records)
            End Select
            lblHeader.Text &= String.Format(" ({0})", _records)
        Else
            HideFooter()
        End If
    End Sub

    Private Sub BindProtocol(idTenantAOO As Guid)
        BindProtocol(String.Empty, idTenantAOO)
    End Sub

    Private Sub BindProtocol(idContainer As String, idTenantAOO As Guid)
        Dim dateFrom As Date? = Nothing
        Dim dateTo As Date? = Nothing
        Select Case Action
            Case ACTION_NAME_PROTOCOL_TO_READ, ACTION_NAME_PROTOCOL_IN_ASSIGNMENT, ACTION_NAME_PROTOCOL_INVOICE_TO_READ
                dateFrom = rdpDateFilterFrom.SelectedDate.Value.Date
                dateTo = rdpDateFilterTo.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString(), dateTo.Value.ToShortDateString())
                uscProtocolGrid.ColumnProtocolContactVisible = DocSuiteContext.Current.ProtocolEnv.SenderToProtocolGridVisible
                uscProtocolGrid.ColumnCategoryNameVisible = DocSuiteContext.Current.ProtocolEnv.CategoryToProtocolGridVisible
                If Action.Eq(ACTION_NAME_PROTOCOL_TO_READ) AndAlso ProtocolEnv.ToReadProtocolTypeFinderEnabled Then
                    uscProtocolGrid.ColumnProtocolTypeShortDescriptionAllowFiltering = False
                End If
            Case ACTION_NAME_PROTOCOL_TO_DISTRIBUTE, ACTION_NAME_PROTOCOL_TO_WORK, ACTION_NAME_PROTOCOL_NEVER_DISTRIBUTED
                dateFrom = rdpDateFilterFrom.SelectedDate.Value.Date
                dateTo = rdpDateFilterTo.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1)
                lblHeader.Text &= String.Format(" dal {0} - al {1}", dateFrom.Value.ToShortDateString(), dateTo.Value.ToShortDateString())
                uscProtocolGrid.ColumnProtocolContactVisible = DocSuiteContext.Current.ProtocolEnv.SenderToProtocolGridVisible
                uscProtocolGrid.ColumnCategoryNameVisible = DocSuiteContext.Current.ProtocolEnv.CategoryToProtocolGridVisible
            Case ACTION_NAME_PROTOCOL_IN_EVIDENCE
                dateFrom = rdpDateFilterFrom.SelectedDate.Value.Date
                dateTo = rdpDateFilterTo.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1)
                lblHeader.Text &= String.Format(" dal {0} - al {1}", dateFrom.Value.ToShortDateString(), dateTo.Value.ToShortDateString())
                uscProtocolGrid.ColumnProtocolContactVisible = DocSuiteContext.Current.ProtocolEnv.SenderToProtocolGridVisible
                uscProtocolGrid.ColumnCategoryNameVisible = DocSuiteContext.Current.ProtocolEnv.CategoryToProtocolGridVisible
                uscProtocolGrid.ColumnHighlightNoteVisible = True
                uscProtocolGrid.ColumnHighlightRegistrationUserVisible = True
            Case ACTION_NAME_PROTOCOL_UPDATED
                dateFrom = rdpDateFilterFrom.SelectedDate.Value.Date
                dateTo = rdpDateFilterTo.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1)
                lblHeader.Text &= String.Format(" dal {0} - al {1}", dateFrom.Value.ToShortDateString(), dateTo.Value.ToShortDateString())
            Case ACTION_NAME_PROTOCOL_RECENT
                dateFrom = rdpCalendar.SelectedDate.Value
            Case ACTION_NAME_PROTOCOL_ASSIGNED
                dateFrom = Nothing
            Case ACTION_NAME_PROTOCOL_INSERT_TODAY
                dateFrom = Date.Today
                lblHeader.Text &= String.Format(" del {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_PROTOCOL_INSERT_THIS_WEEK
                dateFrom = Date.Today.AddDays(-7)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_PROTOCOL_INSERT_THIS_MONTH
                dateFrom = Date.Today.AddMonths(-1)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
        End Select

        PrepareProtocolFinder(dateFrom, dateTo, ProtocolEnv.DesktopMaxRecords, _records, idTenantAOO)

        ' Imposto filtro per contenitore
        If Not String.IsNullOrEmpty(idContainer) Then
            _protocolFinder.IdContainer = idContainer
        End If

        uscProtocolGrid.Grid.Finder = _protocolFinder
        If Action.Eq(ACTION_NAME_PROTOCOL_ASSIGNED) Then
            _protocolFinder.PageSize = If(_records = 0, 1, _records)
        Else
            _protocolFinder.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
        End If
        uscProtocolGrid.Grid.PageSize = _protocolFinder.PageSize

        Select Case Action
            Case ACTION_NAME_PROTOCOL_ASSIGNED
                uscProtocolGrid.Grid.MasterTableView.AllowMultiColumnSorting = True
                uscProtocolGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("RegistrationDate ASC")
                _protocolFinder.SortExpressions.Add("RegistrationDate", "ASC")
            Case ACTION_NAME_PROTOCOL_RECENT
                uscProtocolGrid.Grid.MasterTableView.AllowMultiColumnSorting = True
                uscProtocolGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("RegistrationDate DESC")
                _protocolFinder.SortExpressions.Add("RegistrationDate", "DESC")
            Case ACTION_NAME_PROTOCOL_UPDATED
                uscProtocolGrid.Grid.MasterTableView.AllowMultiColumnSorting = True
                _protocolFinder.SortExpressions.Add("LastChangedDate", "DESC")
            Case Else
                uscProtocolGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
                _protocolFinder.SortExpressions.Add("Id", "DESC")
        End Select

        uscProtocolGrid.Grid.DataBindFinderWithExtCount(_records)
        If DocSuiteContext.Current.ProtocolEnv.CategoryView <> 1 Then
            uscProtocolGrid.DisableColumn(uscProtGrid.COLUMN_CATEGORY_NAME)
        End If
        lblHeader.Text &= String.Format(" ({0})", _records)
    End Sub

    Private Sub BindResolution(idTenantAOO As Guid)
        Dim dateFrom As Date? = Nothing
        Dim dateTo As Date? = Nothing

        Select Case Action
            Case ACTION_NAME_RESOLUTION_ADOPTED, ACTION_NAME_RESOLUTION_PUBLISHED, ACTION_NAME_RESOLUTION_EXECUTIVE, ACTION_NAME_RESOLUTION_CHECK_COMPLIANT, ACTION_NAME_RESOLUTION_COMPLIANT_TO_ADOPTION, ACTION_NAME_RESOLUTION_NOT_COMPLIANT, ACTION_NAME_RESOLUTION_VERIFY, ACTION_NAME_RESOLUTION_AFF_GEN
                dateFrom = Date.Today.AddDays(-ResolutionEnv.DesktopDayDiff)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_RESOLUTION_INSERT_TODAY
                dateFrom = Date.Today
            Case ACTION_NAME_RESOLUTION_INSERT_THIS_WEEK
                dateFrom = Date.Today.AddDays(-7)
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())
            Case ACTION_NAME_RESOLUTION_INSERT_THIS_MONTH
                dateFrom = Date.Today.AddMonths(-1)
                lblHeader.Text &= String.Format(" del {0}", Date.Today.ToShortDateString())
            Case UserDesktop.RESOLUTION_PROPOSED_BY_ROLE
                dateFrom = rdpDateFilterFrom.SelectedDate
                ' Setto le ore alle 23:59:59
                dateTo = rdpDateFilterTo.SelectedDate
                lblHeader.Text &= String.Format(" dal {0} - al {1}", dateFrom.Value.ToShortDateString(), dateTo.Value.ToShortDateString())
            Case ACTION_NAME_RESOLUTION_PROVISION_REGION_CONTROL, ACTION_NAME_RESOLUTION_PROVISION_TO_BE_PUBLISHED
                If rdpDateFilterFrom.SelectedDate.HasValue Then
                    dateFrom = rdpDateFilterFrom.SelectedDate.Value.Date
                Else
                    dateFrom = Date.Today.AddDays(-ResolutionEnv.DesktopDayDiff)
                End If
                If rdpDateFilterTo.SelectedDate.HasValue Then
                    dateTo = rdpDateFilterTo.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1)
                End If
                lblHeader.Text &= String.Format(" dal {0}", dateFrom.Value.ToShortDateString())

        End Select

        Dim success As Boolean = PrepareResolutionFinder(dateFrom, dateTo, ResolutionEnv.DesktopMaxRecords, _records, idTenantAOO)

        If success Then
            _resolutionFinder.PageSize = DocSuiteContext.Current.ResolutionEnv.SearchMaxRecords
            uscReslGrid.Grid.Finder = _resolutionFinder
            uscReslGrid.Grid.PageSize = _resolutionFinder.PageSize
            'EF 20120214 Rimosso per AUSL-PC per ottenere un ordinamento diverso
            If (DocSuiteContext.Current.IsResolutionEnabled AndAlso Not ResolutionEnv.Configuration.Eq("AUSL-PC")) Then
                uscReslGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("RW.RegistrationDate DESC")
            End If
            If Action.Eq(UserDesktop.RESOLUTION_PROPOSED_BY_ROLE) Then
                uscReslGrid.Grid.MasterTableView.SortExpressions.AddSortExpression("RW.AdoptionDate DESC")
            End If

            uscReslGrid.Grid.DataBindFinderWithExtCount(_records)
            lblHeader.Text &= String.Format(" ({0})", _records)
        Else
            HideFooter()
        End If
    End Sub

    Private Sub HideFooter()
        pnlMultiAutorizza.Visible = False
        pnlContainer.Visible = False
        pnlButtonBar.Visible = False

    End Sub

    Private Sub InitializeProposedByRole()
        pnlContainer.Visible = False
        pnlDateFilter.Visible = True
        rdpDateFilterFrom.DateInput.Label = "Da"
        rdpDateFilterTo.DateInput.Label = "A"
        rdpDateFilterFrom.SelectedDate = DateTime.Today.AddMonths(-3)
        ' Setto le ore alle 23:59:59
        rdpDateFilterTo.SelectedDate = DateTime.Today.AddDays(1).AddSeconds(-1)

        rowRoles.Visible = True
        rowResolutionTypes.Visible = True
        Dim roles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Resolution, 1, True, String.Empty, False, Nothing, CurrentTenant.TenantAOO.UniqueId)
        If Not roles.Any() Then
            Throw New DocSuiteException("Nessun Settore configurato", "Utente non avente diritti alla visualizzazione")
        End If
        ddlRoles.DataSource = roles
        ddlRoles.DataTextField = "Name"
        ddlRoles.DataValueField = "Id"
        ddlRoles.DataBind()
        ddlRoles.Items(0).Selected = True

        rblResolutionTypes.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeliberaCaption, ResolutionType.IdentifierDelibera.ToString()))
        rblResolutionTypes.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeterminaCaption, ResolutionType.IdentifierDetermina.ToString()))
        rblResolutionTypes.Items(0).Selected = True
    End Sub

    Private Function GetStepDescriptionsFromFilterAction(filterAction As String) As List(Of String)
        Dim stepDescriptions As List(Of String) = New List(Of String)
        If ResolutionEnv.ResolutionSearchableSteps IsNot Nothing AndAlso ResolutionEnv.ResolutionSearchableSteps.Any(Function(f) f.FilterAction.Equals(filterAction)) Then
            stepDescriptions = ResolutionEnv.ResolutionSearchableSteps.FirstOrDefault(Function(f) f.FilterAction.Equals(filterAction)).ResolutionStepDescription.Select(Function(i) i.Description).ToList()
        End If
        Return stepDescriptions
    End Function
#End Region

End Class
