<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="Duplicate.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Duplicate" %>

<%@ Register Src="~/UserControl/DocumentSeriesItemPreview.ascx" TagName="DSIPreview" TagPrefix="usc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <usc:DSIPreview runat="server" ID="ItemPreview" />

    <table id="tblPreview" class="datatable" runat="server">
        <tr>
            <th colspan="2">Elementi da duplicare</th>
        </tr>

        <tr>
            <td class="label">&nbsp;</td>
            <td>
                <asp:CheckBoxList runat="server" ID="DuplicationList">
                </asp:CheckBoxList>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="btnDuplicate" Text="Duplica" PostBackUrl="Item.aspx?Type=Series&Action=Duplicate" Width="150" />
    <asp:Button runat="server" ID="cmdRestoreDefault" Text="Ripristina"/>
</asp:Content>
