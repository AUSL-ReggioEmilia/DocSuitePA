using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.JeepService.ResolutionImporter
{
    internal class JsonResolutionModel
    {
        public DateTimeOffset DataAdozione { get; set; }
        public DateTimeOffset DataEsecutiva { get; set; }
        public string Oggetto { get; set; }
        public string Proponente { get; set; }
        public string Adottante { get; set; }
        public string TipologiaAtto { get; set; }
        public string Segnatura { get; set; }
        public short Anno { get; set; }
        public int Numero { get; set; }
        public short DelDet { get; set; }
        public string MaindocumentPath { get; set; }
        public List<string> AttachmentsDocumentPath { get; set; }
    }
}
