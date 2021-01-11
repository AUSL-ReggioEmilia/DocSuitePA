import { ProtocolModel } from './models/protocols/protocol.model';
import { ProtocolAuthorizedModel } from './models/protocols/protocol-authorized.model';
import { DocumentSeriesItemModel } from './models/document-series/document-series-item.model';
import { FascicleModel } from './models/fascicles/fascicle.model';
import { ErrorLogModel } from './models/commons/error-log.model';
import { DocumentModel } from './models/commons/document.model';
import { AuthenticationModel } from './models/commons/authentication.model';
import { PECMailModel } from './models/pec-mails/pec-mail.model';
import { DocumentUnitModel } from './models/document-units/document-unit.model';
import { ProtocolAuthorizedSearchModel } from './models/protocols/protocol-authorized-search.model';

import { ProtocolMapper } from './mappers/protocols/protocol.mapper';
import { ProtocolSectorMapper } from './mappers/protocols/protocol-sector.mapper';
import { ProtocolUserMapper } from './mappers/protocols/protocol-user.mapper';
import { ProtocolContactMapper } from './mappers/protocols/protocol-contact.mapper';
import { CategoryMapper } from './mappers/commons/category.mapper';
import { ContainerMapper } from './mappers/commons/container.mapper';
import { DocumentMapper } from './mappers/commons/document.mapper';
import { DocumentUnitMapper } from './mappers/document-units/document-unit.mapper';
import { PECMailMapper } from './mappers/pec-mails/pec-mail.mapper';
import { PECMailReceiptMapper } from './mappers/pec-mails/pec-mail-receipt.mapper';
import { FascicleMapper } from './mappers/fascicles/fascicle.mapper';
import { FascicleContactMapper } from './mappers/fascicles/fascicle-contact.mapper';
import { AuthenticationMapper } from './mappers/commons/authentication.mapper';

import { ProtocolService } from './services/protocols/protocol.service';
import { PECMailService } from './services/pec-mails/pec-mail.service';
import { DocumentService } from './services/commons/document.service';
import { DocumentUnitService } from './services/document-units/document-unit.service';
import { FascicleService } from './services/fascicles/fascicle.service';
import { LoadingSpinnerService } from './services/commons/loading-spinner.service';
import { ErrorLogService } from './services/error-log.service';
import { FascicleMenuResolve } from './services/menu/fascicle-menu.resolve';
import { AuthenticationService } from './services/commons/authentication.service';
import { AppConfigService } from './services/commons/app-config.service';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home.component';
import { ErrorPageComponent } from './components/commons/error-page.component';
import { LoadingComponent } from './components/commons/loading.component';
import { FascicleMenuComponent } from './components/menu/fascicle-menu.component';
import { DocumentComponent } from './components/commons/document.component';
import { NodeComponent } from './components/templates/tree-list/node.component';
import { TreeComponent } from './components/templates/tree-list/tree.component';
import { PecPanelComponent } from './components/templates/pec-panel/pec-panel.component';
import { PecInfoComponent } from './components/templates/pec-panel/pec-info.component';
import { PecTraceComponent } from './components/templates/pec-panel/pec-trace.component';
import { FascicleSummaryComponent } from './components/fascicles/fascicle-summary.component';
import { DocumentPanelComponent } from './components/templates/document-panel/document-panel.component';
import { AuthenticationComponent } from './components/commons/authentication.component';

import { ExceptionHandlerHelper } from './helpers/exception-handler.helper';
import { BaseHelper } from './helpers/base.helper';
import { GlobalSetting } from './settings/global.setting';
import { UploadComponentComponent } from './components/templates/upload-component/upload-component.component';
import { CommunicationService } from './services/commons/communication.service';
import { DossiersComponent } from './components/dossiers/dossiers.component';
import { DossierService } from './services/dossiers/dossier.service';
import { DossierResolve } from './resolvers/dossiers/dossiers.resolve';
import { DossierMapper } from './mappers/dossiers/dossier.mapper';
import { DossierFolderMapper } from './mappers/dossiers/dossier-folder.mapper';
import { DossierModel } from './models/dossiers/dossier.model';
import { DossierFolderModel } from './models/dossiers/dossier-folder.model';
import { FascicleFolderModel } from './models/fascicles/fascicle-folder.model';
import { FascicleFolderMapper } from './mappers/fascicles/fascicle-folder.mapper';
import { DossierSummaryComponent } from './components/dossiers/dossier-summary.component';
import { DossierRoleModel } from './models/dossiers/dossier-role.model';
import { RoleModel } from './models/roles/role.model';
import { DossierRoleMapper } from './mappers/dossiers/dossier-role.mapper';
import { RoleMapper } from './mappers/roles/role.mapper';
import { ProtocolSummaryComponent } from './components/protocols/protocol-summary.component';
import { ItemType } from './helpers/item-type.helper';
import { BFSTreeNode } from './helpers/tree-node.helper';
import { AdaptedBFSTree } from './helpers/adapted-bfs-tree.helper';
import { DossierFolderService } from './services/dossiers/dossier-folder.service';
import { FascicleFolderService } from './services/fascicles/fascicle-folder.service';



export type BaseModelType = ProtocolModel | DocumentSeriesItemModel | FascicleModel | ErrorLogModel | DocumentModel | PECMailModel | DocumentUnitModel | AuthenticationModel | ProtocolAuthorizedModel | ProtocolAuthorizedSearchModel | DossierModel | DossierFolderModel | FascicleFolderModel | DossierRoleModel | RoleModel | ItemType | BFSTreeNode | AdaptedBFSTree;

//mappers
export const mappers = [ProtocolMapper, ProtocolSectorMapper, ProtocolUserMapper,
    ProtocolContactMapper, ContainerMapper, CategoryMapper, DocumentMapper,
    PECMailMapper, PECMailReceiptMapper, FascicleMapper, FascicleContactMapper,
    DocumentUnitMapper, AuthenticationMapper,
    DossierMapper, DossierFolderMapper, FascicleFolderMapper, DossierRoleMapper, RoleMapper];

//services
export const services = [ErrorLogService, ProtocolService, DocumentService,
    PECMailService, LoadingSpinnerService,
    FascicleService, FascicleMenuResolve, DocumentUnitService,
    AuthenticationService, AppConfigService,
    CommunicationService, DossierService, DossierResolve, DossierFolderService, FascicleFolderService
    ];

//components
export const components = [
    AppComponent, HomeComponent,
    ErrorPageComponent, LoadingComponent, TreeComponent, NodeComponent,
    PecInfoComponent, PecTraceComponent, PecPanelComponent, DocumentComponent,
    FascicleMenuComponent, FascicleSummaryComponent, DocumentPanelComponent, AuthenticationComponent,
    UploadComponentComponent, DossiersComponent, DossierSummaryComponent, ProtocolSummaryComponent
];

//helpers
export const helpers = [ExceptionHandlerHelper, BaseHelper, GlobalSetting];



