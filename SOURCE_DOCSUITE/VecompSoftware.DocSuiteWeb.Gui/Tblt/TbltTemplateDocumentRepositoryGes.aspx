<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltTemplateDocumentRepositoryGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTemplateDocumentRepositoryGes" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="UscDocumentUpload" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock2" EnableViewState="false">
        <script type="text/javascript">
            var tbltTemplateDocumentRepositoryGes;
            require(["Tblt/TbltTemplateDocumentRepositoryGes"], function (TbltTemplateDocumentRepositoryGes) {
                $(function () {
                    tbltTemplateDocumentRepositoryGes = new TbltTemplateDocumentRepositoryGes(tenantModelConfiguration.serviceConfiguration);
                    tbltTemplateDocumentRepositoryGes.action = "<%= Action %>";
                    tbltTemplateDocumentRepositoryGes.currentTemplateId = "<%= TemplateId  %>";
                    tbltTemplateDocumentRepositoryGes.pnlMetadataId = "<%= pnlMetadata.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.txtNameId = "<%= txtName.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.rfvNameId = "<%= rfvName.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.txtObjectId = "<%= txtObject.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.acbQualityTagId = "<%= acbQualityTag.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.racTagsDataSourceId = "<%= racTagsDataSource.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.btnPublishId = "<%= btnPublish.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.pnlButtonsId = "<%= pnlButtons.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.btnConfirmUniqueId = "<%= btnConfirm.UniqueID %>";
                    tbltTemplateDocumentRepositoryGes.btnPublishUniqueId = "<%= btnPublish.UniqueID %>";
                    tbltTemplateDocumentRepositoryGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.ajaxFlatLoadingPanelId = "<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>";
                    tbltTemplateDocumentRepositoryGes.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltTemplateDocumentRepositoryGes.templateDocumentRepositoryLocationId = <%= CurrentTemplateDocumentRepositoryLocation.Id %>;
                    tbltTemplateDocumentRepositoryGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
      <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <%-- Metadati Template --%>
    <asp:Panel runat="server" ID="pnlMetadata">
        <table class="datatable">
            <tr>
                <th colspan="2" class="tabella">Dati deposito
                </th>
            </tr>
            <tr class="Chiaro" id="rowName">
                <td class="label" style="width: 25%">Nome:</td>
                <td>
                    <telerik:RadTextBox ID="txtName" runat="server" Width="100%" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" EnableViewState="false" ErrorMessage="Il campo Nome è obbligatorio" Display="Dynamic" ID="rfvName" />
                </td>
            </tr>
            <tr class="Chiaro" id="rowQualityTag">
                <td class="label" style="width: 25%">Tags:</td>
                <td>
                    <telerik:RadClientDataSource runat="server" ID="racTagsDataSource"></telerik:RadClientDataSource>
                    <telerik:RadAutoCompleteBox runat="server" ID="acbQualityTag" Width="100%" DropDownWidth="300px" DropDownHeight="250" RenderMode="Lightweight"
                        AutoPostBack="false" EmptyMessage="Inserire un tag" ClientDataSourceID="racTagsDataSource"
                        AllowCustomEntry="true" DataTextField="value" DataValueField="id">
                    </telerik:RadAutoCompleteBox>
                </td>
            </tr>
            <tr class="Chiaro" id="rowObject">
                <td class="label" style="width: 25%">Oggetto:</td>
                <td>
                    <telerik:RadTextBox ID="txtObject" Rows="3" TextMode="MultiLine" runat="server" Width="100%" />
                </td>
            </tr>
            <%-- Upload Template --%>
            <tr class="Chiaro" id="rowUpload">
                <td class="label" style="width: 25%">Documento:</td>
                <td>
                    <usc:UscDocumentUpload Caption="" HeaderVisible="false" ID="uscUploadDocument" IsDocumentRequired="true" MultipleDocuments="false" runat="server" TreeViewCaption="Documenti" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnConfirm" Width="150px" AutoPostBack="false" Text="Salva Bozza"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnPublish" Width="150px" AutoPostBack="false" Text="Pubblica"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
