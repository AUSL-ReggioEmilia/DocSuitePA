Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Text

Public Class ProtRiferimento
    Inherits ProtBasePage

#Region " Constants "
    Const PIPE As String = "|"
    Const SEP As String = ","
#End Region

#Region " Fields "

#End Region

#Region "Properties"

#End Region

#Region " Events "

    Private Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        WebUtils.ExpandOnClientNodeAttachEvent(tvwPratiche)

        'verifica diritti
        If Not CurrentProtocolRights.IsEditable Then
            Throw New DocSuiteException(
                $"Protocollo n. {CurrentProtocol.FullNumber}",
                "Mancano diritti di Autorizzazione",
                Request.Url.ToString(),
                DocSuiteContext.Current.User.FullUserName)
        End If

        UscProtocollo.CurrentProtocol = CurrentProtocol

        UscProtocollo.VisibleAltri = False
        UscProtocollo.VisibleClassificazione = False
        UscProtocollo.VisibleFascicolo = True
        UscProtocollo.VisibleMittentiDestinatari = True
        UscProtocollo.VisibleOggetto = False
        UscProtocollo.VisibleProtocollo = True
        UscProtocollo.VisibleSettori = False
        UscProtocollo.VisibleStatoProtocollo = False
        UscProtocollo.VisibleTipoDocumento = False
        UscProtocollo.VisibleScatolone = False

        Dim contacts As New StringBuilder
        For Each protContact As ProtocolContact In CurrentProtocol.Contacts
            If protContact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) OrElse protContact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                If contacts.Length <> 0 Then
                    contacts.Append(",")
                End If
                contacts.Append(protContact.Contact.Id.ToString())
            End If
        Next

        If contacts.Length = 0 Then
            Exit Sub
        End If

        Dim documents As IList(Of DocumentContact)
        documents = Facade.DocumentFacade.GetDocumentProtocol(contacts.ToString())

        If Not DocSuiteContext.Current.DocumentEnv.IsPubblicationEnabled OrElse documents.Count <= 0 Then
            Exit Sub
        End If

        For Each dc As DocumentContact In documents
            Dim nodo As New RadTreeNode
            nodo.Expanded = True
            nodo.Value = dc.Year & PIPE & dc.Number
            nodo.Text = "<B>" & dc.Document.Id.ToString() & "</B> " & dc.Document.DocumentObject & " (" & Replace(dc.Contact.Description, PIPE, " ") & ")"
            nodo.ImageUrl = ImagePath.SmallEmpty
            Dim s As String = "Year=" & dc.Year & "&Number=" & dc.Number
            nodo.NavigateUrl = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(s)
            tvwPratiche.Nodes(0).Nodes.Add(nodo)
        Next
    End Sub

#End Region
End Class

