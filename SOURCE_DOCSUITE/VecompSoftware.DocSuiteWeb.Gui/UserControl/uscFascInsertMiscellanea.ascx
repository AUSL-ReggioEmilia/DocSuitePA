<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascInsertMiscellanea.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascInsertMiscellanea" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMiscellanea.ascx" TagName="uscMiscellanea" TagPrefix="uc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscFascicleDocumentsInserti;
        require(["UserControl/uscFascInsertMiscellanea"], function (UscFascInsertMiscellanea) {
            $(function () {
                uscFascInsertMiscellanea = new UscFascInsertMiscellanea(tenantModelConfiguration.serviceConfiguration);
                uscFascInsertMiscellanea.currentFascicleId = "<%= IdFascicle %>";
                uscFascInsertMiscellanea.locationId = "<%=ProtocolEnv.FascicleMiscellaneaLocation%>";
                uscFascInsertMiscellanea.ajaxManagerId = "<%= BasePage.AjaxManager.ClientID %>";
                uscFascInsertMiscellanea.currentPageId = "<%= Me.ClientID %>";
                uscFascInsertMiscellanea.radWindowManagerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscFascInsertMiscellanea.ajaxLoadingPanelId = "<%=BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFascInsertMiscellanea.btnUploadDocumentId = "<%= btnUploadDocument.ClientID %>";
                uscFascInsertMiscellanea.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascInsertMiscellanea.uscMiscellaneaId = "<%= uscMiscellanea.PageContentDiv.ClientID %>";
                uscFascInsertMiscellanea.pageContentId = "<%= contentPane.ClientID %>";
                uscFascInsertMiscellanea.btnUploadZipDocumentId = "<%= btnUploadZipDocument.ClientID%>";
                uscFascInsertMiscellanea.archiveName = "<%= ArchiveName%>";
                uscFascInsertMiscellanea.idFascicleFolder = "<%= IdFascicleFolder %>";
                uscFascInsertMiscellanea.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>



<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerCollegamenti" runat="server">
    <Windows>
    </Windows>
</telerik:RadWindowManager>

<div class="splitterWrapper">
    <telerik:RadSplitter runat="server" Height="100%" Width="100%" ID="splContent" Orientation="Horizontal" ResizeWithParentPane="false">
        <telerik:RadPane runat="server" Height="100%" Scrolling="Y" ID="contentPane">
            <telerik:RadPageLayout runat="server" Width="100%" ID="pageContent" HtmlTag="Div">
                <Rows>
                    <telerik:LayoutRow Height="100%" HtmlTag="Div">
                        <Content>
                            <uc:uscMiscellanea ID="uscMiscellanea" runat="server" />
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:RadPageLayout>
        </telerik:RadPane>
        <telerik:RadPane runat="server" Scrolling="None" Height="40px" ID="radPaneButton">
            <div style="border-top:1px solid #d9d9d9;">
                <div style="margin: 10px 3px;">
                    <telerik:RadButton ID="btnUploadDocument" runat="server" AutoPostBack="false" Width="150px" Text="Aggiungi" />
                    <telerik:RadButton ID="btnUploadZipDocument" runat="server" AutoPostBack="false" Width="150px" Text="Carica da ZIP" />
                </div>
            </div>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>
<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>