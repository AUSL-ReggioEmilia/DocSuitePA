using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.AVCP;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.ENCO;

namespace VecompSoftware.JeepService.AVCP
{
    public class Tester
    {
        public static void ENCOConvert()
        {
            var doc = ENCOHelper.GetTestDocument();
            string xml = ENCOHelper.GetAVCPXml(doc);
        }


        public static bool ValidateAVCP(string xsdFile)
        {
            var doc = ENCOHelper.GetTestDocument();
            string xml = ENCOHelper.GetAVCPXml(doc);

            XmlValidator validator = new XmlValidator();
            return validator.ValidateXml(xml, xsdFile, AVCPHelper.AvcpNamespace);
        }


        public static void UpdateXml(string modulePath, string xmlFile, string xsdFile)
        {
            //fake response
            WSDBPPubblicazioneOutput response = new WSDBPPubblicazioneOutput
            {
                AttoConcluso = false,
                DatiAvcp = ENCOHelper.GetTestDocument(),
                Result = new WSDBPAVCPResult()
            };

            string testFile = System.IO.Path.Combine(modulePath, xmlFile);
            string avcpXml = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(testFile))
            {
                avcpXml = sr.ReadToEnd();
            }
            avcpXml = AVCPHelper.UpdateXml(avcpXml, new UpdateData
            {
                @abstract = "",
                entePubblicatore = "",
                licenza = "",
                titolo = "",
                urlFile = ""
            });

            XmlValidator validator = new XmlValidator();
            bool res = validator.ValidateXml(avcpXml, System.IO.Path.Combine(modulePath, xsdFile), AVCPHelper.AvcpNamespace);
            if (!res)
            {
                foreach (var item in validator.Errors)
                {
                    Console.WriteLine(item);
                }
            }
        }


        public static void CreateIndex(string username, string urlMask)
        {
            var finder = new DocumentSeriesItemFinder(username);

            //esegue la ricerca
            finder.ItemStatusIn = new List<DocumentSeriesItemStatus>() { DocumentSeriesItemStatus.Active };
            finder.IdDocumentSeriesIn = new List<int> { DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value }; // Identificativo della serie di riferimento
            finder.EnablePaging = false; // disattiva la paginazione dei risultati
            finder.IsPublished = true;

            List<DocumentSeriesItem> list = finder.DoSearch().ToList();
            //string xml = AVCPHelper.GetIndexFile(urlMask, list);
        }

    }

}
