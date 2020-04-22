<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PECMoveMails.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECMoveMails" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/PEC/uscPECInfo.ascx" TagPrefix="usc" TagName="uscPECInfo" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <th>Selezionare la Casella PEC dove spostare i messaggi selezionati</th>
        </tr>
        <tr class="Chiaro">
            <td>
                <asp:DropDownList ID="ddlMailbox" DataValueField="Id" DataTextField="MailBoxName" AutoPostBack="false" runat="server" />
            </td>
        </tr>
    </table>
    <table class="datatable">
        <tr>
            <th>Motivazione dello spostamento</th>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox CausesValidation="True" ID="txtDescription" Rows="5" runat="server" TextMode="MultiLine" Width="100%" EmptyMessage="Inserisci qui la motivazione" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid runat="server" ID="PecGrid" Width="100%">
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="ID">
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Dettagli" UniqueName="deletable">
                    <ItemTemplate>
                        <usc:uscPECInfo ID="uscPECInfo" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>

        <ClientSettings EnableRowHoverStyle="False">
            <Selecting AllowRowSelect="False"></Selecting>
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnMove" runat="server" Text="Sposta" Enabled="true" Width="150" UseSubmitBehavior="false" CausesValidation="false" />
</asp:Content>
