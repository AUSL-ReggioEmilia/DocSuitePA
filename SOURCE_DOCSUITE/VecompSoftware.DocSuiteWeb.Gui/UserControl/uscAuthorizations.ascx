<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscAuthorizations.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscAuthorizations" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">

        function BtnExpandFascAuth_OnClick(sender, args) {
            
            var _fascInfoContent = $("#".concat("<%=fascInfo.ClientID%>"));
            var _btnExpandFascInfo = $("#".concat("<%=btnExpandFascInfo.ClientID%>"));
            if (_btnExpandFascInfo.hasClass("dsw-arrow-down")) {
                _fascInfoContent.hide();
                _btnExpandFascInfo.removeClass("dsw-arrow-down");
                _btnExpandFascInfo.addClass("dsw-arrow-up");
            }
            else {
                _fascInfoContent.show();
                _btnExpandFascInfo.removeClass("dsw-arrow-up");
                _btnExpandFascInfo.addClass("dsw-arrow-down");
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title" id="fascCaption" runat="server">
                        Autorizzazioni
                        <telerik:RadButton ID="btnExpandFascInfo" CssClass="dsw-vertical-middle dsw-arrow-down" AutoPostBack="true" runat="server" Width="16px" Height="16px" Visible="true">
                            <Image EnableImageButton="true" />
                        </telerik:RadButton>
                    </div>
                    <div class="dsw-panel-content" id="fascInfo" runat="server">
                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow HtmlTag="Div" ID="responsibleUser" runat="server">
                                    <Content>
                                        <telerik:RadTreeView ID="rtvResponsibleUser" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand">
                                        </telerik:RadTreeView>
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div" ID="rowResponsibleRole" runat="server">
                                    <Content>
                                        <telerik:RadTreeView ID="rtvResponsibleRole" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand">
                                        </telerik:RadTreeView>
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div" ID="rowMasterRole" runat="server">
                                    <Content>
                                        <telerik:RadTreeView ID="rtvMasterRole" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand">
                                        </telerik:RadTreeView>
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div" ID="rowWorkflowHandler" runat="server">
                                    <Content>
                                        <telerik:RadTreeView ID="rtvWorkflowHandler" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand">
                                        </telerik:RadTreeView>
                                    </Content>
                                </telerik:LayoutRow>
                                <telerik:LayoutRow HtmlTag="Div" ID="authorizedRoles" runat="server">
                                    <Content>
                                        <telerik:RadTreeView ID="rtvAuthorizedRoles" runat="server" Width="100%" CausesValidation="False" OnClientNodeClicked="OnClientNodeClickedExpand">
                                        </telerik:RadTreeView>
                                    </Content>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
