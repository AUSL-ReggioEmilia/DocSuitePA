define(["require", "exports", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Dossiers/DossierService", "App/DTOs/ExceptionDTO", "App/Models/Commons/AuthorizationRoleType", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, EnumHelper, ServiceConfigurationHelper, DossierService, ExceptionDTO, AuthorizationRoleType, SessionStorageKeysHelper) {
    var uscWorkflowFolderSelRest = /** @class */ (function () {
        function uscWorkflowFolderSelRest(serviceConfigurations) {
            var _this = this;
            this.populateTreeByProperties = function (worfklowFolderProperties) {
                _this.enableValidator(false);
                _this.enableTemplateValidator(false);
                _this._rtvWorkflowFolderSelRest.get_nodes().clear();
                _this.workflowProp = worfklowFolderProperties;
                if (worfklowFolderProperties.DossierEnable && worfklowFolderProperties.IdFascicle != null) {
                    _this._dossierService.getDossiersWithTemplatesByFascicleId(worfklowFolderProperties.IdFascicle, worfklowFolderProperties.DossierType, worfklowFolderProperties.OnlyFolderHasTemplate, 1, '', function (data) {
                        if (!data) {
                            return;
                        }
                        _this.dossierModel = data;
                        _this.createTree(_this.dossierModel);
                    }, function (exception) {
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
            };
            this.rtvDossiersWithTemplates_NodeExpanding = function (sender, args) {
                args.get_node().get_nodes().clear();
                _this._dossierService.getDossiersWithTemplatesByFascicleId(_this.workflowProp.IdFascicle, _this.workflowProp.DossierType, _this.workflowProp.OnlyFolderHasTemplate, args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL) + 1, args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH), function (data) {
                    if (!data) {
                        return;
                    }
                    for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                        var dossier = data_1[_i];
                        if (dossier.DossierFolders.length != 0) {
                            for (var _a = 0, _b = dossier.DossierFolders; _a < _b.length; _a++) {
                                var dossierFolder = _b[_a];
                                _this.createDossierFolderNode(dossierFolder, args.get_node());
                            }
                        }
                    }
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.createTree = function (dossiersModel) {
                for (var _i = 0, dossiersModel_1 = dossiersModel; _i < dossiersModel_1.length; _i++) {
                    var dossier = dossiersModel_1[_i];
                    _this.createDossierNode(dossier);
                }
            };
            this.rtvDossiersWithTemplates_NodeClicking = function (sender, args) {
                if (args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER) === uscWorkflowFolderSelRest.DOSSIER_FOLDER &&
                    args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.JSON_METADATA) !== null) {
                    var currentNode = args.get_node();
                    _this.dossierEvaluation(currentNode);
                }
                else {
                    alert("La funzionalità non è supportata.Attualmente è disponibile solo la selezione di cartelle di dossier per la creazione automatica di fascicoli.");
                    uscWorkflowFolderSelRest.isValidTemplateNode = false;
                }
            };
            this.enableValidator = function (state) {
                ValidatorEnable($get(_this.validatorAnyNodeId), state);
            };
            this.enableTemplateValidator = function (state) {
                ValidatorEnable($get(_this.validatorTemplateNodeCheckId), state);
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscWorkflowFolderSelRest.prototype.initialize = function () {
            $("#" + this.pageContentId).data(this);
            var dossierServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Dossier");
            this._dossierService = new DossierService(dossierServiceConfiguration);
            this._rtvWorkflowFolderSelRest = $find(this.rtvWorkflowFolderSelRestId);
            this._rtvWorkflowFolderSelRest.add_nodeClicking(this.rtvDossiersWithTemplates_NodeClicking);
            this._rtvWorkflowFolderSelRest.add_nodeExpanding(this.rtvDossiersWithTemplates_NodeExpanding);
        };
        uscWorkflowFolderSelRest.prototype.createDossierNode = function (dossier) {
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(dossier.Subject);
            node.set_value(dossier.UniqueId);
            node.set_imageUrl("../Comm/Images/DocSuite/Dossier_16.png");
            this._rtvWorkflowFolderSelRest.get_nodes().add(node);
            for (var _i = 0, _a = dossier.DossierFolders; _i < _a.length; _i++) {
                var dossierFolder = _a[_i];
                node.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH, dossierFolder.DossierFolderPath);
                node.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL, dossierFolder.DossierFolderLevel);
                this.createEmptyNode(node);
            }
        };
        uscWorkflowFolderSelRest.prototype.createDossierFolderNode = function (dossierFolder, parenNode) {
            var dossierNode = new Telerik.Web.UI.RadTreeNode();
            dossierNode.set_text(dossierFolder.Name);
            dossierNode.set_value(dossierFolder.UniqueId);
            dossierNode.set_imageUrl(dossierFolder.JsonMetadata === null ? "../App_Themes/DocSuite2008/imgset16/folder_closed.png" : "../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
            dossierNode.set_expandedImageUrl(dossierFolder.JsonMetadata !== null ? "../App_Themes/DocSuite2008/imgset16/folder_hidden.png" : "../App_Themes/DocSuite2008/imgset16/folder_open.png");
            dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER, uscWorkflowFolderSelRest.DOSSIER_FOLDER);
            dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.JSON_METADATA, dossierFolder.JsonMetadata);
            dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.SET_RECIPIENT_ROLE, this.workflowProp.SetRecipientRole);
            dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH, dossierFolder.DossierFolderPath);
            dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL, dossierFolder.DossierFolderLevel);
            dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_MODEL, JSON.stringify(dossierFolder));
            parenNode.get_nodes().add(dossierNode);
            this.createEmptyNode(dossierNode);
        };
        uscWorkflowFolderSelRest.prototype.createEmptyNode = function (parenNode) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text("");
            parenNode.get_nodes().add(emptyNode);
        };
        uscWorkflowFolderSelRest.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscWorkflowFolderSelRest.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#" + uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscWorkflowFolderSelRest.prototype.dossierEvaluation = function (node) {
            var jsonMetadata = node.get_attributes().getAttribute(uscWorkflowFolderSelRest.JSON_METADATA);
            var fascicleModel = JSON.parse(JSON.parse(jsonMetadata)[0].Model);
            var dossierFolderModel = JSON.parse(node.get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_MODEL));
            fascicleModel.DossierFolders.push(dossierFolderModel);
            if (dossierFolderModel.DossierFolderRoles && dossierFolderModel.DossierFolderRoles.length > 0) {
                var dossierRolesToAdd = dossierFolderModel.DossierFolderRoles.filter(function (x) { return !fascicleModel.FascicleRoles.some(function (fr) { return fr.Role.EntityShortId == x.Role.EntityShortId; }); });
                for (var _i = 0, dossierRolesToAdd_1 = dossierRolesToAdd; _i < dossierRolesToAdd_1.length; _i++) {
                    var dossierRole = dossierRolesToAdd_1[_i];
                    var fascRole = {};
                    fascRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                    fascRole.IsMaster = dossierRole.IsMaster;
                    fascRole.Role = { IdRole: dossierRole.Role.EntityShortId };
                    fascicleModel.FascicleRoles.push(fascRole);
                }
            }
            uscWorkflowFolderSelRest.isValidTemplateNode = true;
            if (node.get_attributes().getAttribute(uscWorkflowFolderSelRest.SET_RECIPIENT_ROLE) === true) {
                var fascicleRole = fascicleModel.FascicleRoles.filter(function (x) { return x.IsMaster === true; })[0];
                if (fascicleRole) {
                    var role = { IdRole: fascicleRole.Role.EntityShortId };
                    var roles = [];
                    roles.push(role);
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_RECIPIENT_ROLES, JSON.stringify(roles));
                }
            }
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(fascicleModel));
        };
        uscWorkflowFolderSelRest.prototype.getSelectedNode = function () {
            return uscWorkflowFolderSelRest.isValidTemplateNode;
        };
        uscWorkflowFolderSelRest.prototype.validateTemplateSelectedNode = function (sender, args) {
            var selectedNode = this._rtvWorkflowFolderSelRest.get_selectedNode();
            if (!uscWorkflowFolderSelRest.isValidTemplateNode && selectedNode) {
                args.IsValid = false;
            }
        };
        uscWorkflowFolderSelRest.isValidTemplateNode = true;
        uscWorkflowFolderSelRest.JSON_METADATA = "JsonMetadata";
        uscWorkflowFolderSelRest.DOSSIER_FOLDER = "DossierFolder";
        uscWorkflowFolderSelRest.SET_RECIPIENT_ROLE = "Set_Recipient_Role";
        uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH = "DossierFolderPath";
        uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL = "DossierFolderLevel";
        uscWorkflowFolderSelRest.DOSSIER_FOLDER_MODEL = "DossierFolderModel";
        return uscWorkflowFolderSelRest;
    }());
    return uscWorkflowFolderSelRest;
});
//# sourceMappingURL=uscWorkflowFolderSelRest.js.map