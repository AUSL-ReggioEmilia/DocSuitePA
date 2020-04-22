<%@ Page AutoEventWireup="false" Codebehind="DocmDocumenti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmDocumenti" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
        <script type="text/javascript" language="javascript">
            	    
            function OpenWindow(url, name, parameters) {
                var manager = $find("<%= alertManager.ClientID %>");
                var wnd = manager.open(url + "?" + parameters, name);
                
                wnd.center();

                return false;
            }

             function OpenWindowFullScreen(url, name, parameters) {
                var manager = $find("<%= alertManager.ClientID %>");
                var wnd = manager.open(url + "?" + parameters, name);
                wnd.maximize()
                wnd.center();

                return false;
            }

            function CloseFileFunction(sender, args) {
                if (args.get_argument() !== null && args.get_argument().length !== 0) {
                    document.getElementById(args.get_argument()).click();
                } else {
                    Refresh();
                }
            }

            function CloseProtocolFunction(sender, args) {
                Refresh();

                if (args.get_argument() !== null) {
                    document.getElementById(args.get_argument()).click();
                }
            }

            function OpenWindowMailCC(name, param) {
                var url = "../Docm/DocmMailCC.aspx?";
                url += param;

                var manager = $find("<%= alertManager.ClientID %>");
                manager.open(url, name);

                return false;
            }

            function CloseMailFunction(sender, args) {
                if (args.get_argument() !== null) {
                    MailRefresh();
                }
            }

            function MailRefresh() {
                var wnd = $find("<%= windowDocmFile.ClientID %>");
                if (wnd != null)
                    wnd.close("<%= btnRefresh.ClientID %>");
                Refresh();
                parent.FolderRefresh();
            }

            function Refresh() {
                document.getElementById("<%= btnRefresh.ClientID %>").click();
            }

            function OpenFile(path) {
                var bdsView = new ActiveXObject("VecompWEB.ThinClient2003");
                bdsView.OpenLocalFile(path);
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="alertManager" runat="server" ReloadOnShow="True">
        <Windows>
            <telerik:RadWindow Height="550" ID="windowDocmFile" OnClientClose="CloseFileFunction" runat="server" Width="700" />
            <telerik:RadWindow Height="550" ID="windowDocmProt" OnClientClose="CloseProtocolFunction" runat="server" Width="700" />
            <telerik:RadWindow Height="550" ID="wndSearch" Modal="false" runat="server" Width="700" />
            <telerik:RadWindow Height="300" ID="wndMailCC" OnClientClose="CloseMailFunction" runat="server" Width="600" />
        </Windows>
    </telerik:RadWindowManager>

    <table cellspacing="0" cellpadding="1" width="100%" border="0">
        <tr class="titolo" id="AddToolbar" runat="server">
            <td>
                <asp:ImageButton ID="AddFile" ImageUrl="../Comm/Images/File16.gif" runat="server" ToolTip="Inserimento Documento" Visible="true" />
                &nbsp;
                <asp:ImageButton ID="AddMultipleFile" ImageUrl="../Comm/Images/Files16.gif" runat="server" ToolTip="Inserimento Multiplo" Visible="false" />
                &nbsp;
                <asp:ImageButton ID="AddProtocol" ImageUrl="../Comm/Images/DocSuite/Protocollo16.gif" runat="server" ToolTip="Inserimento collegamento al Protocollo" />
                &nbsp;
                <asp:ImageButton ID="AddFascicle" ImageUrl="../App_Themes/DocSuite2008/imgset16/fascicle_open.png" runat="server" ToolTip="Inserimento collegamento al Fascicolo" />
                &nbsp;
                <asp:ImageButton ID="AddResolution" ImageUrl="../Comm/Images/DocSuite/Atti16.gif" runat="server" ToolTip="Inserimento collegamento Atti" />
            </td>
        </tr>
        <tr>
            <td style="height: 21px">
                <asp:Label ID="lblTitolo" runat="server" Font-Bold="True">Label</asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" ID="DG" runat="server" ShowGroupPanel="True">
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="False" />
            <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" GridLines="Horizontal" NoMasterRecordsText="Nessun documento presente" TableLayout="Auto" Width="100%">
                <Columns>
                    <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" UniqueName="cSelezione">
                        <itemtemplate>
                            <telerik:radButton runat="server" Height="16px" Width="16px" ID="btnEdit" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderText="Cartella" UniqueName="cCartella">
                        <itemtemplate>
                            <asp:Label runat="server" Text='<%# SetupFolder(Eval("IncrementalFolder"))%>' ID="Label2"  />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="15%" wrap="True" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo Documento" UniqueName="cTipoDocumento">
                        <itemtemplate>
                            <asp:image runat="server" ID="imgType" Height="16px" Width="16px" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png" HeaderText="F" UniqueName="TemplateColumn3">
                        <itemtemplate>
                            <asp:Image runat="server" ImageUrl='<%# IsSignedIcon(Eval("description"))%>' ID="Image2" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderImageUrl="../Comm/Images/File/Version16.gif" HeaderText="V" UniqueName="cVersioningDocumento">
                        <itemtemplate>
                            <telerik:radButton Height="16px" Width="16px" ID="btnVersion" runat="server" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="StepLinearDescription" HeaderText="Step" UniqueName="StepLinearDescription">
                        <headerstyle horizontalalign="Center" width="3%" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderImageUrl="../Comm/Images/ArrowDown.gif" HeaderText="AD" UniqueName="cArrowDown">
                        <itemtemplate>
                            <telerik:radButton Width="16px" Height="16px" runat="server" ID="btnDown" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderImageUrl="../Comm/Images/ArrowUp.gif" HeaderText="AU" UniqueName="cArrowUp">
                        <itemtemplate>
                            <telerik:radButton Width="16px" Height="16px" runat="server" ID="btnUp" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="2%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy<BR/>HH.mm.ss}" HeaderText="Data Ins." SortExpression="RegistrationDate" UniqueName="RegistrationDate">
                        <headerstyle horizontalalign="Center" width="100px" wrap="False" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridTemplateColumn HeaderText="Documento" SortExpression="Description" CurrentFilterFunction="Contains" UniqueName="Description" GroupByExpression="Group By Description">
                        <itemtemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# SetupDescription("" & Eval("idObjectType"),"" & Eval("description"),"" & Eval("link"))%>'
                                CommandName='<%# "Docm:" & Eval("Year") & String.Format("{0:0000000}",Eval("Number")) & Eval("id.incremental")%>' />
                        </itemtemplate>
                        <headerstyle width="15%" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="DocumentDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data Doc." SortExpression="DocumentDate" UniqueName="DocumentDate">
                        <headerstyle horizontalalign="Center" width="80px" wrap="False" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Oggetto" UniqueName="cOggetto">
                        <itemtemplate>
                            <asp:Label runat="server" Text='<%# SetupObject("" & Eval("idObjectType"),"" & Eval("DocObject"),"" & Eval("link"))%>' ID="Label3" />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="150px" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="reason" HeaderText="Motivo" SortExpression="Reason" UniqueName="Reason" />
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="note" HeaderText="Note" SortExpression="Note" UniqueName="Note" />
                    <telerik:GridBoundColumn AllowFiltering="False" DataField="RegistrationUserDateDescription" HeaderText="Inserimento" SortExpression="RegistrationUserDateDescription" UniqueName="RegistrationUserDateDescription">
                        <headerstyle horizontalalign="Center" width="10%" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn AllowFiltering="False" DataField="LastChangeUserDateDescription" HeaderText="Modifica" SortExpression="LastChangeUserDateDescription" UniqueName="LastChangeUserDateDescription">
                        <headerstyle horizontalalign="Center" width="10%" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Mail CC" UniqueName="MailCC" Visible="False">
                        <itemtemplate>
                            <asp:Button runat="server" ID="btnMail" Text="Invia" CommandName='<%# "Mail:" & Eval("description") %>' />
                        </itemtemplate>
                        <headerstyle horizontalalign="Center" width="5%" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridTemplateColumn>
                </Columns>
                <RowIndicatorColumn Visible="False">
                    <HeaderStyle Width="20px" />
                </RowIndicatorColumn>
                <ExpandCollapseColumn Resizable="False" Visible="False">
                    <HeaderStyle Width="20px" />
                </ExpandCollapseColumn>
                <EditFormSettings>
                    <PopUpSettings ScrollBars="None" />
                </EditFormSettings>
                <PagerStyle Position="Top" />
            </MasterTableView>
            <ExportSettings FileName="Esportazione">
                <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
                <Excel Format="ExcelML" />
            </ExportSettings>
            <ClientSettings AllowDragToGroup="True" />
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Decrescente" SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
</asp:Content>
