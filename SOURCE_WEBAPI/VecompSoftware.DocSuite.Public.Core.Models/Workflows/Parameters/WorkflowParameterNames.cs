namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Classe helper coi parametri chiave da utilizzare nei Workflow della DocSuite
    /// </summary>
    public partial class WorkflowParameterNames
    {

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo Collaborazione
        /// </summary>
        public static class CollaborationNames
        {
            /// <summary>
            /// Firmatario
            /// </summary>
            public const string SIGNER = "dsw_p_collaboration_signer";

            /// <summary>
            /// Segreteria di gestione collaborazione
            /// </summary>
            public const string MANAGE = "dsw_p_collaboration_manage";
            /// <summary>
            /// Documento principale
            /// </summary>
            public const string MAIN_DOCUMENT = "dsw_p_collaboration_main_document";

            /// <summary>
            /// Documento allegato
            /// </summary>
            public const string ATTACHMENT_DOCUMENT = "dsw_p_collaboration_attachment_document";

            /// <summary>
            /// Documento annesso
            /// </summary>
            public const string ANNEXED_DOCUMENT = "dsw_p_collaboration_annexed_document";

            /// <summary>
            /// Documento attestazione di conformità
            /// </summary>
            public const string DEMATERIALISATION_DOCUMENT = "dsw_p_collaboration_dematerialisation_document";

            /// <summary>
            /// Modello della collaborazione
            /// </summary>
            public const string COLLABORATION_MODEL = "dsw_p_collaboration_model";

            /// <summary>
            /// Modello di unità documentale di destinazione della collaborazione
            /// </summary>
            public const string DOCUMENT_UNIT_MODEL = "dsw_p_collaboration_manage_model";
        }

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo Archivio
        /// </summary>
        public static class DocumentUnitNames
        {
            /// <summary>
            /// Metadati
            /// </summary>
            public const string METADATA = "dsw_p_documentUnit_metadata";

            /// <summary>
            /// Modello di unità documentale
            /// </summary>
            public const string DOCUMENT_UNIT_MODEL = "dsw_p_documentUnit_model";

            /// <summary>
            /// Settore competente alla presa in carico dell'unità documentaria
            /// </summary>
            public const string MANAGE = "dsw_p_documentUnit_manage";
        }

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo Protocollo
        /// </summary>
        public static class ProtocolNames
        {
            /// <summary>
            /// Documento principale
            /// </summary>
            public const string MAIN_DOCUMENT = "dsw_p_protocol_main_document";

            /// <summary>
            /// Documento allegato
            /// </summary>
            public const string ATTACHMENT_DOCUMENT = "dsw_p_protocol_attachment_document";

            /// <summary>
            /// Documento annesso
            /// </summary>
            public const string ANNEXED_DOCUMENT = "dsw_p_protocol_annexed_document";

            /// <summary>
            /// Documento attestazione di conformità
            /// </summary>
            public const string DEMATERIALISATION_DOCUMENT = "dsw_p_protocol_dematerialisation_document";

            /// <summary>
            /// Riferimento
            /// </summary>
            public const string REFERENCE = "dsw_p_protocol";

            /// <summary>
            /// Settore competente alla presa in carico del protocollo
            /// </summary>
            public const string MANAGE = "dsw_p_protocol_manage";

            /// <summary>
            /// Modello del protocollo
            /// </summary>
            public const string PROTOCOL_MODEL = "dsw_p_protocol_model";

            /// <summary>
            /// Modello del classificatore
            /// </summary>
            public const string CATEGORY_MODEL = "dsw_p_protocol_category";

            /// <summary>
            /// Modello del contenitore
            /// </summary>
            public const string CONTAINER_MODEL = "dsw_p_protocol_container";
        }

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo Archivi
        /// </summary>
        public static class ArchiveNames
        {
            /// <summary>
            /// Contatto
            /// </summary>
            public const string CONTACT = "dsw_p_archive_contact";
            /// <summary>
            /// Settore della DocSuite / MappingTag
            /// </summary>
            /// 
            public const string SECTOR = "dsw_p_archive_authorization";
            /// <summary>
            /// Documento principale
            /// </summary>
            /// 
            public const string MAIN_DOCUMENT = "dsw_p_archive_main_document";

            /// <summary>
            /// Documento allegato
            /// </summary>
            public const string ATTACHMENT_DOCUMENT = "dsw_p_archive_attachment_document";

            /// <summary>
            /// Documento annesso
            /// </summary>
            public const string ANNEXED_DOCUMENT = "dsw_p_archive_annexed_document";

            /// <summary>
            /// Documento attestazione di conformità
            /// </summary>
            public const string DEMATERIALISATION_DOCUMENT = "dsw_p_archive_dematerialisation_document";

            /// <summary>
            /// Riferimento
            /// </summary>
            public const string REFERENCE = "dsw_p_archive";

            /// <summary>
            /// Modello dell'archivio
            /// </summary>
            public const string ARCHIVE_MODEL = "dsw_p_archive_model";

            /// <summary>
            /// Modello del classificatore
            /// </summary>
            public const string CATEGORY_MODEL = "dsw_p_archive_category";
        }

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo Dossier
        /// </summary>
        public static class DossierNames
        {
            /// <summary>
            /// Riferimento
            /// </summary>
            public const string REFERENCE = "dsw_p_dossier";
        }

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo Fascicoli
        /// </summary>
        public static class FascicleNames
        {
            /// <summary>
            /// Riferimento
            /// </summary>
            public const string REFERENCE = "dsw_p_fascicle";
            /// <summary>
            /// Modello del fascicolo
            /// </summary>
            public const string FASCICLE_MODEL = "dsw_p_fascicle_model";
            /// <summary>
            /// Settore responsabile del fascicolo
            /// </summary>
            public const string MANAGE = "dsw_p_fascicle_manage";
            /// <summary>
            /// Cartella del fascicolo
            /// </summary>
            public const string FASCICLE_FOLDER = "dsw_p_fascicle_folder";
            /// <summary>
            /// Inserto del fascicolo
            /// </summary>
            public const string FASCICLE_MISCELLANEA = "dsw_p_fascicle_miscellanea";
        }

        /// <summary>
        /// Sottoclasse coi parametri specifici per il modulo PECMail
        /// </summary>
        public static class PECMailNames
        {
            /// <summary>
            /// Riferimento
            /// </summary>
            public const string PECMAIL_MODEL = "dsw_p_pecmail_model";

            /// <summary>
            /// Documento principale
            /// </summary>
            public const string MAIN_DOCUMENT = "dsw_p_pecmail_main_document";

            /// <summary>
            /// Documento allegato
            /// </summary>
            public const string ATTACHMENT_DOCUMENT = "dsw_p_pecmail_attachment_document";

        }


        /// <summary>
        /// Sottoclasse coi parametri specifici per le integrazioni
        /// </summary>
        public static class IntegrationNames
        {
            /// <summary>
            /// Riferimento
            /// </summary>
            public const string EVENT = "dsw_p_integration_event";
        }
    }
}
