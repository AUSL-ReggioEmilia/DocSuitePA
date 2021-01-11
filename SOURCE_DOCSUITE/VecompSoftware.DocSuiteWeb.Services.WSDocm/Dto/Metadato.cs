using System;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto
{
    [Serializable]
    public class Metadato
    {
        #region Constructor

        public Metadato() { }

        public Metadato(string etichetta, object valore)
        {
            Etichetta = etichetta;
            Valore = valore;
        }
        #endregion

        public string Etichetta { get; set; }

        public object Valore { get; set; }
    }
}