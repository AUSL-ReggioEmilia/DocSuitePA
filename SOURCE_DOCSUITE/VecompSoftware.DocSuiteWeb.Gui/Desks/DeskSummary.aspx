<%@ Page Title="Tavoli - Sommario" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskSummary.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskSummary" %>

<%@ Register Src="~/UserControl/uscDeskDocument.ascx" TagPrefix="usc" TagName="DeskDocument" %>
<%@ Register Src="~/UserControl/uscInvitationUser.ascx" TagPrefix="usc" TagName="InvitationUser" %>
<%@ Register Src="~/UserControl/uscDeskStoryBoard.ascx" TagPrefix="usc" TagName="DeskStoryBoard" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <style type="text/css">
            .rgDataDiv {
                height: auto !important;
            }
        </style>
        <script type="text/javascript">
            function pageLoad(sender, eventArgs) {
            }

        </script>

    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel Style="width: 100%" runat="server" ID="deskContainer">
        <!-- Oggetto del Tavolo -->
        <asp:Panel runat="server" ID="pnlDeskSubject">
            <table class="datatable">
                <tr>
                    <th colspan="2">Oggetto/Descrizione</th>
                </tr>
                <tr>
                    <td style="width: 100%">
                        <asp:Label runat="server" ID="lblDeskObject"></asp:Label>
                        <telerik:RadTextBox runat="server" ID="txtObject" Width="100%" MaxLength="500" InputType="Text" TextMode="MultiLine" Rows="4" EmptyMessage="Oggetto"></telerik:RadTextBox>
                        <asp:RequiredFieldValidator runat="server" ValidationGroup="deskValidation" ID="deskObjectValidator" Display="Dynamic" ControlToValidate="txtObject" ErrorMessage="Il campo Oggetto/Descrizione è obbligatorio"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Informazioni del Tavolo -->
        <asp:Panel runat="server" ID="pnlInformations">
            <table class="datatable" cellspacing="0" cellpadding="2"  style="border: 0.5px ;">
                <tr>
                    <th colspan="2">Informazioni
                        <telerik:RadButton ID="btnCollapse" runat="server" CssClass="arrow-down" Width="16px" Height="16px" Visible="true" AutoPostBack="true">
                            <Image EnableImageButton="true" />
                        </telerik:RadButton>
                    </th>
                </tr>
                <asp:Panel ID="pnlContents" runat="server">
                    <tr class="Chiaro">
                        <td class="col-dsw-5">
                            <table cellspacing="0" cellpadding="2" class="col-dsw-10" style="border: 0;">
                                <asp:Panel runat="server" ID="pnlDeskInformationRestricted">
                                    <tr>
                                        <td class="col-dsw-2 DeskLabel" style="vertical-align: middle!important;">
                                            <label>Stato e Data scadenza:</label>
                                        </td>
                                        <td class="col-dsw-9">
                                            <asp:Label runat="server" ID="lblDeskStatusRestricted" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="col-dsw-2 DeskLabel" style="vertical-align: middle!important;">
                                            <label>Creato da:</label>
                                        </td>
                                        <td class="col-dsw-9">
                                            <asp:Label runat="server" ID="lblRegistrationUser" />
                                        </td>
                                    </tr>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="pnlDeskInformationExtended">
                                    <tr>
                                        <td class="col-dsw-2 DeskLabel" style="vertical-align: middle!important;">
                                            <label>Stato e Data scadenza:</label>
                                        </td>
                                        <td class="col-dsw-9">
                                            <asp:Label runat="server" ID="lblDeskStatusExteded"></asp:Label>
                                            <telerik:RadDatePicker runat="server" ID="dtpDeskExpired"></telerik:RadDatePicker>
                                            <asp:RequiredFieldValidator runat="server" ValidationGroup="deskValidation" ID="deskDateExpiredValidator" Display="Dynamic" ControlToValidate="dtpDeskExpired" ErrorMessage="Il campo Data scadenza è obbligatorio"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="col-dsw-2 DeskLabel" style="vertical-align: middle!important;">
                                            <label>Creato da:</label>
                                        </td>
                                        <td class="col-dsw-9">
                                            <asp:Label runat="server" ID="lblRegistrationUser2" />
                                        </td>
                                    </tr>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="pnlContainers">
                                    <tr>
                                        <td style="vertical-align: middle;">
                                            <label class="DeskLabel">Contenitore:</label>
                                        </td>
                                        <td style="width: 100%;">
                                            <asp:Label ID="lblDeskContainer" runat="server" />
                                        </td>
                                    </tr>
                                </asp:Panel>
                            </table>
                        </td>
                        <td class="col-dsw-5">
                            <table cellspacing="0" cellpadding="2" style="border: 0; width: 100%">
                                <asp:Panel runat="server" ID="pnlDeskCollaborationLink">
                                    <tr>
                                        <td style="float: left;">
                                            <asp:Label ID="lblCollaborationTitle" CssClass="DeskLabel" runat="server" Text="Collaborazione:" />
                                        </td>
                                        <td class="col-dsw-10">
                                            <asp:Repeater ID="links" runat="server">
                                                <ItemTemplate>
                                                    <asp:HyperLink runat="server" ID="collaborationLink" Text='<%#Eval("Key")%>' NavigateUrl='<%#Eval("Value")%>'></asp:HyperLink> <br />
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </asp:Panel>
                            </table>
                        </td>
                    </tr>
                </asp:Panel>

            </table>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlDeskData">
            <table cellspacing="2">
                <tr>
                    <td style="vertical-align: top; width: 35%; white-space: nowrap;">
                        <!-- Invita Utenti -->
                        <usc:InvitationUser ID="uscInvitationUser" Type="Modify" IsReadOnly="True" runat="server" BindAsyncEnable="False" />
                    </td>
                    <td style="vertical-align: top; width: 65%; white-space: nowrap;">
                        <!--Documenti del Tavolo -->
                        <usc:DeskDocument ID="uscDeskDocument" MultipleDocuments="True" HideScannerMultipleDocumentButton="true" Type="Modify" IsReadOnly="True" runat="server" BindAsyncEnable="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <asp:Panel runat="server" ID="pnlStoryBoard">
            <usc:DeskStoryBoard runat="server" ID="uscDeskStoryBoard" GenericStoryBoard="True"></usc:DeskStoryBoard>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlActionButtons">
        <telerik:RadButton runat="server" ValidationGroup="deskValidation" ID="btnSave" Width="150" Text="Salva"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnViewDocument" Width="150" Text="Documenti"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnCancel" Width="150" Text="Annulla"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnModify" Width="150" Text="Modifica"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnCloseDesk" Width="150" Text="Chiudi tavolo"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnOpenDesk" Width="150" Text="Apri tavolo"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnApprove" Width="150" Text="Richiedi approvazione"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnApproveManager" Width="150" Text="Controlla approvazioni"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnManage" Width="150" Text="Gestisci"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
