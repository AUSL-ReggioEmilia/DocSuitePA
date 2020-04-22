Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers

Partial Class SelContattiIPA
    Inherits CommonBasePage

#Region " Properties "

    Public ReadOnly Property CallerId() As String
        Get
            Return Request.QueryString("ParentID")
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        MasterDocSuite.TitleVisible = False

        If String.IsNullOrEmpty(ProtocolEnv.LdapIndicePa) OrElse Not ProtocolEnv.LdapIndicePa.ContainsIgnoreCase(CommonAD.LDAPRoot) Then
            Throw New DocSuiteException(ContactTypeFacade.LegacyDescription(ContactType.Ipa), "Controllare parametri di configurazione per ricerca IPA")
        End If

        If Not IsPostBack Then
            Title = ContactTypeFacade.LegacyDescription(ContactType.Ipa)
            InitializeTreeView()
            txtSearch.Focus()
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        cmdDetail.Enabled = False
        If txtSearch.Text.Length < 2 Then
            AjaxAlert("Impossibile cercare.{0}Inserire almeno due caratteri.", Environment.NewLine)
            Exit Sub
        End If
        Dim selectedValues As List(Of String) = rblFilterTypeObject.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()


        Dim ipaEntities As List(Of IPA) = IPARetriever.GetIpaEntities(ProtocolEnv.LdapIndicePa, txtSearch.Text, selectedValues)
        If ipaEntities.IsNullOrEmpty() Then
            AjaxAlert("Ricerca Nulla")
            Exit Sub
        End If

        InitializeTreeView(ipaEntities.Count)

        For Each pa As IPA In ipaEntities
            tvwIPA.Nodes(0).Nodes.Add(AddNode(pa))
        Next

        If ipaEntities.Count = 1 Then
            tvwIPA.Nodes(0).Nodes(0).Focus()
        End If

    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Ritorna(True)
    End Sub

    Private Sub btnConfermaNuovo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfermaNuovo.Click
        Ritorna(False)
    End Sub

    Protected Sub tvwIPA_NodeClick(ByVal sender As System.Object, ByVal e As RadTreeNodeEventArgs) Handles tvwIPA.NodeClick
        cmdDetail.Enabled = Not String.IsNullOrEmpty(e.Node.Value)
    End Sub

    Private Sub cmdDetail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDetail.Click
        Dim url As String = "../Utlt/UtltIPAd.aspx?Path=" & tvwIPA.SelectedNode.Value.Replace("'", "\'")
        AjaxManager.ResponseScripts.Add(String.Format("OpenWindow('{0}','{1}');", windowDetails.ClientID, url))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, tvwIPA, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(tvwIPA, cmdDetail)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnConfermaNuovo, tvwIPA, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub InitializeTreeView(Optional ByVal count As Integer = 0)
        Dim tn As New RadTreeNode
        tn.Text = "Pubbliche amministrazioni" + If(count > 0, String.Format(" ({0})", count.ToString()), Nothing)
        tn.Expanded = True
        tvwIPA.Nodes.Clear()
        tvwIPA.Nodes.Add(tn)
    End Sub

    Private Shared Function AddNode(ByVal pa As IPA) As RadTreeNode
        Dim tn As New RadTreeNode()
        tn.Text = pa.Description
        tn.Value = pa.ADSPath
        tn.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Ipa)
        tn.Expanded = False
        tn.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
        tn.Nodes.Add(New RadTreeNode())
        Return tn
    End Function

    Private Sub Ritorna(ByVal closeForm As Boolean)
        Dim tn As RadTreeNode = tvwIPA.SelectedNode
        If (tn Is Nothing) OrElse String.IsNullOrEmpty(tn.Value) Then
            Exit Sub
        End If
        Dim ipaEntity As IPA = IPARetriever.GetIpaEntitieByPath(ProtocolEnv.LdapIndicePa, tn.Value)
        If ipaEntity Is Nothing Then
            Exit Sub
        End If
        Dim contact As Contact = ContactFacade.CreateFromIpa(ipaEntity)
        Dim [object] As String = StringHelper.EncodeJS(JsonConvert.SerializeObject(contact))
        Dim script As String = String.Format("ReturnValuesJSon('{0}','{1}','{2}','{3}');", "Ins", tn.Value, [object], closeForm.ToString.ToLowerInvariant())
        AjaxManager.ResponseScripts.Add(script)
    End Sub
    Private Sub tvwIPATreeNodeExpand(ByVal o As Object, ByVal e As RadTreeNodeEventArgs) Handles tvwIPA.NodeExpand
        SelectedNodeChange(e.Node)
    End Sub
    Private Sub tvwIPATreeNodeClick(ByVal o As Object, ByVal e As RadTreeNodeEventArgs) Handles tvwIPA.NodeClick
        SelectedNodeChange(e.Node)
    End Sub
    Public Sub SelectedNodeChange(ByVal selectNode As RadTreeNode)
        Dim thispath As String = selectNode.Value.Replace("'", "\'")
        selectNode.Nodes.Clear()
        selectNode.Expanded = True
        Dim ipaEntities As List(Of IPA) = IPARetriever.GetIpaChildEntities(thispath)
        If ipaEntities.Count > 0 Then
            For Each pa As IPA In ipaEntities
                selectNode.Nodes.Add(AddNode(pa))
            Next
        End If
    End Sub
#End Region

End Class