<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtImportResult" Codebehind="ProtImportResult.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Importazione - Risultati" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table runat="server" id="tblButton" cellSpacing="0" cellPadding="1" width="100%" border="0">
	    <tr class="Spazio">
		    <td colSpan="2"></td>
		</tr>
		<tr>
			<td align="left">
			    <b>Tipologia: </b>
				<asp:dropdownlist id="ddlResult" Width="100px" AutoPostBack="True" Runat="server">
					<asp:ListItem Value="Protocollati">Protocollati</asp:ListItem>
					<asp:ListItem Value="Errori">In Errore</asp:ListItem>
					<asp:ListItem Value="Tutti" Selected="True">Tutti</asp:ListItem>
				</asp:dropdownlist>
			</td>
			<td align="right">
			    <asp:button id="btnExcel" runat="server" Visible="True" Text="Esporta in Excel"></asp:button>
			    <asp:button id="btnStampa" runat="server" Width="80px" Text="Stampa"></asp:button>
			</td>
		</tr>
	</table>
</asp:Content>		

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div runat="server" id="PageDiv">
        <asp:datagrid  id="DG" runat="server" AutoGenerateColumns="False" BorderColor="#999999" BorderStyle="None"
	        BorderWidth="1px" BackColor="White" CellPadding="3" GridLines="Vertical" AllowSorting="True">
		    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
            <HeaderStyle Wrap="False" CssClass="tabella" />
            <Columns>
		        <asp:TemplateColumn HeaderImageUrl="../Comm/Images/Remove16.gif" HeaderText="Stato">
			        <HeaderStyle  HorizontalAlign="Center" Width="2%"></HeaderStyle>
			        <ItemStyle HorizontalAlign="Center"></ItemStyle>
				    <ItemTemplate>
				        <asp:ImageButton runat="server" ImageUrl='<%# If(DataBinder.Eval(Container.DataItem, "RESULT") = "-1", "../Comm/Images/Remove16.gif", "../Comm/Images/Flag16.gif")%>' ID="Imagebutton1" CommandArgument='<%# databinder.eval(container.dataitem,"RESULT") %>'/>
				    </ItemTemplate>
			    </asp:TemplateColumn>
			    <asp:BoundColumn DataField="FILEXML" HeaderText="Metadati">
			        <HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			    </asp:BoundColumn>
			    <asp:BoundColumn DataField="FILEDOC" HeaderText="Documento">
				    <HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			    </asp:BoundColumn>
			    <asp:BoundColumn DataField="FILEXML" HeaderText="IdPrenotazione">
			        <HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			    </asp:BoundColumn>
			    <asp:BoundColumn DataField="FILEDOC" HeaderText="Contatto">
				    <HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			    </asp:BoundColumn>
				    <asp:BoundColumn DataField="ERROR" HeaderText="Risultato">
			        <HeaderStyle Wrap="False" Width="10%"></HeaderStyle>
			    </asp:BoundColumn>
		    </Columns>
		    <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999"></PagerStyle>
        </asp:datagrid>
    </div>
</asp:Content>