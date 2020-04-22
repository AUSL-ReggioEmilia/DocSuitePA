<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocumentFolder.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentFolder" %>

<asp:Panel ID="pnlCartella" runat="server">
	<table style="BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; BORDER-LEFT: gray 1px solid; WIDTH: 100%; BORDER-BOTTOM: gray 1px solid; BORDER-COLLAPSE: collapse"
		borderColor="gray" cellSpacing="0"  cellPadding="3" border="0">
		<tr>
			<td width="50%" >
				<telerik:RadTreeView id="tvwOrigin" runat="server" Width="100%">
					<Nodes>
					    <telerik:RadTreeNode Text="Origine" Expanded="True"></telerik:RadTreeNode>
					</Nodes>
				</telerik:RadTreeView></td>
			<td width="50%">
				<telerik:RadTreeView id="tvwDestination" runat="server" Width="100%" >
					<Nodes>
					    <telerik:RadTreeNode Text="Destinazione"></telerik:RadTreeNode>
					</Nodes>
				</telerik:RadTreeView></td>
		</tr>
	</table>
</asp:Panel>
