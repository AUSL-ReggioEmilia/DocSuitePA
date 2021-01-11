using System;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto
{
    [Serializable]
    public class Filtro
    {
        #region Constructor
        public Filtro() { }

        public Filtro(string chiave, object valore)
        {
            Chiave = chiave;
            Valore = valore;
        }

        public Filtro(string chiave, string tipo)
        {
            Chiave = chiave;
            Tipo = tipo;
        }
        #endregion

        public string Chiave { get; set; }
        public object Valore { get; set; }
        public string Tipo { get; set; }
    }
}