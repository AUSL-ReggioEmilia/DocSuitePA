Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.MassimariScarto
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Class TbltClassificatore
    Inherits CommonBasePage

#Region " Fields "
    Private _currentCategorySelected As Integer?
    Private _udsRepositoryFacade As UDSRepositoryFacade
    Private _categoryFascicleFacade As CategoryFascicleFacade
    Private _categoryFascicleRightFacade As CategoryFascicleRightFacade
    Private _currentMassimarioScartoFacade As MassimarioScartoFacade
    Private Const CATEGORY_METADATA_DETAILS_CALLBACK As String = "tbltClassificatore.loadMetadataName('{0}');"
    Private Const SET_VISIBILITY_PANEL As String = "tbltClassificatore.setVisibilityPanel('{0}');"
    Private Const SEND_AJAX_REQUEST As String = "tbltClassificatore.sendAjaxRequest('{0}');"

#End Region

#Region " Properties "
    Public ReadOnly Property IsCategoryManager As Boolean
        Get
            Return CommonShared.HasGroupTblCategoryRight And DocSuiteContext.IsFullApplication
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFascicleFacade As CategoryFascicleFacade
        Get
            If _categoryFascicleFacade Is Nothing Then
                _categoryFascicleFacade = New CategoryFascicleFacade()
            End If
            Return _categoryFascicleFacade
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFascicleRightFacade As CategoryFascicleRightFacade
        Get
            If _categoryFascicleRightFacade Is Nothing Then
                _categoryFascicleRightFacade = New CategoryFascicleRightFacade()
            End If
            Return _categoryFascicleRightFacade
        End Get
    End Property
    Private ReadOnly Property CurrentCategorySelected As Integer?
        Get
            If _currentCategorySelected.HasValue Then
                Return _currentCategorySelected
            End If
            If rtvCategories.SelectedNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(rtvCategories.SelectedNode.Value) Then
                _currentCategorySelected = Integer.Parse(rtvCategories.SelectedNode.Value)
            End If
            Return _currentCategorySelected
        End Get
    End Property

    Public Property CategoriesNotFascicled As IList(Of Integer)
        Get
            If ViewState("CategoriesNotFascicled") Is Nothing Then
                ViewState("CategoriesNotFascicled") = GetCategoriesNotFascicoled()
            End If
            Return DirectCast(ViewState("CategoriesNotFascicled"), IList(Of Integer))
        End Get
        Set(value As IList(Of Integer))
            ViewState("CategoriesNotFascicled") = value
        End Set
    End Property

    Public Property CategoriesProcedureFascicles As IList(Of Integer)
        Get
            If ViewState("CategoriesProcedureFascicles") Is Nothing Then
                ViewState("CategoriesProcedureFascicles") = GetProcedureFascicles()
            End If
            Return DirectCast(ViewState("CategoriesProcedureFascicles"), IList(Of Integer))
        End Get
        Set(value As IList(Of Integer))
            ViewState("CategoriesProcedureFascicles") = value
        End Set
    End Property
    Public Property CategorySubFascicles As IList(Of Integer)
        Get
            If ViewState("CategorySubFascicles") Is Nothing Then
                ViewState("CategorySubFascicles") = GetSubFascicles()
            End If
            Return DirectCast(ViewState("CategorySubFascicles"), IList(Of Integer))
        End Get
        Set(value As IList(Of Integer))
            ViewState("CategorySubFascicles") = value
        End Set
    End Property

    Public ReadOnly Property SearchCategoryTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchDescription")
            Return DirectCast(toolBarItem.FindControl("txtSearchCategory"), RadTextBox)
        End Get
    End Property

    Public ReadOnly Property SearchCategoryCodeTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchCode")
            Return DirectCast(toolBarItem.FindControl("txtSearchCategoryCode"), RadTextBox)
        End Get
    End Property

    Public ReadOnly Property SearchOnlyFascicolable As RadToolBarButton
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchOnlyFascicolable")
            Return DirectCast(toolBarItem, RadToolBarButton)
        End Get
    End Property

    Public ReadOnly Property SearchDisabled As Boolean
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarStatus.FindItemByValue("searchDisabled")
            Return DirectCast(toolBarItem, RadToolBarButton).Checked
        End Get
    End Property

    Public ReadOnly Property SearchActive As Boolean
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarStatus.FindItemByValue("searchActive")
            Return DirectCast(toolBarItem, RadToolBarButton).Checked
        End Get
    End Property

    Public ReadOnly Property SearchSchema As RadComboBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarStatus.FindItemByValue("searchSchema")
            Return DirectCast(toolBarItem.FindControl("rcbSchemas"), RadComboBox)
        End Get
    End Property

    Public ReadOnly Property HasFilters As Boolean
        Get
            Return (Not String.IsNullOrEmpty(SearchCategoryTextBox.Text) OrElse Not String.IsNullOrEmpty(SearchCategoryCodeTextBox.Text) OrElse SearchOnlyFascicolable.Checked)
        End Get
    End Property

    Private ReadOnly Property CurrentMassimarioScartoFacade As MassimarioScartoFacade
        Get
            If _currentMassimarioScartoFacade Is Nothing Then
                _currentMassimarioScartoFacade = New MassimarioScartoFacade(DocSuiteContext.Current.Tenants)
            End If
            Return _currentMassimarioScartoFacade
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFinder As CategoryFinder
        Get
            Dim categoryFinder As CategoryFinder = New CategoryFinder(New MapperCategoryModel(), DocSuiteContext.Current.User.FullUserName)
            categoryFinder.EnablePaging = False
            Return categoryFinder
        End Get
    End Property

    Public Property CategoriesWithChildren As IList(Of Integer)
        Get
            If ViewState("CategoriesWithChildren") Is Nothing Then
                ViewState("CategoriesWithChildren") = GetCategoriesWithChildren()
            End If
            Return DirectCast(ViewState("CategoriesWithChildren"), IList(Of Integer))
        End Get
        Set(value As IList(Of Integer))
            ViewState("CategoriesWithChildren") = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
            InitializeButtons()
            Initialize()
            pnlInfo.Visible = False
            pnlDetails.Visible = False
            FillComboBoxCategorySchemas()
            LoadNodes(rtvCategories.Nodes(0))
        End If
    End Sub

    Protected Sub rtvCategories_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles rtvCategories.NodeClick
        If e.Node.Attributes("NodeType").Eq("Root") Then
            pnlDetails.Visible = False
            pnlInfo.Visible = False
            Exit Sub
        End If

        If e.Node.Nodes.Count = 0 AndAlso Not HasFilters Then
            LoadNodes(e.Node)
        End If
        pnlInfo.Visible = True
        pnlDetails.Visible = True

        Dim selectedCategory As Category = Facade.CategoryFacade.GetById(Integer.Parse(e.Node.Value))
        Dim categories As ICollection(Of CategoryFascicle) = CurrentCategoryFascicleFacade.GetByIdCategory(selectedCategory.Id)
        uscSettori.Initialize(True)
        Dim roles As List(Of Role) = New List(Of Role)
        Dim role As Role = New Role()
        If categories.Any(Function(x) x.FascicleType = FascicleType.Procedure AndAlso x.DSWEnvironment = 0) Then
            Dim categoryFascicleRightId As Guid = categories.Where(Function(x) x.FascicleType = FascicleType.Procedure AndAlso x.DSWEnvironment = 0).First.Id
            Dim categoryRight As ICollection(Of CategoryFascicleRight) = CurrentCategoryFascicleRightFacade.GetByIdCategoryFascicle(categoryFascicleRightId)
            For Each item As CategoryFascicleRight In categoryRight.Where(Function(x) x.Role.Id <> FacadeFactory.Instance.RoleFacade.DefaultAllUserRole.Id)
                roles.Add(item.Role)
            Next
            uscSettori.SourceRoles = roles
            uscSettori.IdCategorySelected = selectedCategory.Id
        End If
        uscSettori.DataBind()
        divProcedureType.Visible = categories.Any(Function(x) x.FascicleType = FascicleType.Procedure)
        pnlSettori.Visible = categories.Count > 0
        divSubFascicleType.Visible = categories.Any(Function(x) x.FascicleType = FascicleType.SubFascicle)
        lblRegistrationDate.Text = If(divProcedureType.Visible, categories.Where(Function(x) x.FascicleType = FascicleType.Procedure OrElse x.FascicleType = FascicleType.SubFascicle).First.RegistrationDate.ToString("dd/MM/yyyy"), "")
        divNoFasciclePlan.Visible = Not (divProcedureType.Visible OrElse divSubFascicleType.Visible)
        btnRunFasciclePlan.Enabled = False
        lblCategoryName.Text = $"{selectedCategory.Name} ({selectedCategory.Id})"
        lblCategoryCode.Text = selectedCategory.FullCodeDotted
        lblStartDate.Text = selectedCategory.StartDate.ToString("dd/MM/yyyy")
        lblEndDate.Text = String.Empty
        If selectedCategory.EndDate.HasValue Then
            lblEndDate.Text = selectedCategory.EndDate.Value.ToString("dd/MM/yyyy")
        End If
        LoadRoleUsers(selectedCategory.Id)

        lblMassimarioName.Text = String.Empty
        Dim massimarioFullName As String = GetMassimarioFullName(Integer.Parse(e.Node.Value))
        If Not String.IsNullOrEmpty(massimarioFullName) Then
            lblMassimarioName.Text = massimarioFullName
        End If

        If ProtocolEnv.MetadataRepositoryEnabled Then
            metadataDetails.Visible = True
            lblMetadata.Text = String.Empty
            If selectedCategory.IdMetadataRepository.HasValue Then
                AjaxManager.ResponseScripts.Add(String.Format(CATEGORY_METADATA_DETAILS_CALLBACK, selectedCategory.IdMetadataRepository.Value))
            End If
        End If
        AjaxManager.ResponseScripts.Add(String.Format(SET_VISIBILITY_PANEL, roles.Count))
    End Sub

    Protected Sub TbltClassificatore_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arr As String() = e.Argument.Split({"|"c}, StringSplitOptions.None)

        Dim operation As String = arr(0)
        Dim IdCategory As Integer? = Nothing
        Dim node As RadTreeNode = Nothing
        If arr.Length > 1 Then
            IdCategory = Integer.Parse(arr(1))
            If arr.Length = 3 Then
                node = JsonConvert.DeserializeObject(Of RadTreeNode)(arr(2))
            End If
        End If

        If operation.Contains("uscSettori") Then
            operation = "uscSettori"
        End If

        Select Case operation
            Case "ReloadNodes"
                CategoriesNotFascicled = Nothing
                CategoriesProcedureFascicles = Nothing
                CategorySubFascicles = Nothing
                rtvCategories.Nodes(0).Nodes.Clear()
                pnlDetails.Visible = False
                CategoriesWithChildren = Nothing
                LoadNodes(rtvCategories.Nodes(0))
                If Not HasFilters AndAlso IdCategory.HasValue Then
                    Dim expandedCategory As Category = Facade.CategoryFacade.GetById(IdCategory.Value)
                    For Each idParentCategory As String In expandedCategory.FullIncrementalPath.Split("|"c)
                        If rtvCategories.FindNodeByValue(idParentCategory) IsNot Nothing Then
                            rtvCategories.FindNodeByValue(idParentCategory).Expanded = True
                            LoadNodes(rtvCategories.FindNodeByValue(idParentCategory))
                        End If
                    Next
                    If rtvCategories.FindNodeByValue(IdCategory.ToString()) IsNot Nothing Then
                        rtvCategories.FindNodeByValue(IdCategory.ToString()).Selected = True
                        Me.rtvCategories_NodeClick(rtvCategories, New RadTreeNodeEventArgs(rtvCategories.FindNodeByValue(IdCategory.ToString())))
                    Else
                        rtvCategories.Nodes(0).Selected = True
                    End If
                End If
                AjaxManager.ResponseScripts.Add("ReloadNodesCallback()")
            Case "ExpandNode"
                If rtvCategories.FindNodeByValue(node.Value).Nodes.Count = 0 AndAlso Not HasFilters Then
                    LoadNodes(node)
                End If
            Case "uscSettori"
                Dim categoryFascicleRight As New CategoryFascicleRight
                Dim selectedCategory As Category = Facade.CategoryFacade.GetById(CurrentCategorySelected.Value)
                AddRole(IdCategory.Value)
                SetDefaultAllUserRole()
                LoadRoleUsers(selectedCategory.Id)
                AjaxManager.ResponseScripts.Add(String.Format(SET_VISIBILITY_PANEL, 1))
            Case "ReloadRoleUsers"
                Dim categoryFascicleRight As New CategoryFascicleRight
                Dim selectedCategory As Category = Facade.CategoryFacade.GetById(CurrentCategorySelected.Value)
                LoadRoleUsers(selectedCategory.Id)
            Case "ResetPeriodicCategoryFascicles"
                DeletePeriodicCategoryFascicles()
                AjaxManager.RaisePostBackEvent(String.Concat("ReloadNodes|", CurrentCategorySelected.Value))

        End Select

        ' Case "ProcedureExternalDataCallback"
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try
        If ajaxModel Is Nothing Then
            Return
        End If
        Select Case ajaxModel.ActionName
            Case "ProcedureExternalDataCallback"
                If ajaxModel.Value IsNot Nothing Then
                    Dim categoryFascicle As CategoryFascicle = JsonConvert.DeserializeObject(Of CategoryFascicle)(ajaxModel.Value(0))
                    If categoryFascicle IsNot Nothing Then
                        Dim currentCategory As Category = Facade.CategoryFacade.GetById(categoryFascicle.Category.Id)
                        categoryFascicle.Category = currentCategory
                        CurrentCategoryFascicleFacade.Save(categoryFascicle)
                        'settore speciale se neccessario
                        If ProtocolEnv.FascicleContainerEnabled AndAlso CurrentCategoryFascicleRightFacade.GetByIdCategoryFascicle(currentCategory.UniqueId).Count = 0 Then
                            AddRole(FacadeFactory.Instance.RoleFacade.DefaultAllUserRole.Id)
                        End If
                        uscSettori.Visible = True
                        AjaxManager.RaisePostBackEvent(String.Concat("ReloadNodes|", currentCategory.Id.ToString()))
                    End If
                End If
                Exit Select
        End Select

    End Sub
    Protected Sub ddlEnvironments_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        CategoriesNotFascicled = Nothing
        CategoriesProcedureFascicles = Nothing
        CategorySubFascicles = Nothing
        rtvCategories.Nodes(0).Nodes.Clear()
        pnlDetails.Visible = False
        LoadNodes(rtvCategories.Nodes(0))
        rtvCategories.Nodes(0).Selected = True
        AjaxManager.ResponseScripts.Add(String.Format("UpdateVisibility('{0}')", False))
    End Sub

    Protected Sub ToolBarSearch_ButtonClick(sender As Object, e As RadToolBarEventArgs)
        rtvCategories.Nodes(0).Nodes.Clear()
        pnlDetails.Visible = False
        LoadNodes(rtvCategories.Nodes(0))
        rtvCategories.Nodes(0).Selected = True
        AjaxManager.ResponseScripts.Add(String.Format("UpdateVisibility('{0}')", False))
    End Sub

    Protected Sub ToolBarStatus_ButtonClick(sender As Object, e As EventArgs)
        rtvCategories.Nodes(0).Nodes.Clear()
        pnlDetails.Visible = False
        LoadNodes(rtvCategories.Nodes(0))
        rtvCategories.Nodes(0).Selected = True
        AjaxManager.ResponseScripts.Add(String.Format("UpdateVisibility('{0}')", False))
    End Sub

    Protected Sub rcbSchemas_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        rtvCategories.Nodes(0).Nodes.Clear()
        pnlDetails.Visible = False
        LoadNodes(rtvCategories.Nodes(0))
        rtvCategories.Nodes(0).Selected = True
        AjaxManager.ResponseScripts.Add(String.Format("UpdateVisibility('{0}')", False))
    End Sub

    Private Sub uscSettori_RoleRemoved(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles uscSettori.RoleRemoved
        Dim idRole As Integer = Convert.ToInt32(e.Node.Value)
        Dim numberOfRoles As Integer = 0
        DeleteRole(idRole)
        SetDefaultAllUserRole()
        If CurrentCategorySelected.HasValue Then
            LoadRoleUsers(CurrentCategorySelected.Value)
            If CurrentCategoryFascicleRightFacade.GetByIdCategory(CurrentCategorySelected.Value).Any(Function(x) x.Role.Id <> FacadeFactory.Instance.RoleFacade.DefaultAllUserRole.Id) Then
                numberOfRoles = 1
            End If
        End If
        AjaxManager.ResponseScripts.Add(String.Format(SET_VISIBILITY_PANEL, numberOfRoles))
        AjaxManager.ResponseScripts.Add(String.Format(SEND_AJAX_REQUEST, "ReloadRoleUsers"))
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()

    End Sub
    Private Sub InitializeButtons()
        If IsCategoryManager Then
            btnAggiungi.Visible = True
            btnModifica.Visible = True
            btnElimina.Visible = True
            btnLog.Visible = DocSuiteContext.Current.ProtocolEnv.IsLogEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblCategoryRight)

            SearchOnlyFascicolable.Visible = DocSuiteContext.Current.ProtocolEnv.FascicleEnabled
            btnMassimari.Visible = True
            btnMetadata.Visible = ProtocolEnv.MetadataRepositoryEnabled
            CreateContextMenu(rtvCategories)
        Else
            btnAggiungi.Visible = False
            btnModifica.Visible = False
            btnElimina.Visible = False
            btnRecovery.Visible = False
            btnLog.Visible = False

            btnMassimari.Visible = False
            btnMetadata.Visible = False
            btnRunFasciclePlan.Visible = False
            btnCloseFasciclePlan.Visible = False
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltClassificatore_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvCategories)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDetails)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlInfo)


        AjaxManager.AjaxSettings.AddAjaxSetting(rtvCategories, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvCategories, pnlSettori, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvCategories, pnlInfo)

        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch, splPage, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarStatus, splPage, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, pnlInfo, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, rtvCategories, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub CreateContextMenu(ByRef tree As RadTreeView)
        If Not DocSuiteContext.IsFullApplication Then
            Exit Sub
        End If

        Dim menu As RadTreeViewContextMenu = New RadTreeViewContextMenu()
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Aggiungi", "Add", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Modifica", "Rename", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Elimina", "Delete", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Recupera", "Recovery", Unit.Pixel(200)))
        If DocSuiteContext.Current.ProtocolEnv.IsLogEnabled Then
            menu.Items.Add(TreeViewUtils.CreateMenuItem("Log", "Log", Unit.Pixel(200)))
        End If
        tree.ContextMenus.Add(menu)
        tree.OnClientContextMenuItemClicked = "OnContextMenuItemClicked"
        tree.OnClientContextMenuShowing = "OnContextMenuShowing"
    End Sub

    Private Sub LoadNodes(ByRef father As RadTreeNode)
        Dim categoryFinder As CategoryFinder = CurrentCategoryFinder
        If Not String.IsNullOrEmpty(father.Value) Then
            categoryFinder.ParentId = Convert.ToInt32(father.Value)
        End If

        'categoryFinder.IsActive = True

        If HasFilters Then
            categoryFinder.Name = SearchCategoryTextBox.Text
            If Not String.IsNullOrEmpty(SearchCategoryCodeTextBox.Text) Then
                categoryFinder.FullCode = Facade.CategoryFacade.FormatCategoryFullCode(SearchCategoryCodeTextBox.Text)
            End If
            categoryFinder.CheckFascicolable = SearchOnlyFascicolable.Checked
        End If

        Dim categorySchemaId As Guid = Guid.Empty
        Dim currentSchema As CategorySchema = Facade.CategorySchemaFacade.GetCurrentCategorySchema()

        If Not String.IsNullOrEmpty(SearchSchema.SelectedValue) AndAlso Guid.TryParse(SearchSchema.SelectedValue, categorySchemaId) Then
            categoryFinder.CategorySchemaId = categorySchemaId
            categoryFinder.IsCurrentSchema = categorySchemaId = currentSchema.Id
            currentSchema = Facade.CategorySchemaFacade.GetById(categorySchemaId)

        End If

        categoryFinder.ViewActive = SearchActive
        categoryFinder.ViewDisabled = SearchDisabled

        If SearchDisabled Then
            categoryFinder.IsActive = False
        End If

        categoryFinder.SortExpressions.Add(New SortExpression(Of Category) With {.Direction = SortDirection.Ascending, .Expression = Function(x) x.Code})
        Dim categories As ICollection(Of Category) = categoryFinder.DoSearch()
        For Each item As Category In categories
            AddNode(item, currentSchema)
        Next
        rtvRoleUsers.Nodes.Clear()
        If categoryFinder.ParentId.HasValue Then
            LoadRoleUsers(categoryFinder.ParentId.Value)
        End If
    End Sub

    Private Function AddNode(ByVal category As Category, categorySchema As CategorySchema) As RadTreeNode
        Dim currentNode As RadTreeNode = rtvCategories.FindNodeByValue(category.Id.ToString())
        If currentNode IsNot Nothing Then
            Return Nothing
        End If

        Dim father As RadTreeNode
        If category.Parent IsNot Nothing Then
            father = rtvCategories.FindNodeByValue(category.Parent.Id.ToString())
            If father Is Nothing Then
                father = AddNode(category.Parent, categorySchema)
            End If
        Else

            father = rtvCategories.Nodes(0)
        End If

        Dim nodeToAdd As RadTreeNode = CreateNode(category, categorySchema)
        father.Nodes.Add(nodeToAdd)
        Return nodeToAdd
    End Function

    Private Function CreateNode(ByVal category As Category, categorySchema As CategorySchema) As RadTreeNode
        Dim vNode As New RadTreeNode()
        vNode.Text = category.GetFullName()
        Dim schemaEndDate As DateTimeOffset = If(categorySchema.EndDate.HasValue, categorySchema.EndDate.Value, DateTimeOffset.MaxValue)
        Dim categoryActive As Boolean = False
        If categorySchema.StartDate > DateTimeOffset.UtcNow Then
            categoryActive = category.IsActive = 1 AndAlso (category.EndDate Is Nothing OrElse category.EndDate.Value > schemaEndDate)
        Else
            categoryActive = Facade.CategoryFacade.IsCategoryActive(category) AndAlso category.IsActive = 1
        End If

        Dim categoryFuture As Boolean = category.StartDate > DateTimeOffset.UtcNow
        If Not categoryActive AndAlso Not categoryFuture Then
            vNode.CssClass = String.Concat(vNode.CssClass, "node-disabled")
        Else
            vNode.CssClass = vNode.CssClass.Replace("node-disabled", "")
        End If

        SetNodoHasFascicle(vNode, category, (categoryActive OrElse categoryFuture))

        vNode.Attributes.Add("Code", category.Code.ToString())
        vNode.Attributes.Add("Active", (categoryActive OrElse categoryFuture).ToString())
        vNode.Attributes.Add("IsRecoverable", (Not categoryActive AndAlso Not categoryFuture).ToString())
        vNode.Attributes.Add("NodeType", If(category.Parent Is Nothing, "Category", "SubCategory"))
        vNode.Value = category.Id.ToString()

        vNode.Attributes.Add("UniqueId", category.UniqueId.ToString())
        Dim categoryHasChildren As Boolean = CategoriesWithChildren.Any(Function(x) x = category.Id)
        vNode.Attributes.Add("HasChildren", categoryHasChildren.ToString())
        If (categoryHasChildren) Then
            vNode.ImageUrl = "../Comm/images/FolderOpen16.gif"
            vNode.Expanded = HasFilters
            If Not HasFilters Then
                vNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            End If
        Else
            vNode.ImageUrl = "../Comm/images/Classificatore.gif"
        End If

        If ProtocolEnv.MetadataRepositoryEnabled AndAlso category.IdMetadataRepository.HasValue Then
            vNode.Attributes.Add("MetadataRepository", category.IdMetadataRepository.Value.ToString())
        End If

        Return vNode

    End Function

    Private Sub SetNodoHasFascicle(ByRef node As RadTreeNode, category As Category, isActive As Boolean)
        Dim hasFascicle As Boolean = Not CategoriesNotFascicled.Any(Function(x) x.Equals(category.Id))
        Dim hasProcedureFascicle As Boolean = CategoriesProcedureFascicles.Any(Function(x) x.Equals(category.Id))
        If hasFascicle AndAlso isActive Then
            If CategorySubFascicles.Any(Function(x) x = category.Id) Then
                node.CssClass = "node-subfascicle"
                SetCurrentNodeIsSubFascicle(node, True)
            Else
                node.CssClass = "node-fascicle"
                SetCurrentNodeIsSubFascicle(node, False)
            End If
            node.BorderStyle = BorderStyle.None

            SetCurrentNodeHasFascicle(node, True, hasProcedureFascicle)
            SetCurrentNodeMustHaveFascicle(node, True)
        Else
            If Not isActive Then
                SetCurrentNodeHasFascicle(node, False, True)
                SetCurrentNodeMustHaveFascicle(node, False)
            Else
                ' Nodo finale al quale manca il fascicolo
                node.CssClass = "node-no-fascicle"
                node.BorderStyle = BorderStyle.None
                SetCurrentNodeHasFascicle(node, False, False)
                SetCurrentNodeMustHaveFascicle(node, True)
            End If
        End If
    End Sub

    Private Function GetCategoriesNotFascicoled() As IList(Of Integer)
        Return CurrentCategoryFascicleFacade.GetNotFascicoled()
    End Function

    Private Function GetProcedureFascicles() As IList(Of Integer)
        Return CurrentCategoryFascicleFacade.GetProcedureFascicles()
    End Function

    Private Function GetSubFascicles() As IList(Of Integer)
        Return CurrentCategoryFascicleFacade.GetCategorySubFascicles() _
                        .Select(Function(s) s.Category.Id) _
                        .ToList()
    End Function

    Private Sub SetCurrentNodeIsSubFascicle(ByRef node As RadTreeNode, IsSubFascicle As Boolean)
        node.Attributes.Add("IsSubFascicle", IsSubFascicle.ToString())
    End Sub

    Private Sub SetCurrentNodeHasFascicle(ByRef node As RadTreeNode, hasFascicle As Boolean, hasProcedureFascicle As Boolean)
        node.Attributes.Add("HasFascicle", hasFascicle.ToString())
        node.Attributes.Add("HasProcedureFascicle", hasProcedureFascicle.ToString())

    End Sub

    Private Sub SetCurrentNodeMustHaveFascicle(ByRef node As RadTreeNode, HasFascicle As Boolean)
        node.Attributes.Add("MustHaveFascicle", HasFascicle.ToString())
    End Sub

    Private Function GetMassimarioFullName(idCategory As Integer) As String
        Dim category As Category = Facade.CategoryFacade.GetById(idCategory)
        If Not category.IdMassimarioScarto.HasValue Then
            Return String.Empty
        End If

        Return CurrentMassimarioScartoFacade.GetFullName(category.IdMassimarioScarto.Value)
    End Function

    Private Sub FillComboBoxCategorySchemas()
        Dim schemas As ICollection(Of CategorySchema) = Facade.CategorySchemaFacade.GetAllOrdered("StartDate ASC")
        SearchSchema.Items.AddRange(schemas.Select(Function(s) New RadComboBoxItem(String.Format("Versione dal {0}", s.StartDate.DefaultString()), s.Id.ToString())))

        Dim currentSchema As CategorySchema = Facade.CategorySchemaFacade.GetCurrentCategorySchema()
        SearchSchema.SelectedValue = currentSchema.Id.ToString()
    End Sub

    Private Function GetCategoriesWithChildren() As IList(Of Integer)
        Return Facade.CategoryFacade.GetWithChildren()
    End Function

    Private Sub DeleteRole(idRole As Integer)
        If Not CurrentCategorySelected.HasValue Then
            AjaxAlert($"Non è possibile rimuovere il settore {idRole} senza aver selezionato un nodo del classificatore")
            Exit Sub
        End If

        Dim selectedCategory As Category = Facade.CategoryFacade.GetById(CurrentCategorySelected.Value)
        Dim categories As ICollection(Of CategoryFascicle) = CurrentCategoryFascicleFacade.GetByIdCategory(selectedCategory.Id)
        Dim roles As List(Of Role) = New List(Of Role)
        Dim role As Role = New Role()
        Dim categoryFascicle As CategoryFascicle = categories.Where(Function(x) x.FascicleType = FascicleType.Procedure AndAlso x.DSWEnvironment = 0).FirstOrDefault()
        If categoryFascicle IsNot Nothing Then
            Dim categoryRight As ICollection(Of CategoryFascicleRight) = CurrentCategoryFascicleRightFacade.GetByIdCategoryRole(categoryFascicle.Id, idRole)
            If categoryRight.Count > 0 Then
                CurrentCategoryFascicleRightFacade.Delete(categoryRight.First())
            End If
        End If

        Dim periodicFascicles As ICollection(Of CategoryFascicle) = categories.Where(Function(x) x.FascicleType = FascicleType.Period).ToList()
        Dim periodicCategoryRights As ICollection(Of CategoryFascicleRight) = Nothing
        For Each periodicFascicle As CategoryFascicle In periodicFascicles
            periodicCategoryRights = CurrentCategoryFascicleRightFacade.GetByIdCategoryRole(periodicFascicle.Id, idRole)
            If periodicCategoryRights.Count > 0 Then
                CurrentCategoryFascicleRightFacade.Delete(periodicCategoryRights.First())
            End If
        Next
    End Sub

    Private Sub SetDefaultAllUserRole()
        If ProtocolEnv.FascicleContainerEnabled Then
            Dim selectedCategory As Category = Facade.CategoryFacade.GetById(CurrentCategorySelected.Value)
            Dim categories As ICollection(Of CategoryFascicle) = CurrentCategoryFascicleFacade.GetByIdCategory(selectedCategory.Id)
            Dim categoryFascicle As CategoryFascicle = categories.Where(Function(x) x.FascicleType = FascicleType.Procedure AndAlso x.DSWEnvironment = 0).FirstOrDefault()
            Dim categoryFascicleRights As ICollection(Of CategoryFascicleRight) = New List(Of CategoryFascicleRight)()
            If categoryFascicle IsNot Nothing Then
                categoryFascicleRights = CurrentCategoryFascicleRightFacade.GetByIdCategoryFascicle(categoryFascicle.Id)
            End If

            If categoryFascicleRights.Count > 1 AndAlso categoryFascicleRights.Any(Function(x) x.Role.Id = FacadeFactory.Instance.RoleFacade.DefaultAllUserRole.Id) Then
                DeleteRole(FacadeFactory.Instance.RoleFacade.DefaultAllUserRole.Id)
            End If
            If categoryFascicleRights.Count = 0 Then
                AddRole(FacadeFactory.Instance.RoleFacade.DefaultAllUserRole.Id)
            End If
        End If
    End Sub

    Protected Sub AddRole(idRole As Integer)
        If Not CurrentCategorySelected.HasValue Then
            AjaxAlert($"Non è possibile aggiungere il settore {idRole} senza aver selezionato un nodo del classificatore")
            Exit Sub
        End If
        Dim selectedRole As Role = Facade.RoleFacade.GetById(idRole)
        If uscSettori.GetRoles().Any(Function(x) x.Id = selectedRole.Id) Then
            AjaxAlert($"Non è possibile selezionare il settore {selectedRole.Name} in quanto già presente")
            Exit Sub
        End If
        Dim selectedCategory As Category = Facade.CategoryFacade.GetById(CurrentCategorySelected.Value)
        Dim categories As ICollection(Of CategoryFascicle) = CurrentCategoryFascicleFacade.GetByIdCategory(selectedCategory.Id)
        Dim categoryProcedureFascicle As CategoryFascicle = categories.Where(Function(x) x.FascicleType = FascicleType.Procedure AndAlso x.DSWEnvironment = 0).FirstOrDefault()
        Dim categoryFascicleRight As CategoryFascicleRight = Nothing
        If categoryProcedureFascicle IsNot Nothing Then
            categoryFascicleRight = New CategoryFascicleRight With {
                .Role = selectedRole,
                .CategoryFascicle = categoryProcedureFascicle
            }
            CurrentCategoryFascicleRightFacade.Save(categoryFascicleRight)
        End If
        Dim periodicFascicles As ICollection(Of CategoryFascicle) = categories.Where(Function(x) x.FascicleType = FascicleType.Period).ToList()
        For Each periodicFascicle As CategoryFascicle In periodicFascicles.Where(Function(f) Not CurrentCategoryFascicleRightFacade.HasCategoryFascicleRight(f.Id, selectedRole.Id))
            categoryFascicleRight = New CategoryFascicleRight()
            categoryFascicleRight.Role = selectedRole
            categoryFascicleRight.CategoryFascicle = periodicFascicle
            CurrentCategoryFascicleRightFacade.Save(categoryFascicleRight)
        Next
    End Sub

    Private Sub DeletePeriodicCategoryFascicles()
        If Not CurrentCategorySelected.HasValue Then
            AjaxAlert($"Selezionare un nodo del classificatore")
            Exit Sub
        End If

        Dim selectedCategory As Category = Facade.CategoryFacade.GetById(CurrentCategorySelected.Value)
        Dim categories As ICollection(Of CategoryFascicle) = CurrentCategoryFascicleFacade.GetByIdCategory(selectedCategory.Id)
        Dim periodicFascicles As ICollection(Of CategoryFascicle) = categories.Where(Function(x) x.FascicleType = FascicleType.Period).ToList()
        Dim selectedRole As Role = FacadeFactory.Instance.RoleFacade.DefaultAllUserRole
        Dim periodicRights As ICollection(Of CategoryFascicleRight)
        Dim specialRight As CategoryFascicleRight
        For Each periodicFascicle As CategoryFascicle In periodicFascicles
            periodicRights = CurrentCategoryFascicleRightFacade.GetByIdCategoryFascicle(periodicFascicle.Id)
            For Each periodicRight As CategoryFascicleRight In periodicRights
                CurrentCategoryFascicleRightFacade.Delete(periodicRight)
            Next

            If ProtocolEnv.FascicleContainerEnabled Then
                specialRight = New CategoryFascicleRight()
                specialRight.Role = selectedRole
                specialRight.CategoryFascicle = periodicFascicle
                CurrentCategoryFascicleRightFacade.Save(specialRight)
            End If
        Next
    End Sub

    Private Sub LoadRoleUsers(categoryIdSelected As Integer)
        pnlDetails.Visible = True
        rtvRoleUsers.Nodes.Clear()
        Dim roles As ICollection(Of Role) = CurrentCategoryFascicleRightFacade.GetByIdCategory(categoryIdSelected).Select(Function(f) f.Role).ToList()
        Dim roleRootNode As RadTreeNode
        Dim rootResponsabiliProcedimento As RadTreeNode
        Dim rootSegreteriaProcedimento As RadTreeNode
        For Each role As Role In roles
            roleRootNode = CreateRoleUserRootNode(role.Name, Nothing)
            rtvRoleUsers.Nodes.Add(roleRootNode)
            rootResponsabiliProcedimento = CreateRoleUserRootNode(DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel, RoleUserType.RP)
            roleRootNode.Nodes.Add(rootResponsabiliProcedimento)
            rootSegreteriaProcedimento = CreateRoleUserRootNode(DocSuiteContext.Current.ProtocolEnv.FascicleRoleSPLabel, RoleUserType.SP)
            roleRootNode.Nodes.Add(rootSegreteriaProcedimento)
            For Each roleUser As RoleUser In role.RoleUsers.Where(Function(f) f.Type = RoleUserType.RP.ToString() AndAlso (f.DSWEnvironment = DSWEnvironment.Protocol OrElse f.DSWEnvironment = 0))
                rootResponsabiliProcedimento.Nodes.Add(CreateRoleUserUserNode(roleUser))
            Next
            For Each roleUser As RoleUser In role.RoleUsers.Where(Function(f) f.Type = RoleUserType.SP.ToString() AndAlso (f.DSWEnvironment = DSWEnvironment.Protocol OrElse f.DSWEnvironment = 0))
                rootSegreteriaProcedimento.Nodes.Add(CreateRoleUserUserNode(roleUser))
            Next
        Next
        If Not roles.Any() Then
            pnlDetails.Visible = False
        End If
    End Sub

    Private Function CreateRoleUserRootNode(ByVal txt As String, ByVal value As RoleUserType?) As RadTreeNode
        Dim tn As New RadTreeNode
        tn.Text = txt
        tn.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/bricks.png"
        If value.HasValue Then
            tn.ImageUrl = "../Comm/images/Interop/Ruolo.gif"
            tn.Attributes("Type") = value.ToString()
        End If
        tn.Font.Bold = True
        tn.Expanded = True
        tn.Checkable = False

        Return tn
    End Function

    Private Function CreateRoleUserUserNode(ByVal roleUser As RoleUser) As RadTreeNode
        Dim tn As RadTreeNode = New RadTreeNode
        tn.Text = String.Format("{0} ({1})", roleUser.Description, roleUser.Email)
        tn.Value = roleUser.Account
        tn.AddAttribute("account", roleUser.Account)
        tn.Expanded = True
        tn.ImageUrl = "../App_Themes/DocSuite2008/imgset16/user.png"
        tn.Checkable = False
        tn.Attributes("Type") = roleUser.Type
        Return tn
    End Function
#End Region

End Class


