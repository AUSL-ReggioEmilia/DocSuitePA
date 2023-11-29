Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class UserSelRoleUser
    Inherits UserBasePage


#Region " Fields "
    Private _collaborationType As String = String.Empty
#End Region

#Region " Properties "
    Private ReadOnly Property CollaborationType As String
        Get
            If String.IsNullOrEmpty(_collaborationType) Then
                _collaborationType = HttpContext.Current.Request.QueryString.GetValueOrDefault("CollaborationType", String.Empty)
            End If
            Return _collaborationType
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        btnConferma.OnClientClick = "return CloseWindow();"
        MasterDocSuite.TitleVisible = False

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub Tvw_NodeClick(ByVal sender As System.Object, ByVal e As RadTreeNodeEventArgs) Handles Tvw.NodeClick
        btnConferma.Enabled = False

        Dim tn As RadTreeNode = Tvw.SelectedNode
        If tn.Attributes("Person") Is Nothing Then
            Exit Sub
        End If

        Dim roleuser As RoleUser = Facade.RoleUserFacade.GetById(Integer.Parse(tn.Value))
        If roleuser Is Nothing Then
            Exit Sub
        End If

        Dim cRet As Contact = New Contact()
        With cRet
            .Description = Facade.CollaborationFacade.GetSignerDescription(roleuser.Description, roleuser.Account, CollaborationType)
            .EmailAddress = roleuser.Email
            .Code = roleuser.Account
            .RoleUserIdRole = roleuser.Role.Id.ToString()
            .ContactType = New ContactType(ContactType.Role)
        End With

        btnConferma.OnClientClick = String.Format("return CloseWindow('{0}');", StringHelper.EncodeJS(JsonConvert.SerializeObject(cRet)))
        btnConferma.Enabled = True
    End Sub

    Private Sub CmdFilterClick(sender As Object, e As EventArgs) Handles cmdFilter.Click
        Initialize()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Tvw.Nodes.Clear()
        If ProtocolEnv.CollaborationRightsEnabled Then
            Dim finder As New NHRoleUserFinder()
            finder.DSWEnvironmentIn.Add(Env)
            finder.TypeIn = New List(Of String) From {RoleUserType.D.ToString(), RoleUserType.V.ToString()}
            finder.RoleName = txtNameFilter.Text
            finder.RoleEnabled = True
            Dim results As IList(Of RoleUser) = finder.List()
            Dim results_D As IList(Of RoleUser) = results.Where(Function(f) f.Type = RoleUserType.D.ToString()).OrderBy(Function(f) f.Role.Name).ThenBy(Function(f) f.Account).ToList()
            Dim results_V As IList(Of RoleUser) = results.Where(Function(f) f.Type = RoleUserType.V.ToString()).OrderBy(Function(f) f.Role.Name).ThenBy(Function(f) f.Account).ToList()

            PopulateRoleUser(results_D)
            PopulateRoleUser(results_V)

            If (Not ProtocolEnv.HideSegreteriaOnDirigSelect) Then
                Dim results_S As IList(Of RoleUser) = results.Where(Function(f) f.Type = RoleUserType.S.ToString()).OrderBy(Function(f) f.Role.Name).ThenBy(Function(f) f.Account).ToList()
                PopulateRoleUser(results_S)
            End If
        Else
            ' Popolo dirigenti
            PopulateRoleUser(Facade.RoleUserFacade.GetByType(RoleUserType.D, True, txtNameFilter.Text, CurrentTenant.TenantAOO.UniqueId))

            ' Popolo vicedirigenti
            PopulateRoleUser(Facade.RoleUserFacade.GetByType(RoleUserType.V, True, txtNameFilter.Text, CurrentTenant.TenantAOO.UniqueId))

            If (Not ProtocolEnv.HideSegreteriaOnDirigSelect) Then
                ' Popolo segreterie
                PopulateRoleUser(Facade.RoleUserFacade.GetByType(RoleUserType.S, True, txtNameFilter.Text, CurrentTenant.TenantAOO.UniqueId))
            End If
        End If

        txtNameFilter.Focus()
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(Tvw, btnConferma)
    End Sub

    Private Sub PopulateRoleUser(ByVal roleUsers As IList(Of RoleUser))
        For Each roleUser As RoleUser In roleUsers
            ' Verifico esistenza settore
            Dim roleNode As RadTreeNode = Tvw.FindNodeByValue(roleUser.Role.Id.ToString())
            If roleNode Is Nothing Then
                roleNode = New RadTreeNode
                roleNode.Text = roleUser.Role.Name
                roleNode.Value = roleUser.Role.Id.ToString
                roleNode.Expanded = Not ProtocolEnv.SelRoleUserStartCollapsed
                roleNode.ImageUrl = ImagePath.SmallRole
                roleNode.Style.Add("font-weight", "bold")
                Tvw.Nodes.Add(roleNode)
            End If

            ' Verifico nel settore esistenza del ruolo
            Dim typeNode As RadTreeNode = roleNode.Nodes.FindNodeByValue(roleUser.Type)
            If typeNode Is Nothing Then
                typeNode = New RadTreeNode()
                typeNode.Text = Facade.RoleUserFacade.GetRoleUserTypeName(roleUser.Type)
                typeNode.Value = roleUser.Type
                typeNode.Expanded = True
                typeNode.ImageUrl = "../Comm/images/Interop/Ruolo.gif"
                typeNode.Style.Add("font-weight", "bold")
                roleNode.Nodes.Add(typeNode)
            End If

            ' Verifico nel settore esistenza dell'utente
            Dim userNode As RadTreeNode = Nothing
            For Each n As RadTreeNode In roleNode.Nodes
                userNode = n.Nodes.FindNodeByText(roleUser.Description)
                If userNode IsNot Nothing Then
                    Exit For
                End If
            Next

            If userNode Is Nothing Then
                userNode = New RadTreeNode()
                userNode.Text = roleUser.Description
                If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
                    userNode.Text = String.Concat(roleUser.Description, " (", roleUser.Account, ")")
                End If
                userNode.Value = roleUser.Id.ToString()
                userNode.Attributes.Add("account", roleUser.Account)
                userNode.ImageUrl = "../App_Themes/DocSuite2008/imgset16/user.png"
                userNode.Attributes.Add("Person", "Person")
                userNode.Style.Add("font-weight", "bold")
                typeNode.Nodes.Add(userNode)
            End If
        Next
        ' Se è presente un singolo settore lo predispongo espanso indipendentemente da ProtocolEnv.SelRoleUserStartCollapsed
        If Tvw.Nodes.Count = 1 Then
            Tvw.Nodes(0).Expanded = True
        End If
    End Sub

#End Region

End Class