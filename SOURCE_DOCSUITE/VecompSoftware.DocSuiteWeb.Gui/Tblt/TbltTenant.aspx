<%@ Page AutoEventWireup="false" CodeBehind="TbltTenant.aspx.vb" EnableViewState="True" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTenant" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione AOO" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagPrefix="usc" TagName="uscContattiSelRest" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <style type="text/css">
            div.RadGrid_Office2007 input {
                background-color: white;
            }
            .RadWindow .rwIcon{
                margin:3px 5px 0 0;
                position: static;
            }
            .RadWindow .rwIcon::before{
                content: unset;
            }
        </style>

        <script type="text/javascript">
            var tbltTenant;
            require(["Tblt/TbltTenant"], function (TbltTenant) {
                $(function () {
                    tbltTenant = new TbltTenant(tenantModelConfiguration.serviceConfiguration);

                    tbltTenant.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    tbltTenant.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltTenant.pnlDetailsId = "<%= pnlDetails.ClientID %>";

                    //rad tree views
                    tbltTenant.rtvTenantsId = "<%= rtvTenants.ClientID %>";
                    tbltTenant.rtvContainersId = "<%= rtvContainers.ClientID %>";
                    tbltTenant.rtvPECMailBoxesId = "<%= rtvPECMailBoxes.ClientID %>";
                    tbltTenant.rtvWorkflowRepositoriesId = "<%= rtvWorkflowRepositories.ClientID %>";
                    tbltTenant.rtvTenantConfigurationsId = "<%= rtvTenantConfigurations.ClientID %>";


                    tbltTenant.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    tbltTenant.splitterMainId = "<%= splitterMain.ClientID %>";
                    tbltTenant.toolBarSearchId = "<%= ToolBarSearch.ClientID %>";

                    // details right zone
                    tbltTenant.lblCompanyNameId = "<%= lblCompanyName.ClientID %>";
                    tbltTenant.lblTenantNameId = "<%= lblTenantName.ClientID %>";
                    tbltTenant.lblTenantNoteId = "<%= lblTenantNote.ClientID %>";
                    tbltTenant.lblTenantDataDiAttivazioneId = "<%= lblTenantDataDiAttivazione.ClientID %>";
                    tbltTenant.lblTenantDataDiDisattivazioneId = "<%= lblTenantDataDiDisattivazione.ClientID %>";

                    //Containers, PECMailBoxes, Rules, WorkflowReposiory, TenantConfiguration, Contact
                    tbltTenant.tbContainersControlId = "<%= tbContainersControl.ClientID%>";
                    tbltTenant.tbPECMailBoxesControlId = "<%= tbPECMailBoxControl.ClientID%>";
                    tbltTenant.tbWorkflowRepositoryControlId = "<%= tbWorkflowControl.ClientID%>";
                    tbltTenant.tbConfigurationControlId = "<%= tbConfigurationControl.ClientID%>";


                    // windows
                    tbltTenant.rwContainerId = "<%=rwContainerSelector.ClientID%>";
                    tbltTenant.rwPECMailBoxId = "<%=rwPECMailBoxSelector.ClientID%>";
                    tbltTenant.rwTenantConfigurationId = "<%=rwTenantConfigurationSelector.ClientID%>";
                    tbltTenant.rwRoleId = "<%=rwRoleSelector.ClientID%>";
                    tbltTenant.rwWorkflowRepositoryId = "<%=rwWorkflowRepositorySelector.ClientID%>";
                    tbltTenant.rwTenantSelectorId = "<%= rwTenantSelector.ClientID %>";


                    //window combos
                    tbltTenant.cmbContainerId = "<%= cbContainer.ClientID %>";
                    tbltTenant.cmbPECMailBoxId = "<%= cbPECMailBoxSelector.ClientID %>";
                    tbltTenant.cmbRoleId = "<%= cbRoleSelector.ClientID %>";
                    tbltTenant.cmbWorkflowRepositoryId = "<%= cbWorkflowRepositorySelector.ClientID %>";
                    tbltTenant.cmbConfigurationTypeId = "<%=cmbConfigurationType.ClientID%>";
                    tbltTenant.cmbTenantWorkflowRepositoryTypeId = "<%= cmbTenantWorkflowRepositoryType.ClientID %>";

                    // window configuration fields
                    tbltTenant.dpStartDateFromId = "<%= dtpDateFrom.ClientID%>";
                    tbltTenant.dpEndDateFromId = "<%= dtpDateTo.ClientID%>";
                    tbltTenant.tenantConfigurationNoteId = "<%= tenantConfigurationNote.ClientID%>";
                    tbltTenant.dpTenantDateFromId = "<%= dpTenantDateFrom.ClientID %>";
                    tbltTenant.dpTenantDateToId = "<%= dpTenantDateTo.ClientID %>";
                    tbltTenant.txtTenantNameId = "<%= txtTenantName.ClientID %>";
                    tbltTenant.txtTenantCompanyId = "<%= txtTenantCompany.ClientID %>";
                    tbltTenant.txtTenantNoteId = "<%= txtTenantNote.ClientID %>";
                    tbltTenant.txtTenantConfigurationJsonValueId = "<%=txtTenantConfigurationJsonValue.ClientID%>";
                    tbltTenant.dpTenantWorkflowRepositoryDateFromId = "<%= dtpTenantWorkflowRepositoryDateFrom.ClientID %>";
                    tbltTenant.dpTenantWorkflowRepositoryDateToId = "<%= dtpTenantWorkflowRepositoryDateTo.ClientID %>";
                    tbltTenant.txtTenantWorkflowRepositoryJsonValueId = "<%= txtTenantWorkflowRepositoryJsonValue.ClientID %>";
                    tbltTenant.txtTenantWorkflowRepositoryIntegrationModuleNameId = "<%= txtTenantWorkflowRepositoryIntegrationModuleName.ClientID %>";
                    tbltTenant.txtTenantWorkflowRepositoryConditionsId = "<%= txtTenantWorkflowRepositoryConditions.ClientID %>";
                    
                    // Window buttons Confirm, Cancel
                    tbltTenant.btnContainerSelectorOkId = "<%= btnContainerSelectorOk.ClientID %>";
                    tbltTenant.btnContainerSelectorCancelId = "<%= btnContainerSelectorCancel.ClientID %>";
                    tbltTenant.btnPECMailBoxSelectorOkId = "<%= btnPECMailBoxSelectorOk.ClientID %>";
                    tbltTenant.btnPECMailBoxSelectorCancelId = "<%= cbRoleSelector.ClientID %>";
                    tbltTenant.btnRoleSelectorOkId = "<%= cbWorkflowRepositorySelector.ClientID %>";
                    tbltTenant.btnPECMailBoxSelectorCancelId = "<%= btnPECMailBoxSelectorCancel.ClientID %>";
                    tbltTenant.btnRoleSelectorOkId = "<%= btnRoleSelectorOk.ClientID %>";
                    tbltTenant.btnRoleSelectorCancelId = "<%= btnRoleSelectorCancel.ClientID %>";
                    tbltTenant.btnWorkflowRepositorySelectorOkId = "<%= btnWorkflowRepositorySelectorOk.ClientID %>";
                    tbltTenant.btnWorkflowRepositorySelectorCancelId = "<%= btnWorkflowRepositorySelectorCancel.ClientID %>";
                    tbltTenant.btnTenantConfigurationSelectorOkId = "<%= btnTenantConfigurationSelectorOk.ClientID %>";
                    tbltTenant.btnTenantConfigurationSelectorCancelId = "<%= btnTenantConfigurationSelectorCancel.ClientID %>";
                    tbltTenant.btnTenantSelectorOkId = "<%= btnTenantSelectorOk.ClientID %>";
                    tbltTenant.btnTenantSelectorCancelId = "<%= btnTenantSelectorCancel.ClientID %>";
                    tbltTenant.btnTenantInsertId = "<%= btnTenantInsert.ClientID %>";
                    tbltTenant.btnTenantUpdateId = "<%= btnTenantUpdate.ClientID %>";

                    tbltTenant.uscContattiSelRestId = "<% =uscContattiSelRest.PanelContent.ClientID%>";
                    tbltTenant.uscRoleRestId = "<%= uscRoles.TableContentControl.ClientID %>";

                    tbltTenant.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";

                    tbltTenant.initialize();
                });
            });

        </script>
        <style>
            #ctl00_cphContent_rtvTenants {
                height: 90%;
            }
        </style>
    </telerik:RadScriptBlock>

    <telerik:RadWindow runat="server" ID="rwContainerSelector" Title="Seleziona Contenitore" Width="650" Height="100" Behaviors="Close, Move, Maximize" RenderMode="Lightweight">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="ContainerSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="ContainerSelectorWindowTable">
                        <tr>
                            <td class="label">Contenitore</td>
                            <td>
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="cbContainer" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnContainerSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnContainerSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwPECMailBoxSelector" Title="Seleziona casella PEC" Width="650" Height="100" Behaviors="Close, Move, Maximize">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="PECMailBoxSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="PECMailBoxSelectorWindowTable">
                        <tr>
                            <td class="label">Casella PEC</td>
                            <td>
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="cbPECMailBoxSelector" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnPECMailBoxSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnPECMailBoxSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                            
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwRoleSelector" Title="Seleziona settore" Width="650" Height="100" Behaviors="Close, Move, Maximize" RenderMode="Lightweight">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="RoleSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="RoleSelectorWindowTable">
                        <tr>
                            <td class="label">Settore</td>
                            <td>
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="cbRoleSelector" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnRoleSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnRoleSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwWorkflowRepositorySelector" Title="Seleziona attività" Width="650" Height="700" Behaviors="Close, Move, Maximize">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="WorkflowRepositorySelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="WorkflowRepositorySelectorWindowTable">
                        <tr>
                            <td class="label">Flusso di lavoro</td>
                            <td>
                                <telerik:RadComboBox runat="server" CausesValidation="false"
                                    ID="cbWorkflowRepositorySelector" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                    ItemRequestTimeout="500" Width="450px" ShowMoreResultsBox="true">
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Attivazione/Disattivazione:
                            </td>
                            <td>
                                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Da data" ID="dtpTenantWorkflowRepositoryDateFrom" runat="server" />
                                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="A data" ID="dtpTenantWorkflowRepositoryDateTo" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="dtpTenantWorkflowRepositoryDateFromValidator" Display="Dynamic" ControlToValidate="dtpTenantWorkflowRepositoryDateFrom" ErrorMessage="Da data non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Configurazione attività:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantWorkflowRepositoryJsonValue" MaxLength="1000" runat="server" Width="100%" Height="260px" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Nome del modulo di integrazione:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantWorkflowRepositoryIntegrationModuleName" MaxLength="1000" runat="server" Width="100%" Height="100px" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Condizioni:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantWorkflowRepositoryConditions" MaxLength="1000" runat="server" Width="100%" Height="100px" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="txtTenantWorkflowRepositoryJsonValueValidator" Display="Dynamic" ControlToValidate="txtTenantWorkflowRepositoryJsonValue" ErrorMessage="Configurazione attività non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Tipologia:</td>
                            <td>
                                <telerik:RadComboBox runat="server" ID="cmbTenantWorkflowRepositoryType" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="cmbTenantWorkflowRepositoryTypeValidator" Display="Dynamic" ControlToValidate="cmbTenantWorkflowRepositoryType" ErrorMessage="Tipologia non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnWorkflowRepositorySelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnWorkflowRepositorySelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwTenantConfigurationSelector" Title="Seleziona configurazione" Width="650" Height="500" Behaviors="Close, Move, Maximize">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="TenantConfigurationSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="TenantConfigurationSelectorSelectorWindowTable">

                        <tr>
                            <td class="label">Attivazione/Disattivazione:
                            </td>
                            <td>
                                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Da data" ID="dtpDateFrom" runat="server" />
                                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="A data" ID="dtpDateTo" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="dtpDateFromValidator" Display="Dynamic" ControlToValidate="dtpDateFrom" ErrorMessage="Da data non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Note:</td>
                            <td>
                                <telerik:RadTextBox ID="tenantConfigurationNote" MaxLength="255" runat="server" Width="100%" Height="100%" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">JsonValue:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantConfigurationJsonValue" MaxLength="1000" runat="server" Width="100%" Height="260px" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="txtTenantConfigurationJsonValueValidator" Display="Dynamic" ControlToValidate="txtTenantConfigurationJsonValue" ErrorMessage="JsonValue non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Configuration Type:</td>
                            <td>
                                <telerik:RadComboBox runat="server" ID="cmbConfigurationType" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="cmbConfigurationTypeValidator" Display="Dynamic" ControlToValidate="cmbConfigurationType" ErrorMessage="Configuration Type non può essere vuoto!" />
                            </td>
                        </tr>

                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnTenantConfigurationSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnTenantConfigurationSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwTenantSelector" Width="650" Height="300" Behaviors="Close, Move, Maximize">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="TenantSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="datatable" id="TenantSelectorSelectorWindowTable">

                        <tr>
                            <td class="label">Intervallo di date:
                            </td>
                            <td>
                                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Da data" ID="dpTenantDateFrom" runat="server" />
                                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="A data" ID="dpTenantDateTo" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="dpTenantDateFromValidator" Display="Dynamic" ControlToValidate="dpTenantDateFrom" ValidationGroup="TenantValidationGroup" ErrorMessage="Da data non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Sigla AOO:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantName" MaxLength="10" runat="server" Width="100%" Height="100%" TextMode="SingleLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="txtTenantNameValidator" Display="Dynamic" ControlToValidate="txtTenantName" ValidationGroup="TenantValidationGroup" ErrorMessage="Il nome del tenant non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Nome AOO:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantCompany" MaxLength="255" runat="server" Width="100%" Height="100%" TextMode="SingleLine" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="txtTenantCompanyValidator" Display="Dynamic" ControlToValidate="txtTenantCompany" ValidationGroup="TenantValidationGroup" ErrorMessage="Il nome del'AOO non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Note:</td>
                            <td>
                                <telerik:RadTextBox ID="txtTenantNote" MaxLength="255" runat="server" Width="100%" Height="100%" TextMode="MultiLine" />
                            </td>
                        </tr>

                        <tr>
                            <td class="buttons" colspan="2">
                                <telerik:RadButton runat="server" ID="btnTenantSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnTenantSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" Height="100%">

            <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">
                    <telerik:RadPane runat="server" Width="50%" Scrolling="None">
                        <telerik:RadToolBar AutoPostBack="False" ID="ToolBarSearch" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchTenantName">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtSearchTenantName" Placeholder="Sigla AOO" runat="server" AutoPostBack="False" Width="200px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Value="searchCompanyName">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtSearchCompanyName" Placeholder="Nome AOO esteso" runat="server" AutoPostBack="False" Width="200px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                                <telerik:RadToolBarButton IsSeparator="true" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarStatus" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton>
                                    <ItemTemplate>
                                        <label>Stato dell'AOO:</label>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Disattivi" CheckOnClick="true" Group="Disabled" Checked="false" Value="searchDisabled" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Attivi" CheckOnClick="true" Checked="true" Group="Active" Value="searchActive" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="rtvTenants" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="AOO" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>

                    <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>

                </telerik:RadSplitter>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" ID="Bar1" />

            <telerik:RadPane runat="server">
                <asp:Panel runat="server" ID="pnlDetails" Enabled="True">

                    <table id="ItemDetailTable" class="datatable pec" style="height: 100%; margin-bottom: 0; table-layout: fixed">
                        <tr>
                            <th colspan="2">Informazioni</th>
                        </tr>
                        <tr>
                            <td class="label">Nome AOO
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblCompanyName" Width="90%"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Sigla AOO
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTenantName" Width="90%"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Note
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTenantNote" Width="90%"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Data di attivazione
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTenantDataDiAttivazione" Width="90%"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Data di disattivazione
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTenantDataDiDisattivazione" Width="90%"></asp:Label>
                            </td>
                        </tr>

                    </table>
                    <table class="datatable">
                        <thead>
                            <tr>
                                <th>Contenitori</th>
                            </tr>
                        </thead>
                        <tr>
                            <td>
                                <telerik:RadToolBar AutoPostBack="False" ID="tbContainersControl" CssClass="ToolBarContainer"
                                    RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                                    <Items>
                                        <telerik:RadToolBarButton CommandName="ADDNEW" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_add.png" ToolTip="Aggiungi contenitore esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open_remove.png" ToolTip="Elimina contenitore selezionato"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTreeView ID="rtvContainers" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
                                </telerik:RadTreeView>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable">
                        <thead>
                            <tr>
                                <th>Caselle PEC</th>
                            </tr>
                        </thead>
                        <tr>
                            <td>
                                <telerik:RadToolBar AutoPostBack="False" runat="server" ID="tbPECMailBoxControl" CssClass="ToolBarContainer"
                                    RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" Width="100%">
                                    <Items>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/mail_box_add.png" ToolTip="Aggiungi casella PEC"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/mail_box_remove.png" ToolTip="Elimina casella PEC selezionato"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTreeView ID="rtvPECMailBoxes" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
                                </telerik:RadTreeView>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable">
                        <tr>
                            <td>
                                <usc:uscRoleRest Caption="Settori collegati" 
                                                 MultipleRoles="true" 
                                                 ID="uscRoles" 
                                                 ReadOnlyMode="false" 
                                                 runat="server"></usc:uscRoleRest>
                            </td>
                        </tr>
                    </table>
                    <table class="datatable">
                        <tr>
                            <th>Flusso di lavoro</th>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadToolBar AutoPostBack="False" runat="server" ID="tbWorkflowControl" CssClass="ToolBarContainer"
                                    RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" Width="100%">
                                    <Items>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="../Comm/Images/DocSuite/Workflow16.png" ToolTip="Aggiungi attività esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="EDIT" ImageUrl="~/App_Themes/DocSuite2008/imgset16/workflow_edit.png" ToolTip="Modifica attività esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/workflow_delete.png" ToolTip="Elimina attività selezionato"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTreeView ID="rtvWorkflowRepositories" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
                                </telerik:RadTreeView>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable">
                        <tr>
                            <th>Configurazioni</th>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadToolBar runat="server" ID="tbConfigurationControl" CssClass="ToolBarContainer"
                                    RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" Width="100%">
                                    <Items>
                                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" ToolTip="Aggiungi configurazione esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="EDIT" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_edit.png" ToolTip="Modifica configurazione esistente"></telerik:RadToolBarButton>
                                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" ToolTip="Elimina configurazione selezionato"></telerik:RadToolBarButton>
                                    </Items>
                                </telerik:RadToolBar>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadTreeView ID="rtvTenantConfigurations" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%" />
                            </td>
                        </tr>
                    </table>


                    <%-- Contatti --%>

                    <table class="datatable">
                        <tr>
                            <th>Contatti</th>
                        </tr>
                         <tr>
                            <td>
                                <asp:Panel runat="server">
                                    <usc:uscContattiSelRest runat="server" ID="uscContattiSelRest"></usc:uscContattiSelRest>
                                </asp:Panel>
                                
                            </td>
                        </tr>
                    </table>      
                </asp:Panel>
            </telerik:RadPane>

        </telerik:RadSplitter>
    </div>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphFooter">
    <table style="width: 100%;">
        <tr>
            <td style="width: 50%;">
                <telerik:RadButton ID="btnTenantInsert" runat="server" Text="Aggiungi" Width="100px" AutoPostBack="false"></telerik:RadButton>
                <telerik:RadButton ID="btnTenantUpdate" runat="server" Text="Modifica" Width="100px" AutoPostBack="false"></telerik:RadButton>
            </td>
        </tr>
    </table>
</asp:Content>
