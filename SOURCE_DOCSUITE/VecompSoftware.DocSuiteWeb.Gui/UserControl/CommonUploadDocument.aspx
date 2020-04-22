<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonUploadDocument.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonUploadDocument" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Documento" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
    <asp:CheckBox ID="chkDisableFileExtensionWhiteList" runat="server" Text="Autorizza caricamento estensioni sconosciute" AutoPostBack="true" TextAlign="Right" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock ID="RadScriptBlock" runat="server" EnableViewState="false">
        <style type="text/css">
            html .RadAsyncUpload {
                width: 100%;
            }

                html .RadAsyncUpload .ruFileWrap {
                    display: block;
                }

            .ruFakeInput {
                display: inline-block;
                width: 85%;
            }
        </style>
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                if (window.radWindow)
                    return window.radWindow;
                else if (window.frameElement.radWindow)
                    return window.frameElement.radWindow;
                return null;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

            function OnClientFileUploading(sender, args) {
                //Verifico la blacklist
                if (isBlackListed(args.get_fileName())) {
                    validationBlackList(args.get_fileName());
                    args.set_cancel(true);
                }

                var validext = true;
                
                var exts = args.get_fileName().toLowerCase().split('.');
                var l = exts.length;               
                if(exts[l-1].toLowerCase() == 'p7m'){
                    validext=false;
                    if(exts.length>2){
                        for(var i=exts.length-2; i>0; i--){

                            if (sender.get_allowedFileExtensions().length > 0 && sender.get_allowedFileExtensions().indexOf(exts[i]) == -1) {
                                validext = false;
                                break;
                            }
                            if(exts[i].toLowerCase()!='p7m')
                            {
                                validext = true;
                                break;
                            }
                        }
                    }
                    else
                        validext = false;                    
                }
                if(!validext){
                    args.set_cancel(true);  
                    $("#ErrorHolder").append("<p>Il file '" + args.get_fileName() + "' non presenta un'estensione valida al caricamento.</p>");
                    $("#ErrorHolder").show();
                }
            }

            function OnClientFileUploaded(sender, args) {
                //Verifico la size
                checkSize(args.get_fileInfo());
            }

            function isBlackListed(fileName) {
                if (fileName.indexOf('.') == -1)
                    return true;

                var disallowed = '<%= ClientSideFileExtensionBlackList%>';
                if (disallowed.length == 0)
                    return false;

                var splitted = disallowed.split('|');
                var extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
                var i = splitted.length;
                while (i--)
                    if (splitted[i] == extension)
                        return true;
                return false;
            }

            function purgeBlackListed() {
                var upload = $find("<%= AsyncUploadDocument.ClientID%>");
                var files = upload.getUploadedFiles();
                var i = files.length;
                while (i--)
                    if (isBlackListed(files[i])) {
                        var delta = (upload._additionalFieldIndex > 0 ? 1 : 0);
                        if (upload._currentIndex == 1)
                            delta = 1;
                        upload.deleteFileInputAt(i + delta);
                    }
                hideErrors();
            }

            function checkSize(fileInfo) {
                var warningUploadThresholdType = "<%= WarningUploadThresholdType%>";
                if (warningUploadThresholdType == "ErrorHolder") {
                    var warningUploadThreshold = <%= WarningUploadThreshold%>;
                    if (fileInfo.ContentLength > warningUploadThreshold) {
                        $("#ErrorHolder").append("<p>Il file '" + fileInfo.FileName + "' ha grandezza elevata.</p>");
                        $("#ErrorHolder").show();
                    }    
                }
            }

            function submitPage() {
                var uploadingRows = $(".RadAsyncUpload").find(".ruUploadProgress");
                var i = uploadingRows.length;
                while (i--) {
                    if (!$(uploadingRows[i]).hasClass("ruUploadCancelled") && !$(uploadingRows[i]).hasClass("ruUploadFailure") && !$(uploadingRows[i]).hasClass("ruUploadSuccess")) {
                        alert("Attendere il caricamento dei file prima di procedere.");
                        return false;
                    }
                }
                return true;
            }

            function validationBlackList(args) {
                $("#ErrorHolder").append("<p>Estensione non valida per il file: '" + args + "'.</p>");
                $("#ErrorHolder").show();
            }

            function validationFailed(sender, eventArgs) {
                var maxfilesize = "<%=ProtocolEnv.MaxUploadThreshold%>";
                var mbSize = maxfilesize/1048576;
                var fileExtention = eventArgs.get_fileName().substring(eventArgs.get_fileName().lastIndexOf('.') + 1, eventArgs.get_fileName().length);
                if (eventArgs.get_fileName().lastIndexOf('.') != -1) {
                    if (sender.get_allowedFileExtensions().length > 0 && sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                        //Se ho impostato delle estensioni consentite e non è in queste ==> ritorno errore su estensione
                        $("#ErrorHolder").append("<p>Estensione non valida per il file: '" + eventArgs.get_fileName() + "'.</p>");
                    }
                    else {
                        //Altrimenti si tratta di grandezza eccessiva
                        $("#ErrorHolder").append("<p>Grandezza massima superata per il file: '" + eventArgs.get_fileName() + "'. La dimensione massima consentita è di "+ mbSize +" Mb.</p>");
                    }
                }
                else {
                    $("#ErrorHolder").append("<p>Impossibile gestire il file: '" + eventArgs.get_fileName() + "'.</p>");
                }
                $("#ErrorHolder").show();
            }
            
            function OnClientRemoved(sender, args) {
                if ($("#ErrorHolder").is(':visible')) {
                    // Controllo se ci sono ancora file in errore
                    var upload = $find("<%= AsyncUploadDocument.ClientID%>");
                    var files = upload.getUploadedFiles();
                    var i = files.length;
                    while (i--) {
                        if (isBlackListed(files[i])) {
                            return;
                        }
                    }
                    // Se sono tutti file validi nascondo il messaggio di errore
                    hideErrors();
                }
            }
            
            function hideErrors() {
                $("#ErrorHolder").empty();
                $("#ErrorHolder").hide();
            }

            function cmdConfirm_OnClick(sender, args){
                purgeBlackListed(); 
                if(submitPage()){
                    $find("<%= AjaxManager.ClientID %>").ajaxRequestWithTarget("<%= cmdConfirm.UniqueID %>", "");
                }
                return false;
            }

        </script>
    </telerik:RadScriptBlock>

    <div id="ErrorHolder" class="warningArea" style="display: none;"></div>



    <div id="excelModel" runat="server" visible="False" style="width: 100%; border: 0;">
        <p style="margin-left: 2px;">Modello compilabile per l'importazione massiva di contatti:
        <asp:HyperLink ID="lnkModel" runat="server">modello.xls</asp:HyperLink></p>
    </div>

    <div style="margin: 2px;">
        <telerik:RadAsyncUpload ID="AsyncUploadDocument" runat="server"
            InitialFileInputsCount="1"            
			ChunkSize="1048576"
            UploadedFilesRendering="BelowFileInput"
            OnClientValidationFailed="validationFailed"
            OnClientFileUploading="OnClientFileUploading"
            OnClientFileUploaded="OnClientFileUploaded"
            OnClientFileUploadRemoved="OnClientRemoved">
            <Localization Select="Sfoglia" Remove="Rimuovi" Cancel="Cancella" />
        </telerik:RadAsyncUpload>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="cmdConfirm" runat="server" Text="Conferma" Width="120px" AutoPostBack="false" OnClientClicked="cmdConfirm_OnClick" />
</asp:Content>
