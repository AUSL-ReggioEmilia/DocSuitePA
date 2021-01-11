Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports Newtonsoft.Json

Partial Public Class CommonSelSettoriUtenti
    Inherits CommBasePage

#Region "Enum"
    Private Enum RightTitle
        None = 0
        Gestione = 1
        Workflow = 2
        Manager = 3
    End Enum
#End Region

#Region "Properties"
    Private ReadOnly Property Role() As Role
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("idRole")) Then
                Return Facade.RoleFacade.GetById(Request.QueryString("idRole"))
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property CommUtilInstance() As CommonUtil
        Get
            Return CommonInstance
        End Get
    End Property

#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim Reference As String = String.Empty
        ' TODO: verificare che è sta roba
        Select Case Type
            Case "Docm"
                Reference = DocSuiteContext.Current.DossierAndPraticheLabel
            Case "Prot"
                Reference = "Protocollo"
                Exit Sub
            Case "Resl"
                Reference = Facade.TabMasterFacade.TreeViewCaption
                Exit Sub
        End Select

        'verifiche parametri
        If Role Is Nothing Then
            Throw New DocSuiteException(Reference, "Specificare codice Settore", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Sub Initialize()
        Dim node As RadTreeNode = Nothing
        Dim userNode As RadTreeNode = Nothing
        Dim rightTitle As RightTitle
        Dim users As IList(Of AccountModel)
        If Role.RoleGroups.Count <= 0 Then
            Exit Sub
        End If

        RadTreeUsers.Nodes(0).Text = Role.Name
        For Each roleGroup As RoleGroup In Role.RoleGroups
            node = New RadTreeNode
            rightTitle = GetRightTitle(roleGroup.DocumentRights)
            node.Text = roleGroup.Name & StringEnum.GetStringValue(rightTitle)
            node.Value = roleGroup.Name
            node.Expanded = True
            node.ImageUrl = ImagePath.SmallSubRole
            node.Checkable = False
            RadTreeUsers.Nodes(0).Nodes.Add(node)
            users = CommonAD.GetADUsersFromGroup(roleGroup.Name, CommonShared.UserDomain)
            For Each user As AccountModel In users
                userNode = New RadTreeNode
                userNode.Text = user.Account
                userNode.Value = user.Name
                If rightTitle <> RightTitle.Manager And roleGroup.Name <> "Domain Users" Then
                    userNode.Checkable = True
                Else
                    userNode.Checkable = False
                End If
                userNode.ImageUrl = "../Comm/Images/User16.gif"
                userNode.Font.Bold = True
                node.Nodes.Add(userNode)
            Next

        Next
    End Sub
#End Region

#Region "Private Function"
    Private Function GetRightTitle(ByVal Rights As String) As RightTitle
        Select Case Rights
            Case "10000000000000000000"
                Return RightTitle.Gestione
            Case "11000000000000000000"
                Return RightTitle.Workflow
            Case "11100000000000000000"
                Return RightTitle.Manager
            Case Else
                Return RightTitle.None
        End Select
    End Function
#End Region

#Region "Conferma Button Event"
    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Dim userNodeList As New List(Of UserNode)
        Dim jsonUserNodeList As String = String.Empty

        For Each node As RadTreeNode In RadTreeUsers.CheckedNodes
            userNodeList.Add(New UserNode(node.Value, node.Text, node.ParentNode.Value))
        Next

        jsonUserNodeList = JsonConvert.SerializeObject(userNodeList)
        AjaxManager.ResponseScripts.Add("ReturnValues('" & jsonUserNodeList & "');")
    End Sub
#End Region

End Class