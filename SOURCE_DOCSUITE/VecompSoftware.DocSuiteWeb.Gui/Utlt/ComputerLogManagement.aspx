<%@ Page Title="Gestione ComputerLog" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ComputerLogManagement.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ComputerLogManagement" %>
<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label">Filtri:
            </td>
            <td>
                <asp:Panel DefaultButton="cmdFilterSystemComputer" runat="server">
                <span class="miniLabel">Id</span>
                <asp:TextBox ID="txtFilterSystemComputer" runat="server" Width="200px" />
                <span class="miniLabel">Utente</span>
                <asp:TextBox ID="txtFilterSystemUser" runat="server" Width="200px" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <asp:Button ID="cmdFilterSystemComputer" runat="server" Text="Esegui" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid AllowPaging="true" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="rgComputerLog" PagerStyle-AlwaysVisible="true" PagerStyle-Mode="NextPrevAndNumeric" PagerStyle-Position="Top" PagerStyle-Visible="true" PageSize="50" runat="Server" Width="100%">
        <MasterTableView>
            <RowIndicatorColumn>
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="Id" HeaderText="Id" AllowFiltering="false" UniqueName="Id">
                    <ItemStyle Width="200px" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SystemServer" HeaderText="SystemServer" AllowFiltering="false" UniqueName="SystemServer">
                    <ItemStyle Width="200px" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SystemUser"  HeaderText="SystemUser" UniqueName="SystemUser">                    
                    <ItemStyle Width="200px" />                        
                </telerik:GridBoundColumn>

                <telerik:GridTemplateColumn DataField="AdvancedScanner" HeaderText="AdvancedScanner" AllowFiltering="false">
                    <ItemStyle Width="200px" />
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlAdvancedScanner" runat="server" CommandArgument='<%# Eval("Id") %>'>
                            <asp:ListItem Text="DSScan" Value="0"></asp:ListItem>
                            <asp:ListItem Text="SmartClient" Value="1"></asp:ListItem>
                            <asp:ListItem Text="ScannerLight" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="IdScannerConfiguration" HeaderText="ScannerConfiguration" AllowFiltering="false" UniqueName="ScannerConfiguration">
                    <ItemStyle Width="200px" />
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlIdScannerConfiguration" runat="server" CommandArgument='<%# Eval("Id") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="AdvancedViewer" HeaderText="AdvancedViewer" AllowFiltering="false">
                    <ItemStyle Width="200px" />
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlAdvancedViewer" runat="server" CommandArgument='<%# Eval("Id") %>'>
                            <asp:ListItem Text="DSView" Value="0"></asp:ListItem>
                            <asp:ListItem Text="SmartClient" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Light + DSView" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Light + SmartClient" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="IdZebraPrinter" HeaderText="ZebraPrinter" AllowFiltering="false" UniqueName="ZebraPrinter">
                    <ItemStyle Width="200px" />
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlIdZebraPrinter" runat="server" CommandArgument='<%# Eval("Id") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
