import { Component, Input, OnInit } from '@angular/core';

import { DocumentModel } from '../../../models/commons/document.model';

@Component({
    moduleId: module.id,
    selector: 'document-panel',
    templateUrl: 'document-panel.component.html'
})


export class DocumentPanelComponent {
    @Input() documents: DocumentModel[];
    @Input() rootLabel: string;

    rootDocumentImageUrl: string = 'app/images/documents/folder-open.png';

}