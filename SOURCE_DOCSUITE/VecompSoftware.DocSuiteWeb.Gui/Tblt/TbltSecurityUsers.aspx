<%@ Page Title="Gestione utenti" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltSecurityUsers.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSecurityUsers" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">

            var tbltSecurityUsers;
            require(["Tblt/TbltSecurityUsers"], function (TbltSecurityUsers) {
                $(function () {
                    tbltSecurityUsers = new TbltSecurityUsers();
                    tbltSecurityUsers.radWindowManagerGroupsId = "<%= RadWindowManagerGroups.ClientID %>";
                    tbltSecurityUsers.rtvUsersId = "<%= rtvUsers.ClientID %>";
                    tbltSecurityUsers.rtvGroupsId = "<%= rtvGroups.ClientID %>";
                    tbltSecurityUsers.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltSecurityUsers.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    tbltSecurityUsers.actionsToolbarId = "<%= ActionsToolbar.ClientID %>";
                    tbltSecurityUsers.initialize();
                });
            });

        </script>

         <style>
            #ctl00_cphContent_rtvUsers {
                height: 90%;
            }
        </style>

    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerGroups" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowGroupsAdd" runat="server" Title="Assegna gruppi" />
            <telerik:RadWindow ID="windowCopyFromUser" runat="server" Title="Copia da utente" />
            <telerik:RadWindow ID="windowUserAdd" runat="server" Title="Aggiungi utente" />
        </Windows>
    </telerik:RadWindowManager>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" ResizeWithParentPane="False" Height="100%">
            <telerik:RadPane runat="server" ID="paneSelection" Width="50%" Height="100%" Scrolling="None">
                        <%--OnButtonClick=""--%>
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchAccount">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtAccount" EmptyMessage="Nome" runat="server" Width="170px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Value="domainOptions">
                                    <ItemTemplate>
                                        <telerik:RadDropDownList ID="rcbDomain" runat="server" Width="160px"></telerik:RadDropDownList>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadToolBar AutoPostBack="false" 
                                            CssClass="ToolBarContainer" 
                                            RenderMode="Lightweight" 
                                            EnableRoundedCorners="False"    
                                            EnableShadows="False"   
                                            ID="ActionsToolbar" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton ID="btnAddUser"
                                    runat="server"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png"
                                    AutoPostBack="false"
                                    CommandName="AddUser"
                                    Tooltip="Aggiungi utente"/>
                                <telerik:RadToolBarButton ID="btnDeleteUser"
                                    runat="server"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png"
                                    AutoPostBack="false"
                                    CommandName="DeleteUser"
                                    Tooltip="Elimina utente"/>
                                <telerik:RadToolBarButton ID="btnGroupsAdd"
                                    runat="server"
                                    ImageUrl="~/Comm/Images/Interop/Gruppo.gif"
                                    AutoPostBack="false"
                                    CommandName="AddGroups"
                                    Tooltip="Aggiungi gruppi"
                                    Style="display:none;"/>
                                <telerik:RadToolBarButton ID="btnCopyFromUser"
                                    runat="server"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/document_copies.png"
                                    AutoPostBack="false"
                                    CommandName="CopyFromUser"
                                    Tooltip="Copia da utente"
                                    Style="display:none;"/>
                                <telerik:RadToolBarButton ID="btnGuidedGroupsAdd"
                                    runat="server"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png"
                                    AutoPostBack="false"
                                    CommandName="GuidedGroupsAdd"
                                    Tooltip="Aggiungere gruppi guidati"
                                    Style="display:none;"/>
                                <telerik:RadToolBarButton ID="btnDelete"
                                    runat="server"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png"
                                    AutoPostBack="false"
                                    CommandName="Delete"
                                    Tooltip="Elimina"
                                    Style="display:none;"/>
                                <telerik:RadToolBarButton ID="btnPrivacy"
                                    runat="server"
                                    AutoPostBack="false"
                                    CommandName="Privacy"
                                    Visible="false"/>
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="rtvUsers" runat="server" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" Selected="true" runat="server" Font-Bold="true" Text="Utenti" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>

            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>

            <telerik:RadPane runat="server" Width="50%">
                <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="margin-top: 5px;">
                    <div class="dsw-panel-content">
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbGroups">
                            <Items>
                                <telerik:RadPanelItem Value="rpiGroups" Text="Gruppi" Expanded="true" runat="server">
                                    <ContentTemplate>
                                        <telerik:RadTreeView ID="rtvGroups" MultipleSelect="true" CheckBoxes="true" runat="server">
                                        </telerik:RadTreeView>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                 <%-- Contenitori--%>
                                <telerik:RadPanelItem Text="Contenitori" Expanded="true">
                                    <ContentTemplate>
                                        <telerik:RadGrid runat="server" ID="grdContainers" AutoGenerateColumns="False" Style="margin-top: 2px;" GridLines="none" ItemStyle-BackColor="LightGray">
                                            <MasterTableView Width="100%" DataKeyNames="Id" NoMasterRecordsText="Nessun contenitore presente">
                                                <DetailTables>
                                                    <telerik:GridTableView DataKeyNames="GroupName" Name="Groups" runat="server" Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px">
                                                        <DetailTables>
                                                            <telerik:GridTableView Name="Rights" runat="server" Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px">
                                                                <Columns>
                                                                    <telerik:GridBoundColumn DataField="DocumentType" UniqueName="DocumentType" ItemStyle-Width="40"></telerik:GridBoundColumn>
                                                                    <telerik:GridBoundColumn DataField="Rights" UniqueName="Rights" ItemStyle-Width="60"></telerik:GridBoundColumn>
                                                                </Columns>
                                                            </telerik:GridTableView>
                                                        </DetailTables>

                                                        <Columns>
                                                            <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                                <ItemTemplate>
                                                                    <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png" />
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridBoundColumn HeaderText="Gruppo" UniqueName="GroupName" DataField="GroupName" />
                                                        </Columns>
                                                    </telerik:GridTableView>
                                                </DetailTables>
                                                <Columns>
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                        <ItemTemplate>
                                                            <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open.png" />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn HeaderText="Nome Contenitore" UniqueName="Name" DataField="Name" />
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                 <%-- Settori--%>
                                <telerik:RadPanelItem Text="Settori" Expanded="true">
                                    <ContentTemplate>
                                        <telerik:RadGrid runat="server" ID="grdRoles" AutoGenerateColumns="False" Style="margin-top: 2px;" GridLines="none" ItemStyle-BackColor="LightGray" expa>
                                            <MasterTableView Width="100%" DataKeyNames="Id" NoMasterRecordsText="Nessun settore presente">
                                                 <DetailTables>
                                                    <telerik:GridTableView DataKeyNames="GroupName" Name="Groups" runat="server" Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px">
                                                        <DetailTables>
                                                            <telerik:GridTableView Name="Rights" runat="server" Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px">
                                                                <Columns>
                                                                    <telerik:GridBoundColumn DataField="DocumentType" UniqueName="DocumentType" ItemStyle-Width="40"></telerik:GridBoundColumn>
                                                                    <telerik:GridBoundColumn DataField="Rights" UniqueName="Rights" ItemStyle-Width="60"></telerik:GridBoundColumn>
                                                                </Columns>
                                                            </telerik:GridTableView>
                                                        </DetailTables>

                                                        <Columns>
                                                            <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                                <ItemTemplate>
                                                                    <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png" />
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridBoundColumn HeaderText="Gruppo" UniqueName="GroupName" DataField="GroupName" />
                                                        </Columns>
                                                    </telerik:GridTableView>
                                                </DetailTables>
                                                <Columns>
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                        <ItemTemplate>
                                                            <asp:Image runat="server" ID="roleImage" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick.png" />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn HeaderText="Nome Settore" UniqueName="Name" DataField="Name" />
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
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
