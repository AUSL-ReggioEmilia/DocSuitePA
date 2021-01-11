import MetadataRepositoryStatus = require('App/Models/Commons/MetadataRepositoryStatus');

interface MetadataRepositoryViewModel {
    UniqueId: string;
    Name: string;
    Status: MetadataRepositoryStatus;
}
export = MetadataRepositoryViewModel;