<%@ Page Title="Firma documento" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="SingleSign.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SingleSign" %>

<%@ Register Src="~/Viewers/ViewerLight.ascx" TagPrefix="uc1" TagName="ViewerLight" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock runat="server">
        <script type="text/javascript">
            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript">
            // Timeout di login in minuti
            var timeout = 30;
            var isRemoteSignEnabled = '<%=ProtocolEnv.RemoteSignEnabled %>';
            // Controlla che sia avvenuto il login e lo memorizza in pagina tra i postback
            function isLogged() {
                var loginTimeField = $get("<%=loginTime.ClientID%>");
                var loginTime = new Date();
                loginTime.setTime(!loginTimeField.value || loginTimeField.value.length === 0 ? 0 : loginTimeField.value);
                var timeLimit = new Date();
                timeLimit.setMinutes(new Date().getMinutes() - timeout);
                return loginTime >= timeLimit;
            }

            // Ritorna il componente di firma
            function getComped() {
                var dsv = $get("signOcx");
                // Controllo presenza e versione Comped
                if (!dsv || !('ShowExceptions' in dsv)) {
                    if (isRemoteSignEnabled == "False") {
                        throw new DigitalSignException("Comped Professional non trovato, verificarne l'installazione.");
                    }
                    return null;
                }
                if (!versionComparer("3, 1, 3, 46", dsv.GetVersionInfo(DigitalSign.enumVerInfoItem.EnumVerInfoVersion))) {
                    throw new DigitalSignException("Versione non supportata.\nVersione minima di Comped Professional 3.1.3.47");
                }
                return dsv;
            }

            function setInvisibility(dsv) {
                // Imposto parametri
                dsv.TBShowChildFrameMain = false;
                dsv.TBShowImageFrame = false;
                dsv.TBShowRtfFrame = false;
                dsv.TBShowTree = false;
                dsv.OpenExternalViewer = false;
                dsv.ShowSecurityWarningOpenExternal = false;
                dsv.EncryptedSourceReadOnly = true;
                dsv.ShowMacroWarning = false;
                dsv.EnableEventLog = true;
                dsv.SaveBase64 = true;
                dsv.ShowExceptions = false;
                dsv.RaiseExceptions = true;
                dsv.UserCanChangeSigningTime = false;

                dsv.visible = false;
            }

            function pageLoad(sender, args) {
                var signTypeDropdown = $find("<%= ToolBar.ClientID %>").findItemByValue('signTypeDropdown').findControl('signTypeDropdown');
                var selectedVal = signTypeDropdown.get_selectedItem().get_value();
                if (selectedVal == "0") {
                    try {
                        var dsv = getComped();
                        if (!args.get_isPartialLoad()) {
                            if (dsv) {
                                setInvisibility(dsv);
                            }
                        }

                        var toolbar = $find("<%= ToolBar.ClientID%>");
                        toolbar.findItemByValue("pinContainer2").set_visible(false);
                        toolbar.findItemByValue("pinContainer3").set_visible(false);
                        if (dsv) {
                            toolbar.findItemByValue("pinContainer2").set_visible(dsv.IsPin2Enabled);
                            toolbar.findItemByValue("pinContainer3").set_visible(dsv.IsPin2Enabled);
                        }
                    }
                    catch (error) {
                        console.log(error);
                        hideAjaxLoadingPanel("<%= ToolBar.ClientID %>");
                    }
                }
            }

            // Imposta il bottone selezionato di default
            function openDefault() {
                setTimeout(function () {
                    try {
                        var toolBar = $find("<%= ToolBar.ClientID%>");
                        // Inibisco l'inserimento password se sono già loggato
                        var pinBox = toolBar.findItemByValue("pinContainer").findControl("pin");
                        if (pinBox != null) {
                            if (isLogged()) {
                                pinBox.disable();
                            } else {
                                pinBox.enable();
                            }
                        }
                        // Scateno il caricamento del file
                        loadSelectedDocument();
                    } catch (exception) {
                        ErrorHandler.logAndShow(exception);
                        return false;
                    }
                    return true;
                }, 0);
            }

            function loadSelectedDocument() {
                var dsv = getComped();
                // Pulisco da eventuale documento precedente
                if (dsv) {
                    dsv.CloseDocument();
                }

                // Carico il documento nel controllo
                var stream = $get("<%= startStream.ClientID%>").value;
                if (stream.length === 0) {
                    throw new GenericException("Impossibile trovare il documento richiesto.");
                }
                if (dsv) {
                    dsv.ContentBuffer = dsv.DecodeBase64StrToByteArray(stream);
                    // Se è un p7m lo carico con la funzione apposita
                    try {
                        if (dsv.P7mVerify()) {
                            dsv.P7mOpenFromBuffer(dsv.DecodeBase64StrToByteArray(stream));
                        }
                    } catch (e) {
                        // lascio caricato il file normalmente
                    }

                    setInvisibility(dsv);

                    // Mostro il risultato
                    dsv.UpdateListPane();

                    setInvisibility(dsv);

                    var dsv$ = $("#signOcx");
                    dsv$.css("display", "inline");
                }
            }

            function sign(toolBar) {
                try {
                    var signTypeDropdown = $find("<%= ToolBar.ClientID %>").findItemByValue('signTypeDropdown').findControl('signTypeDropdown');
                    if (signTypeDropdown.get_selectedItem().get_value() == "0") {
                        var dsv = getComped();
                        // Logon con calcolo del timeout
                        if (!isLogged()) {
                            var pinInput = toolBar.findItemByValue("pinContainer").findControl("pin");
                            var pin = pinInput.get_value();
                            if (!pin || !pin.length) {
                                throw new UserException("Inserire PIN.");
                            }
                            dsv.UnattendedLogon(pin, timeout);
                            if (dsv.IsPin2Enabled) {
                                var pin2Input = toolBar.findItemByValue("pinContainer3").findControl("pin2");
                                var pin2 = pin2Input.get_value();
                                pin2Input.set_value('');
                                if (!pin2 || !pin2.length) {
                                    throw new UserException("Inserire PIN 2.");
                                }
                                dsv.UnattendedLogon2(pin2, timeout);
                            }
                            $get("<%=loginTime.ClientID%>").value = new Date().getTime();
                        }

                        // Scateno il tipo di firma richiesto
                        if (toolBar.findItemByValue("CAdES").get_checked()) {
                            var now = new Date();
                            var signDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getYear() + " " + now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
                            dsv.SetDefaultSigningTime(true, signDate);
                            dsv.ToolBarInvokeCommand(DigitalSign.enumToolBarButtons.Btn_dsv_add_sign);
                            // Check in caso di annullamento
                            if (dsv.P7mIsModified === false) {
                                return false;
                            }

                        } else if (toolBar.findItemByValue("PAdES").get_checked()) {
                            if (toolBar.findItemByValue("pdfConverted").get_enabled() && !toolBar.findItemByValue("pdfConverted").get_checked()) {
                                throw new GenericException("Questa firma è applicabile solo sui PDF.");
                            }
                            var signCount = dsv.PADES_GetSignatureCount();
                            dsv.ToolBarInvokeCommand(DigitalSign.enumToolBarButtons.Btn_dsv_add_pdfsign);
                            // Check in caso di annullamento
                            if (signCount === dsv.PADES_GetSignatureCount()) {
                                return false;
                            }
                        } else {
                            throw new GenericException("Caso non previsto.");
                        }

                        // Mostro il risultato
                        dsv.UpdateListPane();

                        // Salvataggio stream e postback
                        $("#<%= endStream.ClientID%>").val(dsv.P7mBuffer);
                        log("File salvato correttamente.");
                        dsv.CloseDocument();
                        return true;
                    } else {
                        //other than SmartCard
                        return true;
                    }

                } catch (exception) {
                    ErrorHandler.logAndShow(exception);
                    return false;
                }
            }

            function toolbar_clientClicking(sender, args) {
                switch (args.get_item().get_value()) {
                    case "CAdES":
                    case "PAdES":
                        {
                            args.get_item().check();
                            args.set_cancel(true);
                        }
                    case "original":
                    case "pdfConverted":
                        {
                            // Blocco il callback se sono già nella visualizzazione selezionata
                            if (sender.findItemByValue(args.get_item().get_value()).get_checked()) {
                                args.set_cancel(true);
                            }
                            break;
                        }
                    case 'sign':
                        {
                            // Disabilito per un secondo la toolbar per evitare il doppio click
                            sender.set_enabled(false);
                            setTimeout('$find("' + sender.get_id() + '").set_enabled(true);', 1000);
                            // Blocco il postback se la firma non va a buon fine
                            if (!sign(sender)) {
                                var pinInput = toolBar.findItemByValue("pinContainer").findControl("pin");
                                pinInput.set_value('');
                                args.set_cancel(true);
                            }
                            break;
                        }
                }
            }

            function toolbar_clientClicked(sender, args) {
                switch (args.get_item().get_value()) {
                    case "original":
                    case "pdfConverted":
                        {
                            // Eseguo il callback per caricare lo stream desiderato
                            var dsv$ = $("#signOcx");
                            dsv$.css("display", "none");
                            break;
                        }
                }
                return true;
            }

            // Firma PAdES automatica, non rimuovere
            function PAdESSignDefaultPosition(dsv) {
                var now = new Date();
                var signDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getYear() + " " + now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();

                var signObject = {
                    PdfSignPage: -1,                        // ultima pagina
                    PdfSignUseImage: "N",                   // nessuna immagine
                    //PdfSignImage: "",                     // Percorso dell'immagine
                    PdfSignX: 0,                            // posizione immagine dall'alto in millimetri
                    PdfSignY: 0,                            // posizione immagine da sinistra in millimetri
                    PdfSignW: 200,                          // Larghezza immagine in millimetri
                    PdfSignH: 99,                           // Altezza immagine in millimetri
                    //PdfSignReason: $("#SignReason").val(),  // Motivo della firma
                    PdfSignCN: "Y",                         // Aggiunta etichetta Nome
                    PdfSignEmail: "Y",                      // Aggiunta etichetta Email
                    PdfSignCF: "Y",                         // Aggiunta etichetta Codice Fiscale
                    PdfSignRole: "Y",                       // Aggiunta etichetta Ruolo
                    PdfSignValid: "Y",                      // Aggiunta etichetta Validità
                    PdfSignIssue: "N",                      // Aggiunta etichetta Issue
                    PdfSignTime: "N"                        // Aggiunta etichetta Ora e Data Firma
                    //PdfSignTimeType: 1,                     // Specifica il tipo di riferimento temporale da usare per la firma: 1 = signingTime, 2 = marca temporale
                    //PdfSignDateTime: signDate,              // Stringa che codifica la data ed ora (formato locale Windows) per l’attributo signingTime
                    //PdfSignTSP: 0                           // Valore numerico che individua l’indice dell’account di marcatura temporale da usare: 0 = account predefinito, 1 = primao account, ecc.
                };

                var signProperties = "";
                for (var property in signObject) {
                    signProperties += property + "=" + signObject[property].toString() + "\n";
                }
                var result = dsv.PADES_AddSignature(signProperties);

                //var a = 0, b = 0, c = 0;
                //var prova = dsv.GetLastError(a, b, c);

                dsv.UpdateListPane();
            }

            function signTypeDropdown_SelectedIndexChanged(sender, eventArgs) {
                document.getElementById("<%= PinTextbox.ClientID %>").value = "";
                showAjaxLoadingPanel("<%= ToolBar.ClientID %>");
                var selectedValue = sender._selectedValue;//eventArgs.get_item().get_value();
                showHideContainers(selectedValue);

            }

            function toolBar_OnClientLoad(sender, args) {
                var dropdown = $find("<%= ToolBar.ClientID %>").findItemByValue('signTypeDropdown').findControl('signTypeDropdown');
                var selectedValue = dropdown.get_selectedItem().get_value();
                showHideContainers(selectedValue);

            }

            function signTypeDropdown_OnClientLoad(sender) {
                setTimeout(function () {
                    var selectedValue = sender._selectedValue;
                    showHideContainers(selectedValue);
                }, 500)

            }

            function showHideContainers(selectedValue) {
                var toolBar = $find("<%= ToolBar.ClientID %>");
                if (toolBar != null)
                    switch (selectedValue) {
                        case "0": //CARD
                            {
                                toolBar.findItemByValue("pinText").set_visible(true);
                                toolBar.findItemByValue("pinContainer").set_visible(true);
                                toolBar.findItemByValue("pinContainer2").set_visible(false);
                                toolBar.findItemByValue("requestOtp").set_visible(false);
                                toolBar.findItemByValue("otpContainer").set_visible(false);
                                toolBar.findItemByValue("otpContainer2").set_visible(false);
                                toolBar.findItemByValue("PAdES").set_visible(true);
                                break;
                            }
                        case "1": //ARUBA REMOTE
                            {
                                toolBar.findItemByValue("pinText").set_visible(true);
                                toolBar.findItemByValue("pinContainer2").set_visible(false);
                                toolBar.findItemByValue("requestOtp").set_visible(true);
                                toolBar.findItemByValue("otpContainer").set_visible(true);
                                toolBar.findItemByValue("otpContainer2").set_visible(true);
                                toolBar.findItemByValue("PAdES").set_visible(false);
                                toolBar.findItemByValue("PAdES").unCheck();
                                toolBar.findItemByValue("CAdES").check();
                                break;
                            }
                        case "2": //INFOCERT REMOTE
                            {
                                toolBar.findItemByValue("pinText").set_visible(true);
                                toolBar.findItemByValue("pinContainer2").set_visible(false);
                                toolBar.findItemByValue("requestOtp").set_visible(true);
                                toolBar.findItemByValue("otpContainer").set_visible(true);
                                toolBar.findItemByValue("otpContainer2").set_visible(true);
                                toolBar.findItemByValue("PAdES").set_visible(true);
                                toolBar.findItemByValue("PAdES").unCheck();
                                toolBar.findItemByValue("CAdES").check();
                                break;
                            }
                        case "3": //ARUBA AUTO
                            {
                                toolBar.findItemByValue("pinText").set_visible(false);
                                toolBar.findItemByValue("pinContainer").set_visible(true);
                                toolBar.findItemByValue("pinContainer2").set_visible(true);
                                toolBar.findItemByValue("requestOtp").set_visible(false);
                                toolBar.findItemByValue("otpContainer").set_visible(false);
                                toolBar.findItemByValue("otpContainer2").set_visible(false);
                                toolBar.findItemByValue("PAdES").set_visible(false);
                                toolBar.findItemByValue("PAdES").unCheck();
                                toolBar.findItemByValue("CAdES").check();
                                break;
                            }
                        case "4": //INFOCERT AUTO
                            {
                                toolBar.findItemByValue("pinText").set_visible(false);
                                toolBar.findItemByValue("pinContainer").set_visible(true);
                                toolBar.findItemByValue("pinContainer2").set_visible(true);
                                toolBar.findItemByValue("requestOtp").set_visible(false);
                                toolBar.findItemByValue("otpContainer").set_visible(false);
                                toolBar.findItemByValue("otpContainer2").set_visible(false);
                                toolBar.findItemByValue("PAdES").set_visible(true);
                                toolBar.findItemByValue("PAdES").unCheck();
                                toolBar.findItemByValue("CAdES").check();
                                break;
                            }
                    }
            }

            function showAjaxLoadingPanel(htmlElementId) {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>");
                ajaxLoadingPanel.show(htmlElementId);
            }

            function hideAjaxLoadingPanel(htmlElementId) {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>");
                ajaxLoadingPanel.hide(htmlElementId);
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:HiddenField runat="server" ID="loginTime" />
    <asp:HiddenField runat="server" ID="serializedDocumentSource" />
    <asp:HiddenField runat="server" ID="startStream" />
    <asp:HiddenField runat="server" ID="endStream" />

    <asp:Panel ID="signContainer" runat="server">
        <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" OnClientButtonClicking="toolbar_clientClicking" OnClientButtonClicked="toolbar_clientClicked" runat="server" Width="100%">
            <Items>
                <telerik:RadToolBarButton Value="signTypeDropdown">
                    <ItemTemplate>
                        <telerik:RadDropDownList runat="server" ID="signTypeDropdown" Width="140px" AutoPostBack="true" DropDownHeight="200px" OnClientLoad="signTypeDropdown_OnClientLoad" OnClientSelectedIndexChanged="signTypeDropdown_SelectedIndexChanged" OnSelectedIndexChanged="signTypeDropdown_SelectedIndexChanged">
                            <Items>
                                <telerik:DropDownListItem Selected="true" Text="Smartcard" Value="0" />
                                <telerik:DropDownListItem Text="Remota Aruba" Value="1" />
                                <telerik:DropDownListItem Text="Remota Infocert" Value="2" />
                                <telerik:DropDownListItem Text="Automatica Aruba" Value="3" />
                                <telerik:DropDownListItem Text="Automatica Infocert" Value="4" />
                            </Items>
                        </telerik:RadDropDownList>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton IsSeparator="true" Value="signTypeSeparator" />
                <telerik:RadToolBarButton>
                    <ItemTemplate>
                        <span class="templateText">File:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton CheckOnClick="true" CommandName="original" Group="type" Text="Originale" ToolTip="Firma il documento originale" Value="original" />
                <telerik:RadToolBarButton CheckOnClick="true" CommandName="pdfConverted" Group="type" Text="PDF" Checked="True" ToolTip="Firma il documento convertito in pdf" Value="pdfConverted" />
                <telerik:RadToolBarButton IsSeparator="true" />

                <telerik:RadToolBarButton>
                    <ItemTemplate>
                        <span class="templateText">Tipo:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton CheckOnClick="true" Group="file" Text="CAdES" ToolTip="Firma il file trasformandolo in un P7M" Value="CAdES" />
                <telerik:RadToolBarButton CheckOnClick="true" Group="file" Text="PAdES" Checked="True" ToolTip="Firma PDF che non cambia l'estensione del File" Value="PAdES" />
                <telerik:RadToolBarButton IsSeparator="true" />

                <telerik:RadToolBarButton Value="pinText">
                    <ItemTemplate>
                        <span class="templateText">Pin:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton Value="pinContainer2">
                    <ItemTemplate>
                        <span class="templateText">Pin:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton Value="pinContainer">
                    <ItemTemplate>
                        <telerik:RadTextBox ID="pin" runat="server" TextMode="Password" Width="75px" />
                    </ItemTemplate>
                </telerik:RadToolBarButton>

                <telerik:RadToolBarButton CommandName="requestOtp" Text="Richiedi OTP" ToolTip="Richiesta OTP" Value="requestOtp" />
                <telerik:RadToolBarButton Value="otpContainer">
                    <ItemTemplate>
                        <span class="templateText">OTP:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton Value="otpContainer2">
                    <ItemTemplate>
                        <telerik:RadTextBox ID="proxyOtp" runat="server" TextMode="Password" Width="85px" />
                    </ItemTemplate>
                </telerik:RadToolBarButton>

                <telerik:RadToolBarButton CommandName="sign" Text="Firma" ToolTip="Firma il documento visualizzato." Value="sign" ImageUrl="~/App_Themes/DocSuite2008/imgset16/text_signature.png" />

            </Items>
        </telerik:RadToolBar>

        <object classid="clsid:CBBABF89-D183-11D2-819C-00001C011F1D" style="height: 1px; width: 1px; border: none 0 transparent;" id="signOcx" name="signOcx">
        </object>
    </asp:Panel>
    <asp:Panel runat="server" Width="100%" Style="overflow: hidden; height: 100%;">
        <uc1:ViewerLight runat="server" ID="uscViewerLight" ToolBarVisible="false" />
    </asp:Panel>
</asp:Content>
