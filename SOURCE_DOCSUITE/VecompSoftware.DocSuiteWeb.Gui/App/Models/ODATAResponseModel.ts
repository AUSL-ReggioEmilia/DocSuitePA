class ODATAResponseModel<T> {
    context: string;
    count: number;
    value: T[];

    constructor(model: any) {
        this.context = model['@odata.context'];
        this.count = model['@odata.count'];
    }
}

export = ODATAResponseModel;