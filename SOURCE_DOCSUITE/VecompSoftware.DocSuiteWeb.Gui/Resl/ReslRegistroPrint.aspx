<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReslRegistroPrint.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslRegistroPrint" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="cphHeader">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnSelectAll">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnDeselectAll">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <table id="TBLTITLE" class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%">Tipologia:</td>
            <td style="width: 80%;">
                <asp:RadioButtonList ID="rblTipologia" runat="server" RepeatDirection="Horizontal">
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">
                <asp:Label ID="lblDa" runat="server" Font-Bold="True" Width="99px">Dal giorno:</asp:Label>
            </td>
            <td style="width: 80%">
                <telerik:RadDatePicker ID="RadDatePicker1" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="RadDatePicker1" ErrorMessage="Campo dal giorno obbligatorio" ID="RequiredFieldValidator2" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">
                <asp:Label ID="lblAl" runat="server" Font-Bold="True" Width="99px">Al giorno:</asp:Label>
            </td>
            <td style="width: 80%; padding-bottom: 5px;">
                <telerik:RadDatePicker ID="RadDatePicker2" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="RadDatePicker2" Display="Dynamic" ErrorMessage="Campo al giorno obbligatorio" ID="RequiredFieldValidator1" runat="server" />
            </td>
            <asp:CompareValidator ID="dateCompareValidator" runat="server" ControlToValidate="Raddatepicker2"
                ControlToCompare="RadDatePicker1" Operator="GreaterThanEqual" Type="Date" ErrorMessage="La Seconda data deve essere Superiore alla prima"
                Display="Dynamic"></asp:CompareValidator></tr>
    </table>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <table style="height: 100%; width: 100%;">
        <%-- Contenitore --%>
        <tr style="height: 13px">
            <td>
                <asp:Button ID="btnSelectAll" runat="server" Width="120px" CausesValidation="False"
                    Text="Seleziona tutti"></asp:Button>
                <asp:Button ID="btnDeselectAll" runat="server" Width="120px" Text="Annulla selezione"
                    CausesValidation="False"></asp:Button>
            </td>
            <td style="vertical-align: top; width: 50%; border-left: 1px solid black;">&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top; width: 50%;">
                <telerik:RadTreeView ID="RadTreeView1" CheckBoxes="True" runat="server">
                    <CollapseAnimation Duration="100" Type="OutQuint" />
                    <ExpandAnimation Duration="100" Type="OutQuart" />
                </telerik:RadTreeView>
            </td>
            <td style="vertical-align: top; width: 50%; border-left: 1px solid black;">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="cmdStampaPDF" runat="server" Text="Stampa"></asp:Button>
</asp:Content>
