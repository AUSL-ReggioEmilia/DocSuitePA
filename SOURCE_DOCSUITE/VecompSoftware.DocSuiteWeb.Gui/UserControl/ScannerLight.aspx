<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ScannerLight.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.ScannerLight" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="ScannerLightScript" EnableViewState="false">
        <script type="text/javascript">

            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            // Chiude la finestra modale con i parametri
            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }

            function GetScannerCom() {
                return $get("DynamicWebTwain1");
            }

            function GetScannerSource() {
                var toolBar = $find("<%= ToolBar.ClientID%>");
                var containerButton = toolBar.findItemByValue("sourceContainer");
                return $($telerik.findElement(containerButton.get_element(), "source"));
            }

            function pageLoad() {
                if (!scanSoftwareAvailable()) {
                    return false;
                }

                GetRadWindow().set_title("Scanner (" + GetScannerCom().VersionInfo + ")");

                var scannerSources = GetScannerSource();
                for (var i = 0; i < GetScannerCom().SourceCount; i++) {
                    var name = GetScannerCom().SourceNameItems(i);
                    scannerSources.append($("<option />").val(name).text(name));
                }
               
                return true;
            }

            // Controlla se il componente è disponibile, se lo è torna true
            function scanSoftwareAvailable() {
                if (!GetScannerCom()) {
                    alert("Scanner non trovato: possibile componente di scansione errato o non installato. <%=ProtocolEnv.DefaultErrorMessage%>");
                    return false;
                }
                return true;
            }

            // Controlla se è ancora possibile aggiungere pagine e gestisce bottoni e tooltip di conseguenza
            function bufferLimitValidation() {
                var scanButton = $find("<%=ToolBar.ClientID%>").findItemByValue("scan");
                if (GetScannerCom().HowManyImagesInBuffer === GetScannerCom().MaxImagesInBuffer) {
                    scanButton.set_toolTip("Limite di pagine raggiunto. Salvare il lavoro e ricominciare.");
                    scanButton.set_enabled(false);
                    return true;
                }
                else {
                    scanButton.set_toolTip("");
                    scanButton.set_enabled(true);
                    return false;
                }
            }

            function btnUpload_onclick(button, args, strHostIp, httpPort, isSSL, actionPage, documentName) {
                try {
                    filenameValidation(button, args);
                    if (!scanSoftwareAvailable()) {
                        args.set_cancel(true);
                        return false;
                    }

                    if (GetScannerCom().HowManyImagesInBuffer == 0) {
                        throw new GenericException("Nessuna immagine in memoria.");
                    }
                    GetScannerCom().HTTPPort = httpPort;
                    GetScannerCom().IfSSL = isSSL;

                    // Eseguo l'upload
                    try {
                        GetScannerCom().HTTPUploadAllThroughPostAsPDF(strHostIp, actionPage, documentName);
                    }
                    catch (e) {
                        throw new GenericException("Errore: " + e.message +
                            " Codice: " + GetScannerCom().ErrorCode +
                            " Messaggio: " + GetScannerCom().ErrorString +
                            " Risposta: " + GetScannerCom().HTTPPostResponseString);
                    }

                    if (GetScannerCom().ErrorCode !== 0) {
                        // Inserire GetScannerCom().ErrorString ?
                        throw new GenericException("Errore in fase di acquisizione.");
                    }

                    return true;
                }
                catch (ex) {
                    alert(ex.message);
                    args.set_cancel(true);
                    return false;
                }
            }

            function OnPostTransferFunction() {
                // Dopo ogni scansione controllo se il check ADF è Deselezionato, in tal caso fermo le stampe
                if (GetScannerCom().XferCount == 1) {
                    GetScannerCom().CloseSource();
                }
            }

            function DynamicWebTwain1_OnPostTransfer() {

                if ($get("<%=chkDiscardBlank.ClientID%>").checked) {
                    var newlyScannedImage = GetScannerCom().CurrentImageIndexInbuffer;
                    GetScannerCom().BlankImageThreshold = $find("<%= txtThreshold.ClientID%>").get_value();
                    if (GetScannerCom().IsBlankImage(newlyScannedImage)) {
                        GetScannerCom().RemoveImage(newlyScannedImage);
                    }
                }
                $get("TotalImage").value = GetScannerCom().HowManyImagesInBuffer;
                $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
                bufferLimitValidation();
            }

            function DynamicWebTwain1_OnMouseClick(index) {
                if (!scanSoftwareAvailable()) {
                    return null;
                }
                $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
            }

            //#region [ Toolbar ]

            function toolbar_clientClicking(sender, args) {
                if (args.get_item().get_value() === "scan") {
                    acquireFromScanner();
                    bufferLimitValidation();
                    return true;
                }

                if (!CheckIfImagesInBuffer()) {
                    return false;
                }

                switch (args.get_item().get_value()) {
                    case "first":
                        GetScannerCom().CurrentImageIndexInBuffer = 0;
                        $get("TotalImage").value = GetScannerCom().HowManyImagesInBuffer;
                        $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
                        break;
                    case "before":
                        if (GetScannerCom().CurrentImageIndexInBuffer == 0) {
                            return false;
                        }
                        GetScannerCom().CurrentImageIndexInBuffer = GetScannerCom().CurrentImageIndexInBuffer - 1;
                        $get("TotalImage").value = GetScannerCom().HowManyImagesInBuffer;
                        $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
                        break;
                    case "after":
                        if (GetScannerCom().CurrentImageIndexInBuffer == GetScannerCom().HowManyImagesInBuffer - 1) {
                            return false;
                        }
                        GetScannerCom().CurrentImageIndexInBuffer = GetScannerCom().CurrentImageIndexInBuffer + 1;
                        $get("TotalImage").value = GetScannerCom().HowManyImagesInBuffer;
                        $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
                        break;
                    case "last":
                        GetScannerCom().CurrentImageIndexInBuffer = GetScannerCom().HowManyImagesInBuffer - 1;
                        $get("TotalImage").value = GetScannerCom().HowManyImagesInBuffer;
                        $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
                        break;
                    case "rotatePage":
                        GetScannerCom().RotateRight(GetScannerCom().CurrentImageIndexInBuffer);
                        break;
                    case "deletePage":
                        GetScannerCom().RemoveImage(GetScannerCom().CurrentImageIndexInBuffer);
                        $get("TotalImage").value = GetScannerCom().HowManyImagesInBuffer;
                        if (GetScannerCom().HowManyImagesInBuffer == 0) {
                            $get("CurrentImage").value = "";
                        }
                        else {
                            $get("CurrentImage").value = GetScannerCom().CurrentImageIndexInBuffer + 1;
                        }
                        bufferLimitValidation();
                        break;
                    case "deleteAll":
                        GetScannerCom().RemoveAllImages();
                        $get("TotalImage").value = "0";
                        $get("CurrentImage").value = "";
                        bufferLimitValidation();
                        break;
                }

                return true;
            }

            function CheckIfImagesInBuffer() {

                if (!scanSoftwareAvailable())
                    return false;

                if (GetScannerCom().HowManyImagesInBuffer == 0) {
                    alert("Nessuna immagine in memoria.");
                    return false;
                }

                return true;
            }

            // Acquisisce l'immagine
            function acquireFromScanner() {
                if (!scanSoftwareAvailable()) {
                    return false;
                }
                // Selezione dello scanner, chiusura del source per eventuali operazioni appese e riapertura per la nuova acquisizione
                var scannerSources = GetScannerSource();
                if (!scannerSources.val()) {
                    alert("Nessuna sorgente selezionata.");
                    return false;
                }
                try {
                    GetScannerCom().SelectSourceByIndex(scannerSources.prop("selectedIndex"));
                    GetScannerCom().CloseSource();
                    GetScannerCom().OpenSource();
                } catch (e) {
                    alert("Errore inizializzazione: " + e.message + " Codice: " + GetScannerCom().ErrorCode + " Messaggio: " + GetScannerCom().ErrorString);
                    // Source non trovata, non posso continuare in assenza di scanner
                    if (GetScannerCom().ErrorCode == -1033) {
                        return false;
                    }
                }

                var colors = document.getElementsByName("<%=rblColor.UniqueID%>");
                for (var j = 0; j < colors.length; j++) {
                    if (colors[j].checked) {
                        GetScannerCom().PixelType = j;
                    }
                }

                GetScannerCom().MaxImagesInBuffer = <%=MaxImagesToScan%>;

                var resolution = $get("<%= ddlResolution.ClientID%>");
                GetScannerCom().Resolution = resolution.value;
                if (GetScannerCom().Resolution != resolution.value) {
                    alert("Lo scanner selezionato non supporta la risoluzione selezionata.");
                    return false;
                }

                GetScannerCom().IfShowUI = $get("<%=chkShowUI.ClientID%>").checked;

                // Di default leggo il massimo numero di pagine memorizzabili (da parametro)
                var pagineRimanenti = GetScannerCom().MaxImagesInBuffer - GetScannerCom().HowManyImagesInBuffer;
                if (pagineRimanenti == 0) {
                    pagineRimanenti = -1;
                }
                GetScannerCom().XferCount = pagineRimanenti;
                GetScannerCom().CDM = true;

                // Verifico se i driver consentono l'uso dell'Automatic Document Feeder e lo imposto
                if ($get("<%=chkADF.ClientID%>").checked) {
                    try {
                        GetScannerCom().IfFeederEnabled = true;
                        GetScannerCom().IfAutoFeed = true;
                    } catch (e) {
                        // alert("Problemi attivazione Feeder, probabile scanner piatto: " + e.message + " Codice: " + frmScan.DynamicWebTwain1.ErrorCode + " Messaggio: " + frmScan.DynamicWebTwain1.ErrorString);
                    }
                } else {
                    try {
                        // Deve essere letta una sola pagina
                        GetScannerCom().XferCount = 1;
                        GetScannerCom().IfFeederEnabled = false;
                    } catch (e) {
                        // alert("Problemi disattivazione Feeder: " + e.message + " Codice: " + frmScan.DynamicWebTwain1.ErrorCode + " Messaggio: " + frmScan.DynamicWebTwain1.ErrorString);
                    }
                }

                // Verifico se i driver consentono l'uso del duplex e lo imposto
                var duplexAvailable;
                try {
                    duplexAvailable = GetScannerCom().Duplex != 0;
                } catch (e) {
                    duplexAvailable = false;
                    // alert("Errore: " + e.message + " Codice: " + frmScan.DynamicWebTwain1.ErrorCode + " Messaggio: " + frmScan.DynamicWebTwain1.ErrorString);
                }

                if (duplexAvailable) {
                    GetScannerCom().IfDuplexEnabled = $get("<%=chkDuplex.ClientID%>").checked;
                }

                // Chiudo la Data Source User Interface dopo l'acquisizione
                GetScannerCom().IfDisableSourceAfterAcquire = true;

                // Acquisizione immagine
                try {
                    GetScannerCom().AcquireImage();
                } catch (e) {
                    alert("Errore in acquisizione immagine: " + e.message + " Codice: " + GetScannerCom().ErrorCode + " Messaggio: " + GetScannerCom().ErrorString);
                    GetScannerCom().CloseSource();
                    return false;
                }
                return true;
            }

            function filenameValidation(sender, eventArgs) {
                if (Page_ClientValidate("file")) {
                    return true;
                } else {
                    eventArgs.set_cancel(true);
                }
            }
            //#endregion
        </script>
        <script event="OnPostTransfer" for="DynamicWebTwain1" language="javascript" type="text/javascript">
            <!--
            OnPostTransferFunction();
    //-->
        </script>
        <script event="OnPostTransfer" for="DynamicWebTwain1" language="javascript" type="text/javascript">
            <!--
            DynamicWebTwain1_OnPostTransfer();
    //-->
        </script>
        <script event="OnMouseClick(index)" for="DynamicWebTwain1" language="javascript"
            type="text/javascript">
            <!--
            DynamicWebTwain1_OnMouseClick(index);
    //-->
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadToolBar AutoPostBack="False" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" OnClientButtonClicking="toolbar_clientClicking" runat="server" Width="100%">
        <Items>
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/resultset_first.png" ToolTip="Prima pagina" Value="first" />
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/resultset_previous.png" ToolTip="Precedente" Value="before" />
            <telerik:RadToolBarButton>
                <ItemTemplate>
                    <input name="CurrentImage" id="CurrentImage" type="text" size="2" readonly="readonly" />
                    <input id="TotalImage" name="TotalImage" readonly="readonly" size="2" type="text" value="0" />
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/resultset_next.png" ToolTip="Successiva" Value="after" />
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/resultset_last.png" ToolTip="Ultima pagina" Value="last" />
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/arrow_undo.png" ToolTip="Ruota la pagina a destra di 90°" Value="rotatePage" />
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/page_delete.png" ToolTip="Cancella pagina corrente" Value="deletePage" />
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" ToolTip="Cancella tutte" Value="deleteAll" />
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton>
                <ItemTemplate>
                    <span class="templateText">Sorgente:</span>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton Value="sourceContainer">
                <ItemTemplate>
                    <asp:DropDownList runat="server" ID="source" />
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/scanner.png" Text="Acquisisci" ToolTip="Acquisizione delle pagine" Value="scan" />
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton CommandName="CheckDematerialisationCompliance">
                <ItemTemplate>
                    <telerik:RadButton runat="server" ID="chkDematerialisationCompliance" ToggleType="CheckBox" ButtonType="StandardButton" Checked="False" AutoPostBack="false" Visible="False">
                        <ToggleStates>
                            <telerik:RadButtonToggleState Text="Attestazione di conformità" PrimaryIconCssClass="rbToggleCheckboxChecked"/>
                            <telerik:RadButtonToggleState Text="Attestazione di conformità" PrimaryIconCssClass="rbToggleCheckbox" />
                        </ToggleStates>
                    </telerik:RadButton>
                </ItemTemplate>
            </telerik:RadToolBarButton>
        </Items>
    </telerik:RadToolBar>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent" style="vertical-align: top;">
    <table style="width: 100%; height: 100%;">
        <tr>
            <td style="text-align: center; vertical-align: top;">
                <object classid="clsid:5220cb21-c88d-11cf-b347-00aa00a28331" viewastext>
                    <param name="LPKPath" value="DynamicWebTwain.lpk" />
                </object>
                <object classid="clsid:E7DA7F8D-27AB-4EE9-8FC0-3FEC9ECFE758" id="DynamicWebTwain1"
                    width="250" height="350" codebase="DynamicWebTwain.cab#version=8,0" style="border: 1px solid black; margin-top: 5px;">
                    <param name="_cx" value="847" />
                    <param name="_cy" value="847" />
                    <!--<param name="JpgQuality" value="80">-->
                    <param name="Manufacturer" value="DynamSoft Corporation" />
                    <param name="ProductFamily" value="Dynamic Web TWAIN" />
                    <param name="ProductName" value="Dynamic Web TWAIN" />
                    <param name="VersionInfo" value="Dynamic Web TWAIN 8.0" />
                    <param name="TransferMode" value="0" />
                    <param name="BorderStyle" value="1" />
                    <param name="FTPUserName" value />
                    <param name="FTPPassword" value />
                    <param name="FTPPort" value="21" />
                    <param name="HTTPUserName" value />
                    <param name="HTTPPassword" value />
                    <param name="HTTPPort" value="80" />
                    <param name="ProxyServer" value />
                    <param name="IfDisableSourceAfterAcquire" value="0" />
                    <param name="IfThrowException" value="1" />
                </object>
            </td>
            <td style="text-align: center; vertical-align: top;">
                <div style="background-color: Silver; margin-top: 5px; padding: 10px; text-align: left; width: 440px;">
                    <span style="font-family: Verdana; font-size: large; font-weight: bold">Impostazioni
                        avanzate
                    </span>
                    <div id="divScannerConfiguration" runat="server">
                        Seleziona configurazione:
                        &nbsp;
                        <asp:DropDownList ID="ddlScannerConfiguration" runat="server" AutoPostBack="true" />
                    </div>
                    <hr />
                    <asp:UpdatePanel ID="updScannerConfiguration" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlScannerConfiguration" EventName="SelectedIndexChanged" />
                        </Triggers>
                        <ContentTemplate>
                            Risoluzione:
                            &nbsp;
                            <asp:DropDownList ID="ddlResolution" runat="server">
                                <asp:ListItem Text="100 dpi" Value="100" />
                                <asp:ListItem Text="150 dpi" Value="150" />
                                <asp:ListItem Text="200 dpi" Value="200" Selected="True" />
                                <asp:ListItem Text="300 dpi" Value="300" />
                            </asp:DropDownList>
                            <hr />
                            Colore acquisizione:
                            <br />
                            <asp:RadioButtonList ID="rblColor" RepeatDirection="Horizontal" runat="server">
                                <asp:ListItem Selected="True" Text="Bianco/Nero" Value="V15" />
                                <asp:ListItem Text="Scala di Grigi" Value="V13" />
                                <asp:ListItem Text="Colori" Value="V14" />
                            </asp:RadioButtonList>
                            <hr />
                            <asp:CheckBox ID="chkShowUI" runat="server" Text="Visualizza interfaccia" />
                            <br />
                            <asp:CheckBox ID="chkADF" runat="server" Text="Attiva ADF" Checked="true" />
                            <br />
                            <asp:CheckBox ID="chkDuplex" runat="server" Text="Scansione Fronte/Retro" />
                            <br />
                            <hr />
                            <asp:CheckBox ID="chkDiscardBlank" runat="server" Text="Elimina pagine bianche" />
                            <br />
                            Sensibilità riconoscimento pagina bianca (0-255):
                            &nbsp;
                            <telerik:RadNumericTextBox ID="txtThreshold" MaxLength="3" MaxValue="255" MinValue="0" runat="server" Value="128" Width="30px">
                                <NumberFormat AllowRounding="true" DecimalDigits="0" GroupSeparator="" KeepNotRoundedValue="false" />
                            </telerik:RadNumericTextBox>
                            <br />
                            <hr />
                            Numero massimo di pagine per file:
                            <asp:Label Font-Bold="True" ID="maxPagesToScan" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <table class="dataform">
        <tr>
            <td>
                <telerik:RadButton ButtonType="StandardButton" ID="btnUpload" runat="server" Text="Conferma" />
                &nbsp;
                Nome del file:
                &nbsp;
                <telerik:RadTextBox runat="server" ID="fileName" EmptyMessage="Immagine_da_scanner"></telerik:RadTextBox>
                <asp:RegularExpressionValidator runat="server" ValidationGroup="file" ControlToValidate="fileName" ID="FileNameExpressionValidation" ValidationExpression="^[\w\-. ]+$" ErrorMessage="Sono stati inseriti dei caratteri non validi." />
            </td>
        </tr>
    </table>
</asp:Content>
