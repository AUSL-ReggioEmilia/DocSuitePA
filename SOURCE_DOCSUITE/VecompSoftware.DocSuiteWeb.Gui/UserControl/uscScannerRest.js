define(["require", "exports"], function (require, exports) {
    var UscScannerRest = /** @class */ (function () {
        function UscScannerRest() {
            var _this = this;
            this.showScannerWindow = function (sender, args) {
                _this.openScanWindow();
            };
        }
        UscScannerRest.prototype.initialize = function () {
            this._btnScan = $find(this.btnScanId);
            this._rwScanner = $find(this.rwScannerId);
            this._rwScanner.add_close(this.onScanWindowClosed);
            this._btnScan.add_clicked(this.showScannerWindow);
        };
        UscScannerRest.prototype.openScanWindow = function () {
            this.openWindow("../UserControl/ScannerRest.aspx?multipleEnabled=" + this.multipleEnabled, "Scanner", 700, 480);
        };
        UscScannerRest.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.rwScannerId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        UscScannerRest.prototype.onScanWindowClosed = function (sender, args) {
            if (sessionStorage.getItem("component.scanner.upload.scan") !== null) {
                var encodedScans = JSON.parse(sessionStorage.getItem("component.scanner.upload.scan"));
                $("#scan-items").empty();
                for (var i = 0; i < encodedScans.length; i++) {
                    $("#scan-items").append("\n                   \n                    <li id='" + encodedScans[i].FileName + "' \n                    style='margin: 5px 0 7px 16px;font:normal 11px/10px \"Segoe UI\",Arial,sans-serif;'>\n                    <span >\n                    <img class=\"manImg\" src=\"../App_Themes/DocSuite2008/images/green-dot-document.png\"></img>\n                    </span>\n                    " + encodedScans[i].FileName + " \n                    <span class='removable-span-item' onclick=\"removeScanFromList('" + encodedScans[i].FileName + "')\">\n                    &times;</span><span class='remove-file' onclick=\"removeScanFromList('" + encodedScans[i].FileName + "')\">Rimuovi</span></li>");
                }
            }
            else {
                $("#scan-items").empty();
            }
        };
        return UscScannerRest;
    }());
    return UscScannerRest;
});
//# sourceMappingURL=uscScannerRest.js.map