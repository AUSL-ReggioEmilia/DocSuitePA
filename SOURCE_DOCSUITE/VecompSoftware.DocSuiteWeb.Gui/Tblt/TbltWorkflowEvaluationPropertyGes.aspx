<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltWorkflowEvaluationPropertyGes.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltWorkflowEvaluationPropertyGes"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>


<%@ Register Src="../UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc3" %>

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

                    tbltWorkflowEvaluationPropertyGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltWorkflowEvaluationPropertyGes.pageContentId = "<%= pnlRinomina.ClientID %>";
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
                    <telerik:RadComboBox  AllowCustomText="true" runat="server" CausesValidation="false"
                        ID="rcbName" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        ItemRequestTimeout="500" Width="300">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="tfvNameWorkflowStartup" runat="server" Display="Dynamic" ErrorMessage="Campo Proprietà Obbligatorio" ControlToValidate="rcbName" />
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

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="300"
                        ID="rtbValueString" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true">
                    </telerik:RadTextBox>

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="300"
                        ID="rtbValueJson" Rows="5" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        TextMode="MultiLine">
                    </telerik:RadTextBox>

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
