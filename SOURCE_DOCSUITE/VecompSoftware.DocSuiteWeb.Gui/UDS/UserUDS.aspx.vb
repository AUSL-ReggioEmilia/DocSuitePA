Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.UDSDesigner
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UserUDS
    Inherits CommonBasePage

#Region "Fields"
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

#Region " Properties "
    Public ReadOnly Property TenantCompanyName As String
        Get
            Dim companyname As String = String.Empty
            If CurrentTenant IsNot Nothing Then
                companyname = CurrentTenant.TenantName
            End If
            Return companyname
        End Get
    End Property
    Protected ReadOnly Property CurrentDisplayName As String
        Get
            Return String.Format("{0}\\{1}", DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName)
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

    Private ReadOnly Property CurrentConverter As UDSConverter
        Get
            If _currentConverter Is Nothing Then
                _currentConverter = New UDSConverter()
            End If
            Return _currentConverter
        End Get
    End Property

    Public ReadOnly Property IsFromUDSLink As Boolean?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Boolean?)("isFromUDSLink", Nothing)
        End Get
    End Property

    Public ReadOnly Property CopyToPEC As Boolean?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Boolean?)("CopyToPEC", Nothing)
        End Get
    End Property

    Public ReadOnly Property MultiTenantEnabled As Boolean
        Get
            Return ProtocolEnv.MultiTenantEnabled
        End Get
    End Property

    Public ReadOnly Property UDSRoleUrl As String
        Get
            If _UDSRoleUrl Is Nothing Then
                Dim webApiController As String = String.Empty
                Dim webApiPath As String = DocSuiteContext.Current.CurrentTenant.ODATAUrl
                If Not webApiPath.EndsWith("/") Then webApiPath = String.Concat(webApiPath, "/")
                If DocSuiteContext.Current.CurrentTenant.Entities.ContainsKey(UDSROLEKEY) Then
                    webApiController = DocSuiteContext.Current.CurrentTenant.Entities.Item(UDSROLEKEY).ODATAControllerName.ToString()
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
                    webApiController = DocSuiteContext.Current.CurrentTenant.Entities.Item(UDSCONTACTKEY).ODATAControllerName.ToString()
                End If
                _UDSContactUrl = Path.Combine(webApiPath, webApiController)
            End If
            Return _UDSContactUrl
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            HideDefaultGridColumns()
        End If
    End Sub

    Protected Sub rcbRepositoryName_OnSelectedIndexChanged(ByVal sender As Object, ByVal e As RadComboBoxSelectedIndexChangedEventArgs) Handles rcbRepositoryName.SelectedIndexChanged
        ShowDefaultGridColumns()
        CurrentFinder = GetFinderModel()
        CreateDynamicColumn()
        udsGrid.Rebind()
    End Sub

    Protected Sub udsGrid_OnNeedDataSource(ByVal sender As Object, ByVal e As GridNeedDataSourceEventArgs) Handles udsGrid.NeedDataSource
        udsGrid.DataSource = New List(Of Object)()
    End Sub

#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbRepositoryName, RadScriptBlock1)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbRepositoryName, udsGrid)
    End Sub

    Private Sub HideDefaultGridColumns()
        For Each gridColumn As GridTemplateColumn In udsGrid.MasterTableView.Columns
            gridColumn.Visible = False
        Next
    End Sub

    Private Sub ShowDefaultGridColumns()
        For Each gridColumn As GridTemplateColumn In udsGrid.MasterTableView.Columns
            gridColumn.Visible = True
        Next
    End Sub

    Private Sub CreateDynamicColumn()
        Dim UDSModel As UDSModel = UDSModel.LoadXml(CurrentRepository.ModuleXML)
        Dim jsModel As JsModel = CurrentConverter.ConvertToJs(UDSModel)
        Dim newColumn As GridTemplateColumn

        RemoveGeneratedColumns()

        For Each element As Element In jsModel.elements.Where(Function(x) x.resultVisibility).OrderBy(Function(t) t.resultPosition)
            If Not element.ctrlType = ctlHeader AndAlso Not element.ctrlType = ctlTitle Then
                CreateElementColumn(element, newColumn, udsGrid.MasterTableView.Columns)
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

    Public Function GetFinderModel() As UDSFinderDto
        Dim finderModel As UDSFinderDto = New UDSFinderDto()

        finderModel.IdRepository = Guid.Parse(rcbRepositoryName.SelectedValue)

        Return finderModel
    End Function

    Private Sub RemoveGeneratedColumns()
        Dim columnLength As Integer = udsGrid.MasterTableView.Columns.Count
        For index As Integer = columnLength - 1 To 5 Step -1
            udsGrid.MasterTableView.Columns.RemoveAt(index)
        Next
    End Sub

#End Region
End Class