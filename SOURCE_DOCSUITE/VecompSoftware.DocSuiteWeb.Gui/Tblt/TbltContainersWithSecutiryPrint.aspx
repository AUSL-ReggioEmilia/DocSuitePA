<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.PrintContainersWithSecurity" Codebehind="TbltContainersWithSecutiryPrint.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Contenitore per Stampa"%>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table style="height: 100%; width: 100%;">
        <tr style="height: 13px">
            <td>
                <asp:button CausesValidation="False" id="btnSelectAll" runat="server" Text="Seleziona tutti" Width="120px" />
                <asp:button CausesValidation="False" id="btnDeselectAll" runat="server" Text="Annulla selezione" Width="120px" />
            </td>
            <td style="vertical-align: top; width: 50%; border-left: 1px solid black;">&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top; width: 50%;">
                <telerik:RadTreeView ID="RadTreeContainer" runat="server" CheckBoxes="true">
                    <Nodes>
                        <telerik:RadTreeNode Checkable="false" Expanded="true" runat="server" Text="Contenitore" />
                    </Nodes>
                </telerik:RadTreeView>
            </td>
            <td style="vertical-align: top; width: 50%; border-left: 1px solid black;">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button id="cmdStampa" runat="server" Text="Stampa" />   
</asp:Content>