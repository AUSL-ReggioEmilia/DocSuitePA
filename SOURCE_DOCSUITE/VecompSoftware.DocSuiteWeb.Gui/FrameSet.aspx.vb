Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Dossiers
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Dossiers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons

Partial Public Class FrameSet
    Inherits CommonBasePage

#Region " Helper della vergogna "

    ''' <summary> Classe d'appoggio per la creazione di un nodo del menu </summary>
    ''' <remarks> Speriamo tutti che cose del genere non rimangano troppo nella docsuite, e sappiamo tutti che non sarà così </remarks>
    Public Enum NodeLevel
        First
        Second
    End Enum

    ''' <summary> Classe d'appoggio per la creazione di un nodo del menu </summary>
    ''' <remarks> Speriamo tutti che cose del genere non rimangano troppo nella docsuite, e sappiamo tutti che non sarà così </remarks>
    Public Enum NodeImage
        Point
        Line
        Desktop
        Help
    End Enum

    ''' <summary> Classe d'appoggio per la creazione di un nodo del menu </summary>
    ''' <remarks> Speriamo tutti che cose del genere non rimangano troppo nella docsuite, e sappiamo tutti che non sarà così </remarks>
    Public Class NodeLink
        Public Property Level As NodeLevel
        Public Property Image As NodeImage
        Public Property Name As String
        Public Property Url As String
        Public Property Expanded As Boolean

        Public Sub New(level As NodeLevel, image As NodeImage, name As String, page As String, parameters As String)
            Me.Level = level
            Me.Image = image
            Me.Name = name
            If Not String.IsNullOrEmpty(page) Then
                Url = page
                If Not String.IsNullOrEmpty(parameters) Then
                    Url += "?"
                    Url += parameters
                End If
            End If

            Expanded = True
        End Sub

        Public Sub New(level As NodeLevel, image As NodeImage, name As String)
            Me.New(level, image, name, "", "")
        End Sub

    End Class

#End Region

#Region " Fields "

    Private Const JavascriptToolBarClicking As String = "ToolBarClientClicking"
    Private _currentUserLog As UserLog

#End Region

