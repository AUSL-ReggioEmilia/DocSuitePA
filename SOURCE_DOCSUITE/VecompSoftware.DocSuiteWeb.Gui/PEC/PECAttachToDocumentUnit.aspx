<%@ Page AutoEventWireup="false" CodeBehind="PECAttachToDocumentUnit.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECAttachToDocumentUnit" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagName="ProtocolPreview" TagPrefix="usc" %>
<%@ Register Src="~/PEC/uscPECInfo.ascx" TagPrefix="usc" TagName="uscPECInfo" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadWindow ID="modalPopup" runat="server" Width="360px" Height="100px" Modal="true"
        Title="Inserire password" Behaviors="Close">
        <ContentTemplate>
            <div class="cfgContent qsfClear">
                <label for="password">Inserire la password</label>
                <input type="password" id="password" name="password" onkeydown="return (event.keyCode!=13);" />
                <input type="button" value="Conferma" onclick="showDialogInitiallyClose()" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
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
                        $find("<%= btnAdd.ClientID %>").set_enabled(false);
                    }, 200);

                    currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                    currentFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                    currentUpdatedControl = "<%= ContentPanel.ClientID%>";
                    currentUpdatedToolbar = "<%= btnAdd.ClientID %>";
                    currentLoadingPanel.show(currentUpdatedControl);
                    currentFlatLoadingPanel.show(currentUpdatedToolbar);
                };

                udsScripts.prototype.hideLoadingPanel = function () {
                    setTimeout(function () {
                        $find("<%= btnAdd.ClientID %>").set_enabled(true);
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
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script type="text/javascript">
            function OpenSearchWindow() {
                var rblSelectedEnvironment = $('[id$="<%= rblDocumentUnit.ClientID %>"]').find(":checked").val();
                if (rblSelectedEnvironment == "UDS") {
                    OpenUDSSearchWindow();
                } else {
                    OpenProtSearchWindow();
                }
            }

            function OpenProtSearchWindow() {
                var wnd = window.radopen("../Prot/ProtRicerca.aspx?Titolo=Selezione Protocollo&Action=Fasc", "wndSearch");
                wnd.setSize(700, 550);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.add_close(top.OnClientClose);
                wnd.center();
            }

            function OpenUDSSearchWindow() {
                var wnd = window.radopen("../UDS/UDSSearch.aspx?Type=UDS&CopyToPEC=true", "wndSearch");
                wnd.setSize(700, 550);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.add_close(top.OnClientClose);
                wnd.center();
            }

            top.OnClientClose = function (sender, args) {
                if (args.get_argument() !== null) {
                    var splitted = args.get_argument().split("|");
                    document.getElementById('<%= txtYear.ClientID%>').value = splitted[0];
                    document.getElementById('<%= txtNumber.ClientID%>').value = splitted[1];
                    if (splitted.length > 2) {
                        var cmbArchives = $find('<%= ddlAvailableUDS.ClientID %>');
                        var itm = cmbArchives.findItemByValue(splitted[2]);
                        itm.select();
                    }
                    document.getElementById('<%= btnSeleziona.ClientId %>').click();
                }
            };

            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                var oWindow = GetRadWindow();
                oWindow.argument = "../Prot/ProtVisualizza.aspx?Year=" + $get('<%=txtYear.ClientId %>').value + "&Number=" + $get('<%=txtNumber.ClientId %>').value;
                oWindow.close();
            }

            function GoBack(url) {
                window.location = url;
            }

            function CloseMe() {
                var oWindow = GetRadWindow();
                oWindow.close();
            }

            function RemoveConfirm() {
                $get('<%= btnAdd.ClientId %>').setAttribute("onclick", "");
            }

            function DisableRowMouseOver(sender, args) {
                var item = args.get_gridDataItem(),
                    toolTip = $find("<%= disableTooltipRow.ClientID %>");
                if (item._selectable == false) {
                    toolTip.set_targetControl(item.get_element());
                    setTimeout(function () {
                        toolTip.show();
                    }, 11);
                } else {
                    toolTip.hide();
                }
                }

                var objtxt;
                var objbtn;
                function showDialogInitially(txt, btn) {
                    objtxt = document.getElementById(txt);
                    objtxt.value = "";
                    objbtn = btn;
                    var wnd = $find("<%=modalPopup.ClientID %>");
                    wnd.show();
                }

                function showDialogInitiallyClose() {
                    $find('<%=modalPopup.ClientID %>').close();
                    objtxt.value = document.getElementById("password").value;
                    var o = document.getElementById(objbtn);
                    if (o.onclick) {
                        o.onclick();
                    }
                    else if (o.click) {
                        o.click();
                    }
                }

                function ShowLoadingPanel() {
                    var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                    var currentUpdatedControl = "<%= DocumentListGrid.ClientID%>";
                    currentLoadingPanel.show(currentUpdatedControl);
                }

                function HideLoadingPanel() {
                    var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= DocumentListGrid.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function responseEnd(sender, eventArgs) {
                HideLoadingPanel();
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
                        $find("<%= btnAdd.ClientID %>"),
                        $get("<%= HFcorrelatedCommandId.ClientID %>"),
                        "<%= btnAdd.UniqueID %>", udsScripts
                    );
                    udsHub.start("Update", onSuccessCallback, onErrorCallback);
                }
            }

            function onSuccessCallback(model) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("udscallback|" + model.UniqueId + "|" + model.UDSRepository.Id);
            }

            function onErrorCallback() {
                $find("<%= btnAdd.ClientID %>").set_enabled(true);
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
    <asp:Panel runat="server" ID="ContentPanel">

        <table class="datatable">
            <tr class="Chiaro">
                <td>
                    <usc:uscPECInfo runat="server" ID="uscPECInfo"></usc:uscPECInfo>

                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                        <tr>
                            <td style="width: 150px; vertical-align: top; font-size: 8pt; text-align: left;">
                                <b>Elenco dei documenti selezionati:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt;">

                                <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" AllowMultiRowSelection="true">
                                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="Serialized">
                                        <Columns>

                                            <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                                            <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
                                                <ItemTemplate>
                                                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# ImagePath.FromFile(Eval("Name").ToString())%>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                            <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblFileName" Text='<%# Eval("Name")%>'></asp:Label>
                                                    <asp:ImageButton ID="imgDecrypt" runat="server" ImageUrl='<%# ImagePath.SmallBoxOpen %>' Visible="false" CausesValidation="false" ToolTip="Archivio protetto da password. Premi e inserisci la password dell'archivio" />
                                                    <asp:HiddenField runat="server" ID="txtPassword" />
                                                    <asp:Button runat="server" ID="btnUnzip" Text="Elabora contenuto" OnClientClick="ShowLoadingPanel();" Style="display: none" Visible="false" CommandName="unzip" CausesValidation="false" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                    <ClientSettings EnableRowHoverStyle="False">
                                        <ClientEvents OnRowMouseOver="DisableRowMouseOver"></ClientEvents>
                                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                                    </ClientSettings>
                                </telerik:RadGrid>
                                <telerik:RadToolTip ID="disableTooltipRow" runat="server" ShowEvent="FromCode" AutoCloseDelay="0">
                                    Documento non selezionabile per la protocollazione
                                </telerik:RadToolTip>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <table class="datatable" id="tbDocumentUnitSelect" runat="server" visible="false">
            <tr>
                <th colspan="2">Selezione tipologia a cui allegare i documenti selezionati
                </th>
            </tr>
            <tr>
                <td>
                    <asp:RadioButtonList ID="rblDocumentUnit" AutoPostBack="true" runat="server">
                    </asp:RadioButtonList>
                </td>
                <td id="pnlUDSSelected" runat="server" visible="false">
                    <b>Archivio: </b>
                    <telerik:RadComboBox ID="ddlAvailableUDS" AllowCustomText="false" EmptyMessage="Seleziona un archivio" Width="300px" Height="200px"
                        CausesValidation="false" EnableLoadOnDemand="False" Filter="Contains" DataSourceID="RepositoriesSource" DataTextField="Text" DataValueField="Value" AutoPostBack="false" runat="server" />
                    <asp:ObjectDataSource ID="RepositoriesSource" runat="server" TypeName="VecompSoftware.DocSuiteWeb.Gui.PECAttachToDocumentUnit"
                        SelectMethod="GetRepositories" />
                </td>
            </tr>
        </table>

        <table class="datatable" id="tblFindDocumentUnit" runat="server">

            <tr>
                <th colspan="3">
                    <asp:Label runat="server" ID="lbSelectedDocumentUnit"></asp:Label>
                </th>
            </tr>

            <tr>
                <td>
                    <b>Anno: </b>
                    <asp:TextBox ID="txtYear" runat="server" Width="60px" MaxLength="4" />
                </td>
                <td>
                    <b>Numero: </b>
                    <asp:TextBox ID="txtNumber" runat="server" Width="110px" MaxLength="7" />
                </td>
                <td>
                    <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />
                    <asp:Button ID="btnCerca" runat="server" Text="Cerca" UseSubmitBehavior="false" OnClientClick="OpenSearchWindow();return false;" />
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:RegularExpressionValidator ValidationGroup="Attach" ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="yearValidator" runat="server" ValidationExpression="\d{4}" />
                    <asp:RequiredFieldValidator ValidationGroup="Attach" ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="requiredYearValidator" runat="server" />
                </td>
                <td style="text-align: center">
                    <asp:RegularExpressionValidator ValidationGroup="Attach" ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Errore formato" ID="numberValidator" runat="server" ValidationExpression="\d*" />
                    <asp:RequiredFieldValidator ValidationGroup="Attach" ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="requiredNumberValidator" runat="server" />
                </td>
                <td>&nbsp;
                </td>
            </tr>
        </table>
        <usc:ProtocolPreview ID="uscProtocolPreview" runat="server" Visible="false" />

        <table class="datatable" id="udsPanel" runat="server" visible="false">
            <tr>
                <th colspan="2">
                    <label class="label">Archivio Selezionato</label></th>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Archivio</td>
                <td style="width: 85%;">
                    <telerik:RadButton ButtonType="LinkButton" ID="cmdUDS" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Contenitore</td>
                <td>
                    <asp:Label ID="lblContainer" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Oggetto</td>
                <td>
                    <asp:Label ID="lblObject" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Classificazione</td>
                <td>
                    <asp:Label ID="lblCategoryCode" runat="server" /><br />
                    <asp:Label ID="lblCategoryDescription" runat="server" />
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
    <telerik:RadButton Enabled="False" ID="btnAdd" runat="server" Text="Allega" ValidationGroup="Attach" Width="150px" />
    <telerik:RadButton runat="server" ID="cmdCancel" Text="Annulla" Width="150" />
</asp:Content>
