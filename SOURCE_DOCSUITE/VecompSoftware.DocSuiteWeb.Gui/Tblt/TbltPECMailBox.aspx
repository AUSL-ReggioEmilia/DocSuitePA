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
                        tbltPECMailBox.lblMailBoxRecipientId = "<%= lblMailBoxRecipient.ClientID %>";
                        tbltPECMailBox.lblIncomingServerId = "<%= lblIncomingServer.ClientID %>";
                        tbltPECMailBox.lblOutgoingServerId = "<%= lblOutgoingServer.ClientID %>";
                        tbltPECMailBox.lblRulesetNameId = "<%= lblRulesetName.ClientID %>";
                        tbltPECMailBox.lblRulesetConditionId = "<%= lblRulesetCondition.ClientID %>";
                        tbltPECMailBox.lblRulesetTypeId = "<%= lblRulesetType.ClientID %>";
                        tbltPECMailBox.windowSetRuleId = "<%= windowSetRule.ClientID %>";
                        tbltPECMailBox.windowInsertId = "<%= windowInsert.ClientID %>";
                        tbltPECMailBox.txtRulesetNameId = "<%= txtRulesetName.ClientID %>";
                        tbltPECMailBox.txtSpecifySenderId = "<%= txtSpecifySender.ClientID %>";
                        tbltPECMailBox.rlbSpecifyPECMailBoxId = "<%= rlbSpecifyPECMailBox.ClientID %>";
                        tbltPECMailBox.btnPECMAilBoxSaveId = "<%= btnPECMAilBoxSave.ClientID %>";
                        tbltPECMailBox.uscPECMailBoxSettingsId = "<%= uscPECMailBoxSettings.PageContent.ClientID %>";
                        tbltPECMailBox.uscPECMailBoxSettingsInsertId = "<%= uscPECMailBoxSettingsInsert.PageContent.ClientID %>";
                        tbltPECMailBox.folderToolBarId = "<%= FolderToolBar.ClientID %>";
                        tbltPECMailBox.lblUsernameId = "<%= lblUsername.ClientID %>";
                        tbltPECMailBox.lblServerTypeId = "<%= lblServerType.ClientID %>";
                        tbltPECMailBox.lblOUTPortId = "<%= lblOUTPort.ClientID %>";
                        tbltPECMailBox.lblProfileId = "<%= lblProfile.ClientID %>";
                        tbltPECMailBox.lblJeepServINId = "<%= lblJeepServIN.ClientID %>";
                        tbltPECMailBox.lblJeepServOUTId = "<%= lblJeepServOUT.ClientID %>";
                        tbltPECMailBox.lblElectronicTypeId = "<%= lblElectronicType.ClientID %>";
                        tbltPECMailBox.lblINPortId = "<%= lblINPort.ClientID%>";
                        tbltPECMailBox.lblLocationId = "<%= lblLocation.ClientID%>";
                        tbltPECMailBox.lblIsInteropId = "<%=lblIsInterop.ClientID%>";
                        tbltPECMailBox.lblIsProtocolId = "<%=lblIsProtocol.ClientID%>";
                        tbltPECMailBox.lblIsPublicProtocolId = "<%=lblIsPublicProtocol.ClientID%>";
                        tbltPECMailBox.lblINSSLId = "<%=lblINSSL.ClientID%>";
                        tbltPECMailBox.lblOUTSSLId = "<%=lblOUTSSL.ClientID%>";
                        tbltPECMailBox.lblIsManagedId = "<%=lblIsManaged.ClientID%>";
                        tbltPECMailBox.lblIsNotManagedId = "<%=lblIsNotManaged.ClientID%>";
                        tbltPECMailBox.lblIsHandleEnabledId = "<%=lblIsHandleEnabled.ClientID%>";
                        tbltPECMailBox.treeViewNodesPageSize = <%= ProtocolEnv.TreeViewNodesPageSize %>;
                        tbltPECMailBox.viewLoginError = "<%=ViewLoginError %>"

                        tbltPECMailBox.initialize();
                    });
                });
        </script>

        <style type="text/css">
            #ctl00_cphContent_rtvPECMailBoxes {
                height: 88%;
            }
        </style>
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

            <telerik:RadWindow runat="server" ID="windowInsert" Title="Aggiungi PEC" Width="600" Height="600px" Behaviors="Maximize,Close,Resize">
                <ContentTemplate>
                    <div id="insertPEC">
                        <usc:uscPECMailBoxSettings runat="server" ID="uscPECMailBoxSettingsInsert" />
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" ResizeWithParentPane="False" Height="100%">
            <telerik:RadPane runat="server" ID="paneSelection" Width="50%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" ID="RadSplitter1" Width="100%" Height="100%">
                    <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None" ID="Pane1">
                        <%--OnButtonClick=""--%>
                        <telerik:RadToolBar AutoPostBack="False" ID="ToolBarSearch" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchName">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtName" runat="server" Width="170px" AutoPostBack="False"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Caselle PEC in errore" CheckOnClick="true" Checked="false" Value="mailboxError" Group="mailboxErrorGroup" AllowSelfUnCheck ="true"/>
                                <telerik:RadToolBarButton Text="Includi non gestite" CheckOnClick="true" Checked="false" Value="notHandled" Group="notHandledGroup" AllowSelfUnCheck ="true"/>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" Value="searchMailbox" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>

                        <telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%" RenderMode="Lightweight">
                            <Items>
                                <telerik:RadToolBarButton ToolTip="Aggiungi" CheckOnClick="false" Checked="false" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                                <telerik:RadToolBarButton ToolTip="Modifica" CheckOnClick="false" Checked="false" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="rtvPECMailBoxes" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%" Height="88%">
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
                                        <asp:Panel runat="server">
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Nome Casella PEC:</b>
                                                    <asp:Label runat="server" ID="lblMailBoxRecipient"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Nome utente:</b>
                                                    <asp:Label runat="server" ID="lblUsername"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Casella interoperabile:</b>
                                                    <asp:Label runat="server" ID="lblIsInterop"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Casella di protocollazione:</b>
                                                    <asp:Label runat="server" ID="lblIsProtocol"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-10 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Casella di protocollazione pubblic:</b>
                                                    <asp:Label runat="server" ID="lblIsPublicProtocol"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Location:</b>
                                                    <asp:Label runat="server" ID="lblLocation"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Tipo server ingoing:</b>
                                                    <asp:Label runat="server" ID="lblServerType"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Informazioni server ingoing:</b>
                                                    <asp:Label runat="server" ID="lblIncomingServer"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Porta ingoing:</b>
                                                    <asp:Label runat="server" ID="lblINPort"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>IN: SSL:</b>
                                                    <asp:Label runat="server" ID="lblINSSL"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>OUT: SSL: </b>
                                                    <asp:Label runat="server" ID="lblOUTSSL"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Informazioni server outgoing:</b>
                                                    <asp:Label runat="server" ID="lblOutgoingServer"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Porta outgoing:</b>
                                                    <asp:Label runat="server" ID="lblOUTPort"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Gestita:</b>
                                                    <asp:Label runat="server" ID="lblIsManaged"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Non gestita: </b>
                                                    <asp:Label runat="server" ID="lblIsNotManaged"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Profilo:</b>
                                                    <asp:Label runat="server" ID="lblProfile"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Abilita presa in carico:</b>
                                                    <asp:Label runat="server" ID="lblIsHandleEnabled"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>JeepService Associato ingoing:</b>
                                                    <asp:Label runat="server" ID="lblJeepServIN"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>JeepService Associato outgoing:</b>
                                                    <asp:Label runat="server" ID="lblJeepServOUT"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Nome regola:</b>
                                                    <asp:Label runat="server" ID="lblRulesetName"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Condizione regola:</b>
                                                    <asp:Label runat="server" ID="lblRulesetCondition"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="col-dsw-5 dsw-align-left">
                                                <div class="col-dsw-10">
                                                    <b>Tipologia di fatturazione elettronica:</b>
                                                    <asp:Label runat="server" ID="lblElectronicType"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Tipo di regola:</b>
                                                    <asp:Label runat="server" ID="lblRulesetType"></asp:Label>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server">
                    <usc:uscPECMailBoxSettings runat="server" ID="uscPECMailBoxSettings" />
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>

</asp:Content>
