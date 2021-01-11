<%@ Page Language="vb" AutoEventWireup="False" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECToDocumentUnit.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECToDocumentUnit" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>

<%@ Register Src="~/PEC/uscPECInfo.ascx" TagPrefix="usc" TagName="uscPECInfo" %>
<%@ Register Src="~/PEC/uscInteropInfo.ascx" TagPrefix="usc" TagName="uscInteropInfo" %>
<%@ Register Src="~/UserControl/uscFascicleSearch.ascx" TagName="uscFascicleSearch" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadWindow runat="server" ID="renameAttachmentsWindow" ReloadOnShow="True" Width="470px" Height="300px" Modal="True">
    </telerik:RadWindow>
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
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script type="text/javascript">
            var pecToDocumentUnit;

            function Initialize() {
                pecToDocumentUnit.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                pecToDocumentUnit.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                pecToDocumentUnit.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                pecToDocumentUnit.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                pecToDocumentUnit.pageContentId = "<%= panelPEC.ClientID %>";
                pecToDocumentUnit.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                pecToDocumentUnit.pnlTemplateProtocolId = "<%=pnlTemplateProtocol.ClientID%>";
                pecToDocumentUnit.pnlUDSSelectId = "<%=pnlUDSSelect.ClientID%>";
                pecToDocumentUnit.pnlFascicleSelectId = "<%=pnlFascicleSelect.ClientID%>";
                pecToDocumentUnit.rblDocumentUnitId = "<%=rblDocumentUnit.ClientID%>";
                pecToDocumentUnit.cmdInitId = "<%= cmdInit.ClientID %>";
                pecToDocumentUnit.cmdInitAndCloneId = "<%= cmdInitAndClone.ClientID %>";
                pecToDocumentUnit.isPecClone = <%=ProtocolEnv.PECClone.ToString().ToLower() %>;
                pecToDocumentUnit.templateProtocolEnabled = <%=ProtocolEnv.TemplateProtocolEnable.ToString().ToLower() %>;
                pecToDocumentUnit.uscFascicleSearchId = "<%= uscFascicleSearch.PageControl.ClientID %>";
                pecToDocumentUnit.documentListGridId = "<%= DocumentListGrid.ClientID %>";
                pecToDocumentUnit.pnlButtonsId = "<%= pnlButtons.ClientID %>";
            }

            require(["PEC/PECToDocumentUnit"], function (PECToDocumentUnit) {
                pecToDocumentUnit = new PECToDocumentUnit(tenantModelConfiguration.serviceConfiguration);
                Initialize();
                pecToDocumentUnit.initialize();
                setResizeSensors();
            });



            function insertWorkflowActivity(fascicleModel, workflowActivityModel, url) {
                require(["PEC/PECToDocumentUnit"], function (PECToDocumentUnit) {
                    pecToDocumentUnit = new PECToDocumentUnit(tenantModelConfiguration.serviceConfiguration);
                    pecToDocumentUnit.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    Initialize();

                    pecToDocumentUnit.initialize();
                    pecToDocumentUnit.insertWorkflowActivity(fascicleModel, workflowActivityModel, url);
                    setResizeSensors();
                });

            }

            function radioButton_OnClick() {
                require(["PEC/PECToDocumentUnit"], function (PECToDocumentUnit) {
                    pecToDocumentUnit = new PECToDocumentUnit(tenantModelConfiguration.serviceConfiguration);
                    Initialize();
                    pecToDocumentUnit.initialize();
                    pecToDocumentUnit.radioButton_OnClick();

                    setResizeSensors();
                });

            }

            function setResizeSensors() {
                new ResizeSensor($(".details-template")[0], function () {
                    var height = $(".details-template").height() - 4;
                    $("#<%= pnlDocumentUnitSelect.ClientID %>").height(height);
                });
            }

            function onFascicleMiscellaneaClick() {
                if (pecToDocumentUnit.hasSelectedFascicle()) {
                    ShowLoadingPagePanel();
                    pecToDocumentUnit.cmdFascMiscellaneaInsert_Click();
                } else {
                    alert("Nessun fascicolo selezionato");
                }
                return false;
            }

            function onArchiveClick(sender, args) {
                if (Page_ClientValidate('Attach')) {
                    ShowLoadingPagePanel();
                } else {
                    return false;
                }
            }

            function MutuallyExclusive(radio, rowIndex) {
                var CurrentRdbID = radio.id;

                var grdDocuments = document.getElementById("<%= DocumentListGrid.ClientID%>");

                var items = grdDocuments.getElementsByTagName('input');
                for (i = 0; i < items.length; i++) {
                    if (items[i].id != CurrentRdbID && items[i].type == "radio")
                        if (items[i].checked)
                            items[i].checked = false;
                }

                // Al momento non funziona, non si riesce ad accedere a get_masterTableView
                // SelectRow(rowIndex);
            }

            function SelectRow(rowIndex) {
                var grdDocuments = document.getElementById("<%= DocumentListGrid.ClientID%>");
                var rowControl = grdDocuments.get_masterTableView().get_dataItems()[rowindex].get_element();
                grdDocuments.get_masterTableView().selectItem(rowControl, true);
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

            function OpenRenameAttachmentsWindow() {
                var wnd = $find("<%=renameAttachmentsWindow.ClientID%>");
                wnd.show();
            }

            function showDialogInitiallyClose() {
                $find('<%=modalPopup.ClientID %>').close();
                objtxt.value = document.getElementById("password").value
                var o = document.getElementById(objbtn)
                if (o.onclick) {
                    o.onclick();
                }
                else if (o.click) {
                    o.click();
                }
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%=modalPopup.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);

            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= DocumentListGrid.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function ShowLoadingPagePanel() {
                $("#<%= pnlButtons.ClientID%>").hide();
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%=DocumentListGrid.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPagePanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= panelPEC.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function responseEnd(sender, eventArgs) {
                HideLoadingPanel();
            }

            //Metodi per editor radgrid
            var fileName = null;
            var oldFileName = null;
            var rgx = /[^\\/]+\.[^\\/]+$/;

            function IsValidFileName(name) {
                var validName = (name.match(rgx) || []).pop();
                if (validName != "" && validName != undefined) {
                    return true;
                } else {
                    return false;
                }
            }

            function GetCellValue(sender, args) {
                if (args.get_columnUniqueName() === "Description") {
                    args.set_cancel(true);
                    var container = args.get_container();
                    fileName = $telerik.findElement(container, "lblFileName").innerHTML;
                    args.set_value(fileName);
                }
            }

            function SetCellValue(sender, args) {
                if (args.get_columnUniqueName() === "Description") {
                    args.set_cancel(true);
                    var container = args.get_container(),
                        documentName = args.get_value();
                    $telerik.findElement(container, "lblFileName").innerHTML = documentName;
                    $telerik.findElement(container, "txtNewFileName").value = documentName;
                }
            }

            function GetEditorValue(sender, args) {
                if (args.get_columnUniqueName() === "Description") {
                    args.set_cancel(true);
                    var container = args.get_container(),
                        fileName = $telerik.findControl(container, "txtDocumentName").get_value();
                    if (!IsValidFileName(fileName)) {
                        alert("Il nome file inserito non è corretto");
                        fileName = oldFileName;
                    }
                    args.set_value(fileName);
                }
            }

            function SetEditorValue(sender, args) {
                if (args.get_columnUniqueName() === "Description") {
                    args.set_cancel(true);
                    var container = args.get_container(),
                        documentName = args.get_value();
                    oldFileName = documentName;
                    $telerik.findControl(container, "txtDocumentName").set_value(documentName);
                }
            }

        </script>
        <style type="text/css">
            .RadGrid .rgRow,
            .RadGrid .rgAltRow {
                height: 30px;
            }

            .RadGrid .rgHeader {
                width: 16px;
                height: 30px;
            }

            .customThTable {
                height: 20px;
                padding-left: 3px;
                vertical-align: middle;
                text-align: left;
                line-height: 20px;
                font-weight: bold;
                font-size: 11px;
                background-image: none;
                border-bottom: 1px solid #999999;
                color: navy;
                background-color: #dae9fe;
            }

            .customTdTable {
                text-align: left;
                vertical-align: top;
                padding: 2px;
                font-weight: normal;
                font-size: 11px;
                color: #000;
            }
        </style>
    </telerik:RadScriptBlock>

    <asp:Panel runat="server" ID="WarningPanel" CssClass="hiddenField">
        <asp:Label ID="WarningLabel" runat="server" />
    </asp:Panel>

    <asp:Panel runat="server" ID="WarningInteropPanel" CssClass="hiddenField">
        <asp:Label ID="WarningInteropLabel" runat="server" />
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlDestination" CssClass="hiddenField">
        <asp:Label ID="lblDestination" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="panelPECFromFile">
        <table class="datatable">
            <tr class="Chiaro">
                <td style="vertical-align: middle; font-size: 8pt; text-align: left;">
                    <b>
                        <label runat="server" id="ddlMailBoxLabel"></label>
                    </b>
                </td>
                <td style="vertical-align: middle; font-size: 8pt">
                    <asp:DropDownList AutoPostBack="true" DataTextField="MailBoxName" DataValueField="Id" ID="ddlMailbox" runat="server" Width="300" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <usc:UploadDocument runat="server" ID="uscUploadDocumenti" AllowedExtensions=".eml,.msg"
                        MultipleDocuments="false" Caption="PEC Mail" IsDocumentRequired="true" ButtonPreviewEnabled="false" ButtonRemoveEnabled="false" ButtonScannerEnabled="false" SignButtonEnabled="false" />
                </td>
            </tr>

        </table>
    </asp:Panel>

    <asp:Panel runat="server" ID="panelPEC">

        <usc:uscPECInfo ID="uscPECInfo" runat="server" />

        <usc:uscInteropInfo ID="uscInteropInfo" runat="server" Visible="false" />

        <telerik:RadAjaxPanel runat="server" ID="pnlInterop">
            <table class="datatable">
                <tr>
                    <th colspan="2">Dati Interoperabilità</th>
                </tr>
                <tr class="Chiaro">
                    <td class="label">Utilizza dati
                    </td>
                    <td style="width: 85%">
                        <asp:CheckBox ID="chkInterop" runat="server" Enabled="false" AutoPostBack="True" />
                    </td>
                </tr>
                <tr class="Chiaro">
                    <td class="label">Mittente
                    </td>
                    <td style="width: 85%">
                        <asp:CheckBox ID="chkInteropMittente" runat="server" Text="" Enabled="false" AutoPostBack="True" />
                    </td>
                </tr>
            </table>
        </telerik:RadAjaxPanel>

        <telerik:RadPageLayout runat="server">
            <Rows>
                <telerik:LayoutRow HtmlTag="Div" Height="100%">
                    <Columns>
                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding t-col-right-padding">
                            <asp:Panel runat="server" ID="pnlDocumentUnitSelect">
                                <div class="dsw-panel" style="min-height: 100%;" id="mainContainer">
                                    <div class="dsw-panel-title">
                                        Gestisci in
                                    </div>
                                    <div class="dsw-panel-content">
                                        <asp:RadioButtonList ID="rblDocumentUnit" CssClass="col-dsw-offset-1" AutoPostBack="true" runat="server">
                                            <asp:ListItem Text="Protocollo" Value="1" Selected="True" />
                                            <asp:ListItem Text="Archivi" Value="7" />
                                            <asp:ListItem Text="Fascicoli" Value="8" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                        <telerik:LayoutColumn Span="8" CssClass="details-template t-col-left-padding t-col-right-padding" Style="min-height: 100px;">
                            <asp:Panel runat="server" ID="pnlTemplateProtocol">
                                <div class="dsw-panel" style="height: 100px;">
                                    <div class="dsw-panel-title">
                                        Template Protocollo
                                    </div>
                                    <div class="dsw-panel-content" style="padding-left: 10%; padding-top: 10px;">
                                        <asp:DropDownList ID="ddlTemplateProtocol" runat="server" AutoPostBack="True"></asp:DropDownList>
                                    </div>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" ID="pnlUDSSelect" Style="display: none;">
                                <div class="dsw-panel" style="height: 100px;">
                                    <div class="dsw-panel-title">
                                        Archivio
                                    </div>
                                    <div class="dsw-panel-content" style="padding-left: 10%; padding-top: 10px;">
                                        <telerik:RadComboBox runat="server" ID="ddlUDSArchives" DataTextField="Name" DataValueField="Id" AutoPostBack="true" Filter="Contains" CausesValidation="false" Height="200px" Width="300" />
                                        <asp:RequiredFieldValidator runat="server" ID="rfvUDSArchives" ErrorMessage="Selezionare un archivio" ValidationGroup="Attach" ControlToValidate="ddlUDSArchives"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" ID="pnlFascicleSelect" Style="display: none;">
                                <uc1:uscFascicleSearch runat="server" ID="uscFascicleSearch" MinHeight="100px"></uc1:uscFascicleSearch>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>

        <asp:Panel runat="server" ID="radAjaxPnlSender">
            <table class="datatable">
                <tr>
                    <th colspan="3">Indirizzo del mittente</th>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 10%;">Importazione:
                    </td>
                    <td style="padding-top: 5px; width: 500px;">
                        <asp:RadioButtonList ID="rblTypeSender" runat="server" AutoPostBack="True" Font-Names="Verdana" DataTextField="Description" DataValueField="Id" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Non riportare mittente" Value="0" />
                            <asp:ListItem Text="Aggiungere contatto manuale" Value="1" />
                            <asp:ListItem Text="Contatto da rubrica" Value="2" />
                        </asp:RadioButtonList>
                    </td>
                    <td style="padding-top: 5px;">
                        <asp:Panel runat="server" ID="pnlUDSContact">
                            <span class="label">Seleziona tipo contatto</span>
                            <telerik:RadComboBox runat="server" ID="ddlContactFields" DataTextField="Label" DataValueField="Label" Height="200px" Width="150" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="Chiaro">
                    <td class="label">Email certificata:
                    </td>
                    <td>
                        <asp:CheckBox ID="chbEMailCertified" runat="server" Text="" AutoPostBack="True" />
                    </td>
                </tr>
            </table>

            <table class="datatable">

                <tr class="Chiaro">
                    <td>
                        <asp:CheckBox ID="chkUseOChart" runat="server" Text="Utilizza protocollazione da Organigramma" Visible="False" AutoPostBack="True" />
                    </td>
                </tr>
            </table>
            <usc:SelContatti ButtonDeleteVisible="False" ButtonImportManualVisible="false" ButtonImportVisible="False" ButtonManualVisible="False" ButtonPropertiesVisible="True" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="False" Caption="Mittenti" EnableCC="false" ID="uscMittenti" Multiple="true" MultiSelect="true" ProtType="True" ReadOnly="False" RequiredErrorMessage="Mittente obbligatorio" runat="server" TreeViewCaption="Mittenti" Type="Prot" />

        </asp:Panel>

        <asp:Panel runat="server" ID="pnlRenameAttahmentWarning" CssClass="hiddenField">
            <asp:Label ID="lblMessage" runat="server" Text="Clicca sul nome del documento per modificarlo" />
        </asp:Panel>

        <table width="100%">

            <tr>
                <th class="customThTable">Elenco dei documenti.<b><asp:Label Text=" Attenzione il controllo della firma sui documenti è stato disattivato." runat="server" ID="lblWarning" /></b></th>
            </tr>

            <tr class="Chiaro">
                <td class="customTdTable" style="vertical-align: middle;">
                    <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" AllowMultiRowSelection="true">
                        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="Serialized" TableLayout="Fixed">
                            <BatchEditingSettings EditType="Cell" />
                            <Columns>

                                <telerik:GridClientSelectColumn HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" UniqueName="Select" />

                                <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento Principale">
                                    <ItemTemplate>
                                        <asp:RadioButton runat="server" ID="rbtMainDocument" AutoPostBack="false" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn UniqueName="DocumentExtensionImage" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
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
                                        <asp:TextBox runat="server" ID="txtNewFileName" CssClass="hiddenField"></asp:TextBox>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadTextBox runat="server" ID="txtDocumentName" Width="300px"></telerik:RadTextBox>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings EnableRowHoverStyle="False">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                            <ClientEvents OnRowMouseOver="DisableRowMouseOver" OnBatchEditGetCellValue="GetCellValue" OnBatchEditGetEditorValue="GetEditorValue"
                                OnBatchEditSetCellValue="SetCellValue" OnBatchEditSetEditorValue="SetEditorValue" />
                        </ClientSettings>
                    </telerik:RadGrid>
                    <telerik:RadToolTip ID="disableTooltipRow" runat="server" ShowEvent="FromCode" AutoCloseDelay="0">
                        Documento non selezionabile per la protocollazione
                    </telerik:RadToolTip>
                </td>
            </tr>

        </table>
    </asp:Panel>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="cmdInit" OnClientClick="ShowLoadingPagePanel();" runat="server" Text="Conferma" CausesValidation="false" ValidationGroup="Attach" Width="150px" />
        <asp:Button ID="cmdInitAndClone" OnClientClick="ShowLoadingPagePanel();" runat="server" Text="Conferma e duplica" CausesValidation="false" ValidationGroup="Attach" Width="150px" />
        <asp:Button ID="cmdAnnulla" CausesValidation="false" OnClientClick="ShowLoadingPagePanel();" runat="server" Text="Annulla" ValidationGroup="Attach" Width="150px" />
    </asp:Panel>
</asp:Content>
