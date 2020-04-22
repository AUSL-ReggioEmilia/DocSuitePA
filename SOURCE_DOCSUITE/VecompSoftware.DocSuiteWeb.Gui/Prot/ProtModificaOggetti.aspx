<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ProtModificaOggetti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtModificaOggetti" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Ricerca" %>

<%@ Register Src="~/UserControl/uscProtocolObjectFinder.ascx" TagName="uscProtocolObjectFinder" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscProtocolObjectModifier.ascx" TagName="uscProtocolObjectModifier" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscProtGrid.ascx" TagName="uscProtocolGrid" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <usc:uscProtocolObjectFinder id="uscObjectFinder" runat="server" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table class="datatable" style="width: 100%">
        <tr>
            <th colspan="2">
                <div style="float:left;">
                    Risultati Ricerca
                </div>
                <div style="float:left;">
                    <telerik:RadAjaxPanel runat="server" ID="radAjaxPanel">
                        &nbsp;<asp:Label ID="lblCounter" runat="server"></asp:Label>
                    </telerik:RadAjaxPanel>
                </div>
            </th>
        </tr>
        <tr>
            <td style="border-right: gray 1px solid" valign="top" width="180">
                <div style="overflow-y: auto; width: 100%; height: 100%">
                    <usc:uscProtocolGrid runat="server" ID="uscProtGrid" />
                </div>
            </td>
            <td valign="top">
                <iframe id="BDViewer" style="border-right: gray 0px solid; border-top: gray 0px solid;
                    border-left: gray 0px solid; border-bottom: gray 0px solid" frameborder="0" width="100%"
                    scrolling="no" height="100%" runat="server"></iframe>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <usc:uscProtocolObjectModifier id="uscObjectModifier" runat="server" />
</asp:Content>

			
