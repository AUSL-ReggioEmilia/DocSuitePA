<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserFDQMultiple.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserFDQMultiple" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Collaborazione - Gestione Firma Multipla" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbFDQMultiple" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function Close(returnValue) {
                var oWindow = GetRadWindow();
                oWindow.close(returnValue);
            }

            var activex = new ActiveXObject("CSActiveX.CSActiveXCtrl");
            function raise(inputDir, outputDir) {
                if (!activex) {
                    alert("Impossibile trovare il componente.");
                    return;
                }

                activex.InputDirectory = inputDir;
                activex.OutputDirectory = outputDir;
                eval("function activex::OnError(message) {return showError(message);}");
                eval("function activex::OnProcessedItem(index) {return showProcessed(index);}");
                activex.SignDocuments();
                setTimeout(checkIsDone, 1000);
            }

            function showError(message) {
                document.getElementById("risultati").value += "\n\r " + message;
                return true;
            }

            function showProcessed(index) {
                document.getElementById("risultati").value += "\n\r Item " + index;
                return true;
            }

            function checkIsDone() {
                if (!activex.IsDone()) {
                    setTimeout(checkIsDone, 8000);
                    return;
                }
                var btnCheckIn = document.getElementById('<%= btnCheckIn.ClientID %>');
                btnCheckIn.click();
            }

            //function OpenFile(inputDir, outputDir) {
            //    try {
            //        var signer = new ActiveXObject("VecVBSignerMngOCX.VecFileSharedMngOCX");
            //        if (typeof(signer) != "undefined") {
            //            signer.InputDir = inputDir;
            //            signer.OutputDir = outputDir;
            //        
            //            var chkFVicario = $get('<%= chkFVicario.ClientID %>');
            //            var strFVicario = '<%= FVicario %>';
            //            switch (true) {
            //            case typeof(chkFVicario) == "undefined" || !chkFVicario.checked:
            //                // Funzione non disponibile o non selezionata.
            //                break;
            //            case typeof(signer.FVicario) == "undefined":
            //                // Funzione non supportata dal componente di firma.
            //                throw "Necessario aggiornamento del prodotto per SHA256.\rContattare il supporto IT.";
            //                break;
            //            case typeof(strFVicario) == "undefined" || strFVicario == '':
            //                // Parametro "DefaultFVicario" mancante o non valorizzato in ParameterEnv di Protocollo.
            //                throw "Parametro \"DefaultFVicario\" non valorizzato in ParameterEnv di Protocollo.\rContattare il supporto IT.";
            //                break;
            //            default:
            //                // Funzione selezionata e configurata correttamente.
            //                signer.FVicario = strFVicario;
            //                break;
            //            }
            //            signer.SignDocuments();
            //        
            //            var btnCheckIn = document.getElementById('<%= btnCheckIn.ClientID %>')
            //            btnCheckIn.click();
            //        } else {
            //            throw "Componente di firma non installato.\rContattare il supporto IT.";
            //        }
            //    } catch(ex) {
            //        alert(ex);
            //    }
            //}
        </script>
        <object codebase="CSActiveX.cab#Version=1,0,0,0"></object>
        <script language="javascript" type="text/javascript">
			var oHasError = document.getElementById('<%= hdnHasError.ClientID %>');
			var oComplete = document.getElementById('<%= hdnComplete.ClientID %>');
			function GetFileIndex(fileName)
			{
				fileName = fileName.substr(0, fileName.lastIndexOf("§"));
				return fileName.replace('§',';');
			}
        </script>

        <script language="javascript" type="text/javascript" src="../js/fixit.js"></script>

        <script for="SIGNER" event="OnStartSignature()" language="Jscript">
			oHasError.value = '';
        </script>

        <script for="SIGNER" event="OnSign(FileName, Status)" language="Jscript">
		var index = GetFileIndex(FileName);
		if ( Status )
		{
		    if (icons[index]) icons[index].src = '../App_Themes/DocSuite2008/imgset16/card_chip_gold.png';
		    oComplete.value = "if ( icons['" + index + "'] ) icons['" + index + "'].src = '../App_Themes/DocSuite2008/imgset16/card_chip_gold.png';"
		}
		else 
		{
			oHasError.value += "if ( icons['" + index + "'] ) icons['" + index + "'].src = '../Comm/Images/Remove16.gif';"
			oHasError.value += "if ( icons['" + index + "'] ) icons['" + index + "'].alt = 'Firma documento non eseguita';"
		}
        </script>
    </telerik:RadScriptBlock>
    
    <input type="hidden" id="hdnHasError" runat="server" /> 
    <input type="hidden" id="hdnComplete" runat="server" />
    <asp:CheckBox ID="chkAllegati" AutoPostBack="True" Checked="False" Font-Bold="True" Text="Includi allegati" runat="server"></asp:CheckBox>
