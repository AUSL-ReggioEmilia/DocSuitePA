<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtImportList" Codebehind="ProtImportList.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Importazione - Documenti" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:datagrid id="DG" runat="server" AllowSorting="True" GridLines="Vertical" CellPadding="3"
		BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#999999" AutoGenerateColumns="False">
		<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
		<HeaderStyle Wrap="False" CssClass="tabella" />
        <Columns>
			<asp:TemplateColumn HeaderImageUrl="../Comm/Images/Remove16.gif" HeaderText="Stato">
				<HeaderStyle HorizontalAlign="Center" Width="2%"></HeaderStyle>
				<ItemStyle HorizontalAlign="Center"></ItemStyle>
				<ItemTemplate>
					<asp:ImageButton ID="ImageButton1" Runat="server" ImageUrl='<%# If(DataBinder.Eval(Container.DataItem, "STATUS") = 0, "../Comm/Images/Remove16.gif", "../Comm/Images/Flag16.gif")%>' />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="FILEXML" HeaderText="Metadati">
				<HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			</asp:BoundColumn>
			<asp:BoundColumn DataField="FILEDOC" HeaderText="Documento">
				<HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			</asp:BoundColumn>
		</Columns>
		<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999"></PagerStyle>
	</asp:datagrid>
	<asp:table id="tblRicerca" runat="server" Width="100%">
		<asp:TableRow>
			<asp:TableCell Width="100%" Font-Bold="True" Text="Nessun File Disponibile per l'Importazione"></asp:TableCell>
		</asp:TableRow>
	</asp:table>
</asp:Content>