<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslRitiraAtti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslRitiraAtti" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Facade" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
	<telerik:RadScriptBlock runat="server" ID="rsbFDQMultiple" EnableViewState="false">
		<script language="javascript" type="text/javascript">
		    function OpenFile(p_inputDir, p_outputDir) {
		        try {
		            var signer = new ActiveXObject("VecVBSignerMngOCX.VecFileSharedMngOCX");
		            if (typeof(signer) != "undefined") {
		                signer.InputDir = p_inputDir;
		                signer.OutputDir = p_outputDir;

		                signer.SignDocuments();

		                var manager = $find("<%= Me.AjaxManager.ClientID %>");
		                manager.ajaxRequest("SIGN");
		            } else {
		                throw new GenericException();
		            }
		        } catch(ex) {
		            alert("Componente di firma non installato.\nContattare il supporto IT.\n"+ ex.Message );
		        }
		    }
		    
		</script>
		<script language="javascript" type="text/javascript">
			var oHasError = document.getElementById('<%= hdnHasError.ClientID %>');
			var oComplete = document.getElementById('<%= hdnComplete.ClientID %>');
			function GetFileIndex(fileName) {
				fileName = fileName.substr(0, fileName.lastIndexOf("<%= ResolutionJournalFacade.Separator %>"));
				return fileName.replace('<%= ResolutionJournalFacade.Separator %>', ';');
			}
		</script>
		<script language="javascript" type="text/javascript" src="../js/fixit.js"></script>
		<script for="SIGNER" event="OnStartSignature()" language="Jscript" type="text/javascript">
		    oHasError.value = '';
		</script>
		<script for="SIGNER" event="OnSign(FileName, Status)" language="Jscript" type="text/javascript">
		    var index = GetFileIndex(FileName);
		    if (Status) {
		        if (icons[index]) icons[index].src = '../App_Themes/DocSuite2008/imgset16/card_chip_gold.png';
		        oComplete.value = "if ( icons['" + index + "'] ) icons['" + index + "'].src = '../App_Themes/DocSuite2008/imgset16/card_chip_gold.png';"
		    }
		    else {
		        oHasError.value += "if ( icons['" + index + "'] ) icons['" + index + "'].src = '../Comm/Images/Remove16.gif';"
		        oHasError.value += "if ( icons['" + index + "'] ) icons['" + index + "'].alt = 'Firma documento non eseguita';"
		    }
		</script>
	</telerik:RadScriptBlock>
    <input type="hidden" id="hdnHasError" runat="server" />
    <input type="hidden" id="hdnComplete" runat="server" />
	<table class="dataform" style="display:none">
		<%-- Prova --%>
		<tr>
			<td class="label" style="width: 25%;">
				Storico:
			</td>
			<td>
				<asp:CheckBox ID="showHistory" runat="server" Text="Mostra atti ritirati già firmati" />
			</td>
		</tr>
	</table>
	<table style="width: 100%;">
		<tr id="trGridHeader" runat="server">
			<td>
				<div class="titolo" id="divTitolo">
					<asp:Label ID="lblHeader" runat="server" />
				</div>
			</td>
		</tr>
	</table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
	    <DocSuite:BindGrid AllowCustomPaging="true" AllowFilteringByColumn="true" AllowMultiRowSelection="true" AllowSorting="True" AutoGenerateColumns="False" GridLines="Both" ID="Resolutions" runat="server" ShowGroupPanel="True" Visible="true">
		    <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="false" />
		    <MasterTableView AllowMultiColumnSorting="True" DataKeyNames="ID" NoMasterRecordsText="Nessun Atto Trovato" TableLayout="Fixed">
			    <Columns>
				    <%-- CheckBox --%>
				    <telerik:GridTemplateColumn AllowFiltering="false" DataField="ResolutionJournal" Groupable="false" UniqueName="colSelect">
					    <HeaderStyle HorizontalAlign="Center" Width="25px" />
					    <ItemStyle HorizontalAlign="Center" Width="25px" />
					    <ItemTemplate>
						    <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="false" CommandName="Selected"></asp:CheckBox>
					    </ItemTemplate>
				    </telerik:GridTemplateColumn>
				    <%-- Visualizzazione --%>
				    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/text_signature.png" HeaderText="Visualizza documento da firmare" UniqueName="cType">
					    <HeaderStyle HorizontalAlign="Center" Width="25px" />
					    <ItemStyle HorizontalAlign="Center" Width="25px" />
					    <ItemTemplate>
						    <asp:ImageButton ID="imgType" runat="server" CommandName="Open" />
					    </ItemTemplate>
				    </telerik:GridTemplateColumn>
				    <%-- Tipo --%>
				    <telerik:GridTemplateColumn HeaderText="Tipo" UniqueName="idTipologia" AllowFiltering="false" Groupable="false">
					    <HeaderStyle HorizontalAlign="Left" Width="7%" />
					    <ItemStyle HorizontalAlign="Left" Width="7%" />
					    <ItemTemplate>
						    <asp:label runat="server" ID="Type" />
					    </ItemTemplate>
				    </telerik:GridTemplateColumn>
				    <%-- Oggetto --%>
				    <telerik:GridBoundColumn DataField="ResolutionObject" HeaderText="Oggetto" CurrentFilterFunction="Contains" UniqueName="ResolutionObject" SortExpression="ResolutionObject">
					    <headerstyle horizontalalign="Left" width="100%" />
					    <itemstyle horizontalalign="Left" wrap="true"/>
				    </telerik:GridBoundColumn>
			    </Columns>
		    </MasterTableView>
		    <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente"
			    SortToolTip="Ordina" />
		    <ClientSettings AllowGroupExpandCollapse="True" AllowDragToGroup="True">
			    <Selecting AllowRowSelect="true" />
		    </ClientSettings>
	    </DocSuite:BindGrid>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
	<asp:Panel runat="server" ID="pnlFooter">
		<asp:Button runat="server" ID="FirmaSelezionate" Text="Firma Selezionate" />
	</asp:Panel>
</asp:Content>
