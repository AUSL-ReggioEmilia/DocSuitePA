<%@ Page Title="Collegamenti Fascicolo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="FascicleLink.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascicleLink" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscCategory" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscFascSummary.ascx" TagName="uscFascSummary" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscFascicleSearch.ascx" TagName="uscFascicleSearch" TagPrefix="usc" %>

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
                fascicleLink.initialize();
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
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
                    <usc:uscFascicleSearch runat="server" ID="uscFascicleSearch"></usc:uscFascicleSearch>
                </Content>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
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
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnLink" runat="server" Text="Collega"></telerik:RadButton>
    <telerik:RadButton ID="btnRemove" runat="server" Text="Rimuovi"></telerik:RadButton>
</asp:Content>
