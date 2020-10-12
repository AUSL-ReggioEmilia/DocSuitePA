using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Helpers
{
    public static class CommandValidatorHelper
    {
        private static IDictionary<SkyDocCommandType, Action<SkyDocCommand, IWebAPIClient>> _commandTypeValidators;

        static CommandValidatorHelper()
        {
            _commandTypeValidators = new Dictionary<SkyDocCommandType, Action<SkyDocCommand, IWebAPIClient>>
            {
                { SkyDocCommandType.ManualProtocol, ValidateManualProtocolCommandAsync },
                { SkyDocCommandType.AutomaticProtocol, ValidateAutomaticProtocolCommandAsync },
                { SkyDocCommandType.DigitalSignature, ValidateAutomaticProtocolCommandAsync }
            };
        }

        public static void ValidateCommandFields(SkyDocCommand skyDocCommand, IWebAPIClient webApiClient)
        {
            ICollection<SkyDocDocument> commandMainDocs = skyDocCommand.Documents.Where(doc => doc.DocumentType == SkyDocDocumentType.MainDocument
                && !doc.WFSkyDocStarted.HasValue
                && !doc.WFSkyDocStatus.HasValue).ToList();

            if (commandMainDocs.Count == 0)
            {
                throw new ArgumentException($"Undefined Documento Principale for {skyDocCommand.CommandType} Command with id {skyDocCommand.Id}");
            }

            if (commandMainDocs.Count > 1)
            {
                throw new ArgumentException($"{skyDocCommand.CommandType} Command with id {skyDocCommand.Id} has more than one Main Document");
            }

            Tenant commandTenant = webApiClient.GetTenantAsync(skyDocCommand.TenantId, "$expand=TenantAOO").Result;
            if (commandTenant == null)
            {
                throw new ArgumentException($"Tenant with id {skyDocCommand.TenantId} not found for Command with id {skyDocCommand.Id}");
            }

            #region [ Check undefined values ]
            if (string.IsNullOrEmpty(skyDocCommand.Object))
            {
                throw new ArgumentException($"Undefined Oggetto for {skyDocCommand.CommandType} Command with id {skyDocCommand.Id}");
            }

            if (string.IsNullOrEmpty(skyDocCommand.DossierReference))
            {
                throw new ArgumentException($"Undefined Riferimento_dossier for {skyDocCommand.CommandType} Command with id {skyDocCommand.Id}");
            }

            if (string.IsNullOrEmpty(skyDocCommand.FascicleReference))
            {
                throw new ArgumentException($"Undefined Riferimento_fascicolo for {skyDocCommand.CommandType} Command with id {skyDocCommand.Id}");
            }

            #endregion

            // Invoke command specific validator
            _commandTypeValidators[skyDocCommand.CommandType].Invoke(skyDocCommand, webApiClient);
        }

        private static void ValidateManualProtocolCommandAsync(SkyDocCommand manualProtocolCommand, IWebAPIClient webApiClient)
        {
            if (string.IsNullOrEmpty(manualProtocolCommand.ResponsibleRoleMappingTag))
            {
                throw new ArgumentException($"Undefined Mapping_Tag_Esecutore for Manual Protocol Command with id {manualProtocolCommand.Id}");
            }

            if (!manualProtocolCommand.Typology.HasValue)
            {
                throw new ArgumentException($"Undefined Tipologia for Manual Protocol Command with id {manualProtocolCommand.Id}");
            }
        }

        private static void ValidateAutomaticProtocolCommandAsync(SkyDocCommand automaticProtocolCommand, IWebAPIClient webApiClient)
        {
            if (string.IsNullOrEmpty(automaticProtocolCommand.Contact1))
            {
                throw new ArgumentException($"Undefined Contatto_01 for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }

            if (string.IsNullOrEmpty(automaticProtocolCommand.Contact2))
            {
                throw new ArgumentException($"Undefined Contatto_02 for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }

            if (!automaticProtocolCommand.Typology.HasValue)
            {
                throw new ArgumentException($"Undefined Tipologia for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }

            if (string.IsNullOrEmpty(automaticProtocolCommand.CategoryId))
            {
                throw new ArgumentException($"Undefined Classificazione for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }

            Category commandCategory = webApiClient.GetCategoryAsync(int.Parse(automaticProtocolCommand.CategoryId)).Result;
            if (commandCategory == null)
            {
                throw new ArgumentException($"Invalid Classificazione with id {automaticProtocolCommand.CategoryId} for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }

            if (string.IsNullOrEmpty(automaticProtocolCommand.ContainerId))
            {
                throw new ArgumentException($"Undefined Contenitore for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }

            Container commandContainer = webApiClient.GetContainerAsync(short.Parse(automaticProtocolCommand.ContainerId)).Result.FirstOrDefault();
            if (commandContainer == null)
            {
                throw new ArgumentException($"Invalid Contenitore with id {automaticProtocolCommand.ContainerId} for Automatic Protocol Command with id {automaticProtocolCommand.Id}");
            }
        }

        // TODO: Uncomment when implementing "Digital Signature" workflow
        //private static void ValidateDigitalSignatureCommand(SkyDocCommand digitalSignatureCommand, IWebAPIClient webApiClient)
        //{
        //    if (string.IsNullOrEmpty(digitalSignatureCommand.Contact1))
        //    {
        //        throw new ArgumentException($"Undefined Contatto_01 for Digital Signature Command with id {digitalSignatureCommand.Id}");
        //    }

        //    if (string.IsNullOrEmpty(digitalSignatureCommand.Contact2))
        //    {
        //        throw new ArgumentException($"Undefined Contatto_02 for Digital Signature Command with id {digitalSignatureCommand.Id}");
        //    }

        //    if (!string.IsNullOrEmpty(digitalSignatureCommand.Contact3) && !ValidateContactNameFormat(digitalSignatureCommand.Contact3))
        //    {
        //        throw new ArgumentException($"Incorrect format for Contatto_03 ({digitalSignatureCommand.Contact3}) of Digital Signature Command with id {digitalSignatureCommand.Id}. The format must be '<dominio>\\<samAccount>'");
        //    }

        //    if (!string.IsNullOrEmpty(digitalSignatureCommand.Contact4) && !ValidateContactNameFormat(digitalSignatureCommand.Contact4))
        //    {
        //        throw new ArgumentException($"Incorrect format for Contatto_04 ({digitalSignatureCommand.Contact4}) of Digital Signature Command with id {digitalSignatureCommand.Id}. The format must be '<dominio>\\<samAccount>'");
        //    }

        //    if (string.IsNullOrEmpty(digitalSignatureCommand.ResponsibleRoleMappingTag))
        //    {
        //        throw new ArgumentException($"Undefined Mapping_Tag_Esecutore for Digital Signature Command with id {digitalSignatureCommand.Id}");
        //    }

        //    if (string.IsNullOrEmpty(digitalSignatureCommand.AuthorizedRoleMappingTag))
        //    {
        //        throw new ArgumentException($"Undefined Mapping_Tag_Autorizzato for Digital Signature Command with id {digitalSignatureCommand.Id}");
        //    }
        //}

        // TODO: complete when implementing Digital Signature workflow
        // This method is used for validating the format of Contatto_03 and Contatto_04 
        // values. If these fields have value, the values must have the format as: domain\samAccount
        // if not, throw validation exception
        //private static bool ValidateContactNameFormat(string contactName)
        //{
        //    return true;
        //}
    }
}
