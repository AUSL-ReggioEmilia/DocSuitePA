<%@ Page AutoEventWireup="false" Codebehind="UtltConfig.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltConfig" MaintainScrollPositionOnPostback="true" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Configurazione" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Table CssClass="datatable" ID="tblConfig" runat="server" />
    <div class="titolo">Configurazione Contenitori Utente</div>
    <span class="miniLabel">Modulo:</span>
    <telerik:RadButton ButtonType="ToggleButton" GroupName="StandardButton" ID="cmdDocument" runat="server" Text="Pratiche" ToggleType="Radio" Visible="false" />
    <telerik:RadButton ButtonType="ToggleButton" GroupName="StandardButton" ID="cmdProtocol" runat="server" Text="Protocolli" ToggleType="Radio" Visible="false" />
    <telerik:RadButton ButtonType="ToggleButton" GroupName="StandardButton" ID="cmdDocumentSeries" runat="server" Text="Archivi" ToggleType="Radio" Visible="false" />
    <telerik:RadButton ButtonType="ToggleButton" GroupName="StandardButton" ID="cmdResolution" runat="server" Text="Atti" ToggleType="Radio" Visible="false" />
    <br class="Spazio" />
    <asp:Table CssClass="datatable" GridLines="Horizontal" ID="tblDiritti" runat="server" />
</asp:Content>
