<%@ Page AutoEventWireup="false" CodeBehind="CommonSelSharedFolder.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelSharedFolder" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
        //restituisce un riferimento alla radwindow
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
    
    <telerik:RadGrid runat="server" ID="RadGridFiles" AutoGenerateColumns="False" Height="100%">
        <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" Dir="LTR" Frame="Border" TableLayout="Auto">
            <Columns>
                <telerik:GridTemplateColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType" ForceExtractValue="None" UniqueName="TemplateColumn">
                    <ItemTemplate>
                        <telerik:radButton CommandName="Select" Height="16px" ID="btnSelFile" ImageUrl="~/Comm/Images/File16.gif" runat="server" ToolTip="Seleziona File" Width="16px" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="NoFilter" DataField="Name" FilterListOptions="VaryByDataType" ForceExtractValue="None" HeaderText="Nome File" UniqueName="cName">
                    <ItemStyle Wrap="false" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="NoFilter" DataField="Dimension" FilterListOptions="VaryByDataType" ForceExtractValue="None" HeaderText="Dimensione" UniqueName="cDimension">
                    <HeaderStyle HorizontalAlign="Right" />
                    <ItemStyle HorizontalAlign="Right" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="NoFilter" DataField="CreationTime" FilterListOptions="VaryByDataType" ForceExtractValue="None" HeaderText="Data Creazione" UniqueName="cCreationTime">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="NoFilter" DataField="LastWriteTime" FilterListOptions="VaryByDataType" ForceExtractValue="None" HeaderText="Data ultima mod." UniqueName="cLastWriteTime">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>