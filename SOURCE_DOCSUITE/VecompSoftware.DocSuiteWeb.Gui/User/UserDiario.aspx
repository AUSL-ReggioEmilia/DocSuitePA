<%@ Page AutoEventWireup="false" Codebehind="UserDiario.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserDiario" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Scrivania - Diario" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <div class="titolo" id="divTitolo" runat="server" visible="true">
        &nbsp;
        <asp:Label ID="lblHeader" runat="server" />
        <br class="Spazio" />
    </div>
    <table class="dataform">
        <tr>
            <td class="label" width="20%">
                Data Operazione:
            </td>
            <td style="text-align: left; vertical-align: middle; width: 80%;">
                <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Data Operazione Da obbligatoria" ID="rfvDateFrom" runat="server" />
                <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Data Operazione A obbligatoria" ID="rfvDateTo" runat="server" />
            </td>
        </tr>
    </table>
    <asp:Button ID="btnCerca" runat="server" Text="Cerca" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent" style="overflow:hidden;width:100%;height:100%;">
    <%-- SEZIONE PROTOCOLLO --%>
    <asp:Panel ID="pnlProtGrid" runat="server" style="overflow:hidden;width:100%;height:100%;">        
            <DocSuite:BaseGrid AllowFilteringByColumn="False" AllowMultiRowSelection="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProtocol" GridLines="None" ID="gvProtocols" PageSize="10000" runat="server" Width="100%">
                <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="False" />
                <ExportSettings FileName="Esportazione">
                    <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
                    <Excel Format="ExcelML" />
                </ExportSettings>
                <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" CommandItemDisplay="Top" DataSourceID="odsProtocol" NoMasterRecordsText="Nessun Protocollo trovato" Width="100%">
                    <RowIndicatorColumn Visible="False">
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn Resizable="False" Visible="False">
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderImageUrl="../Comm/Images/Selezione16.gif" HeaderText="Selezione" UniqueName="TemplateColumn">
                            <headerstyle horizontalalign="Center" width="2%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <telerik:RadButton CausesValidation="False" Height="16px" ID="imgSelection" Image-ImageUrl="../Comm/Images/Selezione16.gif" Runat="server" Width="16px" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Protocollo" SortExpression="Codice" UniqueName="ProtocolNumber">
                            <headerstyle horizontalalign="Center" width="100px" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:LinkButton id="lbProtocollo" Runat="server"/>
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Oggetto" SortExpression="Object" UniqueName="Object">
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
                                <asp:Image ID="imgInserimento" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Sommario" UniqueName="cSommario">
                            <headerstyle horizontalalign="Center" width="8%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgSommario" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Documento" UniqueName="cDocumento">
                            <headerstyle horizontalalign="Center" width="8%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgDocumento" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Autorizz." UniqueName="cAutoriz">
                            <headerstyle horizontalalign="Center" width="8%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgAutoriz" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Modifica" UniqueName="cModifica">
                            <headerstyle horizontalalign="Center" width="8%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgModifica" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Presa in carico" UniqueName="cHandled">
                            <headerstyle horizontalalign="Center" width="8%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgHandled" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <EditFormSettings>
                        <PopUpSettings ScrollBars="None" />
                    </EditFormSettings>
                    <PagerStyle Position="Top" Visible="False" />
                </MasterTableView>
                <ClientSettings AllowDragToGroup="True">
                </ClientSettings>
                <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine Decrescente" SortToolTip="Ordina" />
            </DocSuite:BaseGrid>
            <asp:ObjectDataSource ID="odsProtocol" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetUserProtocolDiary" TypeName="VecompSoftware.DocSuiteWeb.Facade.JournalFacade">
                <SelectParameters>
                    <asp:Parameter Name="pDateFrom" Type="DateTime" />
                    <asp:Parameter Name="pDateTo" Type="DateTime" />
                    <asp:Parameter Name="currentTenantAOOId" Type="Object" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
    </asp:Panel>
   
    <%-- SEZIONE PRATICHE--%>
    <asp:Panel ID="pnlDocmGrid" runat="server" style="overflow:hidden;width:100%;height:100%;">
        
            <DocSuite:BaseGrid AllowFilteringByColumn="False" AllowMultiRowSelection="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsDocument" GridLines="None" ID="gvDocuments" PageSize="10000" runat="server">
                <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom"
                    ShowPagerText="False" />
                <ExportSettings>
                    <Pdf FontType="Subset" PaperSize="Letter" />
                    <Excel Format="Html" />
                    <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" />
                </ExportSettings>
                <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" DataSourceID="odsDocument" Dir="LTR" Frame="Border" NoMasterRecordsText="Nessuna Pratica trovata" TableLayout="Auto">
                    <RowIndicatorColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                        Visible="False">
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                        Resizable="False" Visible="False">
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="false" HeaderImageUrl="../Comm/Images/Selezione16.gif" HeaderText="Selezione">
                            <headerstyle horizontalalign="Center" width="2%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <telerik:RadButton Height="16px" ID="imgSelection" Image-ImageUrl="../Comm/Images/Selezione16.gif" Runat="server" Width="16px" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Pratica" SortExpression="Year" UniqueName="cDocumentNumber">
                            <itemtemplate>
                                <asp:LinkButton id="lblPratica" Runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="false" HeaderText="Oggetto" SortExpression="Object" UniqueName="ProtocolObject">
                            <itemtemplate>
                                <asp:Label ID="lblObject" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="False" Groupable="false" HeaderText="Data" SortExpression="LogDate" UniqueName="MaxLogDate">
                            <itemtemplate>
                                <asp:Label ID="lblLogDate" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Inserimento" UniqueName="cLogPI">
                            <headerstyle horizontalalign="Center" width="2%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgInserimento" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Sommario" UniqueName="cSommario">
                            <headerstyle horizontalalign="Center" width="2%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgSummary" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Documento" UniqueName="cDocumento">
                            <headerstyle horizontalalign="Center" width="2%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgDocument" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Autorizz." UniqueName="cAutoriz">
                            <headerstyle horizontalalign="Center" width="2%" />
                            <itemstyle horizontalalign="Center" />
                            <itemtemplate>
                                <asp:Image ID="imgAuth" runat="server" />
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <EditFormSettings>
                        <EditColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType">
                        </EditColumn>
                    </EditFormSettings>
                </MasterTableView>
            </DocSuite:BaseGrid>
            <asp:ObjectDataSource ID="odsDocument" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetUserDocumentDiary" TypeName="VecompSoftware.DocSuiteWeb.Facade.JournalFacade">
                <SelectParameters>
                    <asp:Parameter Name="pDateFrom" Type="DateTime" />
                    <asp:Parameter Name="pDateTo" Type="DateTime" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
    </asp:Panel>
    
    <%-- SEZIONE ATTI--%>
    <asp:Panel ID="pnlReslGrid" runat="server" style="overflow:hidden;width:100%;height:100%;">
        
        <DocSuite:BaseGrid AllowFilteringByColumn="False" AllowMultiRowSelection="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsResolution" GridLines="None" ID="gvResolutions" PageSize="10000" runat="server">
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="False" />
            <ExportSettings>
                <Pdf FontType="Subset" PaperSize="Letter" />
                <Excel Format="Html" />
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" />
            </ExportSettings>
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" DataSourceID="odsResolution" Dir="LTR" Frame="Border" NoMasterRecordsText="Nessun Atto trovato" TableLayout="Auto">
                <RowIndicatorColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType" Visible="False">
                    <HeaderStyle Width="20px" />
                </RowIndicatorColumn>
                <ExpandCollapseColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType" Resizable="False" Visible="False">
                    <HeaderStyle Width="20px" />
                </ExpandCollapseColumn>
                <Columns>
                    <telerik:GridTemplateColumn Groupable="false" HeaderImageUrl="../Comm/Images/Selezione16.gif" HeaderText="Selezione">
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                        <itemtemplate>
                            <telerik:RadButton Height="16px" ID="imgSelection" Image-ImageUrl="../Comm/Images/Selezione16.gif" Runat="server" Width="16px" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="False" HeaderText="Documento" SortExpression="Year" UniqueName="cResolutionNumber">
                        <itemtemplate>
                            <asp:LinkButton ID="cmdResolution" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="false" HeaderText="Oggetto" SortExpression="Object" UniqueName="ProtocolObject">
                        <itemtemplate>
                            <asp:Label ID="lblObject" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="false" HeaderText="Data" SortExpression="LogDate" UniqueName="MaxLogDate">
                        <itemtemplate>
                            <asp:Label ID="lblLogDate" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="false" HeaderText="Inserimento" UniqueName="cLogPI">
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                        <itemtemplate>
                            <asp:Image ID="imgInserimento" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="false" HeaderText="Sommario" UniqueName="cSommario">
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                        <itemtemplate>
                            <asp:Image ID="imgSummary" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="false" HeaderText="Documento" UniqueName="cDocumento">
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                        <itemtemplate>
                            <asp:Image ID="imgDocument" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn Groupable="false" HeaderText="Autorizz." UniqueName="cAutoriz">
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                        <itemtemplate>
                            <asp:Image ID="imgAuth" runat="server" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings>
                    <EditColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType" />
                </EditFormSettings>
            </MasterTableView>
        </DocSuite:BaseGrid>
        <asp:ObjectDataSource ID="odsResolution" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetUserResolutionDiary" TypeName="VecompSoftware.DocSuiteWeb.Facade.JournalFacade">
            <SelectParameters>
                <asp:Parameter Name="pDateFrom" Type="DateTime" />
                <asp:Parameter Name="pDateTo" Type="DateTime" />
            </SelectParameters>
        </asp:ObjectDataSource>
        
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    
</asp:Content>
