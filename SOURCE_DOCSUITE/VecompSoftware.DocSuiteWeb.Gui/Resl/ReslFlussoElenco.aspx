<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReslFlussoElenco.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslFlussoElenco" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register Src="~/UserControl/uscProtocolSelTree.ascx" TagName="uscProtocolSelTree" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);

                return false;
            }

            function DateSelected(sender, eventArgs){
                var date = eventArgs.get_newDate();
                var pickSpedData = $find("<%= rdpSpedData.ClientID %>");
                var pickSpedGesData = $find("<%= rdpSpedGestioneData.ClientID %>");
                if(date != null){
                    pickSpedData.set_selectedDate(date);
                    pickSpedGesData.set_selectedDate(date);
                }
            }

        </script>
    </telerik:RadScriptBlock>
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblTitolo" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlNextStep" runat="server">
        <table style="width: 100%">
            <tr class="titolo">
                <td colspan="2" align="center">
                    <asp:Label ID="lblActualStep" runat="server"></asp:Label>
                    <asp:Label ID="Label2" runat="server" Width="20px">  ->  </asp:Label>
                    <asp:Label ID="lblNextStep" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
     <%--Lettera--%> 
    <asp:Panel ID="pnlLettera" runat="server">
        <table class="datatable" style="width: 100%">
            <tr>
                <th>
                    <asp:Label ID="lblLettera" runat="server"></asp:Label>
                </th>
            </tr>
            <tr class="Chiaro">
                <td>
                    <usc:uscProtocolSelTree runat="server" ID="uscLettera" ButtonDeleteVisible="true"
                        ButtonSelectVisible="true" IsRequired="true" RequiredErrorMessage="Protocollo Trasmissione Obbligatorio"
                        Multiple="false" TreeViewCaption="Protocollo trasmissione" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlOrganoContollo" runat="server">
        <table class="datatable" style="width: 100%">
            <tr>
                <th colspan="4">
                    Collegio Sindacale
                </th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 15%">
                    Spedizione:
                </td>
                <td style="width: 20%; vertical-align: middle;">
                    <telerik:RadDatePicker ID="rdpSpedCollegioData" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="rdpSpedCollegioData" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvSpedCollegioData" runat="server" />
                </td>
                <td style="width: 15%; vertical-align: top; text-align: right">
                    Prot.
                </td>
                <td style="width: 50%; vertical-align: middle;">
                    <usc:uscProtocolSelTree runat="server" ID="uscLetteraCollegio" ButtonDeleteVisible="true"
                        ButtonSelectVisible="true" IsRequired="true" RequiredErrorMessage="Protocollo Trasmissione Obbligatorio"
                        Multiple="false" TreeViewCaption="Protocollo trasmissione" />
                </td>
            </tr>
        </table>
        <%--Regione--%> 
        <table class="datatable" style="width: 100%">
            <tr>
                <th colspan="4">
                    Regione
                </th>
            </tr>
            <tr>
                <td class="label" style="width: 15%">
                    Spedizione:
                </td>
                <td style="width: 20%; vertical-align: middle;">
                    <telerik:RadDatePicker ID="rdpSpedData" runat="server" />
                </td>
                <td style="width: 15%; vertical-align: top; text-align: right">
                    Prot.
                </td>
                <td style="width: 50%; vertical-align: top">
                    <usc:uscProtocolSelTree runat="server" ID="uscLetteraRegione" ButtonDeleteVisible="true"
                        ButtonSelectVisible="true" IsRequired="true" RequiredErrorMessage="Protocollo Trasmissione Obbligatorio"
                        Multiple="false" TreeViewCaption="Protocollo trasmissione" />
                </td>
            </tr>
        </table>
        <%--Controllo Gestione--%> 
        <table class="datatable" style="width: 100%">
            <tr>
                <th colspan="4">
                    Controllo di Gestione
                </th>
            </tr>
            <tr>
                <td class="label" style="width: 15%">
                    Spedizione:
                </td>
                <td style="width: 20%; vertical-align: middle;">
                    <telerik:RadDatePicker ID="rdpSpedGestioneData" runat="server" />
                </td>
                <td style="width: 15%; vertical-align: top; text-align: right">
                    Prot.
                </td>
                <td style="width: 50%; vertical-align: top">
                    <usc:uscProtocolSelTree runat="server" ID="uscLetteraGestione" ButtonDeleteVisible="true"
                        ButtonSelectVisible="true" IsRequired="true" RequiredErrorMessage="Protocollo Trasmissione Obbligatorio"
                        Multiple="false" TreeViewCaption="Protocollo trasmissione" />
                </td>
            </tr>
        </table>
    </asp:Panel>
     <%--Flusso--%> 
    <asp:Panel ID="pnlData" runat="server">
        <table class="datatable" style="width: 100%">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblFlusso" runat="server"></asp:Label>
                </th>
            </tr>
            <tr>
                <td class="label" style="width: 15%">
                    Data:
                </td>
                <td style="width: 85%">
                    <telerik:RadDatePicker ID="rdpData" runat="server" DateInput-ReadOnly="true" Enabled="false" />
                    <asp:RequiredFieldValidator ControlToValidate="rdpData" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvData" runat="server"  />&nbsp;
                    <br />
                    <asp:CompareValidator ID="cvCompareData" runat="server" ControlToValidate="rdpData"
                        ErrorMessage="La data del workflow deve essere maggiore o uguale a " Display="Dynamic"
                        Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtLastWorkflowDate"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <%--NumeroServizio--%>
    <asp:Panel ID="pnlServiceCode" runat="server">
                 <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">Codice di servizio
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 20%">
                        <asp:Label runat="server" ID="lblNumeroServizio" Text="Codice"></asp:Label>
                    </td>
                    <td style="width: 80%">
                        <asp:DropDownList ID="ddlServizio" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                        &nbsp;
                    <span runat="server" id="ddlServizioSeparator" visible="False"></span>
                        <asp:RequiredFieldValidator ID="rfvServizio" runat="server" ControlToValidate="ddlServizio" ErrorMessage="<br />Codice di servizio Obbligatorio" Display="Dynamic" Visible="True" />                        
                        &nbsp;
                    </td>
                </tr>
            </table>
    </asp:Panel>
     <%--Frontalino digitale--%> 
    <asp:Panel ID="pnlFrontalino" runat="server" Visible="false">
        <table class="datatable" style="width: 100%">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblFrontalino" runat="server" Text="Frontalino: "></asp:Label>
                </th>
            </tr>
            <tr>
                <td class="label" style="width: 15%">
                    Data:
                </td>
                <td style="width: 85%">
                    <asp:RadioButtonList ID="radioFrontalino" runat="server">
                        <asp:ListItem Selected="True" Text="Cartaceo" Value="0"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Digitale" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
    </asp:Panel>
     <%--Fine Pubblicazione--%> 
    <asp:Panel runat="server" ID="pnlDataFine" Visible="false">
        <table class="datatable" style="width: 100%">
            <tr>
                <td class="label" style="width: 15%">
                    Data Fine Pubbl.:
                </td>
                <td style="width: 85%">
                    <telerik:RadDatePicker ID="rdpDataFine" runat="server" DateInput-ReadOnly="true" Enabled="false" />
                    <asp:RequiredFieldValidator ControlToValidate="rdpDataFine" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvDataFine" runat="server" />&nbsp;
                    <br />
                    <asp:CompareValidator ID="cvCompareDataFine" runat="server" ControlToValidate="rdpDataFine"
                        ErrorMessage="La data del workflow deve essere maggiore o uguale a " Display="Dynamic"
                        Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtLastWorkflowDate"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
     <%--Firma--%> 
    <asp:Panel ID="pnlFirma" runat="server">
        <table class="datatable" style="width: 100%">
            <tr>
                <td class="label" style="width: 15%">
                    Ruolo:
                </td>
                <td style="width: 85%">
                    <asp:DropDownList ID="cboRuolo" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="Resl-Chiaro">
                <td class="label" style="width: 15%">
                    Firma:
                </td>
                <td style="width: 85%">
                    <telerik:RadTextBox ID="txtFirma" runat="server" Width="100%" MaxLength="50"></telerik:RadTextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <%--Firmatario Collaborazione--%> 
    <asp:Panel ID="pnlCollaborationSigner" runat="server">
        <table class="datatable" style="width: 100%">
            <tr>
                <td></td>
                <td>
                    <p><asp:Label runat="server" ID="lblCollaborationSignerWarning" Text="Inserire i firmatari. Verrà creata una collaborazione per ogni contenitore."></asp:Label></p>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 15%">
                    Firmatario:
                </td>
                <td style="width: 85%">
                    <usc:uscContattiSel runat="server" ID="uscVisioneFirma" TreeViewCaption="Destinatari"
                        Multiple="true" IsRequired="true" ButtonSelectVisible="true" ButtonDeleteVisible="true"
                        ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false"
                        ButtonPropertiesVisible="false" HeaderVisible="false" ButtonRoleVisible="true"
                        EnableViewState="true" UseAD="true" EnableCheck="True" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 15%">
                    Oggetto della collaborazione:
                </td>
                <td style="width: 85%">
                    <telerik:RadTextBox ID="txtCollaborationObject" TextMode="MultiLine" Rows="3" runat="server" Width="100%"></telerik:RadTextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <%--Avviso per l'utente--%> 
    <asp:Panel ID="pnlMessage" runat="server">
        <table class="datatable" style="width: 100%">
            <tr>
                <td>
                  <p>      
                      Il processo di pubblicazione viene completato in automatico dal sistema.
                  </p> 
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:TextBox ID="txtLastWorkflowDate" runat="server" Width="16px"></asp:TextBox>
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
</asp:Content>
