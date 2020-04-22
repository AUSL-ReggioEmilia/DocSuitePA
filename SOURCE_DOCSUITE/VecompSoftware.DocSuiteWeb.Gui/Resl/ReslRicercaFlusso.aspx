<%@ Page AutoEventWireup="false" Codebehind="ReslRicercaFlusso.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslRicercaFlusso" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscResolutionWorkflowFinder.ascx" TagName="uscResWorkflowFinder" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function <%= Me.ID %>_OpenWindow(name, url, width, height) {
                var manager = $find("<%= RadWindowManagerRicercaFlusso.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerRicercaFlusso" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowReport" ReloadOnShow="false" runat="server" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <usc:uscResWorkflowFinder ID="uscResWorkflowFinder" runat="server" />
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnSearch" Text="Ricerca" runat="server" TabIndex="1" />
</asp:Content>
