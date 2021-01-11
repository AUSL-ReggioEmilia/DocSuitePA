<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslJournal.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslJournal" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="dataform">
        <%-- Anno --%>
        <tr>
            <td class="label" style="width: 25%;">Anno:
            </td>
            <td>
                <asp:DropDownList AutoPostBack="true" ID="Years" runat="server" />
            </td>
        </tr>
        <%-- Template --%>
        <tr runat="server" id="TemplatesRow">
            <td class="label" style="width: 25%;">Registro:
            </td>
            <td>
                <asp:DropDownList AutoPostBack="true" ID="Templates" runat="server" />
            </td>
        </tr>
    </table>
    <table style="width: 100%;">
        <tr id="trGridHeader" runat="server">
            <td>
                <div class="titolo" id="divTitolo">
                    <asp:Label ID="lblHeader" runat="server" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowCustomPaging="true" AllowFilteringByColumn="true" AllowMultiRowSelection="true" AllowSorting="True" AutoGenerateColumns="False" GridLines="Both" ID="Journals" runat="server" ShowGroupPanel="True" Visible="true">
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="false" />
            <MasterTableView DataKeyNames="Id" AllowMultiColumnSorting="True" NoMasterRecordsText="Nessun Registro Trovato" TableLayout="Auto">
                <Columns>
                    <telerik:GridTemplateColumn AllowFiltering="false" DataField="ResolutionJournal" Groupable="false" UniqueName="colSelect">
                        <HeaderStyle HorizontalAlign="Center" Width="25px" />
                        <ItemStyle HorizontalAlign="Center" Width="25px" />
                        <ItemTemplate>
                            <asp:CheckBox AutoPostBack="false" CommandName="Selected" ID="chkSelect" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Prot/Images/Mail16.gif" HeaderText="Tipo" UniqueName="cType">
                        <HeaderStyle HorizontalAlign="Center" Width="25px" />
                        <ItemStyle HorizontalAlign="Center" Width="25px" />
                        <ItemTemplate>
                            <telerik:RadButton Width="16px" Height="16px" ID="imgType" ButtonType="LinkButton" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Registro" UniqueName="Description">
                        <HeaderStyle HorizontalAlign="Left" Width="300px" />
                        <ItemStyle HorizontalAlign="Left" Width="300px" />
                        <ItemTemplate>
                            <asp:LinkButton ID="Description" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn AllowFiltering="false" DataField="Year" HeaderStyle-Width="55px" HeaderText="Anno" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="55px" UniqueName="Year" />
                    <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Mese" UniqueName="MonthDesc">
                        <HeaderStyle HorizontalAlign="Left" Width="100px" />
                        <ItemStyle HorizontalAlign="Left" Width="100px" />
                        <ItemTemplate>
                            <asp:Label ID="Month" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn AllowFiltering="false" AllowSorting="false" DataField="Template.Description" HeaderText="Registro" UniqueName="Template" />
                    <telerik:GridBoundColumn AllowFiltering="false" DataField="FirstPage" HeaderStyle-Width="60px" HeaderText="Prima pag." ItemStyle-Width="60px" UniqueName="FirstPage" />
                    <telerik:GridBoundColumn AllowFiltering="false" DataField="LastPage" HeaderStyle-Width="60px" HeaderText="Ultima pag." ItemStyle-Width="60px" UniqueName="LastPage" />
                    <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Id Pubbl." UniqueName="publicationIds" Visible="false" Display="false">
                        <HeaderStyle HorizontalAlign="Left" Width="80px" />
                        <ItemStyle HorizontalAlign="Left" Width="80px" />
                        <ItemTemplate>
                            <asp:Label ID="publicationId" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn AllowFiltering="false" DataField="SignDate" HeaderText="Data ultima firma" UniqueName="SignDate" />
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
            <ClientSettings AllowDragToGroup="True" AllowGroupExpandCollapse="True">
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlFooter">
        <asp:Button ID="Nuovo" runat="server" Text="Nuovo Registro" />
        <asp:Button ID="btnMultiSign" runat="server" Text="Firma Selezionati" />
    </asp:Panel>
</asp:Content>
