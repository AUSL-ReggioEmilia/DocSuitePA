<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocumentDati.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentDati" %>

<table id="tblMovRichiesta" class="datatable">
    <tr>
        <th colspan="2"><asp:Label runat="server" ID="lblInfo" Text="Informazioni" /></th>
    </tr>
    <tr runat="server" id="tblRowDate">
        <td class="label" width="20%" style="vertical-align: middle;">
            <asp:Label runat="server" ID="lblFieldDate" Font-Bold="True" Text="Data Documento:" />
        </td>
        <td width="80%">
            <telerik:RadDatePicker ID="datePickerDate" runat="server">
                <DateInput runat="server">
                    <ReadOnlyStyle BackColor="#ffffff" BorderColor="#7f9db9" />
                </DateInput>
            </telerik:RadDatePicker>
            <asp:CompareValidator ControlToValidate="datePickerDate" Display="Dynamic" ErrorMessage="Data Documento non Valida" ID="CompareValidatorDate" Operator="DataTypeCheck" runat="server" Type="Date" />
        </td>
    </tr>
    <tr runat="server" id="tblRowObject">
        <td class="label" width="20%" style="vertical-align: middle;">
            <asp:Label runat="server" ID="lblFieldObject" Font-Bold="true" Text="Oggetto:" />
        </td>
        <td width="80%">
            <telerik:RadTextBox ID="txtObject" runat="server" Width="100%" MaxLength="255" />
        </td>
    </tr>
    <tr runat="server" id="tblRowReason">
        <td style="height: 25px; vertical-align: middle;" class="label" width="20%">
            <asp:Label runat="server" ID="lblFieldReason" Font-Bold="true" Text="Motivo:" />
        </td>
        <td style="height: 25px" width="80%">
            <telerik:RadTextBox ID="txtReason" runat="server" Width="100%" MaxLength="255" />
        </td>
    </tr>
    <tr runat="server" id="tblRowNote">
        <td class="label" width="20%" style="vertical-align: middle;">
            <asp:Label runat="server" ID="lblFieldNote" Font-Bold="true" Text="Note:" />
        </td>
        <td width="80%">
            <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" MaxLength="255" />
        </td>
    </tr>
</table>
