/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Declarations/jstree.d.ts" />
var UdsDesigner;
(function (UdsDesigner) {
    var BaseTreeViewModel = /** @class */ (function () {
        //Constructor
        function BaseTreeViewModel() {
        }
        //Methods
        BaseTreeViewModel.prototype.setup = function () {
            this.initializeTree();
            $("#configurationTree").jstree();
        };
        BaseTreeViewModel.prototype.initializePlugins = function () {
            $.jstree.defaults.plugins.push("search");
            $.jstree.defaults.plugins.push("themes");
            $.jstree.defaults.plugins.push("ui");
        };
        BaseTreeViewModel.prototype.initializeThemes = function () {
            $.jstree.defaults.core.themes.name = "proton";
            $.jstree.defaults.core.themes.responsive = true;
        };
        BaseTreeViewModel.prototype.initializeCore = function () {
            $.jstree.defaults.core.animation = 5;
            $.jstree.defaults.core.multiple = false;
            this.initializeThemes();
            $.jstree.defaults.core.check_callback = false;
        };
        BaseTreeViewModel.prototype.initializeTree = function () {
            this.initializePlugins();
            this.initializeCore();
        };
        BaseTreeViewModel.prototype.searchElements = function (e) {
            e.preventDefault();
            $("#configurationTree").jstree(true).refresh(false, true);
        };
        BaseTreeViewModel.prototype.bind = function (element) {
            this.unbind();
            this.binder = rivets.bind(element, { ctrl: this });
        };
        BaseTreeViewModel.prototype.unbind = function () {
            if (this.binder != null) {
                this.binder.unbind();
                this.binder = null;
            }
        };
        return BaseTreeViewModel;
    }());
    UdsDesigner.BaseTreeViewModel = BaseTreeViewModel;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=BaseTreeViewModel.js.map