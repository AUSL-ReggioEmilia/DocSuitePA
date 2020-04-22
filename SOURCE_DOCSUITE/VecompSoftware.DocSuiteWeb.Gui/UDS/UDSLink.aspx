<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UDSLink.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSLink" %>

<%@ Register Src="UserControl/uscUDS.ascx" TagPrefix="usc" TagName="UDS" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
        <Rows>
            <telerik:LayoutRow RowType="Container" HtmlTag="None" CssClass="col-dsw-10">
                <Columns>
                    <telerik:CompositeLayoutColumn HtmlTag="None">
                        <Content>
                            <usc:UDS runat="server" ActionType="Search" ID="uscUDS" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var udsScripts = new UscUDSScripts();
            function showLoadinPanel(sender, args) {
                setTimeout(function () {
                    $find("<%= btnConnect.ClientID %>").set_enabled(false);
                }, 200);
                udsScripts.showLoadingPanel();
            }

            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnConnect.ClientID %>").click();
                    e.preventDefault();
                }
            });
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var udsLink;
            require(["UDS/UDSLink"], function (UDSLink) {
                $(function () {
                    udsLink = new UDSLink();
                    udsLink.btnConnectId = "<%= btnConnect.ClientID %>";
                    udsLink.currentUDSRepositoryId = "<%= CurrentUDSRepositoryId %>";
                    udsLink.currentIdUDS = "<%= CurrentIdUDS %>";
                    udsLink.selectedUDSRepositoryId = "<%= SelectedUDSRepository %>";
                    udsLink.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    udsLink.initialize();
                });
            });

            function UDSRepositoryOnChange(currentRepositoryId) {
                if (udsLink && currentRepositoryId) {
                    udsLink.selectedUDSRepositoryId = currentRepositoryId;
                }
            };
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlActionButtons">
        <telerik:RadButton runat="server" ID="btnConnect" OnClientClicked="showLoadinPanel" AutoPostBack="false" CausesValidation="false" Width="150" Text="Cerca"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
