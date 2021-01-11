import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');

class TemplateDocumentFinderViewModel {
    Name: string;
    Tags: string[];
    Status: TemplateDocumentRepositoryStatus[];

    constructor() {
        this.Tags = new Array<string>();
        this.Status = new Array<TemplateDocumentRepositoryStatus>();
    }

    hasFilter(): boolean {
        return !!this.Name || this.Tags.length > 0 || this.Status.length > 0;
    }
}

export = TemplateDocumentFinderViewModel;