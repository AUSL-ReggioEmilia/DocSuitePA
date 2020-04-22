
namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    /*
        M	Amministrazione
        A	Area Organizzativa Omogenea (AOO)
        U	Unità Organizzativa (AO)
        R	Ruolo
        P	Persona
        G	Gruppo
        D	Persona AdAm
        I	Pubblica amministrazione da IPA
        S	Settore 
     */

    /// <summary>
    /// Tipologia del contatto
    /// </summary>
    public enum ContactType : int
    {
        /// <summary>
        /// Codice non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Amministrazione
        /// </summary>
        Administration = 1,
        /// <summary>
        /// Area Organizzativa Omogenea (AOO)
        /// </summary>
        AOO = 2,
        /// <summary>
        /// Unità Organizzativa (AO)
        /// </summary>
        AO = AOO * 2,
        /// <summary>
        /// Ruolo
        /// </summary>
        Role = AO * 2,
        /// <summary>
        /// Gruppo
        /// </summary>
        Group = Role * 2,
        /// <summary>
        /// Settore
        /// </summary>
        Sector = Group * 2,
        /// <summary>
        /// Persona
        /// </summary>
        Citizen = Sector * 2,
        /// <summary>
        /// Pubblica amministrazione da IPA
        /// </summary>
        IPA = Citizen * 2,
        /// <summary>
        /// Persona di contatto Manuale
        /// </summary>
        CitizenManual = IPA * 2,
        /// <summary>
        /// Area Organizzativa Omogenea (AOO) di contatto Manuale
        /// </summary>
        AOOManual = CitizenManual * 2,
    }
}