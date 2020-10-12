Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.IO

Public Class ProtCollegamenti
    Inherits ProtBasePage
    ' Icone email
    Private Const iconMailI As String = "~/Prot/images/mail16_i.gif"
    Private Const iconMailU As String = "~/Prot/images/mail16_u.gif"
    Private Const iconRemove As String = "~/comm/images/remove16.gif"

#Region "Page events"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeAjaxSetting()
        If Not Me.IsPostBack Then
            ' Iniazializza gli elementi di pagina
            Initialize()
            InitializeMail()
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializeMail()
        If DocSuiteContext.Current.ProtocolEnv.EnableButtonLinkProtocolSend Then
            btnMail.Visible = True
            MailFacade.RegisterOpenerMailWindow(btnMail, MailFacade.CreateProtocolLinkMailParameters(CurrentProtocol.Id, btnMail.ID))
        End If
    End Sub

    Private Sub InitializeAjaxSetting()
        ' Inizializza ajax events
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf tvwProtocolLink_AjaxRequest
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(MasterDocSuite.AjaxManager, tvwProtocolLink)
    End Sub

    ''' <summary> Inizializza gli oggetti della pagina </summary>
    Private Sub Initialize()
        Dim rootList As New List(Of Protocol)

        If DocSuiteContext.Current.ProtocolEnv.EnableButtonLinkProtocolSend Then
            btnMail.Visible = True
        End If

        If DocSuiteContext.Current.ProtocolEnv.EnableButtonLinkProtocolPrint Then
            btnStampa.Visible = True
        End If

        ' trovo i protocolli padre di tutta la lista 
        GetRootProtocols(CurrentProtocol, rootList)
        For Each rootProtocol As Protocol In rootList
            Dim firstNode As RadTreeNode = CreateNode(rootProtocol)
            tvwProtocolLink.Nodes(0).Nodes.Add(firstNode)
            LoadNodes(firstNode, New List(Of RadTreeNode)())
            firstNode.ContextMenuID = "ContextMenuCollegamentiCurrentProtocollo"
        Next

        Dim Right As Boolean = False

        ' Inizializza menu contestuale
        Dim menuItemAdd As RadMenuItem = ContextMenuCollegamenti.FindItemByValue("Add")
        Dim menuItemAddCurrentProtocol As RadMenuItem = ContextMenuCollegamentiCurrentProtocollo.FindItemByValue("Add")
        Dim menuItemDelete As RadMenuItem = ContextMenuCollegamenti.FindItemByValue("Delete")

        Right = CurrentProtocolRights.IsLinkable

        If Right Then
            ' Menu contestuale aggiungi
            menuItemAdd.Enabled = True
            menuItemAdd.Visible = True
            menuItemAddCurrentProtocol.Enabled = True
            menuItemAddCurrentProtocol.Visible = True
            ' Menu contestuale delete
            menuItemDelete.Enabled = True
            menuItemDelete.Visible = True
        Else
            ' nasconde le voci del menu contestuale
            menuItemAdd.Enabled = True
            menuItemAdd.Visible = True
            menuItemAddCurrentProtocol.Enabled = False
            menuItemAddCurrentProtocol.Visible = False
            menuItemDelete.Enabled = False
            menuItemDelete.Visible = False
        End If

        If tvwProtocolLink.Nodes(0).Nodes.Count > 0 Then
            tvwProtocolLink.Nodes(0).Nodes(0).Selected = True
            If Right Then
                MasterDocSuite.AjaxManager.ResponseScripts.Add("showActionButtons();")
            End If
        End If

    End Sub
#End Region

#Region "Create Nodes"
    ''' <summary> Crea un nodo del treeview con i dati del protocollo </summary>
    Private Function CreateNode(ByVal protocol As Protocol) As RadTreeNode
        Dim childNode As New RadTreeNode()
        childNode.Text = protocol.Year.ToString() & "/"
        childNode.Text &= protocol.Number.ToString.PadLeft(7, "0"c) & " del "
        childNode.Text &= String.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate.ToLocalTime())
        childNode.Text &= " (" & protocol.ProtocolObject & ")"
        ' Icona protocollo in entrata
        If protocol.Type.Id = 1 Then
            childNode.ImageUrl = iconMailU
            ' Protocollo in uscita
        ElseIf protocol.Type.Id = -1 Then
            childNode.ImageUrl = iconMailI
        End If
        ' Icona Protocollo rimosso
        If protocol.IdStatus.HasValue AndAlso protocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            childNode.ImageUrl = iconRemove
        End If
        childNode.Value = protocol.Id.ToString()
        childNode.Attributes.Add("Year", protocol.Year.ToString())
        childNode.Attributes.Add("Number", protocol.Number.ToString())
        If protocol.ProtocolLinks.Count > 0 Then
            childNode.ExpandMode = TreeNodeExpandMode.ClientSide
        End If
        childNode.Font.Bold = protocol.Id = CurrentProtocol.Id
        childNode.Expanded = True
        ' Menu contestuale
        childNode.ContextMenuID = "ContextMenuCollegamenti"
        Return childNode
    End Function

    ''' <summary>
    ''' Carica tutti i protocolli collegati ad un singolo protocollo
    ''' </summary>
    ''' <param name="vFather">RadtreeNode: nodo padre</param>
    ''' <remarks></remarks>
    Private Sub LoadNodes(ByRef vFather As RadTreeNode, ByVal nodeList As List(Of RadTreeNode))
        Dim vProtocol As Protocol
        'Primo protocollo
        If String.IsNullOrEmpty(vFather.Value) Then
            vProtocol = CurrentProtocol
        Else
            ' Protocolli figli
            Dim parentId As Guid = Guid.Parse(vFather.Value)
            vProtocol = Facade.ProtocolFacade.GetById(parentId, False)
        End If
        Dim vNode As RadTreeNode = Nothing
        ' Aggiunge i protocolli come nodi del treeview
        If vProtocol.ProtocolLinks.Count > 0 Then
            For Each link As ProtocolLink In vProtocol.ProtocolLinks
                vNode = CreateNode(link.ProtocolLinked)
                'Se il nodo è già stato inserito allora va in errore
                If nodeList.Find(Function(value As RadTreeNode) value.Value = vNode.Value) IsNot Nothing Then
                    Throw New DocSuiteException("Rilevato collegamento circolare tra protocolli", $"La pagina dei collegamenti è stata bloccata per la rilevazione di collegamenti circolari. Contattare l'assistenza e richiedere la verifica del protocollo {CurrentProtocol.FullNumber}", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
                End If
                If Not vNode Is Nothing Then
                    vFather.Nodes.Add(vNode)
                    nodeList.Add(vFather)
                    Dim nodeListCopy As New List(Of RadTreeNode)
                    nodeListCopy.AddRange(nodeList)
                    LoadNodes(vNode, nodeListCopy)
                End If
            Next
        End If
    End Sub

    Private Sub GetRootProtocols(ByVal sonProtocol As Protocol, ByRef rooList As IList(Of Protocol))
        If sonProtocol.ProtocolParentLinks.Count > 0 Then
            For Each parentLink As ProtocolLink In sonProtocol.ProtocolParentLinks
                Dim fatherProtocol As Protocol = parentLink.Protocol
                If fatherProtocol.Year > sonProtocol.Year OrElse (fatherProtocol.Year = sonProtocol.Year AndAlso fatherProtocol.Number > sonProtocol.Number) Then
                    GetRootProtocols(fatherProtocol, rooList)
                Else
                    rooList.Add(sonProtocol)
                End If
            Next
        Else
            '' Aggiunto controllo che evita di inserire padri già presenti
            If Not rooList.Contains(sonProtocol) Then rooList.Add(sonProtocol)
        End If
    End Sub
#End Region

#Region "Ajax Request"
    ''' <summary>
    ''' Gestisce le chiamate ajax generate dal treeview e dal relativo menu contestuale
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub tvwProtocolLink_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim node As RadTreeNode = tvwProtocolLink.SelectedNode
        Select Case e.Argument
            ' Visualizzazione protocollo
            Case "View"
                ViewProtocol(node)
                ' Eliminazione collegamento al protocollo
            Case "Delete"
                DeleteNode(node)
                ' Aggiornamento del treeview
            Case "Refresh"
                RefreshProtocol(node)
        End Select
    End Sub
#End Region

    ''' <summary> Cancella il collegamento al protocollo selezionato </summary>
    ''' <param name="node">RadTreeNode: nodo da cancellare</param>
    Private Sub DeleteNode(ByVal node As RadTreeNode)
        Dim vProtocol As Protocol
        Dim vParentNode As RadTreeNode = node.ParentNode
        If Not String.IsNullOrEmpty(vParentNode.Value) Then
            Dim parentId As Guid = Guid.Parse(vParentNode.Value)
            vProtocol = Facade.ProtocolFacade.GetById(parentId, False)
            ' Se esiste il protocollo, elimina il collegamento al protocollo selezionato
            If vProtocol IsNot Nothing Then
                Dim linkProtocol As Protocol
                For Each pl As ProtocolLink In vProtocol.ProtocolLinks
                    linkProtocol = pl.ProtocolLinked
                    If linkProtocol.Id.ToString() = node.Value Then
                        ' Elimina il collegamento al protocollo
                        Facade.ProtocolFacade.RemoveProtocolLink(vProtocol, linkProtocol)
                        vProtocol.ProtocolLinks.Remove(pl)
                        ' Aggiorna il treeview
                        vParentNode.Nodes.Clear()
                        LoadNodes(vParentNode, New List(Of RadTreeNode)())
                        vParentNode.ExpandChildNodes()
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

#Region "Operations: View"
    ''' <summary>
    ''' Visualizza il protocollo selezionato nel treeview
    ''' </summary>
    ''' <param name="node"></param>
    ''' <remarks></remarks>
    Private Sub ViewProtocol(ByVal node As RadTreeNode)
        ' Se un protocollo è selezionato lo visualizza
        If node IsNot Nothing Then
            Dim vUniqueId As Guid = Guid.Parse(node.Value)
            ' Redirect alla pagina di visualizzazione
            Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={vUniqueId}&Type=Prot")}")
        End If
    End Sub