#Region " Properties "

    Private ReadOnly Property Tipo As String
        Get
            Return Request.QueryString("Tipo")
        End Get
    End Property

    Private ReadOnly Property Azione As String
        Get
            Return Request.QueryString("Azione")
        End Get
    End Property

    Private ReadOnly Property Anno As String
        Get
            Return Request.QueryString("Anno")
        End Get
    End Property

    Private ReadOnly Property Numero As String
        Get
            Return Request.QueryString("Numero")
        End Get
    End Property

    Private ReadOnly Property TargetId As String
        Get
            Return Request.QueryString("Identificativo")
        End Get
    End Property

    Private ReadOnly Property TargetDocumentSeriesId As String
        Get
            Return Request.QueryString("IdDocumentSeriesItem")
        End Get
    End Property

    Private ReadOnly Property IdUDSRepository As String
        Get
            Return Request.QueryString("IdUDSRepository")
        End Get
    End Property

    Private ReadOnly Property IdUDS As String
        Get
            Return Request.QueryString("IdUDS")
        End Get
    End Property

    Protected ReadOnly Property IdFascicle As Guid
        Get
            Return Request.QueryString.GetValue(Of Guid)("IdFascicle")
        End Get
    End Property

    Protected ReadOnly Property DefaultDocumentId As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault("DefaultDocumentId", Guid.Empty)
        End Get
    End Property

    Private ReadOnly Property OpenTargetDocument As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("ApriDocumento", String.Empty).Eq("si")
        End Get
    End Property

    Private ReadOnly Property Metadato As String
        Get
            Return Request.QueryString("Metadato")
        End Get
    End Property

    Private ReadOnly Property Valore As String
        Get
            Return Request.QueryString("Valore")
        End Get
    End Property

    Private ReadOnly Property Fascicolo As String
        Get
            Return Request.QueryString("Fascicolo")
        End Get
    End Property

    Private ReadOnly Property RepositoryName As String
        Get
            Return Request.QueryString.GetValueOrDefault("RepositoryName", String.Empty)
        End Get
    End Property

    Private ReadOnly Property IsRejectionVisible As Boolean
        Get
            ' Verifico sia abilitato il rigetto di protocollo,
            ' verifico sia configurato il contenitore destinatario dei rigetti
            ' e verifoco di avere i permessi su quel contenitore.
            Return DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionEnabled _
                AndAlso Not ProtocolEnv.ProtocolRejectionContainerId.Equals(0) AndAlso
                FacadeFactory.Instance.ContainerFacade.CheckContainerRight(ProtocolEnv.ProtocolRejectionContainerId, DSWEnvironment.Protocol, ProtocolContainerRightPositions.Modify, True)
        End Get
    End Property

    Public ReadOnly Property IsNotificationEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.NotificationEnabled
        End Get
    End Property

    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentUserLog
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        If Not Page.IsPostBack Then
            LoadNotificationCounters()
        End If
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not Page.IsPostBack Then

            ' Stili personalizzati per cliente
            Dim stylePath As String = Path.Combine(CommonInstance.AppPath, "SubStyles", DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
            If Directory.Exists(stylePath) Then
                stylesheet.Href = String.Format("SubStyles/{0}/custom.css", DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
            End If

            ' header 
            applicationTitle.Text = DocSuiteContext.Current.ProtocolEnv.ApplicationName

            btnCompanyName.Visible = DocSuiteContext.Current.ProtocolEnv.MultiTenantEnabled
            btnCompanyName.Text = SearchSelectedCompanyForTheCurrentUser()
            btnCompanyName.AutoPostBack = True
            btnCompanyName.OnClientClicking = JavascriptToolBarClicking
            btnCompanyName.NavigateUrl = "Utlt/UserProfile.aspx"
            btnCompanyName.Target = main.ClientID
            btnReloadSessionTenant.Visible = DocSuiteContext.Current.ProtocolEnv.MultiTenantEnabled

            ' Se abilitato il parametro MoveScrivaniaMenu disabilito la scrivania, le voci di menu vengono spostate nel menu principale
            btnScrivania.Visible = False

            If Not DocSuiteContext.Current.ProtocolEnv.MoveScrivaniaMenu Then

                btnScrivania.Visible = True
                btnScrivania.AutoPostBack = True
                btnScrivania.OnClientClicking = JavascriptToolBarClicking
                btnScrivania.Icon.PrimaryIconUrl = ImagePath.SmallDesktop
                btnScrivania.NavigateUrl = "User/UserDesktop.aspx"
                btnScrivania.Target = main.ClientID
            End If

            ' Creazione menu
            If CommonInstance.AppAccessOk Then
                CreateMenu()
            End If

            ' Apertura menu
            If ProtocolEnv.MainMenuCollapsed Then
                StartWithCollapsedMenu()
            End If

            ' Main (interno ai pannelli)
            main.ContentUrl = CreateContentUrl()

            ' footer
            applicationName.Text = ProtocolEnv.CorporateName
            If Assembly.GetExecutingAssembly().GetName().Version IsNot Nothing Then
                Dim version As String = Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
                If Not String.IsNullOrEmpty(version) Then
                    applicationName.Text += String.Format(" (Ver. {0})", version)
                End If
            End If

            Dim userLabel As String = DocSuiteContext.Current.User.FullUserName
            If Not String.IsNullOrEmpty(CommonInstance.UserDescription) Then
                userLabel = String.Format("{0} - {1}", userLabel, CommonInstance.UserDescription)
            End If

            lblUtenteCollegato.Text = userLabel

            If ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
                Try
                    Facade.UserLogFacade.EvaluateProtocolSearchStatistics(CurrentUserLog, ProtocolEnv.ProtocolDefaultAdaptiveSearch.Affinity)
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)
                End Try
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso Not DocSuiteContext.Current.HasPrivacyLevels Then
                Dim privacyLevelFacade As PrivacyLevelFacade = New PrivacyLevelFacade()
                DocSuiteContext.Current.RefreshPrivacyLevel(privacyLevelFacade.GetCurrentPrivacyLevels())
            End If

            If CurrentTenant Is Nothing Then
                AjaxManager.ResponseScripts.Add($"notifyAndRedirectToConfiguration('Utlt/UserProfile.aspx','{main.ClientID}');")
            End If

            btnHelp.OnClientClicking = JavascriptToolBarClicking
            btnHelp.Target = main.ClientID

            If ProtocolEnv.FulltextEnabled Then
                btnFullText.OnClientClicking = JavascriptToolBarClicking
                btnFullText.Target = main.ClientID
                btnFullText.Visible = True
            End If
        End If

    End Sub



    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        ' Gestione Dock, Unodck del pannello menu
        Dim cookieValue As HttpCookie = Request.Cookies("PaneDocked")
        If cookieValue IsNot Nothing Then
            Select Case cookieValue.Value
                Case "true"
                    SlidingZoneMenu.DockedPaneId = SlidingPaneMenu.ClientID
                Case Else
                    SlidingZoneMenu.DockedPaneId = Nothing
            End Select
        End If

        ' Gestione ultima voce del menu aperta
        cookieValue = Request.Cookies("ItemExpanded")
        Dim selectedItem As RadPanelItem = RadPanelBarMenu.SelectedItem
        If selectedItem IsNot Nothing Then
            selectedItem.Expanded = True
        ElseIf cookieValue IsNot Nothing Then
            'recupero il pannello dal cookie
            selectedItem = RadPanelBarMenu.FindItemByValue(cookieValue.Value)
            If selectedItem IsNot Nothing Then
                selectedItem.Expanded = True
            Else
                RadPanelBarMenu.Items(0).Expanded = True
            End If
        Else
            RadPanelBarMenu.Items(0).Expanded = True
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReloadSessionTenant, btnReloadSessionTenant)
    End Sub

    Private Sub ButtonRenew_Click(sender As Object, e As EventArgs) Handles btnReloadSessionTenant.Click
        Dim tenantFacade As Facade.Common.Tenants.TenantFacade = New Facade.Common.Tenants.TenantFacade()
        Session(CommonShared.USER_CURRENT_TENANT) = tenantFacade.GetCurrentTenant()
        ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "reloadSession", "reloadSession();", True)
    End Sub

    Private Function SearchSelectedCompanyForTheCurrentUser() As String
        Dim companyname As String = "AOO non configurata"
        Try
            If CurrentTenant IsNot Nothing Then
                companyname = CurrentTenant.CompanyName
            End If
        Catch ex As Exception
        End Try
        Return companyname
    End Function


    Private Shared Function AddTreeViewItem(ByVal treeId As String, ByVal menuList As IList(Of NodeLink), ByVal targetWindow As String) As RadPanelItem
        Dim treeView As New RadTreeView()
        treeView.ID = treeId
        treeView.CssClass = "DocSuiteMenu"
        treeView.ShowLineImages = False
        treeView.SingleExpandPath = False
        treeView.Skin = "Bootstrap"
        'apertura singolo click
        treeView.OnClientNodeClicked = "NodeClicked"
        'nodo padre per di secondo livello
        Dim treeNode1 As RadTreeNode = Nothing
        ' Creo tutti i nodi
        For Each node As NodeLink In menuList

            Dim treeNode As New RadTreeNode()
            Select Case node.Image
                Case NodeImage.Line
                    treeNode.ImageUrl = ""
                Case NodeImage.Point
                    treeNode.ImageUrl = "Comm/Images/Home/Punto.gif"
                Case NodeImage.Desktop
                    treeNode.ImageUrl = ImagePath.SmallDesktop
                Case NodeImage.Help
                    treeNode.ImageUrl = ImagePath.SmallHelp
                Case Else
                    treeNode.ImageUrl = "Comm/Images/Home/Punto.gif"
            End Select

            treeNode.Text = node.Name
            treeNode.Expanded = node.Expanded
            If Not String.IsNullOrEmpty(node.Url) Then
                treeNode.Target = targetWindow
                treeNode.NavigateUrl = node.Url
            End If

            Select Case node.Level
                Case NodeLevel.First
                    treeView.Nodes.Add(treeNode)
                    treeNode1 = treeNode
                Case NodeLevel.Second
                    If treeNode1 IsNot Nothing Then
                        treeNode1.Nodes.Add(treeNode)
                    Else
                        Throw New DocSuiteException("Errore inizializzazione", String.Format("Impossibile aggiungere il nodo [{0}] al menu.", treeNode.Text))
                    End If
            End Select
        Next node

        ' Inserisco in un panel item per consentire al pannello di chiudersi
        Dim panelItem As New RadPanelItem()
        panelItem.Controls.Add(treeView)
        Return panelItem

    End Function

    Private Sub StartWithCollapsedMenu()
        SlidingZoneMenu.OnClientLoad = "CollapseMenu"
    End Sub

    Private Function CreateContentUrl() As String
        Dim customTipo As String = Tipo
        Dim customAzione As String = Azione

        If Action.Eq("InserimentoCompleto") OrElse Action.Eq("IC") Then
            customTipo = "Prot"
            customAzione = "InserimentoCompleto"
            Page.Response.AddHeader("X-XSS-Protection", "0")
        End If

        ' Se il tipo è specificato non espando il menu
        If Not String.IsNullOrEmpty(customTipo) Then
            StartWithCollapsedMenu()
        End If
        If Not String.IsNullOrEmpty(customTipo) Then
            customTipo = customTipo.ToLower()
        End If

        Select Case customTipo
            Case ""
                Dim url As String
                If ProtocolEnv.IsDesktopEnabled Then
                    If ProtocolEnv.IsCollaborationEnabled Then
                        url = "Comm/CommIntro.aspx"
                    Else
                        url = "User/UserDesktop.aspx"
                    End If
                Else
                    url = "Comm/CommIntro.aspx"
                End If
                Return url

            Case "coll"
                Dim url As String
                Select Case customAzione
                    Case "Apri", ""
                        ' Caso generico
                        Dim parameters As String = String.Format("Titolo=Inserimento&Type={0}&Action={1}&idCollaboration={2}&Action2={3}&Title2={4}",
                                                                 Request.QueryString("CollType"), Request.QueryString("SubAction"), TargetId, Request.QueryString("Stato"), Request.QueryString("TitleStep"))
                        url = "User/UserCollGestione.aspx?" & CommonShared.AppendSecurityCheck(parameters)

                    Case CollaborationMainAction.DaVisionareFirmare,
                        CollaborationMainAction.DaProtocollareGestire,
                        CollaborationMainAction.AlProtocolloSegreteria,
                        CollaborationMainAction.AllaVisioneFirma,
                        CollaborationMainAction.AttivitaInCorso,
                        CollaborationMainAction.ProtocollatiGestiti
                        ' Casi corrispondenti alle azioni predefinite
                        url = "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction("", customAzione, "", UserDesktop.PageCollRisultati, "", ""))
                    Case "Documenti"
                        url = String.Concat("Viewers/CollaborationViewer.aspx?", CommonShared.AppendSecurityCheck(String.Concat("DataSourceType=coll&id=", TargetId)))
                    Case Else
                        url = ""
                End Select
                Return url

            Case "docm"
                Dim url As String
                Select Case customAzione
                    Case "Inserimento"
                        url = "docm/DocmInserimento.aspx"
                    Case "Apri"
                        url = $"docm/DocmVisualizza.aspx?{CommonShared.AppendSecurityCheck($"Year={Anno}&Number={Numero}&Type=Docm")}"
                        url = "docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck("Year=" & Anno & "&Number=" & Numero & "&Type=Docm")
                    Case "ApriConServiceNumber"
                        Dim document As Document = Facade.DocumentFacade.GetByServiceNumber(Request.QueryString.Get("ServiceNumber")).FirstOrDefault()
                        If document Is Nothing Then
                            Throw New DocSuiteException("Pratica non trovata coi seguenti riferimenti")
                        End If
                        url = $"docm/DocmVisualizza.aspx?{CommonShared.AppendSecurityCheck($"Year={document.Year}&Number={document.Number}&Type=Docm")}"
                    Case "Cerca"
                        url = "docm/DocmRicerca.aspx"
                    Case Else
                        url = ""
                End Select
                Return url

            Case "prot"
                Select Case customAzione
                    Case "Inserimento"
                        Return "Prot/ProtInserimento.aspx?" & CommonShared.AppendSecurityCheck("Action=Insert")
                    Case "InserimentoMail"
                        ''EMANUELE: azione inserimento protocollo da outlook.
                        Dim documento As String = Server.UrlEncode(Request.QueryString("Documento"))
                        Dim allegati As String = Server.UrlEncode(Request.QueryString("Allegati"))
                        Dim mittenti As String = Server.UrlEncode(Request.QueryString("Mittenti"))
                        Dim oggetto As String = Server.UrlEncode(Request.QueryString("Oggetto"))
                        Dim destinatari As String = Server.UrlEncode(Request.QueryString("Destinatari"))
                        Dim path As String = Server.UrlEncode(Request.QueryString("Path"))
                        Dim direzione As String = Server.UrlEncode(Request.QueryString("Direzione"))
                        Dim parameters As String = String.Format("Action=InsertMail&Document={0}&Attachment={1}&Sender={2}&Recipient={3}&Object={4}&Direction={5}&Path={6}",
                                                                 documento, allegati, mittenti, destinatari, oggetto, direzione, path)
                        Return "Prot/ProtInserimento.aspx?" & CommonShared.AppendSecurityCheck(parameters)
                    Case "InserimentoCompleto"
                        ' Conversione parametri da italiano (dsw 5.0.0) a inglese (7.0.0)
                        Dim container As String = Server.UrlEncode(Request.QueryString("Ct"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Contenitore")) Then
                            container = Server.UrlEncode(Request.QueryString("Contenitore"))
                        End If
                        Dim direction As String = Server.UrlEncode(Request.QueryString("D"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Direzione")) Then
                            direction = Server.UrlEncode(Request.QueryString("Direzione"))
                        End If
                        Dim [object] As String = Server.UrlEncode(Request.QueryString("Obj"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Oggetto")) Then
                            [object] = Server.UrlEncode(Request.QueryString("Oggetto"))
                        End If
                        Dim sender As String = Server.UrlEncode(Request.QueryString("Mt"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Mittente")) Then
                            sender = Server.UrlEncode(Request.QueryString("Mittente"))
                        End If
                        Dim senderCode As String = Server.UrlEncode(Request.QueryString("CdMt"))
                        If Not String.IsNullOrEmpty(Request.QueryString("CodMittente")) Then
                            senderCode = Server.UrlEncode(Request.QueryString("CodMittente"))
                        End If
                        Dim recipient As String = Server.UrlEncode(Request.QueryString("Dt"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Destinatario")) Then
                            recipient = Server.UrlEncode(Request.QueryString("Destinatario"))
                        End If
                        Dim recipientCode As String = Server.UrlEncode(Request.QueryString("CdDt"))
                        If Not String.IsNullOrEmpty(Request.QueryString("CodDestinatario")) Then
                            recipientCode = Server.UrlEncode(Request.QueryString("CodDestinatario"))
                        End If
                        Dim category As String = Server.UrlEncode(Request.QueryString("Clt"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Classificatore")) Then
                            category = Server.UrlEncode(Request.QueryString("Classificatore"))
                        End If
                        Dim note As String = Server.UrlEncode(Request.QueryString("Nt"))
                        If Not String.IsNullOrEmpty(Request.QueryString("Note")) Then
                            note = Server.UrlEncode(Request.QueryString("Note"))
                        End If
                        Dim sectionalNumber As String = Server.UrlEncode(Request.QueryString("NbSz"))
                        Dim vatRegistrationNumber As String = Server.UrlEncode(Request.QueryString("INbRg"))
                        Dim vatRegistrationDate As String = Server.UrlEncode(Request.QueryString("IDtRg"))
                        Dim invoiceNumber As String = Server.UrlEncode(Request.QueryString("NbFt"))
                        Dim invoiceDate As String = Server.UrlEncode(Request.QueryString("DFt"))
                        Dim piva As String = Server.UrlEncode(Request.QueryString("PIF"))
                        ' Ricompongo la querystring e inoltro alla pagina di inserimento protocollo

                        Dim parameters As String = String.Format("Action={0}&Ct={1}&D={2}&Mt={3}&Dt={4}&CdDt={5}&CdMt={6}&Obj={7}&Clt={8}&Nt={9}&NbSz={10}&INbRg={11}&IDtRg={12}&NbFt={13}&DFt={14}&PIF={15}",
                                                                 "IC", container, direction, sender, recipient, recipientCode, senderCode, [object], category, note, sectionalNumber, vatRegistrationNumber, vatRegistrationDate, invoiceNumber, invoiceDate, piva)
                        Return "Prot/ProtInserimento.aspx?" & CommonShared.AppendSecurityCheck(parameters)
                    Case "Apri"
                        Dim year As String = Anno
                        Dim number As String = Numero
                        Dim accountingSectional As String = Request.QueryString("NbSz")
                        Dim container As String = Request.QueryString("Contenitore")
                        Dim vatRegistrationNumber As String = Request.QueryString("INbRg")
                        Dim extension As String = String.Empty
                        Dim claim As String = String.Empty
                        If OpenTargetDocument Then
                            extension = "&OpenDocument=Yes"
                        End If
                        If Not String.IsNullOrEmpty(Anno) AndAlso String.IsNullOrEmpty(Numero) Then
                            Dim prot As Protocol = Facade.ProtocolFacade.FinderProtocolByMetadati(Short.Parse(Anno), accountingSectional, container, Integer.Parse(vatRegistrationNumber))
                            If prot Is Nothing Then
                                Throw New DocSuiteException("Protocollo non trovato nella AOO corrente")
                            End If
                            If prot IsNot Nothing Then
                                number = prot.Number.ToString()
                            End If
                        End If
                        Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(year), Integer.Parse(number))
                        If currentProtocol Is Nothing Then
                            Throw New DocSuiteException("Protocollo non trovato nella AOO corrente")
                        End If
                        Return $"Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={currentProtocol.Id}{extension}")}"
                    Case "ApriFattura"
                        ' TODO: questa cosa non funzionava, non esisteva e fa riferimento a cose che solo gli antichi conoscono
                        Throw New DocSuiteException("Errore apertura link", String.Format("Problema con [ApriFattura]. {0}", ProtocolEnv.DefaultErrorMessage))
                    Case "Modifica"
                        Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(Anno), Integer.Parse(Numero))
                        If currentProtocol Is Nothing Then
                            Throw New DocSuiteException("Protocollo non trovato nella AOO corrente")
                        End If
                        Return $"Prot/ProtModifica.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={currentProtocol.Id}&Type=Prot")}"
                    Case "Interop"
                        Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(Anno), Integer.Parse(Numero))
                        If currentProtocol Is Nothing Then
                            Throw New DocSuiteException("Protocollo non trovato nella AOO corrente")
                        End If
                        Return $"Prot/ProtInterop.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={currentProtocol.Id}&Type=Prot")}"
                    Case "Collegamenti"
                        Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(Short.Parse(Anno), Integer.Parse(Numero))
                        If currentProtocol Is Nothing Then
                            Throw New DocSuiteException("Protocollo non trovato nella AOO corrente")
                        End If
                        Return $"Prot/ProtCollegamenti.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={currentProtocol.Id}&Type=Prot")}"
                    Case "Cerca"
                        Return "Prot/ProtRicerca.aspx"
                    Case "Ricerca"
                        Dim tipologia As String
                        Select Case Request.QueryString("Tipologia")
                            Case "I" : tipologia = "-1"
                            Case "U" : tipologia = "1"
                            Case Else : tipologia = ""
                        End Select
                        Dim parameters As String = String.Format("Year={0}&Number={1}&RegistrationDate_From={2}&RegistrationDate_To={3}&idType={4}&Recipient={5}&ObjectP={6}",
                                                        Anno,
                                                        Numero,
                                                        Request.QueryString("DaDataRegistrazione"),
                                                        Request.QueryString("ADataRegistrazione"),
                                                        tipologia,
                                                        Request.QueryString("MittenteDestinatario"),
                                                        Request.QueryString("Oggetto"))
                        Return "Prot/ProtRisultati.aspx?" & CommonShared.AppendSecurityCheck(parameters)
                End Select
            Case "fasc"
                Select Case customAzione
                    Case "Documenti"
                        Dim parameters As String = String.Format("Type=Fasc&IdFascicle={0}&DefaultDocumentId={1}", IdFascicle, DefaultDocumentId)
                        Return String.Format("Viewers/FascicleViewer.aspx?{0}", CommonShared.AppendSecurityCheck(parameters))
                    Case "Inserimento"
                        Return String.Concat($"Fasc/{If(ProtocolEnv.ProcessEnabled, "FascProcessInserimento.aspx", "FascInserimento.aspx")}?", CommonShared.AppendSecurityCheck("Type=Fasc&Action=Insert"))
                    Case "Apri"
                        Dim fascicleId As Guid
                        If Anno IsNot Nothing AndAlso Numero IsNot Nothing Then
                            Dim fascicleFinder As FascicleFinder = New FascicleFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .Year = Integer.Parse(Anno),
                                .FascicleTitle = Numero,
                                .EnablePaging = False
                            }
                            Dim fascicleDto As WebAPIDto(Of Entity.Fascicles.Fascicle) = fascicleFinder.DoSearch().FirstOrDefault()
                            If fascicleDto IsNot Nothing Then
                                fascicleId = fascicleDto.Entity.UniqueId
                            Else
                                Throw New DocSuiteException($"Fascicolo con l'anno {Anno} e numero {Numero} non trovato")
                            End If
                        ElseIf Metadato IsNot Nothing AndAlso Valore IsNot Nothing Then
                            Dim metadataRepositoryFinder As MetadataRepositoryFinder = New MetadataRepositoryFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .MetadataKeyName = Metadato,
                                .EnablePaging = False
                            }
                            Dim metadataRepositories As List(Of Entity.Commons.MetadataRepository) = metadataRepositoryFinder.DoSearch().Select(Function(x) x.Entity).ToList()
                            Dim fascicleFinder As FascicleFinder = New FascicleFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .EnablePaging = False
                            }
                            Dim metadataRepositoryId As Guid? = Nothing
                            For Each metadataRepository As Entity.Commons.MetadataRepository In metadataRepositories
                                fascicleFinder.IdMetadataRepository = metadataRepository.UniqueId
                                Dim fascicle As WebAPIDto(Of Entity.Fascicles.Fascicle) = fascicleFinder.DoSearch().FirstOrDefault()
                                If fascicle IsNot Nothing Then
                                    metadataRepositoryId = metadataRepository.UniqueId
                                    Exit For
                                End If
                            Next
                            fascicleFinder.IdMetadataRepository = Nothing
                            fascicleFinder.FascicleFinderModel = New FascicleFinderModel() With {
                                .MetadataValues = New List(Of MetadataFinderModel) From {
                                    New MetadataFinderModel With {
                                        .KeyName = Metadato,
                                        .Value = Valore
                                    }
                                },
                                .IdMetadataRepository = metadataRepositoryId,
                                .Top = 1,
                                .ApplySecurity = True,
                                .FascicleStatus = 1,
                                .SubjectSearchStrategy = 1
                            }
                            fascicleFinder.CustomPageIndex = 0
                            fascicleFinder.PageSize = 1
                            If Not String.IsNullOrEmpty(Anno) Then
                                fascicleFinder.FascicleFinderModel.Year = Short.Parse(Anno)
                            End If
                            Dim fascicleDto As WebAPIDto(Of FascicleModel) = fascicleFinder.GetFromPostMethod().FirstOrDefault()
                            If fascicleDto IsNot Nothing Then
                                fascicleId = fascicleDto.Entity.UniqueId
                            Else
                                Throw New DocSuiteException($"Fascicolo con metadato {Metadato} e valore {Valore} non trovato")
                            End If
                        Else
                            fascicleId = IdFascicle
                        End If
                        Dim extension As String = String.Empty
                        Dim claim As String = String.Empty
                        Return String.Concat("Fasc/FascVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Concat("Type=Fasc&IdFascicle=", fascicleId, claim)))
                    Case "Modifica"
                        Return String.Concat("Fasc/FascModifica.aspx?", CommonShared.AppendSecurityCheck(String.Format("Type=Fasc&IdFascicle={0}", IdFascicle)))
                    Case "Cerca"
                        Return "Fasc/FascRicerca.aspx"
                End Select
            Case "resl"
                Select Case customAzione.ToLower()
                    Case "apri"
                        Return String.Format("Resl/{0}?{1}", ReslBasePage.GetViewPageName(Integer.Parse(TargetId)), CommonShared.AppendSecurityCheck("Type=Resl&idResolution=" & TargetId))
                    Case "cerca"
                        Return "Resl/ReslRicerca.aspx"
                    Case "firmaultimapagina"
                        Return String.Concat("Resl/ReslFirmaUltimaPagina.aspx?", CommonShared.AppendSecurityCheck("Type=Resl"))
                End Select
            Case "comm"
                Select Case customAzione
                End Select
            Case "documentseries"
                Select Case customAzione
                    Case "Apri"
                        Return "Series/Item.aspx?" & CommonShared.AppendSecurityCheck("IdDocumentSeriesItem=" & TargetDocumentSeriesId & "&Action=View&Type=Series")
                End Select
            Case "uds"
                Select Case customAzione
                    Case "Apri"
                        Return String.Concat("UDS/UDSView.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdUDSRepository={0}&IdUDS={1}&Action=View&Type=UDS", IdUDSRepository, IdUDS)))
                End Select
            Case "dossier"
                Dim url As String
                Dim dossierVisualizzaUrl As String = "Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier={0}&DossierTitle={1}"
                Select Case customAzione
                    Case "Apri"
                        Dim idDossier, dossierTitle As String
                        If Not String.IsNullOrEmpty(Anno) AndAlso Not String.IsNullOrEmpty(Numero) Then
                            Dim dossierFinder As DossierFinder = New DossierFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .Year = Integer.Parse(Anno),
                                .Number = Integer.Parse(Numero),
                                .EnablePaging = False
                            }
                            Dim dossierDto As WebAPIDto(Of Dossier) = dossierFinder.DoSearch().FirstOrDefault()
                            If dossierDto IsNot Nothing Then
                                idDossier = dossierDto.Entity.UniqueId.ToString()
                                dossierTitle = $"{dossierDto.Entity.Year}/{dossierDto.Entity.Number:0000000}"
                            Else
                                Throw New DocSuiteException($"Dossier con l'anno {Anno} e numero {Numero} non trovato")
                            End If
                        ElseIf Not String.IsNullOrEmpty(Metadato) AndAlso Not String.IsNullOrEmpty(Valore) Then
                            Dim metadataRepositoryFinder As MetadataRepositoryFinder = New MetadataRepositoryFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .MetadataKeyName = Metadato,
                                .EnablePaging = False
                            }
                            Dim metadataRepositories As List(Of Entity.Commons.MetadataRepository) = metadataRepositoryFinder.DoSearch().Select(Function(x) x.Entity).ToList()
                            Dim dossierFinder As DossierFinder = New DossierFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .EnablePaging = False
                            }
                            Dim metadataRepositoryId As Guid? = Nothing
                            For Each metadataRepository As Entity.Commons.MetadataRepository In metadataRepositories
                                dossierFinder.IdMetadataRepository = metadataRepository.UniqueId
                                Dim dossier As WebAPIDto(Of Entity.Dossiers.Dossier) = dossierFinder.DoSearch().FirstOrDefault()
                                If dossier IsNot Nothing Then
                                    metadataRepositoryId = metadataRepository.UniqueId
                                    Exit For
                                End If
                            Next
                            dossierFinder.IdMetadataRepository = Nothing
                            dossierFinder.DossierFinderModel = New DossierFinderModel() With {
                                .MetadataValues = New List(Of MetadataFinderModel) From {
                                    New MetadataFinderModel With {
                                        .KeyName = Metadato,
                                        .Value = Valore
                                    }
                                },
                                .IdMetadataRepository = metadataRepositoryId,
                                .Top = 1
                            }
                            dossierFinder.CustomPageIndex = 0
                            dossierFinder.PageSize = 1
                            If Not String.IsNullOrEmpty(Anno) Then
                                dossierFinder.DossierFinderModel.Year = Short.Parse(Anno)
                            End If
                            Dim dossierDto As WebAPIDto(Of DossierModel) = dossierFinder.GetFromPostMethod().FirstOrDefault()
                            If dossierDto IsNot Nothing Then
                                idDossier = dossierDto.Entity.UniqueId.ToString()
                                dossierTitle = $"{dossierDto.Entity.Year}/{dossierDto.Entity.Number:0000000}"
                            Else
                                Throw New DocSuiteException($"Dossier con metadato {Metadato} e valore {Valore} non trovato")
                            End If
                        Else
                            idDossier = Request.QueryString("IdDossier")
                            dossierTitle = Request.QueryString("DossierTitle")
                        End If
                        url = String.Format(dossierVisualizzaUrl, idDossier, dossierTitle)
                    Case "ApriConFascicolo"
                        If Not String.IsNullOrEmpty(Metadato) AndAlso Not String.IsNullOrEmpty(Valore) AndAlso Not String.IsNullOrEmpty(Fascicolo) Then
                            Dim metadataRepositoryFinder As MetadataRepositoryFinder = New MetadataRepositoryFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .MetadataKeyName = Metadato,
                                .EnablePaging = False
                            }
                            Dim metadataRepositories As List(Of Entity.Commons.MetadataRepository) = metadataRepositoryFinder.DoSearch().Select(Function(x) x.Entity).ToList()
                            Dim dossierFinder As DossierFinder = New DossierFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .EnablePaging = False
                            }
                            Dim metadataRepositoryId As Guid? = Nothing
                            For Each metadataRepository As Entity.Commons.MetadataRepository In metadataRepositories
                                dossierFinder.IdMetadataRepository = metadataRepository.UniqueId
                                Dim dossier As WebAPIDto(Of Entity.Dossiers.Dossier) = dossierFinder.DoSearch().FirstOrDefault()
                                If dossier IsNot Nothing Then
                                    metadataRepositoryId = metadataRepository.UniqueId
                                    Exit For
                                End If
                            Next
                            dossierFinder.IdMetadataRepository = Nothing
                            dossierFinder.DossierFinderModel = New DossierFinderModel() With {
                                .MetadataValues = New List(Of MetadataFinderModel) From {
                                    New MetadataFinderModel With {
                                        .KeyName = Metadato,
                                        .Value = Valore
                                    }
                                },
                                .IdMetadataRepository = metadataRepositoryId,
                                .Top = 1
                            }
                            dossierFinder.CustomPageIndex = 0
                            dossierFinder.PageSize = 1
                            If Not String.IsNullOrEmpty(Anno) Then
                                dossierFinder.DossierFinderModel.Year = Short.Parse(Anno)
                            End If
                            Dim dossierDto As WebAPIDto(Of DossierModel) = dossierFinder.GetFromPostMethod().FirstOrDefault()
                            If dossierDto Is Nothing Then
                                Throw New DocSuiteException($"Dossier con metadato {Metadato}, valore {Valore} e fascicolo {Fascicolo} non trovato")
                            End If
                            Dim fascicleFinder As FascicleFinder = New FascicleFinder(DocSuiteContext.Current.CurrentTenant) With {
                                .Dossier = dossierDto.Entity.UniqueId,
                                .FascicleSubject = Fascicolo,
                                .EnablePaging = False
                            }
                            Dim fascicleDto As WebAPIDto(Of Entity.Fascicles.Fascicle) = fascicleFinder.DoSearch().FirstOrDefault()
                            If fascicleDto IsNot Nothing Then
                                url = $"{String.Format(dossierVisualizzaUrl, dossierDto.Entity.UniqueId.ToString(), dossierDto.Entity.Title)}&IdFascicle={fascicleDto.Entity.UniqueId}"
                            Else
                                Throw New DocSuiteException($"Dossier con metadato {Metadato}, valore {Valore} e fascicolo {Fascicolo} non trovato")
                            End If
                        End If
                End Select
                Return url
            Case "workflow"
                Select Case customAzione
                    Case "Cerca"
                        Dim parameters As New List(Of String) From {
                            "Type=Comm"
                        }
                        If Not String.IsNullOrEmpty(RepositoryName) Then
                            parameters.Add($"RepositoryName={RepositoryName}")
                        End If
                        Return $"User/UserWorkflow.aspx?{CommonShared.AppendSecurityCheck(String.Join("&", parameters))}"

                End Select
        End Select

        Return String.Empty

    End Function

    Private Sub CreateMenu()

        Dim menuJson As IDictionary(Of String, MenuNodeModel) = DocSuiteContext.Current.DocSuiteMenuConfiguration

        'Utenti - visualizzazione pannello - Usr
        CreateMenuPersonalUser(menuJson)

        'Tavoli - visualizzazione pannello 
        CreateMenuDesks(menuJson)

        'Collaborazione - visualizzazione pannello - Coll
        CreateMenuCollaborations(menuJson)

        'Pratiche - visualizzazione pannello - Docm
        CreateMenuPratiche(menuJson)

        'Protocollo - visualizzazione pannello - Prot
        CreateMenuProtocols(menuJson)

        ' Serie Documentali e Repertori
        CreateMenuDocumentSeriesArchives(menuJson)

        'Fascicoli - visualizzazione pannello - Fasc
        CreateMenuDossiersAndFascicles(menuJson)

        'Fatture - visualizzazione pannello
        CreateMenuInvoice(menuJson)

        'Atti - visualizzazione pannello - Resl
        CreateMenuResolutions(menuJson)

        ' PEC non integrata, solo per utenti abilitati e per amministratori
        CreateMenuPec(menuJson)

        'Tabelle - visualizzazione pannello - Tbl
        CreateMenuTables(menuJson)

        'Amministrazione - visualizzazione pannello - Admin
        CreateMenuAdmins(menuJson)

    End Sub

    Private Sub CreateMenuAdmins(menuJson As IDictionary(Of String, MenuNodeModel))
        If CommonShared.HasGroupAdministratorRight OrElse CommonUtil.HasGroupSuspendRight AndAlso menuJson.Keys.Contains("Menu12") Then
            Dim menu As MenuNodeModel = menuJson("Menu12")
            Dim alMenu As New List(Of NodeLink)

            If menu.Nodes.Keys.Contains("FirstNode1") Then
                alMenu.Add(New NodeLink(
                           NodeLevel.First,
                           If(SuperAdminAuthored, NodeImage.Line, NodeImage.Point),
                               menu.Nodes("FirstNode1").Name,
                           "Utlt/SuperAdmin.aspx",
                           CommonShared.AppendSecurityCheck("Type=Comm&Page=SuperAdmin")))

                If SuperAdminAuthored Then
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode1") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode1").Name, "Utlt/UtltParameter.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Parameter")))
                    End If
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode3") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode3").Name, "Utlt/UtltCheckDBMapping.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=CheckDBMapping")))
                    End If
                    If ProtocolEnv.EnvUtltRenderingEnabled AndAlso menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode6") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode6").Name, "Utlt/UtltRenderingDocument.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Document")))
                    End If
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode7") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode7").Name, "Utlt/UtltSmtpMail.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Mail")))
                    End If
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode8") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode8").Name, "Utlt/UtltTrace.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Trace")))
                    End If
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode11") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode11").Name, "Utlt/UtltTestSignature.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                    End If
                End If
            End If


            If menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "Utlt/UtltUserLog.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Users")))
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "Utlt/UtltUsers.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Users")))
            End If
            If menu.Nodes.Keys.Contains("FirstNode4") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode4").Name, "Utlt/UtltADProperties.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
            End If
            If menu.Nodes.Keys.Contains("FirstNode5") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode5").Name))
                If menu.Nodes("FirstNode5").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode5").Nodes("SecondNode1").Name, "Utlt/UtltUserGroup.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
            End If
            If menu.Nodes.Keys.Contains("FirstNode6") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode6").Name, "Utlt/UtltConfig.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Config")))
            End If
            If ProtocolEnv.IsPECEnabled Then
                If menu.Nodes.Keys.Contains("FirstNode8") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode8").Name))
                    If menu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode8").Nodes("SecondNode2").Name, "PEC/PECMailBoxProfileSettings.aspx", "Type=Pec"))
                    End If
                    If menu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode3") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode8").Nodes("SecondNode3").Name, "PEC/PECMailBoxJeepService.aspx", "Type=Pec"))
                    End If
                End If
                If menu.Nodes.Keys.Contains("FirstNode9") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode9").Name))
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode1") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode1").Name, "PEC/PECMailBoxModuliMonitor.aspx", "Type=Pec"))
                    End If
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode2").Name, "PEC/PECMailBoxCaselleMonitor.aspx", "Type=Pec"))
                    End If
                End If
            End If

            If menu.Nodes.Keys.Contains("FirstNode10") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode10").Name))
                If menu.Nodes("FirstNode10").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode10").Nodes("SecondNode2").Name, "Utlt/UtltContatori.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Contatori")))
                End If
                If ProtocolEnv.ChangeYearDocumentSeriesEnabled AndAlso menu.Nodes("FirstNode10").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode10").Nodes("SecondNode3").Name, "Utlt/UtltDocumentSeries.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=ContatoriDocumentSeries")))
                End If
            End If

            If menu.Nodes.Keys.Contains("FirstNode12") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode12").Name))
                If DocSuiteContext.Current.IsProtocolEnabled AndAlso menu.Nodes("FirstNode12").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode12").Nodes("SecondNode1").Name, "Utlt/UtltLog.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Prot")))
                End If
                If DocSuiteContext.Current.IsDocumentEnabled AndAlso DocumentEnv.IsEnvLogEnabled AndAlso menu.Nodes("FirstNode12").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode12").Nodes("SecondNode2").Name, "Utlt/UtltLog.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Docm")))
                End If
                If DocSuiteContext.Current.IsResolutionEnabled AndAlso ResolutionEnv.IsLogEnabled Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, Facade.TabMasterFacade.TreeViewCaption, "Utlt/UtltLog.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Resl")))
                End If
                If ProtocolEnv.EnvTableLogEnabled AndAlso menu.Nodes("FirstNode12").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode12").Nodes("SecondNode3").Name, "Utlt/UtltLog.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Tblt")))
                End If
                If ProtocolEnv.IsUserErrorEnabled AndAlso menu.Nodes("FirstNode12").Nodes.Keys.Contains("SecondNode5") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode12").Nodes("SecondNode5").Name, "Utlt/UtltUserError.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
            End If
            If menu.Nodes.Keys.Contains("FirstNode13") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode13").Name))
                If DocSuiteContext.Current.IsDocumentEnabled AndAlso DocumentEnv.IsEnvLogEnabled AndAlso menu.Nodes("FirstNode13").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode13").Nodes("SecondNode1").Name, "Utlt/UtltUserStatistics.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Docm")))
                End If
                If DocSuiteContext.Current.IsProtocolEnabled AndAlso menu.Nodes("FirstNode13").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode13").Nodes("SecondNode2").Name, "Utlt/UtltUserStatistics.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Prot")))
                End If
                If DocSuiteContext.Current.IsResolutionEnabled AndAlso ResolutionEnv.IsLogEnabled Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, Facade.TabMasterFacade.TreeViewCaption & " Stat.", "Utlt/UtltUserStatistics.aspx", CommonShared.AppendSecurityCheck("Type=Comm&LogType=Resl")))
                End If
            End If
            If menu.Nodes.Keys.Contains("FirstNode14") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode14").Name))
                If menu.Nodes("FirstNode14").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode14").Nodes("SecondNode1").Name, "Utlt/ComputerLogManagement.aspx", "Type=Prot"))
                End If

                If ProtocolEnv.ScannerConfigurationEnabled AndAlso menu.Nodes("FirstNode14").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode14").Nodes("SecondNode2").Name, "Utlt/ScannerManagement.aspx", "Type=Prot"))
                End If
                If menu.Nodes("FirstNode14").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode14").Nodes("SecondNode4").Name, "Utlt/ResetParameters.aspx", "Type=Prot"))
                End If
            End If

            If CommonUtil.HasGroupSuspendRight AndAlso menu.Nodes.Keys.Contains("FirstNode15") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode15").Name, "Utlt/UtltProtSospendi.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Action=Utlt")))
            End If

            Dim amministrazionePanel As RadPanelItem = RadPanelBarMenu.FindItemByValue("AmministrazioneItem")
            amministrazionePanel.Text = menu.Name
            amministrazionePanel.Items.Clear()
            amministrazionePanel.Items.Add(AddTreeViewItem("RadTreeAmministrazione", alMenu, main.ClientID))
            amministrazionePanel.Visible = True
        End If
    End Sub

    Private Sub CreateMenuTables(menuJson As IDictionary(Of String, MenuNodeModel))
        If menuJson.Keys.Contains("Menu11") Then

            Dim tableMenu As MenuNodeModel = menuJson("Menu11")
            Dim tbltMenu As New List(Of NodeLink)

            If (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight OrElse CommonShared.HasGroupTblRoleAdminRight OrElse CommonShared.HasGroupTblRoleRight OrElse CommonShared.HasSecurityGroupAdminRight OrElse CommonShared.HasSecurityGroupPowerUserRight OrElse CommonShared.HasGroupOChartAdminRight) AndAlso tableMenu.Nodes.Keys.Contains("FirstNode1") Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode1").Name))

                If (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight OrElse CommonShared.HasSecurityGroupPowerUserRight) Then  'oppure responsabile privacy
                    If tableMenu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode1") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode1").Nodes("SecondNode1").Name, "Tblt/TbltSecurityUsers.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                    End If
                    If tableMenu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode2") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode1").Nodes("SecondNode2").Name, "Tblt/TbltGruppi.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                    End If
                End If

                If (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleRight OrElse CommonShared.HasGroupTblRoleAdminRight) Then
                    If tableMenu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode3") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode1").Nodes("SecondNode3").Name, "Tblt/TbltSettore.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Action=Role")))
                    End If
                End If

                If (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
                    If tableMenu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode4") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode1").Nodes("SecondNode4").Name, "Tblt/TbltContenitori.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Action=Container")))
                    End If
                End If

                If ProtocolEnv.OChartEnabled AndAlso CommonShared.HasGroupOChartAdminRight Then
                    If tableMenu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode5") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode1").Nodes("SecondNode5").Name, "Tblt/TbltOChart.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                    End If
                End If

                If (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
                    If tableMenu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode6") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode1").Nodes("SecondNode6").Name, "Tblt/TbltLocation.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                    End If
                End If

            End If

            If tableMenu.Nodes.Keys.Contains("FirstNode8") AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupProcessesViewsManageableRight OrElse CommonShared.HasGroupTblCategoryRight) Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode8").Name))
                If DocSuiteContext.Current.ProtocolEnv.MultiTenantEnabled AndAlso CommonShared.HasGroupAdministratorRight Then
                    If tableMenu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode1") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode8").Nodes("SecondNode1").Name, "Tblt/TbltTenant.aspx", CommonUtil.AppendSecurityCheck("Type=Comm")))
                    End If
                End If
                If ProtocolEnv.ProcessEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupProcessesViewsManageableRight) Then
                    If tableMenu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode2") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode8").Nodes("SecondNode2").Name, "Tblt/TbltProcess.aspx", "Type=Comm"))
                    End If
                End If
                If tableMenu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode3") AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblCategoryRight) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode8").Nodes("SecondNode3").Name, "Tblt/TbltClassificatore.aspx", "Type=Comm"))
                End If
                If tableMenu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode4") AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblCategoryRight) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode8").Nodes("SecondNode4").Name, "Tblt/TbltMassimarioScartoGes.aspx", "Type=Comm"))
                End If
                If tableMenu.Nodes("FirstNode8").Nodes.Keys.Contains("SecondNode5") AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblCategoryRight) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode8").Nodes("SecondNode5").Name, "Tblt/TbltCategorySchema.aspx", "Type=Comm"))
                End If
            End If

            If tableMenu.Nodes.Keys.Contains("FirstNode26") AndAlso CommonShared.HasGroupAdministratorRight AndAlso ProtocolEnv.IsPECEnabled Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode26").Name))
                If tableMenu.Nodes("FirstNode26").Nodes.Keys.Contains("SecondNode1") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode26").Nodes("SecondNode1").Name, "Tblt/TbltPECMailBox.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
                If ProtocolEnv.SMSPecNotificationEnabled AndAlso tableMenu.Nodes("FirstNode26").Nodes.Keys.Contains("SecondNode2") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode26").Nodes("SecondNode2").Name, "Tblt/TbltSMSPecNotification.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
                End If

            End If

            If tableMenu.Nodes.Keys.Contains("FirstNode9") AndAlso ProtocolEnv.IsInteropEnabled Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode9").Name))
                If tableMenu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode1") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode9").Nodes("SecondNode1").Name, "Tblt/TbltContatti.aspx", "Type=Comm"))
                End If
                If ProtocolEnv.ContactListsEnabled AndAlso tableMenu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode2") AndAlso CommonShared.HasGroupAdministratorRight Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode9").Nodes("SecondNode2").Name, "Tblt/TbltContactLists.aspx", "Type=Comm"))
                End If
                If tableMenu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode3") AndAlso CommonShared.HasGroupAdministratorRight Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode9").Nodes("SecondNode3").Name, "Tblt/TbltTitoloStudio.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
                End If
                If tableMenu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode4") AndAlso CommonShared.HasGroupAdministratorRight Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode9").Nodes("SecondNode4").Name, "Tblt/TbltOggetto.aspx", "Type=Comm"))
                End If
                If tableMenu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode5") AndAlso CommonShared.HasGroupAdministratorRight Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode9").Nodes("SecondNode5").Name, "Tblt/TbltServiceCategory.aspx", "Type=Comm"))
                End If
                If ProtocolEnv.IsTableDocTypeEnabled AndAlso tableMenu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode6") AndAlso CommonShared.HasGroupAdministratorRight Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode9").Nodes("SecondNode6").Name, "Tblt/TbltTipoDoc.aspx", "Type=Comm"))
                End If

            End If

            If CommonShared.HasGroupAdministratorRight AndAlso ProtocolEnv.UDSEnabled AndAlso tableMenu.Nodes.Keys.Contains("FirstNode7") Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode7").Name))
                If tableMenu.Nodes("FirstNode7").Nodes.Keys.Contains("SecondNode1") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode7").Nodes("SecondNode1").Name, "UdsDesigner/Designer.aspx", String.Empty))
                End If
                If tableMenu.Nodes("FirstNode7").Nodes.Keys.Contains("SecondNode2") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode7").Nodes("SecondNode2").Name, "UdsDesigner/DesignerResults.aspx", String.Empty))
                End If
                If tableMenu.Nodes("FirstNode7").Nodes.Keys.Contains("SecondNode3") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode7").Nodes("SecondNode3").Name, "Tblt/TbltUDSTypology.aspx", String.Empty))
                End If
            End If

            If tableMenu.Nodes.Keys.Contains("FirstNode21") AndAlso (ProtocolEnv.TemplateProtocolEnable OrElse CommonShared.HasGroupAdministratorRight OrElse
                CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups) OrElse CommonShared.HasGroupTblCategoryRight OrElse CommonShared.HasGroupTblRoleTypeResolutionRight OrElse
                CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateDocumentGroups)) Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode21").Name))
                If ProtocolEnv.WorkflowManagerEnabled AndAlso tableMenu.Nodes("FirstNode21").Nodes.Keys.Contains("SecondNode1") AndAlso CommonShared.HasGroupAdministratorRight Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode21").Nodes("SecondNode1").Name, "Tblt/TbltWorkflowRepository.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
                If ProtocolEnv.TemplateProtocolEnable AndAlso tableMenu.Nodes("FirstNode21").Nodes.Keys.Contains("SecondNode2") AndAlso CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateGroups) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode21").Nodes("SecondNode2").Name, "Tblt/TbltTemplateProtocolManager.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
                End If
                If tableMenu.Nodes("FirstNode21").Nodes.Keys.Contains("SecondNode3") AndAlso CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode21").Nodes("SecondNode3").Name, "Tblt/TbltTemplateCollaboration.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
                If tableMenu.Nodes("FirstNode21").Nodes.Keys.Contains("SecondNode4") AndAlso DocSuiteContext.Current.IsResolutionEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleTypeResolutionRight) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode21").Nodes("SecondNode4").Name, "Resl/ReslTipologia.aspx", CommonShared.AppendSecurityCheck("Type=Resl")))
                End If
                If tableMenu.Nodes("FirstNode21").Nodes.Keys.Contains("SecondNode5") AndAlso (CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateDocumentGroups)) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode21").Nodes("SecondNode5").Name, "Tblt/TbltTemplateDocumentRepository.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
                If tableMenu.Nodes("FirstNode21").Nodes.Keys.Contains("SecondNode6") AndAlso ProtocolEnv.MetadataRepositoryEnabled AndAlso CommonShared.UserConnectedBelongsTo(ProtocolEnv.MetadataRepositoryGroups) Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode21").Nodes("SecondNode6").Name, "Tblt/TbltMetadataRepository.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
            End If

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblPrivacyLevelManagerGroupRight AndAlso tableMenu.Nodes.Keys.Contains("FirstNode25")) Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, tableMenu.Nodes("FirstNode25").Name, "Tblt/TbltPrivacyLevel.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
            End If

            If ProtocolEnv.IsPackageEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)) AndAlso tableMenu.Nodes.Keys.Contains("FirstNode14") Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, tableMenu.Nodes("FirstNode14").Name, "Prot/ProtPackage.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
            End If

            If tableMenu.Nodes.Keys.Contains("FirstNode15") Then
                tbltMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, tableMenu.Nodes("FirstNode15").Name))
                If tableMenu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode1") Then
                    tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode15").Nodes("SecondNode1").Name, "Tblt/TbltClassificatorePrint.aspx", "Type=Comm"))
                End If
                If CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblStampeSecurityRight OrElse CommonShared.HasGroupTblStampeRight Then
                    If tableMenu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode2") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode15").Nodes("SecondNode2").Name, "Tblt/TbltRolesPrint.aspx", "Type=Comm"))
                    End If
                    If tableMenu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode3") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode15").Nodes("SecondNode3").Name, "Tblt/TbltContainersWithSecutiryPrint.aspx", "Type=Comm"))
                    End If
                End If

                If CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblStampeSecurityRight Then
                    If tableMenu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode4") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode15").Nodes("SecondNode4").Name, "Tblt/TbltRolesPrint.aspx", "Type=Comm&IsSecurity=True"))
                    End If
                    If tableMenu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode5") Then
                        tbltMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, tableMenu.Nodes("FirstNode15").Nodes("SecondNode5").Name, "Tblt/TbltContainersWithSecutiryPrint.aspx", "Type=Comm&IsSecurity=True"))
                    End If
                End If
            End If

            Dim tabelleItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("TabelleItem")
            tabelleItem.Text = tableMenu.Name
            tabelleItem.Items.Clear()
            tabelleItem.Items.Add(AddTreeViewItem("RadTreeTabelle", tbltMenu, main.ClientID))
            tabelleItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuPec(menuJson As IDictionary(Of String, MenuNodeModel))
        If ProtocolEnv.IsPECEnabled AndAlso menuJson.Keys.Contains("Menu10") Then
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.PECMail, DomainUserFacade.HasInsertable)
            Dim menu As MenuNodeModel = menuJson("Menu10")
            Dim alMenu As New List(Of NodeLink)
            If menu.Nodes.Keys.Contains("FirstNode1") AndAlso hasInsertable Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "PEC/PECInsert.aspx", String.Format("Type=Pec&SimpleMode={0}&RedirectTo={1}", ProtocolEnv.PECSimpleMode.ToString(), Server.UrlEncode("PECOutgoingMails.aspx?Type=Pec"))))
            End If
            If menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "PEC/PECIncomingMails.aspx", "Type=Pec"))
            End If
            If menu.Nodes.Keys.Contains("FirstNode3") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "PEC/PECOutgoingMails.aspx", "Type=Pec"))
            End If

            If ProtocolEnv.PECExternalTrashbin AndAlso menu.Nodes.Keys.Contains("FirstNode4") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode4").Name, "PEC/PECIncomingMails.aspx", "Type=Pec&ExternalTrashbin=True"))
            End If
            If ProtocolEnv.PECFromFile AndAlso menu.Nodes.Keys.Contains("FirstNode5") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode5").Name, "PEC/PECViewFromFile.aspx", CommonShared.AppendSecurityCheck("Type=Pec&Action=Insert")))
            End If
            If CommonShared.HasGroupAdministratorRight Then
                If menu.Nodes.Keys.Contains("FirstNode6") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode6").Name, "Prot/Stampe/ReportisticaPEC.aspx", CommonShared.AppendSecurityCheck("Type=Pec")))
                End If
                If menu.Nodes.Keys.Contains("FirstNode7") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode7").Name, "PEC/PECMailBoxLog.aspx", "Type=Pec"))
                End If
            End If

            Dim pecItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("PECItem")
            pecItem.Text = menu.Name
            pecItem.Items.Clear()
            pecItem.Items.Add(AddTreeViewItem("RadTreePEC", alMenu, main.ClientID))
            pecItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuResolutions(menuJson As IDictionary(Of String, MenuNodeModel))
        If DocSuiteContext.Current.IsResolutionEnabled AndAlso menuJson.Keys.Contains("Menu9") Then
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Resolution, DomainUserFacade.HasInsertable)
            Dim menu As MenuNodeModel = menuJson("Menu9")
            Dim alMenu As New List(Of NodeLink)
            If menu.Nodes.Keys.Contains("FirstNode1") AndAlso hasInsertable Then
                If ResolutionEnv.InsertDisclaimer = "1" Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "Resl/ReslDisclaimer.aspx", CommonShared.AppendSecurityCheck("Type=Resl")))
                Else
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "Resl/ReslInserimento.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Insert")))
                End If
            End If
            If menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "Resl/ReslRicerca.aspx", "Type=Resl"))
            End If
            If CommonShared.ResolutionPECOCEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode5") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode5").Name, "Resl/ReslPecOc.aspx", CommonShared.AppendSecurityCheck("Type=Resl")))
            End If
            If ResolutionEnv.ShowMassiveResolutionSearchPageEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode6") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode6").Name, "Resl/ReslRicercaFlusso.aspx", "Type=Resl"))
            End If
            If ResolutionEnv.ShowMassiveResolutionSearchPageEnabled AndAlso CommonShared.HasGroupDigitalLastPageRight AndAlso ResolutionEnv.UltimaPaginaReportSignEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode7") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode7").Name, "Resl/ReslFirmaUltimaPagina.aspx", CommonShared.AppendSecurityCheck("Type=Resl")))
            End If
            If ResolutionEnv.ShowResolutionStatisticsEnabled AndAlso (CommonShared.HasGroupStatisticsReslolutionRight OrElse CommonShared.HasGroupAdministratorRight) AndAlso menu.Nodes.Keys.Contains("FirstNode8") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode8").Name, "Resl/ReslStatistiche.aspx", CommonShared.AppendSecurityCheck("Type=Resl")))
            End If

            If menu.Nodes.Keys.Contains("FirstNode9") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode9").Name))
                If (DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-To")) Then
                    'Frontalino
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode1") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode1").Name, "Resl/ReslPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print&PrintType=F")))
                    End If
                    '-- Print - Elenco provvedimenti adottati
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode2").Name, "Resl/ReslPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print&PrintType=EDA")))
                    End If
                    'Collegio
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode7") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode7").Name, "Resl/ReslProposedPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print")))
                    End If
                    'Lettere
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode3") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode3").Name, "Resl/ReslPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print&PrintType=LT")))
                    End If
                    'Registro
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode4") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode4").Name, "Resl/ReslPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print&PrintType=RG")))
                    End If
                Else
                    If menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode4") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode4").Name, "Resl/ReslRegistroPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print")))
                    End If
                End If

                If ResolutionEnv.WebPublicationPrint AndAlso menu.Nodes("FirstNode9").Nodes.Keys.Contains("SecondNode5") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode9").Nodes("SecondNode5").Name, "Resl/ReslPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Print&PrintType=REGPUB")))
                End If

            End If

            If (DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-To")) Then
                '-- Print - Promemoria
                If menu.Nodes.Keys.Contains("FirstNode10") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode10").Name))
                    If menu.Nodes("FirstNode10").Nodes.Keys.Contains("SecondNode1") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode10").Nodes("SecondNode1").Name, "Comm/CommPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&PrintName=ReslMemoPublicationPrint")))
                    End If
                    If menu.Nodes("FirstNode10").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode10").Nodes("SecondNode2").Name, "Comm/CommPrint.aspx", CommonShared.AppendSecurityCheck("Type=Resl&PrintName=ReslMemoExecutivePrint")))
                    End If
                End If
                If CommonUtil.HasGroupExtractionRight AndAlso menu.Nodes.Keys.Contains("FirstNode11") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode11").Name, "Resl/ReslEstrazione.aspx", CommonShared.AppendSecurityCheck("Type=Resl&Action=Extract")))
                End If
            End If

            If CommonShared.ResolutionJournalEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode12") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode12").Name, "Resl/ReslJournal.aspx", String.Format("Type=Resl&Group=RJ&TitleString={0}", menu.Nodes("FirstNode12").Name)))
            End If

            If CommonShared.ResolutionPublicationJournalEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode13") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode13").Name, "Resl/ReslJournal.aspx", String.Format("Type=Resl&Group=RP&TitleString={0}", menu.Nodes("FirstNode13").Name)))
            End If

            If CommonShared.RitiraAttiEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode14") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode14").Name, "Resl/ReslRitiraAtti.aspx", "Type=Resl"))
            End If

            ' Delibere e Determine: pubblicazione Web
            If ResolutionEnv.WebPublishEnabled And DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-To") AndAlso Not ResolutionEnv.AutomaticActivityStepEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode15") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode15").Name, "Resl/ReslPubblicaWeb.aspx", "Type=Resl"))
            End If

            If ProtocolEnv.MoveScrivaniaMenu AndAlso DocSuiteContext.Current.ProtocolEnv.DiaryFullEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode16") Then

                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Desktop, menu.Nodes("FirstNode16").Name))

                If ResolutionEnv.IsLogEnabled Then
                    If ResolutionEnv.Configuration.Eq("AUSL-PC") AndAlso menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode1") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode1").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode1").Name, "RY", UserDesktop.PageDiario, "Resl")), ""))
                    Else
                        If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode2") Then
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode2").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode2").Name, "RY", UserDesktop.PageDiario, "Prot")), ""))
                        End If
                    End If
                End If
                If ResolutionEnv.Configuration.Eq("AUSL-PC") Then

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode3") Then
                        If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                            Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Insert, Nothing)
                            If resolutionContainers IsNot Nothing AndAlso resolutionContainers.Count() > 0 Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode3").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode3").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_NOT_COMPLIANT, UserDesktop.PageScrivania, "Resl")), ""))
                            End If
                        Else
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode3").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode3").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_NOT_COMPLIANT, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode18") Then
                        If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                            Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Insert, Nothing)
                            If resolutionContainers IsNot Nothing AndAlso resolutionContainers.Count() > 0 Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode18").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode18").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_VERIFY, UserDesktop.PageScrivania, "Resl")), ""))
                            End If
                        Else
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode18").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode18").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_VERIFY, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode4") Then
                        If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                            Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Executive, Nothing)
                            If resolutionContainers IsNot Nothing AndAlso resolutionContainers.Count() > 0 Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode4").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode4").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_CHECK_COMPLIANT, UserDesktop.PageScrivania, "Resl")), ""))
                            End If
                        Else
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode4").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode4").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_CHECK_COMPLIANT, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode5") Then
                        If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                            Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Adoption, Nothing)
                            If resolutionContainers IsNot Nothing AndAlso resolutionContainers.Count() > 0 Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode5").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode5").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_COMPLIANT_TO_ADOPTION, UserDesktop.PageScrivania, "Resl")), ""))
                            End If
                        Else
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode5").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode5").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_COMPLIANT_TO_ADOPTION, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode6") Then
                        If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                            Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Administration, Nothing)
                            If resolutionContainers IsNot Nothing AndAlso resolutionContainers.Count() > 0 Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode6").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode6").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_ADOPTED, UserDesktop.PageScrivania, "Resl")), ""))
                            End If
                        Else
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode6").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode6").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_ADOPTED, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode7") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode7").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode7").Name, UserDesktop.ActionNameAdottateNonEsecutive, UserDesktop.PageScrivania, "Resl")), ""))
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode8") Then
                        If Not ResolutionEnv.ApplyGeneralistUserDeskRight Then
                            Dim resolutionContainers As IList(Of Container) = New ContainerFacade("ReslDB").GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Administration, Nothing)
                            If resolutionContainers IsNot Nothing AndAlso resolutionContainers.Count() > 0 Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode8").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode8").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_PUBLISHED, UserDesktop.PageScrivania, "Resl")), ""))
                            End If
                        Else
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode8").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode8").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_PUBLISHED, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If

                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode9") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode9").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode9").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_EXECUTIVE, UserDesktop.PageScrivania, "Resl")), ""))
                    End If
                Else
                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode10") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode10").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode10").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_ADOPTED, UserDesktop.PageScrivania, "Resl")), ""))
                    End If
                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode11") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode11").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode11").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_PUBLISHED, UserDesktop.PageScrivania, "Resl")), ""))
                    End If
                    If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode12") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode12").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode12").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_EXECUTIVE, UserDesktop.PageScrivania, "Resl")), ""))
                    End If
                    If (menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode19")) Then
                        Dim currentDeterminaTabMaster As TabMaster = Facade.TabMasterFacade.GetByConfigurationAndType(DocSuiteContext.Current.ResolutionEnv.Configuration, ResolutionType.IdentifierDetermina)
                        Dim affGenStep As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription(WorkflowStep.AFF_GEN_CHECK_STEP_DESCRIPTION, currentDeterminaTabMaster.WorkflowType)
                        If affGenStep IsNot Nothing Then
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode19").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode19").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_AFF_GEN, UserDesktop.PageScrivania, "Resl")), ""))
                        End If
                    End If
                    If (ResolutionEnv.ResolutionKindEnabled) AndAlso menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode13") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode13").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Atti per Amministrazione Trasparente", UserScrivaniaD.ACTION_NAME_RESOLUTION_AMM_TRASP, UserDesktop.PageScrivania, "Resl")), ""))
                    End If
                End If
                If ResolutionEnv.ViewResolutionProposedByRoleEnabled AndAlso menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode14") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode14").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode14").Name, "ResolutionProposedByRole", UserDesktop.PageScrivania, "Resl")), ""))
                End If
                If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode15") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode15").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode15").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_INSERT_TODAY, UserDesktop.PageScrivania, "Resl")), ""))
                End If
                If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode16") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode16").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode16").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_INSERT_THIS_WEEK, UserDesktop.PageScrivania, "Resl")), ""))
                End If
                If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNod17") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode17").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode16").Nodes("SecondNode17").Name, UserScrivaniaD.ACTION_NAME_RESOLUTION_INSERT_THIS_MONTH, UserDesktop.PageScrivania, "Resl")), ""))
                End If
            End If
            If ResolutionEnv.WebPublishEnabled And DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode17") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode17").Name, "Resl/ReslActivityResults.aspx", "Type=Resl"))
            End If
            Dim attiItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("AttiItem")
            'Dim titleText As String = "Resolutions"
            'If Not String.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.Configuration) Then
            '    titleText = Facade.TabMasterFacade.TreeViewCaption
            'End If
            attiItem.Text = menu.Name
            attiItem.Items.Clear()
            attiItem.Items.Add(AddTreeViewItem("RadTreeAtti", alMenu, main.ClientID))
            attiItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuDossiersAndFascicles(menuJson As IDictionary(Of String, MenuNodeModel))
        If DocSuiteContext.Current.IsProtocolEnabled AndAlso (ProtocolEnv.FascicleEnabled OrElse ProtocolEnv.IsIssueEnabled) AndAlso menuJson.Keys.Contains("Menu7") Then
            Dim menu As MenuNodeModel = menuJson("Menu7")
            Dim alMenu As New List(Of NodeLink)
            Dim hasInsertableDossier As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Dossier, DomainUserFacade.HasInsertable)
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Fascicle, DomainUserFacade.HasInsertable)
            Dim hasFascicleResponsibleRole As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Fascicle, DomainUserFacade.HasFascicleResponsibleRole)
            Dim hasFascicleSecretaryRole As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Fascicle, DomainUserFacade.HasFascicleSecretaryRole)

            If ProtocolEnv.ProcessEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupProcessesViewsManageableRight OrElse CommonShared.HasGroupProcessesViewsReadableRight) Then
                If menu.Nodes.Keys.Contains("FirstNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "User/ProcessesTreeView.aspx", CommonUtil.AppendSecurityCheck("Type=Comm")))
                End If
            End If

            If ProtocolEnv.DossierEnabled Then
                If menu.Nodes.Keys.Contains("FirstNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode1").Name))
                End If
                If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode1") AndAlso hasInsertableDossier Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode1").Name, "Dossiers/DossierInserimento.aspx", "Type=Dossier"))
                End If
                If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode2").Name, "Dossiers/DossierRicerca.aspx", "Type=Dossier"))
                End If

            End If

            Select Case True
                Case ProtocolEnv.FascicleEnabled
                    If menu.Nodes.Keys.Contains("FirstNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode2").Name))
                    End If

                    If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode1") AndAlso (hasInsertable OrElse hasFascicleSecretaryRole OrElse hasFascicleResponsibleRole) Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode1").Name, $"Fasc/{If(ProtocolEnv.ProcessEnabled, "FascProcessInserimento.aspx", "FascInserimento.aspx")}", CommonShared.AppendSecurityCheck("Type=Fasc&Action=Insert")))
                    End If

                    If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode2").Name, "Fasc/FascRicerca.aspx", "Type=Fasc"))
                    End If

                    If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode4") AndAlso (hasInsertable OrElse hasFascicleSecretaryRole OrElse hasFascicleResponsibleRole) Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode4").Name, "Fasc/FascPeriodInserimento.aspx", CommonShared.AppendSecurityCheck("Type=Fasc&Action=Insert")))
                    End If
                Case ProtocolEnv.IsIssueEnabled
                    If menu.Nodes.Keys.Contains("FirstNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode2").Name))
                    End If
                    If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode2").Name, "Comm/CommFascicoloRicerca.aspx", CommonShared.AppendSecurityCheck("Type=Fasc&Page=Users")))
                    End If
            End Select

            'Scrivania in Fascicoli
            If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode3") Then
                If ProtocolEnv.MoveScrivaniaMenu Then
                    If DocSuiteContext.Current.ProtocolEnv.DiaryFullEnabled Then
                        If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode3") Then
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, String.Concat("User/", UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, "FDF", UserDesktop.PageUDFascicle, "Fasc"))), ""))
                        End If
                    Else
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, String.Concat("User/", UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, "FDF", UserDesktop.PageUDFascicle, "Fasc"))), ""))
                    End If
                Else
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, String.Concat("User/", UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, "FDF", UserDesktop.PageUDFascicle, "Fasc"))), ""))
                End If
            End If

            If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode5") AndAlso ProtocolEnv.FascicleEnabled Then
                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode5").Name, "Fasc/FascicleClose.aspx", "Type=Fasc"))
            End If

            Dim fascicoliItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("FascicoliItem")
            fascicoliItem.Text = menu.Name
            fascicoliItem.Items.Clear()
            fascicoliItem.Items.Add(AddTreeViewItem("RadTreeFascicoli", alMenu, main.ClientID))
            fascicoliItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuDocumentSeriesArchives(menuJson As IDictionary(Of String, MenuNodeModel))
        If (ProtocolEnv.DocumentSeriesEnabled OrElse ProtocolEnv.UDSEnabled) AndAlso menuJson.Keys.Contains("Menu6") Then
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.DocumentSeries, DomainUserFacade.HasInsertable)
            Dim menu As MenuNodeModel = menuJson("Menu6")
            Dim alMenu As New List(Of NodeLink)

            If menu.Nodes.Keys.Contains("FirstNode1") AndAlso ProtocolEnv.DocumentSeriesEnabled Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode1").Name))
                If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode1") AndAlso hasInsertable Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode1").Name, "Series/Item.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If
                If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode2").Name, "Series/Search.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If
                If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode3").Name, "Series/SearchResult.aspx", CommonShared.AppendSecurityCheck("Draft=True&Type=Series")))
                End If
                If ProtocolEnv.DocumentSeriesLogSummaryEnabled AndAlso menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode4").Name, "Series/LogSummary.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If

                If CommonShared.UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.SeriesImportGroups) Then
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode5") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode5").Name, "Series/Import.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                    End If
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode6") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode6").Name, "Task/TaskHeaderGrid.aspx", CommonShared.AppendSecurityCheck("Type=Series&TaskType=" & CInt(TaskTypeEnum.DocSeriesImporter).ToString())))
                    End If
                    If menu.Nodes("FirstNode1").Nodes.Keys.Contains("SecondNode7") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode1").Nodes("SecondNode7").Name, "Series/Export.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                    End If

                    If CommonShared.UserConnectedBelongsTo(ProtocolEnv.TaskHeaderGroups) Then
                        For Each item As String In ProtocolEnv.TaskTypeEnabled.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            Dim subItems As String() = item.Split("|"c)
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, subItems(0), "Task/TaskHeaderGrid.aspx", CommonShared.AppendSecurityCheck("Type=Series&TaskType=" & subItems(1))))
                        Next
                    End If
                End If
            End If

            If DocSuiteContext.Current.ProtocolEnv.TransparentMonitoringEnabled AndAlso CommonShared.HasGroupTransparentManagerRight AndAlso menu.Nodes.Keys.Contains("FirstNode3") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode3").Name))
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode1").Name, "Monitors/MonitoringSeriesSection.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode2").Name, "Monitors/MonitoringSeriesRole.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode3").Name, "Monitors/MonitoringQuality.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode4").Name, "Monitors/TransparentAdministrationMonitorLog.aspx", CommonShared.AppendSecurityCheck("Type=Series")))
                End If
            End If

            hasInsertable = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.UDS, DomainUserFacade.HasInsertable)
            If menu.Nodes.Keys.Contains("FirstNode2") AndAlso ProtocolEnv.UDSEnabled Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode2").Name))
                If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode1") AndAlso hasInsertable Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode1").Name, "UDS/UDSInsert.aspx", "Type=UDS&Action=Insert"))
                End If
                If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode2").Name, "UDS/UDSSearch.aspx", "Type=UDS"))
                End If
            End If

            Dim seriesItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("SeriesItem")
            seriesItem.Text = menu.Name
            seriesItem.ImageUrl = ImagePath.SmallDocumentSeries
            seriesItem.Items.Clear()
            seriesItem.Items.Add(AddTreeViewItem("RadTreeSeries", alMenu, main.ClientID))
            seriesItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuProtocols(menuJson As IDictionary(Of String, MenuNodeModel))
        If ProtocolEnv.ShowProtocol AndAlso DocSuiteContext.Current.IsProtocolEnabled AndAlso menuJson.Keys.Contains("Menu5") Then
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Protocol, DomainUserFacade.HasInsertable)
            Dim menu As MenuNodeModel = menuJson("Menu5")
            Dim alMenu As New List(Of NodeLink)

            If menu.Nodes.Keys.Contains("FirstNode1") Then
                If hasInsertable Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "Prot/ProtInserimento.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Insert")))
                End If
                If ProtocolEnv.ProtocolBoxEnabled Then
                    If menu.Nodes.Keys.Contains("FirstNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "PEC/PECIncomingMails.aspx", CommonShared.AppendSecurityCheck("Type=Pec&ProtocolBox=True")))
                    End If
                    If menu.Nodes.Keys.Contains("FirstNode3") AndAlso ProtocolEnv.IsPECEnabled Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "PEC/PECToDocumentUnit.aspx", CommonShared.AppendSecurityCheck("Type=Pec&PecFromFile=True")))
                    End If
                End If
            End If
            If menu.Nodes.Keys.Contains("FirstNode4") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode4").Name, "Prot/ProtRicercaFromMenu.aspx", "Type=Prot"))
            End If
            If Not String.IsNullOrEmpty(ProtocolEnv.EnvGroupSuspend) Then
                If Facade.ProtocolFacade.HasProtSuspended() Then
                    If menu.Nodes.Keys.Contains("FirstNode5") Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode5").Name, "Prot/ProtInserimento.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Recovery")))
                    End If
                    If menu.Nodes.Keys.Contains("FirstNode6") Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode6").Name, "Prot/ProtSelezione.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=View")))
                    End If
                End If
            End If

            If ProtocolEnv.IsProtocolRecoverEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode8") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode8").Name, "Prot/ProtRecupera.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
            End If

            If ProtocolEnv.ProtParzialeEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode9") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode9").Name, "Prot/ProtIncompleti.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
            End If

            If ProtocolEnv.EnvWordEnabled AndAlso CommonUtil.HasWordGroupImportRight AndAlso menu.Nodes.Keys.Contains("FirstNode10") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode10").Name, "Prot/ProtImportLettera.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Import&ImportFile=Word")))
            End If
            If ProtocolEnv.EnvExcelEnabled AndAlso CommonUtil.HasExcelGroupImportRight AndAlso menu.Nodes.Keys.Contains("FirstNode11") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode11").Name, "Prot/ProtImportLettera.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Import&ImportFile=Excel")))
            End If
            If CommonUtil.HasGroupJournalRight AndAlso menu.Nodes.Keys.Contains("FirstNode14") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode14").Name, "Prot/ProtJournal.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Print")))
            End If
            If CommonShared.PosteWebEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode15") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode15").Name))
                If menu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode15").Nodes("SecondNode1").Name, "Prot/PosteWebRicerca.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
                End If
                If CommonShared.HasPosteWebReportRight AndAlso menu.Nodes("FirstNode15").Nodes.Keys.Contains("SecondNode2") AndAlso Not ProtocolEnv.CostiPosteWebAccountEnabled Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode15").Nodes("SecondNode2").Name, "Prot/PosteWebCosti.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
                Else
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode15").Nodes("SecondNode2").Name, "Prot/PosteWebCostiAccount.aspx", CommonShared.AppendSecurityCheck("Type=Prot")))
                End If
            End If
            If menu.Nodes.Keys.Contains("FirstNode16") AndAlso ProtocolEnv.ProtocolPrintEnabled Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode16").Name))
                If menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode1").Name, "Prot/ProtRegistroPrint.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Print")))
                End If
                If CommonUtil.HasGroupConcourseRight AndAlso menu.Nodes("FirstNode16").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode16").Nodes("SecondNode2").Name, "Prot/ProtConcorsiPrint.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=Print")))
                End If
            End If
            If CommonUtil.HasGroupStatisticsRight AndAlso menu.Nodes.Keys.Contains("FirstNode17") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode17").Name))
                If menu.Nodes("FirstNode17").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode17").Nodes("SecondNode1").Name, "Prot/ProtStatistiche.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=P")))
                End If
                If menu.Nodes("FirstNode17").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode17").Nodes("SecondNode2").Name, "Prot/ProtStatistiche.aspx", CommonShared.AppendSecurityCheck("Type=Prot&Action=C")))
                End If
            End If

            ' Scrivania in menu protocollo
            If ProtocolEnv.MoveScrivaniaMenu AndAlso DocSuiteContext.Current.ProtocolEnv.DiaryFullEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode18") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Desktop, menu.Nodes("FirstNode18").Name))
                If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode1").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode1").Name, "PY", UserDesktop.PageDiario, "Prot")), ""))
                End If
                If ProtocolEnv.IsDistributionEnabled Then
                    If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode2") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode2").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode2").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_TO_READ, UserDesktop.PageScrivania, "Prot")), ""))
                    End If
                    If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode3") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode3").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode3").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_TO_DISTRIBUTE, UserDesktop.PageScrivania, "Prot")), ""))
                    End If
                    If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode4") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode4").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode4").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_TO_WORK, UserDesktop.PageScrivania, "Prot")), ""))
                    End If
                    If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode5") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode5").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode5").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_IN_ASSIGNMENT, UserDesktop.PageScrivania, "Prot")), ""))
                    End If
                    If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode6") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode6").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode6").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_RECENT, UserDesktop.PageScrivania, "Prot")), ""))
                    End If
                    If (ProtocolEnv.DistributionRejectableEnabled) Then
                        If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode15") Then
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode15").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode15").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_NEVER_DISTRIBUTED, UserDesktop.PageScrivania, "Prot")), ""))
                        End If
                    End If
                Else
                    If ProtocolEnv.IsLogStatusEnabled AndAlso menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode7") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode7").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode7").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_TO_READ, UserDesktop.PageScrivania, "Prot")), ""))
                    End If

                    If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
                        If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode12") Then
                            alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode12").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode12").Name, "PDA", UserDesktop.PageAuthorized, "Prot")), ""))
                        End If
                        If CommonShared.HasRefusedProtocolGroupsRight Then
                            If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode13") Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode13").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode13").Name, "PANA", UserDesktop.PageAuthorized, "Prot")), ""))
                            End If
                            If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode14") Then
                                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode14").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode14").Name, "PRS", UserDesktop.PageAuthorized, "Prot")), ""))
                            End If
                        End If
                    End If

                    If ProtocolEnv.IsStatusEnabled AndAlso menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode8") Then
                        alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode8").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode8").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_ASSIGNED, UserDesktop.PageScrivania, "Prot")), ""))
                    End If
                End If
                If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode9") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode9").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode9").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_INSERT_TODAY, UserDesktop.PageScrivania, "Prot")), ""))
                End If
                If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode10") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode10").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode10").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_INSERT_THIS_WEEK, UserDesktop.PageScrivania, "Prot")), ""))
                End If
                If menu.Nodes("FirstNode18").Nodes.Keys.Contains("SecondNode11") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode18").Nodes("SecondNode11").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode18").Nodes("SecondNode11").Name, UserScrivaniaD.ACTION_NAME_PROTOCOL_INSERT_THIS_MONTH, UserDesktop.PageScrivania, "Prot")), ""))
                End If
            End If

            If ProtocolEnv.IsInvoiceEnabled AndAlso ProtocolEnv.ProtocolKindEnabled AndAlso ProtocolEnv.InvoicePAEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode19") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode19").Name))
                If menu.Nodes("FirstNode19").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode19").Nodes("SecondNode1").Name, "Prot/ProtGestioneFatturePA.aspx", CommonShared.AppendSecurityCheck(String.Format("Type=Prot&Action={0}&Title={1}", ProtGestioneFatturePA.ConsegnateSDI, HttpUtility.UrlEncode(menu.Nodes("FirstNode19").Nodes("SecondNode1").Name)))))
                End If
                If menu.Nodes("FirstNode19").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode19").Nodes("SecondNode2").Name, "Prot/ProtGestioneFatturePA.aspx", CommonShared.AppendSecurityCheck(String.Format("Type=Prot&Action={0}&Title={1}", ProtGestioneFatturePA.Consegnate, HttpUtility.UrlEncode(menu.Nodes("FirstNode19").Nodes("SecondNode2").Name)))))
                End If
                If menu.Nodes("FirstNode19").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode19").Nodes("SecondNode3").Name, "Prot/ProtGestioneFatturePA.aspx", CommonShared.AppendSecurityCheck(String.Format("Type=Prot&Action={0}&Title={1}", ProtGestioneFatturePA.Rifiutate, HttpUtility.UrlEncode(menu.Nodes("FirstNode19").Nodes("SecondNode3").Name)))))
                End If
                If menu.Nodes("FirstNode19").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode19").Nodes("SecondNode4").Name, "Prot/ProtGestioneFatturePA.aspx", CommonShared.AppendSecurityCheck(String.Format("Type=Prot&Action={0}&Title={1}", ProtGestioneFatturePA.DaInviare, HttpUtility.UrlEncode(menu.Nodes("FirstNode19").Nodes("SecondNode4").Name)))))
                End If
            End If

            If IsRejectionVisible AndAlso menu.Nodes.Keys.Contains("FirstNode20") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode20").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Rigettati", UserDesktop.ActionNameProtocolliRigettati, UserDesktop.PageScrivania, "Prot")), ""))
            End If

            If DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode21") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode21").Name, "Task/TaskHeaderGrid.aspx", CommonShared.AppendSecurityCheck("Type=Series&TaskType=" & CInt(TaskTypeEnum.FastProtocolSender).ToString())))
            End If

            Dim protocolloItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("ProtocolloItem")
            protocolloItem.Text = menu.Name
            protocolloItem.Items.Clear()
            protocolloItem.Items.Add(AddTreeViewItem("RadTreeProtocollo", alMenu, main.ClientID))
            protocolloItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuPratiche(menuJson As IDictionary(Of String, MenuNodeModel))
        If DocSuiteContext.Current.ProtocolEnv.PraticheEnabled AndAlso menuJson.Keys.Contains("Menu4") Then
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Document, DomainUserFacade.HasInsertable)
            Dim menu As MenuNodeModel = menuJson("Menu4")
            Dim alMenu As New List(Of NodeLink)
            If menu.Nodes.Keys.Contains("FirstNode1") AndAlso hasInsertable Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "Docm/DocmInserimento.aspx", CommonShared.AppendSecurityCheck("Type=Docm&Action=Insert")))
            End If
            If menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "Docm/DocmRicerca.aspx", "Type=Docm"))
            End If

            'cliente
            If ProtocolEnv.MoveScrivaniaMenu AndAlso DocSuiteContext.Current.ProtocolEnv.DiaryFullEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode3") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Desktop, menu.Nodes("FirstNode3").Name))
                If DocumentEnv.IsEnvLogEnabled AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode1").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode1").Name, "DY", UserDesktop.PageDiario, "Docm")), ""))
                End If

                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode2").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode2").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_TO_READ, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode3").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode3").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_OPEN_TO_READ, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode4").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode4").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_CHECKOUT, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode5") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode5").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode5").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_TAKE_IN_CHARGE, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode6") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode6").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode6").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_INSERT_TODAY, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode7") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode7").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode7").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_INSERT_THIS_WEEK, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode8") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode8").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode8").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_INSERT_THIS_MONTH, UserDesktop.PageScrivania, "Docm")), ""))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode9") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode9").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Nodes("SecondNode9").Name, UserScrivaniaD.ACTION_NAME_DOCUMENT_TIMETABLE, UserDesktop.PageScrivania, "Docm")), ""))
                End If
            End If

            Dim praticheItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("PraticheItem")
            praticheItem.Text = menu.Name
            praticheItem.Items.Clear()
            praticheItem.Items.Add(AddTreeViewItem("RadTreePratiche", alMenu, main.ClientID))
            praticheItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuCollaborations(menuJson As IDictionary(Of String, MenuNodeModel))
        If ProtocolEnv.MoveCollaborationMenu AndAlso CollaborationRights.GetIsCollaborationEnabled(CurrentTenant.TenantAOO.UniqueId) AndAlso menuJson.Keys.Contains("Menu3") Then
            Dim hasSecretaryRole As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Collaboration, DomainUserFacade.HasSecretaryRole)
            Dim hasSignerRole As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Collaboration, DomainUserFacade.HasSignerRole)
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Protocol, DomainUserFacade.HasInsertable) OrElse GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Resolution, DomainUserFacade.HasInsertable) OrElse GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.DocumentSeries, DomainUserFacade.HasInsertable) OrElse GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.UDS, DomainUserFacade.HasInsertable)
            Dim menu As MenuNodeModel = menuJson("Menu3")
            Dim alMenu As New List(Of NodeLink)
            If CollaborationRights.GetInserimentoAllaVisioneFirmaEnabled(CurrentTenant.TenantAOO.UniqueId) AndAlso menu.Nodes.Keys.Contains("FirstNode1") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, String.Format("User/UserCollGestione.aspx?Titolo=Inserimento&Action=Add&Title2={0}&Action2=CI&Type=Prot", menu.Nodes("FirstNode1").Name), String.Empty))
            End If

            If CollaborationRights.GetInserimentoAlProtocolloSegreteriaEnabled(CurrentTenant.TenantAOO.UniqueId) AndAlso menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, String.Format("User/UserCollGestione.aspx?Titolo=Inserimento&Action=Apt&Title2={0}&Action2=CA&Type=Prot", menu.Nodes("FirstNode2").Name), String.Empty))
            End If

            If menu.Nodes.Keys.Contains("FirstNode3") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode3").Name, CollaborationMainAction.AllaVisioneFirma, UserDesktop.PageCollRisultati, "Prot")), ""))
            End If
            If menu.Nodes.Keys.Contains("FirstNode4") AndAlso hasSignerRole Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode4").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode4").Name, CollaborationMainAction.DaVisionareFirmare, UserDesktop.PageCollRisultati, "Prot")), ""))
            End If

            If menu.Nodes.Keys.Contains("FirstNode14") AndAlso ProtocolEnv.RemoteSignDelegateEnabled AndAlso hasSignerRole Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode14").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode14").Name, CollaborationMainAction.DaFirmareInDelega, UserDesktop.PageCollRisultati, "Prot")), ""))
            End If

            If menu.Nodes.Keys.Contains("FirstNode5") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode5").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode5").Name, CollaborationMainAction.AlProtocolloSegreteria, UserDesktop.PageCollRisultati, "Prot")), ""))
            End If

            If Facade.ContainerFacade.HasInsertOrProposalRights() AndAlso menu.Nodes.Keys.Contains("FirstNode6") AndAlso hasSecretaryRole Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode6").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode6").Name, CollaborationMainAction.DaProtocollareGestire, UserDesktop.PageCollRisultati, "Prot")), ""))
            End If
            If menu.Nodes.Keys.Contains("FirstNode7") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode7").Name, "User/" & UserDesktop.GetNodeUrl(New UserDesktopNodeAction(menu.Nodes("FirstNode7").Name, CollaborationMainAction.ProtocollatiGestiti, UserDesktop.PageCollRisultati, "Prot")), ""))
            End If
            If menu.Nodes.Keys.Contains("FirstNode9") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode9").Name, "User/" & UserDesktop.GetNodeUrl(menu.Nodes("FirstNode9").Name, CollaborationMainAction.AttivitaInCorso), ""))
            End If
            If menu.Nodes.Keys.Contains("FirstNode10") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode10").Name, "User/" & UserDesktop.GetNodeUrl(menu.Nodes("FirstNode10").Name, CollaborationMainAction.MieiCheckOut), "CCheckedOut"))
            End If

            'If Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode11") Then
            '    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode11").Name, "User/CollaborationVersioningManagement.aspx", String.Empty))
            'End If

            If (DocSuiteContext.Current.ProtocolEnv.UDSEnabled OrElse (DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso DocSuiteContext.Current.Tenants.Count > 1)) AndAlso menu.Nodes.Keys.Contains("FirstNode12") Then
                Dim active As Boolean = DocSuiteContext.Current.ProtocolEnv.UDSEnabled
                If Not DocSuiteContext.Current.ProtocolEnv.UDSEnabled Then
                    active = False
                    Dim tenantModel As TenantModel = DocSuiteContext.Current.ProtocolEnv.TenantModels.FirstOrDefault(Function(x) Not x.CurrentTenant AndAlso x.Entities.Any(Function(t) t.Key.Eq("Protocol")))
                    If tenantModel IsNot Nothing Then
                        Dim entityConfiguration As TenantEntityConfiguration = GetFromType(Of Protocol)(tenantModel.Entities)
                        active = entityConfiguration.IsActive
                    End If
                End If
                If active Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode12").Name, "Prot/ProtMDRisultati.aspx", String.Empty))
                End If
            End If

            If ProtocolEnv.MoveWorkflowDeskToCollaboration AndAlso DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode13") Then
                Dim firstNode13 As NodeLink = New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode13").Name, String.Empty, String.Empty)
                If menu.Nodes("FirstNode13").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode13").Name, "User/UserWorkflow.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
            End If

            Dim collaborazioneItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("CollaborazioneItem")
            collaborazioneItem.Text = menu.Name
            collaborazioneItem.Items.Clear()
            collaborazioneItem.Items.Add(AddTreeViewItem("RadTreeCollaborazione", alMenu, main.ClientID))
            collaborazioneItem.Visible = True
        End If
    End Sub
    Private Sub CreateMenuInvoice(menuJson As IDictionary(Of String, MenuNodeModel))
        If ProtocolEnv.InvoiceSDIEnabled AndAlso CommonShared.HasGroupInvoiceSDIRight AndAlso menuJson.Keys.Contains("Menu8") Then
            Dim menu As MenuNodeModel = menuJson("Menu8")
            Dim alMenu As New List(Of NodeLink)

            If (ProtocolEnv.InvoiceSDIB2BKind > 0 OrElse ProtocolEnv.InvoiceSDIPAKind > 0) AndAlso menu.Nodes.Keys.Contains("FirstNode1") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Type=UDS")))
            End If

            'Fatture attive B2B
            If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode2").Name))
                If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode1").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=Archiviata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode2").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=In attesa metadati fiscali&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode3").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=Contabilizzata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode4").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=PEC inviata con accettazione&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode5") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode5").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=PEC inviata con consegna allo SDI&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode9") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode9").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=Rifiutata dallo SDI&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode6") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode6").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=Recapito in corso&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode7") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode7").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=Mancato recapito&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode8") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode8").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=B2B&InvoiceStatus=Consegnata&Type=UDS")))
                End If
                If menu.Nodes("FirstNode2").Nodes.Keys.Contains("SecondNode10") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode10").Name, "Workflows/WorkflowInstances.aspx", CommonShared.AppendSecurityCheck("WorkflowRepositoryName=Fatturazione elettronica - Fattura tra privati attiva")))
                End If
            End If

            'Fatture attive PA
            If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes.Keys.Contains("FirstNode3") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode3").Name))
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode2").Nodes("SecondNode1").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Archiviata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode2").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=In attesa metadati fiscali&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode3").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Contabilizzata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode4") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode4").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=PEC inviata con accettazione&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode5") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode5").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=PEC inviata con consegna allo SDI&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode9") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode9").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Rifiutata dallo SDI&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode6") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode6").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Recapito in corso&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode7") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode7").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Mancato recapito&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode8") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode8").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Consegnata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode10") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode10").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Accettata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIPAKind > 0 AndAlso menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode11") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode11").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=1&InvoiceKind=PA&InvoiceStatus=Rifiutata&Type=UDS")))
                End If
                If menu.Nodes("FirstNode3").Nodes.Keys.Contains("SecondNode12") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode3").Nodes("SecondNode12").Name, "Workflows/WorkflowInstances.aspx", CommonShared.AppendSecurityCheck("WorkflowRepositoryName=Fatturazione elettronica - Fattura pubblica amministrazione attiva")))
                End If
            End If

            'Fatture passive
            If ProtocolEnv.InvoiceSDIB2BKind > 0 AndAlso menu.Nodes.Keys.Contains("FirstNode4") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Line, menu.Nodes("FirstNode4").Name))
                If ProtocolEnv.InvoiceSDIB2BKind > 1 AndAlso menu.Nodes("FirstNode4").Nodes.Keys.Contains("SecondNode1") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode4").Nodes("SecondNode1").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=0&InvoiceKind=B2B&InvoiceStatus=Archiviata&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 1 AndAlso menu.Nodes("FirstNode4").Nodes.Keys.Contains("SecondNode2") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode4").Nodes("SecondNode2").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=0&InvoiceKind=B2B&InvoiceStatus=In attesa metadati fiscali&Type=UDS")))
                End If
                If ProtocolEnv.InvoiceSDIB2BKind > 1 AndAlso menu.Nodes("FirstNode4").Nodes.Keys.Contains("SecondNode3") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode4").Nodes("SecondNode3").Name, "UDS/UDSInvoiceSearch.aspx", CommonShared.AppendSecurityCheck("Direction=0&InvoiceKind=B2B&InvoiceStatus=Contabilizzata&Type=UDS")))
                End If
                If menu.Nodes("FirstNode4").Nodes.Keys.Contains("SecondNode6") Then
                    alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, menu.Nodes("FirstNode4").Nodes("SecondNode6").Name, "Workflows/WorkflowInstances.aspx", CommonShared.AppendSecurityCheck("WorkflowRepositoryName=Fatturazione elettronica - Fattura tra privati passiva")))
                End If
            End If

            Dim invoiceItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("InvoiceItem")
            invoiceItem.Text = menu.Name
            invoiceItem.Items.Clear()
            invoiceItem.Items.Add(AddTreeViewItem("RadTreeInvoice", alMenu, main.ClientID))
            invoiceItem.Visible = True
        End If

    End Sub
    Private Sub CreateMenuDesks(menuJson As IDictionary(Of String, MenuNodeModel))
        If ProtocolEnv.DeskEnable AndAlso menuJson.Keys.Contains("Menu2") Then
            Dim menu As MenuNodeModel = menuJson("Menu2")
            Dim alMenu As New List(Of NodeLink)
            Dim hasInsertable As Boolean = GetCurrentRight(Model.Entities.Commons.DSWEnvironmentType.Desk, DomainUserFacade.HasInsertable)
            If menu.Nodes.Keys.Contains("FirstNode1") AndAlso hasInsertable Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "Desks/DeskInsert.aspx", CommonShared.AppendSecurityCheck("Type=Desk")))
            End If
            If menu.Nodes.Keys.Contains("FirstNode2") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "Desks/DeskRisultati.aspx", CommonShared.AppendSecurityCheck("Type=Desk")))
            End If
            If menu.Nodes.Keys.Contains("FirstNode3") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "Desks/DeskRisultati.aspx", CommonShared.AppendSecurityCheck("Type=Desk&Finder=OpenDesk")))
            End If

            Dim deskItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("DeskItem")
            deskItem.Text = menu.Name
            deskItem.Items.Clear()
            deskItem.Items.Add(AddTreeViewItem("RadTreeCollaborazione", alMenu, main.ClientID))
            deskItem.Visible = True
        End If
    End Sub

    Private Sub CreateMenuPersonalUser(menuJson As IDictionary(Of String, MenuNodeModel))
        If ProtocolEnv.ShowUser AndAlso (ProtocolEnv.IsDistributionEnabled OrElse ProtocolEnv.IsDesktopEnabled) AndAlso menuJson.Keys.Contains("Menu1") Then
            Dim menu As MenuNodeModel = menuJson("Menu1")

            Dim alMenu As New List(Of NodeLink)
            If menu.Nodes.Keys.Contains("FirstNode1") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode1").Name, "Comm/CommIntro.aspx", CommonShared.AppendSecurityCheck("Type=Intro")))
            End If

            If menu.Nodes.Keys.Contains("FirstNode2") Then
                If Not ProtocolEnv.EnableUnifiedDiary Then
                    If DocSuiteContext.Current.ProtocolEnv.MoveScrivaniaMenu AndAlso DocSuiteContext.Current.ProtocolEnv.IsDiaryEnabled Then
                        alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Desktop, menu.Nodes("FirstNode2").Name, "User/UserDiarioComune.aspx", CommonShared.AppendSecurityCheck("Type=User")))
                    End If
                Else
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode2").Name, "User/UserDiarioUnificato.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
            End If

            If Not ProtocolEnv.MoveWorkflowDeskToCollaboration AndAlso menu.Nodes.Keys.Contains("FirstNode3") Then
                If DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled Then
                    alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode3").Name, "User/UserWorkflow.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
                End If
            End If

            If DocSuiteContext.Current.ProtocolEnv.EnableMessageView AndAlso menu.Nodes.Keys.Contains("FirstNode4") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode4").Name, "MailSenders/MessageEmailList.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
            End If

            If ProtocolEnv.IsDesktopEnabled AndAlso Not DocSuiteContext.Current.ProtocolEnv.MoveScrivaniaMenu AndAlso menu.Nodes.Keys.Contains("FirstNode5") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Desktop, menu.Nodes("FirstNode5").Name, "User/UserDesktop.aspx", CommonShared.AppendSecurityCheck("Type=User")))
            End If
            If (ProtocolEnv.IsDistributionEnabled OrElse ProtocolEnv.RolesUserProfileEnabled) AndAlso menu.Nodes.Keys.Contains("FirstNode6") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode6").Name, "User/UserProfilo.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
            End If
            If ProtocolEnv.EnvUtltRenderingEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode7") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode7").Name, "Utlt/UtltRenderingDocument.aspx", CommonShared.AppendSecurityCheck("Type=Comm&Page=Document&Action=User")))
            End If

            If ProtocolEnv.EnableUserProfile OrElse (ProtocolEnv.UserConsoleEnabled AndAlso ProtocolEnv.UserLogEmail) AndAlso menu.Nodes.Keys.Contains("FirstNode8") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Point, menu.Nodes("FirstNode8").Name, "Utlt/UserProfile.aspx", CommonShared.AppendSecurityCheck("Type=Comm")))
            End If

            If DocSuiteContext.Current.ProtocolEnv.IsHelpEnabled AndAlso menu.Nodes.Keys.Contains("FirstNode9") Then
                alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Help, menu.Nodes("FirstNode9").Name, "User/ZenDeskHelp.aspx?IsButtonPressed=False", CommonShared.AppendSecurityCheck("Type=Comm")))
            End If

            Dim utenteItem As RadPanelItem = RadPanelBarMenu.FindItemByValue("UtenteItem")
            utenteItem.Text = menu.Name
            utenteItem.Items.Clear()
            utenteItem.Items.Add(AddTreeViewItem("RadTreeUtente", alMenu, main.ClientID))
            utenteItem.Visible = True
        End If
    End Sub

    Private Sub LoadNotificationCounters()
        For Each notification As NotificationType In ProtocolEnv.NotificationTypes
            InitializeButtons(notification)
        Next
    End Sub

    ''' <summary>
    ''' Inizializza i pulsanti di notifica
    ''' </summary>
    Public Sub InitializeButtons(ByVal notificationTypeEnum As NotificationType)
        Dim url As String = String.Empty
        Dim cutil As CommonUtil = New CommonUtil()
        Select Case notificationTypeEnum
            Case NotificationType.ProtocolliDaLeggere
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Da leggere", "PL", UserDesktop.PageScrivania, "Prot"))
                btnProtocolNotReaded.Visible = True
                btnProtocolNotReaded.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                Exit Select
            Case NotificationType.ProtocolliDifatturaDaLeggere
                If DocSuiteContext.Current.ProtocolEnv.InvoiceSDIEnabled Then
                    url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Fatture da leggere", "IPL", UserDesktop.PageScrivania, "Prot"))
                    btnProtocolInvoiceNotReaded.Visible = True
                    btnProtocolInvoiceNotReaded.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                End If
                Exit Select
            Case NotificationType.ProtocolliDaDistribuire
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Da distribuire", "PD", UserDesktop.PageScrivania, "Prot"))
                btnProtocolToDistribute.Visible = True
                btnProtocolToDistribute.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                Exit Select
            Case NotificationType.ProtocolliRigettati
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Rigettati", UserDesktop.ActionNameProtocolliRigettati, UserDesktop.PageScrivania, "Prot"))
                btnProtocolRejected.Visible = True
                btnProtocolRejected.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                btnProtocolRejected.Icon.PrimaryIconUrl = ImagePath.SmallReject
                Exit Select
            Case NotificationType.CollaborazioniDaProtocollare
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Da protocollare/Gestire", CollaborationMainAction.DaProtocollareGestire, UserDesktop.PageCollRisultati, "Prot"))
                btnCollToProtocol.Visible = True
                btnCollToProtocol.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                Exit Select
            Case NotificationType.CollaborazioniDaVisionare
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Da visionare/firmare", CollaborationMainAction.DaVisionareFirmare, UserDesktop.PageCollRisultati, "Prot"))
                btnCollToVision.Visible = True
                btnCollToVision.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                Exit Select
            Case NotificationType.WorkflowUtenteCorrente
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Attività", Nothing, UserDesktop.PageWorkflowRisultati))
                btnWorkflow.Visible = DocSuiteContext.Current.ProtocolEnv.WorkflowManagerEnabled
                btnWorkflow.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                Exit Select
            Case NotificationType.ProtocolliInEvidenza
                url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("In evidenza", "PE", UserDesktop.PageScrivania, "Prot"))
                btnHighlightProtocols.Visible = True
                btnHighlightProtocols.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
            Case NotificationType.PECDaLeggere
                If Not ProtocolEnv.IsPECEnabled Then
                    Exit Select
                End If
                Dim menuJson As IDictionary(Of String, MenuNodeModel) = DocSuiteContext.Current.DocSuiteMenuConfiguration
                If menuJson.Keys.Contains("Menu10") Then
                    Dim menu As MenuNodeModel = menuJson("Menu10")
                    If menu.Nodes.Keys.Contains("FirstNode2") Then
                        url = ("PEC/PECIncomingMails.aspx?Type=Pec")
                    End If
                End If
                btnPECNotReaded.Visible = ProtocolEnv.IsPECEnabled
                btnPECNotReaded.NavigateUrl = url
            Case NotificationType.ProtocolliDaAccettare
                If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
                    url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Da accettare", "PDA", UserDesktop.PageAuthorized, "Prot"))
                    btnProtocolToAccept.Visible = True
                    btnProtocolToAccept.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                End If
                Exit Select
            Case NotificationType.ProtocolliRespinti
                If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso CommonShared.HasRefusedProtocolGroupsRight Then
                    url = UserDesktop.GetNodeUrl(New UserDesktopNodeAction("Respinti", "PRS", UserDesktop.PageAuthorized, "Prot"))
                    btnProtocolRefused.Visible = True
                    btnProtocolRefused.NavigateUrl = GetScrivaniaUrl(url, notificationTypeEnum)
                End If
                Exit Select
                Exit Select
            Case NotificationType.UltimePagineDaFirmare
                If DocSuiteContext.Current.IsResolutionEnabled AndAlso ResolutionEnv.ShowMassiveResolutionSearchPageEnabled AndAlso CommonShared.HasGroupDigitalLastPageRight Then
                    btnLastPagesToSign.Visible = True
                    btnLastPagesToSign.NavigateUrl = String.Concat("Resl/ReslFirmaUltimaPagina.aspx?", CommonShared.AppendSecurityCheck("Type=Resl"))
                End If
                Exit Select
        End Select
    End Sub

    Private Function GetScrivaniaUrl(action As String, notificationType As NotificationType) As String
        ' Accrocchio url scrivania
        Select Case notificationType
            Case NotificationType.CollaborazioniDaProtocollare,
                 NotificationType.CollaborazioniDaVisionare
                Return If(ProtocolEnv.MoveCollaborationMenu, String.Format("User/{0}", action), String.Format("User/UserDesktop.aspx?content={0}", HttpUtility.UrlEncode(action)))

            Case Else
                Return If(ProtocolEnv.MoveScrivaniaMenu, String.Format("User/{0}", action), String.Format("User/UserDesktop.aspx?content={0}", HttpUtility.UrlEncode(action)))
        End Select
    End Function

    Private Function GetCurrentRight(env As Model.Entities.Commons.DSWEnvironmentType, thisProperty As String) As Boolean
        Dim value As Boolean = True
        If ProtocolEnv.MenuRightEnabled Then
            value = Facade.DomainUserFacade.HasCurrentRight(CurrentDomainUser, env, thisProperty)
        End If
        Return value
    End Function
#End Region

End Class