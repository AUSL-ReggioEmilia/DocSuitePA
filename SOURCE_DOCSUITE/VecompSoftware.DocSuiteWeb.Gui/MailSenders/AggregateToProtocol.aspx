<%@ Page Language="vb" AutoEventWireup="False" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="AggregateToProtocol.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.AggregateToProtocol" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>
<%@ Register Src="~/MailSenders/MailSenderControl.ascx" TagPrefix="uc1" TagName="MailSenderControl" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>



<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script type="text/javascript">

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= panelProt.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
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
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="panelProt">
        <table class="datatable">
            <tr>
                <th>
                    <label class="label">Elenco Protocolli</label></th>
            </tr>
            <tr>
                <td>
                    <telerik:RadTreeView ID="rtvProtocol" Width="100%" runat="server" PersistLoadOnDemandNodes="true">
                    </telerik:RadTreeView>
                </td>
            </tr>
        </table>
        <usc:SelContatti ButtonImportManualVisible="false" ButtonImportVisible="False" ButtonManualVisible="False" ButtonPropertiesVisible="True" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="False" Caption="Destinatari" EnableCC="false" ID="uscMittenti" Multiple="true" MultiSelect="true" ProtType="True" ReadOnly="False" RequiredErrorMessage="Mittente obbligatorio" runat="server" TreeViewCaption="Destinatari" Type="Prot" />
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
                                <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />

                                <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento Principale">
                                    <ItemTemplate>
                                        <asp:RadioButton runat="server" ID="rbtMainDocument" AutoPostBack="false" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn UniqueName="DocumentExtensionImage" HeaderStyle-Width="16px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
                                    <ItemTemplate>
                                        <asp:Image ID="Image1" runat="server" ImageUrl='<%# ImagePath.FromFile(Eval("Name").ToString())%>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento" HeaderStyle-Width="90%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblFileName" Text='<%# Eval("Name")%>'></asp:Label>
                                        <asp:TextBox runat="server" ID="txtNewFileName" CssClass="hiddenField"></asp:TextBox>
                                    </ItemTemplate>
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
        <asp:Button ID="cmdProtInit" OnClientClick="ShowLoadingPanel();" runat="server" Text="Conferma" ValidationGroup="Attach" Width="150px" />
    </asp:Panel>
</asp:Content>
