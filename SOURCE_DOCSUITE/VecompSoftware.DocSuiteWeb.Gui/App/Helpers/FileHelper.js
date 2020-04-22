define(["require", "exports"], function (require, exports) {
    var FileHelper = /** @class */ (function () {
        function FileHelper() {
        }
        FileHelper.getImageByFileName = function (filename, smallImage) {
            var image = "document_empty";
            var extensions = filename.split(".");
            if (extensions != null && extensions.length > 0) {
                switch (extensions.pop().toLowerCase()) {
                    case FileHelper.M7M:
                    case FileHelper.P7X:
                    case FileHelper.P7M:
                    case FileHelper.P7S:
                    case FileHelper.TSD:
                        image = FileHelper.Signed;
                        break;
                    case FileHelper.DAT:
                        image = "file_extension_dat";
                        break;
                    case FileHelper.DOC:
                    case FileHelper.DOCX:
                    case FileHelper.ODT:
                        image = "file_extension_doc";
                        break;
                    case FileHelper.RTF:
                        image = "file_extension_rtf";
                        break;
                    case FileHelper.TIF:
                    case FileHelper.TIFF:
                        image = "file_extension_tif";
                        break;
                    case FileHelper.GIF:
                        image = "file_extension_gif";
                        break;
                    case FileHelper.JPEG:
                    case FileHelper.JPG:
                        image = "file_extension_jpg";
                        break;
                    case FileHelper.PNG:
                        image = "file_extension_png";
                        break;
                    case FileHelper.PDF:
                        image = "file_extension_pdf";
                        break;
                    case FileHelper.MSG:
                    case FileHelper.EML:
                        image = "file_extension_eml";
                        break;
                    case FileHelper.XLS:
                    case FileHelper.XLSX:
                        image = "file_extension_xls";
                        break;
                    case FileHelper.PPT:
                    case FileHelper.PPTX:
                    case FileHelper.PPS:
                    case FileHelper.PPSX:
                        image = "file_extension_pps";
                        break;
                    case FileHelper.MHT:
                        image = "picture_frame";
                        break;
                    case FileHelper.HTM:
                        image = "file_extension_htm";
                        break;
                    case FileHelper.HTML:
                        image = "file_extension_html";
                        break;
                    case FileHelper.LOG:
                        image = "file_extension_log";
                        break;
                    case FileHelper.TXT:
                        image = "file_extension_txt";
                        break;
                    case FileHelper.XML:
                        image = "tag";
                        break;
                    case FileHelper.SevenZip:
                        image = "file_extension_7z";
                        break;
                    case FileHelper.ZIP:
                        image = "file_extension_zip";
                        break;
                    case FileHelper.RAR:
                        image = "file_extension_rar";
                        break;
                    case FileHelper.ACE:
                        image = "file_extension_ace";
                        break;
                }
            }
            var size = "16";
            if (!smallImage) {
                size = "32";
            }
            return "../App_Themes/DocSuite2008/imgset".concat(size).concat("/").concat(image).concat(".png");
        };
        FileHelper.PPSX = "ppsx";
        FileHelper.RAR = "rar";
        FileHelper.TIF = "tif";
        FileHelper.TIFF = "tiff";
        FileHelper.TSD = "tsd";
        FileHelper.LOG = "log";
        FileHelper.TXT = "txt";
        FileHelper.XLS = "xls";
        FileHelper.XML = "xml";
        FileHelper.ZIP = "zip";
        FileHelper.P7M = "p7m";
        FileHelper.P7S = "p7s";
        FileHelper.RTF = "rtf";
        FileHelper.PNG = "png";
        FileHelper.BUSTA = "busta.eml";
        FileHelper.CORPO_DEL_MESSAGGIO = "corpo_del_messaggio.eml";
        FileHelper.BUSTA_PDF = "CC_busta.eml.pdf";
        FileHelper.PPS = "pps";
        FileHelper.PPTX = "pptx";
        FileHelper.XLSX = "xlsx";
        FileHelper.PDF = "pdf";
        FileHelper.ACE = "ace";
        FileHelper.DAT = "dat";
        FileHelper.DOC = "doc";
        FileHelper.DOCX = "docx";
        FileHelper.ODT = "odt";
        FileHelper.EML = "eml";
        FileHelper.GIF = "gif";
        FileHelper.SevenZip = "7z";
        FileHelper.PPT = "ppt";
        FileHelper.HTML = "html";
        FileHelper.JPEG = "jpeg";
        FileHelper.JPG = "jpg";
        FileHelper.M7M = "m7m";
        FileHelper.MSG = "msg";
        FileHelper.MHT = "mht";
        FileHelper.P7X = "p7x";
        FileHelper.HTM = "htm";
        FileHelper.Signed = "card_chip_gold";
        return FileHelper;
    }());
    return FileHelper;
});
//# sourceMappingURL=FileHelper.js.map