Imports System.Text
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Web
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class DocmVisualizza
    Inherits DocmBasePage
    Implements ISendMail

#Region " Fields "

    Const PIPE As Char = "|"c
    Const SEP As Char = ","c
    Private _roleVerifica As Boolean?
    Private _roleVerificaCC As Boolean = False

#End Region

#Region " Properties "

    ''' <summary> Identificativo Cartella </summary>
    Public ReadOnly Property IncrementalFolder() As Short?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short?)("IncrementalFolder", Nothing)
        End Get
    End Property

    Private Property SelectedIncremental As Short?
        Get
            Return DirectCast(Session("SommarioSelectedNode"), Short?)
        End Get
        Set(value As Short?)
            Session("SommarioSelectedNode") = value
        End Set
    End Property

    Public ReadOnly Property UserSecurityEnabled() As Boolean
        Get
            Return String.IsNullOrEmpty(Request.QueryString("UserSecurity"))
        End Get
    End Property

    ''' <summary> Diritti sul container </summary>
    Private Property RoleVerifica() As Boolean
        Get
            If Not _roleVerifica.HasValue Then
                _roleVerifica = CommonShared.UserDocumentCheckRight(DocumentContainerRightPositions.Preview)
            End If
            Return _roleVerifica.Value
        End Get
        Set(ByVal value As Boolean)
            _roleVerifica = value
        End Set
    End Property

    Private Property RoleVerificaCC() As Boolean
        Get
            Return _roleVerificaCC
        End Get
        Set(ByVal value As Boolean)
            _roleVerificaCC = value
        End Set
    End Property

    Public ReadOnly Property CurrentYear() As String
        Get
            Return Request.QueryString("txtSelYear")
        End Get
    End Property

    Public ReadOnly Property CurrentNumber() As String
        Get
            Return Request.QueryString("txtSelNumber")
        End Get
    End Property

    Public ReadOnly Property SelectedId() As String
        Get
            Return Request.QueryString("txtSelId")
        End Get
    End Property

    Private ReadOnly Property Heading As String
        Get
            Return String.Format("Pratica n. {0}/{1} del {2:dd/MM/yyyy}", CurrentDocumentYear, CurrentDocumentNumber, CurrentDocument.StartDate)
        End Get
    End Property

    Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
        Get
            Return CommonUtil.GetInstance().UserDescription
        End Get
    End Property

    Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
        Get
            Return Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True)
        End Get
    End Property

    Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return String.Format("{0} {1} - {2}", DocSuiteContext.ProductName, Heading, CurrentDocument.DocumentObject)
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Return String.Format("<b>{0} - Gestione Documentale</b>{6}{6}Allego la {1}{6}Oggetto: {2}{6}{6}<a href=""{3}?Tipo=Docm&Azione=Apri&Anno={4}&Numero={5}"">{1}</a>",
                                 DocSuiteContext.ProductName,
                                 Heading,
                                 StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(CurrentDocument.DocumentObject)),
                                 DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                 CurrentDocumentYear,
                                 CurrentDocumentNumber,
                                 WebHelper.Br)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        CheckAuthorizations()
        If DocumentEnv.EnableButtonNuovaPratica Then
            cmdNuovo.Visible = True
        End If

        If Not Page.IsPostBack Then
            If Not Page.IsCallback AndAlso Not CommonUtil.VerifyChkQueryString(Request.QueryString, False) Then
                Throw New DocSuiteException("Visualizzazione pratica", "Verifica di sicurezza fallita, parametro mancante o non valido.")
            End If

            toolbarDocument.Visible = DocSuiteContext.IsFullApplication
            toolbarFolder.Visible = DocSuiteContext.IsFullApplication
            toolbarWorkflow.Visible = DocSuiteContext.IsFullApplication
            ' Status documento
            If CurrentDocument.Status IsNot Nothing AndAlso (CurrentDocument.Status.Id.Eq("CP") OrElse CurrentDocument.Status.Id.Eq("PA")) Then
                toolbarFolder.Visible = False
                toolbarWorkflow.Visible = False
            End If

            Title = "Pratiche - Visualizza " & CurrentDocument.Id.ToString()
            Facade.DocumentLogFacade.Insert(CurrentDocument.Year, CurrentDocument.Number, "DS", String.Empty)
            paneDocument.ContentUrl = String.Format("../Docm/DocmInfo.aspx?Year={0}&Number={1}&Type=Docm", CurrentDocumentYear, CurrentDocumentNumber)

            If IncrementalFolder.HasValue Then
                SelectedIncremental = IncrementalFolder
            End If

            Initialize()
        End If

        Textboxes(TextboxAction.Hide)
        InitializeAjax()

    End Sub
    Private Sub cmdNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdNuovo.Click

        Dim params As String = CommonShared.AppendSecurityCheck("Type=Docm&Action=Insert")
        Dim url As String = String.Concat("../Docm/DocmInserimento.aspx?", params)
        Response.Redirect(url)

    End Sub
    Protected Sub Tvw_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles tvDocument.NodeClick
        SetButtonsFolder()
    End Sub

    Protected Sub ToolBars_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles toolbarInfo.ButtonClick, toolbarFolder.ButtonClick, toolbarDocument.ButtonClick, toolbarWorkflow.ButtonClick
        Dim button As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case button.CommandName
            Case "refresh"
                Initialize()
                AjaxManager.ResponseScripts.Add(String.Format("SetDocumentPane('{0}');", paneDocument.ContentUrl))

            Case "info", "token", "user", "log", "checkout"
                Dim baseParameters As String = CommonShared.AppendSecurityCheck(String.Format("Type=Docm&Year={0}&Number={1}", CurrentDocumentYear, CurrentDocumentNumber))
                AjaxManager.ResponseScripts.Add(String.Format("SetDocumentPane('{0}?{1}');", button.CommandArgument, baseParameters))

            Case "legenda"
                AjaxManager.ResponseScripts.Add(String.Format("return OpenWindow('../Docm/DocmSommarioL.aspx','{0}',350,200,'Titolo=Pratica Legenda&Type=Docm');", windowDocmSommarioLegenda.ClientID))

            Case "workflow", "step", "request", "retrieval", "take", "restitution", "assignment", "authorize", "modify", "publication", "lock", "cancel", "reopen"
                Dim params As New StringBuilder
                params.AppendFormat("Type=Docm&Year={0}&Number={1}", CurrentDocumentYear, CurrentDocumentNumber)
                params.AppendFormat("&{0}", Textboxes(TextboxAction.GetContent))
                Select Case button.CommandName
                    Case "publication"
                        params.Append("&Publication=1")
                    Case "lock"
                        params.Append("&Action=Close")
                    Case "cancel"
                        params.Append("&Action=Cancel")
                    Case "reopen"
                        params.Append("&Action=ReOpen")
                End Select
                AjaxManager.ResponseScripts.Add(String.Format("SetDocumentPane('{0}?{1}');", button.CommandArgument, CommonShared.AppendSecurityCheck(params.ToString())))

            Case "send"
                FileLogger.Info(LoggerName, String.Format("Apertura invio mail da utente [{0}] in pagina [{1}].", DocSuiteContext.Current.User.FullUserName, Request.RawUrl))

            Case "add", "rename", "delete"
                Dim v As String() = tvDocument.SelectedNode.Value.Split("|"c)
                Dim param As String = String.Format("Type=Docm&Year={0}&Number={1}&Incremental={2}&Action={3}", CurrentDocumentYear, CurrentDocumentNumber, v(0), button.Value)

                AjaxManager.ResponseScripts.Clear()
                AjaxManager.ResponseScripts.Add(String.Format("return OpenWindow('{0}','{1}',{2},{3},'{4}');", button.CommandArgument, windowDocmGestioneCartella.ClientID, 600, 370, param))

            Case Else
                Throw New DocSuiteException("Visualizzazione pratica", "Caso non previsto")

        End Select
    End Sub

    Private Sub AjaxRequest_Refresh(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim args As String() = e.Argument.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)
        If args.Length > 1 Then
            SelectedIncremental = Short.Parse(args(1))
        End If

        Initialize()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        InitializeTrees()

        If Not RoleVerifica AndAlso Not RoleVerificaCC Then
            Throw New DocSuiteException("Visualizzazione pratica", String.Format("Verificare se si dispone l'autorizzazione per visualizzare la pratica {0}.", DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)))
        End If

        DirectCast(toolbarInfo.FindButtonByCommandName("send"), RadToolBarButton).Visible = True

        If Not DocSuiteContext.IsFullApplication Then
            Exit Sub
        End If
        If (CurrentDocument.Status IsNot Nothing) AndAlso (CurrentDocument.Status.Id.Eq("CP") OrElse CurrentDocument.Status.Id.Eq("PA")) Then
            Exit Sub
        End If

        'contenitore senza workflow
        If txtPStep.Text.Eq("|1|") Then
            If CommonShared.UserDocumentCheckRight(DocumentContainerRightPositions.Workflow) Then
                Exit Sub
            End If
        End If

        Dim tmp As String = txtIdRoleRightW.Text.Replace(PIPE, "")
        Dim roles As String() = Split(tmp, ",")

        If txtIdRoleRightW.Text <> "" Then
            DirectCast(toolbarWorkflow.FindButtonByCommandName("request"), RadToolBarButton).Visible = StringHelper.InStrTest(txtIdRoleRightW.Text, txtPIdOwner.Text)
            DirectCast(toolbarWorkflow.FindButtonByCommandName("retrieval"), RadToolBarButton).Visible = StringHelper.InStrTest(txtIdRoleRightW.Text, txtPRIdOwner.Text)

            Dim documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PT"}, roles)
            DirectCast(toolbarWorkflow.FindButtonByCommandName("restitution"), RadToolBarButton).Visible = (documentTokenList.Count > 0)
        End If

        'token presa in carico
        Dim takeVisibility As Boolean
        Select Case DocSuiteContext.Current.DocumentEnv.WorkFlowCapture
            Case "W"
                If txtIdRoleRightW.Text <> "" Then
                    Dim documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RP", "RR"}, roles)
                    takeVisibility = (documentTokenList.Count > 0)
                End If
            Case "M"
                If txtIdRoleRightM.Text <> "" Then
                    Dim documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RP", "RR"}, roles)
                    takeVisibility = (documentTokenList.Count > 0)
                End If
        End Select
        DirectCast(toolbarWorkflow.FindButtonByCommandName("take"), RadToolBarButton).Visible = takeVisibility

        Dim assignmentVisible As Boolean
        If Not String.IsNullOrEmpty(txtIdRoleRightM.Text) Then
            assignmentVisible = StringHelper.InStrTest(txtIdRoleRightM.Text, txtPIdOwner.Text)
            If Not assignmentVisible Then
                Dim documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PT"}, roles)
                assignmentVisible = (documentTokenList.Count > 0)
            End If
        End If
        DirectCast(toolbarWorkflow.FindButtonByCommandName("assignment"), RadToolBarButton).Visible = assignmentVisible

        SetButtonsDocument()
        SetButtonsFolder()

    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest_Refresh

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, toolbarDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, toolbarFolder)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, toolbarWorkflow)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tvDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tvSettoriCC)
        AjaxManager.AjaxSettings.AddAjaxSetting(tvDocument, toolbarDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(tvDocument, toolbarFolder)
        AjaxManager.AjaxSettings.AddAjaxSetting(tvDocument, toolbarWorkflow)

        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarDocument, tvDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarDocument, tvSettoriCC)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarDocument, toolbarDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarDocument, toolbarFolder)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarDocument, toolbarWorkflow)

        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarInfo, toolbarInfo)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarDocument, toolbarDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarFolder, toolbarFolder)
        AjaxManager.AjaxSettings.AddAjaxSetting(toolbarWorkflow, toolbarWorkflow)
    End Sub

    ''' <summary> Inglobato qui tutta la porcheria di autorizzazione </summary>
    Private Sub CheckAuthorizations()
        If CurrentDocument Is Nothing Then
            Throw New DocSuiteException("Visualizzazione pratica", String.Format("Impossibile trovare pratica [{0}]", DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)))
        End If

        '---sicurezza user
        Dim roleFacade As New RoleFacade()

        Dim enabledRoles As IList(Of Role) = roleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Enabled, True)
        Dim txtAll As String = String.Empty
        For Each role As Role In enabledRoles
            If txtAll <> "" Then
                txtAll += SEP
            End If
            txtAll += (PIPE & role.Id & PIPE)
        Next
        enabledRoles.Clear()

        Dim workflowRoles As IList(Of Role) = roleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Workflow, True)
        Dim txtWorkFlow As String = String.Empty
        For Each role As Role In workflowRoles
            If txtWorkFlow <> "" Then txtWorkFlow += SEP
            txtWorkFlow += (PIPE & role.Id & PIPE)
        Next
        workflowRoles.Clear()

        Dim managerRoles As IList(Of Role) = roleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Manager, True)
        Dim txtManager As String = String.Empty
        For Each role As Role In managerRoles
            If txtManager <> "" Then
                txtManager += SEP
            End If
            txtManager += (PIPE & role.Id & PIPE)
        Next

        ' Verifica Contenitori
        Dim containerFacade As New ContainerFacade()

        Dim previewContainers As IList(Of Container) = containerFacade.GetSecurityGroupsContainerRights(Type, Facade.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName), 1, DocumentContainerRightPositions.Preview, CurrentDocument.Container.Id)

        Dim enableView As Boolean = previewContainers.Count > 0

        ''PA','PM','PC','PT','PR' documenttokenrolePSec
        ''RC','RP','RR' RSec
        'manager prese e richieste
        If Not String.IsNullOrEmpty(txtManager) Then

            Dim sDestRoles() As String
            Dim sTmp As String

            sTmp = txtManager.Replace("|", "")
            sDestRoles = sTmp.Split(",".ToCharArray())

            Dim docTokenListP As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenByTokenType(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PA", "PM", "PC", "PT", "PR"}, sDestRoles)
            If docTokenListP.Count Then
                enableView = True
            End If

            Dim docTokenListR As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenByTokenType(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RP", "RR"}, sDestRoles)
            If docTokenListR.Count Then
                enableView = True
            End If
        End If

        'CC
        If Not enableView AndAlso Not String.IsNullOrEmpty(txtAll) Then
            Dim documentTokenListCc As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True)
            If documentTokenListCc.Count Then
                For Each docToken As DocumentToken In documentTokenListCc
                    If Not enableView Then
                        If InStr(txtAll, "|" & docToken.RoleDestination.Id & "|") <> 0 Then
                            enableView = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End If

        'utenti prese in carico
        If enableView AndAlso Not String.IsNullOrEmpty(txtAll) Then

            Dim sDestRoles() As String
            Dim sTmp As String

            sTmp = txtAll.Replace("|", "")
            sDestRoles = sTmp.Split(",".ToCharArray())

            Dim docTokenListP As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenByTokenType(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PA", "PM", "PC", "PT", "PR"}, sDestRoles)

            If docTokenListP.Count > 0 Then
                For Each docToken As DocumentToken In docTokenListP
                    Dim documentTokenUserFacade As New DocumentTokenUserFacade()
                    Dim documentTokenUsersList As IList(Of DocumentTokenUser) = documentTokenUserFacade.GetDocumentTokenUserList(CurrentDocumentYear, CurrentDocumentNumber, docToken, , , , , True)
                    If documentTokenUsersList.Count > 0 Then
                        Dim documentTokenUserList As IList(Of DocumentTokenUser) = documentTokenUserFacade.GetDocumentTokenUserList(CurrentDocumentYear, CurrentDocumentNumber, docToken, DocSuiteContext.Current.User.UserName, , , , True)
                        If documentTokenUserList.Count > 0 Then
                            enableView = True
                            Exit For
                        End If
                    Else
                        enableView = True
                        Exit For
                    End If
                Next
            End If
        End If

        'utenti richieste prese in carico
        If Not enableView AndAlso DocSuiteContext.Current.DocumentEnv.WorkFlowCapture = "W" AndAlso Not String.IsNullOrEmpty(txtWorkFlow) Then

            Dim sDestRoles() As String
            Dim sTmp As String

            sTmp = txtWorkFlow.Replace("|", "")
            sDestRoles = sTmp.Split(",".ToCharArray())

            Dim docTokenListR As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenByTokenType(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RP", "RR"}, sDestRoles)

            If docTokenListR.Count > 0 Then
                For Each docToken As DocumentToken In docTokenListR
                    Dim documentTokenUserFacade As New DocumentTokenUserFacade()
                    Dim documentTokenUsersList As IList(Of DocumentTokenUser) = documentTokenUserFacade.GetDocumentTokenUserList(CurrentDocumentYear, CurrentDocumentNumber, docToken, , , , , True)
                    If documentTokenUsersList.Count > 0 Then
                        Dim documentTokenUserList As IList(Of DocumentTokenUser) = documentTokenUserFacade.GetDocumentTokenUserList(CurrentDocumentYear, CurrentDocumentNumber, docToken, DocSuiteContext.Current.User.UserName, , , , True)
                        If documentTokenUserList.Count > 0 Then
                            enableView = True
                            Exit For
                        End If
                    Else
                        enableView = True
                        Exit For
                    End If
                Next
            End If
        End If

        'messaggio in chiusura
        If Not enableView Then
            Throw New DocSuiteException("Visualizzazione pratica", "Impossibile visualizzare la Pratica richiesta. Verificare se si dispone di sufficienti autorizzazioni.")
        End If
    End Sub

    Private Function Textboxes(ByVal [do] As TextboxAction) As String
        Dim retStr As New StringBuilder

        For Each obj As Object In misteryBox.Controls
            If Not (TypeOf (obj) Is TextBox) Then
                Continue For
            End If
            ' TODO: distruggere questo orrore
            If Not Left(obj.ID, 3).Eq("TXT") Then
                Continue For
            End If

            Dim txt As TextBox = DirectCast(obj, TextBox)
            Select Case [do]
                Case TextboxAction.Hide
                    txt.CssClass = "hiddenField"
                Case TextboxAction.Clean
                    txt.Text = ""
                Case TextboxAction.GetContent
                    If Not String.IsNullOrEmpty(txt.Text) Then
                        If retStr.Length > 0 Then
                            retStr.Append("&")
                        End If
                        retStr.AppendFormat("{0}={1}", txt.ID, txt.Text)
                    End If
            End Select
        Next
        Return retStr.ToString()
    End Function

    Private Sub SetButtonsFolder()
        DirectCast(toolbarFolder.FindButtonByCommandName("add"), RadToolBarButton).Visible = False
        DirectCast(toolbarFolder.FindButtonByCommandName("rename"), RadToolBarButton).Visible = False
        DirectCast(toolbarFolder.FindButtonByCommandName("delete"), RadToolBarButton).Visible = False

        Dim docVersioning As DocumentVersioning = Facade.DocumentVersioningFacade.GetDocumentVersion(CurrentDocumentYear, CurrentDocumentNumber, 0, "O")
        DirectCast(toolbarFolder.FindButtonByCommandName("checkout"), RadToolBarButton).Visible = docVersioning IsNot Nothing

        If tvDocument.SelectedNode Is Nothing Then
            Exit Sub
        End If

        Dim v As String() = tvDocument.SelectedNode.Value.Split("|"c)
        Dim incremental As String = v(0)
        Dim linkType As String = v(1)

        Dim url As New StringBuilder
        url.AppendFormat("../Docm/DocmDocumenti.aspx?Type=Docm&Year={0}&Number={1}", CurrentDocumentYear, CurrentDocumentNumber)
        If Not incremental.Eq("0") Then
            SelectedIncremental = incremental
            url.AppendFormat("&Incremental={0}", incremental)
        End If

        If DocumentCheckOn(tvDocument.SelectedNode) Then
            Select Case Left(linkType, 1)
                Case "P"
                Case "R"
                    DirectCast(toolbarFolder.FindButtonByCommandName("add"), RadToolBarButton).Visible = True
                Case "F"
                    DirectCast(toolbarFolder.FindButtonByCommandName("add"), RadToolBarButton).Visible = True
                    DirectCast(toolbarFolder.FindButtonByCommandName("rename"), RadToolBarButton).Visible = True
                    DirectCast(toolbarFolder.FindButtonByCommandName("delete"), RadToolBarButton).Visible = True
            End Select

            url.Append("&Add=ON")
        Else
            url.Append("&Add=OFF")
        End If

        If Not String.IsNullOrEmpty(CurrentYear) And Not String.IsNullOrEmpty(CurrentNumber) Then
            url.AppendFormat("&txtSelYear{0}&txtSelNumber={1}", CurrentYear, CurrentNumber)
        End If

        If Not String.IsNullOrEmpty(SelectedId) Then
            url.AppendFormat("&txtSelId={0}", SelectedId)
        End If

        AjaxManager.ResponseScripts.Add(String.Format("SetDocumentPane('{0}');", url.ToString()))
    End Sub

    Private Function DocumentCheckOn(ByVal node As RadTreeNode) As Boolean
        Dim v As Object = Split(node.Value, "|")
        If Left(v(1), 1) = "R" Then
            If v(2) = "ON" Then
                Return True
            End If
        Else
            If node.Level <> 0 Then
                Return DocumentCheckOn(node.ParentNode)
            End If
        End If
        Return False
    End Function

    Private Sub SetButtonsDocument()
        If Not DocSuiteContext.IsFullApplication Then
            Exit Sub
        End If

        DirectCast(toolbarInfo.FindButtonByCommandName("log"), RadToolBarButton).Visible = DocSuiteContext.Current.DocumentEnv.IsEnvLogEnabled AndAlso StringHelper.InStrTest(txtIdRoleRightM.Text, txtPIdOwner.Text)

        For Each item As RadToolBarItem In toolbarDocument.GetGroupButtons("document")
            item.Visible = False
        Next

        Dim status As String = String.Empty
        If CurrentDocument.Status IsNot Nothing Then
            status = CurrentDocument.Status.Id
        End If

        Select Case status
            Case "CP", "PA"
                DirectCast(toolbarDocument.FindButtonByCommandName("reopen"), RadToolBarButton).Visible = StringHelper.InStrTest(txtIdRoleRightW.Text, txtPIdOwner.Text) And (CurrentDocument.Status Is Nothing Or Not CurrentDocument.Status.Id.Eq("PA"))

            Case "AR"
                toolbarDocument.Visible = False
                toolbarFolder.Visible = False
                toolbarWorkflow.Visible = False

            Case Else
                If CommonShared.UserDocumentCheckRight(DocumentContainerRightPositions.Modify) Then
                    If txtPStep.Text.Eq("|1|") Then
                        If StringHelper.InStrTest(txtIdRoleRight.Text, txtPIdOwner.Text) Then
                            DirectCast(toolbarDocument.FindButtonByCommandName("modify"), RadToolBarButton).Visible = True
                            DirectCast(toolbarDocument.FindButtonByCommandName("publication"), RadToolBarButton).Visible = False
                        End If
                    Else
                        DirectCast(toolbarDocument.FindButtonByCommandName("modify"), RadToolBarButton).Visible = False
                        If RoleVerifica Then
                            DirectCast(toolbarDocument.FindButtonByCommandName("publication"), RadToolBarButton).Visible = DocSuiteContext.Current.DocumentEnv.IsPubblicationEnabled
                        End If
                    End If
                End If
                DirectCast(toolbarDocument.FindButtonByCommandName("authorize"), RadToolBarButton).Visible = StringHelper.InStrTest(txtIdRoleRightW.Text, txtPIdOwner.Text)
                DirectCast(toolbarDocument.FindButtonByCommandName("lock"), RadToolBarButton).Visible = StringHelper.InStrTest(txtIdRoleRightW.Text, txtPIdOwner.Text)
                DirectCast(toolbarDocument.FindButtonByCommandName("cancel"), RadToolBarButton).Visible = StringHelper.InStrTest(txtIdRoleRightW.Text, txtPIdOwner.Text)

        End Select
    End Sub

    Private Sub InitializeTrees()
        Textboxes(TextboxAction.Clean)

        ' Controlla diritti
        Dim roleRightsList As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Enabled, True)
        If roleRightsList.Count > 0 Then
            Dim roles As New StringBuilder
            For Each role As Role In roleRightsList
                If roles.Length > 0 Then
                    roles.Append(SEP)
                End If
                roles.AppendFormat("{0}{1}{0}", PIPE, role.Id)
            Next
            txtIdRoleRight.Text = roles.ToString()
        End If
        roleRightsList.Clear()

        roleRightsList = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Workflow, True)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If (IsNumeric(role.Id)) Then
                    If txtIdRoleRightW.Text <> "" Then txtIdRoleRightW.Text &= SEP
                    txtIdRoleRightW.Text += (PIPE & role.Id & PIPE)
                End If
            Next
        End If
        roleRightsList.Clear()

        roleRightsList = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, DocumentRoleRightPositions.Manager, True)

        If roleRightsList.Count > 0 Then
            For Each role As Role In roleRightsList
                If (IsNumeric(role.Id)) Then
                    If txtIdRoleRightM.Text <> "" Then txtIdRoleRightM.Text &= SEP
                    txtIdRoleRightM.Text += (PIPE & role.Id & PIPE)
                End If
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
        Dim documentTokenListCc As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber)
        If documentTokenListCc.Count > 0 Then
            For Each docToken As DocumentToken In documentTokenListCc
                WriteStep(docToken, txtCCStep, txtCCidOwner, "")
            Next
        End If

        Dim root As New RadTreeNode()
        root.Text = "Pratica " & DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber)
        root.Style.Add("font-weight", "bold")
        root.Value = "0" & PIPE & "P"
        root.ImageUrl = Page.ResolveUrl("~/Docm/Images/pratica.gif")
        root.Expanded = True

        InitializeFolderTreeview(Nothing, root)
        tvDocument.Nodes.Clear()
        tvDocument.Nodes.Add(root)

        AddRoleTvwImages(tvDocument.Nodes(0))
        RoleCc()
    End Sub

    Private Sub RoleCc()
        tvSettoriCC.Nodes.Clear()

        Dim documentTokenListCc As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True, , 1)
        If documentTokenListCc.IsNullOrEmpty() Then
            Exit Sub
        End If

        Dim tn As New RadTreeNode
        tn.Text = "Settori in Copia Conoscenza"
        tn.Expanded = True
        tn.ImageUrl = "~/Docm/images/RoleOn.gif"
        tn.Style.Add("font-weight", "bold")
        tvSettoriCC.Nodes.Add(tn)

        For Each docToken As DocumentToken In documentTokenListCc
            WebUtils.ObjTreeViewRoleAdd(tvSettoriCC, Nothing, docToken.RoleDestination, True, True, True, "gray", False, False)
            Dim s As String = PIPE & docToken.RoleDestination.Id & PIPE
            If StringHelper.InStrTest(txtIdRoleRight.Text, s) Then
                RoleVerificaCC = True
            End If
        Next

    End Sub

    Private Sub AddRoleTvwImages(ByVal nodo As RadTreeNode)

        For Each n As RadTreeNode In nodo.Nodes
            Dim sEnabled As String = "OFF"
            Dim a As String() = Split(n.Value.ToString(), "|")
            If Left(a(1), 1) = "R" Then
                Dim s As String = PIPE & Mid(a(1), 2) & PIPE

                Dim status As String = String.Empty
                If CurrentDocument.Status IsNot Nothing Then
                    status = CurrentDocument.Status.Id
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
                If StringHelper.InStrTest(txtIdRoleRight.Text, s) Then
                    RoleVerifica = True
                End If
            End If
            n.Value &= "|" & sEnabled
            If sEnabled = "ON" Then
                n.Style.Add("font-weight", "bold")
            End If
            If n.Nodes.Count > 0 Then
                AddRoleTvwImages(n)
            End If
        Next
    End Sub

    Private Sub InitializeFolderTreeview(ByVal incrementalFather As Short?, ByVal father As RadTreeNode)
        Dim docFolderList As IList(Of DocumentFolder) = Facade.DocumentFolderFacade.GetByYearAndNumber(CurrentDocumentYear, CurrentDocumentNumber, incrementalFather)
        If docFolderList.Count <= 0 Then
            Exit Sub
        End If

        For Each docFolder As DocumentFolder In docFolderList
            Dim docCount As Integer = Facade.DocumentObjectFacade.GetDocumentCountOfDocumentFolder(docFolder.Id)

            Dim node As New RadTreeNode()
            If docFolder.Role Is Nothing Then
                If docFolder.DocumentsRequired.HasValue AndAlso docFolder.DocumentsRequired.Value > 0 Then
                    node.Text = String.Format("({0}/{1})", docCount, docFolder.DocumentsRequired.Value) & " "
                Else
                    node.Text = String.Format("({0})", docCount) & " "
                End If
                node.Text = String.Format("<b>{0}</b>{1}", node.Text, docFolder.FolderName)
                node.Value = String.Format("{0}{1}F", docFolder.Incremental, PIPE)
                node.ImageUrl = Page.ResolveUrl("~/Comm/images/folderclose16.gif")
                node.SelectedImageUrl = Page.ResolveUrl("~/Comm/images/folderopen16.gif")
            Else
                node.Text = String.Format("({0})", docCount) & " "
                node.Text = String.Format("<b>{0}</b>{1}", node.Text, docFolder.Role.Name)
                node.Value = String.Format("{0}{1}R{2}", docFolder.Incremental, PIPE, docFolder.Role.Id)
                node.ImageUrl = Page.ResolveUrl("~/Docm/images/roleon.gif")
            End If

            'Scadenza cartella
            Dim isExpired As Boolean = True
            Dim expiryDescription As String = DocumentFolderFacade.GetExpiryDescription(docFolder, isExpired)
            node.Text &= If(isExpired, String.Format("<b>{0}</b>", expiryDescription), expiryDescription)
            node.ToolTip &= expiryDescription

            node.Expanded = True

            If SelectedIncremental.HasValue AndAlso SelectedIncremental.Value = docFolder.Incremental Then
                node.Selected = True
            End If

            InitializeFolderTreeview(docFolder.Incremental, node)
            father.Nodes.Add(node)
        Next
    End Sub

    ''' <summary> scrive su textbox l'idOwner dello step </summary>
    Private Sub WriteStep(ByVal docToken As DocumentToken, ByVal sStep As TextBox, ByVal idOwner As TextBox, ByVal account As String)
        If sStep.Text <> "" Then
            sStep.Text &= SEP
        End If
        If idOwner.Text <> "" Then
            idOwner.Text &= SEP
        End If
        sStep.Text &= PIPE & docToken.DocStep & If(docToken.SubStep = 0, "", "." & docToken.SubStep) & PIPE

        If account <> String.Empty Then
            idOwner.Text &= PIPE & account & PIPE
        Else
            idOwner.Text &= PIPE & docToken.RoleDestination.Id & PIPE
        End If
    End Sub

    ''' <summary> Funzione che scorre ricorsivamente i nodi e mantiene la selezione effettuata in precedenza </summary>
    ''' <param name="selectedUniqueId">lo uniqueId del nodo selezionato in precedenza</param>
    ''' <param name="node">il nodo da controllare</param>
    Public Sub SetSelectedNode(ByVal selectedUniqueId As String, ByVal node As RadTreeNode, ByVal incr As String)
        If node.Nodes Is Nothing Then
            Exit Sub
        End If

        For Each childnode As RadTreeNode In node.Nodes
            If Not childnode.Nodes Is Nothing Then
                SetSelectedNode(selectedUniqueId, childnode, incr)
            End If

            Dim str As String = childnode.Value.Split(PIPE)(0)
            If str = incr Then
                childnode.Selected = True
                Exit For
            End If

            If childnode.UniqueID.Eq(selectedUniqueId) Then
                childnode.Selected = True
            End If
        Next
    End Sub

#End Region

End Class