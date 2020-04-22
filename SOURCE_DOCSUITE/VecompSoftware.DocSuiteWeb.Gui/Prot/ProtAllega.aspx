<%@ Page AutoEventWireup="false" CodeBehind="ProtAllega.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtAllega" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagPrefix="usc" TagName="ProtocolPreview" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function OpenSearchWindow() {
                var winManager = GetRadWindow().get_windowManager();
                var window = winManager.open("../Prot/ProtRicerca.aspx?Titolo=Selezione Protocollo&Action=CopyProtocolDocuments", "wndSearch");
                window.set_width(GetRadWindow().get_width());
                window.set_height(GetRadWindow().get_height());
                window.add_close(OnSearchClientClose);
                window.center();
            }
            
            function OpenGenericWindow(button, args) {
                if (args._commandArgument === null)
                    return false;

                var wnd = DSWOpenGenericWindow(args._commandArgument, WindowTypeEnum.NORMAL);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_showOnTopWhenMaximized(false);
                wnd.set_destroyOnClose(true);
                return wnd;
            }

            // funzione di chiusura della finestra (prima era iniettata nella pagina chiamante: ProtVisualizza)
            function OnSearchClientClose(sender, args) {
                sender.remove_close(OnSearchClientClose);
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest(args.get_argument());
                }
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }

        </script>
    </telerik:RadScriptBlock>

    <table class="datatable" runat="server" id="headerTable">
        <tr>
            <th colspan="3">
                Selezione Protocollo da cui copiare i documenti
            </th>
        </tr>
        <tr>
            <td>
                <asp:HiddenField id="hf_selectYear" runat="server" />
                <asp:HiddenField id="hf_selectNumber" runat="server" />
                <telerik:RadNumericTextBox ID="txtYear" Label="Anno" MinValue="1900" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="90px" />
            </td>
            <td>
                <telerik:radTextBox Label="Numero" ID="txtNumber" runat="server" Width="130px" MaxLength="7" />
            </td>
            <td style="vertical-align: middle; text-align: right;">
                <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />
                <asp:Button ID="btnCerca" OnClientClick="OpenSearchWindow();return false;" runat="server" Text="Cerca" UseSubmitBehavior="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:RegularExpressionValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
                <asp:RequiredFieldValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="Requiredfieldvalidator2" runat="server" />
            </td>
            <td>
                <asp:RegularExpressionValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" />
                <asp:RequiredFieldValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
            </td>
            <td></td>
        </tr>
    </table>
    <usc:ProtocolPreview runat="server" ID="uscProtocolPreview" Visible="false" Title="Dettaglio [Protocollo]" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
   <telerik:RadGrid runat="server" ID="DocumentListGrid" AllowMultiRowSelection="True" EnableViewState="true">
        <MasterTableView AutoGenerateColumns="False" Width="100%" DataKeyNames="Serialized">
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="selectColumn" HeaderText="Copia" HeaderTooltip="Documenti da copiare" ItemStyle-Width="20px" />
                <telerik:GridTemplateColumn  ItemStyle-Width="70px" HeaderText="Tipo">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="fileType" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento">
                    <ItemTemplate>
                        <asp:ImageButton ID="fileImage" runat="server" CommandName="preview" />
                        <asp:LinkButton ID="fileName" runat="server" CommandName="preview" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="exportType" HeaderText="Copia conforme"  ItemStyle-Width="100px">
                    <ItemTemplate>
                        <telerik:RadButton AutoPostBack="false" ButtonType="ToggleButton" Checked="True" ID="CopiaConforme" runat="server" ToggleType="CheckBox" ToolTip="Seleziona per la Copia conforme, altrimenti verrà usato l'originale" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="false" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
    </telerik:RadGrid>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <div runat="server" id="footerTable">
        <asp:Button ID="btnAdd" runat="server" Text="Conferma selezione" Enabled="False" Visible="false" UseSubmitBehavior="false" CausesValidation="false" />
        <asp:Button ID="btnAddPdf" runat="server" Text="Allega Copia conforme" Enabled="False" Visible="false" UseSubmitBehavior="false" CausesValidation="false" />
    </div>
</asp:Content>
