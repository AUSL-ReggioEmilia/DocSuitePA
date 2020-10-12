<%@ Page AutoEventWireup="false" CodeBehind="PosteWebItem.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Prot.PosteWeb.Item" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Poste OnLine" %>

<%@ Register Src="~/UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc2" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function OnClientClose() {
                var wnd = GetRadWindow();
                if (wnd) wnd.close();
            }

            function RefreshOnPECAddressesInserted() {
                $find("<%= AjaxManager.ClientID%>").ajaxRequest("content");
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlContent">
        <table id="tblPrincipale" width="100%" height="100%">
            <tr>
                <td height="100" style="vertical-align: top; width: 100%;">
                    <uc:uscProtocollo ID="uscProtocollo" runat="server" OnMittenteAdded="uscProtocollo_MittenteAdded" />
                </td>
                <td class="center" width="3%" rowspan="2" style="vertical-align: top;">
                    <asp:Table BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" CellSpacing="0" Height="100%" ID="TblButtons" runat="server">
                        <asp:TableRow Height="100%">
                            <asp:TableCell Height="100%" VerticalAlign="Top"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
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
                                <uc2:uscDocumentUpload HeaderVisible="false" ID="uscProtDocument" ReadOnly="true" runat="server" TreeViewCaption="Documenti" Type="Prot" />
                            </td>
                            <td width="50%" valign="top">
                                <uc2:uscDocumentUpload HeaderVisible="false" ID="uscProtAttachs" ReadOnly="true" runat="server" TreeViewCaption="Allegati" Type="Prot" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; height: 30px; vertical-align: top;">
                    <table id="Table1" class="datatable" runat="server" width="100%">
                        <tr>
                            <th width="50%">
                                <asp:Label ID="lblAccountName" runat="server" Text="Account PosteWeb" />
                            </th> 
                            <th width="50%" ID="lblTNoticeSender" runat="server" visible="false">
                                <asp:Label runat="server" Text="Mittente TNotice" />
                            </th>
                        </tr>
                        <tr>
                            <td width="50%" valign="top">
                                <asp:DropDownList AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" ID="ddlPolAccount" runat="server" Width="300px" AutoPostBack="true"/>
                            </td>
                            <td style="width: 50; white-space: nowrap;" ID="lblTNoticeSenderDescription" runat="server" visible="false">
                                <div class="label" style="width: 15%">
                                    Denominazioni: 
                                    <asp:Label ID="_sender_denominazioni" Text="" runat="server" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <table id="tblMessagge" class="datatable" runat="server" width="100%">
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="Label1" EnableViewState="False" runat="server" CssClass="Prot-Tabella">Testo messaggio:</asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <telerik:RadTextBox ID="txtMessage" BorderWidth="1" Width="100%" Rows="5" TextMode="MultiLine" Visible="true" runat="server"></telerik:RadTextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="cmdSend" runat="server" Text="Invia" Enabled="false" Width="120px" />
</asp:Content>
