<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscStartWorkflow.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscStartWorkflow" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="oggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscUploadDocumentRest.ascx" TagName="uscUploadDocumentRest" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscStartWorkflow;
        require(["UserControl/uscStartWorkflow"], function (UscStartWorkflow) {
            $(function () {
                uscStartWorkflow = new UscStartWorkflow(tenantModelConfiguration.serviceConfiguration);
                uscStartWorkflow.dswEnvironment = "<%= Environment.ToString()%>"
                uscStartWorkflow.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscStartWorkflow.contentId = "<%= pnlWorkflowStart.ClientID %>";
                uscStartWorkflow.rdlWorkflowRepositoryId = "<%= ddlWorkflowRepository.ClientID %>"
                uscStartWorkflow.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscStartWorkflow.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscStartWorkflow.btnConfirmId = "<%= btnConfirm.ClientID %>";
                uscStartWorkflow.txtObjectId = "<%= txtObject.ClientID %>";
                uscStartWorkflow.tenantName = "<%= TenantName %>";
                uscStartWorkflow.tenantId = "<%= TenantId %>";
                uscStartWorkflow.ctrlTxtWfId = "<%=ctrlTxtWf.ClientID%>";
                uscStartWorkflow.rblPriorityId = "<%=rblPriority.UniqueID%>";
                uscStartWorkflow.dueDateId = "<%= dueDate.ClientID %>"

                uscStartWorkflow.uscProposerRoleId = "<%= uscProposerRole.TableContentControl.ClientID%>";
                uscStartWorkflow.uscProposerContactId = "<%=uscProposerContact.TableContent.ClientID%>";
                uscStartWorkflow.proposerRoleRowId = "<%=proposerRoleRow.ClientID%>";
                uscStartWorkflow.proposerContactRowId = "<%=proposerContactRow.ClientID%>";
                uscStartWorkflow.proposerContactRowLabelId = "<%=proposerContactRowLabel.ClientID%>";

                uscStartWorkflow.uscRecipientRoleId = "<%= uscRecipientRole.TableContentControl.ClientID%>";
                uscStartWorkflow.uscRecipientContactId = "<%=uscRecipientContact.TableContent.ClientID%>";
                uscStartWorkflow.recipientRoleRowId = "<%= recipientRoleRow.ClientID%>";
                uscStartWorkflow.recipientContactRowId = "<%=recipientContactRow.ClientID%>";
                uscStartWorkflow.recipientContactRowLabelId = "<%=recipientContactRowLabel.ClientID%>";

                uscStartWorkflow.uploadDocumentId = "<%=lrUploadDocument.ClientID%>";
                uscStartWorkflow.lblUploadDocumentId = "<%=lblUploadDocumentId.ClientID%>";
                uscStartWorkflow.uscDocumentId = "<%= uscUploadDocumentRest.uploadDocumentComponent.ClientID %>";

                uscStartWorkflow.lblTemplateCollaborationRowId = "<%= lblTemplateCollaborationRow.ClientID %>";
                uscStartWorkflow.ddlTemplateCollaborationRowId = "<%= ddlTemplateCollaborationRow.ClientID %>";
                uscStartWorkflow.ddlTemplateCollaborationId = "<%= ddlTemplateCollaboration.ClientID %>";

                uscStartWorkflow.lblChainTypeRowId = "<%= lblChainTypeRow.ClientID %>";
                uscStartWorkflow.chainTypeRowId = "<%= chainTypeRow.ClientID %>";
                uscStartWorkflow.rgvDocumentListsId = "<%= rgvDocumentLists.ClientID %>";

                uscStartWorkflow.docSuiteVersion = "<%=DSWVersion%>";
                uscStartWorkflow.showOnlyNoInstanceWorkflows = <%=ShowOnlyNoInstanceWorkflows.ToString().ToLower()%>;

                uscStartWorkflow.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>


<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<asp:Panel runat="server" ID="pnlWorkflowStart">
    <telerik:RadPageLayout runat="server" widht="100%" ID="pnlWorkflow" CssClass="dsw-panel">
        <Rows>
            <telerik:LayoutRow CssClass="dsw-panel-title" Style="margin-bottom: 2px;">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblTitle" runat="server" Font-Bold="True">Seleziona attività</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 2px;">
                        <telerik:RadComboBox runat="server" ID="ddlWorkflowRepository" Width="100%" Filter="Contains"
                            DataTextField="CompanyName" DataValueField="SupplierID"
                            CausesValidation="false" EnableLoadOnDemand="True" AutoPostBack="false" EmptyMessage="Selezionare attività">
                        </telerik:RadComboBox>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 2px;">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlWorkflowRepository"
                            ErrorMessage="E' obbligatorio selezionare una attività" Display="Dynamic"></asp:RequiredFieldValidator>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="dsw-panel-title ts-initialize" ID="lblTemplateCollaborationRow" Style="margin-bottom: 2px;">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblTemplateCollaboration" runat="server" Font-Bold="True">Template di collaborazione</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" CssClass="ts-initialize" ID="ddlTemplateCollaborationRow">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 2px;">
                        <telerik:RadComboBox runat="server" ID="ddlTemplateCollaboration" Width="100%" Filter="Contains"
                            CausesValidation="false" EnableLoadOnDemand="True" AutoPostBack="false" EmptyMessage="Selezionare template">
                        </telerik:RadComboBox>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>          
            <telerik:LayoutRow Style="margin-bottom: 2px;" CssClass="ts-initialize" ID="proposerRoleRow">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <usc:settori Caption="Settore richiedente" ID="uscProposerRole" RoleRestictions="OnlyMine" MultipleRoles="False" MultiSelect="False" runat="server" Required="False" UseSessionStorage="true" />
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="dsw-panel-title ts-initialize" Style="margin-bottom: 2px;" ID="proposerContactRowLabel">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblContactProposer" runat="server" Font-Bold="True">Utente proponente</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" HtmlTag="Div" CssClass="ts-initialize" ID="proposerContactRow" runat="server">
                <Content>
                    <usc:uscContattiSel ID="uscProposerContact" ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false"
                        ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="false" ButtonSelectOChartVisible="false"
                        ButtonSelectVisible="false" EnableCheck="True" EnableViewState="true" HeaderVisible="false" IsRequired="false"
                        TreeViewCaption="Utente proponente" UseAD="true" runat="server" UseSessionStorage="true" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" CssClass="ts-initialize" ID="recipientRoleRow">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <usc:settori Caption="Settore destinatario" ID="uscRecipientRole" MultipleRoles="False" MultiSelect="False" runat="server" Required="False" UseSessionStorage="true" />
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="recipientContactRowLabel" CssClass="dsw-panel-title ts-initialize" Style="margin-bottom: 2px;">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblContactRecipient" runat="server" Font-Bold="True">Utente destinatario</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="recipientContactRow" Style="margin-bottom: 2px;" HtmlTag="Div" CssClass="ts-initialize"  runat="server">
                <Content>
                    <usc:uscContattiSel ID="uscRecipientContact" ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false"
                        ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="false" ButtonSelectOChartVisible="false"
                        ButtonSelectVisible="false" EnableCheck="True" EnableViewState="true" HeaderVisible="false" IsRequired="false"
                        TreeViewCaption="Utente destinatario" UseAD="true" runat="server" UseSessionStorage="true" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="dsw-panel-title ts-initialize" Style="margin-bottom: 2px;" ID="lblUploadDocumentId">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblUploadDocument" runat="server" Font-Bold="True">Inserisci documento</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" HtmlTag="Div" CssClass="ts-initialize" ID="lrUploadDocument" runat="server">
                <Content>
                      <usc:uscUploadDocumentRest MultipleUploadEnabled="false" ID="uscUploadDocumentRest" runat="server" UseSessionStorage="true" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="lblChainTypeRow" CssClass="dsw-panel-title ts-initialize" Style="margin-bottom: 2px;display:none">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblChainType" runat="server" Font-Bold="True">Elenco degli inserti selezionati, specificare come gestire il tipo documento</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="chainTypeRow" CssClass="ts-initialize"  Style="margin-bottom: 2px;display:none">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" Style="margin-bottom: 10px;">
                        <telerik:RadGrid ID="rgvDocumentLists" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                            <MasterTableView TableLayout="Auto" ClientDataKeyNames="ArchiveDocumentId" AllowFilteringByColumn="false" GridLines="Both">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="DocumentName" UniqueName="DocumentName" HeaderText="Nome documento">
                                        <HeaderStyle HorizontalAlign="Left" Width="60%"  />
                                        <ItemStyle HorizontalAlign="Left" />
                                      </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Tipologia di documento"  Visible="true" DataField="ChainType" UniqueName="ChainType">
                                        <HeaderStyle HorizontalAlign="Left" Width="40%" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ClientItemTemplate>
                                            <input type="radio" Id="#=ArchiveDocumentId#_1" name="#=ArchiveDocumentId#_chainTypes" value="1">Principale
                                            <input type="radio" Id="#=ArchiveDocumentId#_2" name="#=ArchiveDocumentId#_chainTypes" value="2">Allegati
                                            <input type="radio" Id="#=ArchiveDocumentId#_4" name="#=ArchiveDocumentId#_chainTypes" value="4">Annessi
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="false" />
                            </ClientSettings>
                        </telerik:RadGrid>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="dataRow" CssClass="dsw-panel-title" Style="margin-bottom: 2px;">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblDatas" runat="server" Font-Bold="True">Dati</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px">
                <Columns>
                    <telerik:LayoutColumn Span="3">
                            <asp:Label ID="lblPriority" runat="server" Font-Bold="True">Priorità:</asp:Label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rblPriority">
                                <asp:ListItem Text="Normale" Value="0" Selected="True" />
                                <asp:ListItem Text="Bassa" Value="1" />
                                <asp:ListItem Text="Alta" Value="2" />
                            </asp:RadioButtonList>
                        </asp:Panel>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="3">
                        <asp:Label ID="lblDueDate" runat="server" Font-Bold="True">Data di scadenza:</asp:Label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <telerik:RadDatePicker ID="dueDate" runat="server"></telerik:RadDatePicker>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="12">
                        <asp:Label ID="lblSubject" runat="server" Font-Bold="True">Oggetto:</asp:Label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 2px;margin-top: 2px">
                        <telerik:RadTextBox ID="txtObject" MaxLength="256" TextMode="MultiLine" onBlur="javascript:ChangeStrWithValidCharacter(this);" Rows="3" runat="server" Width="100%" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:RequiredFieldValidator runat="server" ID="ctrlTxtWf" ControlToValidate="txtObject"
                            ErrorMessage="E' obbligatorio inserire le oggetto" Display="Dynamic"></asp:RequiredFieldValidator>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <telerik:RadAjaxPanel runat="server" CssClass="window-footer-wrapper">
        <telerik:RadButton ID="btnConfirm" runat="server" CausesValidation="false" Text="Conferma" AutoPostBack="false" />
    </telerik:RadAjaxPanel>

</asp:Panel>
