import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import DossierService = require("App/Services/Dossiers/DossierService");
import DossierModel = require("App/Models/Dossiers/DossierModel");
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import WorfklowFolderPropertiesModel = require("App/Models/Workflows/WorfklowFolderPropertiesModel");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import FascicleRoleModel = require("App/Models/Fascicles/FascicleRoleModel");
import AuthorizationRoleType = require("App/Models/Commons/AuthorizationRoleType");
import RoleModel = require("App/Models/Commons/RoleModel");
import DossierFolderRoleModel = require("App/Models/Dossiers/DossierFolderRoleModel");
import WorkflowRoleModel = require("App/Models/Workflows/WorkflowRoleModel");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");
import Guid = require("App/Helpers/GuidHelper");

declare var ValidatorEnable: any;
class uscWorkflowFolderSelRest {
    rtvWorkflowFolderSelRestId: string;
    dossierModel: DossierModel[];
    uscNotificationId: string;
    pageContentId: string;
    validatorAnyNodeId: string;
    validatorTemplateNodeCheckId: string;
    workflowProp: WorfklowFolderPropertiesModel;

    public static isValidTemplateNode = true;

    private _rtvWorkflowFolderSelRest: Telerik.Web.UI.RadTreeView;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _dossierService: DossierService;

    public static JSON_METADATA = "JsonMetadata";
    public static DOSSIER_FOLDER = "DossierFolder";
    public static SET_RECIPIENT_ROLE= "Set_Recipient_Role";
    public static DOSSIER_FOLDER_PATH = "DossierFolderPath";
    public static DOSSIER_FOLDER_LEVEL = "DossierFolderLevel";
    public static DOSSIER_FOLDER_MODEL = "DossierFolderModel";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        $(`#${this.pageContentId}`).data(this);
        const dossierServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Dossier");
        this._dossierService = new DossierService(dossierServiceConfiguration);

