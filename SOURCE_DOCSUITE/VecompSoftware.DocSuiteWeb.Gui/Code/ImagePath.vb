Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

''' <summary> Classe per i percorsi comuni all'interno della docsuite. </summary>
Public Class ImagePath

#Region " Fields "

    Private Const Add As String = "Add"
    Private Const Accept As String = "accept"
    Private Const Cross As String = "cross"
    Private Const Xml As String = "tag"
    Private Const Folder As String = "folder"
    Private Const Pdf As String = "file_extension_pdf"
    Private Const Signed As String = "card_chip_gold"
    Private Const PageError As String = "page_error"
    Private Const Excel As String = "file_extension_xls"
    Private Const Log As String = "file_extension_log"
    Private Const EventLog As String = "logs"
    Private Const DocumentSeries As String = "document_copies"
    Private Const EmptyDocument As String = "document_empty"
    Private Const Reject As String = "copyleft"
    Private Const Role As String = "bricks"
    Private Const SubRole As String = "brick"
    Private Const Email As String = "mail_blue"
    Private Const EmailDocuments As String = "documents_email"
    Private Const EmailDocumentsAttach As String = "documents_email_attach"
    Private Const EmailReply As String = "email_reply"
    Private Const EmailForward As String = "email_forward"
    Private Const EmailOpen As String = "email_open"
    Private Const Link As String = "link"
    Private Const FolderDocument As String = "folder_document"
    Private Const LinkedFolder As String = "linked_folder"
    Private Const MedalGold As String = "medal_award_gold"
    Private Const MedalSilver As String = "medal_award_silver"
    Private Const Document As String = "document"
    Private Const DocumentSignature As String = "document_signature"
    Private Const DocumentSignaturePink As String = "document_signature_pink"
    Private Const DocumentSignatureAzure As String = "document_signature_azure"
    Private Const DocumentSignatureBlue As String = "document_signature_blue"
    Private Const DocumentSignatureYellow As String = "document_signature_yellow"
    Private Const DocumentSignatureGreen As String = "document_signature_green"
    Private Const DocumentSignatureViolet As String = "document_signature_violet"
    Private Const DocumentCopies As String = "document_copies"
    Private Const Clone As String = "clone"
    Private Const Tag As String = "tag"
    Private Const CodeGray As String = "code_gray"
    Private Const CodeGreen As String = "code_green"
    Private Const MoveToFolder As String = "move_to_folder"
    Private Const Desktop As String = "desktop"
    Private Const Help As String = "help"
    Private Const Info As String = "information"
    Private Const [Error] As String = "error"
    Private Const Expand As String = "bullet_arrow_down"
    Private Const Shrink As String = "bullet_arrow_up"
    Private Const World As String = "world"
    Private Const WorldKey As String = "world_key"
    Private Const NetworkShare As String = "network-share"
    Private Const NetworkShareAdd As String = "network-shar-add"
    Private Const NetworkShareRemove As String = "network-share-remove"
    Private Const NetworkShareStar As String = "network-share-star"
    Private Const BoxOpen As String = "box_open"
    Private Const MailBox As String = "mail_box"
    Private Const Printer As String = "printer"
    Private Const Recycle As String = "recycle"
    Private Const MailIsCc As String = "mail_send"
    Private Const Legend As String = "legend"

    Private Const FlagBlue As String = "flag_blue"
    Private Const FlagGreen As String = "flag_green"
    Private Const FlagOrange As String = "flag_orange"
    Private Const FlagPink As String = "flag_pink"
    Private Const FlagPurple As String = "flag_purple"
    Private Const FlagRed As String = "flag_red"
    Private Const FlagYellow As String = "flag_yellow"

    Private Const AddressEditor As String = "addressEditor"
    Private Const Edit As String = "pencil"
    Private Const Delete As String = "delete"
    Private Const Cancel As String = "cancel"
    Private Const Remove As String = "remove"

    Private Const SmallSet As String = "16"
    Private Const BigSet As String = "32"

    Private Const ImageSetPath As String = "~/App_Themes/DocSuite2008/imgset{0}/{1}.png"

    Public Const SpacerImage As String = "~/Comm/Images/spacer.gif"

    Public Const SmallEmpty As String = "~/Comm/Images/none16.gif"

    Public Const DraftMessageImagePath As String = "~/Comm/Images/pec-preavviso-errore-consegna.gif"
    Public Const ActiveMessageImagePath As String = "~/Comm/Images/pec-accettazione.gif"
    Public Const SentMessageImagePath As String = "~/Comm/Images/pec-avvenuta-consegna.gif"
    Public Const ErrorMessageImagePath As String = "~/Comm/Images/pec-errore-consegna.gif"

#End Region

#Region " Properties "

    Public Shared ReadOnly Property SmallAdd As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Add)
        End Get
    End Property

    Public Shared ReadOnly Property BigAdd As String
        Get
            Return String.Format(ImageSetPath, BigSet, Add)
        End Get
    End Property

    Public Shared ReadOnly Property SmallAccept As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Accept)
        End Get
    End Property

    Public Shared ReadOnly Property BigAccept As String
        Get
            Return String.Format(ImageSetPath, BigSet, Accept)
        End Get
    End Property

    Public Shared ReadOnly Property SmallCross As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Cross)
        End Get
    End Property

    Public Shared ReadOnly Property BigCross As String
        Get
            Return String.Format(ImageSetPath, BigSet, Cross)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFolder As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Folder)
        End Get
    End Property

    Public Shared ReadOnly Property SmallPdf As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Pdf)
        End Get
    End Property

    Public Shared ReadOnly Property SmallSigned As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Signed)
        End Get
    End Property

    Public Shared ReadOnly Property BigSigned As String
        Get
            Return String.Format(ImageSetPath, BigSet, Signed)
        End Get
    End Property

    Public Shared ReadOnly Property SmallPageError As String
        Get
            Return String.Format(ImageSetPath, SmallSet, PageError)
        End Get
    End Property

    Public Shared ReadOnly Property BigPageError As String
        Get
            Return String.Format(ImageSetPath, BigSet, PageError)
        End Get
    End Property

    Public Shared ReadOnly Property SmallExcel As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Excel)
        End Get
    End Property

    Public Shared ReadOnly Property SmallLog As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EventLog)
        End Get
    End Property

    Public Shared ReadOnly Property BigLog As String
        Get
            Return String.Format(ImageSetPath, BigSet, Log)
        End Get
    End Property

    Public Shared ReadOnly Property SmallXml As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Xml)
        End Get
    End Property

    Public Shared ReadOnly Property SmallDocumentSeries As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSeries)
        End Get
    End Property

    Public Shared ReadOnly Property BigDocumentSeries As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSeries)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEmptyDocument As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EmptyDocument)
        End Get
    End Property

    Public Shared ReadOnly Property BigEmptyDocument As String
        Get
            Return String.Format(ImageSetPath, BigSet, EmptyDocument)
        End Get
    End Property

    Public Shared ReadOnly Property SmallReject As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Reject)
        End Get
    End Property

    Public Shared ReadOnly Property BigReject As String
        Get
            Return String.Format(ImageSetPath, BigSet, Reject)
        End Get
    End Property

    Public Shared ReadOnly Property SmallRole As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Role)
        End Get
    End Property

    Public Shared ReadOnly Property SmallSubRole As String
        Get
            Return String.Format(ImageSetPath, SmallSet, SubRole)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEmail() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Email)
        End Get
    End Property

    Public Shared ReadOnly Property BigEmail() As String
        Get
            Return String.Format(ImageSetPath, BigSet, Email)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEmailDocuments() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EmailDocuments)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEmailDocumentsAttach() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EmailDocumentsAttach)
        End Get
    End Property

    Public Shared ReadOnly Property BigEmailDocumentsAttach() As String
        Get
            Return String.Format(ImageSetPath, BigSet, EmailDocumentsAttach)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEmailReply() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EmailReply)
        End Get
    End Property

    Public Shared ReadOnly Property BigEmailReply() As String
        Get
            Return String.Format(ImageSetPath, BigSet, EmailReply)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEmailForward() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EmailForward)
        End Get
    End Property

    Public Shared ReadOnly Property BigEmailForward() As String
        Get
            Return String.Format(ImageSetPath, BigSet, EmailForward)
        End Get
    End Property

    Public Shared ReadOnly Property SmallMedalGold() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, MedalGold)
        End Get
    End Property

    Public Shared ReadOnly Property SmallMedalSilver() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, MedalSilver)
        End Get
    End Property

    Public Shared ReadOnly Property SmallDocumentSignature() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignature)
        End Get
    End Property
    Public Shared ReadOnly Property SmallDocumentSignatureAzure() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignatureAzure)
        End Get
    End Property
    Public Shared ReadOnly Property SmallDocumentSignaturePink() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignaturePink)
        End Get
    End Property

    Public Shared ReadOnly Property SmallDocumentSignatureBlue() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignatureBlue)
        End Get
    End Property
    Public Shared ReadOnly Property SmallDocumentSignatureYellow() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignatureYellow)
        End Get
    End Property
    Public Shared ReadOnly Property SmallDocumentSignatureGreen() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignatureGreen)
        End Get
    End Property
    Public Shared ReadOnly Property SmallDocumentSignatureViolet() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentSignatureViolet)
        End Get
    End Property
    Public Shared ReadOnly Property SmallDocumentCopies() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, DocumentCopies)
        End Get
    End Property

    Public Shared ReadOnly Property SmallClone() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Clone)
        End Get
    End Property

    Public Shared ReadOnly Property BigDocumentSignature() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignature)
        End Get
    End Property

    Public Shared ReadOnly Property BigDocumentSignatureAzure() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignatureAzure)
        End Get
    End Property

    Public Shared ReadOnly Property BigDocumentSignaturePink() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignaturePink)
        End Get
    End Property

    Public Shared ReadOnly Property BigDocumentSignatureBlue() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignatureBlue)
        End Get
    End Property
    Public Shared ReadOnly Property BigDocumentSignatureYellow() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignatureYellow)
        End Get
    End Property
    Public Shared ReadOnly Property BigDocumentSignatureGreen() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignatureGreen)
        End Get
    End Property

    Public Shared ReadOnly Property BigDocumentSignatureViolet() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentSignatureViolet)
        End Get
    End Property
    Public Shared ReadOnly Property BigDocumentCopies() As String
        Get
            Return String.Format(ImageSetPath, BigSet, DocumentCopies)
        End Get
    End Property
    Public Shared ReadOnly Property SmallTag() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Tag)
        End Get
    End Property
    Public Shared ReadOnly Property SmallCodeGray() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, CodeGray)
        End Get
    End Property
    Public Shared ReadOnly Property SmallCodeGreen() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, CodeGreen)
        End Get
    End Property

    Public Shared ReadOnly Property BigCodeGray() As String
        Get
            Return String.Format(ImageSetPath, BigSet, CodeGray)
        End Get
    End Property
    Public Shared ReadOnly Property BigCodeGreen() As String
        Get
            Return String.Format(ImageSetPath, BigSet, CodeGreen)
        End Get
    End Property
    Public Shared ReadOnly Property SmallMoveToFolder() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, MoveToFolder)
        End Get
    End Property
    Public Shared ReadOnly Property SmallEmailOpen() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, EmailOpen)
        End Get
    End Property

    Public Shared ReadOnly Property SmallLink() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Link)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFolderDocument() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FolderDocument)
        End Get
    End Property

    Public Shared ReadOnly Property SmallLinkedFolder As String
        Get
            Return String.Format(ImageSetPath, SmallSet, LinkedFolder)
        End Get
    End Property

    Public Shared ReadOnly Property SmallInfo() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Info)
        End Get
    End Property

    Public Shared ReadOnly Property BigInfo() As String
        Get
            Return String.Format(ImageSetPath, BigSet, Info)
        End Get
    End Property

    Public Shared ReadOnly Property SmallError() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, [Error])
        End Get
    End Property

    Public Shared ReadOnly Property BigError() As String
        Get
            Return String.Format(ImageSetPath, BigSet, [Error])
        End Get
    End Property

    Public Shared ReadOnly Property SmallDesktop As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Desktop)
        End Get
    End Property
    Public Shared ReadOnly Property BigDesktop As String
        Get
            Return String.Format(ImageSetPath, BigSet, Desktop)
        End Get
    End Property

    Public Shared ReadOnly Property SmallHelp As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Help)
        End Get
    End Property
    Public Shared ReadOnly Property BigHelp As String
        Get
            Return String.Format(ImageSetPath, BigSet, Help)
        End Get
    End Property

    Public Shared ReadOnly Property SmallExpand As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Expand)
        End Get
    End Property
    Public Shared ReadOnly Property BigExpand As String
        Get
            Return String.Format(ImageSetPath, BigSet, Expand)
        End Get
    End Property

    Public Shared ReadOnly Property SmallShrink As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Shrink)
        End Get
    End Property
    Public Shared ReadOnly Property BigShrink As String
        Get
            Return String.Format(ImageSetPath, BigSet, Shrink)
        End Get
    End Property

    Public Shared ReadOnly Property SmallWorld As String
        Get
            Return String.Format(ImageSetPath, SmallSet, World)
        End Get
    End Property
    Public Shared ReadOnly Property BigWorld As String
        Get
            Return String.Format(ImageSetPath, BigSet, World)
        End Get
    End Property

    Public Shared ReadOnly Property SmallWorldKey As String
        Get
            Return String.Format(ImageSetPath, SmallSet, WorldKey)
        End Get
    End Property
    Public Shared ReadOnly Property BigWorldKey As String
        Get
            Return String.Format(ImageSetPath, BigSet, WorldKey)
        End Get
    End Property

    Public Shared ReadOnly Property SmallNetworkShare As String
        Get
            Return String.Format(ImageSetPath, SmallSet, NetworkShare)
        End Get
    End Property
    Public Shared ReadOnly Property BigNetworkShare As String
        Get
            Return String.Format(ImageSetPath, BigSet, NetworkShare)
        End Get
    End Property

    Public Shared ReadOnly Property SmallNetworkShareAdd As String
        Get
            Return String.Format(ImageSetPath, SmallSet, NetworkShareAdd)
        End Get
    End Property
    Public Shared ReadOnly Property BigNetworkShareAdd As String
        Get
            Return String.Format(ImageSetPath, BigSet, NetworkShareAdd)
        End Get
    End Property

    Public Shared ReadOnly Property SmallNetworkShareRemove As String
        Get
            Return String.Format(ImageSetPath, SmallSet, NetworkShareRemove)
        End Get
    End Property

    Public Shared ReadOnly Property BigNetworkShareRemove As String
        Get
            Return String.Format(ImageSetPath, BigSet, NetworkShareRemove)
        End Get
    End Property

    Public Shared ReadOnly Property SmallNetworkShareStar As String
        Get
            Return String.Format(ImageSetPath, SmallSet, NetworkShareStar)
        End Get
    End Property

    Public Shared ReadOnly Property BigNetworkShareStar As String
        Get
            Return String.Format(ImageSetPath, BigSet, NetworkShareStar)
        End Get
    End Property

    Public Shared ReadOnly Property SmallBoxOpen As String
        Get
            Return String.Format(ImageSetPath, SmallSet, BoxOpen)
        End Get
    End Property
    Public Shared ReadOnly Property BigBoxOpen As String
        Get
            Return String.Format(ImageSetPath, BigSet, BoxOpen)
        End Get
    End Property

    Public Shared ReadOnly Property SmallMailBox As String
        Get
            Return String.Format(ImageSetPath, SmallSet, MailBox)
        End Get
    End Property

    Public Shared ReadOnly Property BigMailBox As String
        Get
            Return String.Format(ImageSetPath, BigSet, MailBox)
        End Get
    End Property

    Public Shared ReadOnly Property SmallMailIsCc As String
        Get
            Return String.Format(ImageSetPath, SmallSet, MailIsCc)
        End Get
    End Property

    Public Shared ReadOnly Property BigMailIsCc As String
        Get
            Return String.Format(ImageSetPath, BigSet, MailIsCc)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagBlue As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagBlue)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagBlue As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagBlue)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagGreen As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagGreen)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagGreen As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagGreen)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagOrange As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagOrange)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagOrange As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagOrange)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagPink As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagPink)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagPink As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagPink)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagPurple As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagPurple)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagPurple As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagPurple)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagRed As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagRed)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagRed As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagRed)
        End Get
    End Property

    Public Shared ReadOnly Property SmallFlagYellow As String
        Get
            Return String.Format(ImageSetPath, SmallSet, FlagYellow)
        End Get
    End Property

    Public Shared ReadOnly Property BigFlagYellow As String
        Get
            Return String.Format(ImageSetPath, BigSet, FlagYellow)
        End Get
    End Property

    Public Shared ReadOnly Property SmallAddressEditor As String
        Get
            Return String.Format(ImageSetPath, SmallSet, AddressEditor)
        End Get
    End Property

    Public Shared ReadOnly Property SmallEdit As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Edit)
        End Get
    End Property

    Public Shared ReadOnly Property BigEdit As String
        Get
            Return String.Format(ImageSetPath, BigSet, Edit)
        End Get
    End Property

    Public Shared ReadOnly Property SmallDelete As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Delete)
        End Get
    End Property

    Public Shared ReadOnly Property BigDelete As String
        Get
            Return String.Format(ImageSetPath, BigSet, Delete)
        End Get
    End Property

    Public Shared ReadOnly Property SmallCancel As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Cancel)
        End Get
    End Property

    Public Shared ReadOnly Property BigCancel As String
        Get
            Return String.Format(ImageSetPath, BigSet, Cancel)
        End Get
    End Property

    Public Shared ReadOnly Property SmallRemove As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Remove)
        End Get
    End Property

    Public Shared ReadOnly Property BigRemove As String
        Get
            Return String.Format(ImageSetPath, BigSet, Remove)
        End Get
    End Property

    Public Shared ReadOnly Property SmallPrinter As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Printer)
        End Get
    End Property

    Public Shared ReadOnly Property BigPrinter As String
        Get
            Return String.Format(ImageSetPath, BigSet, Printer)
        End Get
    End Property

    Public Shared ReadOnly Property SmallRecycle As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Recycle)
        End Get
    End Property

    Public Shared ReadOnly Property BigRecycle As String
        Get
            Return String.Format(ImageSetPath, BigSet, Recycle)
        End Get
    End Property

    Public Shared ReadOnly Property SmallLegend As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Legend)
        End Get
    End Property

    Public Shared ReadOnly Property BigLegend As String
        Get
            Return String.Format(ImageSetPath, BigSet, Legend)
        End Get
    End Property

    Public Shared ReadOnly Property SmallDocument() As String
        Get
            Return String.Format(ImageSetPath, SmallSet, Document)
        End Get
    End Property

#End Region

#Region " Methods "

    Public Shared Function FromFileNoSignature(ByVal fileName As String, ByVal smallImage As Boolean) As String
        Return FromFile(fileName.ReplaceInsensitive(FileHelper.P7M, ""), smallImage)
    End Function

    ''' <summary> Ritorna il percorso inerente l'immagine del file di 16px. </summary>
    Public Shared Function FromFile(ByVal fileName As String) As String
        Return FromFile(fileName, True)
    End Function

    ''' <summary>
    ''' Ritorna la classe css corretta per l'immagine del file. Verifica anche PDF firmati, se attivo in ParameterEnv.
    ''' </summary>
    ''' <param name="doc">Documento</param>
    Public Shared Function FromDocumentInfo(doc As DocumentInfo) As String
        Return FromDocumentInfo(doc, True)
    End Function

    ''' <summary>
    ''' Ritorna la classe css corretta per l'immagine del file. Verifica anche PDF firmati, se attivo in ParameterEnv.
    ''' </summary>
    ''' <param name="doc">Documento</param>
    ''' <param name="smallImage">Se True torna immagine larga 16px, altrimenti 32px.</param>
    Public Shared Function FromDocumentInfo(doc As DocumentInfo, smallImage As Boolean) As String
        If DocSuiteContext.Current.ProtocolEnv.ChekPDFSigned AndAlso FileHelper.MatchExtension(doc.Name, FileHelper.PDF) AndAlso doc.IsSigned Then
            Return FormatImageName("file_extension_pdf_signed", True)
        End If
        Return FromFile(doc.Name)
    End Function

    ''' <summary> Ritorna la classe css corretta per l'immagine del file. </summary>
    ''' <param name="fileName">Nome del file dal quale estrarre il tipo.</param>
    ''' <param name="smallImage">Se True torna immagine larga 16px, altrimenti 32px.</param>
    Public Shared Function FromFile(ByVal fileName As String, ByVal smallImage As Boolean) As String
        Dim image As String
        Select Case True
            Case FileHelper.MatchExtension(fileName, FileHelper.M7M),
                FileHelper.MatchExtension(fileName, FileHelper.P7X),
                FileHelper.MatchExtension(fileName, FileHelper.P7M),
                FileHelper.MatchExtension(fileName, FileHelper.P7S)
                FileHelper.MatchExtension(fileName, FileHelper.TSD)
                image = Signed
            Case FileHelper.MatchExtension(fileName, FileHelper.DAT)
                image = "file_extension_dat"
            Case FileHelper.MatchExtension(fileName, FileHelper.DOC),
                FileHelper.MatchExtension(fileName, FileHelper.DOCX),
                FileHelper.MatchExtension(fileName, FileHelper.ODT)
                image = "file_extension_doc"
            Case FileHelper.MatchExtension(fileName, FileHelper.RTF)
                image = "file_extension_rtf"
            Case FileHelper.MatchExtension(fileName, FileHelper.TIF),
                FileHelper.MatchExtension(fileName, FileHelper.TIFF)
                image = "file_extension_tif"
            Case FileHelper.MatchExtension(fileName, FileHelper.GIF)
                image = "file_extension_gif"
            Case FileHelper.MatchExtension(fileName, FileHelper.JPG),
                FileHelper.MatchExtension(fileName, FileHelper.JPEG)
                image = "file_extension_jpg"
            Case FileHelper.MatchExtension(fileName, FileHelper.PNG)
                image = "file_extension_png"
            Case FileHelper.MatchExtension(fileName, FileHelper.PDF)
                image = Pdf
            Case FileHelper.MatchExtension(fileName, FileHelper.MSG),
                FileHelper.MatchExtension(fileName, FileHelper.EML)
                image = "file_extension_eml"
            Case FileHelper.MatchExtension(fileName, FileHelper.XLS),
                FileHelper.MatchExtension(fileName, FileHelper.XLSX)
                image = Excel
            Case FileHelper.MatchExtension(fileName, FileHelper.PPT),
                FileHelper.MatchExtension(fileName, FileHelper.PPTX),
                FileHelper.MatchExtension(fileName, FileHelper.PPS),
                FileHelper.MatchExtension(fileName, FileHelper.PPSX)
                image = "file_extension_pps"
            Case FileHelper.MatchExtension(fileName, FileHelper.MHT)
                image = "picture_frame"
            Case FileHelper.MatchExtension(fileName, FileHelper.HTM)
                image = "file_extension_htm"
            Case FileHelper.MatchExtension(fileName, FileHelper.HTML)
                image = "file_extension_html"
            Case FileHelper.MatchExtension(fileName, FileHelper.LOG)
                image = Log
            Case FileHelper.MatchExtension(fileName, FileHelper.TXT)
                image = "file_extension_txt"
            Case FileHelper.MatchExtension(fileName, FileHelper.XML)
                image = Xml
            Case FileHelper.MatchExtension(fileName, FileHelper.SevenZip)
                image = "file_extension_7z"
            Case FileHelper.MatchExtension(fileName, FileHelper.ZIP)
                image = "file_extension_zip"
            Case FileHelper.MatchExtension(fileName, FileHelper.RAR)
                image = "file_extension_rar"
            Case FileHelper.MatchExtension(fileName, FileHelper.ACE)
                image = "file_extension_ace"
            Case Else
                image = EmptyDocument
        End Select

        Return FormatImageName(image, smallImage)

    End Function

    Public Shared Function FormatImageName(image As String, smallImage As Boolean) As String
        Return String.Format(ImageSetPath, If(smallImage, SmallSet, BigSet), image)
    End Function

    ''' <summary> Dato un identificativo di <see cref="ContactType"/> torna l'icona corretta. </summary>
    Public Shared Function ContactTypeIcon(ByVal type As Char) As String
        Return ContactTypeIcon(type, False)
    End Function

    ''' <summary> Dato un identificativo di <see cref="ContactType"/> torna l'icona corretta. </summary>
    Public Shared Function ContactTypeIcon(ByVal type As Char, ByVal isLocked As Boolean) As String
        Dim s As New StringBuilder()
        Select Case type
            Case ContactType.Administration
                s.Append("Amministrazione")
            Case ContactType.Group
                s.Append("Gruppo")
            Case ContactType.Sector
                s.Append("Gruppo16")
            Case ContactType.Aoo
                s.Append("Aoo")
            Case ContactType.OrganizationUnit
                s.Append("Uo")
            Case ContactType.Role
                s.Append("Ruolo")
            Case ContactType.Person
                s.Append("Persona")
            Case ContactType.AdAmPerson
                s.Append("AdAm")
            Case ContactType.Mistery
                s.Append("Manuale")
            Case ContactType.Ipa
                s.Append("building")
        End Select
        ' completo il percorso all'immagine
        If s.Length <> 0 Then
            s.Insert(0, "../comm/images/interop/")
            If isLocked Then
                s.Append("_locked")
            End If
            s.Append(".gif")
        End If
        Return s.ToString()
    End Function

    Public Shared Function GetMessageStatusIconPath(ByVal messageStatus As DSWMessage.MessageStatusEnum) As String
        Select Case messageStatus
            Case DSWMessage.MessageStatusEnum.Draft
                Return DraftMessageImagePath
            Case DSWMessage.MessageStatusEnum.Active
                Return ActiveMessageImagePath
            Case DSWMessage.MessageStatusEnum.Sent
                Return SentMessageImagePath
            Case DSWMessage.MessageStatusEnum.Error
                Return ErrorMessageImagePath
            Case Else
                Return ErrorMessageImagePath
        End Select
    End Function

#End Region

End Class
