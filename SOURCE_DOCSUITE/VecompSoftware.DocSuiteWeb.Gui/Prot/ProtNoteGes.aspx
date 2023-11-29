<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtNoteGes" CodeBehind="ProtNoteGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Modifica Note" %>

<%@ Register Src="~/UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc" %>
<%@ Register Src="../UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <%--documenti--%>
    <uc:uscDocumentUpload ID="uscDocumento" IsDocumentRequired="false" ReadOnly="true" runat="server" Caption="Documento" TreeViewCaption="Documento" Type="Prot" />
    <uc:uscDocumentUpload ID="uscAllegati" IsDocumentRequired="false" ReadOnly="true" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" Caption="Allegati (parte integrante)" TreeViewCaption="Allegati" />
    <uc:uscDocumentUpload ID="uscAnnexes" IsDocumentRequired="false" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" Prefix="" runat="server" Caption="Annessi (non parte integrante)" TreeViewCaption="Annessi" />
    <%--protocollo--%>
    <uc:uscProtocollo ID="uscProtocollo" runat="server" />
    <%--note--%>
    <table class="datatable">
        <tr>
            <th width="50%">Autorizzazioni</th>
            <th width="50%" colspan="3">Note</th>
        </tr>
        <tr>
            <td>
                <uc:uscSettori HeaderVisible="false" ID="uscAutorizza" ReadOnly="true" Required="false" runat="server" />
            </td>
            <td>
                <telerik:RadTextBox ID="txtNote" Rows="4" runat="server" TextMode="MultiLine" Width="100%" />
            </td>
            <td style="width: 20px; display: none;" runat="server" id="colAcceptanceRole">
                <asp:RadioButtonList AutoPostBack="true" ID="rblAcceptance" RepeatDirection="Vertical" runat="server">
                    <asp:ListItem Text='<img src="../App_Themes/DocSuite2008/imgset16/accept.png"/> Accetta' Value="1" />
                    <asp:ListItem Text='<img  src="../App_Themes/DocSuite2008/imgset16/delete.png"/> Rifiuta' Value="-1" />
                </asp:RadioButtonList>
            </td>
            <td style="width: 20px; display: none;" runat="server" id="colVisibilityNoteRole">
                <asp:CheckBox runat="server" ID="cbVisibilityNoteRole" Checked="false" AutoPostBack="true" Text="Riservata" value="1" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConferma" runat="server" Text="Conferma modifica" />
</asp:Content>

