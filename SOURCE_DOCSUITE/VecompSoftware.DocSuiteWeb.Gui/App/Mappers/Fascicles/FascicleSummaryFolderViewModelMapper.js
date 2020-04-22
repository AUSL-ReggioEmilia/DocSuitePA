var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var FascicleSummaryFolderViewModelMapper = /** @class */ (function (_super) {
        __extends(FascicleSummaryFolderViewModelMapper, _super);
        function FascicleSummaryFolderViewModelMapper() {
            return _super.call(this) || this;
        }
        //questo mapper serve a mappare il modello di dossierFolder che arriva dalle API in un 
        //dossierSummaryFolderViewModel
        FascicleSummaryFolderViewModelMapper.prototype.Map = function (source) {
            if (!source) {
                return null;
            }
            var toMap = {};
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.Status = source.Status;
            toMap.Typology = source.Typology;
            toMap.idFascicle = source.IdFascicle;
            toMap.idCategory = source.Category ? source.Category.EntityShortId : source.IdCategory;
            toMap.hasChildren = source.HasChildren;
            toMap.hasDocuments = source.HasDocuments;
            toMap.FascicleFolderLevel = source.FascicleFolderLevel;
            return toMap;
        };
        return FascicleSummaryFolderViewModelMapper;
    }(BaseMapper));
    return FascicleSummaryFolderViewModelMapper;
});
//# sourceMappingURL=FascicleSummaryFolderViewModelMapper.js.map