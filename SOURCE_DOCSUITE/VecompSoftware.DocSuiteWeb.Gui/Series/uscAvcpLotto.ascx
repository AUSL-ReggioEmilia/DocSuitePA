<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscAvcpLotto.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.uscLotto" %>

<%@ Register Src="~/UserControl/uscSelezionaAziende.ascx" TagPrefix="uc" TagName="uscSelezionaAziende" %>

<!-- Seleziona aziende partecipanti del bando di gara-->
<telerik:RadWindow ID="windowsAziendePartecipanti" runat="server" Width="700px" Height="350px" Modal="true">
    <ContentTemplate>
        <uc:uscSelezionaAziende ID="uscAziendePartecipanti" runat="server" SearchEnabled="False"></uc:uscSelezionaAziende>
    </ContentTemplate>
</telerik:RadWindow>
<!-- Seleziona aziende aggiudicatarie del bando di gara-->
<telerik:RadWindow ID="windowsAziendeAggiudicatarie" runat="server" Width="700px" Height="350px" Modal="true">
    <ContentTemplate>
        <uc:uscSelezionaAziende ID="uscAziendeAggiudicatarie" runat="server" SearchEnabled="False"></uc:uscSelezionaAziende>
    </ContentTemplate>
</telerik:RadWindow>

<telerik:RadAjaxPanel runat="server" ID="panelIntestazione">
    <table class="datatable">
    <tr>
        <th colspan="2">
            Lotto
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 20%">CIG</td>
        <td>
            <telerik:RadTextBox ID="cig" MaxLength="10" runat="server" ToolTip="Codice Identificativo Gara rilasciato dall’Autorità" />
        </td>
    </tr>
    <tr>
        <td class="label" rowspan="2">Struttura Proponente</td>
        <td>
            <telerik:RadTextBox id="structFiscalCode" Label="Codice fiscale" MaxLength="11" Width="250px" runat="server" ToolTip="Codice fiscale della Stazione Appaltante responsabile del procedimento di scelta del contraente" />
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadTextBox id="structName" Label="Denominazione" MaxLength="250" Width="250px" runat="server" ToolTip="Denominazione della Stazione Appaltante responsabile del procedimento di scelta del contraente" />
        </td>
    </tr>
    <tr>
        <td class="label">Oggetto</td>
        <td>
            <telerik:RadTextBox id="subject" MaxLength="250" runat="server" Width="100%" />
        </td>
    </tr>
    <tr>
        <td class="label">Scelta Contraente</td>
        <td>
            <telerik:RadComboBox CausesValidation ="False" EnableCheckAllItemsCheckBox="false" EnableItemCaching="True" ID="choice" runat="server" ToolTip="Procedura di scelta del contraente" ViewStateMode ="Enabled" Width="300" />
        </td>
    </tr>
    <tr>
        <td class="label">Aziende Partecipanti</td>
        <td>
            <asp:Label runat="server" ID="lblSumAziendePartecipanti"></asp:Label>
            <telerik:RadButton ID="btnElencoAziendePartecipanti" runat="server" Text="Elenco" />      
        </td>
    </tr>
    <tr>
        <td class="label">Aziende Aggiudicatari</td>
        <td>
            <asp:Label runat="server" ID="lblSumAziendeAggiudicatarie"></asp:Label>
            <telerik:RadButton ID="btnElencoAziendeAggiudicatarie" runat="server" Text="Elenco" />      
        </td>
    </tr>
    <tr>
        <td class="label">Importo aggiudicazione</td>
        <td>
            <telerik:RadNumericTextBox ID="awardAmount" IncrementSettings-InterceptArrowKeys="True" NumberFormat-DecimalDigits="2" runat="server" ToolTip="Importo di aggiudicazione al lordo degli oneri di sicurezza ed al netto dell’IVA" Width="150px" />
        </td>
    </tr>
    <tr>
        <td class="label">Importo somme liquidate</td>
        <td>
            <telerik:RadNumericTextBox ID="paidAmount" IncrementSettings-InterceptArrowKeys="True" NumberFormat-DecimalDigits="2" runat="server" ToolTip="Importo complessivo dell’appalto al netto dell’IVA" Width="150px" />
        </td>
    </tr>
    <tr>
        <td class="label" rowspan="2">Tempi Completamento</td>
        <td>
            <telerik:RadDatePicker ID="begin" DateInput-LabelWidth="35%" Style="height: auto !important;" Width="230" DateInput-Label="Data Inizio" ToolTip="Data di effettivo inizio lavori, servizi o forniture" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadDatePicker ID="end" DateInput-LabelWidth="45%" Style="height: auto !important;" Width="250" DateInput-Label="Data Ultimazione" ToolTip="Data di ultimazione lavori, servizi o forniture" runat="server" />
        </td>
    </tr>
</table>
</telerik:RadAjaxPanel>