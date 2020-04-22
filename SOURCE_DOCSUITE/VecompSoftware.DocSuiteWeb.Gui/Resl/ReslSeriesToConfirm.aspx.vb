Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
Imports Telerik.Web.UI.Map
Imports VecompSoftware.Helpers
Imports iTextSharp.text.pdf
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers.Web.HtmlStructure
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports System.Web
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslSeriesToConfirm
    Inherits ReslBasePage

#Region "Fields"

    Private Const CONFIRM_REMOVE_LINK_MESSAGE As String = "Attenzione! La conferma rimuoverà il collegamento della Bozza selezionata. Procedere?"
    Private Const REMOVE_DRAFT_COMMAND As String = "removeDraft"
    Private Const PAGE_TITLE As String = "Conferma Bozze Amministrazione Trasparente"
    Private Const MAIN_DOC As String = "Documento Principale"
    Private Const MAIN_DOC_OMISSIS As String = "Documenti Omissis"
    Private Const ATTACHMENT As String = "Allegati (parte integrante)"
    Private Const ATTACHMENT_OMISSIS As String = "Allegati Omissis (parte integrante)"
    Private Const ANNEXES As String = "Annessi (non parte integrante)"
    Private Const SERIES_MAINDOCUMENT_CODE As String = "MAINDOC"
    Private Const SERIES_ANNEXED_CODE As String = "ANX"
    Private Const SERIES_UNPUBLISHED_ANNEXED_CODE As String = "UANX"
#End Region

