<%@ Page AutoEventWireup="false" CodeBehind="ProtConcorsiViewGrid.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Prot.ProtConcorsiViewGrid" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
            <div class="titolo" id="divTitolo" runat="server" visible="true">
                <asp:Label ID="lblHeader" runat="server" />
            </div>
        </telerik:RadAjaxPanel>
        <DocSuite:BindGrid AllowMultiRowSelection="False" AutoGenerateColumns="False"
            GridLines="Both" ID="grdConcourse" runat="server">
            <MasterTableView AllowFilteringByColumn="True" GridLines="Both" NoMasterRecordsText="Nessun Protocollo Trovato" TableLayout="Auto">
                <Columns>
                    <DocSuite:YearNumberBoundColumn UniqueName="Id" HeaderText="Protocollo" CurrentFilterFunction="EqualTo" Groupable="false" SortExpression="Id">
                        <HeaderStyle HorizontalAlign="Center" Width="98px" />
                        <ItemStyle HorizontalAlign="center" Width="98px" />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbtViewProtocol" CommandName="ViewProtocol"></asp:LinkButton>
                        </ItemTemplate>
                    </DocSuite:YearNumberBoundColumn>
                   <%-- <telerik:GridDateTimeColumn UniqueName="RegistrationDate" HeaderText="Data registrazione" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" SortExpression="RegistrationDate">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="125px" />
                        <ItemStyle HorizontalAlign="Center" Width="125px" />
                    </telerik:GridDateTimeColumn>--%>

                    <telerik:GridTemplateColumn UniqueName="colProtocolContactLastName" HeaderText="Cognome" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblProtocolContactLastName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="colProtocolContactFirstName" HeaderText="Nome" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblProtocolContactFirstName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Data nascita" UniqueName="cMittDestDataNascita" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMittDestDataNascita" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Indirizzo" UniqueName="cMittDestIndirizzo" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMittDestIndirizzo" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Località" UniqueName="cMittDestLocalita" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMittDestLocalita" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Provincia" UniqueName="cMittDestProvincia" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMittDestProvincia" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="PEC" UniqueName="cMittDestPEC" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMittDestPEC" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Telefono" UniqueName="cMittDestTelefono" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMittDestTelefono" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="btnPanel">
        <asp:Repeater runat="server" ID="ReportRepeater">
            <ItemTemplate>
                <asp:Button runat="server" Width="120" OnClick="BtnRicevutaClick" ID="btnReport" />
            </ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="ReportButtons"></asp:PlaceHolder>
        <asp:CheckBox runat="server" ID="ReportCurrentPage" Text="Solo pagina corrente" Checked="True" Visible="False" />
        <telerik:RadButton ID="btnExcel" runat="server" Text="Esporta" Visible="false" />
    </asp:Panel>
</asp:Content>
