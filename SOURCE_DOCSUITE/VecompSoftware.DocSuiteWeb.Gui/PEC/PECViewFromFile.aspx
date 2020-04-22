<%@ Page AutoEventWireup="false" CodeBehind="PECViewFromFile.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECViewFromFile" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register TagPrefix="uc1" TagName="uscViewerLight" Src="~/Viewers/ViewerLight.ascx" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <asp:Panel runat="server" ID="pnlHeader">
        <usc:UploadDocument ButtonPreviewEnabled="false" ButtonScannerEnabled="False" Caption="Documento" ID="uscUploadDocumenti" IsDocumentRequired="true" MultipleDocuments="false" runat="server" SignButtonEnabled="False" />

        <table runat="server" id="HeaderPanel" class="datatable">
            <tr class="Chiaro">
                <td>
                    <table style="width: 100%; border: 0; margin: 0; padding: 0;">
                        <tr>
                            <td style="width: 60px; vertical-align: top; font-size: 8pt; text-align: left;">
                                <b>Da:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:Label BackColor="Transparent" BorderStyle="None" ID="lblFrom" ReadOnly="true" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 60px; vertical-align: top; font-size: 8pt; text-align: left;">
                                <b>A:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:Label BackColor="Transparent" BorderStyle="None" ID="lblTo" ReadOnly="true" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 60px; vertical-align: top; font-size: 8pt; text-align: left;">
                                <asp:Label Font-Bold="true" ID="lblInviatoRicevuto" runat="server" Text="Data:" />
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt;">
                                <telerik:RadTextBox BackColor="Transparent" BorderStyle="None" ID="txtDate" ReadOnly="true" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 60px; vertical-align: top; font-size: 8pt; text-align: left;">
                                <asp:Label Font-Bold="true" ID="lblEmlSize" runat="server" Text="Dimensione:" />
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt;">
                                <telerik:RadTextBox BackColor="Transparent" BorderStyle="None" ID="txtEmlSize" ReadOnly="true" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 60px; vertical-align: top; font-size: 8pt; text-align: left;">
                                <b>Oggetto:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt;">
                                <telerik:RadTextBox BackColor="Transparent" BorderStyle="None" ID="txtObject" ReadOnly="true" runat="server" Width="100%" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscViewerLight runat="server" ID="viewer" CheckBoxes="true" ToolBarVisible="False" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button Enabled="False" ID="cmdAttach" runat="server" Text="Allega" Width="100" />
        <asp:Button Enabled="False" ID="cmdProtocol" runat="server" Text="Protocolla" Width="100" />
    </asp:Panel>
</asp:Content>