#End Region

#Region "Operations: Refresh"
    ''' <summary>
    ''' Aggiorna il protocollo selezionato nel treeview
    ''' </summary>
    ''' <param name="node"></param>
    ''' <remarks></remarks>
    Private Sub RefreshProtocol(ByVal node As RadTreeNode)
        node.Nodes.Clear()
        LoadNodes(node, New List(Of RadTreeNode)())
        node.ExpandChildNodes()
    End Sub
#End Region

#Region "Operations: Print Links"

    ''' <summary> Stampa l'elenco dei collegamenti </summary>
    Private Sub btnStampa_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnStampa.Click
        Dim Struttura As String = ""
        StampaCollegamenti(Nothing, Struttura)
        If Struttura = "" Then
            AjaxAlert("Errore in Generazione Stampa")
            Exit Sub
        End If
        CommonInstance.UserDeleteTemp(TempType.P)
        Dim FileTemp As String = CommonUtil.UserDocumentName & "-Print-" & String.Format("{0:HHmmss}", Now()) & ".txt"
        Dim fDestination As String = CommonInstance.AppTempPath & "\" & FileTemp
        Dim sw As StreamWriter = New StreamWriter(fDestination, True)
        Dim aArray As Array
        Dim i As Integer
        Dim Type As String = String.Empty
        Dim Row As String = String.Empty
        aArray = Split(Struttura, vbNewLine)
        Type = "100-N-L"
        For i = 0 To aArray.Length - 2
            Row = aArray(i)
            sw.Write(Row & "§" & Type & "§True§" & vbNewLine)
        Next
        sw.Close()
        'redirect
        Response.Redirect("..\Comm\CommStampa.aspx?Type=Comm&FileName=" & fDestination & "&Title=Stampa Collegamenti")
    End Sub

    ''' <summary>
    ''' Crea riepilogo collegamenti per la stampa
    ''' </summary>
    ''' <param name="Nodo"></param>
    ''' <param name="Struttura"></param>
    ''' <remarks></remarks>
    Private Sub StampaCollegamenti(ByRef Nodo As RadTreeNode, ByRef Struttura As String)
        Dim nNodo As RadTreeNode = Nothing
        If IsNothing(Nodo) Then
            Nodo = tvwProtocolLink.Nodes(0)
        End If
        For Each nNodo In Nodo.Nodes
            Dim sep As String = ""
            Dim s As String = nNodo.Index()
            Dim i As Integer
            For i = 0 To Regex.Matches(s, "\.").Count - 2
                sep &= ".  "
            Next
            Struttura &= sep & nNodo.Text & vbNewLine
            If Nodo.Nodes.Count = 0 Then Exit Sub
            StampaCollegamenti(nNodo, Struttura)
        Next
    End Sub
#End Region

End Class

