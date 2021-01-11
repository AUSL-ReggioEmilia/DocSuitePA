Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos.Models

Namespace Series
    Public Class Search
        Inherits CommBasePage

#Region " Fields "
        Private _selectedDocumentSeriesSubsection As DocumentSeriesSubsection
        Private _currentSeries As DocumentSeries
        Private _documentSeriesAttributeEnumFacade As DocumentSeriesAttributeEnumFacade
#End Region

#Region " Properties "

        Public ReadOnly Property DocumentSeriesAttributeEnumFacade() As DocumentSeriesAttributeEnumFacade
            Get
                If _documentSeriesAttributeEnumFacade Is Nothing Then
                    _documentSeriesAttributeEnumFacade = New DocumentSeriesAttributeEnumFacade()
                End If
                Return _documentSeriesAttributeEnumFacade
            End Get
        End Property

        Private ReadOnly Property SelectedDocumentSeries As DocumentSeries
            Get
                If _currentSeries Is Nothing AndAlso Not String.IsNullOrEmpty(ddlDocumentSeries.SelectedValue) Then
                    _currentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(CType(ddlDocumentSeries.SelectedValue, Integer))
                End If
                Return _currentSeries
            End Get
        End Property

        Private ReadOnly Property SelectedDocumentSeriesSubsection As DocumentSeriesSubsection
            Get
                If _selectedDocumentSeriesSubsection Is Nothing AndAlso Not String.IsNullOrEmpty(ddlSubsection.SelectedValue) Then
                    _selectedDocumentSeriesSubsection = Facade.DocumentSeriesSubsectionFacade.GetById(CType(ddlSubsection.SelectedValue, Integer))
                End If
                Return _selectedDocumentSeriesSubsection
            End Get
        End Property

        Private ReadOnly Property DefaultDocumentSeriesId As Integer?
            Get
                Return Request.QueryString.GetValueOrDefault(Of Integer?)("DocumentSeries", Nothing)
            End Get
        End Property

        Private ReadOnly Property DefaultYear As Integer?
            Get
                Return Request.QueryString.GetValueOrDefault(Of Integer?)("Year", Nothing)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Action.Eq("CopyDocuments") Then
                MasterDocSuite.TitleVisible = False
            End If

            InitializeAjax()

            ' Al primo caricamento della pagina devo caricare l'elenco delle Serie documentali disponibili per la ricerca
            If Not IsPostBack Then
                Title = ProtocolEnv.DocumentSeriesName & " - Ricerca"
                DocumentSeries.Text = ProtocolEnv.DocumentSeriesName & ":"

                ddlContainerArchive.DataValueField = "Id"
                ddlContainerArchive.DataTextField = "Name"
                ddlContainerArchive.DataSource = Facade.ContainerArchiveFacade.GetAllOrdered("Name ASC")
                ddlContainerArchive.DataBind()
                ddlContainerArchive.Items.Insert(0, "")
                SetContainerArchives(Nothing)

                txtYear.Value = If(DefaultYear.HasValue, DefaultYear.Value, DateTime.Now.Year)

                ' Carico la struttura della Serie
                If DefaultDocumentSeriesId.HasValue Then
                    Dim defaultDocumentSeries As DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentByContainerId(DefaultDocumentSeriesId.Value)
                    If defaultDocumentSeries.Container IsNot Nothing AndAlso defaultDocumentSeries.Container.Archive IsNot Nothing Then
                        SetDocumentSeries(defaultDocumentSeries)
                    End If
                End If

                ' Setto il finder SSE presente in sessione
                SetFinder()
            End If

            ' Carico la struttura della Serie
            trIncludeCancelled.Visible = False
            If SelectedDocumentSeries IsNot Nothing Then
                ' campo non sensibile al cambio di Serie
                trIncludeCancelled.Visible = DocumentSeriesItemRights.CheckDocumentSeriesRight(SelectedDocumentSeries, DocumentSeriesContainerRightPositions.ViewCanceled)
                FillDynamicDataStructure(DocumentSeriesFacade.GetArchiveInfo(SelectedDocumentSeries))
            End If
        End Sub

        Private Sub BtnSearchClick(sender As Object, e As EventArgs) Handles btnSearch.Click
            Dim finder As DocumentSeriesItemFinder = GetFinder()
            SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.DocumentSeriesFinderType)
            ClearSessions(Of SearchResult)()
            Response.Redirect(String.Format("../Series/SearchResult.aspx?Type=Series&Action={0}", Action), True)
        End Sub

        Private Sub BtnClearClick(sender As Object, e As EventArgs) Handles btnClear.Click
            SessionSearchController.SaveSessionFinder(Nothing, SessionSearchController.SessionFinderType.DocumentSeriesFinderType)

            ' Effettuo il clear dei controlli base nella pagina aspx corrente
            ClearControl(Form)

            ' Effettuo il clear dei controlli composti
            uscRoleOwner.SourceRoles.Clear()
            uscRoleOwner.DataBind()
            uscCategory.Clear()
            uscCategory.DataBind()
            DynamicControls.Controls.Clear()
        End Sub

        Private Sub ddlContainerArchive_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlContainerArchive.SelectedIndexChanged
            Dim archive As ContainerArchive = Nothing
            Dim idArchive As Integer
            If Integer.TryParse(ddlContainerArchive.SelectedValue, idArchive) Then
                archive = Facade.ContainerArchiveFacade.GetById(idArchive)
            End If

            SetContainerArchives(archive)
            SetDocumentSeries(SelectedDocumentSeries)
        End Sub

        Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
            SetDocumentSeries(SelectedDocumentSeries)
        End Sub

        Protected Sub uscCategory_CategoryAdding(sender As Object, args As EventArgs) Handles uscCategory.CategoryAdding
            uscCategory.Year = Nothing
            If txtYear.Value.HasValue Then
                uscCategory.Year = Convert.ToInt32(txtYear.Value)
            End If

            uscCategory.FromDate = Nothing
            If txtPublishingDateFrom.SelectedDate.HasValue Then
                uscCategory.FromDate = txtPublishingDateFrom.SelectedDate.Value
            End If

            If txtRetireDateFrom.SelectedDate.HasValue Then
                If uscCategory.FromDate Is Nothing OrElse uscCategory.FromDate > txtRetireDateFrom.SelectedDate Then
                    uscCategory.FromDate = txtRetireDateFrom.SelectedDate
                End If
            End If

            If rdpRegistrationFrom.SelectedDate.HasValue Then
                If uscCategory.FromDate Is Nothing OrElse uscCategory.FromDate > rdpRegistrationFrom.SelectedDate Then
                    uscCategory.FromDate = rdpRegistrationFrom.SelectedDate
                End If
            End If

            uscCategory.ToDate = Nothing
            If txtPublishingDateTo.SelectedDate.HasValue Then
                uscCategory.ToDate = txtPublishingDateTo.SelectedDate.Value
            End If

            If txtRetireDateTo.SelectedDate.HasValue Then
                If uscCategory.FromDate Is Nothing OrElse uscCategory.ToDate > txtRetireDateTo.SelectedDate Then
                    uscCategory.ToDate = txtRetireDateTo.SelectedDate
                End If
            End If

            If rdpRegistrationTo.SelectedDate.HasValue Then
                If uscCategory.FromDate Is Nothing OrElse uscCategory.ToDate > rdpRegistrationTo.SelectedDate Then
                    uscCategory.ToDate = rdpRegistrationTo.SelectedDate
                End If
            End If
        End Sub
