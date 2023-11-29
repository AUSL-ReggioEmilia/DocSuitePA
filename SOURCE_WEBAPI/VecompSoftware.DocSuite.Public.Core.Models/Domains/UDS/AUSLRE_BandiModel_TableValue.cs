using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.UDS
{
    public class AUSLRE_BandiModel_TableValue
    {
        public Guid UDSId { get; set; }
        public DateTimeOffset? DataScadenza { get; set; }
        public string Categoria { get; set; }
        public DateTimeOffset? DataFinePubblicazione { get; set; }
        public string Descrizione { get; set; }
        public DateTimeOffset? DataPubblicazione { get; set; }
        public string DataScadenzaLabel { get; set; }
        public string DataPubblicazioneLabel { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public string Subject { get; set; }
        public Guid Menu { get; set; }
        public Guid IdUDSRepository { get; set; }
        public Guid? IdDocument { get; set; }
        public string Informazioni { get; set; }
        public string LinkEsterno { get; set; }
        public string IstruttorePratica { get; set; }
        public string ProvinciaSedeDiGara { get; set; }
        public string IndirizzoSedeDiGara { get; set; }
        public bool? SenzaImporto { get; set; }
        public string CodiceCpv { get; set; }
        public string CodiceCig { get; set; }
        public string AmministrazioneAggiudicatrice { get; set; }
        public string Amministrazione { get; set; }
        public string ResponsabileProcedimento { get; set; }
        public string UrlPubblicazione { get; set; }
    }
}
