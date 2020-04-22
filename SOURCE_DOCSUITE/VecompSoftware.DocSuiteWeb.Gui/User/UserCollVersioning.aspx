<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserCollVersioning.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserCollVersioning" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid ID="gvCollVersioning" runat="server" AutoGenerateColumns="False" AllowMultiRowSelection="True" AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CloseFilterMenuOnClientClick="True" ShowGroupPanel="True">
            <MasterTableView TableLayout="Auto" NoMasterRecordsText="Nessun Documento Trovato" GridLines="Both" AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" CommandItemDisplay="Top">
            <Columns>
                <telerik:GridBoundColumn DataField="CollaborationIncremental" HeaderText="Oggetto" UniqueName="CollaborationIncremental" CurrentFilterFunction="EqualTo" SortExpression="CollaborationIncremental" DataType="System.Int16">
                    <HeaderStyle horizontalalign="Center" width="120px"/>
	                <ItemStyle horizontalalign="Center" width="120px"/>
	            </telerik:GridBoundColumn>
	            <telerik:GridBoundColumn DataField="Incremental" HeaderText="Progressivo" UniqueName="Incremental" CurrentFilterFunction="EqualTo" SortExpression="Incremental" DataType="System.Int16">
		            <HeaderStyle horizontalalign="Center" width="120px"/>
	                <ItemStyle horizontalalign="Center" width="120px"/>
	            </telerik:GridBoundColumn>
	            <telerik:GridTemplateColumn HeaderText="Nome Documento" UniqueName="DocumentName" CurrentFilterFunction="Contains" Groupable="False" InitializeTemplatesFirst="False">
		            <ItemTemplate>
                        <asp:LinkButton runat="server" id="lnkButton" CommandName="Docu" />
                    </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="RegistrationUser" HeaderText="Utente" UniqueName="RegistrationUser" CurrentFilterFunction="Contains" SortExpression="RegistrationUser">
	            </telerik:GridBoundColumn>
	            <telerik:GridDateTimeColumn DataField="RegistrationDate" HeaderText="Data" UniqueName="RegistrationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" CurrentFilterFunction="EqualTo" SortExpression="RegistrationDate"> 
                    <HeaderStyle horizontalalign="Center" wrap="False" width="125px"/>
                    <ItemStyle horizontalalign="Center" width="125px"/>
                </telerik:GridDateTimeColumn>
	            <telerik:GridBoundColumn DataField="IdDocument" HeaderText="IdDocument" UniqueName="IdDocument" CurrentFilterFunction="EqualTo" SortExpression="IdDocument" DataType="System.Int32">
		            <HeaderStyle horizontalalign="Center" width="120px"/>
	                <ItemStyle horizontalalign="Center" width="120px"/>
	            </telerik:GridBoundColumn>
            </Columns>
            <RowIndicatorColumn>
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <ItemStyle Font-Names="Verdana" />
            <AlternatingItemStyle Font-Names="Verdana" />
            <PagerStyle Position="Top" Visible="False" />
            </MasterTableView>
            <clientsettings AllowDragToGroup="True">
                <Selecting AllowRowSelect="True" />
            <ClientMessages DragToResize="Ridimensiona" />
            <Resizing AllowColumnResize="True" ClipCellContentOnResize="False" ResizeGridOnColumnResize="True" />
            </clientsettings>
            <ExportSettings FileName="Esportazione">
            <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
            <Excel Format="ExcelML" />
            </ExportSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Decrescente"
            SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>