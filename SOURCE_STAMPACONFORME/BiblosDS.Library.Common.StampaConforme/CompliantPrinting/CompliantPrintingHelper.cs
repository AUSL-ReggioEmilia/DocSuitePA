using System;
using System.Collections.Generic;
using System.Linq;
using BiblosDS.Library.Common.StampaConforme;
using VecompSoftware.CompEd;
using VecompSoftware.Helpers.iTextSharp;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.Infos;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Helpers.iTextSharp.CommonEx;
using VecompSoftware.Helpers.LimilabsMail;
using VecompSoftware.Commons;
using VecompSoftware.StampaConforme.Converter.Common;
using VecompSoftware.StampaConforme.Converter.iTextSharp;

namespace VecompSoftware.CompliantPrinting
{
    public static class CompliantPrintingHelper
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(IPdfContentEx));

        private static PdfContentInfo ConvertByExternalConverter(IContent contentInfo, CompliantTransformer tx)
        {
            byte[] pdfContent = null;
            ContentInfo content = null;
            string extension = String.Empty;

            XmlToHtmlConverter xmlToHtmlConverter = new XmlToHtmlConverter();
            byte[] htmlConversion = null;
            if (xmlToHtmlConverter.IsDocumentProcessable(contentInfo.Extension))
            {
                if (!string.IsNullOrEmpty(tx.CustomStyleContent))
                {
                    htmlConversion = xmlToHtmlConverter.Convert(contentInfo.Content, tx.CustomStyleContent, contentInfo.Extension, PathUtil.EXTENSIONPDF, false);
                }
                else
                {
                    htmlConversion = xmlToHtmlConverter.Convert(contentInfo.Content, contentInfo.Extension, PathUtil.EXTENSIONPDF, AttachConversionMode.Default);
                }                
            }
            // salvataggio del contenuto in html
            if (htmlConversion != null)
            {
                extension = ".html";
                string filename = contentInfo.FileName.Replace(contentInfo.Extension, extension);
                content = new ContentInfo(filename, htmlConversion);
            }
            else
            {
                extension = contentInfo.Extension;
                content = new ContentInfo(contentInfo.FileName, contentInfo.Content);
            }

            var converter = PrintRedirectConfigurations.GetConverter(extension);
            switch (converter)
            {
                case ConverterType.OpenOffice:
                case ConverterType.Redirect:
                    pdfContent = new ExternalRedirectService().Convert(content.Content, content.FileName, "pdf", converter, tx.AttachConversionMode);
                    if (pdfContent.IsNullOrEmpty())
                    {
                        converter = converter == ConverterType.OpenOffice ? ConverterType.Redirect : ConverterType.OpenOffice;
                        pdfContent = new ExternalRedirectService().Convert(content.Content, content.FileName, "pdf", converter, tx.AttachConversionMode);
                    }
                    break;

                case ConverterType.Tif:
                    pdfContent = new TifToPdfConverter().Convert(content.Content, content.FileName, "pdf", tx.AttachConversionMode);
                    break;

                case ConverterType.Image:
                    pdfContent = new ImageToPdfConverter().Convert(content.Content, content.FileName, "pdf", tx.AttachConversionMode);
                    break;

                case ConverterType.Txt:
                    {
                        pdfContent = new TxtToPdfConverter().Convert(content.Content, content.FileName, "pdf", tx.AttachConversionMode);
                    }
                    break;

                default:
                    throw new FormatException("CompliantPrintingHelper.ConvertByExternalConverter: formato non supportato.");
            }

            if (pdfContent.IsNullOrEmpty())
                throw new InvalidOperationException("CompliantPrintingHelper.ConvertByExternalConverter: conversione fallita.");

            return new PdfContentInfo(content.FileName, pdfContent);
        }

        private static PdfContentInfo ConvertPortfolioToPdf(IPdfContent pdf)
        {
            IList<ContentInfo> extractedPortfolios = iTextSharpHelper.ExtractPortfolio(pdf);
            PdfContentInfo result = pdf.ToPdfContent();
            if (result.XfaPresent || result.HasAnnots)
            {
                if (!result.TryToFlattenedPdf(out result))
                {
                    logger.Warn("Errore di appiattimento PDF. TryToFlattenedPdf ha restituito il file originale e non il file appiattito. Nel file verrà inserito un warning nella firma.");
                }
            }

            if (extractedPortfolios == null)
            {
                return result;
            }

            IEnumerable<PdfContentInfo> toMergeContentInfos = Enumerable.Empty<PdfContentInfo>();
            toMergeContentInfos = toMergeContentInfos.Concat(new List<PdfContentInfo>() { result })
                .Concat(extractedPortfolios.Select(ConvertToPdf));
            byte[] mergedContent = iTextSharpHelper.Merge(toMergeContentInfos).Content;
            return new PdfContentInfo(pdf.FileName, mergedContent);
        }

        public static PdfContentInfo ConvertToPdf(IContent contentInfo, CompliantTransformer tx)
        {
            var inputContent = contentInfo;
            if (contentInfo.Extension.IsNullOrWhiteSpace())
                try
                {
                    inputContent = contentInfo.ToIdentifiedContent();
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException("Contenuto del documento non riconosciuto.", ex);
                }

            PdfContentInfo resultContent;
            switch (inputContent.Extension)
            {
                case PathUtil.EXTENSIONPDF:
                    resultContent = ConvertPdfToPdf(inputContent.ToPdfContent(), tx);
                    break;

                case PathUtil.EXTENSIONP7M:
                case ".p7s":
                case ".p7x":
                case ".m7m":
                case ".tsd":
                    var comped = inputContent.ToCompEdContent();
                    var rootContent = ConvertToPdf(comped.Root, tx);
                    var compedResults = new List<PdfContentInfo>() { rootContent };
                    if (tx.RenderSignatures && comped.HasSignatures)
                        compedResults.Add(comped.PrintSignContentReport(false));
                    resultContent = iTextSharpHelper.Merge(compedResults);
                    break;

                //case PathHelper.EmlExtension:
                //case PathHelper.MsgExtension:
                //    try
                //    {
                //        resultContent = ConvertMailToPdf(contentInfo, tx);
                //    }
                //    catch (MsgTypeNotImplementedException ex)
                //    {
                //        if (ex.IsMsg && ex.MsgType.IsMultipartSigned())
                //            resultContent = ConvertByExternalConverter(contentInfo, tx);
                //        else
                //            throw;
                //    }
                //    break;

                default:
                    resultContent = ConvertByExternalConverter(contentInfo, tx);
                    break;
            }
            if (resultContent.IsNullOrEmpty())
                throw new InvalidOperationException("CompliantPrintingHelper.ConvertToPdf: conversione fallita.");

            return iTextSharpHelper.ApplyTransformer(resultContent, tx.PdfTransformer);
        }
        public static PdfContentInfo ConvertToPdf(IContent contentInfo)
        {
            return ConvertToPdf(contentInfo, CompliantTransformerFactory.Default());
        }

        private static PdfContentInfo ConvertPdfToPdf(IPdfContent pdf, CompliantTransformer tx)
        {
            if (tx.RenderSignatures)
            {
                try
                {
                    pdf = RenderSignatures(pdf);
                }
                catch (Exception ex)
                {
                    logger.WarnFormat("E' avvenuto un errore durante la rappresentazione del documento firmato. Verrà restituito il documento originale.", ex);
                }//se fallisce il render delle firme prosegue 
            }

            PdfContentInfo result = ConvertPortfolioToPdf(pdf);
            return result;
        }

        private static PdfContentInfo RenderSignatures(IPdfContent pdf)
        {
            logger.Debug("RenderSignatures pdf");
            if (pdf.HasSignatures)
            {
                logger.Debug("RenderSignatures pdf enter");
                var mergeable = new List<PdfContentInfo>();

                PdfContentInfo result = pdf.ToPdfContent();
                bool enableErrorSection = !pdf.TryToFlattenedPdf(out result);
                logger.Debug(string.Format("RenderSignatures pdf enableErrorSection: {0}", enableErrorSection));

                mergeable.Add(result);
                mergeable.Add(pdf.PrintSignContentReport(enableErrorSection));
                return iTextSharpHelper.Merge(mergeable);
            }

            return pdf.ToPdfContent();
        }

    }
}