</asp:Content>

<asp:Content  runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid ID="gvFDQMultiple" runat="server" Width="100%" AutoGenerateColumns="False" AllowMultiRowSelection="True" AllowFilteringByColumn="False" AllowPaging="False" AllowSorting="True" CloseFilterMenuOnClientClick="True" CustomPageIndex="0" GroupingEnabled="false" EnableClearFilterButton="false" > 
        <MasterTableView TableLayout="Fixed" Width="100%" Height="100%" NoMasterRecordsText="Nessun Documento Trovato" GridLines="Both" AllowCustomPaging="False" AllowCustomSorting="False" AllowMultiColumnSorting="True">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="SignDoc" AllowFiltering="false" Groupable="false">
                    <HeaderStyle Width="30px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSignDoc" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="T" UniqueName="cType" AllowFiltering="False" Groupable="False">
		    		<ItemTemplate>
                        <asp:ImageButton runat="server" ID="imgType" AlternateText='Apertura del Documento' CommandName='Docu' />
                    </ItemTemplate>
    				<HeaderStyle horizontalalign="Center" width="25px" />
	    			<ItemStyle horizontalalign="Center" width="25px" />
			    </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Collaboration" HeaderText="Codice" UniqueName="Collaboration" AllowFiltering="False">
                    <HeaderStyle horizontalalign="Center" width="8%" />
	    			<ItemStyle horizontalalign="Center" width="8%" />
		    	</telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DocumentType" HeaderText="Tipo" UniqueName="DocumentType" AllowFiltering="False" Groupable="False" />
                <telerik:GridBoundColumn DataField="DocumentName" HeaderText="Documento" UniqueName="DocumentName" AllowFiltering="False" Groupable="False" />
                <telerik:GridBoundColumn DataField="Object" HeaderText="Oggetto" UniqueName="CollaborationObject" AllowFiltering="False" Groupable="False" />
                <telerik:GridBoundColumn DataField="FileName" HeaderText="FileName" UniqueName="FileName" AllowFiltering="False" Groupable="False" />
                <telerik:GridBoundColumn DataField="Incremental" HeaderText="Incremental" UniqueName="Incremental" AllowFiltering="False" Groupable="False" />
                <telerik:GridBoundColumn DataField="IdDocument" UniqueName="IdDocument" AllowFiltering="False" Groupable="False" />
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
        <clientsettings AllowDragToGroup="False">
            <Selecting AllowRowSelect="True" />
            <ClientMessages DragToResize="Ridimensiona" />
            <Resizing AllowColumnResize="True" ClipCellContentOnResize="False" ResizeGridOnColumnResize="True" />
        </clientsettings>
        <ExportSettings FileName="Esportazione">
            <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
            <Excel Format="ExcelML" />
        </ExportSettings>
        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine Decrescente" SortToolTip="Ordina" />
    </DocSuite:BindGrid>
    </div>
    <textarea id="risultati" rows="8" style="width: 100%"></textarea>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <div class="dsw-text-center">
        <asp:Button ID="btnFDQ" Text="Firma documenti" runat="server"></asp:Button>
        <asp:Button ID="btnCheckIn" runat="server" Text="Check-In Documenti"></asp:Button>
        <asp:CheckBox ID="chkFVicario" runat="server" Text="Firma con funzioni vicariali" />
    </div>
</asp:Content>