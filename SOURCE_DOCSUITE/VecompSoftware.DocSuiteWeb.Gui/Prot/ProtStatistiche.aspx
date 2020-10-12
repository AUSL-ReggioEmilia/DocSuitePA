<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtStatistiche" CodeBehind="ProtStatistiche.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Statistiche" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">

            function ApriStampa(sender, args) {
                window.print();
                return false;
            }

        </script>
    </telerik:RadScriptBlock>
    <table class="datatable">
        <tr>
            <td class="label labelPanel">Data:
            </td>
            <td style="vertical-align: middle">
                <telerik:RadDatePicker ID="txtSelDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <asp:RequiredFieldValidator ID="rfvDateFrom" runat="server" ControlToValidate="txtSelDateFrom" ErrorMessage="Seleziona una data di partenza" Display="Dynamic" />
                <telerik:RadDatePicker ID="txtSelDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                <asp:RequiredFieldValidator ID="rfvDateTo" runat="server" ControlToValidate="txtSelDateTo" ErrorMessage="Seleziona una data di fine" Display="Dynamic" />
                <asp:CompareValidator ID="cfDate" runat="server" ControlToValidate="txtSelDateFrom" ControlToCompare="txtSelDateTo" Operator="LessThanEqual" ErrorMessage="La data di inizio è più recente della data di fine" Display="Dynamic" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">Tipo:
            </td>
            <td>
                <asp:DropDownList ID="ddlType" runat="server" />
                <span class="miniLabel">Contenitore:</span>
                <asp:DropDownList ID="ddlContainer" runat="server" DataTextField="Name" DataValueField="Id"></asp:DropDownList>
                <span class="miniLabel">Utente:</span>
                <asp:TextBox ID="txtRegistrationUser" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnClaim" runat="server">
            <tr>
                <td class="label labelPanel">Reclamo:
                </td>
                <td>
                    <asp:RadioButtonList RepeatDirection="Horizontal" runat="server" ID="rblClaim">
                        <asp:ListItem Value="0">Si</asp:ListItem>
                        <asp:ListItem Value="1">No</asp:ListItem>
                        <asp:ListItem Value="2" Selected="True">Tutti</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </asp:Panel>
        <tr id="rowSelClassificatore" runat="server">
            <td class="label labelPanel">Classificatore:
            </td>
            <td>
                <uc:uscClassificatore ID="uscClassificatore" runat="server" Required="false" HeaderVisible="false" />
                <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle Sottocategorie" />
            </td>
        </tr>
        <tr id="rowChbNulle" runat="server">
            <td class="label labelPanel"></td>
            <td>
                <asp:CheckBox ID="chbNulle" runat="server" Visible="true" Text="Escludi Categorie/Sottocategorie non utilizzate" />
            </td>
        </tr>
    </table>
    <div style="display: inline-block; width: 100%;">
        <div style="float: left;">
            <telerik:RadButton ID="btnRicerca" runat="server" Text="Ricerca" OnClick="btnRicerca_Click"></telerik:RadButton>
        </div>
        <div style="float: right;">
            <telerik:RadButton ID="btnExcel" runat="server" Text="Esporta in Excel" OnClick="BtnExportExcel_Click"></telerik:RadButton>
            <telerik:RadButton ID="btnStampa" runat="server" Text="Stampa" AutoPostBack="false" OnClientClicked="ApriStampa"></telerik:RadButton>
        </div>
    </div>
 </asp:Content>
<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlProtocollo" runat="server" Visible="False" Height="100%">
        <table class="datatable">
            <!-- protocolli totali -->
            <%If Protocols.Count > 0 Then%>
            <tr>
                <th colspan="2">Totale n. Protocolli</th>
            </tr>
            <tr>
                <td style="width: 30%;">
                    <strong>Protocolli</strong>
                </td>
                <td>
                    <asp:Label ID="lblTotal" runat="server" Font-Bold="true" />
                </td>
            </tr>
            <!-- protocolli per stato -->
            <%  If ProtocolsByStatus.Count > 0 Then%>
            <tr>
                <th colspan="2">Dettaglio per Stato</th>
            </tr>
            <asp:Repeater ID="rpStatus" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="">
                            <strong><%#DataBinder.Eval(Container.DataItem,"[0]")%></strong>
                        </td>
                        <td>
                            <strong><%#DataBinder.Eval(Container.DataItem,"[1]")%></strong>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            <!-- protocolli attivi per tipo -->
            <%If ProtocolsAttivi.Count > 0 Then%>
            <tr>
                <th colspan="2">Dettaglio Attivi per Tipologia</th>
            </tr>
            <asp:Repeater ID="rpAttivi" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 30%;">
                            <strong><%#DataBinder.Eval(Container.DataItem,"[0]")%></strong>
                        </td>
                        <td>
                            <strong><%#DataBinder.Eval(Container.DataItem,"[2]")%></strong>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            <!-- protocolli attivi per contenitore -->
            <%If ProtocolsByContainer.Count > 0 Then%>
            <tr>
                <th colspan="2">Dettaglio Attivi per contenitore</th>
            </tr>
            <asp:Repeater ID="rpContainer" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 30%;">
                            <strong><%#DataBinder.Eval(Container.DataItem,"[1]")%></strong>
                        </td>
                        <td>
                            <strong><%#DataBinder.Eval(Container.DataItem,"[2]")%></strong>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            <!-- protocolli attivi per utente -->
            <%If ProtocolsByContainer.Count > 0 Then%>
            <tr>
                <th colspan="2">Dettaglio Attivi per inserimento utente</th>
            </tr>
            <asp:Repeater ID="rpUtente" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 30%;">
                            <strong><%#DataBinder.Eval(Container.DataItem,"[0]")%></strong>
                        </td>
                        <td>
                            <strong><%#DataBinder.Eval(Container.DataItem,"[1]")%></strong>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%> <%End If%>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlClassificatore" runat="server" Visible="False" Height="100%">
        <telerik:RadGrid ID="gvClassifications" AllowSorting="True" runat="server" Visible="False" AllowPaging="False" AutoGenerateColumns="False" AllowCustomPaging="False" GridLines="None" AllowMultiRowSelection="true" AllowFilteringByColumn="false" Height="100%">
            <ExportSettings ExportOnlyData="True" IgnorePaging="True" OpenInNewWindow="True"></ExportSettings>
            <MasterTableView Width="0" TableLayout="Auto" AllowMultiColumnSorting="False" AllowSorting="true" CommandItemDisplay="None" Dir="LTR" NoMasterRecordsText="Nessun Protocollo Trovato" Frame="Border" GridLines="Horizontal" RowIndicatorColumn-SortExpression="FullCode">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Codice" UniqueName="FullCode" SortExpression="FullCode" />
                    <telerik:GridTemplateColumn HeaderText="Descrizione" UniqueName="Name" SortExpression="Name" />
                    <telerik:GridTemplateColumn HeaderText="Tot." ItemStyle-HorizontalAlign="Right" UniqueName="Protocols" SortExpression="Protocols" />
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
