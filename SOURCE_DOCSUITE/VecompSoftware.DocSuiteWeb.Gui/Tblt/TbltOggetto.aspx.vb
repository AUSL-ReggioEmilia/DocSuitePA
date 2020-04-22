Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class TbltOggetto
    Inherits CommonBasePage

#Region " Fields "

    Private _facade As FacadeFactory

#End Region

#Region " Properties "

    ''' <summary> Facade relativa al DB selezionato. </summary>
    ''' <remarks> Per avere il comportamento predefinito istanziare le eventuali facade in pagina. </remarks>
    Public Overrides ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory(rblDocType.SelectedValue)
            End If
            Return _facade
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        If Not Page.IsPostBack Then
            InitializeControls()

            If CommonInstance.DocmEnabled Then
                rblDocType.Items.Add(New ListItem(DocSuiteContext.Current.DossierAndPraticheLabel, DocumentFacade.DocmDB))
            End If
            If CommonInstance.ProtEnabled Then
                rblDocType.Items.Add(New ListItem("Protocollo", ProtocolFacade.ProtDB))
            End If
            If CommonInstance.ReslEnabled Then
                rblDocType.Items.Add(New ListItem(New TabMasterFacade(ResolutionFacade.ReslDB).TreeViewCaption, ResolutionFacade.ReslDB))
            End If

            InitializeButtons()
        End If
    End Sub

    Protected Sub TbltOggettoAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Update"
                LoadObjects()
        End Select
    End Sub

    Protected Sub RblDocTypeSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rblDocType.SelectedIndexChanged
        LoadObjects()
        RadTreeViewObjects.Nodes(0).Selected = True
        InitializeButtons()
    End Sub

    Protected Sub RadTreeViewObjectsNodeClick(ByVal sender As System.Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeViewObjects.NodeClick
        InitializeButtons()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeControls()
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeViewObjects)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewObjects)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocType, RadTreeViewObjects, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocType, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewObjects, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons)
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltOggettoAjaxRequest
    End Sub

    Private Sub InitializeButtons()
        btnAggiungi.Enabled = True
        btnElimina.Enabled = True
        btnRinomina.Enabled = True

        If rblDocType.SelectedIndex < 0 OrElse (RadTreeViewObjects.SelectedNode Is Nothing) Then
            btnAggiungi.Enabled = False
            btnElimina.Enabled = False
            btnRinomina.Enabled = False
            RadTreeViewObjects.ContextMenus(0).Enabled = False
            Exit Sub
        End If

        If RadTreeViewObjects.SelectedNode.Equals(RadTreeViewObjects.Nodes(0)) Then
            btnAggiungi.Enabled = True
            btnElimina.Enabled = False
            btnRinomina.Enabled = False
        Else
            btnAggiungi.Enabled = False
            btnElimina.Enabled = True
            btnRinomina.Enabled = True
        End If
        RadTreeViewObjects.ContextMenus(0).Enabled = True
    End Sub

    Private Sub LoadObjects()
        Dim modify As Boolean

        Select Case rblDocType.SelectedValue
            Case DocumentFacade.DocmDB
                modify = CommonShared.UserDocumentCheckRight(DocumentContainerRightPositions.Insert)
            Case ProtocolFacade.ProtDB
                modify = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)
            Case ResolutionFacade.ReslDB
                modify = CommonShared.UserResolutionCheckRight(ResolutionRightPositions.Insert)
        End Select

        Dim selnode As RadTreeNode = RadTreeViewObjects.SelectedNode
        RadTreeViewObjects.Nodes(0).Nodes.Clear()
        Dim objects As IList(Of CommonObject) = Facade.CommonObjectFacade.GetAll(rblDocType.SelectedValue)
        For Each obj As CommonObject In objects
            Dim node As New RadTreeNode()
            node.Value = obj.Id.ToString()
            node.Text = String.Format("({0}) {1}", obj.Code, obj.Description)
            node.ImageUrl = "../Comm/images/Oggetto.gif"
            node.Expanded = True
            RadTreeViewObjects.Nodes(0).Nodes.Add(node)
        Next
        If selnode IsNot Nothing Then
            selnode = RadTreeViewObjects.FindNodeByValue(selnode.Value)
            If selnode IsNot Nothing Then
                selnode.Selected = True
            Else
                RadTreeViewObjects.Nodes(0).Selected = True
            End If
        End If
        'Abilitazione
        If modify Then
            EnableModifyMode()
        Else
            DisableModifyMode()
        End If
    End Sub

    Private Sub EnableModifyMode()
        pnlButtons.Visible = True
        RadTreeViewObjects.ContextMenus(0).Visible = True
        RadTreeViewObjects.OnClientContextMenuItemClicked = "ContextMenuItemClicked"
        RadTreeViewObjects.OnClientContextMenuShowing = "ContextMenuShowing"
        InitializeButtons()
    End Sub

    Private Sub DisableModifyMode()
        pnlButtons.Visible = False
        RadTreeViewObjects.ContextMenus(0).Visible = False
        RadTreeViewObjects.OnClientContextMenuItemClicked = ""
        RadTreeViewObjects.OnClientContextMenuShowing = ""
    End Sub

#End Region

End Class







