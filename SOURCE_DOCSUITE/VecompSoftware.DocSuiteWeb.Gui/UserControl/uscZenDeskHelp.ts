import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import CategoryModel = require('App/Models/ZenDesk/CategoryModel');
import SectionModel = require('App/Models/ZenDesk/SectionModel');
import ArticleModel = require('App/Models/ZenDesk/ArticleModel');
import ArticleSearchModel = require('App/Models/ZenDesk/ArticleSearchModel');
import AjaxModel = require('App/Models/AjaxModel');

class uscZenDeskHelp {

    protected _serviceConfigurations: ServiceConfiguration[];

    rtbSearchId: string;
    btnRulesId: string;
    btnSolutionsId: string;
    btnDocSuiteId: string;
    btnFAQsId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    ZenDeskPaneId: string;
    rtvArticlesId: string;
    rpArticleId: string;
    btnSearchId: string;

    serializedCategories: string;
    categories: CategoryModel[];
    articles: ArticleModel[];
    selectedCategoryId: string;
    selectedCategoryButtonId: string;
    sectionsCount: number;
    articleSearchNextPage: string;
    isButtonPressed: string;

    private _rtbSearch: Telerik.Web.UI.RadTextBox;
    private _btnRules: Telerik.Web.UI.RadButton;
    private _btnSolutions: Telerik.Web.UI.RadButton;
    private _btnDocSuite: Telerik.Web.UI.RadButton;
    private _btnFAQs: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rtvArticles: Telerik.Web.UI.RadTreeView;
    private _rpArticle: Telerik.Web.UI.RadPane;
    private _btnSearch: Telerik.Web.UI.RadButton;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        this._rtbSearch = <Telerik.Web.UI.RadTextBox>$find(this.rtbSearchId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._rtvArticles = <Telerik.Web.UI.RadTreeView>$find(this.rtvArticlesId);
        this._rtvArticles.add_nodeClicked(this.rtvArticles_onNodeClick);
        this._rpArticle = <Telerik.Web.UI.RadPane>$find(this.rpArticleId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicked(this.btnSearch_Clicked);

        this._btnRules = $find(this.btnRulesId) as Telerik.Web.UI.RadButton;
        this._btnRules.add_clicked(this.btnRules_Clicked);
        document.getElementById(this.btnRulesId).style.backgroundColor = "white";
        this._btnSolutions = $find(this.btnSolutionsId) as Telerik.Web.UI.RadButton;
        this._btnSolutions.add_clicked(this.btnSolutions_Clicked);
        document.getElementById(this.btnSolutionsId).style.backgroundColor = "white";
        this._btnDocSuite = $find(this.btnDocSuiteId) as Telerik.Web.UI.RadButton;
        this._btnDocSuite.add_clicked(this.btnDocSuite_Clicked);
        document.getElementById(this.btnDocSuiteId).style.backgroundColor = "white";
        this._btnFAQs = $find(this.btnFAQsId) as Telerik.Web.UI.RadButton;
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
    }

    setCategoryByDefault(): void {
        this._btnRules.click();
    }

    btnRules_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.selectedCategoryButtonId = this.btnRulesId;
        let buttonsIds: string[] = [this.btnSolutionsId, this.btnDocSuiteId, this.btnFAQsId];
        this.setButtonsColor(this.btnRulesId, buttonsIds);
        this.sendAjaxRequest("Normativa");
    }