        this._rtvWorkflowFolderSelRest = $find(this.rtvWorkflowFolderSelRestId) as Telerik.Web.UI.RadTreeView;
        this._rtvWorkflowFolderSelRest.add_nodeClicking(this.rtvDossiersWithTemplates_NodeClicking);
        this._rtvWorkflowFolderSelRest.add_nodeExpanding(this.rtvDossiersWithTemplates_NodeExpanding);
    }

    populateTreeByProperties = (worfklowFolderProperties: WorfklowFolderPropertiesModel) => {
        this.enableValidator(false);
        this.enableTemplateValidator(false);
        this._rtvWorkflowFolderSelRest.get_nodes().clear();

        this.workflowProp = worfklowFolderProperties;

        if (worfklowFolderProperties.DossierEnable && worfklowFolderProperties.IdFascicle != null) {
            this._dossierService.getDossiersWithTemplatesByFascicleId(worfklowFolderProperties.IdFascicle, worfklowFolderProperties.DossierType, worfklowFolderProperties.OnlyFolderHasTemplate, 1, '', (data) => {
                if (!data) { return; }
                this.dossierModel = data;

                this.createTree(this.dossierModel);
            }, (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
        }
    }

    rtvDossiersWithTemplates_NodeExpanding = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        args.get_node().get_nodes().clear();

        this._dossierService.getDossiersWithTemplatesByFascicleId(this.workflowProp.IdFascicle, this.workflowProp.DossierType, this.workflowProp.OnlyFolderHasTemplate,
            args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL) + 1,
            args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH), (data) => {
                if (!data) { return; }

                for (const dossier of data) {
                    if (dossier.DossierFolders.length != 0) {
                        for (const dossierFolder of dossier.DossierFolders) {
                            this.createDossierFolderNode(dossierFolder, args.get_node());
                        }
                    }
                }
            }, (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private createTree = (dossiersModel: DossierModel[]) => {
        for (const dossier of dossiersModel) {
            this.createDossierNode(dossier);
        }
    }

    private createDossierNode(dossier: DossierModel) {
        const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(dossier.Subject);
        node.set_value(dossier.UniqueId);
        node.set_imageUrl("../Comm/Images/DocSuite/Dossier_16.png");

        this._rtvWorkflowFolderSelRest.get_nodes().add(node);
        for (const dossierFolder of dossier.DossierFolders) {
            node.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH, dossierFolder.DossierFolderPath);
            node.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL, dossierFolder.DossierFolderLevel);
            this.createEmptyNode(node);
        }
    }

    private createDossierFolderNode(dossierFolder: DossierFolderModel, parenNode: Telerik.Web.UI.RadTreeNode): void {
        const dossierNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        dossierNode.set_text(dossierFolder.Name);
        dossierNode.set_value(dossierFolder.UniqueId);
        dossierNode.set_imageUrl(dossierFolder.JsonMetadata === null ? "../App_Themes/DocSuite2008/imgset16/folder_closed.png" : "../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
        dossierNode.set_expandedImageUrl(dossierFolder.JsonMetadata !== null ? "../App_Themes/DocSuite2008/imgset16/folder_hidden.png" :"../App_Themes/DocSuite2008/imgset16/folder_open.png")
        dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER, uscWorkflowFolderSelRest.DOSSIER_FOLDER);
        dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.JSON_METADATA, dossierFolder.JsonMetadata);
        dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.SET_RECIPIENT_ROLE, this.workflowProp.SetRecipientRole);
        dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_PATH, dossierFolder.DossierFolderPath);
        dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_LEVEL, dossierFolder.DossierFolderLevel);
        dossierNode.get_attributes().setAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_MODEL, JSON.stringify(dossierFolder));
        parenNode.get_nodes().add(dossierNode);
        this.createEmptyNode(dossierNode);
    }

    private createEmptyNode(parenNode: Telerik.Web.UI.RadTreeNode): void {
        const emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("");
        parenNode.get_nodes().add(emptyNode);
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            const uscNotification: UscErrorNotification = $(`#${uscNotificationId}`).data() as UscErrorNotification;
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        const uscNotification: UscErrorNotification = $(`#${uscNotificationId}`).data() as UscErrorNotification;
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    rtvDossiersWithTemplates_NodeClicking = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        if (args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER) === uscWorkflowFolderSelRest.DOSSIER_FOLDER &&
            args.get_node().get_attributes().getAttribute(uscWorkflowFolderSelRest.JSON_METADATA) !== null) {
            const currentNode: Telerik.Web.UI.RadTreeNode = args.get_node();
            this.dossierEvaluation(currentNode);
        } else {
            alert("La funzionalità non è supportata.Attualmente è disponibile solo la selezione di cartelle di dossier per la creazione automatica di fascicoli.");
            uscWorkflowFolderSelRest.isValidTemplateNode = false;
        }
    }

    private dossierEvaluation(node: Telerik.Web.UI.RadTreeNode) {
        const jsonMetadata: string = node.get_attributes().getAttribute(uscWorkflowFolderSelRest.JSON_METADATA);
        const fascicleModel: FascicleModel = JSON.parse(JSON.parse(jsonMetadata)[0].Model);
        const dossierFolderModel: DossierFolderModel = JSON.parse(node.get_attributes().getAttribute(uscWorkflowFolderSelRest.DOSSIER_FOLDER_MODEL));
        fascicleModel.DossierFolders.push(dossierFolderModel);
        if (dossierFolderModel.DossierFolderRoles && dossierFolderModel.DossierFolderRoles.length > 0) {
            const dossierRolesToAdd: DossierFolderRoleModel[] = dossierFolderModel.DossierFolderRoles.filter(x => !fascicleModel.FascicleRoles.some(fr => fr.Role.EntityShortId == x.Role.EntityShortId));
            for (const dossierRole of dossierRolesToAdd) {
                const fascRole: FascicleRoleModel = {} as FascicleRoleModel;
                fascRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                fascRole.IsMaster = dossierRole.IsMaster;
                fascRole.Role = { IdRole: dossierRole.Role.EntityShortId } as RoleModel;
                fascicleModel.FascicleRoles.push(fascRole);
            }
        }

        uscWorkflowFolderSelRest.isValidTemplateNode = true;

        if (node.get_attributes().getAttribute(uscWorkflowFolderSelRest.SET_RECIPIENT_ROLE) === true) {
            const fascicleRole: FascicleRoleModel = fascicleModel.FascicleRoles.filter(x => x.IsMaster === true)[0];
            if (fascicleRole) {
                const role: WorkflowRoleModel = { IdRole: fascicleRole.Role.EntityShortId } as WorkflowRoleModel;
                const roles: WorkflowRoleModel[] = [];
                roles.push(role);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_RECIPIENT_ROLES, JSON.stringify(roles));
            }
        }
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(fascicleModel));
    }

    getSelectedNode(): boolean {
        return uscWorkflowFolderSelRest.isValidTemplateNode;
    }

    validateTemplateSelectedNode(sender, args) {
        const selectedNode = this._rtvWorkflowFolderSelRest.get_selectedNode();
        if (!uscWorkflowFolderSelRest.isValidTemplateNode && selectedNode) {
            args.IsValid = false;
        }
    }

    enableValidator = (state: boolean) => {
        ValidatorEnable($get(this.validatorAnyNodeId), state);
    }

    enableTemplateValidator = (state: boolean) => {
        ValidatorEnable($get(this.validatorTemplateNodeCheckId), state);
    }
}

export = uscWorkflowFolderSelRest;