#Region "Properties"
    Public Property CurrentDocumentSeries As DocumentSeries
        Get
            Return DirectCast(ViewState("CurrentDocumentSeries"), DocumentSeries)
        End Get
        Set(value As DocumentSeries)
            ViewState("CurrentDocumentSeries") = value
        End Set
    End Property

    Public ReadOnly Property CurrentDocumentSeriesItem As DocumentSeriesItem
        Get
            If Not String.IsNullOrEmpty(ddlDraftSeries.SelectedValue) Then
                Return Facade.DocumentSeriesItemFacade.GetById(Integer.Parse(ddlDraftSeries.SelectedValue))
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property SelectedArchiveInfo As ArchiveInfo
        Get
            If CurrentDocumentSeries IsNot Nothing Then
                Return DocumentSeriesFacade.GetArchiveInfo(CurrentDocumentSeries)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property SelectedMainDocument As IList(Of DocumentInfo)
        Get
            Dim mainDocuments As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_MAINDOCUMENT_CODE) Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    If CType(item.FindControl("chkCcDocument"), CheckBox).Checked Then
                        documentInfo = New BiblosPdfDocumentInfo(DirectCast(documentInfo, BiblosDocumentInfo))
                    End If
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    mainDocuments.Add(documentInfo)
                End If
            Next
            Return mainDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedUnpublishedAnnexed As IList(Of DocumentInfo)
        Get
            Dim unpublishedAnnexed As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_UNPUBLISHED_ANNEXED_CODE) Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    If CType(item.FindControl("chkCcDocument"), CheckBox).Checked Then
                        documentInfo = New BiblosPdfDocumentInfo(DirectCast(documentInfo, BiblosDocumentInfo))
                    End If
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    unpublishedAnnexed.Add(documentInfo)
                End If
            Next
            Return unpublishedAnnexed
        End Get
    End Property

    Public ReadOnly Property SelectedAnnexed As IList(Of DocumentInfo)
        Get
            Dim annexed As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_ANNEXED_CODE) Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    If CType(item.FindControl("chkCcDocument"), CheckBox).Checked Then
                        documentInfo = New BiblosPdfDocumentInfo(DirectCast(documentInfo, BiblosDocumentInfo))
                    End If
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    annexed.Add(documentInfo)
                End If
            Next
            Return annexed
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub DdlSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlDraftSeries.SelectedIndexChanged
        If CurrentDocumentSeriesItem Is Nothing Then
            Exit Sub
        End If

        CurrentDocumentSeries = CurrentDocumentSeriesItem.DocumentSeries
        LoadAttributesControls(SelectedArchiveInfo)
        DataBindAttributes(SelectedArchiveInfo, CurrentDocumentSeriesItem)
        BindDocumentGrid()
    End Sub

    Protected Sub DgvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If


        ' imgDocumentExtensionType
        Dim dto As ResolutionDocument = DirectCast(e.Item.DataItem, ResolutionDocument)

        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = TryCast(e.Item, GridDataItem)
            If dto.DocumentGroup.Equals(MAIN_DOC_OMISSIS) Then
                item.Selected = True
            End If
        End If


        With DirectCast(e.Item.FindControl("lblDocumentName"), Label)
            .Text = dto.DocumentName
        End With

        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        documentTypeModControl.Items.Clear()
        documentTypeModControl.Visible = False

        If CurrentDocumentSeries IsNot Nothing AndAlso CurrentDocumentSeries.Container IsNot Nothing Then
            'Documento principale
            If CurrentDocumentSeries.Container.DocumentSeriesLocation IsNot Nothing Then
                documentTypeModControl.Visible = True
                Dim mainCaption As String = String.Empty
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain)) Then
                    mainCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.MainChain)
                End If
                documentTypeModControl.Items.Add(New DropDownListItem(mainCaption, SERIES_MAINDOCUMENT_CODE))
            End If

            'Annesso
            If CurrentDocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                documentTypeModControl.Visible = True
                Dim annexedCaption As String = String.Empty
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain)) Then
                    annexedCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
                End If
                documentTypeModControl.Items.Add(New DropDownListItem(annexedCaption, SERIES_ANNEXED_CODE))
            End If

            'Annesso da non pubblicare
            If CurrentDocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                documentTypeModControl.Visible = True
                Dim unpublishedAnnexedCaption As String = String.Empty
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)) Then
                    unpublishedAnnexedCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
                End If
                documentTypeModControl.Items.Add(New DropDownListItem(unpublishedAnnexedCaption, SERIES_UNPUBLISHED_ANNEXED_CODE))
            End If
        End If

    End Sub

    Protected Sub BtnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        ConfirmDraftSeriesItem()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            BindSeriesToComplete(False)
            Initialize()
        End If
        'Ricarico i controlli dinamici ad ogni postback
        LoadAttributesControls(SelectedArchiveInfo)
        If Not IsPostBack Then
            DataBindAttributes(SelectedArchiveInfo, CurrentDocumentSeriesItem)
            BindDocumentGrid()
        End If
    End Sub

    Protected Sub BtnRemoveDraftLink_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRemoveDraftLink.Click
        AjaxAlertConfirm(CONFIRM_REMOVE_LINK_MESSAGE, String.Format("ExecuteAjaxRequest('{0}');", REMOVE_DRAFT_COMMAND), String.Empty, True)
    End Sub

    Protected Sub ReslSeriesToConfirm_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case REMOVE_DRAFT_COMMAND
                Facade.ResolutionDocumentSeriesItemFacade.RemoveLinkResolutionToDocumentSeriesItem(CurrentResolution, CurrentDocumentSeriesItem)
                GoToView()
        End Select
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        Title = PAGE_TITLE
        dgvTitle.Text = String.Format("Documenti {0}", Facade.TabMasterFacade.TreeViewCaption)

        If CurrentResolution Is Nothing Then
            Throw New DocSuiteException("Nessuna Resolution trovata per l'Identificativo passato")
        End If

        If CurrentDocumentSeriesItem IsNot Nothing Then
            CurrentDocumentSeries = CurrentDocumentSeriesItem.DocumentSeries
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ReslSeriesToConfirm_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvDocuments, dgvDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDraftSeries, pnlPageContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDraftSeries, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlResolutionDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, pnlPageContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRemoveDraftLink, pnlPageContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRemoveDraftLink, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DynamicControls)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPageContainer, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ddlDraftSeries)
    End Sub

    Private Sub BindSeriesToComplete(Optional needBindind As Boolean = True)
        ddlDraftSeries.Items.Clear()
        If Not Facade.ResolutionFacade.HasSeriesToComplete(CurrentResolution) Then
            FileLogger.Warn(LoggerName, "Nessuna Bozza da completare")
            Exit Sub
        End If

        Dim seriesToComplete As ICollection(Of DocumentSeriesItem) = Facade.ResolutionFacade.GetSeriesToComplete(CurrentResolution)
        For Each seriesItem As DocumentSeriesItem In seriesToComplete
            Dim textName As String = String.Format("{0} : {1} del {2:dd/MM/yyyy}", seriesItem.DocumentSeries.Name, seriesItem.Id, seriesItem.RegistrationDate)
            ddlDraftSeries.Items.Add(New DropDownListItem(textName, seriesItem.Id.ToString()))
        Next

        If HasAvcpSeriesToAutomaticConfirm() Then
            Dim avcpSeries As DocumentSeriesItem = seriesToComplete.SingleOrDefault(Function(s) s.DocumentSeries.Id.Equals(ProtocolEnv.AvcpDocumentSeriesId))
            Dim avcpItem As DropDownListItem = ddlDraftSeries.FindItemByValue(avcpSeries.Id.ToString())
            ddlDraftSeries.Items.Remove(avcpItem)
        End If

        ddlDraftSeries.Items.OrderBy(Function(o) o.Text)
        If needBindind Then
            Call DdlSeries_SelectedIndexChanged(Me.Page, Nothing)
        End If
    End Sub

    Private Sub BindDocumentGrid()
        pnlResolutionDocuments.Visible = True
        dgvDocuments.DataSource = Nothing
        dgvDocuments.DataBind()

        Dim isDocumentRequired As Boolean = New ResolutionKindDocumentSeriesFacade(DocSuiteContext.Current.User.FullUserName).IsDocumentRequired(CurrentResolution.ResolutionKind.Id, CurrentDocumentSeries.Id)
        If Not isDocumentRequired Then
            pnlResolutionDocuments.Visible = False
            Exit Sub
        End If

        Dim incremental As Short = Facade.ResolutionWorkflowFacade.GetActiveIncremental(CurrentResolution.Id, 1)
        Dim resolutionDocuments As List(Of ResolutionDocument) = New List(Of ResolutionDocument)

        resolutionDocuments.AddRange(Facade.ResolutionFacade.GetResolutionMainDocumentDtos(CurrentResolution, MAIN_DOC, incremental))
        Dim previousIncremental As Integer? = GetPreviousIncremental(CurrentResolution, incremental)
        If previousIncremental.HasValue Then
            resolutionDocuments.AddRange(Facade.ResolutionFacade.GetResolutionMainDocumentDtos(CurrentResolution, MAIN_DOC, previousIncremental.Value))
        End If

        resolutionDocuments.AddRange(Facade.ResolutionFacade.GetResolutionDocumentOmissisDtos(CurrentResolution, MAIN_DOC_OMISSIS, incremental))
        resolutionDocuments.AddRange(Facade.ResolutionFacade.GetResolutionAttachmentDtos(CurrentResolution, ATTACHMENT, incremental))
        resolutionDocuments.AddRange(Facade.ResolutionFacade.GetResolutionAttachmentOmissisDtos(CurrentResolution, ATTACHMENT_OMISSIS, incremental))
        resolutionDocuments.AddRange(Facade.ResolutionFacade.GetResolutionAnnexesDtos(CurrentResolution, ANNEXES, incremental))

        dgvDocuments.DataSource = resolutionDocuments
        dgvDocuments.DataBind()
    End Sub

    ''' <summary> Popola la pagina con i campi dinamici della Serie </summary>
    Private Sub LoadAttributesControls(archive As ArchiveInfo)
        If archive Is Nothing Then
            Return
        End If
        ' Metodo per cancellazione dei dati in ViewState dei controlli figli di DynamicControl
        DynamicControls.Controls.Clear()

        Dim attributes As List(Of ArchiveAttribute) = archive.VisibleChainAttributes
        If attributes.Count = 0 Then
            Exit Sub
        End If

        Dim table As Table = CreateDynamicTable()

        Dim attributeEnums As List(Of DocumentSeriesAttributeEnum) = (New DocumentSeriesAttributeEnumFacade).GetByDocumentSeries(CurrentDocumentSeries.Id).ToList()

        For Each attribute As ArchiveAttribute In archive.VisibleChainAttributes
            Dim attributeLabel As String = String.Format("{0}:", attribute.Label)
            If attributeEnums.Exists(Function(x) x.AttributeName.Eq(attribute.Name)) Then
                Dim attributeType As DocumentSeriesAttributeEnum = attributeEnums.Single(Function(x) x.AttributeName.Eq(attribute.Name))
                Select Case attributeType.EnumType
                    Case AttributeEnumTypes.Checkbox
                        Dim label As Control = New LabelStructure().GetStructure(Guid.NewGuid().ToString(), attributeLabel, String.Empty)
                        Dim checkBox As Control = New CheckBoxStructure().GetStructure(attribute.Id.ToString("N"), New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), False, String.Empty)
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, checkBox}, {"DocumentSeriesLabel"})

                    Case AttributeEnumTypes.Combo
                        Dim comboValues As IDictionary(Of String, String) = New Dictionary(Of String, String)
                        For Each value As DocumentSeriesAttributeEnumValue In attributeType.EnumValues
                            comboValues.Add(value.Description, value.AttributeValue.ToString())
                        Next

                        Dim label As Control = New LabelStructure().GetStructure(Guid.NewGuid().ToString(), attributeLabel, String.Empty)
                        Dim combo As Control = New ComboStructure().GetStructure(attribute.Id.ToString("N"), New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), comboValues, String.Empty)
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, combo}, {"DocumentSeriesLabel"})
                End Select

            Else
                Select Case attribute.DataType
                    Case "System.String"
                        Dim label As Control = New LabelStructure().GetStructure(Guid.NewGuid().ToString(), attributeLabel, String.Empty)
                        Dim textBox As Control = New TextStructure().GetRadStructure(attribute.Id.ToString("N"), New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), Nothing, String.Empty, InputMode.MultiLine, String.Empty)
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, textBox}, {"DocumentSeriesLabel"})
                    Case "System.DateTime"
                        Dim label As Control = New LabelStructure().GetStructure(Guid.NewGuid().ToString(), attributeLabel, String.Empty)
                        Dim textBox As Control = New DateTimeStructure().GetRadStructure(attribute.Id.ToString("N"), Nothing, Nothing, String.Empty)
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, textBox}, {"DocumentSeriesLabel"})
                    Case "System.Int64"
                        Dim label As Control = New LabelStructure().GetStructure(Guid.NewGuid().ToString(), attributeLabel, String.Empty)
                        Dim textBox As Control = New NumericStructure().GetRadStructure(attribute.Id.ToString("N"), New KeyValuePair(Of Integer, UnitType)(150, UnitType.Pixel), Nothing, Nothing, String.Empty)
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, textBox}, {"DocumentSeriesLabel"})
                    Case "System.Double"
                        Dim label As Control = New LabelStructure().GetStructure(Guid.NewGuid().ToString(), attributeLabel, String.Empty)
                        Dim textBox As Control = New NumericStructure().GetRadStructure(attribute.Id.ToString("N"), New KeyValuePair(Of Integer, UnitType)(150, UnitType.Pixel), Nothing, Nothing, String.Empty)
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, textBox}, {"DocumentSeriesLabel"})
                End Select

            End If

            If attribute.Required Then
                Dim control As Control = DynamicControls.FindControl(WebHelper.SafeControlIdName(attribute.Id.ToString("N")))
                Dim validator As Control = New ValidatorStructure().GetStructure(control.ClientID, String.Format("Campo {0} Obbligatorio", attribute.Label), attribute.Id.ToString("N"), ValidatorDisplay.Dynamic, String.Empty)
                table.Rows.AddRaw("Chiaro", {2}, Nothing, {validator}, Nothing)
            End If
        Next

    End Sub

    Private Function CreateDynamicTable() As Table
        Dim table As New Table
        table.CssClass = "datatable"
        table.EnableViewState = False
        DynamicControls.Controls.Add(table)

        Dim tr As New TableHeaderRow
        table.Rows.Add(tr)

        Dim th As New TableHeaderCell
        th.Text = "Dati dinamici"
        th.ColumnSpan = 2
        tr.Cells.Add(th)

        Return table
    End Function

#Region "Item.aspx Methods"
    Private Shared Function GetAttributeValue(attributes As Dictionary(Of String, String), searched As ArchiveAttribute) As Object
        If Not attributes.ContainsKey(searched.Name) Then
            Return Nothing
        End If

        Dim val As String = attributes.Item(searched.Name)

        Select Case searched.DataType
            Case "System.String"
                Return val
            Case "System.DateTime"
                Return DateTime.Parse(val)
            Case "System.Int64"
                Return Int64.Parse(val)
            Case "System.Double"
                Return Double.Parse(val)
        End Select

        Return val
    End Function

    Private Sub SetControlValue(control As Control, value As Object)
        Dim checkBox As CheckBox = TryCast(control, CheckBox)
        If (checkBox IsNot Nothing) Then
            If value Is Nothing Then
                checkBox.Checked = False
            Else
                checkBox.Checked = (DirectCast(value, Long) = 1)
            End If
        End If

        Dim comboBox As DropDownList = TryCast(control, DropDownList)
        If (comboBox IsNot Nothing) Then
            If value Is Nothing Then
                comboBox.SelectedValue = ""
            Else
                comboBox.SelectedValue = value.ToString()
            End If
        End If

        Dim radTextBox As RadTextBox = TryCast(control, RadTextBox)
        If (radTextBox IsNot Nothing) Then
            If value Is Nothing Then
                radTextBox.Text = String.Empty
            Else
                radTextBox.Text = DirectCast(value, String)
                ' Calcolo delle righe da mettere nel campo testuale. Se supero i multipli di 100 lettere creo una nuova riga
                radTextBox.Rows = Convert.ToInt32(Math.Floor(DirectCast(radTextBox.Text, String).Length / 100)) + 1
            End If
            Return
        End If

        Dim radDatePicker As RadDatePicker = TryCast(control, RadDatePicker)
        If (radDatePicker IsNot Nothing) Then
            If value Is Nothing Then
                radDatePicker.SelectedDate = Nothing
            Else
                radDatePicker.SelectedDate = DirectCast(value, DateTime)
            End If
            Return
        End If

        Dim radNumericTextBox As RadNumericTextBox = TryCast(control, RadNumericTextBox)
        If (radNumericTextBox IsNot Nothing) Then

            If value Is Nothing Then
                radNumericTextBox.Value = Nothing
            Else
                If value Is Nothing Then
                    radNumericTextBox.Value = Nothing
                Else
                    radNumericTextBox.Value = DirectCast(value, Long)
                End If
            End If
            Return
        End If
    End Sub

    Private Shared Function GetControlValue(source As Control) As Object

        Dim checkBox As CheckBox = TryCast(source, CheckBox)
        If (checkBox IsNot Nothing) Then
            If checkBox.Checked Then
                Return 1
            End If
            Return 0
        End If

        Dim comboBox As DropDownList = TryCast(source, DropDownList)
        If (comboBox IsNot Nothing) Then
            If String.IsNullOrEmpty(comboBox.SelectedValue) OrElse comboBox.SelectedValue = "-1" Then
                Return Nothing
            End If
            Return Integer.Parse(comboBox.SelectedValue)
        End If

        Dim radTextBox As RadTextBox = TryCast(source, RadTextBox)
        If (radTextBox IsNot Nothing) AndAlso Not String.IsNullOrEmpty(radTextBox.Text) Then
            Return radTextBox.Text
        End If

        Dim radNumericTextBox As RadNumericTextBox = TryCast(source, RadNumericTextBox)
        If (radNumericTextBox IsNot Nothing) AndAlso radNumericTextBox.Value.HasValue Then
            Return radNumericTextBox.Value
        End If

        Dim radDatePicker As RadDatePicker = TryCast(source, RadDatePicker)
        If (radDatePicker IsNot Nothing) AndAlso radDatePicker.SelectedDate.HasValue Then
            Return radDatePicker.SelectedDate.Value
        End If

        FileLogger.Debug(LoggerName, String.Format("Controllo [{0}] in Item non gestito correttamente", source.ID))
        Return Nothing

    End Function

    Private Sub DataBindAttributes(ByVal archive As ArchiveInfo, ByVal item As DocumentSeriesItem)
        Dim attributes As Dictionary(Of String, String) = Facade.DocumentSeriesItemFacade.GetAttributes(item)
        For Each a As ArchiveAttribute In archive.VisibleChainAttributes
            If Not attributes.ContainsKey(a.Name) Then
                Continue For
            End If

            Dim val As Object = GetAttributeValue(attributes, a)
            If val Is Nothing Then
                Continue For
            End If

            Dim control As Control = DynamicControls.FindControl(WebHelper.SafeControlIdName(a.Id.ToString("N")))
            If (TypeOf control Is Label) AndAlso (TypeOf val Is Long) Then
                Dim tempInt As Integer = DirectCast(val, Long)
                Dim desc As String = (New DocumentSeriesAttributeEnumFacade).GetValueDescription(item.DocumentSeries.Id, a.Name, tempInt)
                If Not String.IsNullOrEmpty(desc) Then
                    val = desc
                End If
            End If

            SetControlValue(control, val)
        Next
    End Sub

    Private Shared Function GetControlValueFormatted(source As Control, format As String) As String
        If String.IsNullOrEmpty(format) Then
            format = "{0}"
        End If

        Dim val As Object = GetControlValue(source)
        If val IsNot Nothing Then
            Return String.Format(format, val)
        End If

        Return Nothing
    End Function

    Private Sub FillDataFromPage(chain As BiblosChainInfo)
        For Each attribute As ArchiveAttribute In SelectedArchiveInfo.VisibleChainAttributes
            Dim control As Control = DynamicControls.FindControl(WebHelper.SafeControlIdName(attribute.Id.ToString("N")))
            If control Is Nothing Then Continue For ' Controllo non trovato nella pagina
            Dim val As String = GetControlValueFormatted(control, attribute.Format) ' Recupero il valore del controllo
            chain.AddAttribute(attribute.Name, val)
        Next
    End Sub

    ''' <summary>
    ''' Imposto la data di pubblicazione della serie documentale. 
    ''' La data sarà 15 giorni successiva alla data di pubblicazione dell'atto associato.
    ''' </summary>
    ''' <param name="item"></param>
    Private Sub SetPublishingDate(ByRef item As DocumentSeriesItem)
        'Imposto la data di pubblicazione quando l'atto sarà ritirato (se non è attivo il parametro CompleteTransparencyExecutiveStepEnabled)
        If CurrentResolution.PublishingDate.HasValue Then
            item.PublishingDate = CurrentResolution.PublishingDate.Value.AddDays(15)
            If DocSuiteContext.Current.ResolutionEnv.CompleteTransparencyExecutiveStepEnabled AndAlso CurrentResolution.EffectivenessDate.HasValue Then
                item.PublishingDate = CurrentResolution.EffectivenessDate
            End If
        End If
    End Sub
