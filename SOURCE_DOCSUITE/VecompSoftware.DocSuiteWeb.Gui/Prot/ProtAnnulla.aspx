<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtAnnulla.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtAnnulla" Title="Protocollo - Annulla" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <uc1:uscProtocollo ID="uscProtocollo" runat="server" />

    <table id="tblDocumentiAllegati" class="datatable" runat="server" width="100%">
        <tr>
            <th width="50%">Documenti</th>
            <th width="50%">Allegati</th>
        </tr>
        <tr>
            <td width="50%" valign="top">
                <uc2:uscDocumentUpload HeaderVisible="false" ID="UscDocumentUpload1" ReadOnly="true" runat="server" TreeViewCaption="Documenti" Type="Prot" />
            </td>
            <td width="50%" valign="top">
                <uc2:uscDocumentUpload HeaderVisible="false" ID="UscDocumentUpload2" ReadOnly="true" runat="server" TreeViewCaption="Allegati" Type="Prot" />
            </td>
        </tr>
    </table>

    <table id="tblAnnullamento" class="datatable" runat="server">
        <tr>
            <th colspan="2" style="width: 885px">Estremi del provvedimento di annullamento del protocollo</th>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox ID="txtAnnulla" runat="server" Width="100%" />
                <asp:RequiredFieldValidator ControlToValidate="txtAnnulla" Display="Dynamic" ErrorMessage="Motivazione obbligatoria" ID="rfvObject" runat="server" /></td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="chkDisableUnlinkPec" runat="server" Text="Scollegare anche le PEC dal protocollo." Checked="false" Enabled="false" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma annullamento"></asp:Button>
</asp:Content>
