<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReslTipologia.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslTipologia" %>

<%@ Register Src="~/UserControl/uscResolutionKindDetails.ascx" TagName="uscResolutionKindDetails" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscResolutionKindSeries.ascx" TagName="uscResolutionKindSeries" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var reslTipologia;
            require(["Resl/ReslTipologia"], function (ReslTipologia) {
                $(function () {
                    reslTipologia = new ReslTipologia(tenantModelConfiguration.serviceConfiguration);
                    reslTipologia.rtvResolutionKindsId = "<%= rtvResolutionKinds.ClientID %>";
                    reslTipologia.tlbStatusSearchId = "<%= tlbStatusSearch.ClientID %>";
                    reslTipologia.btnAddId = "<%= btnAdd.ClientID %>";
                    reslTipologia.btnEditId = "<%= btnEdit.ClientID %>";
                    reslTipologia.btnCancelId = "<%= btnCancel.ClientID %>";
                    reslTipologia.managerWindowsId = "<%= wndManage.ClientID %>";
                    reslTipologia.defaultManagerWindowsId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    reslTipologia.wndResolutionKindId = "<%= wndResolutionKind.ClientID %>";
                    reslTipologia.txtKindNameId = "<%= txtKindName.ClientID %>";
                    reslTipologia.rcbKindActiveId = "<%= rcbKindActive.ClientID %>";
                    reslTipologia.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    reslTipologia.uscResolutionKindDetailsId = "<%= uscResolutionKindDetails.PageContent.ClientID %>";
                    reslTipologia.uscResolutionKindSeriesId = "<%= uscResolutionKindSeries.PageContent.ClientID %>";
                    reslTipologia.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                    reslTipologia.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    reslTipologia.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    reslTipologia.pnlWindowContentId = "<%= pnlWindowContent.ClientID %>";
                    reslTipologia.btnRestoreId = "<%= btnRestore.ClientID %>";
                    reslTipologia.initialize();
                });
            });
        </script>
    </telerik:RadCodeBlock>

</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager ID="wndManage" runat="server">
        <Windows>
            <telerik:RadWindow ID="wndResolutionKind" Behaviors="Close" Height="300px" Width="500px" runat="server">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlWindowContent" CssClass="dsw-panel">
                        <div class="dsw-panel-content">
                            <b>Tipologia:</b>
                            <telerik:RadTextBox runat="server" ID="txtKindName" Style="margin-bottom: 5px;" Width="100%"></telerik:RadTextBox>
                            <asp:CheckBox runat="server" ID="rcbKindActive" Text="Attivo"></asp:CheckBox>
                            <div class="window-footer-wrapper">
                                <telerik:RadButton runat="server" ID="btnConfirm" SingleClick="true" SingleClickText="Attendere..." AutoPostBack="false" Text="Conferma" />
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div runat="server" class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" Orientation="Vertical" ID="splContent">
            <telerik:RadPane runat="server" Width="50%">
                <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" ID="tlbStatusSearch" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton>
                            <ItemTemplate>
                                <label>Stato della Tipologia:</label>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton ID="inActive" Text="Disattivi" CheckOnClick="true" Group="Disabled" Checked="false" Value="searchDisabled"
                            AllowSelfUnCheck="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton ID="active" Text="Attivi" CheckOnClick="true" Checked="true" Group="Active" Value="searchActive"
                            AllowSelfUnCheck="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadTreeView ID="rtvResolutionKinds" LoadingStatusPosition="BeforeNodeText" runat="server" Style="margin-top: 10px;" Width="100%">
                    <Nodes>
                        <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Tipologia atto" Value="" />
                    </Nodes>
                </telerik:RadTreeView>
            </telerik:RadPane>
            <telerik:RadSplitBar runat="server"></telerik:RadSplitBar>
            <telerik:RadPane runat="server" Width="50%">
                <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel">
                    <div class="dsw-panel-content">
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                            <Items>
                                <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                    <ContentTemplate>
                                        <usc:uscResolutionKindDetails runat="server" ID="uscResolutionKindDetails"></usc:uscResolutionKindDetails>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                                <telerik:RadPanelItem Text="Archivi" Expanded="true">
                                    <ContentTemplate>
                                        <usc:uscResolutionKindSeries runat="server" ID="uscResolutionKindSeries"></usc:uscResolutionKindSeries>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </div>
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnAdd" Text="Aggiungi" AutoPostBack="false"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnEdit" Text="Modifica" AutoPostBack="false"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnCancel" Text="Elimina" SingleClick="true" SingleClickText="Attendere..." AutoPostBack="false"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnRestore" Text="Ripristina" SingleClick="true" SingleClickText="Attendere..." AutoPostBack="false"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
