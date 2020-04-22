<%@ Page AutoEventWireup="false" CodeBehind="DocmTokenRichiamo.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmTokenRichiamo" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Richiamo delle Richieste di Presa in carico" %>

<%@ Register Src="~/UserControl/uscDocumentToken.ascx" TagName="UscDocumentToken" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="False" AllowPaging="True" AutoGenerateColumns="False" ID="gvToken" PageSize="3" runat="server">
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="false" />
            <MasterTableView AllowCustomPaging="True" CurrentResetPageIndexAction="SetPageIndexToFirst" DataKeyNames="Id" GridLines="Horizontal" NoMasterRecordsText="Nulla" TableLayout="Auto">
                <Columns>
                    <telerik:GridTemplateColumn HeaderImageUrl="Images/Info16.gif" HeaderText="T" AllowFiltering="false">
                        <HeaderStyle HorizontalAlign="Center" Width="3%"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgInfo" ImageUrl='<%# if(Eval("Response")="N","../Comm/Images/Remove16.gif","../Comm/Images/None16.gif")%>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="FullStep" HeaderText="Step" UniqueName="FullStep" CurrentFilterFunction="Contains" SortExpression="FullStep" AllowFiltering="false">
                        <HeaderStyle HorizontalAlign="Center" Width="3%"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DocumentTabToken.Id" HeaderText="Tip." UniqueName="DocumentTabToken.Id" CurrentFilterFunction="Contains" SortExpression="DocumentTabToken.Id" AllowFiltering="false">
                        <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SourceDestinationRoleDescription" HeaderText="Mitt./Dest." CurrentFilterFunction="Contains" UniqueName="SourceDestinationRoleDescription" SortExpression="SourceDestinationRoleDescription">
                        <HeaderStyle HorizontalAlign="Left" Width="10%"></HeaderStyle>
                        <ItemStyle Wrap="False" HorizontalAlign="Left"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="OperationExpiryDateDescription" HeaderText="Oper./Scad." CurrentFilterFunction="Contains" UniqueName="OperationExpiryDateDescription" SortExpression="OperationExpiryDateDescription">
                        <HeaderStyle HorizontalAlign="Center" Width="12%"></HeaderStyle>
                        <ItemStyle Wrap="False" HorizontalAlign="Center"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DocObject" HeaderText="Oggetto" UniqueName="DocObject" CurrentFilterFunction="Contains" SortExpression="DocObject"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Reason" HeaderText="Motivo" UniqueName="Reason" SortExpression="Reason"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Note" HeaderText="Note" UniqueName="Note" CurrentFilterFunction="Contains" SortExpression="Note"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="RegistrationUserDateDescription" HeaderText="Inserimento" CurrentFilterFunction="Contains" UniqueName="RegistrationUserDateDescription" SortExpression="RegistrationUserDateDescription">
                        <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                    </telerik:GridBoundColumn>
                </Columns>
                <ExpandCollapseColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType" Resizable="False" Visible="False">
                    <HeaderStyle Width="20px" />
                </ExpandCollapseColumn>
            </MasterTableView>
            <ClientSettings AllowGroupExpandCollapse="True" AllowDragToGroup="True">
                <Selecting AllowRowSelect="True"></Selecting>
            </ClientSettings>
            <ExportSettings>
                <Pdf FontType="Subset" PaperSize="Letter" />
                <Excel Format="Html" />
            </ExportSettings>
        </DocSuite:BindGrid>
    </div>

    <uc:UscDocumentToken runat="server" ID="uscDocumentToken" PanelTipologiaRichiestaVisible="false" PanelDatiRichiestaVisible="false" />

    <asp:Panel ID="pnlRichiamo" Visible="true" runat="server">
        <table class="datatable">
            <tr>
                <th colspan="2">Motivo del Richiamo</th>
            </tr>
            <tr>
                <td class="label">Motivo:</td>
                <td width="80%">
                    <telerik:RadTextBox ID="txtReasonResponse" runat="server" Width="100%" MaxLength="255" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
</asp:Content>
