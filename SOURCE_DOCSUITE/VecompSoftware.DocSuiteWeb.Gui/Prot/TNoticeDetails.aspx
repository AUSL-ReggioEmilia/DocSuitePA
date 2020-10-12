<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TNoticeDetails.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.TNoticeDetails" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">

            var details;
            require(["Prot/TNoticeDetails"], function (TNoticeDetails) {
                $(function () {
                    details = new TNoticeDetails(tenantModelConfiguration.serviceConfiguration);
                    details.dateId = "<%=sendingDate.ClientID%>";
                    details.senderId = "<%=sender.ClientID%>";
                    details.statusId = "<%=status.ClientID%>";
                    details.urlAcceptId = "<%=urlAccept.ClientID%>";
                    details.urlCpfId = "<%=urlCpf.ClientID%>";
                    details.cmdDocumentsId = "<%= cmdDocuments.ClientID%>";
                    details.initialize();
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
                            Dettaglio di invio
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
                                                <b>Mittente:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="sender"></asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>

                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Stato Invio:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="status">
                                                    <div style="display: flex;">
                                                        <span>
                                                            <img class="rtImg" data-name="statusBlue"  alt = "" src = "../Comm/Images/pec-accettazione.gif" style="display:none;"/>
                                                            <img class="rtImg" data-name="statusYellow" alt = "" src = "../Comm/Images/pec-preavviso-errore-consegna.gif" style="display:none;" />
                                                            <img class="rtImg" data-name="statusGreen" alt = "" src = "../Comm/Images/pec-avvenuta-consegna.gif" style="display:none;"/>
                                                            <img class="rtImg" data-name="statusRed" alt = "" src = "../Comm/Images/pec-errore-consegna.gif" style="display:none;"/>
                                                        </span>
                                                        <span id="statusTextId"></span>
                                                    </div>
                                                </asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>


                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Ricevuta di accettazione:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="urlAccept">
                                                    <a target="_blank" href="#" style="display:none">Navigare</a>
                                                    <span style="display:none">-</span>
                                                </asp:Label>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>

                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Attestazione:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" Style="margin-top: 5px;">
                                                <asp:Label runat="server" ID="urlCpf">
                                                    <a target="_blank" href="#" style="display:none">Navigare</a>
                                                    <span style="display:none">-</span>
                                                </asp:Label>
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
<asp:Content runat="server" ContentPlaceHolderID="cphFooter" ID="footerContent">
    <telerik:RadButton Text="Documenti" runat="server" ID="cmdDocuments" AutoPostBack="false" />
</asp:Content>
