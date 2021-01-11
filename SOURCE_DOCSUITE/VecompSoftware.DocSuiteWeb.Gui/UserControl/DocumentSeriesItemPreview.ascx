<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocumentSeriesItemPreview.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocumentSeriesItemPreview" %>


<table id="tblPreview" class="datatable" runat="server" style="display: none">
    <tr>
        <th colspan="2"><asp:Label ID="DocumentSeries" runat="server" /></th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">
            <asp:Label runat="server" ID="lblStatus" />
        </td>
        <td style="width: 80%;">
            <asp:Label ID="lblId" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Serie: </td>
        <td>
            <asp:Label ID="lblDocumentSeries" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Oggetto: </td>
        <td>
            <asp:Label ID="lblObject" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Classificazione: </td>
        <td>
            <asp:Label ID="lblCategoryDescription" runat="server" />
        </td>
    </tr>
</table>
