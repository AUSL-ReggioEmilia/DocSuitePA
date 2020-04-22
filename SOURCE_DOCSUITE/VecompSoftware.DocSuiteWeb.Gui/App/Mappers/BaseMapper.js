define(["require", "exports"], function (require, exports) {
    var BaseMapper = /** @class */ (function () {
        function BaseMapper() {
            var _this = this;
            this.MapCollection = function (source) {
                var mappedEntities = new Array();
                if (source) {
                    source.forEach(function (item) { if (item) {
                        mappedEntities.push(_this.Map(item));
                    } });
                }
                return mappedEntities;
            };
        }
        return BaseMapper;
    }());
    return BaseMapper;
});
//# sourceMappingURL=BaseMapper.js.map