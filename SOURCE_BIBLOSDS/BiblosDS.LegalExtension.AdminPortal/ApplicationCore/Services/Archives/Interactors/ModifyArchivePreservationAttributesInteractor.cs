using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Archives.Interactors
{
    public class ModifyArchivePreservationAttributesInteractor : IInteractor<ModifyArchivePreservationAttributesRequestModel, EmptyResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]
        public ModifyArchivePreservationAttributesInteractor(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public EmptyResponseModel Process(ModifyArchivePreservationAttributesRequestModel request)
        {
            _logger.Info($"Process -> Edit preservation attributes for archive {request.IdArchive}");
            DocumentArchive archive = ArchiveService.GetArchive(request.IdArchive);
            if (archive == null)
            {
                _logger.Error($"Archive with id {request.IdArchive} not found");
                throw new Exception($"Archive with id {request.IdArchive} not found");
            }

            ICollection<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(request.IdArchive);
            if (attributes == null || attributes.Count == 0)
            {
                _logger.Error($"There aren't attributes for archive with id {request.IdArchive}");
                throw new Exception($"There aren't attributes for archive with id {request.IdArchive}");
            }
            
            foreach (DocumentAttribute attribute in attributes)
            {
                attribute.ConservationPosition = null;
                if (request.OrderedPreservationAttributes.ContainsKey(attribute.IdAttribute))
                {
                    attribute.ConservationPosition = request.OrderedPreservationAttributes[attribute.IdAttribute];
                }
                _logger.Debug($"Update attribute {attribute.IdAttribute}: ConservationPosition = {attribute.ConservationPosition}");
                AttributeService.UpdateAttribute(attribute);
            }
            return new EmptyResponseModel();
        }
        #endregion           
    }
}