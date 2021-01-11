<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslProposedPrint.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslProposedPrint"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ID="Content1" runat="server"  ContentPlaceHolderID="cphHeader">
     <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            function AdoptionDateFromChanged(sender, args) {
                var rdpAdoptionDate_To = $find('<%= AdoptionDate_To.ClientID %>')
                if (rdpAdoptionDate_To.get_selectedDate()==null) {
                 rdpAdoptionDate_To.set_selectedDate(sender.get_selectedDate());
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

        <%-- Data Sessione --%>
        <asp:Panel ID="pnlSession" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Data Seduta: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="SessionDate" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="SessionDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvAdoptionDate" runat="server" /> 
                <asp:CompareValidator ControlToValidate="AdoptionDate_To" ControlToCompare="SessionDate" Operator="LessThanEqual" runat="server" Display="Dynamic" ErrorMessage="L'intervallo di Date di Adozione devono essere antecedenti alla Seduta"/> 
            </td>
        </tr>
        </asp:Panel>
        <%-- Adottata Da A --%>
        <asp:Panel ID="pnlAdottataIntervallo" runat="server">
        <tr>
            <td class="label" style="width: 20%">Adottata da data: </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="AdoptionDate_From" runat="server" ClientEvents-OnDateSelected="AdoptionDateFromChanged"/>
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="AdoptionDate_From" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvAdoptionDate_From" runat="server" />
                <span style="font-weight: bold">a data:</span>&nbsp;
                <telerik:RadDatePicker ID="AdoptionDate_To" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="AdoptionDate_To" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvAdoptionDate_To" runat="server" />                         
                <asp:CompareValidator ControlToValidate="AdoptionDate_From" ControlToCompare="AdoptionDate_To" Operator="LessThanEqual" runat="server" Display="Dynamic" ErrorMessage="La data di inizio deve essere antecedente alla data di fine intervallo"/>       
             </td>
        </tr>
        </asp:Panel>
       
        <%-- Gestione --%>
        <asp:Panel ID="pnlGestione" runat="server" Visible="False">
        <tr>
            <td class="label" style="width: 20%">Controllo Gestione: </td>
            <td align="left" style="width: 80%">
                <asp:CheckBox ID="chkCollegio" runat="server" Text="Solo Delibere soggette al Controllo del Collegio Sindacale">
                </asp:CheckBox>
            </td>
        </tr>
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
