/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Declarations/jstree.d.ts" />

module UdsDesigner {
    declare var rivets: any;

    export class BaseTreeViewModel {
        private binder: any;
        //Constructor
        constructor() {
            
        }

        //Methods
        setup(): void {
            this.initializeTree();
            $("#configurationTree").jstree();
        }

        private initializePlugins() {
            $.jstree.defaults.plugins.push("search");
            $.jstree.defaults.plugins.push("themes");
            $.jstree.defaults.plugins.push("ui");
        }

        private initializeThemes() {
            $.jstree.defaults.core.themes.name = "proton";
            $.jstree.defaults.core.themes.responsive = true;
        }

        private initializeCore() {
            $.jstree.defaults.core.animation = 5;
            $.jstree.defaults.core.multiple = false;
            this.initializeThemes();
            $.jstree.defaults.core.check_callback = false;
        }

        private initializeTree() {
            this.initializePlugins();
            this.initializeCore();
        }

        searchElements(e: any): void {
            e.preventDefault();
            $("#configurationTree").jstree(true).refresh(false, true);
        }

        bind(element: Element) {
            this.unbind();
            this.binder = rivets.bind(element, { ctrl: this });
        }

        unbind() {
            if (this.binder != null) {
                this.binder.unbind();
                this.binder = null;
            }
        }
    }

    interface ITreeViewModel {
        id: any;
        text: string;
        children: Array<ITreeViewModel>;
        state: ITreeViewState;
    }

    interface ITreeViewState {
        selected: boolean;
        opened: boolean;
        disabled: boolean;
        checked: boolean;
    }
}