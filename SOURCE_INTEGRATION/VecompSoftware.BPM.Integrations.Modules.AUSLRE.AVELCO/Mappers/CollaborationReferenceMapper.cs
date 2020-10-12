using System;
using System.Collections.Generic;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Mappers
{
    public class CollaborationReferenceMapper : IMapper<IDictionary<string, object>, DocSuiteModel>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public CollaborationReferenceMapper()
        {
        }
        #endregion

        #region [ Methods ]
        public DocSuiteModel Map(IDictionary<string, object> eventProperties)
        {
            Guid collaborationUniqueId = Guid.Empty;
            if (!eventProperties.ContainsKey(CustomPropertyName.COLLABORATION_ID) ||
                !int.TryParse(eventProperties[CustomPropertyName.COLLABORATION_ID].ToString(), out int collaborationId))
            {
                throw new ArgumentNullException($"Undefined {CustomPropertyName.COLLABORATION_ID} property in collaboration reference mapper");
            }
            if (!eventProperties.ContainsKey(CustomPropertyName.COLLABORATION_UNIQUE_ID) ||
                !Guid.TryParse(eventProperties[CustomPropertyName.COLLABORATION_UNIQUE_ID].ToString(), out collaborationUniqueId))
            {
                throw new ArgumentNullException($"Undefined {CustomPropertyName.COLLABORATION_UNIQUE_ID} property in collaboration reference mapper");
            }

            return new DocSuiteModel()
            {
                Title = string.Format(DocSuiteModel.COLLABORATION_TITLE_FORMAT, collaborationId),
                UniqueId = collaborationUniqueId,
                EntityId = collaborationId,
                ModelType = DocSuiteType.Collaboration,
                ModelStatus = DocSuiteStatus.Activated
            };
        }
        #endregion        
    }
}
