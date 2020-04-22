Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging

Partial Public Class DocmMailCC
    Inherits DocmBasePage

#Region "Fields"
    Private _multiSelect As String = String.Empty
    Private _selected As String = String.Empty

    Private _fileDescription As String = String.Empty
    Private _folder As String = String.Empty
#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        GetQueryString()
        InitializeRequestFields()

        InitializeAjax()
        If Not Page.IsPostBack Then
            LoadRoles()
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializeRequestFields()
        'Parametri selezione settori
        _multiSelect = Server.UrlDecode(Request.QueryString("MultiSelect"))
        _selected = Request.QueryString("Selected")
    End Sub

    Private Sub InitializeAjax()
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, RadTreeSettori)
    End Sub
#End Region

#Region "Private Methods"
    Private Sub GetQueryString()
        _folder = Server.UrlDecode(Request.QueryString("Folder"))
        _fileDescription = Server.UrlDecode(Request.QueryString("File"))

    End Sub

    Private Sub SetNode(ByRef node As RadTreeNode, ByVal role As Role, Optional ByVal expandCallBack As Boolean = False)
        node.Text = role.Name
        If Not String.IsNullOrEmpty(role.EMailAddress) Then
            node.Text &= " (" & role.EMailAddress & ")"
            node.Checked = True
        Else
            node.Enabled = False
        End If
        node.Value = role.Id
        node.Attributes.Add("ID", role.Id.ToString())
        If (role.Father Is Nothing) Then
            If (expandCallBack) Then
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            End If
            node.ImageUrl = ImagePath.SmallRole
        Else
            node.ImageUrl = ImagePath.SmallSubRole
        End If
        node.Font.Bold = True
        node.Expanded = True
    End Sub

    Private Sub AddRecursiveNode(ByRef node As RadTreeNode, ByRef role As Role, Optional ByVal setNodeBold As Boolean = True)
        Dim nodeToAdd As New RadTreeNode()

        If (RadTreeSettori.FindNodeByValue(role.Id) Is Nothing) Then
            If Not (role Is Nothing) Then
                SetNode(nodeToAdd, role, False)
                If role.Father Is Nothing Then 'Primo Livello
                    ''If LocationEnabled Then                         'locazione abilitata?
                    ''    If (role.DocmLocation.Name = Location) Or (role.ProtLocation.Name = Location) Or (role.ReslLocation.Name = Location) Then
                    ''        Exit Sub
                    ''    End If
                    ''End If
                    RadTreeSettori.Nodes(0).Nodes.Add(nodeToAdd)
                Else
                    Dim newNode As RadTreeNode = RadTreeSettori.FindNodeByValue(role.Father.Id)
                    If (newNode Is Nothing) Then
                        AddRecursiveNode(nodeToAdd, role.Father, setNodeBold)
                    Else
                        newNode.Nodes.Add(nodeToAdd)
                    End If
                End If
            End If
            If Not (node Is Nothing) Then
                nodeToAdd.Nodes.Add(node)
            End If
        End If
    End Sub

    Private Sub LoadRoles()
        Dim refNode As RadTreeNode = Nothing
        Dim selectedRoles As String() = Nothing

        RadTreeSettori.Nodes(0).Checkable = MultiSelect

        Dim cc As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True, , 1)

        RadTreeSettori.Nodes(0).Nodes.Clear()
        For Each docToken As DocumentToken In cc
            AddRecursiveNode(Nothing, docToken.RoleDestination)
        Next

        selectedRoles = Split(_selected, "|")
        For Each roleId As String In selectedRoles
            refNode = RadTreeSettori.FindNodeByValue(roleId)
            If Not (refNode Is Nothing) Then
                refNode.Checkable = MultiSelect
                refNode.Style.Add("color", "#00008B")
            End If
        Next
    End Sub

    Private Sub GetRoleRecoursively(ByVal root As RadTreeNode, ByVal ids As IList(Of Integer))
        For Each node As RadTreeNode In root.Nodes
            If node.Checked Then
                ids.Add(node.Value)
            End If

            If node.Nodes IsNot Nothing AndAlso node.Nodes.Count > 0 Then
                GetRoleRecoursively(node, ids)
            End If
        Next
    End Sub
#End Region

#Region "Public Properties"
    Public Property MultiSelect() As Boolean
        Get
            Return _multiSelect.Eq("TRUE")
        End Get
        Set(ByVal value As Boolean)
            _multiSelect = If(value = True, "TRUE", "FALSE")
        End Set
    End Property

    Public Property Selected() As String
        Get
            Return _selected
        End Get
        Set(ByVal value As String)
            _selected = value
        End Set
    End Property
#End Region

#Region "Events"
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAnnulla.Click
        MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow('')")
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim ids As New List(Of Integer)

        'Recupero ricorsivamente gli indirizzi email selezionati
        GetRoleRecoursively(RadTreeSettori.Nodes(0), ids)

        Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(ids)

        If (roles Is Nothing) OrElse roles.Count <= 0 Then
            AjaxAlert("Nessun indirizzo E-Mail selezionato")
            Exit Sub
        End If

        Try
            DocUtil.SendMailCC(roles, CurrentDocumentYear, CurrentDocumentNumber, _folder, _fileDescription)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxManager.Alert(ex.Message)
        End Try

        AjaxManager.ResponseScripts.Add("CloseWindow('')")
    End Sub
#End Region
End Class