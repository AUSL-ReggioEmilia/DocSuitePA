<%@ Page Title="Tavoli - Lavagna" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskViewStoryBoard.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskViewStoryBoard" %>

<%@ Register Src="~/UserControl/uscDeskStoryBoard.ascx" TagPrefix="usc" TagName="DeskStoryBoard" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <style type="text/css">
            .rgDataDiv {
                height: auto !important;
            }
        </style>

        <script type="text/javascript">

        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table class="datatable">
        <tr>
            <th>Documento -
                <asp:Label runat="server" ID="lblDocumentVersion"></asp:Label></th>
        </tr>
        <tr>
            <td>
                <div>
                    <asp:Image runat="server" ID="imgDocumentType" />
                    <asp:Label runat="server" ID="lblDocumentName" />
                </div>
            </td>
        </tr>
    </table>

    <asp:Panel runat="server" ID="pnlStoryBoard">
        <usc:DeskStoryBoard runat="server" ID="uscDeskStoryBoard" GenericStoryBoard="False"></usc:DeskStoryBoard>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <telerik:RadButton runat="server" ID="btnViewDocument" Width="150" Text="Visualizza Documento"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnAddComment" Width="150" Text="Commenta"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnAddUser" Width="150" Text="Invia Email Partecipante"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnApprove" Width="150" Text="Approva"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnRefuse" Width="150" Text="Rifiuta"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
