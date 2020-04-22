/// <reference path="../../../Scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="Controllers/DesignerController.ts" />
/// <reference path="Controllers/DesignerResultsController.ts" />
/// <reference path="Service/PathService.ts" />
$(function () {
    $.ajaxSetup({ cache: false });
    moment.locale('it');
    rivets.formatters.date = function (value) {
        return moment(value).format('DD/MM/YYYY');
    };
    rivets.formatters.eq = function (value, valueToCheck) {
        return (value == valueToCheck);
    };
    rivets.formatters.notEq = function (value, valueToCheck) {
        return (value != valueToCheck);
    };
    rivets.formatters.isEmpty = function (value) {
        return $.isEmptyObject(value) || value.length == 0;
    };
    $(document).ajaxStart(function () {
        NProgress.start();
    });
    $(document).ajaxStop(function () {
        NProgress.done();
    });
    $(document).ajaxError(function (event, jqxhr, settings, thrownError) {
        $("#alertErrorMessage").show().delay(5000).fadeOut(1000);
        var errorMessage;
        if (jqxhr.responseJSON) {
            errorMessage = jqxhr.responseJSON.Message;
        }
        else {
            errorMessage = jqxhr.responseText;
        }
        rivets.bind($("#alertErrorMessage"), { message: errorMessage });
    });
    var pathname = new UdsDesigner.PathService().PathName();
    switch (pathname) {
        case "Designer.aspx":
            new UdsDesigner.DesignerController();
            break;
        case "DesignerResults.aspx":
            new UdsDesigner.DesignerResultsController();
            break;
    }
});
//# sourceMappingURL=Main.js.map