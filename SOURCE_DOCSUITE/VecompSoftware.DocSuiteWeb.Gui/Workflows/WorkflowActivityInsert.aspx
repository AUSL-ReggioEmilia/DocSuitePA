<%@ Page Language="vb" Title="Nuova attività" AutoEventWireup="false" CodeBehind="WorkflowActivityInsert.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.WorkflowActivityInsert" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register Src="~/UserControl/uscUploadDocumentRest.ascx" TagPrefix="usc" TagName="uscUploadDocumentRest" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="ContattiSel" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var workflowActivityInsert;
            require(["Workflows/WorkflowActivityInsert"], function (WorkflowActivityInsert) {
                $(function () {
                    workflowActivityInsert = new WorkflowActivityInsert(tenantModelConfiguration.serviceConfiguration);
                    workflowActivityInsert.ddlWorkflowActivityId = "<%= ddlWorkflowActivity.ClientID %>";
                    workflowActivityInsert.rblPriorityId = "<%= rblPriority.ClientID %>";
                    workflowActivityInsert.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    workflowActivityInsert.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientId %>";
                    workflowActivityInsert.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    workflowActivityInsert.pnlWorkflowActivityInsertId = "<%= pnlWorkflowActivityInsert.ClientID %>";
                    workflowActivityInsert.uscDestinatariId = "<%= uscDestinatari.ClientID %>";
                    workflowActivityInsert.uscSettoriId = "<%= uscDestinatari.TableContent.ClientID %>";
                    workflowActivityInsert.uscDocumentId = "<%= uscUploadDocumentRest.uploadDocumentComponent.ClientID %>";
                    workflowActivityInsert.tenantName = "<%= TenantName %>";
                    workflowActivityInsert.tenantId = "<%= TenantId %>";
                    workflowActivityInsert.txtNoteId = "<%= txtNote.ClientID %>";
                    workflowActivityInsert.fullUserName = <%= FullUserName %>;
                    workflowActivityInsert.fullName = <%= FullName %>;
                    workflowActivityInsert.email = "<%= Email %>";
                    workflowActivityInsert.dataScadentaId = "<%= dataScadenza.ClientID %>"
                    workflowActivityInsert.docSuiteVersion = "<%=DSWVersion%>";
                    workflowActivityInsert.idTenantAOO = "<%= If(CurrentTenant Is Nothing, String.Empty, CurrentTenant.TenantAOO.UniqueId) %>";

                    workflowActivityInsert.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlWorkflowActivityInsert">
        <asp:Panel ID="pnlNuovaAttivita" runat="server">
            <table class="datatable" id="tblSeries">                
                <tr>
                    <td class="label" style="width: 15%">Nome attività:
                    </td>
                    <td style="width: 60%">
                        <telerik:RadDropDownList AppendDataBoundItems="true" AutoPostBack="False" CausesValidation="True" ID="ddlWorkflowActivity" Width="350px" runat="server" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlWorkflowActivity"
                                ErrorMessage="E' obbligatorio selezionare una attività" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;</td>
                </tr>
                <tr>
                    <td class="label" style="width: 15%">Priorità:
                    </td>
                    <td style="width: 60%">
                        <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rblPriority">
                            <asp:ListItem Text="Normale" Value="0" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Bassa" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Alta" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlDocumenti" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="3" style="text-align: left">Documento
                    </th>
                </tr>
                <tr>
                    <td class="label" style="width: 15%">
                        Documenti:
                    </td>
                    <td style="width: 60%">                        
                        <usc:uscUploadDocumentRest MultipleUploadEnabled="true" ID="uscUploadDocumentRest" runat="server" UseSessionStorage="true" />
                    </td>
                    <td style="width: 21%; white-space: nowrap">&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlDesstinatario" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="3" style="text-align: left">Destinatario
                    </th>
                </tr>
                <tr>
                    <td class="label" style="width: 15%">Destinatari:
                    </td>
                    <td style="width: 60%">
                        <usc:ContattiSel ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" 
                            ButtonPropertiesVisible="false" ButtonRoleVisible="false"  ButtonSelectOChartVisible="false" ButtonSelectVisible="false" 
                            EnableCheck="True" EnableViewState="true" HeaderVisible="false" ID="uscDestinatari" IsRequired="true" Multiple="true" 
                            runat="server" TreeViewCaption="Destinatari" UseAD="true" UseSessionStorage="true" />
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlDati" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="3" style="text-align: left">Dati
                    </th>
                </tr>
                <tr>
                    <td class="label" style="width: 15%">Data di scadenza:
                    </td>
                    <td style="width: 60%">
                        <telerik:RadDatePicker ID="dataScadenza" runat="server"></telerik:RadDatePicker>
                    </td>
                    <td style="width: 20%; white-space: nowrap">&nbsp;</td>
                </tr>
                <tr>
                    <td class="label" style="width: 15%">Oggetto:
                    </td>
                    <td>
                        <telerik:RadTextBox runat="server" ID="txtNote" CausesValidation="true" EmptyMessage="Campo note" Width="100%" TextMode="MultiLine" Rows="3"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNote"
                                ErrorMessage="Campo note di avvio obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadAjaxPanel runat="server">
    <telerik:RadButton ID="btnConfirm" runat="server" CausesValidation="true" Text="Conferma" AutoPostBack="false" />
    </telerik:RadAjaxPanel>
</asp:Content>