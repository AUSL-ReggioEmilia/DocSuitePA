<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascSummary.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascSummary" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">        
        var uscFascSummary;
        require(["UserControl/uscFascSummary"], function (UscFascSummary) {
            $(function () {
                uscFascSummary = new UscFascSummary(tenantModelConfiguration.serviceConfiguration);
                uscFascSummary.lblTitleId = "<%= lblTitle.ClientID %>";
                uscFascSummary.lblFascicleTypeId = "<%= lblFascicleType.ClientID %>";
                uscFascSummary.lblStartDateId = "<%= lblStartDate.ClientID %>";
                uscFascSummary.lblEndDateId = "<%= lblEndDate.ClientID %>";
                uscFascSummary.lblFascicleObjectId = "<%= lblFascicleObject.ClientID %>";
                uscFascSummary.lblFascicleNoteId = "<%= lblFascicleNote.ClientID %>";
                uscFascSummary.pageId = "<%= pageContent.ClientID %>";
                uscFascSummary.isEditPage = JSON.parse("<%= IsEditPage %>".toLowerCase());
                uscFascSummary.isAuthorizePage = JSON.parse("<%= IsAuthorizePage %>".toLowerCase());
                uscFascSummary.isSummaryLink = JSON.parse("<%= IsSummaryLink %>".toLowerCase());
                uscFascSummary.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscFascSummary.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascSummary.btnExpandFascInfoId = "<%=btnExpandFascInfo.ClientID%>";
                uscFascSummary.fascInfoId = "<%=fascInfo.ClientID%>";
                uscFascSummary.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";                
                uscFascSummary.lblRegistrationUserId = "<%= lblRegistrationUser.ClientID %>";
                uscFascSummary.lblLastChangedUserId = "<%=lblLastChangedUser.ClientID%>";
                uscFascSummary.lblLastChangedDateId = "<%= lblLastChangedDate.ClientID %>";
                uscFascSummary.lblRegistrationDateId = "<%= lblRegistrationDate.ClientID %>";              
                uscFascSummary.workflowActivityId = "<%= CurrentWorkflowActivityId%>";
                uscFascSummary.currentFascicleId = "<%= CurrentFascicleId %>"
                uscFascSummary.lblViewFascicleId = "<%=lblViewFascicle.ClientID%>";
                uscFascSummary.fascCaptionId = "<%=fascCaption.ClientID%>";
                uscFascSummary.containerRowId = "<%= containerRow.ClientID %>";
                uscFascSummary.lblContainerId = "<%= lblContainer.ClientID %>";
                uscFascSummary.serieLabelId = "<%= lblSerieName.ClientID %>";
                uscFascSummary.fascicleContainerEnabled = <%= ProtocolEnv.FascicleContainerEnabled.ToString().ToLower() %>;
                uscFascSummary.processEnabled = <%= ProtocolEnv.ProcessEnabled.ToString().ToLower() %>;
                uscFascSummary.serieLabelRowId = "<%= lblSerieNameRow.ClientID %>";
                uscFascSummary.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title" id="fascCaption" runat="server">
                        Fascicolo
                        <telerik:RadButton ID="btnExpandFascInfo" CssClass="dsw-vertical-middle" runat="server" Width="16px" Height="16px" Visible="true">
                            <Image EnableImageButton="true" />
                        </telerik:RadButton>
                    </div>
                    <div class="dsw-panel-content" id="fascInfo" runat="server">
                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Numero:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblTitle" runat="server"></asp:Label>
                                             <asp:HyperLink runat="server" ID="lblViewFascicle"></asp:HyperLink>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                            <b>Tipo:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblFascicleType" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>          
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Data apertura:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                            <b>Data chiusura:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>      
                                <telerik:LayoutRow HtmlTag="Div" ID="lblSerieNameRow" Style="display: none;"> 
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Serie:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblSerieName" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>      
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Oggetto:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblFascicleObject" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Note:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblFascicleNote" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>                           
                                
                                <telerik:LayoutRow HtmlTag="Div">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Creato da:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="3" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblRegistrationUser" runat="server"></asp:Label>
                                            <asp:Label ID="lblRegistrationDate" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                            <b>Modificato da:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblLastChangedUser" runat="server"></asp:Label>
                                            <asp:Label ID="lblLastChangedDate" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow>   
                                <telerik:LayoutRow HtmlTag="Div" runat="server" ID="containerRow">
                                    <Columns>
                                        <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                            <b>Contenitore:</b>
                                        </telerik:LayoutColumn>
                                        <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                            <asp:Label ID="lblContainer" runat="server"></asp:Label>
                                        </telerik:LayoutColumn>
                                    </Columns>
                                </telerik:LayoutRow> 
                            </Rows>
                        </telerik:RadPageLayout>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>       
    </Rows>
</telerik:RadPageLayout>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
