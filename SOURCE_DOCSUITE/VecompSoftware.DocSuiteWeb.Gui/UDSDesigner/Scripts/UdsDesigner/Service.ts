/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts" />

module UdsDesigner {

    export class Service {

        static loadComponentOptions(componentType, callback: (data: any) => any) {
            $.ajax({
                type: "GET",
                url: "../UdsDesigner/options/" + componentType + ".html",
                contentType: "text/html; charset=utf-8",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static loadControls(controlName, callback: (data: any) => any) {
            $.ajax({
                type: "GET",
                url: "../UdsDesigner/Controls/" + controlName + ".html",
                contentType: "text/html; charset=utf-8",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static findRepositories(callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadRepositories",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data.d);
                }
            });
        }

        static loadRepository(idRepository: any, callback: (data: any) => any) {
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
        }

        static newDocument(callback: (data: any) => any) {
            if (callback != null) {
                let initModel: TitleCtrl[] = [new TitleCtrl()];
                callback(initModel);
            }
        }

        static loadBiblosArchives(callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadBiblosArchives",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static loadContactTypes(callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadContactTypes",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static loadCustomActions(callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadCustomActions",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static loadUDSRepositories(callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadUDSRepositories",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static loadUDSFields(callback: (data: any) => any) {
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
        }

        static loadSample(callback: (data: any) => any) {
            $.getJSON("sample.json", (elements) => {
                if (callback != null)
                    callback(elements);
            });
        }

        static saveModel(model: any, idUDSRepository: string, publish: boolean, callback: (data: any) => any, activeDate?: Date) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/SaveModel",
                data: `{"jsModel":${JSON.stringify(model)},"idUDSRepositoryToSave":${idUDSRepository},"activeDate":${JSON.stringify(activeDate)},"publish":${JSON.stringify(publish)}}`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    callback(response);
                }
            });
        }


        static loadTempModel(callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/LoadTempModel",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static saveTempModel(modelJson, callback: (data: any) => any) {
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
        }


        static validateModel(modelJson, callback: (data: any) => any) {
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
        }

        static loadRepositoryVersionByRepositoryId(idRepository: any, callback: (data: any) => any) {
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
        }

        static getUDSFieldListById(odataPath: string, idUDSFieldList: string, callback: (data: any) => any) {
            $.ajax({
                type: "GET",
                url: `${odataPath}/UDSFieldLists?$filter=UniqueId eq ${idUDSFieldList}`,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static loadUDSFieldListChildren(odataPath: string, parentId: string, callback: (data: any) => any) {
            $.ajax({
                type: "GET",
                url: `${odataPath}/UDSFieldLists/UDSFieldListService.GetChildrenByParent(parentId=${parentId})`,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static addUDSFieldListCtrlNode(idUDSRepository: string, treeListName: string, callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/AddUDSFieldListCtrlNode",
                data: `{idUDSRepository:'${idUDSRepository}',name:'${treeListName}'}`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static removeUDSFieldListCtrlNode(restPath: string, model: any, callback: (data: any) => any) {
            $.ajax({
                type: "DELETE",
                url: `${restPath}/UDSFieldList?actionType=128`,
                data: JSON.stringify(model),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

        static updateUDSFieldListRootNode(idUDSFieldList: string, treeListName: string, callback: (data: any) => any) {
            $.ajax({
                type: "POST",
                url: "DesignerService.aspx/UpdateUDSFieldListRootNode",
                data: `{idUDSFieldList:'${idUDSFieldList}',name:'${treeListName}'}`,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    callback(data);
                }
            });
        }

    }

}