#define Ws_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BiblosDS.Library.Common.StampaConforme;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace StampaConformeConsole
{
    class Program
    {
        private static BackgroundWorker bgw;
        private static Semaphore sem;
        private static Timer tm;

        public Program()
        {
            log4net.Config.XmlConfigurator.Configure();

            //using (var comped = new CompEdLib())
            //{
            //    byte[] blobOut = comped.SoftSign("BiblosDS", "Frontalino.pdf", File.ReadAllBytes(@"C:\Lavori\Docs\BiblosDS\TestStampaConforme\Nuova cartella\Frontalino.pdf"));
            //    File.WriteAllBytes(@"C:\Lavori\Docs\BiblosDS\TestStampaConforme\Nuova cartella\Frontalino.pdf.p7m", blobOut);
            //}

            bgw_DoWork(null, null);
            //Program.bgw = new BackgroundWorker();
            //Program.bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            //Program.bgw.WorkerSupportsCancellation = true;
            //bgw.RunWorkerAsync();
            //bgw.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            //{
            //    bgw.RunWorkerAsync();
            //};
            //Program.tm = new Timer((x) =>
            //{
            //    if (this.RequestsQuit())
            //    {
            //        if (Program.bgw.IsBusy) Program.bgw.CancelAsync();
            //        if (Program.sem != null) Program.sem.Release();
            //    }
            //}, null, 0, Timeout.Infinite);
            Console.WriteLine("Premere un tasto per iniziare (tasto SPAZIO per uscire/interrompere)...");
            if (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
            {                               
                Console.WriteLine("Lavoro concluso. Premere un tasto per uscire...");
                Console.ReadKey(true);
            }
        }

        
        private bool RequestsQuit()
        {
            int keycode = -1;
            if (Console.KeyAvailable)
            {
                keycode = Console.Read();
#if DEBUG
                if (keycode != -1) 
                {
                    Debugger.Break();
                }
#endif
            }
            return keycode == (char)ConsoleKey.Spacebar;
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
#if ws
            StampaConformeSvc.BiblosDSConv convVersion = new StampaConformeSvc.BiblosDSConv();
            Console.WriteLine(convVersion.GetVersion());
#endif
            PrintRedirected pr = new PrintRedirected();
            DirectoryInfo dir = new DirectoryInfo(@"C:\Lavori\Docs\BiblosDs\TestStampaConforme\");
            string fileName;
            foreach (var item in dir.GetFiles())
            {
                Console.WriteLine("Elab:\t" + item.Name);
                try
                {
                    if (Path.GetExtension(item.Name).Length > 0 && String.Compare(Path.GetExtension(item.Name).Substring(1).Substring(0, 2), "P7", true) != 0)
                        fileName = Path.GetExtension(item.Name).Substring(1);
                    else
                        fileName = item.Name;
                    byte[] FileBlob = File.ReadAllBytes(item.FullName);
                    string label = "<Label>"
                           + "  <Text>Protocollo 123 (pagina) di (pagine)</Text><Footer>Protocollo 123 (pagina) di (pagine)</Footer>"
                           + "  <Font Face=\"Arial\" Size=\"18\" Style=\"Bold,Italic\" />"                           
                           + "</Label>";
                    label = string.Format("<Label><Text>BiblosTester.GetDocumentHeadedEx - {0}</Text><Font Face='Arial' Size='12' Style='Bold' /></Label>", DateTime.Now.ToString());
                    //+ "  <Scale scalePercent=\"96\" />"
                    string Waternamrk = "<Watermark>"
                         + "  <Text>Copia#per la pubblicazione</Text>"
                         + "  <Font Color=\"58,95,205\" Face=\"Arial\" Size=\"50\" Style=\"Bold,Italic\" />"
                         + "  <WatermarkConfig WatermarkRotation=\"45\" FillOpacity=\"30\" />"
                         + "  <Scale scalePercent=\"80\" />"
                         + "</Watermark>";
                    byte[] result = null;
                    var cfg = new BoxConfig
                    {
                        BoxLine = new[] 
                           {
                               new BoxLineConfig {  Message = "Prova 1" },
                               new BoxLineConfig { Message = "Prova 2" },
                               new BoxLineConfig { Message = "Prova 3" }, 
                           },
                    };
#if Ws
                   StampaConformeSvc.BiblosDSConv conv = new StampaConformeSvc.BiblosDSConv();
                   //conv.Url = "http://./StampaConforme/BiblosDSConv.asmx";// "http://win2k8sc.rmvsw.local/StampaConforme2010/BiblosDSConv.asmx";//"http://localhost/StampaConforme/BiblosDSConv.asmx"; //
                   //Console.Write(conv.Test(item.Extension));
                   //var doc = conv.ToRasterFormat(new StampaConformeSvc.stDoc { Blob = Convert.ToBase64String(FileBlob), FileExtension = fileName }, "PDF");                  
                   //var doc = conv.ToRasterFormatExParameters(new StampaConformeSvc.stDoc { Blob = Convert.ToBase64String(FileBlob), FileExtension = fileName }, "PDF", label, new StampaConformeSvc.stParameter[]{ new StampaConformeSvc.stParameter{ Name = StampaConformeSvc.stParameterOption.AttachMode, Value = "3" }});
                   var doc = conv.ToRasterFormatEx(new StampaConformeSvc.stDoc { Blob = Convert.ToBase64String(FileBlob), FileExtension = fileName }, "PDF", label);
                   //var doc = conv.ToRasterFormatWatermarked(new StampaConformeSvc.stDoc { Blob = Convert.ToBase64String(FileBlob), FileExtension = fileName }, label,Waternamrk);
                   //var doc = conv.ToRasterFormatRgWatermarked(new StampaConformeSvc.stDoc { Blob = Convert.ToBase64String(FileBlob), FileExtension = fileName }, "PDF", label, Waternamrk, "", "", 0, 0, 0, 0, 0, 0, 0, 0);
                   result = Convert.FromBase64String(doc.Blob);
#else
                    // label = "";
                    byte[] res;
                    //result = pr.ConvertToFormatLabeledWithForm(FileBlob, fileName, "PDF", label, cfg);
                    result = pr.ConvertToFormatLabeled(FileBlob, fileName, "PDF", label, BiblosDS.Library.Common.Enums.AttachConversionMode.Default);
                    //PdfLabeler ePdf = new PdfLabeler();
                    //var resSecure = ePdf.RightPdf(result,
                    //    "xxx",
                    //    "xx",
                    //    0,
                    //    0,
                    //    0,
                    //    0,
                    //    0,
                    //    0,
                    //    0,
                    //    0);
                    /*
                    PdfLabeler ePdf = new PdfLabeler();
                    result = ePdf.RightPdf(result,
                   "",
                   "",
                   0,
                   0,
                   0,
                   0,
                   0,
                   0,
                   0,
                   0);*/
                    //result = pr.ToRasterFormat(FileBlob, fileName, "PDF", out res);                    
                    //result = pr.EtichettaWatermark(result, Waternamrk); 
                    
#endif

                    File.WriteAllBytes(Path.Combine(@"C:\Lavori\Docs\BiblosDs", item.Name + ".PDF"), result);
                }
                catch(Exception exx)
                {
                    Console.WriteLine(exx.Message);
                }
            }
            Program.tm.Dispose();
            //Program.sem.Release();
        }

        static void Main(string[] args)
        {
            bool isNew;
            using (Mutex mtx = new Mutex(true, "MTX_STAMPA", out isNew))
            {
                if (isNew)
                    new Program();
            }
        }
    }
}
