/*
 * DOCUMENTAZIONE UTILIZZATA PER REDARRE IL FILE
 * 
 * http://www.garykessler.net/library/file_sigs.html
 * http://en.wikipedia.org/wiki/List_of_file_signatures
 * http://www.filesignatures.net/index.php?page=all&order=SIGNATURE&sort=DESC&alpha=D
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

namespace VecompSoftware.MagicNumber
{
    public class ExtensionFileByMagicNumbers
    {
        private static byte[] _fileArray {get; set;}
        private static string _filePath { get; set;}

        // doc 2.0
        private static readonly byte[] DocTwo = { 0xDB, 0xA5, 0x2D, 0x00 };
        private static readonly byte[] CompoundBinaryFile = { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
        private static readonly byte[] DocSubHeaderUno = { 0xEC, 0xA5, 0xC1, 0x00 };
        private static readonly byte[] XlsSubHeaderUno = { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 };
        private static readonly byte[] XlsSubHeaderDue = { 0xFD, 0xFF, 0xFF, 0xFF, 0x29 };
        private static readonly byte[] PptSubHeaderUno = { 0xA0, 0x46, 0x1D, 0xF0 };
        private static readonly byte[] BMP = { 66, 77 };
        private static readonly byte[] GIF = { 71, 73, 70, 56 };
        private static readonly byte[] ICO = { 0, 0, 1, 0 };
        private static readonly byte[] JPG = { 255, 216, 255 };
        private static readonly byte[] MP3 = { 255, 251, 48 };
        private static readonly byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
        private static readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
        private static readonly byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
        private static readonly byte[] TIFF = { 73, 73, 42, 0 };
        private static readonly byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
        private static readonly byte[] TTF = { 0, 1, 0, 0, 0 };
        private static readonly byte[] WAV_AVI = { 82, 73, 70, 70 };
        private static readonly byte[] WMV_WMA = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
        private static readonly byte[] GZ = { 0x1F, 0x8B, 0x08};
        private static readonly byte[] EML = { 0x46, 0x72, 0x6D, 0x3A, 0x20 };
        private static readonly byte[] SEVENZIP = { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C };
        
        public static ExtensionFile GetExtension(string filePath)
        {
            _filePath = filePath;
            GetFileToByteArray();
            return GetExtension(GetFileToByteArray());
        }

        public static ExtensionFile GetExtension(byte[] fileArray)
        {
            _fileArray = fileArray;
            try
            {

                if (_fileArray.Take(4).SequenceEqual(DocTwo))
                    return ExtensionFile.doc;

                #region Compound Binary File
                else if (_fileArray.Take(8).SequenceEqual(CompoundBinaryFile))
                    {
                    // Intercetto il SubHeader per verificare la natura del file.
                        if (_fileArray.Skip(512).Take(4).SequenceEqual(DocSubHeaderUno))
                            return ExtensionFile.doc;

                        else if (_fileArray.Skip(512).Take(8).SequenceEqual(XlsSubHeaderUno) || _fileArray.Skip(512).Take(5).SequenceEqual(XlsSubHeaderDue))
                            return ExtensionFile.xls;

                        else if (_fileArray.Skip(512).Take(4).SequenceEqual(PptSubHeaderUno))
                            return ExtensionFile.ppt;
                    }
                #endregion

                else if (_fileArray.Take(3).SequenceEqual(GZ))
                    return ExtensionFile.gz;

                else if (_fileArray.Take(7).SequenceEqual(PDF))
                    return ExtensionFile.pdf;

                else if (_fileArray.Take(5).SequenceEqual(PDF))
                    return ExtensionFile.eml;

                else if (_fileArray.Take(6).SequenceEqual(SEVENZIP))
                    return ExtensionFile.SevenZ;
                    
                else
                {
                    // Microsoft Office
                    try
                    {
                        if (WordprocessingDocument.Open(new MemoryStream(_fileArray), false) != null)
                            return ExtensionFile.docx;
                    }
                    catch (Exception){}
                    try
                    {
                        if (SpreadsheetDocument.Open(new MemoryStream(_fileArray), false) != null)
                            return ExtensionFile.xlsx;
                    }
                    catch (Exception){}
                    try
                    {
                        if (PresentationDocument.Open(new MemoryStream(_fileArray), false) != null)
                            return ExtensionFile.pptx;
                    }
                    catch (Exception){}

                    // Open Office
                    try
                    {}
                    catch (Exception) { }
                }
                return ExtensionFile.none;
            }
            catch (Exception ex)
            {
                string exStr = "Errore nell'individuazione della estensione del file";
                if(ex.InnerException != null)
                    exStr = string.Format("{0}{1}{2}{3}", exStr, ex.Message, Environment.NewLine, ex.InnerException);
                else
                    exStr = string.Format("{0}{1}", ex.Message, exStr);

                throw new Exception(exStr);
            }     
        }

        private static byte[] GetFileToByteArray()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Stream fs = File.OpenRead(_filePath);
                fs.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
