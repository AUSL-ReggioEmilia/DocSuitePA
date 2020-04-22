<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MessageDetails.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.MessageDetails" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">

            var messageDetails;
            require(["Prot/MessageDetails"], function (MessageDetails) {
                $(function () {
                    messageDetails = new MessageDetails(tenantModelConfiguration.serviceConfiguration);
                    messageDetails.dateId = "<%=sendingDate.ClientID%>";
                    messageDetails.nameId = "<%=name.ClientID%>";
                    messageDetails.senderId = "<%=sender.ClientID%>";
                    messageDetails.receiverId = "<%=receiver.ClientID%>";
                    messageDetails.receiverBccId = "<%=receiverBcc.ClientID%>";
                    messageDetails.statusId = "<%=status.ClientID%>";
                    messageDetails.pecImageId = "<%=pecImage.ClientID%>";
                    messageDetails.layoutRowRecipientId = "<%=layoutRowRecipient.ClientID%>";
                    messageDetails.layoutRowRecipientBccId = "<%=layoutRowRecipientBcc.ClientID%>";
                    messageDetails.isMessageReadable = <%= IsMessageReadable.ToString().ToLower() %>;
                    messageDetails.initialize();
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
                            Messaggi
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
                                    <telerik:LayoutRow HtmlTag="Div" ID="layoutRowRecipient">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Destinatari:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="receiver"></asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" ID="layoutRowRecipientBcc">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Destinatari Ccn:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="receiverBcc"></asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px; margin-bottom: 5px; display: inline-block;">
                                                <b>Status:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="display: block;">
                                                <asp:Image runat="server" ID="pecImage" Style="margin-top: 5px;" />
                                                <asp:Label runat="server" ID="status" Style="display: block; margin-left: 20px; margin-top: -18px;"></asp:Label>
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
