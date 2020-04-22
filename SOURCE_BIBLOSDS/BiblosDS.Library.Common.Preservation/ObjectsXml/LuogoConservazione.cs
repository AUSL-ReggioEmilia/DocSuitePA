namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class LuogoConservazione : DomFiscale
    {
        public new string GetSerializedForm()
        {
            /*
             Sezione 3.6.1: luogo di conservazione.
                E’ emerso che si intende il LUOGO IN CUI RISIEDONO FISICAMENTE I DATI , ed è evidente che ciò comporta qualche indecisione visto che i dati possono risiedere in luoghi non sempre accessibili o in luoghi diversi.
                E’ sicuramente corretto indicare IL LUOGO DOVE RECARSI PER LA LORO CONSULTAZIONE e dove poter EFFETTUARE IL CONFRONTO TRA L’IMPRONTA CONSERVATA E QUELLA TRASMESSA.
             */
            return base.GetSerializedForm();
        }
    }
}