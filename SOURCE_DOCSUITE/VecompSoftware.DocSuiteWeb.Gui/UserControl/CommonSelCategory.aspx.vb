Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonSelCategory
    Inherits CommBasePage

#Region " Fields "

    Private _idCategory As Integer?
    Private _rights As Short?
    Private _onlyActive As Boolean?
    Private _viewOnlyFascicolable As Boolean?
    Private _enableOnlyFascicleProcedure As Boolean?
    Private _currentCategoryFascicleFacade As CategoryFascicleFacade
    Private _fromDate As DateTime?
    Private _toDate As DateTime?
    Private _year As Integer?
    Private _includeChildren As Boolean?

#End Region

#Region " Properties "

    Private ReadOnly Property IDCategory As Integer?
        Get
            If Not _idCategory.HasValue Then
                _idCategory = Request.QueryString.GetValueOrDefault(Of Integer?)("Category", Nothing)
            End If
            Return _idCategory
        End Get
    End Property

    Private ReadOnly Property Right As Short?
        Get
            If Not _rights.HasValue Then
                _rights = Request.QueryString.GetValueOrDefault(Of Short?)("Rights", Nothing)
            End If
            Return _rights
        End Get
    End Property

    Private ReadOnly Property ViewOnlyFascicolable As Boolean
        Get
            If Not _viewOnlyFascicolable.HasValue Then
                _viewOnlyFascicolable = Request.QueryString.GetValueOrDefault(Of Boolean)("ViewOnlyFascicolable", False)
            End If
            Return _viewOnlyFascicolable.Value
        End Get
    End Property

    Private ReadOnly Property EnableOnlyFascicleProcedure As Boolean
        Get
            If Not _enableOnlyFascicleProcedure.HasValue Then
                _enableOnlyFascicleProcedure = Request.QueryString.GetValueOrDefault(Of Boolean)("EnableOnlyFascicleProcedure", False)
            End If
            Return _enableOnlyFascicleProcedure.Value
        End Get
    End Property

    Private ReadOnly Property FromDate As DateTime?
        Get
            If Not _fromDate.HasValue Then
                _fromDate = Request.QueryString.GetValueOrDefault(Of DateTime?)("FromDate", Nothing)
            End If
            Return _fromDate
        End Get
    End Property

    Private ReadOnly Property Year As Integer?
        Get
            If Not _year.HasValue Then
                _year = Request.QueryString.GetValueOrDefault(Of Integer?)("Year", Nothing)
            End If
            Return _year
        End Get
    End Property

    Private ReadOnly Property ToDate As DateTime?
        Get
            If Not _toDate.HasValue Then
                _toDate = Request.QueryString.GetValueOrDefault(Of DateTime?)("ToDate", Nothing)
            End If
            Return _toDate
        End Get
    End Property

    Private ReadOnly Property OnlyActive As Boolean?
        Get
            If Not _onlyActive.HasValue Then
                Dim source As String = Request.QueryString.GetValueOrDefault(Of String)("OnlyActive", "0")
                _onlyActive = source.Eq("1")
            End If
            Return _onlyActive
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFascicleFacade As CategoryFascicleFacade
        Get
            If _currentCategoryFascicleFacade Is Nothing Then
                _currentCategoryFascicleFacade = New CategoryFascicleFacade()
            End If
            Return _currentCategoryFascicleFacade
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

    Private ReadOnly Property IncludeChildren As Boolean
        Get
            If Not _includeChildren.HasValue Then
                _includeChildren = Request.QueryString.GetValueOrDefault(Of Boolean)("IncludeChildren", False)
            End If
            Return _includeChildren.Value
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        btnConferma.Attributes.Add("onclick", "ReturnValues(); return false;")
        RadTreeCategory.OnClientNodeClicked = "ReturnValueOnClick"
        MasterDocSuite.TitleVisible = False
        txtSearch.Focus()

        If Not Page.IsPostBack Then
            rowOnlyFascicolable.Visible = Right.HasValue AndAlso ViewOnlyFascicolable
            btnSearchOnlyFascicolable.Checked = ViewOnlyFascicolable
            rowSelectCategorySchema.Visible = False
            If Action.Eq("Search") Then
                rowSelectCategorySchema.Visible = True
                FillComboBoxCategorySchemas()
                If Not FromDate.HasValue AndAlso Not ToDate.HasValue AndAlso Not Year.HasValue Then
                    Dim currentCategorySchema As CategorySchema = Facade.CategorySchemaFacade.GetCurrentCategorySchema()
                    If currentCategorySchema IsNot Nothing Then
                        rcbCategorySchemas.SelectedValue = currentCategorySchema.Id.ToString()
                    End If
                End If
            End If
            LoadRootNodes()
        End If
    End Sub

    Private Sub CategoryTreeNodeExpand(ByVal o As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeCategory.NodeExpand
        LoadNodes(e.Node)
    End Sub

    Protected Sub SearchClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearch.Click
        LoadRootNodes()
    End Sub

    Protected Sub btnSearchOnlyFascicolable_CheckedChanged(sender As Object, e As EventArgs)
        LoadRootNodes()
    End Sub

    Protected Sub rcbCategorySchemas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rcbCategorySchemas.SelectedIndexChanged
        LoadRootNodes()
    End Sub

    Protected Sub SearchCodeClick(sender As Object, e As EventArgs) Handles btnSearchCode.Click
        If Not String.IsNullOrEmpty(txtSearchCode.Text) Then
            Dim v As String() = txtSearchCode.Text.Split({"."c})
            If (Not String.IsNullOrEmpty(txtCategoryCode.Text) AndAlso (Not v(0).Eq(txtCategoryCode.Text))) Then
                AjaxAlert("Codice non valido.")
                Exit Sub
            End If
        End If

        Dim fullCode As String = Facade.CategoryFacade.FormatCategoryFullCode(txtSearchCode.Text)
        Dim finder As CategoryFinder = GetBaseFinder()
        finder.FullCode = fullCode
        Dim categories As ICollection(Of Category) = finder.DoSearch()

        If (categories.Count = 0) Then
            AjaxAlert("Codice inesistente")
            txtSearchCode.Focus()
        ElseIf (categories.Count > 1) Then
            AjaxAlert("Il Codice non è univoco")
            txtSearchCode.Focus()
        Else
            Dim currentCategory As Category = categories.First()
            If Right.HasValue Then
                Dim chkRight As Boolean = True
                If ViewOnlyFascicolable Then
                    chkRight = CurrentCategoryFascicleFacade.ExistFascicleProcedure(currentCategory.Id)
                End If

                If Not chkRight Then
                    AjaxAlert("Non si dispongono i permessi per questa voce del piano di fascicolazione.")
                    txtSearchCode.Focus()
                    Exit Sub
                End If
            End If
            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", currentCategory.Id.ToString()))
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, RadTreeCategory, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbCategorySchemas, RadTreeCategory, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchOnlyFascicolable, RadTreeCategory, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadRootNodes()
        If IDCategory.HasValue Then
            Dim categoryRoot As Category = Facade.CategoryFacade.GetById(IDCategory.Value)
            If Not categoryRoot.Code.Equals(0) Then
                txtCategoryCode.Text = categoryRoot.Code.ToString()
                txtSearchCode.Text = categoryRoot.Code & "."
                RadTreeCategory.Nodes(0).Text = categoryRoot.GetFullName()
                RadTreeCategory.Nodes(0).Value = categoryRoot.Id.ToString()
            End If
        End If

        RadTreeCategory.Nodes(0).Nodes.Clear()
        LoadNodes(RadTreeCategory.Nodes(0))
    End Sub

    Private Sub LoadNodes(ByRef father As RadTreeNode)
        Dim isSearch As Boolean = Not String.IsNullOrEmpty(txtSearch.Text)
        Dim categories As ICollection(Of Category)
        Dim authorizedCategories As ICollection(Of Category) = New List(Of Category)

        If ViewOnlyFascicolable AndAlso btnSearchOnlyFascicolable.Checked Then
            categories = GetFascicleAuthorizedCategories(father)
            AddNodes(categories, isSearch)
        Else
            categories = GetCategories(isSearch, father)
            If Right.HasValue Then
                authorizedCategories = GetFascicleAuthorizedCategories()
                AddNodes(categories, isSearch, authorizedCategories)
            Else
                AddNodes(categories, isSearch)
            End If
        End If
    End Sub

    Private Sub AddNodes(categories As ICollection(Of Category), isSearch As Boolean)
        AddNodes(categories, isSearch, categories)
    End Sub

    Private Sub AddNodes(categories As ICollection(Of Category), isSearch As Boolean, authorizedCategories As ICollection(Of Category))
        For Each item As Category In categories
            AddNode(item, isSearch, authorizedCategories)
        Next
    End Sub

    Private Function AddNode(category As Category, isSearch As Boolean, authorizedCategories As ICollection(Of Category)) As RadTreeNode
        Dim currentNode As RadTreeNode = RadTreeCategory.FindNodeByValue(category.Id.ToString())
        If currentNode IsNot Nothing Then
            Return Nothing
        End If

        Dim father As RadTreeNode
        If category.Parent IsNot Nothing AndAlso category.Parent.Code > 0 Then
            father = RadTreeCategory.FindNodeByValue(category.Parent.Id.ToString())
            If father Is Nothing Then
                ' Se non è ancora stato creato un padre per una classificatore esistente allora lo aggiungo
                father = AddNode(category.Parent, isSearch, authorizedCategories)
            End If
        Else
            ' Se non ha parent il nodo va nella root
            father = RadTreeCategory.Nodes(0)
        End If

        Dim nodeToAdd As New RadTreeNode()
        SetNode(nodeToAdd, category, isSearch, authorizedCategories)
        father.Nodes.Add(nodeToAdd)

        Return nodeToAdd
    End Function

    Private Sub SetNode(ByRef node As RadTreeNode, ByVal category As Category, ByVal isSearch As Boolean, authorizedCategories As ICollection(Of Category))
        node.Text = category.GetFullName()
        node.Font.Bold = Not String.IsNullOrEmpty(txtSearch.Text) AndAlso category.Name.ContainsIgnoreCase(txtSearch.Text)
        node.Value = category.Id.ToString()
        node.Attributes.Add("ID", category.Id.ToString())
        node.Attributes.Add("FullPath", category.FullIncrementalPath.Replace("|", ","))

        Dim categoryHasChildren As Boolean = CategoriesWithChildren.Any(Function(x) x = category.Id)
        If categoryHasChildren Then
            node.Expanded = isSearch OrElse ViewOnlyFascicolable
            If Not isSearch Then
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            End If
            node.ImageUrl = "../Comm/images/folderopen16.gif"
            node.Attributes.Add("TypeNode", "Node")
        Else
            node.ImageUrl = "../Comm/images/Classificatore.gif"
            node.Attributes.Add("TypeNode", "LeafNode")
        End If

        node.Attributes.Add("Rights", "1")

        If Right.HasValue Then
            Dim chkRight As Boolean = authorizedCategories.Any(Function(x) x.Id = category.Id)

            If chkRight OrElse Not ViewOnlyFascicolable Then
                node.ForeColor = Color.DarkBlue
                node.Attributes.Item("Rights") = "1"
            Else
                node.ContentCssClass = "RadTreeNodeOpacity"
                node.Attributes.Item("Rights") = "0"
            End If
        Else
            node.ForeColor = Color.Black
        End If

    End Sub

    Private Function GetBaseFinder() As CategoryFinder
        Dim categoryFinder As CategoryFinder = New CategoryFinder(New MapperCategoryModel(), DocSuiteContext.Current.User.FullUserName) With {
            .EnablePaging = False,
            .IsActive = OnlyActive.GetValueOrDefault(False),
            .IncludeZeroLevel = False
        }

        If Year.HasValue Then
            categoryFinder.Year = Year.Value
        End If

        If FromDate.HasValue Then
            categoryFinder.StartDate = FromDate.Value
        End If

        If ToDate.HasValue Then
            categoryFinder.EndDate = ToDate.Value
        End If

        If Not String.IsNullOrEmpty(rcbCategorySchemas.SelectedValue) Then
            Dim selectedCategorySchema As CategorySchema = Facade.CategorySchemaFacade.GetById(Guid.Parse(rcbCategorySchemas.SelectedValue))
            Dim currentCurrentCategorySchema As CategorySchema = Facade.CategorySchemaFacade.GetCurrentCategorySchema()
            categoryFinder.IsCurrentSchema = selectedCategorySchema.Version.Equals(currentCurrentCategorySchema.Version)
            categoryFinder.CategorySchemaDate = If(selectedCategorySchema.EndDate.HasValue, selectedCategorySchema.EndDate.Value, DateTimeOffset.UtcNow)
            categoryFinder.ViewActive = True
            categoryFinder.CategorySchemaId = selectedCategorySchema.Id

            If Not categoryFinder.IsCurrentSchema Then
                categoryFinder.IsActive = Nothing
                categoryFinder.ViewActive = Nothing
                categoryFinder.Year = Nothing
                categoryFinder.StartDate = Nothing
                categoryFinder.EndDate = Nothing
            End If
        End If

        categoryFinder.SortExpressions.Add(New SortExpression(Of Category) With {.Direction = SortDirection.Ascending, .Expression = Function(x) x.FullCode})
        Return categoryFinder
    End Function

    Private Function GetFascicleAuthorizedCategories(Optional parentNode As RadTreeNode = Nothing) As ICollection(Of Category)
        Dim categoryFinder As CategoryFinder = GetBaseFinder()
        categoryFinder.CheckFascicolable = True

        If Not String.IsNullOrEmpty(txtSearch.Text) Then
            categoryFinder.Name = txtSearch.Text
        End If

        If parentNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(parentNode.Value) Then
            Dim fatherCategoryId As Integer = Integer.Parse(parentNode.Value)
            categoryFinder.ParentId = fatherCategoryId
            categoryFinder.IncludeChildren = IncludeChildren
        End If

        categoryFinder.CheckFascicleProcedure = EnableOnlyFascicleProcedure

        Return categoryFinder.DoSearch()
    End Function

    Private Function GetCategories(isSearch As Boolean, parentNode As RadTreeNode) As ICollection(Of Category)
        Dim categoryFinder As CategoryFinder = GetBaseFinder()

        If Not String.IsNullOrEmpty(parentNode.Value) Then
            Dim fatherCategoryId As Integer = Integer.Parse(parentNode.Value)
            categoryFinder.ParentId = fatherCategoryId
        End If

        If isSearch Then
            categoryFinder.Name = txtSearch.Text
        End If

        Return categoryFinder.DoSearch()
    End Function

    Private Sub FillComboBoxCategorySchemas()
        rcbCategorySchemas.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))

        Dim schemas As ICollection(Of CategorySchema) = Facade.CategorySchemaFacade.GetManageableCategorySchemas(DateTimeOffset.UtcNow)
        rcbCategorySchemas.Items.AddRange(schemas.Select(Function(s) New RadComboBoxItem(String.Format("Versione dal {0}", s.StartDate.DefaultString()), s.Id.ToString())))

        Dim currentCategorySchema As CategorySchema = Nothing
        If Year.HasValue Then
            currentCategorySchema = Facade.CategorySchemaFacade.GetActiveCategorySchema(New DateTimeOffset(Year.Value, 1, 1, 0, 0, 0, TimeSpan.Zero))
        ElseIf FromDate.HasValue Then
            currentCategorySchema = Facade.CategorySchemaFacade.GetActiveCategorySchema(New DateTimeOffset(FromDate.Value, TimeSpan.Zero))
        ElseIf ToDate.HasValue Then
            currentCategorySchema = Facade.CategorySchemaFacade.GetActiveCategorySchema(New DateTimeOffset(ToDate.Value, TimeSpan.Zero))
        End If

        If currentCategorySchema IsNot Nothing Then
            rcbCategorySchemas.SelectedValue = currentCategorySchema.Id.ToString()
        Else
            rcbCategorySchemas.SelectedValue = String.Empty
        End If
    End Sub

    Private Function GetCategoriesWithChildren() As IList(Of Integer)
        Return Facade.CategoryFacade.GetWithChildren()
    End Function
#End Region

End Class