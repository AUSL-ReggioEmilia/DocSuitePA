<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslPubblicaWeb.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslPubblicaWeb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscReslGrid.ascx" TagName="uscReslGrid" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReslGridBar.ascx" TagName="uscReslGridBar" TagPrefix="usc" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="False">
        <script language="javascript" type="text/javascript">
            
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
            
            function CloseWindow(argument)
            {
                var oWindow = GetRadWindow();
                oWindow.close(argument);    
            } 
            
             
        </script>
    </telerik:RadScriptBlock>
        
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">

    <table class="datatable">
        <tr>
            <th><asp:Label ID="lblMailbox" runat="server" EnableViewState="false" Text="Ricerca"></asp:Label></th> 
        </tr>
        <tr>
            <td>
                <table cellspacing="0" cellpadding="2" width="100%" border="0">
                    <tr>
                        <td style="width: 200px; vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Stato:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <asp:DropDownList ID="ddlType" AutoPostBack="true" runat="server">
                                <asp:ListItem Value="0" Text="">Pubblicati</asp:ListItem>
                                <asp:ListItem Value="1" Text="">Scaduti</asp:ListItem>
                                <asp:ListItem Value="2" Text="">Tutti</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>         

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="true">
        <usc:uscReslGrid runat="server" id="uscReslGrid" />
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <div style="width:100%;" align="right">
        <usc:uscReslGridBar runat="server" ID="uscReslGridBar"></usc:uscReslGridBar>
    </div>
</asp:Content>