namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Tipologie di entità della DocSuite
    /// </summary>
    public enum DocSuiteEntityType : int
    {
        /// <summary>
        /// Non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Protocollo
        /// </summary>
        Protocol = 1,
        /// <summary>
        /// Atto
        /// </summary>
        Resolution = 2,
        /// <summary>
        /// Serie documentale
        /// </summary>
        DocumentSeries = Resolution * 2,
        /// <summary>
        /// Archivio / Unità documentaria specifica
        /// </summary>
        Archive = DocumentSeries * 2,
        /// <summary>
        /// PEC
        /// </summary>
        PEC = Archive * 2,
        /// <summary>
        /// Collaborazione
        /// </summary>
        Collaboration = PEC * 2,
        /// <summary>
        /// Fascicolo
        /// </summary>
        Fascicle = Collaboration * 2,
        /// <summary>
        /// Dossier
        /// </summary>
        Dossier = Fascicle * 2,
        /// <summary>
        /// Settore della DocSuite
        /// </summary>
        DocSuiteSector = Dossier * 2,
        /// <summary>
        /// Contatto della DocSuite
        /// </summary>
        DocSuiteContact = DocSuiteSector * 2,
        /// <summary>
        /// Ramo di Organigramma della DocSuite
        /// </summary>
        OChartItem = DocSuiteContact * 2,
        /// <summary>
        /// Contenitore della DocSuite
        /// </summary>
        Container = OChartItem * 2,
        /// <summary>
        /// Evento di intgrazione esterno 
        /// </summary>
        IntegrationEvent = Container * 2,
    }
}