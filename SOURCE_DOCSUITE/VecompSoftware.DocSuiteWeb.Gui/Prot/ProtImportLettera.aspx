<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtImportLettera" Codebehind="ProtImportLettera.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Protocollo - Importazione Lettere" %>

<%@ Register Src="../UserControl/uscOggetto.ascx" TagName="uscOggetto" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="cphContent" ID="content" runat="server">
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerImport" PreserveClientState="True" runat="server" >
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

            function onTaskCompleted(sender, args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("");
            }
        </script>

    </telerik:RadScriptBlock>
    <table class="datatable">
        <tr>
            <th colspan="2">
                <asp:Label ID="lblTitle" runat="server" /></th>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">
                Directory:</td>
            <td style="width: 85%;">
                <asp:Label ID="lblDirectory" runat="server" /></td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">
                File Excel:</td>
            <td style="width: 85%;">
                <asp:Label ID="lblExcel" runat="server" /></td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <asp:Label ID="lblImportPregresso" runat="server" Width="100%" Font-Bold="True" ForeColor="Red"
                    Font-Size="Larger">Label</asp:Label>
            </td>
        </tr>
    </table>
    <table id="tblDocType" class="datatable" runat="server" visible="false">
        <tr>
            <th colspan="2">
                Tipo Documento
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">
            </td>
            <td style="width: 85%">
                <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsTableDocType" ID="cmbIdDocType" runat="server" Visible="false">
                    <asp:ListItem />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="cmbIdDocType" Display="Dynamic" ErrorMessage="Tipo Documento obbligatorio" ID="rfvDocType" runat="server" />
            </td>
        </tr>
    </table>
    <table id="tblIdContainer" class="datatable" runat="server">
        <tr>
            <th colspan="2">
                Contenitore
                </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">
            </td>
            <td style="width: 85%;">
                <asp:DropDownList ID="cmbIdContainer" runat="server" AutoPostBack="true" AppendDataBoundItems="true">
                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvContainer" runat="server" ControlToValidate="cmbIdContainer"
                    ErrorMessage="Campo contenitore obbligatorio" Display="Dynamic" />
            </td>
        </tr>
    </table>
    <table id="tblIdStatus" class="datatable" runat="server" visible="false">
        <tr>
            <th colspan="2">
                Stato del protocollo</th>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">
                Stato</td>
            <td style="width: 85%;">
                <asp:DropDownList ID="cmbProtocolStatus" runat="server" />
            </td>
        </tr>
    </table>
    <uc:uscClassificatore ID="uscClassificatore" runat="server" />
    <table class="datatable" align="center">
        <tr>
            <th>
                Oggetto
            </th>
        </tr>
        <tr>
            <td style="width: 95%;">
                <uc1:uscOggetto ID="UscOggetto1" runat="server" />
            </td>
        </tr>
    </table>
    <table class="datatable" align="center">
        <tr>
            <td class="label" style="width: 15%">
                Note:
            </td>
            <td>
                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" MaxLength="255" /></td>
        </tr>
    </table>
    <div align="center">
        <asp:Panel runat="server" ID="pnlGrid">
            <DocSuite:BaseGrid ID="GrResults" runat="server" GridLines="Vertical"
                AllowPaging="True" AllowSorting="True" Visible="False" Width="99%" AutoGenerateColumns="False">
                <MasterTableView>
                    <RowIndicatorColumn Visible="False">
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn Resizable="False" Visible="False">
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <EditFormSettings>
                        <PopUpSettings ScrollBars="Auto" />
                    </EditFormSettings>
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
                </MasterTableView>
                <AlternatingItemStyle BackColor="#E5E5FF" />
                <ClientSettings AllowColumnsReorder="True" ReorderColumnsOnClient="True">
                </ClientSettings>
            </DocSuite:BaseGrid>
        </asp:Panel>
    </div>
    <!-- datasource -->
    <asp:ObjectDataSource ID="odsTableDocType" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.TableDocTypeFacade" />
    <asp:ObjectDataSource ID="odsProtocolType" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ProtocolTypeFacade" />
    <asp:ObjectDataSource ID="odsContainer" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ContainerFacade" />
    <p>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" DisplayMode="List"></asp:ValidationSummary>
    </p>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" ID="cntFooter" runat="server">
    <table border="0" runat="server" id="buttonTable">
        <tr>
            <td>
                <asp:Button ID="btnInserimento" runat="server" Text="Conferma Importazione" Width="180px" />
                <asp:Button CausesValidation="False" ID="btnDocumenti" runat="server" Text="Visualizza documenti" Width="180px" />
                <asp:Button CausesValidation="False" Enabled="False" ID="btnRisultati" runat="server" Text="Visualizza Risultato Importazione" Width="250px" />
            </td>
        </tr>
    </table>
</asp:Content>
