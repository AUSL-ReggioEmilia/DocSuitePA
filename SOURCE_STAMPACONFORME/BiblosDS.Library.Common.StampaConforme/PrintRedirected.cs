using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using VecompSoftware.Commons;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Commons.Configuration;
using VecompSoftware.Commons.Infos;
using VecompSoftware.CompliantPrinting;
using VecompSoftware.Helpers.iTextSharp;
using VecompSoftware.Helpers.iTextSharp.CommonEx;
using VecompSoftware.StampaConforme.Converter.Common;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampaConforme.Services.Common.Cache;

namespace BiblosDS.Library.Common.StampaConforme
{
    public class PrintRedirected
    {
        #region [ Fields ]
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PrintRedirected));
        private readonly ICacheService _cacheService;
        private readonly PdfToThumbnailConverter _pdfToThumbnailConverter;
        private readonly XmlToHtmlConverter _xmlToHtmlConverter;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PrintRedirected()
        {
            _cacheService = new CacheService();
            _pdfToThumbnailConverter = new PdfToThumbnailConverter();
            _xmlToHtmlConverter = new XmlToHtmlConverter();
        }
        #endregion

        #region [ Methods ]    
        public byte[] ConvertPdfToThumbnailPng(byte[] Doc)
        {
            return _pdfToThumbnailConverter.Convert(Doc, "pdf", "png", AttachConversionMode.Default);
        }

        public byte[] ConvertXmlToPdfWithStylesheet(byte[] documentContent, byte[] xsltContent, string fileName, string label)
        {
            logger.DebugFormat("ConvertXmlToPdfWithStylesheet {0} ({1}) {2}", fileName, PathUtil.EXTENSIONPDF, label);
            string xslt = Encoding.UTF8.GetString(xsltContent);
            if (!_xmlToHtmlConverter.IsDocumentProcessable(fileName.ToPath().GetExtension()))
            {
                return new byte[] { };
            }

            string originalFileName = fileName;
            if (!ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.DisableDSW7Compliant", false))
            {
                if (!originalFileName.ToPath().HasExtension())
                {
                    originalFileName = PathUtil.UNDEFINEDFILENAME.ToPath().ChangeExtension("." + originalFileName);
                }
            }
            byte[] convertedDocument = ConvertToPdf(documentContent, xslt, PathUtil.EXTENSIONPDF, originalFileName, out bool isEncrypted);
            if (!isEncrypted)
            {
                return AddLabelToConvertedDocument(PathUtil.EXTENSIONPDF, label, null, convertedDocument, documentContent);
            }
            return convertedDocument;
        }

        /// <summary>
        /// Converte un documento ed applica la label
        /// </summary>
        /// <returns>
        /// Documento convertito e con label
        /// </returns>
        public byte[] ConvertToFormatLabeled(byte[] documentContent, string fileName, string ext, string label, AttachConversionMode mode = AttachConversionMode.Default)
        {
            logger.DebugFormat("ConvertToFormatLabeled {0} ({1}) {2}", fileName, ext, label);
            //Conversione nel formato richiesto      
            byte[] res = ToRasterFormat(documentContent, fileName, ext, out bool isEncripted, mode);
            if (!isEncripted)
            {
                return AddLabelToConvertedDocument(ext, label, null, res, documentContent);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentContent"></param>
        /// <param name="fileName"></param>
        /// <param name="ext"></param>
        /// <param name="label"></param>
        /// <param name="formBoxConfig"></param>
        /// <returns></returns>
        public byte[] ConvertToFormatLabeledWithForm(byte[] documentContent, string fileName, string ext, string label, BoxConfig formBoxConfig)
        {
            if (formBoxConfig == null)
            {
                throw new ConfigurationException("Nessuna informazione di configurazione legata alla form fornita in input.");
            }

            if (formBoxConfig.BoxLine == null || !formBoxConfig.BoxLine.Any())
            {
                throw new ConfigurationException("Nessuna informazione di configurazione legata alle opzioni fornita in input.");
            }

            formBoxConfig = BoxConfig.MergeWithConfig(formBoxConfig, logger);

            if (formBoxConfig.DrawingPageNumber < -1)
            {
                formBoxConfig.DrawingPageNumber = -1;
            }

            if (formBoxConfig.DrawingPageNumber == 0)
            {
                formBoxConfig.DrawingPageNumber = 1;
            }

            var numPagina = formBoxConfig.DrawingPageNumber.Value;

            var buffer = ConvertToFormatLabeled(documentContent, fileName, ext, label);
            var fname = Path.GetTempFileName() + ".pdf";

            try
            {
                using (var fs = File.Create(fname))
                {
                    var pdfReader = new PdfReader(buffer);

                    try
                    {
                        //Non si può scrivere in una pagina inesistente: quindi viene impostata l'ultima pagina valida del documento.
                        if (numPagina < 1 || numPagina > pdfReader.NumberOfPages)
                        {
                            numPagina = pdfReader.NumberOfPages;
                        }
                        //Recupera le dimensioni della pagina in cui si sta scrivendo.
                        var paginaPerDimensioni = pdfReader.GetPageSizeWithRotation(numPagina);
                        //Re-imposta le dimensioni a seconda del formato della pagina.
                        formBoxConfig.X = (int)Math.Ceiling(Math.Abs(paginaPerDimensioni.Left - formBoxConfig.X.Value));
                        formBoxConfig.Y = (int)Math.Ceiling(Math.Abs(paginaPerDimensioni.Top - formBoxConfig.Y.Value));

                        try
                        {
                            using (var pdfStamper = new PdfStamper(pdfReader, fs))
                            {
                                try
                                {
                                    var canvas = pdfStamper.GetOverContent(numPagina);
                                    var cfgOpzioni = formBoxConfig.BoxLine.First();

                                    using (var layout = new PdfDrawer(canvas, ref formBoxConfig, ref cfgOpzioni).BeginLayout())
                                    {
                                        foreach (var opzione in formBoxConfig.BoxLine)
                                        {
                                            layout.AddMessage(opzione.Message, opzione.SelectedValue, opzione.Options);
                                        }
                                    }

                                    pdfStamper.Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);//i tried with 1_7instead, same result
                                    pdfStamper.Writer.PDFXConformance = PdfWriter.PDFX1A2001;
                                    pdfStamper.Writer.CreateXmpMetadata();
                                }
                                finally
                                {
                                    pdfStamper.Close();
                                }
                            }
                        }
                        finally
                        {
                            pdfReader.Close();
                        }
                    }
                    finally
                    {
                        try { pdfReader.Close(); }
                        catch { }

                        fs.Close();
                    }
                }
                buffer = File.ReadAllBytes(fname);
            }
            finally
            {
                try { File.Delete(fname); }
                catch { }
            }

            return buffer;
        }

        /// <summary>
        /// Converte un documento in formato PDF utilizzando i servizi di conversione
        /// </summary>
        /// <returns>
        /// Documento convertito nel formato <paramref name="extReq">ExtReq</paramref>
        /// </returns>
        public byte[] ToRasterFormat(byte[] documentContent, string fileName, string extReq, out bool isEncripted, AttachConversionMode mode = AttachConversionMode.Default)
        {
            try
            {
                isEncripted = false;
                logger.DebugFormat("ToRasterFormat {0} ({1})", fileName, extReq);
                if (extReq.ToLower().Contains("tif"))
                {
                    if (ConfigurationHelper.GetValueOrDefault<bool>("ConvertTiffAsPdf", false))
                    {
                        logger.Debug("TIF request redirect to PDF conversion");
                        extReq = "PDF";
                    }
                    else
                    {
                        throw new FormatException("Conversione non più supportata, utilizzare il formato PDF");
                    }
                }

                string originalFileName = fileName;
                if (!ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.DisableDSW7Compliant", false))
                {
                    if (!originalFileName.ToPath().HasExtension())
                    {
                        originalFileName = PathUtil.UNDEFINEDFILENAME.ToPath().ChangeExtension("." + originalFileName);
                    }
                }

                //Estrazione del documento se firmato            
                // CacheVerify: Verifica se il documento è presente in cache
                byte[] cachedDocumentContent = _cacheService.FindDocument(documentContent, extReq);
                if (cachedDocumentContent != null)
                {
                    return cachedDocumentContent;
                }
                // Converte il documento nel formato richiesto
                var ret = ConvertToPdf(documentContent, extReq, originalFileName, out isEncripted, mode);
                //Put file to cache
                if (!isEncripted)
                {
                    _cacheService.CreateDocument(ret, documentContent, extReq);
                }

                return ret;
            }
            catch (Exception exp)
            {
                logger.Error(exp);
                throw;
            }
            finally
            {
                logger.Debug("ToRasterFormat End");
            }
        }

        /// <summary>
        /// Appliva il watermark su di un documento
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="watermark"></param>
        /// <returns></returns>
        public byte[] EtichettaWatermark(byte[] Doc, string watermark)
        {
            // add Head+Foot
            PdfLabeler ePdf = new PdfLabeler();
            // parse label
            // <Watermark>
            //    <Text>Protocollo 123 (pagina) di (pagine)</Text>
            //    <Font Face="Arial" Size="18" Style="Bold,Italic" />
            //    <Point WatermarkRotation="90" FillOpacity="3"/>
            // </Watermark>
            XmlDocument document = new XmlDocument();
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
            XmlTextReader reader = new XmlTextReader(watermark, XmlNodeType.Element, context);
            document.Load(reader);
            string watermarkLabel = string.Empty;
            XmlNode root = document.SelectSingleNode("Watermark");
            if (root != null)
            {
                XmlNode textNode = root.SelectSingleNode("Text");
                watermarkLabel = textNode.InnerText.ToString();
                ReadBaseConfig(ePdf, root);
                XmlNode point = root.SelectSingleNode("WatermarkConfig");
                if (point != null)
                {
                    if (point.Attributes["WatermarkRotation"] != null)
                    {
                        ePdf.watermarkRotation = int.Parse(point.Attributes["WatermarkRotation"].InnerText);
                    }

                    if (point.Attributes["FillOpacity"] != null)
                    {
                        ePdf.fillOpacity = float.Parse(point.Attributes["FillOpacity"].InnerText) / 100;
                    }
                }
                else
                {
                    ePdf.useStartPoint = false;
                }
            }
            return ePdf.Watermark(Doc, watermarkLabel);
        }

        public byte[] MergePdfDocuments(IEnumerable<byte[]> documentBlobs)
        {
            var retval = new byte[0];
            Exception toThrow = null;

            if (documentBlobs != null)
            {
                using (var memStream = new MemoryStream())
                {
                    using (var document = new Document(PageSize.A4))
                    {
                        var copy = new PdfCopy(document, memStream);
                        document.OpenDocument();
                        try
                        {
                            PdfReader reader;
                            int pageN, i;
                            foreach (var blob in documentBlobs)
                            {
                                reader = new PdfReader(blob);
                                pageN = reader.NumberOfPages + 1;

                                try
                                {
                                    if (pageN > 1)
                                    {
                                        for (i = 1; i < pageN; i++)
                                        {
                                            copy.AddPage(copy.GetImportedPage(reader, i));
                                        }
                                    }
                                }
                                catch (Exception exx)
                                {
                                    toThrow = exx;
                                }
                                finally
                                {
                                    try { reader.Close(); }
                                    catch { }
                                }

                                if (toThrow != null)
                                {
                                    throw toThrow;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            toThrow = ex;
                        }
                        finally
                        {
                            try { document.CloseDocument(); }
                            catch { }
                            //    try { copy.Close(); }
                            //    catch { }
                        }
                    }

                    if (toThrow == null)
                    {
                        retval = memStream.GetBuffer();
                    }
                }
            }

            if (toThrow != null)
            {
                throw toThrow;
            }

            return retval;
        }

        #region ALLEGATI PDF

        /// <summary>
        /// Estrae i files "embedded" nel documento il cui contenuto è fornito in input sotto forma di array di bytes.
        /// </summary>
        /// <param name="inputDoc">B[L]OB : contenuto del PDF.</param>
        /// <returns>B[L]OB del documento fornito in input, manipolato.</returns>
        internal byte[] ExtractPdfAttachments(byte[] inputDoc)
        {
            var retval = new ByteBuffer();
            var filesDaConvertire = new System.Collections.Generic.List<byte[]>();

            const string STD_EX_MESSAGE = "Impossibile convertire il documento: uno o piu' file allegati non sono validi.";

            #region ESTRAZIONE ALLEGATI

            try
            {
                var pdfReader = new PdfReader(inputDoc);
                try
                {
                    if (pdfReader.Catalog == null || pdfReader.Catalog.Size < 1)
                    {
                        return null;
                    }

                    var nomiCatalogoPdf = pdfReader.Catalog.GetAsDict(PdfName.NAMES);

                    if (nomiCatalogoPdf == null || nomiCatalogoPdf.Size < 1)
                    {
                        return null;
                    }

                    var filesEmbedded = nomiCatalogoPdf.GetAsDict(PdfName.EMBEDDEDFILES);

                    if (filesEmbedded == null || filesEmbedded.Size < 1)
                    {
                        return null;
                    }

                    var nomiEmbeddedFiles = filesEmbedded.GetAsArray(PdfName.NAMES);

                    if (nomiEmbeddedFiles == null || nomiEmbeddedFiles.Size < 1)
                    {
                        return null;
                    }
                    //Variabili per le iterazioni.
                    PdfDictionary files;
                    byte[] buffer;
                    PRStream stream;
                    PdfIndirectReference indRef;
                    string nomeFile;
                    //Variabili per la ToRasterFormat.
                    bool encrypted;
                    //Si comincia!
                    for (int i = 1; i < nomiEmbeddedFiles.Size; i += 2)
                    {
                        var nomi = nomiEmbeddedFiles.GetAsDict(i);
                        files = nomi.GetAsDict(PdfName.EF);
                        //
                        if (files != null)
                        {
                            foreach (var file in files.Keys)
                            {
                                nomeFile = (nomi.GetAsString(file).ToUnicodeString() ?? string.Empty).Trim();

                                if (nomeFile == string.Empty)
                                {
                                    throw new Exception("Impossibile salvare un allegato senza un nome.");
                                }

                                indRef = files.GetAsIndirectObject(file);

                                if (indRef == null)
                                {
                                    throw new Exception(STD_EX_MESSAGE); //File non valido.
                                }

                                stream = PdfReader.GetPdfObject(indRef) as PRStream;

                                if (stream == null)
                                {
                                    throw new Exception(STD_EX_MESSAGE);
                                }

                                buffer = PdfReader.GetStreamBytes(stream) ?? new byte[0];

                                if (buffer.Length < 1)
                                {
                                    throw new Exception("Impossibile salvare un allegato con dimensione zero.");
                                }
                                //Potrebbe essere necessario ri-convertire l'allegato.
                                var others = ToRasterFormat(buffer, nomeFile, "PDF", out encrypted);
                                if (others != null && others.Length > 0)
                                {
                                    filesDaConvertire.Add(others);
                                }

                                var pdfReader2 = new PdfReader(buffer);
                                for (var n = 1; n < pdfReader2.NumberOfPages + 1; n++)
                                {
                                    var annots = pdfReader2.GetPageN(n).GetAsArray(PdfName.ANNOTS);

                                    if (annots != null)
                                    {
                                        PdfDictionary annot, fs;
                                        for (var j = 0; j < annots.Size; j++)
                                        {
                                            annot = annots.GetAsDict(j);
                                            if (PdfName.FILEATTACHMENT.Equals(annot.GetAsName(PdfName.SUBTYPE)))
                                            {
                                                fs = annot.GetAsDict(PdfName.FS);
                                                files = fs.GetAsDict(PdfName.EF);

                                                foreach (var f in files.Keys)
                                                {
                                                    nomeFile = (fs.GetAsString(f).ToUnicodeString() ?? string.Empty).Trim();

                                                    if (nomeFile == string.Empty)
                                                    {
                                                        throw new Exception("Impossibile salvare un allegato senza un nome.");
                                                    }

                                                    stream = files.GetAsStream(f) as PRStream;

                                                    if (stream == null)
                                                    {
                                                        throw new Exception(STD_EX_MESSAGE);
                                                    }

                                                    buffer = PdfReader.GetStreamBytes(stream) ?? new byte[0];

                                                    if (buffer.Length < 1)
                                                    {
                                                        throw new Exception("Impossibile salvare un allegato con dimensione zero.");
                                                    }

                                                    others = ToRasterFormat(buffer, nomeFile, "PDF", out encrypted);
                                                    //
                                                    if (others != null && others.Length > 0)
                                                    {
                                                        filesDaConvertire.Add(buffer);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                pdfReader2.Close();
                                pdfReader.Close();
                            }
                        }
                    }
                }
                finally
                {
                    try { pdfReader.Close(); }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            #endregion

            #region MERGE IN UN UNICO PDF

            using (var memStream = new MemoryStream())
            {
                PdfReader pdfReader;
                using (var doc = new Document(PageSize.A4))
                {
                    using (var pdfCopy = new PdfCopy(doc, memStream))
                    {
                        doc.Open();
                        foreach (var item in filesDaConvertire)
                        {
                            pdfReader = new PdfReader(item);
                            try
                            {
                                for (int i = 1; i < pdfReader.NumberOfPages + 1; i++)
                                {
                                    pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, i));
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                try { pdfReader.Close(); }
                                catch { }
                            }
                        }
                        doc.Close();
                        pdfCopy.Close();
                    }
                }
                retval.Append(memStream.ToArray());
            }

            #endregion

            return retval.ToByteArray();
        }

        #endregion

        #region COMMENTI PDF

        /// <summary>
        /// Estrae i commenti "embedded" nel documento il cui contenuto è fornito in input sotto forma di array di bytes.
        /// </summary>
        /// <param name="inputDoc">B[L]OB : contenuto del PDF.</param>
        /// <returns>B[L]OB del documento fornito in input, manipolato.</returns>
        internal static byte[] ExtractPdfComments(byte[] inputDoc)
        {
            var pdfReader = new PdfReader(inputDoc);
            var retval = new byte[0];
            //
            using (var document = new Document(PageSize.A4))
            {
                //Usare un memory stream evita di dover usare un file temporaneo su disco.
                using (var memStream = new MemoryStream())
                {
                    using (var pdfWriter = PdfWriter.GetInstance(document, memStream))
                    {
                        document.Open();
                        try
                        {
                            for (var idx = 1; idx < pdfReader.NumberOfPages + 1; idx++)
                            {
                                pdfWriter.DirectContent.AddTemplate(pdfWriter.GetImportedPage(pdfReader, idx), 0f, 0f);

                                var annots = pdfReader.GetPageN(idx)
                                    .GetAsArray(PdfName.ANNOTS);

                                if (annots != null)
                                {
                                    PdfDictionary dict;
                                    iTextSharp.text.Rectangle bounds;
                                    PdfAnnotation toAdd;
                                    System.Collections.Generic.IEnumerable<float> rect;
                                    foreach (var item in annots)
                                    {
                                        dict = PdfReader.GetPdfObject(item) as PdfDictionary;
                                        //Evita di estrarre nuovamente gli allegati.
                                        if (dict != null && !PdfName.FILEATTACHMENT.Equals(dict.GetAsName(PdfName.SUBTYPE)))
                                        {
                                            //Non copiare MAI l'annotazione "/Rect" !
                                            if (dict.Keys != null && dict.Keys.Any(x => x.Equals(PdfName.RECT)))
                                            {
                                                rect = dict.GetAsArray(PdfName.RECT).ArrayList
                                                    .OfType<PdfNumber>()
                                                    .Select(x => x.FloatValue);

                                                bounds = new iTextSharp.text.Rectangle(rect.ElementAt(0), rect.ElementAt(1), rect.ElementAt(2), rect.ElementAt(3));

                                                toAdd = new PdfAnnotation(pdfWriter, bounds);

                                                foreach (var chiave in dict.Keys.OfType<PdfName>())
                                                {
                                                    //Non copiare MAI l'annotazione "/Rect" !
                                                    if (!chiave.Equals(PdfName.RECT))
                                                    {
                                                        toAdd.Put(chiave, dict.Get(chiave));
                                                    }
                                                }
                                                //
                                                pdfWriter.AddAnnotation(toAdd);
                                            }
                                        }
                                    }
                                }
                                document.NewPage();
                            }
                        }
                        finally
                        {
                            try { pdfWriter.Close(); }
                            catch { }
                            try { pdfReader.Close(); }
                            catch { }
                        }
                    }
                    try { document.Close(); }
                    catch { }
                    retval = memStream.ToArray();
                }
            }
            return retval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        internal static void ExtractPdfComments(ref PdfReader reader, ref PdfWriter writer)
        {
            if (reader == null || writer == null)
            {
                return;
            }

            for (int idx = 1; idx < reader.NumberOfPages + 1; idx++)
            {
                ExtractPdfComments(ref reader, ref writer, idx);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="pageNumber"></param>
        internal static void ExtractPdfComments(ref PdfReader reader, ref PdfWriter writer, int pageNumber)
        {
            if (reader == null || writer == null || pageNumber < 1)
            {
                return;
            }

            var annots = reader.GetPageN(pageNumber).GetAsArray(PdfName.ANNOTS);

            if (annots != null)
            {
                PdfDictionary dict;
                iTextSharp.text.Rectangle bounds;
                PdfAnnotation toAdd;
                System.Collections.Generic.IEnumerable<float> rect2;
                foreach (var item in annots)
                {
                    dict = PdfReader.GetPdfObject(item) as PdfDictionary;
                    //Evita di estrarre nuovamente gli allegati.
                    if (dict != null && !PdfName.FILEATTACHMENT.Equals(dict.GetAsName(PdfName.SUBTYPE)))
                    {
                        //Non copiare MAI l'annotazione "/Rect" !
                        if (dict.Keys != null && dict.Keys.Any(x => x.Equals(PdfName.RECT)))
                        {
                            rect2 = dict.GetAsArray(PdfName.RECT).ArrayList
                                .OfType<PdfNumber>()
                                .Select(x => x.FloatValue);

                            bounds = new iTextSharp.text.Rectangle(rect2.ElementAt(0), rect2.ElementAt(1), rect2.ElementAt(2), rect2.ElementAt(3));

                            toAdd = new PdfAnnotation(writer, bounds);

                            foreach (var chiave in dict.Keys.OfType<PdfName>())
                            {
                                //Non copiare MAI l'annotazione "/Rect" !
                                if (!chiave.Equals(PdfName.RECT))
                                {
                                    toAdd.Put(chiave, dict.Get(chiave));
                                }
                            }
                            //
                            writer.AddAnnotation(toAdd);
                        }
                    }
                }
            }
        }

        #endregion

        private byte[] AddLabelToConvertedDocument(string ext, string label, byte[] WmfSigns, byte[] res, byte[] doc)
        {
            logger.DebugFormat("AddLabelToConvertedDocument {0}", ext);

            bool deleteFile = ConfigurationHelper.GetValueOrDefault<bool>("DeleteFile", false);
            // add Head+Foot
            PdfLabeler ePdf = new PdfLabeler();
            // parse label
            // <Label>
            //    <Text>Protocollo 123 (pagina) di (pagine)</Text><Footer>Protocollo 123 (pagina) di (pagine)</Footer>
            //    <Font Face="Arial" Size="18" Style="Bold,Italic" />
            // </Label>
            XmlDocument document = new XmlDocument();
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
            XmlTextReader reader = new XmlTextReader(label, XmlNodeType.Element, context);
            document.Load(reader);

            string labtext = string.Empty;
            string footext = string.Empty;
            XmlNode root = document.SelectSingleNode("Label");
            if (root != null)
            {
                XmlNode text = root.SelectSingleNode("Text");
                labtext = text.InnerText;
                footext = string.Empty;
                text = root.SelectSingleNode("Footer");
                if (text != null)
                {
                    footext = text.InnerText;
                }

                ReadBaseConfig(ePdf, root);
            }
            ePdf.testoEtichetta = labtext;
            ePdf.testoFooter = footext;
            ePdf.coverWMF = (WmfSigns == null) ? new byte[0] : WmfSigns;
            string inFile = Path.Combine(Path.GetTempPath(), "SC_" + Guid.NewGuid() + ".tmp");
            try
            {
                if (!deleteFile)
                {
                    logger.Debug("Source file name: " + inFile);
                }

                File.WriteAllBytes(inFile, res);

                var resized = ePdf.AddSignatureToPageAndResizeNew(File.ReadAllBytes(inFile), doc);
                var labeled = ePdf.AddLabelToPagesNew(resized);
                return labeled;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                try
                {
                    if (deleteFile)
                    {
                        File.Delete(inFile);
                    }
                    else
                    {
                        logger.InfoFormat("End Process, del file:{0} - {1} ", inFile, label);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Lettura della configurazione XML base
        /// </summary>
        /// <param name="ePdf"></param>
        /// <param name="root"></param>
        private static void ReadBaseConfig(PdfLabeler ePdf, XmlNode root)
        {
            XmlNode font = root.SelectSingleNode("Font");
            if (font != null)
            {
                FontStyle fs = new FontStyle();
                ePdf.FontStyle = font.Attributes["Style"].InnerText;
                if (font.Attributes["Style"].InnerText.ToString().IndexOf("Bold") >= 0)
                {
                    fs |= FontStyle.Bold;
                }

                if (font.Attributes["Style"].InnerText.ToString().IndexOf("Italic") >= 0)
                {
                    fs |= FontStyle.Italic;
                }

                ePdf.font = new System.Drawing.Font(font.Attributes["Face"].InnerText.ToString(),
                  int.Parse(font.Attributes["Size"].InnerText), fs);
                ePdf.FontFace = font.Attributes["Face"].InnerText.ToString();
                int r, g, b;
                if (font.Attributes["Color"] != null && !string.IsNullOrEmpty(font.Attributes["Color"].InnerText))
                {
                    string[] rgb = font.Attributes["Color"].InnerText.Split(',');
                    if (rgb.Length >= 3)
                    {
                        bool parseOk = true;
                        if (!int.TryParse(rgb[0], out r))
                        {
                            parseOk = false;
                        }

                        if (!int.TryParse(rgb[1], out g))
                        {
                            parseOk = false;
                        }

                        if (!int.TryParse(rgb[2], out b))
                        {
                            parseOk = false;
                        }

                        if (parseOk)
                        {
                            ePdf.Color = new iTextSharp.text.BaseColor(r, g, b);
                        }
                    }
                }
            }
            XmlNode scale = root.SelectSingleNode("Scale");
            if (scale != null)
            {
                int scalePercent = 0;
                if (int.TryParse(scale.Attributes["scalePercent"].InnerText, out scalePercent))
                {
                    ePdf.scalePercent = scalePercent * 0.01f;
                }
            }
        }

        /// <summary>
        /// Normalizza il pdf:
        ///     - Formato sempre Portrait.
        ///     - Orientamento (rotazione) pagine sempre fisso a 0° / 360°.
        ///     - Contenuto della pagina sempre avente ampiezza massima minore od uguale all'ampiezza del foglio A4.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static byte[] NormalizePdfDocument(byte[] document, out bool isEncrypted, log4net.ILog logger = null)
        {
            byte[] retval = null;
            PdfReader reader = null;

            try
            {
                if (document == null)
                {
                    throw new Exception("Documento nullo: impossibile normalizzare.");
                }

                reader = new PdfReader(document);
                isEncrypted = reader.IsEncrypted();

                if (isEncrypted)
                {
                    return null;
                }

                for (int n = 1; n < reader.NumberOfPages + 1; n++)
                {
                    var cnt = reader.GetPageN(n);
                    var rot = cnt.GetAsNumber(PdfName.ROTATE);
                    var mediabox = cnt.GetAsArray(PdfName.MEDIABOX);

                    if (rot != null && mediabox != null && mediabox.ArrayList != null && mediabox.ArrayList.Count > 0)
                    {
                        var mbox = mediabox.ArrayList;
                        var foglioA4 = PageSize.A4;
                        int finalRot = 0;
                        if (rot != null && rot.IntValue != 0 && rot.IntValue != 360)
                        {
                            switch (rot.IntValue)
                            {
                                case 90:
                                    foglioA4.Rotate().Rotate().Rotate();
                                    break;
                                case 180:
                                    foglioA4.Rotate().Rotate();
                                    break;
                                case 270:
                                    foglioA4.Rotate();
                                    break;
                            }

                            if (((PdfNumber)mbox[0]).FloatValue != foglioA4.Left
                                || ((PdfNumber)mbox[1]).FloatValue != foglioA4.Bottom
                                || ((PdfNumber)mbox[2]).FloatValue != foglioA4.Width
                                || ((PdfNumber)mbox[3]).FloatValue != foglioA4.Height)
                            {
                                finalRot = -(360 - rot.IntValue);
                            }

                            cnt.Put(PdfName.ROTATE, new PdfNumber(finalRot));
                        }
                    }
                }

                using (var ms = new MemoryStream())
                {
                    using (var stmp = new PdfStamper(reader, ms))
                    {
                        stmp.Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);//i tried with 1_7instead, same result
                        stmp.Writer.PDFXConformance = PdfWriter.PDFX1A2001;
                        stmp.Writer.CreateXmpMetadata();

                        stmp.Close();
                    }

                    retval = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error(ex);
                }

                throw;
            }
            finally
            {
                try
                {
                    //Chiude il PDFReader.
                    reader.Close();
                }
                catch { }
            }
            //
            return retval;
        }

        private byte[] ConvertToPdf(byte[] content, string destination, string origin, out bool isEncrypted, AttachConversionMode attachConversionMode = AttachConversionMode.Default)
        {
            return ConvertToPdf(content, null, destination, origin, out isEncrypted, attachConversionMode);
        }

        private byte[] ConvertToPdf(byte[] content, string customStyle, string destination, string origin, out bool isEncrypted, AttachConversionMode attachConversionMode = AttachConversionMode.Default)
        {
            logger.InfoFormat("PrintRedirected.ConvertToPdf: origin = {0}, destination = {1}", origin, destination);
            try
            {
                isEncrypted = false;
                var contentInfo = new ContentInfo(origin, content);
                if (contentInfo.Extension.EqualsIgnoreCase(PathUtil.EXTENSIONPDF)
                    && !ConfigurationHelper.GetValueOrDefault<bool>("CompliantPrinting.EnableUnethicalReading", false))
                {
                    isEncrypted = contentInfo.ToPdfContent().IsEncrypted;
                }

                var tx = CompliantTransformerFactory.ToA4Size();
                tx.AttachConversionMode = attachConversionMode;
                tx.PdfTransformer.PdfA = true;
                tx.CustomStyleContent = customStyle;


                var result = CompliantPrintingHelper.ConvertToPdf(contentInfo, tx);
                logger.InfoFormat("PrintRedirected.ConvertToPdf: result.Length = {0}", result.Content.Length);
                return result.Content;
            }
            catch (Exception ex)
            {
                logger.Error("PrintRedirected.ConvertToPdf: Si è verificato un errore.", ex);
                throw ex;
            }
        }

        public string GetVersion()
        {
            return Assembly.GetAssembly(this.GetType()).GetName().Version.ToString();
        }

        /// <summary>
        /// Verify if extension is supported and service is alive
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns>
        /// True id service IS Valid
        /// </returns>
        public bool IsAlive(string fileExtension)
        {
            var ext = PrintRedirectConfigurations.GetConverter(fileExtension);
            switch (ext)
            {
                case ConverterType.OpenOffice:
                case ConverterType.Redirect:
                    return new ExternalRedirectService().IsAlive(fileExtension, ext);
                case ConverterType.Tif:
                default:
                    return true;
            }

        }

        public int GetNumberOfPages(byte[] content, string fileName)
        {
            var converted = ToRasterFormat(content, fileName, "pdf", out bool isEncrypted, AttachConversionMode.Default);
            return new PdfContentInfo(fileName, converted).NumberOfPages;
        }
        #endregion
    }

}