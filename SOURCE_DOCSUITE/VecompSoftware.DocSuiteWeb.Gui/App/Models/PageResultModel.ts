
class PageResultModel {
    items: object[];
    count: number;

    constructor(model: any) {
        this.items = model.Items['$values'];
        this.count = model.Count;
    }

}

export = PageResultModel;