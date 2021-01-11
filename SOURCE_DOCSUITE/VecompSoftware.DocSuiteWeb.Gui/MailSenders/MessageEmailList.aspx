<%@ Page Title="Elenco Messaggi" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="MessageEmailList.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MessageEmailList" %>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <asp:Panel runat="server" DefaultButton="cmdRefreshgrid" ID="pnlFilters">
        <table class="datatable">
            <tr>
                <td class="label">Spedito:</td>
                <td>
                    <span class="miniLabel">Da</span>
                    <telerik:RadDatePicker ID="dtpSentFrom" runat="server" />
                    <span class="miniLabel">A</span>
                    <telerik:RadDatePicker ID="dtpSentTo" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">Stato:</td>
                <td>
                    <telerik:RadButton ButtonType="ToggleButton" ID="chkUnsent" runat="server" Text="Includi non spedite" ToggleType="CheckBox" Checked="True" />
                </td>
            </tr>
            <tr>
                <td class="label">Email contatti:</td>
                <td>
                    <span class="miniLabel">Da</span>
                    <asp:TextBox runat="server" ID="txtSender" Width="250px" Enabled="False" />
                    <span class="miniLabel">A</span>
                    <asp:TextBox runat="server" ID="txtRecipient" Width="250px" />
                </td>
            </tr>
        </table>
        <asp:Button ID="cmdRefreshGrid" runat="server" Width="200px" Text="Aggiorna visualizzazione" />
        <asp:Button ID="cmdClearFilters" runat="server" Text="Azzera filtri" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="radGridWrapper">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="True" AutoGenerateColumns="False" GridLines="Both" ID="dgMessageEmail" 
        PageSize="20" runat="server" ShowGroupPanel="True">
                                
        <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" DataKeyNames="Id" Dir="LTR" TableLayout="Auto" Width="100%">
            <ItemStyle CssClass="Scuro" />
            <AlternatingItemStyle CssClass="Chiaro" />
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Log" UniqueName="Log">
                    <HeaderStyle HorizontalAlign="Center" Width="10px" />
                    <ItemStyle HorizontalAlign="Center" Width="10px" />
                    <ItemTemplate>
                        <telerik:RadButton runat="server" ButtonType="LinkButton" Width="16px" Height="16px" CommandName="Log" Image-ImageUrl="<%# ImagePath.SmallLog%>" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- ImgPriority --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/Mails/highimportance.gif" HeaderText="Priorità" UniqueName="cPriority">
                    <HeaderStyle HorizontalAlign="Center" Width="1%" />
                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                    <ItemTemplate>
                        <asp:Image ID="imgPriority" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Mittente--%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Da" UniqueName="colSenders">
                    <HeaderStyle HorizontalAlign="Left" Width="20%" />
                    <ItemStyle HorizontalAlign="Left" Width="20%" />
                    <ItemTemplate>
                        <asp:Label ID="lblSenders" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Oggetto--%>
                <telerik:GridTemplateColumn Groupable="False" CurrentFilterFunction="Contains" DataField="Subject" HeaderText="Oggetto" SortExpression="Subject" UniqueName="Subject">
                    <HeaderStyle HorizontalAlign="Left" Width="35%" />
                    <ItemStyle HorizontalAlign="Left" Width="35%" />
                    <ItemTemplate>
                        <asp:Label ID="lblSubject" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Destinatari--%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="A" UniqueName="colRecipients">
                    <HeaderStyle HorizontalAlign="Left" Width="20%" />
                    <ItemStyle HorizontalAlign="Left" Width="20%" />
                    <ItemTemplate>
                        <asp:Label ID="lblRecipients" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Destinatari CC--%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="CC" UniqueName="colRecipientsCc">
                    <HeaderStyle HorizontalAlign="Left" Width="15%" />
                    <ItemStyle HorizontalAlign="Left" Width="15%" />
                    <ItemTemplate>
                        <asp:Label ID="lblRecipientsCc" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Data spedizione--%>
                <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="SentDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss tt}" DataType="System.DateTime" HeaderText="Spedito il" SortExpression="SentDate" UniqueName="SentDate">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                </telerik:GridDateTimeColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True" CellSelectionMode="None" UseClientSelectColumnOnly="True" EnableDragToSelectRows="False" />
        </ClientSettings>
        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
    </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
</asp:Content>