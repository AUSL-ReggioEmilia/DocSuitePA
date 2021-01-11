Imports VecompSoftware.Helpers
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.Caching
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports IEnumerableEx = VecompSoftware.Helpers.ExtensionMethods.IEnumerableEx


Partial Public Class CommonSelContactOChart
    Inherits CommonBasePage

#Region " Properties "

    Public ReadOnly Property QueryStringParentID As String
        Get
            Return Request.QueryString("ParentID")
        End Get
    End Property

    Public ReadOnly Property QueryStringAPIDefaultProvider As Boolean?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean?)("APIDefaultProvider", Nothing)
        End Get
    End Property

    Private ReadOnly Property ProviderContext As Dictionary(Of String, IOChartItemDTO())
        Get
            Dim name As String = DropDownProviders.ClientID & "_ProviderContext"
            Dim objCache As ObjectCache = MemoryCache.Default
            If objCache(name) Is Nothing Then
                Dim value As New Dictionary(Of String, IOChartItemDTO())
                Dim policy As New CacheItemPolicy With {.AbsoluteExpiration = DateTime.Now.AddHours(DocSuiteContext.Current.ProtocolEnv.APICacheExpiration)}
                objCache.Set(name, value, policy)
            End If
            Return objCache(name)
        End Get
    End Property
    Private Property SelectedProvider As String
        Get
            Return CType(Session(DropDownProviders.ClientID), String)
        End Get
        Set(value As String)
            Session(DropDownProviders.ClientID) = value
        End Set
    End Property
    Private Property SelectedProviderCode As String
        Get
            Return CType(Session(DropDownProviders.ClientID + "CODE"), String)
        End Get
        Set(value As String)
            Session(DropDownProviders.ClientID + "CODE") = value
        End Set
    End Property
    Private ReadOnly Property SelectedOChart As IOChartItemDTO()
        Get
            Return ProviderContext(SelectedProvider)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            MasterDocSuite.TitleVisible = False
            BindDropDownProviders()
            BindTreeViewItems()
        End If
    End Sub

    Private Sub DropDownProviders_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownProviders.SelectedIndexChanged
        SetSelectedProvider()
        BindTreeViewItems()
    End Sub

    Private Sub ButtonReloadProviders_Click(sender As Object, e As EventArgs) Handles ButtonReloadProviders.Click
        ProviderContext.Clear()
        BindDropDownProviders()
        BindTreeViewItems()
    End Sub
    Private Sub ButtonFilter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonFilter.Click
        BindTreeViewItems()
    End Sub
    Private Sub ButtonConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonConfirm.Click
        If TreeViewItems.SelectedNode Is Nothing Then
            Return
        End If
        ReturnContact(TreeViewItems.SelectedNode)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(DropDownProviders, TreeViewItems, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonReloadProviders, DropDownProviders, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonReloadProviders, TreeViewItems, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonFilter, TreeViewItems, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub BindDropDownProviders()
        Dim availableProviders As IEnumerable(Of APIProvider) = FacadeFactory.Instance.APIProviderFacade().GetAll().Where(Function(p) p.IsEnabled)
        Dim providers As IEnumerable(Of APIProvider) = Nothing
        If QueryStringAPIDefaultProvider.HasThisValue(True) Then
            providers = availableProviders.First(Function(p) p.Address.Eq(DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider)).AsList()
            lblNoProviders.Text = "Nessun provider locale configurato."
            lblNoProviders.Visible = providers.IsNullOrEmpty()
        ElseIf QueryStringAPIDefaultProvider.HasThisValue(False) Then
            providers = availableProviders.Where(Function(p) Not p.Address.Eq(DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider))
            lblNoProviders.Text = "Nessun provider esterno configurato."
            lblNoProviders.Visible = providers.IsNullOrEmpty()
        Else
            providers = availableProviders
            lblNoProviders.Visible = providers.IsNullOrEmpty()
        End If
        providers = providers.OrderBy(Function(p) p.Title)

        DropDownProviders.Items.Clear()
        For Each provider As APIProvider In providers
            Dim item As New ListItem(provider.Title, provider.Address)
            item.Attributes.Add("CODE", provider.Code)
            DropDownProviders.Items.Add(item)
        Next

        If Not String.IsNullOrEmpty(SelectedProvider) Then
            DropDownProviders.SelectedValue = SelectedProvider
        End If
        SetSelectedProvider()

        DropDownProviders.Enabled = DropDownProviders.Items IsNot Nothing AndAlso DropDownProviders.Items.Count > 1
    End Sub
    Private Sub SetSelectedProvider()
        If DropDownProviders.SelectedItem Is Nothing Then
            Return
        End If

        SelectedProvider = DropDownProviders.SelectedValue
        SelectedProviderCode = DropDownProviders.SelectedItem.Attributes("CODE")
        If ProviderContext.ContainsKey(SelectedProvider) Then
            Return
        End If

        Try
            ProviderContext.Add(SelectedProvider, OChartConnector.For(SelectedProvider).GetEffective())
        Catch ex As TimeoutException
            ' FG20131220: TODO gestire meglio qualora il servizio remoto non risponda.
            ProviderContext.Add(SelectedProvider, Enumerable.Empty(Of OChartItemDTO))
        End Try
    End Sub

    Private Function GetNodeText(item As IOChartItemDTO) As String

        If String.IsNullOrEmpty(item.Mailboxes) Then
            Return String.Format("{0} [{1}]", item.Title, item.Code)
        End If
        Return String.Format("{0} [{1}] - {2}", item.Title, item.Code, item.Mailboxes)
    End Function
    Private Function GetNodeValue(item As IOChartItemDTO) As String
        Return item.Code
    End Function
    Private Function GetNode(item As IOChartItemDTO) As RadTreeNode
        Dim node As New RadTreeNode() With {
            .Text = GetNodeText(item), .Value = GetNodeValue(item),
            .Expanded = False,
            .ExpandMode = TreeNodeExpandMode.ClientSide
        }
        node.ImageUrl = CType(If(item.Parent Is Nothing, ImagePath.SmallNetworkShareStar, ImagePath.SmallNetworkShare), String)
        node.AddAttribute("Description", item.Description)
        node.AddAttribute("Title", item.Title)
        node.AddAttribute("Mailboxes", item.Mailboxes)
        node.AddAttribute("FullCode", item.FullCode)
        node.AddAttribute("Provider", DropDownProviders.SelectedItem.Text)
        Return node
    End Function

    Private Sub AddNodeTopDown(parentNodes As RadTreeNodeCollection, item As IOChartItemDTO)
        Dim current As RadTreeNode = GetNode(item)
        parentNodes.Add(current)
        If item.Items.IsNullOrEmpty() Then
            Return
        End If

        item.Items.ToList().ForEach(Sub(i) AddNodeTopDown(current.Nodes, i))
    End Sub

    Private Sub AddNodeBottomUp(parentNodes As RadTreeNodeCollection, item As IOChartItemDTO)
        Dim current As RadTreeNode = GetNode(item)
        current.Expanded = True
        If item.Parent Is Nothing Then
            parentNodes.Add(current)
            Return
        End If
        Dim parentValue As String = GetNodeValue(item.Parent)
        Dim parent As RadTreeNode = TreeViewItems.GetAllNodes().FirstOrDefault(Function(n) n.Value.Eq(parentValue))
        If parent Is Nothing Then
            AddNodeBottomUp(parentNodes, item.Parent)
        End If
        parent = TreeViewItems.GetAllNodes().First(Function(n) n.Value.Eq(parentValue))
        parent.Nodes.Add(current)
    End Sub
    Private Function FindItems(items As IEnumerable(Of IOChartItemDTO), filter As String) As IEnumerable(Of IOChartItemDTO)
        If items.IsNullOrEmpty() Then
            Return Nothing
        End If

        Dim result As IEnumerable(Of IOChartItemDTO) = items.Where(Function(i) String.Format("{0}|{1}|{2}", i.Title, i.Description, i.Mailboxes).ContainsIgnoreCase(filter))
        Dim children As IEnumerable(Of IOChartItemDTO) = items.Where(Function(i) Not i.Items.IsNullOrEmpty()).SelectMany(Function(i) i.Items)
        If children.IsNullOrEmpty() Then
            Return result
        End If

        Dim recursion As IEnumerable(Of IOChartItemDTO) = FindItems(children, filter)
        If recursion.IsNullOrEmpty() Then
            Return result
        End If
        If result.IsNullOrEmpty() Then
            Return recursion
        End If

        Dim merged As List(Of IOChartItemDTO) = result.ToList()
        merged.AddRange(recursion)
        Return merged
    End Function
    Private Sub BindTreeViewItems(filter As String)
        TreeViewItems.Nodes.Clear()
        If String.IsNullOrWhiteSpace(SelectedProvider) Then
            Return
        End If

        If String.IsNullOrWhiteSpace(filter) Then
            SelectedOChart.ToList().ForEach(Sub(i) AddNodeTopDown(TreeViewItems.Nodes, i))
            Return
        End If

        Dim found As IEnumerable(Of IOChartItemDTO) = FindItems(SelectedOChart, filter)
        If found.IsNullOrEmpty() Then
            Return
        End If
        found.ToList().ForEach(Sub(i) AddNodeBottomUp(TreeViewItems.Nodes, i))
    End Sub
    Private Sub BindTreeViewItems()
        Dim filter As String = TextFilter.Text.Trim()
        BindTreeViewItems(filter)
    End Sub

    Private Sub ReturnContact(node As RadTreeNode)
        If node Is Nothing Then
            Return
        End If

        Dim selected As RadTreeNode = TreeViewItems.SelectedNode
        If SelectedProvider.Eq(DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider) Then
            ' Provider locale, recupero il contatto da Organigramma
            Dim fullCode As String = selected.Attributes("FullCode")
            Dim item As OChartItem = CommonShared.EffectiveOChart.Items.FindByFullCode(fullCode)
            If item.HasContacts Then
                Dim contacts As List(Of Contact) = item.Contacts.Select(Function(c) c.Contact).ToList()
                ' FG20140620: La convenzione che il campo note di un contatto da rubrica fittizio contenga il FullCode dell'OChartItem è una convenzione da irrobustire...
                For Each cc As Contact In contacts
                    cc.Note = item.FullCode
                Next
                ResponseAsJson(contacts, True, "OChartItemContact")
                Return
            End If
        End If

        Dim contact As New Contact() With {
            .Description = String.Format("{0} [{1}]", selected.Attributes("Title"), selected.Attributes("Provider")),
            .EmailAddress = selected.Attributes("Mailboxes"),
            .ContactType = New ContactType(ContactType.Aoo)
        }
        ResponseAsJson(contact.AsList(), True, "OChartItemContactManual")
    End Sub

    Private Sub ResponseAsJson(contacts As List(Of Contact), close As Boolean, action As String)
        Dim serialized As String = StringHelper.EncodeJS(JsonConvert.SerializeObject(contacts))
        Dim js As String = String.Format("returnValuesJson('{0}', '{1}', '{2}');", action, serialized, close.ToJavaScriptString())
        AjaxManager.ResponseScripts.Add(js)
    End Sub

    Private Sub ResponseAsJson(idContact As Integer, close As Boolean, myAction As String)
        Dim js As String = String.Format("returnValuesJson('{0}', '{1}', '{2}');", myAction, idContact, close.ToJavaScriptString())
        AjaxManager.ResponseScripts.Add(js)
    End Sub

#End Region

End Class