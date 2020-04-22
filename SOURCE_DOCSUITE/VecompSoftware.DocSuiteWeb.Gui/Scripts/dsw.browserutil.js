
function getBrowserType() {
    if (navigator.userAgent.toLowerCase().match('chrome')) {
        return "chrome";    
    } else if (navigator.userAgent.match(/MSIE 9/)) {
        return "ie9";
    } else if (navigator.userAgent.match(/MSIE 10/)) {
        return "ie10";
    } else if (navigator.userAgent.match(/rv:11.0/i)) {
        return "ie11";
    } else if (navigator.userAgent.indexOf("Firefox") != -1) {
        return "firefox";
    }
}

function checkBrowserType(version) {
    var currentBrowser = getBrowserType();
    return currentBrowser === version;
}