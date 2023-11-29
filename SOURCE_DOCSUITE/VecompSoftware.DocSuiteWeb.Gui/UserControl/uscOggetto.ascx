<%@ Control AutoEventWireup="false" CodeBehind="uscOggetto.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscOggetto" Language="vb" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var <%= Me.ClientID %>_uscOggetto;
        require(["UserControl/uscOggetto"], function (UscOggetto) {
            $(function () {
                <%= Me.ClientID %>_uscOggetto = new UscOggetto();
                <%= Me.ClientID %>_uscOggetto.contentId = "<%=pnlId.ClientID%>";
                <%= Me.ClientID %>_uscOggetto.validationId = "<%=rfvObject.ClientID%>";
                <%= Me.ClientID %>_uscOggetto.txtObjectId = "<%=txtObject.ClientID%>";
                <%= Me.ClientID %>_uscOggetto.initialize();
            });
        });

        function <%= Me.ID %>_OpenObjectWindow(button, args) {
            if (args._commandArgument === null)
                return false;

            var wnd = DSWOpenGenericWindow("<%=GetUrl()%>", WindowTypeEnum.SMALL);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.set_showOnTopWhenMaximized(false);
            wnd.set_destroyOnClose(true);
            wnd.add_close(<%= Me.ID %>_OnClose);
            wnd.setSize(700, 460);
            return true;
        }





        function <%= Me.ID %>_OnClose(sender, args) {
            if (args.get_argument() !== null) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ID %>|" + args.get_argument());
            }
        }


    </script>

</telerik:RadScriptBlock>
<asp:Panel runat="server" ID="pnlId">
    <telerik:RadTextBox ID="txtObject" MaxLength="511" onBlur="javascript:ChangeStrWithValidCharacter(this);" Rows="3" runat="server" Width="100%" ShowButton="True" ButtonCssClass="searchButton" />
    <asp:RequiredFieldValidator ControlToValidate="txtObject" Display="Dynamic" ErrorMessage="Campo oggetto obbligatorio" ID="rfvObject" runat="server" />
</asp:Panel>
