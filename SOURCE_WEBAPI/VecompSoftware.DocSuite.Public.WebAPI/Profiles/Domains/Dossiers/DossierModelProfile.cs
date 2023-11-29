using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Dossiers
{
    public class DossierModelProfile : Profile
    {
        #region [ Constructor ]
        public DossierModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<Dossier, DossierModel>()
                .AfterMap((src, dest) => dest.DossierFolders = GetDossierFolderModels(src.DossierFolders))
                .AfterMap((src, dest) => dest.Roles = GetDossierRoleModels(src.DossierRoles))
                .AfterMap((src, dest) => dest.Title = src.DossierFolders.First().Name.Replace("Dossier ", string.Empty))
                .AfterMap((src, dest) => dest.RegistrationUser = security.GetUser(src.RegistrationUser).DisplayName)
                .AfterMap((src, dest) => dest.LastChangedUser = src.LastChangedUser != null ? security.GetUser(src.LastChangedUser).DisplayName : null);
        }
        #endregion

        #region [ Helpers ]
        private DossierFolderModel GetDossierFolderModel(DossierFolder dossierFolder)
        {
            DossierFolderModel dossierFolderModel = new DossierFolderModel
            {
                IdCategory = dossierFolder.Category != null ? dossierFolder.Category.EntityShortId : (short?)null,
                IdDossier = dossierFolder.Dossier != null ? dossierFolder.Dossier.UniqueId : (Guid?)null,
                IdFascicle = dossierFolder.Fascicle != null ? dossierFolder.Fascicle.UniqueId : (Guid?)null,
                IdRole = dossierFolder.DossierFolderRoles != null && dossierFolder.DossierFolderRoles.Count > 0 ? dossierFolder.DossierFolderRoles.First().EntityShortId : (short?)null,
                JsonMetadata = dossierFolder.JsonMetadata,
                Name = dossierFolder.Name,
                Status = (DocSuiteWeb.Model.Entities.Dossiers.DossierFolderStatus)dossierFolder.Status,
                UniqueId = dossierFolder.UniqueId,
                DossierFolderLevel = dossierFolder.DossierFolderLevel,
                DossierFolderPath = dossierFolder.DossierFolderPath
            };
            return dossierFolderModel;
        }

        private ICollection<DossierFolderModel> GetDossierFolderModels(ICollection<DossierFolder> dossierFolders)
        {
            ICollection<DossierFolderModel> dossierFolderModels = new List<DossierFolderModel>();
            foreach (DossierFolder dossierFolder in dossierFolders)
            {
                dossierFolderModels.Add(GetDossierFolderModel(dossierFolder));
            }
            return dossierFolderModels;
        }

        private RoleModel GetRoleModel(Role role)
        {
            RoleModel roleModel = new RoleModel
            {
                AuthorizationType = DocSuiteWeb.Model.Commons.AuthorizationRoleType.Accounted,
                FullIncrementalPath = role.FullIncrementalPath,
                IdRole = role.EntityShortId,
                EntityShortId = role.EntityShortId,
                Name = role.Name,
                RoleLabel = string.Empty,
                UniqueId = role.UniqueId
            };
            return roleModel;
        }

        private DossierRoleModel GetDossierRoleModel(DossierRole dossierRole)
        {
            DossierRoleModel dossierRoleModel = new DossierRoleModel
            {
                Role = GetRoleModel(dossierRole.Role),
                Type = (DocSuiteWeb.Model.Commons.AuthorizationRoleType)dossierRole.AuthorizationRoleType,
                UniqueId = dossierRole.UniqueId
            };
            return dossierRoleModel;
        }

        private ICollection<DossierRoleModel> GetDossierRoleModels(ICollection<DossierRole> dossierRoles)
        {
            ICollection<DossierRoleModel> dossierRoleModels = new List<DossierRoleModel>();
            foreach (DossierRole dossierRole in dossierRoles)
            {
                dossierRoleModels.Add(GetDossierRoleModel(dossierRole));
            }
            return dossierRoleModels;
        }
        #endregion
    }
}