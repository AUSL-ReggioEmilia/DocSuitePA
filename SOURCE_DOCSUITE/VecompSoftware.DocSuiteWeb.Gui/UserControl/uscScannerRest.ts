import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

class UscScannerRest {

    btnScanId: string;
    rwScannerId: string;
    multipleEnabled: string;


    _btnScan: Telerik.Web.UI.RadButton;
    _rwScanner: Telerik.Web.UI.RadWindowManager;


    initialize() {
        this._btnScan = <Telerik.Web.UI.RadButton>$find(this.btnScanId);

        this._rwScanner = <Telerik.Web.UI.RadWindowManager>$find(this.rwScannerId);
        this._rwScanner.add_close(this.onScanWindowClosed);

        this._btnScan.add_clicked(this.showScannerWindow);

    }


    showScannerWindow = (sender, args) => {
        this.openScanWindow();
    }

    openScanWindow() {
        this.openWindow(`../UserControl/ScannerRest.aspx?multipleEnabled=${this.multipleEnabled}`, "Scanner", 700, 480);
    }

    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.rwScannerId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }


    onScanWindowClosed(sender, args) {
        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_COMPONENT_SCANNER) !== null) {
            let encodedScans = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_COMPONENT_SCANNER));
            $("#scan-items").empty();

            for (let i = 0; i < encodedScans.length; i++) {

                $("#scan-items").append(`
                   
                    <li id='${encodedScans[i].FileName}' 
                    style='margin: 5px 0 7px 16px;font:normal 11px/10px "Segoe UI",Arial,sans-serif;'>
                    <span >
                    <img class="manImg" src="../App_Themes/DocSuite2008/images/green-dot-document.png"></img>
                    </span>
                    ${encodedScans[i].FileName} 
                    <span class='removable-span-item' onclick="removeScanFromList('${encodedScans[i].FileName}')">
                    &times;</span><span class='remove-file' onclick="removeScanFromList('${encodedScans[i].FileName}')">Rimuovi</span></li>`);
            }
        } else {
            $("#scan-items").empty();
        }
    }

}

export = UscScannerRest;