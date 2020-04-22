import ArticleModel = require("App/Models/ZenDesk/ArticleModel");

interface ArticleSearchModel extends ArticleModel {
    Snippet: string;
    ResultType: string;
}

export = ArticleSearchModel;