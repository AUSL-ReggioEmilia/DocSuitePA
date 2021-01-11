<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommFascicoloRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommFascicoloRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscProtGridBar.ascx" TagName="uscProtGridBar" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocmGrid.ascx" TagName="uscDocmGrid" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <div class="titolo" id="divTitolo" runat="server" style="white-space: nowrap">
        Fascicoli -
        <asp:Label ID="lblProt" runat="server" />&nbsp;<asp:Label ID="lblDocm" runat="server" />
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Height="100%" Width="100%" LiveResize="false" Orientation="Horizontal">
            <telerik:RadPane runat="server" Height="50%" Scrolling="None">
                <telerik:RadSplitter runat="server" Height="100%" Width="100%" LiveResize="false" Orientation="Horizontal">
                    <telerik:RadPane ID="rowTop" runat="server" Height="100%" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        <div class="prot" style="height: 100%;">
                            <usc:uscProtGrid runat="server" ID="uscProtocolGrid" />
                        </div>
                    </telerik:RadPane>
                    <telerik:RadPane runat="server" Height="28px">
                        <div class="prot dsw-text-right" style="padding-top: 2px;">
                            <usc:uscProtGridBar runat="server" ID="uscProtocolGridBar" />
                        </div>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
            <telerik:RadSplitBar CollapseMode="Both" runat="server" ID="splitterBar"></telerik:RadSplitBar>
            <telerik:RadPane runat="server" Height="50%" ID="rowBottom" Scrolling="None">
                <div class="docm" style="height: 100%;">
                    <usc:uscDocmGrid runat="server" ID="uscDocumentGrid" />
                </div>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>
