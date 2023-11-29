Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Drawing
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json

Partial Public Class uscClassificatore
    Inherits DocSuite2008BaseControl

    Public Delegate Sub CategoryAddedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub CategoryAddingEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub CategoryRemovedEventHandler(ByVal sender As Object, ByVal e As EventArgs)

    Public Event CategoryAdded As CategoryAddedEventHandler
    Public Event CategoryAdding As CategoryAddingEventHandler
    Public Event CategoryRemoved As CategoryRemovedEventHandler

#Region " Fields "

    Private _action As String
    Private _multiple As Boolean
    Private _onlyActive As Boolean = True
    Private _categoryList As IList(Of Category)

#End Region

#Region " Properties "

    Public Property CategoryID() As Integer
        Get
            If ViewState.Item("_idCategory") IsNot Nothing Then
                Return DirectCast(ViewState.Item("_idCategory"), Integer)
            End If
            Return 0
        End Get
        Set(ByVal value As Integer)
            ViewState.Item("_idCategory") = value
        End Set
    End Property

    Public Property Action() As String
        Get
            Return _action
        End Get
        Set(ByVal value As String)
            _action = value
        End Set
    End Property

    Public Property Rights As String

    Public Property ViewOnlyFascicolable As Boolean?

    Public Property EnableOnlyFascicleProcedure As Boolean?

    Public Property FromDate As DateTime?
        Get
            If ViewState("FromDate") IsNot Nothing Then
                Return DirectCast(ViewState("FromDate"), DateTime)
            End If
            Return Nothing
        End Get
        Set(value As DateTime?)
            ViewState("FromDate") = value
        End Set
    End Property

    Public Property ToDate As DateTime?
        Get
            If ViewState("ToDate") IsNot Nothing Then
                Return DirectCast(ViewState("ToDate"), DateTime)
            End If
            Return Nothing
        End Get
        Set(value As DateTime?)
            ViewState("ToDate") = value
        End Set
    End Property

    Public Property Year As Integer?
        Get
            If ViewState("Year") IsNot Nothing Then
                Return DirectCast(ViewState("Year"), Integer)
            End If
            Return Nothing
        End Get
        Set(value As Integer?)
            ViewState("Year") = value
        End Set
    End Property

    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property

    Public Property DataSource() As IList(Of Category)
        Get
            If _categoryList Is Nothing Then
                _categoryList = New List(Of Category)
            End If
            Return _categoryList
        End Get
        Set(ByVal value As IList(Of Category))
            _categoryList = value
        End Set
    End Property

    Public Property Multiple() As Boolean
        Get
            Return _multiple
        End Get
        Set(ByVal value As Boolean)
            _multiple = value
        End Set
    End Property

    Public Property OnlyActive() As Boolean
        Get
            Return _onlyActive
        End Get
        Set(ByVal value As Boolean)
            _onlyActive = value
        End Set
    End Property

    Public Property [ReadOnly]() As Boolean
        Get
            Return Not ToolBar.Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Visible = Not value
        End Set
    End Property

    Public Property HeaderVisible() As Boolean
        Get
            Return tblHeader.Visible
        End Get
        Set(ByVal value As Boolean)
            tblHeader.Visible = value
        End Set
    End Property

    Public Property Required() As Boolean
        Get
            Return AnyNodeCheck.Enabled
        End Get
        Set(ByVal value As Boolean)
            AnyNodeCheck.Enabled = value
        End Set
    End Property

    ''' <summary> Modalità sottocategoria. </summary>
    ''' <remarks> Il controllo mostra solo un nodo e non scatena evento di cancellazione. </remarks>
    Public Property SubCategoryMode() As Boolean

    Public Property SubCategory() As Category
        Get
            Return If(HasSelectedCategories AndAlso SubCategoryMode, SelectedCategories().First(), Nothing)
        End Get
        Set(ByVal value As Category)
            If (SubCategoryMode) Then
                AddCategory(value)
            End If
        End Set
    End Property

    Public ReadOnly Property HasSelectedCategories() As Boolean
        Get
            Return RadTreeCategory.Nodes.Count > 0
        End Get
    End Property

    Public ReadOnly Property SelectedCategories As IList(Of Category)
        Get
            _categoryList = New List(Of Category)
            GetChildNodesRecursively(RadTreeCategory.Nodes, _categoryList)
            Return _categoryList
        End Get
    End Property

    Public ReadOnly Property TreeCategory As RadTreeView
        Get
            Return RadTreeCategory
        End Get
    End Property

    Public Property HideDeleteButton As Boolean

    Public Property HideAddButton As Boolean

    Public ReadOnly Property TableContentControl() As HtmlTable
        Get
            Return MainTable
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack() Then
            Initialize()
        End If
        AddHandler AnyNodeCheck.ServerValidate, AddressOf AnyNodeCheck_Validate
    End Sub

    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case sourceControl.CommandName
            Case "add"
                RaiseEvent CategoryAdding(Me, New EventArgs())
                AjaxManager.ResponseScripts.Add(String.Format("return {0}_OpenWindow({1},{2},'{3}');", ClientID, 600, 500, GetWindowParameters()))
            Case "delete"
                Dim tn As RadTreeNode = RadTreeCategory.SelectedNode
                If tn Is Nothing Then
                    BasePage.AjaxAlert("Nessun classificatore selezionato.")
                    Exit Select
                End If
                If UseSessionStorage Then
                    'se si sta eliminando un nodo padre, pulisco lo storage della sessione, altrimenti salvo in sessione il nodo che rimane
                    Dim ajaxMethod As String = String.Format("{0}_ClearSessionStorage();", ClientID)
                    If tn.ParentNode IsNot Nothing Then
                        Dim ajaxRequest As New AjaxModel()
                        ajaxRequest.ActionName = "false"
                        ajaxRequest.Value = New List(Of String)()
                        ajaxRequest.Value.Add(tn.ParentNode.Value.ToString())
                        ajaxMethod = String.Format("{0}_uscClassificatoreTS.setCategories({1});", ClientID, JsonConvert.SerializeObject(ajaxRequest))
                    End If
                    AjaxManager.ResponseScripts.Add(ajaxMethod)
                End If

                tn.Remove()
                RaiseEvent CategoryRemoved(Me, New EventArgs())
        End Select
    End Sub

    Private Sub uscClassificatore_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = e.Argument.Split("|"c)
        If Not arguments(0).Eq(ClientID) OrElse String.IsNullOrEmpty(arguments(1)) Then
            Exit Sub
        End If

        Dim category As Category = Facade.CategoryFacade.GetById(Integer.Parse(arguments(1)), False)
        AddCategory(category)
    End Sub

    Private Sub AnyNodeCheck_Validate(ByVal source As Object, ByVal args As ServerValidateEventArgs)
        args.IsValid = HasSelectedCategories
    End Sub


