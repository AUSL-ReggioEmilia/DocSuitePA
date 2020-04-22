<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocmRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscDocmGrid.ascx" TagName="UscDocmGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscDocmGridBar.ascx" TagName="UscDocmGridBar" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript" language="javascript">
            function DC_Visualizza(param) {
                location.href = '"DocmVisualizza.aspx?' & param;                
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
    <telerik:RadWindowManager EnableViewState="False" ID="alertManagerDC" runat="server">
        <Windows>
            <telerik:RadWindow Height="550" ID="windowDocmFile" runat="server" Width="700" />
            <telerik:RadWindow Height="550" ID="windowDocmProt" runat="server" Width="700" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphcontent">
    <uc1:UscDocmGrid ID="uscDocumentGrid" runat="server" />
</asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="cphFooter">
    <div class="dsw-text-right">
        <uc1:UscDocmGridBar runat="server" ID="uscDocumentGridBar"></uc1:UscDocmGridBar>                
    </div>
</asp:Content>
