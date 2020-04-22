<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslPrint.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslPrint"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscProtocolSelTree.ascx" TagName="uscProtocolSelTree" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSelTemplate.ascx" TagName="uscSelTemplate" TagPrefix="usc" %>

<asp:Content ID="Content1" runat="server"  ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            function AdoptionDateChanged(dateInput, args) {
                document.getElementById('<%= AddNumber.ClientID %>').click();
            }

            function lstNumberSelectedIndexChanged() {
                document.getElementById('<%= txtNumber.ClientID %>').value = document.getElementById('<%= lstNumber.ClientID %>').value
            }

            function CblNumbersSelezionaTutti() {
                var cblNumbers = document.getElementById("<%= cblNumbers.ClientID %>");
                var cblNumbersItems;
                if (cblNumbers) cblNumbersItems = cblNumbers.getElementsByTagName("INPUT");

                for (var i = cblNumbersItems.length; i > 0; i--) {
                    if (!cblNumbersItems[i - 1].disabled) cblNumbersItems[i - 1].checked = !cblNumbersItems[0].checked;
                }
            }

        </script>
    </telerik:RadScriptBlock>
    <asp:Panel runat="server" ID="pnlHeaders">
    <table class="dataform">
        <%-- Tipologia --%>
        <asp:Panel ID="pnlTipologia" runat="server">
        <tr>
            <td class="label" width="20%">Tipologia: </td>
            <td align="left" width="80%">
                <asp:RadioButtonList ID="rblTipologia" runat="server" RepeatDirection="Horizontal"
                    AutoPostBack="True" Font-Bold="True" Width="300px">
                </asp:RadioButtonList>
            </td>
        </tr>
        </asp:Panel>
        <%-- Omissis --%>
        <asp:Panel ID="pnlOmissis" runat="server">
        <tr>
            <td class="label" width="20%">Privacy: </td>
            <td align="left" width="80%">
                <asp:CheckBox ID="chbOmissis" runat="server">
                </asp:CheckBox>
            </td>
        </tr>
        </asp:Panel>
        <%-- Firma Digitale --%>
        <asp:Panel ID="pnlUPDigital" runat="server" Visible="false">
        <tr>
            <td class="label" width="20%">Firma digitale: </td>
            <td align="left" width="80%">
                <asp:CheckBox ID="chkDigitalUP" runat="server" Font-Bold="True" Width="100%" Text="Predisponi Ultima Pagina per firma digitale" Checked="false"></asp:CheckBox>
            </td>
        </tr>
        </asp:Panel>
        <%-- Frontalino --%>
        <asp:Panel ID="pnlFDigital" runat="server" Visible="false">
        <tr>
            <td class="label" width="20%">Frontalino: </td>
            <td align="left" width="80%">
                <asp:RadioButtonList ID="radioFrontalino" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Text="Cartaceo" Value="0"></asp:ListItem>
                    <asp:ListItem Selected="False" Text="Digitale" Value="1"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        </asp:Panel>
        <%-- Print --%>
        <asp:Panel ID="pnlPrint" runat="server">
        <%-- Adozione Da --%>
        <tr>
            <td class="label" style="width: 20%">Adozione dal giorno:</td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="PrintDate_From" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="PrintDate_From" Display="Dynamic" ErrorMessage="Campo Dal Giorno Obbligatorio" ID="rfvPrintDate_From" runat="server" />
            </td>
        </tr>
        <%-- Adozione A --%>
        <tr>
            <td class="label" style="width: 20%">Al giorno: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="PrintDate_To" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="PrintDate_To" Display="Dynamic" ErrorMessage="Campo Al Giorno Obbligatorio" ID="rfvPrintDate_To" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnlNumero" runat="server">
        <%-- Anno --%>
        <tr>
            <td class="label" style="width: 20%">Anno adozione: </td>
            <td align="left" style="width: 80%">
                <asp:TextBox ID="txtYear" runat="server" Width="96px" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <%-- Numero Dal Al --%>
        <tr>
            <td class="label" style="width: 20%">Dal numero: </td>
            <td align="left" style="width: 80%">
                <asp:TextBox ID="txtNumber_From" runat="server" Width="96px" MaxLength="10"></asp:TextBox>
                &nbsp;<span style="font-weight: bold;">al numero:</span>
                <asp:TextBox ID="txtNumber_To" runat="server" Width="96px" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        </asp:Panel>
        <%-- Intestazione --%>
        <asp:Panel ID="pnlHeading" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Intestazione: </td>
            <td align="left" style="width: 80%">
                <telerik:RadTextBox ID="txtHeadingFrontalino" runat="server" Width="100%" MaxLength="255"
                    Visible="true" TextMode="MultiLine"></telerik:RadTextBox>
            </td>
        </tr>
        </asp:Panel>
        </asp:Panel>
        <%-- Proposer --%>
        <asp:Panel ID="pnlPropInterop" runat="server">
        <tr>
            <td class="label" style="width: 20%">Proponente: </td>
            <td align="left" style="width: 80%">
                <usc:uscContattiSel runat="server" ID="uscPropInterop" ButtonSelectVisible="true"
                    ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false"
                    ButtonManualVisible="false" ButtonPropertiesVisible="false" TreeViewCaption="Proponente"
                    HeaderVisible="false" IsRequired="true" RequiredErrorMessage="Campo Proponente Obbligatorio"
                    Type="Resl" Multiple="true" MultiSelect="true" />
            </td>
        </tr>
        </asp:Panel>
        <%-- Lettere --%>
        <asp:Panel ID="pnlPrintLettere" runat="server">
        <%-- Lettere Determina --%>
        <asp:Panel ID="pnlTipoLettereDet" runat="server">
        <tr>
            <td class="label" style="width: 20%">Tipologia Lettera: </td>
            <td align="left" style="width: 80%">
                <asp:RadioButtonList ID="rblTipoLetteraDet" runat="server" AutoPostBack="True">
                    <asp:ListItem Value="TAD" Selected="True">Trasm. Adozione Determine</asp:ListItem>
                    <asp:ListItem Value="P">Pubblicazione Albo</asp:ListItem>
                    <asp:ListItem Value="AL">Altra Lettera</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        </asp:Panel>
        <%-- Lettere Delibera --%>
        <asp:Panel ID="pnlTipoLettere" runat="server">
        <tr>
            <td class="label" style="width: 20%">Tipologia Lettera: </td>
            <td align="left" style="width: 80%">
                <asp:RadioButtonList ID="rblTipoLettera" runat="server" AutoPostBack="True">
                    <asp:ListItem Value="TACS">Trasm. Adozione Collegio Sindacale</asp:ListItem>
                    <asp:ListItem Value="TAR-B">Trasm. Adozione Regione (bilancio consuntivo esercizio)</asp:ListItem>
                    <asp:ListItem Value="TAR-A">Trasm. Adozione Regione (modifiche atto aziendale)</asp:ListItem>
                    <asp:ListItem Value="TAR-P">Trasm. Adozione Regione (piano attivit&#224; e Bilancio previsione)</asp:ListItem>
                    <asp:ListItem Value="TAG">Trasm. Adozione per il Controllo di Gestione</asp:ListItem>
                    <asp:ListItem Value="P">Pubblicazione Albo</asp:ListItem>
                    <asp:ListItem Value="AL">Altra Lettera</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        </asp:Panel>
        <%-- Altra Lettera --%>
        <asp:Panel ID="pnlAltraLettera" runat="server">
        <tr>
            <td class="label" style="width: 20%">Lettera Regione: </td>
            <td align="left" style="width: 80%">
                <usc:uscSelTemplate runat="server" ID="uscDocumento" ButtonSelectVisible="true" IsRequired="true"
                    Multiple="false" TreeViewCaption="Lettera Regione" RequiredErrorMessage="Documento Obbligatorio"
                    Type="Resl" />
            </td>
        </tr>
        </asp:Panel>
        <%-- Adottata In --%>
        <asp:Panel ID="pnlAdottata" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Adottata in data: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="AdoptionDate" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="AdoptionDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvAdoptionDate" runat="server" />
                <asp:Button ID="AddNumber" runat="server" Text="Add" CausesValidation="False"></asp:Button>
            </td>
        </tr>
        </asp:Panel>
        <%-- Adottata Da A --%>
        <asp:Panel ID="pnlAdottataIntervallo" runat="server">
        <tr>
            <td class="label" style="width: 20%">Adottata da data: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="AdoptionDate_From" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="AdoptionDate_From" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvAdoptionDate_From" runat="server" />
                <span style="font-weight: bold">a data:</span>&nbsp;
                <telerik:RadDatePicker ID="AdoptionDate_To" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="AdoptionDate_To" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvAdoptionDate_To" runat="server" />
                <asp:Button ID="cmdAddNumberByAdottataIntervallo" runat="server" Text="Estrai" Visible="false" />
             </td>
        </tr>
        </asp:Panel>
        <%-- Numero Regione --%>
        <asp:Panel ID="pnlNumeroRegione" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Numero: </td>
            <td align="left" style="width: 80%">
                <asp:TextBox ID="txtNumber" runat="server" Width="96px" MaxLength="7"></asp:TextBox>&nbsp;
                <asp:RegularExpressionValidator ID="cvNumber" runat="server" ControlToValidate="txtNumber"
                    ErrorMessage="Errore formato" ValidationExpression="\d*"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="rfvNumber" runat="server" ControlToValidate="txtNumber"
                    ErrorMessage="Numero Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator><br />
                <asp:Panel ID="pnlListaNumeroRegione" runat="server" Visible="False">
                    <asp:ListBox ID="lstNumber" runat="server" Width="96px" DataTextField="Number" DataValueField="Number"
                        onclick="return lstNumberSelectedIndexChanged();"></asp:ListBox>
                </asp:Panel>
            </td>
        </tr>
        </asp:Panel>
        <%-- Pubblicata --%>
        <asp:Panel ID="pnlPubblicata" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Pubblicata in data: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="PublishingDate" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="PublishingDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvPublishingDate" runat="server" />
            </td>
        </tr>
        </asp:Panel>
        <%-- Esecutiva --%>
        <asp:Panel ID="pnlEsecutivita" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Esecutiva in data: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="EffectivenessDate" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="EffectivenessDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvEffectivenessDate" runat="server" />
            </td>
        </tr>
        </asp:Panel>
        <%-- Data --%>
        <asp:Panel ID="pnlData" runat="server">
        <tr>
            <td class="label" style="width: 20%">Data di invio: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="txtData" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtData" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvData" runat="server" />
            </td>
        </tr>
        </asp:Panel>
        <asp:Panel ID="pnlFirma" runat="server">
        <%-- Ruolo --%>
        <tr>
            <td class="label" style="width: 20%">Ruolo: </td>
            <td align="left" style="width: 80%">
                <asp:DropDownList ID="cboRuolo" runat="server">
                    <asp:ListItem Text="Il funzionario delegato" Value="FD"></asp:ListItem>
                </asp:DropDownList>
                <br />
            </td>
        </tr>
        <%-- Firma --%>
        <tr>
            <td class="label" style="width: 20%">Firma: </td>
            <td align="left" style="width: 80%">
                <asp:TextBox ID="txtFirma" runat="server" Width="300px" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFirma" runat="server" ControlToValidate="txtFirma"
                    ErrorMessage="Firma Obbligatoria" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        </asp:Panel>
        <%-- Protocollo --%>
        <asp:Panel ID="pnlProtocollo" runat="server" Visible="true">
        <tr>
            <td class="label" style="width: 20%"><asp:Label ID="lblProtocollo" runat="server" Font-Bold="True"></asp:Label></td>
            <td align="left" style="width: 80%">
                <usc:uscProtocolSelTree runat="server" ID="uscProtocollo" ButtonDeleteVisible="true"
                    ButtonSelectVisible="true" IsRequired="true" RequiredErrorMessage="Protocollo Trasmissione Obbligatorio"
                    Multiple="false" TreeViewCaption="Protocollo trasmissione" />
            </td>
        </tr>
        </asp:Panel>
        <%-- Contenitore --%>
        <asp:Panel ID="pnlContenitore" runat="server" Visible="true">
        <tr>
            <td class="label" style="width: 20%">Contenitore: </td>
            <td align="left" style="width: 80%">
                <asp:DropDownList ID="ddlContainers" runat="server">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="ContenitoreValidator" runat="server" ControlToValidate="ddlContainers"
                    ErrorMessage="Campo contenitore obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        </asp:Panel>
        <%-- Numero Regione Multiplo --%>
        <asp:Panel ID="pnlNumeroRegioneMultiplo" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Numeri: </td>
            <td align="left" style="width: 80%">
                <div style="height: 150px; width: 300px; overflow-y: scroll; border: solid 1px #000000;">
                    <asp:CheckBoxList ID="cblNumbers" runat="server" />
                </div>
                <div style="margin-top: 1px; padding: 1px; width: 300px; text-align: right; border: solid 1px #000000;">
                    <input type="button" id="cmdSelectAllNumbers" onclick="CblNumbersSelezionaTutti();"
                        value="Seleziona/Deseleziona tutti" />
                </div>
            </td>
        </tr>
        </asp:Panel>
        <%-- Gestione --%>
        <asp:Panel ID="pnlGestione" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Controllo Gestione: </td>
            <td align="left" style="width: 80%">
                <asp:CheckBox ID="chkGestione" runat="server" Text="Solo Delibere soggette al Controllo di Gestione">
                </asp:CheckBox>
            </td>
        </tr>
        </asp:Panel>
        <%-- Stampa Lettera --%>
        <asp:Panel ID="pnlStampaLettera" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Stampa Lettera: </td>
            <td align="left" style="width: 80%">
                <asp:CheckBox ID="chkLettera" runat="server" AutoPostBack="True" Text="Stampa Lettera Trasm. Adozione Servizi (comunicazione mensile)">
                </asp:CheckBox>
            </td>
        </tr>
        </asp:Panel>
        <%-- Stampa Elenco --%>
        <asp:Panel ID="pnlStampaElenco" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Stampa Elenco: </td>
            <td align="left" style="width: 80%">
                <asp:CheckBox ID="chkElenco" runat="server" AutoPostBack="True" Text="Stampa Elenco Provvedimenti">
                </asp:CheckBox>
            </td>
        </tr>
        </asp:Panel>
        </asp:Panel>
    </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlControls">
    <table style="height: 100%; width: 100%;">
        <%-- Contenitore --%>
        <asp:Panel ID="pnlContenitoreTvw" runat="server">
        <tr style="height: 13px">
            <td>
                <asp:Button ID="btnSelectAll" runat="server" Width="120px" Text="Seleziona tutti"
                    CausesValidation="False"></asp:Button>
                <asp:Button ID="btnDeselectAll" runat="server" Width="120px" Text="Annulla selezione"
                    CausesValidation="False"></asp:Button>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top; width: 50%;">
                <telerik:RadTreeView ID="tvwContenitore" CheckBoxes="True" runat="server">
                    <CollapseAnimation Duration="100" Type="OutQuint" />
                    <ExpandAnimation Duration="100" Type="OutQuart" />
                </telerik:RadTreeView></td>
            <td style="vertical-align: top; width: 50%;">
                &nbsp;
            </td>
        </tr>
        </asp:Panel>
    </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="cphFooter">
    <table width="100%">
        <tr>
            <td>
                <asp:Button ID="cmdStampa" runat="server" Text="Stampa"></asp:Button>
            </td>
            <td align="right">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
