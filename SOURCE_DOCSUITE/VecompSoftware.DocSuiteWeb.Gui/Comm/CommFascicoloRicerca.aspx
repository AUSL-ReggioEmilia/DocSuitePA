<%@ Page AutoEventWireup="false" CodeBehind="CommFascicoloRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommFascicoloRicerca" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Fascicoli - Ricerca" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="uscContactSel" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagPrefix="usc" TagName="uscCategory" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table class="datatable">
        <%--ORDINE DI RICERCA: CRESCENTE/DECRESCENTE--%>
        <tr>
            <td class="label labelPanel col-dsw-2">Ordine:
            </td>
            <td class="col-dsw-7">
                <asp:RadioButtonList BorderWidth="0px" ID="rblOrder" CssClass="autoWidth" RepeatDirection="Horizontal" runat="server" Width="150px">
                    <asp:ListItem Selected="True" Text="Crescente" Value="ASC" />
                    <asp:ListItem Text="Decrescente" Value="DESC" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <%--CRITERIO DI ORDINAMENTO--%>
        <tr>
            <td class="label labelPanel col-dsw-2">Tipologia:</td>
            <td>
                <asp:RadioButtonList BorderWidth="0px" ID="tblType" CssClass="autoWidth" RepeatDirection="Horizontal" runat="server" Width="150px">
                    <asp:ListItem Selected="True" Text="Data" Value="D" />
                    <asp:ListItem Text="Numero" Value="N" />
                    <asp:ListItem Text="Classificatore" Value="C" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <%--RECLAMI--%>
        <asp:Panel ID="pnClaim" runat="server">
            <tr>
                <td class="label labelPanel col-dsw-2">Reclamo:</td>
                <td>
                    <asp:RadioButtonList ID="rblClaim" CssClass="autoWidth" RepeatDirection="Horizontal" runat="server">
                        <asp:ListItem Text="Si" Value="0" />
                        <asp:ListItem Text="No" Value="1" />
                        <asp:ListItem Selected="True" Text="Tutti" Value="2" />
                    </asp:RadioButtonList>
                </td>
            </tr>
        </asp:Panel>
        <%--DATA--%>
        <tr>
            <td class="label labelPanel">Data:</td>
            <td style="vertical-align: middle;">
                <telerik:RadDatePicker ID="rdpRegistrationDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da:" runat="server" />
                <telerik:RadDatePicker ID="rdpRegistrationDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A:" runat="server" />
            </td>
        </tr>
        <%--CONTATTI--%>
        <tr>
            <td class="label labelPanel">Contatti:</td>
            <td style="width: 500px;">
                <usc:uscContactSel ID="uscContact" runat="server" HeaderVisible="false" IsRequired="true" RequiredErrorMessage="Contatto Obbligatorio" Type="Prot" MultiSelect="true" Multiple="true" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ForceAddressBook="true" />
            </td>
        </tr>
        <%--RICERCA CONTATTI--%>
        <tr>
            <td class="label labelPanel col-dsw-2">Ricerca Contatti:</td>
            <td>
                <asp:TextBox ID="txtSearchContact" runat="server" />
                <asp:Button CausesValidation="false" ID="btnSearchContactCode" runat="server" Text="Cerca Codice" />
                <asp:Button CausesValidation="false" ID="btnSearchContact" runat="server" Text="Cerca Descrizione" />
            </td>
        </tr>
        <%--CLASSIFICATORE--%>
        <tr>
            <td class="label labelPanel col-dsw-2">Classificazione:</td>
            <td>
                <usc:uscCategory HeaderVisible="false" ID="uscCategory" Required="false" runat="server" />
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlCategory">
            <tr>
                <td class="label labelPanel">&nbsp;</td>
                <td>
                    <asp:CheckBox Checked="true" ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle Sottocategorie" />
                </td>
            </tr>
        </asp:Panel>
        <%--OGGETTO--%>
        <tr>
            <td class="label labelPanel col-dsw-2">Oggetto:</td>
            <td>
                <asp:TextBox ID="txtObjectProtocol" MaxLength="255" runat="server" Width="300px" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">&nbsp;</td>
            <td>
                <asp:RadioButtonList ID="rblObjectSearch" RepeatDirection="Horizontal" runat="server">
                    <asp:ListItem Selected="True" Text="Tutte le parole" Value="AND" />
                    <asp:ListItem Text="Almeno una parola" Value="OR" />
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnSearch" Text="Ricerca" runat="server" />
</asp:Content>
