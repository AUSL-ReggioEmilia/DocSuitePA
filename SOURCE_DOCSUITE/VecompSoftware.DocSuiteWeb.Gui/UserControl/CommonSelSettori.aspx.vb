Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonSelSettori
    Inherits CommBasePage

#Region " Fields "

    Private _selected As String = String.Empty
    Private _tenantSelected As String = String.Empty
    Private _manageableRoles As IList(Of Integer)
    Private Const DefaultOpenWindowScript As String = "return {0}_OpenWindowOLD('{1}', '{2}', {0}{3});"
    Private _categoryFascicleRightFacade As CategoryFascicleRightFacade

#End Region

#Region " Properties "
    Private ReadOnly Property CurrentCategoryFascicleRightFacade As CategoryFascicleRightFacade
        Get
            If _categoryFascicleRightFacade Is Nothing Then
                _categoryFascicleRightFacade = New CategoryFascicleRightFacade()
            End If
            Return _categoryFascicleRightFacade
        End Get
    End Property

    Public ReadOnly Property ConfirmSelection As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("ConfirmSelection", False)
        End Get
    End Property

    Public ReadOnly Property RootSelectable As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("RootSelectable", False)
        End Get
    End Property


    Private ReadOnly Property RightEnabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("RightEnabled", True)
        End Get
    End Property


    Private ReadOnly Property Environment As DSWEnvironment
        Get
            Return Request.QueryString.GetValueOrDefault("DSWEnvironment", DSWEnvironment.Protocol)
        End Get
    End Property

    Private ReadOnly Property RoleRestriction() As RoleRestrictions
        Get
            Return Request.QueryString.GetValueOrDefault("RoleRestiction", RoleRestrictions.None)
        End Get
    End Property

    Private ReadOnly Property MultiSelect() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("MultiSelect", False)
        End Get
    End Property

    ''' <summary> Visualizza solo gli attivi. </summary>
    Private ReadOnly Property ShowActive() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("isActive", False)
        End Get
    End Property

    Public Property Selected() As String
        Get
            Return _selected
        End Get
        Set(ByVal value As String)
            _selected = value
        End Set
    End Property

    Public ReadOnly Property ManageableRoles As IList(Of Integer)
        Get
            _manageableRoles = New List(Of Integer)
            If Not String.IsNullOrEmpty(Request.QueryString.Item("ManageableRoles")) Then
                For Each roleId As String In Request.QueryString.Item("ManageableRoles").Split("|"c)
                    _manageableRoles.Add(Integer.Parse(roleId))
                Next
            End If
            Return _manageableRoles
        End Get
    End Property

    Private ReadOnly Property SearchByUserEnabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("SearchByUserEnabled", False)
        End Get
    End Property

    Private ReadOnly Property IdCategoryStringSelected() As String
        Get
            Return Request.QueryString.GetValueOrDefault("idCategorySelected", String.Empty)
        End Get
    End Property

    Private ReadOnly Property GetAllRolesEnabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("GetAllRolesEnabled", False)
        End Get
    End Property

    Public ReadOnly Property HidePanelByControlId As String
        Get
            Return Request.QueryString.GetValueOrDefault("HidePanelByControlId", String.Empty)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        InitializeAjax()
        InitializeRequestFields()
        InitializeButtons()

        If Not Page.IsPostBack Then
            Initialize()
            If DocSuiteContext.Current.ProtocolEnv.AutoLoadRoles Then
                BindRoles()
            End If
            txtFiltraSettori.Focus()
        End If

    End Sub

    Protected Sub uscContattiSel_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As String() = Split(arg, "|", 2)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If
        Dim localArg As String = HttpUtility.HtmlDecode(arguments(1))
        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(localArg)
        If contact IsNot Nothing Then
            lblSearchUser.Text = contact.Code
            RadTreeSettori.Nodes(0).Nodes.Clear()
            RadTreeSettori.Nodes(0).Expanded = True
            Dim results As IList(Of Role) = Facade.RoleFacade.GetRolesFromUserName(Environment, contact.Code, CurrentTenant.TenantAOO.UniqueId)
            LoadRoles(results)
        End If

    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        BindRoles()
    End Sub

    Private Sub RadTreeSettori_NodeExpand(sender As Object, e As RadTreeNodeEventArgs) Handles RadTreeSettori.NodeExpand

        If e.Node.Value.Eq("Root") AndAlso e.Node.Nodes.Count = 0 Then

            txtFiltraSettori.Text = String.Empty
            txtSearchCode.Text = String.Empty
            BindRoles()
        Else

            Dim children As IList(Of Role)
            Dim idTenantAOO As Guid = CurrentTenant.TenantAOO.UniqueId

            If e.Node.Value.Eq("Root") Then
                If RoleRestriction <> RoleRestrictions.None Then
                    children = Facade.RoleFacade.GetUserRoles(Env, 1, True, "", True, Nothing, CurrentTenant.TenantAOO.UniqueId)
                Else
                    children = Facade.RoleFacade.GetRoles(Env, 1, True, "", True, Nothing, idTenantAOO)
                End If
            Else
                Dim parentRole As Role = Facade.RoleFacade.GetById(CType(e.Node.Value, Integer))

                If RoleRestriction = RoleRestrictions.OnlyMine Then
                    children = Facade.RoleFacade.GetUserRoles(Env, 1, True, "", False, parentRole, idTenantAOO)
                Else
                    children = Facade.RoleFacade.GetRoles(Env, 1, True, "", False, parentRole, idTenantAOO)
                End If
            End If

            LoadChildrenRoles(e.Node, children)

            e.Node.ExpandMode = TreeNodeExpandMode.ClientSide

        End If
    End Sub

    Private Sub btnSearchCode_Click(sender As Object, e As EventArgs) Handles btnSearchCode.Click
        If String.IsNullOrEmpty(txtSearchCode.Text) Then
            AjaxAlert("Codice non valido.")
            Exit Sub
        End If
        Dim idTenantAOO As Guid = CurrentTenant.TenantAOO.UniqueId

        Dim role As Role
        Dim idCategorySelected As Integer
        If Integer.TryParse(IdCategoryStringSelected, idCategorySelected) Then
            Dim roleIds As IList(Of Integer) = CurrentCategoryFascicleRightFacade.GetByIdCategory(idCategorySelected).Select(Function(f) f.Role.Id).ToList()
            role = Facade.RoleFacade.GetByServiceCode(txtSearchCode.Text, idTenantAOO, roleIds, RoleUserType.RP, Environment)
            If role Is Nothing Then
                AjaxAlert("Codice non trovato.")
                Exit Sub
            End If
        Else
            role = Facade.RoleFacade.GetByServiceCode(txtSearchCode.Text, idTenantAOO, Nothing)
            If role Is Nothing Then
                AjaxAlert("Codice non trovato.")
                Exit Sub
            End If
        End If

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}')", role.Id))
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, RadTreeSettori, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeSettori, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlSearchAccount)
        AddHandler AjaxManager.AjaxRequest, AddressOf uscContattiSel_AjaxRequest

    End Sub

    Private Sub BindRoles()
        ' Svuoto l'albero dei Settori
        RadTreeSettori.Nodes(0).Nodes.Clear()
        RadTreeSettori.Nodes(0).Expanded = True

        Dim results As IList(Of Role) = Search(txtFiltraSettori.Text, CurrentTenant.TenantAOO.UniqueId)
        LoadRoles(results)

        If RootSelectable Then
            SetSelectableNode(RadTreeSettori.Nodes(0))
        End If
    End Sub

    Private Function Search(filter As String, idTenantAOO As Guid) As IList(Of Role)
        Dim isActive As Boolean? = Nothing
        If ShowActive Then
            isActive = True
        End If

        If ManageableRoles IsNot Nothing AndAlso ManageableRoles.Count > 0 Then
            Return Facade.RoleFacade.GetManageableRoles(ManageableRoles, isActive, idTenantAOO)
        End If

        Dim rightPosition As Integer? = Nothing
        If RoleRestriction <> RoleRestrictions.None Then
            rightPosition = 1
            If Environment = DSWEnvironment.Any Then
                rightPosition = Nothing
            End If

            Dim idCategorySelected As Integer
            If Integer.TryParse(IdCategoryStringSelected, idCategorySelected) Then
                Dim roleIds As IList(Of Integer) = CurrentCategoryFascicleRightFacade.GetByIdCategory(idCategorySelected).Select(Function(f) f.Role.Id).ToList()
                Return Facade.RoleFacade.GetUserRolesByCategory(Environment, roleIds, rightPosition, isActive, filter, False, idTenantAOO, roleUserType:=RoleUserType.RP)
            End If
            Return Facade.RoleFacade.GetUserRoles(Environment, rightPosition, isActive, filter, False, Nothing, idTenantAOO)
        Else
            If RightEnabled Then
                rightPosition = 1
            End If
            Return Facade.RoleFacade.GetRoles(Environment, rightPosition, isActive, filter, False, Nothing, idTenantAOO)
        End If
    End Function

    Private Sub Initialize()
    End Sub

    Private Sub InitializeButtons()
        tr_SearchByUser.Visible = SearchByUserEnabled
        If Not MultiSelect Then
            btnConferma.CssClass = "hiddenField"
            btnUtenti.CssClass = "hiddenField"
            RadTreeSettori.OnClientNodeClicked = "ReturnValueOnClick"
        Else
            WebUtils.ExpandOnClientNodeAttachEvent(RadTreeSettori)
            RadTreeSettori.CheckBoxes = True
            RadTreeSettori.MultipleSelect = True
            btnConferma.Attributes.Add("onclick", "ReturnValues();")
            btnUtenti.OnClientClick = "return OpenUsersWindow();"
        End If
        btnSelContactDomain.OnClientClick = String.Format(DefaultOpenWindowScript, ID, String.Concat("../UserControl/CommonSelContactDomain.aspx?Type=", Type, "&ParentID= ", ID), "windowSelContact", "_CloseDomain")
    End Sub

    ''' <summary>Parametri selezione settori</summary>
    Private Sub InitializeRequestFields()
        _selected = Request.QueryString("Selected")
        _tenantSelected = Request.QueryString("TenantSelected")
    End Sub

    Private Sub SetSelectableNode(ByRef node As RadTreeNode)
        If node IsNot Nothing Then
            node.Attributes.Add("Selectable", "TRUE")
            node.Checkable = MultiSelect
            node.ForeColor = Color.Empty
        End If
    End Sub

    Private Function CreateNode(ByVal role As Role) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = role.FullDescription
        node.Value = role.Id.ToString()
        node.Attributes.Add("ID", role.Id.ToString())
        node.Attributes.Add("TenantAOOId", role.IdTenantAOO.ToString())

        If (role.Father Is Nothing) Then
            node.ImageUrl = ImagePath.SmallRole
        Else
            node.ImageUrl = ImagePath.SmallSubRole
        End If
        node.Font.Bold = True

        node.Checkable = False

        If Facade.RoleFacade.GetRolesCount(Env, Nothing, True, "", False, role, role.IdTenantAOO) > 0 Then
            node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
        End If

        If Not role.IsActive OrElse Not role.IsActiveRange() Then
            node.CssClass = "notActive"
        End If

        If RoleRestriction = RoleRestrictions.OnlyMine AndAlso Not Facade.RoleFacade.CurrentUserBelongsToRoles(Env, role) Then
            node.ForeColor = Color.Gray
        End If

        Return node
    End Function

    Private Sub AddRecursiveNode(ByRef node As RadTreeNode, ByRef role As Role, ByVal expanded As Boolean)
        If RadTreeSettori.FindNodeByValue(role.Id.ToString()) IsNot Nothing Then
            Return
        End If

        Dim nodeToAdd As RadTreeNode
        If role Is Nothing Then
            Exit Sub
        End If

        nodeToAdd = CreateNode(role)

        If role.Father Is Nothing Then 'Primo Livello
            RadTreeSettori.Nodes(0).Nodes.Add(nodeToAdd)
            RadTreeSettori.Nodes(0).ExpandMode = TreeNodeExpandMode.ClientSide
        Else
            Dim parentNode As RadTreeNode = RadTreeSettori.FindNodeByValue(role.Father.Id.ToString())
            If parentNode Is Nothing Then
                AddRecursiveNode(nodeToAdd, role.Father, Not role.Father.Collapsed)
            Else
                parentNode.ExpandMode = TreeNodeExpandMode.ClientSide
                parentNode.Nodes.Add(nodeToAdd)
            End If
        End If
        nodeToAdd.Expanded = expanded

        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If
    End Sub

    Private Sub LoadRoles(roleList As IList(Of Role))
        For Each role As Role In roleList.Where(Function(x) ((ShowActive AndAlso x.IsActive) OrElse Not ShowActive))
            AddRecursiveNode(Nothing, role, Not role.Collapsed)

            If role.Children.Count > 0 Then
                LoadRoles(role.Children)
            End If

            Dim selNode As RadTreeNode = RadTreeSettori.FindNodeByValue(role.Id.ToString())
            SetSelectableNode(selNode)
        Next

        TreeViewUtils.SortNodes(RadTreeSettori.Nodes)

        CheckSelectedNodes()

    End Sub

    Private Sub CheckSelectedNodes()
        Dim selectedRoles As String() = Split(Selected, "|")
        For Each roleId As String In selectedRoles
            Dim refNode As RadTreeNode = RadTreeSettori.FindNodeByValue(roleId)
            If refNode IsNot Nothing Then
                refNode.Checkable = False
                refNode.Style.Add("color", "#00008B")
            End If
        Next
    End Sub

    Private Sub LoadChildrenRoles(parentNode As RadTreeNode, children As IList(Of Role))
        For Each role As Role In children

            Dim node As New RadTreeNode With {
                .Text = role.FullDescription,
                .Value = role.Id.ToString()
            }
            node.Attributes.Add("ID", role.Id.ToString())
            node.Font.Bold = True
            node.Checkable = MultiSelect
            node.Attributes.Add("Selectable", "TRUE")
            node.ForeColor = Color.Empty

            If (role.Father Is Nothing) Then
                node.ImageUrl = ImagePath.SmallRole
            Else
                node.ImageUrl = ImagePath.SmallSubRole
            End If

            If Facade.RoleFacade.GetRolesCount(Env, 1, True, "", False, role, role.IdTenantAOO) > 0 Then
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            End If

            If RoleRestriction = RoleRestrictions.OnlyMine Then
                If Not Facade.RoleFacade.CurrentUserBelongsToRoles(Env, role) Then
                    node.ForeColor = Color.Gray
                End If
            End If

            If Not String.IsNullOrWhiteSpace(Selected) Then
                If Selected.Contains(role.Id.ToString()) Then
                    node.Checkable = False
                    node.Style.Add("color", "#00008B")
                End If
            End If

            parentNode.Nodes.Add(node)
        Next

        TreeViewUtils.SortNodes(parentNode.Nodes)

        CheckSelectedNodes()
    End Sub

#End Region

End Class