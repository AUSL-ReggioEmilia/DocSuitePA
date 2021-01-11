<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReslRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslRicerca"  MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscResolutionFinder.ascx" TagName="uscResolutionFinder" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="rsb">
        <script type="text/javascript">
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnSearch.ClientID%>").click();
                    e.preventDefault();
                }
            });
        </script>
    </telerik:RadScriptBlock>
    <asp:Panel runat="server" ID="searchPanel">
        <usc:uscResolutionFinder ID="uscResolutionFinder" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnSearch" Text="Ricerca" runat="server" TabIndex="1" />
</asp:Content>
