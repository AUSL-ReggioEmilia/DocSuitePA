Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports OfficeOpenXml
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.UDSDesigner
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.Web.RadGrid

Public Class UDSResults
    Inherits UDSBasePage

#Region "Fields"
    Private _udsRepositoryFacade As UDSRepositoryFacade
    Private _radGridFilterHelper As RadGridFilterHelper
    Private Const PAGE_RESULTS_TITLE As String = "Risultati archivio"
    Private _currentRepository As UDSRepository
    Private _currentConverter As UDSConverter
    Private _UDSRoleUrl As String
    Private Const UDSROLEKEY As String = "UDSRole"
    Private _UDSContactUrl As String
    Private Const UDSCONTACTKEY As String = "UDSContact"


    Private ctlTitle As String = "Title"
    Private ctlHeader As String = "Header"
    Private ctlStatus As String = "Status"
    Private ctlEnum As String = "Enum"
    Private ctlText As String = "Text"
    Private ctlNumber As String = "Number"
    Private ctlDate As String = "Date"
    Private ctlCheckbox As String = "Checkbox"
    Private ctlLookup As String = "Lookup"
    Private ctlContact As String = "Contact"
    Private ctlAuthorization As String = "Authorization"

#End Region

#Region "Properties"

    Public ReadOnly Property RadGridFilterHelper As RadGridFilterHelper
        Get
            If _radGridFilterHelper Is Nothing Then
                _radGridFilterHelper = New RadGridFilterHelper(DocSuiteContext.RadGridLocalizeConfiguration)
            End If
            Return _radGridFilterHelper
        End Get
    End Property

    Private Property CurrentFinder As UDSFinderDto
        Get
            If Session("CurrentFinder") IsNot Nothing Then
                Return DirectCast(Session("CurrentFinder"), UDSFinderDto)
            End If
            Return Nothing
        End Get
        Set(value As UDSFinderDto)
            If value Is Nothing Then
                Session.Remove("CurrentFinder")
            Else
                Session("CurrentFinder") = value
            End If
        End Set
    End Property

    Public ReadOnly Property SerializedDefaultFinder As String
        Get
            Dim defaultFinder As String = JsonConvert.SerializeObject(Me.CurrentFinder.Filters)
            Return defaultFinder
        End Get
    End Property

    Private ReadOnly Property CurrentRepository As UDSRepository
        Get
            If _currentRepository Is Nothing AndAlso Not CurrentFinder.IdRepository Is Nothing Then
                _currentRepository = CurrentUDSRepositoryFacade.GetById(CurrentFinder.IdRepository.Value)
            End If
            Return _currentRepository
        End Get
    End Property

    Private ReadOnly Property CurrentConverter As UDSConverter
        Get
            If _currentConverter Is Nothing Then
                _currentConverter = New UDSConverter()
            End If
            Return _currentConverter
        End Get
    End Property

    Public ReadOnly Property CurrentRepositoryName As String
        Get
            If CurrentFinder.IdRepository.HasValue Then
                Return CurrentRepository.Name
            Else
                Return "BaseController"
            End If
        End Get
    End Property
    Public ReadOnly Property UDSRoleUrl As String
        Get
            If _UDSRoleUrl Is Nothing Then
                Dim webApiController As String = String.Empty
                Dim webApiPath As String = DocSuiteContext.Current.CurrentTenant.ODATAUrl
                If Not webApiPath.EndsWith("/") Then webApiPath = String.Concat(webApiPath, "/")
                If DocSuiteContext.Current.CurrentTenant.Entities.ContainsKey(UDSROLEKEY) Then
                    webApiController = DocSuiteContext.Current.CurrentTenant.Entities.Item(UDSROLEKEY).ODATAControllerName.ToString
                End If
                _UDSRoleUrl = Path.Combine(webApiPath, webApiController)
            End If
            Return _UDSRoleUrl
        End Get
    End Property
    Public ReadOnly Property UDSContactUrl As String
        Get
            If _UDSContactUrl Is Nothing Then
                Dim webApiController As String = String.Empty
                Dim webApiPath As String = DocSuiteContext.Current.CurrentTenant.ODATAUrl
                If Not webApiPath.EndsWith("/") Then webApiPath = String.Concat(webApiPath, "/")
                If DocSuiteContext.Current.CurrentTenant.Entities.ContainsKey(UDSCONTACTKEY) Then
                    webApiController = DocSuiteContext.Current.CurrentTenant.Entities.Item(UDSCONTACTKEY).ODATAControllerName.ToString
                End If
                _UDSContactUrl = Path.Combine(webApiPath, webApiController)
            End If
            Return _UDSContactUrl
        End Get
    End Property
    Public ReadOnly Property CopyToPEC As Boolean?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Boolean?)("CopyToPEC", Nothing)
        End Get
    End Property
    Public ReadOnly Property IsFromUDSLink As Boolean?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Boolean?)("isFromUDSLink", Nothing)
        End Get
    End Property
    Public ReadOnly Property CurrentAction As String
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of String)("Action", Nothing)
        End Get
    End Property
    Public ReadOnly Property dgvUDSItems As String
        Get
            Return Request.Form("dgvUDSItems")
        End Get
    End Property
    Public ReadOnly Property dgvUDSAllItems As String
        Get
            Return Request.Form("dgvUDSAllItems")
        End Get
    End Property

    Private ReadOnly Property IsActionCopyDocument As Boolean
        Get
            Return CurrentAction.Eq("CopyDocuments")
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
    Protected Sub hiddenButton_Click(sender As Object, e As EventArgs) Handles hiddenButton.Click
        ExportGrid(True)
    End Sub
    Protected Sub btnEsportaPagina_Click(sender As Object, e As EventArgs) Handles btnEsportaPagina.Click
        ExportGrid(False)
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()

    End Sub

    Private Sub Initialize()
        Title = $"{PAGE_RESULTS_TITLE}: {CurrentRepository.Name}"
        If CurrentFinder Is Nothing Then
            Throw New DocSuiteException("Errore ricerca UDS", "Impossibile inizializzare dalla pagina di provenienza.")
        End If
        btnEsportaPagina.Icon.SecondaryIconUrl = ImagePath.SmallExcel
        btnEsportaTutto.Icon.SecondaryIconUrl = ImagePath.SmallExcel
        btnDocuments.Visible = ProtocolEnv.EnableMultiChainView

        CreateDynamicColumn()
        InitializeFilterMenu()

        If IsActionCopyDocument Then
            btnEsportaPagina.Visible = False
            btnEsportaTutto.Visible = False
            btnDocuments.Visible = False
            btnSelectAll.Visible = False
            btnDeselectAll.Visible = False
        End If
    End Sub

    Private Sub InitializeFilterMenu()
        Dim menu As GridFilterMenu = dgvUDS.FilterMenu
        For Each item As RadMenuItem In menu.Items
            item.Text = RadGridFilterHelper.GetLocalizeFilterName(item.Text)
        Next
    End Sub

    'Qui creo le nuove colonne 
    Private Sub CreateDynamicColumn()
        Dim udsModel As UDSModel = udsModel.LoadXml(CurrentRepository.ModuleXML)
        Dim jsModel As JsModel = CurrentConverter.ConvertToJs(udsModel)
        Dim newColumn As GridTemplateColumn = Nothing

        CommonColumnsVisibility(jsModel.elements.Where(Function(x) x.ctrlType = ctlTitle).First(), dgvUDS.MasterTableView.Columns)

        For Each element As Element In jsModel.elements.Where(Function(x) x.resultVisibility).OrderBy(Function(t) t.resultPosition)
            If Not element.ctrlType = ctlHeader AndAlso Not element.ctrlType = ctlTitle Then
                CreateElementColumn(element, newColumn, dgvUDS.MasterTableView.Columns)
            End If
        Next
        If Not udsModel.Model.StampaConformeEnabled Then
            btnDocuments.Enabled = False
            btnDocuments.ToolTip = "Funzionalità disattivata come da configurazione prevista per l'archivio selezionato"
        End If
    End Sub

    Private Sub CreateElementColumn(element As Element, column As GridTemplateColumn, gridColumns As GridColumnCollection)
        column = New GridTemplateColumn()
        Select Case element.ctrlType
            Case ctlText
                column.CurrentFilterFunction = GridKnownFunction.Contains
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 25)
                column.AllowFiltering = True
            Case ctlNumber
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 15, String.Concat("getNumber(", element.columnName, ")"))
            Case ctlDate
                column.DataType = GetType(DateTime)
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 15, String.Concat("getDate(", element.columnName, ")"))
            Case ctlCheckbox
                column.DataType = GetType(Boolean)
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 5, String.Concat("getBoolean(", element.columnName, ")"))
            Case ctlEnum
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                Dim itemTemplate As String = element.columnName
                itemTemplate = String.Concat("getEnum(", element.columnName, ")")
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 15, itemTemplate)
            Case ctlStatus
                '<asp:Image ImageUrl = "imageurl" AlternateText="" ToolTip="" runat="server" />
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 15)
            Case ctlLookup
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                Dim itemTemplate As String = element.columnName
                itemTemplate = String.Concat("getLookup(", element.columnName, ")")
                CommonColumnAttributes(column, element.lookupFieldName, element.clientId, String.Concat(element.lookupRepositoryName, ": ", element.lookupFieldName), 20, itemTemplate)
            Case ctlContact
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 15, String.Concat("getContact(UDSId,'", element.label, "')"))
            Case ctlAuthorization
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                CommonColumnAttributes(column, element.columnName, element.clientId, element.label, 15, String.Concat("getAuthorization(UDSId)"))
            Case Else
                Exit Sub
        End Select
        gridColumns.Add(column)
    End Sub

    Private Sub CommonColumnAttributes(column As GridTemplateColumn, columnName As String, uniqueColumnName As String, columnLabel As String, width As Integer)
        CommonColumnAttributes(column, columnName, uniqueColumnName, columnLabel, width, columnName)
    End Sub

    Private Sub CommonColumnAttributes(column As GridTemplateColumn, columnName As String, uniqueColumnName As String, columnLabel As String, width As Integer, clientItemTemplate As String)
        column.DataField = columnName
        column.HeaderText = columnLabel
        column.UniqueName = uniqueColumnName
        column.AllowSorting = True
        column.AllowFiltering = False
        column.SortExpression = columnName
        column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        column.HeaderStyle.Width = Unit.Percentage(width)
        column.ClientItemTemplate = String.Concat("#=", clientItemTemplate, "#")
    End Sub

    Private Sub CommonColumnsVisibility(element As Element, gridColumns As GridColumnCollection)
        If Not element.subjectResultVisibility Then
            Dim subjectColumn As GridTemplateColumn = CType(gridColumns.FindByUniqueName("_subject"), GridTemplateColumn)
            subjectColumn.Visible = False
        End If

        If Not element.categoryResultVisibility Then
            Dim categoryColumn As GridTemplateColumn = CType(gridColumns.FindByUniqueName("Category_FullSearchComputed"), GridTemplateColumn)
            categoryColumn.Visible = False
        End If

        If Not element.showLastChangedDate Then
            Dim lastChangedDateColumn As GridTemplateColumn = CType(gridColumns.FindByUniqueName("LastChangedDate"), GridTemplateColumn)
            lastChangedDateColumn.Visible = False
        End If

        If Not element.showLastChangedUser Then
            Dim lastChangedUserColumn As GridTemplateColumn = CType(gridColumns.FindByUniqueName("LastChangedUser"), GridTemplateColumn)
            lastChangedUserColumn.Visible = False
        End If
    End Sub

    Private Function IsDate(ByRef input As String) As Boolean
        If input Is Nothing Then
            Return False
        End If

        '2019-02-01T11:46:53.0371985+00:00
        If (input.Contains("T"c) Or input.Contains("t"c)) Then
            Dim parts As String() = input.Split("T"c, "t"c)
            Return parts.Any(Function(x) Regex.IsMatch(x, "^\d{4}-(?:0[1-9]|1[0-2])-(?:0[1-9]|[1-2]\d|3[0-1])$"))
        End If
        Return False
    End Function

    Private Function IsJsonArray(ByRef value As String) As Boolean

        If value Is Nothing Then
            Return False
        End If

        Return value.StartsWith("[") AndAlso value.EndsWith("]")
    End Function

    Private Function MaybeParse(ByRef input As KeyValuePair(Of String, String)) As Object
        If IsDate(input.Value) Then
            Dim parsed As DateTimeOffset
            Dim succeded As Boolean = DateTimeOffset.TryParse(input.Value, parsed)

            If (succeded) Then
                'to LocalTime to get the correct offset
                'Date.Parse because EPPlus does not now DateTimeOffset
                Return Date.Parse(parsed.ToLocalTime().ToString())
            End If
        ElseIf IsJsonArray(input.Value) Then
            Try
                Dim billTypes As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(input.Value)
                Return String.Join(",", billTypes)
            Catch ex As Exception
                Return input.Value
            End Try

        End If

        Return input.Value
    End Function

    Private Sub MaybeSetColumnStyle(column As ExcelColumn, colIndex As Integer, value As String)
        'NOTE: formatting is made over the column once, but because the header names may change in prod
        'we have to check cell contents for value format shape. In the colIndices list we keep a track over
        'the already formatted columns to avoid formatting them twice
        Static Dim formattedColIndices As New List(Of Integer)

        If formattedColIndices.Contains(colIndex) Then
            Return
        End If

        If IsDate(value) Then
            column.Style.Numberformat.Format = "yyyy-mm-dd"
            formattedColIndices.Add(colIndex)
        Else
            column.Style.Numberformat.Format = "@"
            formattedColIndices.Add(colIndex)
        End If
    End Sub

    Public Sub ExportGrid(ignorePagining As Boolean)
        Dim dataSource As List(Of Dictionary(Of String, String))

        If ignorePagining = False Then
            dataSource = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(dgvUDSItems)
        Else
            dataSource = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(dgvUDSAllItems)
        End If

        If dataSource.Count = 0 Then
            Return
        End If

        Dim propertyNames As List(Of String) = New List(Of String)()
        Dim headerNames As List(Of String) = New List(Of String)()

        For Each item As String In dataSource(0).Keys.ToList()
            propertyNames.Add(item)

            If item.Eq("_year") Then
                headerNames.Add("Anno")
                Continue For
            End If
            If item.Eq("_number") Then
                headerNames.Add("Numero")
                Continue For
            End If
            If item.Eq("_subject") Then
                headerNames.Add("Oggetto")
                Continue For
            End If
            If item.Eq("RegistrationUser") Then
                headerNames.Add("Creato da ")
                Continue For
            End If
            If item.Eq("RegistrationDate") Then
                headerNames.Add("Data Registrazione")
                Continue For
            End If
            If item.Eq("LastChangedUser") Then
                headerNames.Add("Modificato da")
                Continue For
            End If
            If item.Eq("LastChangedDate") Then
                headerNames.Add("Data Modifica")
                Continue For
            End If
            headerNames.Add(item)
        Next

        Dim value As KeyValuePair(Of String, String)

        Using package As ExcelPackage = New ExcelPackage
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("registro")

            'create headers
            For colIndex As Integer = 0 To headerNames.Count - 1
                worksheet.Cells(1, colIndex + 1).Value = headerNames(colIndex)
                worksheet.Cells(1, colIndex + 1).Style.Font.Bold = True
                Dim propertyKey As String = propertyNames(colIndex)

                For rowIndex As Integer = 0 To dataSource.Count - 1
                    'get value
                    value = dataSource(rowIndex).Where(Function(x) x.Key = propertyKey).FirstOrDefault()

                    'create value
                    If (value.Value IsNot Nothing) Then
                        worksheet.Cells(rowIndex + 2, colIndex + 1).Value = MaybeParse(value)
                        'formatting
                        'https://stackoverflow.com/questions/40209636/epplus-number-format
                        MaybeSetColumnStyle(worksheet.Column(colIndex + 1), colIndex + 1, value.Value)
                    Else
                        worksheet.Cells(rowIndex + 2, colIndex + 1).Value = String.Empty
                    End If

                Next
            Next

            Dim archiveTypeState As Object = Web.HttpContext.Current.Session.Item("Archive.Search.ArchiveType")
            Dim fileName As String = "registro.xlsx"

            If archiveTypeState IsNot Nothing Then
                fileName = $"{CStr(archiveTypeState)}_{fileName}"
            End If


            With System.Web.HttpContext.Current.Response
                .Clear()
                .ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                .AppendHeader("content-disposition", "attachment;  filename=" + fileName)
                .BinaryWrite(package.GetAsByteArray())
                .End() ' Sends all currently buffered output To the client.
            End With

        End Using
    End Sub
#End Region

End Class