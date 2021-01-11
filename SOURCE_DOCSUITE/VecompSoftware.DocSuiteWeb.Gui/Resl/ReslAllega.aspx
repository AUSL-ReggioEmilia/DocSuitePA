<%@ Page AutoEventWireup="false" CodeBehind="ReslAllega.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslAllega" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscResolutionPreview.ascx" TagPrefix="uc" TagName="uscResolutionPreview" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function OpenSearchWindow() {
                var winManager = GetRadWindow().get_windowManager();
                setTimeout(function () {
                    var window = winManager.open("../Resl/ReslRicerca.aspx?Titolo=Ricerca&Action=CopyReslDocuments&Type=Resl", null);
                    window.set_width(GetRadWindow().get_width());
                    window.set_height(GetRadWindow().get_height());
                    window.add_close(OnSearchClientClose);
                    window.center();
                }, 0);
            }

            function OpenGenericWindow(url) {
                var wnd = window.radopen(url, null);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_showOnTopWhenMaximized(false);
                wnd.set_destroyOnClose(true);
                return wnd;
            }

            function OnSearchClientClose(sender, args) {
                sender.remove_close(OnSearchClientClose);
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest(args.get_argument());
                }
            };

            //restituisce un riferimento alla radwindow
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
    <style>
        #ctl00_cphHeader_txtNumber {width:115px!important}
    </style>
    <table class="datatable" id="header" runat="server">
        <tr>
            <th colspan="4">Selezione Atto da cui copiare i documenti
            </th>
        </tr>
        <tr>
            <td>
                <asp:RadioButtonList ID="rblProposta" RepeatDirection="Horizontal" runat="server" />
            </td>
            <td>
                <asp:HiddenField id="hf_selectYear" runat="server" />
                <asp:HiddenField id="hf_selectNumber" runat="server" />
                <telerik:RadNumericTextBox ID="txtYear" Label="Anno" MinValue="1900" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="90px" />
            </td>
            <td>
                <telerik:RadTextBox ID="txtNumber" runat="server" Width="250px" />
            </td>
            <td style="vertical-align: middle; text-align: right;">
                <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" />
                <asp:Button ID="btnCerca" OnClientClick="OpenSearchWindow();return false;" runat="server" Text="Cerca" UseSubmitBehavior="false" />
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="Requiredfieldvalidator2" runat="server" />
            </td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
            </td>
            <td></td>
        </tr>
    </table>
    <uc:uscResolutionPreview runat="server" ID="uscResolutionPreview" Visible="false" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid runat="server" ID="DocumentListGrid" AllowMultiRowSelection="True" EnableViewState="true">
        <MasterTableView AutoGenerateColumns="False" Width="100%" DataKeyNames="Serialized">
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="selectColumn" HeaderText="Copia" HeaderTooltip="Documenti da copiare" ItemStyle-Width="20px" />
                <telerik:GridTemplateColumn UniqueName="Type" HeaderText="Tipo">
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
                <telerik:GridTemplateColumn UniqueName="exportType" HeaderText="Copia conforme">
                    <ItemTemplate>
                        <telerik:RadButton AutoPostBack="false" ButtonType="ToggleButton" Checked="True" ID="CopiaConforme" runat="server" ToggleType="CheckBox" ToolTip="Seleziona per la Copia conforme, altrimenti verrà usato l'originale" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
    </telerik:RadGrid>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button CausesValidation="false" ID="btnAdd" runat="server" Text="Conferma selezione" UseSubmitBehavior="false" Visible="false" />
    <asp:HiddenField ID="txtIdResl" runat="server" />
</asp:Content>
