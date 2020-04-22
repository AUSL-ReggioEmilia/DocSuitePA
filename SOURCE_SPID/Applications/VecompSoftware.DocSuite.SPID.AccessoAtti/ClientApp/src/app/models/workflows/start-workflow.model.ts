import { StartWorkflowContentType } from './start-workflow-contenttype.model';
import { IdentityContextModel } from '@app-models/shared/identity-context.model';

export interface StartWorkflowModel {
    ContentType: StartWorkflowContentType;
    MessageName: string;
    MessageDate: Date;
    CustomProperties: { [key: string]: any };
    IdentityContext: IdentityContextModel;
}