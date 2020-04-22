<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionSelect.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionSelect" %>

<%@ Register Src="~/UserControl/uscResolutionPreview.ascx" TagName="uscResolutionPreview" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="true">
    <script language="javascript" type="text/javascript">
        //Apre una finestra generale
        function <%= Me.ID %>_OpenSearchWindow() {
            var oManager = GetRadWindow().get_windowManager();
            setTimeout(function () {
                var window = oManager.open("../Resl/ReslRicerca.aspx?Type=Resl&Titolo=Selezione Atto&Action=Docm", "wndSearch");
                window.add_close(OnReslClientClose);
                window.setSize(700, 480);
                window.maximize();
            }, 0);
        }

        function OnReslClientClose(sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("<%= Me.ClientID %>" + "|" + args.get_argument());
            }
        }

    </script>
</telerik:RadScriptBlock>
<div class="resl">
    <telerik:RadAjaxPanel runat="server" ID="ajaxPanel">
        <table runat="server" id="tblSearch" class="datatable">
            <tr>
                <td style="width: 25%; vertical-align: middle;">
                    <b>Archivio di ricerca:</b></td>
                <td colspan="3">
                    <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" />
                </td>
            </tr>
            <tr>
                <td style="height: 36px; width: 25%;" runat="server" id="tdAdoptionYear">
                    <b>Anno adozione:</b>
                    <asp:TextBox ID="txtYear" runat="server" MaxLength="4" Width="56px" />
                    <asp:RegularExpressionValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
                </td>
                <td style="height: 36px; width: 28%;" runat="server" id="tdAdoptionNumber">
                    <b>Numero:</b>
                    <asp:TextBox ID="txtNumber" runat="server" Width="120px" />
                    <asp:RegularExpressionValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" />
                    <asp:CustomValidator Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                </td>
                <td style="height: 36px; width: 25%;" runat="server" id="tdIdResolution">
                    <asp:Label Font-Bold="True" ID="lblIdResl" runat="server" Text="N. prov.:" />&nbsp;
                    <asp:TextBox ID="txtIdResolution" runat="server" Width="96px" />
                </td>
                <td style="height: 36px" colspan="3" runat="server" id="tdServiceNumber">
                    <b>Numero di servizio:</b>
                    <asp:TextBox ID="txtServiceNumber" runat="server" Width="115px" />
                </td>
                <td style="height: 36px; width: 25%;">
                    <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />&nbsp;
                    <asp:Button ID="btnCerca" runat="server" Text="Cerca" />
                </td>
            </tr>
        </table>
    </telerik:RadAjaxPanel>

    <telerik:RadScriptBlock runat="server" ID="rsbtable" EnableViewState="false">
        <table style="width: 100%;">
            <tr class="Spazio">
                <td></td>
            </tr>
            <tr>
                <td>
                    <usc:uscResolutionPreview ID="uscResolutionPreview" runat="server" Visible="false" />
                </td>
            </tr>
        </table>
    </telerik:RadScriptBlock>
</div>

