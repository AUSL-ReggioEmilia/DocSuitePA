<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="LogSummary.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.LogSummary" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label labelPanel">Tipo:</td>
            <td style="width:80%;"><asp:DropDownList runat="server" CausesValidation="false" ID="ddlContainerArchive" AutoPostBack="True" Width="300px" /></td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width:20%;">Periodo:
            </td>
            <td style="width:80%;">
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Dalla data" ID="dtpLogDateFrom" runat="server" />
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Alla data" ID="dtpLogDateTo" runat="server" />
            </td>
        </tr>        
    </table>
    <div style="margin:1px;">
        <div style="float: left;">
            <asp:Button ID="cmdRefresh" runat="server" Text="Aggiorna visualizzazione" Width="200px" />
            <asp:Button ID="cmdClearFilters" runat="server" Text="Azzera filtri" />
        </div>
        <div style="text-align:right;">
            <telerik:RadButton ID="cmdExportToExcel" runat="server" Text="Esporta" ToolTip="Esporta in Excel" CausesValidation="false" />
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="grdLogSummary" runat="server" Width="100%">
        <HeaderStyle CssClass="tabella" />
        <MasterTableView CommandItemDisplay="None" Dir="LTR" Frame="Border" TableLayout="Auto">
            <Columns>
                <telerik:GridBoundColumn DataField="Total" HeaderText="Totali" ItemStyle-Width="75px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true" />
                <telerik:GridBoundColumn DataField="Name" HeaderText="Serie Documentale" />
                <telerik:GridBoundColumn DataField="Added" HeaderText="Nuove" ItemStyle-Width="75px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn DataField="Drafted" HeaderText="In bozza" ItemStyle-Width="75px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn DataField="Edits" HeaderText="Modifiche" ItemStyle-Width="75px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicazioni" ItemStyle-Width="75px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridBoundColumn DataField="Retired" HeaderText="Ritiri" ItemStyle-Width="75px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>