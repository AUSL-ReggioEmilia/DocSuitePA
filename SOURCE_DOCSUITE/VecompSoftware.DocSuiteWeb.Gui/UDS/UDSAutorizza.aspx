<%@ Page AutoEventWireup="false" CodeBehind="UDSAutorizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSAutorizza" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Archivio - Autorizzazioni" %>

<%@ Register Src="~/UDS/UserControl/uscUDS.ascx" TagName="uscUDS" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <script type="text/javascript" src="../Scripts/dsw.uds.hub.js"></script>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var dswSignalR;
            var tentativeCount = 0;
            var udsScripts;

            function confirmUds(sender, args) {
                var validated = Page_ClientValidate();
                if (validated) {
                    udsScripts = new UscUDSScripts()
                    var udsHub = new DSWUDSHub("<%= SignalRServerAddress %>",
                        $find("<%= udsNotification.ClientID %>"),
                        $find("<%= responseNotificationError.ClientID %>"),
                        $find("<%= AjaxManager.ClientID %>"),
                        $find("<%= btnConferma.ClientID %>"),
                        $get("<%= HFcorrelatedCommandId.ClientID %>"),
                        "<%= btnConferma.UniqueID %>", udsScripts
                    );
                    udsHub.start("Update", onSuccessCallback, onErrorCallback);
                }
            }

            function onSuccessCallback(model) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("callback|" + model.UniqueId + "|" + model.UDSRepository.Id);
            }

            function onErrorCallback() {
                $find("<%= btnConferma.ClientID %>").set_enabled(true);
            }

            function onError(message) {
                var notification = $find("<%=udsNotification.ClientID %>");
                notification.hide();
                var responseNotificationError = $find("<%=responseNotificationError.ClientID %>");
                responseNotificationError.set_updateInterval(0);
                udsScripts.hideLoadingPanel();
                responseNotificationError.show();
                responseNotificationError.set_text(message);
                onErrorCallback();
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:HiddenField id="HFcorrelatedCommandId" runat="server" Value="" />
    <asp:Panel runat="server" ID="MainPanel">
        <%--Dati--%>
        <uc1:uscUDS runat="server" ID="uscUDS" />

        <table id="tblDati" cellspacing="1" cellpadding="1" width="100%" border="0">
            <tr>
                <td>
                    <uc1:uscSettori Caption="Autorizzazioni" ID="uscAutorizza" MultiSelect="true" MultipleRoles="true" Required="false" runat="server" Type="UDS" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <telerik:RadNotification ID="udsNotification" runat="server"
        Width="400" Height="150" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true"
        Title="Notifica Archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
        TitleIcon="none" Style="z-index: 100000;" />

    <telerik:RadNotification ID="responseNotificationError" runat="server"
        Width="400" Height="150" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true" 
        Title="Anomalia in archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
        TitleIcon="delete" Style="z-index: 100000;" />
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton runat="server" ID="btnConferma" Width="150" Text="Conferma modifica" AutoPostBack="false" OnClientClicked="confirmUds" CausesValidation="false"></telerik:RadButton>
</asp:Content>
