Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class CommonSelContactDouble
    Inherits CommBasePage

#Region " Properties "

    Private ReadOnly Property ContactIdList() As String()
        Get
            Dim ids As String = Request.QueryString("ContactId")
            Return ids.Split("|"c)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub tvwContacts_NodeCheck(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles tvwContacts.NodeCheck
        Dim emailAddress As String = e.Node.Attributes("emailAddress")
        If Not String.IsNullOrEmpty(emailAddress) Then
            tvwCheckMail(tvwContacts.Nodes(0), e.Node, emailAddress)
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(tvwContacts, tvwContacts)
    End Sub

    Private Sub Initialize()
        For Each contactId As String In ContactIdList
            If String.IsNullOrEmpty(contactId) Then
                Continue For
            End If

            Dim contact As Contact = Facade.ContactFacade.GetById(contactId)
            If contact Is Nothing Then
                Continue For
            End If

            Dim node As RadTreeNode = AddNode(Nothing, contact)
            node.Checkable = True
        Next
    End Sub

    Private Function AddNode(ByRef node As RadTreeNode, ByVal contact As Contact) As RadTreeNode
        Dim nodeToAdd As New RadTreeNode()
        If (tvwContacts.FindNodeByValue(contact.Id.ToString()) Is Nothing) Then
            SetNode(nodeToAdd, contact)
            If Not (contact Is Nothing) Then
                If contact.Parent Is Nothing Then 'Primo Livello
                    tvwContacts.Nodes(0).Nodes.Add(nodeToAdd)
                Else
                    Dim newNode As RadTreeNode = tvwContacts.FindNodeByValue(contact.Parent.Id.ToString())
                    If (newNode Is Nothing) Then
                        AddNode(nodeToAdd, contact.Parent)
                    Else
                        newNode.Nodes.Add(nodeToAdd)
                    End If
                End If
            End If
            If Not (node Is Nothing) Then
                nodeToAdd.Nodes.Add(node)
            End If
            nodeToAdd.Checkable = False
        End If
        Return nodeToAdd
    End Function

    Private Sub SetNode(ByRef node As RadTreeNode, ByVal contact As Contact)
        node.Text = Replace(contact.Description, "|", " ")
        node.Value = contact.Id.ToString()
        node.ImageUrl = Page.ResolveUrl(ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1))
        node.Attributes.Add("emailAddress", contact.EmailAddress)
        node.Expanded = True
    End Sub

    Private Sub tvwCheckMail(ByVal rootNode As RadTreeNode, ByVal currentNode As RadTreeNode, ByVal mail As String)
        If rootNode.Nodes.Count = 0 Then Exit Sub
        For Each tn As RadTreeNode In rootNode.Nodes
            If tn.Checked AndAlso tn.Value <> currentNode.Value AndAlso tn.Attributes("emailAddress") = mail Then
                tn.Checked = False
            End If
            tvwCheckMail(tn, currentNode, mail)
        Next
    End Sub

#End Region

End Class