    btnSolutions_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.selectedCategoryButtonId = this.btnSolutionsId;
        let buttonsIds: string[] = [this.btnRulesId, this.btnDocSuiteId, this.btnFAQsId];
        this.setButtonsColor(this.btnSolutionsId, buttonsIds);
        this.sendAjaxRequest("Soluzioni");
    }

    btnDocSuite_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.selectedCategoryButtonId = this.btnDocSuiteId;
        let buttonsIds: string[] = [this.btnSolutionsId, this.btnRulesId, this.btnFAQsId];
        this.setButtonsColor(this.btnDocSuiteId, buttonsIds);
        this.sendAjaxRequest("DocSuite");
    }

    btnFAQs_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.selectedCategoryButtonId = this.btnFAQsId;
        let buttonsIds: string[] = [this.btnSolutionsId, this.btnDocSuiteId, this.btnSolutionsId];
        this.setButtonsColor(this.btnFAQsId, buttonsIds);
        this.sendAjaxRequest("FAQs");
    }

    setButtonsColor(pressedButtonId: string, buttonsIds: string[]): void {
        document.getElementById(pressedButtonId).style.background = "#dedede";
        document.getElementById(pressedButtonId).style.fontWeight = "bold";
        this.removeButtonTextCount(pressedButtonId);
        for (let buttonId of buttonsIds) {
            document.getElementById(buttonId).style.background = "white";
            document.getElementById(buttonId).style.fontWeight = "500";
            this.removeButtonTextCount(buttonId);
        }
    }

    removeButtonTextCount(buttonId: string): void {
        let button: Telerik.Web.UI.RadButton = <Telerik.Web.UI.RadButton>$find(buttonId);
        let buttonText: string = button.get_text();
        buttonText = buttonText.split(" (")[0];
        button.set_text(buttonText);
    }

    setCategoriesCountOnButtons(sectionsCount: number): void {
        let selectedCategoryButton: Telerik.Web.UI.RadButton = <Telerik.Web.UI.RadButton>$find(this.selectedCategoryButtonId);
        let selectedCategoryButtonText: string = selectedCategoryButton.get_text();
        selectedCategoryButtonText = `${selectedCategoryButtonText} (${sectionsCount.toString()})`;
        selectedCategoryButton.set_text(selectedCategoryButtonText);
    }

    sendAjaxRequest(categoryName: string): void {
        this._btnSearch.set_enabled(false);
        this._ajaxLoadingPanel.show(this.ZenDeskPaneId);
        this._rtvArticles.get_nodes().clear();
        this._rpArticle.get_element().firstElementChild.innerHTML = "";
        for (let category of this.categories) {
            if (category.Name === categoryName) {
                this.selectedCategoryId = category.Id.toString();
                break;
            }
        }
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(this.selectedCategoryId);
        ajaxModel.ActionName = "LoadSectionsAndArticles";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    loadSections(serializedSections: string): void {
        let sections: SectionModel[] = JSON.parse(this.validateJsonResult(serializedSections));
        this.setCategoriesCountOnButtons(sections.length);
        this.loadSectionList(sections);
    }

    loadSectionList(sections: SectionModel[]): void {
        for (let section of sections) {
            let rtvItem: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            rtvItem.set_text(section.Name);
            rtvItem.set_value(section.Id.toString());
            rtvItem.expand();
            this._rtvArticles.get_nodes().add(rtvItem);
        }
        this._ajaxLoadingPanel.hide(this.ZenDeskPaneId);
    }

    loadArticlesBySection(serializedArticles: string, index: string): void {
        let articles: ArticleModel[] = JSON.parse(this.validateJsonResult(serializedArticles));
        for (let article of articles) {
            this.articles.push(article);
        }
        if (this._rtvArticles.get_nodes().getItem(+index)) {
            this._rtvArticles.get_nodes().getItem(+index).get_nodes().clear();
        }
        for (let article of articles) {
            let rtvItem: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
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
    }

    setArticlesCountOnTreeView(treeViewIndex: number, articlesCount: number): void {
        let rtvArticlesNodeText: string = this._rtvArticles.get_nodes().getItem(treeViewIndex).get_text();
        rtvArticlesNodeText = `${rtvArticlesNodeText} (${articlesCount})`;
        this._rtvArticles.get_nodes().getItem(treeViewIndex).set_text(rtvArticlesNodeText);
    }

    rtvArticles_onNodeClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        if (this._rtvArticles.get_selectedNode().get_level() === 0) {
            if (this._rtvArticles.get_selectedNode().get_expanded()) {
                this._rtvArticles.get_selectedNode().collapse();
            }
            else {
                this._rtvArticles.get_selectedNode().expand();
            }
        }
        if (this._rtvArticles.get_selectedNode().get_level() === 1) {
            let rwArticleContent: string;
            for (let article of this.articles) {
                if (article.Id.toString() === this._rtvArticles.get_selectedNode().get_value()) {
                    rwArticleContent = article.Body;
                    break;
                }
            }
            this._rpArticle.get_element().firstElementChild.innerHTML = rwArticleContent;
        }
    }

    btnSearch_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._rtbSearch.get_textBoxValue() === "Cerca...") {
            return;
        }
        this._btnSearch.set_enabled(false);
        this._ajaxLoadingPanel.show(this.ZenDeskPaneId);
        this._rpArticle.get_element().firstElementChild.innerHTML = "";

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(this.selectedCategoryId);
        ajaxModel.Value.push(this._rtbSearch.get_textBoxValue());
        ajaxModel.ActionName = "LoadSearchedArticles";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    loadSearchedArticles(serializedArticles: string): void {
        this.loadSearchedArticlesPanel(serializedArticles, true);
    }

    showArticle(articleId: number): void {
        let rwArticleContent: string;
        for (let article of this.articles) {
            if (article.Id === articleId) {
                rwArticleContent = article.Body;
                break;
            }
        }
        this._rpArticle.get_element().firstElementChild.innerHTML = rwArticleContent;
    }

    showMoreArticles(nextPageLink: string): void {
        this._ajaxLoadingPanel.show(this.ZenDeskPaneId);
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(nextPageLink);
        ajaxModel.ActionName = "LoadArticleSearchNextPage";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    loadNextSearchedArticles(serializedArticles: string): void {
        this.loadSearchedArticlesPanel(serializedArticles, false);
    }

    loadSearchedArticlesPanel(serializedArticles: string, isFirstPage: boolean): void {
        let articles: ArticleSearchModel[] = JSON.parse(this.validateJsonResult(serializedArticles));
        let searchedArticlesElement: string = "";
        if (!isFirstPage) {
            searchedArticlesElement = this._rpArticle.get_element().firstElementChild.innerHTML;
            searchedArticlesElement = searchedArticlesElement.replace(this._rpArticle.get_element().firstElementChild.lastElementChild.outerHTML, "");
        }
        for (let article of articles) {
            searchedArticlesElement += `<div class="article-container"><a class="article-link" onclick="zenDesk.showArticle(${article.Id})">${article.Title}</a><br/><p class="article-link-paragraph">${article.Snippet}</p><hr/></div>`;
        }
        if (this.articleSearchNextPage) {
            searchedArticlesElement += `<div class="article-container" id="moreArticles"><a onclick="zenDesk.showMoreArticles('${this.articleSearchNextPage}')"><div class="more-articles-link">Carica più articoli</div></a></div>`;
        }
        this._rpArticle.get_element().firstElementChild.innerHTML = searchedArticlesElement;
        this._ajaxLoadingPanel.hide(this.ZenDeskPaneId);
        this._btnSearch.set_enabled(true);
    }

    setArticlesNextPage(nextPage: string): void {
        this.articleSearchNextPage = nextPage;
    }

    validateJsonResult(result: string): string {
        return result.replace(/""/g, "\"");
    }

    getSearchTextByPageUrl(pageUrl: string): string {
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
    }
}

export = uscZenDeskHelp;