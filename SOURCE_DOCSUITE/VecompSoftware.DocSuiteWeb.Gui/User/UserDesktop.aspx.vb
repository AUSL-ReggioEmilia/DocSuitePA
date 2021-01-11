Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class UserDesktop
    Inherits UserBasePage


#Region " Fields "

    ''' <summary>  </summary>
    ''' <remarks> Ci sono una serie di convenzione legacy di navigazione sul secondo carattere di questa stringa. </remarks>
    Public Const ActionNameProtocolliRigettati As String = "PZProtocolliRigettati"
    ''' <summary>  </summary>
    ''' <remarks> Ci sono una serie di convenzione legacy di navigazione sul secondo carattere di questa stringa. </remarks>
    Public Const ActionNameAdottateNonEsecutive As String = "RAdottateNonEsecutive"
    Public Const RESOLUTION_PROPOSED_BY_ROLE As String = "ResolutionProposedByRole"

    Public Const PageScrivania As String = "UserScrivaniaD.aspx"

    Public Const PageAuthorized As String = "UserAuthorizedDocuments.aspx"

    Public Const PageDiario As String = "UserDiario.aspx"

    Public Const PageUDFascicle As String = "UserUDFascicle.aspx"

    Public Const PageFascicle As String = "UserFascicle.aspx"

    Public Const PageDossier As String = "UserDossier.aspx"

    Public Const PageDiarioComune As String = "UserDiarioComune.aspx"

    Public Const PageDiarioUnificato As String = "UserDiarioUnificato.aspx"

    Public Const PageCollRisultati As String = "UserCollRisultati.aspx"

    Public Const PageAmministrazioneTrasparente As String = "UserScrivaniaD.aspx"

    Public Const PageWorkflowRisultati As String = "UserWorkflow.aspx"

    Public Const PageUserUDS As String = "~/UDS/UserUDS.aspx"

    Public Const PageZenDeskHelp As String = "ZenDeskHelp.aspx"

#End Region

