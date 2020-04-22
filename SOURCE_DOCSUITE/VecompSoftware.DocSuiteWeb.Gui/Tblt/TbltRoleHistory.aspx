<%@ Page Title="Storicizzazione Settore" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltRoleHistory.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltHistory" %>

<asp:Content  ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            function CloseWin() {
                    var HistoryWindows = new Object();
                    GetRadWindow().close(HistoryWindows);
            }       
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content  ContentPlaceHolderID="cphContent" runat="server">   
    <telerik:RadGrid runat="server" ID="grdRoleHistory" Width="100%">
        <MasterTableView AutoGenerateColumns="False" TableLayout="Auto">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="NameRole" HeaderText="Nome Settore">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblNomeSettore" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="FromDate" HeaderText="Dalla Data">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblFromDate" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="ToDate" HeaderText="Alla Data">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblToDate" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <SortExpressions>
                  <telerik:GridSortExpression FieldName="FromDate" SortOrder="Ascending" />
            </SortExpressions>
        </MasterTableView>
        <ClientSettings />
    </telerik:RadGrid>
</asp:Content>
<asp:Content  ContentPlaceHolderID="cphFooter" runat="server">     
     <asp:Button ID="cmdClose" runat="server" Text="Chiudi" Width="150" />
</asp:Content>
