/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts" />
var UdsDesigner;
(function (UdsDesigner) {
    var Service = /** @class */ (function () {
        function Service() {
        }
        Service.loadComponentOptions = function (componentType, callback) {
            $.ajax({
                type: "GET",
                url: "../UdsDesigner/options/" + componentType + ".html",
                contentType: "text/html; charset=utf-8",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.loadControls = function (controlName, callback) {
            $.ajax({
                type: "GET",
                url: "../UdsDesigner/Controls/" + controlName + ".html",
                contentType: "text/html; charset=utf-8",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.findRepositories = function (callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadRepositories",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data.d);
                }
            });
        };
        Service.loadRepository = function (idRepository, callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadRepositoryById",
                data: '{idRepository:"' + idRepository + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.newDocument = function (callback) {
            if (callback != null) {
                var initModel = [new UdsDesigner.TitleCtrl()];
                callback(initModel);
            }
        };
        Service.loadBiblosArchives = function (callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadBiblosArchives",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.loadContactTypes = function (callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadContactTypes",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.loadUDSRepositories = function (callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadUDSRepositories",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.loadUDSFields = function (callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadUDSFields",
                data: '{ "udsRepositoryName":' + JSON.stringify($("#selectRepository")[0].innerText) + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.loadSample = function (callback) {
            $.getJSON("sample.json", function (elements) {
                if (callback != null)
                    callback(elements);
            });
        };
        Service.saveModel = function (model, publish, callback, activeDate) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/SaveModel",
                data: '{ "jsModel":' + JSON.stringify(model) + ',"activeDate":' + JSON.stringify(activeDate) + ',"publish":' + JSON.stringify(publish) + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    callback(response);
                }
            });
        };
        Service.loadTempModel = function (callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadTempModel",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        Service.saveTempModel = function (modelJson, callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/SaveTempModel",
                data: modelJson,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    callback(response);
                }
            });
        };
        Service.validateModel = function (modelJson, callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/ValidateModel",
                data: modelJson,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    callback(response);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    var response = {
                        d: {
                            error: true,
                            messages: errorThrown
                        }
                    };
                    callback(response);
                }
            });
        };
        Service.loadRepositoryVersionByRepositoryId = function (idRepository, callback) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadRepositoryVersionByRepositoryId",
                data: '{idRepository:"' + idRepository + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        };
        return Service;
    }());
    UdsDesigner.Service = Service;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=Service.js.map