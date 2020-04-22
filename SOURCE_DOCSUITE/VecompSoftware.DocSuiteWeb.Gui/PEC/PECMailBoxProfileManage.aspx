<%@ Page Title="Gestione profili caselle PEC" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxProfileManage.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECMailBoxProfileManage" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPageContent">
        <table class="datatable">
            <tr>
                <th colspan="2" class="tabella">Modifica profilo casella PEC
                </th>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Nome configurazione</td>
                <td>
                    <asp:TextBox runat="server" ID="txtConfigurationName" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="txtConfigurationNameRequiredValidator" runat="server" ControlToValidate="txtConfigurationName"
                        ErrorMessage="Il campo Nome configurazione è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Elementi scaricati per sessione</td>
                <td>
                    <asp:TextBox runat="server" ID="txtMaxReadForSession" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="txtMaxReadForSessionRequiredValidator" runat="server" ControlToValidate="txtMaxReadForSession"
                        ErrorMessage="Il campo Elementi scaricati... è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Elementi inviati per sessione</td>
                <td>
                    <asp:TextBox runat="server" ID="txtMaxSendForSession" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="txtMaxSendForSessionRequiredValidator" runat="server" ControlToValidate="txtMaxSendForSession"
                        ErrorMessage="Il campo Elementi inviati... è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Estrazione ZIP</td>
                <td>
                    <asp:CheckBox runat="server" ID="chkZipExtract" /></td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Segna come letto</td>
                <td>
                    <asp:CheckBox runat="server" ID="chkMarkAsRead" /></td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Sposta nella cartella</td>
                <td>
                    <asp:TextBox runat="server" ID="txtMoveToFolder" Width="240" ValidationGroup="PecMailBoxProfile" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Sposta Errore nella cartella</td>
                <td>
                    <asp:TextBox runat="server" ID="txtMoveErrorToFolder" Width="240" ValidationGroup="PecMailBoxProfile" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Cartella Inbox</td>
                <td>
                    <asp:TextBox runat="server" ID="txtInboxFolder" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="txtInboxFolderRequiredValidator" runat="server" ControlToValidate="txtInboxFolder"
                        ErrorMessage="Il campo Cartella Inbox è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Upload Inviate</td>
                <td>
                    <asp:CheckBox runat="server" ID="chkUploadSend" /></td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Cartella Inviate</td>
                <td>
                    <asp:TextBox runat="server" ID="txtFolderSent" Width="240" ValidationGroup="PecMailBoxProfile" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Testo default Pec senza oggetto </td>
                <td>
                    <asp:TextBox runat="server" ID="txtNoSubjectDefaultText" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="ttxtNoSubjectDefaultTextRequiredValidator" runat="server" ControlToValidate="txtNoSubjectDefaultText"
                        ErrorMessage="Il campo Test default Pec... è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Cancella PEC dal server</td>
                <td>
                    <asp:CheckBox runat="server" ID="chkDeleteMailFromServer" /></td>
            </tr>

            <tr>
                <td class="label" style="width: 30%">Dimensione massima Ricevute</td>
                <td>
                    <asp:TextBox runat="server" ID="txtMaxReceiveByteSize" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="txtMaxReceiveByteSizeTextRequiredValidator" runat="server" ControlToValidate="txtMaxReceiveByteSize"
                        ErrorMessage="Il campo Dimensione massima Ricevute è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Dimensione massima Inviate</td>
                <td>
                    <asp:TextBox runat="server" ID="txtMaxSendByteSize" Width="240" ValidationGroup="PecMailBoxProfile" />
                    <asp:RequiredFieldValidator ID="txtMaxSendByteSizeTextRequiredValidator" runat="server" ControlToValidate="txtMaxSendByteSize"
                        ErrorMessage="Il campo Dimensione massima Inviate è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxProfile"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Flag ricerca IMAP</td>
                <td>
                    <asp:DropDownList runat="server" ID="drpSearchIMAP" DataTextField="Value" DataValueField="Key" Width="240" /></td>

            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnSave" runat="server" Text="Salva" Width="150" ValidationGroup="PecMailBoxProfile" />
    <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" Width="150" />
</asp:Content>

