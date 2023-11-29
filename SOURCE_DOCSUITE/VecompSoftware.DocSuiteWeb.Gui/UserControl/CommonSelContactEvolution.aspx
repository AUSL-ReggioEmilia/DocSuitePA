<%@ Page AutoEventWireup="false" CodeBehind="CommonSelContactEvolution.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactEvolution" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Inserimento Contatti e Liste di Distribuzione" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function ReturnValuesJson(serializedContact, close) {
                if (close == "true") {
                    CloseWindow(serializedContact);
                    return;
                }
                GetRadWindow().BrowserWindow.<%= CallerId%>_CloseManualMulti(serializedContact);
            }

            function CloseWindow(contact) {
                GetRadWindow().close(contact);
            }

            function btnConfirm_OnClick(sender, args) {
                CommonSelContactEvolutionSend('btnConfirm');
                return false;
            }

            function btnSearch_OnClick(sender, args) {
                CommonSelContactEvolutionSend('btnSearch');
                return false;
            }

            function CommonSelContactEvolutionSend(val) {
                var res = document.getElementById('<%=txtFilter.ClientID%>').value;
                document.getElementById('<%=txtFilter.ClientID%>').value = ""
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(val+'|'+res);
                return false;
            }
        </script>

    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="ajaxPanel">
        <table class="datatable">
            <tr>
                <td class="label">Inserisci indirizzi:</td>
                <td>
                    <asp:Panel DefaultButton="btnSearch" runat="server" Style="display: inline;">
                        <asp:TextBox ID="txtFilter" runat="server" width="430px"/>
                    </asp:Panel>
                    <telerik:RadButton ID="btnSearch" runat="server" Text="Controlla nomi" AutoPostBack="false" OnClientClicked="btnSearch_OnClick" />
                </td>
            </tr>
        </table>
        <telerik:RadTreeView ID="tvwContactDomain" runat="server" Visible="false"/>
    </asp:Panel>    
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnConfirm" runat="server" Text="Conferma" AutoPostBack="false" OnClientClicked="btnConfirm_OnClick"/>
</asp:Content>
