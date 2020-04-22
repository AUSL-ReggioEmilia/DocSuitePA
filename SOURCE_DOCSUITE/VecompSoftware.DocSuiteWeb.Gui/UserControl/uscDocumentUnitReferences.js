define(["require", "exports", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleDocumentUnitService", "App/Services/UDS/UDSDocumentUnitService", "App/DTOs/ExceptionDTO", "App/Services/Protocols/ProtocolLinkService", "App/Services/Protocols/ProtocolService", "App/Services/Messages/MessageService", "App/Services/DocumentArchives/DocumentSeriesItemService", "App/Models/Messages/MessageContactPosition", "App/Services/PECMails/PECMailService", "App/Services/DocumentArchives/DocumentSeriesItemLinksService", "App/Services/Resolutions/ResolutionService", "App/Services/Resolutions/ResolutionDocumentSeriesItemService", "App/Services/Fascicles/FascicleLinkService", "App/Services/Fascicles/FascicleService", "App/Services/Dossiers/DossierService", "App/Models/Commons/DocumentUnitReferenceTypeEnum"], function (require, exports, EnumHelper, ServiceConfigurationHelper, FascicleDocumentUnitService, UDSDocumentUnitService, ExceptionDTO, ProtocolLinkService, ProtocolService, MessageService, DocumentSeriesItemService, MessageContactPosition, PECMailService, DocumentSeriesItemLinksService, ResolutionService, ResolutionDocumentSeriesItemService, FascicleLinkService, FascicleService, DossierService, DocumentUnitReferenceTypeEnum) {
    var uscDocumentUnitReferences = /** @class */ (function () {
        function uscDocumentUnitReferences(serviceConfigurations) {
            var _this = this;
            this.index = 0;
            this.generalOnNodeExpanding = function (sender, args) {
                var node = args.get_node();
                node.toggle();
                var parentNodeAttribute = args.get_node().get_attributes().getAttribute("ParentNodeType");
                var loadIndex = args.get_node().get_index();
                var node = args.get_node();
                if (node.get_level() != 0) {
                    var parentNode = node.get_parent();
                    if (parentNode.get_text().startsWith("Messaggi")) {
                        var nodeValue = args.get_node().get_value();
                        var url = "../Prot/MessageDetails.aspx?MessageId=" + nodeValue + "&DocumentUnitId=" + _this.documentUnitId;
                        _this.openWindow(url, "searchMessages", 750, 300);
                    }
                    if (parentNode.get_text().startsWith("PEC ingresso")) {
                        var nodeValue = args.get_node().get_value();
                        var url = "../Prot/PECDetails.aspx?PECEntityId=" + nodeValue;
                        _this.openWindow(url, "searchPECDetails", 750, 300);
                    }
                    if (parentNode.get_text().startsWith("PEC uscita")) {
                        var nodeValue = args.get_node().get_value();
                        var url = "../Prot/PECDetails.aspx?PECEntityId=" + nodeValue + "&Direction='Outgoing'";
                        _this.openWindow(url, "searchPECDetails", 750, 300);
                    }
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.FascicleProtocol.toString()) {
                    _this.loadFasciclesData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Fascicle.toString()) {
                    _this.loadFascicleData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.SerieRepertorio.toString()) {
                    _this.loadDossiersData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Atti.toString()) {
                    _this.loadDocumentSeriesItemLinkData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.PECIngresso.toString()) {
                    _this.loadIncomingPECMailData(Number(_this.documentUnitYear), Number(_this.documentUnitNumber), "'Incoming'", loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.PECUscita.toString()) {
                    _this.loadOutgoingPECMailData(Number(_this.documentUnitYear), Number(_this.documentUnitNumber), "'Outgoing'", loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Archive.toString()) {
                    _this.loadUDSData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ArchiveProtocol.toString()) {
                    _this.loadUDSProtocolData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Protocol.toString()) {
                    _this.loadProtocolData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ProtocolLinks.toString()) {
                    _this.loadProtocolLinkData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ProtocolSeries.toString()) {
                    _this.loadDocumentSeriesProtocolsLinksData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.MessageResolution.toString()) {
                    _this.loadResolutionMessageData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.MessageProtocol.toString()) {
                    _this.loadMessageData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.MessageSeries.toString()) {
                    _this.loadDocumentSeriesMessageData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Series.toString() && _this.showResolutionDocumentSeriesLinks.toLowerCase() == "true") {
                    _this.loadResolutionDocumentSerieData(_this.documentUnitId, loadIndex);
                }
                if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.SeriesProtocol.toString() && _this.showResolutionDocumentSeriesLinks.toLowerCase() == "false") {
                    _this.loadDocumentSerieData(_this.documentUnitId, loadIndex);
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
            //$(document).ready(() => {
            //});
        }
        uscDocumentUnitReferences.prototype.initialize = function () {
            var _this = this;
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocumentUnit");
            this._fascService = new FascicleDocumentUnitService(serviceConfiguration);
            var udsServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSDocumentUnit");
            this._udsService = new UDSDocumentUnitService(udsServiceConfiguration);
            var protocolLinkServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ProtocolLink");
            this._protocolLinkService = new ProtocolLinkService(protocolLinkServiceConfiguration);
            var protocolServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Protocol");
            this._protocolService = new ProtocolService(protocolServiceConfiguration);
            var messageServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Message");
            this._messageService = new MessageService(messageServiceConfiguration);
            var documentSeriesItemServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesItem");
            this._documentSeriesItemService = new DocumentSeriesItemService(documentSeriesItemServiceConfiguration);
            var documentSeriesItemLinksServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesItemLink");
            this._documentSeriesItemLinkService = new DocumentSeriesItemLinksService(documentSeriesItemLinksServiceConfiguration);
            var pecMailServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "PECMail");
            this._pecMailService = new PECMailService(pecMailServiceConfiguration);
            var resolutionServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Resolution");
            this._resolutionService = new ResolutionService(resolutionServiceConfiguration);
            var resolutionDocumentSeriesItemServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ResolutionDocumentSeriesItem");
            this._resolutionDocumentSeriesItemService = new ResolutionDocumentSeriesItemService(resolutionDocumentSeriesItemServiceConfiguration);
            var fascicleLinksServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleLink");
            this._fascicleLinksService = new FascicleLinkService(fascicleLinksServiceConfiguration);
            var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleServiceConfiguration);
            var dossierServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Dossier");
            this._dossierService = new DossierService(dossierServiceConfiguration);
            this._DocumentRadTreeId = $find(this.radTreeDocumentsId);
            if (this.showIncomingPECMailLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("PEC ingresso", DocumentUnitReferenceTypeEnum.PECIngresso, function () { return _this.loadIncomingPECMailCount(Number(_this.documentUnitYear), Number(_this.documentUnitNumber), "'Incoming'", _this.index); });
            }
            if (this.showOutgoingPECMailLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("PEC uscita", DocumentUnitReferenceTypeEnum.PECUscita, function () { return _this.loadOutgoingPECMailCount(Number(_this.documentUnitYear), Number(_this.documentUnitNumber), "'Outgoing'", _this.index); });
            }
            if (this.showArchiveRelationLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Archivi", DocumentUnitReferenceTypeEnum.ArchiveProtocol, function () { return _this.loadProtocolUDSCount(_this.documentUnitId, _this.index); });
            }
            if (this.showArchiveLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Archivi", DocumentUnitReferenceTypeEnum.Archive, function () { return _this.loadUDSCount(_this.documentUnitId, _this.index); });
            }
            if (this.administrationTrasparenteProtocol.toLocaleLowerCase() == "true" && this.showProtocolDocumentSeriesLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory(this.seriesTitle ? this.seriesTitle : "Serie documentali", DocumentUnitReferenceTypeEnum.SeriesProtocol, function () { return _this.loadProtocolDocumentSeriesCount(_this.documentUnitId, _this.index); });
            }
            if (this.showResolutionDocumentSeriesLinks.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory(this.seriesTitle ? this.seriesTitle : "Serie documentali", DocumentUnitReferenceTypeEnum.Series, function () { return _this.loadDocumentSeriesItemCount(_this.documentUnitId, _this.index); });
            }
            if (this.showProtocolRelationLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.ProtocolLinks, function () { return _this.loadProtocolLinkCount(_this.documentUnitId, _this.index); });
            }
            if (this.showProtocolLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.Protocol, function () { return _this.loadProtocolCount(_this.documentUnitId, _this.index); });
            }
            if (this.showDocumentSeriesProtocolsLinks.toLocaleLowerCase() == "true" && this.protocolDocumentSeriesButtonEnable.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.ProtocolSeries, function () { return _this.loadDocumentSeriesProtocolsLinksCount(_this.documentUnitId, _this.index); });
            }
            if (this.showResolutionlMessageLinks.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageResolution, function () { return _this.loadResolutionMessageCount(_this.documentUnitId, _this.index); });
            }
            if (this.showProtocolMessageLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageProtocol, function () { return _this.loadProtocolMessageCount(_this.documentUnitId, _this.index); });
            }
            if (this.showDocumentSeriesMessageLinks.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageSeries, function () { return _this.loadDocumentSeriesMessageCount(_this.documentUnitId, _this.index); });
            }
            if (this.showDocumentSeriesResolutionsLinks.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Atti", DocumentUnitReferenceTypeEnum.Atti, function () { return _this.loadDocumentSeriesItemLinksCount(_this.documentUnitId, _this.index); });
            }
            if (this.showFascicleLinks.toLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Fascicoli", DocumentUnitReferenceTypeEnum.Fascicle, function () { return _this.loadFascicleCount(_this.documentUnitId, _this.index); });
            }
            if (this.showFasciclesLinks.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Fascicolo", DocumentUnitReferenceTypeEnum.FascicleProtocol, function () { return _this.loadFasciclesCount(_this.documentUnitId, _this.index); });
            }
            if (this.showDossierLinks.toLocaleLowerCase() == "true") {
                this.evaluateNodePropertyFactory("Repertorio/Volume", DocumentUnitReferenceTypeEnum.SerieRepertorio, function () { return _this.loadDossierCount(_this.documentUnitId, _this.index); });
            }
            this._DocumentRadTreeId.add_nodeClicked(this.generalOnNodeExpanding);
        };
        uscDocumentUnitReferences.prototype.evaluateNodePropertyFactory = function (nodeText, value, callback) {
            var serieNode = new Telerik.Web.UI.RadTreeNode();
            serieNode.set_text(nodeText);
            serieNode.set_cssClass("font_node");
            serieNode.get_attributes().setAttribute("ParentNodeType", value);
            this._DocumentRadTreeId.get_nodes().add(serieNode);
            callback();
            this.index++;
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesProtocolsLinksData = function (uniqueId, position) {
            var _this = this;
            if (!this.documentSeriesItemProtocol) {
                this._documentSeriesItemService.getDocumentSeriesItemProtocolById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.documentSeriesItemProtocol = data;
                    _this.renderDocumentSerieItemProtocolNodes(_this.documentSeriesItemProtocol, position);
                });
            }
            else {
                this.renderDocumentSerieItemProtocolNodes(this.documentSeriesItemProtocol, position);
            }
        };
        uscDocumentUnitReferences.prototype.renderDocumentSerieItemProtocolNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            var protocolDocumentSeriesItems = currentItems.Protocols;
            var numberOfItems = currentItems.Protocols.length;
            for (var i = 0; i < numberOfItems; i++) {
                var documentSeriesItemProtocolName = protocolDocumentSeriesItems[i].Year + "-" + protocolDocumentSeriesItems[i].Number + " - " + protocolDocumentSeriesItems[i].Object;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(documentSeriesItemProtocolName);
                node.set_value(protocolDocumentSeriesItems[i].UniqueId);
                node.set_navigateUrl("../Prot/ProtVisualizza.aspx?Year=" + protocolDocumentSeriesItems[i].Year.toString() + "&Number=" + protocolDocumentSeriesItems[i].Number.toString() + "&Type=Prot");
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesMessageData = function (uniqueId, position) {
            var _this = this;
            if (!this.documentSeriesItemList) {
                this._documentSeriesItemService.getDocumentSeriesItemById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.documentSeriesItemList = data;
                    _this.renderDocumentSerieMessageNodes(_this.documentSeriesItemList, position);
                });
            }
            else {
                this.renderDocumentSerieMessageNodes(this.documentSeriesItemList, position);
            }
        };
        uscDocumentUnitReferences.prototype.renderDocumentSerieMessageNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            var numberOfItems = currentItems.Messages.length;
            var messages = currentItems.Messages;
            this.renderMessageNodes(numberOfItems, messages, position);
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesItemLinkData = function (uniqueId, position) {
            var _this = this;
            if (!this.documentSeriesItemLinksList) {
                this._documentSeriesItemLinkService.getDocumentSeriesItemLinksById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.documentSeriesItemLinksList = data;
                    _this.renderDocumentSeriesItemLinkNodes(_this.documentSeriesItemLinksList, position);
                });
            }
            else {
                this.renderDocumentSeriesItemLinkNodes(this.documentSeriesItemLinksList, position);
            }
        };
        uscDocumentUnitReferences.prototype.renderDocumentSeriesItemLinkNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            var documentSeriesItemName = currentItems.Resolution.InclusiveNumber + " - " + currentItems.Resolution.Object;
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(documentSeriesItemName);
            node.set_value(currentItems.Resolution.EntityId);
            node.set_navigateUrl("../Resl/ReslVisualizza.aspx?Type=Resl&IdResolution=" + currentItems.Resolution.EntityId);
            var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            this._DocumentRadTreeId.commitChanges();
        };
        uscDocumentUnitReferences.prototype.loadIncomingPECMailData = function (year, number, pecMailDirection, position) {
            var _this = this;
            if (!this.pecMailIncomingAndOutgoing || this.pecMailIncomingAndOutgoing.length === 0) {
                this._pecMailService.getIncomingPECMail(year, number, pecMailDirection, function (data) {
                    if (!data)
                        return;
                    _this.pecMailIncomingAndOutgoing = data;
                    _this.renderIncomingPECMailNodes(_this.pecMailIncomingAndOutgoing, position);
                });
            }
            else {
                this.renderIncomingPECMailNodes(this.pecMailIncomingAndOutgoing, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadOutgoingPECMailData = function (year, number, direction, position) {
            var _this = this;
            if (!this.pecMailOutgoings) {
                this._pecMailService.getOutgoingPECMail(year, number, direction, function (data) {
                    if (!data)
                        return;
                    _this.pecMailOutgoings = data;
                    _this.renderOutgoingPECMailNodes(_this.pecMailOutgoings, position);
                });
            }
            else {
                this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadFascicleData = function (uniqueId, position) {
            var _this = this;
            if (!this.fascicleDocumentList || this.fascicleDocumentList.length === 0) {
                this._fascService.getFascicleListById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.fascicleDocumentList = data;
                    _this.renderFascicleNodes(_this.fascicleDocumentList, position);
                });
            }
            else {
                this.renderFascicleNodes(this.fascicleDocumentList, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadFasciclesData = function (uniqueId, position) {
            var _this = this;
            if (!this.fascicleLinkList || this.fascicleLinkList.length === 0) {
                this._fascicleLinksService.getLinkedFasciclesById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.fascicleLinkList = data;
                    _this.renderFasciclesNodes(_this.fascicleLinkList, position);
                });
            }
            else {
                this.renderFasciclesNodes(this.fascicleLinkList, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadDossiersData = function (uniqueId, position) {
            var _this = this;
            if (!this.fascicleList || this.fascicleList.length === 0) {
                this._fascicleService.getDossiersById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.fascicleList = data;
                    _this.renderDossiersNodes(_this.fascicleList, position);
                });
            }
            else {
                this.renderDossiersNodes(this.fascicleList, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadDocumentSerieData = function (uniqueId, position) {
            var _this = this;
            if (!this.protocolDocumentSeries) {
                this._protocolService.getDocumentSerieslByUniqueId(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.protocolDocumentSeries = data;
                    _this.renderDocumentSerieNodes(_this.protocolDocumentSeries, position);
                });
            }
            else {
                this.renderDocumentSerieNodes(this.protocolDocumentSeries, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadResolutionDocumentSerieData = function (uniqueId, position) {
            var _this = this;
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            if (!this.resolutionDocumentSeriesItem) {
                this._resolutionDocumentSeriesItemService.getResolutionDocumentSeriesItemLinks(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.resolutionDocumentSeriesItem = data;
                    _this.renderResolutionDocumentSeriesItemNode(_this.resolutionDocumentSeriesItem, position);
                });
            }
            else {
                this.renderResolutionDocumentSeriesItemNode(this.resolutionDocumentSeriesItem, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadResolutionMessageData = function (uniqueId, position) {
            var _this = this;
            if (!this.resolutionMessage) {
                this._resolutionService.getResolutionMessageByUniqueId(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.resolutionMessage = data;
                    _this.renderResolutionMessageNodes(_this.resolutionMessage, position);
                });
            }
            else {
                this.renderResolutionMessageNodes(this.resolutionMessage, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadMessageData = function (uniqueId, position) {
            var _this = this;
            if (!this.protocolMessages) {
                this._protocolService.getProtocolMessageById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.protocolMessages = data;
                    _this.renderProtocolMessageNodes(_this.protocolMessages, position);
                });
            }
            else {
                this.renderProtocolMessageNodes(this.protocolMessages, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadUDSProtocolData = function (uniqueId, position) {
            var _this = this;
            if (!this.udsIdList || this.udsIdList.length === 0) {
                this._udsService.getUDSByProtocolId(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.udsIdList = data;
                    _this.renderUDSByProtocolNodes(_this.udsIdList, position);
                });
            }
            else {
                this.renderUDSByProtocolNodes(this.udsIdList, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadProtocolLinkData = function (uniqueId, position) {
            var _this = this;
            if (!this.protocolLinks || this.protocolLinks.length === 0) {
                this._protocolLinkService.getProtocolById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.protocolLinks = data;
                    _this.renderProtocolLinkNodes(_this.protocolLinks, position);
                });
            }
            else {
                this.renderProtocolLinkNodes(this.protocolLinks, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadProtocolData = function (uniqueId, position) {
            var _this = this;
            if (!this.protocolDocumentList || this.protocolDocumentList.length === 0) {
                this._udsService.getProtocolListById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.protocolDocumentList = data;
                    _this.renderProtocolNodes(_this.protocolDocumentList, position);
                });
            }
            else {
                this.renderProtocolNodes(this.protocolDocumentList, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadUDSData = function (uniqueId, position) {
            var _this = this;
            if (!this.udsDocumentList || this.udsDocumentList.length === 0) {
                this._udsService.getUDSListById(uniqueId, function (data) {
                    if (!data)
                        return;
                    _this.udsDocumentList = data;
                    _this.renderUDSNodes(_this.udsDocumentList, position);
                });
            }
            else {
                this.renderUDSNodes(this.udsDocumentList, position);
            }
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesMessageCount = function (uniqueId, position) {
            var _this = this;
            this._messageService.countDocumentSeriesItemById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Messaggi (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesItemLinksCount = function (uniqueId, position) {
            var _this = this;
            this._documentSeriesItemLinkService.countDocumentSeriesItemLinksById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Atti (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesProtocolsLinksCount = function (uniqueId, position) {
            var _this = this;
            this._protocolService.countDocumentSeriesItemProtocolById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Protocolli (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadFascicleCount = function (uniqueId, position) {
            var _this = this;
            this._fascService.countFascicleById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Fascicoli (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadProtocolCount = function (uniqueId, position) {
            var _this = this;
            this._udsService.countProtocolsById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Protocolli (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadProtocolLinkCount = function (uniqueId, position) {
            var _this = this;
            this._protocolLinkService.countProtocolsById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Protocolli (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadUDSCount = function (uniqueId, position) {
            var _this = this;
            this._udsService.countUDSById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Archivi (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadProtocolUDSCount = function (uniqueId, position) {
            var _this = this;
            this._udsService.countUDSByRelationId(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Archivi (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadProtocolMessageCount = function (uniqueId, position) {
            var _this = this;
            this._messageService.countProtocolMessagesById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Messaggi (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadProtocolDocumentSeriesCount = function (uniqueId, position) {
            var _this = this;
            this._documentSeriesItemService.countDocumentSeriesItemById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                var nodeText = _this.seriesTitle ? _this.seriesTitle : "Serie documentali";
                parentNode.set_text(nodeText + " (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadIncomingPECMailCount = function (year, number, direction, position) {
            var _this = this;
            this._pecMailService.countIncomingPECMail(year, number, direction, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("PEC ingresso (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadOutgoingPECMailCount = function (year, number, direction, position) {
            var _this = this;
            this._pecMailService.countOutgoingPECMail(year, number, direction, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("PEC uscita (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadResolutionMessageCount = function (uniqueId, position) {
            var _this = this;
            this._messageService.countResolutionMessagesById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Messaggi (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadDocumentSeriesItemCount = function (uniqueId, position) {
            var _this = this;
            this._resolutionDocumentSeriesItemService.getResolutionDocumentSeriesItemLinksCount(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                var nodeText = _this.seriesTitle ? _this.seriesTitle : "Serie documentali";
                parentNode.set_text(nodeText + " (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadFasciclesCount = function (uniqueId, position) {
            var _this = this;
            this._fascicleLinksService.countLinkedFascicleById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Fascicoli (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.loadDossierCount = function (uniqueId, position) {
            var _this = this;
            this._dossierService.countDossiersById(uniqueId, function (data) {
                var parentNode = _this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.set_text("Repertorio/Volume (" + data + ")");
            });
        };
        uscDocumentUnitReferences.prototype.renderResolutionDocumentSeriesItemNode = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_1 = currentItems; _i < currentItems_1.length; _i++) {
                var currentItem = currentItems_1[_i];
                var documentSeriesItemName = currentItem.DocumentSeriesItem.DocumentSeries.Name + " " + currentItem.DocumentSeriesItem.Year + " " +
                    (this.pad(currentItem.DocumentSeriesItem.Number, 7) + " del " + moment(currentItem.DocumentSeriesItem.RegistrationDate).format("DD/MM/YYYY") + " ") +
                    ((currentItem.DocumentSeriesItem.PublishingDate !== null ? "Pubblicata il: " + moment(currentItem.DocumentSeriesItem.PublishingDate).format("DD/MM/YYYY") : "") + " ") +
                    ("" + (currentItem.DocumentSeriesItem.RetireDate !== null ? "Ritirato il: " + moment(currentItem.DocumentSeriesItem.PublishingDate).format("DD/MM/YYYY") : ""));
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(documentSeriesItemName);
                node.set_value(currentItems[0].EntityId);
                node.set_navigateUrl("../Series/Item.aspx?Type=Series&Action=2&IdDocumentSeriesItem=" + currentItems[0].EntityId);
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderIncomingPECMailNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_2 = currentItems; _i < currentItems_2.length; _i++) {
                var pecMails = currentItems_2[_i];
                var regex = /\S+[a-z0-9]@[a-z0-9\.]+/img;
                var recipients = pecMails.MailRecipients.replace(/[<>]/g, '').match(regex).join(';');
                var senderAndRecipient = pecMails.MailSenders + " " + recipients;
                var result = "";
                if (pecMails.MailDate) {
                    result = senderAndRecipient + " - " + moment(pecMails.MailDate).format("DD/MM/YYYY, hh:mm:ss");
                }
                else {
                    result = senderAndRecipient;
                }
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(result);
                node.set_value(pecMails.EntityId);
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
            }
        };
        uscDocumentUnitReferences.prototype.renderDocumentSerieNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            var protocolDocumentSeriesItems = currentItems.DocumentSeriesItems;
            var numberOfItems = currentItems.DocumentSeriesItems.length;
            for (var i = 0; i < numberOfItems; i++) {
                var regDate = moment(protocolDocumentSeriesItems[i].RegistrationDate).format("DD/MM/YYYY");
                var publishDate = moment(protocolDocumentSeriesItems[i].PublishingDate).format("DD/MM/YYYY");
                var retDate = moment(protocolDocumentSeriesItems[i].RetireDate).format("DD/MM/YYYY");
                var documentSeriesItemTitle = protocolDocumentSeriesItems[i].Subject + " n. " + protocolDocumentSeriesItems[i].Year + "/" + protocolDocumentSeriesItems[i].Number + " - Reg: " + regDate + " - Pubb: " + publishDate + " - Rit: " + retDate;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(documentSeriesItemTitle);
                node.set_value(protocolDocumentSeriesItems[i].UniqueId);
                node.set_navigateUrl("../Series/Item.aspx?Type=Series&Action=2&IdDocumentSeriesItem=" + protocolDocumentSeriesItems[i].EntityId);
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderOutgoingPECMailNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_3 = currentItems; _i < currentItems_3.length; _i++) {
                var outGoingPECMails = currentItems_3[_i];
                var regex = /\S+[a-z0-9]@[a-z0-9\.]+/img;
                var recipients = outGoingPECMails.MailRecipients.replace(/[<>]/g, '').match(regex).join(';');
                var senderAndRecipient = outGoingPECMails.MailSenders + " " + recipients;
                var result = "";
                if (outGoingPECMails.MailDate) {
                    result = senderAndRecipient + " - " + moment(outGoingPECMails.MailDate).format("DD/MM/YYYY, hh:mm:ss");
                }
                else {
                    result = senderAndRecipient;
                }
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(result);
                node.set_value(outGoingPECMails.EntityId);
                var numberOfItems = outGoingPECMails.PECMailReceipts.length;
                var pecMailsReceipts = outGoingPECMails.PECMailReceipts;
                var nodeImages = "";
                for (var i = 0; i < numberOfItems; i++) {
                    var status_1 = pecMailsReceipts[i].ReceiptType;
                    if (status_1 == "accettazione") {
                        node.set_imageUrl("../Comm/Images/pec-accettazione.gif");
                        node.set_toolTip("accettazione");
                        nodeImages = nodeImages + "<img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-accettazione.gif\" title=\"accettazione\">";
                    }
                    if (status_1 == "avvenuta-consegna") {
                        node.set_imageUrl("../Comm/Images/pec-avvenuta-consegna.gif");
                        node.set_toolTip("avvenuta-consegna");
                        nodeImages = nodeImages + "<img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-avvenuta-consegna.gif\" title=\"avvenuta-consegna\">";
                    }
                    if (status_1 == "non-accettazione") {
                        node.set_imageUrl("../Comm/Images/pec-non-accettazione.gif");
                        node.set_toolTip("non-accettazione");
                        nodeImages = nodeImages + "<img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-non-accettazione.gif\" title=\"non-accettazione\">";
                    }
                    if (status_1 == "preavviso-errore-consegna") {
                        node.set_imageUrl("../Comm/Images/pec-preavviso-errore-consegna.gif");
                        node.set_toolTip("preavviso-errore-consegna");
                        nodeImages = nodeImages + "<img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/preavviso-errore-consegna.gif\" title=\"preavviso-errore-consegna\">";
                    }
                    if (status_1 == "errore-consegna") {
                        node.set_imageUrl("../Comm/Images/pec-errore-consegna.gif");
                        node.set_toolTip("errore-consegna");
                        nodeImages = nodeImages + "<img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-errore-consegna.gif\" title=\"errore-consegna\">";
                    }
                }
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                if (nodeImages) {
                    node.get_imageElement().outerHTML = nodeImages;
                }
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderResolutionMessageNodes = function (currentItems, position) {
            var messages = currentItems.Messages;
            var numberOfItems = currentItems.Messages.length;
            this.renderMessageNodes(numberOfItems, messages, position);
        };
        uscDocumentUnitReferences.prototype.renderProtocolMessageNodes = function (currentItems, position) {
            var numberOfItems = currentItems[0].Messages.length;
            var messages = currentItems[0].Messages;
            this.renderMessageNodes(numberOfItems, messages, position);
        };
        uscDocumentUnitReferences.prototype.renderFascicleNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_4 = currentItems; _i < currentItems_4.length; _i++) {
                var fascDoc = currentItems_4[_i];
                var fascicleDocumentsName = fascDoc.Fascicle.Title + " - " + fascDoc.Fascicle.FascicleObject;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(fascicleDocumentsName);
                node.set_value(fascDoc.Fascicle.UniqueId);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
                node.set_navigateUrl("../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + fascDoc.Fascicle.UniqueId);
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderFasciclesNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_5 = currentItems; _i < currentItems_5.length; _i++) {
                var fascicles = currentItems_5[_i];
                var fascicleName = fascicles.FascicleLinked.Title + " - " + fascicles.FascicleLinked.FascicleObject;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(fascicleName);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
                node.set_navigateUrl("../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + fascicles.FascicleLinked.UniqueId);
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderDossiersNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_6 = currentItems; _i < currentItems_6.length; _i++) {
                var fascicle = currentItems_6[_i];
                for (var _a = 0, _b = fascicle.DossierFolders; _a < _b.length; _a++) {
                    var dossierFolder = _b[_a];
                    var dossierName = dossierFolder.Dossier.Year + "/" + this.pad(+dossierFolder.Dossier.Number, 7) + " - " + dossierFolder.Dossier.Subject + "/" + dossierFolder.Name;
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(dossierName);
                    node.set_value(dossierFolder.Dossier.UniqueId);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/process.png");
                    node.set_navigateUrl("../Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier=" + dossierFolder.Dossier.UniqueId);
                    var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                    parentNode.get_nodes().add(node);
                    this._DocumentRadTreeId.commitChanges();
                }
            }
        };
        uscDocumentUnitReferences.prototype.renderMessageNodes = function (numberOfItems, messages, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var i = 0; i < numberOfItems; i++) {
                var status_2 = messages[i].Status;
                //Inviato in data <data> a : elenco destinatari
                var protocolMessageName = messages[i].MessageContacts.filter(function (f) { return f.ContactPosition == MessageContactPosition.Sender; }).map(function (f) { return f.Description; }).join("; ") + " " + (messages[i].MessageEmails[0].SentDate ? 'ha inviato in data ' + moment(messages[i].MessageEmails[0].SentDate).format("DD/MM/YYYY") : 'invio in corso') + " a " + messages[i].MessageContacts.filter(function (f) { return f.ContactPosition == MessageContactPosition.Recipient || f.ContactPosition == MessageContactPosition.RecipientBcc; }).map(function (f) { return f.Description; }).join("; ");
                ;
                var idMessage = messages[i].EntityId;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(protocolMessageName);
                node.set_value(idMessage);
                if (status_2 == "Active") {
                    node.set_imageUrl("../Comm/Images/pec-accettazione.gif");
                    node.set_toolTip("Invio in corso");
                }
                if (status_2 == "Sent") {
                    node.set_imageUrl("../Comm/Images/pec-avvenuta-consegna.gif");
                    node.set_toolTip("Inviato");
                }
                if (status_2 == "Error") {
                    node.set_imageUrl("../Comm/Images/pec-errore-consegna.gif");
                    node.set_toolTip("Errore");
                }
                if (status_2 == "Draft") {
                    node.set_imageUrl("../Comm/Images/pec-non-accettazione.gif");
                    node.set_toolTip("Bozza");
                }
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderUDSByProtocolNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_7 = currentItems; _i < currentItems_7.length; _i++) {
                var udsDoc = currentItems_7[_i];
                var udsDocumentsName = udsDoc.Relation.Title + " - " + udsDoc.Relation.Subject;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(udsDocumentsName);
                node.set_value(udsDoc.Relation.UniqueId);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/document_copies.png");
                if (udsDoc.Relation.UDSRepository) {
                    node.set_navigateUrl("../UDS/UDSView.aspx?Type=UDS&IdUDS=" + udsDoc.IdUDS + "&IdUDSRepository=" + udsDoc.Relation.UDSRepository.UniqueId.toString());
                }
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderProtocolLinkNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_8 = currentItems; _i < currentItems_8.length; _i++) {
                var protLink = currentItems_8[_i];
                var protocolLinkName = protLink.ProtocolLinked.Year + "-" + protLink.ProtocolLinked.Number + " - " + protLink.ProtocolLinked.Object;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(protocolLinkName);
                node.set_value(protLink.ProtocolLinked.UniqueId);
                node.set_imageUrl("../Comm/Images/DocSuite/Protocollo16.gif");
                node.set_navigateUrl("../Prot/ProtVisualizza.aspx?Year=" + protLink.ProtocolLinked.Year.toString() + "&Number=" + protLink.ProtocolLinked.Number.toString() + "&Type=Prot");
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderProtocolNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_9 = currentItems; _i < currentItems_9.length; _i++) {
                var protDoc = currentItems_9[_i];
                var protocolDocumentsName = protDoc.Relation.Title + " - " + protDoc.Relation.Subject;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(protocolDocumentsName);
                node.set_value(protDoc.Relation.UniqueId);
                node.set_imageUrl("../Comm/Images/DocSuite/Protocollo16.gif");
                node.set_navigateUrl("../Prot/ProtVisualizza.aspx?Year=" + protDoc.Relation.Year.toString() + "&Number=" + protDoc.Relation.Number.toString() + "&Type=Prot");
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.renderUDSNodes = function (currentItems, position) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
            for (var _i = 0, currentItems_10 = currentItems; _i < currentItems_10.length; _i++) {
                var udsDoc = currentItems_10[_i];
                var udsDocumentsName = udsDoc.Relation.Title + " - " + udsDoc.Relation.Subject;
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(udsDocumentsName);
                node.set_value(udsDoc.Relation.UniqueId);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/document_copies.png");
                if (udsDoc.Relation.UDSRepository) {
                    node.set_navigateUrl("../UDS/UDSView.aspx?Type=UDS&IdUDS=" + udsDoc.Relation.UniqueId.toString() + "&IdUDSRepository=" + udsDoc.Relation.UDSRepository.UniqueId.toString());
                }
                var parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
                parentNode.get_nodes().add(node);
                this._DocumentRadTreeId.commitChanges();
            }
        };
        uscDocumentUnitReferences.prototype.showNotificationException = function (uscNotificationId, customMessage) {
            var exception;
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
        uscDocumentUnitReferences.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#" + uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscDocumentUnitReferences.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        uscDocumentUnitReferences.prototype.pad = function (currentNumber, paddingSize) {
            var s = currentNumber + "";
            while (s.length < paddingSize) {
                s = "0" + s;
            }
            return s;
        };
        return uscDocumentUnitReferences;
    }());
    return uscDocumentUnitReferences;
});
//# sourceMappingURL=uscDocumentUnitReferences.js.map