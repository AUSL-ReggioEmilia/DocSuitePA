define(["require", "exports"], function (require, exports) {
    var UDSLink = /** @class */ (function () {
        //default constructor
        function UDSLink() {
            var _this = this;
            this.UDS_RESULTS_PAGE_URL = "UDSResults.aspx?Type=UDS&isFromUDSLink=True";
            this._btnConnect_onClick = function () {
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.ActionName = "CurentFinderChanged";
                ajaxModel.Value.push(_this.selectedUDSRepositoryId);
                $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                window.location.href = _this.UDS_RESULTS_PAGE_URL + "&IdUDS=" + _this.currentIdUDS + "&IdUDSRepository=" + _this.selectedUDSRepositoryId + "&Action=" + _this.currentAction;
            };
        }
        UDSLink.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConnect = $find(this.btnConnectId);
            if (this._btnConnect) {
                this._btnConnect.add_clicking(this._btnConnect_onClick);
            }
        };
        return UDSLink;
    }());
    return UDSLink;
});
//# sourceMappingURL=UDSLink.js.map