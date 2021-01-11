<%@ Page AutoEventWireup="false" CodeBehind="CommonSelOggetto.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelOggetto"
    Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Oggetto" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function OnRowClick(sender, eventArgs) {
                ReturnValues(eventArgs.getDataKeyValue("Description"));
            }

            function ReturnValues(values) {
                CloseWindow(values);
            }

            function CloseWindow(values) {
                var oWindow = GetRadWindow();
                oWindow.close(values);
            }
        </script>
    </telerik:RadScriptBlock>

    <asp:Panel runat="server" DefaultButton="btnCerca">
        <table class="datatable">
            <tr>
                <td class="label" width="20%">Descrizione:</td>
                <td>
                    <telerik:RadTextBox ID="txtCercaDes" MaxLength="255" runat="server" Width="100%" />
                </td>
                <td rowspan="2">
                    <asp:RadioButtonList ID="rblClausola" RepeatLayout="Flow" runat="server">
                        <asp:ListItem Selected="True" Text="Tutte le parole" Value="AND" />
                        <asp:ListItem Text="Almeno una parola" Value="OR" />
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">Codice:</td>
                <td>
                    <asp:TextBox ID="txtCercaCode" MaxLength="10" runat="server" Width="100px" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Button ID="btnCerca" runat="server" Text="Cerca" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid AllowAutomaticDeletes="true" AllowCustomPaging="false" AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false" EnableViewState="true" GridLines="none" ID="RadGridObject" runat="server" Visible="true">
        <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" NextPagesToolTip="Pagina successiva" Width="100%" PrevPagesToolTip="Pagina precedente" />
        <MasterTableView ClientDataKeyNames="Code,Description" NoMasterRecordsText="Nessun Oggetto" TableLayout="Fixed">
            <RowIndicatorColumn Visible="False">
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>
            <ExpandCollapseColumn Visible="False" Resizable="False">
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="Code" HeaderText="Codice" UniqueName="Code">
                    <HeaderStyle Width="20%"></HeaderStyle>
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Description" HeaderText="Descrizione" UniqueName="Description">
                    <HeaderStyle Width="80%"></HeaderStyle>
                </telerik:GridBoundColumn>
            </Columns>
            <EditFormSettings>
                <PopUpSettings ScrollBars="None"></PopUpSettings>
            </EditFormSettings>
            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True"></Selecting>
            <ClientEvents OnRowClick="OnRowClick"></ClientEvents>
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="bntInserimento" runat="server" Text="Aggiungi" />
</asp:Content>
