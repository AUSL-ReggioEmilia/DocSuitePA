<%@ Page AutoEventWireup="false" CodeBehind="FascDocumentsInsert.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascDocumentsInsert" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione documenti" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<%@ Register Src="~/UserControl/uscFascInsertMiscellanea.ascx" TagName="uscFascInsertMiscellanea" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscFascInsertUD.ascx" TagName="uscFascInsertUD" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">

    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var fascDocumentsInsert;
            require(["Fasc/FascDocumentsInsert"], function (FascDocumentsInsert) {
                $(function () {
                    fascDocumentsInsert = new FascDocumentsInsert(tenantModelConfiguration.serviceConfiguration);
                    fascDocumentsInsert.currentFascicleId = "<%= IdFascicle %>";
                    fascDocumentsInsert.locationId = "<%= ProtocolEnv.FascicleMiscellaneaLocation%>";
                    fascDocumentsInsert.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascDocumentsInsert.currentPageId = "<%= Me.ClientID %>";
                    fascDocumentsInsert.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascDocumentsInsert.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascDocumentsInsert.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascDocumentsInsert.radNotificationInfoId = "<%= radNotificationInfo.ClientID %>";
                    fascDocumentsInsert.archiveName = "<%=ArchiveName%>";
                    fascDocumentsInsert.idFascicleFolder = "<%= IdFascicleFolder %>";
                    fascDocumentsInsert.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>

</asp:Content>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splContent" ResizeWithParentPane="false" Height="100%" Width="100%" Orientation="Horizontal">
            <telerik:RadPane runat="server" Height="40px" Scrolling="None" ID="radPane">
                <telerik:RadTabStrip RenderMode="Lightweight" runat="server" MultiPageID="rmpPages" SelectedIndex="0" Skin="Silk" Align="left">
                    <Tabs>
                        <telerik:RadTab Text="Documenti" Width="150px" Value="Documenti" />
                        <telerik:RadTab Text="Inserti" Width="150px" Value="Inserti" />
                    </Tabs>
                </telerik:RadTabStrip>
            </telerik:RadPane>
            <telerik:RadSplitBar runat="server" CollapseMode="None" EnableResize="false"></telerik:RadSplitBar>
            <telerik:RadPane runat="server" Scrolling="Y">
                <telerik:RadMultiPage runat="server" ID="rmpPages" SelectedIndex="0" Height="100%">
                    <telerik:RadPageView runat="server" ID="UDInsertView" Height="100%">
                        <uc:uscFascInsertUD ID="uscFascInsertUD" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView runat="server" ID="MiscellaneaInsertView" Height="100%">
                        <uc:uscFascInsertMiscellanea ID="uscFascInsertMiscellanea" runat="server" />
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadNotification ID="radNotificationInfo" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="300" Height="150" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni documento del Fascicolo" TitleIcon="none" AutoCloseDelay="4000" Position="Center" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
</asp:Content>



