<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DossierAutorizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierAutorizza" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Dossier - Autorizzazioni" %>

<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var dossierAutorizza;
            require(["Dossiers/DossierAutorizza"], function (DossierAutorizza) {
                $(function () {
                    dossierAutorizza = new DossierAutorizza(tenantModelConfiguration.serviceConfiguration);
                    dossierAutorizza.currentDossierId = "<%= IdDossier %>";
                    dossierAutorizza.dossierPageContentId = "<%= dossierPageContent.ClientID %>";
                    dossierAutorizza.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierAutorizza.lblStartDateId = "<%= lblStartDate.ClientID%>";
                    dossierAutorizza.lblYearId = "<%= lblYear.ClientID%>";
                    dossierAutorizza.lblNumberId = "<%= lblNumber.ClientID%>";
                    dossierAutorizza.uscRoleRestId = "<%=uscRoleRest.TableContentControl.ClientID%>";
                    dossierAutorizza.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierAutorizza.btnConfirmId = "<%= btnConferma.ClientID %>";
                    dossierAutorizza.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadPageLayout runat="server" ID="dossierPageContent" Width="100%" Height="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
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
            
            <%-- Sezione Settori autorizzati--%>
            <telerik:LayoutRow ID="rowRoles" runat="server">
                <Content>
                    <usc:uscRoleRest runat="server" ID="uscRoleRest" Caption="Settori autorizzati" Required="false" MultipleRoles="true"/>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConferma" runat="server" CausesValidation="true" AutoPostBack="false" Width="150px" Text="Conferma" />
</asp:Content>
