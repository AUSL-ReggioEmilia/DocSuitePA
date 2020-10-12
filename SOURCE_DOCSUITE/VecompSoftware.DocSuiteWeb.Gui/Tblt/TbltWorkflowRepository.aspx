<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltWorkflowRepository" CodeBehind="TbltWorkflowRepository.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Gestione attività di workflow" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltWorkflowRepository;
            require(["Tblt/TbltWorkflowRepository"], function (TbltWorkflowRepository) {
                $(function () {
                    tbltWorkflowRepository = new TbltWorkflowRepository(tenantModelConfiguration.serviceConfiguration);
                    tbltWorkflowRepository.windowAddWorkflowRoleMappingId = "<%= windowAddWorkflowRoleMapping.ClientID %>";
                    tbltWorkflowRepository.rtvWorkflowRepositoryId = "<%= rtvWorkflowRepository.ClientID %>";
                    tbltWorkflowRepository.rgvWorkflowRoleMappingsId = "<%= rgvWorkflowRoleMappings.ClientID %>";
                    tbltWorkflowRepository.rgvXamlWorkflowRoleMappingsId = "<%= rgvXamlWorkflowRoleMappings.ClientID %>";
                    tbltWorkflowRepository.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltWorkflowRepository.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    tbltWorkflowRepository.btnAggiungiId = "<%= btnAggiungi.ClientID %>";
                    tbltWorkflowRepository.btnModificaId = "<%= btnModifica.ClientID %>";
                    tbltWorkflowRepository.btnEliminaId = "<%= btnElimina.ClientID %>";
                    tbltWorkflowRepository.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                    tbltWorkflowRepository.lblVersionId = "<%= lblVersion.ClientID %>";
                    tbltWorkflowRepository.lblActiveFromId = "<%= lblActiveFrom.ClientID %>";
                    tbltWorkflowRepository.lblActiveToId = "<%= lblActiveTo.ClientID %>";
                    tbltWorkflowRepository.lblStatusId = "<%= lblStatus.ClientID %>";
                    tbltWorkflowRepository.lblTipoligiaId = "<%= lblTipoligia.ClientID %>";
                    tbltWorkflowRepository.lblPositionId = "<%= lblPosition.ClientID %>";
                    tbltWorkflowRepository.lblStepNameId = "<%= lblStepName.ClientID %>";
                    tbltWorkflowRepository.lblAutorizationTypeId = "<%= lblAutorizationType.ClientID %>";
                    tbltWorkflowRepository.lblActivityTypeId = "<%= lblActivityType.ClientID %>";
                    tbltWorkflowRepository.lblAreaId = "<%= lblArea.ClientID %>";
                    tbltWorkflowRepository.lblActionId = "<%= lblAction.ClientID %>";
                    tbltWorkflowRepository.pnlRepositoryInformationsId = "<%= pnlInformations.ClientID %>";
                    tbltWorkflowRepository.pnlStepInformationsId = "<%= pnlStepInformations.ClientID %>";
                    tbltWorkflowRepository.pnlBarDetailsId = "<% = pnlBarDetails.ClientID%>";

                    tbltWorkflowRepository.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltWorkflowRepository.pnlSelectMappingTagId = "<%= pnlSelectMappingTag.ClientID %>";
                    tbltWorkflowRepository.rcbSelectMappingTagId = "<%= rcbSelectMappingTag.ClientID %>";
                    tbltWorkflowRepository.btnSelectMappingTagId = "<%= btnSelectMappingTag.ClientID %>";
                    tbltWorkflowRepository.mappingDataSourceId = "<%= mappingDataSource.ClientID %>";
                    tbltWorkflowRepository.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";

                    tbltWorkflowRepository.ToolBarSearchId = "<%= ToolBarSearch.ClientID %>";

                    tbltWorkflowRepository.ToolBarStepId = "<%= ToolBarStep.ClientID %>";
                    tbltWorkflowRepository.rwmWorkflowStepId = "<%= RadWindowManagerWorkflowStep.ClientID%>";
                    tbltWorkflowRepository.rwWorkflowPropertyId = "<%= RadWindowManagerWorkflowProperty.ClientID %>";
                    tbltWorkflowRepository.rwWorkflowRepositoryId = "<%= RadWindowManagerWorkflowRepository.ClientID%>";

                    tbltWorkflowRepository.btnAddId = "<%= btnAdd.ClientID %>";
                    tbltWorkflowRepository.btnEditId = "<%= btnEdit.ClientID %>";
                    tbltWorkflowRepository.btnDeleteId = "<%= btnDelete.ClientID %>";

                    tbltWorkflowRepository.btnAddInputArgumentId = "<%= btnAddInputArgument.ClientID%>";
                    tbltWorkflowRepository.btnEditInputArgumentId = "<%= btnEditInputArgument.ClientID%>";
                    tbltWorkflowRepository.btnDeleteInputArgumentId = "<%= btnDeleteInputArgument.ClientID%>";

                    tbltWorkflowRepository.btnAddEvaluationArgumentId = "<%= btnAddEvaluationArgument.ClientID%>";
                    tbltWorkflowRepository.btnEditEvaluationArgumentId = "<%= btnEditEvaluationArgument.ClientID%>";
                    tbltWorkflowRepository.btnDeleteEvaluationArgumentId = "<%= btnDeleteEvaluationArgument.ClientID%>";

                    tbltWorkflowRepository.btnAddOutputArgumentId = "<%= btnAddOutputArgument.ClientID%>";
                    tbltWorkflowRepository.btnEditOutputArgumentId = "<%= btnEditOutputArgument.ClientID%>";
                    tbltWorkflowRepository.btnDeleteOutputArgumentId = "<%= btnDeleteOutputArgument.ClientID%>";

                    tbltWorkflowRepository.rgvWorkflowStartUpId = "<%= rgvWorkflowStartUp.ClientID%>";
                    tbltWorkflowRepository.rgvStepInputPropertiesId = "<%= rgvStepInputProperties.ClientID%>";
                    tbltWorkflowRepository.rgvStepEvaluationPropertiesId = "<%= rgvStepEvaluationProperties.ClientID %>";
                    tbltWorkflowRepository.rgvStepOutputPropertiesId = "<%= rgvStepOutputProperties.ClientID%>";

                    tbltWorkflowRepository.uscRoleRestId = "<%= uscRole.TableContentControl.ClientID%>";
                    tbltWorkflowRepository.pnlSelectRolesId = "<%= pnlRoleRest.ClientID%>";

                    tbltWorkflowRepository.initialize();
                });
            });

        </script>
        <style>
    .remove .rwTable {
        height: 170px !important;
    }

    #RadWindowWrapper_ctl00_cphContent_windowAddWorkflowRoleMapping,
    .RadWindow .rwTable {
        height: 500px !important;
    }
    </style>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerWorkflowRepository" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwWorkflowRepository" runat="server" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerWorkflowRoleMappings" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowAddWorkflowRoleMapping" runat="server" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerWorkflowStep" runat="server">
        <Windows>
            <telerik:RadWindow runat="server" ID="rwAddWorkflowStep" Width="650" Height="400" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerWorkflowProperty" runat="server">
        <Windows>
            <telerik:RadWindow runat="server" ID="rwWorkflowProperty" Width="650" Height="250"></telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" ResizeWithParentPane="False" Height="100%">
            <telerik:RadPane runat="server" ID="paneSelection" Width="50%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Height="100%" Width="100%" Orientation="Horizontal">
                    <telerik:RadPane runat="server" Width="100%" Height="70px" Scrolling="None" CssClass="dsw-panel">
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchName">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtName" runat="server" Width="170px" AutoPostBack="False"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>

                        <telerik:RadToolBar runat="server" ID="ToolBarStep" CssClass="ToolBarContainer"
                            RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton runat="server" CommandName="ADD" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" Enabled="false" />
                                <telerik:RadToolBarButton runat="server" CommandName="EDIT" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" Enabled="false" />
                                <telerik:RadToolBarButton runat="server" CommandName="REMOVE" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" Enabled="false" />
                            </Items>
                        </telerik:RadToolBar>
                    </telerik:RadPane>

                    <telerik:RadPane runat="server" Height="100%">
                        <div style="        height: 99%;" class="elementBordered">
                            <telerik:RadTreeView ID="rtvWorkflowRepository" runat="server" Height="100%">
                            </telerik:RadTreeView>
                        </div>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
            <telerik:RadPane runat="server" Width="50%">
                <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="        margin-top: 5px;
        visibility: hidden;">
                    <div class="dsw-panel-content">
                        <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
                        <telerik:RadPanelBar runat="server" ID="pnlBarDetails" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                            <Items>
                                <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlInformations">
                                            <div class="col-dsw-10">
                                                <b>Versione:</b>
                                                <asp:Label runat="server" ID="lblVersion"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Data di inizio:</b>
                                                <asp:Label runat="server" ID="lblActiveFrom"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Data di fine:</b>
                                                <asp:Label runat="server" ID="lblActiveTo"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Stato:</b>
                                                <asp:Label runat="server" ID="lblStatus"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Tipologia archivistica: </b>
                                                <asp:Label runat="server" ID="lblTipoligia"></asp:Label>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlStepInformations">
                                            <div class="col-dsw-10">
                                                <b>Posizione attività: </b>
                                                <asp:Label runat="server" ID="lblPosition"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Nome attività: </b>
                                                <asp:Label runat="server" ID="lblStepName"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Tipo autorizzazione: </b>
                                                <asp:Label runat="server" ID="lblAutorizationType"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Tipo attività: </b>
                                                <asp:Label runat="server" ID="lblActivityType"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Area workflow: </b>
                                                <asp:Label runat="server" ID="lblArea"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Azione workflow: </b>
                                                <asp:Label runat="server" ID="lblAction"></asp:Label>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                <telerik:RadPanelItem Text="Tag" Expanded="true" Value="Tag">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlTag">
                                            <div class="radGridWrapper" id="workflowRoleMappings">
                                                <telerik:RadGrid ID="rgvWorkflowRoleMappings" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                                                    <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both">
                                                        <Columns>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                                                            </telerik:GridClientSelectColumn>
                                                            <telerik:GridTemplateColumn UniqueName="MappingTag" HeaderText="Tag" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                            <div class="dsw-text-left">      
                                                                <img class="dsw-vertical-middle" src="../App_Themes/DocSuite2008/imgset16/Tag_16x.png"></img>                                          
                                                                <span>#=MappingTag#</span>
                                                            </div>                                            
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Role" HeaderText="Settore" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                <div class="dsw-text-left">                                                
                                                                    <img class="dsw-vertical-middle" src="..\App_Themes\DocSuite2008\imgset16\bricks.png" style="        display: #=Role != undefined ? 'inline-block' : 'none' #"></img>
                                                                    <span>#=Role != undefined ? Role.Name : ''#</span>
                                                                </div>                                            
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="AuthorizationType" HeaderText="Tipologia Autorizzazione" AllowFiltering="false" Groupable="false" AllowSorting="True">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" Height="40px" />
                                                                <ClientItemTemplate>
                                                                 <span>#=tbltWorkflowRepository.getAuthorizationTypeDescription(AuthorizationType)#</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings>
                                                        <Selecting AllowRowSelect="true" />
                                                    </ClientSettings>
                                                </telerik:RadGrid>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlButtons">
                                            <telerik:RadButton ID="btnAggiungi" AutoPostBack="false" runat="server" Text="Aggiungi" />
                                            <telerik:RadButton ID="btnModifica" AutoPostBack="false" runat="server" Text="Modifica" />
                                            <telerik:RadButton ID="btnElimina" AutoPostBack="false" runat="server" Text="Elimina" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                <telerik:RadPanelItem Text="Proprietà di avvio" Expanded="true" Value="Startup">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlStartup">
                                            <div class="radGridWrapper" id="workflowStartup">
                                                <telerik:RadGrid ID="rgvWorkflowStartUp" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                                                    <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both">
                                                        <Columns>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                                                            </telerik:GridClientSelectColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <div class="dsw-text-left">      
                                                                        <span>#=tbltWorkflowRepository.getNameDescription(Name)#</span>
                                                                    </div>  
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Value" HeaderText="Valore " AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <span>#=Value#</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings>
                                                        <Selecting AllowRowSelect="true" />
                                                    </ClientSettings>
                                                </telerik:RadGrid>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlButtonsStartUp">
                                            <telerik:RadButton ID="btnAdd" AutoPostBack="false" runat="server" Text="Aggiungi" />
                                            <telerik:RadButton ID="btnEdit" AutoPostBack="false" runat="server" Text="Modifica" />
                                            <telerik:RadButton ID="btnDelete" AutoPostBack="false" runat="server" Text="Elimina" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                <telerik:RadPanelItem Text="Proprietà in ingresso" Expanded="true" Value="Input">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlInputProperties">
                                            <div class="radGridWrapper" id="stepInputProperties">
                                                <telerik:RadGrid ID="rgvStepInputProperties" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                                                    <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both">
                                                        <Columns>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                                                            </telerik:GridClientSelectColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <div class="dsw-text-left">      
                                                                        <span>#=tbltWorkflowRepository.getNameDescription(Name)#</span>
                                                                    </div>  
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Value" HeaderText="Valore " AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <span>#=Value#</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings>
                                                        <Selecting AllowRowSelect="true" />
                                                    </ClientSettings>
                                                </telerik:RadGrid>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlButtonsInputArguments">
                                            <telerik:RadButton ID="btnAddInputArgument" AutoPostBack="false" runat="server" Text="Aggiungi" />
                                            <telerik:RadButton ID="btnEditInputArgument" AutoPostBack="false" runat="server" Text="Modifica" />
                                            <telerik:RadButton ID="btnDeleteInputArgument" AutoPostBack="false" runat="server" Text="Elimina" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                <telerik:RadPanelItem Text="Proprietà di valutazione step" Expanded="true" Value="Evaluation">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlEvaluationProperties">
                                            <div class="radGridWrapper" id="stepEvaluationProperties">
                                                <telerik:RadGrid ID="rgvStepEvaluationProperties" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                                                    <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both">
                                                        <Columns>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                                                            </telerik:GridClientSelectColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <div class="dsw-text-left">      
                                                                        <span>#=tbltWorkflowRepository.getNameDescription(Name)#</span>
                                                                    </div>  
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Value" HeaderText="Valore " AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <span>#=Value#</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings>
                                                        <Selecting AllowRowSelect="true" />
                                                    </ClientSettings>
                                                </telerik:RadGrid>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlButtonsEvaluationArguments">
                                            <telerik:RadButton ID="btnAddEvaluationArgument" AutoPostBack="false" runat="server" Text="Aggiungi" />
                                            <telerik:RadButton ID="btnEditEvaluationArgument" AutoPostBack="false" runat="server" Text="Modifica" />
                                            <telerik:RadButton ID="btnDeleteEvaluationArgument" AutoPostBack="false" runat="server" Text="Elimina" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                                <telerik:RadPanelItem Text="Proprietà in uscita" Expanded="true" Value="Output">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlOutputProperties">
                                            <div class="radGridWrapper" id="stepOutputProperties">
                                                <telerik:RadGrid ID="rgvStepOutputProperties" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                                                    <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both">
                                                        <Columns>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                                                            </telerik:GridClientSelectColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <div class="dsw-text-left">      
                                                                        <span>#=tbltWorkflowRepository.getNameDescription(Name)#</span>
                                                                    </div>  
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Value" HeaderText="Valore " AllowFiltering="false" AllowSorting="true" Groupable="false">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ClientItemTemplate>
                                                                     <span>#=Value#</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings>
                                                        <Selecting AllowRowSelect="true" />
                                                    </ClientSettings>
                                                </telerik:RadGrid>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="pnlButtonsOutputArguments">
                                            <telerik:RadButton ID="btnAddOutputArgument" AutoPostBack="false" runat="server" Text="Aggiungi" />
                                            <telerik:RadButton ID="btnEditOutputArgument" AutoPostBack="false" runat="server" Text="Modifica" />
                                            <telerik:RadButton ID="btnDeleteOutputArgument" AutoPostBack="false" runat="server" Text="Elimina" />
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>

                            </Items>
                        </telerik:RadPanelBar>
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" ID="pnlSelectMappingTag" Width="100%">
                            <Items>
                                <telerik:RadPanelItem Text="Gestisci Tag" Expanded="true">
                                    <ContentTemplate>
                                        <div class="dsw-display-inline-block" style="        padding: 2px;">
                                            <telerik:RadClientDataSource runat="server" ID="mappingDataSource" />
                                            <telerik:RadComboBox Filter="Contains" AllowCustomText="true" ID="rcbSelectMappingTag" runat="server" Width="250"
                                                ClientDataSourceID="mappingDataSource" DataTextField="MappingTag" AutoPostBack="false" EnableLoadOnDemand="true" EmptyMessage="Seleziona o inserisci un Tag" />
                                            <telerik:RadButton runat="server" ID="btnSelectMappingTag" Style="        vertical-align: middle !important;" AutoPostBack="false" Text="Seleziona"></telerik:RadButton>
                                        </div>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>

                    </div>
                    <asp:Panel runat="server" ID="pnlRoleRest">
                        <div class="dsw-display-inline-block" style="        padding: 2px;
        width: 100%;">
                            <usc:uscRoleRest Caption="Settori autorizzati all'avvio del workflow"
                                MultipleRoles="true"
                                ID="uscRole"
                                ReadOnlyMode="false"
                                runat="server"></usc:uscRoleRest>
                        </div>
                    </asp:Panel>
                    <div class="radGridWrapper" id="xamlWorkflowRoleMappings">
                        <telerik:RadGrid ID="rgvXamlWorkflowRoleMappings" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
                            <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="Id" AllowFilteringByColumn="false" GridLines="Both">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn UniqueName="ActivityName" HeaderText="Nome Activity" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ClientItemTemplate>
                                            <div class="dsw-text-left">      
                                                <img class="dsw-vertical-middle" src="../App_Themes/DocSuite2008/imgset16/Activity_16x.png"></img>                                          
                                                <b>#=Activity != undefined ? Activity.Name : ''#</b>
                                            </div>                                            
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="MappingTag" HeaderText="Tag" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ClientItemTemplate>
                                            <div class="dsw-text-left">
                                                <img class="dsw-vertical-middle" src="../App_Themes/DocSuite2008/imgset16/Tag_16x.png"></img>
                                                <span>#=MappingTag#</span>
                                            </div>                                            
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="Role" HeaderText="Settore" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ClientItemTemplate>
                                            <div class="dsw-text-left">                                                
                                                <img class="dsw-vertical-middle" src="../App_Themes/DocSuite2008/imgset16/bricks.png" style="        display: #=WorkflowRoleMapping != undefined && WorkflowRoleMapping.Role != undefined ? 'inline-block' : 'none' #"></img>
                                                <span>#=WorkflowRoleMapping != undefined &&  WorkflowRoleMapping.Role != undefined ? WorkflowRoleMapping.Role.Name : ''#</span>
                                            </div>                                            
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="AccountName" HeaderText="Nome utente" AllowFiltering="false" AllowSorting="true" Groupable="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ClientItemTemplate>
                                                <span>#=WorkflowRoleMapping.AccountName != undefined ? WorkflowRoleMapping.AccountName : ''#</span>                                  
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="AuthorizationType" HeaderText="Tipologia Autorizzazione" AllowFiltering="false" Groupable="false" AllowSorting="True">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Height="40px" />
                                        <ClientItemTemplate>
                                            <span>#=tbltWorkflowRepository.getAuthorizationTypeDescription(WorkflowRoleMapping.AuthorizationType)#</span>
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </asp:Panel>

            </telerik:RadPane>

        </telerik:RadSplitter>
    </div>
</asp:Content>


