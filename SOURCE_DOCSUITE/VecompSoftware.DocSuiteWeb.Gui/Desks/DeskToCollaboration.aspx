<%@ Page Title="Tavoli - Inserimento collaborazione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskToCollaboration.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskToCollaboration" %>

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
                    var dropDown = row.findElement("ddlDocumentType");

                    if (objRadioButton.id == radioBtn.id) {
                        radioBtn.checked = true;
                        $find(dropDown.id).set_visible(false);
                    } else {
                        radioBtn.checked = false;
                        $find(dropDown.id).set_visible(true);
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
                    var dropDown = row.findElement("ddlDocumentType");
                    radioBtn.checked = false;
                    $find(dropDown.id).set_visible(true);

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

            function OpenNewCollaboration(sender, args) {
                if (Page_IsValid) {
                    ShowLoadingPanel();
                    args.IsValid = true;
                }
                else {
                    args.IsValid = false;
                }
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
                var btn = $find("<%=btnToProtocol.ClientID%>");
                var enabled = false;
                var masterTable = grid.get_masterTableView();
                var selectedRows = masterTable.get_selectedItems();               

                btn.set_enabled(false);
                if (!selectedRows.length) return;
                enabled = true;
                $.each(selectedRows, function( index, row ) {
                    var documentSigned = masterTable.getCellByColumnUniqueName(row, "IsSigned");
                    if (documentSigned.outerText == "False") {
                        enabled = false;
                        return;
                    }
                });

                btn.set_enabled(enabled);
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
        <asp:Panel runat="server" ID="pnlCollaborationType">
            <table class="datatable">
                <tr>
                    <th colspan="2">Tipologia di collaborazione</th>
                </tr>
                <tr>
                    <td class="col-dsw-2 label">Tipologia Documento:</td>
                    <td class="col-dsw-8">
                        <telerik:RadDropDownList runat="server" ID="ddlCollaborationType" AutoPostBack="False" Width="300px" CausesValidation="True" DefaultMessage="Seleziona una tipologia..."></telerik:RadDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="col-dsw-2"></td>
                    <td class="col-dsw-8">
                        <asp:RequiredFieldValidator ValidationGroup="deskValidation" runat="server" ID="ddlCollaborationTypeValidator" Display="Dynamic" ControlToValidate="ddlCollaborationType" ErrorMessage="Il campo tipologia di collaborazione è obbligatorio"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlDeskMetaData">
            <table class="datatable">
                <tr>
                    <th colspan="2">Dati Tavolo</th>
                </tr>
                <tr>
                    <td style="vertical-align: middle;">
                        <label class="DeskLabel">Nome del tavolo:</label>
                    </td>
                    <td class="col-dsw-10">
                        <asp:Label runat="server" ID="lblDeskName"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: middle;">
                        <label class="DeskLabel">Oggetto:</label>
                    </td>
                    <td class="col-dsw-10">
                        <asp:Label runat="server" ID="lblDeskSubject"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: middle;">
                        <label class="DeskLabel">Data scadenza:</label>
                    </td>
                    <td class="col-dsw-10">
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
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" Width="170px" OnClientClicked="OpenNewCollaboration" ValidationGroup="deskValidation" ID="btnToSign" Text="Inserisci alla Visione/Firma"></telerik:RadButton>
        <telerik:RadButton runat="server" Width="200px" OnClientClicked="OpenNewCollaboration" ValidationGroup="deskValidation" ID="btnToProtocol" Enabled="False" Text="Inserisci al Protocollo/Segreteria"></telerik:RadButton>
        <telerik:RadButton runat="server" Width="150px" OnClientClicking="ShowLoadingPanel" ID="btnCancel" Text="Annulla"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
