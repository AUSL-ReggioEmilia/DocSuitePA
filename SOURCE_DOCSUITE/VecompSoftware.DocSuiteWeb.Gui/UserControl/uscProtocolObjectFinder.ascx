<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtocolObjectFinder.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocolObjectFinder" %>
<table class="datatable">
    <tr>
        <td class="DXChiaro">
            <asp:Panel runat="server" ID="pnlObjectFinder" DefaultButton="btnSearch">
                <asp:Label Font-Bold="true" ID="lblDateFrom" runat="server" Text="Data da:" />
                &nbsp;
                <telerik:RadDatePicker ID="rdpRegDate_From" runat="server" />
                <asp:CompareValidator ControlToValidate="rdpRegDate_From" Display="Dynamic" ErrorMessage="Errore formato" ID="cvRegDate_From" Operator="DataTypeCheck" runat="server" Type="Date" />
                &nbsp;
                        
                <asp:Label Font-Bold="true" ID="lblDateTo" runat="server" Text="a:" />
                &nbsp;
                <telerik:RadDatePicker ID="rdpRegDate_To" runat="server" />
                <asp:CompareValidator ControlToValidate="rdpRegDate_To" Display="Dynamic" ErrorMessage="Errore formato" ID="cvRegDate_To" Operator="DataTypeCheck" runat="server" Type="Date" />
                &nbsp;                    
                &nbsp;
                <asp:Label Font-Bold="true" ID="lblNumberFrom" runat="server" Text="Num. da:" />
                &nbsp;
                <asp:TextBox ID="txtNumber_From" MaxLength="10" runat="server" style="vertical-align:middle;" Width="60px" />
                <asp:RegularExpressionValidator ControlToValidate="txtNumber_From" Display="Dynamic" ErrorMessage="Errore formato" ID="cvNumber_From" runat="server" ValidationExpression="\d*" />
                <asp:Label Font-Bold="true" ID="lblNumberTo" runat="server" Text="a:" />
                &nbsp;
                <asp:TextBox ID="txtNumber_To" MaxLength="10" runat="server" style="vertical-align:middle;" Width="60px" />
                <asp:RegularExpressionValidator ControlToValidate="txtNumber_To" Display="Dynamic" ErrorMessage="Errore formato" ID="cvNumber_To" runat="server" ValidationExpression="\d*" />
                &nbsp;
                <asp:Label Font-Bold="true" ID="lblContainer" runat="server" Text="Contenitore:" />
                &nbsp;
                <asp:DropDownList ID="ddlContainer" runat="server" AppendDataBoundItems="True" style="vertical-align:middle;">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </asp:Panel>
        </td>
    </tr>
</table>
<asp:Button ID="btnSearch" runat="server" Text="Ricerca" />
