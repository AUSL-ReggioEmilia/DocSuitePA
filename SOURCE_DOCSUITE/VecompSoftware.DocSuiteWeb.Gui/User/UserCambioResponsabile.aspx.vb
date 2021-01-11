Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class UserCambioResponsabile
    Inherits UserBasePage

#Region " Fields "
    Private _collaborationType As String = String.Empty
#End Region

#Region " Properties "

    Private Property OldSelection As String
        Get
            Return CType(ViewState("OldSelection"), String)
        End Get
        Set(value As String)
            ViewState("OldSelection") = value
        End Set
    End Property

    Private Property OldDestination As String
        Get
            Return CType(ViewState("OldDestination"), String)
        End Get
        Set(value As String)
            ViewState("OldDestination") = value
        End Set
    End Property

    Private ReadOnly Property SelectedIdRole As Integer
        Get
            Return Integer.Parse(ddlRoles.SelectedValue)
        End Get
    End Property

    Public ReadOnly Property CurrentEnvironment As DSWEnvironment
        Get
            Dim _currentEnvironment As DSWEnvironment
            If Not String.IsNullOrEmpty(Request.QueryString("DSWEnvironment")) Then
                Dim tmpValue As String = Request.QueryString("DSWEnvironment")
                If [Enum].TryParse(tmpValue, _currentEnvironment) Then
                    Return _currentEnvironment
                End If
            End If
            Return DSWEnvironment.Any
        End Get
    End Property

    Private ReadOnly Property CollaborationType As String
        Get
            If String.IsNullOrEmpty(_collaborationType) Then
                _collaborationType = HttpContext.Current.Request.QueryString.GetValueOrDefault("CollaborationType", String.Empty)
            End If
            Return _collaborationType
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        InitializeAjax()
        If Not IsPostBack Then
            If Not BindRoles() Then
                AjaxManager.ResponseScripts.Add("return Close(null);")
                Exit Sub
            Else
                Initialize()
            End If
        End If
    End Sub

    Private Sub ddlRoles_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlRoles.SelectedIndexChanged
        Initialize()
    End Sub

    Private Sub rtvOrigin_NodeCheck(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles rtvOrigin.NodeCheck
        If OldSelection IsNot Nothing Then
            rtvOrigin.FindNodeByValue(OldSelection).Checked = False
        End If

        If e.Node.Checked Then
            OldSelection = e.Node.Value
        Else
            OldSelection = Nothing
        End If

        For Each tn As RadTreeNode In rtvDestination.Nodes(0).Nodes
            tn.Checkable = (e.Node.Value <> tn.Value) OrElse (Not e.Node.Checked AndAlso e.Node.Value.Eq(tn.Value))
            If Not tn.Checkable Then
                tn.Checked = False
            End If
        Next

        For Each tn As RadTreeNode In rtvDestination.Nodes(1).Nodes
            tn.Checkable = (e.Node.Value <> tn.Value) OrElse (Not e.Node.Checked AndAlso e.Node.Value.Eq(tn.Value))
            If Not tn.Checkable Then
                tn.Checked = False
            End If
        Next
    End Sub

    Private Sub rtvDestination_NodeCheck(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles rtvDestination.NodeCheck
        If OldDestination IsNot Nothing Then
            rtvDestination.FindNodeByValue(OldDestination).Checked = False
        End If

        If e.Node.Checked Then
            OldDestination = e.Node.Value
        Else
            OldDestination = Nothing
        End If
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        Dim origin As String = String.Empty
        GetCheckedValue(rtvOrigin, 0, origin) ' dirigente
        GetCheckedValue(rtvOrigin, 1, origin) ' vice
        If String.IsNullOrEmpty(origin) Then
            AjaxAlert("Nessun Responsabile Origine selezionato.")
            Exit Sub
        End If
        origin = origin.Replace("'", "~")

        Dim destination As String = String.Empty
        GetCheckedValue(rtvDestination, 0, destination) ' dirigente
        GetCheckedValue(rtvDestination, 1, destination) ' vice
        If String.IsNullOrEmpty(destination) Then
            AjaxAlert("Nessun Responsabile Destinazione selezionato.")
            Exit Sub
        End If
        destination = destination.Replace("'", "~")

        Dim changeSigner As New ChangeSignerDTO(Integer.Parse(ddlRoles.SelectedValue), origin, destination)

        AjaxManager.ResponseScripts.Add(String.Format("return Close('{0}');", StringHelper.EncodeJS(JsonConvert.SerializeObject(changeSigner))))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvDestination, rtvOrigin)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvOrigin, rtvDestination)
    End Sub

    Private Function GetRoleUsers() As IList(Of RoleUser)
        Dim finder As NHRoleUserFinder = New NHRoleUserFinder()

        finder.RoleIdIn.Add(Me.SelectedIdRole)
        finder.TypeIn.Add(RoleUserType.D.ToString())
        finder.TypeIn.Add(RoleUserType.V.ToString())
        If ProtocolEnv.CollaborationRightsEnabled OrElse (Not ProtocolEnv.CollaborationRightsEnabled AndAlso (CurrentEnvironment.Equals(DSWEnvironment.Any) OrElse CurrentEnvironment.Equals(DSWEnvironment.Protocol))) Then
            finder.DSWEnvironmentIn.Add(CurrentEnvironment)
        End If
        Dim roleUsers As IList(Of RoleUser) = finder.List()

        Dim tmp As IList(Of RoleUser) = roleUsers.ToList()
        For Each roleUser As RoleUser In tmp
            If roleUsers.Where(Function(x) x.Account.Eq(roleUser.Account) AndAlso x.Type.Eq(roleUser.Type)).Count() > 1 Then
                roleUsers.Remove(roleUser)
            End If
        Next
        Return roleUsers
    End Function

    Private Function BindRoles() As Boolean

        Dim roles As IList(Of Role) = New List(Of Role)
        If Not ProtocolEnv.CollaborationRightsEnabled Then
            roles = Facade.RoleUserFacade.GetSecretaryRolesByAccount(DocSuiteContext.Current.User.FullUserName, Nothing)
        End If
        If (ProtocolEnv.CollaborationRightsEnabled AndAlso Not CurrentEnvironment.Equals(DSWEnvironment.Any)) Then
            roles = Facade.RoleUserFacade.GetSecretaryRolesByAccount(DocSuiteContext.Current.User.FullUserName, CurrentEnvironment)
        End If

        If roles.IsNullOrEmpty() Then
            Return False
        End If

        For Each role As Role In roles
            ddlRoles.Items.Add(New ListItem(role.Name, role.Id.ToString()))
        Next
        ddlRoles.Enabled = roles.Count > 1
        Return True
    End Function

    Private Sub Initialize()
        Dim roleUserList As IList(Of RoleUser) = GetRoleUsers()
        InitOriginNodes(roleUserList)
        InitDestinationNodes(roleUserList)
    End Sub

    Private Sub InitOriginNodes(roleUsers As IList(Of RoleUser))
        rtvOrigin.CheckBoxes = True
        rtvOrigin.Nodes.Clear()

        Dim tn As RadTreeNode = CreateRootNode("Dirigente", RoleUserType.D)
        rtvOrigin.Nodes.Add(tn)
        tn = CreateRootNode("Vice", RoleUserType.V)
        rtvOrigin.Nodes.Add(tn)

        For Each roleUser As RoleUser In roleUsers
            tn = CreateNode(roleUser)
            tn.Checkable = True
            tn.Checked = False
            Select Case roleUser.Type
                Case RoleUserType.D.ToString()
                    rtvOrigin.Nodes(0).Nodes.Add(tn)
                Case RoleUserType.V.ToString()
                    rtvOrigin.Nodes(1).Nodes.Add(tn)
            End Select
        Next
    End Sub

    Private Sub InitDestinationNodes(roleUsers As IList(Of RoleUser))
        rtvDestination.CheckBoxes = True
        rtvDestination.Nodes.Clear()

        Dim tn As RadTreeNode = CreateRootNode("Dirigente", RoleUserType.D)
        rtvDestination.Nodes.Add(tn)
        tn = CreateRootNode("Vice", RoleUserType.V)
        rtvDestination.Nodes.Add(tn)

        For Each roleUser As RoleUser In roleUsers
            tn = CreateNode(roleUser)
            tn.Checkable = True
            tn.Checked = False
            Select Case roleUser.Type
                Case RoleUserType.D.ToString()
                    rtvDestination.Nodes(0).Nodes.Add(tn)
                Case RoleUserType.V.ToString()
                    rtvDestination.Nodes(1).Nodes.Add(tn)
            End Select
        Next
    End Sub

    Private Function CreateNode(ByRef roleUser As RoleUser) As RadTreeNode
        Dim tn As New RadTreeNode
        tn.Text = Facade.CollaborationFacade.GetSignerDescription(roleUser.Description, roleUser.Account, CollaborationType)
        tn.Value = roleUser.Account
        tn.Attributes.Add("account", roleUser.Account)
        tn.Expanded = True
        tn.Checkable = False
        Return tn
    End Function

    Private Function CreateRootNode(ByVal txt As String, ByVal value As RoleUserType) As RadTreeNode
        Dim tn As RadTreeNode = New RadTreeNode
        tn.Text = txt
        tn.Value = value.ToString()
        tn.Font.Bold = True
        tn.Expanded = True
        tn.Checkable = False
        tn.ImageUrl = "../Comm/images/Interop/Ruolo.gif"
        Return tn
    End Function

    Private Sub GetCheckedValue(ByRef radTreeView As RadTreeView, ByVal level As Integer, ByRef value As String)
        For Each tn As RadTreeNode In radTreeView.Nodes(level).Nodes
            If tn.Checked Then
                value = tn.Attributes("account")
                Exit For
            End If
        Next
    End Sub

#End Region

End Class