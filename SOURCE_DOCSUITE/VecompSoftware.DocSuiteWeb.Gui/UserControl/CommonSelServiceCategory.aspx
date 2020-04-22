<%@ Page AutoEventWireup="false" CodeBehind="CommonSelServiceCategory.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelServiceCategory" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Categoria Servizio" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
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
                var des = eventArgs.getDataKeyValue("Description");
                ReturnValues(des);
            }


            function ReturnValues(valore) {
                CloseWindow(valore);
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>

    </telerik:RadScriptBlock>
    <table id="TBLTITLE" class="datatable">
        <tr>
            <td align="left" width="473" style="width: 473px; height: 36px;" valign="top">
                <table id="table1">
                    <tr>
                        <td class="label" width="20%">
                            <b>Descrizione:</b></td>
                        <td>
                            <asp:Panel runat="server" ID="pnlCerca1" DefaultButton="btnCerca" Style="float: left;">
                                <telerik:RadTextBox ID="txtCercaDes" runat="server" Width="100%" MaxLength="255" ValidationGroup="CercaDes"></telerik:RadTextBox>
                                <asp:RequiredFieldValidator ControlToValidate="txtCercaDes" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="rfvDescription" ValidationGroup="CercaDes" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" width="20%">
                            <b>Codice:</b></td>
                        <td>
                            <asp:Panel runat="server" ID="pnlCerca2" DefaultButton="btnCerca" Style="float: left;">
                                <telerik:RadTextBox ID="txtCercaCode" runat="server" Width="50%" MaxLength="10"></telerik:RadTextBox>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr class="Spazio">
                        <td colspan="2">
                            <asp:Button ID="BtnCerca" runat="server" Text="Cerca" /></td>
                    </tr>
                    <tr>
                        <td colspan="2"></td>
                    </tr>
                </table>
            </td>
            <td align="left" width="30%" valign="top" style="height: 36px">
                <asp:RadioButtonList ID="rblClausola" runat="server" Width="100%">
                    <asp:ListItem Value="AND" Selected="True">Tutte le parole.</asp:ListItem>
                    <asp:ListItem Value="OR">Almeno una parola.</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow: hidden; width: 100%; height: 100%;">
        <telerik:RadGrid AllowAutomaticDeletes="true" AllowCustomPaging="false" AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false" EnableViewState="true" GridLines="none" ID="RadGridObject" runat="server" Visible="true">
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" NextPagesToolTip="Pagina successiva" PrevPagesToolTip="Pagina precedente" />
            <MasterTableView ClientDataKeyNames="Code,Description" NoMasterRecordsText="Nessun Categoria Servizio" TableLayout="Fixed">
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
    </div>
</asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="bntInserimento" runat="server" Text="Aggiungi" ValidationGroup="CercaDes" />
</asp:Content>
