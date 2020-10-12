using System;
using System.Collections.Generic;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Mappers
{
    public class ProtocolReferenceMapper : IMapper<IDictionary<string, object>, DocSuiteModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ProtocolReferenceMapper()
        {

        }
        #endregion

        #region [ Methods ]
        public DocSuiteModel Map(IDictionary<string, object> eventProperties)
        {

            Guid protocolUniqueId = Guid.Empty;
            if (!eventProperties.ContainsKey(CustomPropertyName.PROTOCOL_YEAR) ||
                !short.TryParse(eventProperties[CustomPropertyName.PROTOCOL_YEAR].ToString(), out short protocolYear))
            {
                throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_YEAR} property in protocol reference mapper");
            }
            if (!eventProperties.ContainsKey(CustomPropertyName.PROTOCOL_NUMBER) ||
                !int.TryParse(eventProperties[CustomPropertyName.PROTOCOL_NUMBER].ToString(), out int protocolNumber))
            {
                throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_NUMBER} property in protocol reference mapper");
            }
            if (!eventProperties.ContainsKey(CustomPropertyName.PROTOCOL_UNIQUE_ID) ||
                !Guid.TryParse(eventProperties[CustomPropertyName.PROTOCOL_UNIQUE_ID].ToString(), out protocolUniqueId))
            {
                throw new ArgumentNullException($"Undefined {CustomPropertyName.PROTOCOL_UNIQUE_ID} property in protocol reference mapper");
            }

            return new DocSuiteModel()
            {
                Title = string.Format(DocSuiteModel.PROTOCOL_TITLE_FORMAT, protocolYear, protocolNumber),
                Year = protocolYear,
                Number = protocolNumber,
                UniqueId = protocolUniqueId,
                ModelType = DocSuiteType.Protocol,
                ModelStatus = DocSuiteStatus.Activated
            };
        }
        #endregion        
    }
}
