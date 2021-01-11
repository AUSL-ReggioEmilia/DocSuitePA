<%@ Page Title="Collegamenti Fascicolo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="FascicleLink.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascicleLink" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscCategory" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscFascSummary.ascx" TagName="uscFascSummary" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscFascicleSearch.ascx" TagName="uscFascicleSearch" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscDossierSummary.ascx" TagName="uscDossierSummary" TagPrefix="usc1" %>
<%@ Register Src="../UserControl/uscDossierFolders.ascx" TagName="uscDossierFolder" TagPrefix="usc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var fascicleLink;
            require(["Fasc/FascicleLink"], function (FascicleLink) {
                fascicleLink = new FascicleLink(tenantModelConfiguration.serviceConfiguration);
                fascicleLink.currentFascicleId = "<%= IdFascicle %>";
                fascicleLink.btnLinkId = "<%= btnLink.ClientID %>";
                fascicleLink.btnRemoveId = "<%= btnRemove.ClientID %>";
                fascicleLink.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                fascicleLink.pageContentId = "<%= pageContent.ClientID %>";
                fascicleLink.rgvLinkedFasciclesId = "<%= rgvLinkedFascicles.ClientID %>";
                fascicleLink.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                fascicleLink.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                fascicleLink.btnLinkUniqueId = "<%= btnLink.UniqueID %>";
                fascicleLink.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                fascicleLink.uscFascSummaryId = "<%=uscFascSummary.PageContentDiv.ClientID%>";
                fascicleLink.uscFascicleSearchId = "<%= uscFascicleSearch.PageControl.ClientID %>";
                fascicleLink.rtsFascicleLinkId = "<%= rtsFascicleLink.ClientID %>";
                fascicleLink.radWindowManagerFascicleLink = "<%=RadWindowManagerFascicleLink.ClientID %>";
                fascicleLink.btnSearchDossierId = "<%=btnSearchDossier.ClientID%>";

                fascicleLink.lblDossierModifiedUserId = "<%=uscFascSummary.PageContentDiv.ClientID%>";
                fascicleLink.rgvLinkedDossiersId = "<%=rgvLinkedDossiers.ClientID%>";

                fascicleLink.fascicoliCollegatiId ="<%=fascicoliCollegati.ClientID%>";
                fascicleLink.dossierDisponibiliId ="<%=dossierDisponibili.ClientID%>";
                fascicleLink.dossierCollegatiId = "<%=dossierCollegati.ClientID%>";
                fascicleLink.uscDossierSummaryId = "<%=uscDossierSummary.PageContentDiv.ClientID%>";
                fascicleLink.uscDossierFoldersId = "<%= uscDossierFolder.PageContentDiv.ClientID %>";
                fascicleLink.dossierFolderCotainerId = "<%=dossierFolderCotainer.ClientID%>";
                fascicleLink.dossierSummaryContainerId = "<%=dossierSummaryContainer.ClientID%>";

                fascicleLink.initialize();
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerFascicleLink" runat="server">
        <Windows>
            <telerik:RadWindow Height="450" ID="windowOpenDossierRicerca" runat="server" Title="Dossier - Ricerca" Width="600" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
    <telerik:RadPageLayout ID="pageContent" HtmlTag="Div" Width="100%" Height="100%" runat="server">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Content>
                                            <uc1:uscFascSummary ID="uscFascSummary" runat="server" />
                                        </Content>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Gestione collegamenti
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadSplitter runat="server" ID="splContent" ResizeWithParentPane="false" Height="100%" Width="100%" Orientation="Horizontal">
                                <telerik:RadPane runat="server" Height="40px" Scrolling="None" ID="radPane">
                                    <telerik:RadTabStrip ID="rtsFascicleLink" RenderMode="Lightweight" runat="server" MultiPageID="rmpPages" Skin="Silk" Align="left">
                                        <Tabs>
                                            <telerik:RadTab Text="Dossier" Width="150px" Value="Dossier" />
                                            <telerik:RadTab Text="Fascicoli" Width="150px" Value="Fascicoli" />
                                        </Tabs>
                                    </telerik:RadTabStrip>
                                </telerik:RadPane>
                            </telerik:RadSplitter>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <usc:uscFascicleSearch runat="server" ID="uscFascicleSearch"></usc:uscFascicleSearch>
                </Content>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel" id="fascicoliCollegati" runat="server" style="display: none;">
                        <div class="dsw-panel-title">
                            Fascicoli collegati
                        </div>
                        <div runat="server" class="dsw-panel-content">
                            <telerik:RadGrid runat="server" ID="rgvLinkedFascicles" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" Skin="Office2010Blue" AllowMultiRowSelection="false" AllowFilteringByColumn="False">
                                <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="UniqueId" NoMasterRecordsText="Nessun fascicolo collegato" NoDetailRecordsText="Nessun fascicolo collegato">
                                    <Columns>
                                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn"></telerik:GridClientSelectColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="20px" HeaderText="Stato" AllowFiltering="false" UniqueName="colOpenClose">
                                            <FilterTemplate>
                                                <telerik:RadComboBox runat="server" ID="cmbOpenClose" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="True">
                                                </telerik:RadComboBox>
                                            </FilterTemplate>
                                            <ClientItemTemplate>
                                                 <center>
                                                    <img src="#= ImageUrl #" title="#= OpenCloseTooltip #" align="middle"></img>
                                                 </center>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="20px" HeaderText="Tipo" AllowFiltering="false" UniqueName="colFascicleType">
                                            <ClientItemTemplate>
                                                 <center>
                                                    <img src="#= FascicleTypeImageUrl #" title="#= FascicleTypeToolTip #" align="middle"></img>
                                                 </center>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="60%" HeaderText="Fascicolo" AllowFiltering="false">
                                            <FilterTemplate>
                                                <telerik:RadTextBox runat="server" ID="txtObject" DataTextField="Text" DataValueField="Value" Width="30%" AutoPostBack="True">
                                                </telerik:RadTextBox>
                                            </FilterTemplate>
                                            <ClientItemTemplate>
                                                <a href="../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=#= UniqueId #" class="ctrl">#= Name #</a>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Classificazione" AllowFiltering="false" UniqueName="colCategory">
                                            <ClientItemTemplate>
                                                <label>#= Category #</label>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings EnableRowHoverStyle="False">
                                    <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                                </ClientSettings>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel" id="dossierDisponibili" runat="server" style="display: none;">
                        <div class="dsw-panel-title" style="padding: 5px">
                            <label>
                                Dossier disponibili
                            </label>
                            <telerik:RadButton ID="btnSearchDossier" Style="margin-left: 5px;" runat="server" Text="Cerca" AutoPostBack="false" CausesValidation="false" />
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Columns>
                        <telerik:LayoutColumn Span="8" CssClass="details-column" Style="padding-left: 5px; padding-right: 5px;">
                            <asp:Panel runat="server" ID="dossierSummaryContainer" Style="display: none;">
                                <usc1:uscDossierSummary runat="server" ID="uscDossierSummary"></usc1:uscDossierSummary>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                        <telerik:LayoutColumn Span="4" CssClass="folders-column" Height="100%" Style="padding: 0px 5px 0px;">
                            <asp:Panel runat="server" ID="dossierFolderCotainer" Style="display: none;">
                                <usc:uscDossierFolder ID="uscDossierFolder" ViewOnlyFolders="true" runat="server"></usc:uscDossierFolder>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel" id="dossierCollegati" runat="server" style="display: none;">
                        <div class="dsw-panel-title">
                            Dossier collegati
                        </div>
                        <div runat="server" class="dsw-panel-content">
                            <telerik:RadGrid runat="server" ID="rgvLinkedDossiers" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" Skin="Office2010Blue" AllowMultiRowSelection="false" AllowFilteringByColumn="False">
                                <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="UniqueId" NoMasterRecordsText="Nessun dossier collegato" NoDetailRecordsText="Nessun dossier collegato">
                                    <Columns>
                                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn"></telerik:GridClientSelectColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="25%" HeaderText="Dossier" AllowFiltering="false">
                                            <FilterTemplate>
                                                <telerik:RadTextBox runat="server" ID="txtObject" DataTextField="Text" DataValueField="Value" Width="30%" AutoPostBack="True">
                                                </telerik:RadTextBox>
                                            </FilterTemplate>
                                            <ClientItemTemplate>
                                                <a href="../Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier=#= UniqueId #" class="ctrl">#= DossierName #</a>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="25%" HeaderText="Data apertura" AllowFiltering="false">
                                            <ClientItemTemplate>
                                                <label>#= StartDate #</label>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="25%" HeaderText="Contenitori" AllowFiltering="false">
                                            <ClientItemTemplate>
                                                <label>#= Contenitori #</label>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="25%" HeaderText="Oggetto" AllowFiltering="false">
                                            <ClientItemTemplate>
                                                <label>#= Subject #</label>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderStyle-Width="40%" HeaderText="Classificazione" AllowFiltering="false" UniqueName="colCategory">
                                            <ClientItemTemplate>
                                                <label>#= Category #</label>
                                            </ClientItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings EnableRowHoverStyle="False">
                                    <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                                </ClientSettings>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnLink" runat="server" Text="Collega"></telerik:RadButton>
    <telerik:RadButton ID="btnRemove" runat="server" Text="Rimuovi"></telerik:RadButton>
</asp:Content>
