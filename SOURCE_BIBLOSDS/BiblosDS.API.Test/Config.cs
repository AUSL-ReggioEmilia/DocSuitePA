using BiblosDS.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BiblosDS.API.Test
{           
    public static class Config
    {
        public const string USERNAME = "Gianni", PASSWORD = "Passw0rd", ID_CLIENTE = "TeraSoftware", TIPO_DOCUMENTO = "DDT_ATH_TEST2", CHIAVE = "IVA00155954";

        public const string CHIAVE_ALTRO_CLIENTE = "IVA23132215";
        public static Guid IdDocument = new Guid("62020f87-838e-4175-aa03-67dc3f409139");
        public static Guid IdDocumentLegame = new Guid("93c7ee0f-b8fc-4198-9a94-bea1a6aebcbd");

        public readonly static string ChiaveDocucuntoLink = "IVA00165739";

        public readonly static string TipoDocumentoLink = "Athesia";

        public readonly static FileInfo FILE_TEST = new FileInfo(@"C:\Lavori\Docs\BiblosDS\prima installazione biblosDS2010.txt");
    }
}
