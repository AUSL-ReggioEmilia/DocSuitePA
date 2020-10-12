Imports System.Collections.Generic
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers

Partial Public Class DocmAtti
    Inherits DocmBasePage

#Region "Fields"
    Private _selectedResolution As Resolution
    Private _selectedNodeIncremental As Short?
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

    Private ReadOnly Property SelResolutionId() As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer?)("txtSelId", Nothing)
        End Get
    End Property

    Private ReadOnly Property SelectedNodeIncremental As Short?
        Get
            If SelResolutionId.HasValue Then
                Dim v As String() = Split(Tvw.SelectedNode.Value, "|")
                _selectedNodeIncremental = Short.Parse(v(0))
            Else
                _selectedNodeIncremental = Incremental
            End If
            Return _selectedNodeIncremental
        End Get
    End Property

    Private ReadOnly Property SelectedResolution As Resolution
        Get
            If SelResolutionId.HasValue Then
                _selectedResolution = Facade.ResolutionFacade.GetById(SelResolutionId.Value)
            Else
                If uscResolutionSelect.SelectedResolution IsNot Nothing Then
                    _selectedResolution = uscResolutionSelect.SelectedResolution
                End If
            End If
            Return _selectedResolution
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        InitializeAjax()

        If Not Page.IsPostBack Then
            Initialize()
        End If

        If Action.Eq("Modify") Then
            uscResolutionSelect.InitResolutionTypes()
            uscResolutionSelect.ShowPreviewMode()
        End If

    End Sub

    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles uscResolutionSelect.ResolutionSelected
        SearchResolution()
    End Sub

    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        Dim titolo As String = String.Empty
        Dim fullIncremental As String = String.Empty
        Dim role As Integer
        Dim roleIncremental As Integer

        If SelectedResolution Is Nothing Then
            Throw New DocSuiteException("Nessun atto selezionato per il collegamento alla pratica")
        End If

        If Not SelectedNodeIncremental.HasValue Then
            Throw New DocSuiteException("Nessun incremental selezionato")
        End If

        DocUtil.FncCalcolaPath(role, roleIncremental, fullIncremental, titolo, CurrentDocumentYear, CurrentDocumentNumber, SelectedNodeIncremental.Value, True)

        If role = 0 Then
            AjaxAlert("Nessun settore trovato per l'incremental passato")
            Exit Sub
        End If

        Dim _documentTokens As IList(Of DocumentToken) = Facade.DocumentTokenFacade.DocumentTokenStep(CurrentDocumentYear, CurrentDocumentNumber, role)
        If _documentTokens.Count = 1 Then
            Dim newIncremental As Short = Facade.DocumentObjectFacade.GetMaxId(CurrentDocumentYear, CurrentDocumentNumber)

            Dim documentObject As DocumentObject = New DocumentObject() With {
                .Year = CurrentDocumentYear,
                .Number = CurrentDocumentNumber,
                .Incremental = newIncremental,
                .IncrementalFolder = SelectedNodeIncremental.Value,
                .OrdinalPosition = newIncremental,
                .DocStep = _documentTokens(0).DocStep,
                .SubStep = _documentTokens(0).SubStep,
                .idObjectType = "LR",
                .Reason = txtReason.Text,
                .Note = txtNote.Text,
                .idBiblos = 0,
                .Link = SelectedResolution.CalculatedLink
            }

            Facade.DocumentObjectFacade.Save(documentObject)

            If Request.QueryString("Redirect") <> "" Then
                Dim url As String = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Docm", CurrentDocumentYear, CurrentDocumentNumber))
                Response.Redirect(url)
            Else
                AjaxManager.ResponseScripts.Add("CloseWindow('');")
                AjaxManager.ResponseScripts.Add("parent.parent.RefreshFolderAjax();")
            End If
        End If
    End Sub

    Private Sub btnCancella_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancella.Click
        Dim documentObject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
        Facade.DocumentObjectFacade.UpdateStatus(documentObject)

        RegisterFolderRefreshFullScript()
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnModifica.Click
        Dim documentobject As DocumentObject

        Dim NewIncrementalFolder As Short
        Dim tn As RadTreeNode = uscDocumentFolderProt.Destination.SelectedNode()

        documentobject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))

        If (tn IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(tn.Value)) Then
            NewIncrementalFolder = tn.Value
            If (documentobject IsNot Nothing) AndAlso documentobject.IncrementalFolder <> NewIncrementalFolder Then
                Facade.DocumentObjectFacade.UpdateFolder(documentobject, NewIncrementalFolder)
            End If
        End If

        Facade.DocumentObjectFacade.UpdateDescription(documentobject, Nothing, "", txtReason.Text, txtNote.Text)

        If NewIncrementalFolder = 0 Then
            AjaxManager.ResponseScripts.Add("CloseWindow('" & RefreshDocument & "')")

        Else
            AjaxManager.ResponseScripts.Add("CloseWindow('')")
        End If
        RegisterFolderRefreshScript(NewIncrementalFolder)
    End Sub


    Private Sub Tvw_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles Tvw.NodeClick
        btnInserimento.Enabled = DocumentCheckOn(Tvw.SelectedNode)
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim titolo As String = String.Empty
        Dim fullIncremental As String = String.Empty
        Dim role As Integer
        Dim roleIncremental As Integer

        btnInserimento.Visible = False
        pnlCartella.Visible = False
        btnModifica.Visible = False
        btnCancella.Visible = False

        If Incremental Is Nothing Then
            tbFolders.Visible = True
            uscResolutionSelect.Visible = False
            RoleTvw()
        End If

        DocUtil.FncCalcolaPath(role, roleIncremental, fullIncremental, titolo, CurrentDocumentYear, CurrentDocumentNumber, Incremental.GetValueOrDefault(0), True)

        idRole.Text = role.ToString()

        'Inizializza usercontrol selezione atto
        uscResolutionSelect.VisibleYearNumberPanel = Facade.ResolutionFacade.IsManagedProperty("Number", ResolutionType.IdentifierDetermina)
        uscResolutionSelect.VisibleServiceNumberPanel = Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", ResolutionType.IdentifierDetermina)

        pnlCartella.Visible = False
        btnInserimento.Visible = False
        btnModifica.Visible = False
        btnCancella.Visible = False

        Dim documentObject As DocumentObject = Nothing
        Select Case Action
            Case "Insert"
                Title = "Inserimento Collegamento Atti"
                uscResolutionSelect.TextBoxYear.Text = CurrentDocumentYear.ToString()
                btnInserimento.Visible = True
            Case "Modify"
                If Add = "ON" Then
                    Title = "Modifica Collegamento Atto"
                Else
                    Title = "Visualizza Collegamento Atto"
                End If
                documentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
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
                uscDocumentFolderProt.Year = CurrentDocumentYear
                uscDocumentFolderProt.Number = CurrentDocumentNumber
                uscDocumentFolderProt.Incremental = Incremental.Value
                uscDocumentFolderProt.Document = documentObject.Document
                uscDocumentFolderProt.IncrementalFolder = documentObject.IncrementalFolder
        End Select

        If Not String.IsNullOrEmpty(titolo) Then
            Title &= " - Cartella: " & titolo
        End If


    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, btnModifica)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancella, btnCancella)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnInserimento)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, uscDocumentFolderProt)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscResolutionSelect, btnInserimento)
    End Sub

    Private Sub BindData(ByRef documentObject As DocumentObject)
        Dim link As String() = documentObject.Link.Split("|"c)
        uscResolutionSelect.TextBoxIdResolution.Text = link(0)
        txtReason.Text = documentObject.Reason
        txtNote.Text = documentObject.Note
    End Sub

    Private Sub SearchResolution()
        btnInserimento.Enabled = False

        'verifica se l'utente ha immesso tutti i dati necessari
        Dim incompleteData As Boolean
        If Not uscResolutionSelect.CheckInputData(incompleteData) Then
            Exit Sub
        End If

        'crea descrizione dell'atto
        Dim description As String = uscResolutionSelect.GetResolutionDescription(incompleteData)

        'verifica se l'atto esiste
        If uscResolutionSelect.SelectedResolution Is Nothing Then
            AjaxAlert(description & uscResolutionSelect.SelectedResolutionType & " inesistente")
            Exit Sub
        End If

        'verifica se collegamento è già stato inserito
        Dim sLink As String = uscResolutionSelect.SelectedResolution.CalculatedLink
        If Action.Eq("Insert") Then
            If DocumentUtil.DocLinkVerify(Me, sLink, Incremental.Value, CurrentDocumentYear, CurrentDocumentNumber) Then
                AjaxAlert("Collegamento " & description & "Già inserito nella Pratica")
                Exit Sub
            End If
        End If

        'Verifica sicurezza
        If Not ResolutionRights.CheckRight(uscResolutionSelect.SelectedResolution, DocSuiteContext.Current.DocumentEnv.ReslSecurity) Then
            AjaxAlert(description & "Mancano i diritti necessari")
            Exit Sub
        End If

        btnInserimento.Enabled = True
    End Sub


    Private Sub RoleTvw()
        ' Controlla i diritti
        Dim roleRightsList As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DossierRoleRightPositions.Enabled, True)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If txtIdRoleRight.Text <> "" Then
                    txtIdRoleRight.Text &= ","c
                End If
                txtIdRoleRight.Text += ("|"c & role.Id & "|"c)
            Next
        End If
        roleRightsList.Clear()
        roleRightsList = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DossierRoleRightPositions.Workflow, True)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If txtIdRoleRightW.Text <> "" Then
                    txtIdRoleRightW.Text &= ","c
                End If
                txtIdRoleRightW.Text += ("|"c & role.Id & "|"c)
            Next
        End If
        roleRightsList.Clear()
        roleRightsList = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DossierRoleRightPositions.Manager, True)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If txtIdRoleRightM.Text <> "" Then
                    txtIdRoleRightM.Text &= ","c
                End If
                txtIdRoleRightM.Text += ("|"c & role.Id & "|"c)
            Next
        End If

        '---prese in carico
        Dim documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PA", "PM", "PC", "PT", "PR"})
        If documentTokenList.Count > 0 Then

            For Each docToken As DocumentToken In documentTokenList
                Select Case docToken.IsActive
                    Case 1
                        WriteStep(docToken, txtPStep, txtPIdOwner, "")
                    Case 2
                        If docToken.DocumentTabToken.Id = "PT" Then
                            WriteStep(docToken, txtRRStep, txtRRIdOwner, "")
                        Else
                            WriteStep(docToken, txtPRStep, txtPRIdOwner, "")
                        End If
                End Select

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
                    Dim s As String = "|"c & Mid(a(1), 2) & "|"c

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

    Private Sub WriteStep(ByVal docToken As DocumentToken, ByVal txtStep As TextBox, ByVal txtIdOwner As TextBox, ByVal account As String)
        If txtStep.Text <> "" Then
            txtStep.Text &= ","c
        End If
        If txtIdOwner.Text <> "" Then
            txtIdOwner.Text &= ","c
        End If
        txtStep.Text &= String.Format("{0}{1}{2}{0}", "|"c, docToken.DocStep, If(docToken.SubStep = 0, "", "." & docToken.SubStep))

        If account <> String.Empty Then
            txtIdOwner.Text &= "|"c & account & "|"c
        Else
            txtIdOwner.Text &= "|"c & docToken.RoleDestination.Id & "|"c
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


    Private Sub InitializeFolderTreeview(ByRef treeView As RadTreeView, ByVal root As Boolean, ByVal incrementalFather As Short?, ByVal nodo As RadTreeNode, ByVal incrementalFocus As Short?)

        Dim tnRoot As RadTreeNode

        If root Then
            tnRoot = New RadTreeNode()
            treeView.Nodes.Clear()
            tnRoot.Text = "Pratica " + DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)
            tnRoot.Style.Add("font-weight", "bold")
            ' al posto di ID ( come nell'originale ) uso Value (node.Value = id.ToString() in uscContatti.vb)
            tnRoot.Value = "0" & "|" & "P"
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
                    tn.Value = docFolder.Incremental & "|" & "F"
                    tn.ImageUrl = Page.ResolveUrl("~/Comm/images/folderclose16.gif")
                    tn.SelectedImageUrl = Page.ResolveUrl("~/Comm/images/folderopen16.gif")
                Else
                    tn.Text = String.Format("({0})", docCount) & " "
                    tn.Text = "<b>" & tn.Text & "</b>" & docFolder.Role.Name
                    tn.Value = docFolder.Incremental & "|" & "R" & docFolder.Role.Id
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
#End Region

End Class