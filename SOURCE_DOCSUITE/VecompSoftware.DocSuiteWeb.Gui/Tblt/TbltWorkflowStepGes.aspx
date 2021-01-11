<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltWorkflowStepGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltWorkflowStepGes"  MasterPageFile="~/MasterPages/DocSuite2008.Master" 
    Title="Gestione step del flusso di lavoro"%>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltWorkflowStepGes;
            require(["Tblt/TbltWorkflowStepGes"], function (TbltWorkflowStepGes) {
                $(function () {
                    tbltWorkflowStepGes = new TbltWorkflowStepGes();

                    tbltWorkflowStepGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltWorkflowStepGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltWorkflowStepGes.pageContentId = "<%= pnlWorkflowStep.ClientID %>";
                    tbltWorkflowStepGes.txtWorkflowStepNameId = "<%= txtWorkflowStepName.ClientID%>";
                    tbltWorkflowStepGes.cmbWorkflowStepAuthorizationTypeId = "<%= cbWorkflowStepAuthorizationType.ClientID %>";
                    tbltWorkflowStepGes.cmbWorkflowStepActivityTypeId = "<%= cbWorkflowStepActivityType.ClientID %>";
                    tbltWorkflowStepGes.cmbWorkflowStepActivityActionId = "<%= cbWorkflowStepActivityAction.ClientID %>";
                    tbltWorkflowStepGes.cmbWorkflowStepActivityAreaId = "<%= cbWorkflowStepActivityArea.ClientID %>";
                    tbltWorkflowStepGes.btnWorkflowStepSelectorOkId = "<%= btnWorkflowStepSelectorOk.ClientID %>";
                    tbltWorkflowStepGes.btnWorkflowStepSelectorCancelId = "<%= btnWorkflowStepSelectorCancel.ClientID %>";
                    tbltWorkflowStepGes.actionPage = "<% = PageAction %>";

                    tbltWorkflowStepGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlWorkflowStep" runat="server" Width="100%">
        <table class="dataform" id="WorkflowStepSelectorTable">
            <tr>
                <td class="label">Tipo autorizzazione: </td>
                <td>
                    <telerik:RadComboBox runat="server" CausesValidation="false" ID="cbWorkflowStepAuthorizationType"
                                        AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true" 
                                        ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true" Filter="Contains" 
                                        DataTextField="Name" DataValueField="Value">
                    </telerik:RadComboBox>
                </td>
             </tr>
                <tr>
                    <td class="label">Nome: </td>
                    <td>
                        <telerik:RadTextBox ID="txtWorkflowStepName" runat="server" Width="550px"></telerik:RadTextBox>
                    </td>
                </tr>
              
                <tr>
                    <td class="label">Tipo attività: </td>
                    <td>
                        <telerik:RadComboBox runat="server" CausesValidation="false" ID="cbWorkflowStepActivityType"
                                            AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true" 
                                            ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true"
                                            DataTextField="Name" DataValueField="Value">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                 <tr>
                    <td class="label">Area workflow: </td>
                    <td>
                        <telerik:RadComboBox runat="server" CausesValidation="false" ID="cbWorkflowStepActivityArea"
                                            AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true" 
                                            ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true"
                                            DataTextField="Name" DataValueField="Value">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">Azione workflow: </td>
                    <td>
                        <telerik:RadComboBox runat="server" CausesValidation="false" ID="cbWorkflowStepActivityAction"
                                            AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true" 
                                            ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true"
                                            DataTextField="Name" DataValueField="Value">
                        </telerik:RadComboBox>
                    </td>
                </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton runat="server" ID="btnWorkflowStepSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnWorkflowStepSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
</asp:Content>
