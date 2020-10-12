<%@ Page Language="vb" AutoEventWireup="false" Title="Deposito Documentale" CodeBehind="TbltLocation.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltLocation" EnableViewState="True" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscLocationDetails.ascx" TagName="uscLocationDetails" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltLocation;
            require(["Tblt/TbltLocation"], function (TbltLocation) {
                $(function () {
                    tbltLocation = new TbltLocation(tenantModelConfiguration.serviceConfiguration);
                    tbltLocation.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltLocation.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltLocation.uscLocationDetailsId = "<%= uscLocationDetails.PanelDetails.ClientID%>";
                    tbltLocation.rtvLocationId = "<%=rtvLocation.ClientID%>";
                    tbltLocation.actionToolBarId = "<%=actionToolBar.ClientID%>";
                    tbltLocation.filterToolbarId = "<%=filterToolbar.ClientID%>";
                    tbltLocation.locationViewPaneId = "<%= Pane1.ClientID %>";
                    tbltLocation.rwInsertId = "<%= rwInsert.ClientID %>";
                    tbltLocation.rtbLocationNameId = "<%=rtbLocationName.ClientID%>";
                    tbltLocation.rtbArchivioProtocolloId = "<%=rtbArchivioProtocollo.ClientID%>";
                    tbltLocation.rtbArchivioDossierId = "<%=rtbArchivioDossier.ClientID%>";
                    tbltLocation.rtbArchivioAttiId = "<%=rtbArchivioAtti.ClientID%>";
                    tbltLocation.rbConfirmId = "<%=rbConfirm.ClientID%>";
                    tbltLocation.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadWindow runat="server" ID="rwInsert" Behaviors="Maximize,Close,Move" Width="750px" Height="450px">
        <ContentTemplate>
            <div id="insertLocation">
                <asp:Panel ID="pnlAggiungi" runat="server">
                    <table class="dataform">
                        <tr>
                            <td class="label">Nome della deposito:</td>
                            <td>
                                <telerik:RadTextBox ID="rtbLocationName" MaxLength="256" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="rtbLocationName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Archivio Protocollo:</td>
                            <td>
                                <telerik:RadTextBox ID="rtbArchivioProtocollo" MaxLength="256" runat="server" />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Archivio Dossier:</td>
                            <td>
                                <telerik:RadTextBox ID="rtbArchivioDossier" MaxLength="256" runat="server" />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Archivio Atti:</td>
                            <td>
                                <telerik:RadTextBox ID="rtbArchivioAtti" MaxLength="256" runat="server" />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <div style="background: #f5f5f5;">
                <telerik:RadButton AutoPostBack="false" runat="server" ID="rbConfirm" Text="Conferma" Style="margin: 0.5em;" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" Height="100%" ResizeWithParentPane="False">
        <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None" ID="Pane1">
            <telerik:RadToolBar AutoPostBack="False" ID="filterToolbar" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton Value="searchInput">
                        <ItemTemplate>
                            <telerik:RadTextBox ID="txtSearch" Placeholder="Cerca..." runat="server" AutoPostBack="False" Width="150px"></telerik:RadTextBox>
                            <telerik:RadTextBox ID="txtSearchArchive" Placeholder="Cerca archivio..." runat="server" AutoPostBack="False" Width="150px"></telerik:RadTextBox>
                        </ItemTemplate>
                        </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" Value="search" PostBack="false" />
                    <telerik:RadToolBarButton IsSeparator="true" />
                </Items>
            </telerik:RadToolBar>

            <telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="actionToolBar" runat="server" Width="100%" RenderMode="Lightweight">
                <Items>
                    <telerik:RadToolBarButton ToolTip="Aggiungi" CheckOnClick="false" Checked="false" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                    <telerik:RadToolBarButton ToolTip="Modifica" CheckOnClick="false" Checked="false" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                </Items>
            </telerik:RadToolBar>
            <telerik:RadTreeView ID="rtvLocation" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%" Height="91%">
                <Nodes>
                    <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Deposito Documentale" Value="" />
                </Nodes>
            </telerik:RadTreeView>
        </telerik:RadPane>

        <telerik:RadSplitBar runat="server" ID="Bar1" />

        <telerik:RadPane runat="server" ID="Pane2">
            <usc:uscLocationDetails runat="server" ID="uscLocationDetails" />
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
