Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json
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

    Public ReadOnly Property CurrentController As String
        Get
            Dim webApiPath As Uri = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Where(Function(x) x.AddressName.Eq(UDS_ADDRESS_NAME)).Select(Function(s) s.Address).FirstOrDefault()
            Dim path As String = String.Empty
            If CurrentFinder.IdRepository.HasValue Then
                Dim controllerName As String = Utils.GetWebAPIControllerName(CurrentRepository.Name)
                If Char.IsDigit(controllerName.ElementAt(0)) Then
                    controllerName = $"_{controllerName}"
                End If

                path = New Uri(webApiPath, controllerName).AbsoluteUri
            Else
                path = New Uri(webApiPath, "BaseController").AbsoluteUri 'prendere da configurazione
            End If
            Return path
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

    Public ReadOnly Property dgvUDSItems As String
        Get
            Return Request.Form("dgvUDSItems")
        End Get
    End Property
    Public ReadOnly Property dgvColumns As String
        Get
            Return Request.Form("dgvColumns")
        End Get
    End Property
    Public ReadOnly Property dgvUDSAllItems As String
        Get
            Return Request.Form("dgvUDSAllItems")
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            btnEsportaPagina.Icon.SecondaryIconUrl = ImagePath.SmallExcel
            btnEsportaTutto.Icon.SecondaryIconUrl = ImagePath.SmallExcel
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
        If Me.CurrentFinder Is Nothing Then
            Throw New DocSuiteException("Errore ricerca UDS", "Impossibile inizializzare dalla pagina di provenienza.")
        End If

        CreateDynamicColumn()
        InitializeFilterMenu()
        Title = String.Concat(PAGE_RESULTS_TITLE, ": ", CurrentRepository.Name)
        btnDocuments.Visible = ProtocolEnv.EnableMultiChainView

    End Sub

    Private Sub InitializeFilterMenu()
        Dim menu As GridFilterMenu = dgvUDS.FilterMenu
        For Each item As RadMenuItem In menu.Items
            item.Text = RadGridFilterHelper.GetLocalizeFilterName(item.Text)
        Next
    End Sub

    'Qui creo le nuove colonne 
    Private Sub CreateDynamicColumn()
        Dim UDSModel As UDSModel = UDSModel.LoadXml(CurrentRepository.ModuleXML)
        Dim jsModel As JsModel = CurrentConverter.ConvertToJs(UDSModel)
        Dim newColumn As GridTemplateColumn

        CommonColumnsVisibility(jsModel.elements.Where(Function(x) x.ctrlType = ctlTitle).First(), dgvUDS.MasterTableView.Columns)

        For Each element As Element In jsModel.elements.Where(Function(x) x.resultVisibility).OrderBy(Function(t) t.resultPosition)
            If Not element.ctrlType = ctlHeader AndAlso Not element.ctrlType = ctlTitle Then
                CreateElementColumn(element, newColumn, dgvUDS.MasterTableView.Columns)
            End If
        Next
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
    End Sub
    Private Sub CreateColumns(columns As GridColumnCollection, dataField As List(Of String), headerText As List(Of String))
        For index As Integer = 0 To dataField.Count - 1
            Dim column As GridBoundColumn = New GridBoundColumn()
            column.DataField = dataField(index)
            column.HeaderText = headerText(index)
            columns.Add(column)
        Next
    End Sub
    Private Function getDataSourceFromJson(dataSource As List(Of Dictionary(Of String, String))) As DataTable
        Dim dataTable As DataTable = New DataTable()
        For Each columnName As String In dataSource.First().Keys
            dataTable.Columns.Add(New DataColumn(columnName))
        Next
        For Each row As Dictionary(Of String, String) In dataSource
            dataTable.Rows.Add(row.Values.ToArray())
        Next
        Return dataTable
    End Function
    Public Sub ExportGrid(ignorePagining As Boolean)
            Dim dataSource As List(Of Dictionary(Of String, String))

            If ignorePagining = False Then
                dataSource = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(dgvUDSItems)
            Else
                dataSource = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(dgvUDSAllItems)
            End If

            If dataSource.Count <> 0 Then
                Dim headerTextList As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(dgvColumns)
                Dim dataFieldsList As List(Of String) = dataSource.First().Keys.ToList()
                CreateColumns(intermediateGrid.MasterTableView.Columns, dataFieldsList, headerTextList)

                intermediateGrid.Visible = True
                intermediateGrid.DataSource = getDataSourceFromJson(dataSource)
                intermediateGrid.DataBind()
                intermediateGrid.ExportSettings.IgnorePaging = ignorePagining
                intermediateGrid.ExportSettings.ExportOnlyData = True
                intermediateGrid.ExportSettings.OpenInNewWindow = True
                intermediateGrid.ExportSettings.FileName = "registro"
                intermediateGrid.MasterTableView.ExportToExcel()
            End If
        End Sub
#End Region

End Class