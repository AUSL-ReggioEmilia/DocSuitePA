using System;
using System.Runtime.InteropServices;
using System.IO;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;

namespace VecompSoftware.StampaConforme.Converter.Common
{
    public class PdfToThumbnailConverter : IConverter
    {
        [DllImport("gsdll32.dll")]
        private static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

        [DllImport("gsdll32.dll")]
        private static extern int gsapi_exit(IntPtr instance);

        [DllImport("gsdll32.dll")]
        private static extern void gsapi_delete_instance(IntPtr instance);

        [DllImport("gsdll32.dll")]
        private static extern int gsapi_init_with_args(IntPtr instance, int argc, string[] argv);

        public byte[] Convert(byte[] fileSource, string fileExtension, string extReq, AttachConversionMode mode)
        {
            byte[] retval = new byte[0];

            try
            {
                Exception toThrow = null;
                var fileName = Path.Combine(Path.GetTempPath(), "SC_" + Guid.NewGuid() + ".tmp");

                File.WriteAllBytes(fileName, fileSource);

                try
                {
                    IntPtr instance;

                    if (gsapi_new_instance(out instance, IntPtr.Zero) == 0)
                    {
                        string convertedFile = Path.Combine(Path.GetTempPath(), "SC_" + Guid.NewGuid() + ".tmp");

                        try
                        {
                            string[] parameters = new string[] {
                                "-dSAFER",
                                "-dBATCH",
                                "-dNOPAUSE", 
                                "-sDEVICE=jpeg",
                                "-dFirstPage=1", 
                                "-dLastPage=1", 
                                "-dPDFFitPage", 
                                string.Format("-sOutputFile={0}", convertedFile),
                                fileName,
                            };

                            int retCode = gsapi_init_with_args(instance, parameters.Length, parameters);

                            gsapi_exit(instance);

                            if (retCode == 0)
                            {
                                retval = File.ReadAllBytes(convertedFile);
                            }
                            else
                                throw new Exception("Error in file conversion.");
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            try { gsapi_delete_instance(instance); }
                            catch { }

                            try { File.Delete(convertedFile); }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    toThrow = ex;
                }
                finally
                {
                    try { File.Delete(fileName); }
                    catch { }

                    if (toThrow != null)
                        throw toThrow;
                }
            }
            catch (Exception exx)
            {
                throw exx;
            }

            return retval;
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }
    }
}
