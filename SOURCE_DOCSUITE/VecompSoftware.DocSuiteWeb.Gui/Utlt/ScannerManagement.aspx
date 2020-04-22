<%@ Page Title="Configurazione Scanner" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ScannerManagement.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ScannerManagement" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <script language="javascript" type="text/javascript">
        function ConfirmOnEnter(p_confirmButton) {
            if (window.event.keyCode == 13) document.getElementById(p_confirmButton).click();
        }

        function CycleOnArrow(p_prevId, p_nextId) {
            var prevCtrl = document.getElementById(p_prevId);
            var nextCtrl = document.getElementById(p_nextId);

            switch (window.event.keyCode) {
                case 38:
                    if (nextCtrl) nextCtrl.focus();
                    break;
                case 40:
                    if (prevCtrl) prevCtrl.focus();
                    break;
            }
        }
    </script>
    <table class="datatable">
        <tr>
            <td class="label">Seleziona configurazione:
            </td>
            <td>
                <asp:DropDownList ID="ddlScannerConfiguration" runat="server" AutoPostBack="true" />
                <asp:Button ID="cmdAddScannerConfiguration" runat="server" Text="Nuova Configurazione" />
            </td>
        </tr>
        <tr>
            <td class="head" colspan="2">Configurazione</td>
        </tr>
        <tr>
            <td class="label">Nome:
            </td>
            <td>
                <asp:TextBox ID="txtScannerConfigurationName" runat="server" Width="300px" />
            </td>
        </tr>
        <tr>
            <td class="label">Opzioni:
            </td>
            <td>
                <asp:Button ID="cmdRenameScannerConfiguration" runat="server" Text="Rinomina" />
                <asp:Button ID="cmdDuplicateScannerConfiguration" runat="server" Text="Duplica Configurazione" />
                <asp:Button ID="cmdDeleteScannerConfiguration" runat="server" Text="Elimina Configurazione" OnClientClick="if (!confirm('Si è scelto di eliminare la configurazione corrente. \nProcedere?')) return;" />
                <asp:Button ID="cmdDefaultScannerConfiguration" runat="server" Text="Imposta Come Default" OnClientClick="if (!confirm('Si è scelto di impostare la configurazione corrente come default. \nProcedere?')) return;" />
            </td>
        </tr>
        <tr>
            <td class="head" colspan="2">Parametri</td>
        </tr>
        <tr>
            <td class="label">Opzioni:
            </td>
            <td>
                <asp:Button ID="cmdConfirmScannerParameter" runat="server" Text="Salva" />
                <asp:Button ID="cmdAddScannerParameter" runat="server" Text="Nuovo" />
                <asp:Button ID="cmdDeleteScannerParameter" runat="server" Text="Elimina selezione" />
            </td>
        </tr>
        <tr>
            <td class="label">Filtri:
            </td>
            <td>
                <asp:Panel runat="server" DefaultButton="cmdFilterScannerParameter">
                    <span class="miniLabel">Nome</span>
                    <asp:TextBox ID="txtFilterName" runat="server" Width="200px" />
                    <span class="miniLabel">Valore</span>
                    <asp:TextBox ID="txtFilterValue" runat="server" Width="200px" />
                    <span class="miniLabel">Descrizione</span>
                    <asp:TextBox ID="txtFilterDescription" runat="server" Width="200px" />
                    <asp:Button ID="cmdFilterScannerParameter" runat="server" Text="Esegui" />
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="rgScannerParameter" runat="Server" Width="100%">
        <MasterTableView>
            <RowIndicatorColumn>
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false">
                    <ItemStyle Width="20px" />
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="Id" HeaderText="Id" AllowFiltering="false">
                    <ItemStyle Width="20px" />
                    <ItemTemplate>
                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("Id") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="Name" HeaderText="Name" AllowFiltering="false">
                    <ItemStyle Width="200px" />
                    <ItemTemplate>
                        <telerik:RadTextBox ID="txtName" runat="server" Text='<%# Eval("Name") %>' Width="200px" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="Value" HeaderText="Value" AllowFiltering="false">
                    <ItemStyle Width="500px" />
                    <ItemTemplate>
                        <telerik:RadTextBox ID="txtValue" runat="server" Text='<%# Eval("Value") %>' Width="500px" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="Description" HeaderText="Description" AllowFiltering="false">
                    <ItemTemplate>
                        <telerik:RadTextBox ID="txtDescription" runat="server" Text='<%# Eval("Description") %>' TextMode="MultiLine" Height="30px" Width="400px" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>