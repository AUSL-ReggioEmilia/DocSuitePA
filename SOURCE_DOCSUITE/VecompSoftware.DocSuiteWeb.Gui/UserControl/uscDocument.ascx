<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocument.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocument" %>
<%@ Register Src="../UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagName="uscContattiTesto" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc" %>

<table id="tblPratica" class="datatable" runat="server">
    <tr>
        <th colspan="6">Pratica</th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Anno:</td>
        <td style="width: 15%;"><b><%=CurrentDocument.Year.ToString()%></b></td>
        <td class="label" style="width: 15%;">Numero:</td>
        <td style="width: 15%;"><b><%=String.Format("{0:0000000}", CurrentDocument.Number)%></b></td>
        <td class="label" style="width: 15%;">Data:</td>
        <td style="width: 25%;"><b><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.StartDate)%></b></td>
    </tr>
</table>
<table id="tblGenerale" class="datatable" runat="server">
    <tr>
        <th colspan="6">Generale</th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Contenitore:</td>
        <td style="width: 85%;"><%If CurrentDocument.Container IsNot Nothing Then Response.Write(CurrentDocument.Container.Name)%></td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Settore:</td>
        <td style="width: 85%;"><%If CurrentDocument.Role IsNot Nothing Then Response.Write(CurrentDocument.Role.Name)%>
            
        </td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Stato:</td>
        <td style="width: 85%;"><b><%If CurrentDocument.Status IsNot Nothing Then Response.Write(CurrentDocument.Status.Id & " " & CurrentDocument.Status.Description)%></b></td>
    </tr>
</table>
<table id="tblDate" class="datatable" runat="server">
    <tr>
        <th colspan="6">Date</th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Scadenza:</td>
        <td style="width: 15%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.ExpiryDate)%></td>
        <td class="label" style="width: 15%;">Riapertura:</td>
        <td style="width: 15%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.ReStartDate)%></td>
        <td class="label" style="width: 15%;">Chiusura:</td>
        <td style="width: 25%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.EndDate)%></td>
    </tr>
</table>
<table id="tblDateSovrapposte" class="datatable" runat="server" visible="false">
    <tr>
        <th colspan="6">Date</th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Inizio:</td>
        <td style="width: 15%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.StartDate)%></td>
        <td class="label" style="width: 15%;">Scadenza:</td>
        <td style="width: 15%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.ExpiryDate)%></td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Riapertura:</td>
        <td style="width: 15%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.ReStartDate)%></td>
        <td class="label" style="width: 15%;">Chiusura:</td>
        <td style="width: 25%;"><%=String.Format("{0:dd/MM/yyyy}", CurrentDocument.EndDate)%></td>
    </tr>
</table>
<table id="tblDateModifica" class="datatable" runat="server" visible="false" width="100%">
    <tr class="tabella">
        <th colspan="2">Date</th>
    </tr>
    <tr valign="middle">
        <td align="right" class="label" width="20%" style="vertical-align: middle">Data inizio:</td>
        <td valign="middle">
            <telerik:RadDatePicker ID="txtStartDateMod" runat="server" />
            <asp:CompareValidator ControlToValidate="txtStartDateMod" Display="Dynamic" ErrorMessage="Data Inizio non Valida" ID="CompareValidator2" Operator="DataTypeCheck" runat="server" Type="Date" />
            <asp:RequiredFieldValidator ControlToValidate="txtStartDateMod" Display="Dynamic" ErrorMessage="Campo Data Inizio Obbligatorio" ID="RequiredFieldValidator4" runat="server" />
        </td>
    </tr>
    <tr valign="middle">
        <td align="right" class="label" width="20%" style="vertical-align: middle">Data scadenza:</td>
        <td valign="middle">
            <telerik:RadDatePicker ID="txtExpiryDateMod" runat="server" />
            <asp:CompareValidator ControlToValidate="txtExpiryDateMod" Display="Dynamic" ErrorMessage="Data scadenza non Valida" ID="CompareValidator1" Operator="DataTypeCheck" runat="server" Type="Date" />
        </td>
    </tr>
</table>
<%-- Riferimenti --%>
<uc:uscContatti ID="uscContatti" runat="server" Visible="false" ReadOnly="False" EnableViewState="true" ForceAddressBook="true" Multiple="true" MultiSelect="true" />
<%-- end riferimenti --%>
<table id="tblDati" class="datatable" runat="server">
    <tr>
        <th colspan="6">Dati</th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">N. Posizione:</td>
        <td style="width: 85%;"><%=CurrentDocument.ServiceNumber%></td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Nome:</td>
        <td style="width: 85%;"><%=CurrentDocument.Name%></td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Oggetto:</td>
        <td style="width: 85%;"><%=CurrentDocument.DocumentObject %></td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Responsabile:</td>
        <td style="width: 85%;"><%=CurrentDocument.Manager %></td>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Note:</td>
        <td style="width: 85%;"><%=CurrentDocument.Note%></td>
    </tr>
</table>

