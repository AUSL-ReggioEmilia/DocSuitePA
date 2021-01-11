<%@ Page Title="Caselle PEC" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltPECMailBox.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltPECMailBox" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscPECMailBoxSettings.ascx" TagName="uscPECMailBoxSettings" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var tbltPECMailBox;
            require(["Tblt/TbltPECMailBox"],
                function (TbltPECMailBox) {
                    $(function () {
                        tbltPECMailBox = new TbltPECMailBox(tenantModelConfiguration.serviceConfiguration);
                        tbltPECMailBox.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                        tbltPECMailBox.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                        tbltPECMailBox.splitterMainId = "<%= splitterMain.ClientID %>";
                        tbltPECMailBox.ToolBarSearchId = "<%= ToolBarSearch.ClientID %>";
                        tbltPECMailBox.rtvPECMailBoxesId = "<%=rtvPECMailBoxes.ClientID %>";
                        tbltPECMailBox.rpbDetailsId = "<%= rpbDetails.ClientID %>";
                        tbltPECMailBox.lblPECMailBoxIdId = "<%= lblPECMailBoxId.ClientID %>";
                        tbltPECMailBox.lblMailBoxRecipientId = "<%= lblMailBoxRecipient.ClientID %>";
                        tbltPECMailBox.lblIncomingServerId = "<%= lblIncomingServer.ClientID %>";
                        tbltPECMailBox.lblOutgoingServerId = "<%= lblOutgoingServer.ClientID %>";
                        tbltPECMailBox.lblRulesetNameId = "<%= lblRulesetName.ClientID %>";
                        tbltPECMailBox.lblRulesetConditionId = "<%= lblRulesetCondition.ClientID %>";
                        tbltPECMailBox.lblRulesetTypeId = "<%= lblRulesetType.ClientID %>";
                        tbltPECMailBox.btnPECMailBoxSetRuleId = "<%= btnPECMailBoxSetRule.ClientID %>";
                        tbltPECMailBox.windowSetRuleId = "<%= windowSetRule.ClientID %>";
                        tbltPECMailBox.txtRulesetNameId = "<%= txtRulesetName.ClientID %>";
                        tbltPECMailBox.txtSpecifySenderId = "<%= txtSpecifySender.ClientID %>";
                        tbltPECMailBox.rlbSpecifyPECMailBoxId = "<%= rlbSpecifyPECMailBox.ClientID %>";
                        tbltPECMailBox.btnPECMAilBoxSaveId = "<%= btnPECMAilBoxSave.ClientID %>";
                        tbltPECMailBox.cmdUpdatePECId = "<%= cmdUpdatePEC.ClientID %>";
                        tbltPECMailBox.cmdAddPECMailBoxId = "<%= cmdAddPECMailBox.ClientID %>";
                        tbltPECMailBox.uscPECMailBoxSettingsId = "<%= uscPECMailBoxSettings.PageContent.ClientID %>";
                        tbltPECMailBox.hfPECMailBoxIdId = "<%= uscPECMailBoxSettings.PECMailBoxId.ClientID %>";
                        tbltPECMailBox.initialize();
                    });
                });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager runat="server" EnableViewState="False" ID="RadWindowManagerGroups">
        <Windows>
            <telerik:RadWindow runat="server" ID="windowSetRule" Title="Imposta regola" Width="600" Height="190px" Behaviors="Close">
                <ContentTemplate>
                    <table id="ammTraspMonitorLogWindowTable" class="datatable monitorLog" style="margin-bottom: 0">
                        <tr>
                            <td class="label"></td>
                            <td>Spostamento automatico - mittente</td>
                        </tr>
                        <tr>
                            <td class="label">Nome regola</td>
                            <td>
                                <telerik:RadTextBox ID="txtRulesetName" runat="server" Width="90%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="txtRulesetNameValidator" Display="Dynamic" ControlToValidate="txtRulesetName" ErrorMessage="Il nome della regola non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Specificare mittente</td>
                            <td>
                                <telerik:RadTextBox ID="txtSpecifySender" runat="server" Width="90%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"></td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="txtSpecifySenderValidator" Display="Dynamic" ControlToValidate="txtSpecifySender" ErrorMessage="Il mittente non può essere vuoto!" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Specificare caselle di PEC</td>
                            <td>
                                <telerik:RadDropDownList runat="server" ID="rlbSpecifyPECMailBox" Width="300px" AutoPostBack="False" selected="true" DropDownHeight="200px">
                                    <Items>
                                        <telerik:DropDownListItem Text="" Value="" />
                                    </Items>
                                </telerik:RadDropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="buttons">
                                <telerik:RadButton ID="btnPECMAilBoxSave" runat="server" Text="Salva" Width="100px" AutoPostBack="false" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" ResizeWithParentPane="False" Height="100%">
            <telerik:RadPane runat="server" ID="paneSelection" Width="50%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Height="100%" Width="100%" Orientation="Horizontal">
                    <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="Y">
                        <%--OnButtonClick=""--%>
                        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchName">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtName" runat="server" Width="170px" AutoPostBack="False"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="rtvPECMailBoxes" runat="server" Style="margin-top: 10px;" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" Selected="true" runat="server" Font-Bold="true" Text="Caselle PEC" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>

            <telerik:RadPane runat="server" Width="50%">
                <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel" Style="margin-top: 5px;">
                    <div class="dsw-panel-content">
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbDetails">
                            <Items>
                                <telerik:RadPanelItem Value="rpiDetails" Text="Dettagli" Expanded="true" runat="server">
                                    <ContentTemplate>
                                        <table id="PECMailBoxWindowTable" class="datatable pec" style="height: 100%; margin-bottom: 0; table-layout: fixed">
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="lblPECMailBoxId" runat="server" Width="100%" ReadOnly="true"
                                                        Visible="true" ClientIDMode="Static" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">Nome Casella PEC</td>
                                                <td>
                                                    <asp:Label ID="lblMailBoxRecipient" runat="server" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">Informazioni server ingoing</td>
                                                <td>
                                                    <asp:Label ID="lblIncomingServer" runat="server" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">Informazioni server outgoing</td>
                                                <td>
                                                    <asp:Label ID="lblOutgoingServer" runat="server" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">Nome regola</td>
                                                <td>
                                                    <asp:Label ID="lblRulesetName" runat="server" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">Condizione regola</td>
                                                <td>
                                                    <asp:Label ID="lblRulesetCondition" runat="server" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="label">Tipo di regola</td>
                                                <td>
                                                    <asp:Label ID="lblRulesetType" runat="server" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" class="buttons">
                                                    <telerik:RadButton ID="btnPECMailBoxSetRule" runat="server" Text="Imposta regola" Width="100px" AutoPostBack="false" Enabled="False" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </div>
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server">
        <asp:Button ID="cmdUpdatePEC" runat="server" Text="Modifica casella PEC" Width="150" />
        <asp:Button ID="cmdAddPECMailBox" runat="server" Text="Aggiungi casella PEC" Width="150" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlPECMailBoxSettings">
        <usc:uscPECMailBoxSettings runat="server" ID="uscPECMailBoxSettings" />
    </asp:Panel>
</asp:Content>
