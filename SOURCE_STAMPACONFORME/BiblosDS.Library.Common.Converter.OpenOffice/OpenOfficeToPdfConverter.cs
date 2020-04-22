using System;
using System.Linq;
using System.Runtime.InteropServices;
using unoidl.com.sun.star.frame;
using unoidl.com.sun.star.beans;
using unoidl.com.sun.star.lang;
using unoidl.com.sun.star.text;
using unoidl.com.sun.star.awt;
using System.Configuration;
using System.IO;
using unoidl.com.sun.star.sheet;
using unoidl.com.sun.star.style;
using unoidl.com.sun.star.container;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.Commons.BiblosDS.Objects.Exceptions;
using System.Diagnostics;

namespace BiblosDS.Library.Common.Converter.OpenOffice
{

    public class OpenOfficeToPdfConverter : IConverter
    {
        private const int _retry_tentative = 5;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(OpenOfficeToPdfConverter));

        static unoidl.com.sun.star.uno.XComponentContext localContext;
        public byte[] Convert(byte[] fileSource, string fileExtension, string extReq, AttachConversionMode mode)
        {
            logger.DebugFormat("Convert {0} to {1}", fileExtension, extReq);
            string fileName = Path.GetTempFileName();
            try
            {
                return RetryingPolicyAction<byte[]>(() => {
                    File.WriteAllBytes(fileName + fileExtension, fileSource);
                    if (localContext == null)
                    {
                        localContext = uno.util.Bootstrap.bootstrap();
                    }

                    unoidl.com.sun.star.lang.XMultiServiceFactory multiServiceFactory = (unoidl.com.sun.star.lang.XMultiServiceFactory)localContext.getServiceManager();
                    XComponentLoader componentLoader = (XComponentLoader)multiServiceFactory.createInstance("com.sun.star.frame.Desktop");

                    PropertyValue[] propertyValue = new PropertyValue[2];
                    PropertyValue aProperty = new PropertyValue();
                    aProperty.Name = "Hidden";
                    aProperty.Value = new uno.Any(true);
                    propertyValue[0] = aProperty;

                    propertyValue[1] = new PropertyValue();
                    propertyValue[1].Name = "IsSkipEmptyPages";
                    propertyValue[1].Value = new uno.Any(true);

                    XComponent xComponent = null;
                    bool toConvert = false;
                    string fileConvertedName;
                    try
                    {
                        string fileToConvertName = PathConverter(fileName + fileExtension);
                        fileConvertedName = PathConverter(fileName + "." + extReq);
                        logger.DebugFormat("Url to comvert: {0} - {1}", fileToConvertName, fileConvertedName);
                        xComponent = componentLoader.loadComponentFromURL(
                                                                               fileToConvertName,
                                                                                "_blank",
                                                                                0,
                                                                                propertyValue
                                                                                );
                        if (xComponent == null)
                            throw new DocumentNotConvertible_Exception();

                        logger.Debug("Verify if is to process...");
                        if (fileExtension.Contains("doc") && (UseRedirectMethod(xComponent)))
                        {
                            logger.Debug("No conversion to do...");
                            toConvert = true;
                        }
                        else
                        {
                            object xls = null;

                            if (xComponent is XSpreadsheetDocument)
                                xls = xComponent as XSpreadsheetDocument;
                            else if (xComponent is XTextDocument)
                                xls = xComponent as XTextDocument;

                            if (xls != null)
                            {
                                //Se i nomi degli sheets sono fra quelli non convertibili, usare il converter Office.
                                if (xls is XSpreadsheetDocument)
                                {
                                    var sheetNames = (xls as XSpreadsheetDocument)
                                        .getSheets()
                                        .getElementNames()
                                        .ToArray();

                                    for (int i = 0; i < sheetNames.Length; i++)
                                    {
                                        sheetNames[i] = (sheetNames[i] ?? string.Empty).ToUpper();
                                    }

                                    var namesSet = (ConfigurationManager.AppSettings["RedirectSheetNames"] ?? string.Empty)
                                        .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                        .OrderByDescending(x => x.Length);

                                    var founded = false;

                                    foreach (var set in namesSet)
                                    {
                                        var names = set
                                            .Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                                        if (names.Length <= sheetNames.Length)
                                        {
                                            founded = true;
                                            foreach (var n in names)
                                            {
                                                if (!sheetNames.Contains(n.ToUpper()))
                                                {
                                                    founded = false;
                                                    break;
                                                }
                                            }

                                            if (founded)
                                            {
                                                logger.Debug("No conversion to do...");
                                                return null;
                                            }
                                        }
                                    }
                                }

                                //var xls = xComponent as XSpreadsheetDocument;
                                var styles = (xls as XStyleFamiliesSupplier).getStyleFamilies();
                                var pageStyles = styles.getByName("PageStyles").Value as XNameContainer;

                                bool scaleToPages = false;

                                var configured = ConfigurationManager.AppSettings["ScaleToPages"];
                                try
                                {
                                    if (!string.IsNullOrEmpty(configured))
                                    {
                                        var dummy = bool.Parse(configured);
                                        scaleToPages = dummy;
                                    }
                                }
                                catch { logger.ErrorFormat("Invalid value for configured parameter \"{0}\". Current value is \"{1}\".", "ScaleToPages", configured); }

                                var configValue = ConfigurationManager.AppSettings["StampaConforme.ForcePortrait"];
                                var forcePortrait = !string.IsNullOrEmpty(configValue) && configValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);
                                logger.Info("StampaConforme.ForcePortrait: " + forcePortrait.ToString());

                                foreach (var nome in pageStyles.getElementNames())
                                {
                                    var props = pageStyles.getByName(nome).Value as XPropertySet;
                                    if (forcePortrait)
                                    {
                                        var isLandscape = (bool)props.getPropertyValue("IsLandscape").Value;
                                        if (isLandscape)
                                        {
                                            var size = props.getPropertyValue("Size").Value as Size;
                                            var w = size.Width;
                                            size.Width = size.Height;
                                            size.Height = w;
                                            props.setPropertyValue("Size", new uno.Any(typeof(Size), size));
                                            props.setPropertyValue("IsLandscape", new uno.Any(false));
                                        }
                                    }

                                    if (scaleToPages && props.getPropertySetInfo().hasPropertyByName("ScaleToPages"))
                                        props.setPropertyValue("ScaleToPages", new uno.Any((short)1));
                                }
                            }

                            PropertyValue[] saveProps = new PropertyValue[2];
                            saveProps[0] = new PropertyValue();
                            saveProps[0].Name = "FilterName";
                            saveProps[0].Value = new uno.Any("writer_pdf_Export");

                            saveProps[1] = new PropertyValue();
                            saveProps[1].Name = "IsSkipEmptyPages";
                            saveProps[1].Value = new uno.Any(true);
                            logger.Debug("loadComponentFromURL end.");

                            ((XStorable)xComponent).storeToURL(fileConvertedName, saveProps);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        throw;
                    }
                    finally
                    {
                        if (xComponent != null)
                            xComponent.dispose();
                    }
                    logger.DebugFormat("Conversione END - ToConvert: {0}", toConvert);
                    if (toConvert)
                        return null;
                    else
                        return File.ReadAllBytes(fileName + "." + extReq);
                });
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
                    File.Delete(fileName + fileExtension);
                    File.Delete(fileName + extReq);
                    File.Delete(fileName);
                }
                catch { }
            }
        }

        private bool UseRedirectMethod(XComponent xComponent)
        {
            var textGraphicObjectsSupplier = (XTextGraphicObjectsSupplier)xComponent;
            var names = textGraphicObjectsSupplier.getGraphicObjects().getElementNames();
            foreach (string i in names)
            {
                if (VerifyRedirectParameter(ref textGraphicObjectsSupplier, i))
                    return true;
            }
            return false;
        }

        private bool VerifyRedirectParameter(ref XTextGraphicObjectsSupplier comp, string imageName)
        {
            logger.Debug("VerifyRedirectParameter");
            // Get old image 
            Object xImageObject = comp.getGraphicObjects().getByName(imageName).Value;
            XTextContent xImage = (XTextContent)xImageObject;

            // Get property set object of the existing image (XContent) 
            //  We will use this property set object to set the new Graphic property of the old image 
            // to be the new image stream 
            XPropertySet xPropSet = (XPropertySet)xImage;
            // Get XMCF to create a graphic provider component 
            XMultiComponentFactory xMCF = localContext.getServiceManager();

            // Assign the new graphic image to existent page object 
            //xPropSet.setPropertyValue("Graphic", new uno.Any(typeof(UNOIDL.graphic.XGraphic), xNewGraphic));
            //var crop = (UNOIDL.container.XIndexContainer) xPropSet.getPropertyValue("ImageMap").Value;
            var size = (Size)xPropSet.getPropertyValue("ActualSize").Value;
            var url = xPropSet.getPropertyValue("GraphicURL").Value as string;
            bool elementFind = false;
            if (ConfigurationManager.AppSettings["RedirectSizeFilter"] != null && size != null)
            {
                var sizeFilter = ConfigurationManager.AppSettings["RedirectSizeFilter"].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var arrOfSizeFilter = sizeFilter.Select(p => new { Width = int.Parse(p.Split(',')[0]), Height = int.Parse(p.Split(',')[1]) });
                if (arrOfSizeFilter.Count() > 0)
                    elementFind = arrOfSizeFilter.Any(x => x.Width == size.Width && x.Height == size.Height);
            }
            if (ConfigurationManager.AppSettings["RedirectUrlFilter"] != null && size != null)
            {
                var urlFilter = ConfigurationManager.AppSettings["RedirectUrlFilter"].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (urlFilter.Count() > 0)
                    elementFind = urlFilter.Contains(url);
            }
            return elementFind;
        }


        string PathConverter(string file)
        {
            try
            {
                file = file.Replace(@"\", "/");
                logger.DebugFormat("PathConverter: {0}", file);
                return "file:///" + file;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }

        private T RetryingPolicyAction<T>(Func<T> func, int step = 1)
        {
            logger.Debug($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress...");
            if (step >= _retry_tentative)
            {
                logger.Error("BiblosDS.Library.Common.Converter.OpenOffice.OpenOfficeToPdfConverter.RetryingPolicyAction: retry policy expired maximum tentatives");
                throw new Exception("OpenOfficeToPdfConverter retry policy expired maximum tentatives");
            }
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                if (ex is unoidl.com.sun.star.uno.RuntimeException || ex is unoidl.com.sun.star.io.IOException)
                {
                    logger.Warn($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} failed. Killing OpenOffice process before retry.", ex);
                    localContext = null;
                    KillOpenOfficeProcesses();
                    return RetryingPolicyAction(func, ++step);
                }
                logger.Error($"RetryingPolicyAction: exception type not manageable.", ex);
                throw ex;
            }
        }

        private void KillOpenOfficeProcesses()
        {
            Process[] openOfficeStartupProcesses = Process.GetProcessesByName("soffice.bin");
            Process[] openOfficeExecutableProcesses = Process.GetProcessesByName("soffice.exe");
            if ((openOfficeStartupProcesses == null || openOfficeStartupProcesses.Length == 0) && (openOfficeExecutableProcesses == null || openOfficeExecutableProcesses.Length == 0))
            {
                return;
            }

            Process[] openOfficeProcesses = openOfficeExecutableProcesses.Concat(openOfficeStartupProcesses).ToArray();
            foreach (Process openOfficeProcess in openOfficeProcesses)
            {
                try
                {
                    if (!openOfficeProcess.HasExited)
                    {
                        openOfficeProcess.Kill();
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn($"KillOpenOfficeProcesses: OpenOffice process with id {openOfficeProcess.Id} not killed", ex);
                }
            }
        }
    }
}
