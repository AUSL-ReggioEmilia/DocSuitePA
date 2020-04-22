<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtImport"
    Codebehind="ProtImport.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphContent" ID="content" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerImport" PreserveClientState="False" runat="server">
        <Windows>
            <telerik:RadWindow Height="550" ID="wndResult" runat="server" Title="Importazione - Risultati" Width="700" />
            <telerik:RadWindow Behaviors="None" Height="550" ID="wndProgress" runat="server" Width="700" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
        <script type="text/javascript" language="javascript">
                      
            function <%= Me.ID %>_ShowResults() {
                document.getElementById("<%= pnlGrid.ClientID %>").visible = true;
            }

            function <%= Me.ID %>_HideResults() {
                document.getElementById("<%= pnlGrid.ClientID %>").visible = false;
            }

            function onTaskCompleted(sender,args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("");
            }
        </script>
    </telerik:RadScriptBlock>

    <table id="TBLTITLE" cellspacing="0" cellpadding="1" width="100%" border="0">
        <tr>
            <td>
                <div class="PageDiv" id="PageDiv">
                    <table id="TblContenitore" style="width: 100%; border-collapse: collapse" cellspacing="0"
                        cellpadding="0" border="0">
                        <tr>
                            <td valign="top">
                                <table cellspacing="0" cellpadding="1" width="100%" border="0">
                                    <tr class="Spazio">
                                        <td align="center">
                                            <asp:Label ID="lblImportPregresso" runat="server" Width="100%" Font-Bold="True" ForeColor="Red"
                                                Font-Size="Larger">Label</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <table id="tblDocType" runat="server" class="datatable" visible="true">
                                    <tr>
                                        <th colspan="2" id="rowImportPregresso" runat="server">
                                            Tipo Documento
                                        </th>
                                    </tr>
                                    <tr>
                                        <td class="label" style="width: 15%">
                                            &nbsp;</td>
                                        <td style="width: 85%">
                                            <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsTableDocType" ID="cmbIdDocType" runat="server">
                                                <asp:ListItem Text="" Value="0" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing="0" cellpadding="1" width="100%" border="0">
                        <tr>
                            <td>
                                <table id="tblIdContainer" runat="server" class="datatable">
                                    <tr>
                                        <th colspan="2">
                                            Contenitore</th>
                                    </tr>
                                    <tr>
                                        <td class="label" style="width: 15%; height: 23px;">
                                            &nbsp;</td>
                                        <td style="width: 85%; height: 23px;">
                                            <asp:DropDownList ID="cmbIdContainer" runat="server" AutoPostBack="true" AppendDataBoundItems="True">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvContainer" runat="server" ControlToValidate="cmbIdContainer"
                                                Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing="0" cellpadding="1" width="100%" border="0">
                        <tr class="Spazio">
                            <td style="height: 21px">
                                <uc1:uscClassificatore ID="UscClassificatore1" runat="server"></uc1:uscClassificatore>
                            </td>
                        </tr>
                    </table>
                    <table class="datatable">
                        <tr>
                            <td class="label" style="width: 15%">
                                Note:
                            </td>
                            <td style="width: 85%">
                                <telerik:RadTextBox ID="txtNote" runat="server" MaxLength="255" Width="100%"></telerik:RadTextBox>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing="0" cellpadding="1" width="100%" border="0">
                        <tr class="Spazio">
                            <td>
                            </td>
                        </tr>
                    </table>
                    <table id="tblLongRunningTask" cellpadding="0" cellspacing="0" width="100%" border="0">
                        <tr>
                            <td align="center">
                                <telerik:RadAjaxPanel runat="server" ID="pnlGrid" EnableAJAX="true">
                                    <telerik:RadGrid ID="GrResults" runat="server" AllowPaging="True" AllowSorting="True"
                                        AutoGenerateColumns="False" GridLines="Vertical" Visible="False"
                                        Width="99%">
                                        <AlternatingItemStyle BackColor="#E5E5FF"></AlternatingItemStyle>
                                        <MasterTableView>
                                            <RowIndicatorColumn Visible="False">
                                                <HeaderStyle Width="20px"></HeaderStyle>
                                            </RowIndicatorColumn>
                                            <ExpandCollapseColumn Visible="False" Resizable="False">
                                                <HeaderStyle Width="20px"></HeaderStyle>
                                            </ExpandCollapseColumn>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="FILEXML" HeaderText="Metadati" UniqueName="column3">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="FILEDOC" HeaderText="Documento" UniqueName="column1">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="ERROR" HeaderText="Errore" UniqueName="column2">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="RESULT" HeaderText="Risultato" UniqueName="column">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                            <EditFormSettings>
                                                <PopUpSettings ScrollBars="None"></PopUpSettings>
                                            </EditFormSettings>
                                        </MasterTableView>
                                        <ClientSettings AllowColumnsReorder="True" ReorderColumnsOnClient="True">
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadAjaxPanel>
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:ObjectDataSource ID="odsTableDocType" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.TableDocTypeFacade"></asp:ObjectDataSource>
            </td>
        </tr>
        <tr class="Spazio">
            <td>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                    ShowSummary="False" DisplayMode="List"></asp:ValidationSummary>
            </td>
        </tr>
    </table>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" ID="cntFooter" runat="server">
    <table border="0" runat="server" id="buttonTable">
        <tr>
            <td style="width: 180px">
                <asp:Button ID="btnInserimentoParziale" runat="server" Text="Importazione LOTTO" Width="180px" Enabled="false" />
            </td>
            <td style="width: 180px">
                <asp:Button ID="btnInserimento" runat="server" Text="Importazione Completa" Width="180px" Enabled="false" />
            </td>
            <td style="width: 180px">
                <asp:Button ID="btnDocumenti" runat="server" Text="Visualizza documenti" CausesValidation="False"
                    Width="180px" Enabled="False" />
            </td>
            <td style="width: 250px">
                <asp:Button ID="btnRisultati" runat="server" Text="Visualizza Risultato Importazione"
                    CausesValidation="False" Width="250px" />
            </td>
        </tr>
    </table>
</asp:Content>
