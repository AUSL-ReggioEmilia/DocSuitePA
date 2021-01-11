<%@ Page AutoEventWireup="false" Codebehind="UtltWSImport.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltWSImport" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Importazione da WebService" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table id="tabContenitore" class="datatable" style="height: 99%">
        <tr>
            <td>
                <usc:uscDocumentUpload ButtonFDQEnabled="false" ButtonFileEnabled="true" ButtonPdfAndFDQEnabled="false" ButtonScannerEnabled="false" ButtonSharedFolederEnabled="false" ID="uscXml" IsDocumentRequired="true" runat="server" TreeViewCaption="Xml da importare" />
                <br />
                <asp:Label runat="server" ID="lblResult" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button runat="server" ID="btnImporta" Text="Importa" Visible="true" />
</asp:Content>
