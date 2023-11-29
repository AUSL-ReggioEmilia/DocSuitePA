using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using VecompSoftware.Helpers.iTextSharp;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.StampaConforme.Models.ConversionParameters;

namespace BiblosDS.Library.Common.StampaConforme
{
    public class SignConfig
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? DPI { get; set; }
    }
    /// <summary>
    /// questa classe fà l'overpring sui PDF
    /// </summary>
    public class PdfLabeler
    {
        public SignConfig signConfig { get; set; }
        public Point posizione;
        public bool useStartPoint = false;
        public int watermarkRotation = 45;
        public float fillOpacity = 0.3F;
        public float scalePercent = 0.96f;
        public System.Drawing.Font font;
        public string FontFace = "", FontStyle = "";
        public Brush brush;
        public string inpFile, outFile, testoEtichetta = "", testoFooter = "";
        public int altezzaTestata;
        public Byte[] coverWMF;
        public iTextSharp.text.BaseColor Color = new iTextSharp.text.BaseColor(255, 125, 125);
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PdfLabeler));

        /// <summary>
        /// Set default value for Labeler
        /// </summary>
        public PdfLabeler()
        {
            signConfig = new SignConfig { Width = 400, Height = 550, X = 10, Y = 550, DPI = 300 };
            font = new System.Drawing.Font("Arial", 18, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
            brush = Brushes.White;
            if (ConfigurationManager.AppSettings["LabelPosizioneXY"] != null)
            {
                int x = 0;
                int y = 0;
                try
                {
                    string[] posXY = ConfigurationManager.AppSettings["LabelPosizioneXY"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    int.TryParse(posXY[0], out x);
                    int.TryParse(posXY[1], out y);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                posizione = new Point(x, y);
            }
            else
                posizione = new Point(0, 0);
            if (ConfigurationManager.AppSettings["SignConfig"] != null)
            {
                var tmpSignConfig = JsonConvert.DeserializeObject<SignConfig>(ConfigurationManager.AppSettings["SignConfig"]);
                if (signConfig != null)
                {
                    signConfig = tmpSignConfig;
                    logger.DebugFormat("Get SignConfig Width:{0} - Height:{1} - Left:{2} - Top:{3}", signConfig.Width.GetValueOrDefault(), signConfig.Height.GetValueOrDefault(), signConfig.X.GetValueOrDefault(), signConfig.Y.GetValueOrDefault());
                }
                else
                    logger.WarnFormat("Unable to parse config - Format with json : {0}", ConfigurationManager.AppSettings["SignConfig"]);
            }
            if (ConfigurationManager.AppSettings["ScalePercent"] != null)
            {
                int perc = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["ScalePercent"].ToString(), out perc))
                {
                    scalePercent = (float)perc / (float)100;
                }
            }
            altezzaTestata = 50;
        }

        [Obsolete("Use AddLabelToPages + AddSignatureToPageAndResize", false)]
        public bool Etichetta(string inFile, string ouFile, string FileFormat, string etic, string footer)
        {
            inpFile = inFile;
            outFile = ouFile;
            testoEtichetta = etic;
            testoFooter = footer;
            return EtichettaPdf();
        }


        /// <summary>
        /// Aggiunge al PDF la signatura della firma
        /// </summary>
        /// <param name="fileBlob"></param>
        /// <returns>
        /// PDF con signature inclusa
        /// </returns>
        public byte[] AddSignatureToPageAndResize(byte[] fileBlob, byte[] originalFileBlob)
        {
            logger.DebugFormat("AddSignatureToPageAndResize..New");
            string inFile = Path.Combine(Path.GetTempPath(), "SC_" + Guid.NewGuid() + ".tmp");
            try
            {
                PdfReader reader = null;
                Document document = null;
                PdfWriter writer = null;
                try
                {
                    reader = new PdfReader(fileBlob);
                    document = new Document(reader.GetPageSizeWithRotation(1));
                    writer = PdfWriter.GetInstance(document, new FileStream(inFile, FileMode.OpenOrCreate));
                    writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);//i tried with 1_7instead, same result
                    writer.PDFXConformance = PdfWriter.PDFX1A2001;
                    writer.CreateXmpMetadata();
                    document.Open();
                    var cb = writer.DirectContent;
                    iTextSharp.text.Rectangle rect = null;
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        rect = reader.GetPageSizeWithRotation(i);
                        document.SetPageSize(rect);
                        document.NewPage();
                        var page = writer.GetImportedPage(reader, i);
                        if (rect.Rotation == 90)
                            cb.AddTemplate(page, 0, -1.0F, scalePercent, 0, 0, rect.Height);
                        else if (rect.Rotation == 270)
                            cb.AddTemplate(page, 0, scalePercent, -1f, 0, rect.Width, 0);
                        else if (rect.Rotation == 180)
                        {
                            double angle = Math.PI; //Math.sin/cos want in radians
                            switch (rect.Rotation)
                            {
                                case 90: angle = Math.PI * 90 / 180; break;
                                case 180: angle = Math.PI * 180 / 180; break;
                                case 270: angle = Math.PI * 270 / 180; break;
                            }
                            float a = (float)Math.Cos(angle);
                            float b = (float)Math.Sin(angle);
                            float c = (float)-Math.Sin(angle);
                            float d = (float)Math.Cos(angle);
                            cb.AddTemplate(page, a, b, c, d, rect.Width, rect.Height * scalePercent);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1, 0, 0, scalePercent, 1, 1);
                            //cb.AddTemplate(page, 1f, 0, 0, scalePercent, 0, 0);
#warning GESTIRE MEGLIO LA ROTAZIONE! Se il documento viene ruotato, la grafica eventualmente associata ai commenti PDF (linee rosse, fumetti, eccetera) viene disegnata sbagliata.
                            //cb.AddTemplate(page, 0, 0);
                        }
                        PrintRedirected.ExtractPdfComments(ref reader, ref writer, i);
                    }

                    if (coverWMF.Length > 0)    // retro copertina con le firme
                    {
                        document.NewPage();
                        P7Mmanager p7m = new P7Mmanager();
                        if (p7m.LastErr.Length > 0)
                        {
                            logger.Debug("Warning in creazione CompEd: " + p7m.LastErr);
                            p7m.LastErr = "";
                        }
                        logger.Debug("ExtractDocumentFromBuffer");
                        string originalFileName;
                        p7m.ExtractDocumentFromBuffer(originalFileBlob, out originalFileName);
                        p7m.PrintMetaFileDocumentToFile(null, cb, document, rect.Height);
                        p7m.Close();
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
                finally
                {
                    ////Try to dispose ITextSharp
                    if (reader != null)
                        reader.Close();
                    if (document != null)
                        document.Close();
                    if (writer != null)
                        writer.Close();
                }
                return File.ReadAllBytes(inFile);
            }
            finally
            {
                bool DelFile = string.Compare(ConfigurationManager.AppSettings["DeleteFile"], "true", true) == 0;
                if (DelFile)
                    File.Delete(inFile);
            }
        }

        public byte[] AddSignatureToPageAndResizeNew(byte[] fileBlob, byte[] originalFileBlob)
        {
            logger.DebugFormat("AddSignatureToPageAndResizeNew..New");

            byte[] result = null;
            var pdf = new PdfContentInfo(fileBlob);

            PdfReader reader = null;
            PdfWriter writer = null;
            using (reader = pdf.GetReader())
            using (var ms = new MemoryStream())
            using (var document = new Document(reader.GetPageSizeWithRotation(1)))
            using (writer = PdfWriter.GetInstance(document, ms))
            {
                try
                {
                    writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                    writer.PDFXConformance = PdfWriter.PDFX1A2001;
                    writer.CreateXmpMetadata();
                    document.Open();
                    var cb = writer.DirectContent;

                    iTextSharp.text.Rectangle rect = null;
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        rect = reader.GetPageSizeWithRotation(i);
                        document.NewPage(rect);
                        var page = writer.GetImportedPage(reader, i);

                        var tx = PdfTransformerFactory.Default().SetOrigin(page, rect);
                        tx.ContentScaling = Convert.ToInt32(scalePercent * 100F);
                        logger.DebugFormat("AddSignatureToPageAndResizeNew -> tx.ContentScaling: {0}", tx.ContentScaling);
                        writer.DirectContent.TransformTo(tx);

                        //PrintRedirected.ExtractPdfComments(ref reader, ref writer, i);
                    }

                    if (coverWMF.Length > 0)    // retro copertina con le firme
                    {
                        document.NewPage();
                        P7Mmanager p7m = new P7Mmanager();
                        if (p7m.LastErr.Length > 0)
                        {
                            logger.Debug("Warning in creazione CompEd: " + p7m.LastErr);
                            p7m.LastErr = "";
                        }
                        logger.Debug("ExtractDocumentFromBuffer");
                        string originalFileName;
                        p7m.ExtractDocumentFromBuffer(originalFileBlob, out originalFileName);
                        p7m.PrintMetaFileDocumentToFile(null, cb, document, rect.Height);
                        p7m.Close();
                    }

                    document.Close();
                    result = ms.ToDeepCopyArray();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }

            }


            return result;
        }

        /// <summary>
        ///  Add configured Page Sigmatutre
        /// </summary>
        /// <param name="fileBlob"></param>
        /// <returns></returns>
        public byte[] AddLabelToPages(byte[] fileBlob)
        {
            PdfReader reader = new PdfReader(fileBlob);
            string tempFile = Path.Combine(Path.GetTempPath(), "SC_" + Guid.NewGuid() + ".tmp");
            logger.DebugFormat("EtichettaPdfWithOverprint..Pages:{0}", reader.NumberOfPages);
            try
            {
                Document document = null;
                PdfWriter writer = null;
                PdfContentByte cb = null;
                try
                {
                    for (int j = 1; j <= reader.NumberOfPages; j++)
                    {
                        var rt = reader.GetPageRotation(j);
                        var rect = reader.GetPageSizeWithRotation(j); //
                        if (j == 1)
                        {
                            document = new Document(rect);
                            writer = PdfWriter.GetInstance(document, new FileStream(tempFile, FileMode.Create));
                            /*
                             * PDF-A: lo standard pdf-a NECESSITA che i font siano embedded nel documento. 
                             * I font usati per stampare l'etichetta ed il numero di pagina NON fanno eccezione.
                             */
                            //writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);//i tried with 1_7instead, same result
                            //writer.PDFXConformance = PdfWriter.PDFA1B;
                            //writer.CreateXmpMetadata();
                            document.Open();
                            cb = writer.DirectContent;
                        }
                        else
                        {
                            document.SetPageSize(rect);
                            document.NewPage();
                        }

                        var page = writer.GetImportedPage(reader, j);
                        logger.DebugFormat("AddLabelToPages -> GetPageRotation: {0}", rt);
                        logger.DebugFormat("AddLabelToPages -> scalePercent: {0}", scalePercent);
                        if (rt == 90)
                            cb.AddTemplate(page, 0, -1.0F, scalePercent, 0, 0, rect.Height);
                        else if (rt == 270)
                            cb.AddTemplate(page, 0, scalePercent, -1f, 0, rect.Width, 0);
                        else if (rt == 180)
                        {
                            double angle = Math.PI; //Math.sin/cos want in radians
                            switch (rt)
                            {
                                case 90: angle = Math.PI * 90 / 180; break;
                                case 180: angle = Math.PI * 180 / 180; break;
                                case 270: angle = Math.PI * 270 / 180; break;
                            }
                            float a = (float)Math.Cos(angle);
                            float b = (float)Math.Sin(angle);
                            float c = (float)-Math.Sin(angle);
                            float d = (float)Math.Cos(angle);
                            cb.AddTemplate(page, a, b, c, d, rect.Width, rect.Height);

                        }
                        else
                            cb.AddTemplate(page, 1f, 0, 0, scalePercent, 0, 0);
                        //#warning GESTIRE MEGLIO LA ROTAZIONE! Se il documento viene ruotato, la grafica eventualmente associata ai commenti PDF (linee rosse, fumetti, eccetera) viene disegnata sbagliata.
                        //                            cb.AddTemplate(page, 0, 0);
                        // put header                    
                        string testo = testoEtichetta.Replace("(pagina)", j.ToString());
                        testo = testo.Replace("(pagine)", reader.NumberOfPages.ToString());

                        string FontName = FontFactory.COURIER;
                        if (string.Compare(FontFace, "HELVETICA", true) == 0)
                            FontName = FontFactory.HELVETICA;
                        if (string.Compare(FontFace, "TIMES", true) == 0)
                            FontName = FontFactory.TIMES;
                        int style = iTextSharp.text.Font.NORMAL;
                        if (FontStyle.IndexOf("Bold") >= 0)
                            style |= iTextSharp.text.Font.BOLD;
                        if (FontStyle.IndexOf("Italic") >= 0)
                            style |= iTextSharp.text.Font.ITALIC;

                        var phrase = new Phrase();
                        var chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                        phrase.Add(chunk);
                        var ct = new ColumnText(cb);
                        ct.SetSimpleColumn(phrase, posizione.X, (rect.Height - font.SizeInPoints) - posizione.Y, rect.Right, 0, 0, Element.ALIGN_LEFT);
                        ct.Go();
                        if (testoFooter.Length > 0)
                        {
                            testo = testoFooter.Replace("(pagina)", j.ToString());
                            testo = testo.Replace("(pagine)", reader.NumberOfPages.ToString());

                            phrase = new Phrase();
                            chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                            phrase.Add(chunk);
                            ct = new ColumnText(cb);
                            ct.SetSimpleColumn(phrase, posizione.X, posizione.Y, rect.Right, 0, 0, Element.ALIGN_LEFT);
                            ct.Go();
                        }
                        cb.Stroke();

                        PrintRedirected.ExtractPdfComments(ref reader, ref writer, j);
                    }
                }
                finally
                {
                    ////Try to dispose ITextSharp
                    if (reader != null)
                        reader.Close();
                    if (document != null)
                        document.Close();
                    if (writer != null)
                        writer.Close();
                }
                return File.ReadAllBytes(tempFile);
            }
            finally
            {
                try
                {
                    bool DelFile = string.Compare(ConfigurationManager.AppSettings["DeleteFile"], "true", true) == 0;
                    if (DelFile)
                        File.Delete(tempFile);
                    else
                        logger.DebugFormat("EtichettaPdfWithOverprint..File:{0}", tempFile);
                }
                catch (Exception ex) { logger.Error(ex); }
            }
        }

        public byte[] AddLabelToPagesNew(byte[] fileBlob)
        {
            byte[] result = null;
            var pdf = new PdfContentInfo(fileBlob);

            PdfReader reader = null;
            PdfWriter writer = null;
            using (reader = pdf.GetReader())
            using (var ms = new MemoryStream())
            using (var document = new Document())
            using (writer = PdfWriter.GetInstance(document, ms))
            {

                logger.DebugFormat("EtichettaPdfWithOverprint..Pages:{0}", reader.NumberOfPages);
                try
                {
                    PdfContentByte cb = null;

                    document.Open();
                    cb = writer.DirectContent;

                    for (int j = 1; j <= reader.NumberOfPages; j++)
                    {
                        var rt = reader.GetPageRotation(j);
                        var rect = reader.GetPageSizeWithRotation(j);

                        document.SetPageSize(rect);
                        document.NewPage();

                        var page = writer.GetImportedPage(reader, j);

                        var tx = PdfTransformerFactory.Default().SetOrigin(page, rect);
                        cb.TransformTo(tx);

                        string testo = testoEtichetta.Replace("(pagina)", j.ToString());
                        testo = testo.Replace("(pagine)", reader.NumberOfPages.ToString());

                        string FontName = FontFactory.COURIER;
                        if (string.Compare(FontFace, "HELVETICA", true) == 0)
                            FontName = FontFactory.HELVETICA;
                        if (string.Compare(FontFace, "TIMES", true) == 0)
                            FontName = FontFactory.TIMES;
                        int style = iTextSharp.text.Font.NORMAL;
                        if (FontStyle.IndexOf("Bold") >= 0)
                            style |= iTextSharp.text.Font.BOLD;
                        if (FontStyle.IndexOf("Italic") >= 0)
                            style |= iTextSharp.text.Font.ITALIC;

                        var phrase = new Phrase();
                        var chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                        phrase.Add(chunk);
                        var ct = new ColumnText(cb);
                        ct.SetSimpleColumn(phrase, posizione.X, (rect.Height - font.SizeInPoints) - posizione.Y, rect.Right, 0, 0, Element.ALIGN_LEFT);
                        ct.Go();
                        if (testoFooter.Length > 0)
                        {
                            testo = testoFooter.Replace("(pagina)", j.ToString());
                            testo = testo.Replace("(pagine)", reader.NumberOfPages.ToString());

                            phrase = new Phrase();
                            chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                            phrase.Add(chunk);
                            ct = new ColumnText(cb);
                            ct.SetSimpleColumn(phrase, posizione.X, posizione.Y, rect.Right, 0, 0, Element.ALIGN_LEFT);
                            ct.Go();
                        }
                        cb.Stroke();

                        PrintRedirected.ExtractPdfComments(ref reader, ref writer, j);
                    }

                    document.Close();
                    result = ms.ToDeepCopyArray();

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Add configured Page Sigmatutre
        /// </summary>
        /// <example>
        /// Page 1 of 10
        /// </example>
        /// <param name="fileBlob"></param>
        /// <returns>
        /// New PDF File
        /// </returns>
        public byte[] AddLabelToPagesWithOverprint(byte[] fileBlob)
        {
            PdfReader reader = new PdfReader(fileBlob);
            logger.DebugFormat("EtichettaPdfWithOverprint..New Pages:{0}", reader.NumberOfPages);

            PdfStamper stamper = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    stamper = new PdfStamper(reader, memoryStream);
                    string FontName = FontFactory.COURIER;
                    if (string.Compare(FontFace, "HELVETICA", true) == 0)
                        FontName = FontFactory.HELVETICA;
                    if (string.Compare(FontFace, "TIMES", true) == 0)
                        FontName = FontFactory.TIMES;
                    int style = iTextSharp.text.Font.NORMAL;
                    if (FontStyle.IndexOf("Bold") >= 0)
                        style |= iTextSharp.text.Font.BOLD;
                    if (FontStyle.IndexOf("Italic") >= 0)
                        style |= iTextSharp.text.Font.ITALIC;
                    //
                    var fontForText = FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0));
                    for (int j = 1; j <= reader.NumberOfPages; j++)
                    {
                        var rect = reader.GetPageSizeWithRotation(j);
                        //
                        var underContent = stamper.GetOverContent(j);
                        underContent.BeginText();
                        underContent.SetFontAndSize(fontForText.BaseFont, font.SizeInPoints);
                        underContent.SetColorFill(new iTextSharp.text.BaseColor(0, 0, 0));
                        underContent.SaveState();
                        // put header
                        string testo = testoEtichetta.Replace("(pagina)", j.ToString());
                        testo = testo.Replace("(pagine)", reader.NumberOfPages.ToString());
                        underContent.ShowTextAligned(PdfContentByte.ALIGN_LEFT, testo, posizione.X, (rect.Height - font.SizeInPoints) - posizione.Y, 0);
                        if (testoFooter.Length > 0)
                        {
                            testo = testoFooter.Replace("(pagina)", j.ToString());
                            testo = testo.Replace("(pagine)", reader.NumberOfPages.ToString());
                            underContent.ShowTextAligned(PdfContentByte.ALIGN_LEFT, testo, posizione.X, posizione.Y, 0);
                        }
                        underContent.EndText();
                        underContent.RestoreState();
                    }
                }
                finally
                {
                    if (stamper != null)
                        stamper.Close();
                    if (reader != null)
                        reader.Close();
                }
                return memoryStream.ToArray();
            }
        }


        [Obsolete("Use AddLabelToPages + AddSignatureToPageAndResize", false)]
        public bool EtichettaPdf()
        {
            PdfReader reader = new PdfReader(inpFile);
            int pages = reader.NumberOfPages,
              pagestw = reader.NumberOfPages + ((coverWMF.Length > 0) ? 1 : 0);
            logger.DebugFormat("EtichettaPdf..Pages:{0} - Sign:{1}", pages, pagestw);
            Document document = null;
            PdfWriter writer = null;
            PdfContentByte cb = null;

            PdfImportedPage page;
            iTextSharp.text.Rectangle rect = null;
            Chunk chunk = null;
            Phrase phrase = null;
            ColumnText ct = null;

            int rt = 0;
            try
            {
                for (int j = 0; j < pages; j++)
                {
                    rt = reader.GetPageRotation(j + 1);
                    rect = reader.GetPageSizeWithRotation(j + 1); //
                    if (j == 0)
                    {
                        logger.Debug("Document..add");
                        document = new Document(rect);
                        writer = PdfWriter.GetInstance(document, new FileStream(outFile, FileMode.Create));
                        document.Open();
                        cb = writer.DirectContent;
                    }
                    else
                    {
                        document.SetPageSize(rect);
                        document.NewPage();
                    }

                    page = writer.GetImportedPage(reader, j + 1);

                    logger.DebugFormat("AddTemplate {0} - {1} - {2} - {3}", j, rt, scalePercent, rect.Width);

                    if (rt == 90)
                        cb.AddTemplate(page, 0, -1.0F, scalePercent, 0, 0, rect.Height);
                    else if (rt == 270)
                        cb.AddTemplate(page, 0, scalePercent, -1f, 0, rect.Width, 0);
                    else if (rt == 180)
                    {
                        double angle = Math.PI; //Math.sin/cos want in radians
                        switch (rt)
                        {
                            case 90: angle = Math.PI * 90 / 180; break;
                            case 180: angle = Math.PI * 180 / 180; break;
                            case 270: angle = Math.PI * 270 / 180; break;
                        }
                        float a = (float)Math.Cos(angle);
                        float b = (float)Math.Sin(angle);
                        float c = (float)-Math.Sin(angle);
                        float d = (float)Math.Cos(angle);
                        cb.AddTemplate(page, a, b, c, d, rect.Width, rect.Height);
                    }
                    else
                        cb.AddTemplate(page, 1f, 0, 0, scalePercent, 0, 0);

                    //cb.AddTemplate(page, 1f, 1f);

                    // put header
                    int pagen = j + 1;
                    string testo = testoEtichetta.Replace("(pagina)", pagen.ToString());
                    testo = testo.Replace("(pagine)", pagestw.ToString());

                    string FontName = FontFactory.COURIER;
                    if (string.Compare(FontFace, "HELVETICA", true) == 0)
                        FontName = FontFactory.HELVETICA;
                    if (string.Compare(FontFace, "TIMES", true) == 0)
                        FontName = FontFactory.TIMES;
                    int style = iTextSharp.text.Font.NORMAL;
                    if (FontStyle.IndexOf("Bold") >= 0)
                        style |= iTextSharp.text.Font.BOLD;
                    if (FontStyle.IndexOf("Italic") >= 0)
                        style |= iTextSharp.text.Font.ITALIC;

                    chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                    document.Add(chunk);

                    if (testoFooter.Length > 0)
                    {
                        testo = testoFooter.Replace("(pagina)", pagen.ToString());
                        testo = testo.Replace("(pagine)", pagestw.ToString());

                        phrase = new Phrase();
                        chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                        phrase.Add(chunk);
                        ct = new ColumnText(cb);
                        ct.SetSimpleColumn(phrase, 40, 10, rect.Right, 0, 0, Element.ALIGN_LEFT);
                        ct.Go();
                    }
                    cb.Stroke();
                }

                if (coverWMF.Length > 0)    // retro copertina con le firme
                {
                    string wmfFile = Path.Combine(Path.GetTempPath(), "SC_" + Guid.NewGuid() + ".tmp");
                    try
                    {
                        FileStream fs = new FileStream(wmfFile + ".EMF", FileMode.Create);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(coverWMF, 0, coverWMF.Length);
                        fs.Close();

                        using (Metafile mf = new Metafile(wmfFile + ".EMF"))
                        {
                            mf.Save(wmfFile + ".WMF", ImageFormat.Emf);
                        }

                        iTextSharp.text.Image wmf = iTextSharp.text.Image.GetInstance(wmfFile + ".WMF");
                        //wmf.SetAbsolutePosition(10f, 550f);
                        //wmf.ScaleToFit(400, 550);
                        //wmf.SetDpi(300, 300);
                        wmf.SetAbsolutePosition(30f, 300f);
                        wmf.ScaleToFit(400, 550);
                        wmf.SetDpi(300, 300);

                        document.NewPage();

                        // label
                        string testo = testoEtichetta.Replace("(pagina)", pagestw.ToString());
                        testo = testo.Replace("(pagine)", pagestw.ToString());

                        string FontName = FontFactory.COURIER;
                        if (string.Compare(FontFace, "HELVETICA", true) == 0)
                            FontName = FontFactory.HELVETICA;
                        if (string.Compare(FontFace, "TIMES", true) == 0)
                            FontName = FontFactory.TIMES;
                        int style = iTextSharp.text.Font.NORMAL;
                        if (FontStyle.IndexOf("Bold") >= 0)
                            style |= iTextSharp.text.Font.BOLD;
                        if (FontStyle.IndexOf("Italic") >= 0)
                            style |= iTextSharp.text.Font.ITALIC;

                        chunk = new Chunk(testo,
                          FontFactory.GetFont(FontName,
                          font.SizeInPoints,
                          style,
                          new iTextSharp.text.BaseColor(0, 0, 0)));
                        document.Add(chunk);

                        document.Add(wmf);

                        if (testoFooter.Length > 0)
                        {
                            testo = testoFooter.Replace("(pagina)", pagestw.ToString());
                            testo = testo.Replace("(pagine)", pagestw.ToString());

                            phrase = new Phrase();
                            chunk = new Chunk(testo, FontFactory.GetFont(FontName, font.SizeInPoints, style, new iTextSharp.text.BaseColor(0, 0, 0)));
                            phrase.Add(chunk);
                            ct = new ColumnText(cb);
                            ct.SetSimpleColumn(phrase, 40, 10, rect.Right, 0, 0, Element.ALIGN_LEFT);
                            ct.Go();
                        }
                    }

                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message + " " + exp.StackTrace);
                    }
                    File.Delete(wmfFile);
                    File.Delete(wmfFile + ".WMF");
                    File.Delete(wmfFile + ".EMF");
                }
            }
            finally
            {

                if (reader != null)
                    reader.Close();
                document.Close();
            }
            return true;
        }


        public byte[] RightPdf(byte[] fileBlob, ConversionSecureParameter secureParameter)
        {
            if (string.IsNullOrEmpty(secureParameter.Password))
            {
                secureParameter.Password = ConfigurationManager.AppSettings["PdfOwnerPassword"] == null ? "." : ConfigurationManager.AppSettings["PdfOwnerPassword"].ToString();
            }
            PdfReader reader = new PdfReader(fileBlob);
            using (MemoryStream fileMemoryStream = new MemoryStream())
            {
                Document document = null;
                PdfWriter writer = null;
                PdfContentByte cb = null;
                try
                {
                    PdfImportedPage page;
                    iTextSharp.text.Rectangle rect = null;

                    for (int j = 0; j < reader.NumberOfPages; j++)
                    {
                        int rt = reader.GetPageRotation(j + 1);
                        rect = reader.GetPageSizeWithRotation(j + 1); //
                        if (j == 0)
                        {
                            document = new Document(rect);
                            writer = PdfWriter.GetInstance(document, fileMemoryStream);
                            writer.SetEncryption(PdfWriter.STRENGTH40BITS, secureParameter.User, secureParameter.Password,
                                 (secureParameter.AllowPrinting ? PdfWriter.AllowPrinting : 0)
                                | (secureParameter.AllowModifyContents ? PdfWriter.AllowModifyContents : 0)
                                | (secureParameter.AllowCopy ? PdfWriter.AllowCopy : 0)
                                | (secureParameter.AllowModifyAnnotations ? PdfWriter.AllowModifyAnnotations : 0)
                                | (secureParameter.AllowFillIn ? PdfWriter.AllowFillIn : 0)
                                | (secureParameter.AllowScreenReaders ? PdfWriter.AllowScreenReaders : 0)
                                | (secureParameter.AllowAssembly ? PdfWriter.AllowAssembly : 0)
                                | (secureParameter.AllowDegradedPrinting ? PdfWriter.AllowDegradedPrinting : 0));
                            document.Open();
                            cb = writer.DirectContent;
                        }
                        else
                        {
                            document.SetPageSize(rect);
                            document.NewPage();
                        }

                        page = writer.GetImportedPage(reader, j + 1);

                        if (rt == 90)
                            cb.AddTemplate(page, 0, -1.0F, 1.0F, 0, 0, rect.Height);
                        else if (rt == 270)
                            cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, rect.Width, 0);
                        else if (rt == 180)
                        {
                            double angle = Math.PI; //Math.sin/cos want in radians
                            switch (rt)
                            {
                                case 90: angle = Math.PI * 90 / 180; break;
                                case 180: angle = Math.PI * 180 / 180; break;
                                case 270: angle = Math.PI * 270 / 180; break;
                            }
                            float a = (float)Math.Cos(angle);
                            float b = (float)Math.Sin(angle);
                            float c = (float)-Math.Sin(angle);
                            float d = (float)Math.Cos(angle);
                            cb.AddTemplate(page, a, b, c, d, rect.Width, rect.Height);
                        }
                        else
                            cb.AddTemplate(page, 1.0F, 0, 0, 1.0F, 0, 0);
                        //if (rt == 90 || rt == 270)
                        //    cb.AddTemplate(page, 0, 1f, -1f, 0, 0, rect.Height);
                        //else
                        //    cb.AddTemplate(page, 0, 0);


                    }
                }
                finally
                {
                    if (document != null)
                        document.Close();
                    if (reader != null)
                        reader.Close();
                }
                return fileMemoryStream.ToArray();
            }
        }

        public byte[] Watermark(byte[] fileBlob, string watermark)
        {
            testoEtichetta = watermark;
            return WatermarkPdf(fileBlob);
        }

        /// <summary>
        /// Add watermark to a document
        /// </summary>
        /// <returns>
        /// true if watermak is apply
        /// </returns>
        private byte[] WatermarkPdf(byte[] fileBlob)
        {
            PdfReader reader = new PdfReader(fileBlob);
            int pages = reader.NumberOfPages;
            using (MemoryStream fileMemoryStream = new MemoryStream())
            {
                PdfStamper stamper = new PdfStamper(reader, fileMemoryStream);
                iTextSharp.text.Rectangle rect = null;
                string testo = testoEtichetta;
                int style = iTextSharp.text.Font.NORMAL;
                string FontName = FontFactory.COURIER;

                if (string.Compare(FontFace, "HELVETICA", true) == 0)
                    FontName = FontFactory.HELVETICA;
                if (string.Compare(FontFace, "TIMES", true) == 0)
                    FontName = FontFactory.TIMES;
                if (FontStyle.IndexOf("Bold") >= 0)
                    style |= iTextSharp.text.Font.BOLD;
                if (FontStyle.IndexOf("Italic") >= 0)
                    style |= iTextSharp.text.Font.ITALIC;

                var gstate = new iTextSharp.text.pdf.PdfGState();
                gstate.FillOpacity = fillOpacity;
                gstate.StrokeOpacity = fillOpacity;

                var watermarkFont = FontFactory.GetFont(FontName, font.SizeInPoints, style, Color);


                Single offset = 0.0F;
                Single offsetX = 0.0F;
                Single currentY = 0;
                Single currentX = 0;
                rect = reader.GetPageSizeWithRotation(1);
                try
                {
                    for (int j = 1; j <= pages; j++)
                    {
                        var underContent = stamper.GetOverContent(j);
                        //stamper.GetOverContent(j);                    
                        underContent.BeginText();
                        underContent.SetFontAndSize(watermarkFont.BaseFont, font.SizeInPoints);
                        underContent.SetColorFill(Color);
                        underContent.SaveState();
                        underContent.SetGState(gstate);
                        //underContent.SetTextMatrix(font.SizeInPoints+2, font.SizeInPoints + 2);
                        if (testo.Length > 1)
                        {
                            currentY = (rect.Height / 2) + ((font.SizeInPoints * testo.Length) / 2);
                            if (watermarkRotation != 90)
                                currentX = (rect.Width / 2) + ((font.SizeInPoints * testo.Length) / 2);
                            else
                                currentX = (rect.Width / 2);
                        }
                        else
                        {
                            currentY = (rect.Height / 2);
                            currentX = (rect.Width / 2);
                        }
                        if (testo.Contains("#"))
                        {
                            string[] partTesto = testo.Split(new Char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                            int yPlus = 0;
                            foreach (var item in partTesto)
                            {
                                float extY = (yPlus * font.SizeInPoints);
                                if (extY > 0)
                                    extY += 20;
                                underContent.ShowTextAligned(PdfContentByte.ALIGN_CENTER, item, rect.Width / 2, (rect.Height / 2) - extY, watermarkRotation);
                                yPlus += 1;
                            }

                        }
                        else
                            for (int i = 0; i < testo.Length; i++)
                            {
                                if (i > 0)
                                {
                                    offset = (i * font.SizeInPoints);
                                    if (watermarkRotation != 90)
                                        offsetX = (i * font.SizeInPoints);
                                }
                                else
                                {
                                    offset = 0;
                                    offsetX = 0;
                                }

                                underContent.ShowTextAligned(iTextSharp.text.Element.ALIGN_CENTER, testo[testo.Length - (i + 1)].ToString(), currentX - offsetX, currentY - offset, watermarkRotation);

                            }

                        //float textAngle = (float)GetHypotenuseAngleInDegreesFrom(rect.Height, rect.Width);

                        //underContent.ShowTextAligned(PdfContentByte.ALIGN_CENTER, testo, rect.Width / 2, rect.Height / 2, watermarkRotation);

                        underContent.EndText();
                        underContent.RestoreState();
                    }
                    stamper.FormFlattening = true;
                }
                finally
                {
                    try { if (stamper != null) stamper.Close(); }
                    catch { }
                    try { if (reader != null) reader.Close(); }
                    catch { }
                }
                return fileMemoryStream.ToArray();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

    }
}
