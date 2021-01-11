class GenericHelper {
    public static getUrlParams(URL: string, name: string): string {
        if (!URL) URL = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        let regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)');
        let results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[2].replace(/\+/g, ' '));
    }
}

export = GenericHelper;