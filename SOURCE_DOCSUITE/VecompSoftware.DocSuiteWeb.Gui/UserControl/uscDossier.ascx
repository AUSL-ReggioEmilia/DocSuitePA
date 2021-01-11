<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDossier.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDossier" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataSummaryRest.ascx" TagName="uscDynamicMetadataSummaryRest" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>

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
                uscDossier.uscDynamicMetadataSummaryRestId = "<%= uscDynamicMetadataSummaryRest.PageContent.ClientID%>";
                uscDossier.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                uscDossier.rowMetadataId = "<%= rowMetadata.ClientID%>";
                uscDossier.uscRoleRestId = "<%=uscRoleRest.TableContentControl.ClientID%>";
                uscDossier.uscResponsableRoleRestId = "<%=uscResponsableRoleRest.TableContentControl.ClientID%>";
                uscDossier.uscContattiSelRestId = "<%=uscContattiSelRest.PanelContent.ClientID%>";
                uscDossier.uscCategoryRestId = "<%= uscCategoryRest.MainContent.ClientID %>";
                uscDossier.lblDossierTypeId = "<%= lblDossierType.ClientID %>";
                uscDossier.lblDossierStatusId = "<%= lblDossierStatus.ClientID %>";
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
                                            <b>Tipologia:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblDossierType" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Stato:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblDossierStatus" runat="server"></asp:Label>
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
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Riferimento
                        </div>

                        <div class="dsw-panel-content">
                            <usc:uscContattiSelRest ID="uscContattiSelRest" runat="server" ToolbarVisible="false" Required="false" />
                        </div>
                    </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowResponsableRoles" runat="server">
            <Content>
                <usc:uscRoleRest runat="server" ID="uscResponsableRoleRest" Caption="Settore responsabile" Required="false" ReadOnlyMode="true"  />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowRoles" runat="server">
            <Content>
                <usc:uscRoleRest runat="server" ID="uscRoleRest" Caption="Settori autorizzati" Required="false" ReadOnlyMode="true"  />
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
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Contenitore:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblContainer" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow>
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Creato da:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblRegistrationUser" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Modificato da:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblModifiedUser" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div" ID="rowWorkflowProposer" runat="server">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Richiedente flusso:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblWorkflowProposerRole" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Flusso in carico a:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
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
        <telerik:LayoutRow ID="rowCategory" runat="server">
            <Content>
                <usc:uscCategoryRest runat="server" ID="uscCategoryRest" IsRequired="false" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowMetadata" runat="server">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Metadati
                    </div>
                    <div class="dsw-panel-content" style="width: 98%">
                        <uc:uscDynamicMetadataSummaryRest runat="server" ID="uscDynamicMetadataSummaryRest"></uc:uscDynamicMetadataSummaryRest>
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
