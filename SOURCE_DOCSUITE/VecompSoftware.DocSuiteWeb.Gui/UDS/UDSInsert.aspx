<%@ Page Title="UDS Inserimento" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSInsert.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSInsert" %>

<%@ Register Src="UserControl/uscUDS.ascx" TagPrefix="usc" TagName="UDS" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
     <script type="text/javascript" src="../Scripts/dsw.uds.hub.js"></script>
    
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var udsScripts = null;

            function initialize() {
                if (!udsScripts) {
                    udsScripts = new UscUDSScripts();
                }
            }

            function confirmUds(sender, args) {
                var validated = Page_ClientValidate('');
                if (validated) {
                    initialize();
                    var udsHub = new DSWUDSHub("<%= SignalRServerAddress %>",
                        $find("<%= udsNotification.ClientID %>"),
                        $find("<%= responseNotificationError.ClientID %>"),
                        $find("<%= AjaxManager.ClientID %>"),
                        $find("<%= btnSave.ClientID %>"),
                        $get("<%= HFcorrelatedCommandId.ClientID %>"),
                        "<%= btnSave.UniqueID %>", udsScripts
                    );
                    udsHub.start("<%= If(Action = "Insert" OrElse Action = "Duplicate", "Insert", If(Action = "Edit", "Update", Nothing)) %>", onSuccessCallback, onErrorCallback);
                }
            }

            function onSuccessCallback(model) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("udsInsertCallback|" + model.UniqueId + "|" + model.UDSRepository.Id);
            }

            function onErrorCallback() {
                $find("<%= btnSave.ClientID %>").set_enabled(true);
            }

            function onError(message) {
                initialize();
                var notification = $find("<%=udsNotification.ClientID %>");
                notification.hide();
                var responseNotificationError = $find("<%=responseNotificationError.ClientID %>");
                responseNotificationError.set_updateInterval(0);
                udsScripts.hideLoadingPanel();
                responseNotificationError.show();
                responseNotificationError.set_text(message);
                onErrorCallback();
            }

            function RemovePostbackSessionState() {
                if (sessionStorage.getItem("FieldListPostbackState")) {
                    sessionStorage.removeItem("FieldListPostbackState");
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:HiddenField id="HFcorrelatedCommandId" runat="server" Value="" />

    <telerik:RadNotification ID="udsNotification" runat="server"  
        Width="400" Height="150" Animation="Slide" EnableRoundedCorners="true" EnableShadow="true"
        Title="Notifica Archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
        TitleIcon="ok" Style="z-index: 100000;" />

    <telerik:RadNotification ID="responseNotificationError" runat="server"
        Width="400" Height="170" Animation="Slide" EnableRoundedCorners="true" EnableShadow="true"
        Title="Anomalia in archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
        TitleIcon="delete" Style="z-index: 100000;" />

    <telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
        <Rows>
            <telerik:LayoutRow RowType="Container" HtmlTag="None" CssClass="col-dsw-10">
                <Columns>
                    <telerik:CompositeLayoutColumn HtmlTag="None">
                        <Content>
                            <usc:UDS runat="server" ID="uscUDS" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlActionButtons">
        <telerik:RadButton runat="server" ID="btnSave" Width="150" OnClientClicked="confirmUds" Text="Conferma inserimento" AutoPostBack="false" />
    </asp:Panel>
</asp:Content>
