<%@ Page AutoEventWireup="false" CodeBehind="AttachDocuments.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.AttachDocuments" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/DocumentSeriesItemPreview.ascx" TagName="DSIPreview" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function getCurrentPath() {
                var current = $("[id*='<%= ddlDocumentSeries.ClientID%>'] :selected");
                var path = "../Series/Search.aspx?Type=Series&Action=CopyDocuments";
                if (current && current.val) {
                    path = path + "&DocumentSeries=" + current.val();
                }

                var year = $("#<%=txtYear.ClientID%>").val();
                if (year) {
                    path = path + "&Year=" + year;
                }
                return path;
            }
            
            function OpenSearchWindow() {
                var winManager = GetRadWindow().get_windowManager();
                var window = winManager.open(getCurrentPath(), null);
                window.set_width(GetRadWindow().get_width());
                window.set_height(GetRadWindow().get_height());
                window.add_close(OnSearchClientClose);
                window.center();
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

    <div runat="server" id="HeaderWrapper">
        <table class="datatable">
            <tr>
                <th colspan="4">Selezione Serie da cui copiare i documenti
                </th>
            </tr>
            <tr>
                <td class="dsw-vertical-middle">
                        Serie
                        <asp:DropDownList runat="server" CausesValidation="false" ID="ddlDocumentSeries" Visible="True" Width="200px"  ValidationGroup="val"/>
                </td>
                <td>
                    <asp:HiddenField id="hf_selectYear" runat="server" />
                    <asp:HiddenField id="hf_selectNumber" runat="server" />
                    <telerik:RadNumericTextBox ID="txtYear" Label="Anno" MinValue="1900" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="90px" ValidationGroup="val" />
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtNumber" Label="Numero"  NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="120px" ValidationGroup="val" />
                </td>
                <td style="vertical-align: middle; text-align: right;">
                    <asp:Button ID="btnSeleziona" runat="server" Text="Seleziona" ValidationGroup="val" />
                    <asp:Button ID="btnCerca" OnClientClick="OpenSearchWindow();return false;" runat="server" Text="Cerca" UseSubmitBehavior="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RequiredFieldValidator ID="rfvDdlDocumentSeries" runat="server" ControlToValidate="ddlDocumentSeries" ErrorMessage="Campo Tipo Documento Obbligatorio" Display="Dynamic" ValidationGroup="val" />
                </td>
                <td>
                    <asp:RequiredFieldValidator ControlToValidate="txtYear" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="Requiredfieldvalidator2" runat="server" ValidationGroup="val" />
                </td>
                <td>
                    <asp:RequiredFieldValidator ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="RequiredFieldValidator1" runat="server"  ValidationGroup="val"/>
                </td>
                <td></td>
            </tr>
        </table>

        <usc:DSIPreview runat="server" ID="ItemPreview" />
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid runat="server" ID="DocumentListGrid" AllowMultiRowSelection="True" EnableViewState="true" Visible="true">
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
                        <asp:Image ID="fileImage" runat="server" />
                        <asp:Label ID="fileName" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="exportType" HeaderText="Copia conforme">
                    <ItemTemplate>
                        <telerik:RadButton AutoPostBack="false" ButtonType="ToggleButton" Checked="True" ID="pdf" runat="server" ToggleType="CheckBox" ToolTip="Seleziona per la Copia conforme, altrimenti verrà usato l'originale" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
    </telerik:RadGrid>
</asp:Content>

<asp:Content ID="PageFooter" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnAdd" runat="server" Text="Conferma selezione" Visible="false" UseSubmitBehavior="false" CausesValidation="false" />
</asp:Content>
