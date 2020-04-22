<%@ Page AutoEventWireup="false" CodeBehind="ReportisticaPEC.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReportisticaPEC" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="dataform">
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label ID="lblDa" runat="server" Font-Bold="True" Width="99px">Anno:</asp:Label>
            </td>
            <td align="left" >
                <asp:DropDownList AutoPostBack="true" Enabled="true" ID="Years" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="Years" ErrorMessage="Campo dal giorno obbligatorio" ID="RequiredFieldValidator2" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label ID="lblAl" runat="server" Font-Bold="True" Width="99px">Mese:</asp:Label>
            </td>
            <td align="left" >
                <asp:DropDownList runat="server" ID="Months" Enabled="true">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="Months" Display="Dynamic" ErrorMessage="Campo al giorno obbligatorio" ID="RequiredFieldValidator1" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <b>Seleziona Casella di posta:</b>
            </td>
            <td style="vertical-align: middle; font-size: 8pt">
                <asp:DropDownList DataTextField="MailBoxName" DataValueField="Id" ID="ddlMailbox" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button ID="cmdStampa" runat="server" Text="Stampa" />
</asp:Content>
