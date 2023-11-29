Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers

Partial Public Class uscReslGrid
    Inherits GridControl

    Public Enum WorkflowDescription
        Standard = 0
        Database = 1
    End Enum

    Protected Delegate Sub SetReadDelegate(ByVal Year As Short, ByVal Number As Integer, ByVal LogType As String, ByVal LogDescription As String)

#Region " Fields "

    Public Const COLUMN_CLIENT_SELECT As String = "ClientSelectColumn"
    Public Const COLUMN_TYPE As String = "T"
    Public Const COLUMN_DOCUMENT As String = "D"
    Public Const COLUMN_DOCUMENT_SIGN As String = "S"
    Public Const COLUMN_DOCUMENT_TYPE As String = "Type.Id"
    Public Const COLUMN_YEAR As String = "Year"
    Public Const COLUMN_NUMBER As String = "Id"
    Public Const COLUMN_STATUS As String = "Status.Id"
    Public Const COLUMN_DATE As String = "Data"
    Public Const COLUMN_CONTROLLER_STATUS As String = "ControllerStatus.Acronym"
    Public Const COLUMN_CONTAINER As String = "Container.Name"
    Public Const COLUMN_CATEGORY As String = "Category.Name"
    Public Const COLUMN_OBJECT As String = "ResolutionObject"
    Public Const COLUMN_ATTACH_SELECT As String = "AllegatiSelectColumn"
    Public Const COLUMN_NOTE As String = "Note"
    ''' <summary>  </summary>
    ''' <remarks> ASL-TO2 </remarks>
    Public Const COLUMN_TIPOC As String = "TipOC"
    Public Const COLUMN_PROPOSER_CODE As String = "ProposerCode"
    Public Const COLUMN_REGIONE As String = "Regione"
    Public Const COLUMN_ADOPTION_DATE As String = "AdoptionDate"

    Public Const COLUMN_RESOLUTION_TASK_STATUS As String = "ResolutionTaskStatus"
    Public Const COLUMN_DECLINE_NOTE As String = "DeclineNote"

    Public Const COLUMN_LAST_LOG As String = "LastReslLog"
    Public Const COLUMN_RETURN_FROM_COLLABORATION As String = "ReturnFromCollaboration"
    Public Const COLUMN_USER_TAKE_CHARGE As String = "UserTakeCharge"

    Private _autoHideColumns As Nullable(Of Boolean)
    Private _workflowDescriptionType As Nullable(Of WorkflowDescription)
    Private _determinaManagedData As String
    Private _deliberaManagedData As String

    Private _isWorkflow As Boolean = False

    Protected _setReadDelegate As SetReadDelegate
    Private _workflowStep As Short?
    Private _workflowType As String
#End Region

