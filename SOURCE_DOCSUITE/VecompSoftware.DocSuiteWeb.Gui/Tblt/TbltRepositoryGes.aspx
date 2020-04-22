<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltRepositoryGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltRepositoryGes" 
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Workflow" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var TbltRepositoryGes;
            require(["Tblt/TbltRepositoryGes"], function (TbltRepositoryGes) {
                $(function () {
                    TbltRepositoryGes = new TbltRepositoryGes();

                    TbltRepositoryGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    TbltRepositoryGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    TbltRepositoryGes.pageContentId = "<%= pnlWorkflow.ClientID %>";
                    TbltRepositoryGes.txtWorkflowNameId = "<%= txtWorkflowName.ClientID%>";
                    TbltRepositoryGes.rntbVersionValueId = "<%= rntbVersionValue.ClientID%>";
                    TbltRepositoryGes.rdpValueDateId = "<%= rdpValueDate.ClientID%>";
                    TbltRepositoryGes.cmbEnvironmentId = "<%= cbEnvironment.ClientID %>";
                    TbltRepositoryGes.btnWorkflowSelectorOkId = "<%= btnWorkflowSelectorOk.ClientID %>";
                    TbltRepositoryGes.btnWorkflowSelectorCancelId = "<%= btnWorkflowSelectorCancel.ClientID %>";
                    TbltRepositoryGes.actionPage = "<% = PageAction %>";
                    TbltRepositoryGes.environmentDataSourceId = "<% = environmentDataSource.ClientID %>";

                    TbltRepositoryGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlWorkflow" runat="server" Width="100%">
        <table class="dataform" id="WorkflowSelectorTable" style="text-align:center;">
                <tr>
                    <td class="label">Nome workflow: </td>
                    <td>
                        <telerik:RadTextBox ID="txtWorkflowName" runat="server"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">Numero versione: </td>
                    <td>
                        <telerik:RadNumericTextBox runat="server" CausesValidation="false"
                            ID="rntbVersionValue" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                            ShowSpinButtons="true" Width="300">
                            <IncrementSettings Step="1" />
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="label">Data attivazione: </td>
                    <td>
                        <div id="valueDate">
                            <telerik:RadDatePicker runat="server" ID="rdpValueDate" Width="300">
                            </telerik:RadDatePicker>
                    </div>
                    </td>
                </tr>
                <tr>
                    <td class="label">Environment: </td>
                    <td>
                        <telerik:RadClientDataSource runat="server" ID="environmentDataSource" />
                        <telerik:RadComboBox runat="server" CausesValidation="false" ID="cbEnvironment"
                                            AutoPostBack="false" EnableLoadOnDemand="true" Filter="Contains"
                                            ItemRequestTimeout="500" Width="550px" ShowMoreResultsBox="true"
                                            ClientDataSourceID="environmentDataSource" DataTextField="Name" DataValueField="Value">
                        </telerik:RadComboBox>
                    </td>
                </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton runat="server" ID="btnWorkflowSelectorOk" Text="Conferma" Width="100px" AutoPostBack="false"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnWorkflowSelectorCancel" Text="Annulla" Width="100px" AutoPostBack="false"></telerik:RadButton>
</asp:Content>

