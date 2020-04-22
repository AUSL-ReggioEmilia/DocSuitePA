<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltCategorySchemaGes.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltCategorySchemaGes" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltCategorySchemaGes;
            require(["Tblt/TbltCategorySchemaGes"], function (TbltCategorySchemaGes) {
                $(function () {
                    tbltCategorySchemaGes = new TbltCategorySchemaGes();
                    tbltCategorySchemaGes.initialize();
                });
            });

            function CloseWindow(operator) {
                tbltCategorySchemaGes.closeWindow(operator);
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadTreeView runat="server" ID="RadTreeViewSelectedCategory" EnableViewState="false" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" Width="100%">
        <table class="dataform">
            <tr>
                <td class="label col-dsw-3">
                    <asp:Label runat="server" Text="Versione:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldVersion" runat="server" Width="100%" DataField="Version" Style="margin-left: 3px;" ReadOnly="true" Enabled="false"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label col-dsw-3">
                    <asp:Label runat="server" Text="Note:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldNote" runat="server" Width="100%" ReadOnly="true" Style="margin-left: 3px;" Enabled="false"></telerik:RadTextBox>
                </td>
            </tr>
            <tr runat="server" id="rowOldStartDate">
                <td class="label col-dsw-3" align="right">
                    <asp:Label runat="server" Text="Data di attivazione:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadDatePicker ID="rdpOldStartDate" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" DateInput-ReadOnly="true" Enabled="false" Width="150" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="rowNewNote">
                <td class="label col-dsw-3" align="right">
                    <asp:Label runat="server" Text="Note:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtNewNote" runat="server" Style="margin-left: 3px;" Width="100%"></telerik:RadTextBox>
                </td>
            </tr>
            <tr runat="server" id="rowNewEndDate">
                <td class="label col-dsw-3" align="right">
                    <asp:Label runat="server" Text="Data di Disattivazione:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadDatePicker ID="rdpNewEndDate" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvNewEndDate" runat="server" Display="Dynamic" ErrorMessage="Campo Data di Disattivazione Obbligatorio" ControlToValidate="rdpNewEndDate" />
                </td>
            </tr> 
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConferma" runat="server" Text="Conferma"></telerik:RadButton>
</asp:Content>