#Region " Properties "

    Private ReadOnly Property ContentUrl As String
        Get
            Dim queryStringVal As String = Request.QueryString.GetValueOrDefault(Of String)("content", "")
            If String.IsNullOrEmpty(queryStringVal) Then
                Return contentPane.ContentUrl
            Else
                Return queryStringVal
            End If
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
            InitializeMenu()
        End If

        Dim url As String = ContentUrl
        If Not String.IsNullOrEmpty(url) Then
            contentPane.ContentUrl = HttpUtility.UrlDecode(url)
        Else
            contentPane.ContentUrl = "../Comm/CommIntro.aspx"
        End If

    End Sub

    Private Sub RadTreeMenu_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeMenu.NodeClick
        Dim item As UserDesktopNodeAction = GetNodeActionFromRadTree(e.Node)
        If IsNothing(item) Then
            Exit Sub
        End If

        Dim url As String = GetNodeUrl(item)
        If String.IsNullOrEmpty(url) Then
            Exit Sub
        End If

        Session("Scrivania") = item.ActionType
        contentPane.ContentUrl = HttpUtility.UrlDecode(url)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        tempAjaxManager.AjaxSettings.AddAjaxSetting(RadTreeMenu, userSplitter)
    End Sub

    Private Sub InitializeMenu()
        If Not CommonInstance.AppAccessOk Then
            Exit Sub
        End If

        ' Se le voci della scrivania sono spostate nel menu principale, allora non inizializzo la scrivania
        If ProtocolEnv.MoveScrivaniaMenu Then
            Exit Sub
        End If

        Dim nodo As New RadTreeNode

        'Creazione Menu con Help
        If ProtocolEnv.IsHelpEnabled Then
            Dim helpDir As New DirectoryInfo(Path.Combine(CommonInstance.AppPath, "Help"))
            If Not helpDir.Exists Then
                FileLogger.Error(LoggerName, String.Format("Help abilitato ma directory di help non trovata [{0}].", helpDir.FullName))
            End If
            'Imposta il nodo con la documentazione del prodotto recuperandola dalla cartella "Help" dell'applicazione
            RadTreeMenu.Nodes.Add(AddNode(nodo, New UserDesktopNodeAction("Documentazione", "T", "T", PageZenDeskHelp, "", "../Comm/Images/Help16.gif"), False))
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.MoveCollaborationMenu AndAlso CollaborationRights.GetIsCollaborationEnabled() Then
            'Creazione Menu Collaborazione
            RadTreeMenu.Nodes.Add(CreateCollaborationGeneralNode())

            RadTreeMenu.Nodes.Add(CreateCollaborationNode())

        Else
            'Creazione Menu Standard
            If DocSuiteContext.Current.IsLogEnable Then
                Dim node As New RadTreeNode()
                RadTreeMenu.Nodes.Add(AddNode(node, New UserDesktopNodeAction("Diario generale", "GY", "T", If(DocSuiteContext.Current.ProtocolEnv.EnableUnifiedDiary, PageDiarioUnificato, PageDiarioComune), String.Empty, "../Comm/Images/Calendario16.gif"), True))
            End If
        End If
        If ProtocolEnv.DiaryFullEnabled Then
            InitializeMenuItems()
        End If

        Dim selezione As String = CType(Session("Scrivania"), String)
        If String.IsNullOrEmpty(selezione) Then
            contentPane.ContentUrl = "../Comm/CommIntro.aspx"
            Exit Sub
        End If

        Dim toSelect As RadTreeNode = RadTreeMenu.FindNodeByValue(selezione)

        Dim item As UserDesktopNodeAction = GetNodeActionFromRadTree(toSelect)
        If IsNothing(item) Then
            Exit Sub
        End If

        Dim url As String = GetNodeUrl(item)
        If String.IsNullOrEmpty(url) Then
            Exit Sub
        End If
        toSelect.Selected = True
        Session("Scrivania") = item.ActionType
        contentPane.ContentUrl = url

    End Sub

    Private Shared Function CreateCollaborationGeneralNode() As RadTreeNode
        If Not DocSuiteContext.Current.ProtocolEnv.IsDiaryEnabled Then
            Return Nothing
        End If

        Dim node As New RadTreeNode()
        AddNode(node, New UserDesktopNodeAction("Diario", "", "T", "", "", "../Comm/Images/Calendario16.gif"), True)

        If DocSuiteContext.Current.IsDocumentEnabled AndAlso DocSuiteContext.Current.DocumentEnv.IsEnvLogEnabled _
           AndAlso DocSuiteContext.Current.IsProtocolEnabled AndAlso DocSuiteContext.Current.IsResolutionEnabled _
           AndAlso DocSuiteContext.Current.ResolutionEnv.IsLogEnabled Then
            AddNode(node, New UserDesktopNodeAction("Generale", "GY", "N", PageDiarioComune, "", "../Comm/Images/FolderClose16.gif"), True)
            Return node
        End If

        If DocSuiteContext.Current.IsProtocolEnabled Then
            AddNode(node, New UserDesktopNodeAction("Protocolli", "PY", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), True)
        End If
        Return node
    End Function

    Public Function CreateCollaborationNode() As RadTreeNode
        If Not DocSuiteContext.Current.IsProtocolEnabled Then
            Return Nothing
        End If

        Dim node As New RadTreeNode()
        AddNode(node, New UserDesktopNodeAction("Collaborazione", "", "T", "", "", "../App_Themes/DocSuite2008/imgset16/collaboration.png"), True)
        AddNode(node, New UserDesktopNodeAction("Alla visione/Firma", CollaborationMainAction.AllaVisioneFirma, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Da visionare/Firmare", CollaborationMainAction.DaVisionareFirmare, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Al protocollo/Segreteria", CollaborationMainAction.AlProtocolloSegreteria, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)

        If Facade.ContainerFacade.HasInsertOrProposalRights() Then
            AddNode(node, New UserDesktopNodeAction("Da protocollare/Gestire", CollaborationMainAction.DaProtocollareGestire, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        End If

        AddNode(node, New UserDesktopNodeAction("Protocollati/Gestiti", CollaborationMainAction.ProtocollatiGestiti, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)

        AddNode(node, New UserDesktopNodeAction("Attività in corso", CollaborationMainAction.AttivitaInCorso, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction(DocSuiteContext.Current.ProtocolEnv.MieiCheckOutMenuLabel, CollaborationMainAction.MieiCheckOut, "N", PageCollRisultati, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Check In multiplo", CollaborationMainAction.CheckInMultiplo, "N", "CollaborationVersioningManagement.aspx", "", "../Comm/Images/FolderClose16.gif"), False)

        If DocSuiteContext.Current.ProtocolEnv.IsUserCollOfflineEnabled Then
            AddNode(node, New UserDesktopNodeAction("Scarica visualizzatore offline", CollaborationMainAction.VisualizzatoreOffline, "N", "Offline.zip", "Docm", "../Comm/Images/FolderClose16.gif"), False)
        End If

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            Dim tenantModel As TenantModel = DocSuiteContext.Current.Tenants.FirstOrDefault(Function(x) Not x.CurrentTenant AndAlso x.Entities.Any(Function(t) t.Key.Eq("Protocol")))
            Dim entityConfiguration As TenantEntityConfiguration = GetFromType(Of Protocol)(tenantModel.Entities)
            If entityConfiguration.IsActive Then
                AddNode(node, New UserDesktopNodeAction("I miei documenti autorizzati", "ADL", "N", "../Prot/ProtMDRisultati.aspx", "", "../Comm/Images/FolderClose16.gif"), False)
            End If
        End If

        Return node
    End Function

    Private Sub InitializeMenuItems()
        Dim menuJson As IDictionary(Of String, MenuNodeModel) = DocSuiteContext.Current.DocSuiteMenuConfiguration
        If ProtocolEnv.PraticheEnabled Then
            RadTreeMenu.Nodes.Add(CreatePraticheNodes())
        End If

        If DocSuiteContext.Current.IsProtocolEnabled AndAlso ProtocolEnv.ShowProtocol Then
            RadTreeMenu.Nodes.Add(CreateProtocolNodes())
        End If

        If (DocSuiteContext.Current.ProtocolEnv.DossierEnabled OrElse
            (DocSuiteContext.Current.ProtocolEnv.FascicleEnabled AndAlso menuJson("Menu7").Nodes.Keys.Contains("FirstNode2"))) Then
            RadTreeMenu.Nodes.Add(CreateFascicleAndDossierNodes(menuJson))
        End If

        If DocSuiteContext.Current.IsResolutionEnabled Then
            RadTreeMenu.Nodes.Add(CreateResolutionNodes())
        End If

        If DocSuiteContext.Current.ProtocolEnv.UDSEnabled = True Then
            RadTreeMenu.Nodes.Add(CreateArchiveNodes())
        End If

    End Sub

    Private Shared Function AddNode(ByRef nodoPrincipale As RadTreeNode, ByVal nodeParams As UserDesktopNodeAction, ByVal expanded As Boolean) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = nodeParams.Title
        node.Value = nodeParams.ActionType
        node.Attributes("NodeType") = nodeParams.NodeType
        node.Attributes("PageName") = nodeParams.PageName
        node.Attributes("DocumentType") = nodeParams.DocumentType

        Select Case nodeParams.NodeType
            Case "X"
                node.ImageUrl = nodeParams.ImageUrl
                nodoPrincipale.Nodes.Add(node)

            Case "N"
                node.ImageUrl = nodeParams.ImageUrl
                node.SelectedImageUrl = "../Comm/Images/FolderOpen16.gif"
                nodoPrincipale.Nodes.Add(node)

            Case "T"
                node.ImageUrl = nodeParams.ImageUrl
                node.Font.Bold = True
                nodoPrincipale = node
            Case "Z"
                node.ImageUrl = nodeParams.ImageUrl
                node.Font.Bold = True
                nodoPrincipale = node

        End Select

        If expanded Then
            nodoPrincipale.Expanded = True
        End If

        Return node

    End Function

    Public Shared Function GetNodeUrl(item As UserDesktopNodeAction) As String
        If String.IsNullOrEmpty(item.PageName) Then
            Return GetNodeUrl(item.Title, item.ActionType)
        End If

        Dim parameters As New StringBuilder()
        If Not String.IsNullOrEmpty(item.DocumentType) Then
            parameters.AppendFormat("Type={0}", item.DocumentType)
        End If

        parameters.AppendFormat("&Title={0}&Action={1}", item.Title, item.ActionType)
        Dim url As String = String.Format("{0}?{1}", item.PageName, CommonShared.AppendSecurityCheck(parameters.ToString()))
        Return url
    End Function

    ''' <summary> Torna l'url corrispondente alla action. </summary>
    ''' <remarks>
    ''' TODO: se possibile, evitare di usarlo o implementare cose nuove con esso
    ''' </remarks>
    Public Shared Function GetNodeUrl(ByRef title As String, ByRef action As String) As String
        If String.IsNullOrEmpty(action) Then
            Return Nothing
        End If

        Dim url As String = ""
        Dim parameters As New StringBuilder()
        Select Case action.Substring(0, 1)
            Case "O"
                url = "../Comm/CommIntro.aspx"

            Case "X"
                url = action.Substring(1)

            Case "D"
                Select Case action
                    Case "DY"
                        parameters.Append("Type=Docm")
                        url = "UserDiario.aspx"
                    Case Else
                        parameters.Append("Type=Docm")
                        url = "UserScrivaniaD.aspx"
                End Select

            Case "C"
                Select Case action
                    Case CollaborationMainAction.VisualizzatoreOffline
                        parameters.Append("Type=Docm")
                        url = "Offline.zip"
                    Case CollaborationMainAction.CheckInMultiplo
                        url = "CollaborationVersioningManagement.aspx"
                    Case Else
                        parameters.Append("Type=Prot")
                        url = "UserCollRisultati.aspx"
                End Select

            Case "P"
                Select Case action
                    Case "PY"
                        parameters.Append("Type=Prot")
                        url = "UserDiario.aspx"
                    Case Else
                        parameters.Append("Type=Prot")
                        url = "UserScrivaniaD.aspx"
                End Select

            Case "R"
                Select Case action
                    Case "RY"
                        parameters.Append("Type=Resl")
                        url = "UserDiario.aspx"

                    Case Else
                        parameters.Append("Type=Resl")
                        url = "UserScrivaniaD.aspx"
                End Select
            Case "Z"
                Select Case action
                    Case "Z"
                        parameters.Append("Type=Resl&")
                        parameters.Append("AmmTras=True")
                        url = "UserScrivaniaD.aspx"
                End Select
            Case "G"
                Select Case action
                    Case "GY"
                        url = "UserDiarioComune.aspx"
                    Case Else
                        url = "UserScrivaniaD.aspx"
                End Select
            Case "F"
                parameters.Append("Type=Prot")
                url = PageUDFascicle
        End Select

        If String.IsNullOrEmpty(url) Then
            Return Nothing
        End If

        parameters.AppendFormat("&Title={0}&Action={1}", title, action)
        Return String.Format("{0}?{1}", url, CommonShared.AppendSecurityCheck(parameters.ToString()))
    End Function

    Public Function GetNodeActionFromRadTree(node As RadTreeNode) As UserDesktopNodeAction
        Try
            Dim model As New UserDesktopNodeAction With {
                .Title = node.Text,
                .ActionType = node.Value,
                .NodeType = node.Attributes.Item("NodeType"),
                .PageName = node.Attributes.Item("PageName"),
                .DocumentType = node.Attributes.Item("DocumentType"),
                .ImageUrl = node.ImageUrl
            }
            Return model
        Catch
            Return Nothing
        End Try
    End Function

    Private Function CreateArchiveNodes() As RadTreeNode
        Dim node As New RadTreeNode()
        AddNode(node, New UserDesktopNodeAction("Archivi", "", "T", "", "", "../App_Themes/DocSuite2008/imgset16/document_copies.png"), True)
        AddNode(node, New UserDesktopNodeAction("Da leggere", "DL", "N", PageUserUDS, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        Return node
    End Function

    Private Function CreateFascicleAndDossierNodes(menuJson As IDictionary(Of String, MenuNodeModel)) As RadTreeNode
        Dim menu As MenuNodeModel = menuJson("Menu7")
        Dim node As New RadTreeNode()
        RadTreeMenu.Nodes.Add(AddNode(node, New UserDesktopNodeAction(menu.Name, "", "T", "", "", "~/App_Themes/DocSuite2008/imgset16/fascicle_open.png"), True))
        CreateFascicleNodes(node, menu)
        CreateDossierNodes(node, menu)
        Return node
    End Function

    Private Sub CreateFascicleNodes(parent As RadTreeNode, menu As MenuNodeModel)
        If (DocSuiteContext.Current.ProtocolEnv.FascicleEnabled) Then
            AddNode(parent, New UserDesktopNodeAction(menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, "FDF", "N", PageUDFascicle, "Fasc", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(parent, New UserDesktopNodeAction("Fascicoli aperti", "FOR", "N", PageFascicle, "Fasc", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(parent, New UserDesktopNodeAction("Fascicoli autorizzati ", "FAU", "N", PageFascicle, "Fasc", "../Comm/Images/FolderClose16.gif"), False)
        End If
    End Sub

    Private Sub CreateDossierNodes(parent As RadTreeNode, menu As MenuNodeModel)
        If DocSuiteContext.Current.ProtocolEnv.DossierEnabled Then
            AddNode(parent, New UserDesktopNodeAction("Dossier aperti", "DOP", "N", PageDossier, "Dossier", "../Comm/Images/FolderClose16.gif"), False)
        End If
    End Sub

    Private Function CreatePraticheNodes() As RadTreeNode
        Dim node As New RadTreeNode()

        RadTreeMenu.Nodes.Add(AddNode(node, New UserDesktopNodeAction("Pratiche", "", "T", "", "", "../Comm/Images/DocSuite/Pratica16.gif"), True))
        If DocumentEnv.IsEnvLogEnabled Then
            AddNode(node, New UserDesktopNodeAction("Diario", "DY", "N", PageDiario, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        End If
        AddNode(node, New UserDesktopNodeAction("Da leggere", "DL", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Aperte da leggere", "DAL", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Documenti checkOut", "DK", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Richiesta presa in Car.", "DR", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento oggi", "DO", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento settimana", "DS", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento mese", "DM", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Scadenziario", "DE", "N", PageScrivania, "Docm", "../Comm/Images/FolderClose16.gif"), False)
        Return node
    End Function

    Private Function CreateProtocolNodes() As RadTreeNode
        Dim node As New RadTreeNode()
        RadTreeMenu.Nodes.Add(AddNode(node, New UserDesktopNodeAction("Protocolli", "", "T", "", "", "../Comm/Images/DocSuite/Protocollo16.gif"), True))
        AddNode(node, New UserDesktopNodeAction("Diario", "PY", "N", PageDiario, "Prot", "../Comm/Images/FolderClose16.gif"), False)

        If ProtocolEnv.ProtocolHighlightEnabled Then
            AddNode(node, New UserDesktopNodeAction("In evidenza", "PE", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        End If

        If ProtocolEnv.IsDistributionEnabled Then
            AddNode(node, New UserDesktopNodeAction("Da leggere", "PL", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Da distribuire", "PD", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Da lavorare", "PDL", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("In assegnazione", "PA", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Recenti", "PR", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        Else
            If ProtocolEnv.IsLogStatusEnabled Then
                AddNode(node, New UserDesktopNodeAction("Da leggere", "PL", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            End If
            If ProtocolEnv.InvoiceSDIEnabled Then
                AddNode(node, New UserDesktopNodeAction("Fatture da leggere", "IPL", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            End If
            If ProtocolEnv.IsStatusEnabled Then
                AddNode(node, New UserDesktopNodeAction("Assegnato", "PV", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            End If
        End If

        If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
            AddNode(node, New UserDesktopNodeAction("Da Accettare", "PDA", "N", PageAuthorized, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            If ProtocolEnv.IsSecurityGroupEnabled AndAlso CommonShared.HasRefusedProtocolGroupsRight Then
                AddNode(node, New UserDesktopNodeAction("Autorizzati non accettati", "PANA", "N", PageAuthorized, "Prot", "../Comm/Images/FolderClose16.gif"), False)
                AddNode(node, New UserDesktopNodeAction("Respinti", "PRS", "N", PageAuthorized, "Prot", "../Comm/Images/FolderClose16.gif"), False)
            End If
        End If

        AddNode(node, New UserDesktopNodeAction("Aggiornati", "PU", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento oggi", "PO", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento settimana", "PS", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento mese", "PM", "N", PageScrivania, "Prot", "../Comm/Images/FolderClose16.gif"), False)

        If ProtocolEnv.ProtocolKindEnabled Then
            If ProtocolEnv.InvoicePAEnabled AndAlso ProtocolEnv.IsInvoiceEnabled Then
                Dim invoicePaNode As RadTreeNode = AddNode(node, New UserDesktopNodeAction("Fatture PA", "", "N", "", "", "../Comm/Images/FolderOpen16.gif"), True)
                AddNode(invoicePaNode, New UserDesktopNodeAction("Consegnate a SDI", ProtGestioneFatturePA.ConsegnateSDI, "N", "../Prot/ProtGestioneFatturePA.aspx", "Prot", "../Comm/Images/FolderClose16.gif"), True)
                AddNode(invoicePaNode, New UserDesktopNodeAction("Consegnate", ProtGestioneFatturePA.Consegnate, "N", "../Prot/ProtGestioneFatturePA.aspx", "Prot", "../Comm/Images/FolderClose16.gif"), True)
                AddNode(invoicePaNode, New UserDesktopNodeAction("Rifiutate", ProtGestioneFatturePA.Rifiutate, "N", "../Prot/ProtGestioneFatturePA.aspx", "Prot", "../Comm/Images/FolderClose16.gif"), True)
                AddNode(invoicePaNode, New UserDesktopNodeAction("Da inviare", ProtGestioneFatturePA.DaInviare, "N", "../Prot/ProtGestioneFatturePA.aspx", "Prot", "../Comm/Images/FolderClose16.gif"), True)
            End If
        End If
        Return node
    End Function

    Private Function CreateResolutionNodes() As RadTreeNode
        Dim node As New RadTreeNode()
        RadTreeMenu.Nodes.Add(AddNode(node, New UserDesktopNodeAction(Facade.TabMasterFacade.TreeViewCaption, "", "T", "", "", "../Comm/Images/DocSuite/Resolution16.gif"), True))
        If ResolutionEnv.IsLogEnabled Then
            If ResolutionEnv.Configuration.Eq("AUSL-PC") Then
                AddNode(node, New UserDesktopNodeAction("Diario giornaliero", "RY", "N", PageDiario, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            Else
                AddNode(node, New UserDesktopNodeAction("Diario", "RY", "N", PageDiario, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            End If
        End If
        If ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            AddNode(node, New UserDesktopNodeAction("Non conformi", "RNC", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Da controllare per conformità", "RICC", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Conformi da adottare", "RCDA", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Adottate", "RA", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Adottate non esecutive", ActionNameAdottateNonEsecutive, "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Pubblicate", "RP", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("Esecutive", "RE", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
        Else
            AddNode(node, New UserDesktopNodeAction("In proposta", "RPO", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("In adozione", "RA", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("In pubblicazione", "RP", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            AddNode(node, New UserDesktopNodeAction("In esecutività", "RE", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            If (ResolutionEnv.ResolutionKindEnabled) Then
                AddNode(node, New UserDesktopNodeAction("Atti per Amministrazione Trasparente", "Z", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
            End If
        End If
        If ResolutionEnv.ViewResolutionProposedByRoleEnabled Then
            AddNode(node, New UserDesktopNodeAction("Proposte dai singoli settori", "ResolutionProposedByRole", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
        End If
        AddNode(node, New UserDesktopNodeAction("Inserimento oggi", "RO", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento settimana", "RS", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
        AddNode(node, New UserDesktopNodeAction("Inserimento mese", "RM", "N", PageScrivania, "Resl", "../Comm/Images/FolderClose16.gif"), False)
        Return node
    End Function
#End Region

End Class
