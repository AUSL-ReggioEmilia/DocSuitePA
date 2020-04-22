Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Linq
Imports System.Web
Imports System.Text
Imports System.Text.RegularExpressions

Partial Public Class CommonSelContactEvolution
    Inherits CommonBasePage
    Private ReadOnly _mailDescritionExp As Regex = New Regex("[,.]")

#Region " Properties "

    Public ReadOnly Property CallerId() As String
        Get
            Return Request.QueryString("ParentID")
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        btnSearch.Text = ProtocolEnv.TextBtnSearchContacts
        If Not IsPostBack() Then
            MasterDocSuite.TitleVisible = False
            txtFilter.Focus()
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        'btnSearch.Enabled = False
        tvwContactDomain.Visible = False
        Dim filter As String = txtFilter.Text.Trim()
        Dim values As ICollection(Of String) = filter.Split(";"c) _
                                 .Where(Function(f) Not String.IsNullOrEmpty(f)) _
                                 .Select(Function(f) f.Trim()) _
                                 .ToList()
        Dim found As IList(Of ADContact)
        Dim newValue As StringBuilder = New StringBuilder()
        Dim skip As Boolean = False
        For Each val As String In values.Where(Function(v) Not String.IsNullOrEmpty(v))
            If (skip) Then
                txtFilter.Text = String.Concat(txtFilter.Text, val, "; ")
                Continue For
            End If
            found = CommonAD.FindADContactsAndDistributionsList(val, String.Empty)
            If (found.Count = 0) Then
                Dim mail As String = RegexHelper.MatchEmail(val)
                If (RegexHelper.IsValidEmail(mail)) Then
                    newValue.AppendFormat("{0}; ", val)
                Else
                    'mettere errore se necessario
                End If
            End If
            If (found.Count = 1) Then
                newValue.AppendFormat("{0}; ", found.First().DisplayEmail)
            End If
            If (found.Count > 1) Then
                tvwContactDomain.Visible = True
                tvwContactDomain.Nodes.Clear()
                Dim root As New RadTreeNode() With
                    {
                    .Text = String.Format("Selezionare il contatto corretto per il valore '{0}'", val),
                    .Expanded = True
                    }
                tvwContactDomain.Nodes.Add(root)
                For Each contact As ADContact In found
                    AddToTree(contact, root)
                Next
                txtFilter.Text = newValue.ToString()
                skip = True
            End If
        Next
        If skip Then
            Return
        End If
        txtFilter.Text = newValue.ToString()
        btnSearch.Enabled = True
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs)
        If (tvwContactDomain.Visible) Then
            If (tvwContactDomain.SelectedNode Is Nothing) Then
                AjaxAlert("Selezionare un contatto dall'elenco")
                Return
            End If
            txtFilter.Text = String.Concat(txtFilter.Text, tvwContactDomain.SelectedNode.Attributes("objectValue"), "; ")
            btnSearch_Click(sender, e)
        End If
        If (Not tvwContactDomain.Visible) Then
            btnSearch_Click(sender, e)
            If (tvwContactDomain.Visible) Then
                Return
            End If
            GetContacts(True)
        End If
    End Sub

    Protected Sub CommonSelContactEvolution_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim args As String() = e.Argument.Split("|"c)
        If (args.Count = 2) Then
            txtFilter.Text = args(1)
            If (args(0).Eq("btnSearch")) Then
                btnSearch_Click(sender, e)
            End If
            If (args(0).Eq("btnConfirm")) Then
                btnConfirm_Click(sender, e)
            End If
        End If
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ajaxPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf CommonSelContactEvolution_AjaxRequest
    End Sub

    Private Sub AddToTree(adContact As ADContact, root As RadTreeNode)
        Dim node As New RadTreeNode()
        node.Text = Server.HtmlEncode(adContact.DisplayEmail)
        node.Value = adContact.Email
        node.Attributes.Add("objectEmail", adContact.Email)
        node.Attributes.Add("objectValue", adContact.DisplayEmail)
        node.Attributes.Add("objectDisplay", adContact.DisplayName)
        root.Nodes.Add(node)
    End Sub

    Private Sub GetContacts(ByVal close As Boolean)
        Dim filter As String = txtFilter.Text.Trim()
        Dim values As ICollection(Of String) = filter.Split(";"c) _
                         .Where(Function(f) Not String.IsNullOrEmpty(f)) _
                         .Select(Function(f) f.Trim()) _
                         .ToList()
        Dim contacs As List(Of Contact) = New List(Of Contact)()
        For Each item As String In values
            Dim name As String = RegexHelper.MatchName(item)
            Dim email As String = RegexHelper.MatchEmail(item)

            Dim c As New Contact()
            c.Description = If(String.IsNullOrEmpty(name), email, _mailDescritionExp.Replace(name, String.Empty))
            c.EmailAddress = email
            c.CertifiedMail = email
            c.IsActive = Convert.ToInt16(1)
            c.Parent = Nothing

            c.ContactType = New ContactType()
            c.ContactType.Id = ContactType.Person
            contacs.Add(c)
        Next

        Dim temp As String = JsonConvert.SerializeObject(contacs)
        Dim serialized As String = HttpUtility.HtmlEncode(temp)
        Dim jsScript As String = "var jsonRes= ""{0}""; ReturnValuesJson(jsonRes, '{1}');"
        jsScript = String.Format(jsScript, serialized, close.ToString().ToLowerInvariant())
        AjaxManager.ResponseScripts.Add(jsScript)
    End Sub

#End Region

End Class