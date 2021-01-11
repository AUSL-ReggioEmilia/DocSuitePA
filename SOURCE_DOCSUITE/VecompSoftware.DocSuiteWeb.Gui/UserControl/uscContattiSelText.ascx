<%@ Control AutoEventWireup="false" Codebehind="uscContattiSelText.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContattiSelText" Language="vb" %>
<telerik:RadScriptBlock runat="server" ID="codeBlock">
    <script type="text/javascript">
        function <%= ID %>_OpenWindow(url, name, onClientClose) {
            var wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.NORMAL);
            wnd.set_visibleStatusbar(false);
            if (onClientClose) {
                wnd.add_close(onClientClose);
            }
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Minimize);
            wnd.add_close(<%=Me.ID%>_CloseFunction);
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();
            return false;
        }
        
        //richiamata quando la finestra rubrica viene chiusa
        function <%= ID%>_OnClose(sender, args) {
            sender.remove_close(<%= ID%>_OnClose);
            if (args.get_argument() !== null) {
                <%= ID%>_Update("ADDRESS|" + args.get_argument());
            }
        }

        //richiamata quando la finestra Mitt/Dest rubrica viene chiusa
        function <%= ID%>_CloseMittDest(sender, args) {
            sender.remove_close(<%= ID%>_CloseMittDest);
            if (args.get_argument() !== null) {
                document.getElementById('<%=txtContact.clientID %>').innerText = args.get_argument();
            }
        }

        //richiamata quando la finestra contatti AD viene Chiusa
        function  <%= Me.ID %>_CloseDomain(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseDomain);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument(), 'ContactAD');
            }
        }

        //esegue l'aggiornamento della lista contatti manuali
        function <%= Me.ID %>_UpdateManual(contact, action) {
            //restituito un oggetto javascript serializzato con JSon
            $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|" + action + "|" + contact);
        }

        //esegue l'aggiornamento della lista dei contatti
        function <%= ID%>_Update(args) {
            //restituito un oggetto javascript serializzato con JSon
            $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|" + args);
        }
    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerContact" runat="server">
    <Windows>
        <telerik:RadWindow ReloadOnShow="false" ID="windowSelContact" runat="server" Title="Selezione Contatto" />
    </Windows>
</telerik:RadWindowManager>
<table width="100%" runat="server" id="tblContactText">
    <tr class="Chiaro" style="height: 100%;">
        <td style="vertical-align: middle; padding: 0 !important;">
            <telerik:RadTextBox runat="server" ID="txtContact" Width="100%"></telerik:RadTextBox>
        </td>
        <td runat="server" ID="pnlButtons" style="width:50px; vertical-align: middle; text-align: center; white-space: nowrap; text-align: left;">
            <asp:ImageButton CausesValidation="False" ID="btnADContact" Visible="false" ImageUrl="../Comm/Images/ActiveDirectory16.png" runat="server" />
            <asp:ImageButton CausesValidation="False" ID="btnSelContact" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" />
            <asp:ImageButton CausesValidation="False" ID="btnDelContact" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" runat="server" />
        </td>
    </tr>
</table>