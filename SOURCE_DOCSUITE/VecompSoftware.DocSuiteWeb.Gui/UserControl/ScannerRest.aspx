<%@ Page AutoEventWireup="false" CodeBehind="ScannerRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ScannerRest" Language="vb" %>
<!DOCTYPE html>
<html>

<head>
    <title>Scansione </title>

    <link  rel="stylesheet" href="<%=Page.ResolveUrl("~/Content/scanner.css")%>" />

    <script type="text/javascript" src="<%=Page.ResolveUrl("~/Scripts/dynamsoft/v15/dynamsoft.webtwain.config.js")%>"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~/Scripts/dynamsoft/v15/dynamsoft.webtwain.initiate.js")%>"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/jquery-3.3.1.js")%>"></script>

</head>

<body>
    <div id="wrapper">
        <div id="demoContent">
            <div id="dwtScanDemo">
                <div class="ct-top">

                    <div id="DWTcontainer" class="container">
                        <div id="DWTcontainerTop">
                            <div id="divEdit">
                                <ul class="operateGrp">
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/ShowEditor.png" title="Mostra l'editor di immagini" alt="Mostra l'editor di immagini" id="btnEditor" onclick="btnShowImageEditor_onclick()" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/RotateLeft.png" title="Gira a sinistra" alt="Gira a sinistra" id="btnRotateL" onclick="btnRotateLeft_onclick()" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/RotateRight.png" title="Girare a destra" alt="Girare a destra" id="btnRotateR" onclick="btnRotateRight_onclick()" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/Rotate180.png" alt="Turno 180" title="Rotate 180" onclick="btnRotate180_onclick()" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/Mirror.png" title="Specchio" alt="Mirror" id="btnMirror" onclick="btnMirror_onclick()" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/Flip.png" title="Flip" alt="Flip" id="btnFlip" onclick="btnFlip_onclick()" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/RemoveSelectedImages.png" title="Rimuovi le immagini selezionate" alt="Rimuovi le immagini selezionate" id="DW_btnRemoveCurrentImage" onclick="btnRemoveCurrentImage_onclick();" /></li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/RemoveAllImages.png" title="Rimuovi tutte le immagini" alt="Rimuovi tutte le immagini" id="DW_btnRemoveAllImages" onclick="btnRemoveAllImages_onclick();" /></li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/ChangeSize.png" title="Cambia dimensione immagine" alt="Cambia dimensione immagine" id="btnChangeImageSize" onclick="btnChangeImageSize_onclick();" />
                                    </li>
                                    <li>
                                        <img src="../App_Themes/DocSuite2008/Images/scanner/Crop.png" title="Raccolto" alt="Raccolto" id="btnCrop" onclick="btnCrop_onclick();" /></li>
                                </ul>
                                <div id="ImgSizeEditor" style="visibility: hidden">
                                    <ul>
                                        <li>
                                            <label for="img_height">
                                                Nuova altezza :
                                        <input type="text" id="img_height" style="width: 50%;" size="10" />
                                                pixel</label>
                                        </li>
                                        <li>
                                            <label for="img_width">
                                                Nuova larghezza :&nbsp;
                                        <input type="text" id="img_width" style="width: 50%;" size="10" />
                                                pixel</label>
                                        </li>
                                        <li>Metodo di interpolazione:
                                    <select size="1" id="InterpolationMethod">
                                        <option value=""></option>
                                    </select>
                                        </li>
                                        <li style="text-align: center;">
                                            <input type="button" value="   OK   " id="btnChangeImageSizeOK" onclick="btnChangeImageSizeOK_onclick();" />
                                            <input type="button" value=" Annulla " id="btnCancelChange" onclick="btnCancelChange_onclick();" />
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <div id="dwtcontrolContainer"></div>
                            <div id="btnGroupBtm" class="clearfix"></div>
                        </div>
                        <div id="ScanWrapper">
                            <div id="divScanner" class="divinput">
                                <ul class="PCollapse">
                                    <li>
                                        <div class="divType">
                                            <div class="mark_arrow expanded"></div>
                                            <p class="title-menu">
                                            Scansione personalizzata
                                            </p>

                                        </div>
                                        <div id="div_ScanImage" class="divTableStyle">
                                            <ul id="ulScaneImageHIDE">
                                                <li>
                                                    <label for="source">
                                                        <p>Seleziona la fonte:</p>
                                                    </label>
                                                    <select size="1" id="source" style="position: relative;" onchange="source_onchange()">
                                                        <option value=""></option>
                                                    </select>
                                                </li>
                                                <li style="display: none;" id="pNoScanner"><a href="javascript: void(0)" class="ShowtblLoadImage" style="color: #fe8e14" id="aNoScanner">(No TWAIN compatible drivers detected)</a></li>
                                                <li id="divProductDetail"></li>
                                                <li class="tc">
                                                    <input id="btnScan" disabled="disabled" type="button" value="Scansione" onclick="acquireImage();" />
                                                </li>
                                            </ul>
                                        </div>
                                    </li>

                                </ul>
                            </div>
                            <div id="divUpload" class="divinput mt30" style="position: relative">
                                <ul>
                                    <li class="toggle title-menu">Documenti</li>
                                    <li>
                                        <p>Nome del file:</p>
                                        <input type="text" size="20" id="txt_fileName" value="" />
                                        <input type="hidden" id="txt_fileNameforSave" value="immagine_da_scanner" />
                                    </li>
                                    <li id="html_btnSave" style="display:none">
                                        <input id="btnSave" class="btnOrg" type="button" value="Memorizza scansione" onclick="encodeImages(false)" />
                                    </li>
                                </ul>
                                <div id="scan-list" class="clearfix">
                                    <ul id="scan-items">
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div id="DWTcontainerBtm" class="clearfix">
                            <button id="btnConferma" class="btn-confirm" onclick="OnClientClose()" runat="server">Conferma</button>
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
        var _strTempStr = "";       // Store the temp string for display
        var _iLeft, _iTop, _iRight, _iBottom; //These variables are used to remember the selected area

        function OnClientClose() {
            if (!IsMultipleUpload()) {
                encodeImages(true);
                return false;
            } else {
                CloseWindow();
            }
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

            var imagesToScan = [];
            for (var i = 0; i <= currentIndex; i++) {
                imagesToScan.push(i);
            }

            DWObject.ConvertToBase64(imagesToScan, EnumDWT_ImageType.IT_PDF, function (result) {
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
    </script>

    <script>
        // Assign the page onload fucntion.
        $(function () {
            pageonload();
        });
    </script>

    <script>
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
            currentIndex++;
            var cIndex = document.getElementById("source").selectedIndex;
            if (cIndex < 0)
                return;

            DWObject.SelectSourceByIndex(cIndex);
            DWObject.CloseSource();
            DWObject.OpenSource();
            DWObject.IfShowUI = document.getElementById("ShowUI").checked;

            var i;
            for (i = 0; i < 3; i++) {
                if (document.getElementsByName("PixelType").item(i).checked == true)
                    DWObject.PixelType = i;
            }

            DWObject.Resolution = document.getElementById("Resolution").value;

            var bADFChecked = document.getElementById("ADF").checked;
            DWObject.IfFeederEnabled = bADFChecked;

            var bDuplexChecked = document.getElementById("Duplex").checked;
            DWObject.IfDuplexEnabled = bDuplexChecked;

            DWObject.IfDisableSourceAfterAcquire = true;
            DWObject.AcquireImage();
            if (IsMultipleUpload()) {
                $("#txt_fileName").val("immagine_da_scanner_" + encodedScans.length);
            }
        }
        /*-----------------Load Image---------------------*/
        function btnLoadImagesOrPDFs_onclick() {
            var OnPDFSuccess = function () {
                updatePageInfo();
            };

            var OnPDFFailure = function (errorCode, errorString) {
                checkErrorStringWithErrorCode(errorCode, errorString);
            };

            DWObject.IfShowFileDialog = true;
            DWObject.Addon.PDF.SetResolution(200);
            DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConvertMode.CM_RENDERALL);
            DWObject.LoadImageEx("", EnumDWT_ImageType.IT_ALL, OnPDFSuccess, OnPDFFailure);
        }
        //--------------------------------------------------------------------------------------
        //************************** Edit Image ******************************
        //--------------------------------------------------------------------------------------
        function btnShowImageEditor_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.ShowImageEditor();
        }
        function btnRotateRight_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.RotateRight(DWObject.CurrentImageIndexInBuffer);
            if (checkErrorString()) {
                return;
            }
        }
        function btnRotateLeft_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.RotateLeft(DWObject.CurrentImageIndexInBuffer);
            if (checkErrorString()) {
                return;
            }
        }
        function btnRotate180_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.Rotate(DWObject.CurrentImageIndexInBuffer, 180, true);
            if (checkErrorString()) {
                return;
            }
        }
        function btnMirror_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.Mirror(DWObject.CurrentImageIndexInBuffer);
            if (checkErrorString()) {
                return;
            }
        }
        function btnFlip_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.Flip(DWObject.CurrentImageIndexInBuffer);
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
                if (checkErrorString()) {
                    return;
                }
                return;
            }
        }
        /*----------------Change Image Size--------------------*/
        function btnChangeImageSize_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            switch (document.getElementById("ImgSizeEditor").style.visibility) {
                case "visible": document.getElementById("ImgSizeEditor").style.visibility = "hidden"; break;
                case "hidden": document.getElementById("ImgSizeEditor").style.visibility = "visible"; break;
                default: break;
            }
            //document.getElementById("ImgSizeEditor").style.top = ds_gettop(document.getElementById("btnChangeImageSize")) + document.getElementById("btnChangeImageSize").offsetHeight + 15 + "px";
            //document.getElementById("ImgSizeEditor").style.left = ds_getleft(document.getElementById("btnChangeImageSize")) - 14 + "px";

            var iWidth = DWObject.GetImageWidth(DWObject.CurrentImageIndexInBuffer);
            if (iWidth != -1)
                document.getElementById("img_width").value = iWidth;
            var iHeight = DWObject.GetImageHeight(DWObject.CurrentImageIndexInBuffer);
            if (iHeight != -1)
                document.getElementById("img_height").value = iHeight;
        }
        function btnCancelChange_onclick() {
            document.getElementById("ImgSizeEditor").style.visibility = "hidden";
        }
        function btnChangeImageSizeOK_onclick() {
            document.getElementById("img_height").className = "";
            document.getElementById("img_width").className = "";
            if (!re.test(document.getElementById("img_height").value)) {
                document.getElementById("img_height").className += " invalid";
                document.getElementById("img_height").focus();
                return;
            }
            if (!re.test(document.getElementById("img_width").value)) {
                document.getElementById("img_width").className += " invalid";
                document.getElementById("img_width").focus();
                return;
            }
            DWObject.ChangeImageSize(
                DWObject.CurrentImageIndexInBuffer,
                document.getElementById("img_width").value,
                document.getElementById("img_height").value,
                document.getElementById("InterpolationMethod").selectedIndex + 1
            );
            if (checkErrorString()) {
                document.getElementById("ImgSizeEditor").style.visibility = "hidden";
                return;
            }
        }
        //--------------------------------------------------------------------------------------
        //************************** Navigator functions***********************************
        //--------------------------------------------------------------------------------------
        function btnFirstImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.CurrentImageIndexInBuffer = 0;
            updatePageInfo();
        }
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
            else if (DWObject.CurrentImageIndexInBuffer == 0) {
                return;
            }
            DWObject.CurrentImageIndexInBuffer = DWObject.CurrentImageIndexInBuffer - 1;
            updatePageInfo();
        }
        function btnNextImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            else if (DWObject.CurrentImageIndexInBuffer == DWObject.HowManyImagesInBuffer - 1) {
                return;
            }
            DWObject.CurrentImageIndexInBuffer = DWObject.CurrentImageIndexInBuffer + 1;
            updatePageInfo();
        }
        function btnLastImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.CurrentImageIndexInBuffer = DWObject.HowManyImagesInBuffer - 1;
            updatePageInfo();
        }
        function btnRemoveCurrentImage_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.RemoveAllSelectedImages();
            if (DWObject.HowManyImagesInBuffer == 0) {
                document.getElementById("DW_TotalImage").value = DWObject.HowManyImagesInBuffer;
                document.getElementById("DW_CurrentImage").value = "";
                return;
            }
            else {
                updatePageInfo();
            }
            currentIndex--;
        }
        function btnRemoveAllImages_onclick() {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            DWObject.RemoveAllImages();
            document.getElementById("DW_TotalImage").value = "0";
            document.getElementById("DW_CurrentImage").value = "";
            currentIndex = -1;
        }
        function setlPreviewMode() {
            var varNum = parseInt(document.getElementById("DW_PreviewMode").selectedIndex + 1);
            var btnCrop = document.getElementById("btnCrop");
            if (btnCrop) {
                var tmpstr = btnCrop.src;
                if (varNum > 1) {
                    tmpstr = tmpstr.replace('Crop.', 'Crop_gray.');
                    btnCrop.src = tmpstr;
                    btnCrop.onclick = function () { };
                }
                else {
                    tmpstr = tmpstr.replace('Crop_gray.', 'Crop.');
                    btnCrop.src = tmpstr;
                    btnCrop.onclick = function () { btnCrop_onclick(); };
                }
            }

            DWObject.SetViewMode(varNum, varNum);
            if (Dynamsoft.Lib.env.bMac || Dynamsoft.Lib.env.bLinux) {
                return;
            }
            else if (document.getElementById("DW_PreviewMode").selectedIndex != 0) {
                DWObject.MouseShape = true;
            }
            else {
                DWObject.MouseShape = false;
            }
        }
        //--------------------------------------------------------------------------------------
        //************************** Dynamic Web TWAIN Events***********************************
        //--------------------------------------------------------------------------------------
        function Dynamsoft_OnPostTransfer() {
            updatePageInfo();
        }
        function Dynamsoft_OnPostLoadfunction(path, name, type) {
            updatePageInfo();
        }
        function Dynamsoft_OnPostAllTransfers() {
            DWObject.CloseSource();
            updatePageInfo();
            checkErrorString();
        }
        function Dynamsoft_OnMouseClick(index) {
            updatePageInfo();
        }
        function Dynamsoft_OnMouseRightClick(index) {
            // To add
        }
        function Dynamsoft_OnImageAreaSelected(index, left, top, right, bottom) {
            _iLeft = left;
            _iTop = top;
            _iRight = right;
            _iBottom = bottom;
        }
        function Dynamsoft_OnImageAreaDeselected(index) {
            _iLeft = 0;
            _iTop = 0;
            _iRight = 0;
            _iBottom = 0;
        }
        function Dynamsoft_OnMouseDoubleClick() {
            return;
        }
        function Dynamsoft_OnTopImageInTheViewChanged(index) {
            _iLeft = 0;
            _iTop = 0;
            _iRight = 0;
            _iBottom = 0;
            DWObject.CurrentImageIndexInBuffer = index;
            updatePageInfo();
        }
        function Dynamsoft_OnGetFilePath(bSave, count, index, path, name) {
        }
        function pageonload() {

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

            InitBtnGroupBtm(false);
            initCustomScan();

            var twainsource = document.getElementById("source");
            if (twainsource) {
                twainsource.options.length = 0;
                twainsource.options.add(new Option("Looking for devices.Please wait.", 0));
                twainsource.options[0].selected = true;
            }

            initiateInputs();
        }
        function HideLoadImageForLinux() {
            var o = document.getElementById("liLoadImage");
            if (o) {
                if (Dynamsoft.Lib.env.bLinux)
                    o.style.display = "none";
                else
                    o.style.display = "";
            }
        }
        function InitBtnGroupBtm(bNeebBack) {
            var btnGroupBtm = document.getElementById("btnGroupBtm");
            if (btnGroupBtm) {
                var objString = "";
                objString += "<div class='ct-lt'>Page: ";
                objString += "<input id='DW_btnFirstImage' onclick='btnFirstImage_onclick()' type='button' value=' |&lt; '/>&nbsp;";
                objString += "<input id='DW_btnPreImage' onclick='btnPreImage_onclick()' type='button' value=' &lt; '/>&nbsp;&nbsp;";
                objString += "<input type='text' size='2' id='DW_CurrentImage' readonly='readonly'/> / ";
                objString += "<input type='text' size='2' id='DW_TotalImage' readonly='readonly'/>&nbsp;&nbsp;";
                objString += "<input id='DW_btnNextImage' onclick='btnNextImage_onclick()' type='button' value=' &gt; '/>&nbsp;";
                objString += "<input id='DW_btnLastImage' onclick='btnLastImage_onclick()' type='button' value=' &gt;| '/></div>";
                objString += "<div class='ct-rt'>Preview Mode: ";
                objString += "<select size='1' id='DW_PreviewMode' onchange ='setlPreviewMode();'>";
                objString += "    <option value='0'>1X1</option>";
                objString += "</select><br /></div>";
                if (bNeebBack) {
                    objString += "<div class='removeImage'><input id='DW_btnRemoveCurrentImage' onclick='btnRemoveCurrentImage_onclick()' type='button' value='Rimuovi le immagini selezionate'/>";
                    objString += "<input id='DW_btnRemoveAllImages' onclick='btnRemoveAllImages_onclick()' type='button' value='Rimuovi tutte le immagini'/></div>";
                }

                // btnGroupBtm.style.display = "";
                btnGroupBtm.innerHTML = objString;

                // Fill the init data for preview mode selection
                var varPreviewMode = document.getElementById("DW_PreviewMode");
                varPreviewMode.options.length = 0;
                varPreviewMode.options.add(new Option("1X1", 0));
                varPreviewMode.options.add(new Option("2X2", 1));
                varPreviewMode.options.add(new Option("3X3", 2));
                varPreviewMode.options.add(new Option("4X4", 3));
                varPreviewMode.options.add(new Option("5X5", 4));
                varPreviewMode.selectedIndex = 0;

            }
        }
        // split this function
        function initMessageBox(bNeebBack) {
            var objString = "";

            // The container for navigator, view mode and remove button
            objString += "<div style='text-align:center; width:580px; background-color:#FFFFFF;display:block'>";
            objString += "<div style='position:relative; background:white; float:left; width:422px; height:35px;'>";
            objString += "<input id='DW_btnFirstImage' onclick='btnFirstImage_onclick()' type='button' value=' |&lt; '/>&nbsp;";
            objString += "<input id='DW_btnPreImage' onclick='btnPreImage_onclick()' type='button' value=' &lt; '/>&nbsp;&nbsp;";
            objString += "<input type='text' size='2' id='DW_CurrentImage' readonly='readonly'/>/";
            objString += "<input type='text' size='2' id='DW_TotalImage' readonly='readonly'/>&nbsp;&nbsp;";
            objString += "<input id='DW_btnNextImage' onclick='btnNextImage_onclick()' type='button' value=' &gt; '/>&nbsp;";
            objString += "<input id='DW_btnLastImage' onclick='btnLastImage_onclick()' type='button' value=' &gt;| '/></div>";
            objString += "<div style='position:relative; background:white; float:left; width:150px; height:35px;'>Modalita anteprima";
            objString += "<select size='1' id='DW_PreviewMode' onchange ='setlPreviewMode();'>";
            objString += "    <option value='0'>1X1</option>";
            objString += "</select><br /></div>";
            objString += "<div><input id='DW_btnRemoveCurrentImage' onclick='btnRemoveCurrentImage_onclick()' type='button' value='Rimuovi le immagini selezionate'/>";
            if (bNeebBack) {
                objString += "<input id='DW_btnRemoveAllImages' onclick='btnRemoveAllImages_onclick()' type='button' value='Rimuovi tutte le immagini'/><br /><br />";
                objString += "<span style=\"font-size:larger\"><a href =\"online_demo_list.aspx\"><b>Back</b></a></span><br /></div>";
            }
            else {
                objString += "<input id='DW_btnRemoveAllImages' onclick='btnRemoveAllImages_onclick()' type='button' value='Rimuovi tutte le immagini'/><br /></div>";
            }
            objString += "</div>";

            // The container for the error message
            objString += "<div id='DWTdivMsg' style='width:580px;display:inline;'>";
            objString += "Message:<br/>"
            objString += "<div id='DWTemessage' style='width:560px; padding:30px 0 0 3px; height:80px; margin-top:5px; overflow:auto; background-color:#ffffff; border:1px #303030; border-style:solid; text-align:left; position:relative' >";
            objString += "</div></div>";

            var DWTemessageContainer = document.getElementById("DWTemessageContainer");
            DWTemessageContainer.innerHTML = objString;

            // Fill the init data for preview mode selection
            var varPreviewMode = document.getElementById("DW_PreviewMode");
            varPreviewMode.options.length = 0;
            varPreviewMode.options.add(new Option("1X1", 0));
            varPreviewMode.options.add(new Option("2X2", 1));
            varPreviewMode.options.add(new Option("3X3", 2));
            varPreviewMode.options.add(new Option("4X4", 3));
            varPreviewMode.options.add(new Option("5X5", 4));
            varPreviewMode.selectedIndex = 0;

            var _divMessageContainer = document.getElementById("DWTemessage");
            _divMessageContainer.ondblclick = function () {
                this.innerHTML = "";
                _strTempStr = "";
            }

        }
        function initCustomScan() {
            var ObjString = "";
            ObjString += "<ul id='divTwainType'> ";
            ObjString += "<li>";
            ObjString += "<label id ='lblShowUI' for = 'ShowUI'><input type='checkbox' id='ShowUI' />Show UI&nbsp;</label>";
            ObjString += "<label for = 'ADF'><input type='checkbox' id='ADF' />AutoFeeder&nbsp;</label>";
            ObjString += "<label for = 'Duplex'><input type='checkbox' id='Duplex'/>Duplex</label></li>";
            ObjString += "<li id='pixel-type'>Tipo di pixel:";
            ObjString += "<label for='BW' style='margin-left:5px;'><input type='radio' id='B&W' name='PixelType'/>B&amp;W </label>";
            ObjString += "<label for='Gray'><input type='radio' id='Gray' name='PixelType'/>Grigio</label>";
            ObjString += "<label for='RGB'><input type='radio' id='RGB' name='PixelType'/>Colore</label></li>";
            ObjString += "<li>";
            ObjString += "<span id='resolution'>Risoluzione:</span><select size='1' id='Resolution'><option value = ''></option></select></li>";
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
        function IsMultipleUpload() {
            return multipleEnabled && (multipleEnabled === "true" || multipleEnabled === "True");
        }

        function initiateInputs() {
            if (IsMultipleUpload()) {
                $('#html_btnSave').show();
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
        function initDllForChangeImageSize() {

            var vInterpolationMethod = document.getElementById("InterpolationMethod");
            vInterpolationMethod.options.length = 0;
            vInterpolationMethod.options.add(new Option("NearestNeighbor", 1));
            vInterpolationMethod.options.add(new Option("Bilinear", 2));
            vInterpolationMethod.options.add(new Option("Bicubic", 3));

        }
        function setDefaultValue() {
            var vGray = document.getElementById("Gray");
            if (vGray)
                vGray.checked = true;

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

            var _chkMultiPageTIFF_save = document.getElementById("MultiPageTIFF_save");
            if (_chkMultiPageTIFF_save)
                _chkMultiPageTIFF_save.disabled = true;
            var _chkMultiPagePDF_save = document.getElementById("MultiPagePDF_save");
            if (_chkMultiPagePDF_save)
                _chkMultiPagePDF_save.disabled = true;
            var _chkMultiPageTIFF = document.getElementById("MultiPageTIFF");
            if (_chkMultiPageTIFF)
                _chkMultiPageTIFF.disabled = true;
            var _chkMultiPagePDF = document.getElementById("MultiPagePDF");
            if (_chkMultiPagePDF)
                _chkMultiPagePDF.disabled = true;
        }
        // Check if the control is fully loaded.
        function Dynamsoft_OnReady() {

            var liNoScanner = document.getElementById("pNoScanner");
            // If the ErrorCode is 0, it means everything is fine for the control. It is fully loaded.
            DWObject = Dynamsoft.WebTwainEnv.GetWebTwain('dwtcontrolContainer');
            if (DWObject) {
                if (DWObject.ErrorCode == 0) {
                    $('#DWTNonInstallContainerID').hide();

                    DWObject.LogLevel = 0;
                    DWObject.IfAllowLocalCache = true;
                    DWObject.ImageCaptureDriverType = 4;
                    setDefaultValue();

                    DWObject.RegisterEvent("OnTopImageInTheViewChanged", Dynamsoft_OnTopImageInTheViewChanged);
                    DWObject.RegisterEvent("OnMouseClick", Dynamsoft_OnMouseClick);

                    var twainsource = document.getElementById("source");
                    if (!twainsource) {
                        twainsource = document.getElementById("webcamsource");
                    }

                    var vCount = DWObject.SourceCount;
                    DWTSourceCount = vCount;

                    if (twainsource) {
                        twainsource.options.length = 0;
                        for (var i = 0; i < vCount; i++) {
                            twainsource.options.add(new Option(DWObject.GetSourceNameItems(i), i));
                        }
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

                    if (Dynamsoft.Lib.env.bWin)
                        DWObject.MouseShape = false;

                    var btnScan = document.getElementById("btnScan");
                    if (btnScan) {
                        if (vCount == 0)
                            document.getElementById("btnScan").disabled = true;
                        else {
                            document.getElementById("btnScan").disabled = false;
                            document.getElementById("btnScan").style.color = "#fff";
                            document.getElementById("btnScan").style.backgroundColor = "#50a8e1";
                            document.getElementById("btnScan").style.cursor = "pointer";
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

                    initDllForChangeImageSize();

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
                        if (document.links[i].className == "ShowtblLoadImage") {
                            document.links[i].onclick = showtblLoadImage_onclick;
                        }
                        if (document.links[i].className == "ClosetblLoadImage") {
                            document.links[i].onclick = closetblLoadImage_onclick;
                        }
                    }
                    if (vCount == 0) {
                        if (Dynamsoft.Lib.env.bWin) {

                            if (document.getElementById("aNoScanner") && window['bDWTOnlineDemo']) {
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
                    ua = (navigator.userAgent.toLowerCase());
                    if (!ua.indexOf("msie 6.0")) {
                        ShowSiteTour();
                    }

                    DWObject.RegisterEvent("OnPostTransfer", Dynamsoft_OnPostTransfer);
                    DWObject.RegisterEvent("OnPostLoad", Dynamsoft_OnPostLoadfunction);
                    DWObject.RegisterEvent("OnPostAllTransfers", Dynamsoft_OnPostAllTransfers);
                    DWObject.RegisterEvent("OnImageAreaSelected", Dynamsoft_OnImageAreaSelected);
                    DWObject.RegisterEvent("OnImageAreaDeSelected", Dynamsoft_OnImageAreaDeselected);
                    DWObject.RegisterEvent("OnGetFilePath", Dynamsoft_OnGetFilePath);
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
            if (document.getElementById("pNoScanner")) {
                //document.getElementById("tblLoadImage").style.top = ds_gettop(document.getElementById("pNoScanner")) + pNoScanner.offsetHeight + "px";
                //document.getElementById("tblLoadImage").style.left = ds_getleft(document.getElementById("pNoScanner")) + 0 + "px";
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
