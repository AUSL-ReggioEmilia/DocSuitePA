<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscTenantsSelector.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscTenantsSelector" %>


<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscTenantsSelector;
        require(["UserControl/uscTenantsSelector"], function (UscTenantsSelector) {
            $(function () {
                uscTenantsSelector = new UscTenantsSelector(tenantModelConfiguration.serviceConfiguration);
                uscTenantsSelector.cmbSelectAziendaId = "<%= cmbSelectAzienda.ClientID %>";
                uscTenantsSelector.cmbSelectPecMailBoxId = "<%= cmbSelectPecMailBox.ClientID %>";
                uscTenantsSelector.cmbWorkflowRepositoriesId = "<%= cmbWorkflowRepositories.ClientID %>";
                uscTenantsSelector.btnContainerSelectorOkId = "<%= btnContainerSelectorOk.ClientID %>";

                uscTenantsSelector.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function OnClientShow(sender, args) {    
            uscTenantsSelector.onClientShow();
        }
    </script>
</telerik:RadScriptBlock>


<telerik:RadWindow runat="server" ID="rwTenantSelector" Height="150px" Width="350" OnClientShow="OnClientShow">
    <ContentTemplate>
        <table class="datatable" id="ContainerSelectorWindowTable">
            <tr>
                <td class="label">Azienda</td>
                <td>
                    <telerik:RadComboBox runat="server" CausesValidation="false"
                                         ID="cmbSelectAzienda" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                         ItemRequestTimeout="500" Width="200px" ShowMoreResultsBox="True">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="label">Caselle PEC</td>
                <td>
                    <telerik:RadComboBox runat="server" CausesValidation="false"
                                         ID="cmbSelectPecMailBox" AutoPostBack="false" EnableLoadOnDemand="true"  MarkFirstMatch="true"
                                         ItemRequestTimeout="500" Width="200px" ShowMoreResultsBox="True">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="label">Attività</td>
                <td>
                    <telerik:RadComboBox runat="server" CausesValidation="false"
                                         ID="cmbWorkflowRepositories" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                         ItemRequestTimeout="500" Width="200px" ShowMoreResultsBox="True">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="buttons" colspan="2">
                    <telerik:RadButton runat="server" ID="btnContainerSelectorOk" Text="Conferma" AutoPostBack="False"></telerik:RadButton>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</telerik:RadWindow>
