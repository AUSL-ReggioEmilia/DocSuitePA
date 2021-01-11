<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProcessDetails.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProcessDetails" %>

<%@ Register Src="~/UserControl/uscFascicleFolders.ascx" TagName="uscFascicleFolders" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCustomActionsRest.ascx" TagName="uscCustomActionsRest" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        var uscProcessDetails;
        require(["UserControl/uscProcessDetails"], function (UscProcessDetails) {
            $(function () {
                uscProcessDetails = new UscProcessDetails(tenantModelConfiguration.serviceConfiguration);
                uscProcessDetails.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscProcessDetails.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscProcessDetails.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                uscProcessDetails.lblNameId = "<%= lblName.ClientID %>";
                uscProcessDetails.lblFolderNameId = "<%= lblFolderName.ClientID %>";
                uscProcessDetails.divFolderNameId = "<%= divFolderName.ClientID %>";
                uscProcessDetails.lblClasificationNameId = "<%= lblClasificationName.ClientID %>";
                uscProcessDetails.rcbWorkflowRepositoryId = "<%= rcbWorkflowRepository.ClientID %>";
                uscProcessDetails.toolbarWorkflowRepositoryId = "<%= toolbarWorkflowRepository.ClientID %>";
                uscProcessDetails.rtvWorkflowRepositoryId = "<%= rtvWorkflowRepository.ClientID %>";
                uscProcessDetails.rcbMetadataRepositoryId = "<%= rcbMetadataRepository.ClientID %>";
                uscProcessDetails.rbAddFascicleId = "<%= rbAddFascicle.ClientID %>";
                uscProcessDetails.rtbFascicleSubjectId = "<%= rtbFascicleSubject.ClientID %>";
                uscProcessDetails.rbFascicleVisibilityTypeId = "<%= rbFascicleVisibilityType.ClientID %>";
                uscProcessDetails.uscFascicleFoldersId = "<%= uscFascicleFolders.PageContentDiv.ClientID %>";
                uscProcessDetails.uscContactRestId = "<%= uscContattiSelRest.PanelContent.ClientID %>";
                uscProcessDetails.uscRoleRestId = "<%= uscRoleRest.TableContentControl.ClientID %>";
                uscProcessDetails.uscResponsibleRolesId = "<%= uscResponsibleRoles.TableContentControl.ClientID %>";
                uscProcessDetails.uscAuthorizedRolesId = "<%= uscAuthorizedRoles.TableContentControl.ClientID %>";
                uscProcessDetails.lblActivationDateId = "<%= lblActivationDate.ClientID %>";
                uscProcessDetails.rcbFascicleTypeId = "<%= rcbFascicleType.ClientID %>";
                uscProcessDetails.rpbDetailsId = "<%= rpbDetails.ClientID %>";
                uscProcessDetails.lblCategoryCodeId = "<%= lblCategoryCode.ClientID %>";
                uscProcessDetails.lblCategoryNameId = "<%= lblCategoryName.ClientID %>";
                uscProcessDetails.lblStartDateId = "<%= lblStartDate.ClientID %>";
                uscProcessDetails.lblEndDateId = "<%= lblEndDate.ClientID %>";
                uscProcessDetails.lblMetadataId = "<%= lblMetadata.ClientID %>";
                uscProcessDetails.lblMassimarioNameId = "<%= lblMassimarioName.ClientID %>";
                uscProcessDetails.lblRegistrationDateId = "<%= lblRegistrationDate.ClientID %>";
                uscProcessDetails.lblNoteId = "<%= lblNote.ClientID %>";
                uscProcessDetails.uscCustomActionsRestId = "<%= uscCustomActionsRest.PageContent.ClientID %>";
                uscProcessDetails.initialize();
            });
        });

    </script>
    <style>
        #ctl00_cphContent_uscProcessDetails_rpbDetails_i4_uscFascicleFolders_pnlTitle,
        #ctl00_cphContent_uscProcessDetails_rpbDetails_i4_uscFascicleFolders_pnlFolderToolbar {
            position: relative !important;
        }

        #ctl00_cphContent_uscProcessDetails_rpbDetails_i4_uscFascicleFolders_pnlFolderToolbar,
        #ctl00_cphContent_uscProcessDetails_rpbDetails_i4_uscFascicleFolders_pnlFascicleFolder {
            margin-top: 0 !important;
        }

        #ctl00_cphContent_uscProcessDetails_rpbDetails_i4_uscFascicleFolders_FolderToolBar {
            z-index: 0 !important;
        }

        #pnlMainFascicleFolder > :first-child,
        #pnlMainFascicleFolder > :nth-child(2) {
            width: 100%;
        }
    </style>


