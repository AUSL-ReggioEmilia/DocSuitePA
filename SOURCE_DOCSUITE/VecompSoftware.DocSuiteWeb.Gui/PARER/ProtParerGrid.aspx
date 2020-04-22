<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtParerGrid.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtParerGrid" %>

<%@ Register Src="../UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <uc1:uscProtGrid runat="server" ID="uscProtocolGrid" />
    </telerik:RadScriptBlock>
</asp:Content>
