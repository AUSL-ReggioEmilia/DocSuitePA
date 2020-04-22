<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FascRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascRicerca"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Fascicoli - Ricerca" %>

<%@ Register Src="../UserControl/uscFascicleFinder.ascx" TagName="uscFascicleFinder" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="rsb">
        <script type="text/javascript">     
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnSearch.ClientID %>").click();
                    e.preventDefault();
                }
            });
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadPageLayout runat="server" HtmlTag="Div" ID="pageContent" Height="100%">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div" Height="100%">
                <Content>
                    <uc1:uscFascicleFinder ID="uscFascicleFinder" runat="server" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnSearch" Text="Ricerca" Width="150px" runat="server" TabIndex="1" />
</asp:Content>
