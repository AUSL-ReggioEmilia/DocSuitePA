using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class Importer
    {
        private IAVCPDataReader reader;
        private IAVCPDocumentHandler provider;

        public Importer(IAVCPDataReader reader, IAVCPDocumentHandler provider)
        {
            this.reader = reader;
            this.provider = provider;
        }


        public List<DocumentData> ImportAcquisti()
        {
            List<DocumentRow> rows;
            List<Notification> errors = new List<Notification>();

            try
            {
                if (!reader.Fetch(out rows, out errors))
                    throw new ApplicationException("Errori in lettura file");

                List<DocumentData> docs = provider.BuildDocuments(rows, out errors);
                if (errors.Count > 0)
                    throw new ApplicationException("Errori in creazione documenti");

                docs.ForEach(p =>
                {
                    Console.WriteLine("Documento:{0}", AVCPHelper.GetDocumentCode(p.Anno, p.CodiceServizio, p.Numero));
                    Console.WriteLine(XmlFile<pubblicazione>.Serialize(p.Pubblicazione));
                });

                return docs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
                errors.ForEach(p =>
                {
                    Console.WriteLine("{0}", p.Message);
                });
                return null;
            }
        }


        public List<DocumentData> ImportPagamenti()
        {
            List<DocumentRow> rows;
            List<Notification> errors = new List<Notification>();

            try
            {
                if (!reader.Fetch(out rows, out errors))
                    throw new ApplicationException("Errori in lettura file");

                List<DocumentData> docs = provider.UpdateDocuments(rows, StoreFacade.ResolveDocumentKey, StoreFacade.GetDocumentKeyXml, out errors);
                if (errors.Count > 0)
                    throw new ApplicationException("Errori in aggiornamento documenti");

                docs.ForEach(p =>
                {
                    Console.WriteLine("Documento:{0}", p.DocumentKey);
                });

                return docs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
                errors.ForEach(p =>
                {
                    Console.WriteLine("{0}", p.Message);
                });
                return null;
            }
        }


        public List<ImportRowPagamento> AggregatePagamenti()
        {
            List<DocumentRow> rows;
            List<Notification> errors = new List<Notification>();

            try
            {
                if (!reader.Fetch(out rows, out errors))
                    throw new ApplicationException("Errori in lettura file");

                List<ImportRowPagamento> docs = provider.AggregatePagamenti(rows);
                docs.ForEach(p =>
                {
                    Console.WriteLine("CIG:{0} / Documento:{1} - Value:{2}", p.CIG, p.DocumentKey, p.ImportoLiquidato.ToString("N"));
                });

                return docs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
                errors.ForEach(p =>
                {
                    Console.WriteLine("{0}", p.Message);
                });
                return null;
            }
        }
    }

}
