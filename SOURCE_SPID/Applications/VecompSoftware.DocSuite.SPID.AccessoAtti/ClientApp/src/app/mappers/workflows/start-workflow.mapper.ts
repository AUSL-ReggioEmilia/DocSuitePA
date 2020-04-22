import { Injectable } from '@angular/core';

import { BaseMapper } from '@app-mappers/base.mapper';
import { IdentityContextMapper } from '@app-mappers/shared/identity-context.mapper';
import { ContactMapper } from '@app-mappers/shared/contact.mapper';
import {
    StartWorkflowModel,
    StartWorkflowContentType,
    IdentityContextModel, 
    AuthorizationType,
    RoleModel,
    ContactModel,
    MetadataModel,
    WorkflowModel,
    WorkflowParameterModel,
    ArchiveModel,
    WorkflowRequestState
} from '@app-models/_index';
import { NewRequestViewModel } from '@app-viewmodels/requests/new-request.viewmodel';
import { WorkflowParameterName } from '@app-helpers/workflows/workflow-parameter-name.helper';
import { ConfigurationService } from '@app-services/shared/configuration.service';

@Injectable()
export class StartWorkflowMapper extends BaseMapper<NewRequestViewModel, StartWorkflowModel> {

    constructor(private identityContextMapper: IdentityContextMapper, 
                private contactMapper: ContactMapper, private configurationService: ConfigurationService) {
        super();
    }

    map(source: NewRequestViewModel): StartWorkflowModel {
        let identity: IdentityContextModel = this.identityContextMapper.map(source);

        let role: RoleModel = <RoleModel>{};
        role.AuthorizationType = AuthorizationType.DocSuiteSecurity;
        identity.Roles = [role];

        let workflowmodel: WorkflowModel = <WorkflowModel>{};
        workflowmodel.ActivityTitlePrefix = '';
        workflowmodel.Name = this.configurationService.config.WorkflowName;
        workflowmodel.WorkflowParameters = new Array<WorkflowParameterModel>();

        let archive: ArchiveModel = <ArchiveModel>{
            $type: 'VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters.ArchiveModel, VecompSoftware.DocSuite.Public.Core.Models',
            ArchiveName: this.configurationService.config.ArchiveName,
            Metadatas: new Array<MetadataModel>()            
        };

        //Richiedente
        let contact: ContactModel = this.contactMapper.map(source);
        let contactParameter: WorkflowParameterModel = <WorkflowParameterModel>{
            $type: 'VecompSoftware.DocSuite.Public.Core.Models.Workflows.WorkflowParameterModel, VecompSoftware.DocSuite.Public.Core.Models',
            ParameterModel: contact,
            ParameterName: WorkflowParameterName.CONTACT            
        };
        workflowmodel.WorkflowParameters.push(contactParameter);

        //Data di invio
        let sendDateMetadata: MetadataModel = this.createMetadata('Data invio', new Date());
        let userTypeMetadata: MetadataModel = this.createMetadata('Tipologia utente', source.UserType);
        let accessTypeMetadata: MetadataModel = this.createMetadata('Tipologia accesso', source.AccessType);
        let documentsMetadata: MetadataModel = this.createMetadata('Documenti', source.Documents);
        let motivationsMetadata: MetadataModel = this.createMetadata('Motivazioni', source.Motivations);
        let returnInstanceMetadata: MetadataModel = this.createMetadata('Ritorno documentazione', source.ReturnType);
        let statusMetadata: MetadataModel = this.createMetadata('Stato richiesta', WorkflowRequestState.requested);
        let channelMetadata: MetadataModel = this.createMetadata('Canale comunicazione', 0);

        archive.Metadatas.push(sendDateMetadata);        
        archive.Metadatas.push(userTypeMetadata);
        archive.Metadatas.push(accessTypeMetadata);
        archive.Metadatas.push(documentsMetadata);
        archive.Metadatas.push(motivationsMetadata);
        archive.Metadatas.push(returnInstanceMetadata);
        archive.Metadatas.push(statusMetadata);
        archive.Metadatas.push(channelMetadata);

        let parameter: WorkflowParameterModel = <WorkflowParameterModel>{
            $type: 'VecompSoftware.DocSuite.Public.Core.Models.Workflows.WorkflowParameterModel, VecompSoftware.DocSuite.Public.Core.Models',
            ParameterModel: archive,
            ParameterName: WorkflowParameterName.METADATA            
        };
        workflowmodel.WorkflowParameters.push(parameter);

        let contentType: StartWorkflowContentType = <StartWorkflowContentType>{};
        contentType.Content = workflowmodel;

        let mapped: StartWorkflowModel = <StartWorkflowModel>{};
        mapped.ContentType = contentType;
        mapped.IdentityContext = identity;
        mapped.MessageDate = new Date();

        return mapped;
    }

    private createMetadata(metadataName: string, metadataValue: any): MetadataModel {
        let metadata: MetadataModel = <MetadataModel>{
            KeyName: metadataName,
            Value: metadataValue            
        };
        return metadata;
    }
}
