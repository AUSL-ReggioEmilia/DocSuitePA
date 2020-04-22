<%@ Page Title="Tavoli - Inserimento protocollo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskToProtocol.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskToProtocol" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function <%= Me.ID %>_OpenWindow(url, name, parameters) {
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.set_destroyOnClose(true);
                wnd.show();
                wnd.center();
                return false;
            }

            function SelectMeOnly(objRadioButton, grdName) {

                var i, row;
                var grid = $find("<%=dgvDocuments.ClientID%>");
                var masterTable = grid.get_masterTableView();
                var rows = masterTable.get_dataItems();

                for (i = 0; i < rows.length; i++) {
                    row = rows[i];

                    var radioBtn = row.findElement("rdbMainDocument");                    

                    if (objRadioButton.id == radioBtn.id) {
                        radioBtn.checked = true;                        
                    } else {
                        radioBtn.checked = false;                        
                    }
                }
            }

            //Reset checkBox
            function resetGridControls() {
                var i, row;
                var grid = $find("<%=dgvDocuments.ClientID%>");
                var masterTable = grid.get_masterTableView();
                var rows = masterTable.get_dataItems();

                for (i = 0; i < rows.length; i++) {
                    row = rows[i];

                    var radioBtn = row.findElement("rdbMainDocument");                    
                    radioBtn.checked = false;
                }
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlPageContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlPageContent.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function OpenNewProtocol(sender, args) {
                ShowLoadingPanel();
            }

            function responseEnd(sender, eventArgs) {
                HideLoadingPanel();
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    resetGridControls();
                }
            }

            function RowSelectedChanged(sender, eventArgs) {
                var grid = $find("<%=dgvDocuments.ClientID %>");
                var enabled = false;
                var masterTable = grid.get_masterTableView();
                var selectedRows = masterTable.get_selectedItems();               

                if (!selectedRows.length) return;
                enabled = true;
                $.each(selectedRows, function( index, row ) {
                    var documentSigned = masterTable.getCellByColumnUniqueName(row, "IsSigned");
                    if (documentSigned.outerText == "False") {
                        enabled = false;
                        return;
                    }
                });
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

        <asp:Panel runat="server" ID="pnlDeskMetaData">
            <table class="datatable">
                <tr>
                    <th colspan="2">Dati Tavolo</th>
                </tr>
                <tr>
                    <td style="vertical-align: middle;">
                        <label class="DeskLabel">Nome del tavolo:</label>
                    </td>
                    <td style="width: 100%">
                        <asp:Label runat="server" ID="lblDeskName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: middle;">
                        <label class="DeskLabel">Oggetto:</label>
                    </td>
                    <td style="width: 100%">
                        <asp:Label runat="server" ID="lblDeskSubject"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: middle;">
                        <label class="DeskLabel">Data scadenza:</label>
                    </td>
                    <td style="width: 100%">
                        <asp:Label runat="server" ID="lblExpireDate"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlDeskDocuments">
            <table class="datatable">
                <tr>
                    <th>Documenti Tavolo</th>
                </tr>
                <tr class="Chiaro">
                    <td style="vertical-align: middle;">
                        <telerik:RadGrid runat="server" ID="dgvDocuments" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="BiblosSerializeKey">
                                <Columns>

                                    <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderStyle-HorizontalAlign="Center" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento Principale">
                                        <ItemTemplate>
                                            <asp:RadioButton runat="server" ID="rdbMainDocument" GroupName="selectMainDocument" AutoPostBack="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridBoundColumn UniqueName="IsSigned" DataField="IsSigned" Display="false" />

                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Visualizza documento">
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
                                <ClientEvents OnRowSelected="RowSelectedChanged" OnRowDeselected="RowSelectedChanged" />
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" CssClass="footerButtons" ID="pnlButtons">
        <telerik:RadButton runat="server" Width="170px" OnClientClicked="OpenNewProtocol" ValidationGroup="deskValidation" ID="btnToSign" Text="Crea nuovo protocollo"></telerik:RadButton>        
        <telerik:RadButton runat="server" Width="150px" OnClientClicking="ShowLoadingPanel" ID="btnCancel" Text="Annulla"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
