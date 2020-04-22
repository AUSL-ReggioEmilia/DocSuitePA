<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonChangeOggetto.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonChangeOggetto" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Modifica Oggetto" %>

<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="uscObject" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
         <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
             function GetRadWindow() {
                 var oWindow = null;
                 if (window.radWindow) oWindow = window.radWindow;
                 else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                 return oWindow;
             }

             function ReturnValue(object, reason) {
                 var wnd = GetRadWindow();
                 wnd.close(object + "|" + reason);
             }
         </script>
    </telerik:RadScriptBlock>

<table id="tblObject" class="datatable">
    <tr>
        <th>
            Oggetto
        </th>
    </tr>
    <tr>
        <td>
            <asp:textbox Enabled="False" id="txtObjectOld" runat="server" textmode="MultiLine" width="600px" />
        </td>
    </tr>
    <tr>
        <th>
            Motivazione Cambio Oggetto
        </th>
    </tr>
    <tr>
        <td>
            <asp:textbox Enabled="False" id="txtObjectReasonOld" maxlength="255" runat="server" textmode="MultiLine" width="600px" />
        </td>
    </tr>
    <tr class="Spazio">
        <td>&nbsp;</td>
    </tr>
    <tr>
        <th>
            Nuovo Oggetto
        </th>
    </tr>
    <tr>
        <td Width="670px">
            <usc:uscObject EditMode="true" ID="uscObject" MaxLength="255" MultiLine="true" Required="true" RequiredMessage="Il campo oggetto non è stato modificato" runat="server" Type="Prot" />
        </td>
    </tr>
    <tr>
        <th>
            Nuova Motivazione Cambio Oggetto
        </th>
    </tr>
    <tr>
        <td>
            <asp:textbox id="txtObjectReason" MaxLength="255" runat="server" TextMode="MultiLine" Width="600px" />
            <br />
            <asp:RequiredFieldValidator controltovalidate="txtObjectReason" Display="Dynamic" errormessage="Campo Motivazione Obbligatorio" id="rfvObjectReason" runat="server" />
        </td>
    </tr>
</table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConferma" runat="server" Text="Conferma" />
    <asp:ValidationSummary DisplayMode="List" id="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
</asp:Content>			