#End Region

    Private Sub ConfirmDraftAvcpSeriesItem()
        'Se sto confermando Bandi di Gara ed ho una serie AVCP associata
        If Not HasAvcpSeriesToAutomaticConfirm() Then
            Exit Sub
        End If

        Dim seriesToComplete As ICollection(Of DocumentSeriesItem) = Facade.ResolutionFacade.GetSeriesToComplete(CurrentResolution)
        Dim avcpSeriesItem As DocumentSeriesItem = seriesToComplete.SingleOrDefault(Function(s) s.DocumentSeries.Id.Equals(ProtocolEnv.AvcpDocumentSeriesId))
        'Imposto la data di pubblicazione quando l'atto sarà ritirato (se non è attivo il parametro CompleteTransparencyExecutiveStepEnabled)
        avcpSeriesItem.PublishingDate = CurrentResolution.PublishingDate.Value.AddDays(15)
        If DocSuiteContext.Current.ResolutionEnv.CompleteTransparencyExecutiveStepEnabled AndAlso CurrentResolution.EffectivenessDate.HasValue Then
            avcpSeriesItem.PublishingDate = CurrentResolution.EffectivenessDate
        End If
        Dim avcpChain As BiblosChainInfo = Facade.DocumentSeriesItemFacade.GetMainChainInfo(avcpSeriesItem)
        Facade.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(avcpSeriesItem, avcpChain, Nothing, Nothing, $"Pubblicata registrazione AVCP {avcpSeriesItem.Year:0000}/{avcpSeriesItem.Number:0000000} in data {avcpSeriesItem.PublishingDate:dd/MM/yyyy}")
        Facade.DocumentSeriesItemFacade.AssignNumber(avcpSeriesItem)
        Facade.DocumentSeriesItemFacade.SendInsertDocumentSeriesItemCommand(avcpSeriesItem)
    End Sub

    Public Sub ConfirmDraftSeriesItem()
        If Not Page.IsValid Then
            AjaxAlert("Errore in fase di salvataggio")
            Exit Sub
        End If

        If CurrentDocumentSeriesItem Is Nothing Then
            AjaxAlert("Nessuna Bozza selezionata per il salvataggio")
            Exit Sub
        End If

        Dim seriesItem As DocumentSeriesItem = CurrentDocumentSeriesItem
        Dim isDocumentRequired As Boolean = New ResolutionKindDocumentSeriesFacade(DocSuiteContext.Current.User.FullUserName).IsDocumentRequired(CurrentResolution.ResolutionKind.Id, seriesItem.DocumentSeries.Id)
        Dim chain As BiblosChainInfo = Facade.DocumentSeriesItemFacade.GetMainChainInfo(seriesItem)

        If isDocumentRequired Then
            If SelectedMainDocument IsNot Nothing AndAlso SelectedMainDocument.Any() Then
                chain.AddDocuments(SelectedMainDocument.ToList())
            Else
                AjaxAlert("Selezionare almeno un documento da aggiungere alla Serie Documentale.")
                Exit Sub
            End If
        End If

        FillDataFromPage(chain)
        SetPublishingDate(seriesItem)

        If seriesItem.DocumentSeries.Id.Equals(ProtocolEnv.BandiGaraDocumentSeriesId) Then
            ConfirmDraftAvcpSeriesItem()
        End If

        If isDocumentRequired Then
            Facade.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(seriesItem, chain, SelectedAnnexed.ToList(), SelectedUnpublishedAnnexed.ToList(), $"Pubblicata registrazione {seriesItem.Year:0000}/{seriesItem.Number:0000000} in data {seriesItem.PublishingDate:dd/MM/yyyy}")
        Else
            Facade.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(seriesItem, chain, Nothing, Nothing, $"Pubblicata registrazione {seriesItem.Year:0000}/{seriesItem.Number:0000000} in data {seriesItem.PublishingDate:dd/MM/yyyy}")
        End If

        ' Assegno anno e numero incrementali e confermo la serie documentale.
        Facade.DocumentSeriesItemFacade.AssignNumber(seriesItem)
        Facade.DocumentSeriesItemFacade.SendInsertDocumentSeriesItemCommand(seriesItem)
        GoToView()
    End Sub

    Private Sub GoToView()
        If Not Facade.ResolutionFacade.HasSeriesToComplete(CurrentResolution) Then
            Dim params As String = String.Format("IdResolution={0}&Type=Resl", CurrentResolution.Id)
            Dim url As String = String.Format("../Resl/{0}?{1}", GetViewPageName(CurrentResolution.Id), CommonShared.AppendSecurityCheck(params))
            Response.Redirect(url)
        End If

        BindSeriesToComplete()
    End Sub

    ''' <summary> Indica se per lo step indicato sia necessario visualizzare i documenti dello step precedente. </summary>
    ''' <param name="resl">Resolution di riferimento</param>
    ''' <param name="incr">Identificativo dello step</param>
    ''' <returns>Restituisce nothing nel caso non sia da visualizzare, altrimenti l'identificativo dello step da visualizzare.</returns>
    Private Function GetPreviousIncremental(resl As Resolution, incr As Short) As Short?
        Dim wf As ResolutionWorkflow = Facade.ResolutionWorkflowFacade.GetById(New ResolutionWorkflowCompositeKey() With {.IdResolution = resl.Id, .Incremental = incr})
        Return wf.IncrementalFather
    End Function

    Private Function HasAvcpSeriesToAutomaticConfirm() As Boolean
        If Not ResolutionEnv.AutomaticConfirmAvcpSeries Then Return False
        Dim seriesToComplete As ICollection(Of DocumentSeriesItem) = Facade.ResolutionFacade.GetSeriesToComplete(CurrentResolution)
        If Not seriesToComplete.Count > 0 Then Return False
        Dim checkSeries As ICollection(Of DocumentSeriesItem) = seriesToComplete.Where(Function(x) (x.DocumentSeries.Id.Equals(ProtocolEnv.AvcpDocumentSeriesId) OrElse _
                                                                                                    x.DocumentSeries.Id.Equals(ProtocolEnv.BandiGaraDocumentSeriesId))) _
                                                                                            .ToList()

        Return checkSeries.Count > 1
    End Function
#End Region

End Class