</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel">
    <div class="dsw-panel-content">
        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbDetails">
            <Items>

                <telerik:RadPanelItem Text="Informazioni" Expanded="true" Value="pnlInformations">
                    <ContentTemplate>
                        <asp:Panel runat="server">
                            <div class="col-dsw-10">
                                <b>Nome della serie:</b>
                                <asp:Label runat="server" ID="lblName"></asp:Label>
                            </div>
                            <div class="col-dsw-10" id="divFolderName" style="display: none" runat="server">
                                <b>Nome del volume:</b>
                                <asp:Label runat="server" ID="lblFolderName"></asp:Label>
                            </div>
                            <div class="col-dsw-10">
                                <b>Classificatore:</b>
                                <asp:Label runat="server" ID="lblClasificationName"></asp:Label>
                            </div>
                            <div class="col-dsw-10">
                                <b>Data di attivazione:</b>
                                <asp:Label runat="server" ID="lblActivationDate"></asp:Label>
                            </div>
                            <div class="col-dsw-10">
                                <b>Note:</b>
                                <asp:Label runat="server" ID="lblNote"></asp:Label>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadPanelItem>

                <telerik:RadPanelItem Text="Informazioni" Expanded="true" Value="pnlCategoryInformations">
                    <ContentTemplate>
                        <asp:Panel runat="server">
                            <div class="col-dsw-10 dsw-align-center">
                                <div class="col-dsw-10">
                                    <b>Voce:</b>
                                    <b>
                                        <asp:Label runat="server" ID="lblCategoryCode" />
                                    </b>
                                    <asp:Label runat="server" ID="lblCategoryName" />
                                </div>
                            </div>
                            <div class="col-dsw-5 dsw-align-left">
                                <div class="col-dsw-10">
                                    <b>Data attivazione:</b>
                                    <asp:Label runat="server" ID="lblStartDate"></asp:Label>
                                </div>
                                <div class="col-dsw-10">
                                    <b>Data disattivazione:</b>
                                    <asp:Label runat="server" ID="lblEndDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-dsw-5 dsw-align-left">
                                <div class="col-dsw-10" id="metadataDetails" runat="server">
                                    <b>Metadati:</b>
                                    <asp:Label runat="server" ID="lblMetadata"></asp:Label>
                                </div>
                                <div class="col-dsw-10">
                                    <b>Massimario di scarto:</b>
                                    <asp:Label runat="server" ID="lblMassimarioName"></asp:Label>
                                </div>
                            </div>
                            <div class="col-dsw-10 dsw-align-center">
                                <div class="col-dsw-10" runat="server" id="divProcedureType">
                                    <b>Attivato un piano di fascicolazione di procedimento 
                                                                <asp:Label runat="server" ID="lblRegistrationDate" /></b>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadPanelItem>

                <telerik:RadPanelItem Text="Settori autorizzati" Expanded="true" Value="pnlRoleDetails">
                    <ContentTemplate>
                        <usc:uscRoleRest runat="server" ID="uscRoleRest" ReadOnlyMode="false" MultipleRoles="true"
                            Collapsable="true" Caption="Settori autorizzati" />
                    </ContentTemplate>
                </telerik:RadPanelItem>

                <telerik:RadPanelItem Text="Flusso di lavoro" Expanded="true" Value="pnlWorkflowDetails">
                    <ContentTemplate>
                        <asp:Panel runat="server">
                            <table class="datatable">
                                <tr>
                                    <td>
                                        <telerik:RadToolBar ID="toolbarWorkflowRepository" runat="server" RenderMode="Lightweight" AutoPostBack="false" CssClass="ToolBarContainer" Width="100%">
                                            <Items>
                                                <telerik:RadToolBarButton CausesValidation="False"
                                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png"
                                                    ToolTip="Aggiungi flusso di lavoro"
                                                    CommandName="add" PostBack="false" />
                                                <telerik:RadToolBarButton CausesValidation="False"
                                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png"
                                                    ToolTip="Elimina flusso di lavoro"
                                                    CommandName="delete" PostBack="false" />
                                            </Items>
                                        </telerik:RadToolBar>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <telerik:RadComboBox ID="rcbWorkflowRepository" runat="server" Width="300px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <telerik:RadTreeView ID="rtvWorkflowRepository" runat="server">
                                            <Nodes>
                                                <telerik:RadTreeNode Text="Flussi di lavoro" />
                                            </Nodes>
                                        </telerik:RadTreeView>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadPanelItem>

                <telerik:RadPanelItem Text="Fascicolo" Expanded="true" Value="pnlFascicleDetails">
                    <ContentTemplate>
                        <asp:Panel runat="server" CssClass="dsw-panel">
                            <table class="datatable">
                                <tr>
                                    <td id="fascicleInsert">
                                        <div>
                                            <div id="fascicleInsertFieldset" class="dsw-panel-content">
                                                <div id="fascicleInputs">
                                                    <div class="dsw-panel-content">
                                                        <div class="col-dsw-10">
                                                            <b>Oggetto:</b>
                                                            <telerik:RadTextBox ID="rtbFascicleSubject" runat="server" AutoPostBack="false" />
                                                        </div>
                                                        <div class="col-dsw-10">
                                                            <b>Tipo di fascicolo:</b>
                                                            <telerik:RadComboBox runat="server" ID="rcbFascicleType" AutoPostBack="false" />
                                                        </div>
                                                        <div class="col-dsw-10">
                                                            <telerik:RadButton ID="rbFascicleVisibilityType" runat="server" AutoPostBack="false" ButtonType="StandardButton" ToggleType="CheckBox" Checked="false" Text="Rendi i documenti disponibili ai settori autorizzati" />

                                                        </div>
                                                    </div>
                                                    <div id="uscContactRestFieldset">
                                                        <table class="datatable">
                                                            <thead>
                                                                <tr>
                                                                    <th>Responsabile di procedimento</th>
                                                                </tr>
                                                            </thead>
                                                        </table>
                                                        <usc:uscContattiSelRest runat="server" ID="uscContattiSelRest" />
                                                    </div>
                                                    <div id="responsibleRoleFieldset">
                                                        <usc:uscRoleRest runat="server" ID="uscResponsibleRoles" ReadOnlyMode="false" MultipleRoles="false" OnlyMyRoles="false" Collapsable="true" Caption="Settore responsabile" />
                                                    </div>
                                                    <div id="authorizedRolesFieldset">
                                                        <usc:uscRoleRest runat="server" ID="uscAuthorizedRoles" ReadOnlyMode="false" MultipleRoles="true" Required="false" OnlyMyRoles="false" Collapsable="true" Caption="Settori autorizzati" RACIButtonEnabled="true" />
                                                    </div>
                                                    <div id="uscFascicleFoldersFieldset">
                                                        <usc:uscFascicleFolders runat="server" ID="uscFascicleFolders" DoNotUpdateDatabase="true" IsVisibile="true" />
                                                    </div>
                                                    <div id="metadataRepositoryFieldset">
                                                        <table class="datatable">
                                                            <thead>
                                                                <tr>
                                                                    <th>Metadati</th>
                                                                </tr>
                                                            </thead>
                                                        </table>
                                                        <telerik:RadComboBox ID="rcbMetadataRepository" runat="server" Width="300px" />
                                                    </div>
                                                    <div id="customActionsFieldset">
                                                        <table class="datatable">
                                                            <thead>
                                                                <tr>
                                                                    <th>Azioni personalizzate</th>
                                                                </tr>
                                                            </thead>
                                                        </table>
                                                        <usc:uscCustomActionsRest runat="server" ID="uscCustomActionsRest" IsFromInsertPage="true" />
                                                    </div>
                                                    <telerik:RadButton runat="server" ID="rbAddFascicle" Text="Salva il template" AutoPostBack="false" />
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadPanelItem>

            </Items>
        </telerik:RadPanelBar>
    </div>
</asp:Panel>
