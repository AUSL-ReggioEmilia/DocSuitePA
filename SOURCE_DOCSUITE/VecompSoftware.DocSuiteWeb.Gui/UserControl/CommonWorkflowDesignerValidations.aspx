<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonWorkflowDesignerValidations.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonWorkflowDesignerValidations" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">

            var commonWorkflowDesignerValidations;
            require(["UserControl/CommonWorkflowDesignerValidations"], function (CommonWorkflowDesignerValidations) {
                $(function () {
                    commonWorkflowDesignerValidations = new CommonWorkflowDesignerValidations(tenantModelConfiguration.serviceConfiguration);
                    commonWorkflowDesignerValidations.btnConfirmId = "<%=btnConfirm.ClientID%>";
                    commonWorkflowDesignerValidations.pageContentId = "<%=PageContent%>";
                    commonWorkflowDesignerValidations.lblNameId = "<%=lblName.ClientID%>";
                    commonWorkflowDesignerValidations.lblMessageErrorId = "<%=lblMessageError.ClientID%>";
                    commonWorkflowDesignerValidations.dvCheckBoxListControlId = "<%=dvCheckBoxListControl.ClientID%>";
                    commonWorkflowDesignerValidations.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table class="dataform">
        <tr>
            <td class="label col-dsw-2">Cartelle</td>

            <td>
                <asp:TextBox runat="server" ID="lblName"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">Messaggio di errore</td>

            <td>
                <asp:TextBox runat="server" ID="lblMessageError"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label col-dsw-2">Proprietà</td>

            <td>
                <div id="dvCheckBoxListControl" runat="server">
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnConfirm" runat="server" Text="Conferma" />
</asp:Content>
