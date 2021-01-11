<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECMailBoxLog" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="datatable">
        <tr>
            <th colspan="3"><asp:Label ID="lblMailBox" runat="server" EnableViewState="false" Text="Casella PEC" /></th>
        </tr>
        <tr>
            <td class="label" style="width: 200px;">Casella di posta:</td>
            <td colspan="2">
                <asp:DropDownList AutoPostBack="true" DataTextField="MailBoxName" DataValueField="Id" ID="ddlMailBox" runat="server" Width="300" />
            </td>
        </tr>
        <tr>
            <td class="label">Tipologia:</td>
            <td>
                <asp:DropDownList ID="ddlType" runat="server">
                    <asp:ListItem Text="" Selected="True" />
                    <asp:ListItem Text="Ingresso" Value="Incoming" />
                    <asp:ListItem Text="Importata" Value="Imported" />
                    <asp:ListItem Text="Avvertenza" Value="Warn" />
                    <asp:ListItem Text="Rimossa dal server" Value="ServerRemoved" />
                    <asp:ListItem Text="Uscita" Value="Sent" />
                    <asp:ListItem Text="Uscita con errori" Value="SentError" />
                </asp:DropDownList>                                   
            </td>
            <td style="vertical-align: middle;">
                <telerik:RadDatePicker ID="dtpShowSentFrom" DateInput-LabelWidth="40%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Dalla data:" runat="server" />
                <telerik:RadDatePicker ID="dtpShowSentTo" DateInput-LabelWidth="40%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Alla data:" runat="server" />
            </td>
        </tr>
    </table>
    <asp:Button ID="cmdRefreshGrid" runat="server" Width="200px" Text="Aggiorna visualizzazione" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="true" AutoGenerateColumns="False" GridLines="Both" ID="dgMail" PageSize="20" runat="server">
            <AlternatingItemStyle BackColor="Silver" />
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" Dir="LTR" TableLayout="Auto">
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="MailBoxName" HeaderText="MailBox" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label ID="lblMailboxName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="Type" DataField="Type" HeaderText="Tipo" AllowFiltering="false" />                                           
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Description" HeaderText="Descrizione" SortExpression="Description" UniqueName="colDescription">
                        <HeaderStyle HorizontalAlign="Left" Width="40%" />
                        <ItemStyle HorizontalAlign="Left" Width="40%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="Date" DataType="System.DateTime" HeaderText="Inserito il" SortExpression="MailDate" UniqueName="colDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15%" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15%" />
                    </telerik:GridDateTimeColumn>  
                    <telerik:GridBoundColumn UniqueName="SystemComputer" DataField="SystemComputer" HeaderText="Computer" AllowFiltering="false" /> 
                    <telerik:GridBoundColumn UniqueName="SystemUser" DataField="SystemUser" HeaderText="Utente" AllowFiltering="false">  
                       <HeaderStyle HorizontalAlign="Left" Width="25%" />
                       <ItemStyle HorizontalAlign="Left" Width="25%" />                                           
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>
