Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
Imports System.Collections.Generic
Imports FascicleEntities = VecompSoftware.DocSuiteWeb.Entity.Fascicles
Public Class ProtCollegamentiGes
    Inherits ProtBasePage

#Region " Fields "
    Private Const iconMailI As String = "~/App_Themes/DocSuite2008/imgset16/ingoing.png"
    Private Const iconMailU As String = "~/App_Themes/DocSuite2008/imgset16/outgoing.png"
    Private Const iconMailIU As String = "~/App_Themes/DocSuite2008/imgset16/inout.png"
    Const iconRemove As String = "~/comm/images/remove16.gif"

    Dim yearSon As String = String.Empty
    Dim numberSon As String = String.Empty
    Dim BtnRefresh As String = String.Empty
    Dim selProtocol As New Protocol

#End Region

#Region " Properties "

    Public Property SelectedProtocol() As Protocol
        Get
            Return selProtocol
        End Get
        Set(ByVal value As Protocol)
            selProtocol = value
        End Set
    End Property

    Public ReadOnly Property NewPecWindowWidth As String
        Get
            Return ProtocolEnv.PECWindowWidth
        End Get
    End Property

    Public ReadOnly Property NewPecWindowHeight As String
        Get
            Return ProtocolEnv.PECWindowHeight
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Inizializza gli oggetti di pagina
        If Not Me.IsPostBack Then
            txtYear.Text = CurrentProtocol.Year.ToString()
            txtNumber.Focus()
        End If
        Initialize()
    End Sub

    ''' <summary> Gestisce l'evento onclick del pulsante seleziona </summary>
    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        Dim vYear As Short
        Dim vNumber As Integer
        If Not Short.TryParse(txtYear.Text, vYear) And Not Integer.TryParse(txtNumber.Text, vNumber) Then
            Exit Sub
        End If

        ' Ricerca il protocollo selezionato
        SelectedProtocol = Facade.ProtocolFacade.GetById(vYear, vNumber, False)
        ' Se esiste un protocollo per anno/numero
        If SelectedProtocol Is Nothing Then
            ' Non esiste nessun protocollo per l'anno e il numero selezionato
            uscProtocolPreview.Visible = False
            btnAdd.Visible = False
            btnAdd.Enabled = False
            AjaxAlert("Protocollo inesistente")
            Exit Sub
        End If

        Dim selectedProtocolRights As New ProtocolRights(SelectedProtocol)
        If Not selectedProtocolRights.IsLinkable Then
            AjaxAlert("Protocollo n. {0}{1}Mancano i diritti necessari per collegare i due protocolli.", ProtocolFacade.ProtocolFullNumber(txtYear.Text, txtNumber.Text), Environment.NewLine)
            Exit Sub
        End If

        If CheckExistLink(SelectedProtocol, CurrentProtocol) Then
            ' Il protocollo selezionato è gia presente
            uscProtocolPreview.Visible = False
            btnAdd.Visible = False
            btnAdd.Enabled = False
            AjaxAlert("Protocollo già utilizzato")
            Exit Sub
        End If

        ' Verifica che il protocollo selezionato non sia successivo a quello del padre
        If (SelectedProtocol.Year > CurrentProtocol.Year) OrElse (SelectedProtocol.Year = CurrentProtocol.Year AndAlso SelectedProtocol.Number > CurrentProtocol.Number) Then
            uscProtocolPreview.Visible = False
            btnAdd.Visible = False
            btnAdd.Enabled = False
            AjaxAlert("Il protocollo da collegare deve essere precedente a quello attuale.")
            Exit Sub
        End If

        ' Visualizza i dati del protocollo selezionato
        uscProtocolPreview.CurrentProtocol = SelectedProtocol
        uscProtocolPreview.Visible = True
        uscProtocolPreview.Initialize()
        btnAdd.Visible = True
        btnAdd.Enabled = True
        txtYear.Text = SelectedProtocol.Year.ToString()
        txtNumber.Text = SelectedProtocol.Number.ToString()

    End Sub
    ''' <summary> Gestisce l'evento onclick del pulsante aggiungi </summary>
    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim vYear As Short
        Dim vNumber As Integer
        If Short.TryParse(txtYear.Text, vYear) AndAlso Integer.TryParse(txtNumber.Text, vNumber) Then
            ' Ricerca protocollo per anno/numero
            SelectedProtocol = Facade.ProtocolFacade.GetById(vYear, vNumber, False)
            ' Aggiunge il protocollo selezionato come collegamento al protocollo corrente
            Dim fascicleDocumentUnitFacade As WebAPI.Fascicles.FascicleDocumentUnitFacade = New WebAPI.Fascicles.FascicleDocumentUnitFacade(DocSuiteContext.Current.Tenants.ToList(), CurrentTenant)
            Dim fascicleReference As Action(Of Guid, Guid) = AddressOf fascicleDocumentUnitFacade.LinkFascicleReference
            Facade.ProtocolFacade.AddProtocolLink(CurrentProtocol, SelectedProtocol, ProtocolFacade.ProtocolLinkType.Normale, fascicleReference)
            CurrentProtocol.ProtocolLinks.Add(New ProtocolLink() With {.Protocol = CurrentProtocol, .ProtocolLinked = SelectedProtocol})
        End If
        ' Chiude la finestra
        MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscProtocolPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, btnAdd)

        WebUtils.ExpandOnClientNodeAttachEvent(tvwProtocolLink)

        ' Popola il treeview
        ' Primo nodo: protocollo in lavorazione
        Dim firstNode As RadTreeNode = createNode(CurrentProtocol)
        If tvwProtocolLink.Nodes(0).Nodes.Count > 0 Then
            tvwProtocolLink.Nodes(0).Nodes(0).Remove()
        End If
        tvwProtocolLink.Nodes(0).Nodes.Add(firstNode)
        ' Carica nodi figli
        LoadNodes(firstNode)
        ' Disabilito server callback per il protocollo in lavorazione
        firstNode.ExpandMode = TreeNodeExpandMode.ClientSide

    End Sub

    ''' <summary> Crea un nodo del treeview con i dati del protocollo </summary>
    Private Function createNode(ByVal protocol As Protocol) As RadTreeNode

        Dim childNode As New RadTreeNode()
        childNode.Text = $"{protocol.FullNumber} del {protocol.RegistrationDate:dd/MM/yyyy}"
        ' protocollo in entrata
        childNode.ImageUrl = iconMailIU
        If protocol.Type.Id = 1 Then
            childNode.ImageUrl = iconMailU
            ' Protocollo in uscita
        ElseIf protocol.Type.Id = -1 Then
            childNode.ImageUrl = iconMailI
        End If
        ' Protocollo rimosso
        If protocol.IdStatus.HasValue AndAlso protocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            childNode.ImageUrl = iconRemove
        End If
        childNode.Value = protocol.Id.ToString()
        childNode.Expanded = True

        Return childNode
    End Function

    ''' <summary> Carica on-demand figli di un nodo del treeview </summary>
    ''' <param name="vFather">il nodo padre della gerarchia</param>
    Private Sub LoadNodes(ByRef vFather As RadTreeNode)

        Dim vProtocol As Protocol
        'Primo protocollo
        If String.IsNullOrEmpty(vFather.Value) Then
            vProtocol = CurrentProtocol
        Else
            ' Protocolli figli
            Dim vId As Guid = Guid.Parse(vFather.Value)
            ' Protocollo
            vProtocol = Facade.ProtocolFacade.GetById(vId, False)
        End If
        If vProtocol.ProtocolLinks.Count > 0 Then
            ' Per ogni protocollo collegato aggiunge un nodo al treeview
            For Each link As ProtocolLink In vProtocol.ProtocolLinks
                Dim vNode As RadTreeNode
                vNode = createNode(link.ProtocolLinked)
                vFather.Nodes.Add(vNode)
            Next
        End If

    End Sub

    ''' <summary> Verifica se il protocollo selezionato è già collegato al protcollo corrente passato come parametro </summary>
    ''' <param name="selectedProt">Protocollo selezionato</param>
    ''' <param name="currentProt">Protocollo corrente</param>
    ''' <returns>True: il protocollo selezionato è già collegato, False altrimenti</returns>
    Private Function CheckExistLink(ByRef selectedProt As Protocol, ByRef currentProt As Protocol) As Boolean
        Static result As Boolean = False
        If selectedProt.Id.Equals(currentProt.Id) Then
            Return True
        End If
        For Each link As ProtocolLink In currentProt.ProtocolLinks
            result = CheckExistLink(selectedProt, link.ProtocolLinked)
        Next
        Return result
    End Function

#End Region

End Class

