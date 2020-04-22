<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECDestination.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECDestination" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    
    <table class="datatable">
        <tr class="Chiaro">
            <th>Note di destinazione</th>
        </tr>
        <tr class="Chiaro">
            <td>
                <telerik:RadTextBox runat="server" TextMode="MultiLine" Width="100%" ID="txtDestinationNotes"></telerik:RadTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <div style="text-align: right">
        <telerik:RadButton runat="server" ID="cmdCancel" ButtonType="LinkButton" SingleClick="True" Width="100" Text="Annulla"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="cmdSave" SingleClick="True" Width="100" Text="Conferma"></telerik:RadButton>
    </div>
</asp:Content>
