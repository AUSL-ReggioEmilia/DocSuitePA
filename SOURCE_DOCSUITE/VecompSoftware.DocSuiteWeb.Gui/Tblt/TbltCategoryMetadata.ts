/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import UscMetadataRepository = require('UserControl/uscMetadataRepository');
import AjaxModel = require('App/Models/AjaxModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import CategoryService = require('App/Services/Commons/CategoryService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class TbltCategoryMetadata {
    btnSubmitId: string;
    uscMetadataRepositoryId: string;
    ajaxManagerId: string;
    categoryId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    btnRemoveId: string;
    metadataRepositoryId: string;

    private _btnSubmit: Telerik.Web.UI.RadButton;
    private _btnRemove: Telerik.Web.UI.RadButton;
    private _serviceConfigurations: ServiceConfiguration[];
    private _categoryService: CategoryService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _category: CategoryModel;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._categoryService = new CategoryService(ServiceConfigurationHelper.getService(serviceConfigurations, "Category"));
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante di conferma
     */
    private btnSubmit_OnClicking = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonCancelEventArgs) => {
        eventArgs.set_cancel(true);
        this._loadingPanel.show(this.uscMetadataRepositoryId);
        let uscMetadaRepository: UscMetadataRepository = <UscMetadataRepository>$("#".concat(this.uscMetadataRepositoryId)).data();
        if (!jQuery.isEmptyObject(uscMetadaRepository)) {
            if (!uscMetadaRepository.getSelectedNode()) {
                alert("Attenzione: selezionare un elemento.");
                return;
            }

            let node: Telerik.Web.UI.RadTreeNode = uscMetadaRepository.getSelectedNode();
            let idMetadataRepository: string = node.get_value();
            if (idMetadataRepository) {
                this._categoryService.getById(+this.categoryId,
                    (data) => {
                        this._category = data;
                        let metadata: MetadataRepositoryModel = new MetadataRepositoryModel();
                        metadata.UniqueId = idMetadataRepository;
                        this._category.MetadataRepository = metadata;
                        this._categoryService.updateCategory(this._category,
                            (data) => {
                                this._loadingPanel.hide(this.uscMetadataRepositoryId);
                                this.closeWindow();
                            },
                            (exception: ExceptionDTO) => {
                                this._loadingPanel.hide(this.uscMetadataRepositoryId);
                                this.showNotificationException(exception);
                            }
                        );
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.uscMetadataRepositoryId);
                        this.showNotificationException(exception);
                    }
                );

            }
        }
    }

    /**
     * Evento scatenato al click del pulsante di conferma
     */
    private btnRemove_OnClicking = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonCancelEventArgs) => {
        eventArgs.set_cancel(true);

        if (!this.metadataRepositoryId) {
            this.showNotificationException(null, "Attenzione: il classificatore non ha metadati associati.");
            return;
        }

        this._loadingPanel.show(this.uscMetadataRepositoryId);
        this._categoryService.getById(+this.categoryId,
            (data) => {
                this._category = data;
                this._category.MetadataRepository = null;
                this._categoryService.updateCategory(this._category,
                    (data) => {
                        this._loadingPanel.hide(this.uscMetadataRepositoryId);
                        this._btnRemove.set_enabled(false);
                        this.closeWindow();
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.uscMetadataRepositoryId);
                        this.showNotificationException(exception);
                    }
                );
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.uscMetadataRepositoryId);
                this.showNotificationException(exception);
            }
        );
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Inizializzazione
     */
    initialize(): void {
        this._btnSubmit = <Telerik.Web.UI.RadButton>$find(this.btnSubmitId);
        this._btnSubmit.set_enabled(false);
        this._btnSubmit.add_clicking(this.btnSubmit_OnClicking);
        this._btnRemove = <Telerik.Web.UI.RadButton>$find(this.btnRemoveId);
        this._btnRemove.set_enabled(false);
        this._btnRemove.add_clicking(this.btnRemove_OnClicking);
        if (this.metadataRepositoryId) {
            this._btnRemove.set_enabled(true);
        }
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        let uscMetadaRepository: UscMetadataRepository = <UscMetadataRepository>$("#".concat(this.uscMetadataRepositoryId)).data();
        if (!jQuery.isEmptyObject(uscMetadaRepository)) {

            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_TREEVIEW_LOADED, (args, data) => {
                if (this.metadataRepositoryId) {
                    let associatedNode: Telerik.Web.UI.RadTreeNode = uscMetadaRepository.findNodeByValue(this.metadataRepositoryId);
                    associatedNode.get_textElement().style.fontWeight = 'bold';
                }
            });

            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_NODE_CLICKED, (args, data) => {
                this._btnSubmit.set_enabled(true);
            });

            $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_ROOT_NODE_CLICKED, (args) => {
                this._btnSubmit.set_enabled(false);
            });
        }
    }

    /**
     * Chiude la window corrente
     */
    closeWindow(): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(true);
    }

    /**
  * Recupera una RadWindow dalla pagina
  */
    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) {
            wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        }
        else if ((<any>window.frameElement).radWindow) {
            wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        }
        return wnd;
    }

    protected showNotificationException = (exception: ExceptionDTO, customMessage?: string) => {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception && exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(customMessage);
            }
        }

    }
}

export = TbltCategoryMetadata;