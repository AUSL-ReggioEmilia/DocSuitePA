<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltWorkflowEvaluationPropertyGes.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltWorkflowEvaluationPropertyGes"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>


<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDomainUserSelRest.ascx" TagName="uscDomainUserSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscWorkflowDesignerValidations.ascx" TagName="uscWorkflowDesignerValidations" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscTemplateCollaborationSelRest.ascx" TagName="uscTemplateCollaborationSelRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltWorkflowEvaluationPropertyGes;
            require(["Tblt/TbltWorkflowEvaluationPropertyGes"], function (TbltWorkflowEvaluationPropertyGes) {
                $(function () {
                    tbltWorkflowEvaluationPropertyGes = new TbltWorkflowEvaluationPropertyGes(tenantModelConfiguration.serviceConfiguration);

                    tbltWorkflowEvaluationPropertyGes.rcbNameId = "<%= rcbName.ClientID%>";
                    tbltWorkflowEvaluationPropertyGes.rntbValueIntId = "<%= rntbValueInt.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.rtbValueStringId = "<%= rtbValueString.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.rtbValueJsonId = "<%= rtbValueJson.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.rlbValueBoolId = "<%= rlbValueBool.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.rdpValueDateId = "<%= rdpValueDate.ClientID%>";
                    tbltWorkflowEvaluationPropertyGes.rntbValueDoubleId = "<%= rntbValueDouble.ClientID%>";
                    tbltWorkflowEvaluationPropertyGes.rdbGuidId = "<%= rdbGuid.ClientID%>";
                    tbltWorkflowEvaluationPropertyGes.workflowEnv = "<%=WorkflowEnv%>";

                    tbltWorkflowEvaluationPropertyGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.pageContentId = "<%= pnlRinomina.ClientID %>";

                    tbltWorkflowEvaluationPropertyGes.uscRoleRestId = "<%=uscRoleRest.TableContentControl.ClientID%>";
                    tbltWorkflowEvaluationPropertyGes.uscRoleRestContainerId = "<%="uscRoleRestContainer"%>";

                    tbltWorkflowEvaluationPropertyGes.uscWorkflowDesignerValidationsId = "<%=uscWorkflowDesignerValidations.PageContent.ClientID%>";
                    tbltWorkflowEvaluationPropertyGes.uscWorkflowDesignerValidationsContainerId = "<%="uscWorkflowDesignerValidationsContainer"%>";

                    tbltWorkflowEvaluationPropertyGes.uscDomainUserSelRestId = "<%=uscDomainUserSelRest.PageContent.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.uscDomainUserSelRestContainerId = "<%="uscDomainUserSelRestContainer"%>";

                    tbltWorkflowEvaluationPropertyGes.uscTemplateCollaborationSelRestId = "<%= uscTemplateCollaborationSelRest.MainPanel.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.uscTemplateCollaborationContainerId = "<%="uscTemplateCollaborationContainer"%>";

                    tbltWorkflowEvaluationPropertyGes.ddlCollaborationSignSummaryId = "<%=ddlCollaborationSignSummary.ClientID%>"
                    tbltWorkflowEvaluationPropertyGes.uscCollaborationSignSummaryContainerId = "<%="uscCollaborationSignSummaryContainer"%>"

                    tbltWorkflowEvaluationPropertyGes.ddlActionGenerateId = "<%=ddlActionGenerate.ClientID%>"
                    tbltWorkflowEvaluationPropertyGes.uscActionGenerateContainerId = "<%="uscActionGenerateContainer"%>"

                    tbltWorkflowEvaluationPropertyGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table class="dataform">
            <tr>
                <td class="label col-dsw-3">Proprietà</td>

                <td class="col-dsw-7">
                    <telerik:RadComboBox AllowCustomText="true" runat="server" CausesValidation="false"
                        ID="rcbName" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        ItemRequestTimeout="500" Width="500">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="tfvNameWorkflowStartup" runat="server" Display="Dynamic" ErrorMessage="Campo proprietà obbligatorio" ControlToValidate="rcbName" />
                </td>
            </tr>

            <tr>
                <td class="label col-dsw-3" id="labelValue">Valore</td>
                <td class="col-dsw-7">
                    <telerik:RadNumericTextBox runat="server" CausesValidation="false"
                        ID="rntbValueInt" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        ShowSpinButtons="true" Width="300">
                        <IncrementSettings Step="1" />
                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                    </telerik:RadNumericTextBox>

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="400"
                        ID="rtbValueString" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true">
                    </telerik:RadTextBox>

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="500"
                        ID="rtbValueJson" Rows="10" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        TextMode="MultiLine">
                    </telerik:RadTextBox>

                    <div id="uscRoleRestContainer">
                        <usc:uscRoleRest runat="server" ID="uscRoleRest" Caption="Autorizzazioni" Required="true" DSWEnvironmentType="Document" OnlyMyRoles="true"
                            Expanded="true" />
                    </div>

                    <div id="uscWorkflowDesignerValidationsContainer">
                        <usc:uscWorkflowDesignerValidations runat="server" ID="uscWorkflowDesignerValidations" Caption="Regole di validazione" />
                    </div>

                    <div id="uscTemplateCollaborationContainer">
                        <!-- TEMPLATE COLLABORATION DDL -->
                        <usc:uscTemplateCollaborationSelRest runat="server" ID="uscTemplateCollaborationSelRest" AutoPostBack="True" TreeViewInitializationEnabled="False" />
<%--                        <telerik:RadComboBox runat="server" ID="ddlTemplateCollaboration" Width="100%" Filter="Contains"
                            CausesValidation="false" EnableLoadOnDemand="True" AutoPostBack="false" EmptyMessage="Selezionare template">
                        </telerik:RadComboBox>--%>
                    </div>

                    <div id="uscCollaborationSignSummaryContainer">
                        <telerik:RadComboBox runat="server" ID="ddlCollaborationSignSummary" Width="100%" Filter="Contains"
                            CausesValidation="false" EnableLoadOnDemand="True" AutoPostBack="false" EmptyMessage="Selezionare template">
                        </telerik:RadComboBox>
                    </div>

                    <div id="uscActionGenerateContainer">
                        <telerik:RadComboBox runat="server" ID="ddlActionGenerate" Width="100%" Filter="Contains"
                            CausesValidation="false" EnableLoadOnDemand="True" AutoPostBack="false" EmptyMessage="Selezionare template">
                        </telerik:RadComboBox>
                    </div>

                    <div id="uscDomainUserSelRestContainer">
                        <usc:uscDomainUserSelRest runat="server" ID="uscDomainUserSelRest" Required="true" />
                    </div>

                    <div id="valueBool">
                        <telerik:RadListBox runat="server" CausesValidation="false"
                            ID="rlbValueBool" AutoPostBack="false" Width="300">
                            <Items>
                                <telerik:RadListBoxItem Value="0" Text="False" />
                                <telerik:RadListBoxItem Value="1" Text="True" />
                            </Items>
                        </telerik:RadListBox>
                    </div>

                    <div id="valueDate">
                        <telerik:RadDatePicker runat="server" ID="rdpValueDate" Width="300">
                        </telerik:RadDatePicker>
                    </div>

                    <telerik:RadNumericTextBox runat="server" CausesValidation="false"
                        ID="rntbValueDouble" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        ShowSpinButtons="true" Width="300">
                        <IncrementSettings Step="0.01" />
                    </telerik:RadNumericTextBox>

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="300"
                        ID="rdbGuid" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true">
                    </telerik:RadTextBox>

                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>
