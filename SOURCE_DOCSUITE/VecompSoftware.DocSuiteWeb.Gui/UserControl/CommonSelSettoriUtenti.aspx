<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonSelSettoriUtenti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelSettoriUtenti" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }		
        	
            function ReturnValues(values)
            {
                var oWindow = GetRadWindow();
                oWindow.close(values);    
            }
        </script>			
    </telerik:RadScriptBlock>

    <telerik:RadTreeView ID="RadTreeUsers" runat="server" Width="100%" EnableViewState="true" CheckBoxes="true">
        <Nodes>
            <telerik:RadTreeNode runat="server" Text="Titolo" Font-Bold="true" Expanded="true" Checkable="false" EnableViewState="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/bricks.png" />
        </Nodes>
    </telerik:RadTreeView>
 </asp:Content>
 
 <asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma Selezione"></asp:Button>
 </asp:Content>
