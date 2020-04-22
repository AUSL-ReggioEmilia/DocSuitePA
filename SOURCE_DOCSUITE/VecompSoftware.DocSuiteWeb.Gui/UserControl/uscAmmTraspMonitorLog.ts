import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TransparentAdministrationMonitorLogBase = require('Monitors/TransparentAdministrationMonitorLogBase');
import TransparentAdministrationMonitorLogModel = require('App/Models/Monitors/TransparentAdministrationMonitorlogModel');
import TransparentAdministrationMonitorLogService = require('App/Services/Monitors/TransparentAdministrationMonitorLogService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import UscSettori = require('UserControl/uscSettori');
import AjaxModel = require('App/Models/AjaxModel');

declare var Page_IsValid: any;

class uscAmmTraspMonitorLog extends TransparentAdministrationMonitorLogBase {
    rwAmmTraspMonitorLogId: string;
    uscAmmTraspMonitorLogUpdatePanelId: string;
    txtAmmTraspMonitorLogNameId: string;
    dpAmmTraspMonitorLogDateId: string;
    txtAmmTraspMonitorLogNoteId: string;
    cmbAmmTraspMonitorLogRatingId: string;
    cmdAmmTraspMonitorLogSaveId: string;
    lblArchiveId: string;
    lblCreatedById: string;
    lblMonitoringId: string;
    txtAmmTraspMonitorLogDocumentUnitIdValue: string;
    txtAmmTraspMonitorLogDocumentUnitNameValue: string;
    ratingValues: string[];
    uscMonitoraggioContentId: string;
    currentDisplayName: string;
    pageContentId: string;
    uscOwnerRoleId: string;
    ajaxManagerId: string;
    uscNotificationId: string;
    uscMonitoringEditButtonId: string

    uniqueId: string;
    documentUnit: DocumentUnitModel;
    documentUnitName: string;
    actionType: string = "Insert";
    canClearData: boolean;

    private static LOAD_OWNER_ROLE: string = "LoadOwnerRole";
    private static LOAD_ROLE_ID: string = "LoadRoleId";

    private _rwAmmTraspMonitorLog: Telerik.Web.UI.RadWindow;
    private _txtAmmTraspMonitorLogName: Telerik.Web.UI.RadTextBox;
    private _dpAmmTraspMonitorLogDate: Telerik.Web.UI.RadTextBox;
    private _txtAmmTraspMonitorLogNote: Telerik.Web.UI.RadTextBox;
    private _cmbAmmTraspMonitorLogRating: Telerik.Web.UI.RadComboBox;
    private _cmdAmmTraspMonitorLogSave: Telerik.Web.UI.RadButton;
    private _domainUserService: DomainUserService;
    private _uscMonitoringEditButton: Telerik.Web.UI.RadButton;


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME));

        let metadataRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(metadataRepositoryConfiguration);

        $(document).ready(() => {
        });
    }

    public initialize() {
        super.initialize();
        this._rwAmmTraspMonitorLog = <Telerik.Web.UI.RadWindow>$find(this.rwAmmTraspMonitorLogId);
        this._txtAmmTraspMonitorLogName = <Telerik.Web.UI.RadTextBox>$find(this.txtAmmTraspMonitorLogNameId);
        this._dpAmmTraspMonitorLogDate = <Telerik.Web.UI.RadTextBox>$find(this.dpAmmTraspMonitorLogDateId);
        this._txtAmmTraspMonitorLogNote = <Telerik.Web.UI.RadTextBox>$find(this.txtAmmTraspMonitorLogNoteId);
        this._cmbAmmTraspMonitorLogRating = <Telerik.Web.UI.RadComboBox>$find(this.cmbAmmTraspMonitorLogRatingId);
        this._cmdAmmTraspMonitorLogSave = <Telerik.Web.UI.RadButton>$find(this.cmdAmmTraspMonitorLogSaveId);
        this._cmdAmmTraspMonitorLogSave.add_clicking(this.Save_Clicked);
        this._uscMonitoringEditButton = <Telerik.Web.UI.RadButton>$find(this.uscMonitoringEditButtonId);
        this._rwAmmTraspMonitorLog.add_close(this.formClosed);

        if (this._uscMonitoringEditButton) {
            this._uscMonitoringEditButton.add_clicked(this.openMonitor);
        }


        $("#".concat(this.uscMonitoraggioContentId)).hide();


        for (let i = 0; i < this.ratingValues.length; i++) {
            let cmbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            cmbItem.set_text(this.ratingValues[i]);
            this._cmbAmmTraspMonitorLogRating.get_items().add(cmbItem);
        }
        if (this.txtAmmTraspMonitorLogDocumentUnitIdValue) {
            this.loadMonitorLog(this.txtAmmTraspMonitorLogDocumentUnitIdValue);
        }

        this.loadOwnerRole();

        this.bindLoaded();
    }

    openMonitor = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.actionType = "Edit";
        this.canClearData = true;
        this.loadLastLogData();
        this._rwAmmTraspMonitorLog.show();
    }

    formClosed = (sender, args) => {
        this.clearWindowControls();
    }

    private clearWindowControls(): void {
        if (this.canClearData) {
            this.canClearData = false;
            this._cmbAmmTraspMonitorLogRating.get_items().forEach((item: Telerik.Web.UI.RadComboBoxItem) => {
                item.uncheck();
            });

            this._txtAmmTraspMonitorLogNote.clear();
            this._txtAmmTraspMonitorLogName.set_value(this.currentDisplayName);
            this._dpAmmTraspMonitorLogDate.set_value(moment(new Date()).format("DD/MM/YYYY hh:mm"));
            this.loadOwnerRole();
        }
    }

    loadLastLogData() {
        (<TransparentAdministrationMonitorLogService>this.service).getLatestMonitorLogByDocumentUnit(this.txtAmmTraspMonitorLogDocumentUnitIdValue, (data: TransparentAdministrationMonitorLogModel) => {

            this.uniqueId = data.UniqueId;
            this.documentUnit = data.DocumentUnit;
            this.documentUnitName = data.DocumentUnitName;

            this.getLastChangingUser(data);
            this.txtAmmTraspMonitorLogDocumentUnitNameValue = data.DocumentUnitName;
            this._txtAmmTraspMonitorLogNote.set_value(data.Note);
            this._dpAmmTraspMonitorLogDate.set_textBoxValue(moment(data.RegistrationDate).format("YYYY/MM/DD HH:mm"));
            this.getLastCheckedRatings(data);

            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.Value = new Array<string>();
            ajaxModel.Value.push(data.Role.EntityShortId.toString());
            ajaxModel.ActionName = uscAmmTraspMonitorLog.LOAD_ROLE_ID;

            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    updateLastLogData() {

        let role: RoleModel = this.getUscRole(this.uscOwnerRoleId);
        let ratingStringResult: string = "";
        for (let i = 0; i < this._cmbAmmTraspMonitorLogRating.get_checkedItems().length; i++) {
            let item = this._cmbAmmTraspMonitorLogRating.get_checkedItems()[i];
            ratingStringResult += item.get_text() + "|";
        }
        ratingStringResult = ratingStringResult.slice(0, -1);

        let transparentAdministrationMonitorLogModel: TransparentAdministrationMonitorLogModel = {
            UniqueId: this.uniqueId,
            DocumentUnit: this.documentUnit,
            DocumentUnitName: this.txtAmmTraspMonitorLogDocumentUnitNameValue,
            Date: new Date(),
            Note: this._txtAmmTraspMonitorLogNote.get_textBoxValue(),
            RegistrationUser: this._txtAmmTraspMonitorLogName.get_value(),
            RegistrationDate: new Date(this._dpAmmTraspMonitorLogDate.get_textBoxValue()),
            Rating: ratingStringResult,
            Role: role
        };

        this.updateCallback(transparentAdministrationMonitorLogModel);
    }

    updateCallback = (model) => {
        (<TransparentAdministrationMonitorLogService>this.service).updateTransparentAdministrationMonitorLog(model, (data) => {
            if (data == null) return;
            let model: TransparentAdministrationMonitorLogModel = data as TransparentAdministrationMonitorLogModel;
            if (model.LastChangedUser === null)
                this.displayDetailsAfterInsertOrUpdate(model, model.RegistrationUser);
            else
                this.displayDetailsAfterInsertOrUpdate(model, model.LastChangedUser)
            this.actionType = "Insert";
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }


    public loadOwnerRole(): void {
        let uscRoles: UscSettori = <UscSettori>$("#".concat(this.uscOwnerRoleId)).data();
        if (!jQuery.isEmptyObject(uscRoles)) {
            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.Value = new Array<string>();
            ajaxModel.ActionName = uscAmmTraspMonitorLog.LOAD_OWNER_ROLE;

            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
        }
    }

    private bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this);
    }

    Save_Clicked = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this.getUscRole(this.uscOwnerRoleId) === null) {
            alert('Campo Settori Obbligatorio');
            return;
        }
        if (!Page_IsValid) {
            return;
        }
        let ratingStringResult: string = "";
        for (let i = 0; i < this._cmbAmmTraspMonitorLogRating.get_checkedItems().length; i++) {
            let item = this._cmbAmmTraspMonitorLogRating.get_checkedItems()[i];
            ratingStringResult += item.get_text() + "|";
        }
        ratingStringResult = ratingStringResult.slice(0, -1);
        let stringDate: string = this._dpAmmTraspMonitorLogDate.get_textBoxValue();
        let dateFormat: Date = new Date(
            +stringDate.slice(6, 10),
            +stringDate.slice(3, 5) - 1,
            +stringDate.slice(0, 2),
            +stringDate.slice(11, 13),
            +stringDate.slice(14, 16),
            0, 0
        );
        let documentUnit: DocumentUnitModel = new DocumentUnitModel();
        documentUnit.UniqueId = this.txtAmmTraspMonitorLogDocumentUnitIdValue;

        let role: RoleModel = this.getUscRole(this.uscOwnerRoleId);

        let transparentAdministrationMonitorLogModel: TransparentAdministrationMonitorLogModel = {
            UniqueId: "",
            DocumentUnit: documentUnit,
            DocumentUnitName: this.txtAmmTraspMonitorLogDocumentUnitNameValue,
            Date: dateFormat,
            Note: this._txtAmmTraspMonitorLogNote.get_textBoxValue(),
            RegistrationUser: this._txtAmmTraspMonitorLogName.get_textBoxValue(),
            RegistrationDate: new Date(),
            Rating: ratingStringResult,
            Role: role
        };

        if (this.actionType === "Edit") {
            this.updateLastLogData();
        } else if (this.actionType === "Insert") {
            this.insertCallback(transparentAdministrationMonitorLogModel);
        }


    }

    insertCallback(transparentAdministrationMonitorLogModel: TransparentAdministrationMonitorLogModel): void {
        (<TransparentAdministrationMonitorLogService>this.service).insertTransparentAdministrationMonitorLog(transparentAdministrationMonitorLogModel,
            (data: any) => {
                if (data == null) return;
                let model: TransparentAdministrationMonitorLogModel = data as TransparentAdministrationMonitorLogModel;
                if (model.LastChangedUser === null)
                    this.displayDetailsAfterInsertOrUpdate(model, model.RegistrationUser);
                else
                    this.displayDetailsAfterInsertOrUpdate(model, model.LastChangedUser)
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private getUscRole = (uscSettoreId: string) => {
        let roles: Array<RoleModel> = new Array<RoleModel>();
        let uscRoles: UscSettori = <UscSettori>$("#".concat(uscSettoreId)).data();

        if (!jQuery.isEmptyObject(uscRoles)) {
            let source: any = JSON.parse(uscRoles.getRoles());
            if (source != null) {
                for (let s of source) {
                    roles.push(s);
                }
            }
        }

        if (roles.length > 0) {
            return roles[0];
        }

        return null;
    }
    private loadUser(accountUser: string): JQueryPromise<DomainUserModel> {
        let promise: JQueryDeferred<DomainUserModel> = $.Deferred<DomainUserModel>();
        this._domainUserService.getUser(accountUser,
            (data: any) => {
                if (!data) {
                    return promise.resolve({});
                };
                let user: DomainUserModel = data as DomainUserModel;
                return promise.resolve(user);
            },
            (exception: ExceptionDTO) => {
                return promise.reject(exception);
            }
        )
        return promise.promise();
    }

    private loadMonitorLog(documentUnitId: string): void {
        (this.service as TransparentAdministrationMonitorLogService).getLatestMonitorLogByDocumentUnit(documentUnitId,
            (data: any) => {
                if (!data) {
                    return;
                }

                $("#".concat(this.uscMonitoraggioContentId)).show();
                let model: TransparentAdministrationMonitorLogModel = data as TransparentAdministrationMonitorLogModel;

                if (model.LastChangedUser === null)
                    this.displayDetailsAfterInsertOrUpdate(model, model.RegistrationUser);
                else
                    this.displayDetailsAfterInsertOrUpdate(model, model.LastChangedUser)
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            }
        )
    }

    private getLastChangingUser(data: TransparentAdministrationMonitorLogModel): void {
        if (data.LastChangedUser === null) {
            this.loadUser(data.RegistrationUser)
                .done((domainUser) => {
                    this._txtAmmTraspMonitorLogName.set_textBoxValue(domainUser.DisplayName);
                    this._txtAmmTraspMonitorLogName.set_value(data.RegistrationUser);
                })
                .fail((exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        } else {
            this.loadUser(data.LastChangedUser)
                .done((domainUser) => {
                    this._txtAmmTraspMonitorLogName.set_textBoxValue(domainUser.DisplayName);
                    this._txtAmmTraspMonitorLogName.set_value(data.LastChangedUser);
                })
                .fail((exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        }
    }

    private getLastCheckedRatings(data: TransparentAdministrationMonitorLogModel): void {
        let ratings = data.Rating.split('|');
        this._cmbAmmTraspMonitorLogRating.get_items().forEach((item: Telerik.Web.UI.RadComboBoxItem) => {
            for (let rating of ratings) {
                if (item.get_text() === rating) {
                    item.check();
                }
            }
        })
    }

    private displayDetailsAfterInsertOrUpdate(model, user: any): void {
        this.loadUser(user)
            .done((domainUser) => {
                $("#".concat(this.lblMonitoringId)).html("Monitorato da ".concat(domainUser.DisplayName, " in data ", moment(model.Date).format("L").concat(" ", moment(model.Date).format("LTS"))));
                $("#".concat(this.lblArchiveId)).html(model.DocumentUnitName);
                $("#".concat(this.lblCreatedById)).html(domainUser.DisplayName + " " + moment(model.RegistrationDate).format("L"));
                $("#".concat(this.uscMonitoraggioContentId)).show();
                this._rwAmmTraspMonitorLog.close();
            })
            .fail((exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }


}

export = uscAmmTraspMonitorLog;