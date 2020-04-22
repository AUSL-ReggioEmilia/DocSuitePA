<%@ Page Title="Ricerca Archivi" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSSearch.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSSearch" %>

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
                            <usc:UDS runat="server" ID="uscUDS" />
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
                    $find("<%= btnSearch.ClientID %>").set_enabled(false);
                }, 200);
                udsScripts.showLoadingPanel();
            }

            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnSearch.ClientID %>").click();
                    e.preventDefault();
                }
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlActionButtons">
        <telerik:RadButton runat="server" ID="btnSearch" OnClientClicked="showLoadinPanel" CausesValidation="false" Width="150" Text="Cerca"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
