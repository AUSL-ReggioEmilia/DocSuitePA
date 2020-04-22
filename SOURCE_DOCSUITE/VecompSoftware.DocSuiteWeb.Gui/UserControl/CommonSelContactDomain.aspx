<%@ Page AutoEventWireup="false" CodeBehind="CommonSelContactDomain.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactDomain" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione contatto da Rubrica Aziendale" %>

<%@ Register Src="../UserControl/uscSearchADUser.ascx" TagName="uscUserSearch" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function ReturnValuesJson(serializedContact) {
                    CloseWindow(serializedContact);
                    return;
            }

            function ReturnValuesJsonAndNew(serializedContact) {
                GetRadWindow().BrowserWindow.<%= CallerId%>_AddUsersToControl(serializedContact);
            }

            function CloseWindow(contact) {
                GetRadWindow().close(contact);
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc:uscUserSearch runat="server" id="uscUserSearch" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnConfirm" runat="server" Text="Conferma" />
    <telerik:RadButton ID="btnConfermaNuovo" runat="server" Text="Conferma e Nuovo"/>
</asp:Content>
