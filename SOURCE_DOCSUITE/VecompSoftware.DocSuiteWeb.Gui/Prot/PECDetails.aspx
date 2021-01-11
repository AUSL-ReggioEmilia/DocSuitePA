<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PECDetails.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECDetails" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">

            var pecDetails;
            require(["Prot/PECDetails"], function (PECDetails) {
                $(function () {
                    pecDetails = new PECDetails(tenantModelConfiguration.serviceConfiguration);
                    pecDetails.dateId = "<%=sendingDate.ClientID%>";
                    pecDetails.nameId = "<%=name.ClientID%>";
                    pecDetails.senderId = "<%=sender.ClientID%>";
                    pecDetails.receiverId = "<%=receiver.ClientID%>";
                    pecDetails.layoutReceiverId = "<%=layoutReceiver.ClientID%>";
                    pecDetails.layoutMessageId = "<%=layoutMessage.ClientID%>";
                    pecDetails.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadPageLayout runat="server" ID="dateContainer" Width="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Dettaglio PEC
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Data:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="sendingDate"></asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Oggetto:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:HyperLink runat="server" ID="name"></asp:HyperLink>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Mittente:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="sender"></asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" ID="layoutReceiver" Style="display:none;">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                    <b>Destinatari:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                    <asp:Label runat="server" ID="receiver"></asp:Label>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" ID="layoutMessage" Style="display:none;">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px; margin-bottom: 5px; display: inline-block;">
                                                    <b>Messaggi di servizio:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="8" Style="display: block;">
                                                    <table id="messageDetails">

                                                    </table>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                </Rows>
        </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
