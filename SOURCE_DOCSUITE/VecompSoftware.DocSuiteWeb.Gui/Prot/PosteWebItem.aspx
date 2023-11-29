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

            function countChars(sender, eventArgs) {
                window.setTimeout(function () {
                    $get("countChars").innerHTML = $find("<%= txtTNoticeObject.ClientID%>").get_textBoxValue().length;
                }, 1);
            }

            function onMultipleTNoticeSendCheckChanged(sender) {
                var recipientsTree = $find("<%= uscProtocollo.ControlRecipients.TreeViewControl.ClientID %>");
                if (recipientsTree) {
                    recipientsTree.uncheckAllNodes();
                }
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlContent">
        <table id="tblPrincipale" width="100%" height="100%">
            <tr>
                <td height="100" style="vertical-align: top; width: 100%;">
                    <uc:uscProtocollo ID="uscProtocollo" runat="server" OnMittenteAdded="uscProtocollo_MittenteAdded" HideMulticlassification="True" />
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
                            <th width="50%" id="lblTNoticeSender" runat="server" visible="false">
                                <asp:Label runat="server" Text="Mittente TNotice" />
                            </th>
                        </tr>
                        <tr>
                            <td width="50%" valign="top">
                                <asp:DropDownList AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" ID="ddlPolAccount" runat="server" Width="300px" AutoPostBack="true" />
                            </td>
                            <td style="width: 50; white-space: nowrap;" id="lblTNoticeSenderDescription" runat="server" visible="false">
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
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <asp:Panel runat="server" ID="pnlTNoticeOptions">
                        <table class="datatable">
                            <tr>
                                <th>Opzioni di invio:</th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox runat="server" ID="chkMultipleTNotice" Text="Invia una richiesta per destinatario" onclick="onMultipleTNoticeSendCheckChanged(this)"></asp:CheckBox>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>                    
                </td>
            </tr>
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <asp:Panel runat="server" ID="pnlTNoticeObject" Visible="false">
                        <table class="datatable">
                            <tr>
                                <th>Oggetto:</th>
                            </tr>
                            <tr>
                                <td>
                                    <span id="countChars"></span>/100 caratteri.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <telerik:RadTextBox ID="txtTNoticeObject" Width="100%" runat="server" MaxLength="100"
                                        onkeydown="countChars()" ClientEvents-OnValueChanged="countChars()">
                                    </telerik:RadTextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTNoticeObject" ErrorMessage="Il campo Oggetto è richiesto"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <asp:Panel runat="server" ID="pnlTNoticeMessage" Visible="false">
                        <table class="datatable">
                            <tr>
                                <th>Messaggio:</th>
                            </tr>
                            <tr>
                                <td>
                                    <telerik:RadEditor runat="server" ID="txtTNoticeMessage" EditModes="Design" EnableResize="true" AutoResizeHeight="true" 
                                        Width="100%" Height="250px" EmptyMessage="Inserire qui il testo">
                                        <Tools>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="Bold" />
                                                <telerik:EditorTool Name="Italic" />
                                                <telerik:EditorTool Name="Underline" />
                                                <telerik:EditorTool Name="Cut" />
                                                <telerik:EditorTool Name="Copy" />
                                                <telerik:EditorTool Name="Paste" />
                                            </telerik:EditorToolGroup>
                                            <telerik:EditorToolGroup>
                                                <telerik:EditorTool Name="JustifyLeft" />
                                                <telerik:EditorTool Name="JustifyCenter" />
                                                <telerik:EditorTool Name="JustifyRight" />
                                                <telerik:EditorTool Name="JustifyNone" />
                                            </telerik:EditorToolGroup>
                                        </Tools>
                                    </telerik:RadEditor>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTNoticeMessage" ErrorMessage="Il campo Messaggio è richiesto"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="cmdSend" runat="server" Text="Invia" Enabled="false" Width="120px" />
</asp:Content>
