<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDossierSummary.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDossierSummary" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscDossierSummary;
        require(["UserControl/uscDossierSummary"], function (UscDossierSummary) {
            $(function () {
                uscDossierSummary = new UscDossierSummary(tenantModelConfiguration.serviceConfiguration);
                uscDossierSummary.lblDossierSubjectId = "<%= lblDossierSubject.ClientID%>";
                uscDossierSummary.lblStartDateId = "<%= lblStartDate.ClientID%>";
                uscDossierSummary.lblDossierNoteId = "<%= lblDossierNote.ClientID%>";
                uscDossierSummary.pageId = "<%= pageContent.ClientID%>";
                uscDossierSummary.lblYearId = "<%= lblYear.ClientID%>";
                uscDossierSummary.lblNumberId = "<%= lblNumber.ClientID%>";
                uscDossierSummary.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscDossierSummary.lblDossierTypeId = "<%= lblDossierType.ClientID %>";
                uscDossierSummary.lblDossierStatusId = "<%= lblDossierStatus.ClientID %>";
                uscDossierSummary.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <usc:uscerrornotification runat="server" id="uscNotification"></usc:uscerrornotification>
                <div class="dsw-panel">
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
    </Rows>
</telerik:RadPageLayout>
