<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtExportResult.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtExportResult" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Esportazione - Risultati"  %>

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
				        <asp:ImageButton runat="server" ImageUrl="../Comm/Images/Remove16.gif"/>
				    </ItemTemplate>
			    </asp:TemplateColumn>
			    <asp:BoundColumn DataField="SystemUser" HeaderText="Utente">
			        <HeaderStyle Wrap="true" Width="15%"></HeaderStyle>
			    </asp:BoundColumn>
			    <asp:BoundColumn DataField="ModuleName" HeaderText="Modulo">
				    <HeaderStyle Wrap="False" Width="18%"></HeaderStyle>
			    </asp:BoundColumn>
			    <asp:BoundColumn DataField="ErrorDate" HeaderText="Data">
			        <HeaderStyle Wrap="true" Width="12%"></HeaderStyle>
			    </asp:BoundColumn>
			    <asp:BoundColumn DataField="ErrorDescription" HeaderText="Errore">
				    <HeaderStyle Wrap="true" Width="55%"></HeaderStyle>
			    </asp:BoundColumn>
		    </Columns>
		    <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999"></PagerStyle>
        </asp:datagrid>
    </div>
</asp:Content>