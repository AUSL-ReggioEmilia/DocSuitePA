<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSMailSender.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSMailSender" %>

<%@ Register Src="~/MailSenders/MailSenderControl.ascx" TagPrefix="uc1" TagName="MailSenderControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <script type="text/javascript" src="../Scripts/dsw.uds.hub.js"></script>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var udsScripts = (function () {
                var currentFlatLoadingPanel = null;
                var currentLoadingPanel = null;
                var currentUpdatedControl = null;
                var currentUpdatedToolbar = null;

                function udsScripts() {
                    currentFlatLoadingPanel = null;
                    currentLoadingPanel = null;
                    currentUpdatedControl = null;
                    currentUpdatedToolbar = null;
                }

                udsScripts.prototype.showLoadingPanel = function () {
                    setTimeout(function () {
                        $find("<%= MailSenderControl.ButtonToolBar.ClientID %>").set_enabled(true);
                    }, 200);
                    currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                    currentFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                    currentUpdatedControl = "<%= MailSenderControl.MessageTablePanel.ClientID%>";
                    currentUpdatedToolbar = "<%= MailSenderControl.ButtonToolBar.ClientID %>";
                    currentLoadingPanel.show(currentUpdatedControl);
                    currentFlatLoadingPanel.show(currentUpdatedToolbar);
                };

                udsScripts.prototype.hideLoadingPanel = function () {
                    setTimeout(function () {
                        $find("<%= MailSenderControl.ButtonToolBar.ClientID %>").set_enabled(true);
                    }, 200);
                    if (currentLoadingPanel != null) {
                        currentLoadingPanel.hide(currentUpdatedControl);
                    }
                    if (currentFlatLoadingPanel != null) {
                        currentFlatLoadingPanel.hide(currentUpdatedToolbar);
                    }
                    currentUpdatedControl = null;
                    currentUpdatedToolbar = null;
                    currentLoadingPanel = null;
                    currentFlatLoadingPanel = null;
                };

                return udsScripts;
            })();
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var dswSignalR;
            var tentativeCount = 0;
            var udsScripts = new udsScripts();

            function confirmUds(sender, args) {
                var command = args.get_item().get_value();
                if (command == 'MailSender_Send') {
                    args.set_cancel(true);
                    var validated = Page_ClientValidate();
                    if (validated) {
                        var udsHub = new DSWUDSHub("<%= SignalRServerAddress %>",
                            $find("<%= udsNotification.ClientID %>"),
                            $find("<%= responseNotificationError.ClientID %>"),
                            $find("<%= AjaxManager.ClientID %>"),
                            $find("<%= MailSenderControl.ButtonToolBar.ClientID %>"),
                            $get("<%= HFcorrelatedCommandId.ClientID %>"),
                            "<%= HFSubmit.UniqueID %>", udsScripts
                        );
                        udsHub.start("Update", onSuccessCallback, onErrorCallback);
                    }
                }
            }

            function submitSending() {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("MailSender_Send_External");
            }

            function onSuccessCallback(model) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("callback|" + model.UniqueId + "|" + model.UDSRepository.Id);
            }

            function onErrorCallback() {
                $find("<%= MailSenderControl.ButtonToolBar.ClientID %>").set_enabled(true);
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

            function OpenWindowMailSettori(url) {
                setTimeout(function () {
                    var manager = $find("<%=RadWindowManagerUDS.ClientID %>");
                    var wnd = manager.open(url, "windowMailSettori");
                    wnd.add_close(CloseMailSettori);
                    wnd.center();
                    return true;
                }, 0);
            }

            function CloseMailSettori(sender, args) {
                sender.remove_close(CloseMailSettori);
                if (args.get_argument() !== null) {
                    var manager = $find("<%= AjaxManager.ClientID %>");
                    manager.ajaxRequest("Send_Mail|" + args.get_argument());
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerUDS" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="windowMailSettori" runat="server" Title="Selezione Destinatari" Width="500" />
        </Windows>
    </telerik:RadWindowManager>
    <asp:HiddenField ID="HFcorrelatedCommandId" runat="server" Value="" />
    <asp:Panel runat="server" ID="mainPanel">
        <uc1:MailSenderControl runat="server" ID="MailSenderControl"  />
        <telerik:RadButton runat="server" ID="HFSubmit" AutoPostBack="false"></telerik:RadButton>
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

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
