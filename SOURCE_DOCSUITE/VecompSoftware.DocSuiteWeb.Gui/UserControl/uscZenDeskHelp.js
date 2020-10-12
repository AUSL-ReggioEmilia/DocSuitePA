define(["require", "exports"], function (require, exports) {
    var uscZenDeskHelp = /** @class */ (function () {
        function uscZenDeskHelp(serviceConfigurations) {
            var _this = this;
            this.btnRules_Clicked = function (sender, eventArgs) {
                _this.selectedCategoryButtonId = _this.btnRulesId;
                var buttonsIds = [_this.btnSolutionsId, _this.btnDocSuiteId, _this.btnFAQsId];
                _this.setButtonsColor(_this.btnRulesId, buttonsIds);
                _this.sendAjaxRequest("Normativa");
            };
            this.btnSolutions_Clicked = function (sender, eventArgs) {
                _this.selectedCategoryButtonId = _this.btnSolutionsId;
                var buttonsIds = [_this.btnRulesId, _this.btnDocSuiteId, _this.btnFAQsId];
                _this.setButtonsColor(_this.btnSolutionsId, buttonsIds);
                _this.sendAjaxRequest("Soluzioni");
            };
            this.btnDocSuite_Clicked = function (sender, eventArgs) {
                _this.selectedCategoryButtonId = _this.btnDocSuiteId;
                var buttonsIds = [_this.btnSolutionsId, _this.btnRulesId, _this.btnFAQsId];
                _this.setButtonsColor(_this.btnDocSuiteId, buttonsIds);
                _this.sendAjaxRequest("DocSuite PA");
            };
            this.btnFAQs_Clicked = function (sender, eventArgs) {
                _this.selectedCategoryButtonId = _this.btnFAQsId;
                var buttonsIds = [_this.btnSolutionsId, _this.btnDocSuiteId, _this.btnRulesId];
                _this.setButtonsColor(_this.btnFAQsId, buttonsIds);
                _this.sendAjaxRequest("FAQs");
            };
            this.rtvArticles_onNodeClick = function (sender, args) {
                if (_this._rtvArticles.get_selectedNode().get_level() === 0) {
                    if (_this._rtvArticles.get_selectedNode().get_expanded()) {
                        _this._rtvArticles.get_selectedNode().collapse();
                    }
                    else {
                        _this._rtvArticles.get_selectedNode().expand();
                    }
                }
                if (_this._rtvArticles.get_selectedNode().get_level() === 1) {
                    var rwArticleContent = void 0;
                    for (var _i = 0, _a = _this.articles; _i < _a.length; _i++) {
                        var article = _a[_i];
                        if (article.Id.toString() === _this._rtvArticles.get_selectedNode().get_value()) {
                            rwArticleContent = article.Body;
                            break;
                        }
                    }
                    _this._rpArticle.get_element().firstElementChild.innerHTML = rwArticleContent;
                }
            };
            this.btnSearch_Clicked = function (sender, eventArgs) {
                if (_this._rtbSearch.get_textBoxValue() === "Cerca...") {
                    return;
                }
                _this._btnSearch.set_enabled(false);
                _this._ajaxLoadingPanel.show(_this.ZenDeskPaneId);
                _this._rpArticle.get_element().firstElementChild.innerHTML = "";
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.Value.push(_this.selectedCategoryId);
                ajaxModel.Value.push(_this._rtbSearch.get_textBoxValue());
                ajaxModel.ActionName = "LoadSearchedArticles";
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        uscZenDeskHelp.prototype.initialize = function () {
            this._rtbSearch = $find(this.rtbSearchId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._rtvArticles = $find(this.rtvArticlesId);
            this._rtvArticles.add_nodeClicked(this.rtvArticles_onNodeClick);
            this._rpArticle = $find(this.rpArticleId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_Clicked);
            this._btnRules = $find(this.btnRulesId);
            this._btnRules.add_clicked(this.btnRules_Clicked);
            document.getElementById(this.btnRulesId).style.backgroundColor = "white";
            this._btnSolutions = $find(this.btnSolutionsId);
            this._btnSolutions.add_clicked(this.btnSolutions_Clicked);
            document.getElementById(this.btnSolutionsId).style.backgroundColor = "white";
            this._btnDocSuite = $find(this.btnDocSuiteId);
            this._btnDocSuite.add_clicked(this.btnDocSuite_Clicked);
            document.getElementById(this.btnDocSuiteId).style.backgroundColor = "white";
            this._btnFAQs = $find(this.btnFAQsId);
            this._btnFAQs.add_clicked(this.btnFAQs_Clicked);
            document.getElementById(this.btnFAQsId).style.backgroundColor = "white";
            this.categories = JSON.parse(this.validateJsonResult(this.serializedCategories));
            this.articles = [];
            if (localStorage.getItem("CurrentPageUrl") && this.isButtonPressed === "True") {
                this._rtbSearch.set_textBoxValue(this.getSearchTextByPageUrl(localStorage.getItem("CurrentPageUrl")));
                localStorage.removeItem("CurrentPageUrl");
                this._btnDocSuite.click();
            }
            else {
                this.setCategoryByDefault();
            }
        };
        uscZenDeskHelp.prototype.setCategoryByDefault = function () {
            this._btnRules.click();
        };
        uscZenDeskHelp.prototype.setButtonsColor = function (pressedButtonId, buttonsIds) {
            document.getElementById(pressedButtonId).style.background = "#dedede";
            document.getElementById(pressedButtonId).style.fontWeight = "bold";
            this.removeButtonTextCount(pressedButtonId);
            for (var _i = 0, buttonsIds_1 = buttonsIds; _i < buttonsIds_1.length; _i++) {
                var buttonId = buttonsIds_1[_i];
                document.getElementById(buttonId).style.background = "white";
                document.getElementById(buttonId).style.fontWeight = "500";
                this.removeButtonTextCount(buttonId);
            }
        };
        uscZenDeskHelp.prototype.removeButtonTextCount = function (buttonId) {
            var button = $find(buttonId);
            var buttonText = button.get_text();
            buttonText = buttonText.split(" (")[0];
            button.set_text(buttonText);
        };
        uscZenDeskHelp.prototype.setCategoriesCountOnButtons = function (sectionsCount) {
            var selectedCategoryButton = $find(this.selectedCategoryButtonId);
            var selectedCategoryButtonText = selectedCategoryButton.get_text();
            selectedCategoryButtonText = selectedCategoryButtonText + " (" + sectionsCount.toString() + ")";
            selectedCategoryButton.set_text(selectedCategoryButtonText);
        };
        uscZenDeskHelp.prototype.sendAjaxRequest = function (categoryName) {
            this._btnSearch.set_enabled(false);
            this._ajaxLoadingPanel.show(this.ZenDeskPaneId);
            this._rtvArticles.get_nodes().clear();
            this._rpArticle.get_element().firstElementChild.innerHTML = "";
            for (var _i = 0, _a = this.categories; _i < _a.length; _i++) {
                var category = _a[_i];
                if (category.Name === categoryName) {
                    this.selectedCategoryId = category.Id.toString();
                    break;
                }
            }
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(this.selectedCategoryId);
            ajaxModel.ActionName = "LoadSectionsAndArticles";
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        uscZenDeskHelp.prototype.loadSections = function (serializedSections) {
            var sections = JSON.parse(this.validateJsonResult(serializedSections));
            this.setCategoriesCountOnButtons(sections.length);
            this.loadSectionList(sections);
        };
        uscZenDeskHelp.prototype.loadSectionList = function (sections) {
            for (var _i = 0, sections_1 = sections; _i < sections_1.length; _i++) {
                var section = sections_1[_i];
                var rtvItem = new Telerik.Web.UI.RadTreeNode();
                rtvItem.set_text(section.Name);
                rtvItem.set_value(section.Id.toString());
                rtvItem.expand();
                this._rtvArticles.get_nodes().add(rtvItem);
            }
            this._ajaxLoadingPanel.hide(this.ZenDeskPaneId);
        };
        uscZenDeskHelp.prototype.loadArticlesBySection = function (serializedArticles, index) {
            var articles = JSON.parse(this.validateJsonResult(serializedArticles));
            for (var _i = 0, articles_1 = articles; _i < articles_1.length; _i++) {
                var article = articles_1[_i];
                this.articles.push(article);
            }
            if (this._rtvArticles.get_nodes().getItem(+index)) {
                this._rtvArticles.get_nodes().getItem(+index).get_nodes().clear();
            }
            for (var _a = 0, articles_2 = articles; _a < articles_2.length; _a++) {
                var article = articles_2[_a];
                var rtvItem = new Telerik.Web.UI.RadTreeNode();
                rtvItem.set_text(article.Name);
                rtvItem.set_value(article.Id.toString());
                rtvItem.set_imageUrl("../Comm/Images/Home/Punto.gif");
                rtvItem.set_cssClass("tree-node");
                this._rtvArticles.get_nodes().getItem(+index).get_nodes().add(rtvItem);
            }
            this.setArticlesCountOnTreeView(+index, articles.length);
            this._ajaxLoadingPanel.hide(this.ZenDeskPaneId);
            this._btnSearch.set_enabled(true);
            if (this._rtbSearch.get_textBoxValue() !== "Cerca...") {
                this._btnSearch.click();
            }
        };
        uscZenDeskHelp.prototype.setArticlesCountOnTreeView = function (treeViewIndex, articlesCount) {
            var rtvArticlesNodeText = this._rtvArticles.get_nodes().getItem(treeViewIndex).get_text();
            rtvArticlesNodeText = rtvArticlesNodeText + " (" + articlesCount + ")";
            this._rtvArticles.get_nodes().getItem(treeViewIndex).set_text(rtvArticlesNodeText);
        };
        uscZenDeskHelp.prototype.loadSearchedArticles = function (serializedArticles) {
            this.loadSearchedArticlesPanel(serializedArticles, true);
        };
        uscZenDeskHelp.prototype.showArticle = function (articleId) {
            var rwArticleContent;
            for (var _i = 0, _a = this.articles; _i < _a.length; _i++) {
                var article = _a[_i];
                if (article.Id === articleId) {
                    rwArticleContent = article.Body;
                    break;
                }
            }
            this._rpArticle.get_element().firstElementChild.innerHTML = rwArticleContent;
        };
        uscZenDeskHelp.prototype.showMoreArticles = function (nextPageLink) {
            this._ajaxLoadingPanel.show(this.ZenDeskPaneId);
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(nextPageLink);
            ajaxModel.ActionName = "LoadArticleSearchNextPage";
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        uscZenDeskHelp.prototype.loadNextSearchedArticles = function (serializedArticles) {
            this.loadSearchedArticlesPanel(serializedArticles, false);
        };
        uscZenDeskHelp.prototype.loadSearchedArticlesPanel = function (serializedArticles, isFirstPage) {
            var articles = JSON.parse(this.validateJsonResult(serializedArticles));
            var searchedArticlesElement = "";
            if (!isFirstPage) {
                searchedArticlesElement = this._rpArticle.get_element().firstElementChild.innerHTML;
                searchedArticlesElement = searchedArticlesElement.replace(this._rpArticle.get_element().firstElementChild.lastElementChild.outerHTML, "");
            }
            for (var _i = 0, articles_3 = articles; _i < articles_3.length; _i++) {
                var article = articles_3[_i];
                searchedArticlesElement += "<div class=\"article-container\"><a class=\"article-link\" onclick=\"zenDesk.showArticle(" + article.Id + ")\">" + article.Title + "</a><br/><p class=\"article-link-paragraph\">" + article.Snippet + "</p><hr/></div>";
            }
            if (this.articleSearchNextPage) {
                searchedArticlesElement += "<div class=\"article-container\" id=\"moreArticles\"><a onclick=\"zenDesk.showMoreArticles('" + this.articleSearchNextPage + "')\"><div class=\"more-articles-link\">Carica pi\u00F9 articoli</div></a></div>";
            }
            this._rpArticle.get_element().firstElementChild.innerHTML = searchedArticlesElement;
            this._ajaxLoadingPanel.hide(this.ZenDeskPaneId);
            this._btnSearch.set_enabled(true);
        };
        uscZenDeskHelp.prototype.setArticlesNextPage = function (nextPage) {
            this.articleSearchNextPage = nextPage;
        };
        uscZenDeskHelp.prototype.validateJsonResult = function (result) {
            return result.replace(/""/g, "\"");
        };
        uscZenDeskHelp.prototype.getSearchTextByPageUrl = function (pageUrl) {
            if (pageUrl.indexOf("UserDesktop") >= 0) {
                return "Scrivania";
            }
            if (pageUrl.indexOf("UserProfile") >= 0) {
                return "Configurazioni utente";
            }
            if (pageUrl.indexOf("UserColl") >= 0 || pageUrl.indexOf("Collaboration") >= 0) {
                return "Collaborazione";
            }
            if (pageUrl.indexOf("Docm/") >= 0) {
                return "Pratiche";
            }
            if (pageUrl.indexOf("Prot/") >= 0) {
                return "Protocollo";
            }
            if (pageUrl.indexOf("Series/") >= 0 || pageUrl.indexOf("Task/") >= 0) {
                return "Serie";
            }
            if (pageUrl.indexOf("Monitors/") >= 0) {
                return "Amministrazione trasparente";
            }
            if (pageUrl.indexOf("UDSInsert") >= 0 || pageUrl.indexOf("UDSSearch") >= 0 || pageUrl.indexOf("UDSDesigner") >= 0) {
                return "Archivi";
            }
            if (pageUrl.indexOf("Dossiers/") >= 0) {
                return "Dossier";
            }
            if (pageUrl.indexOf("Fasc/") >= 0) {
                return "Fascicoli";
            }
            if (pageUrl.indexOf("UDS/UDSInvoiceSearch") >= 0 || pageUrl.indexOf("PEC/PECInvoice") >= 0) {
                return "Fatturzaione elettronica";
            }
            if (pageUrl.indexOf("Resl/") >= 0) {
                return "Delibere";
            }
            if (pageUrl.indexOf("PEC/") >= 0) {
                return "PEC";
            }
            if (pageUrl.indexOf("Tblt/") >= 0) {
                return "Tabelle";
            }
            if (pageUrl.indexOf("Utlt/") >= 0) {
                return "Amministrazione";
            }
            return "";
        };
        return uscZenDeskHelp;
    }());
    return uscZenDeskHelp;
});
//# sourceMappingURL=uscZenDeskHelp.js.map