<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltWorkflowRepositoryGes.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltWorkflowRepositoryGes" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc3" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltWorkflowRepositoryGes;
            require(["Tblt/TbltWorkflowRepositoryGes"], function (TbltWorkflowRepositoryGes) {
                $(function () {
                    tbltWorkflowRepositoryGes = new TbltWorkflowRepositoryGes();
                    tbltWorkflowRepositoryGes.rowOldMappingTagId = "rowOldMappingTag";
                    tbltWorkflowRepositoryGes.rowOldAuthorizationTypeId = "rowOldAuthorizationType";
                    tbltWorkflowRepositoryGes.rowOldroleId = "rowOldrole";
                    tbltWorkflowRepositoryGes.rowOldContactId = "rowOldContact";
                    tbltWorkflowRepositoryGes.titleId = "<%= MasterDocSuite.TitleContainer.ClientID %>";
                    tbltWorkflowRepositoryGes.action = "<%= Action %>";
                    tbltWorkflowRepositoryGes.btnConfermaId = "<%= btnConferma.ClientID %>";
                    tbltWorkflowRepositoryGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltWorkflowRepositoryGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltWorkflowRepositoryGes.pageContentId = "<%= pnlRinomina.ClientID %>";
                    tbltWorkflowRepositoryGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table class="dataform">
            <!-- OLD -->
            <tr id="rowOldMappingTag">
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblOldMappingTag" Text="Tag:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <telerik:RadTextBox ID="txtOldMappingTag" runat="server" MaxLength="100" Width="40%" Enabled="False"></telerik:RadTextBox>
                </td>
            </tr>
            <tr id="rowOldAuthorizationType">
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblOldAuthorizationType" Text="Tipo Autorizzazione:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <asp:DropDownList ID="ddlOldAuthorizationType" AppendDataBoundItems="True" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="rowOldrole">
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblOldRole" Text="Settore:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <telerik:RadTextBox ID="txtOldRole" runat="server" MaxLength="100" Width="100%" Enabled="False"></telerik:RadTextBox>
                </td>
            </tr>
            <tr id="rowOldContact">
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblOldContact" Text="Utente:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <uc3:uscContatti ReadOnly="true" runat="server" ID="uscOldContatti" HeaderVisible="false" />
                </td>
            </tr>
            <!-- EDIT -->
            <tr>
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblNewMappingTag" Text="Tag:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <telerik:RadTextBox ID="txtNewMappingTag" runat="server" MaxLength="100" Width="40%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="rfvNewMappingTag" runat="server" Display="Dynamic" ErrorMessage="Campo Tag Obbligatorio" ControlToValidate="txtNewMappingTag" />
                </td>
            </tr>
            <tr>
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblNewAuthorizationType" Text="Tipo Autorizzazione:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <asp:DropDownList ID="ddlNewAuthorizationType" AppendDataBoundItems="True" OnSelectedIndexChanged="DdlNewAuthorizationType_SelectedIndexChanged" AutoPostBack="True" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblNewRole" Text="Settore:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <asp:Panel runat="server" ID="pnlSettoriNew">
                        <uc3:uscSettori HeaderVisible="false" ID="uscSettoriNew" MultiSelect="false" Required="true" RequiredMessage="settore obbligatorio" runat="server" RoleRestictions="None" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td class="label col-dsw-3">
                    <asp:Label runat="server" ID="lblNewContact" Text="Utente:" Font-Bold="true"></asp:Label>
                </td>
                <td class="col-dsw-7">
                    <asp:Panel runat="server" ID="pnlNewContact">
                        <uc3:uscContatti ID="uscContattiNew" HeaderVisible="false" ButtonAddMyselfVisible="false" ButtonImportManualVisible="false" ButtonImportVisible="false" ButtonIPAVisible="false"
                            ButtonManualMultiVisible="false" ButtonManualVisible="false" ButtonRoleVisible="false" ButtonSdiContactVisible="false" ButtonSelectOChartVisible="false"
                            ButtonSelectVisible="false" ButtonSelectDomainVisible="true" Multiple="false" MultiSelect="false"
                            Required="true" RequiredMessage="utente obbligatorio" runat="server" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConferma" runat="server" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>
