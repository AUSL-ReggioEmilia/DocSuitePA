namespace VecompSoftware.DocSuiteWeb.Entity.Commons
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
    public static class ContactType
    {
        /// <summary>
        /// Amministrazione
        /// </summary>
        public const string Administration = "M";
        /// <summary>
        /// Area Organizzativa Omogenea (AOO)
        /// </summary>
        public const string AOO = "A";
        /// <summary>
        /// Unità Organizzativa (AO)
        /// </summary>
        public const string AO = "AO";
        /// <summary>
        /// Ruolo
        /// </summary>
        public const string Role = "R";
        /// <summary>
        /// Gruppo
        /// </summary>
        public const string Group = "G";
        /// <summary>
        /// Settore
        /// </summary>
        public const string Sector = "S";
        /// <summary>
        /// Persona
        /// </summary>
        public const string Citizen = "P";
        /// <summary>
        /// Pubblica amministrazione da IPA
        /// </summary>
        public const string IPA = "I";
        /// <summary>
        /// Persona di contatto Manuale
        /// </summary>
        public const string CitizenManual = "D";
        /// <summary>
        /// Area Organizzativa Omogenea (AOO) di contatto Manuale
        /// </summary>
        public const string AOOManual = "DAO";
    }
}
