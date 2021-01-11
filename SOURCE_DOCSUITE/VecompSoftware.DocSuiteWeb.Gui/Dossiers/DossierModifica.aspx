<%@ Page Title="Dossier - Modifica" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierModifica.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierModifica" %>

<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="uscOggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataRest.ascx" TagName="uscDynamicMetadataRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSetiContactSel.ascx" TagName="uscSetiContactSel" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var dossierModifica;
            require(["Dossiers/DossierModifica"], function (DossierModifica) {
                $(function () {
                    dossierModifica = new DossierModifica(tenantModelConfiguration.serviceConfiguration);
                    dossierModifica.currentDossierId = "<%= IdDossier %>";
                    dossierModifica.dossierPageContentId = "<%= dossierPageContent.ClientID %>";
                    dossierModifica.btnConfirmId = "<%= btnConferma.ClientID %>";
                    dossierModifica.btnConfirmUniqueId = "<%= btnConferma.UniqueID %>";
                    dossierModifica.lblStartDateId = "<%= lblStartDate.ClientID%>";
                    dossierModifica.lblYearId = "<%= lblYear.ClientID%>";
                    dossierModifica.lblNumberId = "<%= lblNumber.ClientID%>";
                    dossierModifica.lblContainerId = "<%=lblContainer.ClientID%>";
                    dossierModifica.txtObjectId = "<%= uscObject.TextBoxControl.ClientID %>";
                    dossierModifica.txtNoteId = "<%= txtNote.ClientID %>";
                    dossierModifica.rdpStartDateId = "<%=rdpStartDate.ClientID %>";
                    dossierModifica.rfvStartDateId = "<%= rfvStartDate.ClientID %>";
                    dossierModifica.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    dossierModifica.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierModifica.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    dossierModifica.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierModifica.uscDynamicMetadataId = "<%= UscDynamicMetadataRest.PageContent.ClientID%>";
                    dossierModifica.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                    dossierModifica.rowMetadataId = "<%= rowMetadata.ClientID%>";
                    dossierModifica.setiContactEnabledId = <%=ProtocolEnv.SETIIntegrationEnabled.ToString().ToLower()%>;
                    dossierModifica.uscSetiContactSelId = "<%= uscSetiContact.PageContentDiv.ClientID%>";
                    dossierModifica.uscContattiSelRestId = "<%=uscContattiSelRest.PanelContent.ClientID%>";
                    dossierModifica.rcbDossierStatusId = "<%= rcbDossierStatus.ClientID %>";
                    dossierModifica.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadPageLayout runat="server" ID="dossierPageContent" Width="100%" Height="100%" HtmlTag="Div">
        <Rows>
            <%-- Sezione Dossier (Anno/Numero/Data apertura /chiusura) --%>
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

            <%-- Sezione dati modificabili Oggetto--%>
            <telerik:LayoutRow ID="rowObject">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Oggetto
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Oggetto:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <usc:uscOggetto ID="uscObject" Required="True" RequiredMessage="Campo Oggetto Obbligatorio" MultiLine="True" runat="server" Type="Dossier" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Note:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <%-- Sezione Contenitore --%>
            <telerik:LayoutRow>
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Contenitore
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                <b>Contenitore:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding">
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
            <%-- Sezione Riferimento --%>
            <telerik:LayoutRow ID="contattiRespRow" HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Riferimenti
                        </div>

                        <div class="dsw-panel-content">
                            <usc:uscContattiSelRest ID="uscContattiSelRest" runat="server" Required="false" />
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <%-- Sezione Start Date--%>
            <telerik:LayoutRow ID="rowGeneral">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Data
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Data Apertura:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                <telerik:RadDatePicker ID="rdpStartDate" runat="server" />
                                                <asp:RequiredFieldValidator ControlToValidate="rdpStartDate" Display="Dynamic" ErrorMessage="Data apertura obbligatoria" ID="rfvStartDate" runat="server" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="rowDossierStatus">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Stato
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Stato da dossier:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                <telerik:RadComboBox ID="rcbDossierStatus" runat="server" />
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
                        <div class="dsw-panel-content">
                            <div style="margin-left: 16%;">                              
                                <usc:uscSetiContactSel runat="server" ID="uscSetiContact" />
                            </div>
                            <usc:uscDynamicMetadataRest runat="server" ID="UscDynamicMetadataRest"></usc:uscDynamicMetadataRest>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConferma" runat="server" CausesValidation="true" AutoPostBack="false" Width="150px" Text="Conferma" />
</asp:Content>
