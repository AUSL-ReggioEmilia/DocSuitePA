using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using BiblosDS.Library.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Archives.Interactors
{
    public class ValidateArchiveForPreservationInteractor : IInteractor<ValidateArchiveForPreservationRequestModel, ValidateArchiveForPreservationResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly PreservationService _preservationService;
        #endregion

        #region [ Constructor ]
        public ValidateArchiveForPreservationInteractor(ILogger logger)
        {
            _logger = logger;
            _preservationService = new PreservationService();
        }
        #endregion

        #region [ Methods ]
        public ValidateArchiveForPreservationResponseModel Process(ValidateArchiveForPreservationRequestModel request)
        {
            DocumentArchive archive = ArchiveService.GetArchive(request.IdArchive);
            if (archive == null)
            {
                _logger.Error($"Archive with id {request.IdArchive} not found");
                throw new Exception($"Archive with id {request.IdArchive} not found");
            }

            ValidateArchiveForPreservationResponseModel response = new ValidateArchiveForPreservationResponseModel()
            {
                IdArchive = request.IdArchive
            };

            if (_preservationService.ExistPreservationsByArchive(archive))
            {
                response.HasPreservations = true;
                response.ValidationErrors.Add("Non è possibile configurare un archivio con delle conservazioni già eseguite");
            }

            response.HasPathPreservation = true;
            if (string.IsNullOrEmpty(archive.PathPreservation))
            {
                response.HasPathPreservation = false;
                response.ValidationErrors.Add("Non è stato definito il percorso di conservazione");
            }

            ICollection<DocumentAttribute> archiveAttributes = AttributeService.GetAttributesFromArchive(archive.IdArchive);
            response.HasDateMainAttribute = true;
            if (!archiveAttributes.Any(x => x.IsMainDate == true))
            {
                response.HasDateMainAttribute = false;
                response.ValidationErrors.Add("Non è stato definito un attributo di tipo MainDate");
            }

            response.HasPrimaryKeyAttribute = true;
            if (!archiveAttributes.Any(x => x.KeyOrder.HasValue && x.KeyOrder.Value > 0))
            {
                response.HasPrimaryKeyAttribute = false;
                response.ValidationErrors.Add("Non è stato definito un attributo di tipo Primary Key");
            }            

            return response;
        }
        #endregion        
    }
}