#End Region

#Region " Methods "
    Public Sub Initialize(Optional needClearSessionStorage As Boolean = True)
        _categoryList = New List(Of Category)
        DirectCast(ToolBar.FindButtonByCommandName("add"), RadToolBarButton).Visible = Not HideAddButton
        DirectCast(ToolBar.FindButtonByCommandName("delete"), RadToolBarButton).Visible = Not HideDeleteButton
        If Visible AndAlso UseSessionStorage AndAlso needClearSessionStorage Then
            AjaxManager.ResponseScripts.Add(String.Format("{0}_ClearSessionStorage();", ClientID))
        End If
    End Sub
    Private Sub InitializeAjax()
        If Visible Then
            AddHandler AjaxManager.AjaxRequest, AddressOf uscClassificatore_AjaxRequest

            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, RadTreeCategory)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeCategory)
        End If
    End Sub

    Public Sub Clear()
        RadTreeCategory.Nodes.Clear()
    End Sub

    Private Sub AddCategory(ByVal category As Category)
        If Not Multiple Then
            RadTreeCategory.Nodes.Clear()
        End If
        AddNode(Nothing, category)
        If UseSessionStorage Then
            Dim ajaxRequest As New AjaxModel()
            ajaxRequest.ActionName = "false"
            ajaxRequest.Value = New List(Of String)()
            ajaxRequest.Value.Add(category.Id.ToString())
            AjaxManager.ResponseScripts.Add(String.Format("{0}_uscClassificatoreTS.setCategories({1});", ClientID, JsonConvert.SerializeObject(ajaxRequest)))
        End If
        RaiseEvent CategoryAdded(Me, New EventArgs())
    End Sub

    Private Sub AddNode(ByRef node As RadTreeNode, ByVal category As Category)
        If RadTreeCategory.FindNodeByValue(category.Id.ToString()) IsNot Nothing Then
            Exit Sub
        End If

        Dim nodeToAdd As New RadTreeNode()
        SetNodo(nodeToAdd, category)
        If category IsNot Nothing Then
            If category.Parent Is Nothing OrElse SubCategoryMode OrElse (category.Parent IsNot Nothing AndAlso category.Parent.Code = 0) Then 'Primo Livello
                RadTreeCategory.Nodes.Add(nodeToAdd)
            Else
                Dim newNode As RadTreeNode = RadTreeCategory.FindNodeByValue(category.Parent.Id.ToString())
                If (newNode Is Nothing) Then
                    AddNode(nodeToAdd, category.Parent)
                Else
                    newNode.Nodes.Add(nodeToAdd)
                End If
            End If
        End If
        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If
    End Sub

    ''' <summary>
    ''' Imposta l'immagine e il colore del nodo.
    ''' Se il nodo NON è attivo viene colorato di grigio
    ''' </summary>
    ''' <param name="node"></param>
    ''' <param name="category"></param>
    Private Sub SetNodo(ByRef node As RadTreeNode, ByVal category As Category)
        SetNodoText(node, category)
        SetNodoIdCategory(node, category)
        SetNodoImage(node, category)
        SetNodoHasFascicle(node, category)
        SetNodoExpanded(node, True)
    End Sub

    Private Sub SetNodoText(ByRef node As RadTreeNode, ByVal category As Category)
        node.Text = category.GetFullName()
    End Sub

    Private Sub SetNodoIdCategory(ByRef node As RadTreeNode, ByVal category As Category)
        node.Value = category.Id.ToString()
    End Sub

    Private Sub SetNodoImage(ByRef node As RadTreeNode, ByVal category As Category)
        If (Not category.HasChildren) Then
            node.ImageUrl = "../Comm/images/Classificatore.gif"
        Else
            node.ImageUrl = "../Comm/images/folderopen16.gif"
        End If
    End Sub

    Private Sub SetNodoExpanded(ByRef node As RadTreeNode, Expanded As Boolean)
        node.Expanded = Expanded
    End Sub

    Private Sub SetNodoHasFascicle(ByRef node As RadTreeNode, category As Category)
        Dim hasFascicle As Boolean = CurrentCategoryHasFascicle(category)
        ' MUST -> Il cliente non può avere più di 3 livelli nelle categorie
        ' Se il nodo ha un fascicolo ed è di 3 livello (ultimo livello da gestire per i fascicoli)
        ' Oppure se il nodo non ha figli ed ha un fascicolo (livelli < 3)
        If (hasFascicle AndAlso node.Parent IsNot Nothing AndAlso node.Level = 3 AndAlso category.IsActive) Or
           (hasFascicle AndAlso Not category.HasChildren AndAlso category.IsActive) Then

            'node.BackColor = Color.FromArgb(150, 151, 230, 154)
            node.BorderStyle = BorderStyle.None
            SetCurrentNodeHasFascicle(node, True)
            SetCurrentNodeMustHaveFascicle(node, True)
        Else
            ' Nodo con altri figli. Non è richiesto un fascicolo su questo nodo.
            ' Oppure se la categoria non è piu attiva
            If category.HasChildren OrElse Not category.IsActive Then
                SetCurrentNodeHasFascicle(node, False)
                SetCurrentNodeMustHaveFascicle(node, False)
            Else
                ' Nodo finale al quale manca il fascicolo
                'node.BackColor = Color.FromArgb(150, 255, 117, 117)
                node.BorderStyle = BorderStyle.None
                SetCurrentNodeHasFascicle(node, False)
                SetCurrentNodeMustHaveFascicle(node, True)
            End If
        End If
    End Sub


    Private Function CurrentCategoryHasFascicle(category As Category) As Boolean
        Return Facade.FascicleFacade.CountFascicleByCategory(category) > 0
    End Function

    Private Sub SetCurrentNodeHasFascicle(ByRef node As RadTreeNode, HasFascicle As Boolean)
        node.Attributes.Add("HasFascicle", HasFascicle.ToString())
    End Sub
    ''' <summary>
    ''' Setta la proprietà nel nodo che indica se ha dei fascicoli
    ''' </summary>
    ''' <param name="node"></param>
    ''' <returns></returns>
    Public Function GetCurrentNodeHasFascicle(ByRef node As RadTreeNode) As Boolean
        Return Convert.ToBoolean(node.Attributes.Item("HasFascicle"))
    End Function

    Private Sub SetCurrentNodeMustHaveFascicle(ByRef node As RadTreeNode, HasFascicle As Boolean)
        node.Attributes.Add("MustHaveFascicle", HasFascicle.ToString())
    End Sub
    ''' <summary>
    ''' Setta la proprietà nel nodo che indica se ha dei fascicoli
    ''' </summary>
    ''' <param name="node"></param>
    ''' <returns></returns>
    Public Function GetCurrentNodeMustHaveFascicle(ByRef node As RadTreeNode) As Boolean
        Return Convert.ToBoolean(node.Attributes.Item("MustHaveFascicle"))
    End Function

    Private Sub GetChildNodesRecursively(ByVal nodes As RadTreeNodeCollection, ByRef categories As IList(Of Category))
        For Each node As RadTreeNode In nodes
            If node.Nodes.Count = 0 AndAlso Not String.IsNullOrEmpty(node.Value) Then
                categories.Add(Facade.CategoryFacade.GetById(Integer.Parse(node.Value)))
            End If
            GetChildNodesRecursively(node.Nodes, categories)
        Next
    End Sub

    Protected Function GetWindowParameters() As String
        Dim parameters As New List(Of String)
        parameters.Add("OnlyActive=" & If(OnlyActive AndAlso Not FromDate.HasValue AndAlso Not ToDate.HasValue AndAlso Not Year.HasValue, "1", "0"))

        If Not String.IsNullOrEmpty(Type) Then
            parameters.Add("Type=" & Type)
        End If
        If Not CategoryID = 0 Then
            parameters.Add("Category=" & CategoryID.ToString())
        End If
        If Not String.IsNullOrEmpty(Action) Then
            parameters.Add("Action=" & Action)
        End If

        If Not String.IsNullOrEmpty(Rights) Then
            parameters.Add("Rights=" & Rights)
        End If

        If ViewOnlyFascicolable.HasValue Then
            parameters.Add("ViewOnlyFascicolable=" & ViewOnlyFascicolable)
        End If

        If EnableOnlyFascicleProcedure.HasValue Then
            parameters.Add("EnableOnlyFascicleProcedure=" & EnableOnlyFascicleProcedure)
        End If

        If FromDate.HasValue AndAlso Not Year.HasValue Then
            parameters.Add("FromDate=" & FromDate.ToString())
        End If

        If ToDate.HasValue AndAlso Not Year.HasValue Then
            parameters.Add("ToDate=" & ToDate.ToString())
        End If

        If Year.HasValue Then
            parameters.Add("Year=" & Year.Value.ToString())
        End If

        Return String.Join("&", parameters)
    End Function

    Public Overrides Sub DataBind()
        ' todo: rivedere l'uso dell'override del datasource+databind a favore di custom initialization
        If _categoryList Is Nothing Then
            Exit Sub
        End If

        For Each category As Category In _categoryList
            If category IsNot Nothing Then
                AddCategory(category)
            End If
        Next
    End Sub

    Public Sub InitializeRadTreeCategory()
        If CategoryID <> 0 Then
            Dim category As Category = Facade.CategoryFacade.GetById(CategoryID)
            AddNode(Nothing, category)
        End If
    End Sub
#End Region

End Class