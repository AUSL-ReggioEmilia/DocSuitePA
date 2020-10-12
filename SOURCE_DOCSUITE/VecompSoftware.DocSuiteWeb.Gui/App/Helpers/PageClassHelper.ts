
class PageClassHelper {
    private static LOADED_EVENT: string = "onLoaded";

    static callUserControlFunctionSafe<TResult>(controlId: string): JQueryPromise<TResult> {
        let promise: JQueryDeferred<(TResult)> = $.Deferred<(TResult)>();
        let controlInstance: any = $(`#${controlId}`).data();
        if (jQuery.isEmptyObject(controlInstance)) {
            $(`#${controlId}`).on(this.LOADED_EVENT, () => {
                return promise.resolve(controlInstance);
            });
            return promise.promise();
        }
        return promise.resolve(controlInstance);
    }
}

export = PageClassHelper;