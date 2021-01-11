<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscPrivacyPanel.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscPrivacyPanel" %>
<%@ Register TagPrefix="usc" TagName="uploaddocument" Src="~/UserControl/uscDocumentUpload.ascx" %>

<asp:Panel ID="pnlPrivacyPublishDocumentSelection" runat="server" Visible="False">
    <table class="datatable" width="100%">
        <tr>
            <th colspan="2">Gestione privacy
            </th>
        </tr>
        <tr class="Chiaro">
            <td>
                <table id="tblResolutionNumber" runat="server" visible="False">
                    <tr>
                        <td class="label" style="width: 155px;">
                            <label id="lblNumberLabel" runat="server"></label>
                        </td>
                        <td>
                            <b>
                                <label id="lblNumber" runat="server"></label>
                            </b>
                        </td>
                    </tr>
                </table>
                <table width="100%">
                    <tr>
                        <td class="label" style="width: 155px">Usa versione privacy:
                        </td>
                        <td>
                            <asp:RadioButtonList ID="rblSelectPrivacy" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Si" Value="True"></asp:ListItem>
                                <asp:ListItem Text="No" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:RequiredFieldValidator runat="server" ID="rfvSelectPrivacy" Enabled="false"
                                ControlToValidate="rblSelectPrivacy" Display="Dynamic" ErrorMessage="Definire se utilizzare la modalità privacy">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr class="Spazio">
            <td></td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlPrivacyPublishDocument" runat="server" Visible="False">
    <table class="datatable" width="100%">
        <tr class="Chiaro">
            <td class="label" style="width: 140px; text-align: left; padding-left: 15px;">1) Stampare:
            </td>
            <td>
                <asp:HyperLink ID="privacyImageToPrint" runat="server" ToolTip="File da stampare" Target="_blank" />
                <asp:HyperLink ID="privacyTextToPrint" runat="server" Text="File da stampare" Target="_blank" />
            </td>
        </tr>
        <tr class="Chiaro">
            <td class="label" style="width: 140px; text-align: left; padding-left: 15px;">
                2) Scansionare:
            </td>
            <td>
                <usc:uploaddocument buttonfdqenabled="false" ButtonFileEnabled="false" ButtonFrontespizioEnabled="false" buttonpdfandfdqenabled="false" ButtonPreviewEnabled="false" ButtonRemoveEnabled="false" ButtonScannerEnabled="true" ButtonSharedFolederEnabled="false" Caption="" HeaderVisible="false" ID="uscPrivacyPublicationDocumentUploader" IsDocumentRequired="false" MultipleDocuments="false" runat="server" SignButtonEnabled="False" TreeViewCaption="Documento stampato con omissis" Type="Resl" WindowWidth="620" />
            </td>
        </tr>
        <tr class="Spazio">
            <td></td>
        </tr>
    </table>
</asp:Panel>
