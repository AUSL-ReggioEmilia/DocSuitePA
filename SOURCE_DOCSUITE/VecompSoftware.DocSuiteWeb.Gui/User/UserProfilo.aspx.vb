Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq

Public Class UserProfilo
    Inherits UserBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        WebUtils.ExpandOnClientNodeAttachEvent(tvwSettoriDocm)
        WebUtils.ExpandOnClientNodeAttachEvent(tvwSettoriProt)
        WebUtils.ExpandOnClientNodeAttachEvent(tvwSettoriResl)
        If CommonInstance.ReslEnabled Then
            lblAtti.Text = Facade.TabMasterFacade.TreeViewCaption
        End If
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim roles As String
        If CommonInstance.DocmEnabled AndAlso tvwSettoriDocm.Nodes(0).Nodes.Count > 0 Then
            roles = ""
            TvwCheck(tvwSettoriDocm.Nodes(0), roles)
            If String.IsNullOrEmpty(roles) Then
                AjaxAlert("Pratiche: Selezionare almeno un Settore")
                Exit Sub
            End If
            CommonShared.GroupPaperworkSelected = roles
        End If

        Dim managerRoles As String
        If CommonInstance.ProtEnabled Then
            managerRoles = String.Empty
            roles = String.Empty
            'Gestione manager
            If tvwSettoriProtMng.Nodes(0).Nodes.Count > 0 Then
                managerRoles = String.Empty : TvwCheck(tvwSettoriProtMng.Nodes(0), managerRoles)
            End If
            'Gestione autorizzazioni semplici
            If tvwSettoriProt.Nodes(0).Nodes.Count > 0 Then
                roles = String.Empty : TvwCheck(tvwSettoriProt.Nodes(0), roles)
            End If
            'Verifica che ci sia almeno 1 settore abilitato (semplice o manager)
            If (managerRoles = String.Empty AndAlso roles = String.Empty) Then
                Dim str As String = "Protocollo: Selezionare almeno un Settore"

                If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                    str = "Protocollo: Selezionare almeno un Settore con diritto semplice o con diritto manager"
                End If

                AjaxAlert(str)
                Exit Sub
            Else
                'Se c'è almeno uno dei 2 lo memorizzo
                CommonShared.GroupProtocolManagerSelected = managerRoles
                CommonShared.GroupProtocolNotManagerSelected = roles
            End If
        End If

        If CommonInstance.ReslEnabled AndAlso tvwSettoriResl.Nodes(0).Nodes.Count > 0 Then
            roles = ""
            TvwCheck(tvwSettoriResl.Nodes(0), roles)
            If String.IsNullOrEmpty(roles) Then
                AjaxAlert(String.Concat(Facade.TabMasterFacade.TreeViewCaption, ": Selezionare almeno un Settore"))
                Exit Sub
            End If
            CommonShared.GroupResolutionSelected = roles
        End If
        Response.Redirect("../Comm/CommIntro.aspx")
    End Sub

    Private Sub btnExtend_Click(sender As Object, e As EventArgs) Handles btnExtend.Click
        If DocSuiteContext.Current.ProtocolEnv.IsSecurityGroupEnabled Then
            Throw New NotImplementedException("Funzionalità non ancora implementata.")
        End If

        'Dim roles As IList(Of Role) = Facade.RoleFacade.GetUserRoleList(Type, CommonUtil.UserConnectedGroups, "11")

        'Dim roleList As IList(Of Role) = Facade.RoleFacade.GetManageableRoles(roles)

        'For Each item As Role In roleList
        '    WebUtils.ObjTreeViewRoleAdd(tvwSettoriProtMng, Nothing, item, True, True, True, "", True, False)
        'Next

    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        'Atti
        If CommonInstance.ReslEnabled Then
            RolesAdd(tvwSettoriResl, DSWEnvironment.Resolution, ResolutionRoleRightPositions.Enabled, CommonShared.GroupResolutionSelected)
            RadTabStrip1.FindTabByText("Atti").Text = lblAtti.Text
            RadTabStrip1.FindTabByText(lblAtti.Text).Visible = True
            RadPageView3.Visible = True
            RadTabStrip1.SelectedIndex = 2
        End If
        'pratiche
        If CommonInstance.DocmEnabled Then
            RolesAdd(tvwSettoriDocm, DSWEnvironment.Document, DocumentRoleRightPositions.Enabled, CommonShared.GroupPaperworkSelected)
            RadTabStrip1.FindTabByText("Pratiche").Visible = True
            RadPageView2.Visible = True
            RadTabStrip1.SelectedIndex = 1
        End If
        'protocollo
        If CommonInstance.ProtEnabled Then
            If Not ProtocolEnv.IsDistributionEnabled Then
                tvwSettoriProtMng.Visible = False
            End If
            RolesAdd(tvwSettoriProtMng, DSWEnvironment.Protocol, ProtocolRoleRightPositions.Manager, CommonShared.GroupProtocolManagerSelected)
            RolesAdd(tvwSettoriProt, DSWEnvironment.Protocol, ProtocolRoleRightPositions.Enabled, CommonShared.GroupProtocolNotManagerSelected)
            btnExtend.Visible = True
            RadTabStrip1.FindTabByText("Protocollo").Visible = True
            RadPageView1.Visible = True
            RadTabStrip1.SelectedIndex = 0
        End If
    End Sub

    Private Sub RolesAdd(tvw As RadTreeView, environment As DSWEnvironment, right As Integer, selected As String)
        Dim firstInitialization As Boolean = selected Is Nothing

        Dim roleFacade As New RoleFacade(GetDbName(environment))

        Dim roles As IList(Of Role) = roleFacade.GetUserRoles(environment, right, True)

        'Se ricevo nothing significa che è la prima attivazione, quindi abilito tutto
        If roles Is Nothing OrElse roles.Count <= 0 Then
            Exit Sub
        End If

        For Each role As Role In roles
            'Se trova il settore tra i selezionati allora lo mette True
            'Se non lo trova gli mette il valore di checkValue, che sarà
            ' - True: se è la prima attivazione
            ' - False: se effettivamente è stato chiesto di rimuovere (ovvero String.Empty)
            'Il controllo di coerenza (almeno 1 settore) viene fatto in fase di memorizzazione
            If roleFacade.CurrentUserBelongsToRoles(environment, role) Then
                Dim checkValue As Boolean = (InStr(selected, String.Format("|{0}|", role.Id)) <> 0 OrElse firstInitialization)
                WebUtils.ObjTreeViewRoleAdd(tvw, Nothing, role, True, True, True, "", True, checkValue)
            End If
        Next
    End Sub

    Public Function SetType() As String
        Return Type
    End Function

    Private Sub TvwCheck(ByVal nodo As RadTreeNode, ByRef sId As String)
        If nodo.Nodes.Count = 0 Then Exit Sub
        For Each tn As RadTreeNode In nodo.Nodes
            If tn.Checked Then
                If sId <> "" Then
                    sId &= ","
                End If
                sId &= "|" & tn.Value & "|"
            End If
            TvwCheck(tn, sId)
        Next
    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll.Click
        CheckAll()
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeselectAll.Click
        UnCheckAll()
    End Sub

    Public Sub CheckAll()
        If RadMultiPage1.SelectedPageView.ID = "RadPageView1" Then
            CheckNodes(tvwSettoriProtMng.GetAllNodes(), True)
            CheckNodes(tvwSettoriProt.GetAllNodes(), True)
        End If
        If RadMultiPage1.SelectedPageView.ID = "RadPageView2" Then
            CheckNodes(tvwSettoriDocm.GetAllNodes(), True)
        End If
        If RadMultiPage1.SelectedPageView.ID = "RadPageView3" Then
            CheckNodes(tvwSettoriResl.GetAllNodes(), True)
        End If
    End Sub

    Public Sub UnCheckAll()
        If RadMultiPage1.SelectedPageView.ID = "RadPageView1" Then
            CheckNodes(tvwSettoriProtMng.GetAllNodes(), False)
            CheckNodes(tvwSettoriProt.GetAllNodes(), False)
        End If
        If RadMultiPage1.SelectedPageView.ID = "RadPageView2" Then
            CheckNodes(tvwSettoriDocm.GetAllNodes(), False)
        End If
        If RadMultiPage1.SelectedPageView.ID = "RadPageView3" Then
            CheckNodes(tvwSettoriResl.GetAllNodes(), False)
        End If
    End Sub

    Private Sub CheckNodes(ByRef nodes As IList(Of RadTreeNode), ByVal check As Boolean)
        If nodes.IsNullOrEmpty() Then
            Exit Sub
        End If
        For Each node As RadTreeNode In nodes.Where(Function(x) x.Checkable)
            node.Checked = check
        Next
    End Sub
#End Region

End Class
