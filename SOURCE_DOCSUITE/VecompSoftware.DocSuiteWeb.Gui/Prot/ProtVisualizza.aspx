<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtVisualizza" CodeBehind="ProtVisualizza.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Visualizzazione" %>

<%@ Register Src="~/UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#imgWait").hide();
        });

        function runModule(jeepUrl, sModuleName) {
            $.ajaxSetup({
                beforeSend: function () {
                    $("#btnLaunchAsyn").attr("disabled", "disabled");
                    $("#imgWait").show();
                },
                complete: function () {
                    $("#btnLaunchAsyn").attr("disabled", null);
                    $("#imgWait").hide();
                }
            });
            $.ajax({
                type: "POST",
                url: jeepUrl,
                data: '{"nameMod":"' + sModuleName + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == false)
                        alert("PECMailService non eseguito");
                    else
                        __doPostBack('ctl00$cphFooter$btnRefresh', '');
                },
                error: function (a, b, c) {
                    alert("Operazione di aggiornamento già eseguita");
                }
            });
        }

        function onFlushAnnexedDocument() {
            return confirm("Sei sicuro di voler eliminare gli annessi?");
        }
    </script>
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript" language="javascript">
            Number.isInteger = Number.isInteger || function (value) {
                return typeof value === "number" &&
                    isFinite(value) &&
                    Math.floor(value) === value;
            };

            function openRejectWindow() {
                var oWnd = $find("<%=RejectMotivation.ClientID%>");
                oWnd.show();
                return false;
            }

            function OpenWindow(id) {
                var wnd = $find(id);
                wnd.show();
                return false;
            }

            function OpenHighlightWindow() {
                var url = '../Prot/ProtHighlightUser.aspx?UniqueId=' + '<% =CurrentProtocol.Id %>'
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.SMALL);

                wnd.add_close(UpdateProtocolHighlight);
                return false;
            }

            function UpdateProtocolHighlight(sender, args) {
                var commandName = args.get_argument();

                if (commandName) {
                    ExecuteAjaxRequest(commandName);
                }
            }

            function OpenWindowDuplica() {
                var manager = $find("<%=RadWindowManagerProt.ClientID %>");
                var wnd = manager.open('../Prot/ProtDuplica.aspx?Titolo=Duplicazione Protocollo', 'windowDuplica');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.add_close(CloseDuplica);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function CloseDuplica(sender, args) {
                sender.remove_close(CloseDuplica);
                if (args.get_argument() !== null) {
                    document.getElementById("<%=txtCheckSel.ClientID %>").value = args.get_argument();
                    document.getElementById("<%=btnCallbackDuplica.ClientID %>").click();
                }
            }

            function <%= Me.ID %>_OpenWindow(url, name, width, height, parameters) {
                var completeUrl = url;
                completeUrl += ("?" + parameters);

                var manager = $find("<%= RadWindowManagerProt.ClientID %>");
                var wnd = manager.open(completeUrl, name);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.add_close(CloseRefreshFolderFunction);
                wnd.setSize(width, height);
                wnd.center();

                return false;
            }

            function CloseRefreshFolderFunction(sender, args) {
                sender.remove_close(CloseRefreshFolderFunction);
                if (args.get_argument() !== null) {
                    document.getElementById("<%=SelPratica.ClientID %>").value = args.get_argument();
                    document.getElementById("<%=btnSelPratica.ClientID %>").click();
                }
            }

            function OpenPecMailAddressWindow(sessionSeed, message) {
                var wnd = window.radopen("../Prot/ProtPecEmailAddress.aspx?SessionSeed=" + sessionSeed + "&Message=" + message + "&Type=Prot", "windowViewMail");
                wnd.setSize(700, 500);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal = true;
                wnd.center();
            }

            function RefreshOnPECAddressesInserted() {
                ExecuteAjaxRequest("SendNewPec");
            }

            function ExecuteAjaxRequest(operationName) {
                $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>").ajaxRequest(operationName);
            }

            function ShowLoadingPanel(timeout) {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);
                if (timeout !== undefined && timeout !== "" && Number.isInteger(timeout)) {
                    setTimeout(function () {
                        var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                        var pnlButtons = "<%= pnlButtons.ClientID%>";
                        var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                        currentLoadingPanel.hide(currentUpdatedControl);
                        ajaxFlatLoadingPanel.hide(pnlButtons);
                    }, timeout);
                }
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.hide(pnlButtons);
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var protVisualizza;
            require(["Prot/ProtVisualizza"], function (ProtVisualizza) {
                $(function () {
                    protVisualizza = new ProtVisualizza(tenantModelConfiguration.serviceConfiguration);
                    protVisualizza.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientId %>";
                    protVisualizza.radWindowManagerId = ("<%=RadWindowManagerProt.ClientID %>");
                    protVisualizza.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                    protVisualizza.btnWorkflowId = "<%= btnWorkflow.ClientID %>";
                    protVisualizza.radNotificationInfoId = "<%= radNotificationInfo.ClientID %>";
                    protVisualizza.currentDocumentUnitId = "<%=CurrentProtocol.Id%>";
                    protVisualizza.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadNotification ID="radNotificationInfo" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="300" Height="150" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni protocollo" TitleIcon="none" AutoCloseDelay="4000" Position="Center" />

</asp:Content>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadWindowManager BackColor="Gray" Behaviors="Close" DestroyOnClose="true" EnableViewState="False" ID="RadWindowManagerProt" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="windowDuplica" OnClientClose="CloseDuplica" runat="server" Title="Protocollo - Duplica" Width="500" />
            <telerik:RadWindow Height="300" ID="windowDocmSceltaPratica" OnClientClose="CloseRefreshFolderFunction" runat="server" Title="Pratiche - Seleziona" Width="500" />
            <telerik:RadWindow Height="100" ID="windowPrintLabel" runat="server" Title="Protocollo - Stampa Etichetta" Width="300" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadWindow runat="server" ID="RejectMotivation" Title="Motivazione rigetto" Width="480" Height="140" Behaviors="Close">
        <ContentTemplate>
            <asp:UpdatePanel ID="messagePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="warningAreaLow">
                        <p>È necessario fornire una motivazione per il rigetto del protocollo.</p>
                    </div>
                    <div>
                        <telerik:RadTextBox ID="txtRejectMotivation" Rows="3" runat="server" TextMode="MultiLine" ValidationGroup="Cancel" Width="100%" />
                    </div>
                    <div style="text-align: right">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="Cancel" ControlToValidate="txtRejectMotivation" ErrorMessage="Campo motivazione obbligatorio" Display="Dynamic" />
                        <asp:Button ID="cmdRejectOk" PostBackUrl="~/InformationPage.aspx" runat="server" Text="Conferma" ValidationGroup="Cancel" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="wndInsertCollaboration" Title="Selezionare la tipologia di inserimento in Collaborazione" Width="480" Height="100" Behaviors="Close">
        <ContentTemplate>
            <asp:UpdatePanel ID="updInsertCollaboration" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div>
                        <asp:RadioButtonList ID="rblInsertCollaboration" runat="server" Font-Names="Verdana" />
                    </div>
                    <div style="text-align: right">
                        <asp:Button ID="btnInsertCollaborationConfirm" runat="server" Text="Conferma" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>


    <asp:Panel runat="server" ID="pnlMainContent" Style="height: 100%">
        <table width="100%" height="100%">
            <tr>
                <td width="100%" style="vertical-align: top;">
                    <uc:uscProtocollo ID="uscProtocollo" ViewUDSSource="true" runat="server" />
                </td>
                <td class="center" width="3%" height="100%">
                    <asp:Table BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" CellSpacing="0" Height="100%" ID="tblIcons" runat="server">
                        <asp:TableRow Height="100%">
                            <asp:TableCell Height="100%" VerticalAlign="Top" />
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ID="cn2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <table runat="server" id="tblButton">
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnDocument" OnClientClick="ShowLoadingPanel();" Width="150" Text="Documenti" Enabled="false" />
                    <asp:Button ID="btnAutorizza" runat="server" OnClientClick="ShowLoadingPanel();" Width="150" Text="Autorizza" />
                    <asp:Button ID="btnAutoAssign" runat="server" OnClientClick="ShowLoadingPanel(700);" Width="150" Text="Auto assegna" Visible="false" />
                    <asp:Button ID="btnMail" runat="server" OnClientClick="ShowLoadingPanel();" Width="150" Text="Mail" />
                    <asp:Button ID="btnMailSettori" runat="server" OnClientClick="ShowLoadingPanel();" Width="150" Text="Invia settori" />
                    <asp:Button ID="btnSendToUsers" runat="server" OnClientClick="ShowLoadingPanel();" Width="150" Text="Invia persona" />
                    <asp:Button ID="btnLink" runat="server" OnClientClick="ShowLoadingPanel();" Width="150" Text="Collegamenti" />
                    <asp:Button ID="btnStampa" runat="server" OnClientClick="ShowLoadingPanel(500);" Width="150" Text="Stampa" />
                    <asp:Button ID="btnDuplica" runat="server" Width="150" Text="Duplica" CausesValidation="false" OnClientClick="return OpenWindowDuplica();" />
                    <asp:Button ID="btnCycle" runat="server" Width="150" OnClientClick="ShowLoadingPanel();" Text="Successivo" Visible="false" />
                    <asp:Button ID="btnPrintDocumentLabel" runat="server" OnClientClick="ShowLoadingPanel();" Width="150px" Text="Etichetta documento" />
                    <asp:Button ID="btnPrintAttachmentLabel" runat="server" OnClientClick="ShowLoadingPanel();" Width="150px" Text="Etichetta allegato" />
                    <asp:Button ID="btnHighlight" runat="server" OnClientClick="return OpenHighlightWindow();" Text="Evidenzia" Width="150px" />
                    <asp:Button ID="btnRemoveHighlight" runat="server" OnClientClick="ShowLoadingPanel();" Text="Rimuovi evidenza" Width="150px" />
                    <asp:Button ID="btnWorkflow" runat="server" Width="150" Text="Avvia attività" CausesValidation="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnPratica" runat="server" Text="Pratica" OnClientClick="ShowLoadingPanel();" Visible="false" Width="150" />
                    <asp:Button ID="btnAddToPratica" runat="server" Text="Aggiungi a pratica" OnClientClick="ShowLoadingPanel();" Visible="false" Width="150" />
                    <asp:Button ID="btnFascicle" runat="server" Text="Fascicolo" OnClientClick="ShowLoadingPanel();" Width="150" />
                    <asp:Button ID="btnToSeries" runat="server" Text="Amministrazione aperta" Visible="False" Width="240px" />
                    <asp:Button ID="btnRiferimenti" runat="server" Text="Riferimenti" OnClientClick="ShowLoadingPanel();" Visible="false" Width="150" />
                    <asp:Button ID="btnNoteSettore" runat="server" Text="Note settore" OnClientClick="ShowLoadingPanel();" Width="150" />
                    <asp:Button ID="btnModifica" runat="server" Text="Modifica" OnClientClick="ShowLoadingPanel();" Width="150" />
                    <asp:Button ID="btnUDS" runat="server" Text="Archivia" OnClientClick="ShowLoadingPanel();" Width="150" />
                    <asp:Button ID="btnFlushAnnexed" runat="server" OnClientClick="if(!onFlushAnnexedDocument()) return false;" Text="Svuota annessi" Width="150" />
                    <asp:Button ID="btnAnnulla" runat="server" Text="Annullamento" OnClientClick="ShowLoadingPanel();" Width="150" />
                    <asp:Button ID="btnReject" OnClientClick="return openRejectWindow();" runat="server" Text="Rigetta" Visible="false" Width="150" />
                    <asp:Button ID="btnNewPecMail" runat="server" OnClientClick="ShowLoadingPanel();" Text="PEC" Visible="false" Width="150" />
                    <asp:Button ID="btnLettera" runat="server" OnClientClick="ShowLoadingPanel();" Text="Invia lettera" Visible="False" Width="150" />
                    <asp:Button ID="btnRaccomandata" runat="server" OnClientClick="ShowLoadingPanel();" Text="Invia raccomandata" Visible="False" Width="150" />
                    <asp:Button ID="btnTNotice" runat="server" OnClientClick="ShowLoadingPanel();" Text="Invia TNotice" Visible="false" Width="150" />
                    <asp:Button ID="btnTelegramma" runat="server" OnClientClick="ShowLoadingPanel();" Text="Invia telegramma" Visible="False" Width="150" />
                    <asp:Button ID="btnInterop" runat="server" OnClientClick="ShowLoadingPanel();" Text="Interoperabilità" Visible="False" Width="150" />
                    <asp:Button ID="btnLog" runat="server" OnClientClick="ShowLoadingPanel();" Text="Log" Visible="false" Width="150" />
                    <asp:Button ID="btnRolesLog" runat="server" OnClientClick="ShowLoadingPanel();" Text="Movimentazioni" Width="150" />
                    <asp:Button ID="btnProtocollo" runat="server" OnClientClick="ShowLoadingPanel();" Text="Protocollo" Visible="false" />
                    <asp:Button ID="btnHandle" runat="server" OnClientClick="ShowLoadingPanel(500);" Text="Prendi in carico" Visible="false" Width="150" />
                    <asp:Button ID="btnForzaBiblos" runat="server" Text="Forzatura Biblos" Visible="false" Width="150" />
                    <asp:Button ID="btnInsertCollaboration" runat="server" OnClientClick="ShowLoadingPanel();" Text="Prepara risposta" Width="150" Visible="false" />
                    <asp:Button ID="btnRispondiDaPEC" runat="server" OnClientClick="ShowLoadingPanel();" Text="Rispondi da PEC" Width="150" />
                    <asp:Button ID="btnReassignRejected" runat="server" OnClientClick="ShowLoadingPanel();" Text="Riassegna" Width="150" />
                    <asp:Button ID="btnCorrection" runat="server" OnClientClick="ShowLoadingPanel();" Text="Correggi" Width="150" />

                    <%--hidden fields--%>
                    <asp:TextBox CssClass="hiddenField" ID="SelPratica" runat="server" />
                    <asp:Button CssClass="hiddenField" ID="btnSelPratica" runat="server" />
                    <asp:TextBox CssClass="hiddenField" ID="txtCheckSel" runat="server" />
                    <asp:Button ID="btnCallbackDuplica" runat="server" Style="display: none;" Text="Nuovo" Width="150" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:PlaceHolder runat="server" ID="ReportButtons"></asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
