
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS

Partial Public Class uscODATAProtGrid
    Inherits GridControl

#Region " Fields "

    Public Const COLUMN_VIEW_DOCUMENTS As String = "colViewDocuments"
    Public Const COLUMN_UD_TITLE As String = "colUDTitle"
    Public Const COLUMN_REGISTRATION_DATE As String = "RegistrationDate"
    Public Const COLUMN_CONTAINER_NAME As String = "Entity.Container.Name"
    Public Const COLUMN_CATEGORY_NAME As String = "Entity.Category.Name"
    Public Const COLUMN_PROTOCOL_SUBJECT As String = "Entity.Subject"
    Public Const COLUMN_TENANT As String = "TenantModel.TenantName"
    Public Const COLUMN_DOCUMENT_UNIT_NAME As String = "Entity.DocumentUnitName"

    Private _gridDataSource As IList(Of WebAPIDto(Of ProtocolModel))

#End Region

#Region " Properties "

    Private ReadOnly Property GridDataSource As IList(Of WebAPIDto(Of ProtocolModel))
        Get
            If _gridDataSource Is Nothing Then
                _gridDataSource = DirectCast(grdProtocols.DataSource, IList(Of WebAPIDto(Of ProtocolModel)))
            End If
            Return _gridDataSource
        End Get
    End Property

    Public ReadOnly Property Grid As BindGrid
        Get
            Return grdProtocols
        End Get
    End Property

    Public Property ColumnViewDocumentsVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_DOCUMENTS).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_VIEW_DOCUMENTS).Visible = value
        End Set
    End Property

    Public Property ColumnUDTitleVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_UD_TITLE).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_UD_TITLE).Visible = value
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
    Public Property ColumnUDSubjectVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_SUBJECT).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_PROTOCOL_SUBJECT).Visible = value
        End Set
    End Property

    Public Property ColumnTenantNameVisible() As Boolean
        Get
            Return grdProtocols.Columns.FindByUniqueName(COLUMN_TENANT).Visible
        End Get
        Set(ByVal value As Boolean)
            grdProtocols.Columns.FindByUniqueName(COLUMN_TENANT).Visible = value
        End Set
    End Property

    Public Property EnableGridScrolling As Boolean = True

#End Region

