<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscScannerRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscScannerRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<style>
    .scanner-image {
        background-size: 20px;
        background-image: url(/dsw/App_Themes/DocSuite2008/imgset16/scanner.png);
        /*margin-left: -175px;*/
        height: 12px;
    }

    .manImg {
        margin-top: -1px;
        margin-left: -15px;
    }
</style>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscScannerRest;
        require(["UserControl/uscScannerRest"], function (UscScannerRest) {
            $(function () {
                uscScannerRest = new UscScannerRest();

                uscScannerRest.btnScanId = "<%=btnScanner.ClientID%>";
                uscScannerRest.rwScannerId = "<%=rwScanner.ClientID%>";
                uscScannerRest.multipleEnabled = "<%= MultipleEnabled %>";
                uscScannerRest.initialize();

            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }


    </script>

</telerik:RadScriptBlock>


<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>


<div id="ErrorHolder" class="warningArea" style="display: none;"></div>

<div>
    <telerik:RadImageButton CssClass="scanner-image" runat="server" ID="btnScanner" ToolTip="Carica documento da scanner" AutoPostBack="false">
        <Image Url="~/App_Themes/DocSuite2008/imgset16/scanner.png" />
    </telerik:RadImageButton>
</div>


<telerik:RadWindowManager EnableViewState="False" ID="rwScanner" runat="server" VisibleOnPageLoad="false">
    <Windows>
    </Windows>

</telerik:RadWindowManager>
