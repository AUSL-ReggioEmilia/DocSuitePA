<%@ Page Title="Signature Tester" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UtltTestSignature.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltTestSignature" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <div class="warningArea">
        <h1>Attenzione</h1>
        Protrebbero essere cambiati dei parametri in caso di errore.
    </div>
    <br/>
    <table class="datatable">
        <tr>
            <th colspan="2">Opzioni</th>
        </tr>
        <tr>
            <td class="label col-dsw-2">Protocollo</td>
            <td>
                <telerik:RadNumericTextBox EmptyMessage="anno" ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" Width="56px" runat="server" />
                <telerik:RadNumericTextBox EmptyMessage="numero" ID="txtNumber" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="96px" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">SignatureType<br/><small>da 0 a 5</small></td>
            <td>
                <telerik:RadNumericTextBox EmptyMessage="numero" ID="txtSignatureType" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="96px" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">ProtocolSignatureFormat</td>
            <td><telerik:RadTextBox runat="server" ID="txtProtocolSignatureFormat" Width="100%" /></td>
        </tr>
        <tr>
            <td class="label col-dsw-2">AttachmentSignatureFormat</td>
            <td><telerik:RadTextBox runat="server" ID="txtAttachmentSignatureFormat" Width="100%" /></td>
        </tr>
        <tr>
            <td class="label col-dsw-2">AnnexedSignatureFormat</td>
            <td><telerik:RadTextBox runat="server" ID="txtAnnexedSignatureFormat" Width="100%" /></td>
        </tr>
        <tr>
            <td class="label col-dsw-2">SignatureString</td>
            <td><telerik:RadTextBox runat="server" ID="txtSignatureString" Width="100%" /></td>
        </tr>
    </table>
    <asp:Button ID="btnTest" runat="server" Text="Test" />
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Repeater runat="server" ID="result">
        <HeaderTemplate>
            <table class="datatable">
                <tr>
                    <th>Tipo</th><th>Signature</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="label col-dsw-2">
                    <%# Eval("Item1")%>
                </td>
                <td>
                    <%# Eval("Item2")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
