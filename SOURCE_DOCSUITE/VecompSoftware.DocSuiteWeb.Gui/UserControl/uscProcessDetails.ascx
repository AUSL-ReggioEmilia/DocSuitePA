<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProcessDetails.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProcessDetails" %>

<%@ Register Src="~/UserControl/uscFascicleFolders.ascx" TagName="uscFascicleFolders" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

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
                uscProcessDetails.lblClasificationNameId = "<%= lblClasificationName.ClientID %>";
                uscProcessDetails.lblFascicleTypeId = "<%= lblFascicleType.ClientID %>";
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
                uscProcessDetails.initialize();
            });
        });

    </script>
    <style>
        #ctl00_cphContent_uscProcessDetails_uscFascicleFolders_pnlTitle,
        #ctl00_cphContent_uscProcessDetails_uscFascicleFolders_pnlFolderToolbar {
            position: relative !important;
        }

        #ctl00_cphContent_uscProcessDetails_uscFascicleFolders_pnlFolderToolbar,
        #ctl00_cphContent_uscProcessDetails_uscFascicleFolders_pnlFascicleFolder {
            margin-top: 0 !important;
        }

        .remove .rwTable {
            height: 170px !important;
        }
    </style>


</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<asp:Panel runat="server" ID="pnlDetails">
    <table id="ItemDetailTable" class="datatable" style="height: 100%; margin-bottom: 0; table-layout: fixed">
        <tr>
            <th colspan="2">Informazioni</th>
        </tr>
        <tr>
            <td class="label">Nome del procedimento
            </td>
            <td>
                <asp:Label runat="server" ID="lblName" Width="90%"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label">Classificatore
            </td>
            <td>
                <asp:Label runat="server" ID="lblClasificationName" Width="90%"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label">Tipo di fascicolo
            </td>
            <td>
                <asp:Label runat="server" ID="lblFascicleType" Width="90%"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="datatable" id="roleDetails">
        <tr>
            <td>
                <usc:uscRoleRest runat="server" ID="uscRoleRest" ReadOnlyMode="false" MultipleRoles="true"
                    Collapsable="true" Caption="Settori autorizzati" />
            </td>
        </tr>
    </table>
    <table class="datatable" id="workflowDetails">
        <thead>
            <tr>
                <th>Flusso di lavoro</th>
            </tr>
        </thead>
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
    <table class="datatable" id="fascicleDetails">
        <thead>
            <tr>
                <th>Fascicolo</th>
            </tr>
        </thead>
        <tr>
            <td id="fascicleInsert">
                <div>
                    <div id="fascicleInsertFieldset">
                        <div id="fascicleInputs">
                            Oggetto:
                                    <telerik:RadTextBox ID="rtbFascicleSubject" runat="server" AutoPostBack="false" />
                            <br />
                            <br />
                            <telerik:RadButton ID="rbFascicleVisibilityType" runat="server" AutoPostBack="false" ButtonType="StandardButton" ToggleType="CheckBox" Checked="false" Text="Rendi i documenti disponibili ai settori autorizzati" />
                            <div id="responsibleRoleFieldset">
                                <usc:uscRoleRest runat="server" ID="uscResponsibleRoles" ReadOnlyMode="false" MultipleRoles="true"
                                    Collapsable="true" Caption="Settore responsabile" />
                            </div>
                            <hr />
                            <div id="authorizedRolesFieldset">
                                <usc:uscRoleRest runat="server" ID="uscAuthorizedRoles" ReadOnlyMode="false" MultipleRoles="true"
                                    Required="false" Collapsable="true" Caption="Settori autorizzati" />

                            </div>
                            <hr />
                            <div id="uscFascicleFoldersFieldset">
                                <usc:uscFascicleFolders runat="server" ID="uscFascicleFolders" DoNotUpdateDatabase="true" IsVisibile="true" />
                            </div>
                            <hr />
                            <div id="uscContactRestFieldset">
                                <table class="datatable">
                                    <thead>
                                        <tr>
                                            <th>Contatti</th>
                                        </tr>
                                    </thead>
                                </table>
                                <usc:uscContattiSelRest runat="server" ID="uscContattiSelRest" />
                            </div>
                            <hr />
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
                            <br />
                            <telerik:RadButton runat="server" ID="rbAddFascicle" Text="Salva il fascicolo" AutoPostBack="false" />
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
