<%@ Page Title="Nome del tavolo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskInsert.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskInsert" %>

<%@ Register Src="~/UserControl/uscDeskDocument.ascx" TagPrefix="usc" TagName="DeskDocument" %>
<%@ Register Src="~/UserControl/uscInvitationUser.ascx" TagPrefix="usc" TagName="InvitationUser" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= deskContainer.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= deskContainer.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel CssClass="col-dsw-10" runat="server" ID="deskContainer">
        <!-- Nome del Tavolo -->
        <asp:Panel runat="server" ID="pnlDeskName">
            <table class="datatable">
                <tr>
                    <td class="col-dsw-1 DeskLabel" style="vertical-align: middle!important;">
                        <asp:Label ID="lbNome" runat="server" Text="Nome:" style="vertical-align: middle;"></asp:Label>
                    </td>
                    <td class="col-dsw-9">
                        <telerik:RadTextBox runat="server" ID="txtDeskName"  MaxLength="500" Width="100%" TextMode="SingleLine"/>
                        <asp:RequiredFieldValidator runat="server" ValidationGroup="deskValidation" ID="deskNameValidator" Display="Dynamic" ControlToValidate="txtDeskName" ErrorMessage="Il campo Titolo è obbligatorio"/>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Oggetto del Tavolo -->
        <asp:Panel runat="server" ID="pnlDeskSubject">
            <table class="datatable">
                <tr>
                    <th colspan="2">Oggetto/Descrizione</th>
                </tr>
                <tr>
                    <td class="col-dsw-10">
                        <telerik:RadTextBox runat="server" ID="txtObject" Width="100%" MaxLength="500" InputType="Text" TextMode="MultiLine" Rows="4"/>
                        <asp:RequiredFieldValidator runat="server" ValidationGroup="deskValidation" ID="deskObjectValidator" Display="Dynamic" ControlToValidate="txtObject" ErrorMessage="Il campo Oggetto/Descrizione è obbligatorio"/>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Scadenza del Tavolo -->
        <asp:Panel runat="server" ID="pnlDeskDateExpired">
            <table class="datatable">
                <tr>
                    <th colspan="2">Informazioni</th>
                </tr>
                <tr>
                    <td class="col-dsw-1 DeskLabel" style="vertical-align: middle!important;">
                        <asp:Label ID="lblDeskExpired" runat="server" Text="Data scadenza:" />
                    </td>
                    <td class="col-dsw-9">
                        <telerik:RadDatePicker runat="server" ID="dtpDeskExpired" Width="200px"/>
                        <asp:RequiredFieldValidator runat="server" ValidationGroup="deskValidation" ID="deskDateExpiredValidator" Display="Dynamic" ControlToValidate="dtpDeskExpired" ErrorMessage="Il campo Data scadenza è obbligatorio"/>
                        <asp:CompareValidator ID="cvCompareData" runat="server" Type="Date" Operator="GreaterThanEqual" ControlToValidate="dtpDeskExpired" ValidationGroup="deskValidation" 
                            ErrorMessage="La data di scadenza deve essere maggiore della data odierna" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td class="col-dsw-1 DeskLabel" style="vertical-align: middle!important;">
                        <asp:Label ID="lblDeskContainer" runat="server" Text="Contenitore:"></asp:Label>
                    </td>
                    <td class="col-dsw-9">
                        <telerik:RadDropDownList runat="server" ID="ddlContainers" Width="300px" AutoPostBack="False" />
                        <asp:RequiredFieldValidator ValidationGroup="deskValidation" runat="server" ID="deskContainerValidator" Display="Dynamic" ControlToValidate="ddlContainers" ErrorMessage="Il campo contenitore è obbligatorio"/>
                    </td>
                </tr>    
            </table>
        </asp:Panel>
        
        <br/>
            <table cellspacing="2">
                <tr>
                    <td class="col-dsw-5 dsw-text-nowrap" style="vertical-align: top;">
                        <!-- Invita Utenti -->
                        <usc:InvitationUser ID="uscInvitationUser" ButtonUserDeleteEnabled="true" Type="Modify" runat="server" BindAsyncEnable="true" />
                    </td>
                    <td class="col-dsw-5 dsw-text-nowrap" style="vertical-align: top;">
                         <!--Documenti del Tavolo -->
                        <usc:DeskDocument ID="uscDeskDocument" MultipleDocuments="True" ButtonDeleteEnable="True" Type="Insert" runat="server" BindAsyncEnable="true"/>
                    </td>
                </tr>
            </table>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlActionButtons">
        <telerik:RadButton runat="server" ValidationGroup="deskValidation" ID="btnSave" Width="150" Text="Salva"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
