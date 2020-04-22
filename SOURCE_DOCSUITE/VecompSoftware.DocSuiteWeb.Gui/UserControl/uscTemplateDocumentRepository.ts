/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/helpers/stringhelper.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import TemplateDocumentFinderViewModel = require('App/ViewModels/Templates/TemplateDocumentFinderViewModel');
import TemplateDocumentRepositoryService = require('App/Services/Templates/TemplateDocumentRepositoryService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');


class uscTemplateDocumentRepository {

    treeTemplateDocumentId: string;
    toolBarSearchId: string;
    toolBarTagsId: string;
    racTagsDataSourceId: string;
    onlyPublishedTemplate: boolean;
    uscNotificationId: string;


    private _treeTemplateDocument: Telerik.Web.UI.RadTreeView;
    private _toolBarSearch: Telerik.Web.UI.RadToolBar;
    private _toolBarTags: Telerik.Web.UI.RadToolBar;
    private _racTagsDataSource: Telerik.Web.UI.RadClientDataSource;
    private _service: TemplateDocumentRepositoryService;

    static ON_SELECTED_NODE_EVENT = "onSelectedNode";
    static ON_START_LOAD_EVENT = "onStartLoad";
    static ON_END_LOAD_EVENT = "onEndLoad";
    static ON_ERROR_EVENT = "onErrorEvent";

    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Deposito Documentale");
            return;
        }
        this._service = new TemplateDocumentRepositoryService(serviceConfiguration);
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click di un nodo
     * @param sender
     * @param eventArgs
     */
    treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = eventArgs.get_node();
        $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_SELECTED_NODE_EVENT, node.toJsonString());
    }

    /**
     * Evento scatenato al click di un RadButton nella toolbar di ricerca
     * @param sender
     * @param eventArgs
     */
    toolBar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, eventArgs: Telerik.Web.UI.RadToolBarEventArgs) => {
        $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_START_LOAD_EVENT);
        $.when(this.loadNodes()).done(() => {
            $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_END_LOAD_EVENT);
        }).fail((exception) => {
            $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_ERROR_EVENT, exception);
        });
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Metodo di inizializzazione
     */
    initialize(): void {
        this._treeTemplateDocument = <Telerik.Web.UI.RadTreeView>$find(this.treeTemplateDocumentId);
        this._toolBarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._toolBarSearch.add_buttonClicked(this.toolBar_ButtonClicked);
        this._toolBarTags = <Telerik.Web.UI.RadToolBar>$find(this.toolBarTagsId);
        this._racTagsDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.racTagsDataSourceId);

        $("#".concat(this.treeTemplateDocumentId)).data(this);

        $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_START_LOAD_EVENT);
        $.when(this.loadNodes(), this.loadTags()).done(() => {
            $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_END_LOAD_EVENT);
        }).fail((exception) => {
            $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_ERROR_EVENT, exception);
        });
    }

    /**
     * Imposta gli attributi di un nodo
     * @param node
     * @param templateDocument
     */
    private setNodeAttribute(node: Telerik.Web.UI.RadTreeNode, templateDocument: TemplateDocumentRepositoryModel): Telerik.Web.UI.RadTreeNode {
        node.get_attributes().setAttribute("UniqueId", templateDocument.UniqueId);        
        node.get_attributes().setAttribute("Status", templateDocument.Status);
        node.get_attributes().setAttribute("Name", templateDocument.Name);
        node.get_attributes().setAttribute("QualityTag", templateDocument.QualityTag);
        node.get_attributes().setAttribute("Version", templateDocument.Version);
        node.get_attributes().setAttribute("Object", templateDocument.Object);
        node.get_attributes().setAttribute("IdArchiveChain", templateDocument.IdArchiveChain);

        return node;
    }

    /**
     * Esegue la ricerca dei Template esistenti in base ai filtri impostati
     */
    loadNodes(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            let finder: TemplateDocumentFinderViewModel = this.getFinder();
            this._service.findTemplateDocument(finder,
                (data: any) => {
                    try {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        this.setNodes(data, false);
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /**
     * Esegue il caricamento dei Quality Tag per la funzionalità di ricerca
     */
    loadTags(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._service.getTags(
                (data: any) => {
                    try {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }

                        let sourceModel: any[] = this.prepareTagsSourceModel(data.value);
                        (<any>this._racTagsDataSource).set_data(sourceModel);
                        this._racTagsDataSource.fetch(undefined);
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /**
     * Metodo che prepara il modello da passare al datasource della RadAutoCompleteBox
     * @param tags
     */
    prepareTagsSourceModel(tags: string[]): any {
        let models: any[] = new Array();
        $.each(tags, (index: number, tag: string) => {
            models.push({ id: tag, value: tag });
        });
        return models;
    }

    /**
     * Crea e imposta i nodi nella RadTreeView di visualizzazione
     * @param templates
     */
    setNodes(templates: TemplateDocumentRepositoryModel[], append: boolean): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._treeTemplateDocument.get_nodes().getNode(0);

        if (append == false) {
            rootNode.get_nodes().clear();
        }

        $.each(templates, (index: number, template: TemplateDocumentRepositoryModel) => {
            //Verifico se il nodo già esiste nella treeview
            if (this._treeTemplateDocument.findNodeByValue(template.UniqueId) != undefined) {
                return;
            }

            let newNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            newNode.set_text(template.Name);
            newNode.set_value(template.UniqueId);
            switch (+template.Status) {
                case TemplateDocumentRepositoryStatus.Available:
                    newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/template_active.png");
                    break;
                case TemplateDocumentRepositoryStatus.Draft:
                    newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/template_draft.png");
                    break;
                case TemplateDocumentRepositoryStatus.NotAvailable:
                    newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/template_not_active.png");
                    newNode.set_cssClass('node-disabled');
                    break;
            }
            this.setNodeAttribute(newNode, template);
            rootNode.get_nodes().add(newNode);
        });
        rootNode.set_expanded(true);
        this._treeTemplateDocument.commitChanges();
    }

    /**
     * Prepara il finder per la ricerca
     */
    getFinder(): TemplateDocumentFinderViewModel {
        let txtSearchDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._toolBarSearch.findItemByValue('searchDescription').findControl('txtTemplateName');
        let racTags: Telerik.Web.UI.RadAutoCompleteBox = <Telerik.Web.UI.RadAutoCompleteBox>this._toolBarTags.findItemByValue('searchTags').findControl('racTags');

        let finder: TemplateDocumentFinderViewModel = new TemplateDocumentFinderViewModel();
        finder.Name = txtSearchDescription.get_value();

        if (this.onlyPublishedTemplate) {
            finder.Status.push(TemplateDocumentRepositoryStatus.Available);
        } else {
            finder.Status.push(TemplateDocumentRepositoryStatus.Available);
            finder.Status.push(TemplateDocumentRepositoryStatus.Draft);
        }

        let entries: Telerik.Web.UI.AutoCompleteBoxEntryCollection = racTags.get_entries();
        if (entries.get_count() > 0) {
            for (let i: number = 0; i < entries.get_count(); i++) {
                finder.Tags.push(entries.getEntry(i).get_value());
            }
        }

        return finder;
    }

    /**
     * Recupera il modello dal nodo selezionato nella treeview
     */
    getSelectedTemplateDocument(): TemplateDocumentRepositoryModel {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeTemplateDocument.get_selectedNode();
        if (selectedNode == undefined || !selectedNode.get_value()) {
            return undefined;
        }

        let model: TemplateDocumentRepositoryModel = <TemplateDocumentRepositoryModel>{};

        model.UniqueId = selectedNode.get_attributes().getAttribute("UniqueId");
        if (selectedNode.get_attributes().getAttribute("Status") != undefined) {
            model.Status = TemplateDocumentRepositoryStatus[<string>selectedNode.get_attributes().getAttribute("Status")];
        }
        model.Name = selectedNode.get_attributes().getAttribute("Name");
        model.QualityTag = selectedNode.get_attributes().getAttribute("QualityTag");
        model.Version = selectedNode.get_attributes().getAttribute("Version");
        model.Object = selectedNode.get_attributes().getAttribute("Object");
        model.IdArchiveChain = selectedNode.get_attributes().getAttribute("IdArchiveChain");

        return model;
    }

    /**
     * Rimuove uno specifico nodo dalla treeview
     * @param template
     */
    removeTemplate(template: TemplateDocumentRepositoryModel): void {
        let node: Telerik.Web.UI.RadTreeNode = this._treeTemplateDocument.findNodeByValue(template.UniqueId);
        if (node == undefined) {
            console.warn("Nessun nodo trovato con ID '".concat(template.UniqueId, "'"));
            return;
        }

        node.get_parent().get_nodes().remove(node);
    }

    /**
     * Aggiorna uno specifico nodo della treeview
     * @param template
     */
    refreshTemplates(selectCurrentNode: boolean): void {
        let selectedTemplate: TemplateDocumentRepositoryModel = this.getSelectedTemplateDocument();

        $.when(this.loadNodes()).done(() => {
            if (selectCurrentNode) {
                let node: Telerik.Web.UI.RadTreeNode = this._treeTemplateDocument.findNodeByValue(selectedTemplate.UniqueId);
                if (node == undefined) {
                    console.warn("Nessun nodo trovato con ID '".concat(selectedTemplate.UniqueId, "'"));
                    return;
                }
                node.select();
            }
        }).fail((exception) => {
            $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_ERROR_EVENT, exception);
        });
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

}
export = uscTemplateDocumentRepository;
