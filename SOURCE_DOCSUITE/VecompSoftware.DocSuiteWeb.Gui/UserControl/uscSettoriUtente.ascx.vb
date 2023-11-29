Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json

Partial Public Class uscSettoriUtente
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Public Const ATTR_USERNODE As String = "USERNODE"
    Public Const TRUE_VALUE As String = "TRUE"
    Public Const FALSE_VALUE As String = "FALSE"

    Private _windowHeight As Short = 600
    Private _windowWidth As Short = 500
    Private _role As Role = Nothing

#End Region

#Region " Properties "

    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property

    Public Property [ReadOnly]() As Boolean
        Get
            Return (Not tblCellButtons.Visible)
        End Get
        Set(ByVal value As Boolean)
            tblCellButtons.Visible = (Not value)
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

    Public ReadOnly Property TreeViewControl() As RadTreeView
        Get
            Return RadTreeUsers
        End Get
    End Property

    Public Property WindowHeight() As Short
        Get
            Return _windowHeight
        End Get
        Set(ByVal value As Short)
            _windowHeight = value
        End Set
    End Property

    Public Property WindowWidth() As Short
        Get
            Return _windowWidth
        End Get
        Set(ByVal value As Short)
            _windowWidth = value
        End Set
    End Property

    Public Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        InitializeButtons()
    End Sub

    Protected Sub uscSettoriUtente_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 2)
        If (arguments(0) = Me.ClientID) Then

            Dim userNodeList As List(Of UserNode) = JsonConvert.DeserializeObject(Of List(Of UserNode))(arguments(1))
            DeleteUsersNode(RadTreeUsers.SelectedNode)
            AddUsersNode(RadTreeUsers.SelectedNode, userNodeList)
        End If
    End Sub

    Private Sub RadTreeUsers_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeUsers.NodeClick
        If e.Node.Value = "Root" OrElse e.Node.Attributes(ATTR_USERNODE) = TRUE_VALUE Then
            e.Node.Selected = False
            Exit Sub
        End If
        Role = Facade.RoleFacade.GetById(Integer.Parse(e.Node.Value))
        DeleteUsersNode(RadTreeUsers.SelectedNode)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        If Not [ReadOnly] Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSelUsers, RadTreeUsers)
            AddHandler AjaxManager.AjaxRequest, AddressOf uscSettoriUtente_AjaxRequest
        End If
    End Sub

    Private Sub InitializeButtons()
        'apertura finestra selezione utenti
        btnSelUsers.OnClientClick = "return " & Me.ID & "_OpenWindow('windowSelUsers','" & WindowHeight & "','" & WindowWidth & "','" & GetWindowParameters() & "');"
        RadWindowManagerUsers.Windows(0).OnClientClose = Me.ID & "_CloseFunction"
    End Sub

    Private Sub DeleteUsersNode(ByRef node As RadTreeNode)
        For Each children As RadTreeNode In node.Nodes
            If children.Attributes(ATTR_USERNODE) = TRUE_VALUE Then
                children.Remove()
            End If
        Next
    End Sub

    Private Sub AddUsersNode(ByRef destinationNode As RadTreeNode, ByRef userNodeList As List(Of UserNode))
        Dim userNodeObj As UserNode
        Dim i As Integer = 0
        For i = 0 To userNodeList.Count - 1
            Dim node As New RadTreeNode
            userNodeObj = userNodeList(i)
            node.Text = userNodeObj.Text
            node.Value = userNodeObj.Id
            node.ImageUrl = "../Comm/Images/User16.gif"
            node.Attributes.Add(ATTR_USERNODE, TRUE_VALUE)
            destinationNode.Nodes.Add(node)
        Next i
    End Sub

    Private Function GetWindowParameters() As String
        Dim returnValue As String = String.Empty
        Dim parameters As New Text.StringBuilder

        If Not String.IsNullOrEmpty(BasePage.Type) Then
            parameters.Append("&Type=" & BasePage.Type)
        End If
        If Role IsNot Nothing Then
            parameters.Append("&idRole=" & Role.Id)
        End If
        returnValue = parameters.ToString()
        Return returnValue.Remove(returnValue.IndexOf("&"), 1)
    End Function

    Public Function GetUserNodeFromNode(ByVal node As RadTreeNode) As UserNode
        Return JsonConvert.DeserializeObject(Of UserNode)(node.Value)
    End Function

    Public Function SetRole(ByVal parent As RadTreeNode, ByVal role As Role, Optional ByVal first As Boolean = True) As Boolean
        Dim children As IList(Of Role) = Nothing

        If role IsNot Nothing Then
            Dim newNode As New RadTreeNode
            newNode.Expanded = True
            newNode.Text = role.Name
            newNode.Value = role.Id.ToString()
            newNode.ImageUrl = If(role.Father Is Nothing, ImagePath.SmallRole, ImagePath.SmallSubRole)
            If first Then
                newNode.Font.Bold = True
            End If
            parent.Nodes.Add(newNode)
            parent = newNode
            children = Facade.RoleFacade.GetChildren(role, Nothing, CurrentTenant.TenantAOO.UniqueId)
            For Each child As Role In children
                SetRole(parent, child, False)
            Next
        End If
    End Function

#End Region

End Class