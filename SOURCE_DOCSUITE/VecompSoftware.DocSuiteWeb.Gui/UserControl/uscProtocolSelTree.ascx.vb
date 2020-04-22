Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class uscProtocolSelTree
    Inherits DocSuite2008BaseControl

    Public Delegate Sub ProtocolAddedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub ProtocolRemovedEventHandler(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ProtocolAdded As ProtocolAddedEventHandler
    Public Event ProtocolRemoved As ProtocolAddedEventHandler

#Region " Fields "

    Private _readonly As Boolean
    Private _multiple As Boolean

#End Region

#Region " Properties "

    Public Property Multiple() As Boolean
        Get
            Return _multiple
        End Get
        Set(ByVal value As Boolean)
            _multiple = value
        End Set
    End Property

    Public Property [ReadOnly]() As Boolean
        Get
            Return _readonly
        End Get
        Set(ByVal value As Boolean)
            _readonly = value
            panelButtons.Visible = (Not value)
            If (value = True) Then
                rfvProtocol.Enabled = False
            End If
        End Set
    End Property

    Public Property IsRequired() As Boolean
        Get
            Return rfvProtocol.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvProtocol.Enabled = value
        End Set
    End Property

    Public Property RequiredErrorMessage() As String
        Get
            Return rfvProtocol.ErrorMessage
        End Get
        Set(ByVal value As String)
            rfvProtocol.ErrorMessage = value
        End Set
    End Property

    ''' <summary> Imposta l'etichetta del nodo root della treeview del controllo </summary>
    Public Property TreeViewCaption() As String
        Get
            Return RadTreeProtocollo.Nodes(0).Text
        End Get
        Set(ByVal value As String)
            RadTreeProtocollo.Nodes(0).Text = value
        End Set
    End Property

    ''' <summary> Restituisce il controllo TreeView per la visualizzazione dei protocolli </summary>
    Public ReadOnly Property TreeViewControl() As RadTreeView
        Get
            Return RadTreeProtocollo
        End Get
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di selezione protocollo </summary>
    Public Property ButtonSelectVisible() As Boolean
        Get
            Return imgSelProtocollo.Visible
        End Get
        Set(ByVal value As Boolean)
            imgSelProtocollo.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di eliminazione protocollo </summary>
    Public Property ButtonDeleteVisible() As Boolean
        Get
            Return btnRemoveProtocollo.Visible
        End Get
        Set(ByVal value As Boolean)
            btnRemoveProtocollo.Visible = value
        End Set
    End Property

    Public Property SetVisible() As Boolean
        Get
            Return RadTreeProtocollo.Visible
        End Get
        Set(ByVal value As Boolean)
            RadTreeProtocollo.Visible = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeHideControls()
        InitializeAjaxSettings()
        InitializeControls()
        RadTreeProtocollo.Nodes(0).Checkable = False
    End Sub

    Private Sub BtnAddProtocolloClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddProtocollo.Click
        If Not String.IsNullOrEmpty(txtProtocollo.Text) Then
            Dim v As String() = txtProtocollo.Text.Split("|"c)
            AddProtocolNode(Facade.ProtocolFacade.GetById(Short.Parse(v(0)), Integer.Parse(v(1))))
        End If
    End Sub

    Private Sub BtnRemoveProtocolloClick(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnRemoveProtocollo.Click
        Dim tn As RadTreeNode = RadTreeProtocollo.SelectedNode
        If (tn IsNot Nothing) AndAlso (tn.ParentNode IsNot Nothing) Then
            tn.Remove()
            RaiseEvent ProtocolRemoved(Me, New EventArgs())
        End If
        ' Controlla se esistono nodi figli della roo, in caso contrario registra lo script per far scattare il validatore del controllo
        If RadTreeProtocollo.Nodes(0).Nodes.Count = 0 Then
            AjaxManager.ResponseScripts.Add(ID & "_ClearTextValidator()")
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeControls()
        imgSelProtocollo.OnClientClick = String.Format(
            "return {0}_OpenWindow('{1}', '{2}');",
            ID,
            "../Resl/ReslProtocollo.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Action=Insert&ManagerID=" & RadWindowManagerProtocol.ClientID),
            "windowUploadDocument")

        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeProtocollo)
        If [ReadOnly] Then
            Multiple = True
            IsRequired = False
        End If
    End Sub

    Private Sub InitializeHideControls()
        'Nascondi i controlli per l'inserimento dei contatti nella treeview
        WebUtils.ObjAttDisplayNone(txtProtocollo)
        WebUtils.ObjAttDisplayNone(btnAddProtocollo)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(imgSelProtocollo, RadWindowManagerProtocol)
        If Not [ReadOnly] AndAlso SetVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnRemoveProtocollo, RadTreeProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnAddProtocollo, RadTreeProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeProtocollo)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtProtocollo)
        End If
    End Sub

    Private Sub AddProtocolNode(ByVal prot As Protocol)
        If prot IsNot Nothing Then
            ' Testa se è attivo il flag per inserimento protocolli multipli, in caso contrario pulisce la treeview
            If Not Multiple Then
                RadTreeProtocollo.Nodes(0).Nodes.Clear()
            End If
            Dim node As RadTreeNode = AddNode(Nothing, prot)
            node.Checkable = True
            node.Font.Bold = True
            node.Attributes.Add("Selected", "1")

            RaiseEvent ProtocolAdded(Me, New EventArgs())
        End If
    End Sub

    Private Function AddNode(ByRef node As RadTreeNode, ByVal prot As Protocol) As RadTreeNode
        Dim nodeToAdd As New RadTreeNode()
        If (RadTreeProtocollo.FindNodeByValue(prot.Id.ToString()) Is Nothing) Then
            nodeToAdd.Text = prot.FullNumber
            nodeToAdd.Value = prot.Id.ToString()
            nodeToAdd.ImageUrl = "../Comm/Images/DocSuite/Protocollo16.gif"
            nodeToAdd.Expanded = True

            If prot IsNot Nothing Then
                RadTreeProtocollo.Nodes(0).Nodes.Add(nodeToAdd)
            End If
            If node IsNot Nothing Then
                nodeToAdd.Nodes.Add(node)
            End If
            nodeToAdd.Checkable = False
        End If
        Return nodeToAdd
    End Function

    Public Function GetProtocols() As IList(Of Protocol)
        Dim keys As New List(Of YearNumberCompositeKey)

        For Each node As RadTreeNode In RadTreeProtocollo.Nodes(0).Nodes
            Dim v As String() = node.Value.Split("/"c)
            Dim k As New YearNumberCompositeKey(Short.Parse(v(0)), Integer.Parse(v(1)))
            keys.Add(k)
        Next

        Return Facade.ProtocolFacade.GetProtocols(keys)
    End Function

    Public Sub AddProtocol(ByVal prot As Protocol)
        If prot IsNot Nothing Then
            If RadTreeProtocollo.Nodes(0).Nodes.FindNodeByValue(prot.Id.ToString()) Is Nothing Then
                AddProtocolNode(prot)
            End If
        End If
    End Sub

    Public Sub AddProtocolAsLink(ByVal link As String)
        If Not String.IsNullOrEmpty(link) Then
            Dim arrayLink As String() = link.Split("|"c)
            Dim prot As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(arrayLink(0)), Integer.Parse(arrayLink(1)))
            AddProtocolNode(prot)
        End If
    End Sub

    Public Function GetFirstProtocolLink() As String
        Dim protocols As IList(Of Protocol) = GetProtocols()
        If Not protocols.IsNullOrEmpty() Then
            Return ProtocolFacade.GetCalculatedLink(protocols(0))
        End If
        Return Nothing
    End Function

    Public Sub ClearProtocols()
        RadTreeProtocollo.Nodes(0).Nodes.Clear()
    End Sub

#End Region

End Class