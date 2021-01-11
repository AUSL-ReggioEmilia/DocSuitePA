//Richiede dsw.json.referenceHandling.js

var ODataService = (function () {

    var webApiurl;
    var odataFilterCommand;
    var odataSortingCommand;

    function ODataService(url) {
        webApiurl = url;
        odataFilterCommand = "$filter=";
        odataSortingCommand = "$orderby="
    }

    ODataService.prototype.loadOdata = function (filters, sorts, pager, onSuccessCallback, onErrorCallback) {
        var queryString = "";
        if (filters !== undefined && filters.length > 0) {
            queryString += getFilterODataString(filters);
        }

        if (sorts !== undefined && sorts.length > 0) {
            if (queryString !== "") queryString += "&";
            queryString += getSortODataString(sorts);
        }

        if (pager !== "") {
            if (queryString !== "") queryString += "&";
            queryString += pager
        }

        if (queryString !== "") queryString += "&";
        queryString = queryString += "applySecurity=1";

        var xhrCall = $.ajax({
            type: "GET",
            url: webApiurl + "?" + queryString,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            xhrFields: {
                withCredentials: true
            }
        });

        if (onSuccessCallback) {
            xhrCall.success(onSuccessCallback);
        }

        if (onErrorCallback) {
            xhrCall.error(onErrorCallback);
        }
    }

    function getFilterODataString(filters) {
        var expression = odataFilterCommand + $.map(filters, function (elem) {
            return elem.expression;
        }).join(" and ");
        return expression;
    }

    function getSortODataString(sorts) {
        var comma = ',';
        var expression = odataSortingCommand + $.map(sorts, function (elem) {
            var operator = elem.sortOrder === 1 ? "asc" : "desc";
            if (elem.fieldName.indexOf(",") > 0) {
                var fields = elem.fieldName.split(comma);
                return fields.join(" " + operator + ", ") + " " + operator;
            } else {
                return elem.fieldName + " " + operator;
            }
        }).join(",");
        return expression;
    }

    return ODataService;
})();

