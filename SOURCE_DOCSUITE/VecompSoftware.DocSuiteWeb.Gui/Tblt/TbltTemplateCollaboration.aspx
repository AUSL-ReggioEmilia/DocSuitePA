<%@ Page Title="Template di collaborazione - risultati" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltTemplateCollaboration.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTemplateCollaboration" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscTemplateCollaborationRest.ascx" TagName="uscTemplateCollaborationRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltTemplateCollaboration;
            require(["Tblt/TbltTemplateCollaboration"], function (TbltTemplateCollaboration) {
                $(function () {
                    tbltTemplateCollaboration = new TbltTemplateCollaboration(tenantModelConfiguration.serviceConfiguration);
                    tbltTemplateCollaboration.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltTemplateCollaboration.folderToolbarId = "<%= FolderToolBar.ClientID %>";
                    tbltTemplateCollaboration.filterToolbarId = "<%= filterToolbar.ClientID %>";
                    tbltTemplateCollaboration.uscTemplateCollaborationRestId = "<%= uscTemplateCollaborationRest.MainPanel.ClientID %>";
                    tbltTemplateCollaboration.templateCollaborationDetailsPaneId = "<%= templateCollaborationDetailsPane.ClientID %>";
                    tbltTemplateCollaboration.queryViewNotActive = "<%= ViewNotActive %>";
                    tbltTemplateCollaboration.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" Height="100%">
        <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="Y" ID="Pane1">
            <telerik:RadToolBar AutoPostBack="False" ID="filterToolbar" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton Value="selectStatus">
                        <ItemTemplate>
                            <telerik:RadComboBox runat="server" ID="cmbTemplateStatus" Width="100%" AutoPostBack="False">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Tutti" Value="-1" Selected="true" />
                                    <telerik:RadComboBoxItem Text="Bozza" Value="0" />
                                    <telerik:RadComboBoxItem Text="Attive" Value="1" />
                                    <telerik:RadComboBoxItem Text="Non attive" Value="2" />
                                </Items>
                            </telerik:RadComboBox>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                   <%-- <telerik:RadToolBarButton Value="searchInput">
                        <ItemTemplate>
                            <telerik:RadTextBox ID="txtSearch" Placeholder="Cerca..." runat="server" AutoPostBack="False" Width="200px"></telerik:RadTextBox>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>--%>
                    <%--<telerik:RadToolBarButton  Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" Value="search" PostBack="false" />--%>
                </Items>
            </telerik:RadToolBar>

            <telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%" RenderMode="Lightweight">
                <Items>
                    <telerik:RadToolBarButton Value="createFolder" ToolTip="Aggiungi cartella" CheckOnClick="false" Checked="false" Text="Aggiungi cartella" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                    <telerik:RadToolBarButton Value="createTemplate" ToolTip="Aggiungi template" CheckOnClick="false" Checked="false" Text="Aggiungi template" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder_add.png" />
                    <%--<telerik:RadToolBarButton Value="delete" ToolTip="Elimina" CheckOnClick="false" Checked="false" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />--%>
                </Items>
            </telerik:RadToolBar>

            <usc:uscTemplateCollaborationRest runat="server" ID="uscTemplateCollaborationRest" />

        </telerik:RadPane>

        <telerik:RadSplitBar runat="server" ID="Bar1" />

        <telerik:RadPane runat="server" ID="templateCollaborationDetailsPane" ShowContentDuringLoad="false">
            <%--<usc:uscProcessDetails runat="server" ID="uscTemplateCollaborationDetails" />--%>
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
