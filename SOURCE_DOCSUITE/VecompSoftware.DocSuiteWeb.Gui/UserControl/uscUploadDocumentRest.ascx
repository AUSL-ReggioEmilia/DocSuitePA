<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUploadDocumentRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUploadDocumentRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscScannerRest.ascx" TagName="uscScannerRest" TagPrefix="usc" %>

 <link rel="stylesheet" href="../Content/document-uploader.css" />


<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscUploadDocumentRest;
        require(["UserControl/uscUploadDocumentRest"], function (UscUploadDocumentRest) {
            $(function () {
                uscUploadDocumentRest = new UscUploadDocumentRest();
                uscUploadDocumentRest.asyncUploadDocumentId = "<%=asyncUploadDocument.ClientID%>";
                uscUploadDocumentRest.multipleFilesId = "<%=MultipleUploadEnabled%>";
                uscUploadDocumentRest.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscUploadDocumentRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID  %>";
                uscUploadDocumentRest.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function removeScanFromList(filename) {
            var encodedScans = JSON.parse(sessionStorage.getItem("component.scanner.upload.scan"));
            if (encodedScans) {
                for (var i = 0; i < encodedScans.length; i++) {
                    if (encodedScans[i].FileName == filename) {
                        let element = document.getElementById(encodedScans[i].FileName);
                        if (element) {
                            document.getElementById("scan-items").removeChild(element);
                        }
                        encodedScans.splice(i, 1);
                    }
                }
            }
            if (encodedScans.length > 0) {
                sessionStorage.setItem("component.scanner.upload.scan", JSON.stringify(encodedScans));
            } else {
                sessionStorage.removeItem("component.scanner.upload.scan");
            }

        }
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<div id="ErrorHolder" class="warningArea" style="display: none;"></div>

<div style="width: 575px; margin-left:15px;">
    <div style="float: left; width: 20px;">
        <usc:uscScannerRest runat="server" ID="uscScannerRest" MultipleEnabled="true"></usc:uscScannerRest> 
    </div>

    <div style="float: left; width: 372px; margin-left:5px;">
         <telerik:RadAsyncUpload ID="asyncUploadDocument" runat="server" InitialFileInputsCount="1" ChunkSize="1048576" UploadedFilesRendering="BelowFileInput" ToolTip="Carica documento da esplora risorse">
            <Localization Select="" Remove="Rimuovi" Cancel="Cancella" />
        </telerik:RadAsyncUpload>
        <div>
            <ul id="scan-items">
            </ul>
        </div>

    </div>
</div>
