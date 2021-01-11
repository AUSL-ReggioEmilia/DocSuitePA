<%@ Page AutoEventWireup="false" Codebehind="UserDiarioComune.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserDiarioComune" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Scrivania - Diario Comune" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <div class="titolo" id="divTitolo" runat="server">
        &nbsp;<asp:Label ID="lblHeader" runat="server" />
        <br class="Spazio" />
    </div>
    <table class="dataform">
        <tr>
            <td class="label" width="20%">
                Data Operazione:
            </td>
            <td align="left" width="80%">
                <span>da</span>
                <telerik:RadDatePicker ID="rdpDateFrom" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Data Operazione Da obbligatoria" ID="rfvDateFrom" runat="server" />
                <span>a</span>
                <telerik:RadDatePicker ID="rdpDateTo" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Data Operazione A obbligatoria" ID="rfvDateTo" runat="server" />
            </td>
        </tr>
    </table>
    <asp:Button ID="btnCerca" runat="server" Text="Cerca" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <DocSuite:BaseGrid AllowFilteringByColumn="False" AllowMultiRowSelection="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" GridLines="None" ID="gvDiario" PageSize="10000" runat="server" Width="100%">
        <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="False" />
        <ExportSettings FileName="Esportazione">
            <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
            <Excel Format="ExcelML" />
        </ExportSettings>
        <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" CommandItemDisplay="Top"  NoMasterRecordsText="Nessun risultato trovato" Width="100%">
            <RowIndicatorColumn Visible="False">
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn Resizable="False" Visible="False">
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Tipo" UniqueName="TemplateColumn">
                    <headerstyle horizontalalign="Center" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:Image ID="imgType" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Log" UniqueName="TemplateColumn1">
                    <headerstyle horizontalalign="Center" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <telerik:RadButton Height="16px" ID="lnkLog" Runat="server" ToolTip="Visualizza Log" Width="16px">
                            <Image ImageUrl="../Comm/Images/Selezione16.gif" />
                        </telerik:RadButton>
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Codice" SortExpression="Codice" UniqueName="cDocumentNumber">
                    <headerstyle horizontalalign="Center" width="100px" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:LinkButton ID="lnkReference" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Oggetto" SortExpression="Object" UniqueName="ProtocolObject">
                    <headerstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:Label ID="lblObject" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Data" SortExpression="LogDate" UniqueName="MaxLogDate">
                    <headerstyle horizontalalign="Center" width="120px" />
                    <itemtemplate>
                        <asp:Label ID="lblLogDate" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Inserimento" UniqueName="cLogPI">
                    <headerstyle horizontalalign="Center" width="8%" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:Image ID="imgInsert" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Sommario" UniqueName="cSommario">
                    <headerstyle horizontalalign="Center" width="8%" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:Image  ID="imgSummary" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Documento" UniqueName="cDocumento">
                    <headerstyle horizontalalign="Center" width="8%" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:Image  ID="imgDocument" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <EditFormSettings>
                <PopUpSettings ScrollBars="None" />
            </EditFormSettings>
            <PagerStyle Position="Top" Visible="False" />
        </MasterTableView>
        <ClientSettings AllowDragToGroup="True" />
        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine Decrescente" SortToolTip="Ordina" />
    </DocSuite:BaseGrid>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    
</asp:Content>