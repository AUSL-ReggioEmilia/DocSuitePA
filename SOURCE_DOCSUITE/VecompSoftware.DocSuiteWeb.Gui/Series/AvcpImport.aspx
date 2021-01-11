<%@ Page Title="Importazione dati AVCP" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="AvcpImport.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.AvcpImport" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="MainPanel">
        <usc:UploadDocument AllowedExtensions=".xls" ButtonPreviewEnabled="False" ButtonScannerEnabled="False" Caption="Documento da importare" ID="uscDocument" IsDocumentRequired="True" MultipleDocuments="true" runat="server" SignButtonEnabled="false" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button runat="server" Text="Carica documento" ID="cmdNew" />
    </asp:Panel>
</asp:Content>
