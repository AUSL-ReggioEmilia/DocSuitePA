Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI

Partial Public Class uscSelTemplate
    Inherits WindowControl

#Region "Event Handler"
    Public Delegate Sub ProtocolAddedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ProtocolAdded As ProtocolAddedEventHandler
#End Region

#Region "Fields"
    Private _readonly As Boolean
    Private _multiple As Boolean
#End Region

#Region "Properties"
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
                rfvTemplate.Enabled = False
            End If
        End Set
    End Property

    Public Property IsRequired() As Boolean
        Get
            Return rfvTemplate.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvTemplate.Enabled = value
        End Set
    End Property

    Public Property RequiredErrorMessage() As String
        Get
            Return rfvTemplate.ErrorMessage
        End Get
        Set(ByVal value As String)
            rfvTemplate.ErrorMessage = value
        End Set
    End Property

    ''' <summary>
    ''' Imposta l'etichetta del nodo root della treeview del controllo
    ''' </summary>
    ''' <value>Etichetta nodo root</value>
    ''' <returns>Etichetta nodo root</returns>
    ''' <remarks></remarks>
    Public Property TreeViewCaption() As String
        Get
            Return RadTreeTemplate.Nodes(0).Text
        End Get
        Set(ByVal value As String)
            RadTreeTemplate.Nodes(0).Text = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde il pulsante di selezione template
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ButtonSelectVisible() As Boolean
        Get
            Return btnAddTemplate.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAddTemplate.Visible = value
        End Set
    End Property
#End Region

#Region "Load Page"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeHideControls()
        InitializeButtons()
        InitializeAjaxSettings()
        InitializeControls()
        RadTreeTemplate.Nodes(0).Checkable = False
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializeControls()
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeTemplate)
        If [ReadOnly] Then
            Multiple = True
            IsRequired = False
        End If
    End Sub

    Private Sub InitializeHideControls()
        'Nascondi i controlli per l'inserimento dei contatti nella treeview
        WebUtils.ObjAttDisplayNone(txtTemplate)
        WebUtils.ObjAttDisplayNone(btnAddTemplate)
    End Sub

    Private Sub InitializeButtons()
        Me.RegisterWindowManager(RadWindowManagerProtocol)
        WindowBuilder.RegisterOpenerElement(imgSelTemplate)
    End Sub

    Private Sub InitializeAjaxSettings()
        If Not [ReadOnly] Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnAddTemplate, RadTreeTemplate)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeTemplate)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtTemplate)
        End If
    End Sub
#End Region

#Region "Button Events"
    Private Sub btnAddTemplate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddTemplate.Click
        If txtTemplate.Text <> String.Empty Then
            AddTemplateNode(txtTemplate.Text, txtTemplate.Text)
        End If
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Testa se è attivo il flag per inserimento protocolli multipli, in caso contrario pulisce la treeview
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub MultipleTemplateTest()
        If Not Multiple Then
            RadTreeTemplate.Nodes(0).Nodes.Clear()
        End If
    End Sub

    ''' <summary>
    ''' Controlla se esistono nodi figli della root, in caso contrario registra lo script per far scattare
    ''' il validatore del controllo
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Validate()
        If RadTreeTemplate.Nodes(0).Nodes.Count = 0 Then
            AjaxManager.ResponseScripts.Add(Me.ID & "_ClearTextValidator()")
        End If
    End Sub

    Private Sub AddTemplateNode(ByVal template As String, ByVal description As String)
        If Not String.IsNullOrEmpty(template) Then
            MultipleTemplateTest()
            Dim node As RadTreeNode = AddNode(Nothing, template, description)
            node.Checkable = True
            node.Font.Bold = True
            node.Attributes.Add("Selected", "1")

            RaiseEvent ProtocolAdded(Me, New EventArgs())
        End If
    End Sub

    Private Function AddNode(ByRef node As RadTreeNode, ByVal template As String, ByVal description As String) As RadTreeNode
        Dim nodeToAdd As New RadTreeNode()
        If (RadTreeTemplate.FindNodeByValue(template) Is Nothing) Then
            SetNodo(nodeToAdd, template, description)
            If Not String.IsNullOrEmpty(template) Then
                RadTreeTemplate.Nodes(0).Nodes.Add(nodeToAdd)
            End If
            If node IsNot Nothing Then
                nodeToAdd.Nodes.Add(node)
            End If
            nodeToAdd.Checkable = False
        End If
        Return nodeToAdd
    End Function

    Private Sub SetNodo(ByRef node As RadTreeNode, ByVal template As String, ByVal description As String)
        node.Text = description
        node.Value = template
        node.ImageUrl = ImagePath.FromFile(description)
        node.Expanded = True
    End Sub
#End Region

#Region "Public Methods"
    Public Function GetTemplates() As IList(Of String)
        Dim templates As New List(Of String)

        For Each node As RadTreeNode In RadTreeTemplate.Nodes(0).Nodes
            templates.Add(node.Value)
        Next

        Return templates
    End Function

    Public Sub AddTemplate(ByVal template As String)
        If Not String.IsNullOrEmpty(template) Then
            AddTemplateNode(template, template)
        End If
    End Sub

    Public Function GetFirstTemplate() As String
        Dim templates As IList(Of String) = GetTemplates()
        If templates.Count > 0 Then
            Return templates(0)
        End If
        Return Nothing
    End Function

    Public Sub ClearTemplatess()
        RadTreeTemplate.Nodes(0).Nodes.Clear()
    End Sub
#End Region

#Region "Windows Opener Events"
    'apertura finestra selezione classificatore
    Private Sub imgSelTemplate_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles imgSelTemplate.Click
        Dim sPath As String = Replace(CommonUtil.GetInstance().AppPath & ResolutionFacade.PathStampeTo, "\", "\\") & "Regione\\"
        Dim s As String = "Type=Resl&NomeCampo=Documento&NomeCampoDes=DocumentoDes&AddButton=AddDocumentFile&Path=" & sPath & "&Ext=htm"
        Dim URL As String = "../UserControl/CommonSelTemplate.aspx?" & CommonShared.AppendSecurityCheck("Titolo=Seleziona Template&" & s)
        WindowBuilder.LoadWindow("windowSelTemplate", URL, Me.ID & "_OnClose", Unit.Pixel(600), Unit.Pixel(450))
    End Sub
#End Region

End Class