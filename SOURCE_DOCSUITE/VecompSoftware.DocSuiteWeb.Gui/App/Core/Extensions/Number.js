var _this = this;
define(function () {
    Number.prototype.padLeft = function (size, chToPad) {
        if (chToPad === void 0) { chToPad = '0'; }
        var s = String(_this);
        while (s.length < (size || 2)) {
            s = chToPad + s;
        }
        return s;
    };
});
//# sourceMappingURL=Number.js.map