<%@ Page AutoEventWireup="false" CodeBehind="CommonSelMailRecipients.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelMailRecipients" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Destinatari" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                if (oWindow != null) {
                    oWindow.close(args);
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Button CausesValidation="False" ID="btnSelectAll" runat="server" Text="Seleziona tutti" Width="120px" />
    <asp:Button CausesValidation="False" ID="btnDeselectAll" runat="server" Text="Annulla selezione" Width="120px" />
    <uc1:uscSettori HeaderVisible="false" Caption="Settori" Checkable="true" ID="uscSettori" MultiSelect="true" ReadOnly="true" Required="false" runat="server" />
</asp:content>

<asp:content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnInvia" runat="server" Text="Conferma" />
</asp:content>
