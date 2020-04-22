<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscReportDesignerInformation.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscReportDesignerInformation" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscReportDesignerInformation;
        require(["UserControl/uscReportDesignerInformation"], function (UscReportDesignerInformation) {
            $(function () {
                uscReportDesignerInformation = new UscReportDesignerInformation();
                uscReportDesignerInformation.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscReportDesignerInformation.pnlContentId = "<%= pnlContent.ClientID %>";
                uscReportDesignerInformation.rdlEntityId = "<%= rdlEntity.ClientID %>";
                uscReportDesignerInformation.uscMetadataSelId = "<%= uscMetadataSel.PageContentDiv.ClientID %>";
                uscReportDesignerInformation.rdlUDTypeId = "<%= rdlUDType.ClientID %>";
                uscReportDesignerInformation.rowMetadataId = "<%= rowMetadata.ClientID %>";
                uscReportDesignerInformation.rowUDId = "<%= rowUD.ClientID %>";
                uscReportDesignerInformation.txtNameId = "<%= txtName.ClientID %>";
                uscReportDesignerInformation.btnLoadId = "<%= btnLoad.ClientID %>";
                uscReportDesignerInformation.lblCreatedById = "<%= lblCreatedBy.ClientID %>";
                uscReportDesignerInformation.lblCreatedDateId = "<%= lblCreatedDate.ClientID %>";
                uscReportDesignerInformation.lblStatusId = "<%= lblStatus.ClientID %>";
                uscReportDesignerInformation.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<asp:Panel runat="server" ID="pnlContent">
    <telerik:RadSplitter runat="server" ResizeWithParentPane="true" Orientation="Horizontal">
        <telerik:RadPane runat="server" Height="55%" Scrolling="None">
            <asp:Panel runat="server" Height="100%">
                <telerik:RadPanelBar runat="server" Width="100%" Height="100%" ExpandMode="FullExpandedItem">
                    <Items>
                        <telerik:RadPanelItem Text="Dati del report" Expanded="true">
                            <ContentTemplate>
                                <telerik:RadPageLayout runat="server" Width="100%" Height="100%" CssClass="t-container-padding">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item">
                                            <Content>
                                                <b>Nome del report:</b>
                                                <telerik:RadTextBox runat="server" ID="txtName" Width="100%"></telerik:RadTextBox>
                                                <asp:RequiredFieldValidator runat="server" ID="rfvName" Text="Il campo Nome del report è obbligatorio" Display="Dynamic" ControlToValidate="txtName" ValidationGroup="ReportData"></asp:RequiredFieldValidator>
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item">
                                            <Content>
                                                <b>Seleziona una tipologia:</b>
                                                <telerik:RadComboBox runat="server" ID="rdlEntity" Filter="Contains" AllowCustomText="false" Width="100%">
                                                    <Items>
                                                        <telerik:RadComboBoxItem Text="" Value="" />
                                                    </Items>
                                                </telerik:RadComboBox>                                                
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" ID="rowMetadata" CssClass="report-information-item">
                                            <Content>
                                                <b>Seleziona una tipologia di metadati:</b>
                                                <usc:uscMetadataRepositorySel runat="server" ID="uscMetadataSel" ComboBoxAutoWidthEnabled="true" />
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" ID="rowUD" CssClass="report-information-item">
                                            <Content>
                                                <b>Seleziona una unità documentaria:</b>
                                                <telerik:RadComboBox runat="server" ID="rdlUDType" Filter="Contains" Width="100%" AllowCustomText="false">
                                                    <Items>
                                                        <telerik:RadComboBoxItem Text="" Value="" />
                                                    </Items>
                                                </telerik:RadComboBox>
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item">
                                            <Content>
                                                <b>Seleziona un template:</b>
                                                <usc:uscDocumentUpload runat="server" ID="uscTemplateUpload" HeaderVisible="false" ButtonFileEnabled="true" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false"
                                                    ButtonFrontespizioEnabled="false" AllowZipDocument="false" AllowedExtensions=".xlsx" ButtonSecureDocumentEnabled="false" ButtonSelectTemplateEnabled="false" ButtonSharedFolederEnabled="false"
                                                    SignButtonEnabled="false" ButtonPreviewEnabled="false" IsDocumentRequired="false" />
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item" Style="margin-top: 10px;">
                                            <Content>
                                                <telerik:RadButton runat="server" ID="btnLoad" Text="Aggiorna" AutoPostBack="false" ValidationGroup="ReportData"></telerik:RadButton>
                                            </Content>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelBar>
            </asp:Panel>
        </telerik:RadPane>
        <telerik:RadSplitBar runat="server" CollapseMode="None" EnableResize="false"></telerik:RadSplitBar>
        <telerik:RadPane runat="server" Height="45%" Scrolling="None">
            <asp:Panel runat="server" Height="100%">
                <telerik:RadPanelBar runat="server" Width="100%" Height="100%" ExpandMode="FullExpandedItem">
                    <Items>
                        <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                            <ContentTemplate>
                                <telerik:RadPageLayout runat="server" Width="100%" Height="100%" CssClass="t-container-padding">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item">
                                            <Content>
                                                <b>Creato da:</b>
                                                <asp:Label ID="lblCreatedBy" runat="server"></asp:Label>
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item">
                                            <Content>
                                                <b>Creato il:</b>
                                                <asp:Label ID="lblCreatedDate" runat="server"></asp:Label>
                                            </Content>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" CssClass="report-information-item">
                                            <Content>
                                                <b>Stato:</b>
                                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                            </Content>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelBar>
            </asp:Panel>
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Panel>
