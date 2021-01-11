import { MetadataModel } from './metadata.model';

export interface ArchiveModel {
    $type: string;
    ArchiveName: string;
    Metadatas: MetadataModel[];
}