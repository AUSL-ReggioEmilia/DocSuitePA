<%@ Page AutoEventWireup="false" CodeBehind="ScannerRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ScannerRest" Language="vb" %>

<!DOCTYPE html>
<html>

<head>
    <title>Scansione </title>

    <link rel="stylesheet" href="<%=Page.ResolveUrl("~/Content/scanner.css")%>" />

    <script type="text/javascript" src="<%=Page.ResolveUrl("~/Scripts/dynamsoft/v" + ProtocolEnv.DynamsoftTwainVersion.ToString() + "/dynamsoft.webtwain.config.js")%>"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~/Scripts/dynamsoft/v" + ProtocolEnv.DynamsoftTwainVersion.ToString() + "/dynamsoft.webtwain.initiate.js")%>"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~/Scripts/dynamsoft/v" + ProtocolEnv.DynamsoftTwainVersion.ToString() + "/addon/dynamsoft.webtwain.addon.pdf.js")%>"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/jquery-3.5.1.js")%>"></script>

</head>

<body>
    <div id="wrapper">
        <div id="demoContent">
            <div id="dwtScanDemo">
                <div id="DWTcontainer" class="container">
                    <div id="DWTcontainerTop">
                        <div id="divEdit">
                            <ul class="operateGrp">
                                <li><img class="menuIcon" style="margin-left: 5px;" src="../App_Themes/DocSuite2008/Images/scanner/RemoveSelectedImages.png" title="Rimuovi pagina corrente" alt="Rimuovi immagine corrente" id="DW_btnRemoveCurrentImage" onclick="btnRemoveCurrentImage_onclick();" /></li>
                                <li><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/RemoveAllImages.png" title="Rimuovi tutte le immagini" alt="Rimuovi tutte le immagini" id="DW_btnRemoveAllImages" onclick="btnRemoveAllImages_onclick();" /></li>
                                <li style="width:90px;"></li>
                                <li style="width:50px;line-height: 38px;"><input type="text" size="2" id="DW_CurrentImage" readonly="readonly" />/<input type="text" size="2" id="DW_TotalImage" readonly="readonly" /></li>
                                <li style="width:110px;border-left: 1px solid #4A4A4A;border-right: solid 1px #4A4A4A;">
                                    <img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/ZoomOut.png" title="ZoomOut" alt="Erase" id="btnZoomOut" onclick="btnZoomOut_onclick();" />
                                    <input type="text" id="DW_spanZoom" readonly="readonly" />
                                    <img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/ZoomIn.png" title="ZoomIn" alt="ZoomIn" id="btnZoomIn" onclick="btnZoomIn_onclick();" />
                                </li>
                                <li><img style="margin-left: 10px;" class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/Orig_size.png" title="1:1" alt="1:1" id="btnOrigSize" onclick="btnOrigSize_onclick();" /><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/FitWindow.png" title="Fit To Window" alt="Fit To Window" id="btnFitWindow" style="display:none" onclick="btnFitWindow_onclick()" /></li>
                                <li style="width:50px;"></li>
                                <li><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/RotateLeft.png" title="Ruota a sinistra" alt="Ruota a sinistra" id="btnRotateL" onclick="btnRotateLeft_onclick()" /> </li>
                                <li><img class="menuIcon grayimg" src="../App_Themes/DocSuite2008/Images/scanner/Crop.png" title="Selezionare l'area da ritagliare." alt="Selezionare l'area da ritagliare." id="btnCropGray" /><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/Crop.png" title="Ritaglia" alt="Ritaglia" id="btnCrop" style="display:none" onclick="btnCrop_onclick();" /></li>
                                <li><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/ShowEditor.png" title="Visualizza editor immagine" alt="Visualizza editor immagine" id="btnShowImageEditor" onclick="btnShowImageEditor_onclick();" /></li>
                                <li><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/Select_selected.png" title="Seleziona" alt="Seleziona" id="btnSelect_selected" /><img class="menuIcon" style="display:none;" src="../App_Themes/DocSuite2008/Images/scanner/Select.png" title="Select" alt="Select" id="btnSelect" onclick="btnSelect_onclick();" /></li>
                                <li><img class="menuIcon" style="display:none;" src="../App_Themes/DocSuite2008/Images/scanner/Hand_selected.png" title="Mano" alt="Mano" id="btnHand_selected" /><img class="menuIcon" src="../App_Themes/DocSuite2008/Images/scanner/Hand.png" title="Mano" alt="Mano" id="btnHand" onclick="btnHand_onclick();" /></li>
                            </ul>
                        </div>
                        <div id="dwtcontrolContainer"></div>
                    </div>
                    <div id="ScanWrapper">
                        <div id="divScanner" class="divinput">
                            <ul class="PCollapse">
                                <li>
                                    <div class="divType">
                                        Scansione personalizzata
                                    </div>
                                    <div id="div_ScanImage" class="divTableStyle">
                                        <ul id="ulScaneImageHIDE">
                                            <li>
                                                <label for="source">
                                                    <p>Seleziona la fonte:</p>
                                                </label>
                                                <select size="1" id="source" style="position:relative;" onchange="source_onchange()">
                                                    <option value=""></option>
                                                </select>
                                            </li>
                                            <li id="divProductDetail"></li>
                                            <li>
                                                <input id="btnScan" class="btnScanGray" disabled="disabled" type="button" value="Scansiona" onclick="acquireImage();" />
                                            </li>
                                        </ul>
                                        <div id="tblLoadImage" style="visibility:hidden;">
                                            <a href="javascript: void(0)" class="ClosetblLoadImage"><img class="imgClose" src="../App_Themes/DocSuite2008/Images/scanner/Close.png" alt="Close tblLoadImage" /></a>
                                            <img src="../App_Themes/DocSuite2008/Images/scanner/Warning.png" />
                                            <span class="spanContent">
                                                <p class="contentTitle">No TWAIN compatible drivers detected</p>
                                                <p class="contentDetail">You can Install a Virtual Scanner:</p>
                                                <p class="contentDetail"><a id="samplesource32bit" href="https://download.dynamsoft.com/tool/twainds.win32.installer.2.1.3.msi">32-bit Sample Source</a> <a id="samplesource64bit" style="display:none;" href="https://download.dynamsoft.com/tool/twainds.win64.installer.2.1.3.msi">64-bit Sample Source</a> from <a target="_blank" href="http://www.twain.org">TWG</a></p>
                                            </span>
                                        </div>
                                    </div>
                                </li>
                            </ul>
                        </div>
                        <div id="divUpload" class="divinput">
                            <ul class="PCollapse">
                                <li>
                                    <div class="divType">
                                        Documenti
                                    </div>
                                    <div  id="divSaveDetail" class="divTableStyle">
                                        <ul>
                                            <li>
                                                <p>Nome del file:</p>
                                                <input type="text" size="20" id="txt_fileName" /><span> . </span>
                                                <select size="1" id="fileType" style="position:relative;width: 25%;" onchange="fileType_onchange();">
                                                    <option value=""></option>
                                                </select>
                                            </li>
                                            <li>
                                                Pagine:
                                                <label for='CurrentPage' style='margin-left:5px;'><input type='radio' id='CurrentPage' name='Pages' />Pagina corrente</label>
                                                <label for='AllPages'><input type='radio' id='AllPages' name='Pages' />Tutte le pagine</label>
                                            </li>
                                            <li id="html_btnSave" style="display: none">
                                                <input id="btnSave" class="btnScanGray" type="button" value="Memorizza" onclick="encodeImages(false)" />
                                                <div id="scan-list" class="clearfix">
                                                    <ul id="scan-items">
                                                    </ul>
                                                </div>
                                            </li>
                                            <li>
                                                <input id="btnConferma" class="btnScanGray" type="button" value="Conferma" onclick="OnClientClose()" />
                                            </li>
                                        </ul>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        var encodedScans = [];
        var currentIndex = -1;
        var multipleEnabled = "<%= MultipleEnabled %>";
        var fileExtension = ".pdf";
        var DWObject;            // The DWT Object
        var DWTSourceCount = 0;
        var _iLeft, _iTop, _iRight, _iBottom, bNotShowMessageAgain = false; //These variables are used to remember the selected area
        var dynamsoftTwainVersion = <%=ProtocolEnv.DynamsoftTwainVersion%>;

        function OnClientClose() {
            encodeImages(true);
            return false;
        }

        function IsMultipleUpload() {
            return multipleEnabled && (multipleEnabled === "true" || multipleEnabled === "True");
        }

        function encodeImages(needClose) {

            var _txtFileName = document.getElementById("txt_fileName");
            var fileName = processStringTitle(_txtFileName.value);
            if (fileName === "") {
                alert("Nome di scansione richiesto!");
                return false;
            }

            for (var i = 0; i < encodedScans.length; i++) {
                if (encodedScans[i].FileName == fileName + fileExtension) {
                    alert("Nome gia esistente!");
                    return false;
                }
            }
            var totalimg = DWObject.HowManyImagesInBuffer;
            var imagesToScan = [];
            for (var i = 0; i <= totalimg - 1; i++) {
                imagesToScan.push(i);
            }

            var ImageType_IT_PDF = dynamsoftTwainVersion == 17 ? Dynamsoft.DWT.EnumDWT_ImageType.IT_PDF : EnumDWT_ImageType.IT_PDF;
            DWObject.ConvertToBase64(imagesToScan, ImageType_IT_PDF, function (result) {
                this.encodeResult(result, needClose);
            }, this.asyncFailureFunc);
        }

        function encodeResult(result, needClose) {
            var length = result.getLength();
            var encodedScan = result.getData(0, length);

            var _txtFileName = document.getElementById("txt_fileName");

            var fileName = processStringTitle(_txtFileName.value);

            var encodedScanObject = {
                FileName: fileName + fileExtension,
                ContentStream: encodedScan,
                Segnature: ""
            };
            encodedScans.push(encodedScanObject);
            sessionStorage.setItem("component.scanner.upload.scan", JSON.stringify(encodedScans));

            appendScanToList(fileName);
            removeAllImagesForTheNextScan();
            if (needClose) {
                CloseWindow();
            }
        }

        function processStringTitle(fileName) {
            return fileName.replace(/[^A-Za-z0-9]/g, "_");
        }
        function removeAllImagesForTheNextScan() {
            DWObject.RemoveAllImages();
            document.getElementById("DW_TotalImage").value = "0";
            document.getElementById("DW_CurrentImage").value = "";
            var _txtFileName = document.getElementById("txt_fileName");
            _txtFileName.value = "immagine_da_scanner";
            currentIndex = -1;
        }
        function appendScanToList(fileName) {
            $("#scan-items").append("<li id=" + fileName + " class='removable-list-item'>" +
                fileName + " <span class='removable-span-item' onclick='removeScanFromList(" +
                fileName + ")'>&times;</span></li>");
        }

        function removeScanFromList(fileName) {
            var fileNameWithExtension = fileName.id + fileExtension;
            var element = document.getElementById(fileName.id);
            element.parentNode.removeChild(element);
            //element.remove(); /*IE breaks*/
            encodedScans.indexOf(fileNameWithExtension);
            for (var i = 0; i < encodedScans.length; i++) {
                if (encodedScans[i].FileName == fileNameWithExtension) {
                    encodedScans.splice(i, 1);
                }
            }
            if (encodedScans.length <= 0) {
                sessionStorage.removeItem("component.scanner.upload.scan");
            } else {
                sessionStorage.setItem("component.scanner.upload.scan", JSON.stringify(encodedScans));
            }
        }
        function asyncFailureFunc(errorCode, errorString) {
            alert("ErrorCode: " + errorCode + " \r ErrorString: " + errorString + "");
        }
        function CloseWindow() {
            var oWindow = GetRadWindow();  //Obtaining a reference to the current window 
            oWindow.Close();
        }
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
        // Assign the page onload fucntion.
        $(function () {
            pageonload();
        });
        //--------------------------------------------------------------------------------------
        //************************** Import Image*****************************
        //--------------------------------------------------------------------------------------
        /*-----------------select source---------------------*/
        function source_onchange(bWebcam) {

            if (document.getElementById("divTwainType"))
                document.getElementById("divTwainType").style.display = "";

            if (document.getElementById("source")) {
                var cIndex = document.getElementById("source").selectedIndex;
                if (Dynamsoft.Lib.env.bMac) {
                    var strSourceName = DWObject.GetSourceNameItems(cIndex);
                    if (strSourceName.indexOf("ICA") == 0) {
                        if (document.getElementById("lblShowUI"))
                            document.getElementById("lblShowUI").style.display = "none";
                        if (document.getElementById("ShowUI"))
                            document.getElementById("ShowUI").style.display = "none";
                    } else {
                        if (document.getElementById("lblShowUI"))
                            document.getElementById("lblShowUI").style.display = "";
                        if (document.getElementById("ShowUI"))
                            document.getElementById("ShowUI").style.display = "";
                    }
                }
                else
                    DWObject.SelectSourceByIndex(cIndex);
            }
        }


        function mediaType_onchange() {
            var MediaType = document.getElementById("MediaType");
            if (MediaType && MediaType.options.length > 0) {
                valueMediaType = MediaType.options[MediaType.selectedIndex].text;
                if (valueMediaType != "")
                    if (!DWObject.Addon.Webcam.SetMediaType(valueMediaType)) {
                        return;
                    }
            }

            var ResolutionWebcam = document.getElementById("ResolutionWebcam");
            if (ResolutionWebcam) {
                ResolutionWebcam.options.length = 0;
                var aryResolution = DWObject.Addon.Webcam.GetResolution();
                countResolution = aryResolution.GetCount();
                for (i = 0; i < countResolution; i++) {
                    value = aryResolution.Get(i);
                    ResolutionWebcam.options.add(new Option(value, value));
                }
            }
        }
        /*-----------------Acquire Image---------------------*/

        function acquireImage() {
            var cIndex = document.getElementById('source').selectedIndex;
            if (cIndex < 0)
                return;

            DWObject.SelectSourceByIndex(cIndex);
            DWObject.CloseSource();
            DWObject.OpenSource();

            var i, iPixelType = 0;
            for (i = 0; i < 3; i++) {
                if (document.getElementsByName('PixelType').item(i).checked == true)
                    iPixelType = i;
            }
            var deviceConfiguration = {
                IfShowUI: document.getElementById('ShowUI').checked, //false,
                PixelType: iPixelType,
                Resolution: document.getElementById('Resolution').value,
                IfFeederEnabled: document.getElementById('ADF').checked,
                IfDuplexEnabled: document.getElementById('Duplex').checked,
                IfAutoDiscardBlankpages: document.getElementById('DiscardBlankPage').checked,
                IfDisableSourceAfterAcquire: true
            };

            DWObject.AcquireImage(deviceConfiguration, function () {
                checkErrorStringWithErrorCode(0, 'Successful.'); //checkErrorString();
            }, function (obj, errorCode, errorString) {
                checkErrorStringWithErrorCode(errorCode, errorString);
            });
        }

        /*-----------------Download Image---------------------*/
        function btnDownloadImages_onclick() {
            var OnSuccess = function () {
                checkErrorString();

                var divLoadAndDownload = document.getElementById("divLoadAndDownload");
                if (divLoadAndDownload)
                    divLoadAndDownload.parentNode.removeChild(divLoadAndDownload);
            };

            var OnFailure = function (errorCode, errorString) {
                checkErrorStringWithErrorCode(errorCode, errorString);
            };


            DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
            var _strPort = location.port == "" ? 80 : location.port;
            if (Dynamsoft.Lib.detect.ssl == true)
                _strPort = location.port == "" ? 443 : location.port;
            DWObject.HTTPPort = _strPort;
            var CurrentPathName = unescape(location.pathname); // get current PathName in plain ASCII	
            var CurrentPath = CurrentPathName.substring(0, CurrentPathName.lastIndexOf("/") + 1);
            var strDownloadFile = CurrentPath + "Images/DynamsoftSample.pdf";

            DWObject.HTTPDownload(location.hostname, strDownloadFile, OnSuccess, OnFailure);
        }


        /*-----------------Load Image---------------------*/
        function btnLoadImagesOrPDFs_onclick() {

            var OnPDFSuccess = function () {

                checkErrorString();

                var divLoadAndDownload = document.getElementById('divLoadAndDownload');
                if (divLoadAndDownload)
                    divLoadAndDownload.parentNode.removeChild(divLoadAndDownload);
            };

            var OnPDFFailure = function (errorCode, errorString) {
                checkErrorStringWithErrorCode(errorCode, errorString);
            };
            DWObject.IfShowFileDialog = true;
            DWObject.Addon.PDF.SetResolution(200);
            DWObject.Addon.PDF.SetConvertMode(Dynamsoft.DWT.EnumDWT_ConvertMode.CM_AUTO);
            DWObject.LoadImageEx('', Dynamsoft.DWT.EnumDWT_ImageType.IT_ALL, OnPDFSuccess, OnPDFFailure);
        }
        //--------------------------------------------------------------------------------------
        //************************** Edit Image ******************************

        //--------------------------------------------------------------------------------------
        function btnShowImageEditor_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            var imageEditor = DWObject.Viewer.createImageEditor();
            imageEditor.show();
        }

        /*----------------------RotateLeft Method---------------------*/
        function btnRotateLeft_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.RotateLeft(DWObject.CurrentImageIndexInBuffer);
            if (checkErrorString()) {
                return;
            }
        }

        /*----------------------Crop Method---------------------*/
        function btnCrop_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            if (_iLeft != 0 || _iTop != 0 || _iRight != 0 || _iBottom != 0) {
                DWObject.Crop(
                    DWObject.CurrentImageIndexInBuffer,
                    _iLeft, _iTop, _iRight, _iBottom
                );
                _iLeft = 0;
                _iTop = 0;
                _iRight = 0;
                _iBottom = 0;

                if (DWObject.isUsingActiveX())
                    DWObject.SetSelectedImageArea(DWObject.CurrentImageIndexInBuffer, 0, 0, 0, 0);

                if (checkErrorString()) {
                    return;
                }
                return;
            }
        }

        /*----------------------Select Method---------------------*/
        function btnSelect_onclick() {
            handAndSelectSelected(false);

            DWObject.Viewer.cursor = "crosshair";
        }

        function handAndSelectSelected(bHandSelected) {
            var btnHand = document.getElementById("btnHand");
            var btnHand_selected = document.getElementById("btnHand_selected");
            var btnSelect = document.getElementById("btnSelect");
            var btnSelect_selected = document.getElementById("btnSelect_selected");
            if (bHandSelected) {
                if (btnHand)
                    btnHand.style.display = "none";
                if (btnHand_selected)
                    btnHand_selected.style.display = "";
                if (btnSelect)
                    btnSelect.style.display = "";
                if (btnSelect_selected)
                    btnSelect_selected.style.display = "none";
            } else {
                if (btnHand)
                    btnHand.style.display = "";
                if (btnHand_selected)
                    btnHand_selected.style.display = "none";
                if (btnSelect)
                    btnSelect.style.display = "none";
                if (btnSelect_selected)
                    btnSelect_selected.style.display = "";
            }
        }

        /*----------------------Hand Method---------------------*/
        function btnHand_onclick() {
            handAndSelectSelected(true);
            DWObject.Viewer.cursor = "pointer";
        }

        /*----------------------orig_size Method---------------------*/
        function btnOrigSize_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }

            var btnOrigSize = document.getElementById("btnOrigSize");
            if (btnOrigSize)
                btnOrigSize.style.display = "none";
            var btnFitWindow = document.getElementById("btnFitWindow");
            if (btnFitWindow)
                btnFitWindow.style.display = "";

            DWObject.Viewer.zoom = 1;
            updateZoomInfo();
            enableButtonForZoomInAndOut();
        }

        /*----------------------FitWindow Method---------------------*/
        function btnFitWindow_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }

            var btnOrigSize = document.getElementById("btnOrigSize");
            if (btnOrigSize)
                btnOrigSize.style.display = "";
            var btnFitWindow = document.getElementById("btnFitWindow");
            if (btnFitWindow)
                btnFitWindow.style.display = "none";

            DWObject.Viewer.fitWindow();
            updateZoomInfo();
            enableButtonForZoomInAndOut();
        }


        /*----------------------ZoomIn Method---------------------*/
        function enableButtonForZoomInAndOut() {
            var btnZoomIn = $("#btnZoomIn");
            var zoom = Math.round(DWObject.Viewer.zoom * 100);

            if (zoom >= 6500) {
                if (btnZoomIn)
                    btnZoomIn.addClass('grayimg');
                return;
            } else {
                if (btnZoomIn)
                    btnZoomIn.removeClass('grayimg');

                var btnZoomOut = $("#btnZoomOut");
                if (zoom <= 2) {
                    if (btnZoomOut)
                        btnZoomOut.addClass('grayimg');
                    return;
                } else {
                    if (btnZoomOut)
                        btnZoomOut.removeClass('grayimg');
                }
            }
        }

        function btnZoomIn_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }

            var zoom = Math.round(DWObject.Viewer.zoom * 100);
            if (zoom >= 6500)
                return;

            var zoomInStep = 5;
            DWObject.Viewer.zoom = (DWObject.Viewer.zoom * 100 + zoomInStep) / 100.0;
            updateZoomInfo();
            enableButtonForZoomInAndOut();
        }

        /*----------------------ZoomOut Method---------------------*/
        function btnZoomOut_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }

            var zoom = Math.round(DWObject.Viewer.zoom * 100);
            if (zoom <= 2)
                return;

            var zoomOutStep = 5;
            DWObject.Viewer.zoom = (DWObject.Viewer.zoom * 100 - zoomOutStep) / 100.0;
            updateZoomInfo();
            enableButtonForZoomInAndOut();
        }

        //--------------------------------------------------------------------------------------
        //************************** Navigator functions***********************************
        //--------------------------------------------------------------------------------------
        function btnPreImage_wheel() {
            if (DWObject.HowManyImagesInBuffer != 0)
                btnPreImage_onclick()
        }

        function btnNextImage_wheel() {
            if (DWObject.HowManyImagesInBuffer != 0)
                btnNextImage_onclick()
        }

        function btnPreImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.Viewer.previous();
            updatePageInfo();
        }
        function btnNextImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.Viewer.next();
            updatePageInfo();
        }


        function btnRemoveCurrentImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            if (bNotShowMessageAgain) {
                RemoveCurrentImage();
            } else {
                var title = 'Sei sicuro di voler eliminare la pagina selezionata?';
                var ObjString = [
                    '<div class="dynamsoft-dwt-header"></div>',
                    '<div class="dynamsoft-dwt-dlg-title">',
                    title,
                    '</div>'];

                ObjString.push("<div class='dynamsoft-dwt-showMessage'><label class='dynamsoft-dwt-showMessage-detail' for = 'showMessage'><input type='checkbox' id='showMessage'/>Non mostrare più questo messaggio.&nbsp;</label></div>");
                ObjString.push('<div class="dynamsoft-dwt-installdlg-buttons"><input id="btnDelete" class="button-yes" type="button" value="Sì" onclick ="RemoveCurrentImage()"/><input id="btnCancel" class="button-no" type="button" value="No" onclick ="btnCancel_click()"/> </div>');
                Dynamsoft.DWT.ShowDialog(500, 0, ObjString.join(''), true);
            }
        }

        function RemoveCurrentImage() {
            DWObject.RemoveImage(DWObject.CurrentImageIndexInBuffer);
            if (DWObject.HowManyImagesInBuffer == 0)
                DWObject.RemoveImage(0);
            var showMessage = document.getElementById("showMessage");
            if (showMessage && showMessage.checked)
                bNotShowMessageAgain = true;

            updatePageInfo();
            Dynamsoft.DWT.CloseDialog();
        }

        function btnCancel_click() {
            var showMessage = document.getElementById("showMessage");
            if (showMessage && showMessage.checked)
                bNotShowMessageAgain = true;
            Dynamsoft.DWT.CloseDialog();
        }

        function RemoveAllImages() {
            DWObject.RemoveAllImages();
            DWObject.RemoveImage(0);

            Dynamsoft.DWT.CloseDialog();
        }

        function btnRemoveAllImages_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }

            var title = 'Sei sicuro di voler eliminare tutte le immagini?';
            var ObjString = [
                '<div class="dynamsoft-dwt-header"></div>',
                '<div class="dynamsoft-dwt-dlg-title">',
                title,
                '</div>'];

            ObjString.push('<div class="dynamsoft-dwt-installdlg-iconholder"><input id="btnDelete" class="button-yes" type="button" value="Sì" onclick ="RemoveAllImages()"/><input id="btnCancel" class="button-no" type="button" value="No" onclick ="btnCancel_click()"/> </div>');
            Dynamsoft.DWT.ShowDialog(500, 0, ObjString.join(''), true);
        }

        //--------------------------------------------------------------------------------------
        //************************** Dynamic Web TWAIN Events***********************************
        //--------------------------------------------------------------------------------------
        function Dynamsoft_CloseImageEditorUI() {
            updatePageInfo();
        }

        function Dynamsoft_OnBitmapChanged(aryIndex, type) {
            if (type == 3) {
                updatePageInfo();
            }

            if (type == 4)
                updateZoomInfo();

            if (type == 5)  //only ActiveX
                Dynamsoft_OnImageAreaDeselected();
        }

        function Dynamsoft_OnPostTransfer() {
            updatePageInfo();
        }

        function Dynamsoft_OnPostLoadfunction(path, name, type) {
            updatePageInfo();
        }

        function Dynamsoft_OnPostAllTransfers() {
            DWObject.CloseSource();
            updatePageInfo();
        }

        function Dynamsoft_OnMouseClick() {
            updatePageInfo();
        }

        function Dynamsoft_OnMouseWheel() {
            updatePageInfo();
        }

        function Dynamsoft_OnImageAreaSelected(index, rect) {
            if (rect.length > 0) {
                var currentRect = rect[rect.length - 1];
                _iLeft = currentRect.x;
                _iTop = currentRect.y;
                _iRight = currentRect.x + currentRect.width;
                _iBottom = currentRect.y + currentRect.height;

                enableButtonForCrop(true);
            }
        }

        function Dynamsoft_OnImageAreaDeselected(index) {
            _iLeft = 0;
            _iTop = 0;
            _iRight = 0;
            _iBottom = 0;

            enableButtonForCrop(false);
        }

        function Dynamsoft_OnGetFilePath(bSave, count, index, path, name) {

        }

        function Dynamsoft_OnIndexChangeDragDropDone(event) {
            updatePageInfo();
        }

        function Dynamsoft_OnKeyDown() {
            updatePageInfo();
        }

        function showUploadedFiles(bShow) {
            var tabSave = document.getElementById("tabSave");
            var tabUploadedFiles = document.getElementById("tabUploadedFiles");
            var divSaveDetail = document.getElementById("divSaveDetail");
            var divUploadedFiles = document.getElementById("divUploadedFiles");
            if (tabSave && tabUploadedFiles && divSaveDetail && divUploadedFiles) {
                if (bShow) {
                    tabSave.className = "tabList unselectTab";
                    tabUploadedFiles.className = "tabList selectTab";
                    divSaveDetail.style.display = "none";
                    divUploadedFiles.style.display = "block";
                } else {

                    tabSave.className = "tabList selectTab";
                    tabUploadedFiles.className = "tabList unselectTab";
                    divSaveDetail.style.display = "block";
                    divUploadedFiles.style.display = "none";
                }
            }
        }

        function pageonload() {
            Dynamsoft.DWT.ProductKey = "<%=ProtocolEnv.DynamsoftTwainProductKey%>";

            if (sessionStorage.getItem("component.scanner.upload.scan") != null) {
                var scans = JSON.parse(sessionStorage.getItem("component.scanner.upload.scan"));
                for (var i = 0; i < scans.length; i++) {
                    var processedFilename = "";
                    if (scans[i].FileName.indexOf(fileExtension) > 0) {
                        processedFilename = scans[i].FileName.slice(0, scans[i].FileName.length - 4);
                    } else {
                        processedFilename = scans[i].FileName;
                    }
                    appendScanToList(processedFilename);
                    encodedScans.push(scans[i]);
                }
            }

            HideLoadImageForLinux();
            initCustomScan();

            var twainsource = document.getElementById("source");
            if (twainsource) {
                twainsource.options.length = 0;
                twainsource.options.add(new Option("Ricerca dispositivi. Si prega di attendere...", 0));
                twainsource.options[0].selected = true;
            }

            initiateInputs();
        }
        function HideLoadImageForLinux() {
            var btnLoad = document.getElementById("btnLoad");
            if (btnLoad) {
                if (Dynamsoft.Lib.env.bLinux)
                    btnLoad.style.display = "none";
                else
                    btnLoad.style.display = "";
            }

            var btnSave = document.getElementById("btnSave");
            if (btnSave) {
                if (Dynamsoft.Lib.env.bLinux)
                    btnSave.style.display = "none";
                else
                    btnSave.style.display = "";
            }
        }

        function initCustomScan() {
            var ObjString = "";
            ObjString += "<ul id='divTwainType'> ";
            ObjString += "<li><label style='width: 220px;' id ='lblShowUI' for = 'ShowUI'><input type='checkbox' id='ShowUI' />Visualizza interfaccia scanner &nbsp;</label></li>";
            ObjString += "<li><label style='width: 220px;' for = 'DiscardBlankPage'><input type='checkbox' id='DiscardBlankPage'/>Rimozione automatica pagine vuote</label></li>";
            ObjString += "<li><label for = 'ADF'><input type='checkbox' id='ADF' />AutoFeeder&nbsp;</label></li>";
            ObjString += "<li><label for = 'Duplex'><input type='checkbox' id='Duplex'/>Duplex</label></li>";
            ObjString += "<li>Colore:";
            ObjString += "<label for='BW' style='margin-left:5px;' class='lblPixelType'><input type='radio' id='BW' name='PixelType'/>B&amp;N </label>";
            ObjString += "<label for='Gray' class='lblPixelType'><input type='radio' id='Gray' name='PixelType'/>Grigio</label>";
            ObjString += "<label for='RGB' class='lblPixelType'><input type='radio' id='RGB' name='PixelType'/>Colore</label></li>";
            ObjString += "<li>";
            ObjString += "<span>Risoluzione:</span><select size='1' id='Resolution'><option value = ''></option></select></li>";
            ObjString += "</ul>";

            if (document.getElementById("divProductDetail"))
                document.getElementById("divProductDetail").innerHTML = ObjString;

            var vResolution = document.getElementById("Resolution");
            if (vResolution) {
                vResolution.options.length = 0;
                vResolution.options.add(new Option("100", 100));
                vResolution.options.add(new Option("150", 150));
                vResolution.options.add(new Option("200", 200));
                vResolution.options.add(new Option("300", 300));

                vResolution.options[3].selected = true;
            }
        }

        function initiateInputs() {
            if (IsMultipleUpload()) {
                $('#html_btnSave').hide();
            }

            var allinputs = document.getElementsByTagName("input");
            for (var i = 0; i < allinputs.length; i++) {
                if (allinputs[i].type == "checkbox") {
                    allinputs[i].checked = false;
                }
                else if (allinputs[i].type == "text") {
                    allinputs[i].value = "";
                }
            }

            if (Dynamsoft.Lib.env.bIE == true && Dynamsoft.Lib.env.bWin64 == true) {
                var o = document.getElementById("samplesource64bit");
                if (o)
                    o.style.display = "inline";

                o = document.getElementById("samplesource32bit");
                if (o)
                    o.style.display = "none";
            }
        }

        function setDefaultValue() {
            var vBW = document.getElementById("BW");
            if (vBW)
                vBW.checked = true;

            var varImgTypepng2 = document.getElementById("imgTypepng2");
            if (varImgTypepng2)
                varImgTypepng2.checked = true;
            var varImgTypepng = document.getElementById("imgTypepng");
            if (varImgTypepng)
                varImgTypepng.checked = true;

            var _strDefaultSaveImageName = "immagine_da_scanner";
            var _txtFileNameforSave = document.getElementById("txt_fileNameforSave");
            if (_txtFileNameforSave)
                _txtFileNameforSave.value = _strDefaultSaveImageName;

            var _txtFileName = document.getElementById("txt_fileName");
            if (_txtFileName)
                _txtFileName.value = _strDefaultSaveImageName;

            if (document.getElementById("ADF"))
                document.getElementById("ADF").checked = true;
        }

        function initFileType() {
            var fileType = document.getElementById("fileType");
            fileType.options.length = 0;
            fileType.options.add(new Option("pdf", "pdf"));
            //fileType.options.add(new Option("tif", "tif"));
            //fileType.options.add(new Option("jpg", "jpg"));
            //fileType.options.add(new Option("png", "png"));
            //fileType.options.add(new Option("bmp", "bmp"));

            fileType.selectedIndex = 0;
            fileType.style.visibility = "hidden"
            var vAllPages = document.getElementById("AllPages");
            if (vAllPages)
                vAllPages.checked = true;
        }

        // Check if the control is fully loaded.
        function Dynamsoft_OnReady() {

            var liNoScanner = document.getElementById("pNoScanner");
            // If the ErrorCode is 0, it means everything is fine for the control. It is fully loaded.
            DWObject = Dynamsoft.DWT.GetWebTwain('dwtcontrolContainer');
            if (DWObject) {
                if (DWObject.ErrorCode == 0) {
                    var thumbnailViewer = DWObject.Viewer.createThumbnailViewer();
                    thumbnailViewer.size = "180px";
                    thumbnailViewer.showPageNumber = true;
                    thumbnailViewer.selectedPageBackground = thumbnailViewer.background;
                    thumbnailViewer.selectedPageBorder = "solid 2px #FE8E14";
                    thumbnailViewer.hoverPageBorder = "solid 2px #FE8E14";
                    thumbnailViewer.placeholderBackground = "#D1D1D1";
                    thumbnailViewer.show();
                    thumbnailViewer.hoverPageBackground = thumbnailViewer.background;
                    thumbnailViewer.on("click", Dynamsoft_OnMouseClick);
                    thumbnailViewer.on('dragdone', Dynamsoft_OnIndexChangeDragDropDone);
                    thumbnailViewer.on("keydown", Dynamsoft_OnKeyDown);
                    DWObject.Viewer.on("wheel", Dynamsoft_OnMouseWheel);  //H5 only
                    DWObject.Viewer.on("OnPaintDone", Dynamsoft_OnMouseWheel);   //ActiveX only

                    DWObject.Viewer.allowSlide = false;
                    $('#DWTNonInstallContainerID').hide();

                    DWObject.IfAllowLocalCache = true;
                    DWObject.ImageCaptureDriverType = 4;
                    setDefaultValue();
                    initFileType();

                    var twainsource = document.getElementById("source");
                    if (!twainsource) {
                        twainsource = document.getElementById("webcamsource");
                    }

                    var vCount = DWObject.SourceCount;
                    DWTSourceCount = vCount;
                    var strSourceName = "";

                    if (twainsource) {
                        twainsource.options.length = 0;
                        for (var i = 0; i < vCount; i++) {
                            twainsource.options.add(new Option(DWObject.GetSourceNameItems(i), i));
                            if (i > 0)
                                strSourceName = strSourceName + ";" + DWObject.GetSourceNameItems(i);
                            else
                                strSourceName = DWObject.GetSourceNameItems(i);
                        }
                    }

                    ua = (navigator.userAgent.toLowerCase());
                    if (vCount == 0) {
                        var mips64 = (/mips64/g).test(ua);
                        if (!mips64)
                            btnDownloadImages_onclick();
                    }

                    // If source list need to be displayed, fill in the source items.
                    if (vCount == 0) {
                        if (liNoScanner) {
                            if (Dynamsoft.Lib.env.bWin) {

                                liNoScanner.style.display = "block";
                                liNoScanner.style.textAlign = "center";
                            }
                            else
                                liNoScanner.style.display = "none";
                        }
                    }

                    if (vCount > 0) {
                        source_onchange(false);
                    }

                    var btnScan = document.getElementById("btnScan");
                    if (btnScan) {
                        if (vCount == 0)
                            document.getElementById("btnScan").disabled = true;
                        else {
                            document.getElementById("btnScan").disabled = false;
                            var btnScan = $("#btnScan");
                            if (btnScan)
                                btnScan.addClass('btnScanActive');

                            var btnConferma = $("#btnConferma");
                            if (btnConferma)
                                btnConferma.addClass('btnScanActive');

                            var btnSave = $("#btnSave");
                            if (btnSave)
                                btnSave.addClass('btnScanActive');
                        }
                    }


                    if (!Dynamsoft.Lib.env.bWin && vCount > 0) {
                        if (document.getElementById("lblShowUI"))
                            document.getElementById("lblShowUI").style.display = "none";
                        if (document.getElementById("ShowUI"))
                            document.getElementById("ShowUI").style.display = "none";
                    }
                    else {
                        if (document.getElementById("lblShowUI"))
                            document.getElementById("lblShowUI").style.display = "";
                        if (document.getElementById("ShowUI"))
                            document.getElementById("ShowUI").style.display = "";
                    }

                    if (document.getElementById("ddl_barcodeFormat")) {
                        for (var index = 0; index < BarcodeInfo.length; index++)
                            document.getElementById("ddl_barcodeFormat").options.add(new Option(BarcodeInfo[index].desc, index));
                        document.getElementById("ddl_barcodeFormat").selectedIndex = 0;
                    }

                    re = /^\d+$/;
                    strre = /^[\s\w]+$/;
                    refloat = /^\d+\.*\d*$/i;

                    _iLeft = 0;
                    _iTop = 0;
                    _iRight = 0;
                    _iBottom = 0;

                    for (var i = 0; i < document.links.length; i++) {
                        if (document.links[i].className == "ClosetblLoadImage") {
                            document.links[i].onclick = closetblLoadImage_onclick;
                        }
                    }
                    if (vCount == 0) {
                        if (Dynamsoft.Lib.env.bWin) {

                            if (window['bDWTOnlineDemo']) {
                                if (document.getElementById("div_ScanImage").style.display == "")
                                    showtblLoadImage_onclick();
                            }
                            if (document.getElementById("Resolution"))
                                document.getElementById("Resolution").style.display = "none";

                        }
                    }
                    else {
                        var divBlank = document.getElementById("divBlank");
                        if (divBlank)
                            divBlank.style.display = "none";
                    }

                    updatePageInfo();
                    if (!ua.indexOf("msie 6.0")) {
                        ShowSiteTour();
                    }

                    DWObject.RegisterEvent('CloseImageEditorUI', Dynamsoft_CloseImageEditorUI);
                    DWObject.RegisterEvent('OnBitmapChanged', Dynamsoft_OnBitmapChanged);
                    DWObject.RegisterEvent("OnPostTransfer", Dynamsoft_OnPostTransfer);
                    DWObject.RegisterEvent("OnPostLoad", Dynamsoft_OnPostLoadfunction);
                    DWObject.RegisterEvent("OnPostAllTransfers", Dynamsoft_OnPostAllTransfers);
                    DWObject.RegisterEvent("OnGetFilePath", Dynamsoft_OnGetFilePath);
                    DWObject.Viewer.on("pageAreaSelected", Dynamsoft_OnImageAreaSelected);
                    DWObject.Viewer.on("pageAreaUnselected", Dynamsoft_OnImageAreaDeselected);
                }
            }

            if (typeof (window['start_init_dcs']) == 'function') {
                window['start_init_dcs']();
            }
        }


        function showtblLoadImage_onclick() {
            switch (document.getElementById("tblLoadImage").style.visibility) {
                case "hidden": document.getElementById("tblLoadImage").style.visibility = "visible";
                    document.getElementById("Resolution").style.visibility = "hidden";
                    break;
                case "visible":
                    document.getElementById("tblLoadImage").style.visibility = "hidden";
                    document.getElementById("Resolution").style.visibility = "visible";
                    break;
                default: break;
            }

            return false;
        }

        function closetblLoadImage_onclick() {
            document.getElementById("tblLoadImage").style.visibility = "hidden";
            document.getElementById("Resolution").style.visibility = "visible";
            return false;
        }

        //--------------------------------------------------------------------------------------
        //************************** Used a lot *****************************
        //--------------------------------------------------------------------------------------
        function updatePageInfo() {
            if (document.getElementById("DW_TotalImage"))
                document.getElementById("DW_TotalImage").value = DWObject.HowManyImagesInBuffer;
            if (document.getElementById("DW_CurrentImage"))
                document.getElementById("DW_CurrentImage").value = DWObject.CurrentImageIndexInBuffer + 1;
            updateZoomInfo();
        }

        function updateZoomInfo() {
            if (document.getElementById("DW_spanZoom")) {
                if (DWObject.HowManyImagesInBuffer == 0)
                    document.getElementById("DW_spanZoom").value = "100%";
                else
                    document.getElementById("DW_spanZoom").value = Math.round(DWObject.Viewer.zoom * 100) + "%";
            }
        }

        function checkIfImagesInBuffer() {
            if (DWObject.HowManyImagesInBuffer == 0) {
                return false;
            }
            else
                return true;
        }

        function checkErrorString() {
            return checkErrorStringWithErrorCode(DWObject.ErrorCode, DWObject.ErrorString);
        }

        function checkErrorStringWithErrorCode(errorCode, errorString, responseString) {
            if (errorCode == 0) {
                return true;
            }
            if (errorCode == -2115) //Cancel file dialog
                return true;
            else {
                if (errorCode == -2003) {
                    var ErrorMessageWin = window.open("", "ErrorMessage", "height=500,width=750,top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no");
                    ErrorMessageWin.document.writeln(responseString); //DWObject.HTTPPostResponseString);
                }
                return false;
            }
        }

        function enableButtonForCrop(bEnable) {
            if (bEnable) {
                var btnCrop = document.getElementById("btnCrop");
                if (btnCrop)
                    btnCrop.style.display = "";
                var btnCropGray = document.getElementById("btnCropGray");
                if (btnCropGray)
                    btnCropGray.style.display = "none";
            } else {
                var btnCrop = document.getElementById("btnCrop");
                if (btnCrop)
                    btnCrop.style.display = "none";
                var btnCropGray = document.getElementById("btnCropGray");
                if (btnCropGray)
                    btnCropGray.style.display = "";
            }
        }

        function showCustomInfo() {
            var customDetail = document.getElementById("customDetail");
            customDetail.style.display = "";
        }

        function hideCustomInfo() {
            var customDetail = document.getElementById("customDetail");
            customDetail.style.display = "none";
        }

        function showUploadedFilesDetail() {
            var customDetail = document.getElementById("uploadedFilesDetail");
            customDetail.style.display = "";
        }

        function hideUploadedFilesDetail() {
            var customDetail = document.getElementById("uploadedFilesDetail");
            customDetail.style.display = "none";
        }
        //--------------------------------------------------------------------------------------
        //************************** Used a lot *****************************
        //--------------------------------------------------------------------------------------
        function ds_getleft(el) {
            var tmp = el.offsetLeft;
            el = el.offsetParent
            while (el) {
                tmp += el.offsetLeft;
                el = el.offsetParent;
            }
            return tmp;
        }

        function ds_gettop(el) {
            var tmp = el.offsetTop;
            el = el.offsetParent
            while (el) {
                tmp += el.offsetTop;
                el = el.offsetParent;
            }
            return tmp;
        }

        function Over_Out_DemoImage(obj, url) {
            obj.src = url;
        }


    </script>
</body>

</html>
