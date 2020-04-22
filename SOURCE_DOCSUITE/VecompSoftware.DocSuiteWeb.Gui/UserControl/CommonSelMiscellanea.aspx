<%@ Page AutoEventWireup="false" CodeBehind="CommonSelMiscellanea.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelMiscellanea" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione inserti" %>

<%@ Register Src="../UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var commonSelMiscellanea;
            require(["UserControl/CommonSelMiscellanea"], function (CommonSelMiscellanea) {
                $(function () {
                    commonSelMiscellanea = new CommonSelMiscellanea(tenantModelConfiguration.serviceConfiguration);
                    commonSelMiscellanea.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonSelMiscellanea.txtNoteId = "<%= txtNote.ClientID %>";
                    commonSelMiscellanea.btnSaveId = "<%= btnSave.ClientID%>";
                    commonSelMiscellanea.actionType = "<%= Action %>";
                    commonSelMiscellanea.insertsPageContentId = "<%= insertsPageContent.ClientID %>";
                    commonSelMiscellanea.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    commonSelMiscellanea.initialize();
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
                            <td class="label" style="width: 25%;">Note:</td>
                            <td>
                                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" MaxLength="256" />
                            </td>
                        </tr>
                    </table>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" ID="InsertsDataRow">
                <Rows>
                    <telerik:LayoutRow runat="server" HtmlTag="Div">
                        <Content>
                            <uc1:uscDocumentUpload ID="uscDocumentUpload" Caption="Documento" runat="server" SignButtonEnabled="false" ButtonScannerEnabled="true" ButtonLibrarySharepointEnabled="false" MultipleDocuments='<%# MultiDoc %>' UseSessionStorage="true" Type="Prot" />
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