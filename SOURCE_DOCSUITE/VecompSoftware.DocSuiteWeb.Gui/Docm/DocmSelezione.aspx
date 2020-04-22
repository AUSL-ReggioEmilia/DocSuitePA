<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmSelezione" Codebehind="DocmSelezione.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratiche - Selezione" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

        </script>
    </telerik:RadScriptBlock>

    <div class="docm" style="overflow:hidden;width:100%;height:100%;">
        <docsuite:bindgrid AllowCustomPaging="True" AllowFilteringByColumn="false" AllowMultiRowSelection="true" AllowPaging="True" AllowSorting="False" AutoGenerateColumns="False" GridLines="Both" ID="DG" PageSize="50" runat="server">
            <PagerStyle AlwaysVisible="false" Mode="NextPrevAndNumeric" Position="TopAndBottom"
                ShowPagerText="false" />
            <MasterTableView AllowMultiColumnSorting="False" AllowSorting="false" CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" Dir="LTR" NoMasterRecordsText="Nessuna Pratica Trovata" Frame="Border" TableLayout="Fixed">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Pratica" Groupable="false" UniqueName="cPratica" AllowFiltering="false">
                        <HeaderStyle HorizontalAlign="Left" Width="20%" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" Text='<%# Eval("Year") & "/" & String.Format("{0:0000000}", Eval("Number")) %>'
                                CommandName='<%# "Docm:" & Eval("Year") & "/" & String.Format("{0:0000000}", Eval("Number"))%>' ID="Linkbutton1" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn ItemStyle-Width="80%" DataField="Document.DocumentObject" HeaderText="Oggetto" Groupable="false" UniqueName="Document.DocumentObject" AllowFiltering="false"/>
                </Columns>
                <RowIndicatorColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                    Visible="False">
                    <HeaderStyle Width="20px" />
                </RowIndicatorColumn>
                <ExpandCollapseColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                    Resizable="False" Visible="False">
                    <HeaderStyle Width="20px" />
                </ExpandCollapseColumn>
                <EditFormSettings>
                    <EditColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType">
                    </EditColumn>
                </EditFormSettings>
            </MasterTableView>
            <ClientSettings AllowGroupExpandCollapse="True" AllowDragToGroup="True">
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
            <ExportSettings>
                <Pdf FontType="Subset" PaperSize="Letter" />
                <Excel Format="Html" />
            </ExportSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
        </docsuite:bindgrid>   
    </div>
</asp:Content>
