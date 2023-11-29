import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');
import TemplateCollaborationUserModelMapper = require('App/Mappers/Templates/TemplateCollaborationUserModelMapper');

class TemplateCollaborationModelMapper extends BaseMapper<TemplateCollaborationModel>{

    public Map(source: any): TemplateCollaborationModel {

        let target = new TemplateCollaborationModel();

        const _roleModelMapper: RoleModelMapper = RequireJSHelper.getModule<RoleModelMapper>(RoleModelMapper, 'App/Mappers/Commons/RoleModelMapper');
        const _templateCollaborationUserModelMapper: TemplateCollaborationUserModelMapper = RequireJSHelper.getModule<TemplateCollaborationUserModelMapper>(TemplateCollaborationUserModelMapper, 'App/Mappers/Templates/TemplateCollaborationUserModelMapper');

        target.UniqueId = source.UniqueId;
        target.Name = source.Name;
        target.Status = source.Status;
        target.DocumentType = source.DocumentType;
        target.IdPriority = source.IdPriority;
        target.Object = source.Object;
        target.Note = source.Note;
        target.IsLocked = source.IsLocked;
        target.RegistrationUser = source.RegistrationUser ;
        target.RegistrationDate = source.RegistrationDate;
        target.JsonParameters = source.JsonParameters; 
        target.RepresentationType = source.RepresentationType;
        target.TemplateCollaborationLevel = source.TemplateCollaborationLevel;
        target.TemplateCollaborationPath = source.TemplateCollaborationPath;

        target.TemplateCollaborationUsers = source.TemplateCollaborationUsers ? _templateCollaborationUserModelMapper.MapCollection(source.TemplateCollaborationUsers) : [];
        target.Roles = source.Roles ? _roleModelMapper.MapCollection(source.Roles) : [];

        return target;
    }
}

export = TemplateCollaborationModelMapper;