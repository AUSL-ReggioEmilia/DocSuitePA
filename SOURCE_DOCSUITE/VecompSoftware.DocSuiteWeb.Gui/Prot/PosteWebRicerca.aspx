<%@ Page AutoEventWireup="false" CodeBehind="PosteWebRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Prot.PosteWeb.Ricerca" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Poste Web" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="datatable">
        <tr id="trSession" runat="server" visible="false">
			<td class="label labelPanel col-dsw-2"></td>
            <td  class="label labelPanel col-dsw-8" style="text-align:left">Attenzione filtro personalizzato impostato dalla pagina precedente
            </td>
        </tr>
        <tr>
            <td class="label labelPanel col-dsw-2">Account PosteWeb:
            </td>
            <td class="col-dsw-8">
                <asp:DropDownList AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" ID="ddlPolAccount" runat="server" Width="300px" />
            </td>
        </tr>
        <tr style="vertical-align: middle;">
            <td class="label labelPanel col-dsw-2">Spedizioni:
            </td>
            <td class="col-dsw-8">
                <telerik:RadDatePicker ID="dtpSentDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="dtpSentDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel col-dsw-2">Tipologia:
            </td>
            <td class="col-dsw-8">
                <asp:DropDownList ID="ddlType" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel col-dsw-2">Stato Spedizione:
            </td>
            <td class="col-dsw-8">
                <asp:RadioButtonList ID="rbtSent" RepeatDirection="Horizontal" runat="server">
                    <asp:ListItem Selected="True" Text="Tutti" Value="" />
                    <asp:ListItem Text="Spediti" Value="True" />
                    <asp:ListItem Text="Da Spedire" Value="False" />
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    <asp:Button ID="cmdPosteWebRefresh" runat="server" Text="Aggiorna" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="dgPosteRequestContact" runat="server" Width="100%">
        <HeaderStyle CssClass="tabella" />
        <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" Dir="LTR" Frame="Border" TableLayout="Auto">
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Tipo" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Image ID="imgType" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn AllowFiltering="false" DataField="SenderName" Groupable="false" HeaderStyle-Width="100px" HeaderText="Mittente" />
                <telerik:GridBoundColumn AllowFiltering="false" Groupable="false" HeaderText="Identif. Poste" DataField="GuidPoste" />
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Protocollo">
                    <ItemTemplate>
                        <asp:LinkButton ID="imgShowProt" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Nome" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="RegistrationDate" HeaderStyle-Width="65px" HeaderText="Data Reg." HeaderTooltip="Data registrazione" ItemStyle-Width="65px" UniqueName ="RegistrationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                <telerik:GridBoundColumn DataField="DataSpedizione" HeaderStyle-Width="65px" HeaderText="Data Sped." HeaderTooltip="Data Spedizione" ItemStyle-Width="65px" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                <telerik:GridBoundColumn DataField="RequestStatusDescrition" HeaderText="Stato richiesta" />
                <telerik:GridBoundColumn DataField="StatusDescrition" HeaderText="Stato consegna" />
                <telerik:GridBoundColumn DataField="CostoTotale" HeaderText="Costo" />
                <telerik:GridBoundColumn DataField="ErrorMsg" HeaderText="Errore" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnExcel" runat="server" Text="Esporta" />
</asp:Content>
