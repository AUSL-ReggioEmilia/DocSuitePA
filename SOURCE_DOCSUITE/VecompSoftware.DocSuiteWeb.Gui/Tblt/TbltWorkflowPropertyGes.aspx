<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltWorkflowPropertyGes.aspx.vb" 
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltWorkflowPropertyGes" 
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>


<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltWorkflowEvaluationPropertyGes;
            require(["Tblt/TbltWorkflowPropertyGes"], function (TbltWorkflowPropertyGes) {
                $(function () {
                    TbltWorkflowPropertyGes = new TbltWorkflowPropertyGes();

                    TbltWorkflowPropertyGes.rcbNameId = "<%= rcbName.ClientID%>";
                    TbltWorkflowPropertyGes.rtbPropertyNameId = "<%= rtbPropertyName.ClientID%>";
                    TbltWorkflowPropertyGes.rntbValueIntId = "<%= rntbValueInt.ClientID %>";
                    TbltWorkflowPropertyGes.rtbValueStringId = "<%= rtbValueString.ClientID %>";
                    TbltWorkflowPropertyGes.rlbValueBoolId = "<%= rlbValueBool.ClientID %>";
                    TbltWorkflowPropertyGes.rdpValueDateId = "<%= rdpValueDate.ClientID%>";
                    TbltWorkflowPropertyGes.rntbValueDoubleId = "<%= rntbValueDouble.ClientID%>";
                    TbltWorkflowPropertyGes.rdbGuidId = "<%= rdbGuid.ClientID%>";
                    TbltWorkflowPropertyGes.rtbValueJsonId = "<%= rtbValueJson.ClientID %>";

                    TbltWorkflowPropertyGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    TbltWorkflowPropertyGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    TbltWorkflowPropertyGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    TbltWorkflowPropertyGes.pageContentId = "<%= pnlRinomina.ClientID %>";
                    TbltWorkflowPropertyGes.actionPage = "<% = PageAction %>";
                    TbltWorkflowPropertyGes.argumentType = "<%= ArgumentType %>";
                    TbltWorkflowPropertyGes.argumentsDataSourceId = "<%= argumentsDataSource.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueStringId = "<%= rfvNewValueString.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueBoolId = "<%= rfvNewValueBool.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueDateId = "<%= rfvNewValueDate.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueGuidId = "<%= rfvNewValueGuid.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueJsonId = "<%= rfvNewValueJson.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueDoubleId = "<%= rfvNewValueDouble.ClientID%>";
                    TbltWorkflowPropertyGes.rfvNewValueIntId = "<%= rfvNewValueInt.ClientID%>";

                    TbltWorkflowPropertyGes.validation = "<%= Validation%>";

                    TbltWorkflowPropertyGes.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table class="dataform">
            <tr>
                <td class="label col-dsw-2">Proprietà</td>

                <td class="col-dsw-8">
                 <telerik:RadClientDataSource runat="server" ID="argumentsDataSource" />
                    <telerik:RadComboBox  AllowCustomText="true" runat="server" CausesValidation="false"
                        ID="rcbName" AutoPostBack="false" EnableLoadOnDemand="true"
                        ItemRequestTimeout="500" Width="90%" Filter="Contains" ClientDataSourceID="argumentsDataSource" DataTextField="Name" DataValueField="Value">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="tfvNameWorkflowProperty" runat="server" Display="Dynamic" ErrorMessage="Campo proprietà obbligatorio" ControlToValidate="rcbName" />
                </td>
            </tr>

            <tr>
                <td class="label col-dsw-2">Nome interno</td>

                <td class="col-dsw-8">
                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="90%" ID="rtbPropertyName" AutoPostBack="false" ValidateRequestMode="Disabled" ReadOnly="true" />
                </td>
            </tr>

            <tr>
                <td class="label col-dsw-2" id="labelValue">Valore</td>
                <td class="col-dsw-8" id="idValue" style="display:none;">
                    <telerik:RadNumericTextBox runat="server" CausesValidation="false"
                        ID="rntbValueInt" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        ShowSpinButtons="true" Width="40%">
                        <IncrementSettings Step="1" />
                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                    </telerik:RadNumericTextBox>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueInt" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rntbValueInt" />

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="90%"
                        ID="rtbValueString" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true" ValidateRequestMode="Disabled">
                    </telerik:RadTextBox>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueString" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rtbValueString" />


                    <div id="valueBool">
                        <telerik:RadListBox runat="server" CausesValidation="false"
                            ID="rlbValueBool" AutoPostBack="false" Width="20%">
                            <Items>
                                <telerik:RadListBoxItem Value="0" Text="False" />
                                <telerik:RadListBoxItem Value="1" Text="True" />
                            </Items>
                        </telerik:RadListBox>
                    </div>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueBool" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rlbValueBool" />

                    <div id="valueDate">
                        <telerik:RadDatePicker runat="server" ID="rdpValueDate" Width="40%">
                        </telerik:RadDatePicker>
                    </div>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueDate" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rdpValueDate" />

                    <telerik:RadNumericTextBox runat="server" CausesValidation="false"
                        ID="rntbValueDouble" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        ShowSpinButtons="true" Width="40%">
                        <IncrementSettings Step="0.01" />
                    </telerik:RadNumericTextBox>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueDouble" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rntbValueDouble" />

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="90%"
                        ID="rdbGuid" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true">
                    </telerik:RadTextBox>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueGuid" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rdbGuid" />

                    <telerik:RadTextBox runat="server" CausesValidation="false" Width="90%"
                        ID="rtbValueJson" Rows="5" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                        TextMode="MultiLine">
                    </telerik:RadTextBox>
                    <asp:RequiredFieldValidator Enabled="false" ID="rfvNewValueJson" runat="server" Display="Dynamic" ErrorMessage="Campo valore obbligatorio" ControlToValidate="rtbValueJson" />

                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>
