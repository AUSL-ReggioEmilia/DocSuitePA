<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltPrivacyLevel" CodeBehind="TbltPrivacyLevel.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltPrivacyLevel;
            require(["Tblt/TbltPrivacyLevel"], function (TbltPrivacyLevel) {
                $(function () {
                    tbltPrivacyLevel = new TbltPrivacyLevel(tenantModelConfiguration.serviceConfiguration);
                    tbltPrivacyLevel.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                    tbltPrivacyLevel.pnlInformationsId = "<%= pnlInformations.ClientID %>";
                    tbltPrivacyLevel.toolBarSearchId = "<%= ToolBarSearch.ClientID %>";                    
                    tbltPrivacyLevel.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltPrivacyLevel.rtvLevelsId = "<%= rtvLevels.ClientID %>";
                    tbltPrivacyLevel.btnAddId = "<%= btnAdd.ClientID %>";
                    tbltPrivacyLevel.btnEditId = "<%= btnEdit.ClientID%>";
                    tbltPrivacyLevel.windowAddUDSTypologyId = "<%= windowAddPrivacyLevel.ClientID %>";
                    tbltPrivacyLevel.paneSelectionId = "<%= paneSelection.ClientID %>";
                    tbltPrivacyLevel.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltPrivacyLevel.txtDescriptionId = "<%=txtDescription.ClientID%>";
                    tbltPrivacyLevel.txtLevelId = "<%= txtLevel.ClientID%>";
                    tbltPrivacyLevel.lblActiveFromId = "<%=lblActiveFrom.ClientID%>";
                    tbltPrivacyLevel.btnDeleteId = "<%= btnDelete.ClientID%>";
                    tbltPrivacyLevel.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    tbltPrivacyLevel.lblIsActiveId = "<%= lblIsActive.ClientID%>";
                    tbltPrivacyLevel.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    tbltPrivacyLevel.colorBoxId = "<%= colorBox.ClientID %>";
                    tbltPrivacyLevel.privacyLabel = "<%= PRIVACY_LABEL %>";
                    tbltPrivacyLevel.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerPrivacyLevel" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowAddPrivacyLevel" runat="server" />
        </Windows>
    </telerik:RadWindowManager>


    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" ResizeWithParentPane="False" Height="100%">
            <telerik:RadPane runat="server" ID="paneSelection" Width="40%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Height="100%" Width="100%" Orientation="Horizontal">
                    <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="Y">
                        <%--OnButtonClick=""--%>
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchDescription">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtDescription" EmptyMessage="Livello" runat="server" Width="170px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="rtvLevels" runat="server" Style="margin-top: 10px;" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" Selected="true" runat="server" Font-Bold="true" Text="Livelli" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>

            <telerik:RadPane runat="server" Width="60%">
                <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" >
                    <div class="dsw-panel-content">
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                            <Items>
                                <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlInformations">
                                            <div class="col-dsw-10">
                                                <b>Descrizione:</b>
                                                <asp:Label runat="server" ID="txtDescription"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Livello:</b>
                                                <asp:Label runat="server" ID="txtLevel"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Colore:</b>
                                                <div class="color-box" id="colorBox" runat="server"></div>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Data creazione:</b>
                                                <asp:Label runat="server" ID="lblActiveFrom"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Stato:</b>
                                                <asp:Label runat="server" ID="lblIsActive"></asp:Label>
                                            </div>
                                        </asp:Panel>
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
        <telerik:RadButton ID="btnAdd" runat="server" Text="Aggiungi" AutoPostBack="false" />
        <telerik:RadButton ID="btnEdit" runat="server" Text="Modifica" AutoPostBack="false" />
        <telerik:RadButton ID="btnDelete" runat="server" Text="Elimina" AutoPostBack="false" Visible="false" />
</asp:Content>


