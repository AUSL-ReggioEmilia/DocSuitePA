import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StartWorkflowMapper } from '@app-mappers/workflows/start-workflow.mapper';
import { IdentityContextMapper } from '@app-mappers/shared/identity-context.mapper';
import { ContactMapper } from '@app-mappers/shared/contact.mapper';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [],
    providers: [StartWorkflowMapper, IdentityContextMapper, ContactMapper]
})
export class MappersModule { }
