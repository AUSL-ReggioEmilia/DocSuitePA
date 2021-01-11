using AutoMapper;
using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.Dossiers
{
    public class DossierFolderModelProfile : Profile
    {
        #region [ Constructor ]
        public DossierFolderModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            CreateMap<DossierFolder, DossierFolderModel>()
                .AfterMap((src, dest) => dest = GetDossierFolderModel(src));
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
                IdRole = dossierFolder.DossierFolderRoles != null && dossierFolder.DossierFolderRoles.Count > 0 ? dossierFolder.DossierFolderRoles.First().EntityShortId : (short)0,
                Status = (DocSuiteWeb.Model.Entities.Dossiers.DossierFolderStatus)dossierFolder.Status
            };
            return dossierFolderModel;
        }
        #endregion
    }
}