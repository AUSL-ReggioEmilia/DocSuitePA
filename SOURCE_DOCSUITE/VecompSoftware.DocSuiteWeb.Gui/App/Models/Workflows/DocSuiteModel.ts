interface DocSuiteModel {
    Title: string;
    Year?: number;
    Number?: number;
    UniqueId: string;
    EntityId?: number;
    CustomProperties: { [key: string]: string; };
    ModelType: string;
    ModelStatus: string;
}

export = DocSuiteModel;