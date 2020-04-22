declare var requirejs: any;
class RequireJSHelper {

    static getModule<T>(testType: new () => T, className: string): T{
        let moduleJS: T;
        if (requirejs.defined(className)) {
            moduleJS = new requirejs.s.contexts._.defined[className]();
        } else {
            moduleJS = new testType();
        }
        return moduleJS;
    }
}
export = RequireJSHelper;