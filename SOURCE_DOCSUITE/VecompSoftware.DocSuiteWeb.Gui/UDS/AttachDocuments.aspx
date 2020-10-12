<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AttachDocuments.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.AttachDocuments" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            var attachDocuments;
            require(["UDS/AttachDocuments"], function (AttachDocuments) {
                $(function () {
                    attachDocuments = new AttachDocuments(tenantModelConfiguration.serviceConfiguration);
                    attachDocuments.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    attachDocuments.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    attachDocuments.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    attachDocuments.pageContentId = "<%= HeaderWrapepr.ClientID %>";
                    attachDocuments.btnSearchId = "<%= btnSearch.ClientID %>";
                    attachDocuments.tblPreviewId = "<%= tblPreview.ClientID %>";
                    attachDocuments.lblRecordDetailsId = "<%= lblRecordDetails.ClientID %>";
                    attachDocuments.lblDocumentUDSId = "<%= lblDocumentUDS.ClientID %>";
                    attachDocuments.lblObjectId = "<%= lblObject.ClientID %>";
                    attachDocuments.lblCategoryDescriptionId = "<%= lblCategoryDescription.ClientID %>";
                    attachDocuments.btnAddId = "<%= btnAdd.ClientID %>";
                    attachDocuments.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <div runat="server" id="HeaderWrapepr">
        <table class="datatable">
            <tr>
                <th colspan="4">Selezione Archivio da cui copiare i documenti</th>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Cerca" UseSubmitBehavior="false" />
                </td>
            </tr>
        </table>

        <table id="tblPreview" class="datatable" runat="server" style="display: none">
            <tr>
                <th colspan="2">Registrazione in Archivi</th>
            </tr>
            <tr>
                <td class="label" style="width: 15%;">Registrazione: </td>
                <td style="width: 80%;">
                    <asp:Label ID="lblRecordDetails" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Archivio: </td>
                <td>
                    <asp:Label ID="lblDocumentUDS" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Oggetto: </td>
                <td>
                    <asp:Label ID="lblObject" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Classificazione: </td>
                <td>
                    <asp:Label ID="lblCategoryDescription" runat="server" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid runat="server" ID="DocumentListGrid" AllowMultiRowSelection="true" EnableViewState="true" Visible="true">
        <MasterTableView AutoGenerateColumns="false" Width="100%" DataKeyNames="Serialized">
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="selectColumn" HeaderText="Copia" HeaderTooltip="Documenti da copiare" ItemStyle-Width="20px" />
                <telerik:GridTemplateColumn UniqueName="Type" HeaderText="Tipo">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="fileType" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento">
                    <ItemTemplate>
                        <asp:Image ID="fileImage" runat="server" />
                        <asp:Label ID="fileName" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="exportType" HeaderText="Copia conforme">
                    <ItemTemplate>
                        <telerik:RadButton AutoPostBack="false" ButtonType="ToggleButton" Checked="true" ID="pdf" runat="server" ToggleType="CheckBox" ToolTip="Seleziona per la Copia conforme, altrimenti verrà usato l'originale" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
    </telerik:RadGrid>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ID="PageFooter" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnAdd" runat="server" Text="Conferma selezione" style="display: none" UseSubmitBehavior="false" CausesValidation="false" />
</asp:Content>
