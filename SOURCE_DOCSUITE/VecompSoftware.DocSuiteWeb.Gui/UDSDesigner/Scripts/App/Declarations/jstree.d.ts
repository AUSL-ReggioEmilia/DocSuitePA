
interface JQuery {
    jstree(): any;
    jstree(options: any): any;
    jstree(eventName: string, elementId: any): any;
}

interface JQueryStatic {
    jstree: JSTree;
}

interface JSTree {
    defaults: JSTreeOptions;
}

interface JSTreeOptions {
    core: JSTreeCore;
    plugins?: Array<any>;
    search?: JSTreeSearch;
}

interface JSTreeThemes {
    name: string;
    url?: string;
    responsive?: boolean;
}

interface JSTreeCore {
    data: any;
    multiple?: boolean;
    animation?: number;
    themes?: JSTreeThemes;
    check_callback?: boolean;
}

interface JSTreeSearch {
    ajax: any;
    case_insensitive?: boolean;
    fuzzy?: boolean;
}