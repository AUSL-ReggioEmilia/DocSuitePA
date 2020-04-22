<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDossier.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDossier" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataSummaryClient.ascx" TagName="uscDynamicMetadataSummaryClient" TagPrefix="uc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscDossier;
        require(["UserControl/uscDossier"], function (UscDossier) {
            $(function () {
                uscDossier = new UscDossier(tenantModelConfiguration.serviceConfiguration);
                uscDossier.lblDossierSubjectId = "<%= lblDossierSubject.ClientID%>";
                uscDossier.lblRegistrationUserId = "<%= lblRegistrationUser.ClientID %>";
                uscDossier.lblStartDateId = "<%= lblStartDate.ClientID%>";
                uscDossier.lblDossierNoteId = "<%= lblDossierNote.ClientID%>";
                uscDossier.pageId = "<%= pageContent.ClientID%>";
                uscDossier.lblYearId = "<%= lblYear.ClientID%>";
                uscDossier.lblNumberId = "<%= lblNumber.ClientID%>";
                uscDossier.lblModifiedUserId = "<%= lblModifiedUser.ClientID%>";
                uscDossier.lblContainerId = "<%= lblContainer.ClientID%>";
                uscDossier.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscDossier.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscDossier.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscDossier.lblWorkflowHandlerUserId = "<%= lblWorkflowHandlerUser.ClientID %>";
                uscDossier.lblWorkflowProposerRoleId = "<%= lblWorkflowProposerRole.ClientID %>";
                uscDossier.rowWorkflowProposerId = "<%= rowWorkflowProposer.ClientID %>";
                uscDossier.workflowActivityId = "<%= CurrentWorkflowActivityId%>";
                uscDossier.uscDynamicMetadataSummaryClientId = "<%= uscDynamicMetadataSummaryClient.PageContent.ClientID%>";
                uscDossier.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                uscDossier.rowMetadataId = "<%= rowMetadata.ClientID%>";
                uscDossier.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>


<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Dossier
                    </div>
                    <div class="dsw-panel-content">
                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Anno:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblYear" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Numero:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblNumber" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Data Apertura:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Oggetto
                    </div>
                    <div class="dsw-panel-content">
                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Oggetto:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblDossierSubject" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Note:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblDossierNote" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowContact" runat="server" HtmlTag="Div">
            <Content>
                <uc:uscContatti ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="true"
                    ButtonSelectAdamVisible="false" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false"
                    Caption="Riferimento" EnableCompression="true" EnableCC="false" ID="uscContatto" IsRequired="false" Multiple="true"
                    MultiSelect="true" runat="server" Type="Dossier" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowRoles" runat="server">
            <Content>
                <uc:uscSettori ID="uscSettori" ReadOnly="true" runat="server" Required="false" Caption="Settore Responsabile" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowGeneral">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Informazioni
                    </div>
                    <div class="dsw-panel-content">
                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right col-dsw-16-important">
                                            <b>Contenitore:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding col-dsw-34-important">
                                            <asp:Label ID="lblContainer" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow>
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right col-dsw-16-important">
                                            <b>Creato da:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding col-dsw-34-important">
                                            <asp:Label ID="lblRegistrationUser" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow>
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right col-dsw-16-important">
                                            <b>Modificato da:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding col-dsw-34-important">
                                            <asp:Label ID="lblModifiedUser" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div" ID="rowWorkflowProposer" runat="server">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right col-dsw-16-important">
                                            <b>Richiedente flusso:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding col-dsw-34-important">
                                            <asp:Label ID="lblWorkflowProposerRole" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right col-dsw-16-important">
                                            <b>Flusso in carico a:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding col-dsw-34-important">
                                            <asp:Label ID="lblWorkflowHandlerUser" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowMetadata" runat="server">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Metadati
                    </div>
                    <div class="dsw-panel-content" style="width:98%">
                        <uc:uscDynamicMetadataSummaryClient runat="server" ID="uscDynamicMetadataSummaryClient"></uc:uscDynamicMetadataSummaryClient>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowMail" Visible="false">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Messaggi Mail inviati
                    </div>
                    <asp:Repeater ID="rptDossierMessage" runat="server" Visible="false">
                        <HeaderTemplate>
                            <table class="dataform">
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout HtmlTag="Div" runat="server" Width="100%">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Utente:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                                    <asp:Label ID="lblUser" runat="server"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Data:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                                    <asp:Label ID="lblMailDate" runat="server"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Oggetto:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                                    <asp:Label ID="lblMailSubject" runat="server"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Mittente:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                                    <asp:Label ID="lblSender" runat="server"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Destinatari:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                                    <asp:Label ID="lblReceiver" runat="server"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Status:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
