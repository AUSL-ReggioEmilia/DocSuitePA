<%@ Page Language="vb" Title="Gestione serie e volumi" AutoEventWireup="false" CodeBehind="TbltProcess.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltProcess" EnableViewState="True" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProcessDetails.ascx" TagName="uscProcessDetails" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltProcess;
            require(["Tblt/TbltProcess"], function (TbltProcess) {
                $(function () {
                    tbltProcess = new TbltProcess(tenantModelConfiguration.serviceConfiguration);
                    tbltProcess.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltProcess.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltProcess.processViewPaneId = "<%= Pane1.ClientID %>";
                    tbltProcess.processDetailsPaneId = "<%= Pane2.ClientID %>";
                    tbltProcess.rtvProcessesId = "<%= rtvProcesses.ClientID %>";
                    tbltProcess.uscProcessDetailsId = "<%= uscProcessDetails.PanelDetails.ClientID %>";
                    tbltProcess.rwInsertId = "<%= rwInsert.ClientID %>";
                    tbltProcess.folderToolBarId = "<%= FolderToolBar.ClientID %>";
                    tbltProcess.rtbProcessNameId = "<%= rtbProcessName.ClientID %>";
                    tbltProcess.uscCategoryRestId = "<%= uscCategoryRest.MainContent.ClientID %>";
                    tbltProcess.rtbDossierFolderNameId = "<%= rtbDossierFolderName.ClientID %>";
                    tbltProcess.rtbFascicleTemplateNameId = "<%= rtbFascicleTemplateName.ClientID %>";
                    tbltProcess.rbConfirmId = "<%= rbConfirm.ClientID %>";
                    tbltProcess.rcbProcessNoteId = "<%= rcbProcessNote.ClientID %>";
                    tbltProcess.filterToolbarId = "<%= filterToolbar.ClientID %>";
                    tbltProcess.uscProcessRoleRestId = "<%= uscProcessRoleRest.TableContentControl.ClientID %>";
                    tbltProcess.defaultSelectedProcessId = "<%= ProcessId %>";
                    tbltProcess.defaultSelectedProcessCategoryId = "<%= CategoryId %>";
                    tbltProcess.rtbCloneDossierFolderNameId = "<%= rtbCloneDossierFolderName.ClientID%>"
                    tbltProcess.currentTenantAOOId = "<%= CurrentTenant.TenantAOO.UniqueId %>";
                    tbltProcess.initialize();
                });
            });

        </script>
        <style type="text/css">
            #ctl00_cphContent_rwInsert_C_uscCategoryRest_pnlMainContent,
            #ctl00_cphContent_rwInsert_C_uscProcessRoleRest_pnlMainContent {
                width: 100%;
            }

            #ctl00_cphContent_rtvProcesses {
                height: 90%;
            }
        </style>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <telerik:RadWindow runat="server" ID="rwInsert" Behaviors="Maximize,Close,Move" Width="750px" Height="450px">
        <ContentTemplate>
            <div id="insertProcess">
                <asp:Panel ID="pnlAggiungi" runat="server">
                    <table class="dataform">
                        <tr>
                            <td class="label">Nome della serie:</td>
                            <td>
                                <telerik:RadTextBox ID="rtbProcessName" MaxLength="256" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="rtbProcessName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Note:</td>
                            <td>
                                <telerik:RadTextBox ID="rcbProcessNote" MaxLength="100" runat="server" />
                            </td>
                        </tr>
                    </table>

                    <usc:uscCategoryRest runat="server" ID="uscCategoryRest" />
                    <hr />
                    <usc:uscRoleRest runat="server" ID="uscProcessRoleRest" ReadOnlyMode="false" MultipleRoles="true"
                        Required="true" RequiredMessage="Campo Settori responsabili obligatorio" Collapsable="true" Caption="Settori responsabili" />
                </asp:Panel>
            </div>
            <div id="insertDossierFolder">
                <table class="dataform">
                    <tr>
                        <td class="label">Nome del volume:</td>
                        <td>
                            <telerik:RadTextBox ID="rtbDossierFolderName" MaxLength="256" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="rtbDossierFolderName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="RequiredFieldValidator2" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="insertFascicleTemplate">
                <table class="dataform">
                    <tr>
                        <td class="label">Nome del template di fascicolo:</td>
                        <td>
                            <telerik:RadTextBox ID="rtbFascicleTemplateName" MaxLength="256" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="rtbFascicleTemplateName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="RequiredFieldValidator3" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
             <div id="cloneDossierFolder">
                <table class="dataform">
                    <tr>
                        <td class="label">Serie e volumi:</td>
                        <td>
                            <telerik:RadTextBox ID="rtbCloneDossierFolderName" MaxLength="256" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="rtbCloneDossierFolderName" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ID="RequiredFieldValidator4" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="background: #f5f5f5;">
                <telerik:RadButton runat="server" ID="rbConfirm" Text="Conferma" Style="margin: 0.5em;" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" Height="100%">
        <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None" ID="Pane1">
            <telerik:RadToolBar AutoPostBack="False" ID="filterToolbar" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton Value="searchInput">
                        <ItemTemplate>
                            <telerik:RadTextBox ID="txtSearch" Placeholder="Cerca..." runat="server" AutoPostBack="False" Width="200px"></telerik:RadTextBox>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton Text="Disattivi" CheckOnClick="true" Group="Disabled" Checked="false" Value="processDisabled" PostBack="false"
                        AllowSelfUnCheck="true">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton Text="Attivi" CheckOnClick="true" Checked="false" Group="Active" Value="processActive" PostBack="false"
                        AllowSelfUnCheck="true">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true" />
                    <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" Value="search" PostBack="false" />
                    <telerik:RadToolBarButton IsSeparator="true" />
                </Items>
            </telerik:RadToolBar>

            <telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%" RenderMode="Lightweight">
                <Items>
                    <telerik:RadToolBarButton ToolTip="Aggiungi" CheckOnClick="false" Checked="false" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                    <telerik:RadToolBarButton ToolTip="Aggiungi modello di fascicolo" CheckOnClick="false" Checked="false" Text="Fascicolo" Value="createProcessFascicleTemplate" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder_add.png" />
                    <telerik:RadToolBarButton ToolTip="Modifica" CheckOnClick="false" Checked="false" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                    <telerik:RadToolBarButton ToolTip="Elimina" CheckOnClick="false" Checked="false" Value="delete" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
                    <telerik:RadToolBarButton ToolTip="Clona" CheckOnClick="false" Checked="false" Value="clone" Text="Clona" ImageUrl="~/App_Themes/DocSuite2008/imgset16/clone.png" />
                    <telerik:RadToolBarButton ToolTip="Copia" CheckOnClick="false" Checked="false" Value="copyPFT" Text="Copia in" ImageUrl="~/App_Themes/DocSuite2008/imgset16/document_copies.png" />
                    <telerik:RadToolBarButton ToolTip="Incolla" CheckOnClick="false" Checked="false" Value="pastePFT" Text="Incolla" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder_add.png" />
                </Items>
            </telerik:RadToolBar>
            <telerik:RadTreeView ID="rtvProcesses" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
                <Nodes>
                    <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Classificatore" Value="" />
                </Nodes>
            </telerik:RadTreeView>
        </telerik:RadPane>

        <telerik:RadSplitBar runat="server" ID="Bar1" />

        <telerik:RadPane runat="server" ID="Pane2">
            <usc:uscProcessDetails runat="server" ID="uscProcessDetails" />
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
