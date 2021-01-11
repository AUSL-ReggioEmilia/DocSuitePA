Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI


Partial Public Class DocmWorkFlow
    Inherits DocmBasePage

    Enum Campi
        iStep = 0
        SubStep = 1
        IdRole = 2
        RoleName = 3
        FullIncrementalPath = 4
    End Enum

#Region " Fields "

    Const PIPE As String = "|"

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim s As String = String.Empty
        Dim nodoUser As RadTreeNode = Nothing
        Dim nodoStart As RadTreeNode = Nothing
        Dim nodoOld As RadTreeNode = Nothing
        Dim sStep As String = String.Empty
        Dim sStepOld As String = String.Empty
        Dim iRole As Integer
        Dim iRoleOld As Integer
        Dim sSubStep As String = String.Empty
        Dim sSubStepOld As String = String.Empty
        Dim sFullOld As String = String.Empty
        Dim ok As Boolean
        Dim sFullKey As String = String.Empty
        Dim root As Boolean

        tvwWorkFlow.Visible = True
        tvwWorkFlow.Nodes.Clear()
        Dim documentTokenlist As ArrayList = New ArrayList(Facade.DocumentTokenFacade.GetTokenList(CurrentDocumentYear, CurrentDocumentNumber))
        If documentTokenlist.Count = 0 Then
            Exit Sub
        End If
        Dim nodo As RadTreeNode = New RadTreeNode
        nodo.Font.Bold = True
        nodo.Text = "Pratica " & DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)
        nodo.ImageUrl = "images/Pratica.gif"
        nodo.Value = "0"
        nodo.Expanded = True
        'TODO
        tvwWorkFlow.Nodes.Add(nodo)
        nodoStart = nodo
        For Each documenttoken As Array In documentTokenlist
            ok = True
            sStep = documenttoken(Campi.iStep)
            iRole = documenttoken(Campi.IdRole)
            sSubStep = If(documenttoken(Campi.SubStep) <> 0, "." & documenttoken(Campi.SubStep), "")
            sFullKey = sStep & sSubStep & iRole
            If sFullKey = sFullOld Then ok = False
            If ok Then
                nodo = New RadTreeNode
                s = ""
                DocUtil.SetRoleString(s, documenttoken(Campi.FullIncrementalPath))
                nodo.Text = sStep & sSubStep & " " & s
                nodo.ImageUrl = "images/RoleOn.gif"
                nodo.Value = sStep & sSubStep & "|" & iRole
                nodo.Expanded = True
                nodo.Font.Bold = True
                root = If(sStep & sSubStep = sStepOld & sSubStepOld, False, True)
                Select Case root
                    Case True
                        nodoStart.Nodes.Add(nodo)
                    Case False
                        nodoOld.Nodes.Add(nodo)
                End Select
                'assegnazione utenti
                Dim documenttokenuserlist As IList(Of DocumentTokenUser)
                'If Docm.SqlDocumentTokenUserSearch(dsUser, Year, Number, dw("Step"), dw("SubStep"), iRole) Then
                documenttokenuserlist = Facade.DocumentTokenUserFacade.DocumentTokenUserSearch(CurrentDocumentYear, CurrentDocumentNumber, documenttoken(Campi.iStep), documenttoken(Campi.SubStep), iRole, False, "", True)

                If documenttokenuserlist.Count > 0 Then
                    For Each documenttokenuser As DocumentTokenUser In documenttokenuserlist
                        Dim addUser As Boolean = True
                        nodoUser = New RadTreeNode
                        nodoUser.Text = documenttokenuser.UserName & " (" & documenttokenuser.DocStep & If(documenttokenuser.SubStep <> 0, "." & documenttokenuser.SubStep, "") & ")"
                        Select Case documenttokenuser.IsActive

                            Case 0
                                Select Case True
                                    Case documenttoken(Campi.iStep) = documenttokenuser.LastStep
                                        nodoUser.ImageUrl = "../Comm/Images/Remove16.gif"
                                    Case Else
                                        nodoUser.ImageUrl = "../Comm/Images/User16.gif"
                                End Select
                                'NodoUser.ImageUrl = "../Comm/Images/Remove16.gif"
                                If documenttoken(Campi.iStep) > documenttokenuser.LastStep Then addUser = False
                            Case 1
                                nodoUser.ImageUrl = "../Comm/Images/User16.gif"
                        End Select
                        nodoUser.Expanded = True
                        If addUser Then nodo.Nodes.Add(nodoUser)
                    Next
                End If
            End If
            sFullOld = sFullKey
            sStepOld = sStep
            sSubStepOld = sSubStep
            iRoleOld = iRole
            nodoOld = nodo
        Next
        TokenWorkFlowImages(tvwWorkFlow.Nodes(0))
        Dim documenttokenrolelist As IList(Of DocumentToken)
        documenttokenrolelist = Facade.DocumentTokenFacade.DocumentTokenRoleCC(CurrentDocumentYear, CurrentDocumentNumber)
        If documenttokenrolelist.Count > 0 Then
            nodo = New RadTreeNode
            nodo.Text = "Settori in Copia Conoscenza"
            nodo.Expanded = True
            nodo.Font.Bold = True
            nodo.ImageUrl = "images/RoleOn.gif"
            nodo.Style.Add("color", "gray")
            tvwWorkFlow.Nodes.Add(nodo)
            nodoStart = nodo
            For Each documenttokenrole As DocumentToken In documenttokenrolelist
                nodo = New RadTreeNode
                s = ""
                DocUtil.SetRoleString(s, documenttokenrole.RoleDestination.FullIncrementalPath)
                nodo.Text = documenttokenrole.DocStep & " " & s
                nodo.ImageUrl = "images/RoleOn.gif"
                nodo.Style.Add("color", "gray")
                nodo.Font.Bold = True
                nodoStart.Nodes.Add(nodo)
            Next
        End If
    End Sub

    Private Sub TokenWorkFlowImages(ByVal nodo As RadTreeNode)
        Dim txtPStep As String = Request.QueryString("txtPStep")
        Dim txtPIdOwner As String = Request.QueryString("txtPIdOwner")
        Dim txtRRStep As String = Request.QueryString("txtRRStep")
        Dim txtRRIdOwner As String = Request.QueryString("txtRRIdOwner")
        Dim txtPRStep As String = Request.QueryString("txtPRStep")
        Dim txtPRIdOwner As String = Request.QueryString("txtPRIdOwner")
        Dim txtRNStep As String = Request.QueryString("txtRNStep")
        Dim txtRNIdOwner As String = Request.QueryString("txtRNIdOwner")
        Dim txtRStep As String = Request.QueryString("txtRStep")
        Dim txtRIdOwner As String = Request.QueryString("txtRIdOwner")
        For Each n As RadTreeNode In nodo.Nodes
            If n.Value = "" Then
                Continue For
            End If

            Dim a As String() = Split(n.Value.ToString, "|")
            Dim sS As String = PIPE & a(0) & PIPE
            Dim sI As String = PIPE & a(1) & PIPE
            If txtPIdOwner <> "" Then
                If InStr(txtPStep, sS) <> 0 And InStr(txtPIdOwner, sI) <> 0 Then n.ImageUrl = "../Docm/images/roleonP.gif"
            End If
            If txtRRIdOwner <> "" Then
                If InStr(txtRRStep, sS) <> 0 And InStr(txtRRIdOwner, sI) <> 0 Then n.ImageUrl = "../Docm/images/roleonRR.gif"
            End If
            If txtPRIdOwner <> "" Then
                If InStr(txtPRStep, sS) <> 0 And InStr(txtPRIdOwner, sI) <> 0 Then n.ImageUrl = "../Docm/images/roleonPR.gif"
            End If
            If txtRNIdOwner <> "" Then
                If InStr(txtRNStep, sS) <> 0 And InStr(txtRNIdOwner, sI) <> 0 Then n.ImageUrl = "../Docm/images/roleonRN.gif"
            End If
            If txtRIdOwner <> "" Then
                If InStr(txtRStep, sS) <> 0 And InStr(txtRIdOwner, sI) <> 0 Then n.ImageUrl = "../Docm/images/roleonR.gif"
            End If
            If n.Nodes.Count > 0 Then TokenWorkFlowImages(n)

        Next
    End Sub

#End Region

End Class


