Imports System.Collections.Generic
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocmProtocollo
    Inherits DocmBasePage

#Region " Fields "

    Const PIPE As Char = "|"c
    Const SEP As Char = ","c

#End Region

#Region " Properties "

    Private Property Incremental() As Short?
        Get
            If ViewState("Incremental") Is Nothing Then
                ViewState("Incremental") = Request.QueryString.GetValueOrDefault(Of Short?)("Incremental", Nothing)
            End If
            Return DirectCast(ViewState("Incremental"), Short?)
        End Get
        Set(ByVal value As Short?)
            ViewState("Incremental") = value
        End Set
    End Property

    Private ReadOnly Property IncrementalObject() As Short?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short?)("IncrementalObject", Nothing)
        End Get
    End Property

    Private ReadOnly Property Add() As String
        Get
            Return Request.QueryString("Add")
        End Get
    End Property

    Private ReadOnly Property RefreshDocument() As String
        Get
            Return Request.QueryString("Refresh")
        End Get
    End Property

    Private Property IdRole As Integer
        Get
            Return CType(ViewState("idRole"), Integer)
        End Get
        Set(value As Integer)
            ViewState("idRole") = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        Select Case Action
            Case "Insert"
                Title = "Pratica - Inserimento Collegamento Protocollo"
                If Not String.IsNullOrEmpty(Request.QueryString("Redirect")) Then
                    Title &= String.Format(" {0}/{1}", CurrentYear, CurrentNumber)
                End If
            Case "Modify"
                If Add = "ON" Then
                    Title = "Pratica - Modifica Collegamento Protocollo"
                Else
                    Title = "Pratica - Visualizza Collegamento Protocollo"
                End If
        End Select

        If Not Page.IsPostBack Then
            Initialize()
            RoleTvw()
        End If

        If Action.Eq("Modify") Then
            CercaProtocollo()
            uscProtocolSelect.ShowPreviewMode()
        End If

        If Not String.IsNullOrEmpty(CurrentYear) AndAlso Not String.IsNullOrEmpty(CurrentNumber) Then

            uscProtocolSelect.TextYear = CurrentYear
            uscProtocolSelect.TextNumber = CurrentNumber

            Dim v As String() = Split(Tvw.SelectedNode.Value, "|")
            Incremental = Short.Parse(v(0))

            Dim roleIncremental As Integer
            Dim fullIncremental As String = Nothing
            Dim titolo As String = Nothing
            Dim role As Integer = 0
            DocUtil.FncCalcolaPath(role, roleIncremental, fullIncremental, titolo, CurrentDocumentYear, CurrentDocumentNumber, Incremental.Value, True)
            IdRole = role

            uscProtocolSelect.Visible = False
            tbFolders.Visible = True

        End If

    End Sub

    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocolSelect.ProtocolSelected
        CercaProtocollo()
    End Sub

    ''' <summary>
    ''' Scatena l'inserimento del link di un protocollo in una folder
    ''' </summary>
    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        If uscProtocolSelect.SelectedProtocol Is Nothing Then
            AjaxAlert("Nessun protocollo selezionato")
            Exit Sub
        End If

        Dim link As String = ProtocolFacade.GetCalculatedLink(uscProtocolSelect.SelectedProtocol)
        Dim description As String = uscProtocolSelect.SelectedProtocol.DocumentCode

        Dim _documentTokens As IList(Of DocumentToken) = Facade.DocumentTokenFacade.DocumentTokenStep(CurrentDocumentYear, CurrentDocumentNumber, IdRole)
        If _documentTokens.Count = 1 Then
            Dim _documentObject As New DocumentObject()
            With _documentObject
                .Year = CurrentDocumentYear
                .Number = CurrentDocumentNumber
                .Incremental = Facade.DocumentObjectFacade.GetMaxId(CurrentDocumentYear, CurrentDocumentNumber)
                .IncrementalFolder = Incremental.Value
                .OrdinalPosition = _documentObject.Incremental
                .DocStep = _documentTokens(0).DocStep
                .SubStep = _documentTokens(0).SubStep
                .idObjectType = "LP"
                .Description = description
                .Reason = txtReason.Text
                .Note = txtNote.Text
                .idBiblos = 0
                .Link = link
            End With
            Facade.DocumentObjectFacade.Save(_documentObject)

            If Request.QueryString("Redirect") <> "" Then
                Dim url As String = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Docm", CurrentDocumentYear, CurrentDocumentNumber))
                Response.Redirect(url)
            Else
                AjaxManager.ResponseScripts.Add("CloseWindow('');")
                AjaxManager.ResponseScripts.Add("if(parent.parent.RefreshFolderAjax) parent.parent.RefreshFolderAjax();")
                ' GONZALEZ 20100702: Aggiunta la IF perché andava in errore, non verificato se la chiamata ha senso o se si tratta di un refuso.
                AjaxManager.ResponseScripts.Add("if(parent.parent.parent.DC_Visualizza) parent.parent.parent.DC_Visualizza('" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}", txtSelYear.Text, txtSelNumber.Text)) & "');")
            End If
        End If
    End Sub

    Private Sub btnCancella_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancella.Click
        Dim documentObject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
        Facade.DocumentObjectFacade.UpdateStatus(documentObject)

        RegisterFolderRefreshFullScript()
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnModifica.Click
        Dim tn As RadTreeNode = UscDocumentFolderProt.Destination.SelectedNode()

        Dim documentobject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))

        Dim newIncrementalFolder As Short? = Nothing
        If (tn IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(tn.Value)) Then
            newIncrementalFolder = Short.Parse(tn.Value)
            If documentobject IsNot Nothing AndAlso documentobject.IncrementalFolder <> newIncrementalFolder.Value Then
                Facade.DocumentObjectFacade.UpdateFolder(documentobject, newIncrementalFolder.Value)
            End If
        End If

        Facade.DocumentObjectFacade.UpdateDescription(documentobject, Nothing, "", txtReason.Text, txtNote.Text)

        If newIncrementalFolder.HasValue Then
            AjaxManager.ResponseScripts.Add("CloseWindow('')")
        Else
            AjaxManager.ResponseScripts.Add("CloseWindow('" & RefreshDocument & "')")

        End If
        RegisterFolderRefreshScript(newIncrementalFolder.ToString())
    End Sub

    Private Sub Tvw_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles Tvw.NodeClick
        btnInserimento.Enabled = DocumentCheckOn(Tvw.SelectedNode)
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        btnInserimento.Visible = False
        pnlCartella.Visible = False
        btnModifica.Visible = False
        btnCancella.Visible = False

        Dim titolo As String = String.Empty
        Dim fullIncremental As String = String.Empty
        Dim role As Integer
        Dim roleIncremental As Integer
        DocUtil.FncCalcolaPath(role, roleIncremental, fullIncremental, titolo, CurrentDocumentYear, CurrentDocumentNumber, Incremental.GetValueOrDefault(0), True)

        IdRole = role

        Select Case Action
            Case "Insert"
                Title = "Inserimento Collegamento Protocollo"
                If Not String.IsNullOrEmpty(Request.QueryString("Redirect")) Then
                    Title &= String.Format(" {0}/{1}", CurrentYear, CurrentNumber)
                End If
                uscProtocolSelect.TextYear = CurrentDocumentYear.ToString()
                btnInserimento.Visible = True
            Case "Modify"
                If Add.Eq("ON") Then
                    Title = "Modifica Collegamento Protocollo"
                Else
                    Title = "Visualizza Collegamento Protocollo"
                End If
                Dim documentObject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
                If documentObject IsNot Nothing Then
                    BindData(documentObject)
                    If Add.Eq("ON") Then
                        If documentObject.idObjectStatus <> "A" Then
                            btnModifica.Visible = True
                            btnCancella.Visible = True
                            txtReason.Focus()
                        End If
                    End If
                End If
                pnlCartella.Visible = True
                UscDocumentFolderProt.Year = CurrentDocumentYear
                UscDocumentFolderProt.Number = CurrentDocumentNumber
                UscDocumentFolderProt.Incremental = Incremental.Value
                UscDocumentFolderProt.Document = documentObject.Document
                UscDocumentFolderProt.IncrementalFolder = documentObject.IncrementalFolder
        End Select

        If Not String.IsNullOrEmpty(titolo) Then
            Title &= " - " & titolo
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, btnModifica)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancella, btnCancella)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnInserimento)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, UscDocumentFolderProt)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolSelect, btnInserimento)
    End Sub

    Private Sub BindData(ByRef documentObject As DocumentObject)
        Dim link As String() = documentObject.Link.Split("|"c)
        uscProtocolSelect.TextYear = link(0)
        uscProtocolSelect.TextNumber = link(1)
        txtReason.Text = documentObject.Reason
        txtNote.Text = documentObject.Note
    End Sub

    ''' <summary> Seleziona protocollo e visualizza </summary>
    Private Sub CercaProtocollo()

        Dim des As String = "Protocollo n. " & ProtocolFacade.ProtocolFullNumber(uscProtocolSelect.TextYear, uscProtocolSelect.TextNumber) & "\n\n"
        btnInserimento.Enabled = False
        If uscProtocolSelect.SelectedProtocol Is Nothing Then
            AjaxAlert(des & " Protocollo inesistente")
            Exit Sub
        End If

        ''TFS ID 1195 : Calcolo link magicamente scomparso :D
        Dim sLink As String = ProtocolFacade.GetCalculatedLink(uscProtocolSelect.SelectedProtocol)
        If Action.Eq("Insert") Then
            If DocumentUtil.DocLinkVerify(Me, sLink, Incremental, CurrentDocumentYear, CurrentDocumentNumber) Then
                AjaxAlert("Collegamento " & des & "Già inserito nella Pratica")
                Exit Sub
            End If
        End If

        If Not Facade.ProtocolFacade.SecurityGroupsUserRight(uscProtocolSelect.SelectedProtocol, Facade.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName), DocSuiteContext.Current.DocumentEnv.ProtSecurity, ProtocolContainerRightPositions.Preview, ProtocolContainerRightPositions.View, DocSuiteContext.Current.User.FullUserName) Then
            AjaxAlert(des & "Mancano i diritti necessari")
            Exit Sub
        End If
        btnInserimento.Enabled = True
    End Sub

    Private Sub RoleTvw()
        ' Controlla i diritti
        Dim roleRightsList As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DossierRoleRightPositions.Enabled, True, CurrentTenant.TenantAOO.UniqueId)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If txtIdRoleRight.Text <> "" Then
                    txtIdRoleRight.Text &= SEP
                End If
                txtIdRoleRight.Text += (PIPE & role.Id & PIPE)
            Next
        End If
        roleRightsList.Clear()
        roleRightsList = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DossierRoleRightPositions.Workflow, True, CurrentTenant.TenantAOO.UniqueId)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If txtIdRoleRightW.Text <> "" Then
                    txtIdRoleRightW.Text &= SEP
                End If
                txtIdRoleRightW.Text += (PIPE & role.Id & PIPE)
            Next
        End If
        roleRightsList.Clear()
        roleRightsList = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DossierRoleRightPositions.Manager, True, CurrentTenant.TenantAOO.UniqueId)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If txtIdRoleRightM.Text <> "" Then
                    txtIdRoleRightM.Text &= SEP
                End If
                txtIdRoleRightM.Text += (PIPE & role.Id & PIPE)
            Next
        End If

        '---prese in carico
        Dim documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PA", "PM", "PC", "PT", "PR"})
        If documentTokenList.Count > 0 Then

            For Each docToken As DocumentToken In documentTokenList

                If docToken.IsActive Then
                    WriteStep(docToken, txtPStep, txtPIdOwner, "")
                Else
                    If docToken.DocumentTabToken.Id = "PT" Then
                        WriteStep(docToken, txtRRStep, txtRRIdOwner, "")
                    Else
                        WriteStep(docToken, txtPRStep, txtPRIdOwner, "")
                    End If
                End If

                Dim documentTokenUserList As IList(Of DocumentTokenUser) = Facade.DocumentTokenUserFacade.GetDocumentTokenUserList(CurrentDocumentYear, CurrentDocumentNumber, docToken, , , True, True, True, True)

                If documentTokenUserList.Count > 0 Then
                    For Each docTokenUser As DocumentTokenUser In documentTokenUserList
                        WriteStep(docToken, txtUserStep, txtUserAccount, docTokenUser.Account)
                    Next
                End If

            Next
        End If
        documentTokenList.Clear()

        ''---richieste
        documentTokenList = Facade.DocumentTokenFacade.GetTokenList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RP", "RR"})
        If documentTokenList.Count > 0 Then
            For Each docToken As DocumentToken In documentTokenList

                Select Case docToken.Response
                    Case "N"
                        WriteStep(docToken, txtRNStep, txtRNIdOwner, "")
                    Case Else
                        WriteStep(docToken, txtRStep, txtRIdOwner, "")
                End Select

            Next
        End If
        documentTokenList.Clear()

        ''---settori CC
        Dim documentTokenListCC As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber)
        If documentTokenListCC.Count > 0 Then
            For Each docToken As DocumentToken In documentTokenListCC
                WriteStep(docToken, txtCCStep, txtCCidOwner, "")
            Next
        End If

        ''---treeview
        InitializeFolderTreeview(Tvw, True, Nothing, Nothing, Nothing)
        AddRoleTvwImages(Tvw.Nodes(0))
        RoleCC()

    End Sub

    Private Sub RoleCC()
        tvwSettoriCC.Nodes.Clear()
        Dim docTokenFacade As New DocumentTokenFacade

        Dim documentTokenListCc As IList(Of DocumentToken) = docTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True, , 1)
        If documentTokenListCc.Count > 0 Then

            Dim tn As New RadTreeNode
            tn.Text = "Settori in Copia Conoscenza"
            tn.Expanded = True
            tn.ImageUrl = "~/Docm/images/RoleOn.gif"
            tn.Style.Add("font-weight", "bold")
            tvwSettoriCC.Nodes.Add(tn)

            For Each docToken As DocumentToken In documentTokenListCc
                WebUtils.ObjTreeViewRoleAdd(tvwSettoriCC, Nothing, docToken.RoleDestination, True, True, True, "gray", False, False)
            Next

        End If

    End Sub

    Private Sub AddRoleTvwImages(ByVal nodo As RadTreeNode)
        For Each n As RadTreeNode In nodo.Nodes
            Dim sEnabled As String = "OFF"
            Dim a As String() = Split(n.Value.ToString(), "|")
            Select Case Left(a(1), 1)
                Case "R"
                    Dim s As String = PIPE & Mid(a(1), 2) & PIPE

                    Dim doc As Document = Facade.DocumentFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber)
                    Dim status As String
                    If doc.Status Is Nothing Then
                        status = String.Empty
                    Else
                        status = doc.Status.Id
                    End If

                    If StringHelper.InStrTest(txtPIdOwner.Text, s) Then
                        n.ImageUrl = Page.ResolveUrl("~/Docm/images/roleonP.gif")
                        If Not status.Eq("CP") And Not status.Eq("PA") AndAlso StringHelper.InStrTest(txtIdRoleRight.Text, s) Then
                            sEnabled = "ON"
                        End If
                    End If
                    If StringHelper.InStrTest(txtRRIdOwner.Text, s) Then
                        n.ImageUrl = Page.ResolveUrl("~/Docm/images/roleonRR.gif")
                        If Not status.Eq("CP") And Not status.Eq("PA") AndAlso StringHelper.InStrTest(txtIdRoleRight.Text, s) Then
                            sEnabled = "ON"
                        End If
                    End If
                    If StringHelper.InStrTest(txtPRIdOwner.Text, s) Then
                        n.ImageUrl = Page.ResolveUrl("~/Docm/images/roleonPR.gif")
                    End If
                    If StringHelper.InStrTest(txtRNIdOwner.Text, s) Then
                        n.ImageUrl = Page.ResolveUrl("~/Docm/images/roleonRN.gif")
                    End If
                    If StringHelper.InStrTest(txtRIdOwner.Text, s) Then
                        n.ImageUrl = Page.ResolveUrl("~/Docm/images/roleonR.gif")
                    End If
            End Select
            n.Value &= "|" & sEnabled
            If sEnabled = "ON" Then
                n.Style.Add("font-weight", "bold")
            End If
            If n.Nodes.Count > 0 Then
                AddRoleTvwImages(n)
            End If
        Next
    End Sub

    Private Sub InitializeFolderTreeview(ByRef treeView As RadTreeView, ByVal root As Boolean, ByVal incrementalFather As Short?, ByVal nodo As RadTreeNode, ByVal incrementalFocus As Short?)

        Dim tnRoot As RadTreeNode

        If root Then
            tnRoot = New RadTreeNode()
            treeView.Nodes.Clear()
            tnRoot.Text = "Pratica " + DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)
            tnRoot.Style.Add("font-weight", "bold")
            ' al posto di ID ( come nell'originale ) uso Value (node.Value = id.ToString() in uscContatti.vb)
            tnRoot.Value = "0" & PIPE & "P"
            tnRoot.ImageUrl = Page.ResolveUrl("~/Docm/Images/pratica.gif")
            tnRoot.Expanded = True
            treeView.Nodes.Add(tnRoot)
            tnRoot.Selected = True
            nodo = tnRoot
        End If

        Dim docFolderList As IList(Of DocumentFolder) = Facade.DocumentFolderFacade.GetByYearAndNumber(CurrentDocumentYear, CurrentDocumentNumber, incrementalFather)

        If docFolderList.Count > 0 Then
            For Each docFolder As DocumentFolder In docFolderList
                Dim tn As RadTreeNode = New RadTreeNode()
                Dim docCount As Integer
                Dim yniKey As New YearNumberIncrCompositeKey

                With yniKey
                    .Year = CurrentDocumentYear
                    .Number = CurrentDocumentNumber
                    .Incremental = docFolder.Incremental
                End With

                docCount = Facade.DocumentObjectFacade.GetDocumentCountOfDocumentFolder(yniKey)

                If docFolder.Role Is Nothing Then
                    If docFolder.DocumentsRequired.HasValue AndAlso docFolder.DocumentsRequired.Value > 0 Then
                        tn.Text = String.Format("({0}/{1})", docCount, docFolder.DocumentsRequired.Value) & " "
                    Else
                        tn.Text = String.Format("({0})", docCount) & " "
                    End If
                    tn.Text = "<b>" & tn.Text & "</b>" & docFolder.FolderName
                    tn.Value = docFolder.Incremental & PIPE & "F"
                    tn.ImageUrl = Page.ResolveUrl("~/Comm/images/folderclose16.gif")
                    tn.SelectedImageUrl = Page.ResolveUrl("~/Comm/images/folderopen16.gif")
                Else
                    tn.Text = String.Format("({0})", docCount) & " "
                    tn.Text = "<b>" & tn.Text & "</b>" & docFolder.Role.Name
                    tn.Value = docFolder.Incremental & PIPE & "R" & docFolder.Role.Id
                    tn.ImageUrl = Page.ResolveUrl("~/Docm/images/roleon.gif")
                End If

                'Scadenza cartella
                Dim isExpired As Boolean = True
                Dim expiryDescription As String = DocumentFolderFacade.GetExpiryDescription(docFolder, isExpired)
                tn.Text &= If(isExpired, "<b>" & expiryDescription & "</b>", expiryDescription)
                tn.ToolTip &= expiryDescription

                tn.Expanded = True
                nodo.Nodes.Add(tn)

                If incrementalFocus.HasValue AndAlso incrementalFocus = docFolder.Incremental Then
                    tn.Selected = True
                End If

                InitializeFolderTreeview(treeView, False, docFolder.Incremental, tn, incrementalFocus)
            Next
        Else
            Exit Sub
        End If
    End Sub

    ''' <summary> scrive su textbox l'idOwner dello step </summary>
    Private Sub WriteStep(ByVal docToken As DocumentToken, ByVal txtStep As TextBox, ByVal txtIdOwner As TextBox, ByVal account As String)
        If txtStep.Text <> "" Then
            txtStep.Text &= SEP
        End If
        If txtIdOwner.Text <> "" Then
            txtIdOwner.Text &= SEP
        End If
        txtStep.Text &= String.Format("{0}{1}{2}{0}", PIPE, docToken.DocStep, If(docToken.SubStep = 0, "", "." & docToken.SubStep))

        If account <> String.Empty Then
            txtIdOwner.Text &= PIPE & account & PIPE
        Else
            txtIdOwner.Text &= PIPE & docToken.RoleDestination.Id & PIPE
        End If
    End Sub

    Private Function DocumentCheckOn(ByVal p_node As RadTreeNode) As Boolean
        Dim retval As Boolean = False
        Dim nodeValue() As String = p_node.Value.Split("|"c)

        If nodeValue(1).Substring(0, 1).Equals("R") Then
            If nodeValue(2).Equals("ON") Then retval = True
        Else
            If Not p_node.Level.Equals(0) Then retval = DocumentCheckOn(p_node.ParentNode)
        End If

        Return retval
    End Function

#End Region

End Class

