<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslEstrazione.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslEstrazione" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphcontent">
    <table class="dataform" width="100%">
        <tr>
            <td class="label" style="width:20%;">
                Adozione dal giorno:
            </td>
            <td style="height:100%;">
                <telerik:RadDatePicker ID="rdpAdoptionDateFrom" runat="server" />
                <asp:comparevalidator ControlToValidate="rdpAdoptionDateFrom" Display="Dynamic" ErrorMessage="Data non valida" id="CompareValidatorFrom" Operator="DataTypeCheck" runat="server" Type="Date" />
                <asp:requiredfieldvalidator ControlToValidate="rdpAdoptionDateFrom" Display="Dynamic" ErrorMessage="Campo Dal Giorno Obbligatorio" id="RequiredFieldValidatorFrom" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width:20%;">
                al giorno:
            </td>
            <td>
                <telerik:RadDatePicker ID="rdpAdoptionDateTo" runat="server" />
                <asp:comparevalidator ControlToValidate="rdpAdoptionDateTo" Display="Dynamic" ErrorMessage="Data non valida" id="CompareValidatorTo" Operator="DataTypeCheck" runat="server" Type="Date" />
                <asp:requiredfieldvalidator ControlToValidate="rdpAdoptionDateTo" Display="Dynamic" ErrorMessage="Campo Al Giorno Obbligatorio" id="RequiredFieldValidatorTo" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <asp:Button runat="server" ID="btnExtract" Text="Estrazione" />
</asp:Content>