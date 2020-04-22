<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EventPECStreamError.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.EventPECStreamError" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Informazioni su PEC in errore" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>
<%@ Register Src="~/PEC/uscPECInfo.ascx" TagPrefix="usc" TagName="uscPECInfo" %>
<%@ Register Src="~/PEC/uscInteropInfo.ascx" TagPrefix="usc" TagName="uscInteropInfo" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var eventPECStreamError;
            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    eventPECStreamError.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                eventPECStreamError.onGridDataBound();
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= DocumentListGrid.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function responseEnd(sender, eventArgs) {
                HideLoadingPanel();
            }

            function GetCellValue(sender, args) {
                if (args.get_columnUniqueName() === "Description") {
                    args.set_cancel(true);
                    var container = args.get_container();
                    fileName = $telerik.findElement(container, "lblFileName").innerHTML;
                    args.set_value(fileName);
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

            function SetCellValue(sender, args) {
                if (args.get_columnUniqueName() === "Description") {
                    args.set_cancel(true);
                    var container = args.get_container(),
                        documentName = args.get_value();
                    $telerik.findElement(container, "lblFileName").innerHTML = documentName;
                    $telerik.findElement(container, "txtNewFileName").value = documentName;
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

            function MutuallyExclusive(radio, rowIndex) {
                var CurrentRdbID = radio.id;

                var grdDocuments = document.getElementById("<%= DocumentListGrid.ClientID%>");

                var items = grdDocuments.getElementsByTagName('input');
                for (i = 0; i < items.length; i++) {
                    if (items[i].id != CurrentRdbID && items[i].type === "radio")
                        if (items[i].checked)
                            items[i].checked = false;
                }

                // Al momento non funziona, non si riesce ad accedere a get_masterTableView
                // SelectRow(rowIndex);
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <asp:Panel runat="server" ID="panelPEC">

        <usc:uscPECInfo ID="uscPECInfo" runat="server" />

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

        <table width="100%">
            <tr>
                <td runat="server" style="min-width: 400px;" id="pnlDocumentUnitSelect">
                    <asp:Panel runat="server">
                        <div class="dsw-panel" style="height: 60px;">
                            <div class="dsw-panel-title">
                                Gestisci in
                            </div>
                            <div class="dsw-panel-content">
                                <asp:RadioButtonList ID="rblDocumentUnit" CssClass="col-dsw-offset-1" AutoPostBack="true" runat="server">
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </asp:Panel>
                </td>
                <td style="width: 100%;" runat="server" id="pnlTemplateProtocol">
                    <asp:Panel runat="server">
                        <div class="dsw-panel" style="height: 60px;">
                            <div class="dsw-panel-title">
                                Template Protocollo
                            </div>
                            <div class="dsw-panel-content" style="padding-left: 10%; padding-top: 10px;">
                                <asp:DropDownList ID="ddlTemplateProtocol" runat="server" AutoPostBack="True"></asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>
                </td>
                <td runat="server" style="width: 100%;" id="pnlUDSSelect">
                    <asp:Panel runat="server">
                        <div class="dsw-panel" style="height: 60px;">
                            <div class="dsw-panel-title">
                                Archivio
                            </div>
                            <div class="dsw-panel-content" style="padding-left: 10%; padding-top: 10px;">
                                <telerik:RadComboBox runat="server" ID="ddlUDSArchives" DataTextField="Name" DataValueField="Id" AutoPostBack="true" Filter="Contains" CausesValidation="false" Height="200px" Width="300" />
                                <asp:RequiredFieldValidator runat="server" ID="rfvUDSArchives" ErrorMessage="Selezionare un archivio" ValidationGroup="Attach" ControlToValidate="ddlUDSArchives"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </asp:Panel>
                </td>
            </tr>
        </table>

        <asp:Panel runat="server" ID="radAjaxPnlSender">
            <table class="datatable">
                <tr>
                    <th colspan="3">Indirizzo del mittente</th>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 10%;">Importazione:
                    </td>
                    <td style="width: 250px;">
                        <asp:RadioButtonList ID="rblTypeSender" runat="server" AutoPostBack="True" Font-Names="Verdana" DataTextField="Description" DataValueField="Id">
                            <asp:ListItem Text="Non riportare mittente" Value="0" />
                            <asp:ListItem Text="Aggiungere contatto manuale" Value="1" />
                            <asp:ListItem Text="Contatto da rubrica" Value="2" />
                        </asp:RadioButtonList>
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
            <usc:SelContatti ButtonDeleteVisible="False" ButtonImportManualVisible="false" ButtonImportVisible="False" ButtonManualVisible="False" ButtonPropertiesVisible="True" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="False" Caption="Mittenti" EnableCC="false" ID="uscMittenti" Multiple="true" MultiSelect="true" ProtType="True" ReadOnly="False" RequiredErrorMessage="Mittente obbligatorio" runat="server" TreeViewCaption="Mittenti" Type="Prot" />
        </asp:Panel>

        <asp:Panel runat="server" ID="Panel2" CssClass="hiddenField">
            <asp:Label ID="Label1" runat="server" Text="Clicca sul nome del documento per modificarlo" />
        </asp:Panel>
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
                    <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="Serialized">
                        <BatchEditingSettings EditType="Cell" />
                        <Columns>
                            <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />
                            <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderStyle-HorizontalAlign="Center" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento Principale">
                                <ItemTemplate>
                                    <asp:RadioButton runat="server" ID="rbtMainDocument" AutoPostBack="false" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="DocumentExtensionImage" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
                                <ItemTemplate>
                                    <asp:Image ID="Image1" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFileName" Text='<%# Eval("Name")%>'></asp:Label>
                                    <asp:ImageButton ID="imgDecrypt" runat="server" Visible="false" CausesValidation="false" ToolTip="Archivio protetto da password. Premi e inserisci la password dell'archivio" />
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

    <div style="margin: 1px;">
        <div style="float: left;">
            <telerik:RadButton ID="btnProtocol" Text="Protocolla" Width="150px" runat="server" TabIndex="1" CausesValidation="false" />
        </div>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="cmdProtInit" OnClientClick="ShowLoadingPagePanel();" runat="server" Text="Conferma" ValidationGroup="Attach" Width="150px" />
    </asp:Panel>
</asp:Content>