<table id="tblDatiModifica" class="datatable" runat="server" visible="false">
    <tr>
        <th colspan="6">Dati</th>
    </tr>
    <tr style="vertical-align: middle">
        <td class="label" style="vertical-align: middle; width: 15%;">N. Posizione:</td>
        <td style="width: 85%;">
            <asp:TextBox ID="txtDocPosizione" runat="server" MaxLength="50" Width="300px" Height="20px"></asp:TextBox>
        </td>
    </tr>
    <tr style="vertical-align: middle">
        <td class="label" style="vertical-align: middle; width: 15%;">Nome:</td>
        <td style="width: 85%;">
            <telerik:RadTextBox Height="20px" ID="txtDocNome" MaxLength="255" runat="server" Width="100%" />
        </td>
    </tr>
    <tr style="vertical-align: middle">
        <td class="label" style="vertical-align: middle; width: 15%;">Oggetto:</td>
        <td style="width: 85%;">
            <telerik:RadTextBox ID="txtDocOggetto" runat="server" MaxLength="255" Width="100%" Height="20px"></telerik:RadTextBox>
            <asp:RequiredFieldValidator ControlToValidate="txtDocOggetto" Display="Dynamic" ErrorMessage="Campo Oggetto Obbligatorio" ID="RequiredFieldValidator3" runat="server" />
        </td>
    </tr>
    <tr style="vertical-align: middle">
        <td class="label" style="vertical-align: middle; width: 15%;">Responsabile:</td>
        <td>
            <uc:uscContattiTesto ID="uscManager" MaxLunghezzaTesto="1000" runat="server" Type="Docm" />
        </td>
    </tr>
    <tr style="vertical-align: middle">
        <td class="label" style="vertical-align: middle; width: 15%;">Note:</td>
        <td>
            <telerik:RadTextBox Height="20px" ID="txtDocNote" MaxLength="255" runat="server" Width="100%" />
        </td>
    </tr>
</table>


<table id="tblClassificazione" class="datatable" runat="server">
    <tr>
        <th colspan="2">Classificazione</th>
    </tr>
    <tr>
        <td class="label" style="width: 15%;">Codice:<br />
            Descrizione:</td>
        <td style="width: 85%;">
            <%=ComposeClassificatoreCode()%>
            <br />
            <%=ComposeClassificatoreDescription()%></td>
    </tr>
</table>
<table id="tblClassificatoreModifica" class="datatable" runat="server" visible="false">
    <tr>
        <td colspan="2">
            <uc:uscClassificatore ID="uscClassificatore" runat="server" />
        </td>
    </tr>
    <tr>
        <td style="width: 20%; text-align: right; vertical-align: middle;">
            <div id="spacer"></div>
            <b>Sottoclassificazione:</b></td>
        <td>
            <uc:uscClassificatore ID="uscSottoClassificatore" runat="server" HeaderVisible="false" SubCategoryMode="true" Type="Docm" />
        </td>
    </tr>
</table>
<table id="tblAltri" class="datatable" runat="server">
    <tr>
        <th colspan="4">Altri</th>
    </tr>
    <tr id="trLocazione" runat="server">
        <td class="label" style="width: 15%;">Locazione:</td>
        <td style="width: 35%;"><%If CurrentDocument.Location IsNot Nothing Then Response.Write(CurrentDocument.Location.Name)%> (<%If CurrentDocument.Location IsNot Nothing Then Response.Write(CurrentDocument.Location.Id)%>)</td>
        <td class="label" style="width: 15%;">&nbsp;</td>
        <td style="width: 35%;">&nbsp;</td>
    </tr>
    <tr id="trCreazione" runat="server">
        <td class="label" style="width: 15%;">Creato da:</td>
        <td style="width: 35%;">
            <asp:Label ID="lblRegistrationUser" runat="server"></asp:Label>
        </td>
        <td class="label" style="width: 15%;">Modificato da:</td>
        <td style="width: 35%;">
            <asp:Label ID="lblLastChangedUser" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<table id="tblPubblicazione" class="datatable" runat="server" visible="false">
    <tr>
        <th colspan="2">Pubblicazione</th>
    </tr>
    <tr id="rowPubblication">
        <td class="label" style="width: 15%;">Pubb. Internet:</td>
        <td>
            <input id="cb" type="checkbox" disabled="disabled" <%= If(CurrentDocument.CheckPublication = "1", "checked=""checked""", "") %> />
        </td>
    </tr>
</table>
<table id="tblPubblicazioneModifica" class="datatable" runat="server" visible="false">
    <tr>
        <th colspan="2">Pubblicazione</th>
    </tr>
    <tr id="rowPubblicationModifica">
        <td class="label" style="width: 15%;">Pubb. Internet:</td>
        <td>
            <asp:CheckBox runat="server" ID="chkPubblication" />
        </td>
    </tr>
</table>
<%-- Motivo di annullamento --%>
<table id="tblAnnullamento" class="datatable" runat="server">
    <tr>
        <th colspan="2">Estremi del provvedimento di annullamento del protocollo</th>
    </tr>
    <tr>
        <td style="width: 15%;" class="label">
            <asp:Image ID="imgAnnullamento" ImageUrl="~/Comm/Images/Remove32.gif" runat="server" EnableViewState="False" Height="32px" Width="32px" /></td>
        <td style="width: 85%; vertical-align: middle;">
            <b><%=CurrentDocument.LastChangedReason%></b>
        </td>
    </tr>
</table>
