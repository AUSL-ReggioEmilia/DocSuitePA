<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtImportM.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtImportM" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerImport" PreserveClientState="True" runat="server" >
        <Windows>
            <telerik:RadWindow Height="550" ID="wndResult" runat="server" Title="Importazione - Risultati" Width="700" />
            <telerik:RadWindow Behaviors="None" Height="550" ID="wndProgress" runat="server" Width="700" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
        <script type="text/javascript" language="javascript">
            function onTaskCompleted(sender, args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("");
            }
        </script>
    </telerik:RadScriptBlock>
    
    <div align="center">
        <asp:Panel runat="server" ID="pnlGrid">
            <DocSuite:BaseGrid ID="GrResults" runat="server" GridLines="Vertical" AllowPaging="True" AllowSorting="True" Visible="False" Width="99%" AutoGenerateColumns="False">
                <MasterTableView>
                    <RowIndicatorColumn Visible="False">
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn Resizable="False" Visible="False">
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <EditFormSettings>
                        <PopUpSettings ScrollBars="Auto" />
                    </EditFormSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="FILEXML" HeaderText="Metadati" UniqueName="column3">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="FILEDOC" HeaderText="Documento" UniqueName="column1">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ERROR" HeaderText="Errore" UniqueName="column2">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="RESULT" HeaderText="Risultato" UniqueName="column">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
                <AlternatingItemStyle BackColor="#E5E5FF" />
                <ClientSettings AllowColumnsReorder="True" ReorderColumnsOnClient="True">
                </ClientSettings>
            </DocSuite:BaseGrid>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" ID="cntFooter" runat="server">
    <table border="0" runat="server" id="buttonTable">
        <tr>
            <td style="width: 180px">
                <asp:Button ID="btnInserimento" runat="server" Text="Conferma Inserimento" Width="180px" />            
            </td>
            <td style="width: 180px">
                <asp:Button CausesValidation="False" Enabled="False" ID="btnDocumenti" runat="server" Text="Visualizza documenti" Width="180px" />
            </td>
        </tr>
    </table>
</asp:Content>