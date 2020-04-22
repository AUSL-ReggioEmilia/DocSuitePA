define(["require", "exports"], function (require, exports) {
    var TemplateDocumentFinderViewModel = /** @class */ (function () {
        function TemplateDocumentFinderViewModel() {
            this.Tags = new Array();
            this.Status = new Array();
        }
        TemplateDocumentFinderViewModel.prototype.hasFilter = function () {
            return !!this.Name || this.Tags.length > 0 || this.Status.length > 0;
        };
        return TemplateDocumentFinderViewModel;
    }());
    return TemplateDocumentFinderViewModel;
});
//# sourceMappingURL=TemplateDocumentFinderViewModel.js.map