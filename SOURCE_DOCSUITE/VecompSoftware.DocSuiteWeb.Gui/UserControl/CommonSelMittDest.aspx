<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelMittDest"
    Codebehind="CommonSelMittDest.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Selezione Mittente/Destinatario" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">

        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }	
            
        	function OnRowClick(sender, eventArgs)
            {
               var des = eventArgs.getDataKeyValue("FullName");
               ReturnValues(des);
            }
        	
        	
        	function ReturnValues(valore)
            {
               CloseWindow(valore);
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>

    </telerik:RadScriptBlock>
    <table id="TBLTITLE" cellspacing="0" cellpadding="1" width="100%" border="0">
        <tr class="Scuro">
            <td colspan="2" height="20">
                Lista di Distribuzione:
                <asp:DropDownList ID="lista" runat="server" AutoPostBack="True">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid runat="server" ID="RadGridDistributionList" AllowSorting="false"
        AllowAutomaticDeletes="true" Visible="true" AllowPaging="false" Width="100%"
        EnableViewState="true" AutoGenerateColumns="false" AllowCustomPaging="false"
        GridLines="none">
        <PagerStyle Mode="NextPrevAndNumeric" NextPagesToolTip="Pagina successiva" PrevPagesToolTip="Pagina precedente"
            AlwaysVisible="True"></PagerStyle>
        <MasterTableView ClientDataKeyNames="FullName" NoMasterRecordsText="Nessun Oggetto"
            TableLayout="Fixed">
            <RowIndicatorColumn Visible="False">
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>
            <ExpandCollapseColumn Visible="False" Resizable="False">
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="FullName" HeaderText="Nome" UniqueName="FullName" SortExpression="FullName">
                    <HeaderStyle Width="65%"></HeaderStyle>
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Company" HeaderText="Società" UniqueName="Company">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="EmailAddress" HeaderText="eMail" UniqueName="EmailAddress">
                </telerik:GridBoundColumn>
            </Columns>
            <EditFormSettings>
                <PopUpSettings ScrollBars="None"></PopUpSettings>
            </EditFormSettings>
            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True"></Selecting>
            <ClientEvents OnRowClick="OnRowClick"></ClientEvents>
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>