#End Region

#Region " Methods "

        Private Sub InitializeAjax()

            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, MainContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, MainContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, MainContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(btnClear, MainContentWrapper, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnClear, ButtonsPanel, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscRoleOwner, uscRoleOwner)
        End Sub

        Private Sub SetContainerArchives(archive As ContainerArchive)
            ddlContainerArchive.SelectedValue = If(archive IsNot Nothing, archive.Id.ToString(), "")

            Dim preselectedValue As String = ddlDocumentSeries.SelectedValue
            ' Carico l'elenco delle serie documentali su cui ho diritto di visualizzazione
            Dim series As IList(Of DocumentSeries) = Facade.DocumentSeriesFacade.GetAllOrdered("Name ASC")
            Dim availableContainers As IList(Of Container) = series.Where(Function(S) archive Is Nothing OrElse (S.Container.Archive IsNot Nothing AndAlso S.Container.Archive.Id = archive.Id)).Select(Function(S) S.Container).ToList()
            availableContainers.Insert(0, New Container() With {.Id = -1, .Name = ""})

            ddlDocumentSeries.DataValueField = "Id"
            ddlDocumentSeries.DataTextField = "Name"
            ddlDocumentSeries.DataSource = availableContainers
            ddlDocumentSeries.DataBind()

            If ddlDocumentSeries.Items.FindByValue(preselectedValue) IsNot Nothing Then
                ddlDocumentSeries.SelectedValue = preselectedValue
            End If
        End Sub

        ''' <summary> Gestisce i controlli sensibili al cambio di Serie Documentale. </summary>
        Private Sub SetDocumentSeries(series As DocumentSeries)
            If series Is Nothing Then
                ddlDocumentSeries.SelectedValue = "-1"
                tblPublication.Visible = True
                tblSubsection.Visible = False
                Exit Sub
            End If

            If ddlDocumentSeries.Items.FindByValue(series.Container.Id.ToString()) IsNot Nothing Then
                ddlDocumentSeries.SelectedValue = series.Container.Id.ToString()
            End If

            tblPublication.Visible = series.PublicationEnabled.GetValueOrDefault(False)

            ' Elenco delle Subsection
            If series.SubsectionEnabled.GetValueOrDefault(False) Then
                tblSubsection.Visible = True
                Dim subsections As IList(Of DocumentSeriesSubsection) = Facade.DocumentSeriesSubsectionFacade.GetByDocumentSeries(SelectedDocumentSeries)
                ddlSubsection.DataSource = subsections
                ddlSubsection.DataBind()
                ddlSubsection.Items.Insert(0, "")
                ddlSubsection.SelectedIndex = 0
            Else
                tblSubsection.Visible = False
            End If

        End Sub

        ''' <summary> Popola l'area con i controlli per la ricerca sui Metadati di Biblos </summary>
        ''' <param name="archive"> ArchiveInfo di riferimento </param>
        Private Sub FillDynamicDataStructure(archive As ArchiveInfo)
            DynamicControls.Controls.Clear()

            Dim table As New Table
            table.CssClass = "datatable"
            table.EnableViewState = False

            Dim attributeEnums As List(Of DocumentSeriesAttributeEnum) = DocumentSeriesAttributeEnumFacade.GetByDocumentSeries(SelectedDocumentSeries.Id).ToList()

            For Each a As ArchiveAttribute In archive.VisibleChainAttributes
                If a.Name.Eq("Filename") OrElse a.Name.Eq("Signature") OrElse a.Name.Eq("Position") Then
                    Continue For
                End If

                If attributeEnums.Exists(Function(ae) ae.AttributeName.Eq(a.Name)) Then
                    Dim ae As DocumentSeriesAttributeEnum = attributeEnums.Single(Function(x) x.AttributeName.Eq(a.Name))
                    Select Case ae.EnumType
                        Case AttributeEnumTypes.Checkbox
                            table.Rows.Add(GetCheckboxStructure(a, ae))
                        Case AttributeEnumTypes.Combo
                            table.Rows.Add(GetComboStructure(a, ae))
                    End Select

                Else
                    'todo: prevedere l'utilizzo dei nuovi helper HTML per le strutture dinamiche
                    Select Case a.DataType
                        Case "System.String"
                            table.Rows.Add(GetStringStructure(a))
                        Case "System.DateTime"
                            table.Rows.Add(GetDateTimeStructure(a))
                        Case "System.Int64"
                            table.Rows.Add(GetInt64Structure(a))
                        Case "System.Double"
                            table.Rows.Add(GetDoubleStructure(a))
                    End Select
                End If
            Next

            DynamicControls.Controls.Add(table)
        End Sub

        ''' <summary> Costruisce la struttura base per le componenti dinamiche </summary>
        ''' <param name="identifier">Identificativo del controllo</param>
        ''' <param name="label">Etichetta del controllo</param>
        ''' <param name="contains">Indica se deve essere visualizzato e valorizzato il Check-box "Contiene"</param>
        ''' <param name="innerControls">Elenco dei controlli da aggiungere alla struttura base</param>
        Private Shared Function GetBasicStructure(identifier As String, label As String, contains As Boolean?, innerControls As List(Of WebControl), isNullOrEmpty As Boolean?) As TableRow
            Dim row As New TableRow()
            row.CssClass = "Chiaro"
            row.ID = String.Concat("tr_", identifier)
            row.EnableViewState = False

            ' Label 
            Dim cell1 As New TableCell()
            cell1.ID = String.Concat("td1_", identifier)
            cell1.Style.Add("width", "30%")
            cell1.CssClass = "label"
            cell1.Text = label
            cell1.EnableViewState = False
            row.Cells.AddAt(0, cell1)

            ' Control
            Dim cell2 As New TableCell
            cell2.Style.Add("width", "70%")
            cell2.ID = String.Concat("td2_", identifier)
            For Each item As WebControl In innerControls
                cell2.Controls.Add(item)
            Next

            ' Radio button 
            Dim radioList As RadioButtonList = New RadioButtonList() With {
                .ID = String.Concat("radio_", identifier),
                .RepeatDirection = RepeatDirection.Horizontal,
                .RepeatLayout = RepeatLayout.Flow,
                .AutoPostBack = True
            }

            ' Item Radio Contiene
            If contains.HasValue Then
                Dim listItemContiene As ListItem = New ListItem() With {
                    .Enabled = True,
                    .Selected = True,
                    .Text = "Contiene",
                    .Value = "contains"
                }
                Dim DynamicControlIds As String = "["
                For i As Integer = 0 To innerControls.Count - 1
                    If innerControls.Count - 1 <> 0 Then
                        DynamicControlIds = String.Format("{0}'{1}'{2}", DynamicControlIds, innerControls(i).ClientID, ",")
                    Else
                        DynamicControlIds = String.Format("{0}'{1}'", DynamicControlIds, innerControls(i).ClientID)
                    End If
                Next
                DynamicControlIds = String.Format("{0}{1}", DynamicControlIds, "]")
                listItemContiene.Attributes.Add("onclick", String.Format("disableDynamicsControl({0}, false);", DynamicControlIds))
                radioList.Items.Add(listItemContiene)
            End If

            ' Item Radio Campo vuoto
            If isNullOrEmpty.HasValue Then
                Dim listItemNullOrEmpty As ListItem = New ListItem() With {
                    .Enabled = True,
                    .Selected = False,
                    .Text = "Campo vuoto",
                    .Value = "nullOrEmpty"
                }
                Dim DynamicControlIds As String = "["
                For i As Integer = 0 To innerControls.Count - 1
                    If innerControls.Count - 1 <> 0 Then
                        DynamicControlIds = String.Format("{0}'{1}'{2}", DynamicControlIds, innerControls(i).ClientID, ",")
                    Else
                        DynamicControlIds = String.Format("{0}'{1}'", DynamicControlIds, innerControls(i).ClientID)
                    End If
                Next
                DynamicControlIds = String.Format("{0}{1}", DynamicControlIds, "]")
                listItemNullOrEmpty.Attributes.Add("onclick", String.Format("disableDynamicsControl({0}, true);", DynamicControlIds))
                radioList.Items.Add(listItemNullOrEmpty)
            End If

            ' Se c'è almeno un elemento nella selezione dei radiobutton allora aggiungo la cella
            If (radioList.Items.Count > 0) Then
                cell2.Controls.Add(radioList)
            End If

            cell2.EnableViewState = False
            row.Cells.AddAt(1, cell2)

            Return row
        End Function

        ''' <summary> Costruisce la struttura di un attributo di tipo Stringa </summary>
        ''' <param name="a">Attributo da caricare</param>
        ''' <returns>Restituisce la TableRow con i controlli caricati dinamicamente</returns>
        Private Function GetStringStructure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            Dim label As String = If(String.IsNullOrEmpty(a.Description), a.Name, a.Description)

            Dim txt As New RadTextBox()
            txt.ID = a.Id.ToString("N")
            itemsToAdd.Add(txt)

            Return GetBasicStructure(txt.ID, label, True, itemsToAdd, True)
        End Function

        Private Function GetComboStructure(a As ArchiveAttribute, ae As DocumentSeriesAttributeEnum) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)
            Dim cb As New DropDownList()
            cb.ID = a.Id.ToString("N")
            cb.Width = New Unit(350, UnitType.Pixel)
            cb.Enabled = True

            cb.Items.Add(New ListItem("", "-1"))

            For Each item As DocumentSeriesAttributeEnumValue In ae.EnumValues
                cb.Items.Add(New ListItem(item.Description, item.AttributeValue.ToString()))
            Next

            itemsToAdd.Add(cb)

            Return GetBasicStructure(cb.ID, a.Label, Nothing, itemsToAdd, Nothing)
        End Function

        Private Function GetCheckboxStructure(a As ArchiveAttribute, ae As DocumentSeriesAttributeEnum) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            Dim cb As New CheckBox()
            cb.ID = a.Id.ToString("N")
            cb.Width = New Unit(100, UnitType.Percentage)
            cb.Enabled = True
            itemsToAdd.Add(cb)

            Return GetBasicStructure(cb.ID, a.Label, Nothing, itemsToAdd, Nothing)
        End Function

        ''' <summary>
        ''' Costruisce la struttura di un attributo di tipo DateTime
        ''' </summary>
        ''' <param name="a">Attributo da caricare</param>
        ''' <returns>Restituisce la TableRow con i controlli caricati dinamicamente</returns>
        ''' <remarks></remarks>
        Private Function GetDateTimeStructure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            Dim label As String = If(String.IsNullOrEmpty(a.Description), a.Name, a.Description)

            Dim txt As New RadDatePicker()
            txt.ID = a.Id.ToString("N")
            itemsToAdd.Add(txt)

            Return GetBasicStructure(txt.ID, label, Nothing, itemsToAdd, Nothing)
        End Function

        ''' <summary> Costruisce la struttura di un attributo di tipo Integer </summary>
        ''' <param name="a">Attributo da caricare</param>
        ''' <returns>Restituisce la TableRow con i controlli caricati dinamicamente</returns>
        Private Function GetInt64Structure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            Dim label As String = If(String.IsNullOrEmpty(a.Description), a.Name, a.Description)

            Dim txt As New RadNumericTextBox()
            txt.NumberFormat.DecimalDigits = 0
            txt.ID = a.Id.ToString("N")
            itemsToAdd.Add(txt)

            Return GetBasicStructure(txt.ID, label, Nothing, itemsToAdd, Nothing)

        End Function

        ''' <summary> Costruisce la struttura di un attributo di tipo Double </summary>
        ''' <param name="a">Attributo da caricare</param>
        ''' <returns>Restituisce la TableRow con i controlli caricati dinamicamente</returns>
        Private Function GetDoubleStructure(a As ArchiveAttribute) As TableRow
            Dim itemsToAdd As List(Of WebControl) = New List(Of WebControl)

            Dim label As String = If(String.IsNullOrEmpty(a.Description), a.Name, a.Description)

            Dim txt As New RadNumericTextBox()
            txt.NumberFormat.DecimalDigits = 2
            txt.ID = a.Id.ToString("N")
            itemsToAdd.Add(txt)

            Return GetBasicStructure(txt.ID, label, Nothing, itemsToAdd, Nothing)

        End Function

        ''' <summary> REcupera il valore da un controllo dinamico </summary>
        ''' <param name="source">WebControl da cui recuperare il valore</param>
        ''' <param name="format">Formato del valore da recuperare</param>
        ''' <returns>Restituisce una stringa con il valore inserito formattato secondo parametro</returns>
        Private Shared Function GetValue(source As WebControl, format As String) As String
            If String.IsNullOrEmpty(format) Then
                format = "{0}"
            End If

            Dim checkBox As CheckBox = TryCast(source, CheckBox)
            If (checkBox IsNot Nothing) Then
                If checkBox.Checked Then
                    Return "1"
                End If
                Return "0"
            End If

            Dim comboBox As DropDownList = TryCast(source, DropDownList)
            If (comboBox IsNot Nothing) Then
                If String.IsNullOrEmpty(comboBox.SelectedValue) OrElse comboBox.SelectedValue = "-1" Then
                    Return Nothing
                End If
                Return String.Format(format, Integer.Parse(comboBox.SelectedValue))
            End If

            Dim radTextBox As RadTextBox = TryCast(source, RadTextBox)
            If (radTextBox IsNot Nothing) Then
                Return String.Format(format, radTextBox.Text)
            End If
            Dim radNumericTextBox As RadNumericTextBox = TryCast(source, RadNumericTextBox)
            If (radNumericTextBox IsNot Nothing) Then
                Return String.Format(format, radNumericTextBox.Value)
            End If
            Dim radDateTimePicker As RadDatePicker = TryCast(source, RadDatePicker)
            If (radDateTimePicker IsNot Nothing) Then
                If radDateTimePicker.SelectedDate.HasValue Then
                    Return String.Format(format, radDateTimePicker.SelectedDate.Value)
                End If
            End If
            Return String.Empty
        End Function

        ''' <summary>
        ''' Permette di verificare la presenza del finder in sessione e in caso positivo di ripristinare i filtri selezionati
        ''' </summary>
        Private Sub SetFinder()
            Dim finder As DocumentSeriesItemFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.DocumentSeriesFinderType), DocumentSeriesItemFinder)
            If finder Is Nothing Then
                Exit Sub
            End If

            If finder.IdDocumentSeriesIn Is Nothing OrElse finder.IdDocumentSeriesIn.Count = 0 Then
                Exit Sub
            End If

            ' Recupero Archive
            Dim archive As ContainerArchive = Nothing
            If finder.IdArchive.HasValue Then
                archive = Facade.ContainerArchiveFacade.GetById(finder.IdArchive.Value)
            End If
            SetContainerArchives(archive)

            ' Recupero DocumentSeries
            SetDocumentSeries(Facade.DocumentSeriesFacade.GetById(finder.IdDocumentSeriesIn.FirstOrDefault()))

            ' Filtro SubSection (dopo aver impostato la selected documentseries)
            If SelectedDocumentSeries.SubsectionEnabled AndAlso finder.IdSubsectionIn IsNot Nothing AndAlso finder.IdSubsectionIn.Count > 0 Then
                ddlSubsection.SelectedValue = finder.IdSubsectionIn.First().ToString()
            End If

            ' Recupero stato item
            If finder.ItemStatusIn IsNot Nothing Then
                For Each Item As DocumentSeriesItemStatus In finder.ItemStatusIn
                    If Item = DocumentSeriesItemStatus.Canceled Then
                        chkIncludeCancelled.Checked = True
                    End If
                Next
            End If

            If finder.Year.HasValue Then
                txtYear.Text = finder.Year.ToString()
            End If

            If finder.NumberFrom.HasValue Then
                txtNumberFrom.Text = finder.NumberFrom.ToString()
            End If

            If finder.NumberTo.HasValue Then
                txtNumberTo.Text = finder.NumberTo.ToString()
            End If

            If finder.IsPublished.HasValue Then
                cblPublicationStatus.SelectedValue = If(finder.IsPublished, "PUBLICATED", "NONE")
            End If

            If finder.IsRetired.HasValue AndAlso finder.IsRetired Then
                cblPublicationStatus.SelectedValue = "RETIRED"
            End If

            If finder.RegistrationDateFrom.HasValue Then
                rdpRegistrationFrom.SelectedDate = finder.RegistrationDateFrom.Value.DateTime
            End If

            If finder.RegistrationDateTo.HasValue Then
                rdpRegistrationTo.SelectedDate = finder.RegistrationDateTo.Value.DateTime
            End If

            txtPublishingDateFrom.SelectedDate = finder.PublishingDateFrom

            txtPublishingDateTo.SelectedDate = finder.PublishingDateTo

            txtRetireDateFrom.SelectedDate = finder.RetireDateFrom

            txtRetireDateTo.SelectedDate = finder.RetireDateTo


            chkSubjectContains.Checked = False
            If Not String.IsNullOrEmpty(finder.SubjectContains) Then
                chkSubjectContains.Checked = True
                txtSubject.Text = finder.SubjectContains
            ElseIf Not String.IsNullOrEmpty(finder.SubjectStartsWith) Then
                txtSubject.Text = finder.SubjectStartsWith
            End If

            If Not finder.IdOwnerRoleIn.IsNullOrEmpty() Then
                uscRoleOwner.SourceRoles = Facade.DocumentSeriesItemRoleFacade.GetListByIds(finder.IdOwnerRoleIn).Select(Function(i) i.Role).ToList()
            End If

            If Not String.IsNullOrEmpty(finder.CategoryPath) Then
                Dim array As String() = finder.CategoryPath.Split("|"c)
                uscCategory.DataSource.Add(Facade.CategoryFacade.GetById(Integer.Parse(array(array.Length - 1)), True))
                uscCategory.DataBind()
            End If

        End Sub

        ''' <summary> Istanza il finder per la ricerca </summary>
        ''' <returns>Restituisce il Finder popolato con i valori inseriti dall'operatore</returns>
        Private Function GetFinder() As DocumentSeriesItemFinder
            Dim finder As New DocumentSeriesItemFinder()

            If ProtocolEnv.DocumentSeriesPageSize > 0 Then
                finder.EnablePaging = True
                finder.PageSize = ProtocolEnv.DocumentSeriesPageSize
            End If

            finder.ItemStatusIn = New List(Of DocumentSeriesItemStatus)()
            finder.ItemStatusIn.Add(DocumentSeriesItemStatus.Active)

            If chkIncludeDraft.Checked Then
                finder.ItemStatusIn.Add(DocumentSeriesItemStatus.Draft)
            End If
            If chkIncludeCancelled.Checked Then
                finder.ItemStatusIn.Add(DocumentSeriesItemStatus.Canceled)
            End If

            If Not String.IsNullOrEmpty(ddlContainerArchive.SelectedValue) Then
                finder.IdArchive = Integer.Parse(ddlContainerArchive.SelectedValue)
            End If

            If SelectedDocumentSeries IsNot Nothing Then
                Dim archive As ArchiveInfo = DocumentSeriesFacade.GetArchiveInfo(SelectedDocumentSeries)
                finder.IdDocumentSeriesIn = New List(Of Integer) From {SelectedDocumentSeries.Id}

                Dim conditions As New List(Of SearchCondition)
                For Each attribute As ArchiveAttribute In archive.Attributes
                    Dim control As WebControl = CType(DynamicControls.FindControl(attribute.Id.ToString("N")), WebControl)
                    If control IsNot Nothing Then
                        Dim val As String = GetValue(control, attribute.Format)

                        Dim condition As New SearchCondition() With {
                                .AttributeName = attribute.Name,
                                .AttributeValue = val
                            }

                        Dim radioList As RadioButtonList = CType(DynamicControls.FindControl("radio_" & attribute.Id.ToString("N")), RadioButtonList)
                        ' In caso di filtro con radio button:
                        If radioList IsNot Nothing Then
                            For Each radio As ListItem In radioList.Items
                                If radio.Selected Then
                                    Select Case radio.Value
                                        Case "contains"
                                            ' Eseguo il filtro solamente se è presente del testo nella textbox
                                            If Not String.IsNullOrEmpty(val) Then
                                                condition.Operator = SearchConditionOperator.Contains
                                                conditions.Add(condition)
                                            End If
                                        Case "nullOrEmpty"
                                            condition.Operator = SearchConditionOperator.IsNullOrEmpty
                                            conditions.Add(condition)
                                        Case Else
                                            ' Eseguo il filtro solamente se è presente del testo nella textbox
                                            If Not String.IsNullOrEmpty(val) Then
                                                condition.Operator = SearchConditionOperator.IsEqualTo
                                                conditions.Add(condition)
                                            End If
                                    End Select
                                End If
                            Next
                        Else
                            ' In caso di filtro senza radio button: (data,testo)
                            ' Verifico che l'attributo sia valorizzato
                            If (condition.AttributeValue IsNot Nothing AndAlso Not String.Compare(attribute.DataType, GetType(String).ToString()) = 0) AndAlso Not String.IsNullOrEmpty(DirectCast(condition.AttributeValue, String)) Then
                                condition.Operator = SearchConditionOperator.IsEqualTo
                                conditions.Add(condition)
                            End If
                        End If

                    End If
                Next

                If conditions.Count > 0 Then
                    Facade.DocumentSeriesFacade.FillFinder(finder, conditions)
                End If

                If SelectedDocumentSeries.SubsectionEnabled AndAlso SelectedDocumentSeriesSubsection IsNot Nothing Then
                    finder.IdSubsectionIn = New List(Of Integer)
                    finder.IdSubsectionIn.Add(SelectedDocumentSeriesSubsection.Id)
                End If
            End If

            If chkPriority.Checked Then
                finder.IsPriority = True
            End If

            If txtYear.Value.HasValue Then
                finder.Year = CType(txtYear.Value, Integer?)
            End If

            If txtNumberFrom.Value.HasValue Then
                finder.NumberFrom = CType(txtNumberFrom.Value, Integer?)
            End If

            If txtNumberTo.Value.HasValue Then
                finder.NumberTo = CType(txtNumberTo.Value, Integer?)
            End If

            If clbOriginType.SelectedItem IsNot Nothing Then
                For Each Item As ListItem In clbOriginType.Items
                    If Item.Selected Then
                        finder.LinkTypes.Add(DirectCast(Short.Parse(Item.Value), DocumentSeriesItemLinkType))
                    End If
                Next

            End If

            If Not String.IsNullOrEmpty(txtOrginText.Text) Then
                finder.LinkContains = txtOrginText.Text
            End If

            If cblPublicationStatus.SelectedItem IsNot Nothing Then
                Select Case cblPublicationStatus.SelectedItem.Value
                    Case "NONE"
                        finder.IsPublished = False
                    Case "PUBLICATED"
                        finder.IsPublished = True
                    Case "RETIRED"
                        finder.IsRetired = True
                End Select
            End If

            If rdpRegistrationFrom.SelectedDate.HasValue Then
                finder.RegistrationDateFrom = rdpRegistrationFrom.SelectedDate.Value
            End If

            If rdpRegistrationTo.SelectedDate.HasValue Then
                finder.RegistrationDateTo = rdpRegistrationTo.SelectedDate.Value
            End If

            If txtPublishingDateFrom.SelectedDate.HasValue Then
                finder.PublishingDateFrom = txtPublishingDateFrom.SelectedDate.Value
            End If

            If txtPublishingDateTo.SelectedDate.HasValue Then
                finder.PublishingDateTo = txtPublishingDateTo.SelectedDate.Value
            End If

            If txtRetireDateFrom.SelectedDate.HasValue Then
                finder.RetireDateFrom = txtRetireDateFrom.SelectedDate.Value
            End If
            If txtRetireDateTo.SelectedDate.HasValue Then
                finder.RetireDateTo = txtRetireDateTo.SelectedDate.Value
            End If

            If Not String.IsNullOrEmpty(txtSubject.Text) Then
                If chkSubjectContains.Checked Then
                    finder.SubjectContains = txtSubject.Text
                Else
                    finder.SubjectStartsWith = txtSubject.Text
                End If
            End If

            If uscRoleOwner.RoleListAdded IsNot Nothing AndAlso uscRoleOwner.RoleListAdded.Count > 0 Then
                For Each i As Integer In uscRoleOwner.RoleListAdded
                    finder.IdOwnerRoleIn.Add(i)
                Next
            End If

            If uscCategory.HasSelectedCategories Then
                finder.CategoryPath = uscCategory.SelectedCategories.First().FullIncrementalPath
            End If

            Return finder
        End Function

        ''' <summary> Clear dei controlli base di ASP e di Telerik </summary>
        ''' <param name="control">pagina in cui si vogliono sbiancare i controlli base</param>
        Private Sub ClearControl(control As Control)

            Select Case control.GetType()
                Case GetType(TextBox)
                    If CType(control, TextBox) IsNot Nothing Then
                        CType(control, TextBox).Text = String.Empty
                    End If
                Case GetType(RadNumericTextBox)
                    If CType(control, RadNumericTextBox) IsNot Nothing Then
                        CType(control, RadNumericTextBox).Text = String.Empty
                    End If
                Case GetType(RadTextBox)
                    If CType(control, RadTextBox) IsNot Nothing Then
                        CType(control, RadTextBox).Text = String.Empty
                    End If
                Case GetType(RadDatePicker)
                    If CType(control, RadDatePicker) IsNot Nothing Then
                        CType(control, RadDatePicker).SelectedDate = Nothing
                    End If
                Case GetType(DropDownList)
                    If CType(control, DropDownList) IsNot Nothing Then
                        CType(control, DropDownList).SelectedIndex = -1
                    End If
                Case GetType(CheckBox)
                    If CType(control, CheckBox) IsNot Nothing Then
                        CType(control, CheckBox).Checked = False
                    End If
                Case GetType(CheckBoxList)
                    If CType(control, CheckBoxList) IsNot Nothing Then
                        CType(control, CheckBoxList).ClearSelection()
                    End If
            End Select

            For Each childControl As Control In control.Controls
                ClearControl(childControl)
            Next

        End Sub

#End Region

    End Class
End Namespace