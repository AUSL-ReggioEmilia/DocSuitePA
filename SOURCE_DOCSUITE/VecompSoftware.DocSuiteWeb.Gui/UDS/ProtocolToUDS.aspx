<%@ Page Title="Protocollo - inserimento in Archivio" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtocolToUDS.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtocolToUDS" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagPrefix="usc" TagName="ProtocolPreview" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function openWindow(url, name, parameters) {
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.set_destroyOnClose(true);
                wnd.show();
                wnd.center();
                return false;
            }

            function mutuallyExclusive(checkbox, rowIndex) {
                var currentChkID = checkbox.id;
                var grdDocuments = document.getElementById("<%= dgvDocuments.ClientID%>");

                var items = grdDocuments.getElementsByTagName('input');
                for (i = 0; i < items.length; i++) {
                    if (items[i].id != currentChkID && items[i].type == "checkbox" && items[i].className == "selectmaindocument")
                        if (items[i].checked)
                            items[i].checked = false;
                }

                selectRow(checkbox, rowIndex);
            }

            function selectRow(checkbox, rowIndex) {
                var gridView = $find("<%= dgvDocuments.ClientID %>").get_masterTableView();
                var rows = gridView.get_dataItems();
                for (var i = 0; i < rows.length; i++) {
                    if (i == rowIndex && checkbox.checked) {
                        $(gridView.getCellByColumnUniqueName(rows[i], "typeDoc").childNodes[1]).hide();
                    } else {
                        $(gridView.getCellByColumnUniqueName(rows[i], "typeDoc").childNodes[1]).show();
                    }
                }
            }

            function ShowLoadingPanel() {
                if (Page_ClientValidate("udsValidation")) {
                    var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                    var currentUpdatedControl = "<%= pnlPageContent.ClientID%>";
                    currentLoadingPanel.show(currentUpdatedControl);

                    var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                    var pnlButtons = "<%= btnConfirm.ClientID%>";
                    ajaxFlatLoadingPanel.show(pnlButtons);
                }
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerDocument" runat="server">
        <Windows>
            <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Anteprima documento" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPageContent">
        <asp:Panel runat="server" ID="pnlSeries">
            <table class="datatable">
                <tr>
                    <th colspan="2">Archivi</th>
                </tr>
                <tr>
                    <td class="label col-dsw-2" style="vertical-align: middle;">Seleziona un archivio:
                    </td>
                    <td class="col-dsw-8">
                        <telerik:RadDropDownList runat="server" ID="ddlUDSs" AutoPostBack="True" Width="300px" CausesValidation="False"></telerik:RadDropDownList>
                        <asp:RequiredFieldValidator ValidationGroup="udsValidation" runat="server" ID="ddlSeriesValidator" Display="Dynamic" ControlToValidate="ddlUDSs" ErrorMessage="Il campo è obbligatorio"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlProtocolMetaData">
            <usc:ProtocolPreview runat="server" ID="uscProtocolPreview" Title="Dettaglio Protocollo" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlProtocolDocuments">
            <table class="datatable">
                <tr>
                    <th>Documenti Protocollo</th>
                </tr>
                <tr>
                    <td>
                        <asp:Panel runat="server" ID="WarningPanel" CssClass="hiddenField">
                           <asp:Label ID="WarningLabel" runat="server" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <div class="radGridWrapper">
                <telerik:RadGrid runat="server" ID="dgvDocuments" Width="100%" AllowMultiRowSelection="true">
                    <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
                    <MasterTableView AutoGenerateColumns="False" TableLayout="Auto" DataKeyNames="Serialized">
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <SelectFields>
                                    <telerik:GridGroupByField FieldAlias="Tipologia" FieldName="DocumentType" HeaderText=""></telerik:GridGroupByField>
                                </SelectFields>
                                <GroupByFields>
                                    <telerik:GridGroupByField FieldName="DocumentType" SortOrder="Descending"></telerik:GridGroupByField>
                                </GroupByFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                        <Columns>
                            <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                            <telerik:GridTemplateColumn UniqueName="SelectMainDocument" HeaderStyle-Width="16px" HeaderStyle-HorizontalAlign="Center" 
                                HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento Principale">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelectMainDocument" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn UniqueName="ImageExtensionType" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Visualizza documento">
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgDocumentExtensionType" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome Documento">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDocumentName"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Tipo Documento" UniqueName="typeDoc">
                                <HeaderStyle Width="60%"></HeaderStyle>
                                <ItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="ddlDocumentType" AutoPostBack="False" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnableRowHoverStyle="False">
                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                    </ClientSettings>
                </telerik:RadGrid>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" Width="150px" ValidationGroup="udsValidation" CausesValidation="True" OnClientClick="ShowLoadingPanel();" ID="btnConfirm" Text="Conferma"></asp:Button>
</asp:Content>
