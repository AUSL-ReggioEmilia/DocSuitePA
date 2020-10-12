using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Archives;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Archives.Interactors
{
    public class ModifyArchivePreservationConfigurationInteractor : IInteractor<ModifyArchivePreservationConfigurationRequestModel, EmptyResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private const string DEFAULT_MAIN_DATE_ATTRIBUTE_NAME = "Date";
        private const string DATE_TIME_ATTRIBUTE_TYPE = "System.DateTime";
        #endregion

        #region [ Constructor ]
        public ModifyArchivePreservationConfigurationInteractor(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public EmptyResponseModel Process(ModifyArchivePreservationConfigurationRequestModel request)
        {
            _logger.Info($"Process -> Edit preservation configuration for archive {request.IdArchive}");
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

            if (archive.PathPreservation != request.PathPreservation)
            {
                _logger.Debug($"Update archive preservation path from '{archive.PathPreservation}' to '{request.PathPreservation}'");
                archive.PathPreservation = request.PathPreservation;
                ArchiveService.UpdateArchive(archive, false);
            }

            foreach (DocumentAttribute attribute in attributes)
            {
                attribute.IsMainDate = ((attribute.IdAttribute == request.MainDateAttribute) 
                    || (request.MainDateAttribute == Guid.Empty && attribute.Name == DEFAULT_MAIN_DATE_ATTRIBUTE_NAME && attribute.AttributeType == DATE_TIME_ATTRIBUTE_TYPE));
                attribute.KeyOrder = null;
                if (request.OrderedPrimaryKeyAttributes.ContainsKey(attribute.IdAttribute))
                {
                    attribute.KeyOrder = request.OrderedPrimaryKeyAttributes[attribute.IdAttribute];
                    if (string.IsNullOrEmpty(attribute.KeyFormat) &&
                        attribute.KeyOrder < request.OrderedPrimaryKeyAttributes.Max(x => x.Value))
                    {
                        attribute.KeyFormat = "{0}|";
                    }
                }
                _logger.Debug($"Update attribute {attribute.IdAttribute}: IsMainDate = {attribute.IsMainDate}; KeyOrder = {attribute.KeyOrder}");
                AttributeService.UpdateAttribute(attribute);
            }

            if (request.MainDateAttribute == Guid.Empty &&
                !attributes.Any(x => x.Name == DEFAULT_MAIN_DATE_ATTRIBUTE_NAME && x.AttributeType == DATE_TIME_ATTRIBUTE_TYPE))
            {
                DocumentAttribute dateMainAttribute = new DocumentAttribute
                {
                    Name = "Date",
                    AttributeType = "System.DateTime",
                    IsMainDate = true,
                    IsRequired = true,
                    Archive = archive,
                    ConservationPosition = Convert.ToInt16(request.OrderedPrimaryKeyAttributes.Max(x => x.Value) + 1),
                    Mode = new DocumentAttributeMode(0),
                    AttributeGroup = AttributeService.GetAttributeGroup(archive.IdArchive).SingleOrDefault(s => s.GroupType == AttributeGroupType.Undefined)
                };
                _logger.Debug($"Create new MainDate attribute 'Date' for archive {archive.IdArchive}");
                AttributeService.AddAttribute(dateMainAttribute);
            }
            return new EmptyResponseModel();
        }
        #endregion           
    }
}