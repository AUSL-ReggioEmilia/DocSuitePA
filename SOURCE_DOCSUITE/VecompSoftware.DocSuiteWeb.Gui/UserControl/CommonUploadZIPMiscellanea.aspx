<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonUploadZIPMiscellanea.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonUploadZIPMiscellanea" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione inserti (ZIP)" %>

<%@ Register Src="../UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var commonUploadZIPMiscellanea;
            require(["UserControl/CommonUploadZIPMiscellanea"], function (CommonUploadZIPMiscellanea) {
                $(function () {
                    commonUploadZIPMiscellanea = new CommonUploadZIPMiscellanea(tenantModelConfiguration.serviceConfiguration);
                    commonUploadZIPMiscellanea.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonUploadZIPMiscellanea.txtPrefixId = "<%= txtPrefix.ClientID %>";
                    commonUploadZIPMiscellanea.btnSaveId = "<%= btnSave.ClientID%>";
                    commonUploadZIPMiscellanea.actionType = "<%= Action %>";
                    commonUploadZIPMiscellanea.insertsPageContentId = "<%= insertsPageContent.ClientID %>";
                    commonUploadZIPMiscellanea.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    commonUploadZIPMiscellanea.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadPageLayout runat="server" ID="insertsPageContent" HtmlTag="Div" Width="100%">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div" Style="vertical-align: middle;" ID="tblNoteRow">
                <Content>
                    <table class="datatable">
                        <tr>
                            <td class="label" style="width: 25%;">Prefisso:</td>
                            <td>
                                <telerik:RadTextBox ID="txtPrefix" runat="server" Width="100%" MaxLength="256" />
                            </td>
                        </tr>
                    </table>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" ID="InsertsDataRow">
                <Rows>
                    <telerik:LayoutRow runat="server" HtmlTag="Div">
                        <Content>
                            <uc1:uscDocumentUpload ID="uscDocumentUpload" Caption="Documento" runat="server" SignButtonEnabled="false" ButtonScannerEnabled="true" ButtonLibrarySharepointEnabled="false" MultipleDocuments='<%# MultiDoc %>' HideScannerMultipleDocumentButton='<%# MultiDoc %>' UseSessionStorage="true" Type="Prot" AllowZipDocument="true" AllowedExtensions=".zip" />
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton ID="btnSave" runat="server" Width="150px" Text="Conferma"></telerik:RadButton>
    </asp:Panel>
</asp:Content>