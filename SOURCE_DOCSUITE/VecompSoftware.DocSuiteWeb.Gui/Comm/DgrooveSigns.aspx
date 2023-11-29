<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DgrooveSigns.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DgrooveSigns" Title="Firma" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            window.pdfjsLib.GlobalWorkerOptions.workerSrc = '../Scripts/pdf.worker.js';
        </script>
        <script type="text/javascript">
            var dgrooveSigns;
            require(["Comm/DgrooveSigns"], function (DgrooveSigns) {
                $(function () {
                    dgrooveSigns = new DgrooveSigns(tenantModelConfiguration.serviceConfiguration);
                    dgrooveSigns.currentUserDomain = <%= CurrentUserDomain %>;
                    dgrooveSigns.currentUserTenantName = "<%= CurrentUserTenantName %>";
                    dgrooveSigns.currentUserTenantId = "<%= CurrentUserTenantId %>";
                    dgrooveSigns.currentUserTenantAOOId = "<%= CurrentUserTenantAOOId %>";
                    dgrooveSigns.signalrDgrooveSignerUrl = "<%= SignalrDgrooveSignerUrl %>";
                    dgrooveSigns.signalrWebApiUrl = "<%= SignalrWebApiUrl %>";
                    dgrooveSigns.dswBaseUrl = "<%= DSWUrl %>";
                    dgrooveSigns.hasAruba = "<%= HasAruba %>";
                    dgrooveSigns.hasInfocert = "<%= HasInfocert %>";
                    dgrooveSigns.workflowArchiveName = "<%= WorkflowArchiveName %>";
                    dgrooveSigns.collaborationArchiveName = "<%= CollaborationArchiveName %>";
                    dgrooveSigns.signToolBarId = "<%= signToolBar.ClientID %>";
                    dgrooveSigns.documentsTreeId = "<%= documentsTree.ClientID %>";
                    dgrooveSigns.documentViewerId = "<%= documentViewer.ClientID %>";
                    dgrooveSigns.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dgrooveSigns.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                    dgrooveSigns.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dgrooveSigns.docTreePaneId = "<%= docTreePane.ClientID %>";
                    dgrooveSigns.radWindowManagerSigner = "<%= radWindowManagerSigner.ClientID %>";
                    dgrooveSigns.radListMessagesId = ("<%= radListMessages.ClientID %>");
                    dgrooveSigns.radNotificationInfoId = ("<%= radNotificationInfo.ClientID %>");
                    dgrooveSigns.notificationSignToolBarId = ("<%= notificationToolBar.ClientID %>");
                    dgrooveSigns.defaultSignType = "<%= DefaultSignType %>";
                    dgrooveSigns.initialize();
                });
            });
        </script>
    </telerik:RadCodeBlock>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadNotification ID="radNotificationInfo" runat="server" VisibleOnPageLoad="false" LoadContentOn="PageLoad"
        Width="400" Height="200" Animation="FlyIn" EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni Pagina" Position="Center" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" Height="100%" Width="100%" ID="pnlMainContent">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" BorderStyle="None" BorderSize="0" PanesBorderSize="0" Orientation="Horizontal" ResizeWithParentPane="True" ResizeWithBrowserWindow="True">
            <telerik:RadPane ID="paneToolbar" runat="server" Width="100%" Scrolling="None" Height="35px">
                <telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="signToolBar" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton ID="select" Value="selectAll" ToolTip="Seleziona / Deseleziona" Text="Seleziona / Deseleziona" />
                        <telerik:RadToolBarButton ID="selectInvert" Value="selectInvert" ToolTip="Inverti selezione" Text="Inverti selezione" />
                        <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="providerSignType">
                            <ItemTemplate>
                                <telerik:RadDropDownList runat="server" ID="providerSignType" Width="160px" AutoPostBack="false" >
                                    <Items>
                                        <telerik:DropDownListItem Selected="true" Text="Smartcard" Value="0"/>
                                    </Items>
                                </telerik:RadDropDownList>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton ID="cadesBtn" Value="cades" ToolTip="Cades" CheckOnClick="true" Text="CAdES" Enabled="true" Group="signToggle" 
                            ImageUrl="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png"/>
                        <telerik:RadToolBarButton ID="padesBtn" Value="pades" ToolTip="Pades" CheckOnClick="true" Text="PAdES" Enabled="true" Group="signToggle"
                            ImageUrl="../App_Themes/DocSuite2008/imgset16/file_extension_pdf_signed.png" />
                        <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Text="Richiedi OTP" ToolTip="Richiedi OTP" Value="otp" />
                        <telerik:RadToolBarButton Value="otpInput">
                            <ItemTemplate>        
                                <telerik:RadTextBox ID="otpInput" runat="server" TextMode="Password" Width="85px" />
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton ID="signBtn" Value="sign" ToolTip="Firma" Text="Firma" Enabled="true"
                            ImageUrl="~/App_Themes/DocSuite2008/imgset16/text_signature.png" />
                        <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton ID="comment" Value="comment">
                            <ItemTemplate>
                                <telerik:RadButton ID="btnAddComment" runat="server" ToggleType="CheckBox" ButtonType="LinkButton" AutoPostBack ="false" Checked="false">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState Text="Firma con funzioni vicariali" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                                        <telerik:RadButtonToggleState Text="Firma con funzioni vicariali" PrimaryIconCssClass="rbToggleCheckbox" />
                                    </ToggleStates>
                                </telerik:RadButton>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton ID="next" Value="next" >
                            <ItemTemplate>
                                <telerik:RadButton ID="btnToggleNext" runat="server" ToggleType="CheckBox" ButtonType="LinkButton" AutoPostBack ="false" Checked="false" >
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState Text="Prosegui dopo la firma" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                                        <telerik:RadButtonToggleState Text="Prosegui dopo la firma" PrimaryIconCssClass="rbToggleCheckbox" />
                                    </ToggleStates>
                                </telerik:RadButton>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
            </telerik:RadPane>

            <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" Height="100%" Scrolling="None">
                    <telerik:RadPane runat="server" Width="40%" Height="100%" Scrolling="None" ID="docTreePane">

                        <telerik:RadTreeView ID="documentsTree" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%" CheckBoxes="True" TriStateCheckBoxes="true" CheckChildNodes="true" >
                        </telerik:RadTreeView>

                    </telerik:RadPane>

                    <telerik:RadSplitBar runat="server" ID="Bar1" />
                    <telerik:RadPane runat="server" Width="60%" Height="100%" Scrolling="None" ID="pdfViewerPane">
                        <telerik:RadPdfViewer runat="server" ID="documentViewer" Width="100%" Height="100%">
                            <ToolBarSettings Items="pager, spacer, zoom, search, toggleSelection" />
                        </telerik:RadPdfViewer>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </asp:Panel>

    <telerik:RadWindowManager EnableViewState="False" ID="radWindowManagerSigner" runat="server">
        <Windows>
            <telerik:RadWindow Height="450" ID="windowNotification" runat="server" Title="Esito attività" Width="600" >
                <ContentTemplate>
                    <telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="notificationToolBar" runat="server" Width="100%">
                        <Items>
                            <telerik:RadToolBarButton Text="Richiedi OTP" ToolTip="Richiedi OTP" Value="otp" />
                            <telerik:RadToolBarButton Value="notificationOtpInput">
                                <ItemTemplate>        
                                    <telerik:RadTextBox ID="notificationOtpInput" runat="server" TextMode="Password" Width="85px" />
                                </ItemTemplate>
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Firma" ToolTip="Firma" Value="sign" Enabled="true" ImageUrl="~/App_Themes/DocSuite2008/imgset16/text_signature.png" />
                        </Items>
                    </telerik:RadToolBar>
                    <telerik:RadListBox RenderMode="Lightweight" ID="radListMessages" runat="server" Height="90%" Width="100%" SelectionMode="Single"/>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