#Region " Properties "
    Private ReadOnly Property Action As String
        Get
            Return Request.QueryString.Item("Action")
        End Get
    End Property

    Private ReadOnly Property DeterminaManagedData As String
        Get
            If String.IsNullOrEmpty(_determinaManagedData) Then
                _determinaManagedData = Facade.TabMasterFacade.GetFieldValue(TabMasterFacade.ManagedDataField, DocSuiteContext.Current.ResolutionEnv.Configuration, ResolutionType.IdentifierDetermina)
            End If
            Return _determinaManagedData
        End Get
    End Property

    Public ReadOnly Property Grid As BindGrid
        Get
            Return grdResolutions
        End Get
    End Property

    Public Property AutoHideColumns As Boolean
        Get
            Return _autoHideColumns.GetValueOrDefault(True)
        End Get
        Set(value As Boolean)
            _autoHideColumns = value
        End Set
    End Property

    Public Property WorkflowDescriptionType As WorkflowDescription
        Get
            Return _workflowDescriptionType.GetValueOrDefault(WorkflowDescription.Standard)
        End Get
        Set(ByVal value As WorkflowDescription)
            _workflowDescriptionType = value
        End Set
    End Property

    Public Property ColumnHasSignVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT_SIGN).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT_SIGN).Visible = value
        End Set
    End Property

    Public Property ColumnDocumentTypeVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT).Visible = value
        End Set
    End Property

    Public Property ColumnResolutionTypeVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT_TYPE).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT_TYPE).Visible = value
        End Set
    End Property

    Public Property ColumnAttachSelectVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_ATTACH_SELECT).Visible
        End Get
        Set(ByVal value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_ATTACH_SELECT).Visible = value
        End Set
    End Property

    Public Property ColumnDeclineNoteVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_DECLINE_NOTE).Visible
        End Get
        Set(ByVal value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_DECLINE_NOTE).Visible = value
        End Set
    End Property

    Public Property ColumnControllerStatusVisibile As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_CONTROLLER_STATUS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_CONTROLLER_STATUS).Visible = value
        End Set
    End Property

    Public Property ColumnOcVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_TIPOC).Visible
        End Get
        Set(ByVal value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_TIPOC).Visible = value
        End Set
    End Property

    Public Property ColumnRegistrationDateVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_DATE).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_DATE).Visible = value
        End Set
    End Property

    Public Property ColumnResolutionStatusVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_STATUS).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_STATUS).Visible = value
        End Set
    End Property

    Public Property ColumnTipoAttoVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_TYPE).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_TYPE).Visible = value
        End Set
    End Property

    Public Property ColumnClientSelectVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_CLIENT_SELECT).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_CLIENT_SELECT).Visible = value
        End Set
    End Property

    Public Property ColumnResolutionTaskStatus As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_RESOLUTION_TASK_STATUS).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_RESOLUTION_TASK_STATUS).Visible = value
        End Set
    End Property
    Public Property ColumnLastLogVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_LAST_LOG).Visible
        End Get
        Set(ByVal value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_LAST_LOG).Visible = value
        End Set
    End Property
    Public Property ColumnReturnFromCollaborationVisible As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_RETURN_FROM_COLLABORATION).Visible
        End Get
        Set(ByVal value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_RETURN_FROM_COLLABORATION).Visible = value
        End Set
    End Property
    Public Property ColumnUserTakeChargeVisibile As Boolean
        Get
            Return grdResolutions.Columns.FindByUniqueName(COLUMN_USER_TAKE_CHARGE).Visible
        End Get
        Set(value As Boolean)
            grdResolutions.Columns.FindByUniqueName(COLUMN_USER_TAKE_CHARGE).Visible = value
        End Set
    End Property
    Protected Property SetReadFunction() As SetReadDelegate
        Get
            Return _setReadDelegate
        End Get
        Set(ByVal value As SetReadDelegate)
            _setReadDelegate = value
        End Set
    End Property

    Public Property IsWorkflow As Boolean
        Get
            Return _isWorkflow
        End Get
        Set(value As Boolean)
            _isWorkflow = value
        End Set
    End Property

    ''' <summary> Intestazione colonna del numero resolution </summary>
    ''' <remarks> Sovrascrive il meccanismo standard, creato per griglie ODG </remarks>
    Public Property OverwriteColumnNumberHeaderText As String
        Get
            Return CType(ViewState("OverwriteColumnNumberHeaderText"), String)
        End Get
        Set(value As String)
            ViewState("OverwriteColumnNumberHeaderText") = value
        End Set
    End Property

    Public Property EnableGridScrolling As Boolean = True
    Private ReadOnly Property WorkflowStepIdCheck As Short?
        Get
            If Not _workflowStep.HasValue Then
                _workflowStep = 0
                Dim values As String() = ResolutionEnv.CheckResolutionStep.Split("|"c)
                If values.Length = 2 Then
                    _workflowStep = Facade.TabWorkflowFacade.GetByDescription(values(1), WorkflowTypeCheck).Id.ResStep
                End If
            End If
            Return _workflowStep
        End Get
    End Property
    Private ReadOnly Property WorkflowTypeCheck As String
        Get
            If String.IsNullOrEmpty(_workflowType) Then
                Dim values As String() = ResolutionEnv.CheckResolutionStep.Split("|"c)
                If values.Length = 2 Then
                    _workflowType = values(0)
                End If
            End If
            Return _workflowType
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeColumns()
    End Sub

    Private Sub GrdResolutionsInit(ByVal sender As Object, ByVal e As EventArgs) Handles grdResolutions.Init
        grdResolutions.EnableScrolling = EnableGridScrolling
        InitializeFilterColumns()
    End Sub

    Private Sub GrdResolutionsItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdResolutions.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As ResolutionHeader = CType(e.Item.DataItem, ResolutionHeader)

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") OrElse DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            e.Item.Font.Bold = boundHeader.OCRegion.GetValueOrDefault(False)
        End If

        ' Torino: Se ho il documento principale allora abilito il checkbox di selezione
        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") AndAlso IsWorkflow AndAlso Not boundHeader.PublishingDate.HasValue AndAlso boundHeader.AdoptionDate.HasValue Then
            DirectCast(e.Item.FindControl("cbSelect"), CheckBox).Enabled = boundHeader.IdResolutionFile.HasValue
        End If

        If ColumnTipoAttoVisible Then
            DirectCast(e.Item.FindControl("imgTipoAtto"), Image).ImageUrl = GetImageUrlByHeader(boundHeader, COLUMN_TYPE)
        End If
        If ColumnHasSignVisible Then
            DirectCast(e.Item.FindControl("imgSigned"), Image).ImageUrl = GetImageUrlByHeader(boundHeader, COLUMN_DOCUMENT_SIGN)
        End If
        If ColumnDocumentTypeVisible Then
            With DirectCast(e.Item.FindControl("ibtDocumentType"), ImageButton)
                .Visible = boundHeader.IdDocument.GetValueOrDefault(0) > 0
                If .Visible Then
                    .CommandArgument = boundHeader.Id
                    .ImageUrl = GetImageUrlByHeader(boundHeader, COLUMN_DOCUMENT)

                    .Attributes.Add("onmouseover", "this.style.cursor='hand';")
                    .Attributes.Add("onmouseout", "this.style.cursor='default';")
                End If
            End With
        End If

        If ColumnResolutionTypeVisible Then
            DirectCast(e.Item.FindControl("lblResolutionType"), Label).Text = GetTextByHeader(boundHeader, COLUMN_DOCUMENT_TYPE)
        End If

        If String.IsNullOrEmpty(ResolutionEnv.DeclineNoteViewIndex) Then
            ColumnDeclineNoteVisible = False
        End If

        If ColumnDeclineNoteVisible Then
            Dim declineNoteIndexes As Integer() = ResolutionEnv.DeclineNoteViewIndex.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()
            Dim declineNoteMessageLabel As Label = DirectCast(e.Item.FindControl("lblDeclineNoteMessage"), Label)
            Dim declienNoteStepNumberLabel As Label = DirectCast(e.Item.FindControl("lblDeclineNoteStepNumber"), Label)
            Dim declienNoteStepNameLabel As Label = DirectCast(e.Item.FindControl("lblDeclineNoteStepName"), Label)
            Dim declienNoteDateLabel As Label = DirectCast(e.Item.FindControl("lblDeclineNoteDate"), Label)
            declineNoteMessageLabel.Visible = False
            declienNoteStepNumberLabel.Visible = False
            declienNoteStepNameLabel.Visible = False
            declienNoteDateLabel.Visible = False
            For Each declineNoteIndex As Integer In declineNoteIndexes
                Select Case declineNoteIndex
                    Case 0
                        declineNoteMessageLabel.Visible = True
                        declineNoteMessageLabel.Text = String.Format("Messaggio: '{0}'", boundHeader.DeclineNote.Split("§"c)(declineNoteIndex))
                    Case 1
                        declienNoteStepNumberLabel.Visible = True
                        declienNoteStepNumberLabel.Text = String.Format("Step numero: '{0}'", boundHeader.DeclineNote.Split("§"c)(declineNoteIndex))
                    Case 2
                        declienNoteStepNameLabel.Visible = True
                        declienNoteStepNameLabel.Text = String.Format("Step nome: '{0}'", boundHeader.DeclineNote.Split("§"c)(declineNoteIndex))
                    Case 3
                        declienNoteDateLabel.Visible = True
                        declienNoteDateLabel.Text = String.Format("Step data: '{0}'", boundHeader.DeclineNote.Split("§"c)(declineNoteIndex))
                End Select
            Next
        End If

        With DirectCast(e.Item.FindControl("lnkResolution"), LinkButton)
            .CommandArgument = boundHeader.Id.ToString()
            .Text = GetTextByHeader(boundHeader, COLUMN_NUMBER)
            If RedirectOnParentPage Then
                .OnClientClick = GetOnClientClickByHeader(boundHeader, COLUMN_NUMBER)
            End If

            .Attributes.Add("onmouseover", "this.style.cursor='hand';")
            .Attributes.Add("onmouseout", "this.style.cursor='default';")
        End With

        Dim lblResolutionStatus As Label = TryCast(e.Item.FindControl("lblResolutionStatus"), Label)
        If lblResolutionStatus IsNot Nothing Then
            lblResolutionStatus.Text = Facade.ResolutionFacade.GetWorkflowString("W", boundHeader, WorkflowDescriptionType = WorkflowDescription.Database)
            If boundHeader.StatusId.HasValue AndAlso boundHeader.StatusId.Value.Equals(ResolutionStatusId.Ritirata) Then
                lblResolutionStatus.Text = boundHeader.Status.Description
            End If
        End If

        If ColumnRegistrationDateVisible Then
            DirectCast(e.Item.FindControl("lblRegistrationDate"), Label).Text = GetTextByHeader(boundHeader, COLUMN_DATE)
        End If
        If ColumnOcVisible Then
            DirectCast(e.Item.FindControl("lblTipOC"), Label).Text = GetTextByHeader(boundHeader, COLUMN_TIPOC)
        End If

        If ColumnAttachSelectVisible Then
            Dim cbl As CheckBoxList = CType(e.Item.FindControl("DocumentiCheckList"), CheckBoxList)
            cbl.DataSource = GetWebDocDataSource(boundHeader, String.Empty)
            cbl.DataBind()
            For Each item As ListItem In cbl.Items
                item.Attributes.Add("onclick", "WebCheckClicked(this);")
                item.Selected = False
                If item.Value.EndsWith("|1") Then
                    item.Selected = True
                    item.Attributes.Add("style", "font-weight: bold")
                End If
            Next
        End If

        If ColumnLastLogVisible Then
            Dim lblLastReslLog As Label = DirectCast(e.Item.FindControl("lblLastReslLog"), Label)
            lblLastReslLog.Text = boundHeader.ConfirmViewBy
        End If

        If ColumnReturnFromCollaborationVisible Then
            Dim imgReturnFromCollaboration As Image = DirectCast(e.Item.FindControl("imgReturnFromCollaboration"), Image)
            Dim imgRetroStepResolution As Image = DirectCast(e.Item.FindControl("imgRetroStepResolution"), Image)
            imgReturnFromCollaboration.Visible = False
            imgRetroStepResolution.Visible = False
            Dim activeStep As Short = Facade.ResolutionWorkflowFacade.GetActiveStep(boundHeader.Id)

            If boundHeader.WorkflowType = WorkflowTypeCheck AndAlso activeStep = WorkflowStepIdCheck Then
                imgReturnFromCollaboration.ImageUrl = "../comm/images/parer/red.png"
                imgReturnFromCollaboration.Visible = CBool(boundHeader.ReturnFromCollaboration)

                imgRetroStepResolution.ImageUrl = "../comm/images/parer/yellow.png"
                imgRetroStepResolution.Visible = CBool(boundHeader.ReturnFromRetroStep)
            End If
        End If

        If ColumnUserTakeChargeVisibile Then
            Dim lblUserTakeCharge As Label = DirectCast(e.Item.FindControl("lblUserTakeCharge"), Label)
            lblUserTakeCharge.Text = boundHeader.CurrentUserTakeCharge
        End If
    End Sub

    Private Sub GrdResolutionsItemCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles grdResolutions.ItemCommand
        Select Case e.CommandName
            Case "ShowResl"
                Dim idResolution As Integer = CType(e.CommandArgument, Integer)
                Select Case Action
                    Case "Docm"
                        Dim resolution As Resolution = Facade.ResolutionFacade.GetById(idResolution)
                        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", GetResolutionParamByResolution(resolution)))

                    Case "CopyReslDocuments"
                        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", idResolution))

                    Case Else
                        RedirectOnPage(String.Format("../Resl/{0}?{1}", ReslBasePage.GetViewPageName(idResolution), CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", idResolution))))
                End Select
            Case "Open"
                Dim currentResolution As Resolution = Facade.ResolutionFacade.GetById(CInt(e.CommandArgument))
                Dim currentActiveTabWorkflow As TabWorkflow = Facade.TabWorkflowFacade.GetActive(currentResolution)
                Dim currentResolutionRights As ResolutionRights = New ResolutionRights(currentResolution)
                If Not currentResolutionRights.IsDocumentViewable(currentActiveTabWorkflow) AndAlso (Not currentResolution.EffectivenessDate.HasValue OrElse Not ResolutionEnv.ShowExecutiveDocumentEnabled) Then
                    BasePage.AjaxAlert("Diritti Insufficienti per la Visualizzazione del Documento.")
                    Exit Sub
                End If
                Dim viewableDocument As Boolean = True
                If currentResolution.Container.Privacy.HasValue AndAlso Convert.ToBoolean(currentResolution.Container.Privacy.Value) Then
                    viewableDocument = currentResolutionRights.IsPrivacyViewable
                End If
                Facade.ResolutionLogFacade.Log(currentResolution, ResolutionLogType.RS)
                Dim url As String = GetFileResolutionViewUrl(currentResolution.Id, viewableDocument)
                Response.Redirect(url)
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Function GetColumnNumberHeaderText() As String
        ' 1 customizzazioni in pagina
        If Not String.IsNullOrEmpty(OverwriteColumnNumberHeaderText) Then
            Return OverwriteColumnNumberHeaderText
        End If
        ' 2 configurazioni semplificate per cliente
        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            Return "Numero"
        End If
        ' 3 configurazioni da TabMaster, quelle che tutti dovrebbero utilizzare (T_T )°
        If Facade.ResolutionFacade.IsManagedPropertyByString(DeterminaManagedData, "ServiceNumber") Then
            Return "Servizio"
        ElseIf Facade.ResolutionFacade.IsManagedPropertyByString(DeterminaManagedData, "Number") Then
            Return "Numero"
        End If
        Return String.Empty
    End Function

    Private Sub InitializeColumns()
        If Not DocSuiteContext.Current.IsResolutionEnabled Then
            Exit Sub
        End If

        grdResolutions.Columns.FindByUniqueName(COLUMN_NUMBER).HeaderText = GetColumnNumberHeaderText()

        If AutoHideColumns Then
            If Facade.ResolutionFacade.IsManagedPropertyByString(DeterminaManagedData, "Year") _
                AndAlso Not Facade.ResolutionFacade.IsManagedPropertyByString(DeterminaManagedData, "Year", "VR-VI") Then
                DisableColumn(COLUMN_YEAR)
            End If
            If Not Facade.ResolutionFacade.IsManagedPropertyByString(DeterminaManagedData, "Category") Then
                DisableColumn(COLUMN_CATEGORY)
            End If
        End If

        grdResolutions.Columns.FindByUniqueName(COLUMN_NOTE).Visible = False
        Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
            Case "ASL3-TO"
                DisableColumn(COLUMN_CONTROLLER_STATUS)
                DisableColumn(COLUMN_CATEGORY)
                grdResolutions.Columns.FindByUniqueName(COLUMN_NOTE).Visible = True

            Case "AUSL-PC"
                DisableColumn(COLUMN_CONTROLLER_STATUS)
                DisableColumn(COLUMN_PROPOSER_CODE)
                DisableColumn(COLUMN_DOCUMENT) 'Firme
                SetColumnWidth(COLUMN_DOCUMENT_TYPE, 125)

            Case Else
                DisableColumn(COLUMN_TIPOC)
                DisableColumn(COLUMN_PROPOSER_CODE)

                If Not DocSuiteContext.Current.ResolutionEnv.IsPreViewDocEnabled Then
                    DisableColumn(COLUMN_DOCUMENT) 'Preview Document
                End If
        End Select
    End Sub

    Private Sub InitializeFilterColumns()
        If Not DocSuiteContext.Current.IsResolutionEnabled Then
            Exit Sub
        End If

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Equals("AUSL-PC", StringComparison.InvariantCultureIgnoreCase) Then
            grdResolutions.Columns.FindByUniqueName(COLUMN_NUMBER).SortExpression = "R.InclusiveNumber"
        End If

        If ColumnDocumentTypeVisible Then
            With DirectCast(grdResolutions.Columns.FindByUniqueNameSafe(COLUMN_DOCUMENT_TYPE), SuggestFilteringTemplateColumn)
                .DataSourceCombo = Facade.ResolutionTypeFacade.GetResolutionTypes()
                .DataTextCombo = "Description"
                .DataFieldCombo = "Id"
                .DataType = GetType(Short)
            End With
        End If
    End Sub

    Private Function GetTextByHeader(header As ResolutionHeader, discriminator As String) As String
        Select Case discriminator
            Case COLUMN_DOCUMENT_TYPE ' lblResolutionType
                Dim retval As String = String.Empty
                If Facade.ResolutionFacade.IsManagedPropertyByString(header.ManagedData, "idProposalFile", Nothing) AndAlso Not header.AdoptionDate.HasValue Then
                    retval = "Proposta di "
                End If
                retval &= header.ResolutionTypeCaption
                Return retval

            Case COLUMN_NUMBER ' lnkResolution
                Return Facade.ResolutionFacade.GetResolutionNumber(header, False)

            Case COLUMN_DATE ' lblRegistrationDate
                Dim useTabWorkflow As Boolean = WorkflowDescriptionType.Equals(WorkflowDescription.Database)
                Dim dateWorkflow As String = Facade.ResolutionFacade.GetWorkflowString("D", header, useTabWorkflow)
                Dim registrationDate As DateTime
                If DateTime.TryParse(dateWorkflow, registrationDate) Then
                    dateWorkflow = String.Format("{0:dd/MM/yyyy}", registrationDate)
                End If
                Return dateWorkflow

            Case COLUMN_TIPOC ' lblTipOC
                Dim ocs As New List(Of String)

                Dim supervisoryBoardLabel As String = "CS"
                Dim regionLabel As String = "R"
                Dim managementLabel As String = "CG"
                Dim corteContiLabel As String = "CC"
                Dim otherLabel As String = "A"
                Dim joinString As String = "-"

                Dim ocOptions As OCOptionsModel = ResolutionEnv.OCOptions
                If ocOptions IsNot Nothing Then
                    supervisoryBoardLabel = ocOptions.SupervisoryBoard.Label
                    regionLabel = ocOptions.Region.Label
                    managementLabel = ocOptions.Management.Label
                    corteContiLabel = ocOptions.CorteConti.Label
                    otherLabel = ocOptions.Other.Label
                    joinString = Environment.NewLine
                End If

                If header.OCSupervisoryBoard.GetValueOrDefault(False) Then
                    ocs.Add(supervisoryBoardLabel)
                End If
                If header.OCRegion.GetValueOrDefault(False) Then
                    ocs.Add(regionLabel)
                End If
                If header.OCManagement.GetValueOrDefault(False) Then
                    ocs.Add(managementLabel)
                End If
                If header.OCCorteConti.GetValueOrDefault(False) Then
                    ocs.Add(corteContiLabel)
                End If
                If header.OCOther.GetValueOrDefault(False) Then
                    ocs.Add(otherLabel)
                End If
                Return String.Join(joinString, ocs)
        End Select
        Return String.Empty
    End Function

    Private Function GetImageUrlByHeader(header As ResolutionHeader, discriminator As String) As String
        Select Case discriminator
            Case COLUMN_TYPE ' imgTipoAtto
                Dim statusId As Integer
                If header.AdoptionDate.HasValue AndAlso Not DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
                    If header.PublishingDate.HasValue Then
                        statusId = header.Status.Id
                    Else
                        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
                            If Not header.IdResolutionFile.HasValue AndAlso IsWorkflow Then
                                statusId = -99
                            Else
                                statusId = -10
                            End If
                        Else
                            statusId = -10
                        End If
                    End If
                Else
                    statusId = header.Status.Id
                End If
                Return ReslBasePage.DefineIcon(header.Type, statusId, False)

            Case COLUMN_DOCUMENT_SIGN ' imgSigned
                Dim documentName As String = Facade.ResolutionWorkflowFacade.GetDocumentName(header)
                If FileHelper.MatchExtension(documentName, FileHelper.P7M) Then
                    Return ImagePath.SmallSigned
                End If
                Return ImagePath.SmallEmpty

            Case COLUMN_DOCUMENT ' ibtDocumentType
                If header.IdDocument > 0 Then
                    Dim documentName As String = Facade.ResolutionWorkflowFacade.GetDocumentName(header)
                    Return ImagePath.FromFile(documentName)
                End If
                Return ImagePath.SmallEmpty
        End Select
        Return String.Empty
    End Function

    Private Function GetOnClientClickByHeader(header As ResolutionHeader, discriminator As String) As String
        Select Case discriminator
            Case COLUMN_NUMBER ' lnkResolution
                Dim parentPath As String = String.Format("IdResolution={0}&Type=Resl", header.Id)
                parentPath = String.Format("../Resl/{0}?{1}", ReslBasePage.GetViewPageName(header.Id), CommonShared.AppendSecurityCheck(parentPath))
                Return grdResolutions.GetRedirectParentPageScript(parentPath)
        End Select
        Return String.Empty
    End Function

    Private Function GetResolutionParamByResolution(ByVal resolution As Resolution) As String
        If resolution Is Nothing Then
            Return JsonConvert.SerializeObject(New ResolutionParam())
        End If

        Dim retval As New ResolutionParam()
        With retval
            .Id = resolution.Id
            .IdType = resolution.Type.Id
            Dim managedData As String = Facade.TabMasterFacade.GetFieldValue(TabMasterFacade.ManagedDataField, DocSuiteContext.Current.ResolutionEnv.Configuration, resolution.Type.Id)
            If Facade.ResolutionFacade.IsManagedPropertyByString(managedData, "ServiceNumber") Then
                .ServiceNumber = resolution.ServiceNumber
            End If
            If Facade.ResolutionFacade.IsManagedPropertyByString(managedData, "Number") Then
                .Year = resolution.Year
                .Number = resolution.Number
            End If
        End With
        Return JsonConvert.SerializeObject(retval)
    End Function

    Private Function GetWebDocDataSource(ByVal header As ResolutionHeader, ByVal forcedName As String) As IList(Of WebDoc)
        'WebDocument contiene i documenti che verranno restituiti per l'atto
        Dim webDocuments As New List(Of WebDoc)
        header.File = Facade.FileResolutionFacade.GetById(header.Id) ' Per qualche arcano motivo che non ho ancora scoperto questa property non viene caricata... - FG
        If header.File Is Nothing Then
            Return webDocuments
        End If

        'Definisco la variabile che discrimina tra contenitore standard e contenitore privacy
        Dim isContainerPrivacy As Boolean = Convert.ToBoolean(header.ProxiedContainer.Privacy.GetValueOrDefault(0))

        If Not isContainerPrivacy Then
            'Situazione normale
            'Carico il documento principale standard
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(header.ProxiedLocation, header.File, header.WebDocFieldDocumentName, forcedName, True))

            'Carico gli allegati standard
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(header.ProxiedLocation, header.File, header.WebDocFieldAttachmentName, forcedName, True))
        Else
            'Situazione privacy
            'Carico il documento principale Omissis
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(header.ProxiedLocation, header.File, header.WebDocFieldDocumentOmissisName, forcedName, True))

            'Carico gli allegati Omissis
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(header.ProxiedLocation, header.File, header.WebDocFieldAttachmentOmissisName, forcedName, False))
        End If

        Return webDocuments
    End Function

    Private Function GetFileResolutionViewUrl(idResolution As Integer, viewableDocument As Boolean) As String
        Const viewPage As String = "~/viewers/ResolutionViewer.aspx?"
        Dim paramId As String = String.Format("{0}IdResolution={1}", viewPage, idResolution)
        Dim queryString As String = String.Format("{0}&documents={1}&attachments=true&annexes=true&documentsomissis=true&attachmentsomissis=true&dematerialisation=true&metadata=true&previous=conditional", paramId, viewableDocument)
        Dim queryStringWithSecurityCheck As String = CommonShared.AppendSecurityCheck(queryString)
        Return queryStringWithSecurityCheck
    End Function

    Public Sub RemoveItemCommandHandler()
        RemoveHandler grdResolutions.ItemCommand, AddressOf GrdResolutionsItemCommand
    End Sub

    Public Sub DisableColumn(ByVal columnName As String)
        Dim col As GridColumn = grdResolutions.Columns.FindByUniqueNameSafe(columnName)
        If col IsNot Nothing Then
            Try
                'nasconde la colonna
                col.Visible = False
                ''elimina il bind dei dati della colonna
                Dim gridTemplateColumn As GridTemplateColumn = TryCast(col, GridTemplateColumn)
                If (gridTemplateColumn IsNot Nothing) Then
                    gridTemplateColumn.ItemTemplate = Nothing
                End If

                Dim gridBoundColumn As GridBoundColumn = TryCast(col, GridBoundColumn)
                If (gridBoundColumn IsNot Nothing) Then
                    gridBoundColumn.DataField = ""
                End If
            Catch ex As Exception
                Throw New Exception("Unable to disable column " & col.UniqueName)
            End Try
        End If
    End Sub

    Public Sub SetColumnWidth(ByVal columnName As String, ByVal width As Integer)
        grdResolutions.Columns.FindByUniqueName(columnName).HeaderStyle.Width = Unit.Pixel(width)
        grdResolutions.Columns.FindByUniqueName(columnName).ItemStyle.Width = Unit.Pixel(width)
    End Sub

#End Region

End Class