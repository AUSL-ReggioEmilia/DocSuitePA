<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscStartWorkflow.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscStartWorkflow" %>

<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="oggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDomainUserSelRest.ascx" TagName="uscDomainUserSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscUploadDocumentRest.ascx" TagName="uscUploadDocumentRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscTenantsSelRest.ascx" TagName="uscTenantsSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscWorkflowFolderSelRest.ascx" TagName="uscWorkflowFolderSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscTemplateCollaborationSelRest.ascx" TagName="uscTemplateCollaborationSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleUserSelRest.ascx" TagName="uscRoleUserSelRest" TagPrefix="usc" %>

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
                uscStartWorkflow.rdlCCDocumentId = "<%=rdlCCDocument.UniqueID%>";
                uscStartWorkflow.copiaConformeRowId = "<%=copiaConformeRow.ClientID%>";

                uscStartWorkflow.proposerRoleRowId = "<%=proposerRoleRow.ClientID%>";
                uscStartWorkflow.proposerContactRowId = "<%=proposerContactRow.ClientID%>";
                uscStartWorkflow.proposerContactRowLabelId = "<%=proposerContactRowLabel.ClientID%>";
                uscStartWorkflow.tenantRowId = "<%=tenantRow.ClientID%>";

                uscStartWorkflow.recipientRoleRowId = "<%= recipientRoleRow.ClientID%>";
                uscStartWorkflow.recipientContactRowId = "<%=recipientContactRow.ClientID%>";
                uscStartWorkflow.recipientContactRowLabelId = "<%=recipientContactRowLabel.ClientID%>";

                uscStartWorkflow.uploadDocumentId = "<%=lrUploadDocument.ClientID%>";
                uscStartWorkflow.lblUploadDocumentId = "<%=lblUploadDocumentId.ClientID%>";
                uscStartWorkflow.uscDocumentId = "<%= uscUploadDocumentRest.uploadDocumentComponent.ClientID %>";

                uscStartWorkflow.lblTemplateCollaborationRowId = "<%= lblTemplateCollaborationRow.ClientID %>";
                uscStartWorkflow.ddlTemplateCollaborationRowId = "<%= ddlTemplateCollaborationRow.ClientID %>";

                uscStartWorkflow.lblChainTypeRowId = "<%= lblChainTypeRow.ClientID %>";
                uscStartWorkflow.chainTypeRowId = "<%= chainTypeRow.ClientID %>";
                uscStartWorkflow.rgvDocumentListsId = "<%= rgvDocumentLists.ClientID %>";

                uscStartWorkflow.docSuiteVersion = "<%=DSWVersion%>";
                uscStartWorkflow.showOnlyNoInstanceWorkflows = <%=ShowOnlyNoInstanceWorkflows.ToString().ToLower()%>;
                uscStartWorkflow.showOnlyHasIsFascicleClosedRequired = <%=ShowOnlyHasIsFascicleClosedRequired.ToString().ToLower()%>;

                uscStartWorkflow.tenantAOOId = "<%= BasePage.CurrentTenant.TenantAOO.UniqueId %>";
                uscStartWorkflow.uscTenantsSelRestId = "<%=uscTenantsSelRest.PageContent.ClientID%>";

                uscStartWorkflow.uscWorkflowFolderSelRestId = "<%=uscWorkflowFolderSelRest.TableContentControl.ClientID%>";
                uscStartWorkflow.lrUscWorkflowFolderSelRestId = "<%=lrUscWorkflowFolderSelRest.ClientID%>";
                uscStartWorkflow.lblDossierTitleId = "<%=lblDossierTitle.ClientID%>";

                uscStartWorkflow.uscRoleProposerRestId = "<%=uscRoleProposerRest.TableContentControl.ClientID%>";
                uscStartWorkflow.uscRoleRecipientRestId = "<%=uscRoleRecipientRest.TableContentControl.ClientID%>";
                uscStartWorkflow.uscRecipientContactRestId = "<%=uscRecipientContactRest.PageContent.ClientID%>";
                uscStartWorkflow.uscProposerContactRestId = "<%=uscProposerContactRest.PageContent.ClientID%>";
                uscStartWorkflow.uscTemplateCollaborationSelRestId = "<%= uscTemplateCollaborationSelRest.MainPanel.ClientID %>";
                uscStartWorkflow.signalRServerAddress = "<%= SignalRServerAddress %>";
                uscStartWorkflow.radListMessagesId = ("<%= radListMessages.ClientID %>");
                uscStartWorkflow.pnlWorkflowId = "<%= pnlWorkflow.ClientID %>";
                uscStartWorkflow.pnlNotificationMessagesId = "<%= pnlNotificationMessages.ClientID %>";
                uscStartWorkflow.uscRoleUserSelRestId = "<%=uscRoleUserSelRest.PageContent.ClientID%>";
                uscStartWorkflow.roleUserSelRowId = "<%=roleUserSelRow.ClientID%>";
                uscStartWorkflow.initialize();
            });
        });

    </script>
    <style>
        .RadTreeView_Office2007 .rtPlus, .RadTreeView_Office2007 .rtMinus,
        .RadTreeView .rtLines .rtLI, .RadTreeView .rtLines .rtLI div {
            background: none !important;
        }

        .RadTreeView .rtPlus, .RadTreeView .rtMinus {
            left: 20px;
        }

        div.RadTreeView_Office2007 .rtSelected .rtIn {
            color: black !important;
        }
    </style>
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
                        <usc:uscTemplateCollaborationSelRest runat="server" ID="uscTemplateCollaborationSelRest" TreeViewInitializationEnabled="False" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" CssClass="ts-initialize" ID="proposerRoleRow">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <usc:uscRoleRest runat="server" ID="uscRoleProposerRest" Expanded="true" Caption="Settore richiedente" Required="false" OnlyMyRoles="true" MultipleRoles="false" UseSessionStorage="true" />
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px; margin-left: 0.3em;" CssClass="ts-initialize" ID="tenantRow">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" CssClass="content-wrapper" Span="12" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server">
                            <usc:uscTenantsSelRest runat="server" ID="uscTenantsSelRest" MultiselectionEnabled="false" />
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
                    <usc:uscDomainUserSelRest ID="uscProposerContactRest" runat="server" TreeViewCaption="Utente proponente" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" CssClass="ts-initialize" ID="recipientRoleRow">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <usc:uscRoleRest runat="server" ID="uscRoleRecipientRest" Caption="Settore destinatario" Required="false" MultipleRoles="false" UseSessionStorage="true" />
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
            <telerik:LayoutRow ID="recipientContactRow" Style="margin-bottom: 2px;" HtmlTag="Div" CssClass="ts-initialize" runat="server">
                <Content>
                    <usc:uscDomainUserSelRest ID="uscRecipientContactRest" runat="server" TreeViewCaption="Utente destinatario" />
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
            <telerik:LayoutRow ID="lblChainTypeRow" CssClass="dsw-panel-title ts-initialize" Style="margin-bottom: 2px; display: none">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblChainType" runat="server" Font-Bold="True">Elenco degli inserti selezionati, specificare come gestire il tipo documento</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="chainTypeRow" CssClass="ts-initialize" Style="margin-bottom: 2px; display: none">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" Style="margin-bottom: 10px;">
                        <telerik:RadGrid ID="rgvDocumentLists" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                            <MasterTableView TableLayout="Auto" ClientDataKeyNames="ArchiveDocumentId" AllowFilteringByColumn="false" GridLines="Both">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="DocumentName" UniqueName="DocumentName" HeaderText="Nome documento">
                                        <HeaderStyle HorizontalAlign="Left" Width="60%" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Tipologia di documento" Visible="true" DataField="ChainType" UniqueName="ChainType">
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

            <telerik:LayoutRow CssClass="dsw-panel-title ts-initialize" Style="margin-bottom: 2px;" ID="lblDossierTitle" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblDossier" runat="server" Font-Bold="True">Dossier</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px;" HtmlTag="Div" CssClass="ts-initialize" ID="lrUscWorkflowFolderSelRest" runat="server">
                <Content>
                    <usc:uscWorkflowFolderSelRest ID="uscWorkflowFolderSelRest" runat="server" UseSessionStorage="true" Required="false" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px; margin-left: 0.3em;" CssClass="ts-initialize" ID="roleUserSelRow">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" CssClass="content-wrapper" Span="12" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server">
                            <usc:uscRoleUserSelRest runat="server" ID="uscRoleUserSelRest" />
                        </asp:Panel>
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
            <telerik:LayoutRow ID="copiaConformeRow" Style="margin-bottom: 2px; display: none;">
                <Columns>
                    <telerik:LayoutColumn Span="7">
                        <asp:Label runat="server" Font-Bold="True">Inviare il file in originale o in copia conforme?</asp:Label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rdlCCDocument">
                                <asp:ListItem Text="Copia conforme" Value="0" Selected="True" />
                                <asp:ListItem Text="Originale" Value="1" />
                            </asp:RadioButtonList>
                        </asp:Panel>
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
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 2px; margin-top: 2px">
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

    <telerik:RadPageLayout runat="server" widht="100%" ID="pnlNotificationMessages" CssClass="dsw-panel">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>
                    <telerik:RadListBox RenderMode="Lightweight" ID="radListMessages" runat="server" Height="98%" Width="98%" SelectionMode="Single" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <telerik:RadAjaxPanel runat="server" CssClass="window-footer-wrapper">
        <telerik:RadButton ID="btnConfirm" runat="server" CausesValidation="false" Text="Conferma" AutoPostBack="false" />
    </telerik:RadAjaxPanel>

</asp:Panel>
