<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtocolObjectModifier.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocolObjectModifier" %>

<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="uscObject" TagPrefix="usc" %>

<table class="datatable">
    <tr>
        <th align="left" colspan="2">
            Nuovo Oggetto
        </th>
    </tr>
    <tr class="Chiaro">
        <td colspan="2">
            <telerik:RadAjaxPanel runat="server" ID="ajaxPanelData">
                <asp:Panel runat="server" ID="pnlObjectData">
                    <table width="100%">
                        <tr>
                            <td>
                                <b>Anno:</b>
                            </td>
                            <td>
                                <asp:Label ID="lblYear" runat="server"></asp:Label></td>
                            <td>
                                <b>Numero:</b>
                            </td>
                            <td>
                                <asp:Label ID="lblNumber" runat="server"></asp:Label></td>
                            <td>
                                <b>Contenitore:</b>
                            </td>
                            <td>
                                <asp:Label ID="lblContainer" runat="server"></asp:Label></td>
                            <td>
                                <b>Classificazione:</b>
                            </td>
                            <td>
                                <asp:Label ID="lblClassification" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </telerik:RadAjaxPanel>
        </td>
    </tr>
    <tr class="Chiaro">
        <td style="width:90%;">
            <usc:uscObject runat="server" ID="uscObject" MultiLine="true" MaxLength="255" Type="Prot" Required="false" />
            &nbsp;
            
        </td>
        <td style="width:10%;">
            <asp:Button ID="btnConferma" runat="server" style="vertical-align:middle;" Text="Conferma" />
        </td>
    </tr>
</table>