#Region " Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        InitializeColumns()
    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' DO Nothing
        AjaxManager.AjaxSettings.AddAjaxSetting(grdProtocols, grdProtocols)
    End Sub

    Protected Sub MaskText_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim maskTextBox As RadMaskedTextBox = DirectCast(sender, RadMaskedTextBox)
        Dim filter As String = maskTextBox.Text
        If String.IsNullOrEmpty(filter) Then
            Exit Sub
        End If

        ' Filtro per colonne di stringhe
        Dim year As Short = Convert.ToInt16(maskTextBox.TextWithLiterals.Split("/"c)(0))
        Dim number As String = maskTextBox.TextWithLiterals.Split("/"c)(1)

        Dim filterItem As GridFilteringItem = DirectCast(maskTextBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        filters.Add(New Data.FilterExpression("Year", GetType(Integer), year, Data.FilterExpression.FilterType.EqualTo))
        filters.Add(New Data.FilterExpression("Number", GetType(String), number, Data.FilterExpression.FilterType.EqualTo))

        filterItem.FireCommandEvent("CustomFilter", filters)
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

    Protected Sub grdProtocols_ItemCreated(sender As Object, e As GridItemEventArgs) Handles grdProtocols.ItemCreated
        If Not (TypeOf e.Item Is GridFilteringItem) OrElse (grdProtocols.Finder Is Nothing) Then
            Exit Sub
        End If

        If grdProtocols.Finder.FilterExpressions Is Nothing Then
            Exit Sub
        End If

        Dim filteringItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
        If grdProtocols.Finder.FilterExpressions.Any(Function(x) x.Key.Eq("Year")) Then
            Dim control As Control = filteringItem.FindControl("UDTitleFilter")
            Dim value As String = String.Format("{0:0000}/{1:0000000}",
                                                grdProtocols.Finder.FilterExpressions("Year").FilterValue,
                                                grdProtocols.Finder.FilterExpressions("Number").FilterValue)
            FilterHelper.SetFilterValue(control, value)
        End If
    End Sub

    Private Sub grdProtocols_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdProtocols.ItemDataBound

        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
            Dim combo As RadComboBox = DirectCast(filterItem.FindControl("cmbDocumentUnitName"), RadComboBox)

            combo.Items.AddRange(BasePage.FillComboBoxDocumentUnitNames())
            For Each item As UDSRepository In CurrentUDSRepositoryFacade.GetActiveRepositories()
                combo.Items.Add(New RadComboBoxItem(item.Name, item.Name))
            Next

            If grdProtocols.Finder.FilterExpressions.Any(Function(x) x.Key.Eq(COLUMN_DOCUMENT_UNIT_NAME)) Then
                Dim control As Control = filterItem.FindControl("cmbDocumentUnitName")
                Dim value As String = grdProtocols.Finder.FilterExpressions(COLUMN_DOCUMENT_UNIT_NAME).FilterValue.ToString()
                FilterHelper.SetFilterValue(control, value)
            End If
        End If

        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
            Dim combo As RadComboBox = DirectCast(filterItem.FindControl("cmbTenantName"), RadComboBox)
            combo.Items.Insert(0, New RadComboBoxItem("", ""))
            For Each tenant As TenantModel In DocSuiteContext.Current.Tenants
                combo.Items.Add(New RadComboBoxItem(tenant.TenantName, tenant.TenantName))
            Next

            If grdProtocols.Finder.FilterExpressions.Any(Function(x) x.Key.Eq(COLUMN_TENANT)) Then
                Dim control As Control = filterItem.FindControl("cmbTenantName")
                Dim value As String = grdProtocols.Finder.FilterExpressions(COLUMN_TENANT).FilterValue.ToString()
                FilterHelper.SetFilterValue(control, value)
            End If
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As WebAPIDto(Of DocumentUnitModel) = DirectCast(e.Item.DataItem, WebAPIDto(Of DocumentUnitModel))
        Dim hiddenId As String = String.Format("{0}|{1}|{2}|{3}", boundHeader.Entity.UniqueId, boundHeader.Entity.Year, boundHeader.Entity.Number, boundHeader.TenantModel.TenantName)

        DirectCast(e.Item.FindControl("lblUDName"), Label).Text = boundHeader.Entity.DocumentUnitName

        If ColumnViewDocumentsVisible Then
            With DirectCast(e.Item.FindControl("ibtViewDocuments"), ImageButton)
                .ImageUrl = getHeaderImageUrl(boundHeader.Entity, .CommandName)
                .OnClientClick = String.Empty
                .PostBackUrl = getHeaderPostBackUrl(boundHeader, .CommandName)

                .Attributes.Add("onmouseover", "this.style.cursor='hand';")
                .Attributes.Add("onmouseout", "this.style.cursor='default';")
                .CommandArgument = hiddenId
            End With
        End If
        If ColumnUDTitleVisible Then
            With DirectCast(e.Item.FindControl("lbtViewUD"), LinkButton)
                .Text = boundHeader.Entity.Title
                .CommandName = GetCommandName(boundHeader.Entity)
                .CommandArgument = GetCommandArgument(hiddenId, boundHeader)
            End With
        End If
        If ColumnUDSubjectVisible Then
            DirectCast(e.Item.FindControl("lblUDSubject"), Label).Text = boundHeader.Entity.Subject
        End If
        If ColumnCategoryNameVisible Then
            DirectCast(e.Item.FindControl("lblCategoryProjection"), Label).Text = getTextByHeader(boundHeader.Entity, COLUMN_CATEGORY_NAME)
        End If

        If ColumnTenantNameVisible Then
            DirectCast(e.Item.FindControl("lblTenantName"), Label).Text = boundHeader.TenantModel.TenantName
        End If
    End Sub

    Private Sub grdProtocols_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles grdProtocols.ItemCommand

        Select Case e.CommandName
            Case "ViewProtocol"
                Dim split As String() = e.CommandArgument.ToString().Split("|"c)
                Dim tenant As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.TenantName.Eq(split(3)))
                If tenant.CurrentTenant Then
                    Dim prot As Protocol = Facade.ProtocolFacade.GetById(Guid.Parse(split(0)), False)
                    If prot IsNot Nothing Then
                        RedirectOnPage($"../Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={prot.Id}&Type=Prot")}")
                    Else
                        AjaxManager.Alert(String.Format("Il Protocollo {0}/{1} non è stato trovato", Short.Parse(split(1)), Integer.Parse(split(2))))
                    End If
                Else
                    Dim url As String = String.Format("{0}/?Tipo=Prot&Azione=Apri&Anno={1}&Numero={2}", tenant.DSWUrl, Short.Parse(split(1)), Integer.Parse(split(2)))
                    Response.RedirectToNewWindow(url)
                End If
            Case "ViewResolution"
                RedirectOnPage(String.Concat("../Resl/ReslVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", e.CommandArgument))))
            Case "ViewSeries"
                RedirectOnPage(String.Concat("../Series/Item.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdDocumentSeriesItem={0}&Type=Series&Action=View", e.CommandArgument))))
            Case "ViewUDS"
                Dim split As String() = e.CommandArgument.ToString().Split("|"c)
                RedirectOnPage(String.Concat("../UDS/UDSView.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdUDS={0}&IdUDSRepository={1}&Type=UDS", split(0), split(1)))))
            Case "ViewFascicles"
                Dim split As String() = e.CommandArgument.ToString().Split("|"c)
                Dim tenant As TenantModel = DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) x.TenantName.Eq(split(2)))
                If tenant.CurrentTenant Then
                    RedirectOnPage(String.Concat("~/Fasc/FascUDManager.aspx?", CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Fasc", Short.Parse(split(0)), Integer.Parse(split(1))))))
                Else
                    AjaxManager.Alert("Il protocollo selezionato non è del dominio corrente.")
                End If
        End Select

    End Sub

    Protected Sub cmbTenantName_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim comboBox As RadComboBox = DirectCast(sender, RadComboBox)

        Dim filterItem As GridFilteringItem = DirectCast(comboBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        Dim filterType As Data.FilterExpression.FilterType = Data.FilterExpression.FilterType.NoFilter
        If (Not String.IsNullOrEmpty(comboBox.SelectedValue)) Then
            filterType = Data.FilterExpression.FilterType.EqualTo
        End If
        filters.Add(New Data.FilterExpression(COLUMN_TENANT, GetType(Short), comboBox.SelectedValue, filterType))
        filterItem.FireCommandEvent("CustomFilter", filters)
    End Sub

    Protected Sub cmbDocumentUnitName_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim comboBox As RadComboBox = DirectCast(sender, RadComboBox)

        Dim filterItem As GridFilteringItem = DirectCast(comboBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        Dim filterType As Data.FilterExpression.FilterType = Data.FilterExpression.FilterType.NoFilter
        If (Not String.IsNullOrEmpty(comboBox.SelectedValue)) Then
            filterType = Data.FilterExpression.FilterType.EqualTo
        End If
        filters.Add(New Data.FilterExpression(COLUMN_DOCUMENT_UNIT_NAME, GetType(Short), comboBox.SelectedValue, filterType))
        filterItem.FireCommandEvent("CustomFilter", filters)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeColumns()
        If Not DocSuiteContext.Current.IsProtocolEnabled Then
            Exit Sub
        End If

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            ColumnTenantNameVisible = True
        End If

    End Sub

    Private Sub InitializeFilterColumns()

        Dim columnCategory As CompositeTemplateColumnSqlExpression = TryCast(grdProtocols.Columns.FindByUniqueNameSafe(COLUMN_CATEGORY_NAME), CompositeTemplateColumnSqlExpression)
        If columnCategory IsNot Nothing Then
            columnCategory.BindingType = CompositeTemplateColumnSqlExpression.ColumnBinding.CustomBinding
            columnCategory.CustomExpressionDelegate = New CompositeTemplateColumnSqlExpression.SqlExpressionDelegate(AddressOf Facade.CategoryFacade.CategoryNameFullCodeFilter)
        End If


    End Sub

    Private Function getHeaderImageUrl(header As DocumentUnitModel, discriminator As String) As String
        Select Case discriminator
            Case "ViewDocuments"
                Dim icon As String

                If String.IsNullOrWhiteSpace(header.MainDocumentName) Then
                    icon = ImagePath.SmallDocumentSeries
                ElseIf ProtocolEnv.SignedIconRenderingModality.ProtocolIconModality = IconModality.OriginalIcon Then
                    icon = ImagePath.FromFileNoSignature(header.MainDocumentName, True)
                Else
                    icon = ImagePath.FromFile(header.MainDocumentName, True)
                End If
                Return icon
            Case Else
                Return String.Empty
        End Select
        Return String.Empty
    End Function

    Private Function getHeaderPostBackUrl(header As WebAPIDto(Of DocumentUnitModel), discriminator As String) As String
        Select Case discriminator
            Case "ViewDocuments"
                Dim parameters As String
                Select Case header.Entity.Environment
                    Case DSWEnvironment.Protocol
                        parameters = String.Format("UniqueId={0}&Type=Prot", header.Entity.UniqueId)
                        If (header.TenantModel.CurrentTenant) Then
                            Return String.Concat("~/Viewers/ProtocolViewer.aspx?", CommonShared.AppendSecurityCheck(parameters))
                        End If
                    Case DSWEnvironment.DocumentSeries
                        parameters = String.Format("id={0}", header.Entity.EntityId)
                        Return String.Concat("~/Viewers/DocumentSeriesItemViewer.aspx?", CommonShared.AppendSecurityCheck(parameters))
                    Case DSWEnvironment.Resolution
                        parameters = String.Format("IdResolution={0}&documents=true&attachments=true&annexes=true&documentsomissis=true&attachmentsomissis=true&previous=conditional", header.Entity.EntityId)
                        Return String.Concat("~/Viewers/ResolutionViewer.aspx?", CommonShared.AppendSecurityCheck(parameters))
                    Case > 100
                        parameters = String.Format("IdUDS={0}&IdUDSRepository={1}", header.Entity.UniqueId, header.Entity.IdUDSRepository)
                        Return String.Concat("~/Viewers/UDSViewer.aspx?", CommonShared.AppendSecurityCheck(parameters))
                End Select

            Case Else
                Exit Select
        End Select
        Return String.Empty
    End Function

    Private Function getTextByHeader(header As DocumentUnitModel, discriminator As String) As String
        Select Case discriminator
            Case COLUMN_CATEGORY_NAME
                If header.Category IsNot Nothing Then
                    Return header.Category.Name
                End If

            Case Else
                Exit Select
        End Select
        Return String.Empty
    End Function

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

    Protected Function GetCommandName(documentUnit As DocumentUnitModel) As String
        Dim commandName As String = String.Empty
        Select Case documentUnit.Environment
            Case DSWEnvironment.Protocol
                commandName = "ViewProtocol"
            Case DSWEnvironment.Resolution
                commandName = "ViewResolution"
            Case DSWEnvironment.DocumentSeries
                commandName = "ViewSeries"
            Case > 100
                commandName = "ViewUDS"
        End Select
        Return commandName
    End Function

    Protected Function GetCommandArgument(hiddenId As String, header As WebAPIDto(Of DocumentUnitModel)) As String
        Dim commandArgument As String = String.Empty
        Select Case header.Entity.Environment
            Case DSWEnvironment.Protocol
                commandArgument = hiddenId
            Case DSWEnvironment.Resolution
                commandArgument = header.Entity.EntityId.ToString()
            Case DSWEnvironment.DocumentSeries
                commandArgument = header.Entity.EntityId.ToString()
            Case > 100
                commandArgument = String.Format("{0}|{1}", header.Entity.UniqueId, header.Entity.IdUDSRepository)
        End Select
        Return commandArgument
    End Function

#End Region

End Class