(function () {
    //Creo odataGridSource
    var odataGridSource = window.odataGridSource = window.odataGridSource || {};

    //Resources
    var equalTo = "EqualTo";
    var notEqualTo = "NotEqualTo";
    var greaterThan = "GreaterThan";
    var lessThan = "LessThan";
    var greaterThanOrEqual = "GreaterThanOrEqualTo";
    var lessThanOrEqual = "LessThanOrEqualTo";
    var contains = "Contains";
    var noFilter = "NoFilter";
    var stringDataTypeName = "System.String";

    //Extension
    Array.prototype.clear = function () {
        this.splice(0, this.length);
    };

    //Fields
    var filters = [];
    var sorter = [];

    window.OnCommand = function (sender, args) {
        var command = args.get_commandName();
        var commandArgument = args.get_commandArgument();
        var tableView = odataGridSource.grid.get_masterTableView();

        args.set_cancel(true);

        var filterExpressions = tableView.get_filterExpressions().toList();
        var sortExpressions = tableView.get_sortExpressions().toList();
        if (sortExpressions.length === 0)
            sortExpressions.push(odataGridSource.defaultSortExpression);
        loadRequest(filterExpressions, sortExpressions);
    }

    function getFilters(filterExpressions) {
        filters.clear();
        //aggiungo i filtri di default
        if (odataGridSource.defaultFilters.length > 0) {
            $.each(odataGridSource.defaultFilters, function (i, item) {
                addFilter(item.fieldName, item.expression);
            });
        }

        if (filterExpressions.length > 0) {
            $.each(filterExpressions, function (i, item) {
                HandleFilterCommand(item);
            });
        }
    }

    function getSortExpression(sortExpressions) {
        sorter.clear();

        if (sortExpressions.length > 0) {
            $.each(sortExpressions, function (i, item) {
                addSortExpression(item);
            });
        }
    }

    function getPagerExpressions(customIndex, pageSize) {
        var tableView = odataGridSource.grid.get_masterTableView();
        return "$count=true&$inlinecount=allpages&$skip=" + tableView.CurrentPageIndex * tableView.PageSize + "&$top=" + tableView.PageSize;
    }

    function formatValue(value, filterDataTypeName) {
        if (filterDataTypeName === stringDataTypeName) {
            return "'" + value + "'";
        }
        return value;
    }

    function HandleFilterCommand(radFilterExpression) {

        var filteredColumn = radFilterExpression.get_fieldName();
        var filterValue = radFilterExpression.get_fieldValue();
        var filterExpression = radFilterExpression.get_filterFunction();
        var filterDataTypeName = radFilterExpression.get_dataTypeName();

        if (filterValue === undefined || filterValue === "") {
            return;
        }
        var expr = "";
        switch (filterExpression) {
            case equalTo:
                expr = filteredColumn + " eq " + "" + formatValue(filterValue, filterDataTypeName) + "";
                addFilter(filteredColumn, expr);
                break;
            case notEqualTo:
                expr = filteredColumn + " ne " + "" + formatValue(filterValue, filterDataTypeName) + "";
                addFilter(filteredColumn, expr);
                break;
            case greaterThan:
                expr = filteredColumn + " gt " + "" + formatValue(filterValue, filterDataTypeName) + "";
                addFilter(filteredColumn, expr);
                break;
            case lessThan:
                expr = filteredColumn + " lt " + "" + formatValue(filterValue, filterDataTypeName) + "";
                addFilter(filteredColumn, expr);
                break;
            case greaterThanOrEqual:
                expr = filteredColumn + " ge " + "" + formatValue(filterValue, filterDataTypeName) + "";
                addFilter(filteredColumn, expr);
                break;
            case lessThanOrEqual:
                expr = filteredColumn + " le " + "" + formatValue(filterValue, filterDataTypeName) + "";
                addFilter(filteredColumn, expr);
                break;
            case contains:
                expr = "contains(" + filteredColumn + ", '" + filterValue + "')";
                addFilter(filteredColumn, expr);
                break;
            case noFilter:
                addFilter(null, null, "");
                break;
            default:
                console.log("Espressione non valida per il filtro!");
        }
    }

    function addFilter(fieldName, expression) {
        var objFound = $(filters).filter(function () {
            return this.fieldName === fieldName;
        }).first();

        if (expression !== "") {
            if (objFound.length > 0) {
                $.each(filters, function (i, item) {
                    if (item.fieldName === fieldName) {
                        item.expression = expression;
                    }
                });
            } else {
                var filterObj = {};
                filterObj.fieldName = fieldName;
                filterObj.expression = expression;

                filters.push(filterObj);
            }
        }
    }

    function addSortExpression(sortExpression) {
        var sortObj = {};
        sortObj.fieldName = sortExpression.get_fieldName();
        sortObj.sortOrder = sortExpression.get_sortOrder();

        sorter.push(sortObj);
    }

    function extractItemsToExport(items){
        var shouldInclude = function (propertyName) {
            var excluding = [
                "Documents",
                "UDSId",
                "IdUDSRepository",
                "UDSRepository",
                "_cancelMotivation",
                "IdCategory",
                "Category",
                "DocumentUnit",
                "_status",
                "Timestamp"
            ];

            return !(propertyName.startsWith("$") || $.inArray(propertyName, excluding) > -1);
        };


        var data = [];
        for (var i = 0; i < items.length; i++) {
            var itemdata = {};
            for (var property in items[i]) {
                if (items[i].hasOwnProperty(property)) {
                    if (shouldInclude(property)) {
                        itemdata[property] = items[i][property];
                    }
                }
            }
            data.push(itemdata);
        }
        return data;
    }

    function requestSucceeded(data, args) {
        hideGridLoadingPanel();

        var jsonHelper = new JSONHelper();
        var newData = jsonHelper.resolveReferences(data);

        var gridData = newData.Items;
        var gridCount = newData.Count;
        odataGridSource.grid.get_masterTableView().set_dataSource(gridData);
        odataGridSource.grid.get_masterTableView().set_virtualItemCount(gridCount);
        odataGridSource.grid.get_masterTableView().dataBind();

        var gridModel = [];
        var gridColumns = [];

        $.each(odataGridSource.grid.get_masterTableView().get_dataItems(), function (index, item) {

            gridModel.push({});
            $.each(odataGridSource.grid.get_masterTableView().get_columns(), function (key, column) {
                if (column._data.DataField !== "") {
                    if (column._data.DataField === "AnnoNumero") {
                        gridModel[index][column._data.DataField] = item.get_cell(column.get_uniqueName()).innerText;
                        return true;
                    }
                    var timestamp = Date.parse(item.get_cell(column.get_uniqueName()).innerText);
                    if (isNaN(timestamp) === false) {
                        gridModel[index][column._data.DataField] = new Date(timestamp).toISOString().slice(0, 10);
                    } else {
                        gridModel[index][column._data.DataField] = item.get_cell(column.get_uniqueName()).innerText;
                    }
                }
            });
        });

        $.each(odataGridSource.grid.get_masterTableView().get_columns(), function (key, column) {
            if (column._element.innerText.trim() !== "")
                gridColumns.push(column._element.innerText);
        });

        document.getElementById("dgvUDSItems").value = JSON.stringify(extractItemsToExport(gridData));
    }

    function requestAllDataSucceeded(data, args) {
        var jsonHelper = new JSONHelper();
        var newData = jsonHelper.resolveReferences(data);
        var gridModel = [];

        $.each(newData.Items, function (index, item) {

            gridModel.push({});
            $.each(odataGridSource.grid.get_masterTableView().get_columns(), function (key, column) {

                if (column._data.DataField !== "") {
                    if (column._data.DataField === "AnnoNumero") {
                        gridModel[index][column._data.DataField] = item._year + '/' + getUDNumber(item._number);
                    }
                    else if (column._data.DataField === "Category_FullSearchComputed") {
                        gridModel[index][column._data.DataField] = getCategory(item.Category);
                    }
                    else {
                        gridModel[index][column._data.DataField] = item[column._data.DataField];
                    }
                }
            });
        });

        document.getElementById("dgvUDSAllItems").value = JSON.stringify(extractItemsToExport(newData.Items));

        event.preventDefault();
        $("#" + hiddenButtonId).trigger("click");
    }


    function errorRequest(args) {
        hideGridLoadingPanel();
        alert("Errore generale nel recupero dei dati. Contattare l'assistenza.");
        if (args.error().responseJSON === undefined) {
            console.log(args.statusText);
            console.log(args.responseText);
            return;
        }
        console.log(args.error().responseJSON.ExceptionMessage);
    }

    function getSelectedItems() {
        var selectedItems = new Array();
        var gridItems = odataGridSource.grid.get_masterTableView().get_dataItems();
        $.each(gridItems, function (i, item) {
            var element = item.findElement("cbSelect");
            if (element.checked) {
                selectedItems.push(item);
            }
        });
        return selectedItems;
    }

    window.btnGridDocumentsClick = function () {
        showGridLoadingPanel();
        var selectedItems = getSelectedItems();
        if (!selectedItems || selectedItems.length === 0) {
            hideGridLoadingPanel();
            alert("Nessun archivio selezionato.");
            return;
        }
        var selection = selectedItems.reduce(function (prev, curr) {
            prev[curr.getDataKeyValue("UDSId")] = curr.getDataKeyValue("IdUDSRepository");
            return prev;
        }, {});
        window.location.href = "../Viewers/UDSViewer.aspx?UDSIds=".concat(encodeURIComponent(JSON.stringify(selection)));
    };

    window.btnGridSelectAll = function (threshold) {
        var count = 0;
        $.each(odataGridSource.grid.get_masterTableView().get_dataItems(), function (index, item) {
            if (count >= threshold) {
                alert("Non si possono selezionare più di ".concat(threshold, " archivi."));
                return false;
            }
            var element = item.findElement("cbSelect");
            if (element && !element.disabled) {
                element.checked = true;
                count++;
            }
        });
    };

    window.btnGridDeselectAll = function () {
        $.each(getSelectedItems(), function (i, item) {
            var element = item.findElement("cbSelect");
            if (element) {
                element.checked = false;
            }
        });
    };

    window.showGridLoadingPanel = function () {
        odataGridSource.loadingPanel.show(odataGridSource.grid.get_id());
    };

    window.hideGridLoadingPanel = function () {
        odataGridSource.loadingPanel.hide(odataGridSource.grid.get_id());
    };

    window.loadRequest = function (tblFilters, sorts) {
        showGridLoadingPanel();
        getFilters(tblFilters);
        getSortExpression(sorts);
        var pager = getPagerExpressions();
        var service = new ODataService(odataGridSource.webApi);
        service.loadOdata(filters, sorter, pager, requestSucceeded, errorRequest);
    };

    window.loadData = function () {
        var service = new ODataService(odataGridSource.webApi);
        service.loadOdata(filters, sorter, "$count=true&$inlinecount=allpages", requestAllDataSucceeded, errorRequest);
    };

})();