Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.TreeHelper

Partial Public Class uscContainerExtGes
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _roleFacade As RoleFacade
    Private _managerID As String = String.Empty

#End Region

#Region " Properties "

    Private ReadOnly Property RoleFacade() As RoleFacade
        Get
            If _roleFacade Is Nothing Then
                _roleFacade = New RoleFacade("DocmDB")
            End If
            Return _roleFacade
        End Get
    End Property

    Public Property IDContainer() As Integer
        Get
            If ViewState.Item("_idContainer") Is Nothing Then
                Return 0
            End If
            Return DirectCast(ViewState.Item("_idContainer"), Integer)
        End Get
        Set(ByVal value As Integer)
            ViewState.Item("_idContainer") = value
        End Set
    End Property

    ''' <summary> ClientId del controllo WindoManager contenuto nella pagina che aprirà le finestre. </summary>
    Public Property WindowManagerID() As String
        Get
            Return _managerID
        End Get
        Set(ByVal value As String)
            _managerID = value
        End Set
    End Property

    ''' <summary> KeyType corrente. </summary>
    Public Property KeyType() As ContainerExtensionType
        Get
            If ViewState.Item("KeyType") Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState.Item("KeyType"), ContainerExtensionType)
        End Get
        Set(ByVal value As ContainerExtensionType)
            ViewState.Item("KeyType") = value
        End Set
    End Property

    Public Property StandardRoleRenderMode() As RenderMode
        Get
            Return uscStandardRole.ToolBarRenderMode
        End Get
        Set(ByVal value As RenderMode)
            uscStandardRole.ToolBarRenderMode = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not DocSuiteContext.Current.IsDocumentEnabled Then
            Exit Sub
        End If

        If Not IsPostBack AndAlso Not Page.IsCallback Then
            Initialize()
        End If
        InitializeAjax()
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdDelete.Click
        Dim tn As RadTreeNode = rtvContainerExt.SelectedNode
        Dim s As Short

        If (tn Is Nothing And Short.TryParse(tn.Value, s)) Then
            Exit Sub
        End If

        If tn.ParentNode IsNot Nothing Then
            tn.ParentNode.Selected = True
            SetSelectedNodeButtons()
        End If

        DeleteNodeAndChildNodes(tn)
    End Sub

    Private Sub btnMoveUp_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnMoveUp.Click
        Dim tnPadre As RadTreeNode
        Dim tnSopra As RadTreeNode = Nothing
        Dim tn As RadTreeNode = rtvContainerExt.SelectedNode

        If (tn IsNot Nothing) AndAlso (tn.Level > 0) Then  ' non è root
            tnPadre = tn.ParentNode
            Dim index As Integer = tnPadre.Nodes.IndexOf(tn)
            If index > 0 Then
                tnSopra = tnPadre.Nodes(index - 1)
                tn.Remove()
                tnPadre.Nodes.Insert(index - 1, tn)
            End If
        End If

        tn.Selected = True
    End Sub

    Private Sub btnMoveDown_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnMoveDown.Click
        Dim tnPadre As RadTreeNode
        Dim tnSotto As RadTreeNode = Nothing
        Dim tn As RadTreeNode = rtvContainerExt.SelectedNode

        If (tn IsNot Nothing) AndAlso (tn.Level > 0) Then   ' non è root
            tnPadre = tn.ParentNode
            Dim index As Integer = tnPadre.Nodes.IndexOf(tn)
            If index < tnPadre.Nodes.Count - 1 Then
                tnSotto = tnPadre.Nodes(index + 1)
                tn.Remove()
                tnPadre.Nodes.Insert(index + 1, tn)
            End If
        End If

        tn.Selected = True
    End Sub

    Private Sub cmdConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdConfirm.Click
        ' Salva l'intero albero di ContainerExtension come modificato dall'utente
        Try
            Dim tree As Tree(Of ContainerExtension) = CreateFoldersTree(rtvContainerExt)
            If Not (Facade.ContainerExtensionFacade.CreateContainerExtensionTree(IDContainer, KeyType, tree)) Then
                Throw New DocSuiteException("Container Extension", "Impossibile creare struttura ContainerExtension.")
            End If
            If Not (Facade.ContainerExtensionFacade.SaveContainerExtensionDefaultRole(IDContainer, ContainerExtensionType.SD, uscStandardRole.GetRoles())) Then
                Throw New DocSuiteException("Container Extension", "Impossibile creare il settore di default.")
            End If

            BasePage.AjaxAlert("Modifica dei dati eseguita con successo")

        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore salvataggio ContainerExtension", ex)
            BasePage.AjaxAlert("Si è verificato un errore nel salvataggio dati, contattare l'assistenza.")
        End Try

        SetSelectedNodeButtons()
    End Sub

    Protected Sub Control_RequestHandler(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim tn As RadTreeNode = rtvContainerExt.SelectedNode
        Dim arr As String() = e.Argument.Split(New String() {"|"}, 3, StringSplitOptions.None)

        If (Not arr(0).Eq(Me.ClientID)) OrElse (tn Is Nothing) Then
            Exit Sub
        End If

        Dim action As String = arr(1)
        Dim containerExt As ContainerExtension = JsonConvert.DeserializeObject(Of ContainerExtension)(arr(2))

        Select Case action
            Case "Add"
                Dim newTn As RadTreeNode = New RadTreeNode()
                SetNode(newTn, containerExt)
                newTn.Selected = True

                tn.Nodes.Add(newTn)
                SetSelectedNodeButtons()
            Case "Rename"
                SetNode(tn, containerExt)
                SetSelectedNodeButtons()
        End Select
    End Sub

    Private Sub rtvContainerExt_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles rtvContainerExt.NodeClick
        e.Node.ExpandChildNodes()
        SetSelectedNodeButtons()
    End Sub

    Private Sub SetSelectedNodeButtons()
        Dim isRoot As Boolean = String.IsNullOrEmpty(rtvContainerExt.SelectedNode.Value)

        cmdRename.Enabled = Not isRoot
        cmdDelete.Enabled = Not isRoot
        btnMoveUp.Enabled = Not isRoot
        btnMoveDown.Enabled = Not isRoot
    End Sub

    Protected Sub rtvContainerExt_NodeDrop(ByVal sender As Object, ByVal e As RadTreeNodeDragDropEventArgs)
        Dim sourceNode As RadTreeNode = e.SourceDragNode
        Dim destNode As RadTreeNode = e.DestDragNode
        Dim dropPosition As RadTreeViewDropPosition = e.DropPosition

        If destNode.Attributes("NodeType") = "Root" OrElse sourceNode.Attributes("NodeType") = "Root" Then
            Exit Sub
        End If

        TreeViewUtils.PerformDragAndDrop(dropPosition, sourceNode, destNode)
    End Sub

#End Region

#Region " Methods "

    Public Sub Initialize()
        cmdAdd.OnClientClick = String.Format("return OpenEditWindowContainerExtGes( 'Add', '{0}', '{1}');", IDContainer, KeyType)
        cmdRename.OnClientClick = String.Format("return OpenEditWindowContainerExtGes( 'Rename', '{0}', '{1}');", IDContainer, KeyType)

        rtvContainerExt.Nodes(0).Nodes.Clear()
        Dim containerExtsList As IList(Of ContainerExtension) = Facade.ContainerExtensionFacade.GetByContainerAndKey(IDContainer, KeyType)
        For Each containerExtension As ContainerExtension In containerExtsList
            AddRecursiveNode(Nothing, containerExtension)
        Next
        rtvContainerExt.Nodes(0).Selected = True

        Dim contExtList As IList(Of ContainerExtension) = Facade.ContainerExtensionFacade.GetByContainerAndKey(IDContainer, ContainerExtensionType.SD)
        For Each containerExt As ContainerExtension In contExtList
            Dim role As Role = RoleFacade.GetById(Int32.Parse(containerExt.KeyValue))
            If role IsNot Nothing Then
                uscStandardRole.SourceRoles.Add(role)
                uscStandardRole.DataBind()
            End If
        Next
    End Sub

    Public Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf Control_RequestHandler
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvContainerExt)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscStandardRole)
        
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMoveUp, rtvContainerExt)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMoveUp, btnMoveUp)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMoveDown, rtvContainerExt)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMoveDown, btnMoveDown)

        AjaxManager.AjaxSettings.AddAjaxSetting(rtvContainerExt, rtvContainerExt)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvContainerExt, panelButtons, BasePage.MasterDocSuite.AjaxFlatLoadingPanel)

        'AjaxManager.AjaxSettings.AddAjaxSetting(cmdAdd, cmdAdd)
        'AjaxManager.AjaxSettings.AddAjaxSetting(cmdRename, cmdRename)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDelete, rtvContainerExt)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdConfirm, cmdConfirm)
    End Sub

    Private Sub AddRecursiveNode(ByRef node As RadTreeNode, ByRef containerExtension As ContainerExtension)
        If containerExtension Is Nothing Then
            Exit Sub
        End If

        If rtvContainerExt.FindNodeByValue(containerExtension.Incremental.ToString()) IsNot Nothing Then
            Exit Sub
        End If

        Dim nodeToAdd As New RadTreeNode()
        SetNode(nodeToAdd, containerExtension)
        If containerExtension.IncrementalFather < 1 Then 'Primo Livello
            rtvContainerExt.Nodes(0).Nodes.Add(nodeToAdd)
        Else
            Dim newNode As RadTreeNode = rtvContainerExt.FindNodeByValue(containerExtension.IncrementalFather.ToString())
            If (newNode Is Nothing) Then
                Dim ceck As New ContainerExtensionCompositeKey()
                ceck.idContainer = IDContainer
                ceck.KeyType = KeyType
                ceck.Incremental = containerExtension.IncrementalFather
                AddRecursiveNode(nodeToAdd, Facade.ContainerExtensionFacade.GetById(ceck))
            Else
                newNode.Nodes.Add(nodeToAdd)
            End If
        End If
        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If
    End Sub

    Private Sub SetNode(ByRef node As RadTreeNode, ByVal containerExtension As ContainerExtension)
        Dim arr As String() = containerExtension.KeyValue.Split("|"c)
        If arr.Length > 1 Then
            node.Text = String.Format("({0}) {1}", arr(1), arr(0))
        Else
            node.Text = arr(0)
        End If
        node.Value = containerExtension.Incremental.ToString()
        node.ImageUrl = Page.ResolveClientUrl("~/Comm/images/folderclose16.gif")
        node.Attributes.Add("KeyValue", containerExtension.KeyValue & If(containerExtension.KeyValue.IndexOf("|") < 0, "|0", ""))
        node.Expanded = True
    End Sub

    Private Sub DeleteNodeAndChildNodes(ByRef tn As RadTreeNode)
        While tn.Nodes.Count > 0
            Dim n As RadTreeNode = tn.Nodes(0)
            DeleteNodeAndChildNodes(n)
        End While

        tn.Remove()
    End Sub

    ''' <summary> Creo l'albero delle cartelle da salvare </summary>
    Private Function CreateFoldersTree(ByVal treeView As RadTreeView) As Tree(Of ContainerExtension)
        Dim root As TreeNode(Of ContainerExtension)
        Dim tree As New Tree(Of ContainerExtension)

        root = tree.MakeRoot(New ContainerExtension())
        For Each node As RadTreeNode In treeView.Nodes(0).Nodes
            Dim child As TreeNode(Of ContainerExtension) = root.AddChild(GetContainerExtension(node))
            CreateSubTree(child, node)
        Next

        Return tree
    End Function

    Private Sub CreateSubTree(ByRef treeNode As TreeNode(Of ContainerExtension), ByVal node As RadTreeNode)
        'punto uscita funz ricorsiva
        If node.Nodes.Count = 0 Then
            Exit Sub
        End If

        For Each child As RadTreeNode In node.Nodes
            Dim childNode As TreeNode(Of ContainerExtension) = treeNode.AddChild(GetContainerExtension(child))
            CreateSubTree(childNode, child)
        Next
    End Sub

    ''' <summary> Creo l'oggetto containerExtension che sarà da popolare. </summary>
    Private Function GetContainerExtension(ByVal node As RadTreeNode) As ContainerExtension
        Dim containerExtension As New ContainerExtension()
        containerExtension.Id.idContainer = IDContainer
        containerExtension.KeyType = If(KeyType = Nothing, Nothing, KeyType.ToString())
        containerExtension.KeyValue = node.Attributes("KeyValue")
        Return containerExtension
    End Function

#End Region

End Class