class FileHelper {

    public static PPSX: string = "ppsx";
    public static RAR: string = "rar";
    public static TIF: string = "tif";
    public static TIFF: string = "tiff";
    public static TSD: string = "tsd";
    public static LOG: string = "log";
    public static TXT: string = "txt";
    public static XLS: string = "xls";
    public static XML: string = "xml";
    public static ZIP: string = "zip";
    public static P7M: string = "p7m";
    public static P7S: string = "p7s";
    public static RTF: string = "rtf";
    public static PNG: string = "png";
    public static BUSTA: string = "busta.eml";
    public static CORPO_DEL_MESSAGGIO: string = "corpo_del_messaggio.eml";
    public static BUSTA_PDF: string = "CC_busta.eml.pdf";
    public static PPS: string = "pps";
    public static PPTX: string = "pptx";
    public static XLSX: string = "xlsx";
    public static PDF: string = "pdf";
    public static ACE: string = "ace";
    public static DAT: string = "dat";
    public static DOC: string = "doc";
    public static DOCX: string = "docx";
    public static ODT: string = "odt";
    public static EML: string = "eml";
    public static GIF: string = "gif";
    public static SevenZip: string = "7z";
    public static PPT: string = "ppt";
    public static HTML: string = "html";
    public static JPEG: string = "jpeg";
    public static JPG: string = "jpg";
    public static M7M: string = "m7m";
    public static MSG: string = "msg";
    public static MHT: string = "mht";
    public static P7X: string = "p7x";
    public static HTM: string = "htm";


    private static Signed: string = "card_chip_gold";

    public static getImageByFileName(filename: string, smallImage: boolean): string {
        let image: string = "document_empty";

        let extensions: string[] = filename.split(".");

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
        let size = "16";
        if (!smallImage) {
            size="32"
        }
        return "../App_Themes/DocSuite2008/imgset".concat(size).concat("/").concat(image).concat(".png");
    }

}
export = FileHelper