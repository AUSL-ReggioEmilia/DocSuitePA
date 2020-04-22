<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTemplateDocumentRepository" CodeBehind="TbltTemplateDocumentRepository.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscTemplateDocumentRepository.ascx" TagName="TemplateDocumentRepository" TagPrefix="usc" %>
<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var tbltTemplateDocumentRepository;
            require(["Tblt/TbltTemplateDocumentRepository"], function (TbltTemplateDocumentRepository) {
                $(function () {
                    tbltTemplateDocumentRepository = new TbltTemplateDocumentRepository(tenantModelConfiguration.serviceConfiguration);
                    tbltTemplateDocumentRepository.uscTemplateDocumentRepositoryId = "<%= uscTemplateDocumentRepository.TreeTemplateDocumentRepository.ClientID %>";
                    tbltTemplateDocumentRepository.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                    tbltTemplateDocumentRepository.lblVersionId = "<%= lblVersion.ClientID %>";
                    tbltTemplateDocumentRepository.lblStatusId = "<%= lblStatus.ClientID %>";
                    tbltTemplateDocumentRepository.lblObjectId = "<%= lblObject.ClientID %>";
                    tbltTemplateDocumentRepository.btnAggiungiId = "<%= btnAggiungi.ClientID %>";
                    tbltTemplateDocumentRepository.btnEliminaId = "<%= btnElimina.ClientID %>";
                    tbltTemplateDocumentRepository.btnModificaId = "<%= btnModifica.ClientID %>";
                    tbltTemplateDocumentRepository.btnVisualizzaId = "<%= btnVisualizza.ClientID %>";
                    tbltTemplateDocumentRepository.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltTemplateDocumentRepository.splitterPageId = "<%= splContent.ClientID %>";
                    tbltTemplateDocumentRepository.lblTagsId = "<%= lblTags.ClientID %>";
                    tbltTemplateDocumentRepository.lblIdentifierId = "<%= lblIdentifier.ClientID %>";
                    tbltTemplateDocumentRepository.previewSplitterId = "<%= previewSplitter.ClientID %>";
                    tbltTemplateDocumentRepository.previewPaneId = "<%= previewPane.ClientID %>";
                    tbltTemplateDocumentRepository.pnlInformationsId = "<%= pnlInformations.ClientID %>";
                    tbltTemplateDocumentRepository.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    tbltTemplateDocumentRepository.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    tbltTemplateDocumentRepository.btnEliminaUniqueId = "<%= btnElimina.UniqueID %>";
                    tbltTemplateDocumentRepository.radWindowManagerId = "<%= RadWindowManager.ClientID %>";
                    tbltTemplateDocumentRepository.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltTemplateDocumentRepository.btnLogId = "<%= btnLog.ClientID %>";
                    tbltTemplateDocumentRepository.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManager" Width="700px" Height="400px" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowManageTemplate" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Gestione Deposito Documentale" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowLogTemplate" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Log Deposito Documentale" />
        </Windows>
    </telerik:RadWindowManager>

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" Orientation="Horizontal" ID="splPage">
            <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">
                    <telerik:RadPane runat="server" Width="50%">
                        <usc:TemplateDocumentRepository runat="server" ID="uscTemplateDocumentRepository"></usc:TemplateDocumentRepository>
                    </telerik:RadPane>
                    <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
                    <telerik:RadPane runat="server" Width="50%">
                        <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="margin-top: 5px;">
                            <div class="dsw-panel-content">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                            <ContentTemplate>
                                                <asp:Panel runat="server" ID="pnlInformations">
                                                    <div class="col-dsw-10">

                                                        <div class="leftLabel">
                                                            <b class="labelPadding-7">Oggetto: </b>
                                                        </div>
                                                        <div class="rightValue">
                                                            <asp:Label ID="lblObject" runat="server" />
                                                        </div>

                                                        <div class="leftLabel">
                                                            <b class="labelPadding-7">Versione:</b>
                                                        </div>
                                                        <div class="rightValue">
                                                            <asp:Label ID="lblVersion" runat="server" />
                                                        </div>

                                                        <div class="leftLabel">
                                                            <b class="labelPadding-7">Stato:</b>
                                                        </div>
                                                        <div class="rightValue">
                                                            <asp:Label ID="lblStatus" runat="server" />
                                                        </div>

                                                        <div class="leftLabel">
                                                            <b class="labelPadding-7">Tag:</b>
                                                        </div>
                                                        <div class="rightValue">
                                                            <asp:Label runat="server" ID="lblTags" />
                                                        </div>

                                                        <div class="leftLabel">
                                                            <b class="labelPadding-7">Identificativo univoco:</b>
                                                        </div>
                                                        <div class="rightValue">
                                                            <asp:Label runat="server" ID="lblIdentifier" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                        <telerik:RadPanelItem Text="Anteprima documento" Expanded="true">
                                            <ContentTemplate>
                                                <div class="splitterWrapper">
                                                    <telerik:RadSplitter runat="server" Width="100%" ResizeWithParentPane="False" ResizeMode="Proportional" ID="previewSplitter">
                                                        <telerik:RadPane runat="server" Width="100%" ID="previewPane" ShowContentDuringLoad="true" />
                                                    </telerik:RadSplitter>
                                                </div>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton ID="btnAggiungi" runat="server" Text="Aggiungi" CausesValidation="false" AutoPostBack="false" />
        <telerik:RadButton ID="btnModifica" runat="server" Text="Modifica" Enabled="false" AutoPostBack="false" />
        <telerik:RadButton ID="btnElimina" runat="server" Text="Elimina" Enabled="false" AutoPostBack="false" />
        <telerik:RadButton ID="btnLog" runat="server" Text="Log" Enabled="false" AutoPostBack="false" />
        <telerik:RadButton ID="btnVisualizza" runat="server" Text="Visualizza" Enabled="false" AutoPostBack="false" />
    </asp:Panel>
</asp:Content>
