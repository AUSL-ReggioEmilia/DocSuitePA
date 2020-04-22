<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskApproveManager.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskApproveManager" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlApproveManagerContainer" CssClass="radGridWrapper">
        <telerik:RadPivotGrid runat="server" ID="dgvApproveManagerGrid" AllowPaging="true" AllowFiltering="false" ShowColumnHeaderZone="false" ShowRowHeaderZone="false" ShowDataHeaderZone="false" 
            RowTableLayout="Tabular" Height="100%" Width="100%" EnableZoneContextMenu="false" ShowFilterHeaderZone="false" EnableCaching="false" >            
            <TotalsSettings ColumnsSubTotalsPosition="None" ColumnGrandTotalsPosition="None" GrandTotalsVisibility="None" RowGrandTotalsPosition="None" RowsSubTotalsPosition="None"/>
            <PagerStyle HorizontalAlign="Center" />
            <ClientSettings EnableFieldsDragDrop="true">                
                <Scrolling AllowVerticalScroll="true"></Scrolling>
            </ClientSettings>
            <Fields>
                <telerik:PivotGridRowField SortOrder="Ascending" DataField="DeskDocumentName" Caption="Documento">
                </telerik:PivotGridRowField>
                <telerik:PivotGridRowField DataField="Version" Caption="Versione" DataFormatString="{0:f2}" IsHidden="true">
                </telerik:PivotGridRowField>
                <telerik:PivotGridAggregateField Aggregate="Sum" Caption="Approvazione" DataField="IsApproveForAggregation">
                    <CellTemplate>
                        <asp:Image ID="imgApproval" runat="server" ImageAlign="Middle"/>
                    </CellTemplate>
                </telerik:PivotGridAggregateField>
                <telerik:PivotGridColumnField SortOrder="Ascending" DataField="AccountName" Caption="Utente">
                </telerik:PivotGridColumnField>                    
            </Fields>
            <ClientSettings EnableFieldsDragDrop="false"></ClientSettings>                          
        </telerik:RadPivotGrid>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnCloseDesk" Width="150" Text="Chiudi Tavolo"></telerik:RadButton>
</asp:Content>
