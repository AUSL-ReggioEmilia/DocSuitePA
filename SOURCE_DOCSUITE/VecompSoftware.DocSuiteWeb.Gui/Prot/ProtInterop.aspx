<%@ Page AutoEventWireup="false" Codebehind="ProtInterop.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtInterop" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Interoperabilità" %>

<%@ Register Src="../UserControl/uscProtocollo.ascx" TagName="Protocollo" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="DocumentUpload" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function OpenPecMailAddressWindow(id, contactType, contactName, message) {
                var wnd = window.radopen("<%= PecEMailInsertUrl %>?Type=Prot&id=" + id + "&type=" + contactType + "&name=" + contactName + "&Message=" + message, "windowViewMail");
                wnd.setSize(700, 500);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal = true;
                wnd.add_close(OnClientClose);
                wnd.center();
            }

            function OnClientClose() {
                var wnd = GetRadWindow();
                if (wnd) wnd.close();
            }

            function RefreshOnPECAddressesInserted() {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("content");
            }

            function OpenPECMailViewer(mailId, destinationId) {
                var wnd = window.radopen("<%=PECMailViewerUrl() %>?Type=Pec&PECId=" + mailId + "&Draft=1&DestinationMailBoxId=" + destinationId, "windowViewMail");
                wnd.setSize(700, 525);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.None);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal = true;
                wnd.set_iconUrl("images/mail.gif");
                wnd.add_close(OnClientClose);
                wnd.center();
            }

            function OpenGenericWindow(url, name)
            {
                var wnd = window.radopen(url, name);
                wnd.setSize(<%=WindowWidth() %>, <%=WindowHeight() %>);
                wnd.set_behaviors(<%=WindowBehaviors() %>);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_iconUrl("images/mail.gif");
                wnd.add_close(OnClientClose);
                wnd.set_destroyOnClose(true);
                <%=WindowPosition() %>;
                return wnd; 
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerViewMail" runat="server">
        <Windows>
            <telerik:RadWindow Height="200px" ID="windowInsertEMailAddress" NavigateUrl="ProtPecEmailAddress.aspx" OnClientClose="OnClientClose" ReloadOnShow="false" runat="server" Title="Inserisci Indirizzo PEC" Width="700px" />
            <telerik:RadWindow Behaviors="None" Height="525px" ID="windowViewMail" NavigateUrl="~/PEC/PECView.aspx" OnClientClose="OnClientClose" ReloadOnShow="false" runat="server" Title="Visualizza Messaggio PEC" Width="700px" />
        </Windows>
    </telerik:RadWindowManager>
    <div runat="server" id="Div1" style="height: 100%">
        <table id="tblPrincipale" width="100%" height="100%" runat="server">
            <tr>
                <td height="100" style="vertical-align: top; width: 100%;">
                    <usc:Protocollo ID="uscProtocollo" runat="server" OnMittenteAdded="uscProtocollo_MittenteAdded" />
                </td>
                <td class="center" width="3%" rowspan="2" style="vertical-align: top;">
                    <asp:Table ID="TblButtons" runat="server" CellPadding="3" CellSpacing="0" BorderColor="Gray"
                        Height="100%" BorderWidth="1px" BorderStyle="Solid">
                        <asp:TableRow Height="100%">
                            <asp:TableCell Height="100%" VerticalAlign="Top"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; height: 30px; vertical-align: top;">
                    <asp:UpdatePanel ID="updSettori" runat="server" UpdateMode="conditional">
                        <ContentTemplate>
                            <usc:Settori Caption="Settore mittente PEC" HeaderVisible="true" ID="uscSettori" MultipleRoles="true" MultiSelect="false" OnRoleSelected="uscSettori_RoleSelected" Required="true" RequiredMessage="Campo Settori Obbligatorio" RoleRestictions="OnlyMine" runat="server" Type="Prot" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; height: 30px; vertical-align: top;">
                    <table id="tblDocumentiAllegati" class="datatable" runat="server" width="100%">
                        <tr>
                            <th width="50%">Documenti</th>
                            <th width="50%">Allegati</th>
                        </tr>
                        <tr>
                            <td width="50%" valign="top">
                                <usc:DocumentUpload HeaderVisible="false" ID="uscProtDocument" ReadOnly="true" runat="server" TreeViewCaption="Documenti" Type="Prot" />
                            </td>
                            <td width="50%" valign="top">
                                <usc:DocumentUpload HeaderVisible="false" ID="uscProtAttachs" Visible="False" ReadOnly="true" runat="server" TreeViewCaption="Allegati" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <asp:UpdatePanel ID="updForm" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table id="Table2" cellspacing="1" cellpadding="1" width="100%" border="0" style="margin-top: 10px;">
                               
                                <tr>
                                    <td align="left" style="width: 10%;">
                                        <asp:Button ID="btnSegnatura" runat="server" Enabled="False" Text="Segnatura"></asp:Button></td>
                                    <td align="right" style="width: 90%;">
                                        <telerik:RadTextBox ID="txtValidationReport" runat="server" Width="100%" EnableViewState="False"
                                            Visible="False" ReadOnly="True" Rows="5" TextMode="MultiLine"></telerik:RadTextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPECAddress" EnableViewState="False" runat="server" CssClass="Prot-Tabella">Casella PEC mittente:</asp:Label></td>
                                    <td>
                                        <asp:DropDownList ID="ddlPECAddress" Width="300px" runat="server" AutoPostBack="true" Enabled="false" OnSelectedIndexChanged="ddlPECAddress_SelectedIndexChanged"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" EnableViewState="False" runat="server" CssClass="Prot-Tabella">Testo messaggio:</asp:Label></td>
                                    <td>
                                        <telerik:RadTextBox ID="txtNote" Width="100%" Rows="3" TextMode="MultiLine" runat="server"></telerik:RadTextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPriority" EnableViewState="False" runat="server" CssClass="Prot-Tabella">Priorità:</asp:Label></td>
                                    <td>
                                        <asp:DropDownList ID="ddlPriority" runat="server" Width="300px">
                                            <asp:ListItem Value="-1">Bassa</asp:ListItem>
                                            <asp:ListItem Value="0" Selected="True">Normale</asp:ListItem>
                                            <asp:ListItem Value="1">Alta</asp:ListItem>
                                        </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="Label2" EnableViewState="False" runat="server" CssClass="Prot-Tabella" Visible="false">Conferma Ricezione</asp:Label></td>
                                    <td>
                                        <asp:CheckBox ID="ddlConfermaRicezione" runat="server" Visible="false"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnMail" runat="server" Enabled="False" Text="Invia" Width="80px" />
                                        <asp:Button ID="cmdCreateMail" runat="server" Text="Crea Mail" />
                                    </td>
                                </tr>
                            </table>                        
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <table id="Table3" cellspacing="1" cellpadding="1" width="100%" border="0">
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
