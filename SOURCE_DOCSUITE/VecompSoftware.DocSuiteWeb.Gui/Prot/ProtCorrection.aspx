<%@ Page AutoEventWireup="false" CodeBehind="ProtCorrection.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtCorrection" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Correzione" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagPrefix="uc" TagName="uscOggetto" %>
<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagName="ProtocolPreview" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <usc:ProtocolPreview runat="server" ID="uscProtocolPreview" Type="Prot" Title="Protocollo da correggere" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlCorrectionContent">
        <%-- Oggetto --%>
        <table class="datatable">
            <tr>
                <th>Oggetto
                </th>
            </tr>
            <tr>
                <td>
                    <uc:uscOggetto runat="server" ID="uscObject" EditMode="true" MultiLine="true" MaxLength="255" Required="true" Type="Prot" />
                </td>
            </tr>
        </table>
        <%--Contenitore--%>
        <table id="tblEditContenitore" runat="server" class="datatable">
            <tr>
                <th colspan="2">Contenitore</th>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Contenitore</td>
                <td style="width: 85%;">
                    <telerik:RadComboBox AutoPostBack="true" CausesValidation="false" EnableLoadOnDemand="true" ID="rcbContainer" ItemRequestTimeout="500" MarkFirstMatch="true" runat="server" Width="300px" />
                    &nbsp;
                <asp:RequiredFieldValidator Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" ID="rfvContainer" ControlToValidate="rcbContainer" runat="server" />
                </td>
            </tr>
        </table>
        <%-- Scelta Contatti --%>
        <table class="datatable">
            <tr>
                <td style="width: 50%; white-space: nowrap;">
                    <uc:uscContattiSel ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="true" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" Caption="Mittenti" EnableCompression="true" EnableCC="false" ID="uscMittenti" IsRequired="false" Multiple="true" MultiSelect="true" ProtType="true" ReadOnlyProperties="true" runat="server" Type="Prot" />
                </td>
                <td style="width: 50%; white-space: nowrap;">
                    <uc:uscContattiSel ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="true" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" Caption="Destinatari" EnableCompression="true" EnableCC="True" ID="uscDestinatari" IsRequired="false" Multiple="true" MultiSelect="true" ProtType="true" ReadOnlyProperties="true" runat="server" Type="Prot" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="btnCorrection" Text="Correggi" Width="150" />
</asp:Content>
