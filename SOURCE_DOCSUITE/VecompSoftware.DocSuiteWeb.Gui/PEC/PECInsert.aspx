<%@ Page AutoEventWireup="false" CodeBehind="PECInsert.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECInsert" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="PEC - Inserimento" ValidateRequest="false" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>
<%@ Register Src="~/UserControl/uscDocumentList.ascx" TagPrefix="usc" TagName="UploadDocumentList" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
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
                        $find("<%= cmdSend.ClientID %>").set_enabled(true);
                    }, 200);
                    currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                    currentFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                    currentUpdatedControl = "<%= pnlContent.ClientID%>";
                    currentUpdatedToolbar = "<%= cmdSend.ClientID %>";
                    currentLoadingPanel.show(currentUpdatedControl);
                    currentFlatLoadingPanel.show(currentUpdatedToolbar);
                };

                udsScripts.prototype.hideLoadingPanel = function () {
                    setTimeout(function () {
                        $find("<%= cmdSend.ClientID %>").set_enabled(true);
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
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            window.onbeforeunload = function () {
                location.replace('');
            }

            function GetRadWindow() {
                var retval = null;
                if (window.radWindow) {
                    retval = window.radWindow;
                } else if (window.frameElement.radWindow) {
                    retval = window.frameElement.radWindow;
                }
                return retval;
            }

            function CloseWindow() {
                var wnd = GetRadWindow();
                wnd.close();
            }

            function OnClientClose(sender, args) {
                sender.remove_close(OnClientClose);
                if (args.get_argument() !== null) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest('UpdatedPecAddress');
                }
            }

            function OpenPecMailAddressWindow(sessionSeed, message) {
                var wnd = window.radopen("CheckPecEmailAddress.aspx?SessionSeed=" + sessionSeed + "&Message=" + message + "&Type=Prot", "windowViewMail");
                wnd.setSize(900, 450);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal = true;
                wnd.add_close(OnClientClose);
                wnd.center();
            }

            function RefreshOnPECAddressesInserted() {
                ExecuteAjaxRequest("UpdatedPecAddress");
            }

            function ExecuteAjaxRequest(operationName) {
                $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>").ajaxRequest(operationName);
            }

            var dswSignalR;
            var tentativeCount = 0;
            var udsScripts = new udsScripts();

            function confirmUds(sender, args) {
                var validated = Page_ClientValidate();
                if (validated) {
                    var udsHub = new DSWUDSHub("<%= SignalRServerAddress %>",
                        $find("<%= udsNotification.ClientID %>"),
                        $find("<%= responseNotificationError.ClientID %>"),
                        $find("<%= AjaxManager.ClientID %>"),
                        $find("<%= cmdSend.ClientID %>"),
                        $get("<%= HFcorrelatedCommandId.ClientID %>"),
                        "<%= cmdSend.UniqueID %>", udsScripts
                    );
                    udsHub.start("Update", onSuccessCallback, onErrorCallback);
                }
            }

            function onSuccessCallback(model) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("udsEditCallback|" + model.UniqueId + "|" + model.UDSRepository.Id);
            }

            function onErrorCallback() {
                $find("<%= cmdSend.ClientID %>").set_enabled(true);
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

            function checkZip() {
                if ($("#<%= chkZip.ClientID %>").is(":checked")) {
                    $("#<%= chkZipPassword.ClientID %>").prop("checked", false);
                }
            }

            function checkProtectedZip() {
                if ($("#<%= chkZipPassword.ClientID %>").is(":checked")) {
                    $("#<%= chkZip.ClientID %>").prop("checked", false);
                }
            }
        </script>
    </telerik:RadCodeBlock>
    <asp:Panel runat="server" ID="WarningPanel" CssClass="hiddenField">
        <asp:Label ID="WarningLabel" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:HiddenField ID="HFcorrelatedCommandId" runat="server" Value="" />
    <telerik:RadNotification ID="udsNotification" runat="server"
        Width="400" Height="150" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true"
        Title="Notifica Archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
        TitleIcon="none" Style="z-index: 100000;" />

    <telerik:RadNotification ID="responseNotificationError" runat="server"
        Width="400" Height="150" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true"
        Title="Anomalia in archivio" OffsetX="-20" OffsetY="-20" AutoCloseDelay="0"
        TitleIcon="delete" Style="z-index: 100000;" />

    <asp:Panel runat="server" ID="pnlContent">
        <%--opzioni di invio--%>
        <table class="datatable" id="pnlOptions">
            <tr>
                <th>Opzioni di invio</th>
            </tr>
            <tr>
                <td>
                    <ul>
                        <li>
                            <asp:CheckBox Checked="false" Enabled="true" ID="chkMultiPec" runat="server" Text="Crea una PEC per ogni destinatario" />
                        </li>
                        <li>
                            <asp:CheckBox AutoPostBack="true" Checked="false" Enabled="false" ID="chkSetPecInteroperable" runat="server" Text="Rendi la PEC interoperabile" />
                        </li>
                        <li>
                            <asp:CheckBox Checked="false" Enabled="true" ID="chkZip" runat="server" Text="Invia gli allegati come file zip" onclick="checkZip();" />
                        </li>
                        <li>
                            <asp:CheckBox Checked="false" Enabled="true" ID="chkZipPassword" runat="server" Text="Invia gli allegati come file zip protetto da password" onclick="checkProtectedZip();" />
                        </li>
                        <li>
                            <asp:CheckBox Checked="false" ID="chkAddOriginalAttachments" runat="server" Text="Includi allegati mail originale" AutoPostBack="true" />
                        </li>
                    </ul>
                </td>
            </tr>
        </table>

        <%--mittente--%>
        <table class="datatable">
            <tr>
                <th>Mittente</th>
            </tr>
            <tr>
                <td class="DXChiaro">
                    <asp:DropDownList ID="ddlMailFrom" runat="server" />
                </td>
            </tr>
        </table>
        <%-- destinatari --%>
        <table class="datatable">
            <tr>
                <th>Destinatari</th>
            </tr>
            <tr>
                <td>
                    <usc:SelContatti ButtonDeleteVisible="true" ButtonImportVisible="False" ButtonManualVisible="true" ButtonSelectDomainVisible="false"
                        ButtonSelectOChartVisible="false" ButtonImportManualVisible="true" ButtonSelectVisible="True" Caption="Destinatari"
                        EnableCC="false" HeaderVisible="false" ID="uscDestinatari" IsRequired="true" Multiple="true"
                        MultiSelect="true" ProtType="True" ReadOnlyProperties="true" RequiredErrorMessage="Destinatario obbligatorio"
                        runat="server" TreeViewCaption="Destinatari" Type="Prot" ButtonManualMultiVisible="False" />
                </td>
            </tr>
        </table>
        <%-- destinatari cc --%>
        <table class="datatable" id="tblCc" runat="server">
            <tr>
                <th>Destinatari Copia Conoscenza</th>
            </tr>
            <tr>
                <td>
                    <usc:SelContatti ButtonImportManualVisible="true" ButtonSelectVisible="True" Caption="Destinatari Copia Conoscenza"
                        EnableCC="false" HeaderVisible="false" ID="uscDestinatariCc" IsRequired="false" Multiple="true" MultiSelect="true" ProtType="True" ReadOnlyProperties="true"
                        runat="server" TreeViewCaption="Destinatari Copia Conoscenza" Type="Prot" ButtonManualMultiVisible="True" />
                </td>
            </tr>
        </table>
        <%-- priorità --%>
        <table class="datatable">
            <tr>
                <th>Priorità</th>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlPriority" runat="server" Width="140px">
                        <asp:ListItem Value="-1">Bassa</asp:ListItem>
                        <asp:ListItem Value="0" Selected="True">Normale</asp:ListItem>
                        <asp:ListItem Value="1">Alta</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <%-- documenti del protocollo/pec di origine --%>


        <usc:UploadDocumentList runat="server" ID="uscAttachmentList" ShowDocumentsSize="True" Visible="False" />

        <%-- allegati --%>
        <table class="datatable">
            <tr>
                <th>Allegati</th>
            </tr>
            <tr>
                <td>
                    <usc:UploadDocument TreeViewCaption="Documenti" HeaderVisible="false" ID="uscAttachment" IsDocumentRequired="false" MultipleDocuments="true" runat="server" ShowDocumentsSize="true" />
                </td>
            </tr>
        </table>
        <%-- oggetto --%>
        <table class="datatable">
            <tr>
                <th>Oggetto</th>
            </tr>
            <tr>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtMailSubject" Width="100%" InputType="Text" TextMode="MultiLine" Rows="3" />
                </td>
            </tr>
        </table>
        <%-- messaggio --%>
        <table class="datatable">
            <tr>
                <th>Messaggio</th>
            </tr>
            <tr>
                <td style="height: 100%">
                    <telerik:RadEditor runat="server" ID="txtMailBody" EditModes="Design" EnableResize="true" AutoResizeHeight="true" Width="100%" Height="100%" EmptyMessage="Inserire qui il testo">
                        <Tools>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="Bold" />
                                <telerik:EditorTool Name="Italic" />
                                <telerik:EditorTool Name="Underline" />
                                <telerik:EditorTool Name="Cut" />
                                <telerik:EditorTool Name="Copy" />
                                <telerik:EditorTool Name="Paste" />
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="JustifyLeft" />
                                <telerik:EditorTool Name="JustifyCenter" />
                                <telerik:EditorTool Name="JustifyRight" />
                                <telerik:EditorTool Name="JustifyNone" />
                            </telerik:EditorToolGroup>
                        </Tools>
                    </telerik:RadEditor>
                </td>
            </tr>
        </table>
    </asp:Panel>

</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server">
        <telerik:RadButton ID="cmdSend" runat="server" Text="Invia" Enabled="false" Width="120px" SingleClick="true" />
        <telerik:RadButton ID="cmdInsertSign" UseSubmitBehavior="false" runat="server" Text="Inserisci Firma" Visible="false" Width="120px" ValidationGroup="dsw_alone" />
    </asp:Panel>
</asp:Content>
