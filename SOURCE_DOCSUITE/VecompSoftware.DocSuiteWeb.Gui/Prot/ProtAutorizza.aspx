<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtAutorizza"
    CodeBehind="ProtAutorizza.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Protocollo - Autorizzazioni" %>

<%@ Register Src="~/UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc1" %>
<asp:Content runat="server" ContentPlaceHolderID="cphHeader">

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            function SendDeactivate(uniqueId) {
                if (confirm("Sei sicuro di procedere con la rimozione del settore rifiutato?")) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest("DeactivateNode|" + uniqueId);
                }
                return false;
            }
            function ShowLoadingPanel(timeout) {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);
                if (timeout !== undefined && timeout !== "" && Number.isInteger(timeout)) {
                    setTimeout(function () {
                        var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                        var pnlButtons = "<%= pnlButtons.ClientID%>";
                        var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                        currentLoadingPanel.hide(currentUpdatedControl);
                        ajaxFlatLoadingPanel.hide(pnlButtons);
                    }, timeout);
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlMainContent" Style="height: 100%">

        <uc1:uscProtocollo ID="uscProtocollo" runat="server" />
        <%-- <%-- settori con autorizzazioni rifiutate --%>
        <table class="datatable" id="tbltRefusedAuthorizations" runat="server" visible="false">
            <tr>
                <th colspan="2">Settori con autorizzazione rifiutata</th>
            </tr>
            <tr>
                <td>
                    <telerik:RadTreeView runat="server" ID="TreeViewRefused">
                        <NodeTemplate>
                            <asp:Label ID="RoleName" runat="server" />
                            <asp:ImageButton ID="DeactivateRejectedButton" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" ToolTip="Elimina settore" />
                        </NodeTemplate>
                    </telerik:RadTreeView>
                </td>
            </tr>
        </table>
        <table id="tblDati" cellspacing="1" cellpadding="1" width="100%" border="0">
            <tr>
                <td>
                    <uc1:uscSettori Caption="Autorizzazione" ID="uscAutorizza" MultiSelect="true" MultipleRoles="true" Required="false" runat="server" Type="Prot" />
                </td>
            </tr>
        </table>
        <table id="tblAutorizzazioneCompleta" class="datatable" runat="server">
            <tr>
                <td>
                    <uc1:uscSettori Caption="Autorizzazione completa" ID="uscAutorizzaFull" MultiSelect="true" MultipleRoles="true" runat="server" Type="Prot" />
                </td>
            </tr>
        </table>
        <table id="tblProtocolRoleUser" cellspacing="1" cellpadding="1" width="100%" border="0" runat="server">
            <tr>
                <td>
                    <uc1:uscSettori ID="uscProtocolRoleUser" ReadOnly="true" runat="server" Visible="false" Type="Prot" />
                </td>
                <td style="vertical-align: top; width:50%;" runat="server" id="tableNote"  >
                    <table class="datatable"  style="height:100%">
                        <tr>
                            <th>Note</th>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTextBox ID="txtNote" Rows="8" runat="server" TextMode="MultiLine" Width="100%" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

    </asp:Panel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" OnClientClick="ShowLoadingPanel();" />
    </asp:Panel>
</asp:Content>
