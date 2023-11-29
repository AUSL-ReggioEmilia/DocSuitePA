<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UDSInvoicesUpload.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSInvoicesUpload"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione cassetto fiscale Agenzia delle Entrate" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">

    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script type="text/javascript">
            var udsInvoicesUpload;

            function ValidatePageClient() {
                return Page_ClientValidate('');
            }

            function previewImport(sender, args) {
                udsInvoicesUpload.previewImport(sender, args);
            }

            function confirmImport(sender, args) {
                udsInvoicesUpload.confirmImport(sender, args);
            }

            function RowBinding(sender, args) {
                udsInvoicesUpload.RowBinding(sender, args);
            }

            function SetChainId(id, startWorkflow) {
                udsInvoicesUpload.SetChainId(id, startWorkflow);
            }

            function SetIdDocument(id) {
                udsInvoicesUpload.SetIdDocument(id);
            }

            require(["UDS/UDSInvoicesUpload"], function (UDSInvoicesUpload) {
                $(function () {
                    //newing up
                    udsInvoicesUpload = new UDSInvoicesUpload(ValidatePageClient);

                    //assing dom data
                    udsInvoicesUpload.btnSaveId = ("<%= btnSave.ClientID %>");
                    udsInvoicesUpload.btnPreviewId = ("<%= btnPreview.ClientID %>");
                    udsInvoicesUpload.radListMessagesId = ("<%= radListMessages.ClientID %>");
                    udsInvoicesUpload.currentLoadingPanelId = ("<%=  MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>");
                    udsInvoicesUpload.currentFlatLoadingPanelId = ("<%=  MasterDocSuite.AjaxFlatLoadingPanel.ClientID %>");
                    udsInvoicesUpload.signalRServerAddress = ("<%=  SignalRServerAddress %>");
                    udsInvoicesUpload.hFcorrelatedChainId = ("<%=  HFcorrelatedChainId.ClientID %>");
                    udsInvoicesUpload.hFcorrelatedIdDocument = ("<%=  HFcorrelatedIdDocument.ClientID %>");
                    udsInvoicesUpload.ajaxManagerId = ("<%=  AjaxManager.ClientID %>");
                    udsInvoicesUpload.rgvPreviewDocumentsId = ("<%=  rgvPreviewDocuments.ClientID %>");
                    udsInvoicesUpload.currentUserTenantName = ("<%= CurrentUserTenantName %>");
                    udsInvoicesUpload.currentUserTenantId = ("<%= CurrentUserTenantId %>");
                    udsInvoicesUpload.currentUserTenantAOOId = ("<%= CurrentUserTenantAOOId %>");
                    udsInvoicesUpload.currentUpdatedControlId = "<%= pnlContentInvoice.ClientID%>";
                    udsInvoicesUpload.currentUpdatedToolbarId = "<%= pnlButtons.ClientID %>";
                    udsInvoicesUpload.rdtpDataRicezioneSdiId = "<%= rdtpDataRicezioneSdi.ClientID %>";
                    //initialize
                    udsInvoicesUpload.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlContentInvoice" Width="100%">
        <telerik:RadPageLayout runat="server" ID="insertsPageContent" HtmlTag="Div" Width="100%">
            <Rows>
                <telerik:LayoutRow runat="server" ID="InsertsDataRow">
                    <Rows>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <asp:HiddenField ID="HFcorrelatedChainId" runat="server" Value="" />
                                <asp:HiddenField ID="HFcorrelatedIdDocument" runat="server" Value="" />
                                <uc1:uscDocumentUpload ID="uscDocumentUpload" Caption="Documento" runat="server" IsDocumentRequired="true" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" MultipleDocuments="false" UseSessionStorage="true" Type="Prot" AllowZipDocument="true" AllowedExtensions=".zip" />
                            </Content>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <b class="dsw-text-bold">Data Ricezione Sdi:</b>
                                <telerik:RadDateTimePicker ID="rdtpDataRicezioneSdi" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="rdtpDataRicezioneSdi" Display="Dynamic" ErrorMessage="Campo Dal Giorno Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                            </Content>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div" Width="100%">
                            <Content>
                                <telerik:RadGrid runat="server" ID="rgvPreviewDocuments" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" Skin="Office2010Blue" AllowMultiRowSelection="true" AllowFilteringByColumn="False">
                                    <ClientSettings>
                                        <Selecting AllowRowSelect="true" />
                                        <ClientEvents OnRowDataBound="RowBinding" />
                                    </ClientSettings>
                                    <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="UniqueId">
                                        <Columns>
                                            <telerik:GridClientSelectColumn>
                                            </telerik:GridClientSelectColumn>
                                            <telerik:GridTemplateColumn Visible="false" DataField="InvoiceFilename">
                                                <ItemTemplate>
                                                    <input type="hidden" value="InvoiceFilename" id="hdnInvoiceFilename" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn Visible="false" DataField="InvoiceMetadataFilename">
                                                <ItemTemplate>
                                                    <input type="hidden" value="InvoiceMetadataFilename" id="hdnInvoiceMetadataFilename" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="Description" HeaderStyle-Width="80%" HeaderText="Fattura" AllowFiltering="false" UniqueName="colInvoice">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Result" HeaderStyle-Width="18%" HeaderText="Esito" AllowFiltering="false" UniqueName="colOutcome">
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </Content>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <telerik:RadListBox RenderMode="Lightweight" ID="radListMessages" runat="server" Height="98%" Width="98%" SelectionMode="Single" />
                            </Content>
                        </telerik:LayoutRow>
                    </Rows>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton ID="btnPreview" runat="server" Width="150px" Text="Anteprima importazione" AutoPostBack="false" OnClientClicked="previewImport" />
        <telerik:RadButton ID="btnSave" runat="server" Width="150px" Text="Avvia importazione" AutoPostBack="false" OnClientClicked="confirmImport" />
    </asp:Panel>
</asp:Content>
